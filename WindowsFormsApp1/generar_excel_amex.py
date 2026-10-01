import os
import re
import sys
import pdfplumber
import pandas as pd

def convertir_amex_a_excel(pdf_path, excel_path):
    patron_fecha = re.compile(r'^(\d{1,2})\s*de\s*([A-Za-z]{3,4})\b', re.IGNORECASE)
    # Soporta montos positivos, negativos, y con CR al final: ej. '1,450.00', '-500.00', '1,450.00CR'
    patron_monto = re.compile(r'([-\+]?\d{1,3}(?:,\d{3})*\.\d{2})\s*(CR)?\s*$', re.IGNORECASE)
    patron_rfc = re.compile(r'RFC\s*([A-ZÑ&]{3,4}\d{6}[A-Z0-9]{3})\b', re.IGNORECASE)

    movimientos = []
    
    with pdfplumber.open(pdf_path) as pdf:
        for num_pag, p in enumerate(pdf.pages):
            text = p.extract_text() or ''
            lines = text.split('\n')
            mov_actual = None
            
            for line in lines:
                l = line.strip()
                if not l: continue
                
                # Ignorar encabezados corporativos
                if any(x in l for x in ['Corporate Card', 'americanexpress', 'Estado de Cuenta', 'Fecha de corte', 'Fecha y detalle de las operaciones', 'Detalle de nuevos cargos']):
                    continue
                
                m_f = patron_fecha.match(l)
                if m_f:
                    if mov_actual:
                        movimientos.append(mov_actual)
                        mov_actual = None
                    
                    m_m = patron_monto.search(l)
                    if m_m:
                        monto_raw = m_m.group(1).replace(',', '')
                        monto = float(monto_raw)
                        es_cr = bool(m_m.group(2))
                        
                        fin_f = m_f.end()
                        ini_m = m_m.start()
                        desc = l[fin_f:ini_m].strip()
                        dia = m_f.group(1)
                        mes = m_f.group(2)
                        
                        # Determinar si es crédito / abono / pago / devolución
                        es_credito = False
                        if monto < 0 or es_cr:
                            es_credito = True
                            monto = -abs(monto)
                        elif any(w in desc.upper() for w in ['PAGO RECIBIDO', 'CREDITO', 'CRÉDITO', 'DEVOLUCION', 'DEVOLUCIÓN', 'ABONO', 'REEMBOLSO', 'BONIFICACION', 'SALDO A FAVOR']):
                            es_credito = True
                            monto = -abs(monto)
                        
                        mov_actual = {
                            'Pagina': num_pag + 1,
                            'Fecha': f"{dia} de {mes}",
                            'Concepto / Establecimiento': desc,
                            'Tipo': 'CRÉDITO / ABONO' if es_credito else 'CARGO',
                            'Importe': monto,
                            'RFC Proveedor': ''
                        }
                else:
                    if mov_actual:
                        m_rfc = patron_rfc.search(l)
                        if m_rfc and not mov_actual['RFC Proveedor']:
                            mov_actual['RFC Proveedor'] = m_rfc.group(1).upper()
                        elif '-' in l or 'CR' in l.upper():
                            if any(w in l.upper() for w in ['CREDITO', 'CRÉDITO', 'DEVOLUCION', 'DEVOLUCIÓN', 'REEMBOLSO', 'BONIFICACION']):
                                mov_actual['Tipo'] = 'CRÉDITO / ABONO'
                                mov_actual['Importe'] = -abs(mov_actual['Importe'])
            
            if mov_actual:
                movimientos.append(mov_actual)

    df = pd.DataFrame(movimientos)
    
    # Crear archivo Excel con formato
    with pd.ExcelWriter(excel_path, engine='openpyxl') as writer:
        df.to_excel(writer, index=False, sheet_name='Movimientos AMEX')
        ws = writer.sheets['Movimientos AMEX']
        
        # Ajustar ancho de columnas
        for col in ws.columns:
            max_len = max(len(str(cell.value or '')) for cell in col)
            col_letter = col[0].column_letter
            ws.column_dimensions[col_letter].width = max(max_len + 3, 12)
            
    print(f"Excel generado exitosamente en: {excel_path}")
    print(f"Total registros: {len(df)}")
    print(f"Total Cargos: {len(df[df['Tipo'] == 'CARGO'])}")
    print(f"Total Créditos/Abonos: {len(df[df['Tipo'] == 'CRÉDITO / ABONO'])}")
    return excel_path

if __name__ == '__main__':
    if len(sys.argv) >= 3:
        convertir_amex_a_excel(sys.argv[1], sys.argv[2])
    else:
        pdf_in = r'C:\Users\david.albino\Desktop\31_jul_2026_-_30_ago_2026.pdf'
        excel_out = r'C:\Users\david.albino\Desktop\AMEX_Agosto_2026_Procesado.xlsx'
        convertir_amex_a_excel(pdf_in, excel_out)
