namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.verToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.detallesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invalidosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsAuditoria = new System.Windows.Forms.ToolStripMenuItem();
            this.panelTop = new System.Windows.Forms.Panel();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.btnSeleccionarCarpeta = new System.Windows.Forms.Button();
            this.txtRutaCarpeta = new System.Windows.Forms.TextBox();
            this.btnIniciar = new System.Windows.Forms.Button();
            this.lblExcel = new System.Windows.Forms.Label();
            this.btnSeleccionarExcel = new System.Windows.Forms.Button();
            this.txtRutaExcel = new System.Windows.Forms.TextBox();
            this.btnComparar = new System.Windows.Forms.Button();
            this.lblExcel2 = new System.Windows.Forms.Label();
            this.btnSeleccionarExcel2 = new System.Windows.Forms.Button();
            this.txtRutaExcel2 = new System.Windows.Forms.TextBox();
            this.btnArreglarFallos = new System.Windows.Forms.Button();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.lblEstado = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gridValidas = new System.Windows.Forms.DataGridView();
            this.panelFiltrosValidas = new System.Windows.Forms.Panel();
            this.lblValidas = new System.Windows.Forms.Label();
            this.gridInvalidas = new System.Windows.Forms.DataGridView();
            this.panelFiltrosInvalidas = new System.Windows.Forms.Panel();
            this.lblInvalidas = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridValidas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridInvalidas)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.verToolStripMenuItem,
            this.tsAuditoria});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1284, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // verToolStripMenuItem
            // 
            this.verToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.detallesToolStripMenuItem,
            this.invalidosToolStripMenuItem});
            this.verToolStripMenuItem.Name = "verToolStripMenuItem";
            this.verToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.verToolStripMenuItem.Text = "Ver";
            // 
            // detallesToolStripMenuItem
            // 
            this.detallesToolStripMenuItem.Name = "detallesToolStripMenuItem";
            this.detallesToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.detallesToolStripMenuItem.Text = "Detalles";
            this.detallesToolStripMenuItem.Click += new System.EventHandler(this.detallesToolStripMenuItem_Click);
            // 
            // invalidosToolStripMenuItem
            // 
            this.invalidosToolStripMenuItem.Name = "invalidosToolStripMenuItem";
            this.invalidosToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.invalidosToolStripMenuItem.Text = "Inválidos";
            this.invalidosToolStripMenuItem.Click += new System.EventHandler(this.invalidosToolStripMenuItem_Click);
            // 
            // tsAuditoria
            // 
            this.tsAuditoria.Name = "tsAuditoria";
            this.tsAuditoria.Size = new System.Drawing.Size(68, 20);
            this.tsAuditoria.Text = "Auditoria";
            this.tsAuditoria.Click += new System.EventHandler(this.tsAuditoria_Click);
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.comboBox1);
            this.panelTop.Controls.Add(this.btnSeleccionarCarpeta);
            this.panelTop.Controls.Add(this.txtRutaCarpeta);
            this.panelTop.Controls.Add(this.btnIniciar);
            this.panelTop.Controls.Add(this.lblExcel);
            this.panelTop.Controls.Add(this.btnSeleccionarExcel);
            this.panelTop.Controls.Add(this.txtRutaExcel);
            this.panelTop.Controls.Add(this.btnComparar);
            this.panelTop.Controls.Add(this.lblExcel2);
            this.panelTop.Controls.Add(this.btnSeleccionarExcel2);
            this.panelTop.Controls.Add(this.txtRutaExcel2);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 24);
            this.panelTop.Margin = new System.Windows.Forms.Padding(2);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1284, 116);
            this.panelTop.TabIndex = 1;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Por Carpeta",
            "Por Pila"});
            this.comboBox1.Location = new System.Drawing.Point(9, 12);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(136, 21);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // btnSeleccionarCarpeta
            // 
            this.btnSeleccionarCarpeta.Location = new System.Drawing.Point(154, 10);
            this.btnSeleccionarCarpeta.Margin = new System.Windows.Forms.Padding(2);
            this.btnSeleccionarCarpeta.Name = "btnSeleccionarCarpeta";
            this.btnSeleccionarCarpeta.Size = new System.Drawing.Size(120, 24);
            this.btnSeleccionarCarpeta.TabIndex = 1;
            this.btnSeleccionarCarpeta.Text = "Seleccionar carpeta";
            this.btnSeleccionarCarpeta.UseVisualStyleBackColor = true;
            this.btnSeleccionarCarpeta.Click += new System.EventHandler(this.btnSeleccionarCarpeta_Click);
            // 
            // txtRutaCarpeta
            // 
            this.txtRutaCarpeta.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRutaCarpeta.Location = new System.Drawing.Point(281, 13);
            this.txtRutaCarpeta.Margin = new System.Windows.Forms.Padding(2);
            this.txtRutaCarpeta.Name = "txtRutaCarpeta";
            this.txtRutaCarpeta.ReadOnly = true;
            this.txtRutaCarpeta.Size = new System.Drawing.Size(766, 20);
            this.txtRutaCarpeta.TabIndex = 2;
            // 
            // btnIniciar
            // 
            this.btnIniciar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnIniciar.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnIniciar.Location = new System.Drawing.Point(1110, 15);
            this.btnIniciar.Margin = new System.Windows.Forms.Padding(2);
            this.btnIniciar.Name = "btnIniciar";
            this.btnIniciar.Size = new System.Drawing.Size(116, 24);
            this.btnIniciar.TabIndex = 3;
            this.btnIniciar.Text = "Iniciar";
            this.btnIniciar.UseVisualStyleBackColor = true;
            this.btnIniciar.Click += new System.EventHandler(this.btnIniciar_Click);
            // 
            // lblExcel
            // 
            this.lblExcel.Location = new System.Drawing.Point(9, 49);
            this.lblExcel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblExcel.Name = "lblExcel";
            this.lblExcel.Size = new System.Drawing.Size(135, 16);
            this.lblExcel.TabIndex = 5;
            this.lblExcel.Text = "Comparación con Excel:";
            this.lblExcel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnSeleccionarExcel
            // 
            this.btnSeleccionarExcel.Location = new System.Drawing.Point(154, 45);
            this.btnSeleccionarExcel.Margin = new System.Windows.Forms.Padding(2);
            this.btnSeleccionarExcel.Name = "btnSeleccionarExcel";
            this.btnSeleccionarExcel.Size = new System.Drawing.Size(120, 24);
            this.btnSeleccionarExcel.TabIndex = 6;
            this.btnSeleccionarExcel.Text = "Seleccionar Excel";
            this.btnSeleccionarExcel.UseVisualStyleBackColor = true;
            this.btnSeleccionarExcel.Click += new System.EventHandler(this.btnSeleccionarExcel_Click);
            // 
            // txtRutaExcel
            // 
            this.txtRutaExcel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRutaExcel.Location = new System.Drawing.Point(281, 48);
            this.txtRutaExcel.Margin = new System.Windows.Forms.Padding(2);
            this.txtRutaExcel.Name = "txtRutaExcel";
            this.txtRutaExcel.ReadOnly = true;
            this.txtRutaExcel.Size = new System.Drawing.Size(766, 20);
            this.txtRutaExcel.TabIndex = 7;
            // 
            // btnComparar
            // 
            this.btnComparar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnComparar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnComparar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnComparar.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnComparar.ForeColor = System.Drawing.Color.White;
            this.btnComparar.Location = new System.Drawing.Point(1110, 45);
            this.btnComparar.Margin = new System.Windows.Forms.Padding(2);
            this.btnComparar.Name = "btnComparar";
            this.btnComparar.Size = new System.Drawing.Size(116, 24);
            this.btnComparar.TabIndex = 8;
            this.btnComparar.Text = "📊 Comparar";
            this.btnComparar.UseVisualStyleBackColor = false;
            this.btnComparar.Click += new System.EventHandler(this.btnComparar_Click);
            // 
            // lblExcel2
            // 
            this.lblExcel2.Location = new System.Drawing.Point(9, 84);
            this.lblExcel2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblExcel2.Name = "lblExcel2";
            this.lblExcel2.Size = new System.Drawing.Size(135, 16);
            this.lblExcel2.TabIndex = 9;
            this.lblExcel2.Text = "Comparación con Excel 2:";
            this.lblExcel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnSeleccionarExcel2
            // 
            this.btnSeleccionarExcel2.Location = new System.Drawing.Point(154, 80);
            this.btnSeleccionarExcel2.Margin = new System.Windows.Forms.Padding(2);
            this.btnSeleccionarExcel2.Name = "btnSeleccionarExcel2";
            this.btnSeleccionarExcel2.Size = new System.Drawing.Size(120, 24);
            this.btnSeleccionarExcel2.TabIndex = 10;
            this.btnSeleccionarExcel2.Text = "Seleccionar Excel 2";
            this.btnSeleccionarExcel2.UseVisualStyleBackColor = true;
            this.btnSeleccionarExcel2.Click += new System.EventHandler(this.btnSeleccionarExcel2_Click);
            // 
            // txtRutaExcel2
            // 
            this.txtRutaExcel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRutaExcel2.Location = new System.Drawing.Point(281, 83);
            this.txtRutaExcel2.Margin = new System.Windows.Forms.Padding(2);
            this.txtRutaExcel2.Name = "txtRutaExcel2";
            this.txtRutaExcel2.ReadOnly = true;
            this.txtRutaExcel2.Size = new System.Drawing.Size(766, 20);
            this.txtRutaExcel2.TabIndex = 11;
            // 
            // btnArreglarFallos
            // 
            this.btnArreglarFallos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnArreglarFallos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnArreglarFallos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnArreglarFallos.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnArreglarFallos.ForeColor = System.Drawing.Color.White;
            this.btnArreglarFallos.Location = new System.Drawing.Point(556, -7);
            this.btnArreglarFallos.Margin = new System.Windows.Forms.Padding(2);
            this.btnArreglarFallos.Name = "btnArreglarFallos";
            this.btnArreglarFallos.Size = new System.Drawing.Size(116, 24);
            this.btnArreglarFallos.TabIndex = 4;
            this.btnArreglarFallos.Text = "🛠️ Arreglar fallos";
            this.btnArreglarFallos.UseVisualStyleBackColor = false;
            this.btnArreglarFallos.Visible = false;
            this.btnArreglarFallos.Click += new System.EventHandler(this.btnArreglarFallos_Click);
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.lblEstado);
            this.panelBottom.Controls.Add(this.progressBar1);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 924);
            this.panelBottom.Margin = new System.Windows.Forms.Padding(2);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(1284, 37);
            this.panelBottom.TabIndex = 2;
            // 
            // lblEstado
            // 
            this.lblEstado.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEstado.AutoEllipsis = true;
            this.lblEstado.Location = new System.Drawing.Point(9, 11);
            this.lblEstado.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(976, 16);
            this.lblEstado.TabIndex = 4;
            this.lblEstado.Text = "Estado: Listo. Seleccione una opción.";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(994, 10);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(2);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(281, 16);
            this.progressBar1.TabIndex = 5;
            this.progressBar1.Visible = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 140);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gridValidas);
            this.splitContainer1.Panel1.Controls.Add(this.panelFiltrosValidas);
            this.splitContainer1.Panel1.Controls.Add(this.lblValidas);
            this.splitContainer1.Panel1.Controls.Add(this.btnArreglarFallos);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(9, 0, 9, 5);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.gridInvalidas);
            this.splitContainer1.Panel2.Controls.Add(this.panelFiltrosInvalidas);
            this.splitContainer1.Panel2.Controls.Add(this.lblInvalidas);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(9, 0, 9, 5);
            this.splitContainer1.Size = new System.Drawing.Size(1284, 784);
            this.splitContainer1.SplitterDistance = 379;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 3;
            // 
            // gridValidas
            // 
            this.gridValidas.AllowUserToAddRows = false;
            this.gridValidas.AllowUserToDeleteRows = false;
            this.gridValidas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridValidas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridValidas.Location = new System.Drawing.Point(9, 44);
            this.gridValidas.Margin = new System.Windows.Forms.Padding(2);
            this.gridValidas.Name = "gridValidas";
            this.gridValidas.ReadOnly = true;
            this.gridValidas.RowHeadersWidth = 51;
            this.gridValidas.RowTemplate.Height = 24;
            this.gridValidas.Size = new System.Drawing.Size(1266, 330);
            this.gridValidas.TabIndex = 7;
            // 
            // panelFiltrosValidas
            // 
            this.panelFiltrosValidas.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFiltrosValidas.Location = new System.Drawing.Point(9, 20);
            this.panelFiltrosValidas.Margin = new System.Windows.Forms.Padding(2);
            this.panelFiltrosValidas.Name = "panelFiltrosValidas";
            this.panelFiltrosValidas.Size = new System.Drawing.Size(1266, 24);
            this.panelFiltrosValidas.TabIndex = 10;
            // 
            // lblValidas
            // 
            this.lblValidas.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblValidas.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValidas.ForeColor = System.Drawing.Color.Green;
            this.lblValidas.Location = new System.Drawing.Point(9, 0);
            this.lblValidas.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblValidas.Name = "lblValidas";
            this.lblValidas.Size = new System.Drawing.Size(1266, 20);
            this.lblValidas.TabIndex = 6;
            this.lblValidas.Text = "Facturas Válidas (0)";
            this.lblValidas.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gridInvalidas
            // 
            this.gridInvalidas.AllowUserToAddRows = false;
            this.gridInvalidas.AllowUserToDeleteRows = false;
            this.gridInvalidas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridInvalidas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridInvalidas.Location = new System.Drawing.Point(9, 44);
            this.gridInvalidas.Margin = new System.Windows.Forms.Padding(2);
            this.gridInvalidas.Name = "gridInvalidas";
            this.gridInvalidas.ReadOnly = true;
            this.gridInvalidas.RowHeadersWidth = 51;
            this.gridInvalidas.RowTemplate.Height = 24;
            this.gridInvalidas.Size = new System.Drawing.Size(1266, 353);
            this.gridInvalidas.TabIndex = 9;
            // 
            // panelFiltrosInvalidas
            // 
            this.panelFiltrosInvalidas.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFiltrosInvalidas.Location = new System.Drawing.Point(9, 20);
            this.panelFiltrosInvalidas.Margin = new System.Windows.Forms.Padding(2);
            this.panelFiltrosInvalidas.Name = "panelFiltrosInvalidas";
            this.panelFiltrosInvalidas.Size = new System.Drawing.Size(1266, 24);
            this.panelFiltrosInvalidas.TabIndex = 10;
            // 
            // lblInvalidas
            // 
            this.lblInvalidas.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblInvalidas.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInvalidas.ForeColor = System.Drawing.Color.Red;
            this.lblInvalidas.Location = new System.Drawing.Point(9, 0);
            this.lblInvalidas.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblInvalidas.Name = "lblInvalidas";
            this.lblInvalidas.Size = new System.Drawing.Size(1266, 20);
            this.lblInvalidas.TabIndex = 8;
            this.lblInvalidas.Text = "Facturas Inválidas / Inconsistencias (0)";
            this.lblInvalidas.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(122, 20);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 961);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(604, 414);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Validador de CFDIs - Antigravity";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridValidas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridInvalidas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button btnSeleccionarCarpeta;
        private System.Windows.Forms.TextBox txtRutaCarpeta;
        private System.Windows.Forms.Button btnIniciar;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label lblValidas;
        private System.Windows.Forms.DataGridView gridValidas;
        private System.Windows.Forms.Label lblInvalidas;
        private System.Windows.Forms.DataGridView gridInvalidas;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem verToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem detallesToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem invalidosToolStripMenuItem;
        private System.Windows.Forms.Button btnArreglarFallos;
        private System.Windows.Forms.Label lblExcel;
        private System.Windows.Forms.Button btnSeleccionarExcel;
        private System.Windows.Forms.TextBox txtRutaExcel;
        private System.Windows.Forms.Button btnComparar;
        private System.Windows.Forms.Label lblExcel2;
        private System.Windows.Forms.Button btnSeleccionarExcel2;
        private System.Windows.Forms.TextBox txtRutaExcel2;
        private System.Windows.Forms.Panel panelFiltrosValidas;
        private System.Windows.Forms.Panel panelFiltrosInvalidas;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem tsAuditoria;
    }
}
