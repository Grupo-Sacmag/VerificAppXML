import fitz
import re

doc = fitz.open(r"E:\EDO CTA JUNIO.pdf")
full_text = ""
for page in doc:
    full_text += page.get_text("text") + "\n"

lines = [l.strip() for l in full_text.splitlines() if l.strip()]

# Parsear transacciones
# Cada transaccion tiene:
# Fecha (ej. 4 de Jun, 22 de Jun)
# Descripcion (ej. PASE CARGO ACUMUL..., RADIOMOVIL..., etc)
# Monto (ej. 285.00)
# RFC (ej. RFCISD950921HE5...)

operaciones = []
i = 0
while i < len(lines):
    line = lines[i]
    
    # Detectar RFC
    m_rfc = re.search(r'RFC([A-Z&Ñ]{3,4}\d{6}[A-Z0-9]{3})', line)
    if m_rfc:
        rfc = m_rfc.group(1).upper()
        # Buscar en ventana de -3 a +5
        window = lines[max(0, i-3) : min(len(lines), i+6)]
        
        monto = None
        fecha = ""
        desc = ""
        
        for w in window:
            # fecha
            mf = re.search(r'(\d{1,2}\s+de\s+[A-Za-z]{3})', w)
            if mf and not fecha:
                fecha = mf.group(1)
            # monto (numero con . y 2 decimales)
            mm = re.search(r'^([0-9]{1,3}(?:,[0-9]{3})*\.[0-9]{2})$', w)
            if mm and monto is None:
                try:
                    monto = float(mm.group(1).replace(",", ""))
                except:
                    pass
            # desc
            if len(w) > 4 and not mf and not mm and "RFC" not in w and "americanexpress" not in w and "Tarjeta" not in w:
                if not desc:
                    desc = w
                    
        operaciones.append({
            "idx": i,
            "rfc": rfc,
            "fecha": fecha,
            "monto": monto,
            "desc": desc,
            "linea": line
        })
    i += 1

print(f"Total operaciones extraidas con RFC: {len(operaciones)}")
pase_ops = [op for op in operaciones if op["rfc"] == "ISD950921HE5"]
print(f"Operaciones de PASE (ISD950921HE5): {len(pase_ops)}")

print("\n--- DETALLE DE TODAS LAS OPERACIONES DE PASE ---")
total_pase = 0.0
for p in pase_ops:
    m = p["monto"] if p["monto"] is not None else 0.0
    total_pase += m
    print(f"  {p['fecha']:10} | ${m:8.2f} | {p['desc']}")

print(f"\nSUMA TOTAL DE PASE EN EL PDF: ${total_pase:.2f}")
print("FACTURA XML: $18,472.00")
print(f"DIFERENCIA: ${total_pase - 18472.00:.2f}")
