import os
import sys
import re
import shutil
import csv
import fitz
import xml.etree.ElementTree as ET

xml_dir = r"D:\Nuevo Programa para Mati\AMEXCO 2026\2026"
pdf_edo_cta = r"C:\Users\david.albino\Desktop\TRABAJO\31_jul_2026_-_30_ago_2026.pdf"
csv_edo_cta = r"C:\Users\david.albino\Desktop\activity-2.csv"
dest_base = r"C:\Users\david.albino\Desktop\orchata"

os.makedirs(dest_base, exist_ok=True)
folder_amarradas = dest_base
folder_mismo_prov = os.path.join(dest_base, "Mismo_Proveedor_Monto_Distinto_Referencia")
os.makedirs(folder_mismo_prov, exist_ok=True)

# 1. Parsear PDF para RFCs
doc = fitz.open(pdf_edo_cta)
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
        pdf_txs.append({"rfc": rfc, "monto": monto, "fecha": fecha, "desc": desc, "used": False})

# 2. Cargar CSV
with open(csv_edo_cta, encoding="utf-8") as f:
    r = csv.reader(f)
    headers = next(r)
    csv_rows = list(r)

operaciones = []
for row in csv_rows:
    if len(row) < 4:
        continue
    f_reg = row[0].strip()
    f_compra = row[1].strip()
    desc = row[2].strip()
    try:
        monto = float(row[3].replace("$", "").replace(",", "").strip())
    except:
        continue

    rfc_banco = ""
    monto_abs = abs(monto)
    cands = [p for p in pdf_txs if not p["used"] and p["monto"] is not None and abs(p["monto"] - monto_abs) <= 0.05]
    if len(cands) == 1:
        rfc_banco = cands[0]["rfc"]
        cands[0]["used"] = True
    elif len(cands) > 1:
        rfc_banco = cands[0]["rfc"]
        cands[0]["used"] = True

    operaciones.append({
        "fecha": f_reg,
        "fecha_compra": f_compra,
        "desc": desc,
        "monto": monto,
        "rfc_banco": rfc_banco
    })

# 3. Cargar XMLs
xml_items = []
for root, dirs, files in os.walk(xml_dir):
    for f in files:
        if f.lower().endswith(".xml"):
            path = os.path.join(root, f)
            base = os.path.splitext(f)[0]
            pdf_path = None
            for f2 in files:
                if f2.lower() == (base.lower() + ".pdf"):
                    pdf_path = os.path.join(root, f2)
                    break
            try:
                tree = ET.parse(path)
                r = tree.getroot()
                total = float(r.attrib.get("Total", "0"))
                subtotal = float(r.attrib.get("SubTotal", "0"))
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
                    "xml_path": path,
                    "pdf_path": pdf_path,
                    "filename": f,
                    "total": total,
                    "subtotal": subtotal,
                    "emisor": emisor,
                    "rfc": rfc,
                    "uuid": uuid,
                    "asignada": False
                })
            except:
                pass

# 4. Cruce y Copia
def limpiar_nombre(texto):
    caracteres_invalidos = r'[\/:*?"<>|]'
    t = re.sub(caracteres_invalidos, "_", texto)
    return re.sub(r"\s+", " ", t).strip()

def coincide_precio(f_total, f_subtotal, monto_op):
    m_abs = abs(monto_op)
    if abs(f_total - m_abs) <= 0.05:
        return True, "Total"
    if f_subtotal > 0 and abs(f_subtotal - m_abs) <= 0.05:
        return True, "SubTotal"
    return False, None

def calcular_afinidad(emisor, desc):
    score = 0
    words = [w for w in re.split(r'[\s_.,*-]+', emisor.upper()) if len(w) >= 4]
    for w in words:
        if w in desc.upper():
            score += 10
    return score

copiados_amarrados = 0
usados_nombres = {}
registros_cruce = []

for op in operaciones:
    monto_abs = abs(op["monto"])
    rfc_op = op["rfc_banco"]

    seleccionada = None
    tipo_match = ""

    # Nivel 1: RFC + Precio
    if rfc_op:
        for x in xml_items:
            if not x["asignada"] and x["rfc"] == rfc_op:
                ok, tipo = coincide_precio(x["total"], x["subtotal"], op["monto"])
                if ok:
                    seleccionada = x
                    tipo_match = f"RFC + {tipo}"
                    break

    # Nivel 2: Nombre + Precio
    if not seleccionada:
        cands = []
        for x in xml_items:
            if not x["asignada"]:
                ok, tipo = coincide_precio(x["total"], x["subtotal"], op["monto"])
                if ok:
                    score = calcular_afinidad(x["emisor"], op["desc"])
                    if score > 0:
                        cands.append((x, score, tipo))
        if cands:
            cands.sort(key=lambda item: item[1], reverse=True)
            seleccionada = cands[0][0]
            tipo_match = f"Nombre + {cands[0][2]}"

    # Nivel 3: Solo Precio
    if not seleccionada:
        cands = []
        for x in xml_items:
            if not x["asignada"]:
                ok, tipo = coincide_precio(x["total"], x["subtotal"], op["monto"])
                if ok:
                    score = calcular_afinidad(x["emisor"], op["desc"])
                    cands.append((x, score, tipo))
        if len(cands) == 1:
            seleccionada = cands[0][0]
            tipo_match = f"Precio ({cands[0][2]})"
        elif len(cands) > 1:
            cands.sort(key=lambda item: item[1], reverse=True)
            seleccionada = cands[0][0]
            tipo_match = f"Precio ({cands[0][2]})"

    if seleccionada:
        seleccionada["asignada"] = True
        copiados_amarrados += 1

        base_title = f"{limpiar_nombre(op['desc'])} - ${monto_abs:.2f} ({op['fecha_compra']})"
        count = usados_nombres.get(base_title, 0) + 1
        usados_nombres[base_title] = count
        if count > 1:
            dest_name = f"{base_title} ({count})"
        else:
            dest_name = base_title

        # Copiar XML
        dst_xml = os.path.join(folder_amarradas, dest_name + ".xml")
        shutil.copy2(seleccionada["xml_path"], dst_xml)

        # Copiar PDF si existe
        tiene_pdf = "No"
        if seleccionada["pdf_path"] and os.path.exists(seleccionada["pdf_path"]):
            dst_pdf = os.path.join(folder_amarradas, dest_name + ".pdf")
            shutil.copy2(seleccionada["pdf_path"], dst_pdf)
            tiene_pdf = "Sí"

        registros_cruce.append({
            "Estado": "FACTURA AMARRADA",
            "Fecha Compra": op["fecha_compra"],
            "Descripcion Banco": op["desc"],
            "Cargo Banco": f"${monto_abs:.2f}",
            "RFC Banco": rfc_op,
            "Total XML": f"${seleccionada['total']:.2f}",
            "RFC XML": seleccionada["rfc"],
            "Proveedor XML": seleccionada["emisor"],
            "Archivo XML Destino": dest_name + ".xml",
            "Archivo PDF Destino": (dest_name + ".pdf") if tiene_pdf == "Sí" else "Sin PDF",
            "Archivo Original": seleccionada["filename"],
            "UUID": seleccionada["uuid"]
        })
    else:
        # Revisar si es mismo RFC
        tiene_mismo_rfc = False
        if rfc_op:
            for x in xml_items:
                if x["rfc"] == rfc_op:
                    tiene_mismo_rfc = True
                    break

        registros_cruce.append({
            "Estado": "MISMO PROVEEDOR (MONTO DISTINTO)" if tiene_mismo_rfc else "FALTA XML",
            "Fecha Compra": op["fecha_compra"],
            "Descripcion Banco": op["desc"],
            "Cargo Banco": f"${monto_abs:.2f}",
            "RFC Banco": rfc_op,
            "Total XML": "",
            "RFC XML": "",
            "Proveedor XML": "",
            "Archivo XML Destino": "",
            "Archivo PDF Destino": "",
            "Archivo Original": "",
            "UUID": ""
        })

# Copiar facturas de referencia con mismo RFC pero monto distinto
copiados_ref = 0
rfcs_copiados = set()
for x in xml_items:
    if x["rfc"] in [op["rfc_banco"] for op in operaciones if op["rfc_banco"]] and not x["asignada"]:
        if x["rfc"] not in rfcs_copiados:
            rfcs_copiados.add(x["rfc"])
            copiados_ref += 1
            ref_name = f"{limpiar_nombre(x['emisor'])} ({x['rfc']}) - ${x['total']:.2f} (Ref {x['filename']})"
            dst_xml = os.path.join(folder_mismo_prov, ref_name + ".xml")
            shutil.copy2(x["xml_path"], dst_xml)
            if x["pdf_path"] and os.path.exists(x["pdf_path"]):
                dst_pdf = os.path.join(folder_mismo_prov, ref_name + ".pdf")
                shutil.copy2(x["pdf_path"], dst_pdf)

# Guardar CSV consolidado dentro de orchata
reporte_csv = os.path.join(dest_base, "00_Reporte_Conciliacion_Orchata.csv")
with open(reporte_csv, "w", encoding="utf-8-sig", newline="") as f:
    writer = csv.DictWriter(f, fieldnames=[
        "Estado", "Fecha Compra", "Descripcion Banco", "Cargo Banco",
        "RFC Banco", "Total XML", "RFC XML", "Proveedor XML",
        "Archivo XML Destino", "Archivo PDF Destino", "Archivo Original", "UUID"
    ])
    writer.writeheader()
    writer.writerows(registros_cruce)

# Copiar el CSV original del banco
shutil.copy2(csv_edo_cta, os.path.join(dest_base, "00_Estado_De_Cuenta_Original_AMEX.csv"))

print(f"EXITO!")
print(f"Total Facturas Amarradas copiadas a orchata: {copiados_amarrados} pares (XML + PDF)")
print(f"Facturas de referencia (mismo RFC monto distinto): {copiados_ref} en subcarpeta")
print(f"Reportes CSV copiados: 00_Reporte_Conciliacion_Orchata.csv y 00_Estado_De_Cuenta_Original_AMEX.csv")
