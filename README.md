# VerificAppXML - Validador y Conciliador de CFDI Facturas

**VerificAppXML** es una aplicación de escritorio desarrollada en C# WinForms (.NET Framework 4.8) diseñada para auditar, sanitizar y conciliar de forma masiva los comprobantes fiscales digitales (CFDI XML) reportados en reembolsos de caja chica.

---

## 🚀 Características Principales

### 1. Validación de Integridad (Fase 1)
* **Validación Criptográfica:** Verifica de forma local la validez del sello digital (firma) de cada archivo XML contra el estándar oficial del SAT.
* **Auto-Reparación de Texto:** Detecta y corrige automáticamente en memoria fallos comunes de codificación de texto provenientes de proveedores específicos (por ejemplo, reemplaza secuencias rotas como `segC:n`, `nC:mero`, `canciC:n` o similares a sus acentos correspondientes).
* **Control de Expediente:** Asegura que cada archivo XML cuente con su respectivo archivo PDF correlacionado en la misma carpeta.

### 2. Validación Cruzada (Fase 2)
* **Cruces Automáticos:** Compara los datos del XML físico contra un registro oficial de respaldo (soporta tanto plantillas de control interno como reportes crudos de metadatos descargados del SAT mediante **eFirma**).
* **Búsqueda Dinámica por UUID:** Busca de forma inteligente las facturas en el Excel sin importar en qué fila inicien los encabezados.
* **Detección de Discrepancias:** Compara montos totales (con margen de redondeo de 5 centavos) y métodos de pago (PUE/PPD).
* **Alerta de Columnas Opcionales:** Si al Excel le falta alguna columna (como Forma de Pago en las descargas del SAT), el sistema alerta al usuario y prosigue con los campos que sí están presentes.

### 3. Visor Integrado de Inconsistencias
* Permite visualizar el XML crudo con un resaltado dinámico (en rojo y amarillo) del fragmento de código exacto que contiene el error o la alteración.

---

## 🛠️ Requisitos de Compilación y Ejecución

* **Entorno:** Visual Studio 2022 o posterior
* **Framework:** .NET Framework 4.8
* **Dependencias:**
  * `System.IO.Compression` (para descompresión rápida del contenido XML del archivo Excel .xlsx)

### Instrucciones de Compilación:
Para restaurar y compilar el proyecto por consola:
```bash
dotnet build "WindowsFormsApp1.csproj"
```

---

## 🌿 Estructura de Ramas (Gitflow)
* `main`: Rama de producción (código estable y liberado).
* `dev`: Rama de desarrollo donde se integran las nuevas características.
* `pochoclo`: Rama de trabajo activo para la implementación de cambios de desarrollo.
