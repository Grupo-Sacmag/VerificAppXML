using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FormInvalidos : Form
    {
        private readonly List<ResultadoValidacion> _listaInvalidos;

        public FormInvalidos(List<ResultadoValidacion> resultados)
        {
            InitializeComponent();
            _listaInvalidos = resultados != null 
                ? resultados.Where(r => r.EsValido != "SÍ").ToList() 
                : new List<ResultadoValidacion>();
            CargarGrid();
        }

        private void CargarGrid()
        {
            gridInvalidos.DataSource = null;

            var listaSimplificada = _listaInvalidos.Select(r => new
            {
                Archivo = string.IsNullOrEmpty(r.NombreCarpeta) ? r.NombreArchivo : $"{r.NombreCarpeta}\\{r.NombreArchivo}",
                ObjetoOriginal = r
            }).ToList();

            gridInvalidos.DataSource = listaSimplificada;

            if (gridInvalidos.Columns["ObjetoOriginal"] != null)
            {
                gridInvalidos.Columns["ObjetoOriginal"].Visible = false;
            }
            if (gridInvalidos.Columns["Archivo"] != null)
            {
                gridInvalidos.Columns["Archivo"].HeaderText = "Nombre del XML Inválido";
            }

            if (gridInvalidos.Rows.Count > 0)
            {
                gridInvalidos.Rows[0].Selected = true;
                MostrarXmlCrudoRow(0);
            }
            else
            {
                rtbXmlCrudo.Text = "No se encontraron facturas inválidas o con inconsistencias.";
                lblLeyendaError.Text = "No hay facturas inválidas en la selección actual.";
            }
        }

        private void gridInvalidos_SelectionChanged(object sender, EventArgs e)
        {
            if (gridInvalidos.SelectedRows.Count > 0)
            {
                int index = gridInvalidos.SelectedRows[0].Index;
                MostrarXmlCrudoRow(index);
            }
        }

        private void MostrarXmlCrudoRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= gridInvalidos.Rows.Count) return;

            var item = gridInvalidos.Rows[rowIndex].DataBoundItem;
            if (item == null) return;

            var prop = item.GetType().GetProperty("ObjetoOriginal");
            if (prop != null)
            {
                var r = prop.GetValue(item) as ResultadoValidacion;
                if (r != null)
                {
                    string contenidoXml = r.TextoXmlCrudo;

                    if (string.IsNullOrEmpty(contenidoXml) && !string.IsNullOrEmpty(r.RutaXml) && File.Exists(r.RutaXml))
                    {
                        try { contenidoXml = File.ReadAllText(r.RutaXml); } catch { }
                    }

                    rtbXmlCrudo.Clear();
                    rtbXmlCrudo.Text = contenidoXml ?? "";

                    // Configurar leyenda y resaltar en rojo
                    if (r.TienePdf != "SÍ")
                    {
                        lblLeyendaError.Text = "❗ El error de esta factura es que NO TIENE EL ARCHIVO PDF CORRELACIONADO EN LA CARPETA.";
                        lblLeyendaError.ForeColor = Color.OrangeRed;
                    }
                    else
                    {
                        lblLeyendaError.Text = $"En color rojo se resalta la inconsistencia / alteración ({r.Diagnostico})";
                        lblLeyendaError.ForeColor = Color.DarkRed;
                    }

                    ResaltarErrorEnRed(r);
                }
            }
        }

        private void ResaltarErrorEnRed(ResultadoValidacion r)
        {
            if (rtbXmlCrudo.TextLength == 0) return;

            Font fuenteNormal = new Font("Consolas", 9.5F, FontStyle.Regular);
            Font fuenteBold = new Font("Consolas", 10F, FontStyle.Bold);

            rtbXmlCrudo.SelectAll();
            rtbXmlCrudo.SelectionColor = Color.Black;
            rtbXmlCrudo.SelectionFont = fuenteNormal;
            rtbXmlCrudo.DeselectAll();

            string fragmento = r.FragmentoError;

            // 1. Si hay un fragmento de error definido
            if (!string.IsNullOrEmpty(fragmento) && fragmento != "Falta PDF")
            {
                int index = rtbXmlCrudo.Text.IndexOf(fragmento, StringComparison.OrdinalIgnoreCase);
                if (index >= 0)
                {
                    rtbXmlCrudo.Select(index, fragmento.Length);
                    rtbXmlCrudo.SelectionColor = Color.Red;
                    rtbXmlCrudo.SelectionBackColor = Color.Yellow;
                    rtbXmlCrudo.SelectionFont = fuenteBold;
                    rtbXmlCrudo.ScrollToCaret();
                    return;
                }
            }

            // 2. Si el fallo es por basura al inicio (caracteres antes de <?xml)
            int idxXml = rtbXmlCrudo.Text.IndexOf("<?xml", StringComparison.OrdinalIgnoreCase);
            if (idxXml > 0)
            {
                rtbXmlCrudo.Select(0, idxXml);
                rtbXmlCrudo.SelectionColor = Color.Red;
                rtbXmlCrudo.SelectionBackColor = Color.Yellow;
                rtbXmlCrudo.SelectionFont = fuenteBold;
                rtbXmlCrudo.ScrollToCaret();
                return;
            }

            // 3. Resaltar FormaPago si existe en el texto (solo si el documento realmente está reportado como alterado)
            if (r.EsValido == "NO" && !string.IsNullOrEmpty(r.FormaPago) && r.FormaPago != "N/A" && r.TienePdf == "SÍ")
            {
                string targetFormaPago = $"FormaPago=\"{r.FormaPago}\"";
                int idxForma = rtbXmlCrudo.Text.IndexOf(targetFormaPago, StringComparison.OrdinalIgnoreCase);
                if (idxForma >= 0)
                {
                    rtbXmlCrudo.Select(idxForma, targetFormaPago.Length);
                    rtbXmlCrudo.SelectionColor = Color.Red;
                    rtbXmlCrudo.SelectionBackColor = Color.Yellow;
                    rtbXmlCrudo.SelectionFont = fuenteBold;
                    rtbXmlCrudo.ScrollToCaret();
                }
            }
        }
    }
}
