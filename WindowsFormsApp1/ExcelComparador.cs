using System;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;

namespace WindowsFormsApp1
{
    public class FilaComparacion
    {
        public string FuenteExcel { get; set; }   // NUEVO — identifica de cuál Excel viene esta fila
        public string UUID { get; set; }
        public string NombreArchivo { get; set; }
        public string Campo { get; set; }
        public string ValorXML { get; set; }
        public string ValorExcel { get; set; }
        public string Resultado { get; set; }
    }

    public static class ExcelComparador
    {
        // Alias: nombre canónico -> posibles encabezados en cualquiera de las dos plantillas.
        // Para agregar un campo nuevo a la comparación: agrega una entrada aquí y
        // asegúrate de que el nombre canónico exista como Func en ObtenerValorXml().
        private static readonly Dictionary<string, string[]> AliasCampos =
            new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                ["UUID"] = new[] { "UUID" },
                ["Serie"] = new[] { "Serie" },
                ["Folio"] = new[] { "Folio" },
                ["RfcEmisor"] = new[] { "Emisor RFC", "RFC Emisor" },
                ["NombreEmisor"] = new[] { "Emisor Nombre", "Nombre Emisor" },
                ["RfcReceptor"] = new[] { "RFC Receptor" },
                ["UsoCFDI"] = new[] { "Uso CFDI", "UsoCFDI" },
                ["Moneda"] = new[] { "Moneda" },
                ["FormaPago"] = new[] { "FormaPago", "Forma de pago conciliada" },
                ["MetodoPago"] = new[] { "Método Pago", "Metodo de Pago" },
                ["SubTotal"] = new[] { "SubTotal" },
                ["Descuento"] = new[] { "Descuento" },
                ["IVA"] = new[] { "IVA", "IVA 16 Importe" },
                ["ImpuestosTrasladados"] = new[] { "IVA", "Total Trasladados" },
                ["ImpuestosRetenidos"] = new[] { "IVA Retenido", "Total Retenidos" },
                ["ISRRetenido"] = new[] { "ISR Retenido" },
                ["Total"] = new[] { "Total", "TotalOriginalXML" },
            };

        // Cuánto puede diferir un valor numérico antes de marcarse DIFERENTE
        // (tolerancia por redondeos entre el motor del XML y el reporte del Excel).
        private const decimal ToleranciaDecimal = 0.02m;

        // Reemplaza la firma pública actual por esta sobrecarga múltiple.
        public static List<FilaComparacion> Comparar(List<ResultadoValidacion> resultadosXml, List<(string NombreFuente, string RutaExcel)> fuentes)
        {
            var resultado = new List<FilaComparacion>();

            foreach (var fuente in fuentes)
            {
                if (string.IsNullOrWhiteSpace(fuente.RutaExcel)) continue;

                var filasFuente = CompararContraUnaFuente(resultadosXml, fuente.RutaExcel);
                foreach (var fila in filasFuente)
                {
                    fila.FuenteExcel = fuente.NombreFuente;
                    resultado.Add(fila);
                }
            }

            return resultado;
        }

        public static List<FilaComparacion> CompararContraUnaFuente(List<ResultadoValidacion> resultadosXml, string rutaExcel)
        {
            var filasExcel = LeerExcelGenerico(rutaExcel);

            // Índice por UUID (case-insensitive) para búsqueda O(1)
            var indiceExcel = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            foreach (var fila in filasExcel)
            {
                if (fila.TryGetValue("UUID", out string uuidExcel) && !string.IsNullOrWhiteSpace(uuidExcel))
                {
                    indiceExcel[uuidExcel.Trim()] = fila;
                }
            }

            var resultado = new List<FilaComparacion>();

            foreach (var r in resultadosXml)
            {
                if (string.IsNullOrEmpty(r.UUID) || r.UUID == "SIN_UUID") continue;

                if (!indiceExcel.TryGetValue(r.UUID.Trim(), out var filaExcel))
                {
                    resultado.Add(new FilaComparacion
                    {
                        UUID = r.UUID,
                        NombreArchivo = r.NombreArchivo,
                        Campo = "(General)",
                        ValorXML = "Presente en XML",
                        ValorExcel = "",
                        Resultado = "NO ENCONTRADO EN EXCEL"
                    });
                    continue;
                }

                foreach (var campo in AliasCampos.Keys)
                {
                    if (campo == "UUID") continue; // ya es la clave de emparejamiento

                    string valorXml = ObtenerValorXml(r, campo);
                    string valorExcel = BuscarPorAlias(filaExcel, AliasCampos[campo]);

                    if (valorExcel == null)
                    {
                        resultado.Add(new FilaComparacion
                        {
                            UUID = r.UUID,
                            NombreArchivo = r.NombreArchivo,
                            Campo = campo,
                            ValorXML = valorXml,
                            ValorExcel = "(columna no encontrada en este Excel)",
                            Resultado = "CAMPO NO MAPEADO"
                        });
                        continue;
                    }

                    bool coincide = ValoresCoinciden(valorXml, valorExcel);
                    resultado.Add(new FilaComparacion
                    {
                        UUID = r.UUID,
                        NombreArchivo = r.NombreArchivo,
                        Campo = campo,
                        ValorXML = valorXml,
                        ValorExcel = valorExcel,
                        Resultado = coincide ? "COINCIDE" : "DIFERENTE"
                    });
                }
            }

            return resultado;
        }

        private static string ObtenerValorXml(ResultadoValidacion r, string campo)
        {
            switch (campo)
            {
                case "Serie": return r.Serie;
                case "Folio": return r.Folio;
                case "RfcEmisor": return r.RfcEmisor;
                case "NombreEmisor": return r.NombreEmisor;
                case "RfcReceptor": return r.RfcReceptor;
                case "UsoCFDI": return r.UsoCFDI;
                case "Moneda": return r.Moneda;
                case "FormaPago": return r.FormaPago;
                case "MetodoPago": return r.MetodoPago;
                case "SubTotal": return r.Subtotal?.ToString("0.00");
                case "Descuento": return r.Descuento?.ToString("0.00");
                case "IVA": return r.IVA?.ToString("0.00");
                case "ImpuestosTrasladados": return r.ImpuestosTrasladados?.ToString("0.00");
                case "ImpuestosRetenidos": return r.ImpuestosRetenidos?.ToString("0.00");
                case "Total": return r.Total?.ToString("0.00");
                default: return "";
            }
        }

        private static string BuscarPorAlias(Dictionary<string, string> filaExcel, string[] alias)
        {
            foreach (var nombre in alias)
            {
                if (filaExcel.TryGetValue(nombre, out string valor))
                    return valor;
            }
            return null;
        }

        private static bool ValoresCoinciden(string valorXml, string valorExcel)
        {
            if (string.IsNullOrWhiteSpace(valorXml) && string.IsNullOrWhiteSpace(valorExcel)) return true;

            if (decimal.TryParse(valorXml, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal numXml) &&
                decimal.TryParse(valorExcel, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal numExcel))
            {
                return Math.Abs(numXml - numExcel) <= ToleranciaDecimal;
            }

            string a = (valorXml ?? "").Trim();
            string b = (valorExcel ?? "").Trim();
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        // Lee cualquier hoja/plantilla: detecta automáticamente la fila de encabezado
        // (la primera fila que contenga una celda con el texto "UUID"), y devuelve
        // cada fila de datos como diccionario encabezado -> valor de celda (texto).
        private static List<Dictionary<string, string>> LeerExcelGenerico(string rutaExcel)
        {
            var filas = new List<Dictionary<string, string>>();

            using (var libro = new XLWorkbook(rutaExcel))
            {
                var hoja = libro.Worksheets.First(); // primera hoja con datos
                var rangoUsado = hoja.RangeUsed();
                if (rangoUsado == null) return filas;

                int filaEncabezado = -1;
                int ultimaFilaBusqueda = Math.Min(rangoUsado.LastRow().RowNumber(), 15);

                for (int fila = rangoUsado.FirstRow().RowNumber(); fila <= ultimaFilaBusqueda; fila++)
                {
                    var celdas = hoja.Row(fila).CellsUsed();
                    if (celdas.Any(c => c.GetString().Trim().Equals("UUID", StringComparison.OrdinalIgnoreCase)))
                    {
                        filaEncabezado = fila;
                        break;
                    }
                }

                if (filaEncabezado < 0)
                    throw new InvalidOperationException("No se encontró una columna 'UUID' en las primeras filas del Excel. Verifique que el archivo tenga el formato esperado.");

                var columnas = new Dictionary<int, string>();
                foreach (var celda in hoja.Row(filaEncabezado).CellsUsed())
                {
                    string encabezado = celda.GetString().Trim();
                    if (!string.IsNullOrEmpty(encabezado))
                        columnas[celda.Address.ColumnNumber] = encabezado;
                }

                int primeraFilaDatos = filaEncabezado + 1;
                int ultimaFilaDatos = rangoUsado.LastRow().RowNumber();

                for (int fila = primeraFilaDatos; fila <= ultimaFilaDatos; fila++)
                {
                    var filaExcel = hoja.Row(fila);
                    if (filaExcel.IsEmpty()) continue;

                    var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var kvp in columnas)
                    {
                        var celda = filaExcel.Cell(kvp.Key);
                        dict[kvp.Value] = celda.GetString().Trim();
                    }

                    // Filas de totales al pie ("Total Vigentes", etc.) no traen UUID: se descartan.
                    if (dict.TryGetValue("UUID", out string uuid) && !string.IsNullOrWhiteSpace(uuid))
                        filas.Add(dict);
                }
            }

            return filas;
        }
    }
}