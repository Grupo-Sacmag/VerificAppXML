namespace WindowsFormsApp1
{
    partial class FormConciliacionCsv
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuArchivo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuGuardarSesion = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCargarSesion = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuExportarCsv = new System.Windows.Forms.ToolStripMenuItem();
            this.menuGenerarExcel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSalir = new System.Windows.Forms.ToolStripMenuItem();
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.btnSeleccionarCsv = new System.Windows.Forms.Button();
            this.btnSeleccionarCarpetaEdoCta = new System.Windows.Forms.Button();
            this.txtRutaCsv = new System.Windows.Forms.TextBox();
            this.btnSeleccionarCarpetaXml = new System.Windows.Forms.Button();
            this.txtRutaCarpetaXml = new System.Windows.Forms.TextBox();
            this.btnConciliar = new System.Windows.Forms.Button();
            this.btnExportar = new System.Windows.Forms.Button();
            this.btnGenerarExcel = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblEstado = new System.Windows.Forms.Label();
            this.panelMetricas = new System.Windows.Forms.Panel();
            this.lblTotalCsv = new System.Windows.Forms.Label();
            this.lblTotalXmls = new System.Windows.Forms.Label();
            this.lblCoincidentes = new System.Windows.Forms.Label();
            this.lblFaltantes = new System.Windows.Forms.Label();
            this.lblHuerfanos = new System.Windows.Forms.Label();
            this.tabControl1 = new WindowsFormsApp1.DarkGamerTabControl();
            this.tabTodas = new System.Windows.Forms.TabPage();
            this.gridTodas = new System.Windows.Forms.DataGridView();
            this.tabFaltantes = new System.Windows.Forms.TabPage();
            this.gridFaltantes = new System.Windows.Forms.DataGridView();
            this.tabCoincidentes = new System.Windows.Forms.TabPage();
            this.gridCoincidentes = new System.Windows.Forms.DataGridView();
            this.tabHuerfanos = new System.Windows.Forms.TabPage();
            this.gridHuerfanos = new System.Windows.Forms.DataGridView();
            this.menuStrip1.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelMetricas.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabTodas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridTodas)).BeginInit();
            this.tabFaltantes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridFaltantes)).BeginInit();
            this.tabCoincidentes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridCoincidentes)).BeginInit();
            this.tabHuerfanos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridHuerfanos)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.White;
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuArchivo});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1382, 30);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuArchivo
            // 
            this.menuArchivo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuGuardarSesion,
            this.menuCargarSesion,
            this.toolStripSeparator1,
            this.menuExportarCsv,
            this.menuGenerarExcel,
            this.toolStripSeparator2,
            this.menuSalir});
            this.menuArchivo.Name = "menuArchivo";
            this.menuArchivo.Size = new System.Drawing.Size(73, 24);
            this.menuArchivo.Text = "&Archivo";
            // 
            // menuGuardarSesion
            // 
            this.menuGuardarSesion.Name = "menuGuardarSesion";
            this.menuGuardarSesion.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuGuardarSesion.Size = new System.Drawing.Size(382, 26);
            this.menuGuardarSesion.Text = "💾 &Guardar Sesión de Conciliación...";
            this.menuGuardarSesion.Click += new System.EventHandler(this.menuGuardarSesion_Click);
            // 
            // menuCargarSesion
            // 
            this.menuCargarSesion.Name = "menuCargarSesion";
            this.menuCargarSesion.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuCargarSesion.Size = new System.Drawing.Size(382, 26);
            this.menuCargarSesion.Text = "📂 &Cargar Sesión Guardada...";
            this.menuCargarSesion.Click += new System.EventHandler(this.menuCargarSesion_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(379, 6);
            // 
            // menuExportarCsv
            // 
            this.menuExportarCsv.Name = "menuExportarCsv";
            this.menuExportarCsv.Size = new System.Drawing.Size(382, 26);
            this.menuExportarCsv.Text = "📊 &Exportar Reporte a CSV (.csv)...";
            this.menuExportarCsv.Click += new System.EventHandler(this.btnExportar_Click);
            // 
            // menuGenerarExcel
            // 
            this.menuGenerarExcel.Name = "menuGenerarExcel";
            this.menuGenerarExcel.Size = new System.Drawing.Size(382, 26);
            this.menuGenerarExcel.Text = "📗 &Convertir PDF a Excel (.xlsx)...";
            this.menuGenerarExcel.Click += new System.EventHandler(this.btnGenerarExcel_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(379, 6);
            // 
            // menuSalir
            // 
            this.menuSalir.Name = "menuSalir";
            this.menuSalir.Size = new System.Drawing.Size(382, 26);
            this.menuSalir.Text = "🚪 &Salir";
            this.menuSalir.Click += new System.EventHandler(this.menuSalir_Click);
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.panelTop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTop.Controls.Add(this.lblTitulo);
            this.panelTop.Controls.Add(this.btnSeleccionarCsv);
            this.panelTop.Controls.Add(this.btnSeleccionarCarpetaEdoCta);
            this.panelTop.Controls.Add(this.txtRutaCsv);
            this.panelTop.Controls.Add(this.btnSeleccionarCarpetaXml);
            this.panelTop.Controls.Add(this.txtRutaCarpetaXml);
            this.panelTop.Controls.Add(this.btnConciliar);
            this.panelTop.Controls.Add(this.btnExportar);
            this.panelTop.Controls.Add(this.btnGenerarExcel);
            this.panelTop.Controls.Add(this.progressBar1);
            this.panelTop.Controls.Add(this.lblEstado);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 30);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1382, 175);
            this.panelTop.TabIndex = 0;
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(12, 9);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(407, 25);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Conciliación de Estado de Cuenta vs Facturas";
            // 
            // btnSeleccionarCsv
            // 
            this.btnSeleccionarCsv.BackColor = System.Drawing.Color.White;
            this.btnSeleccionarCsv.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSeleccionarCsv.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSeleccionarCsv.Location = new System.Drawing.Point(16, 42);
            this.btnSeleccionarCsv.Name = "btnSeleccionarCsv";
            this.btnSeleccionarCsv.Size = new System.Drawing.Size(212, 32);
            this.btnSeleccionarCsv.TabIndex = 1;
            this.btnSeleccionarCsv.Text = "📄 Subir PDF";
            this.btnSeleccionarCsv.UseVisualStyleBackColor = false;
            this.btnSeleccionarCsv.Click += new System.EventHandler(this.btnSeleccionarCsv_Click);
            // 
            // btnSeleccionarCarpetaEdoCta
            // 
            this.btnSeleccionarCarpetaEdoCta.BackColor = System.Drawing.Color.White;
            this.btnSeleccionarCarpetaEdoCta.Enabled = false;
            this.btnSeleccionarCarpetaEdoCta.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSeleccionarCarpetaEdoCta.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSeleccionarCarpetaEdoCta.Location = new System.Drawing.Point(118, 42);
            this.btnSeleccionarCarpetaEdoCta.Name = "btnSeleccionarCarpetaEdoCta";
            this.btnSeleccionarCarpetaEdoCta.Size = new System.Drawing.Size(110, 32);
            this.btnSeleccionarCarpetaEdoCta.TabIndex = 2;
            this.btnSeleccionarCarpetaEdoCta.Text = "📁 Carpeta Edo.";
            this.btnSeleccionarCarpetaEdoCta.UseVisualStyleBackColor = false;
            this.btnSeleccionarCarpetaEdoCta.Visible = false;
            this.btnSeleccionarCarpetaEdoCta.Click += new System.EventHandler(this.btnSeleccionarCarpetaEdoCta_Click);
            // 
            // txtRutaCsv
            // 
            this.txtRutaCsv.BackColor = System.Drawing.Color.White;
            this.txtRutaCsv.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtRutaCsv.Location = new System.Drawing.Point(234, 44);
            this.txtRutaCsv.Name = "txtRutaCsv";
            this.txtRutaCsv.ReadOnly = true;
            this.txtRutaCsv.Size = new System.Drawing.Size(430, 27);
            this.txtRutaCsv.TabIndex = 2;
            // 
            // btnSeleccionarCarpetaXml
            // 
            this.btnSeleccionarCarpetaXml.BackColor = System.Drawing.Color.White;
            this.btnSeleccionarCarpetaXml.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSeleccionarCarpetaXml.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSeleccionarCarpetaXml.Location = new System.Drawing.Point(16, 80);
            this.btnSeleccionarCarpetaXml.Name = "btnSeleccionarCarpetaXml";
            this.btnSeleccionarCarpetaXml.Size = new System.Drawing.Size(212, 32);
            this.btnSeleccionarCarpetaXml.TabIndex = 3;
            this.btnSeleccionarCarpetaXml.Text = "📂 Carpeta de Facturas";
            this.btnSeleccionarCarpetaXml.UseVisualStyleBackColor = false;
            this.btnSeleccionarCarpetaXml.Click += new System.EventHandler(this.btnSeleccionarCarpetaXml_Click);
            // 
            // txtRutaCarpetaXml
            // 
            this.txtRutaCarpetaXml.BackColor = System.Drawing.Color.White;
            this.txtRutaCarpetaXml.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtRutaCarpetaXml.Location = new System.Drawing.Point(234, 82);
            this.txtRutaCarpetaXml.Name = "txtRutaCarpetaXml";
            this.txtRutaCarpetaXml.ReadOnly = true;
            this.txtRutaCarpetaXml.Size = new System.Drawing.Size(430, 27);
            this.txtRutaCarpetaXml.TabIndex = 4;
            // 
            // btnConciliar
            // 
            this.btnConciliar.BackColor = System.Drawing.Color.Salmon;
            this.btnConciliar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConciliar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnConciliar.ForeColor = System.Drawing.Color.Black;
            this.btnConciliar.Location = new System.Drawing.Point(702, 44);
            this.btnConciliar.Name = "btnConciliar";
            this.btnConciliar.Size = new System.Drawing.Size(212, 39);
            this.btnConciliar.TabIndex = 5;
            this.btnConciliar.Text = "Buscar Coincidencias";
            this.btnConciliar.UseVisualStyleBackColor = false;
            this.btnConciliar.Click += new System.EventHandler(this.btnConciliar_Click);
            // 
            // btnExportar
            // 
            this.btnExportar.BackColor = System.Drawing.Color.LavenderBlush;
            this.btnExportar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportar.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnExportar.ForeColor = System.Drawing.Color.Black;
            this.btnExportar.Location = new System.Drawing.Point(920, 44);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(214, 39);
            this.btnExportar.TabIndex = 6;
            this.btnExportar.Text = "Exportar Reporte (.csv)";
            this.btnExportar.UseVisualStyleBackColor = false;
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            // 
            // btnGenerarExcel
            // 
            this.btnGenerarExcel.BackColor = System.Drawing.Color.White;
            this.btnGenerarExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerarExcel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnGenerarExcel.ForeColor = System.Drawing.Color.Black;
            this.btnGenerarExcel.Location = new System.Drawing.Point(1140, 44);
            this.btnGenerarExcel.Name = "btnGenerarExcel";
            this.btnGenerarExcel.Size = new System.Drawing.Size(190, 39);
            this.btnGenerarExcel.TabIndex = 9;
            this.btnGenerarExcel.Text = "Estado cta a Excel";
            this.btnGenerarExcel.UseVisualStyleBackColor = false;
            this.btnGenerarExcel.Click += new System.EventHandler(this.btnGenerarExcel_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(17, 117);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(656, 14);
            this.progressBar1.TabIndex = 7;
            this.progressBar1.Visible = false;
            // 
            // lblEstado
            // 
            this.lblEstado.AutoSize = true;
            this.lblEstado.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEstado.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblEstado.Location = new System.Drawing.Point(679, 108);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(612, 25);
            this.lblEstado.TabIndex = 8;
            this.lblEstado.Text = "Seleccione el estado de cuenta PDF y la carpeta de Facturas correspondiente";
            // 
            // panelMetricas
            // 
            this.panelMetricas.BackColor = System.Drawing.Color.White;
            this.panelMetricas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMetricas.Controls.Add(this.lblTotalCsv);
            this.panelMetricas.Controls.Add(this.lblTotalXmls);
            this.panelMetricas.Controls.Add(this.lblCoincidentes);
            this.panelMetricas.Controls.Add(this.lblFaltantes);
            this.panelMetricas.Controls.Add(this.lblHuerfanos);
            this.panelMetricas.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMetricas.Location = new System.Drawing.Point(0, 205);
            this.panelMetricas.Name = "panelMetricas";
            this.panelMetricas.Size = new System.Drawing.Size(1382, 56);
            this.panelMetricas.TabIndex = 1;
            // 
            // lblTotalCsv
            // 
            this.lblTotalCsv.AutoSize = true;
            this.lblTotalCsv.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblTotalCsv.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblTotalCsv.Location = new System.Drawing.Point(15, 11);
            this.lblTotalCsv.Name = "lblTotalCsv";
            this.lblTotalCsv.Size = new System.Drawing.Size(156, 21);
            this.lblTotalCsv.TabIndex = 0;
            this.lblTotalCsv.Text = "Operaciones CSV: 0";
            // 
            // lblTotalXmls
            // 
            this.lblTotalXmls.AutoSize = true;
            this.lblTotalXmls.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblTotalXmls.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblTotalXmls.Location = new System.Drawing.Point(206, 11);
            this.lblTotalXmls.Name = "lblTotalXmls";
            this.lblTotalXmls.Size = new System.Drawing.Size(188, 21);
            this.lblTotalXmls.TabIndex = 1;
            this.lblTotalXmls.Text = "Facturas encontradas: 0";
            // 
            // lblCoincidentes
            // 
            this.lblCoincidentes.AutoSize = true;
            this.lblCoincidentes.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblCoincidentes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(101)))), ((int)(((byte)(52)))));
            this.lblCoincidentes.Location = new System.Drawing.Point(460, 11);
            this.lblCoincidentes.Name = "lblCoincidentes";
            this.lblCoincidentes.Size = new System.Drawing.Size(160, 21);
            this.lblCoincidentes.TabIndex = 2;
            this.lblCoincidentes.Text = "✅ Coincidencias: 0";
            // 
            // lblFaltantes
            // 
            this.lblFaltantes.AutoSize = true;
            this.lblFaltantes.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblFaltantes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.lblFaltantes.Location = new System.Drawing.Point(734, 11);
            this.lblFaltantes.Name = "lblFaltantes";
            this.lblFaltantes.Size = new System.Drawing.Size(123, 21);
            this.lblFaltantes.TabIndex = 3;
            this.lblFaltantes.Text = "❌ Faltantes: 0";
            // 
            // lblHuerfanos
            // 
            this.lblHuerfanos.AutoSize = true;
            this.lblHuerfanos.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblHuerfanos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(40)))), ((int)(((byte)(217)))));
            this.lblHuerfanos.Location = new System.Drawing.Point(974, 11);
            this.lblHuerfanos.Name = "lblHuerfanos";
            this.lblHuerfanos.Size = new System.Drawing.Size(244, 21);
            this.lblHuerfanos.TabIndex = 4;
            this.lblHuerfanos.Text = "⚠️ Facturas fuera de Edo cta: 0";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabTodas);
            this.tabControl1.Controls.Add(this.tabFaltantes);
            this.tabControl1.Controls.Add(this.tabCoincidentes);
            this.tabControl1.Controls.Add(this.tabHuerfanos);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.tabControl1.ItemSize = new System.Drawing.Size(245, 34);
            this.tabControl1.Location = new System.Drawing.Point(0, 261);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1382, 469);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 2;
            // 
            // tabTodas
            // 
            this.tabTodas.Controls.Add(this.gridTodas);
            this.tabTodas.Location = new System.Drawing.Point(4, 38);
            this.tabTodas.Name = "tabTodas";
            this.tabTodas.Padding = new System.Windows.Forms.Padding(3);
            this.tabTodas.Size = new System.Drawing.Size(1374, 427);
            this.tabTodas.TabIndex = 0;
            this.tabTodas.Text = "📋 Todas las Operaciones del CSV";
            this.tabTodas.UseVisualStyleBackColor = true;
            // 
            // gridTodas
            // 
            this.gridTodas.AllowUserToAddRows = false;
            this.gridTodas.AllowUserToDeleteRows = false;
            this.gridTodas.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.gridTodas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridTodas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridTodas.Location = new System.Drawing.Point(3, 3);
            this.gridTodas.Name = "gridTodas";
            this.gridTodas.ReadOnly = true;
            this.gridTodas.RowHeadersWidth = 35;
            this.gridTodas.RowTemplate.Height = 24;
            this.gridTodas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridTodas.Size = new System.Drawing.Size(1368, 421);
            this.gridTodas.TabIndex = 0;
            this.gridTodas.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.GridMovimientos_CellDoubleClick);
            this.gridTodas.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GridMovimientos_KeyDown);
            // 
            // tabFaltantes
            // 
            this.tabFaltantes.Controls.Add(this.gridFaltantes);
            this.tabFaltantes.Location = new System.Drawing.Point(4, 38);
            this.tabFaltantes.Name = "tabFaltantes";
            this.tabFaltantes.Padding = new System.Windows.Forms.Padding(3);
            this.tabFaltantes.Size = new System.Drawing.Size(1374, 429);
            this.tabFaltantes.TabIndex = 1;
            this.tabFaltantes.Text = "❌ Faltantes de Factura (Por Reclamar)";
            this.tabFaltantes.UseVisualStyleBackColor = true;
            // 
            // gridFaltantes
            // 
            this.gridFaltantes.AllowUserToAddRows = false;
            this.gridFaltantes.AllowUserToDeleteRows = false;
            this.gridFaltantes.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.gridFaltantes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridFaltantes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridFaltantes.Location = new System.Drawing.Point(3, 3);
            this.gridFaltantes.Name = "gridFaltantes";
            this.gridFaltantes.ReadOnly = true;
            this.gridFaltantes.RowHeadersWidth = 35;
            this.gridFaltantes.RowTemplate.Height = 24;
            this.gridFaltantes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridFaltantes.Size = new System.Drawing.Size(1368, 423);
            this.gridFaltantes.TabIndex = 0;
            this.gridFaltantes.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.GridMovimientos_CellDoubleClick);
            this.gridFaltantes.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GridMovimientos_KeyDown);
            // 
            // tabCoincidentes
            // 
            this.tabCoincidentes.Controls.Add(this.gridCoincidentes);
            this.tabCoincidentes.Location = new System.Drawing.Point(4, 38);
            this.tabCoincidentes.Name = "tabCoincidentes";
            this.tabCoincidentes.Padding = new System.Windows.Forms.Padding(3);
            this.tabCoincidentes.Size = new System.Drawing.Size(1374, 429);
            this.tabCoincidentes.TabIndex = 2;
            this.tabCoincidentes.Text = "✅ Facturas Encontradas (Coincidencias)";
            this.tabCoincidentes.UseVisualStyleBackColor = true;
            // 
            // gridCoincidentes
            // 
            this.gridCoincidentes.AllowUserToAddRows = false;
            this.gridCoincidentes.AllowUserToDeleteRows = false;
            this.gridCoincidentes.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.gridCoincidentes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridCoincidentes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridCoincidentes.Location = new System.Drawing.Point(3, 3);
            this.gridCoincidentes.Name = "gridCoincidentes";
            this.gridCoincidentes.ReadOnly = true;
            this.gridCoincidentes.RowHeadersWidth = 35;
            this.gridCoincidentes.RowTemplate.Height = 24;
            this.gridCoincidentes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridCoincidentes.Size = new System.Drawing.Size(1368, 423);
            this.gridCoincidentes.TabIndex = 0;
            this.gridCoincidentes.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.GridMovimientos_CellDoubleClick);
            this.gridCoincidentes.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GridMovimientos_KeyDown);
            // 
            // tabHuerfanos
            // 
            this.tabHuerfanos.Controls.Add(this.gridHuerfanos);
            this.tabHuerfanos.Location = new System.Drawing.Point(4, 38);
            this.tabHuerfanos.Name = "tabHuerfanos";
            this.tabHuerfanos.Padding = new System.Windows.Forms.Padding(3);
            this.tabHuerfanos.Size = new System.Drawing.Size(1374, 429);
            this.tabHuerfanos.TabIndex = 3;
            this.tabHuerfanos.Text = "⚠️ XMLs en Carpeta sin Cargo en CSV";
            this.tabHuerfanos.UseVisualStyleBackColor = true;
            // 
            // gridHuerfanos
            // 
            this.gridHuerfanos.AllowUserToAddRows = false;
            this.gridHuerfanos.AllowUserToDeleteRows = false;
            this.gridHuerfanos.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.gridHuerfanos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridHuerfanos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridHuerfanos.Location = new System.Drawing.Point(3, 3);
            this.gridHuerfanos.Name = "gridHuerfanos";
            this.gridHuerfanos.ReadOnly = true;
            this.gridHuerfanos.RowHeadersWidth = 35;
            this.gridHuerfanos.RowTemplate.Height = 24;
            this.gridHuerfanos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridHuerfanos.Size = new System.Drawing.Size(1368, 423);
            this.gridHuerfanos.TabIndex = 0;
            // 
            // FormConciliacionCsv
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1382, 730);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panelMetricas);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.Name = "FormConciliacionCsv";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Conciliación de Estado de Cuenta (CSV) vs Facturas XML - Grupo Sacmag";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelMetricas.ResumeLayout(false);
            this.panelMetricas.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabTodas.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridTodas)).EndInit();
            this.tabFaltantes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridFaltantes)).EndInit();
            this.tabCoincidentes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridCoincidentes)).EndInit();
            this.tabHuerfanos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridHuerfanos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Button btnSeleccionarCsv;
        private System.Windows.Forms.Button btnSeleccionarCarpetaEdoCta;
        private System.Windows.Forms.TextBox txtRutaCsv;
        private System.Windows.Forms.Button btnSeleccionarCarpetaXml;
        private System.Windows.Forms.TextBox txtRutaCarpetaXml;
        private System.Windows.Forms.Button btnConciliar;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Button btnGenerarExcel;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.Panel panelMetricas;
        private System.Windows.Forms.Label lblTotalCsv;
        private System.Windows.Forms.Label lblTotalXmls;
        private System.Windows.Forms.Label lblCoincidentes;
        private System.Windows.Forms.Label lblFaltantes;
        private System.Windows.Forms.Label lblHuerfanos;
        private DarkGamerTabControl tabControl1;
        private System.Windows.Forms.TabPage tabTodas;
        private System.Windows.Forms.DataGridView gridTodas;
        private System.Windows.Forms.TabPage tabFaltantes;
        private System.Windows.Forms.DataGridView gridFaltantes;
        private System.Windows.Forms.TabPage tabCoincidentes;
        private System.Windows.Forms.DataGridView gridCoincidentes;
        private System.Windows.Forms.TabPage tabHuerfanos;
        private System.Windows.Forms.DataGridView gridHuerfanos;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuArchivo;
        private System.Windows.Forms.ToolStripMenuItem menuGuardarSesion;
        private System.Windows.Forms.ToolStripMenuItem menuCargarSesion;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuExportarCsv;
        private System.Windows.Forms.ToolStripMenuItem menuGenerarExcel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem menuSalir;
    }
}
