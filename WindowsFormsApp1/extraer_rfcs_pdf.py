import sys
import os
import json
import re

def extraer_transacciones_pdf(pdf_path):
    import fitz  # PyMuPDF
    doc = fitz.open(pdf_path)
    full_text = ""
    for page in doc:
        full_text += page.get_text("text") + "\n"

    lineas = full_text.splitlines()
    transacciones = []

    for i, line in enumerate(lineas):
        # Buscar RFC en la línea
        m_rfc = re.search(r'RFC([A-Z&Ñ]{3,4}\d{6}[A-Z0-9]{3})', line)
        if m_rfc:
            rfc = m_rfc.group(1).upper()
            desc = ""
            monto = None
            fecha = ""

            # Examinar líneas previas (y posteriores si es necesario) para capturar monto, fecha y descripción
            ventana = [lineas[k].strip() for k in range(max(0, i - 4), i)]
            
            for prev in ventana:
                m_monto = re.search(r'([0-9]{1,3}(?:,[0-9]{3})*\.[0-9]{2})$', prev)
                if m_monto and monto is None:
                    try:
                        monto = float(m_monto.group(1).replace(",", ""))
                    except:
                        pass

                m_fecha = re.search(r'(\d{1,2}\s+de\s+[A-Za-z]{3})', prev)
                if m_fecha and not fecha:
                    fecha = m_fecha.group(1)

                if not desc and len(prev) > 4 and not m_fecha and not m_monto and "RFC" not in prev and "americanexpress" not in prev and "Tarjeta" not in prev and "validez fiscal" not in prev:
                    desc = prev.replace("\t", " ")

            # Si monto o fecha no se encontraron arriba, buscar en las 3 líneas siguientes
            if monto is None or not fecha:
                for k in range(i + 1, min(len(lineas), i + 4)):
                    nxt = lineas[k].strip()
                    m_monto = re.search(r'([0-9]{1,3}(?:,[0-9]{3})*\.[0-9]{2})$', nxt)
                    if m_monto and monto is None:
                        try:
                            monto = float(m_monto.group(1).replace(",", ""))
                        except:
                            pass
                    m_fecha = re.search(r'(\d{1,2}\s+de\s+[A-Za-z]{3})', nxt)
                    if m_fecha and not fecha:
                        fecha = m_fecha.group(1)
                    if not desc and len(nxt) > 4 and not m_fecha and not m_monto and "RFC" not in nxt and "americanexpress" not in nxt and "Tarjeta" not in nxt and "validez fiscal" not in nxt:
                        desc = nxt.replace("\t", " ")

            transacciones.append({
                "rfc": rfc,
                "monto": monto,
                "fecha": fecha,
                "desc": desc,
                "linea_rfc": line.strip()
            })

    return transacciones

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Uso: python extraer_rfcs_pdf.py <ruta_pdf> [ruta_salida_tsv_o_json]")
        sys.exit(1)

    pdf_path = sys.argv[1]
    if not os.path.exists(pdf_path):
        print(f"Error: Archivo no encontrado: {pdf_path}", file=sys.stderr)
        sys.exit(1)

    try:
        txs = extraer_transacciones_pdf(pdf_path)
        out_file = sys.argv[2] if len(sys.argv) >= 3 else None

        if out_file and out_file.lower().endswith(".json"):
            with open(out_file, "w", encoding="utf-8") as f:
                json.dump(txs, f, ensure_ascii=False, indent=2)
        elif out_file:
            with open(out_file, "w", encoding="utf-8") as f:
                for t in txs:
                    monto_str = f"{t['monto']:.2f}" if t['monto'] is not None else ""
                    f.write(f"{t['rfc']}\t{monto_str}\t{t['fecha']}\t{t['desc']}\n")
        else:
            if hasattr(sys.stdout, 'reconfigure'):
                sys.stdout.reconfigure(encoding='utf-8')
            for t in txs:
                monto_str = f"{t['monto']:.2f}" if t['monto'] is not None else ""
                print(f"{t['rfc']}\t{monto_str}\t{t['fecha']}\t{t['desc']}")
    except Exception as e:
        print(f"Error al procesar PDF: {e}", file=sys.stderr)
        sys.exit(1)
