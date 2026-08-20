using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FormAuditoria : Form
    {
        private readonly List<ResultadoValidacion> _todos;
        private GridFiltroColumnas<ResultadoValidacion> _filtroColumnas;

        public FormAuditoria(List<ResultadoValidacion> resultados)
        {
            InitializeComponent();
            _todos = resultados ?? new List<ResultadoValidacion>();

            // Se crea PRIMERO, antes de que cmbEstado dispare su evento.
            _filtroColumnas = new GridFiltroColumnas<ResultadoValidacion>(gridAuditoria, panelFiltrosColumnas);

            cmbEstado.Items.Add("(Todos)");
            cmbEstado.Items.AddRange(_todos.Select(r => r.EsValido).Distinct().OrderBy(x => x).Cast<object>().ToArray());
            cmbEstado.SelectedIndex = 0;

            _filtroColumnas.EstablecerDatos(_todos);
        }

        private void cmbEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            AplicarFiltroSuperior();
        }

        private void txtPatronUuid_TextChanged(object sender, EventArgs e)
        {
            AplicarFiltroSuperior();
        }

        private void AplicarFiltroSuperior()
        {
            IEnumerable<ResultadoValidacion> filtrado = _todos;

            if (cmbEstado.SelectedIndex > 0)
            {
                string estado = cmbEstado.SelectedItem.ToString();
                filtrado = filtrado.Where(r => r.EsValido == estado);
            }

            if (!string.IsNullOrWhiteSpace(txtPatronUuid.Text))
            {
                string patron = txtPatronUuid.Text.Trim();
                filtrado = filtrado.Where(r => !string.IsNullOrEmpty(r.UUID) &&
                    r.UUID.IndexOf(patron, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            _filtroColumnas.EstablecerDatos(filtrado.ToList());
        }

        private void gridAuditoria_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (gridAuditoria.Columns["NombreArchivo"] != null)
                gridAuditoria.Columns["NombreArchivo"].HeaderText = "Archivo";
            if (gridAuditoria.Columns["UUID"] != null)
                gridAuditoria.Columns["UUID"].HeaderText = "UUID (Folio Fiscal)";
            if (gridAuditoria.Columns["EsValido"] != null)
                gridAuditoria.Columns["EsValido"].HeaderText = "Estado";
            if (gridAuditoria.Columns["CriterioAplicado"] != null)
            {
                gridAuditoria.Columns["CriterioAplicado"].HeaderText = "Criterio Aplicado";
                gridAuditoria.Columns["CriterioAplicado"].Width = 200;
            }
            if (gridAuditoria.Columns["Justificacion"] != null)
            {
                gridAuditoria.Columns["Justificacion"].HeaderText = "Justificación";
                gridAuditoria.Columns["Justificacion"].Width = 400;
            }
            if (gridAuditoria.Columns["IncluidoPorRegla"] != null)
                gridAuditoria.Columns["IncluidoPorRegla"].HeaderText = "¿Forzado por Regla?";
        }
    }
}