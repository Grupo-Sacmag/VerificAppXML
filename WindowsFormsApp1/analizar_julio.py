import os
import csv
import xml.etree.ElementTree as ET

xml_dir = r"D:\Nuevo Programa para Mati\AMEXCO 2026\2026"
csv_path = r"C:\Users\david.albino\Desktop\activity-2.csv"

# 1. Analizar la carpeta de Julio
xmls_julio = []
for root, dirs, files in os.walk(xml_dir):
    if "07 julio" in root.lower() or "julio" in root.lower():
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
                    xmls_julio.append({
                        "path": p,
                        "file": f,
                        "subcarpeta": os.path.basename(root),
                        "total": total,
                        "subtotal": subtotal,
                        "fecha": fecha,
                        "emisor": emisor,
                        "rfc": rfc,
                        "uuid": uuid
                    })
                except Exception as e:
                    pass

print(f"Total XMLs en carpetas de Julio (D:\\Nuevo Programa para Mati\\AMEXCO 2026\\2026): {len(xmls_julio)}")
for x in xmls_julio:
    print(f" - {x['file']} | Subcarpeta: {x['subcarpeta']} | Total: ${x['total']:.2f} | Fecha: {x['fecha'][:10]} | RFC: {x['rfc']} | {x['emisor'][:30]}")

# 2. Analizar cargos de Julio en activity-2.csv
print("\n--- CARGOS EN EL CSV ACTUAL (activity-2.csv) QUE CAEN EN JULIO ---")
with open(csv_path, encoding="utf-8") as f:
    r = csv.reader(f)
    next(r)
    rows = list(r)

cargos_julio = []
for row in rows:
    if len(row) < 4:
        continue
    f_reg, f_compra, desc, monto_str = row[0], row[1], row[2], row[3]
    if "Jul" in f_reg or "Jul" in f_compra:
        try:
            monto = float(monto_str.replace("$", "").replace(",", "").strip())
        except:
            monto = 0.0
        cargos_julio.append({
            "f_reg": f_reg,
            "f_compra": f_compra,
            "desc": desc,
            "monto": monto
        })

print(f"Total cargos de Julio en activity-2.csv: {len(cargos_julio)}")
for c in cargos_julio:
    print(f" - {c['f_compra']} | {c['desc']} | ${c['monto']:.2f}")

# 3. Ver cuántos de esos cargos de Julio amarran con los XMLs de Julio
print("\n--- CRUCE ENTRE CARGOS DE JULIO Y XMLS DE JULIO ---")
for c in cargos_julio:
    m_abs = abs(c["monto"])
    matches = [x for x in xmls_julio if abs(x["total"] - m_abs) <= 0.05 or (x["subtotal"] > 0 and abs(x["subtotal"] - m_abs) <= 0.05)]
    if matches:
        print(f" [AMARRA] Cargo: {c['f_compra']} | {c['desc']} | ${c['monto']:.2f}  ==> XML: {matches[0]['file']} (${matches[0]['total']:.2f}) - {matches[0]['emisor']}")
    else:
        # Ver si coincide por RFC/nombre
        matches_prov = [x for x in xmls_julio if any(w in x['emisor'].upper() for w in c['desc'].upper().split() if len(w) > 4)]
        if matches_prov:
            print(f" [MONTO DISTINTO] Cargo: {c['f_compra']} | {c['desc']} | ${c['monto']:.2f}  ==> XML Prov: {matches_prov[0]['file']} (${matches_prov[0]['total']:.2f}) - {matches_prov[0]['emisor']}")
        else:
            print(f" [FALTA XML] Cargo: {c['f_compra']} | {c['desc']} | ${c['monto']:.2f}")
