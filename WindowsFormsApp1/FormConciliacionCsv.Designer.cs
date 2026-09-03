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
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.btnSeleccionarCsv = new System.Windows.Forms.Button();
            this.txtRutaCsv = new System.Windows.Forms.TextBox();
            this.btnSeleccionarCarpetaXml = new System.Windows.Forms.Button();
            this.txtRutaCarpetaXml = new System.Windows.Forms.TextBox();
            this.btnConciliar = new System.Windows.Forms.Button();
            this.btnExportar = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblEstado = new System.Windows.Forms.Label();
            this.panelMetricas = new System.Windows.Forms.Panel();
            this.lblTotalCsv = new System.Windows.Forms.Label();
            this.lblTotalXmls = new System.Windows.Forms.Label();
            this.lblCoincidentes = new System.Windows.Forms.Label();
            this.lblFaltantes = new System.Windows.Forms.Label();
            this.lblHuerfanos = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabTodas = new System.Windows.Forms.TabPage();
            this.gridTodas = new System.Windows.Forms.DataGridView();
            this.tabFaltantes = new System.Windows.Forms.TabPage();
            this.gridFaltantes = new System.Windows.Forms.DataGridView();
            this.tabCoincidentes = new System.Windows.Forms.TabPage();
            this.gridCoincidentes = new System.Windows.Forms.DataGridView();
            this.tabHuerfanos = new System.Windows.Forms.TabPage();
            this.gridHuerfanos = new System.Windows.Forms.DataGridView();
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
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.panelTop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTop.Controls.Add(this.lblTitulo);
            this.panelTop.Controls.Add(this.btnSeleccionarCsv);
            this.panelTop.Controls.Add(this.txtRutaCsv);
            this.panelTop.Controls.Add(this.btnSeleccionarCarpetaXml);
            this.panelTop.Controls.Add(this.txtRutaCarpetaXml);
            this.panelTop.Controls.Add(this.btnConciliar);
            this.panelTop.Controls.Add(this.btnExportar);
            this.panelTop.Controls.Add(this.progressBar1);
            this.panelTop.Controls.Add(this.lblEstado);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1382, 140);
            this.panelTop.TabIndex = 0;
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblTitulo.Location = new System.Drawing.Point(12, 9);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(514, 25);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Conciliación de Estado de Cuenta (CSV) vs Facturas XML";
            // 
            // btnSeleccionarCsv
            // 
            this.btnSeleccionarCsv.BackColor = System.Drawing.Color.White;
            this.btnSeleccionarCsv.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSeleccionarCsv.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSeleccionarCsv.Location = new System.Drawing.Point(17, 43);
            this.btnSeleccionarCsv.Name = "btnSeleccionarCsv";
            this.btnSeleccionarCsv.Size = new System.Drawing.Size(220, 30);
            this.btnSeleccionarCsv.TabIndex = 1;
            this.btnSeleccionarCsv.Text = "📁 Seleccionar CSV (Estado Cuenta)";
            this.btnSeleccionarCsv.UseVisualStyleBackColor = false;
            this.btnSeleccionarCsv.Click += new System.EventHandler(this.btnSeleccionarCsv_Click);
            // 
            // txtRutaCsv
            // 
            this.txtRutaCsv.BackColor = System.Drawing.Color.White;
            this.txtRutaCsv.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtRutaCsv.Location = new System.Drawing.Point(243, 45);
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
            this.btnSeleccionarCarpetaXml.Location = new System.Drawing.Point(17, 80);
            this.btnSeleccionarCarpetaXml.Name = "btnSeleccionarCarpetaXml";
            this.btnSeleccionarCarpetaXml.Size = new System.Drawing.Size(220, 30);
            this.btnSeleccionarCarpetaXml.TabIndex = 3;
            this.btnSeleccionarCarpetaXml.Text = "📂 Seleccionar Carpeta de XMLs";
            this.btnSeleccionarCarpetaXml.UseVisualStyleBackColor = false;
            this.btnSeleccionarCarpetaXml.Click += new System.EventHandler(this.btnSeleccionarCarpetaXml_Click);
            // 
            // txtRutaCarpetaXml
            // 
            this.txtRutaCarpetaXml.BackColor = System.Drawing.Color.White;
            this.txtRutaCarpetaXml.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtRutaCarpetaXml.Location = new System.Drawing.Point(243, 82);
            this.txtRutaCarpetaXml.Name = "txtRutaCarpetaXml";
            this.txtRutaCarpetaXml.ReadOnly = true;
            this.txtRutaCarpetaXml.Size = new System.Drawing.Size(430, 27);
            this.txtRutaCarpetaXml.TabIndex = 4;
            // 
            // btnConciliar
            // 
            this.btnConciliar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(116)))), ((int)(((byte)(144)))));
            this.btnConciliar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConciliar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnConciliar.ForeColor = System.Drawing.Color.White;
            this.btnConciliar.Location = new System.Drawing.Point(690, 43);
            this.btnConciliar.Name = "btnConciliar";
            this.btnConciliar.Size = new System.Drawing.Size(220, 68);
            this.btnConciliar.TabIndex = 5;
            this.btnConciliar.Text = "🔍 Conciliar / Buscar Coincidencias";
            this.btnConciliar.UseVisualStyleBackColor = false;
            this.btnConciliar.Click += new System.EventHandler(this.btnConciliar_Click);
            // 
            // btnExportar
            // 
            this.btnExportar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(101)))), ((int)(((byte)(52)))));
            this.btnExportar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportar.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnExportar.ForeColor = System.Drawing.Color.White;
            this.btnExportar.Location = new System.Drawing.Point(925, 43);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(200, 68);
            this.btnExportar.TabIndex = 6;
            this.btnExportar.Text = "📊 Exportar Reporte (.csv)";
            this.btnExportar.UseVisualStyleBackColor = false;
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
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
            this.lblEstado.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblEstado.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblEstado.Location = new System.Drawing.Point(690, 114);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(370, 20);
            this.lblEstado.TabIndex = 8;
            this.lblEstado.Text = "Seleccione el archivo CSV y la carpeta mensual de XMLs.";
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
            this.panelMetricas.Location = new System.Drawing.Point(0, 140);
            this.panelMetricas.Name = "panelMetricas";
            this.panelMetricas.Size = new System.Drawing.Size(1382, 45);
            this.panelMetricas.TabIndex = 1;
            // 
            // lblTotalCsv
            // 
            this.lblTotalCsv.AutoSize = true;
            this.lblTotalCsv.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblTotalCsv.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblTotalCsv.Location = new System.Drawing.Point(13, 11);
            this.lblTotalCsv.Name = "lblTotalCsv";
            this.lblTotalCsv.Size = new System.Drawing.Size(163, 21);
            this.lblTotalCsv.TabIndex = 0;
            this.lblTotalCsv.Text = "Operaciones CSV: 0";
            // 
            // lblTotalXmls
            // 
            this.lblTotalXmls.AutoSize = true;
            this.lblTotalXmls.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblTotalXmls.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblTotalXmls.Location = new System.Drawing.Point(210, 11);
            this.lblTotalXmls.Name = "lblTotalXmls";
            this.lblTotalXmls.Size = new System.Drawing.Size(176, 21);
            this.lblTotalXmls.TabIndex = 1;
            this.lblTotalXmls.Text = "XMLs en Carpeta: 0";
            // 
            // lblCoincidentes
            // 
            this.lblCoincidentes.AutoSize = true;
            this.lblCoincidentes.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblCoincidentes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(101)))), ((int)(((byte)(52)))));
            this.lblCoincidentes.Location = new System.Drawing.Point(420, 11);
            this.lblCoincidentes.Name = "lblCoincidentes";
            this.lblCoincidentes.Size = new System.Drawing.Size(185, 21);
            this.lblCoincidentes.TabIndex = 2;
            this.lblCoincidentes.Text = "✅ Con Factura XML: 0";
            // 
            // lblFaltantes
            // 
            this.lblFaltantes.AutoSize = true;
            this.lblFaltantes.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblFaltantes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.lblFaltantes.Location = new System.Drawing.Point(645, 11);
            this.lblFaltantes.Name = "lblFaltantes";
            this.lblFaltantes.Size = new System.Drawing.Size(193, 21);
            this.lblFaltantes.TabIndex = 3;
            this.lblFaltantes.Text = "❌ Faltantes de XML: 0";
            // 
            // lblHuerfanos
            // 
            this.lblHuerfanos.AutoSize = true;
            this.lblHuerfanos.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblHuerfanos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(65)))), ((int)(((byte)(12)))));
            this.lblHuerfanos.Location = new System.Drawing.Point(880, 11);
            this.lblHuerfanos.Name = "lblHuerfanos";
            this.lblHuerfanos.Size = new System.Drawing.Size(209, 21);
            this.lblHuerfanos.TabIndex = 4;
            this.lblHuerfanos.Text = "⚠️ XMLs sin Cargo CSV: 0";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabTodas);
            this.tabControl1.Controls.Add(this.tabFaltantes);
            this.tabControl1.Controls.Add(this.tabCoincidentes);
            this.tabControl1.Controls.Add(this.tabHuerfanos);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.tabControl1.Location = new System.Drawing.Point(0, 185);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1382, 545);
            this.tabControl1.TabIndex = 2;
            // 
            // tabTodas
            // 
            this.tabTodas.Controls.Add(this.gridTodas);
            this.tabTodas.Location = new System.Drawing.Point(4, 30);
            this.tabTodas.Name = "tabTodas";
            this.tabTodas.Padding = new System.Windows.Forms.Padding(3);
            this.tabTodas.Size = new System.Drawing.Size(1374, 511);
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
            this.gridTodas.Size = new System.Drawing.Size(1368, 505);
            this.gridTodas.TabIndex = 0;
            // 
            // tabFaltantes
            // 
            this.tabFaltantes.Controls.Add(this.gridFaltantes);
            this.tabFaltantes.Location = new System.Drawing.Point(4, 30);
            this.tabFaltantes.Name = "tabFaltantes";
            this.tabFaltantes.Padding = new System.Windows.Forms.Padding(3);
            this.tabFaltantes.Size = new System.Drawing.Size(1374, 511);
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
            this.gridFaltantes.Size = new System.Drawing.Size(1368, 505);
            this.gridFaltantes.TabIndex = 0;
            // 
            // tabCoincidentes
            // 
            this.tabCoincidentes.Controls.Add(this.gridCoincidentes);
            this.tabCoincidentes.Location = new System.Drawing.Point(4, 30);
            this.tabCoincidentes.Name = "tabCoincidentes";
            this.tabCoincidentes.Padding = new System.Windows.Forms.Padding(3);
            this.tabCoincidentes.Size = new System.Drawing.Size(1374, 511);
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
            this.gridCoincidentes.Size = new System.Drawing.Size(1368, 505);
            this.gridCoincidentes.TabIndex = 0;
            // 
            // tabHuerfanos
            // 
            this.tabHuerfanos.Controls.Add(this.gridHuerfanos);
            this.tabHuerfanos.Location = new System.Drawing.Point(4, 30);
            this.tabHuerfanos.Name = "tabHuerfanos";
            this.tabHuerfanos.Padding = new System.Windows.Forms.Padding(3);
            this.tabHuerfanos.Size = new System.Drawing.Size(1374, 511);
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
            this.gridHuerfanos.Size = new System.Drawing.Size(1368, 505);
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
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.Name = "FormConciliacionCsv";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Conciliación de Estado de Cuenta (CSV) vs Facturas XML - Grupo Sacmag";
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

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Button btnSeleccionarCsv;
        private System.Windows.Forms.TextBox txtRutaCsv;
        private System.Windows.Forms.Button btnSeleccionarCarpetaXml;
        private System.Windows.Forms.TextBox txtRutaCarpetaXml;
        private System.Windows.Forms.Button btnConciliar;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.Panel panelMetricas;
        private System.Windows.Forms.Label lblTotalCsv;
        private System.Windows.Forms.Label lblTotalXmls;
        private System.Windows.Forms.Label lblCoincidentes;
        private System.Windows.Forms.Label lblFaltantes;
        private System.Windows.Forms.Label lblHuerfanos;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabTodas;
        private System.Windows.Forms.DataGridView gridTodas;
        private System.Windows.Forms.TabPage tabFaltantes;
        private System.Windows.Forms.DataGridView gridFaltantes;
        private System.Windows.Forms.TabPage tabCoincidentes;
        private System.Windows.Forms.DataGridView gridCoincidentes;
        private System.Windows.Forms.TabPage tabHuerfanos;
        private System.Windows.Forms.DataGridView gridHuerfanos;
    }
}
