using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    // Agrega una fila de TextBox encima de un DataGridView, uno por columna,
    // y filtra la lista subyacente por coincidencia de subcadena (case-insensitive).
    public class GridFiltroColumnas<T>
    {
        private readonly DataGridView _grid;
        private readonly Panel _panelFiltros;
        private List<T> _listaCompleta = new List<T>();
        private readonly Dictionary<string, TextBox> _cajasPorColumna = new Dictionary<string, TextBox>();

        public GridFiltroColumnas(DataGridView grid, Panel contenedorFiltros)
        {
            _grid = grid;
            _panelFiltros = contenedorFiltros;
            _grid.ColumnWidthChanged += (s, e) => ReubicarCajas();
            _grid.ColumnDisplayIndexChanged += (s, e) => ReubicarCajas();
            _grid.Scroll += (s, e) => ReubicarCajas();
            _grid.DataBindingComplete += (s, e) => ReubicarCajas();
        }

        public void EstablecerDatos(List<T> lista)
        {
            _listaCompleta = lista ?? new List<T>();
            _grid.DataSource = _listaCompleta;
            ConstruirCajasFiltro();
        }

        private void ConstruirCajasFiltro()
        {
            _panelFiltros.Controls.Clear();
            _cajasPorColumna.Clear();

            foreach (DataGridViewColumn col in _grid.Columns)
            {
                if (!col.Visible) continue;

                var txt = new TextBox
                {
                    Tag = col.Name,
                    Font = new System.Drawing.Font(_grid.Font.FontFamily, 8F)
                };
                txt.TextChanged += (s, e) => AplicarFiltros();
                _cajasPorColumna[col.Name] = txt;
                _panelFiltros.Controls.Add(txt);
            }

            ReubicarCajas();
        }

        private void ReubicarCajas()
        {
            foreach (DataGridViewColumn col in _grid.Columns)
            {
                if (!_cajasPorColumna.TryGetValue(col.Name, out var txt)) continue;
                if (!col.Visible) { txt.Visible = false; continue; }

                var rect = _grid.GetColumnDisplayRectangle(col.Index, false);
                txt.Visible = rect.Width > 0;
                if (rect.Width > 0)
                {
                    txt.Left = rect.Left;
                    txt.Width = rect.Width;
                    txt.Top = 2;
                }
            }
        }

        private void AplicarFiltros()
        {
            IEnumerable<T> resultado = _listaCompleta;

            foreach (var kvp in _cajasPorColumna)
            {
                string texto = kvp.Value.Text;
                if (string.IsNullOrWhiteSpace(texto)) continue;

                PropertyInfo prop = typeof(T).GetProperty(kvp.Key);
                if (prop == null) continue;

                resultado = resultado.Where(item =>
                {
                    object valor = prop.GetValue(item);
                    string valorTexto = valor?.ToString() ?? "";
                    return valorTexto.IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0;
                });
            }

            _grid.DataSource = resultado.ToList();
        }
    }
}