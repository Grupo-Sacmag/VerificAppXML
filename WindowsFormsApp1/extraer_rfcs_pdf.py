import sys
import os
import json
import re

def extraer_transacciones_pdf(pdf_path):
    # Patron fecha de inicio de transaccion: '1 deAgo', '15 de Jul', '31 deJulio', etc.
    patron_inicio_tx = re.compile(r'^(\d{1,2})\s*de\s*([A-Za-z]{3,4})\b', re.IGNORECASE)
    # Patron monto al final de linea: soporta '17.00', '2,719.42', '-500.00', '1,450.00CR', etc.
    patron_monto_fin = re.compile(r'([-\+]?\d{1,3}(?:,\d{3})*\.\d{2})\s*(CR)?\s*$', re.IGNORECASE)
    # Patron RFC fiscal
    patron_rfc = re.compile(r'RFC\s*([A-ZÑ&]{3,4}\d{6}[A-Z0-9]{3})\b', re.IGNORECASE)

    # Intentar usar pdfplumber o PyMuPDF
    lineas_por_pagina = []
    try:
        import pdfplumber
        with pdfplumber.open(pdf_path) as pdf:
            for num_pag, page in enumerate(pdf.pages):
                text = page.extract_text() or ''
                lineas_por_pagina.append((num_pag + 1, text.splitlines()))
    except Exception:
        import fitz
        doc = fitz.open(pdf_path)
        for num_pag, page in enumerate(doc):
            text = page.get_text("text") or ''
            lineas_por_pagina.append((num_pag + 1, text.splitlines()))

    transacciones = []

    for num_pag, lines in lineas_por_pagina:
        tx_actual = None

        for line in lines:
            l_str = line.strip()
            if not l_str:
                continue

            # Omitir encabezados corporativos repetitivos de AMEX
            if any(x in l_str for x in [
                'Corporate Card', 'americanexpress', 'Estado de Cuenta', 'Fecha de corte',
                'Fecha y detalle de las operaciones', 'Detalle de nuevos cargos', 'Página', 'Pagina'
            ]):
                continue

            # Revisar si inicia una nueva transaccion por fecha
            m_fecha = patron_inicio_tx.match(l_str)
            if m_fecha:
                if tx_actual:
                    transacciones.append(tx_actual)
                    tx_actual = None

                m_monto = patron_monto_fin.search(l_str)
                if m_monto:
                    monto_raw = m_monto.group(1).replace(',', '')
                    monto = float(monto_raw)
                    es_cr = bool(m_monto.group(2)) or bool(re.search(r'\bCR\b', l_str, re.IGNORECASE))

                    fin_fecha = m_fecha.end()
                    ini_monto = m_monto.start()
                    desc = l_str[fin_fecha:ini_monto].strip()
                    fecha_txt = f"{m_fecha.group(1)} de {m_fecha.group(2)}"

                    # Determinar si es crédito / abono / devolución / pago
                    es_credito = False
                    if monto < 0 or es_cr:
                        es_credito = True
                        monto = -abs(monto)
                    elif any(w in desc.upper() for w in [
                        'PAGO RECIBIDO', 'CREDITO', 'CRÉDITO', 'DEVOLUCION',
                        'DEVOLUCIÓN', 'ABONO', 'REEMBOLSO', 'BONIFICACION', 'SALDO A FAVOR'
                    ]):
                        es_credito = True
                        monto = -abs(monto)

                    tx_actual = {
                        'pagina': num_pag,
                        'fecha': fecha_txt,
                        'desc': desc,
                        'monto': monto,
                        'tipo': 'CRÉDITO / ABONO' if es_credito else 'CARGO',
                        'rfc': ''
                    }
            else:
                # Línea complementaria de la transacción actual (RFC, número de boleto, etc.)
                if tx_actual:
                    m_rfc = patron_rfc.search(l_str)
                    if m_rfc and not tx_actual['rfc']:
                        tx_actual['rfc'] = m_rfc.group(1).upper()
                    elif '-' in l_str or 'CR' in l_str.upper():
                        if any(w in l_str.upper() for w in ['CREDITO', 'CRÉDITO', 'DEVOLUCION', 'DEVOLUCIÓN', 'REEMBOLSO', 'BONIFICACION']):
                            tx_actual['tipo'] = 'CRÉDITO / ABONO'
                            tx_actual['monto'] = -abs(tx_actual['monto'])

        if tx_actual:
            transacciones.append(tx_actual)

    return transacciones

def exportar_a_excel(txs, ruta_excel):
    import pandas as pd
    filas = []
    for t in txs:
        filas.append({
            'Pagina': t.get('pagina', 1),
            'Fecha': t.get('fecha', ''),
            'Concepto / Establecimiento': t.get('desc', ''),
            'Tipo': t.get('tipo', 'CARGO'),
            'Importe': t.get('monto', 0.0),
            'RFC Proveedor': t.get('rfc', '')
        })

    df = pd.DataFrame(filas)
    with pd.ExcelWriter(ruta_excel, engine='openpyxl') as writer:
        df.to_excel(writer, index=False, sheet_name='Movimientos AMEX')
        ws = writer.sheets['Movimientos AMEX']
        for col in ws.columns:
            max_len = max(len(str(cell.value or '')) for cell in col)
            col_letter = col[0].column_letter
            ws.column_dimensions[col_letter].width = max(max_len + 3, 12)

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Uso: python extraer_rfcs_pdf.py <ruta_pdf> [ruta_salida_tsv_o_json] [ruta_salida_excel]")
        sys.exit(1)

    pdf_path = sys.argv[1]
    if not os.path.exists(pdf_path):
        print(f"Error: Archivo no encontrado: {pdf_path}", file=sys.stderr)
        sys.exit(1)

    try:
        txs = extraer_transacciones_pdf(pdf_path)
        out_file = sys.argv[2] if len(sys.argv) >= 3 else None
        excel_file = sys.argv[3] if len(sys.argv) >= 4 else None

        # Si el segundo argumento es directamente .xlsx
        if out_file and out_file.lower().endswith(".xlsx"):
            exportar_a_excel(txs, out_file)
        elif out_file and out_file.lower().endswith(".json"):
            with open(out_file, "w", encoding="utf-8") as f:
                json.dump(txs, f, ensure_ascii=False, indent=2)
        elif out_file:
            with open(out_file, "w", encoding="utf-8") as f:
                for t in txs:
                    monto_str = f"{t['monto']:.2f}" if t['monto'] is not None else ""
                    tipo_str = t.get('tipo', 'CARGO')
                    f.write(f"{t['rfc']}\t{monto_str}\t{t['fecha']}\t{t['desc']}\t{tipo_str}\n")

        # Si se pidió explícitamente generar un Excel en el 3er parámetro
        if excel_file and excel_file.lower().endswith(".xlsx"):
            exportar_a_excel(txs, excel_file)

        if not out_file:
            if hasattr(sys.stdout, 'reconfigure'):
                sys.stdout.reconfigure(encoding='utf-8')
            for t in txs:
                monto_str = f"{t['monto']:.2f}" if t['monto'] is not None else ""
                tipo_str = t.get('tipo', 'CARGO')
                print(f"{t['rfc']}\t{monto_str}\t{t['fecha']}\t{t['desc']}\t{tipo_str}")

    except Exception as e:
        print(f"Error al procesar PDF: {e}", file=sys.stderr)
        sys.exit(1)
