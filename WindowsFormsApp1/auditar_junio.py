import os
import fitz
import re
import xml.etree.ElementTree as ET

# 1. Leer XMLs en D:\facturas
facturas_path = r"D:\facturas"
xml_items = []
for root, dirs, files in os.walk(facturas_path):
    for f in files:
        if f.lower().endswith(".xml"):
            p = os.path.join(root, f)
            try:
                tree = ET.parse(p)
                r = tree.getroot()
                total = float(r.attrib.get("Total", "0"))
                subtotal = float(r.attrib.get("SubTotal", "0"))
                fecha = r.attrib.get("Fecha", "")
                emisor = ""
                rfc = ""
                uuid = ""
                for elem in r.iter():
                    tag = elem.tag.split("}")[-1]
                    if tag == "Emisor":
                        emisor = elem.attrib.get("Nombre", "")
                        rfc = elem.attrib.get("Rfc", "").strip().upper()
                    elif tag == "TimbreFiscalDigital":
                        uuid = elem.attrib.get("UUID", "")
                xml_items.append({
                    "file": f,
                    "total": total,
                    "subtotal": subtotal,
                    "fecha": fecha,
                    "rfc": rfc,
                    "emisor": emisor,
                    "uuid": uuid
                })
            except Exception as e:
                print("Error leyendo", f, e)

print("=== XMLS EN D:\\facturas ===")
for x in xml_items:
    print(f" - {x['file']} | RFC: {x['rfc']} | Total: ${x['total']:.2f} | SubTotal: ${x['subtotal']:.2f} | {x['emisor']}")

# 2. Leer transacciones de E:\EDO CTA JUNIO.pdf
pdf_path = r"E:\EDO CTA JUNIO.pdf"
doc = fitz.open(pdf_path)
full_text = ""
for p in doc:
    full_text += p.get_text("text") + "\n"

lineas = full_text.splitlines()
pdf_txs = []
for i, line in enumerate(lineas):
    m_rfc = re.search(r'RFC([A-Z&Ñ]{3,4}\d{6}[A-Z0-9]{3})', line)
    if m_rfc:
        rfc = m_rfc.group(1).upper()
        desc, monto, fecha = "", None, ""
        for k in range(max(0, i - 4), i):
            prev = lineas[k].strip()
            m_monto = re.search(r'([0-9]{1,3}(?:,[0-9]{3})*\.[0-9]{2})$', prev)
            if m_monto:
                try:
                    monto = float(m_monto.group(1).replace(",", ""))
                except:
                    pass
            m_fecha = re.search(r'(\d{1,2}\s+de\s+[A-Za-z]{3})', prev)
            if m_fecha:
                fecha = m_fecha.group(1)
            if not desc and len(prev) > 4 and not m_fecha and not m_monto:
                desc = prev.replace("\t", " ")
        pdf_txs.append({"rfc": rfc, "monto": monto, "fecha": fecha, "desc": desc, "linea": line})

print(f"\n=== TRANSACCIONES EN {pdf_path} (Total con RFC: {len(pdf_txs)}) ===")
pase_txs = [t for t in pdf_txs if t["rfc"] == "ISD950921HE5"]
print(f"Transacciones de PASE (ISD950921HE5): {len(pase_txs)}")
suma_pase = sum(t["monto"] for t in pase_txs if t["monto"] is not None)
print(f"Suma total de TODOS los cargos de PASE en el PDF: ${suma_pase:.2f}")

print("\nDetalle de cada cargo de PASE en el PDF:")
for t in pase_txs:
    print(f"   {t['fecha']} | ${t['monto']} | {t['desc']}")

# 3. Comparar con los XMLs de PASE en D:\facturas
pase_xmls = [x for x in xml_items if x["rfc"] == "ISD950921HE5"]
print("\n=== XMLS DE PASE EN D:\\facturas ===")
for x in pase_xmls:
    print(f"   XML: {x['file']} | Total: ${x['total']:.2f} | SubTotal: ${x['subtotal']:.2f} | UUID: {x['uuid']}")

# 4. Ver si hay cargos que amarran 1 a 1 y ver la suma de los que sobran
cargos_individuales_amarrados = []
cargos_restantes = []

for t in pase_txs:
    m = t["monto"]
    # checar si amarra 1 a 1 con algun xml de pase (ej. $133.00)
    match_1_1 = [x for x in pase_xmls if abs(x["total"] - m) <= 0.05 or abs(x["subtotal"] - m) <= 0.05]
    if match_1_1 and match_1_1[0]["file"] not in [c["xml"] for c in cargos_individuales_amarrados]:
        cargos_individuales_amarrados.append({"cargo": t, "xml": match_1_1[0]["file"], "monto": m})
    else:
        cargos_restantes.append(t)

print(f"\nCargos de PASE amarrados 1 a 1: {len(cargos_individuales_amarrados)}")
for c in cargos_individuales_amarrados:
    print(f"   Amarrado 1 a 1: ${c['monto']} con {c['xml']}")

suma_restantes = sum(t["monto"] for t in cargos_restantes if t["monto"] is not None)
print(f"\nTotal cargos de PASE restantes: {len(cargos_restantes)}")
print(f"Suma de los cargos de PASE restantes: ${suma_restantes:.2f}")

# Ver contra el XML de 18,472
xml_18472 = next((x for x in pase_xmls if abs(x["total"] - 18472.0) <= 1.0 or abs(x["subtotal"] - 18472.0) <= 1.0), None)
if xml_18472:
    print(f"\nXML acumulado encontrado: {xml_18472['file']} | Total: ${xml_18472['total']:.2f} | Subtotal: ${xml_18472['subtotal']:.2f}")
    dif_total = suma_restantes - xml_18472["total"]
    print(f"DIFERENCIA (Suma Cargos - Total XML $18,472.00): ${dif_total:.2f}")
    dif_subtotal = suma_restantes - xml_18472["subtotal"]
    print(f"DIFERENCIA (Suma Cargos - Subtotal XML $15,924.14): ${dif_subtotal:.2f}")
