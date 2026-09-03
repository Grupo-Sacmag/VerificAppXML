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
                ofd.Filter = "Archivos CSV (*.csv)|*.csv|Todos los archivos (*.*)|*.*";
                ofd.Title = "Seleccionar Estado de Cuenta en formato CSV";
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtRutaCsv.Text = ofd.FileName;
                }
            }
        }

        private void btnSeleccionarCarpetaXml_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Seleccionar Carpeta Mensual de Facturas XML (ej. 07 Julio)";
                fbd.ShowNewFolderButton = false;

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtRutaCarpetaXml.Text = fbd.SelectedPath;
                }
            }
        }

        private async void btnConciliar_Click(object sender, EventArgs e)
        {
            string rutaCsv = txtRutaCsv.Text.Trim();
            string rutaCarpeta = txtRutaCarpetaXml.Text.Trim();

            if (string.IsNullOrEmpty(rutaCsv) || !File.Exists(rutaCsv))
            {
                MessageBox.Show("Por favor seleccione un archivo CSV válido.", "Archivo no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(rutaCarpeta) || !Directory.Exists(rutaCarpeta))
            {
                MessageBox.Show("Por favor seleccione una carpeta válida con archivos XML.", "Carpeta no encontrada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnConciliar.Enabled = false;
            btnSeleccionarCsv.Enabled = false;
            btnSeleccionarCarpetaXml.Enabled = false;
            btnExportar.Enabled = false;

            progressBar1.Visible = true;
            progressBar1.Value = 0;
            lblEstado.Text = "Leyendo operaciones del estado de cuenta CSV...";

            try
            {
                var progreso = new Progress<int>(v => progressBar1.Value = Math.Min(100, Math.Max(0, v)));

                var resultado = await Task.Run(() => ProcesarConciliacion(rutaCsv, rutaCarpeta, progreso));

                _listaConciliacion = resultado.ItemsConciliacion;
                _listaFacturasXml = resultado.FacturasXml;

                MostrarResultadosEnGrids();

                lblEstado.Text = $"Conciliación finalizada. Coincidentes: {resultado.Coincidentes} | Faltantes: {resultado.Faltantes} | XMLs sin cargo: {resultado.Huerfanos}";

                MessageBox.Show(
                    $"¡Conciliación Completada!\n\n" +
                    $"• Total de Operaciones en CSV: {resultado.ItemsConciliacion.Count}\n" +
                    $"• Facturas XML Encontradas: {resultado.Coincidentes}\n" +
                    $"• Operaciones Faltantes de Factura: {resultado.Faltantes}\n" +
                    $"• Facturas XML sin cargo en CSV: {resultado.Huerfanos}",
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
                btnSeleccionarCarpetaXml.Enabled = true;
                btnExportar.Enabled = true;
                progressBar1.Visible = false;
            }
        }

        private ResultadoProceso ProcesarConciliacion(string rutaCsv, string rutaCarpetaXml, IProgress<int> progreso)
        {
            progreso?.Report(10);
            var operaciones = ParsearCsvOperaciones(rutaCsv);

            progreso?.Report(30);
            var facturasXml = EscanearCarpetaXml(rutaCarpetaXml);

            progreso?.Report(60);

            // Algoritmo de emparejamiento inteligente
            var itemsConciliados = new List<ItemConciliacion>();

            foreach (var op in operaciones)
            {
                var item = new ItemConciliacion
                {
                    Fecha = op.FechaTexto,
                    FechaCompra = op.FechaCompraTexto,
                    Descripcion = op.Descripcion,
                    ImporteCsv = op.Importe,
                    FechaCompraDate = op.FechaCompraDate
                };

                // Si es abono / pago de tarjeta negativo grande, se etiqueta
                if (op.Importe < 0 && (op.Descripcion.IndexOf("GRACIAS POR SU PAGO", StringComparison.OrdinalIgnoreCase) >= 0 || op.Importe < -10000))
                {
                    item.EstadoFactura = "ℹ️ PAGO DE TARJETA / ABONO";
                    itemsConciliados.Add(item);
                    continue;
                }

                // Buscar facturas disponibles con el mismo importe (tolerancia de 5 centavos)
                decimal importeAbs = Math.Abs(op.Importe);
                var candidatas = facturasXml
                    .Where(f => !f.Asignada && Math.Abs(f.Total - importeAbs) <= 0.05m)
                    .ToList();

                if (candidatas.Count == 1)
                {
                    var f = candidatas[0];
                    f.Asignada = true;
                    AsignarFacturaAItem(item, f);
                }
                else if (candidatas.Count > 1)
                {
                    // Desempate por afinidad de texto en descripción y cercanía de fecha
                    FacturaXmlItem mejorCandidata = null;
                    int mejorPuntaje = -1;

                    foreach (var f in candidatas)
                    {
                        int puntaje = 0;

                        // Coincidencia de palabras clave del emisor con la descripción del CSV
                        if (!string.IsNullOrEmpty(f.EmisorNombre))
                        {
                            var palabras = f.EmisorNombre.Split(new[] { ' ', '.', ',', '-' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var p in palabras)
                            {
                                if (p.Length >= 4 && op.Descripcion.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    puntaje += 50;
                                }
                            }
                        }

                        // Cercanía de fecha
                        if (op.FechaCompraDate.HasValue && f.FechaCfdi.HasValue)
                        {
                            int diasDiferencia = Math.Abs((op.FechaCompraDate.Value - f.FechaCfdi.Value).Days);
                            if (diasDiferencia <= 2) puntaje += 30;
                            else if (diasDiferencia <= 5) puntaje += 15;
                            else if (diasDiferencia <= 10) puntaje += 5;
                        }

                        if (puntaje > mejorPuntaje)
                        {
                            mejorPuntaje = puntaje;
                            mejorCandidata = f;
                        }
                    }

                    if (mejorCandidata == null) mejorCandidata = candidatas[0];

                    mejorCandidata.Asignada = true;
                    AsignarFacturaAItem(item, mejorCandidata);
                }
                else
                {
                    item.EstadoFactura = "❌ FALTA XML";
                }

                itemsConciliados.Add(item);
            }

            progreso?.Report(100);

            int coincidentes = itemsConciliados.Count(i => i.EstadoFactura.StartsWith("✅"));
            int faltantes = itemsConciliados.Count(i => i.EstadoFactura.StartsWith("❌"));
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

        private void AsignarFacturaAItem(ItemConciliacion item, FacturaXmlItem f)
        {
            item.EstadoFactura = "✅ FACTURA ENCONTRADA";
            item.TotalXml = f.Total;
            item.Diferencia = Math.Abs(item.ImporteCsv - f.Total);
            item.UUID = f.UUID;
            item.Emisor = f.EmisorNombre;
            item.ArchivoXml = f.NombreArchivo;
            item.Subcarpeta = f.Subcarpeta;
            item.RutaXml = f.RutaCompleta;
            item.MetodoPago = f.MetodoPago;
            item.FormaPago = f.FormaPago;
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
            var archivos = Directory.EnumerateFiles(rutaCarpeta, "*.*", SearchOption.AllDirectories)
                .Where(f => f.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                .ToList();

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

                        string uuid = tfdNode?.Attributes?["UUID"]?.Value ?? "";
                        string fechaStr = compNode.Attributes?["Fecha"]?.Value ?? "";
                        DateTime? fechaCfdi = null;
                        if (DateTime.TryParse(fechaStr, out DateTime dt)) fechaCfdi = dt;

                        string subcarpeta = Path.GetFileName(Path.GetDirectoryName(archivo)) ?? "";

                        lista.Add(new FacturaXmlItem
                        {
                            UUID = uuid,
                            Total = total,
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

        private void MostrarResultadosEnGrids()
        {
            // Métricas
            int totalCsv = _listaConciliacion.Count;
            int totalXmls = _listaFacturasXml.Count;
            int coincidentes = _listaConciliacion.Count(i => i.EstadoFactura.StartsWith("✅"));
            int faltantes = _listaConciliacion.Count(i => i.EstadoFactura.StartsWith("❌"));
            int huerfanos = _listaFacturasXml.Count(f => !f.Asignada);

            lblTotalCsv.Text = $"Operaciones CSV: {totalCsv}";
            lblTotalXmls.Text = $"XMLs en Carpeta: {totalXmls}";
            lblCoincidentes.Text = $"✅ Con Factura XML: {coincidentes}";
            lblFaltantes.Text = $"❌ Faltantes de XML: {faltantes}";
            lblHuerfanos.Text = $"⚠️ XMLs sin Cargo CSV: {huerfanos}";

            // Pestaña 1: Todas
            gridTodas.DataSource = null;
            gridTodas.DataSource = _listaConciliacion;
            ConfigurarFormatoGrid(gridTodas);

            // Pestaña 2: Faltantes
            gridFaltantes.DataSource = null;
            gridFaltantes.DataSource = _listaConciliacion.Where(i => i.EstadoFactura.StartsWith("❌")).ToList();
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
                grid.Columns["EstadoFactura"].Width = 190;
            }
            if (grid.Columns["Fecha"] != null)
            {
                grid.Columns["Fecha"].HeaderText = "Fecha Reg.";
                grid.Columns["Fecha"].Width = 100;
            }
            if (grid.Columns["FechaCompra"] != null)
            {
                grid.Columns["FechaCompra"].HeaderText = "Fecha Compra";
                grid.Columns["FechaCompra"].Width = 110;
            }
            if (grid.Columns["Descripcion"] != null)
            {
                grid.Columns["Descripcion"].HeaderText = "Descripción del Cargo (Banco)";
                grid.Columns["Descripcion"].Width = 320;
            }
            if (grid.Columns["ImporteCsv"] != null)
            {
                grid.Columns["ImporteCsv"].HeaderText = "Cargo CSV";
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
            if (grid.Columns["Diferencia"] != null)
            {
                grid.Columns["Diferencia"].HeaderText = "Diferencia";
                grid.Columns["Diferencia"].DefaultCellStyle.Format = "C2";
                grid.Columns["Diferencia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                grid.Columns["Diferencia"].Width = 90;
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

            // Colorear filas
            foreach (DataGridViewRow row in grid.Rows)
            {
                string estado = row.Cells["EstadoFactura"]?.Value?.ToString() ?? "";
                if (estado.StartsWith("✅"))
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(240, 253, 244); // Verde claro suave
                    row.Cells["EstadoFactura"].Style.ForeColor = Color.DarkGreen;
                    row.Cells["EstadoFactura"].Style.Font = new Font(grid.Font, FontStyle.Bold);
                }
                else if (estado.StartsWith("❌"))
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(254, 242, 242); // Rojo claro suave
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
                        sb.AppendLine("Estado Factura,Fecha Registro,Fecha Compra,Descripción Banco,Cargo CSV,Total XML,Diferencia,Emisor XML,UUID,Archivo XML,Subcarpeta");

                        foreach (var i in _listaConciliacion)
                        {
                            sb.AppendLine(
                                $"\"{EscaparCsv(i.EstadoFactura)}\"," +
                                $"\"{EscaparCsv(i.Fecha)}\"," +
                                $"\"{EscaparCsv(i.FechaCompra)}\"," +
                                $"\"{EscaparCsv(i.Descripcion)}\"," +
                                $"{i.ImporteCsv.ToString(CultureInfo.InvariantCulture)}," +
                                $"{(i.TotalXml.HasValue ? i.TotalXml.Value.ToString(CultureInfo.InvariantCulture) : "")}," +
                                $"{(i.Diferencia.HasValue ? i.Diferencia.Value.ToString(CultureInfo.InvariantCulture) : "")}," +
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

    public class OperacionCsv
    {
        public string FechaTexto { get; set; } = "";
        public string FechaCompraTexto { get; set; } = "";
        public DateTime? FechaCompraDate { get; set; }
        public string Descripcion { get; set; } = "";
        public decimal Importe { get; set; }
    }

    public class FacturaXmlItem
    {
        public string UUID { get; set; } = "";
        public decimal Total { get; set; }
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
        public decimal? TotalXml { get; set; }
        public decimal? Diferencia { get; set; }
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
