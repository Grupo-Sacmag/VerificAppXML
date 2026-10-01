using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public static class TemaGamerEmpresarial
    {
        // Paleta Sci-Fi Enterprise Command
        public static readonly Color FondoEspacial = Color.FromArgb(11, 15, 25);       // #0B0F19 Base ultra-oscura
        public static readonly Color FondoPanel = Color.FromArgb(15, 23, 42);          // #0F172A Slate Comando
        public static readonly Color FondoTarjeta = Color.FromArgb(30, 41, 59);        // #1E293B Modulo Táctico
        public static readonly Color FondoAlterno = Color.FromArgb(17, 24, 39);         // #111827 Filas alternadas
        public static readonly Color BordeHud = Color.FromArgb(51, 65, 85);            // #334155 Líneas divisorias
        public static readonly Color BordeSutil = Color.FromArgb(30, 41, 59);          // #1E293B

        // Acentos Neón / Holográficos
        public static readonly Color CianNeon = Color.FromArgb(6, 182, 212);           // #06B6D4 Acento primario / Focus
        public static readonly Color CianClaro = Color.FromArgb(56, 189, 248);          // #38BDF8 Encabezados
        public static readonly Color VerdeNeon = Color.FromArgb(16, 185, 129);         // #10B981 Éxito / SAT
        public static readonly Color MoradoNeon = Color.FromArgb(168, 85, 247);        // #A855F7 Amarradas / Conciliadas
        public static readonly Color AmbarNeon = Color.FromArgb(245, 158, 11);         // #F59E0B Diferencias / Pendiente
        public static readonly Color RojoLaser = Color.FromArgb(239, 68, 68);           // #EF4444 Faltantes / Errores
        public static readonly Color IndigoTech = Color.FromArgb(99, 102, 241);        // #6366F1 Comandos especiales
        public static readonly Color AzulSeleccion = Color.FromArgb(3, 105, 161);       // #0369A1 Fila activa

        // Tipografía
        public static readonly Color TextoPrincipal = Color.FromArgb(248, 250, 252);   // #F8FAFC
        public static readonly Color TextoSecundario = Color.FromArgb(148, 163, 184);  // #94A3B8
        public static readonly Color TextoMuted = Color.FromArgb(100, 116, 139);       // #64748B

        public static void AplicarTema(Form form)
        {
            if (form == null) return;

            form.BackColor = FondoEspacial;
            form.ForeColor = TextoPrincipal;
            form.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            AplicarAControles(form.Controls);
        }

        public static void AplicarAControles(Control.ControlCollection controls)
        {
            if (controls == null) return;

            foreach (Control c in controls)
            {
                if (c is MenuStrip menu)
                {
                    menu.BackColor = FondoPanel;
                    menu.ForeColor = TextoPrincipal;
                    menu.Renderer = new DarkGamerMenuRenderer();
                }
                else if (c is ContextMenuStrip contextMenu)
                {
                    contextMenu.BackColor = FondoPanel;
                    contextMenu.ForeColor = TextoPrincipal;
                    contextMenu.Renderer = new DarkGamerMenuRenderer();
                }
                else if (c is DataGridView dgv)
                {
                    EstilizarGrid(dgv);
                }
                else if (c is Button btn)
                {
                    EstilizarBoton(btn);
                }
                else if (c is TabControl tab)
                {
                    EstilizarTabControl(tab);
                }
                else if (c is TextBox txt)
                {
                    txt.BackColor = FondoPanel;
                    txt.ForeColor = TextoPrincipal;
                    txt.BorderStyle = BorderStyle.FixedSingle;
                }
                else if (c is RichTextBox rtb)
                {
                    rtb.BackColor = FondoPanel;
                    rtb.ForeColor = TextoPrincipal;
                    rtb.BorderStyle = BorderStyle.None;
                }
                else if (c is ComboBox cmb)
                {
                    cmb.BackColor = FondoPanel;
                    cmb.ForeColor = TextoPrincipal;
                    cmb.FlatStyle = FlatStyle.Flat;
                }
                else if (c is GroupBox grp)
                {
                    grp.BackColor = FondoPanel;
                    grp.ForeColor = CianClaro;
                    AplicarAControles(grp.Controls);
                }
                else if (c is Panel pnl)
                {
                    if (pnl.Name.Contains("Top") || pnl.Name.Contains("Header") || pnl.Name.Contains("Bottom") || pnl.Name.Contains("Metricas"))
                    {
                        pnl.BackColor = FondoPanel;
                    }
                    AplicarAControles(pnl.Controls);
                }
                else if (c is SplitContainer split)
                {
                    split.BackColor = BordeHud;
                    split.Panel1.BackColor = FondoEspacial;
                    split.Panel2.BackColor = FondoEspacial;
                    AplicarAControles(split.Panel1.Controls);
                    AplicarAControles(split.Panel2.Controls);
                }
                else if (c is Label lbl)
                {
                    if (lbl.Font.Bold || lbl.Font.Size >= 10.5F)
                    {
                        if (lbl.ForeColor == Color.Black || lbl.ForeColor == SystemColors.ControlText)
                            lbl.ForeColor = CianClaro;
                    }
                    else
                    {
                        if (lbl.ForeColor == Color.Black || lbl.ForeColor == SystemColors.ControlText)
                            lbl.ForeColor = TextoSecundario;
                    }
                }

                if (c.HasChildren && !(c is GroupBox) && !(c is Panel) && !(c is SplitContainer))
                {
                    AplicarAControles(c.Controls);
                }
            }
        }

        public static void EstilizarGrid(DataGridView dgv)
        {
            if (dgv == null) return;

            FormConciliacionCsv.HabilitarDobleBuffer(dgv);

            dgv.BackgroundColor = FondoEspacial;
            dgv.GridColor = BordeSutil;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.EnableHeadersVisualStyles = false;

            // Encabezados
            dgv.ColumnHeadersDefaultCellStyle.BackColor = FondoPanel;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = CianClaro;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = FondoPanel;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = CianClaro;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersHeight = 32;

            // Filas
            dgv.DefaultCellStyle.BackColor = FondoEspacial;
            dgv.DefaultCellStyle.ForeColor = TextoPrincipal;
            dgv.DefaultCellStyle.SelectionBackColor = AzulSeleccion;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular);

            // Alternadas
            dgv.AlternatingRowsDefaultCellStyle.BackColor = FondoAlterno;
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = TextoPrincipal;
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = AzulSeleccion;
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            // Fila de encabezado de fila (lateral)
            dgv.RowHeadersDefaultCellStyle.BackColor = FondoPanel;
            dgv.RowHeadersDefaultCellStyle.ForeColor = TextoSecundario;
            dgv.RowHeadersDefaultCellStyle.SelectionBackColor = AzulSeleccion;
            dgv.RowHeadersWidth = 28;
        }

        public static void EstilizarBoton(Button btn)
        {
            if (btn == null) return;

            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);

            string texto = (btn.Text ?? "").ToUpperInvariant();

            if (texto.Contains("INICIAR") || texto.Contains("CONCILIAR") || texto.Contains("COINCIDENCIAS"))
            {
                btn.BackColor = CianNeon;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(8, 145, 178);
            }
            else if (texto.Contains("ARREGLAR") || texto.Contains("BORRAR") || texto.Contains("ELIMINAR") || texto.Contains("INVALIDOS"))
            {
                btn.BackColor = RojoLaser;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 38, 38);
            }
            else if (texto.Contains("EXCEL") || texto.Contains("EXPORTAR"))
            {
                btn.BackColor = IndigoTech;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(79, 70, 229);
            }
            else if (texto.Contains("COMPARAR") || texto.Contains("ASOCIAR") || texto.Contains("GUARDAR"))
            {
                btn.BackColor = CianNeon;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(8, 145, 178);
            }
            else
            {
                btn.BackColor = FondoTarjeta;
                btn.ForeColor = TextoPrincipal;
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.BorderColor = BordeHud;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(51, 65, 85);
            }
        }

        public static void EstilizarTabControl(TabControl tab)
        {
            if (tab == null) return;

            tab.SizeMode = TabSizeMode.Fixed;
            tab.ItemSize = new Size(245, 34);

            foreach (TabPage page in tab.TabPages)
            {
                page.BackColor = FondoEspacial;
                page.ForeColor = TextoPrincipal;
                AplicarAControles(page.Controls);
            }
        }
    }

    public class DarkGamerTabControl : TabControl
    {
        public DarkGamerTabControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            SizeMode = TabSizeMode.Fixed;
            ItemSize = new Size(245, 34);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Fondo completo de la barra de pestañas (elimina la franja blanca)
            using (var brush = new SolidBrush(TemaGamerEmpresarial.FondoPanel))
            {
                e.Graphics.FillRectangle(brush, ClientRectangle);
            }

            // Dibujar cada pestaña
            for (int i = 0; i < TabCount; i++)
            {
                Rectangle rect = GetTabRect(i);
                bool isSelected = (SelectedIndex == i);

                // Fondo de la pestaña
                using (var brush = new SolidBrush(isSelected ? TemaGamerEmpresarial.FondoTarjeta : TemaGamerEmpresarial.FondoPanel))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }

                // Línea indicadora cian neón en la pestaña activa
                if (isSelected)
                {
                    using (var pen = new Pen(TemaGamerEmpresarial.CianNeon, 3f))
                    {
                        e.Graphics.DrawLine(pen, rect.Left + 4, rect.Bottom - 1, rect.Right - 4, rect.Bottom - 1);
                    }
                }

                // Borde derecho sutil entre pestañas
                using (var pen = new Pen(TemaGamerEmpresarial.BordeSutil, 1f))
                {
                    e.Graphics.DrawLine(pen, rect.Right - 1, rect.Top + 4, rect.Right - 1, rect.Bottom - 4);
                }

                // Texto de la pestaña
                Color textColor = isSelected ? Color.White : TemaGamerEmpresarial.TextoSecundario;
                Font font = isSelected ? new Font("Segoe UI", 9F, FontStyle.Bold) : new Font("Segoe UI", 8.5F, FontStyle.Regular);

                TextRenderer.DrawText(
                    e.Graphics,
                    TabPages[i].Text,
                    font,
                    rect,
                    textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine
                );
            }

            // Línea divisoria inferior entre pestañas y contenido
            if (TabCount > 0)
            {
                int tabBottom = GetTabRect(0).Bottom;
                using (var pen = new Pen(TemaGamerEmpresarial.BordeHud, 1f))
                {
                    e.Graphics.DrawLine(pen, 0, tabBottom, Width, tabBottom);
                }
            }
        }
    }

    public class DarkGamerMenuRenderer : ToolStripProfessionalRenderer
    {
        public DarkGamerMenuRenderer() : base(new DarkGamerColorTable()) { }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = e.Item.Selected ? Color.White : TemaGamerEmpresarial.TextoPrincipal;
            base.OnRenderItemText(e);
        }
    }

    public class DarkGamerColorTable : ProfessionalColorTable
    {
        public override Color MenuStripGradientBegin => TemaGamerEmpresarial.FondoPanel;
        public override Color MenuStripGradientEnd => TemaGamerEmpresarial.FondoPanel;
        public override Color ToolStripDropDownBackground => TemaGamerEmpresarial.FondoPanel;
        public override Color MenuItemSelected => TemaGamerEmpresarial.AzulSeleccion;
        public override Color MenuItemSelectedGradientBegin => TemaGamerEmpresarial.AzulSeleccion;
        public override Color MenuItemSelectedGradientEnd => TemaGamerEmpresarial.AzulSeleccion;
        public override Color MenuItemPressedGradientBegin => TemaGamerEmpresarial.AzulSeleccion;
        public override Color MenuItemPressedGradientEnd => TemaGamerEmpresarial.AzulSeleccion;
        public override Color MenuBorder => TemaGamerEmpresarial.BordeHud;
        public override Color MenuItemBorder => TemaGamerEmpresarial.CianNeon;
        public override Color ImageMarginGradientBegin => TemaGamerEmpresarial.FondoPanel;
        public override Color ImageMarginGradientMiddle => TemaGamerEmpresarial.FondoPanel;
        public override Color ImageMarginGradientEnd => TemaGamerEmpresarial.FondoPanel;
        public override Color SeparatorDark => TemaGamerEmpresarial.BordeHud;
        public override Color SeparatorLight => TemaGamerEmpresarial.BordeSutil;
    }
}
