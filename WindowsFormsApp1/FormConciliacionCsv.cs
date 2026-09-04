using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace WindowsFormsApp1
{
    public partial class FormConciliacionCsv : Form
    {
        private List<ItemConciliacion> _listaConciliacion = new List<ItemConciliacion>();
        private List<FacturaXmlItem> _listaFacturasXml = new List<FacturaXmlItem>();

        public FormConciliacionCsv()
        {
            InitializeComponent();
        }

        private void btnSeleccionarCsv_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Estados de Cuenta (*.pdf;*.csv)|*.pdf;*.csv|Archivos PDF (*.pdf)|*.pdf|Archivos CSV (*.csv)|*.csv|Todos los archivos (*.*)|*.*";
                ofd.Title = "Seleccionar Estado de Cuenta (PDF o CSV)";
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtRutaCsv.Text = ofd.FileName;
                }
            }
        }

        private void btnSeleccionarCarpetaEdoCta_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Seleccionar Carpeta con Estado de Cuenta (PDF y/o CSV)";
                fbd.ShowNewFolderButton = false;
                fbd.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtRutaCsv.Text = fbd.SelectedPath;
                }
            }
        }

        private void btnSeleccionarCarpetaXml_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Seleccionar Carpeta de Facturas XML (ej. Carpeta Anual o Mensual)";
                fbd.ShowNewFolderButton = false;

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtRutaCarpetaXml.Text = fbd.SelectedPath;
                }
            }
        }

        private async void btnConciliar_Click(object sender, EventArgs e)
        {
            string rutaEdoCta = txtRutaCsv.Text.Trim();
            string rutaCarpeta = txtRutaCarpetaXml.Text.Trim();

            bool esArchivo = File.Exists(rutaEdoCta);
            bool esDirectorio = Directory.Exists(rutaEdoCta);

            if (string.IsNullOrEmpty(rutaEdoCta) || (!esArchivo && !esDirectorio))
            {
                MessageBox.Show("Por favor seleccione un archivo (PDF/CSV) o una carpeta válida con el Estado de Cuenta.", "Estado de Cuenta no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(rutaCarpeta) || !Directory.Exists(rutaCarpeta))
            {
                MessageBox.Show("Por favor seleccione una carpeta válida con archivos XML.", "Carpeta no encontrada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnConciliar.Enabled = false;
            btnSeleccionarCsv.Enabled = false;
            btnSeleccionarCarpetaEdoCta.Enabled = false;
            btnSeleccionarCarpetaXml.Enabled = false;
            btnExportar.Enabled = false;

            progressBar1.Visible = true;
            progressBar1.Value = 0;
            lblEstado.Text = "Leyendo Estado de Cuenta y extrayendo RFCs...";

            try
            {
                var progreso = new Progress<int>(v => progressBar1.Value = Math.Min(100, Math.Max(0, v)));

                var resultado = await Task.Run(() => ProcesarConciliacion(rutaEdoCta, rutaCarpeta, progreso));

                _listaConciliacion = resultado.ItemsConciliacion;
                _listaFacturasXml = resultado.FacturasXml;

                MostrarResultadosEnGrids();

                lblEstado.Text = $"Conciliación finalizada. Amarradas: {resultado.Coincidentes} | Faltantes / Dif. Monto: {resultado.Faltantes} | XMLs sin cargo: {resultado.Huerfanos}";

                MessageBox.Show(
                    $"¡Conciliación Completada!\n\n" +
                    $"• Total de Operaciones en Estado de Cuenta: {resultado.ItemsConciliacion.Count}\n" +
                    $"• Facturas XML Amarradas: {resultado.Coincidentes}\n" +
                    $"• Operaciones Faltantes de Factura / Dif. Monto: {resultado.Faltantes}\n" +
                    $"• Facturas XML sin cargo en Banco: {resultado.Huerfanos}",
                    "Resultado de Conciliación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                lblEstado.Text = "Error durante el procesamiento.";
                MessageBox.Show($"Ocurrió un error al procesar la conciliación:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnConciliar.Enabled = true;
                btnSeleccionarCsv.Enabled = true;
                btnSeleccionarCarpetaEdoCta.Enabled = true;
                btnSeleccionarCarpetaXml.Enabled = true;
                btnExportar.Enabled = true;
                progressBar1.Visible = false;
            }
        }

        private ResultadoProceso ProcesarConciliacion(string rutaEdoCta, string rutaCarpetaXml, IProgress<int> progreso)
        {
            progreso?.Report(5);

            string rutaCsv = "";
            string rutaPdf = "";

            if (Directory.Exists(rutaEdoCta))
            {
                // Es carpeta: buscar CSV y PDF adentro
                var archivosCsv = Directory.GetFiles(rutaEdoCta, "*.csv", SearchOption.TopDirectoryOnly);
                if (archivosCsv.Length > 0) rutaCsv = archivosCsv[0];

                var archivosPdf = Directory.GetFiles(rutaEdoCta, "*.pdf", SearchOption.TopDirectoryOnly);
                if (archivosPdf.Length > 0) rutaPdf = archivosPdf[0];
            }
            else if (File.Exists(rutaEdoCta))
            {
                string ext = Path.GetExtension(rutaEdoCta).ToLowerInvariant();
                string dir = Path.GetDirectoryName(rutaEdoCta);

                if (ext == ".pdf")
                {
                    rutaPdf = rutaEdoCta;
                    var archivosCsv = Directory.GetFiles(dir, "*.csv", SearchOption.TopDirectoryOnly);
                    if (archivosCsv.Length > 0) rutaCsv = archivosCsv[0];
                }
                else if (ext == ".csv")
                {
                    rutaCsv = rutaEdoCta;
                    var archivosPdf = Directory.GetFiles(dir, "*.pdf", SearchOption.TopDirectoryOnly);
                    if (archivosPdf.Length > 0) rutaPdf = archivosPdf[0];
                }
            }

            progreso?.Report(10);

            // 1. Extraer RFCs del PDF si está disponible
            List<PdfTransaccionItem> txsPdf = new List<PdfTransaccionItem>();
            if (!string.IsNullOrEmpty(rutaPdf) && File.Exists(rutaPdf))
            {
                txsPdf = ExtraerRfcsDePdf(rutaPdf);
            }

            progreso?.Report(20);

            // 2. Obtener operaciones bancarias (del CSV o directamente del PDF)
            List<OperacionCsv> operaciones = new List<OperacionCsv>();
            if (!string.IsNullOrEmpty(rutaCsv) && File.Exists(rutaCsv))
            {
                operaciones = ParsearCsvOperaciones(rutaCsv);
            }
            else if (txsPdf.Count > 0)
            {
                operaciones = ExtraerOperacionesDesdePdf(rutaPdf, txsPdf);
            }

            // 3. Vincular RFCs del PDF a las operaciones bancarias
            if (txsPdf.Count > 0 && operaciones.Count > 0)
            {
                foreach (var op in operaciones)
                {
                    if (!string.IsNullOrEmpty(op.RfcBanco)) continue;

                    decimal opMontoAbs = Math.Abs(op.Importe);
                    // Buscar coincidencia en txsPdf por monto exacto (tolerancia 5 centavos)
                    var candidatasPdf = txsPdf
                        .Where(p => !p.Usado && p.Monto.HasValue && Math.Abs(p.Monto.Value - opMontoAbs) <= 0.05m)
                        .ToList();

                    if (candidatasPdf.Count == 1)
                    {
                        op.RfcBanco = candidatasPdf[0].Rfc;
                        candidatasPdf[0].Usado = true;
                    }
                    else if (candidatasPdf.Count > 1)
                    {
                        // Desempatar por afinidad de texto en la descripción
                        var mejor = candidatasPdf
                            .OrderByDescending(p => CalcularAfinidadNombre(p.Desc, p.Rfc, op.Descripcion))
                            .First();

                        op.RfcBanco = mejor.Rfc;
                        mejor.Usado = true;
                    }
                }
            }

            progreso?.Report(40);
            var facturasXml = EscanearCarpetaXml(rutaCarpetaXml);

            progreso?.Report(65);

            // 4. Algoritmo de emparejamiento con 3 niveles de auditoría (RFC + Monto, RFC mismo proveedor, Fallbacks)
            var itemsConciliados = new List<ItemConciliacion>();

            foreach (var op in operaciones)
            {
                var item = new ItemConciliacion
                {
                    Fecha = op.FechaTexto,
                    FechaCompra = op.FechaCompraTexto,
                    Descripcion = op.Descripcion,
                    ImporteCsv = op.Importe,
                    RfcBanco = op.RfcBanco,
                    FechaCompraDate = op.FechaCompraDate
                };

                // Si es abono / pago de tarjeta grande, se etiqueta
                if (op.Importe < 0 && (op.Descripcion.IndexOf("GRACIAS POR SU PAGO", StringComparison.OrdinalIgnoreCase) >= 0 || op.Importe < -10000))
                {
                    item.EstadoFactura = "ℹ️ PAGO DE TARJETA / ABONO";
                    itemsConciliados.Add(item);
                    continue;
                }

                decimal importeAbs = Math.Abs(op.Importe);

                bool CoincidePrecio(FacturaXmlItem f, out bool esSubtotal)
                {
                    esSubtotal = false;
                    if (Math.Abs(f.Total - importeAbs) <= 0.05m) return true;
                    if (f.SubTotal > 0 && Math.Abs(f.SubTotal - importeAbs) <= 0.05m)
                    {
                        esSubtotal = true;
                        return true;
                    }
                    return false;
                }

                FacturaXmlItem seleccionada = null;
                bool matchSubtotal = false;
                FacturaXmlItem facturaMismoRfcDistintoMonto = null;

                // NIVEL 1: Si tenemos el RFC exacto del Banco (desde el PDF)
                if (!string.IsNullOrEmpty(op.RfcBanco))
                {
                    var facturasMismoRfc = facturasXml
                        .Where(f => !string.IsNullOrEmpty(f.EmisorRfc) && f.EmisorRfc.Equals(op.RfcBanco, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (facturasMismoRfc.Count > 0)
                    {
                        // 1.1 Buscar si alguna coincide en PRECIO (Total o Subtotal)
                        var candidataPrecio = facturasMismoRfc.FirstOrDefault(f => !f.Asignada && CoincidePrecio(f, out matchSubtotal));
                        if (candidataPrecio != null)
                        {
                            seleccionada = candidataPrecio;
                        }
                        else
                        {
                            // 1.2 Mismo RFC pero monto distinto (ej. Casetas PASE consolidada de $65,588 vs cargo de $126)
                            facturaMismoRfcDistintoMonto = facturasMismoRfc.First();
                        }
                    }
                }

                // NIVEL 2: Fallback por Nombre / Proveedor y Precio si no se amarró por RFC
                if (seleccionada == null && facturaMismoRfcDistintoMonto == null)
                {
                    var candidatasNombre = facturasXml
                        .Where(f => !f.Asignada)
                        .Select(f => new { Factura = f, Afinidad = CalcularAfinidadNombre(f.EmisorNombre, f.EmisorRfc, op.Descripcion) })
                        .Where(x => x.Afinidad > 0)
                        .OrderByDescending(x => x.Afinidad)
                        .ToList();

                    if (candidatasNombre.Count > 0)
                    {
                        var candidatasNombreYPrecio = new List<(FacturaXmlItem Factura, int Afinidad, bool EsSubtotal, int DiasDiff)>();
                        foreach (var c in candidatasNombre)
                        {
                            if (CoincidePrecio(c.Factura, out bool esSub))
                            {
                                int dias = 999;
                                if (op.FechaCompraDate.HasValue && c.Factura.FechaCfdi.HasValue)
                                    dias = Math.Abs((op.FechaCompraDate.Value - c.Factura.FechaCfdi.Value).Days);
                                candidatasNombreYPrecio.Add((c.Factura, c.Afinidad, esSub, dias));
                            }
                        }

                        if (candidatasNombreYPrecio.Count > 0)
                        {
                            var mejor = candidatasNombreYPrecio
                                .OrderByDescending(x => x.Afinidad)
                                .ThenBy(x => x.DiasDiff)
                                .First();

                            seleccionada = mejor.Factura;
                            matchSubtotal = mejor.EsSubtotal;
                        }
                    }
                }

                // NIVEL 3: Fallback por Precio solo (para intermediarios o descripciones genéricas)
                if (seleccionada == null && facturaMismoRfcDistintoMonto == null)
                {
                    var candidatasSoloPrecio = new List<(FacturaXmlItem Factura, int Afinidad, bool EsSubtotal, int DiasDiff)>();
                    foreach (var f in facturasXml.Where(x => !x.Asignada))
                    {
                        if (CoincidePrecio(f, out bool esSub))
                        {
                            int afinidad = CalcularAfinidadNombre(f.EmisorNombre, f.EmisorRfc, op.Descripcion);
                            int dias = 999;
                            if (op.FechaCompraDate.HasValue && f.FechaCfdi.HasValue)
                                dias = Math.Abs((op.FechaCompraDate.Value - f.FechaCfdi.Value).Days);
                            candidatasSoloPrecio.Add((f, afinidad, esSub, dias));
                        }
                    }

                    if (candidatasSoloPrecio.Count > 0)
                    {
                        var mejor = candidatasSoloPrecio
                            .OrderByDescending(x => x.Afinidad)
                            .ThenBy(x => x.DiasDiff)
                            .First();

                        seleccionada = mejor.Factura;
                        matchSubtotal = mejor.EsSubtotal;
                    }
                }

                // ASIGNACIÓN FINAL
                if (seleccionada != null)
                {
                    seleccionada.Asignada = true;
                    AsignarFacturaAItem(item, seleccionada, matchSubtotal);
                }
                else if (facturaMismoRfcDistintoMonto != null)
                {
                    item.EstadoFactura = "⚠️ MISMO PROVEEDOR (MONTO DISTINTO)";
                    item.RfcXml = facturaMismoRfcDistintoMonto.EmisorRfc;
                    item.Emisor = facturaMismoRfcDistintoMonto.EmisorNombre;
                    item.TotalXml = facturaMismoRfcDistintoMonto.Total;
                    item.SubTotalXml = facturaMismoRfcDistintoMonto.SubTotal;
                    item.Diferencia = Math.Abs(item.ImporteCsv - facturaMismoRfcDistintoMonto.Total);
                    item.UUID = facturaMismoRfcDistintoMonto.UUID;
                    item.ArchivoXml = facturaMismoRfcDistintoMonto.NombreArchivo;
                    item.Subcarpeta = facturaMismoRfcDistintoMonto.Subcarpeta;
                    item.RutaXml = facturaMismoRfcDistintoMonto.RutaCompleta;
                }
                else
                {
                    item.EstadoFactura = "❌ FALTA XML";
                }

                itemsConciliados.Add(item);
            }

            // -------------------------------------------------------------
            // NIVEL 4: CONCILIACIÓN AGRUPADA POR PROVEEDOR (RFC)
            // Caso A: N cargos de banco que suman 1 Factura XML acumulada (ej. Casetas PASE)
            // Caso B: 1 cargo de banco que suma N Facturas XML fraccionadas
            // -------------------------------------------------------------
            var rfcsPendientes = itemsConciliados
                .Where(i => !i.EstadoFactura.StartsWith("✅") && !string.IsNullOrEmpty(i.RfcBanco))
                .Select(i => i.RfcBanco)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var rfc in rfcsPendientes)
            {
                var cargosPendientesRfc = itemsConciliados
                    .Where(i => !i.EstadoFactura.StartsWith("✅") && string.Equals(i.RfcBanco, rfc, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var facturasPendientesRfc = facturasXml
                    .Where(f => !f.Asignada && string.Equals(f.EmisorRfc, rfc, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (cargosPendientesRfc.Count == 0 || facturasPendientesRfc.Count == 0) continue;

                // CASO A: N cargos que suman 1 XML acumulado
                foreach (var f in facturasPendientesRfc.ToList())
                {
                    if (f.Asignada) continue;
                    var disponibles = cargosPendientesRfc.Where(i => !i.EstadoFactura.StartsWith("✅")).ToList();
                    if (disponibles.Count == 0) break;

                    decimal sumaTodos = disponibles.Sum(x => Math.Abs(x.ImporteCsv));
                    bool todosCoinciden = Math.Abs(sumaTodos - f.Total) <= 1.0m || (f.SubTotal > 0 && Math.Abs(sumaTodos - f.SubTotal) <= 1.0m);

                    List<ItemConciliacion> grupoMatch = null;
                    bool matchSubtotalGrupo = false;
                    List<int> indicesMatch = null;
                    List<int> indicesSub = null;

                    if (todosCoinciden)
                    {
                        grupoMatch = disponibles;
                        matchSubtotalGrupo = f.SubTotal > 0 && Math.Abs(sumaTodos - f.SubTotal) <= 1.0m;
                    }
                    else if ((indicesMatch = BuscarIndicesSubconjuntoSuma(disponibles.Select(x => Math.Abs(x.ImporteCsv)).ToList(), f.Total, 1.0m)) != null && indicesMatch.Count > 1)
                    {
                        grupoMatch = indicesMatch.Select(idx => disponibles[idx]).ToList();
                    }
                    else if (f.SubTotal > 0 && (indicesSub = BuscarIndicesSubconjuntoSuma(disponibles.Select(x => Math.Abs(x.ImporteCsv)).ToList(), f.SubTotal, 1.0m)) != null && indicesSub.Count > 1)
                    {
                        grupoMatch = indicesSub.Select(idx => disponibles[idx]).ToList();
                        matchSubtotalGrupo = true;
                    }
                    else if (disponibles.Count >= 3 && Math.Abs(sumaTodos - f.Total) <= f.Total * 0.15m)
                    {
                        // Si los cargos de ese proveedor corresponden a esta factura acumulada (desfase por corte bancario < 15%)
                        grupoMatch = disponibles;
                    }

                    if (grupoMatch != null && grupoMatch.Count > 0)
                    {
                        f.Asignada = true;
                        decimal sumaGrupo = grupoMatch.Sum(x => Math.Abs(x.ImporteCsv));
                        decimal difGlobal = Math.Abs(sumaGrupo - (matchSubtotalGrupo ? f.SubTotal : f.Total));

                        foreach (var it in grupoMatch)
                        {
                            it.EstadoFactura = matchSubtotalGrupo
                                ? $"✅ FACTURA AMARRADA (PAQUETE {grupoMatch.Count} CARGOS vs SUBT)"
                                : $"✅ FACTURA AMARRADA (PAQUETE {grupoMatch.Count} CARGOS)";
                            it.TotalXml = f.Total;
                            it.SubTotalXml = f.SubTotal;
                            it.Diferencia = difGlobal;
                            it.UUID = f.UUID;
                            it.Emisor = f.EmisorNombre;
                            it.RfcXml = f.EmisorRfc;
                            it.ArchivoXml = f.NombreArchivo;
                            it.Subcarpeta = f.Subcarpeta;
                            it.RutaXml = f.RutaCompleta;
                            it.MetodoPago = f.MetodoPago;
                            it.FormaPago = f.FormaPago;
                        }
                    }
                }

                // CASO B: 1 cargo que suma N XMLs fraccionados
                var cargosRestantes = cargosPendientesRfc.Where(i => !i.EstadoFactura.StartsWith("✅")).ToList();
                var facturasRestantes = facturasXml.Where(f => !f.Asignada && string.Equals(f.EmisorRfc, rfc, StringComparison.OrdinalIgnoreCase)).ToList();

                foreach (var cargo in cargosRestantes)
                {
                    if (cargo.EstadoFactura.StartsWith("✅")) continue;
                    var fDisponibles = facturasRestantes.Where(f => !f.Asignada).ToList();
                    if (fDisponibles.Count < 2) break;

                    decimal objetivo = Math.Abs(cargo.ImporteCsv);
                    decimal sumaTodasF = fDisponibles.Sum(x => x.Total);
                    List<FacturaXmlItem> fMatch = null;
                    bool esSub = false;

                    if (Math.Abs(sumaTodasF - objetivo) <= 1.0m)
                    {
                        fMatch = fDisponibles;
                    }
                    else
                    {
                        var indices = BuscarIndicesSubconjuntoSuma(fDisponibles.Select(x => x.Total).ToList(), objetivo, 1.0m);
                        if (indices != null && indices.Count > 1)
                        {
                            fMatch = indices.Select(idx => fDisponibles[idx]).ToList();
                        }
                        else
                        {
                            var indicesSub = BuscarIndicesSubconjuntoSuma(fDisponibles.Select(x => x.SubTotal).ToList(), objetivo, 1.0m);
                            if (indicesSub != null && indicesSub.Count > 1)
                            {
                                fMatch = indicesSub.Select(idx => fDisponibles[idx]).ToList();
                                esSub = true;
                            }
                        }
                    }

                    if (fMatch != null && fMatch.Count > 0)
                    {
                        foreach (var f in fMatch) f.Asignada = true;

                        decimal totalSum = fMatch.Sum(x => x.Total);
                        decimal subSum = fMatch.Sum(x => x.SubTotal);

                        cargo.EstadoFactura = $"✅ FACTURA AMARRADA (SUMA DE {fMatch.Count} XMLs)";
                        cargo.TotalXml = totalSum;
                        cargo.SubTotalXml = subSum;
                        cargo.Diferencia = Math.Abs(objetivo - (esSub ? subSum : totalSum));
                        cargo.UUID = string.Join(" | ", fMatch.Select(x => x.UUID));
                        cargo.Emisor = fMatch[0].EmisorNombre;
                        cargo.RfcXml = fMatch[0].EmisorRfc;
                        cargo.ArchivoXml = string.Join(" | ", fMatch.Select(x => x.NombreArchivo));
                        cargo.Subcarpeta = fMatch[0].Subcarpeta;
                        cargo.RutaXml = fMatch[0].RutaCompleta;
                        cargo.MetodoPago = fMatch[0].MetodoPago;
                        cargo.FormaPago = fMatch[0].FormaPago;
                    }
                }
            }

            progreso?.Report(100);

            int coincidentes = itemsConciliados.Count(i => i.EstadoFactura.StartsWith("✅"));
            int faltantes = itemsConciliados.Count(i => i.EstadoFactura.StartsWith("❌") || i.EstadoFactura.StartsWith("⚠️"));
            int huerfanos = facturasXml.Count(f => !f.Asignada);

            return new ResultadoProceso
            {
                ItemsConciliacion = itemsConciliados,
                FacturasXml = facturasXml,
                Coincidentes = coincidentes,
                Faltantes = faltantes,
                Huerfanos = huerfanos
            };
        }

        private List<int> BuscarIndicesSubconjuntoSuma(List<decimal> valores, decimal objetivo, decimal tolerancia)
        {
            if (valores == null || valores.Count == 0 || objetivo <= 0) return null;

            if (valores.Count > 25)
            {
                if (Math.Abs(valores.Sum() - objetivo) <= tolerancia)
                {
                    return Enumerable.Range(0, valores.Count).ToList();
                }
                return null;
            }

            List<int> mejorResultado = null;

            void Backtrack(int startIndex, decimal sumaActual, List<int> seleccionados)
            {
                if (mejorResultado != null) return;

                if (seleccionados.Count > 1 && Math.Abs(sumaActual - objetivo) <= tolerancia)
                {
                    mejorResultado = new List<int>(seleccionados);
                    return;
                }

                if (sumaActual > objetivo + tolerancia) return;

                for (int i = startIndex; i < valores.Count; i++)
                {
                    seleccionados.Add(i);
                    Backtrack(i + 1, sumaActual + valores[i], seleccionados);
                    seleccionados.RemoveAt(seleccionados.Count - 1);
                    if (mejorResultado != null) return;
                }
            }

            Backtrack(0, 0m, new List<int>());
            return mejorResultado;
        }

        private int CalcularAfinidadNombre(string emisorNombre, string emisorRfc, string descripcionCsv)
        {
            if (string.IsNullOrWhiteSpace(descripcionCsv)) return 0;

            int puntaje = 0;

            // Coincidencia directa con RFC si viene en el cargo
            if (!string.IsNullOrEmpty(emisorRfc) && emisorRfc.Length >= 9)
            {
                if (descripcionCsv.IndexOf(emisorRfc, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    puntaje += 100;
                }
            }

            if (string.IsNullOrWhiteSpace(emisorNombre)) return puntaje;

            // Palabras societarias o genéricas a ignorar para evitar falsos positivos
            var palabrasIgnoradas = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "S.A.", "SA", "C.V.", "CV", "SAPI", "S.A.P.I.", "R.L.", "RL", "S.C.", "SC",
                "SOCIEDAD", "ANONIMA", "CAPITAL", "VARIABLE", "MEXICO", "DE", "LA", "EL",
                "LOS", "LAS", "DEL", "Y", "EN", "POR", "PARA", "CON", "SIN", "GRUPO",
                "SERVICIO", "SERVICIOS", "COMPRA", "CARGO", "PAGO", "CIUDAD", "CDMX"
            };

            // 1. Extraer palabras clave del Emisor del XML (>= 4 letras)
            var palabrasEmisor = emisorNombre.Split(new[] { ' ', '.', ',', '-', '/', '*', '_' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(p => p.Length >= 4 && !palabrasIgnoradas.Contains(p));

            foreach (var p in palabrasEmisor)
            {
                if (descripcionCsv.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    puntaje += 50;
                }
            }

            // 2. Extraer palabras clave de la descripción del CSV (>= 4 letras)
            var palabrasCsv = descripcionCsv.Split(new[] { ' ', '.', ',', '-', '/', '*', '_' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(p => p.Length >= 4 && !palabrasIgnoradas.Contains(p));

            foreach (var p in palabrasCsv)
            {
                if (emisorNombre.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    puntaje += 40;
                }
            }

            return puntaje;
        }

        private void AsignarFacturaAItem(ItemConciliacion item, FacturaXmlItem f, bool matchSubtotal = false)
        {
            if (matchSubtotal)
            {
                item.EstadoFactura = "✅ FACTURA ENCONTRADA (Subtotal)";
                item.Diferencia = Math.Abs(item.ImporteCsv - f.SubTotal);
            }
            else
            {
                item.EstadoFactura = "✅ FACTURA ENCONTRADA";
                item.Diferencia = Math.Abs(item.ImporteCsv - f.Total);
            }

            item.TotalXml = f.Total;
            item.SubTotalXml = f.SubTotal;
            item.UUID = f.UUID;
            item.Emisor = f.EmisorNombre;
            item.RfcXml = f.EmisorRfc;
            item.ArchivoXml = f.NombreArchivo;
            item.Subcarpeta = f.Subcarpeta;
            item.RutaXml = f.RutaCompleta;
            item.MetodoPago = f.MetodoPago;
            item.FormaPago = f.FormaPago;
        }

        private List<PdfTransaccionItem> ExtraerRfcsDePdf(string rutaPdf)
        {
            var resultado = new List<PdfTransaccionItem>();
            if (string.IsNullOrEmpty(rutaPdf) || !File.Exists(rutaPdf)) return resultado;

            string tempTsv = "";
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string scriptPath = Path.Combine(baseDir, "extraer_rfcs_pdf.py");
                if (!File.Exists(scriptPath))
                {
                    scriptPath = Path.GetFullPath(Path.Combine(baseDir, @"..\..\extraer_rfcs_pdf.py"));
                }

                if (!File.Exists(scriptPath))
                {
                    return resultado;
                }

                tempTsv = Path.Combine(Path.GetTempPath(), $"amex_rfcs_{Guid.NewGuid():N}.tsv");

                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"\"{scriptPath}\" \"{rutaPdf}\" \"{tempTsv}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };

                using (var proc = System.Diagnostics.Process.Start(psi))
                {
                    proc.WaitForExit(20000);
                }

                if (File.Exists(tempTsv))
                {
                    var lineas = File.ReadAllLines(tempTsv, Encoding.UTF8);
                    foreach (var linea in lineas)
                    {
                        if (string.IsNullOrWhiteSpace(linea)) continue;
                        var partes = linea.Split('\t');
                        if (partes.Length >= 1)
                        {
                            string rfc = partes[0].Trim().ToUpperInvariant();
                            decimal? monto = null;
                            if (partes.Length >= 2 && decimal.TryParse(partes[1], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal m))
                            {
                                monto = m;
                            }
                            string fecha = partes.Length >= 3 ? partes[2].Trim() : "";
                            string desc = partes.Length >= 4 ? partes[3].Trim() : "";

                            resultado.Add(new PdfTransaccionItem
                            {
                                Rfc = rfc,
                                Monto = monto,
                                Fecha = fecha,
                                Desc = desc
                            });
                        }
                    }
                }
            }
            catch { }
            finally
            {
                try
                {
                    if (!string.IsNullOrEmpty(tempTsv) && File.Exists(tempTsv))
                        File.Delete(tempTsv);
                }
                catch { }
            }

            return resultado;
        }

        private List<OperacionCsv> ExtraerOperacionesDesdePdf(string rutaPdf, List<PdfTransaccionItem> txsPdf)
        {
            var operaciones = new List<OperacionCsv>();
            foreach (var t in txsPdf)
            {
                if (!t.Monto.HasValue || t.Monto.Value <= 0) continue;
                operaciones.Add(new OperacionCsv
                {
                    FechaTexto = t.Fecha,
                    FechaCompraTexto = t.Fecha,
                    FechaCompraDate = ParsearFecha(t.Fecha),
                    Descripcion = t.Desc,
                    Importe = t.Monto.Value,
                    RfcBanco = t.Rfc
                });
            }
            return operaciones;
        }

        private List<OperacionCsv> ParsearCsvOperaciones(string rutaCsv)
        {
            var operaciones = new List<OperacionCsv>();
            var lineas = File.ReadAllLines(rutaCsv, Encoding.UTF8);

            int colFecha = -1, colFechaCompra = -1, colDesc = -1, colImporte = -1;
            bool cabeceraDetectada = false;

            for (int i = 0; i < lineas.Length; i++)
            {
                string linea = lineas[i].Trim();
                if (string.IsNullOrEmpty(linea)) continue;

                var partes = ParsearLineaCsv(linea);

                if (!cabeceraDetectada)
                {
                    for (int c = 0; c < partes.Count; c++)
                    {
                        string p = partes[c].Trim().ToLowerInvariant();
                        if (p.Contains("compra")) colFechaCompra = c;
                        else if (p.Contains("fecha")) colFecha = c;
                        else if (p.Contains("descripci") || p.Contains("concepto") || p.Contains("establecimiento")) colDesc = c;
                        else if (p.Contains("importe") || p.Contains("monto") || p.Contains("total")) colImporte = c;
                    }

                    if (colDesc != -1 && colImporte != -1)
                    {
                        cabeceraDetectada = true;
                        continue;
                    }

                    // Si no tiene cabeceras explícitas pero tiene 4 columnas, usar orden estándar
                    if (partes.Count >= 4 && decimal.TryParse(partes[3].Replace("$", "").Replace(",", "").Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                    {
                        colFecha = 0;
                        colFechaCompra = 1;
                        colDesc = 2;
                        colImporte = 3;
                        cabeceraDetectada = true;
                    }
                }

                if (cabeceraDetectada && partes.Count > Math.Max(colDesc, colImporte))
                {
                    string strImporte = partes[colImporte].Replace("$", "").Replace(",", "").Trim();
                    if (decimal.TryParse(strImporte, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal importe))
                    {
                        string fechaStr = colFecha >= 0 && colFecha < partes.Count ? partes[colFecha].Trim() : "";
                        string fechaCompraStr = colFechaCompra >= 0 && colFechaCompra < partes.Count ? partes[colFechaCompra].Trim() : fechaStr;
                        string descStr = colDesc >= 0 && colDesc < partes.Count ? partes[colDesc].Trim() : "";

                        DateTime? dtCompra = ParsearFecha(fechaCompraStr);

                        operaciones.Add(new OperacionCsv
                        {
                            FechaTexto = fechaStr,
                            FechaCompraTexto = fechaCompraStr,
                            FechaCompraDate = dtCompra,
                            Descripcion = descStr,
                            Importe = importe
                        });
                    }
                }
            }

            return operaciones;
        }

        private List<string> ParsearLineaCsv(string linea)
        {
            var resultado = new List<string>();
            bool enComillas = false;
            var sb = new StringBuilder();

            for (int i = 0; i < linea.Length; i++)
            {
                char c = linea[i];
                if (c == '"')
                {
                    enComillas = !enComillas;
                }
                else if (c == ',' && !enComillas)
                {
                    resultado.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }
            }
            resultado.Add(sb.ToString());
            return resultado;
        }

        private DateTime? ParsearFecha(string fechaTexto)
        {
            if (string.IsNullOrEmpty(fechaTexto)) return null;

            // Mapear meses comunes en inglés/español
            string normalizada = fechaTexto.Trim();
            string[] formatos = new[] { "dd MMM yyyy", "d MMM yyyy", "dd/MM/yyyy", "yyyy-MM-dd", "d/M/yyyy" };

            if (DateTime.TryParseExact(normalizada, formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dtInv))
            {
                return dtInv;
            }

            if (DateTime.TryParse(normalizada, new CultureInfo("es-MX"), DateTimeStyles.None, out DateTime dtEs))
            {
                return dtEs;
            }

            if (DateTime.TryParse(normalizada, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dtGen))
            {
                return dtGen;
            }

            return null;
        }

        private List<FacturaXmlItem> EscanearCarpetaXml(string rutaCarpeta)
        {
            var lista = new List<FacturaXmlItem>();
            var archivos = ObtenerTodosLosArchivosXml(rutaCarpeta);

            foreach (var archivo in archivos)
            {
                try
                {
                    var doc = new XmlDocument();
                    doc.Load(archivo);

                    XmlNode compNode = doc.SelectSingleNode("//*[local-name()='Comprobante']");
                    XmlNode tfdNode = doc.SelectSingleNode("//*[local-name()='TimbreFiscalDigital']");
                    XmlNode emisorNode = doc.SelectSingleNode("//*[local-name()='Emisor']");

                    if (compNode != null)
                    {
                        string totalStr = compNode.Attributes?["Total"]?.Value ?? "0";
                        decimal.TryParse(totalStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal total);

                        string subtotalStr = compNode.Attributes?["SubTotal"]?.Value ?? "0";
                        decimal.TryParse(subtotalStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal subtotal);

                        string uuid = tfdNode?.Attributes?["UUID"]?.Value ?? "";
                        string fechaStr = compNode.Attributes?["Fecha"]?.Value ?? "";
                        DateTime? fechaCfdi = null;
                        if (DateTime.TryParse(fechaStr, out DateTime dt)) fechaCfdi = dt;

                        string subcarpeta = Path.GetFileName(Path.GetDirectoryName(archivo)) ?? "";

                        lista.Add(new FacturaXmlItem
                        {
                            UUID = uuid,
                            Total = total,
                            SubTotal = subtotal,
                            FechaCfdi = fechaCfdi,
                            EmisorNombre = emisorNode?.Attributes?["Nombre"]?.Value ?? "",
                            EmisorRfc = emisorNode?.Attributes?["Rfc"]?.Value ?? "",
                            FormaPago = compNode.Attributes?["FormaPago"]?.Value ?? "",
                            MetodoPago = compNode.Attributes?["MetodoPago"]?.Value ?? "",
                            NombreArchivo = Path.GetFileName(archivo),
                            RutaCompleta = archivo,
                            Subcarpeta = subcarpeta
                        });
                    }
                }
                catch { }
            }

            return lista;
        }

        private List<string> ObtenerTodosLosArchivosXml(string rutaRaiz)
        {
            var resultado = new List<string>();
            if (string.IsNullOrEmpty(rutaRaiz) || !Directory.Exists(rutaRaiz))
                return resultado;

            var pila = new Stack<string>();
            pila.Push(rutaRaiz);

            while (pila.Count > 0)
            {
                string dirActual = pila.Pop();

                // 1. Obtener archivos XML de la carpeta actual con protección
                try
                {
                    var archivos = Directory.GetFiles(dirActual, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(f => f.EndsWith(".xml", StringComparison.OrdinalIgnoreCase));
                    resultado.AddRange(archivos);
                }
                catch { }

                // 2. Encolar subcarpetas para explorarlas una a una con protección
                try
                {
                    var subdirs = Directory.GetDirectories(dirActual);
                    foreach (var s in subdirs)
                    {
                        pila.Push(s);
                    }
                }
                catch { }
            }

            return resultado;
        }

        private void MostrarResultadosEnGrids()
        {
            // Métricas
            int totalCsv = _listaConciliacion.Count;
            int totalXmls = _listaFacturasXml.Count;
            int coincidentes = _listaConciliacion.Count(i => i.EstadoFactura.StartsWith("✅"));
            int faltantes = _listaConciliacion.Count(i => i.EstadoFactura.StartsWith("❌") || i.EstadoFactura.StartsWith("⚠️"));
            int huerfanos = _listaFacturasXml.Count(f => !f.Asignada);

            lblTotalCsv.Text = $"Operaciones Estado Cta: {totalCsv}";
            lblTotalXmls.Text = $"XMLs en Carpeta: {totalXmls}";
            lblCoincidentes.Text = $"✅ Facturas Amarradas: {coincidentes}";
            lblFaltantes.Text = $"❌ Faltantes / Dif. Monto: {faltantes}";
            lblHuerfanos.Text = $"⚠️ XMLs sin Cargo Banco: {huerfanos}";

            // Pestaña 1: Todas
            gridTodas.DataSource = null;
            gridTodas.DataSource = _listaConciliacion;
            ConfigurarFormatoGrid(gridTodas);

            // Pestaña 2: Faltantes (incluye Faltantes de XML y Mismo Proveedor con Monto Distinto)
            gridFaltantes.DataSource = null;
            gridFaltantes.DataSource = _listaConciliacion.Where(i => i.EstadoFactura.StartsWith("❌") || i.EstadoFactura.StartsWith("⚠️")).ToList();
            ConfigurarFormatoGrid(gridFaltantes);

            // Pestaña 3: Coincidentes
            gridCoincidentes.DataSource = null;
            gridCoincidentes.DataSource = _listaConciliacion.Where(i => i.EstadoFactura.StartsWith("✅")).ToList();
            ConfigurarFormatoGrid(gridCoincidentes);

            // Pestaña 4: Huérfanos
            gridHuerfanos.DataSource = null;
            gridHuerfanos.DataSource = _listaFacturasXml.Where(f => !f.Asignada).Select(f => new
            {
                f.Subcarpeta,
                f.NombreArchivo,
                ImporteTotal = f.Total.ToString("C2"),
                Fecha = f.FechaCfdi.HasValue ? f.FechaCfdi.Value.ToString("yyyy-MM-dd") : "",
                f.EmisorRfc,
                f.EmisorNombre,
                f.UUID,
                f.MetodoPago,
                f.FormaPago
            }).ToList();

            FormatearGridHuerfanos(gridHuerfanos);
        }

        private void ConfigurarFormatoGrid(DataGridView grid)
        {
            if (grid.Columns.Count == 0) return;

            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            if (grid.Columns["FechaCompraDate"] != null) grid.Columns["FechaCompraDate"].Visible = false;
            if (grid.Columns["RutaXml"] != null) grid.Columns["RutaXml"].Visible = false;

            if (grid.Columns["EstadoFactura"] != null)
            {
                grid.Columns["EstadoFactura"].HeaderText = "Estado Factura";
                grid.Columns["EstadoFactura"].Width = 210;
            }
            if (grid.Columns["Fecha"] != null)
            {
                grid.Columns["Fecha"].HeaderText = "Fecha Reg.";
                grid.Columns["Fecha"].Width = 95;
            }
            if (grid.Columns["FechaCompra"] != null)
            {
                grid.Columns["FechaCompra"].HeaderText = "Fecha Compra";
                grid.Columns["FechaCompra"].Width = 105;
            }
            if (grid.Columns["Descripcion"] != null)
            {
                grid.Columns["Descripcion"].HeaderText = "Descripción del Cargo (Banco)";
                grid.Columns["Descripcion"].Width = 300;
            }
            if (grid.Columns["RfcBanco"] != null)
            {
                grid.Columns["RfcBanco"].HeaderText = "RFC Banco";
                grid.Columns["RfcBanco"].Width = 120;
            }
            if (grid.Columns["ImporteCsv"] != null)
            {
                grid.Columns["ImporteCsv"].HeaderText = "Cargo Banco";
                grid.Columns["ImporteCsv"].DefaultCellStyle.Format = "C2";
                grid.Columns["ImporteCsv"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                grid.Columns["ImporteCsv"].Width = 110;
            }
            if (grid.Columns["TotalXml"] != null)
            {
                grid.Columns["TotalXml"].HeaderText = "Total XML";
                grid.Columns["TotalXml"].DefaultCellStyle.Format = "C2";
                grid.Columns["TotalXml"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                grid.Columns["TotalXml"].Width = 110;
            }
            if (grid.Columns["SubTotalXml"] != null)
            {
                grid.Columns["SubTotalXml"].HeaderText = "Subtotal XML";
                grid.Columns["SubTotalXml"].DefaultCellStyle.Format = "C2";
                grid.Columns["SubTotalXml"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                grid.Columns["SubTotalXml"].Width = 110;
            }
            if (grid.Columns["Diferencia"] != null)
            {
                grid.Columns["Diferencia"].HeaderText = "Diferencia";
                grid.Columns["Diferencia"].DefaultCellStyle.Format = "C2";
                grid.Columns["Diferencia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                grid.Columns["Diferencia"].Width = 90;
            }
            if (grid.Columns["RfcXml"] != null)
            {
                grid.Columns["RfcXml"].HeaderText = "RFC XML";
                grid.Columns["RfcXml"].Width = 120;
            }
            if (grid.Columns["Emisor"] != null)
            {
                grid.Columns["Emisor"].HeaderText = "Proveedor (Emisor XML)";
                grid.Columns["Emisor"].Width = 260;
            }
            if (grid.Columns["UUID"] != null)
            {
                grid.Columns["UUID"].HeaderText = "UUID (Folio Fiscal)";
                grid.Columns["UUID"].Width = 280;
            }
            if (grid.Columns["ArchivoXml"] != null)
            {
                grid.Columns["ArchivoXml"].HeaderText = "Archivo XML";
                grid.Columns["ArchivoXml"].Width = 220;
            }
            if (grid.Columns["Subcarpeta"] != null)
            {
                grid.Columns["Subcarpeta"].HeaderText = "Subcarpeta";
                grid.Columns["Subcarpeta"].Width = 110;
            }

            // Colorear filas por estado
            foreach (DataGridViewRow row in grid.Rows)
            {
                string estado = row.Cells["EstadoFactura"]?.Value?.ToString() ?? "";
                if (estado.StartsWith("✅"))
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(240, 253, 244); // Verde suave
                    row.Cells["EstadoFactura"].Style.ForeColor = Color.DarkGreen;
                    row.Cells["EstadoFactura"].Style.Font = new Font(grid.Font, FontStyle.Bold);
                }
                else if (estado.StartsWith("⚠️"))
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(254, 252, 232); // Amarillo / Ámbar suave
                    row.Cells["EstadoFactura"].Style.ForeColor = Color.FromArgb(180, 83, 9);
                    row.Cells["EstadoFactura"].Style.Font = new Font(grid.Font, FontStyle.Bold);
                }
                else if (estado.StartsWith("❌"))
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(254, 242, 242); // Rojo suave
                    row.Cells["EstadoFactura"].Style.ForeColor = Color.Crimson;
                    row.Cells["EstadoFactura"].Style.Font = new Font(grid.Font, FontStyle.Bold);
                }
            }
        }

        private void FormatearGridHuerfanos(DataGridView grid)
        {
            if (grid.Columns.Count == 0) return;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            if (grid.Columns["Subcarpeta"] != null) grid.Columns["Subcarpeta"].Width = 110;
            if (grid.Columns["NombreArchivo"] != null) grid.Columns["NombreArchivo"].Width = 230;
            if (grid.Columns["ImporteTotal"] != null)
            {
                grid.Columns["ImporteTotal"].Width = 120;
                grid.Columns["ImporteTotal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            if (grid.Columns["Fecha"] != null) grid.Columns["Fecha"].Width = 100;
            if (grid.Columns["EmisorRfc"] != null) grid.Columns["EmisorRfc"].Width = 130;
            if (grid.Columns["EmisorNombre"] != null) grid.Columns["EmisorNombre"].Width = 280;
            if (grid.Columns["UUID"] != null) grid.Columns["UUID"].Width = 280;
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            if (_listaConciliacion == null || _listaConciliacion.Count == 0)
            {
                MessageBox.Show("No hay datos de conciliación para exportar. Ejecute una conciliación primero.", "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Archivo CSV (*.csv)|*.csv";
                sfd.FileName = $"Reporte_Conciliacion_EstadoCuenta_{DateTime.Now:yyyyMMdd_HHmm}.csv";
                sfd.Title = "Guardar Reporte de Conciliación";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine("Estado Factura,Fecha Registro,Fecha Compra,Descripción Banco,RFC Banco,Cargo Banco,Total XML,Subtotal XML,Diferencia,RFC XML,Emisor XML,UUID,Archivo XML,Subcarpeta");

                        foreach (var i in _listaConciliacion)
                        {
                            sb.AppendLine(
                                $"\"{EscaparCsv(i.EstadoFactura)}\"," +
                                $"\"{EscaparCsv(i.Fecha)}\"," +
                                $"\"{EscaparCsv(i.FechaCompra)}\"," +
                                $"\"{EscaparCsv(i.Descripcion)}\"," +
                                $"\"{EscaparCsv(i.RfcBanco)}\"," +
                                $"{i.ImporteCsv.ToString(CultureInfo.InvariantCulture)}," +
                                $"{(i.TotalXml.HasValue ? i.TotalXml.Value.ToString(CultureInfo.InvariantCulture) : "")}," +
                                $"{(i.SubTotalXml.HasValue ? i.SubTotalXml.Value.ToString(CultureInfo.InvariantCulture) : "")}," +
                                $"{(i.Diferencia.HasValue ? i.Diferencia.Value.ToString(CultureInfo.InvariantCulture) : "")}," +
                                $"\"{EscaparCsv(i.RfcXml)}\"," +
                                $"\"{EscaparCsv(i.Emisor)}\"," +
                                $"\"{EscaparCsv(i.UUID)}\"," +
                                $"\"{EscaparCsv(i.ArchivoXml)}\"," +
                                $"\"{EscaparCsv(i.Subcarpeta)}\""
                            );
                        }

                        File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                        MessageBox.Show("¡Reporte exportado exitosamente!", "Exportación Completada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al exportar reporte: {ex.Message}", "Error de Exportación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private string EscaparCsv(string campo)
        {
            if (string.IsNullOrEmpty(campo)) return "";
            return campo.Replace("\"", "\"\"");
        }
    }

    public class PdfTransaccionItem
    {
        public string Rfc { get; set; } = "";
        public decimal? Monto { get; set; }
        public string Fecha { get; set; } = "";
        public string Desc { get; set; } = "";
        public bool Usado { get; set; }
    }

    public class OperacionCsv
    {
        public string FechaTexto { get; set; } = "";
        public string FechaCompraTexto { get; set; } = "";
        public DateTime? FechaCompraDate { get; set; }
        public string Descripcion { get; set; } = "";
        public decimal Importe { get; set; }
        public string RfcBanco { get; set; } = "";
    }

    public class FacturaXmlItem
    {
        public string UUID { get; set; } = "";
        public decimal Total { get; set; }
        public decimal SubTotal { get; set; }
        public DateTime? FechaCfdi { get; set; }
        public string EmisorRfc { get; set; } = "";
        public string EmisorNombre { get; set; } = "";
        public string FormaPago { get; set; } = "";
        public string MetodoPago { get; set; } = "";
        public string NombreArchivo { get; set; } = "";
        public string RutaCompleta { get; set; } = "";
        public string Subcarpeta { get; set; } = "";
        public bool Asignada { get; set; }
    }

    public class ItemConciliacion
    {
        public string EstadoFactura { get; set; } = "";
        public string Fecha { get; set; } = "";
        public string FechaCompra { get; set; } = "";
        public DateTime? FechaCompraDate { get; set; }
        public string Descripcion { get; set; } = "";
        public decimal ImporteCsv { get; set; }
        public string RfcBanco { get; set; } = "";
        public decimal? TotalXml { get; set; }
        public decimal? SubTotalXml { get; set; }
        public decimal? Diferencia { get; set; }
        public string RfcXml { get; set; } = "";
        public string Emisor { get; set; } = "";
        public string UUID { get; set; } = "";
        public string ArchivoXml { get; set; } = "";
        public string Subcarpeta { get; set; } = "";
        public string RutaXml { get; set; } = "";
        public string MetodoPago { get; set; } = "";
        public string FormaPago { get; set; } = "";
    }

    public class ResultadoProceso
    {
        public List<ItemConciliacion> ItemsConciliacion { get; set; }
        public List<FacturaXmlItem> FacturasXml { get; set; }
        public int Coincidentes { get; set; }
        public int Faltantes { get; set; }
        public int Huerfanos { get; set; }
    }
}
