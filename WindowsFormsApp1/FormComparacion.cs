using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FormComparacion : Form
    {
        private readonly List<FilaComparacion> _todas;
        private GridFiltroColumnas<FilaComparacion> _filtroColumnas;

        public FormComparacion(List<FilaComparacion> filas)
        {
            InitializeComponent();
            _todas = filas ?? new List<FilaComparacion>();

            // Se crea PRIMERO, antes de que cualquier combo dispare su evento.
            _filtroColumnas = new GridFiltroColumnas<FilaComparacion>(gridComparacion, panelFiltrosColumnas);

            cmbFuente.Items.Add("(Todas)");
            cmbFuente.Items.AddRange(_todas.Select(f => f.FuenteExcel).Distinct().OrderBy(x => x).Cast<object>().ToArray());
            cmbFuente.SelectedIndex = 0;

            cmbResultado.Items.Add("(Todos)");
            cmbResultado.Items.AddRange(_todas.Select(f => f.Resultado).Distinct().OrderBy(x => x).Cast<object>().ToArray());
            cmbResultado.SelectedIndex = 0;

            AplicarFiltroSuperior();
        }

        private void FiltroSuperior_Changed(object sender, EventArgs e)
        {
            AplicarFiltroSuperior();
        }

        private void AplicarFiltroSuperior()
        {
            IEnumerable<FilaComparacion> filtrado = _todas;

            if (cmbFuente.SelectedIndex > 0)
            {
                string fuente = cmbFuente.SelectedItem.ToString();
                filtrado = filtrado.Where(f => f.FuenteExcel == fuente);
            }

            if (cmbResultado.SelectedIndex > 0)
            {
                string resultado = cmbResultado.SelectedItem.ToString();
                filtrado = filtrado.Where(f => f.Resultado == resultado);
            }

            if (!string.IsNullOrWhiteSpace(txtBuscarUuid.Text))
            {
                string patron = txtBuscarUuid.Text.Trim();
                filtrado = filtrado.Where(f =>
                    (!string.IsNullOrEmpty(f.UUID) && f.UUID.IndexOf(patron, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(f.NombreArchivo) && f.NombreArchivo.IndexOf(patron, StringComparison.OrdinalIgnoreCase) >= 0));
            }

            var lista = filtrado.ToList();
            _filtroColumnas.EstablecerDatos(lista);
            ActualizarResumen(lista);
        }

        private void ActualizarResumen(List<FilaComparacion> lista)
        {
            int coincide = lista.Count(f => f.Resultado == "COINCIDE");
            int diferente = lista.Count(f => f.Resultado == "DIFERENTE");
            int noEncontrado = lista.Count(f => f.Resultado == "NO ENCONTRADO EN EXCEL");
            int noMapeado = lista.Count(f => f.Resultado == "CAMPO NO MAPEADO");

            lblResumen.Text = $"Coincide: {coincide} | Diferente: {diferente} | No encontrado: {noEncontrado} | No mapeado: {noMapeado}";
        }

        private void gridComparacion_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (gridComparacion.Columns["FuenteExcel"] != null)
            {
                gridComparacion.Columns["FuenteExcel"].HeaderText = "Fuente Excel";
                gridComparacion.Columns["FuenteExcel"].Width = 180;
                gridComparacion.Columns["FuenteExcel"].DisplayIndex = 0;
            }
            if (gridComparacion.Columns["UUID"] != null)
            {
                gridComparacion.Columns["UUID"].HeaderText = "UUID";
                gridComparacion.Columns["UUID"].Width = 260;
                gridComparacion.Columns["UUID"].DisplayIndex = 1;
            }
            if (gridComparacion.Columns["NombreArchivo"] != null)
            {
                gridComparacion.Columns["NombreArchivo"].HeaderText = "Archivo";
                gridComparacion.Columns["NombreArchivo"].Width = 220;
            }
            if (gridComparacion.Columns["Campo"] != null)
            {
                gridComparacion.Columns["Campo"].HeaderText = "Campo Comparado";
                gridComparacion.Columns["Campo"].Width = 150;
            }
            if (gridComparacion.Columns["ValorXML"] != null)
            {
                gridComparacion.Columns["ValorXML"].HeaderText = "Valor en XML";
                gridComparacion.Columns["ValorXML"].Width = 200;
            }
            if (gridComparacion.Columns["ValorExcel"] != null)
            {
                gridComparacion.Columns["ValorExcel"].HeaderText = "Valor en Excel";
                gridComparacion.Columns["ValorExcel"].Width = 200;
            }
            if (gridComparacion.Columns["Resultado"] != null)
            {
                gridComparacion.Columns["Resultado"].HeaderText = "Resultado";
                gridComparacion.Columns["Resultado"].Width = 170;
            }
        }

        // Resalta visualmente la fila según el resultado, sin tocar los datos.
        private void gridComparacion_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= gridComparacion.Rows.Count) return;

            var item = gridComparacion.Rows[e.RowIndex].DataBoundItem as FilaComparacion;
            if (item == null) return;

            switch (item.Resultado)
            {
                case "DIFERENTE":
                    gridComparacion.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.MistyRose;
                    break;
                case "NO ENCONTRADO EN EXCEL":
                    gridComparacion.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                    break;
                case "CAMPO NO MAPEADO":
                    gridComparacion.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.WhiteSmoke;
                    break;
                default:
                    gridComparacion.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                    break;
            }
        }
    }
}