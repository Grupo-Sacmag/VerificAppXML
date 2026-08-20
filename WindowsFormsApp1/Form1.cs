using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private AntigravityXmlValidator _validador;
        private int _indiceAnteriorCombo = 0;
        private List<ResultadoValidacion> _ultimosResultados = new List<ResultadoValidacion>();
        private GridFiltroColumnas<ResultadoValidacion> _filtroValidas;
        private GridFiltroColumnas<ResultadoValidacion> _filtroInvalidas;

        public Form1()
        {
            InitializeComponent();
            _validador = new AntigravityXmlValidator();

            _filtroValidas = new GridFiltroColumnas<ResultadoValidacion>(gridValidas, panelFiltrosValidas);
            _filtroInvalidas = new GridFiltroColumnas<ResultadoValidacion>(gridInvalidas, panelFiltrosInvalidas);

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
                _indiceAnteriorCombo = 0;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool tieneDatos = (gridValidas.Rows.Count > 0 || gridInvalidas.Rows.Count > 0);

            if (tieneDatos)
            {
                var resp = MessageBox.Show(
                    "¿SEGURO QUE QUIERE CAMBIAR?, LOS DATOS SE BORRARÁN",
                    "Confirmar cambio de modo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (resp == DialogResult.No)
                {
                    // Revertir la selección del ComboBox sin disparar evento recursivo
                    comboBox1.SelectedIndexChanged -= comboBox1_SelectedIndexChanged;
                    comboBox1.SelectedIndex = _indiceAnteriorCombo;
                    comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
                    return;
                }

                // Si elige SÍ, limpiar todos los datos en pantalla
                LimpiarDatosPantalla();
            }

            _indiceAnteriorCombo = comboBox1.SelectedIndex;

            string opcionSeleccionada = comboBox1.SelectedItem != null ? comboBox1.SelectedItem.ToString() : "";

            if (opcionSeleccionada.Equals("Por Carpeta", StringComparison.OrdinalIgnoreCase))
            {
                btnSeleccionarCarpeta.Enabled = true;
                btnIniciar.Enabled = true;
                lblEstado.Text = "Modo 'Por Carpeta' seleccionado. Seleccione un directorio para iniciar.";
            }
            else if (opcionSeleccionada.Equals("Por Pila", StringComparison.OrdinalIgnoreCase) ||
                     opcionSeleccionada.Equals("Por Lote", StringComparison.OrdinalIgnoreCase))
            {
                btnSeleccionarCarpeta.Enabled = true;
                btnIniciar.Enabled = true;
                lblEstado.Text = "Modo 'Por Pila' seleccionado. Seleccione el directorio raíz con las subcarpetas.";
            }
            else
            {
                btnSeleccionarCarpeta.Enabled = false;
                btnIniciar.Enabled = false;
                lblEstado.Text = "Seleccione una opción del menú.";
            }
        }

        private void LimpiarDatosPantalla()
        {
            gridValidas.DataSource = null;
            gridInvalidas.DataSource = null;
            txtRutaCarpeta.Text = "";
            lblValidas.Text = "Facturas Válidas (0)";
            lblInvalidas.Text = "Facturas Inválidas / Inconsistencias (0)";
            progressBar1.Value = 0;
            progressBar1.Visible = false;
            lblEstado.Text = "Estado: Listo. Seleccione una opción.";
        }

        private void btnSeleccionarCarpeta_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtRutaCarpeta.Text = folderBrowserDialog1.SelectedPath;
                lblEstado.Text = $"Carpeta seleccionada: {Path.GetFileName(folderBrowserDialog1.SelectedPath)}";
            }
        }

        private async void btnIniciar_Click(object sender, EventArgs e)
        {
            string rutaCarpeta = txtRutaCarpeta.Text.Trim();

            if (string.IsNullOrEmpty(rutaCarpeta) || !Directory.Exists(rutaCarpeta))
            {
                MessageBox.Show("Por favor seleccione un directorio válido antes de iniciar.", "Carpeta no seleccionada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string modo = comboBox1.SelectedItem != null ? comboBox1.SelectedItem.ToString() : "Por Carpeta";
            bool esModoPila = modo.Equals("Por Pila", StringComparison.OrdinalIgnoreCase) || modo.Equals("Por Lote", StringComparison.OrdinalIgnoreCase);

            // Deshabilitar controles y MOSTRAR la barra de progreso al pie
            btnIniciar.Enabled = false;
            btnSeleccionarCarpeta.Enabled = false;
            comboBox1.Enabled = false;
            progressBar1.Value = 0;
            progressBar1.Visible = true;

            // Limpiar resultados anteriores
            gridValidas.DataSource = null;
            gridInvalidas.DataSource = null;
            lblValidas.Text = "Facturas Válidas (0)";
            lblInvalidas.Text = "Facturas Inválidas / Inconsistencias (0)";

            try
            {
                lblEstado.Text = "Inicializando motor XSLT del SAT...";
                await _validador.InicializarAsync(rutaCarpeta);

                lblEstado.Text = esModoPila ? "Procesando subcarpetas en lote (Por Pila)..." : "Procesando carpeta...";
                var progreso = new Progress<int>(porcentaje =>
                {
                    progressBar1.Value = Math.Min(100, Math.Max(0, porcentaje));
                });

                List<ResultadoValidacion> resultados;
                if (esModoPila)
                {
                    resultados = await _validador.ProcesarPilaAsync(rutaCarpeta, progreso);
                }
                else
                {
                    resultados = await _validador.ProcesarCarpetaAsync(rutaCarpeta, progreso);
                }

                _ultimosResultados = resultados ?? new List<ResultadoValidacion>();

                // Separar facturas 100% válidas de las inválidas/inconsistentes/dañadas
                var validas = resultados.Where(r => r.EsValido == "SÍ").ToList();
                var invalidas = resultados.Where(r => r.EsValido != "SÍ").ToList();

                // Asignar fuentes de datos a los DataGridViews
                _filtroValidas.EstablecerDatos(validas);
                _filtroInvalidas.EstablecerDatos(invalidas);

                AjustarFormatoGrid(gridValidas);
                AjustarFormatoGrid(gridInvalidas);

                lblValidas.Text = $"Facturas Válidas ({validas.Count})";
                lblInvalidas.Text = $"Facturas Inválidas / Inconsistencias ({invalidas.Count})";

                string msjFinal = esModoPila
                    ? $"Proceso por Pila finalizado. Válidas: {validas.Count} | Inválidas: {invalidas.Count} | Reportes .txt generados en cada subcarpeta y en la raíz."
                    : $"Proceso finalizado. Válidas: {validas.Count} | Inválidas: {invalidas.Count} | Reporte Diagnostico_CFDI.txt generado.";

                lblEstado.Text = msjFinal;
                progressBar1.Value = 100;

                MessageBox.Show(msjFinal, "Proceso Completado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblEstado.Text = "Error en el procesamiento.";
                MessageBox.Show($"Ocurrió un error al procesar: {ex.Message}", "Error de Procesamiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnIniciar.Enabled = true;
                btnSeleccionarCarpeta.Enabled = true;
                comboBox1.Enabled = true;
            }
        }

        private void AjustarFormatoGrid(DataGridView grid)
        {
            if (grid == null || grid.Columns.Count == 0) return;

            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            grid.ScrollBars = ScrollBars.Both;

            if (grid.Columns["NombreCarpeta"] != null)
            {
                grid.Columns["NombreCarpeta"].HeaderText = "Subcarpeta";
                grid.Columns["NombreCarpeta"].Width = 180;
                grid.Columns["NombreCarpeta"].DisplayIndex = 0;
            }
            if (grid.Columns["NombreArchivo"] != null)
            {
                grid.Columns["NombreArchivo"].HeaderText = "Archivo";
                grid.Columns["NombreArchivo"].Width = 240;
                grid.Columns["NombreArchivo"].DisplayIndex = 1;
            }
            if (grid.Columns["UUID"] != null)
            {
                grid.Columns["UUID"].HeaderText = "UUID (Folio Fiscal)";
                grid.Columns["UUID"].Width = 270;
            }
            if (grid.Columns["FormaPago"] != null)
            {
                grid.Columns["FormaPago"].HeaderText = "Forma Pago";
                grid.Columns["FormaPago"].Width = 100;
            }
            if (grid.Columns["MetodoPago"] != null)
            {
                grid.Columns["MetodoPago"].HeaderText = "Método Pago";
                grid.Columns["MetodoPago"].Width = 100;
            }
            if (grid.Columns["EsValido"] != null)
            {
                grid.Columns["EsValido"].HeaderText = "Es Válido";
                grid.Columns["EsValido"].Width = 110;
            }
            if (grid.Columns["TienePdf"] != null)
            {
                grid.Columns["TienePdf"].HeaderText = "Tiene PDF";
                grid.Columns["TienePdf"].Width = 110;
            }
            if (grid.Columns["Diagnostico"] != null)
            {
                grid.Columns["Diagnostico"].HeaderText = "Diagnóstico del Sistema";
                grid.Columns["Diagnostico"].Width = 450;
            }
            if (grid.Columns["DetalleError"] != null)
            {
                grid.Columns["DetalleError"].HeaderText = "Detalle Técnico";
                grid.Columns["DetalleError"].Width = 450;
            }
            if (grid.Columns["Moneda"] != null)
            {
                grid.Columns["Moneda"].HeaderText = "Moneda";
                grid.Columns["Moneda"].Width = 70;
            }
            if (grid.Columns["Subtotal"] != null)
            {
                grid.Columns["Subtotal"].HeaderText = "Subtotal";
                grid.Columns["Subtotal"].Width = 110;
                grid.Columns["Subtotal"].DefaultCellStyle.Format = "N2";
                grid.Columns["Subtotal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            if (grid.Columns["Descuento"] != null)
            {
                grid.Columns["Descuento"].HeaderText = "Descuento";
                grid.Columns["Descuento"].Width = 100;
                grid.Columns["Descuento"].DefaultCellStyle.Format = "N2";
                grid.Columns["Descuento"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                grid.Columns["Descuento"].DefaultCellStyle.NullValue = "—";
            }
            if (grid.Columns["IVA"] != null)
            {
                grid.Columns["IVA"].HeaderText = "IVA";
                grid.Columns["IVA"].Width = 100;
                grid.Columns["IVA"].DefaultCellStyle.Format = "N2";
                grid.Columns["IVA"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                grid.Columns["IVA"].DefaultCellStyle.NullValue = "—";
            }
            if (grid.Columns["ImpuestosTrasladados"] != null)
            {
                grid.Columns["ImpuestosTrasladados"].HeaderText = "Imp. Trasladados";
                grid.Columns["ImpuestosTrasladados"].Width = 120;
                grid.Columns["ImpuestosTrasladados"].DefaultCellStyle.Format = "N2";
                grid.Columns["ImpuestosTrasladados"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                grid.Columns["ImpuestosTrasladados"].DefaultCellStyle.NullValue = "—";
            }
            if (grid.Columns["ImpuestosRetenidos"] != null)
            {
                grid.Columns["ImpuestosRetenidos"].HeaderText = "Imp. Retenidos";
                grid.Columns["ImpuestosRetenidos"].Width = 120;
                grid.Columns["ImpuestosRetenidos"].DefaultCellStyle.Format = "N2";
                grid.Columns["ImpuestosRetenidos"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                grid.Columns["ImpuestosRetenidos"].DefaultCellStyle.NullValue = "—";
            }
            if (grid.Columns["Total"] != null)
            {
                grid.Columns["Total"].HeaderText = "Total";
                grid.Columns["Total"].Width = 110;
                grid.Columns["Total"].DefaultCellStyle.Format = "N2";
                grid.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            if (grid.Columns["Serie"] != null) { grid.Columns["Serie"].HeaderText = "Serie"; grid.Columns["Serie"].Width = 70; }
            if (grid.Columns["Folio"] != null) { grid.Columns["Folio"].HeaderText = "Folio"; grid.Columns["Folio"].Width = 90; }
            if (grid.Columns["Fecha"] != null) { grid.Columns["Fecha"].HeaderText = "Fecha Emisión"; grid.Columns["Fecha"].Width = 140; }
            if (grid.Columns["RfcEmisor"] != null) { grid.Columns["RfcEmisor"].HeaderText = "RFC Emisor"; grid.Columns["RfcEmisor"].Width = 110; }
            if (grid.Columns["NombreEmisor"] != null) { grid.Columns["NombreEmisor"].HeaderText = "Nombre Emisor"; grid.Columns["NombreEmisor"].Width = 220; }
            if (grid.Columns["RfcReceptor"] != null) { grid.Columns["RfcReceptor"].HeaderText = "RFC Receptor"; grid.Columns["RfcReceptor"].Width = 110; }
            if (grid.Columns["UsoCFDI"] != null) { grid.Columns["UsoCFDI"].HeaderText = "Uso CFDI"; grid.Columns["UsoCFDI"].Width = 90; }
            if (grid.Columns["ConceptosDescripcion"] != null) { grid.Columns["ConceptosDescripcion"].HeaderText = "Conceptos"; grid.Columns["ConceptosDescripcion"].Width = 350; }
            if (grid.Columns["NumConceptos"] != null) { grid.Columns["NumConceptos"].HeaderText = "# Conceptos"; grid.Columns["NumConceptos"].Width = 90; }
            if (grid.Columns["LugarExpedicion"] != null) { grid.Columns["LugarExpedicion"].HeaderText = "C.P. Expedición"; grid.Columns["LugarExpedicion"].Width = 100; }
            if (grid.Columns["CondicionesDePago"] != null) { grid.Columns["CondicionesDePago"].HeaderText = "Condiciones Pago"; grid.Columns["CondicionesDePago"].Width = 150; }
        }

        private void detallesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_ultimosResultados == null || _ultimosResultados.Count == 0)
            {
                MessageBox.Show("No hay resultados cargados. Ejecute una validación primero.", "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var frm = new FormDetalles(_ultimosResultados))
            {
                frm.ShowDialog(this);
            }
        }

        private void invalidosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_ultimosResultados == null || _ultimosResultados.Count == 0)
            {
                MessageBox.Show("No hay resultados cargados. Ejecute una validación primero.", "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var invalidos = _ultimosResultados.Where(r => r.EsValido != "SÍ").ToList();
            if (invalidos.Count == 0)
            {
                MessageBox.Show("¡Excelente! No hay facturas inválidas en la última ejecución.", "Facturas 100% Válidas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var frm = new FormInvalidos(_ultimosResultados))
            {
                frm.ShowDialog(this);
            }
        }

        private void btnArreglarFallos_Click(object sender, EventArgs e)
        {
        }

        private void btnSeleccionarExcel_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "Archivos Excel (*.xlsx)|*.xlsx" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtRutaExcel.Text = ofd.FileName;
                    lblEstado.Text = $"Excel seleccionado: {Path.GetFileName(ofd.FileName)}";
                }
            }
        }

        private void btnComparar_Click(object sender, EventArgs e)
        {
            if (_ultimosResultados == null || _ultimosResultados.Count == 0)
            {
                MessageBox.Show("No hay resultados de validación cargados. Ejecute una validación primero.", "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var fuentes = new List<(string NombreFuente, string RutaExcel)>();

            if (!string.IsNullOrWhiteSpace(txtRutaExcel.Text) && File.Exists(txtRutaExcel.Text))
                fuentes.Add(("Excel 1: " + Path.GetFileName(txtRutaExcel.Text), txtRutaExcel.Text));

            if (!string.IsNullOrWhiteSpace(txtRutaExcel2.Text) && File.Exists(txtRutaExcel2.Text))
                fuentes.Add(("Excel 2: " + Path.GetFileName(txtRutaExcel2.Text), txtRutaExcel2.Text));

            if (fuentes.Count == 0)
            {
                MessageBox.Show("Seleccione al menos un archivo Excel válido antes de comparar.", "Excel no seleccionado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var comparacion = ExcelComparador.Comparar(_ultimosResultados, fuentes);
                using (var frm = new FormComparacion(comparacion))
                {
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al comparar contra el Excel: {ex.Message}", "Error de Comparación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsAuditoria_Click(object sender, EventArgs e)
        {
            if (_ultimosResultados == null || _ultimosResultados.Count == 0)
            {
                MessageBox.Show("No hay resultados cargados. Ejecute una validación primero.", "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var frm = new FormAuditoria(_ultimosResultados))
            {
                frm.ShowDialog(this);
            }
        }

        private void btnSeleccionarExcel2_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "Archivos Excel (*.xlsx)|*.xlsx" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtRutaExcel2.Text = ofd.FileName;
                    lblEstado.Text = $"Excel 2 seleccionado: {Path.GetFileName(ofd.FileName)}";
                }
            }
        }
    }
}
