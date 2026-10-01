using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FormDetalleConciliacion : Form
    {
        private readonly ItemConciliacion _item;
        private readonly List<ItemConciliacion> _movimientosVinculados;
        private readonly List<ItemConciliacion> _listaCompletaSesion;
        public bool SeModifico { get; private set; } = false;

        public FormDetalleConciliacion(ItemConciliacion item, List<ItemConciliacion> movimientosVinculados = null, List<ItemConciliacion> listaCompletaSesion = null)
        {
            InitializeComponent();
            _item = item ?? throw new ArgumentNullException(nameof(item));
            _listaCompletaSesion = listaCompletaSesion;

            if (movimientosVinculados != null && movimientosVinculados.Count > 0)
            {
                _movimientosVinculados = movimientosVinculados;
            }
            else if (_listaCompletaSesion != null && !string.IsNullOrEmpty(item.UUID))
            {
                var grupo = _listaCompletaSesion.Where(x => !string.IsNullOrEmpty(x.UUID) && x.UUID == item.UUID).ToList();
                _movimientosVinculados = (grupo.Count > 1) ? grupo : new List<ItemConciliacion> { item };
            }
            else
            {
                _movimientosVinculados = new List<ItemConciliacion> { item };
            }

            FormConciliacionCsv.HabilitarDobleBuffer(gridMovimientosVinculados);
            ConfigurarColumnasGridMovimientos();
            CargarDatosEnPantalla();

            TemaGamerEmpresarial.AplicarTema(this);
            pnlHeader.BackColor = TemaGamerEmpresarial.FondoPanel;
            pnlFooter.BackColor = TemaGamerEmpresarial.FondoPanel;
            grpBanco.BackColor = TemaGamerEmpresarial.FondoPanel;
            grpBanco.ForeColor = TemaGamerEmpresarial.CianClaro;
            grpXml.BackColor = TemaGamerEmpresarial.FondoPanel;
            grpXml.ForeColor = TemaGamerEmpresarial.CianClaro;
            grpPdf.BackColor = TemaGamerEmpresarial.FondoPanel;
            grpPdf.ForeColor = TemaGamerEmpresarial.CianClaro;
            TemaGamerEmpresarial.EstilizarGrid(gridMovimientosVinculados);
        }

        private void ConfigurarColumnasGridMovimientos()
        {
            gridMovimientosVinculados.Columns.Clear();
            gridMovimientosVinculados.AutoGenerateColumns = false;

            var colNo = new DataGridViewTextBoxColumn
            {
                Name = "colNoOp",
                HeaderText = "#",
                Width = 42,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            var colFecha = new DataGridViewTextBoxColumn
            {
                Name = "colFecha",
                HeaderText = "Fecha",
                Width = 90
            };

            var colDesc = new DataGridViewTextBoxColumn
            {
                Name = "colDesc",
                HeaderText = "Descripción (Estado de Cuenta)",
                Width = 340
            };

            var colRfc = new DataGridViewTextBoxColumn
            {
                Name = "colRfc",
                HeaderText = "RFC Banco",
                Width = 115
            };

            var colImporte = new DataGridViewTextBoxColumn
            {
                Name = "colImporte",
                HeaderText = "Cargo Banco",
                Width = 115,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "C2" }
            };

            gridMovimientosVinculados.Columns.AddRange(colNo, colFecha, colDesc, colRfc, colImporte);
        }

        private void CargarDatosEnPantalla()
        {
            try
            {
                // Header
                lblTitulo.Text = $"Detalle del Movimiento Bancario - Operación #{_item.NoOperacion}";
                lblSubtitulo.Text = $"Estado: {_item.EstadoFactura}";

                // Banco
                lblFechaReg.Text = string.IsNullOrWhiteSpace(_item.Fecha) ? "-" : _item.Fecha;
                lblFechaCompra.Text = string.IsNullOrWhiteSpace(_item.FechaCompra) ? "-" : _item.FechaCompra;
                lblRfcBanco.Text = string.IsNullOrWhiteSpace(_item.RfcBanco) ? "(Sin RFC detectado)" : _item.RfcBanco;
                lblMontoBanco.Text = _item.ImporteCsv.ToString("C2");
                txtDescripcionBanco.Text = _item.Descripcion ?? "";

                // Cargar lista de movimientos del estado de cuenta que llenan esta factura
                gridMovimientosVinculados.Rows.Clear();
                decimal sumaCargos = 0;
                int selectedIndex = 0;
                for (int i = 0; i < _movimientosVinculados.Count; i++)
                {
                    var mov = _movimientosVinculados[i];
                    sumaCargos += Math.Abs(mov.ImporteCsv);
                    int idx = gridMovimientosVinculados.Rows.Add(
                        mov.NoOperacion,
                        string.IsNullOrWhiteSpace(mov.FechaCompra) ? mov.Fecha : mov.FechaCompra,
                        mov.Descripcion,
                        string.IsNullOrWhiteSpace(mov.RfcBanco) ? "-" : mov.RfcBanco,
                        mov.ImporteCsv
                    );
                    if (mov.NoOperacion == _item.NoOperacion)
                    {
                        selectedIndex = idx;
                    }
                }

                if (gridMovimientosVinculados.Rows.Count > 0 && selectedIndex < gridMovimientosVinculados.Rows.Count)
                {
                    gridMovimientosVinculados.Rows[selectedIndex].Selected = true;
                }

                if (_movimientosVinculados.Count > 1)
                {
                    decimal totalFactura = _item.TotalXml ?? 0;
                    decimal dif = Math.Abs(sumaCargos - totalFactura);
                    lblResumenPaquete.Text = $"📦 PAQUETE DE {_movimientosVinculados.Count} MOVIMIENTOS BANCARIOS | Suma Cargos: {sumaCargos:C2} | Factura XML: {totalFactura:C2} | Dif: {dif:C2}";
                    lblResumenPaquete.ForeColor = TemaGamerEmpresarial.MoradoNeon;
                    lblSubtitulo.Text = $"Paquete de {_movimientosVinculados.Count} cargos en el estado de cuenta que amparan esta Factura";
                }
                else
                {
                    lblResumenPaquete.Text = $"1 movimiento bancario registrado por {_item.ImporteCsv:C2}";
                    lblResumenPaquete.ForeColor = TemaGamerEmpresarial.TextoSecundario;
                }

                bool existeXml = !string.IsNullOrWhiteSpace(_item.RutaXml) && File.Exists(_item.RutaXml);
                bool tieneXml = existeXml || !string.IsNullOrWhiteSpace(_item.UUID) || _item.TotalXml.HasValue;
                bool esAmarrada = _item.EstadoFactura != null && _item.EstadoFactura.StartsWith("✅");
                bool esFaltante = !tieneXml || (_item.EstadoFactura != null && _item.EstadoFactura.StartsWith("❌"));
                bool esPaquete = esAmarrada && (_movimientosVinculados.Count > 1 || (_item.EstadoFactura != null && (_item.EstadoFactura.Contains("PAQUETE") || _item.EstadoFactura.Contains("SUMA DE"))));
                Color colorAmarre = esPaquete ? TemaGamerEmpresarial.MoradoNeon : TemaGamerEmpresarial.VerdeNeon;

                if (esAmarrada)
                {
                    lblMontoBanco.ForeColor = colorAmarre;
                }
                else if (esFaltante)
                {
                    lblMontoBanco.ForeColor = TemaGamerEmpresarial.RojoLaser;
                    lblSubtitulo.ForeColor = TemaGamerEmpresarial.RojoLaser;
                }
                else
                {
                    lblMontoBanco.ForeColor = TemaGamerEmpresarial.AmbarNeon;
                }

                // XML
                if (tieneXml)
                {
                    lblRfcXml.Text = string.IsNullOrWhiteSpace(_item.RfcXml) ? "-" : _item.RfcXml;
                    lblProveedor.Text = string.IsNullOrWhiteSpace(_item.Emisor) ? "(Proveedor no especificado)" : _item.Emisor;
                    lblTotalXml.Text = _item.TotalXml.HasValue ? _item.TotalXml.Value.ToString("C2") : "$0.00";
                    lblTotalXml.ForeColor = esAmarrada ? colorAmarre : TemaGamerEmpresarial.VerdeNeon;
                    lblSubtotalXml.Text = _item.SubTotalXml.HasValue ? _item.SubTotalXml.Value.ToString("C2") : "$0.00";
                    lblDiferencia.Text = _item.Diferencia.HasValue ? _item.Diferencia.Value.ToString("C2") : "$0.00";
                    lblDiferencia.ForeColor = (_item.Diferencia.HasValue && _item.Diferencia.Value > 0) ? TemaGamerEmpresarial.AmbarNeon : (esAmarrada ? colorAmarre : TemaGamerEmpresarial.CianClaro);
                    txtUuid.Text = string.IsNullOrWhiteSpace(_item.UUID) ? "-" : _item.UUID;
                    txtRutaXml.Text = string.IsNullOrWhiteSpace(_item.RutaXml) ? _item.ArchivoXml : _item.RutaXml;

                    btnAbrirXml.Enabled = existeXml;
                    btnVerCarpetaXml.Enabled = existeXml;
                    btnAsociarXml.Text = "🔄 Cambiar XML...";
                }
                else
                {
                    lblRfcXml.Text = "No asignado";
                    lblProveedor.Text = "No se detectó XML vinculado con este movimiento bancario";
                    lblTotalXml.Text = "-";
                    lblSubtotalXml.Text = "-";
                    lblDiferencia.Text = "-";
                    txtUuid.Text = "-";
                    txtRutaXml.Text = "Sin archivo XML asociado";
                    btnAbrirXml.Enabled = false;
                    btnVerCarpetaXml.Enabled = false;
                    btnAsociarXml.Text = "🔎 Asociar XML...";
                }

                // PDF
                // Si aún no tiene PDF resuelto, intentar resolver por nombre homónimo al XML
                if ((string.IsNullOrWhiteSpace(_item.RutaPdf) || !File.Exists(_item.RutaPdf)) && !string.IsNullOrWhiteSpace(_item.RutaXml))
                {
                    string candidatoHomonimo = Path.ChangeExtension(_item.RutaXml, ".pdf");
                    if (File.Exists(candidatoHomonimo))
                    {
                        _item.RutaPdf = candidatoHomonimo;
                        _item.ArchivoPdf = Path.GetFileName(candidatoHomonimo);
                        _item.PdfAsociadoManual = false;
                    }
                }

                bool existePdf = !string.IsNullOrWhiteSpace(_item.RutaPdf) && File.Exists(_item.RutaPdf);
                if (existePdf)
                {
                    txtRutaPdf.Text = _item.RutaPdf;
                    string tipoAsoc = _item.PdfAsociadoManual ? "Asociado manualmente" : "Homónimo al XML";
                    lblEstadoPdf.Text = $"✅ Archivo PDF disponible ({tipoAsoc}): {_item.ArchivoPdf}";
                    lblEstadoPdf.ForeColor = Color.FromArgb(22, 101, 52); // Verde oscuro
                    btnAbrirPdf.Enabled = true;
                    btnVerCarpetaPdf.Enabled = true;
                    btnEncontrarPdf.Text = "🔄 Cambiar PDF...";
                }
                else
                {
                    txtRutaPdf.Text = "No se ha seleccionado ningún archivo PDF";
                    lblEstadoPdf.Text = "❌ Sin comprobante PDF asignado";
                    lblEstadoPdf.ForeColor = Color.FromArgb(185, 28, 28); // Rojo
                    btnAbrirPdf.Enabled = false;
                    btnVerCarpetaPdf.Enabled = false;
                    btnEncontrarPdf.Text = "🔎 Encontrar / Asociar PDF...";
                }

                btnDesamarrar.Enabled = _item.TotalXml.HasValue || !string.IsNullOrEmpty(_item.RutaXml) || !string.IsNullOrEmpty(_item.RutaPdf) || _item.EstadoFactura.StartsWith("✅") || _item.EstadoFactura.StartsWith("⚠️");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al presentar detalles del movimiento: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAbrirXml_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(_item.RutaXml))
                {
                    Process.Start(new ProcessStartInfo(_item.RutaXml) { UseShellExecute = true });
                }
                else
                {
                    MessageBox.Show("El archivo XML no existe en la ruta:\n" + _item.RutaXml, "Archivo no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir el archivo XML: " + ex.Message, "Error al abrir", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnVerCarpetaXml_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(_item.RutaXml))
                {
                    Process.Start("explorer.exe", $"/select,\"{_item.RutaXml}\"");
                }
                else if (Directory.Exists(Path.GetDirectoryName(_item.RutaXml)))
                {
                    Process.Start("explorer.exe", $"\"{Path.GetDirectoryName(_item.RutaXml)}\"");
                }
                else
                {
                    MessageBox.Show("No se encontró la ubicación del archivo XML en disco.", "Ruta no encontrada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir carpeta de XML: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAbrirPdf_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(_item.RutaPdf))
                {
                    Process.Start(new ProcessStartInfo(_item.RutaPdf) { UseShellExecute = true });
                }
                else
                {
                    MessageBox.Show("El archivo PDF no existe en la ruta:\n" + _item.RutaPdf, "Archivo no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir el archivo PDF: " + ex.Message, "Error al abrir", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnVerCarpetaPdf_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(_item.RutaPdf))
                {
                    Process.Start("explorer.exe", $"/select,\"{_item.RutaPdf}\"");
                }
                else if (Directory.Exists(Path.GetDirectoryName(_item.RutaPdf)))
                {
                    Process.Start("explorer.exe", $"\"{Path.GetDirectoryName(_item.RutaPdf)}\"");
                }
                else
                {
                    MessageBox.Show("No se encontró la ubicación del archivo PDF en disco.", "Ruta no encontrada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir carpeta de PDF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAsociarXml_Click(object sender, EventArgs e)
        {
            try
            {
                using (var ofd = new OpenFileDialog())
                {
                    ofd.Title = $"Asociar Factura CFDI (XML) para Operación #{_item.NoOperacion}";
                    ofd.Filter = "Comprobantes Fiscales XML (*.xml)|*.xml|Todos los archivos (*.*)|*.*";
                    ofd.CheckFileExists = true;

                    if (!string.IsNullOrWhiteSpace(_item.RutaXml) && File.Exists(_item.RutaXml))
                    {
                        ofd.InitialDirectory = Path.GetDirectoryName(_item.RutaXml);
                    }
                    else if (!string.IsNullOrWhiteSpace(_item.RutaPdf) && File.Exists(_item.RutaPdf))
                    {
                        ofd.InitialDirectory = Path.GetDirectoryName(_item.RutaPdf);
                    }

                    if (ofd.ShowDialog(this) == DialogResult.OK)
                    {
                        var factura = FormConciliacionCsv.ParsearFacturaXml(ofd.FileName);
                        if (factura == null)
                        {
                            MessageBox.Show("No se pudo leer o parsear el archivo XML seleccionado. Asegúrese de que sea un CFDI válido.", "XML Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        AsociarFacturaAOperacion(factura, null);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al asociar el archivo XML: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEncontrarPdf_Click(object sender, EventArgs e)
        {
            try
            {
                using (var ofd = new OpenFileDialog())
                {
                    ofd.Title = $"Asociar Comprobante PDF para Operación #{_item.NoOperacion}";
                    ofd.Filter = "Comprobantes PDF (*.pdf)|*.pdf|Todos los archivos (*.*)|*.*";
                    ofd.CheckFileExists = true;

                    if (!string.IsNullOrWhiteSpace(_item.RutaPdf) && File.Exists(_item.RutaPdf))
                    {
                        ofd.InitialDirectory = Path.GetDirectoryName(_item.RutaPdf);
                    }
                    else if (!string.IsNullOrWhiteSpace(_item.RutaXml) && File.Exists(_item.RutaXml))
                    {
                        ofd.InitialDirectory = Path.GetDirectoryName(_item.RutaXml);
                    }

                    if (ofd.ShowDialog(this) == DialogResult.OK)
                    {
                        string homonimoXml = Path.ChangeExtension(ofd.FileName, ".xml");
                        if (File.Exists(homonimoXml))
                        {
                            var factura = FormConciliacionCsv.ParsearFacturaXml(homonimoXml);
                            if (factura != null)
                            {
                                AsociarFacturaAOperacion(factura, ofd.FileName);
                                return;
                            }
                        }

                        // Si no hay XML homónimo, asociar únicamente el PDF
                        _item.RutaPdf = ofd.FileName;
                        _item.ArchivoPdf = Path.GetFileName(ofd.FileName);
                        _item.PdfAsociadoManual = true;

                        if (string.IsNullOrWhiteSpace(_item.RutaXml) || _item.EstadoFactura.StartsWith("❌"))
                        {
                            _item.EstadoFactura = "📄 PDF ASOCIADO MANUALMENTE";
                        }

                        SeModifico = true;
                        CargarDatosEnPantalla();
                        MessageBox.Show($"¡Comprobante PDF vinculado exitosamente a la operación #{_item.NoOperacion}!\n\nArchivo: {_item.ArchivoPdf}", "Comprobante Vinculado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al asociar comprobante PDF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AsociarFacturaAOperacion(FacturaXmlItem factura, string rutaPdfSugerida)
        {
            // 1. Detectar si este XML ya fue asociado a otra operación en la sesión
            var operacionesPrevias = _listaCompletaSesion != null
                ? _listaCompletaSesion.Where(x => x.NoOperacion != _item.NoOperacion &&
                    ((!string.IsNullOrEmpty(x.RutaXml) && string.Equals(x.RutaXml, factura.RutaCompleta, StringComparison.OrdinalIgnoreCase))
                  || (!string.IsNullOrEmpty(x.UUID) && string.Equals(x.UUID, factura.UUID, StringComparison.OrdinalIgnoreCase)))).ToList()
                : new List<ItemConciliacion>();

            if (operacionesPrevias.Count > 0)
            {
                string ops = string.Join(", ", operacionesPrevias.Select(x => $"#{x.NoOperacion}"));
                var confirm = MessageBox.Show(
                    $"Este XML ya está asociado a la(s) operación(es) {ops}.\n\n¿Quieres asociarlo a este movimiento para formar un paquete de cargos?",
                    "XML ya asociado",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirm != DialogResult.Yes)
                {
                    return;
                }

                // Amarrar en paquete
                var grupoCompleto = new List<ItemConciliacion>(operacionesPrevias) { _item };
                int totalCargos = grupoCompleto.Count;
                decimal sumaCargos = grupoCompleto.Sum(x => Math.Abs(x.ImporteCsv));
                decimal difPaquete = Math.Abs(sumaCargos - factura.Total);

                string pdfCandidato = !string.IsNullOrEmpty(rutaPdfSugerida) && File.Exists(rutaPdfSugerida)
                    ? rutaPdfSugerida
                    : Path.ChangeExtension(factura.RutaCompleta, ".pdf");
                bool existePdf = File.Exists(pdfCandidato);

                foreach (var itm in grupoCompleto)
                {
                    itm.EstadoFactura = $"✅ AMARRADO EN PAQUETE ({totalCargos} CARGOS)";
                    itm.NumeroProyecto = FormConciliacionCsv.ExtraerNumeroProyecto(factura.NombreArchivo);
                    itm.UUID = factura.UUID;
                    itm.TotalXml = factura.Total;
                    itm.SubTotalXml = factura.SubTotal;
                    itm.Diferencia = difPaquete;
                    itm.Emisor = factura.EmisorNombre;
                    itm.RfcXml = factura.EmisorRfc;
                    itm.ArchivoXml = factura.NombreArchivo;
                    itm.RutaXml = factura.RutaCompleta;
                    itm.Subcarpeta = factura.Subcarpeta;
                    itm.MetodoPago = factura.MetodoPago;
                    itm.FormaPago = factura.FormaPago;
                    itm.Serie = factura.Serie;
                    itm.Folio = factura.Folio;
                    itm.Moneda = factura.Moneda;
                    itm.TipoComprobante = factura.TipoComprobante;
                    itm.UsoCfdi = factura.UsoCfdi;
                    itm.VersionCfdi = factura.VersionCfdi;
                    itm.FechaXml = factura.FechaCfdi.HasValue ? factura.FechaCfdi.Value.ToString("yyyy-MM-dd") : "";
                    itm.ConceptosDescripcion = factura.ConceptosDescripcion;
                    itm.ClavesSat = factura.ClavesSat;
                    itm.Descuento = factura.Descuento;
                    itm.IvaTrasladado = factura.IvaTrasladado;
                    itm.IepsTrasladado = factura.IepsTrasladado;
                    itm.RetencionIva = factura.RetencionIva;
                    itm.RetencionIsr = factura.RetencionIsr;
                    itm.ImpuestosLocales = factura.ImpuestosLocales;
                    itm.Propina = factura.Propina;
                    itm.EmisorRegimen = factura.EmisorRegimen;
                    itm.ReceptorRfc = factura.ReceptorRfc;
                    itm.ReceptorNombre = factura.ReceptorNombre;
                    itm.ReceptorRegimen = factura.ReceptorRegimen;
                    itm.ReceptorCp = factura.ReceptorCp;
                    itm.LugarExpedicion = factura.LugarExpedicion;
                    itm.TipoCambio = factura.TipoCambio;
                    itm.FechaTimbrado = factura.FechaTimbrado;
                    itm.RfcProvCertif = factura.RfcProvCertif;

                    if (existePdf && string.IsNullOrEmpty(itm.RutaPdf))
                    {
                        itm.RutaPdf = pdfCandidato;
                        itm.ArchivoPdf = Path.GetFileName(pdfCandidato);
                        itm.PdfAsociadoManual = true;
                    }
                }

                _movimientosVinculados.Clear();
                _movimientosVinculados.AddRange(grupoCompleto);

                SeModifico = true;
                CargarDatosEnPantalla();
                MessageBox.Show($"¡Factura XML amarrada en paquete exitosamente con {totalCargos} cargos!\n\nEmisor: {_item.Emisor}\nTotal Factura: {factura.Total:C2}\nSuma de los {totalCargos} Cargos: {sumaCargos:C2}\nDiferencia: {difPaquete:C2}", "Amarrado en Paquete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Asociación individual (1 a 1)
            _item.NumeroProyecto = FormConciliacionCsv.ExtraerNumeroProyecto(factura.NombreArchivo);
            _item.UUID = factura.UUID;
            _item.TotalXml = factura.Total;
            _item.SubTotalXml = factura.SubTotal;
            _item.Diferencia = Math.Abs(_item.ImporteCsv - factura.Total);
            _item.Emisor = factura.EmisorNombre;
            _item.RfcXml = factura.EmisorRfc;
            _item.ArchivoXml = factura.NombreArchivo;
            _item.RutaXml = factura.RutaCompleta;
            _item.Subcarpeta = factura.Subcarpeta;
            _item.MetodoPago = factura.MetodoPago;
            _item.FormaPago = factura.FormaPago;
            _item.Serie = factura.Serie;
            _item.Folio = factura.Folio;
            _item.Moneda = factura.Moneda;
            _item.TipoComprobante = factura.TipoComprobante;
            _item.UsoCfdi = factura.UsoCfdi;
            _item.VersionCfdi = factura.VersionCfdi;
            _item.FechaXml = factura.FechaCfdi.HasValue ? factura.FechaCfdi.Value.ToString("yyyy-MM-dd") : "";
            _item.ConceptosDescripcion = factura.ConceptosDescripcion;
            _item.ClavesSat = factura.ClavesSat;
            _item.Descuento = factura.Descuento;
            _item.IvaTrasladado = factura.IvaTrasladado;
            _item.IepsTrasladado = factura.IepsTrasladado;
            _item.RetencionIva = factura.RetencionIva;
            _item.RetencionIsr = factura.RetencionIsr;
            _item.ImpuestosLocales = factura.ImpuestosLocales;
            _item.Propina = factura.Propina;
            _item.EmisorRegimen = factura.EmisorRegimen;
            _item.ReceptorRfc = factura.ReceptorRfc;
            _item.ReceptorNombre = factura.ReceptorNombre;
            _item.ReceptorRegimen = factura.ReceptorRegimen;
            _item.ReceptorCp = factura.ReceptorCp;
            _item.LugarExpedicion = factura.LugarExpedicion;
            _item.TipoCambio = factura.TipoCambio;
            _item.FechaTimbrado = factura.FechaTimbrado;
            _item.RfcProvCertif = factura.RfcProvCertif;

            string pdfFinal = !string.IsNullOrEmpty(rutaPdfSugerida) && File.Exists(rutaPdfSugerida)
                ? rutaPdfSugerida
                : Path.ChangeExtension(factura.RutaCompleta, ".pdf");

            if (File.Exists(pdfFinal))
            {
                _item.RutaPdf = pdfFinal;
                _item.ArchivoPdf = Path.GetFileName(pdfFinal);
                _item.PdfAsociadoManual = true;
                _item.EstadoFactura = "✅ XML Y PDF ASOCIADOS MANUALMENTE";
            }
            else
            {
                _item.EstadoFactura = "✅ XML ASOCIADO MANUALMENTE";
            }

            SeModifico = true;
            CargarDatosEnPantalla();
            MessageBox.Show($"¡Factura XML vinculada y datos fiscales extraídos exitosamente!\n\nEmisor: {_item.Emisor}\nRFC: {_item.RfcXml}\nTotal XML: {_item.TotalXml:C2}\nUUID: {_item.UUID}", "XML Vinculado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDesamarrar_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                $"¿Estás seguro de desamarrar y desvincular la factura de la operación #{_item.NoOperacion}?",
                "Confirmar Desamarrado",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes) return;

            foreach (var m in _movimientosVinculados)
            {
                FormConciliacionCsv.DesamarrarOperacion(m);
            }

            SeModifico = true;
            CargarDatosEnPantalla();
            MessageBox.Show($"¡Operación #{_item.NoOperacion} desamarrada exitosamente!\n\nAhora pasa al estado 'FALTA XML'.", "Desamarrado Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.DialogResult = SeModifico ? DialogResult.OK : DialogResult.Cancel;
            this.Close();
        }

        private void FormDetalleConciliacion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = SeModifico ? DialogResult.OK : DialogResult.Cancel;
                this.Close();
            }
        }

        private void gridMovimientosVinculados_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (gridMovimientosVinculados.CurrentRow != null && gridMovimientosVinculados.CurrentRow.Index >= 0)
                {
                    int index = gridMovimientosVinculados.CurrentRow.Index;
                    if (index >= 0 && index < _movimientosVinculados.Count)
                    {
                        var seleccionado = _movimientosVinculados[index];
                        lblFechaReg.Text = string.IsNullOrWhiteSpace(seleccionado.Fecha) ? "-" : seleccionado.Fecha;
                        lblFechaCompra.Text = string.IsNullOrWhiteSpace(seleccionado.FechaCompra) ? "-" : seleccionado.FechaCompra;
                        lblRfcBanco.Text = string.IsNullOrWhiteSpace(seleccionado.RfcBanco) ? "(Sin RFC detectado)" : seleccionado.RfcBanco;
                        lblMontoBanco.Text = seleccionado.ImporteCsv.ToString("C2");
                        txtDescripcionBanco.Text = seleccionado.Descripcion ?? "";
                    }
                }
            }
            catch { }
        }
    }
}
