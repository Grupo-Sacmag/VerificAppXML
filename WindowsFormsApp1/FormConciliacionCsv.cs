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
            TemaGamerEmpresarial.AplicarTema(this);
            ConfigurarMenuContextualGrids();

            var menuGuardarFacturas = new ToolStripMenuItem("📁 Guardar Facturas Amarradas en Carpeta...", null, (s, e) => GuardarFacturasAmarradasEnCarpeta());
            menuArchivo.DropDownItems.Insert(1, menuGuardarFacturas);
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

            int numOp = 1;
            foreach (var op in operaciones)
            {
                var item = new ItemConciliacion
                {
                    NoOperacion = numOp++,
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
                    decimal dif = Math.Abs(item.ImporteCsv - facturaMismoRfcDistintoMonto.Total);
                    if (dif >= 500m)
                    {
                        // Si es mismo proveedor pero la diferencia es >= $500 (tache X), se desvincula directamente la factura
                        item.EstadoFactura = "❌ FALTA XML";
                    }
                    else
                    {
                        item.EstadoFactura = "⚠️ MISMO PROVEEDOR (MONTO DISTINTO)";
                        item.Diferencia = dif;
                        item.RfcXml = facturaMismoRfcDistintoMonto.EmisorRfc;
                        item.Emisor = facturaMismoRfcDistintoMonto.EmisorNombre;
                    item.TotalXml = facturaMismoRfcDistintoMonto.Total;
                    item.SubTotalXml = facturaMismoRfcDistintoMonto.SubTotal;
                    item.UUID = facturaMismoRfcDistintoMonto.UUID;
                    item.ArchivoXml = facturaMismoRfcDistintoMonto.NombreArchivo;
                    item.Subcarpeta = facturaMismoRfcDistintoMonto.Subcarpeta;
                    item.RutaXml = facturaMismoRfcDistintoMonto.RutaCompleta;
                    item.Serie = facturaMismoRfcDistintoMonto.Serie;
                    item.Folio = facturaMismoRfcDistintoMonto.Folio;
                    item.Moneda = facturaMismoRfcDistintoMonto.Moneda;
                    item.TipoComprobante = facturaMismoRfcDistintoMonto.TipoComprobante;
                    item.UsoCfdi = facturaMismoRfcDistintoMonto.UsoCfdi;
                    item.VersionCfdi = facturaMismoRfcDistintoMonto.VersionCfdi;
                    item.FechaXml = facturaMismoRfcDistintoMonto.FechaCfdi.HasValue ? facturaMismoRfcDistintoMonto.FechaCfdi.Value.ToString("yyyy-MM-dd") : "";
                    item.ConceptosDescripcion = facturaMismoRfcDistintoMonto.ConceptosDescripcion;
                    item.ClavesSat = facturaMismoRfcDistintoMonto.ClavesSat;
                    item.Descuento = facturaMismoRfcDistintoMonto.Descuento;
                    item.IvaTrasladado = facturaMismoRfcDistintoMonto.IvaTrasladado;
                    item.IepsTrasladado = facturaMismoRfcDistintoMonto.IepsTrasladado;
                    item.RetencionIva = facturaMismoRfcDistintoMonto.RetencionIva;
                    item.RetencionIsr = facturaMismoRfcDistintoMonto.RetencionIsr;
                    item.ImpuestosLocales = facturaMismoRfcDistintoMonto.ImpuestosLocales;
                    item.Propina = facturaMismoRfcDistintoMonto.Propina;
                    item.EmisorRegimen = facturaMismoRfcDistintoMonto.EmisorRegimen;
                    item.ReceptorRfc = facturaMismoRfcDistintoMonto.ReceptorRfc;
                    item.ReceptorNombre = facturaMismoRfcDistintoMonto.ReceptorNombre;
                    item.ReceptorRegimen = facturaMismoRfcDistintoMonto.ReceptorRegimen;
                    item.ReceptorCp = facturaMismoRfcDistintoMonto.ReceptorCp;
                    item.LugarExpedicion = facturaMismoRfcDistintoMonto.LugarExpedicion;
                    item.TipoCambio = facturaMismoRfcDistintoMonto.TipoCambio;
                    item.FechaTimbrado = facturaMismoRfcDistintoMonto.FechaTimbrado;
                    item.RfcProvCertif = facturaMismoRfcDistintoMonto.RfcProvCertif;
                    }
                }
                else
                {
                    item.EstadoFactura = "❌ FALTA XML";
                }

                itemsConciliados.Add(item);
            }

            // -------------------------------------------------------------
            // NIVEL 4: CONCILIACIÓN AGRUPADA POR PROVEEDOR (RFC)
            // Se mantiene deshabilitado para evitar falsos positivos y agrupaciones no deseadas
            // (ej. combinar compras independientes de vuelos o cargos con el mismo RFC).
            // La agrupación en paquete se realiza de forma manual y controlada desde el formulario de detalle.
            // -------------------------------------------------------------
            bool habilitarEmpaquetadoAutomatico = false;
            if (habilitarEmpaquetadoAutomatico)
            {
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
                        decimal sumaGrupo = grupoMatch.Sum(x => Math.Abs(x.ImporteCsv));
                        decimal difGlobal = Math.Abs(sumaGrupo - (matchSubtotalGrupo ? f.SubTotal : f.Total));

                        if (difGlobal >= 500m)
                        {
                            // Si la diferencia del paquete es >= $500, se desvincula directamente
                            continue;
                        }

                        f.Asignada = true;

                        foreach (var it in grupoMatch)
                        {
                            it.EstadoFactura = matchSubtotalGrupo
                                ? $"✅ FACTURA AMARRADA (PAQUETE {grupoMatch.Count} CARGOS vs SUBT)"
                                : $"✅ FACTURA AMARRADA (PAQUETE {grupoMatch.Count} CARGOS)";
                            it.NumeroProyecto = ExtraerNumeroProyecto(f.NombreArchivo);
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
                            it.Serie = f.Serie;
                            it.Folio = f.Folio;
                            it.Moneda = f.Moneda;
                            it.TipoComprobante = f.TipoComprobante;
                            it.UsoCfdi = f.UsoCfdi;
                            it.VersionCfdi = f.VersionCfdi;
                            it.FechaXml = f.FechaCfdi.HasValue ? f.FechaCfdi.Value.ToString("yyyy-MM-dd") : "";
                            it.ConceptosDescripcion = f.ConceptosDescripcion;
                            it.ClavesSat = f.ClavesSat;
                            it.Descuento = f.Descuento;
                            it.IvaTrasladado = f.IvaTrasladado;
                            it.IepsTrasladado = f.IepsTrasladado;
                            it.RetencionIva = f.RetencionIva;
                            it.RetencionIsr = f.RetencionIsr;
                            it.ImpuestosLocales = f.ImpuestosLocales;
                            it.Propina = f.Propina;
                            it.EmisorRegimen = f.EmisorRegimen;
                            it.ReceptorRfc = f.ReceptorRfc;
                            it.ReceptorNombre = f.ReceptorNombre;
                            it.ReceptorRegimen = f.ReceptorRegimen;
                            it.ReceptorCp = f.ReceptorCp;
                            it.LugarExpedicion = f.LugarExpedicion;
                            it.TipoCambio = f.TipoCambio;
                            it.FechaTimbrado = f.FechaTimbrado;
                            it.RfcProvCertif = f.RfcProvCertif;

                            if (!string.IsNullOrEmpty(f.RutaCompleta))
                            {
                                string homonimoPdf = Path.ChangeExtension(f.RutaCompleta, ".pdf");
                                if (File.Exists(homonimoPdf))
                                {
                                    it.RutaPdf = homonimoPdf;
                                    it.ArchivoPdf = Path.GetFileName(homonimoPdf);
                                }
                            }
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
                        cargo.NumeroProyecto = ExtraerNumeroProyecto(fMatch[0].NombreArchivo);
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
                        cargo.Serie = fMatch[0].Serie;
                        cargo.Folio = fMatch[0].Folio;
                        cargo.Moneda = fMatch[0].Moneda;
                        cargo.TipoComprobante = fMatch[0].TipoComprobante;
                        cargo.UsoCfdi = fMatch[0].UsoCfdi;
                        cargo.VersionCfdi = fMatch[0].VersionCfdi;
                        cargo.FechaXml = fMatch[0].FechaCfdi.HasValue ? fMatch[0].FechaCfdi.Value.ToString("yyyy-MM-dd") : "";
                        cargo.ConceptosDescripcion = string.Join(" | ", fMatch.Where(x => !string.IsNullOrEmpty(x.ConceptosDescripcion)).Select(x => x.ConceptosDescripcion));
                        cargo.ClavesSat = string.Join(", ", fMatch.Where(x => !string.IsNullOrEmpty(x.ClavesSat)).Select(x => x.ClavesSat).Distinct());
                        cargo.Descuento = fMatch.Any(x => x.Descuento.HasValue) ? fMatch.Sum(x => x.Descuento ?? 0) : (decimal?)null;
                        cargo.IvaTrasladado = fMatch.Any(x => x.IvaTrasladado.HasValue) ? fMatch.Sum(x => x.IvaTrasladado ?? 0) : (decimal?)null;
                        cargo.IepsTrasladado = fMatch.Any(x => x.IepsTrasladado.HasValue) ? fMatch.Sum(x => x.IepsTrasladado ?? 0) : (decimal?)null;
                        cargo.RetencionIva = fMatch.Any(x => x.RetencionIva.HasValue) ? fMatch.Sum(x => x.RetencionIva ?? 0) : (decimal?)null;
                        cargo.RetencionIsr = fMatch.Any(x => x.RetencionIsr.HasValue) ? fMatch.Sum(x => x.RetencionIsr ?? 0) : (decimal?)null;
                        cargo.ImpuestosLocales = fMatch.Any(x => x.ImpuestosLocales.HasValue) ? fMatch.Sum(x => x.ImpuestosLocales ?? 0) : (decimal?)null;
                        cargo.Propina = fMatch.Any(x => x.Propina.HasValue) ? fMatch.Sum(x => x.Propina ?? 0) : (decimal?)null;
                        cargo.EmisorRegimen = fMatch[0].EmisorRegimen;
                        cargo.ReceptorRfc = fMatch[0].ReceptorRfc;
                        cargo.ReceptorNombre = fMatch[0].ReceptorNombre;
                        cargo.ReceptorRegimen = fMatch[0].ReceptorRegimen;
                        cargo.ReceptorCp = fMatch[0].ReceptorCp;
                        cargo.LugarExpedicion = fMatch[0].LugarExpedicion;
                        cargo.TipoCambio = fMatch[0].TipoCambio;
                        cargo.FechaTimbrado = fMatch[0].FechaTimbrado;
                        cargo.RfcProvCertif = fMatch[0].RfcProvCertif;

                        if (!string.IsNullOrEmpty(fMatch[0].RutaCompleta))
                        {
                            string homonimoPdf = Path.ChangeExtension(fMatch[0].RutaCompleta, ".pdf");
                            if (File.Exists(homonimoPdf))
                            {
                                cargo.RutaPdf = homonimoPdf;
                                cargo.ArchivoPdf = Path.GetFileName(homonimoPdf);
                            }
                        }
                    }
                }
            }
            } // fin if (habilitarEmpaquetadoAutomatico)

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

            item.NumeroProyecto = ExtraerNumeroProyecto(f.NombreArchivo);
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
            item.Serie = f.Serie;
            item.Folio = f.Folio;
            item.Moneda = f.Moneda;
            item.TipoComprobante = f.TipoComprobante;
            item.UsoCfdi = f.UsoCfdi;
            item.VersionCfdi = f.VersionCfdi;
            item.FechaXml = f.FechaCfdi.HasValue ? f.FechaCfdi.Value.ToString("yyyy-MM-dd") : "";

            item.ConceptosDescripcion = f.ConceptosDescripcion;
            item.ClavesSat = f.ClavesSat;
            item.Descuento = f.Descuento;
            item.IvaTrasladado = f.IvaTrasladado;
            item.IepsTrasladado = f.IepsTrasladado;
            item.RetencionIva = f.RetencionIva;
            item.RetencionIsr = f.RetencionIsr;
            item.ImpuestosLocales = f.ImpuestosLocales;
            item.EmisorRegimen = f.EmisorRegimen;
            item.ReceptorRfc = f.ReceptorRfc;
            item.ReceptorNombre = f.ReceptorNombre;
            item.ReceptorRegimen = f.ReceptorRegimen;
            item.ReceptorCp = f.ReceptorCp;
            item.LugarExpedicion = f.LugarExpedicion;
            item.TipoCambio = f.TipoCambio;
            item.FechaTimbrado = f.FechaTimbrado;
            item.RfcProvCertif = f.RfcProvCertif;

            if (f.Propina.HasValue && f.Propina.Value > 0)
            {
                item.Propina = f.Propina.Value;
            }
            else if (item.ImporteCsv > f.Total && f.Total > 0)
            {
                decimal diff = item.ImporteCsv - f.Total;
                decimal pct = diff / f.Total;
                if (pct >= 0.04m && pct <= 0.35m)
                {
                    item.Propina = Math.Round(diff, 2);
                }
            }

            if (!string.IsNullOrEmpty(f.RutaCompleta))
            {
                string homonimoPdf = Path.ChangeExtension(f.RutaCompleta, ".pdf");
                if (File.Exists(homonimoPdf))
                {
                    item.RutaPdf = homonimoPdf;
                    item.ArchivoPdf = Path.GetFileName(homonimoPdf);
                }
            }
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
                if (!t.Monto.HasValue || t.Monto.Value == 0) continue;
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
                    string testRaw = partes[3].Trim();
                    string testLimpio = testRaw.Replace("CR", "").Replace("cr", "").Replace("$", "").Replace(",", "").Trim();
                    if (partes.Count >= 4 && decimal.TryParse(testLimpio, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
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
                    string rawImporte = partes[colImporte].Trim();
                    bool esCredito = rawImporte.EndsWith("CR", StringComparison.OrdinalIgnoreCase) ||
                                     rawImporte.StartsWith("CR", StringComparison.OrdinalIgnoreCase);
                    string strImporte = rawImporte.Replace("CR", "").Replace("cr", "").Replace("$", "").Replace(",", "").Trim();
                    if (decimal.TryParse(strImporte, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal importe))
                    {
                        if (esCredito) importe = -Math.Abs(importe);
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

        public static string ExtraerNumeroProyecto(string nombreArchivo)
        {
            if (string.IsNullOrWhiteSpace(nombreArchivo)) return "";
            string limpio = Path.GetFileNameWithoutExtension(nombreArchivo).Trim();

            // Debe comenzar con al menos 2 dígitos
            var match = Regex.Match(limpio, @"^(\d{2,})");
            if (!match.Success) return "";

            string digitos = match.Groups[1].Value;
            int cantDigitos = digitos.Length;

            // Regla: si son 2 numeros y una letra o 2 y 2 no jala (ej. 12A, 12AB)
            if (cantDigitos == 2 && limpio.Length > 2 && char.IsLetter(limpio[2]))
            {
                return "";
            }

            // Regla: si son 5 numeros solo lee los 4 y ya
            if (cantDigitos >= 4)
            {
                return digitos.Substring(0, 4);
            }

            return digitos;
        }

        public static FacturaXmlItem ParsearFacturaXml(string archivo)
        {
            if (string.IsNullOrWhiteSpace(archivo) || !File.Exists(archivo)) return null;
            try
            {
                var doc = new XmlDocument();
                doc.Load(archivo);

                XmlNode compNode = doc.SelectSingleNode("//*[local-name()='Comprobante']");
                XmlNode tfdNode = doc.SelectSingleNode("//*[local-name()='TimbreFiscalDigital']");
                XmlNode emisorNode = doc.SelectSingleNode("//*[local-name()='Emisor']");
                XmlNode receptorNode = doc.SelectSingleNode("//*[local-name()='Receptor']");

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
                    string nombreArchivo = Path.GetFileName(archivo);

                    string descuentoStr = compNode.Attributes?["Descuento"]?.Value;
                    decimal? descuento = null;
                    if (!string.IsNullOrEmpty(descuentoStr) && decimal.TryParse(descuentoStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal dsc))
                        descuento = dsc;

                    string lugarExp = compNode.Attributes?["LugarExpedicion"]?.Value ?? "";
                    string tipoCambio = compNode.Attributes?["TipoCambio"]?.Value ?? "";
                    string regimenEmisor = emisorNode?.Attributes?["RegimenFiscal"]?.Value ?? "";
                    string receptorRfc = receptorNode?.Attributes?["Rfc"]?.Value ?? "";
                    string receptorNombre = receptorNode?.Attributes?["Nombre"]?.Value ?? "";
                    string receptorRegimen = receptorNode?.Attributes?["RegimenFiscalReceptor"]?.Value ?? "";
                    string receptorCp = receptorNode?.Attributes?["DomicilioFiscalReceptor"]?.Value ?? "";
                    string fechaTimbrado = tfdNode?.Attributes?["FechaTimbrado"]?.Value ?? "";
                    string rfcProvCertif = tfdNode?.Attributes?["RfcProvCertif"]?.Value ?? "";

                    // Conceptos, tipo de cosa y propina
                    var listaDescripciones = new List<string>();
                    var listaClavesSat = new List<string>();
                    decimal propinaDetectada = 0m;
                    bool tienePropina = false;

                    var nodosConceptos = doc.SelectNodes("//*[local-name()='Concepto']");
                    if (nodosConceptos != null)
                    {
                        foreach (XmlNode conc in nodosConceptos)
                        {
                            string desc = conc.Attributes?["Descripcion"]?.Value?.Trim() ?? "";
                            string clave = conc.Attributes?["ClaveProdServ"]?.Value?.Trim() ?? "";
                            string impConcStr = conc.Attributes?["Importe"]?.Value ?? "0";
                            decimal.TryParse(impConcStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal impConc);

                            if (!string.IsNullOrEmpty(desc))
                            {
                                listaDescripciones.Add(desc);

                                if (desc.IndexOf("propina", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    desc.IndexOf("tip", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    desc.IndexOf("gratificacion", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    desc.IndexOf("cargo por servicio", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    propinaDetectada += impConc;
                                    tienePropina = true;
                                }
                            }

                            if (!string.IsNullOrEmpty(clave) && !listaClavesSat.Contains(clave))
                            {
                                listaClavesSat.Add(clave);
                            }
                        }
                    }

                    // Impuestos Trasladados y Retenidos
                    decimal totalIvaTrasladado = 0m;
                    decimal totalIepsTrasladado = 0m;
                    decimal totalRetIva = 0m;
                    decimal totalRetIsr = 0m;

                    var nodosTraslados = doc.SelectNodes("//*[local-name()='Comprobante']/*[local-name()='Impuestos']/*[local-name()='Traslados']/*[local-name()='Traslado']");
                    if (nodosTraslados == null || nodosTraslados.Count == 0)
                    {
                        nodosTraslados = doc.SelectNodes("//*[local-name()='Conceptos']/*[local-name()='Concepto']/*[local-name()='Impuestos']/*[local-name()='Traslados']/*[local-name()='Traslado']");
                    }

                    if (nodosTraslados != null)
                    {
                        foreach (XmlNode tras in nodosTraslados)
                        {
                            string impTipo = tras.Attributes?["Impuesto"]?.Value ?? "";
                            string importeStr = tras.Attributes?["Importe"]?.Value ?? "0";
                            if (decimal.TryParse(importeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal imp))
                            {
                                if (impTipo == "002") totalIvaTrasladado += imp;
                                else if (impTipo == "003") totalIepsTrasladado += imp;
                            }
                        }
                    }

                    var nodosRetenciones = doc.SelectNodes("//*[local-name()='Comprobante']/*[local-name()='Impuestos']/*[local-name()='Retenciones']/*[local-name()='Retencion']");
                    if (nodosRetenciones == null || nodosRetenciones.Count == 0)
                    {
                        nodosRetenciones = doc.SelectNodes("//*[local-name()='Conceptos']/*[local-name()='Concepto']/*[local-name()='Impuestos']/*[local-name()='Retenciones']/*[local-name()='Retencion']");
                    }

                    if (nodosRetenciones != null)
                    {
                        foreach (XmlNode ret in nodosRetenciones)
                        {
                            string impTipo = ret.Attributes?["Impuesto"]?.Value ?? "";
                            string importeStr = ret.Attributes?["Importe"]?.Value ?? "0";
                            if (decimal.TryParse(importeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal imp))
                            {
                                if (impTipo == "001") totalRetIsr += imp;
                                else if (impTipo == "002") totalRetIva += imp;
                            }
                        }
                    }

                    decimal totalImpLocales = 0m;
                    var nodosLocales = doc.SelectNodes("//*[local-name()='ImpuestosLocales']/*[local-name()='TrasladosLocales']");
                    if (nodosLocales != null)
                    {
                        foreach (XmlNode loc in nodosLocales)
                        {
                            string impStr = loc.Attributes?["Importe"]?.Value ?? "0";
                            if (decimal.TryParse(impStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal imp))
                            {
                                totalImpLocales += imp;
                            }
                        }
                    }

                    if (!tienePropina)
                    {
                        var nodoPropina = doc.SelectSingleNode("//*[local-name()='Propina' or local-name()='Tip' or local-name()='Gratificacion']");
                        if (nodoPropina != null)
                        {
                            string valPropina = nodoPropina.Attributes?["Importe"]?.Value ?? nodoPropina.InnerText?.Trim();
                            if (!string.IsNullOrEmpty(valPropina) && decimal.TryParse(valPropina.Replace("$", "").Replace(",", "").Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valP) && valP > 0)
                            {
                                propinaDetectada = valP;
                                tienePropina = true;
                            }
                        }
                    }

                    return new FacturaXmlItem
                    {
                        UUID = uuid,
                        Total = total,
                        SubTotal = subtotal,
                        FechaCfdi = fechaCfdi,
                        EmisorNombre = emisorNode?.Attributes?["Nombre"]?.Value ?? "",
                        EmisorRfc = emisorNode?.Attributes?["Rfc"]?.Value ?? "",
                        FormaPago = compNode.Attributes?["FormaPago"]?.Value ?? "",
                        MetodoPago = compNode.Attributes?["MetodoPago"]?.Value ?? "",
                        Serie = compNode.Attributes?["Serie"]?.Value ?? "",
                        Folio = compNode.Attributes?["Folio"]?.Value ?? "",
                        Moneda = compNode.Attributes?["Moneda"]?.Value ?? "",
                        TipoComprobante = compNode.Attributes?["TipoDeComprobante"]?.Value ?? "",
                        UsoCfdi = receptorNode?.Attributes?["UsoCFDI"]?.Value ?? "",
                        VersionCfdi = compNode.Attributes?["Version"]?.Value ?? "",
                        NombreArchivo = nombreArchivo,
                        RutaCompleta = archivo,
                        Subcarpeta = subcarpeta,
                        NumeroProyecto = ExtraerNumeroProyecto(nombreArchivo),
                        ConceptosDescripcion = string.Join(" | ", listaDescripciones),
                        ClavesSat = string.Join(", ", listaClavesSat),
                        Descuento = descuento,
                        IvaTrasladado = totalIvaTrasladado > 0 ? totalIvaTrasladado : (decimal?)null,
                        IepsTrasladado = totalIepsTrasladado > 0 ? totalIepsTrasladado : (decimal?)null,
                        RetencionIva = totalRetIva > 0 ? totalRetIva : (decimal?)null,
                        RetencionIsr = totalRetIsr > 0 ? totalRetIsr : (decimal?)null,
                        ImpuestosLocales = totalImpLocales > 0 ? totalImpLocales : (decimal?)null,
                        Propina = tienePropina && propinaDetectada > 0 ? propinaDetectada : (decimal?)null,
                        EmisorRegimen = regimenEmisor,
                        ReceptorRfc = receptorRfc,
                        ReceptorNombre = receptorNombre,
                        ReceptorRegimen = receptorRegimen,
                        ReceptorCp = receptorCp,
                        LugarExpedicion = lugarExp,
                        TipoCambio = tipoCambio,
                        FechaTimbrado = fechaTimbrado,
                        RfcProvCertif = rfcProvCertif
                    };
                }
            }
            catch { }
            return null;
        }

        private List<FacturaXmlItem> EscanearCarpetaXml(string rutaCarpeta)
        {
            var lista = new List<FacturaXmlItem>();
            var archivos = ObtenerTodosLosArchivosXml(rutaCarpeta);

            foreach (var archivo in archivos)
            {
                var item = ParsearFacturaXml(archivo);
                if (item != null)
                {
                    lista.Add(item);
                }
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

            lblTotalCsv.ForeColor = TemaGamerEmpresarial.TextoSecundario;
            lblTotalXmls.ForeColor = TemaGamerEmpresarial.TextoSecundario;
            lblCoincidentes.ForeColor = TemaGamerEmpresarial.VerdeNeon;
            lblFaltantes.ForeColor = TemaGamerEmpresarial.RojoLaser;
            lblHuerfanos.ForeColor = TemaGamerEmpresarial.AmbarNeon;

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
                f.NumeroProyecto,
                Total = f.Total.ToString("C2"),
                Subtotal = f.SubTotal.ToString("C2"),
                Descuento = (f.Descuento ?? 0).ToString("C2"),
                IVA = (f.IvaTrasladado ?? 0).ToString("C2"),
                IEPS = (f.IepsTrasladado ?? 0).ToString("C2"),
                RetIVA = (f.RetencionIva ?? 0).ToString("C2"),
                RetISR = (f.RetencionIsr ?? 0).ToString("C2"),
                ImpLocales = (f.ImpuestosLocales ?? 0).ToString("C2"),
                Propina = (f.Propina ?? 0).ToString("C2"),
                Conceptos = f.ConceptosDescripcion,
                ClavesSAT = f.ClavesSat,
                Fecha = f.FechaCfdi.HasValue ? f.FechaCfdi.Value.ToString("yyyy-MM-dd") : "",
                f.EmisorRfc,
                f.EmisorNombre,
                f.EmisorRegimen,
                f.ReceptorRfc,
                f.ReceptorNombre,
                f.ReceptorRegimen,
                f.ReceptorCp,
                f.UsoCfdi,
                f.Serie,
                f.Folio,
                f.TipoComprobante,
                f.MetodoPago,
                f.FormaPago,
                f.Moneda,
                f.TipoCambio,
                f.LugarExpedicion,
                f.UUID,
                f.FechaTimbrado,
                f.RfcProvCertif
            }).ToList();

            FormatearGridHuerfanos(gridHuerfanos);
        }

        private void ConfigurarFormatoGrid(DataGridView grid)
        {
            if (grid.Columns.Count == 0) return;

            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            // Helper seguro para acceder a columnas
            Action<string, Action<DataGridViewColumn>> ConfigureIfExists = (colName, configure) =>
            {
                if (grid.Columns.Contains(colName))
                {
                    configure(grid.Columns[colName]);
                }
            };

            ConfigureIfExists("FechaCompraDate", col => col.Visible = false);
            ConfigureIfExists("RutaXml", col => col.Visible = false);
            ConfigureIfExists("RutaPdf", col => col.Visible = false);
            ConfigureIfExists("ArchivoPdf", col => col.Visible = false);
            ConfigureIfExists("PdfAsociadoManual", col => col.Visible = false);

            ConfigureIfExists("NoOperacion", col =>
            {
                col.HeaderText = "# Op";
                col.Width = 55;
                col.DisplayIndex = 0;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            });

            ConfigureIfExists("EstadoFactura", col =>
            {
                col.HeaderText = "Estado Factura";
                col.Width = 210;
                col.DisplayIndex = 1;
            });

            ConfigureIfExists("NumeroProyecto", col =>
            {
                col.HeaderText = "No. Proyecto";
                col.Width = 100;
                col.DisplayIndex = 2;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            });

            ConfigureIfExists("Fecha", col =>
            {
                col.HeaderText = "Fecha Reg. (Estado)";
                col.Width = 115;
            });

            ConfigureIfExists("FechaCompra", col =>
            {
                col.HeaderText = "Fecha Compra (Estado)";
                col.Width = 120;
            });

            ConfigureIfExists("Descripcion", col =>
            {
                col.HeaderText = "Descripción del Cargo (Banco)";
                col.Width = 260;
            });

            ConfigureIfExists("RfcBanco", col =>
            {
                col.HeaderText = "RFC Banco";
                col.Width = 115;
            });

            ConfigureIfExists("ImporteCsv", col =>
            {
                col.HeaderText = "Cargo Banco";
                col.DefaultCellStyle.Format = "C2";
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                col.Width = 105;
            });

            ConfigureIfExists("TotalXml", col =>
            {
                col.HeaderText = "Total XML";
                col.DefaultCellStyle.Format = "C2";
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                col.Width = 105;
            });

            ConfigureIfExists("Diferencia", col =>
            {
                col.HeaderText = "Diferencia";
                col.DefaultCellStyle.Format = "C2";
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                col.Width = 90;
            });

            ConfigureIfExists("Propina", col =>
            {
                col.HeaderText = "Propina";
                col.DefaultCellStyle.Format = "C2";
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                col.Width = 85;
            });

            ConfigureIfExists("SubTotalXml", col =>
            {
                col.HeaderText = "Subtotal XML";
                col.DefaultCellStyle.Format = "C2";
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                col.Width = 100;
            });

            ConfigureIfExists("Descuento", col =>
            {
                col.HeaderText = "Descuento";
                col.DefaultCellStyle.Format = "C2";
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                col.Width = 90;
            });

            ConfigureIfExists("IvaTrasladado", col =>
            {
                col.HeaderText = "IVA Trasladado";
                col.DefaultCellStyle.Format = "C2";
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                col.Width = 100;
            });

            ConfigureIfExists("IepsTrasladado", col =>
            {
                col.HeaderText = "IEPS";
                col.DefaultCellStyle.Format = "C2";
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                col.Width = 85;
            });

            ConfigureIfExists("RetencionIva", col =>
            {
                col.HeaderText = "Ret. IVA";
                col.DefaultCellStyle.Format = "C2";
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                col.Width = 85;
            });

            ConfigureIfExists("RetencionIsr", col =>
            {
                col.HeaderText = "Ret. ISR";
                col.DefaultCellStyle.Format = "C2";
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                col.Width = 85;
            });

            ConfigureIfExists("ImpuestosLocales", col =>
            {
                col.HeaderText = "Imp. Locales";
                col.DefaultCellStyle.Format = "C2";
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                col.Width = 90;
            });

            ConfigureIfExists("ConceptosDescripcion", col =>
            {
                col.HeaderText = "Conceptos Facturados (Tipo de Cosa)";
                col.Width = 280;
            });

            ConfigureIfExists("ClavesSat", col =>
            {
                col.HeaderText = "Claves SAT";
                col.Width = 110;
            });

            ConfigureIfExists("RfcXml", col =>
            {
                col.HeaderText = "RFC Emisor";
                col.Width = 115;
            });

            ConfigureIfExists("Emisor", col =>
            {
                col.HeaderText = "Proveedor (Emisor XML)";
                col.Width = 240;
            });

            ConfigureIfExists("EmisorRegimen", col =>
            {
                col.HeaderText = "Régimen Emisor";
                col.Width = 110;
            });

            ConfigureIfExists("ReceptorRfc", col =>
            {
                col.HeaderText = "RFC Receptor";
                col.Width = 115;
            });

            ConfigureIfExists("ReceptorNombre", col =>
            {
                col.HeaderText = "Receptor";
                col.Width = 200;
            });

            ConfigureIfExists("ReceptorRegimen", col =>
            {
                col.HeaderText = "Régimen Receptor";
                col.Width = 110;
            });

            ConfigureIfExists("ReceptorCp", col =>
            {
                col.HeaderText = "CP Receptor";
                col.Width = 90;
            });

            ConfigureIfExists("UsoCfdi", col =>
            {
                col.HeaderText = "Uso CFDI";
                col.Width = 95;
            });

            ConfigureIfExists("FormaPago", col =>
            {
                col.HeaderText = "Forma Pago";
                col.Width = 95;
            });

            ConfigureIfExists("MetodoPago", col =>
            {
                col.HeaderText = "Método Pago";
                col.Width = 95;
            });

            ConfigureIfExists("Serie", col =>
            {
                col.HeaderText = "Serie";
                col.Width = 70;
            });

            ConfigureIfExists("Folio", col =>
            {
                col.HeaderText = "Folio";
                col.Width = 80;
            });

            ConfigureIfExists("TipoComprobante", col =>
            {
                col.HeaderText = "Tipo";
                col.Width = 60;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            });

            ConfigureIfExists("Moneda", col =>
            {
                col.HeaderText = "Moneda";
                col.Width = 70;
            });

            ConfigureIfExists("TipoCambio", col =>
            {
                col.HeaderText = "T.C.";
                col.Width = 65;
            });

            ConfigureIfExists("LugarExpedicion", col =>
            {
                col.HeaderText = "CP Emisión";
                col.Width = 90;
            });

            ConfigureIfExists("FechaXml", col =>
            {
                col.HeaderText = "Fecha Factura (XML)";
                col.Width = 115;
            });

            ConfigureIfExists("FechaTimbrado", col =>
            {
                col.HeaderText = "Fecha Timbrado (Factura)";
                col.Width = 135;
            });

            ConfigureIfExists("RfcProvCertif", col =>
            {
                col.HeaderText = "PAC Timbrado";
                col.Width = 110;
            });

            ConfigureIfExists("UUID", col =>
            {
                col.HeaderText = "UUID (Folio Fiscal)";
                col.Width = 260;
            });

            ConfigureIfExists("ArchivoXml", col =>
            {
                col.HeaderText = "Archivo XML";
                col.Width = 200;
            });

            ConfigureIfExists("Subcarpeta", col =>
            {
                col.HeaderText = "Subcarpeta";
                col.Width = 110;
            });

            // Colorear filas de manera homogénea con el resto de la app
            foreach (DataGridViewRow row in grid.Rows)
            {
                bool esPar = (row.Index % 2 == 0);
                row.DefaultCellStyle.BackColor = esPar ? TemaGamerEmpresarial.FondoEspacial : TemaGamerEmpresarial.FondoAlterno;
                row.DefaultCellStyle.ForeColor = TemaGamerEmpresarial.TextoPrincipal;
                row.DefaultCellStyle.SelectionBackColor = TemaGamerEmpresarial.AzulSeleccion;
                row.DefaultCellStyle.SelectionForeColor = Color.White;

                string estado = row.Cells["EstadoFactura"]?.Value?.ToString() ?? "";
                var estCell = row.Cells["EstadoFactura"];
                if (estCell == null) continue;

                var difVal = row.Cells["Diferencia"]?.Value;
                bool difMayor500 = false;
                if (difVal != null && decimal.TryParse(difVal.ToString(), out decimal dVal) && dVal >= 500m)
                {
                    difMayor500 = true;
                }

                if (difMayor500 && !estado.StartsWith("ℹ"))
                {
                    estCell.Style.ForeColor = TemaGamerEmpresarial.RojoLaser;
                    estCell.Style.Font = new Font(grid.Font, FontStyle.Bold);
                }
                else if (estado.StartsWith("✅"))
                {
                    if (estado.Contains("PAQUETE") || estado.Contains("SUMA DE"))
                    {
                        estCell.Style.ForeColor = TemaGamerEmpresarial.MoradoNeon;
                    }
                    else
                    {
                        estCell.Style.ForeColor = TemaGamerEmpresarial.VerdeNeon;
                    }
                    estCell.Style.Font = new Font(grid.Font, FontStyle.Bold);
                }
                else if (estado.StartsWith("⚠️"))
                {
                    estCell.Style.ForeColor = TemaGamerEmpresarial.AmbarNeon;
                    estCell.Style.Font = new Font(grid.Font, FontStyle.Bold);
                }
                else if (estado.StartsWith("❌"))
                {
                    estCell.Style.ForeColor = TemaGamerEmpresarial.RojoLaser;
                    estCell.Style.Font = new Font(grid.Font, FontStyle.Bold);
                }
                else if (estado.StartsWith("ℹ"))
                {
                    estCell.Style.ForeColor = TemaGamerEmpresarial.CianNeon;
                    estCell.Style.Font = new Font(grid.Font, FontStyle.Bold);
                }
            }
        }

        private void FormatearGridHuerfanos(DataGridView grid)
        {
            if (grid.Columns.Count == 0) return;
            TemaGamerEmpresarial.EstilizarGrid(grid);
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            Action<string, string, int, DataGridViewContentAlignment> ConfigCol = (name, header, width, align) =>
            {
                if (grid.Columns.Contains(name))
                {
                    var col = grid.Columns[name];
                    if (!string.IsNullOrEmpty(header)) col.HeaderText = header;
                    if (width > 0) col.Width = width;
                    col.DefaultCellStyle.Alignment = align;
                }
            };

            ConfigCol("Subcarpeta", "Subcarpeta", 110, DataGridViewContentAlignment.MiddleLeft);
            ConfigCol("NombreArchivo", "Archivo XML", 200, DataGridViewContentAlignment.MiddleLeft);
            ConfigCol("NumeroProyecto", "No. Proyecto", 100, DataGridViewContentAlignment.MiddleCenter);
            ConfigCol("Total", "Total XML", 105, DataGridViewContentAlignment.MiddleRight);
            ConfigCol("Subtotal", "Subtotal XML", 100, DataGridViewContentAlignment.MiddleRight);
            ConfigCol("Descuento", "Descuento", 90, DataGridViewContentAlignment.MiddleRight);
            ConfigCol("IVA", "IVA Trasladado", 100, DataGridViewContentAlignment.MiddleRight);
            ConfigCol("IEPS", "IEPS", 85, DataGridViewContentAlignment.MiddleRight);
            ConfigCol("RetIVA", "Ret. IVA", 85, DataGridViewContentAlignment.MiddleRight);
            ConfigCol("RetISR", "Ret. ISR", 85, DataGridViewContentAlignment.MiddleRight);
            ConfigCol("ImpLocales", "Imp. Locales", 90, DataGridViewContentAlignment.MiddleRight);
            ConfigCol("Propina", "Propina", 85, DataGridViewContentAlignment.MiddleRight);
            ConfigCol("Conceptos", "Conceptos Facturados (Tipo de Cosa)", 280, DataGridViewContentAlignment.MiddleLeft);
            ConfigCol("ClavesSAT", "Claves SAT", 110, DataGridViewContentAlignment.MiddleLeft);
            ConfigCol("Fecha", "Fecha Emisión", 95, DataGridViewContentAlignment.MiddleLeft);
            ConfigCol("EmisorRfc", "RFC Emisor", 115, DataGridViewContentAlignment.MiddleLeft);
            ConfigCol("EmisorNombre", "Proveedor (Emisor XML)", 240, DataGridViewContentAlignment.MiddleLeft);
            ConfigCol("EmisorRegimen", "Régimen Emisor", 110, DataGridViewContentAlignment.MiddleLeft);
            ConfigCol("ReceptorRfc", "RFC Receptor", 115, DataGridViewContentAlignment.MiddleLeft);
            ConfigCol("ReceptorNombre", "Receptor", 200, DataGridViewContentAlignment.MiddleLeft);
            ConfigCol("ReceptorRegimen", "Régimen Receptor", 110, DataGridViewContentAlignment.MiddleLeft);
            ConfigCol("ReceptorCp", "CP Receptor", 90, DataGridViewContentAlignment.MiddleLeft);
            ConfigCol("UsoCfdi", "Uso CFDI", 95, DataGridViewContentAlignment.MiddleLeft);
            ConfigCol("Serie", "Serie", 70, DataGridViewContentAlignment.MiddleLeft);
            ConfigCol("Folio", "Folio", 80, DataGridViewContentAlignment.MiddleLeft);
            ConfigCol("TipoComprobante", "Tipo", 60, DataGridViewContentAlignment.MiddleCenter);
            ConfigCol("MetodoPago", "Método Pago", 95, DataGridViewContentAlignment.MiddleLeft);
            ConfigCol("FormaPago", "Forma Pago", 95, DataGridViewContentAlignment.MiddleLeft);
            ConfigCol("Moneda", "Moneda", 70, DataGridViewContentAlignment.MiddleLeft);
            ConfigCol("TipoCambio", "T.C.", 65, DataGridViewContentAlignment.MiddleLeft);
            ConfigCol("LugarExpedicion", "CP Emisión", 90, DataGridViewContentAlignment.MiddleLeft);
            ConfigCol("UUID", "UUID (Folio Fiscal)", 260, DataGridViewContentAlignment.MiddleLeft);
            ConfigCol("FechaTimbrado", "Fecha Timbrado", 125, DataGridViewContentAlignment.MiddleLeft);
            ConfigCol("RfcProvCertif", "PAC Timbrado", 110, DataGridViewContentAlignment.MiddleLeft);

            foreach (DataGridViewRow row in grid.Rows)
            {
                bool esPar = (row.Index % 2 == 0);
                row.DefaultCellStyle.BackColor = esPar ? TemaGamerEmpresarial.FondoEspacial : TemaGamerEmpresarial.FondoAlterno;
                row.DefaultCellStyle.ForeColor = TemaGamerEmpresarial.TextoPrincipal;
                row.DefaultCellStyle.SelectionBackColor = TemaGamerEmpresarial.AzulSeleccion;
                row.DefaultCellStyle.SelectionForeColor = Color.White;
            }
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
                        sb.AppendLine("No. Operacion,Estado Factura,No. Proyecto,Fecha Registro (Estado),Fecha Compra (Estado),Descripcion Banco,RFC Banco,Cargo Banco,Total XML,Diferencia,Propina,Subtotal XML,Descuento,IVA Trasladado,IEPS Trasladado,Retencion IVA,Retencion ISR,Impuestos Locales,Conceptos Facturados (Tipo de Cosa),Claves SAT,RFC Emisor,Proveedor (Emisor XML),Regimen Emisor,RFC Receptor,Receptor,Regimen Receptor,CP Receptor,Uso CFDI,Forma Pago,Metodo Pago,Serie,Folio,Tipo Comprobante,Moneda,Tipo Cambio,CP Emision,Fecha Factura (XML),Fecha Timbrado (Factura),PAC Timbrado,UUID,Archivo XML,Subcarpeta");

                        foreach (var i in _listaConciliacion)
                        {
                            sb.AppendLine(
                                $"{i.NoOperacion}," +
                                $"\"{EscaparCsv(i.EstadoFactura)}\"," +
                                $"\"{EscaparCsv(i.NumeroProyecto)}\"," +
                                $"\"{EscaparCsv(i.Fecha)}\"," +
                                $"\"{EscaparCsv(i.FechaCompra)}\"," +
                                $"\"{EscaparCsv(i.Descripcion)}\"," +
                                $"\"{EscaparCsv(i.RfcBanco)}\"," +
                                $"{i.ImporteCsv.ToString(CultureInfo.InvariantCulture)}," +
                                $"{(i.TotalXml.HasValue ? i.TotalXml.Value.ToString(CultureInfo.InvariantCulture) : "")}," +
                                $"{(i.Diferencia.HasValue ? i.Diferencia.Value.ToString(CultureInfo.InvariantCulture) : "")}," +
                                $"{(i.Propina.HasValue ? i.Propina.Value.ToString(CultureInfo.InvariantCulture) : "")}," +
                                $"{(i.SubTotalXml.HasValue ? i.SubTotalXml.Value.ToString(CultureInfo.InvariantCulture) : "")}," +
                                $"{(i.Descuento.HasValue ? i.Descuento.Value.ToString(CultureInfo.InvariantCulture) : "")}," +
                                $"{(i.IvaTrasladado.HasValue ? i.IvaTrasladado.Value.ToString(CultureInfo.InvariantCulture) : "")}," +
                                $"{(i.IepsTrasladado.HasValue ? i.IepsTrasladado.Value.ToString(CultureInfo.InvariantCulture) : "")}," +
                                $"{(i.RetencionIva.HasValue ? i.RetencionIva.Value.ToString(CultureInfo.InvariantCulture) : "")}," +
                                $"{(i.RetencionIsr.HasValue ? i.RetencionIsr.Value.ToString(CultureInfo.InvariantCulture) : "")}," +
                                $"{(i.ImpuestosLocales.HasValue ? i.ImpuestosLocales.Value.ToString(CultureInfo.InvariantCulture) : "")}," +
                                $"\"{EscaparCsv(i.ConceptosDescripcion)}\"," +
                                $"\"{EscaparCsv(i.ClavesSat)}\"," +
                                $"\"{EscaparCsv(i.RfcXml)}\"," +
                                $"\"{EscaparCsv(i.Emisor)}\"," +
                                $"\"{EscaparCsv(i.EmisorRegimen)}\"," +
                                $"\"{EscaparCsv(i.ReceptorRfc)}\"," +
                                $"\"{EscaparCsv(i.ReceptorNombre)}\"," +
                                $"\"{EscaparCsv(i.ReceptorRegimen)}\"," +
                                $"\"{EscaparCsv(i.ReceptorCp)}\"," +
                                $"\"{EscaparCsv(i.UsoCfdi)}\"," +
                                $"\"{EscaparCsv(i.FormaPago)}\"," +
                                $"\"{EscaparCsv(i.MetodoPago)}\"," +
                                $"\"{EscaparCsv(i.Serie)}\"," +
                                $"\"{EscaparCsv(i.Folio)}\"," +
                                $"\"{EscaparCsv(i.TipoComprobante)}\"," +
                                $"\"{EscaparCsv(i.Moneda)}\"," +
                                $"\"{EscaparCsv(i.TipoCambio)}\"," +
                                $"\"{EscaparCsv(i.LugarExpedicion)}\"," +
                                $"\"{EscaparCsv(i.FechaXml)}\"," +
                                $"\"{EscaparCsv(i.FechaTimbrado)}\"," +
                                $"\"{EscaparCsv(i.RfcProvCertif)}\"," +
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

        public static void HabilitarDobleBuffer(Control c)
        {
            if (c == null) return;
            try
            {
                var pi = typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                pi?.SetValue(c, true, null);
            }
            catch { }
        }

        private void menuGuardarSesion_Click(object sender, EventArgs e)
        {
            try
            {
                if (_listaConciliacion == null || _listaConciliacion.Count == 0)
                {
                    MessageBox.Show("No hay datos de conciliación para guardar. Primero cargue o ejecute una conciliación.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var amarradas = _listaConciliacion.Where(i => i.EstadoFactura.StartsWith("✅")).ToList();
                if (amarradas.Count > 0)
                {
                    var resGuardarFacturas = MessageBox.Show(
                        $"Hay {amarradas.Count} facturas amarradas.\n\n¿Desea seleccionar una carpeta para copiar los archivos XML y PDF amarrados?\n(Las facturas no amarradas permanecerán en su carpeta de origen)",
                        "Guardar Facturas Amarradas",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question
                    );

                    if (resGuardarFacturas == DialogResult.Cancel) return;

                    if (resGuardarFacturas == DialogResult.Yes)
                    {
                        GuardarFacturasAmarradasEnCarpeta();
                    }
                }

                var sesion = new SesionConciliacion
                {
                    FechaGuardado = DateTime.Now,
                    RutaEdoCta = txtRutaCsv.Text,
                    RutaCarpetaXml = txtRutaCarpetaXml.Text,
                    ItemsConciliacion = _listaConciliacion,
                    FacturasXml = _listaFacturasXml
                };

                using (var formSaves = new FormGuardadoVideojuego(sesion))
                {
                    formSaves.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir administrador de guardado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void GuardarFacturasAmarradasEnCarpeta(string carpetaDestino = null)
        {
            if (_listaConciliacion == null || _listaConciliacion.Count == 0)
            {
                MessageBox.Show("No hay datos de conciliación activos. Ejecute una conciliación primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var amarradas = _listaConciliacion.Where(i => i.EstadoFactura.StartsWith("✅")).ToList();
            if (amarradas.Count == 0)
            {
                MessageBox.Show("No hay facturas amarradas (✅) en la conciliación actual para guardar.", "Sin Facturas Amarradas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrEmpty(carpetaDestino))
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "Seleccione la carpeta destino donde se guardarán las facturas amarradas (XML y PDF):";
                    fbd.ShowNewFolderButton = true;
                    if (fbd.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        return;
                    }
                    carpetaDestino = fbd.SelectedPath;
                }
            }

            try
            {
                if (!Directory.Exists(carpetaDestino))
                {
                    Directory.CreateDirectory(carpetaDestino);
                }

                int copiadosXml = 0;
                int copiadosPdf = 0;
                var archivosCopiados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var item in amarradas)
                {
                    // 1. Copiar XML
                    if (!string.IsNullOrEmpty(item.RutaXml) && File.Exists(item.RutaXml))
                    {
                        if (!archivosCopiados.Contains(item.RutaXml))
                        {
                            string nombreXml = Path.GetFileName(item.RutaXml);
                            string destXml = Path.Combine(carpetaDestino, nombreXml);
                            File.Copy(item.RutaXml, destXml, true);
                            archivosCopiados.Add(item.RutaXml);
                            copiadosXml++;
                        }
                    }

                    // 2. Copiar PDF si existe
                    if (!string.IsNullOrEmpty(item.RutaPdf) && File.Exists(item.RutaPdf))
                    {
                        if (!archivosCopiados.Contains(item.RutaPdf))
                        {
                            string nombrePdf = Path.GetFileName(item.RutaPdf);
                            string destPdf = Path.Combine(carpetaDestino, nombrePdf);
                            File.Copy(item.RutaPdf, destPdf, true);
                            archivosCopiados.Add(item.RutaPdf);
                            copiadosPdf++;
                        }
                    }
                    else if (!string.IsNullOrEmpty(item.RutaXml))
                    {
                        // Buscar si existe el PDF homónimo al lado del XML
                        string homonimo = Path.ChangeExtension(item.RutaXml, ".pdf");
                        if (File.Exists(homonimo) && !archivosCopiados.Contains(homonimo))
                        {
                            string nombrePdf = Path.GetFileName(homonimo);
                            string destPdf = Path.Combine(carpetaDestino, nombrePdf);
                            File.Copy(homonimo, destPdf, true);
                            archivosCopiados.Add(homonimo);
                            copiadosPdf++;
                        }
                    }
                }

                MessageBox.Show(
                    $"¡Guardado de Facturas Completado!\n\n" +
                    $"• Carpeta Destino: {carpetaDestino}\n" +
                    $"• Archivos XML copiados: {copiadosXml}\n" +
                    $"• Archivos PDF copiados: {copiadosPdf}\n" +
                    $"• Las facturas no amarradas permanecen en su ubicación original.",
                    "Facturas Guardadas",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al copiar las facturas: {ex.Message}", "Error al Guardar Facturas", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void DesamarrarOperacion(ItemConciliacion item, List<FacturaXmlItem> catalogoXml = null)
        {
            if (item == null) return;

            string uuidAntiguo = item.UUID;
            string archivoXmlAntiguo = item.ArchivoXml;

            // Liberar facturas en el catálogo
            if (catalogoXml != null)
            {
                foreach (var f in catalogoXml)
                {
                    if (!string.IsNullOrEmpty(uuidAntiguo) && !string.IsNullOrEmpty(f.UUID) && uuidAntiguo.Contains(f.UUID))
                    {
                        f.Asignada = false;
                    }
                    else if (!string.IsNullOrEmpty(archivoXmlAntiguo) && !string.IsNullOrEmpty(f.NombreArchivo) && archivoXmlAntiguo.Contains(f.NombreArchivo))
                    {
                        f.Asignada = false;
                    }
                }
            }

            // Reiniciar propiedades de amarre del item
            item.EstadoFactura = "❌ FALTA XML";
            item.NumeroProyecto = "";
            item.TotalXml = null;
            item.SubTotalXml = null;
            item.Diferencia = null;
            item.Descuento = null;
            item.IvaTrasladado = null;
            item.IepsTrasladado = null;
            item.RetencionIva = null;
            item.RetencionIsr = null;
            item.ImpuestosLocales = null;
            item.Propina = null;
            item.ConceptosDescripcion = "";
            item.ClavesSat = "";
            item.EmisorRegimen = "";
            item.ReceptorRfc = "";
            item.ReceptorNombre = "";
            item.ReceptorRegimen = "";
            item.ReceptorCp = "";
            item.LugarExpedicion = "";
            item.TipoCambio = "";
            item.FechaTimbrado = "";
            item.RfcProvCertif = "";
            item.RfcXml = "";
            item.Emisor = "";
            item.UUID = "";
            item.ArchivoXml = "";
            item.Subcarpeta = "";
            item.RutaXml = "";
            item.MetodoPago = "";
            item.FormaPago = "";
            item.Serie = "";
            item.Folio = "";
            item.Moneda = "";
            item.TipoComprobante = "";
            item.UsoCfdi = "";
            item.VersionCfdi = "";
            item.FechaXml = "";
            item.RutaPdf = "";
            item.ArchivoPdf = "";
            item.PdfAsociadoManual = false;
        }

        private void DesamarrarItemYActualizar(ItemConciliacion item)
        {
            if (item == null) return;
            if (!item.EstadoFactura.StartsWith("✅") && string.IsNullOrEmpty(item.UUID) && string.IsNullOrEmpty(item.ArchivoXml))
            {
                MessageBox.Show("Esta operación no tiene ninguna factura amarrada.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var dr = MessageBox.Show(
                $"¿Está seguro de que desea desamarrar la factura vinculada a la operación #{item.NoOperacion} ({item.Descripcion})?\n\nLa operación pasará a '❌ FALTA XML' y el XML quedará libre.",
                "Confirmar Desamarrado",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (dr == DialogResult.Yes)
            {
                DesamarrarOperacion(item, _listaFacturasXml);
                MostrarResultadosEnGrids();
                MessageBox.Show("Factura desamarrada exitosamente.", "Desamarrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private ContextMenuStrip _cmsGrids;

        private void ConfigurarMenuContextualGrids()
        {
            _cmsGrids = new ContextMenuStrip();
            var itemVerDetalle = new ToolStripMenuItem("🔍 Ver / Editar Detalle de Operación", null, (s, e) =>
            {
                var grid = _cmsGrids.SourceControl as DataGridView;
                if (grid != null && grid.CurrentRow != null)
                {
                    AbrirDetalleMovimiento(grid.CurrentRow);
                }
            });

            var itemDesamarrar = new ToolStripMenuItem("❌ Desamarrar Factura Vinculada", null, (s, e) =>
            {
                var grid = _cmsGrids.SourceControl as DataGridView;
                if (grid != null && grid.CurrentRow != null)
                {
                    var item = grid.CurrentRow.DataBoundItem as ItemConciliacion;
                    if (item != null)
                    {
                        DesamarrarItemYActualizar(item);
                    }
                }
            });

            _cmsGrids.Items.Add(itemVerDetalle);
            _cmsGrids.Items.Add(new ToolStripSeparator());
            _cmsGrids.Items.Add(itemDesamarrar);

            _cmsGrids.Opening += (s, e) =>
            {
                var grid = _cmsGrids.SourceControl as DataGridView;
                if (grid != null && grid.CurrentRow != null)
                {
                    var item = grid.CurrentRow.DataBoundItem as ItemConciliacion;
                    bool tieneFactura = item != null && (!string.IsNullOrEmpty(item.UUID) || !string.IsNullOrEmpty(item.ArchivoXml) || item.EstadoFactura.StartsWith("✅") || item.TotalXml.HasValue);
                    itemDesamarrar.Enabled = tieneFactura;
                }
                else
                {
                    e.Cancel = true;
                }
            };

            gridTodas.ContextMenuStrip = _cmsGrids;
            gridFaltantes.ContextMenuStrip = _cmsGrids;
            gridCoincidentes.ContextMenuStrip = _cmsGrids;

            MouseEventHandler onGridMouseDown = (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    var dgv = s as DataGridView;
                    if (dgv == null) return;
                    var hit = dgv.HitTest(e.X, e.Y);
                    if (hit.RowIndex >= 0)
                    {
                        dgv.ClearSelection();
                        dgv.Rows[hit.RowIndex].Selected = true;
                        dgv.CurrentCell = dgv.Rows[hit.RowIndex].Cells[Math.Max(0, hit.ColumnIndex)];
                    }
                }
            };

            gridTodas.MouseDown += onGridMouseDown;
            gridFaltantes.MouseDown += onGridMouseDown;
            gridCoincidentes.MouseDown += onGridMouseDown;
        }

        private void menuCargarSesion_Click(object sender, EventArgs e)
        {
            try
            {
                using (var formSaves = new FormGuardadoVideojuego(null))
                {
                    if (formSaves.ShowDialog(this) == DialogResult.OK && formSaves.SesionCargada != null)
                    {
                        var sesion = formSaves.SesionCargada;
                        txtRutaCsv.Text = sesion.RutaEdoCta ?? "";
                        txtRutaCarpetaXml.Text = sesion.RutaCarpetaXml ?? "";
                        _listaConciliacion = sesion.ItemsConciliacion ?? new List<ItemConciliacion>();
                        _listaFacturasXml = sesion.FacturasXml ?? new List<FacturaXmlItem>();

                        MostrarResultadosEnGrids();
                        btnExportar.Enabled = _listaConciliacion.Count > 0;
                        lblEstado.Text = $"Sesión cargada: {_listaConciliacion.Count} operaciones restauradas.";
                        MessageBox.Show("¡Sesión de conciliación cargada exitosamente!", "Sesión Cargada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar sesión: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void menuSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGenerarExcel_Click(object sender, EventArgs e)
        {
            string rutaPdf = txtRutaCsv.Text.Trim();
            if (string.IsNullOrEmpty(rutaPdf) || !File.Exists(rutaPdf))
            {
                MessageBox.Show("Por favor seleccione un archivo PDF de estado de cuenta válido primero.", "PDF Requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Libro de Excel (*.xlsx)|*.xlsx";
                sfd.FileName = Path.GetFileNameWithoutExtension(rutaPdf) + "_Convertido.xlsx";
                sfd.Title = "Guardar Estado de Cuenta como Excel";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                        string scriptPath = Path.Combine(baseDir, "generar_excel_amex.py");
                        if (!File.Exists(scriptPath))
                        {
                            scriptPath = Path.GetFullPath(Path.Combine(baseDir, @"..\..\generar_excel_amex.py"));
                        }

                        if (!File.Exists(scriptPath))
                        {
                            MessageBox.Show("No se encontró el script auxiliar para convertir a Excel.", "Script no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        var psi = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = "python",
                            Arguments = $"\"{scriptPath}\" \"{rutaPdf}\" \"{sfd.FileName}\"",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        };

                        using (var proc = System.Diagnostics.Process.Start(psi))
                        {
                            proc.WaitForExit(30000);
                            if (proc.ExitCode == 0 && File.Exists(sfd.FileName))
                            {
                                var res = MessageBox.Show("¡Archivo Excel generado exitosamente!\n\n¿Desea abrir el archivo generado?", "Conversión Exitosa", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                                if (res == DialogResult.Yes)
                                {
                                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                                }
                            }
                            else
                            {
                                string err = proc.StandardError.ReadToEnd();
                                MessageBox.Show("Error al generar Excel: " + (string.IsNullOrWhiteSpace(err) ? "Código de salida " + proc.ExitCode : err), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al ejecutar conversión: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void GridMovimientos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var dgv = sender as DataGridView;
            if (dgv == null) return;

            AbrirDetalleMovimiento(dgv.Rows[e.RowIndex]);
        }

        private void GridMovimientos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
            {
                var dgv = sender as DataGridView;
                if (dgv != null && dgv.CurrentRow != null && dgv.CurrentRow.Index >= 0)
                {
                    e.Handled = true;
                    AbrirDetalleMovimiento(dgv.CurrentRow);
                }
            }
        }

        private void AbrirDetalleMovimiento(DataGridViewRow row)
        {
            if (row == null) return;
            var item = row.DataBoundItem as ItemConciliacion;
            if (item == null) return;

            using (var formDetalle = new FormDetalleConciliacion(item, null, _listaConciliacion))
            {
                if (formDetalle.ShowDialog(this) == DialogResult.OK)
                {
                    MostrarResultadosEnGrids();
                }
            }
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
        public string Serie { get; set; } = "";
        public string Folio { get; set; } = "";
        public string Moneda { get; set; } = "";
        public string TipoComprobante { get; set; } = "";
        public string UsoCfdi { get; set; } = "";
        public string VersionCfdi { get; set; } = "";
        public string NombreArchivo { get; set; } = "";
        public string RutaCompleta { get; set; } = "";
        public string Subcarpeta { get; set; } = "";
        public string NumeroProyecto { get; set; } = "";
        public bool Asignada { get; set; }

        // Campos Contables Adicionales
        public string ConceptosDescripcion { get; set; } = "";
        public string ClavesSat { get; set; } = "";
        public decimal? Descuento { get; set; }
        public decimal? IvaTrasladado { get; set; }
        public decimal? IepsTrasladado { get; set; }
        public decimal? RetencionIva { get; set; }
        public decimal? RetencionIsr { get; set; }
        public decimal? ImpuestosLocales { get; set; }
        public decimal? Propina { get; set; }
        public string EmisorRegimen { get; set; } = "";
        public string ReceptorRfc { get; set; } = "";
        public string ReceptorNombre { get; set; } = "";
        public string ReceptorRegimen { get; set; } = "";
        public string ReceptorCp { get; set; } = "";
        public string LugarExpedicion { get; set; } = "";
        public string TipoCambio { get; set; } = "";
        public string FechaTimbrado { get; set; } = "";
        public string RfcProvCertif { get; set; } = "";
    }

    public class ItemConciliacion
    {
        public int NoOperacion { get; set; }
        public string EstadoFactura { get; set; } = "";
        public string NumeroProyecto { get; set; } = "";
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
        public string Serie { get; set; } = "";
        public string Folio { get; set; } = "";
        public string Moneda { get; set; } = "";
        public string TipoComprobante { get; set; } = "";
        public string UsoCfdi { get; set; } = "";
        public string VersionCfdi { get; set; } = "";
        public string FechaXml { get; set; } = "";
        public string RutaPdf { get; set; } = "";
        public string ArchivoPdf { get; set; } = "";
        public bool PdfAsociadoManual { get; set; }

        // Campos Contables Adicionales
        public string ConceptosDescripcion { get; set; } = "";
        public string ClavesSat { get; set; } = "";
        public decimal? Descuento { get; set; }
        public decimal? IvaTrasladado { get; set; }
        public decimal? IepsTrasladado { get; set; }
        public decimal? RetencionIva { get; set; }
        public decimal? RetencionIsr { get; set; }
        public decimal? ImpuestosLocales { get; set; }
        public decimal? Propina { get; set; }
        public string EmisorRegimen { get; set; } = "";
        public string ReceptorRfc { get; set; } = "";
        public string ReceptorNombre { get; set; } = "";
        public string ReceptorRegimen { get; set; } = "";
        public string ReceptorCp { get; set; } = "";
        public string LugarExpedicion { get; set; } = "";
        public string TipoCambio { get; set; } = "";
        public string FechaTimbrado { get; set; } = "";
        public string RfcProvCertif { get; set; } = "";
    }

    public class ResultadoProceso
    {
        public List<ItemConciliacion> ItemsConciliacion { get; set; }
        public List<FacturaXmlItem> FacturasXml { get; set; }
        public int Coincidentes { get; set; }
        public int Faltantes { get; set; }
        public int Huerfanos { get; set; }
    }

    public class SesionConciliacion
    {
        public string Version { get; set; } = "1.0";
        public DateTime FechaGuardado { get; set; }
        public string RutaEdoCta { get; set; } = "";
        public string RutaCarpetaXml { get; set; } = "";
        public List<ItemConciliacion> ItemsConciliacion { get; set; } = new List<ItemConciliacion>();
        public List<FacturaXmlItem> FacturasXml { get; set; } = new List<FacturaXmlItem>();
    }
}
