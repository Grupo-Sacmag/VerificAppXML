using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FormGuardadoVideojuego : Form
    {
        private readonly SesionConciliacion _sesionActual;
        public SesionConciliacion SesionCargada { get; private set; }
        private readonly string _directorioSaves;
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };

        public FormGuardadoVideojuego(SesionConciliacion sesionActual = null)
        {
            InitializeComponent();
            _sesionActual = sesionActual;

            try
            {
                string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_icon.ico");
                if (File.Exists(iconPath)) this.Icon = new Icon(iconPath);
            }
            catch { }

            _directorioSaves = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves");
            if (!Directory.Exists(_directorioSaves))
            {
                try { Directory.CreateDirectory(_directorioSaves); } catch { }
            }

            ConstruirSlots();
        }

        private string ObtenerRutaSlot(int numeroSlot)
        {
            return Path.Combine(_directorioSaves, $"slot_{numeroSlot}.concilia.json");
        }

        private void ConstruirSlots()
        {
            pnlSlotsContainer.SuspendLayout();
            pnlSlotsContainer.Controls.Clear();

            int cardHeight = 118;
            int cardSpacing = 12;
            int top = 12;

            for (int i = 1; i <= 5; i++)
            {
                int slotNum = i;
                string rutaSlot = ObtenerRutaSlot(slotNum);
                bool ocupado = File.Exists(rutaSlot);

                SesionConciliacion sesionSlot = null;
                if (ocupado)
                {
                    try
                    {
                        string json = File.ReadAllText(rutaSlot);
                        sesionSlot = _serializer.Deserialize<SesionConciliacion>(json);
                    }
                    catch
                    {
                        ocupado = false;
                    }
                }

                Panel pnlCard = new Panel
                {
                    Location = new Point(15, top),
                    Size = new Size(pnlSlotsContainer.ClientSize.Width - 30, cardHeight),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    BackColor = ocupado ? Color.FromArgb(30, 41, 59) : Color.FromArgb(15, 23, 42),
                    BorderStyle = BorderStyle.FixedSingle,
                    Tag = slotNum
                };

                // Badge Slot
                Label lblBadge = new Label
                {
                    Text = $"SLOT 0{slotNum}",
                    Location = new Point(14, 16),
                    Size = new Size(74, 26),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = ocupado ? Color.FromArgb(6, 182, 212) : Color.FromArgb(71, 85, 105)
                };
                pnlCard.Controls.Add(lblBadge);

                int btnY = (cardHeight - 38) / 2;

                if (ocupado && sesionSlot != null)
                {
                    int total = sesionSlot.ItemsConciliacion?.Count ?? 0;
                    int amarradas = sesionSlot.ItemsConciliacion?.Count(x => x.EstadoFactura != null && x.EstadoFactura.StartsWith("✅")) ?? 0;
                    int faltantes = sesionSlot.ItemsConciliacion?.Count(x => x.EstadoFactura != null && (x.EstadoFactura.StartsWith("❌") || x.EstadoFactura.StartsWith("⚠️"))) ?? 0;
                    int paquetes = sesionSlot.ItemsConciliacion?.Count(x => x.EstadoFactura != null && x.EstadoFactura.Contains("PAQUETE")) ?? 0;
                    int pct = total > 0 ? (int)Math.Round((double)amarradas / total * 100) : 0;
                    string nombreArchivo = !string.IsNullOrEmpty(sesionSlot.RutaEdoCta) ? Path.GetFileName(sesionSlot.RutaEdoCta) : "Estado de Cuenta";

                    int textWidth = Math.Max(200, pnlCard.Width - 340);

                    // Titulo Sesion
                    Label lblTituloSlot = new Label
                    {
                        Text = $"📁 SESIÓN GUARDADA  •  {sesionSlot.FechaGuardado:dd/MM/yyyy hh:mm tt}",
                        Location = new Point(100, 16),
                        Size = new Size(textWidth, 22),
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                        Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(241, 245, 249)
                    };
                    pnlCard.Controls.Add(lblTituloSlot);

                    // Stats
                    Label lblStats = new Label
                    {
                        Text = $"📊 Cargos: {total}   |   ✅ Amarradas: {amarradas}   |   ❌ Faltantes: {faltantes}   |   📦 Paquetes: {paquetes}   |   📈 Avance: {pct}%",
                        Location = new Point(100, 46),
                        Size = new Size(textWidth, 24),
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                        Font = new Font("Segoe UI", 9F),
                        ForeColor = Color.FromArgb(148, 163, 184)
                    };
                    pnlCard.Controls.Add(lblStats);

                    // Archivo
                    Label lblArchivo = new Label
                    {
                        Text = $"📄 Archivo: {nombreArchivo}",
                        Location = new Point(100, 76),
                        Size = new Size(textWidth, 22),
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                        Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                        ForeColor = Color.FromArgb(203, 213, 225)
                    };
                    pnlCard.Controls.Add(lblArchivo);

                    // Boton Borrar
                    Button btnBorrar = new Button
                    {
                        Text = "BORRAR",
                        Location = new Point(pnlCard.Width - 95, btnY),
                        Size = new Size(85, 38),
                        Anchor = AnchorStyles.Top | AnchorStyles.Right,
                        BackColor = Color.FromArgb(239, 68, 68),
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                        ForeColor = Color.White,
                        Cursor = Cursors.Hand
                    };
                    btnBorrar.FlatAppearance.BorderSize = 0;
                    var ttB = new ToolTip();
                    ttB.SetToolTip(btnBorrar, "Borrar esta sesión guardada");
                    btnBorrar.Click += (s, ev) =>
                    {
                        var confirm = MessageBox.Show($"¿Estás seguro de borrar la sesión guardada en el SLOT 0{slotNum}?", "Borrar sesión", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (confirm == DialogResult.Yes)
                        {
                            try
                            {
                                if (File.Exists(rutaSlot)) File.Delete(rutaSlot);
                                ConstruirSlots();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error al borrar la sesión: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    };
                    pnlCard.Controls.Add(btnBorrar);
                    btnBorrar.BringToFront();

                    // Boton Sobreescribir / Guardar
                    if (_sesionActual != null)
                    {
                        Button btnSobreescribir = new Button
                        {
                            Text = "GUARDAR",
                            Location = new Point(pnlCard.Width - 198, btnY),
                            Size = new Size(95, 38),
                            Anchor = AnchorStyles.Top | AnchorStyles.Right,
                            BackColor = Color.FromArgb(245, 158, 11),
                            FlatStyle = FlatStyle.Flat,
                            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                            ForeColor = Color.White,
                            Cursor = Cursors.Hand
                        };
                        btnSobreescribir.FlatAppearance.BorderSize = 0;
                        var tt = new ToolTip();
                        tt.SetToolTip(btnSobreescribir, "Guardar tu sesión actual en este Slot");
                        btnSobreescribir.Click += (s, ev) =>
                        {
                            var confirm = MessageBox.Show($"¿Deseas sobreescribir la sesión del SLOT 0{slotNum} con tu avance actual?", "Confirmar Guardado", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (confirm == DialogResult.Yes)
                            {
                                GuardarEnSlot(slotNum);
                            }
                        };
                        pnlCard.Controls.Add(btnSobreescribir);
                        btnSobreescribir.BringToFront();
                    }

                    // Boton Cargar
                    int cargarX = _sesionActual != null ? (pnlCard.Width - 301) : (pnlCard.Width - 200);
                    Button btnCargar = new Button
                    {
                        Text = "CARGAR",
                        Location = new Point(cargarX, btnY),
                        Size = new Size(95, 38),
                        Anchor = AnchorStyles.Top | AnchorStyles.Right,
                        BackColor = Color.FromArgb(16, 185, 129),
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                        ForeColor = Color.White,
                        Cursor = Cursors.Hand
                    };
                    btnCargar.FlatAppearance.BorderSize = 0;
                    btnCargar.Click += (s, ev) =>
                    {
                        SesionCargada = sesionSlot;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    };
                    pnlCard.Controls.Add(btnCargar);
                    btnCargar.BringToFront();
                }
                else
                {
                    // Slot vacio
                    int textWidth = Math.Max(200, pnlCard.Width - 190);
                    Label lblTituloVacio = new Label
                    {
                        Text = "[ RANURA VACÍA / ESPACIO DISPONIBLE ]",
                        Location = new Point(100, 30),
                        Size = new Size(textWidth, 24),
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                        Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(100, 116, 139)
                    };
                    pnlCard.Controls.Add(lblTituloVacio);

                    Label lblDescVacio = new Label
                    {
                        Text = "Sin sesión registrada. Presiona 'GUARDAR' para registrar tu avance de conciliación en este slot.",
                        Location = new Point(100, 58),
                        Size = new Size(textWidth, 24),
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                        Font = new Font("Segoe UI", 9F),
                        ForeColor = Color.FromArgb(71, 85, 105)
                    };
                    pnlCard.Controls.Add(lblDescVacio);

                    Button btnGuardar = new Button
                    {
                        Text = "GUARDAR",
                        Location = new Point(pnlCard.Width - 145, btnY),
                        Size = new Size(130, 38),
                        Anchor = AnchorStyles.Top | AnchorStyles.Right,
                        BackColor = _sesionActual != null ? Color.FromArgb(14, 116, 144) : Color.FromArgb(51, 65, 85),
                        Enabled = _sesionActual != null,
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                        ForeColor = Color.White,
                        Cursor = _sesionActual != null ? Cursors.Hand : Cursors.Default
                    };
                    btnGuardar.FlatAppearance.BorderSize = 0;
                    btnGuardar.Click += (s, ev) =>
                    {
                        GuardarEnSlot(slotNum);
                    };
                    pnlCard.Controls.Add(btnGuardar);
                    btnGuardar.BringToFront();
                }

                pnlSlotsContainer.Controls.Add(pnlCard);
                top += cardHeight + cardSpacing;
                top += cardHeight + cardSpacing;
            }

            pnlSlotsContainer.ResumeLayout(true);
        } 

        private void GuardarEnSlot(int slotNum)
        {
            if (_sesionActual == null) return;
            try
            {
                _sesionActual.FechaGuardado = DateTime.Now;
                string ruta = ObtenerRutaSlot(slotNum);
                string json = _serializer.Serialize(_sesionActual);
                File.WriteAllText(ruta, json);

                MessageBox.Show($"✨ ¡SESIÓN GUARDADA EXITOSAMENTE EN EL SLOT 0{slotNum}! ✨\n\nFecha: {_sesionActual.FechaGuardado:dd/MM/yyyy hh:mm tt}\nMovimientos: {_sesionActual.ItemsConciliacion?.Count}", "Guardado Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ConstruirSlots();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar en el slot: " + ex.Message, "Error al Guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
