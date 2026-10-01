import fitz
import re

pdf_path = r"E:\EDO CTA JUNIO.pdf"
doc = fitz.open(pdf_path)
full_text = ""
for p in doc:
    full_text += p.get_text("text") + "\n"

lines = full_text.splitlines()

print("Buscando todos los bloques de PASE e importes:")
pase_items = []
for i, line in enumerate(lines):
    if "ISD950921HE5" in line:
        # Imprimir las lineas anteriores
        prev_lines = lines[max(0, i-5):i+1]
        
        # Buscar montos en prev_lines
        monto = None
        fecha = ""
        desc = ""
        for pl in prev_lines:
            # fecha
            mf = re.search(r'(\d{1,2}\s+de\s+[A-Za-z]{3})', pl)
            if mf: fecha = mf.group(1)
            # monto
            mm = re.search(r'([0-9]{1,3}(?:,[0-9]{3})*\.[0-9]{2})', pl)
            if mm:
                try:
                    monto = float(mm.group(1).replace(",", ""))
                except:
                    pass
            if "PASE" in pl:
                desc = pl.strip()
                
        pase_items.append({"idx": i, "fecha": fecha, "monto": monto, "desc": desc, "line": line})

print(f"Total items de PASE encontrados: {len(pase_items)}")
suma_con_monto = sum(item["monto"] for item in pase_items if item["monto"] is not None)
print(f"Suma de montos detectados: ${suma_con_monto:.2f}")

sin_monto = [p for p in pase_items if p["monto"] is None]
print(f"Items sin monto detectado: {len(sin_monto)}")
for sm in sin_monto:
    print(" - Sin monto:", lines[sm["idx"]-3 : sm["idx"]+2])

# Mostrar todos los montos detectados
print("\nLista completa de cargos PASE:")
todos_montos = []
for item in pase_items:
    if item["monto"] is not None:
        todos_montos.append(item["monto"])
        print(f"  {item['fecha']} : ${item['monto']:.2f}")

print(f"\nSuma total de todos los {len(todos_montos)} cargos: ${sum(todos_montos):.2f}")
print(f"Total de la factura XML: $18,472.00")
print(f"Diferencia: ${sum(todos_montos) - 18472.00:.2f}")
