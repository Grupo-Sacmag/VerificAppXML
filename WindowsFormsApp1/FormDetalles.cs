using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FormDetalles : Form
    {
        private readonly List<ResultadoValidacion> _listaResultados;

        public FormDetalles(List<ResultadoValidacion> resultados)
        {
            InitializeComponent();
            _listaResultados = resultados ?? new List<ResultadoValidacion>();
            CargarGrid();
        }

        private void CargarGrid()
        {
            gridXmls.DataSource = null;

            var listaSimplificada = _listaResultados.Select(r => new
            {
                Archivo = string.IsNullOrEmpty(r.NombreCarpeta) ? r.NombreArchivo : $"{r.NombreCarpeta}\\{r.NombreArchivo}",
                ObjetoOriginal = r
            }).ToList();

            gridXmls.DataSource = listaSimplificada;

            if (gridXmls.Columns["ObjetoOriginal"] != null)
            {
                gridXmls.Columns["ObjetoOriginal"].Visible = false;
            }
            if (gridXmls.Columns["Archivo"] != null)
            {
                gridXmls.Columns["Archivo"].HeaderText = "Nombre del XML";
            }

            if (gridXmls.Rows.Count > 0)
            {
                gridXmls.Rows[0].Selected = true;
                MostrarDetallesRow(0);
            }
        }

        private void gridXmls_SelectionChanged(object sender, EventArgs e)
        {
            if (gridXmls.SelectedRows.Count > 0)
            {
                int index = gridXmls.SelectedRows[0].Index;
                MostrarDetallesRow(index);
            }
        }

        private void MostrarDetallesRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= gridXmls.Rows.Count) return;

            var item = gridXmls.Rows[rowIndex].DataBoundItem;
            if (item == null) return;

            var prop = item.GetType().GetProperty("ObjetoOriginal");
            if (prop != null)
            {
                var r = prop.GetValue(item) as ResultadoValidacion;
                if (r != null)
                {
                    txtNombreArchivo.Text = string.IsNullOrEmpty(r.NombreCarpeta) ? r.NombreArchivo : $"{r.NombreCarpeta}\\{r.NombreArchivo}";
                    txtUuid.Text = r.UUID;
                    txtFormaPago.Text = r.FormaPago;
                    txtMetodoPago.Text = r.MetodoPago;
                    txtEsValido.Text = r.EsValido;
                    txtTienePdf.Text = r.TienePdf;
                    txtDiagnostico.Text = r.Diagnostico;
                    txtDetalleTecnico.Text = r.DetalleError;
                    txtSello.Text = r.Sello;
                    txtCertificado.Text = r.Certificado;
                }
            }
        }
    }
}
