namespace WindowsFormsApp1
{
    partial class FormComparacion
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

        #region Código generado por el Diseñador de Windows Forms

        private void InitializeComponent()
        {
            this.panelSuperior = new System.Windows.Forms.Panel();
            this.lblResumen = new System.Windows.Forms.Label();
            this.txtBuscarUuid = new System.Windows.Forms.TextBox();
            this.lblBuscarUuid = new System.Windows.Forms.Label();
            this.cmbResultado = new System.Windows.Forms.ComboBox();
            this.lblResultadoFiltro = new System.Windows.Forms.Label();
            this.cmbFuente = new System.Windows.Forms.ComboBox();
            this.lblFuenteFiltro = new System.Windows.Forms.Label();
            this.panelFiltrosColumnas = new System.Windows.Forms.Panel();
            this.gridComparacion = new System.Windows.Forms.DataGridView();
            this.panelSuperior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridComparacion)).BeginInit();
            this.SuspendLayout();
            // 
            // panelSuperior
            // 
            this.panelSuperior.Controls.Add(this.lblResumen);
            this.panelSuperior.Controls.Add(this.txtBuscarUuid);
            this.panelSuperior.Controls.Add(this.lblBuscarUuid);
            this.panelSuperior.Controls.Add(this.cmbResultado);
            this.panelSuperior.Controls.Add(this.lblResultadoFiltro);
            this.panelSuperior.Controls.Add(this.cmbFuente);
            this.panelSuperior.Controls.Add(this.lblFuenteFiltro);
            this.panelSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSuperior.Location = new System.Drawing.Point(0, 0);
            this.panelSuperior.Name = "panelSuperior";
            this.panelSuperior.Size = new System.Drawing.Size(1284, 66);
            this.panelSuperior.TabIndex = 0;
            // 
            // lblResumen
            // 
            this.lblResumen.AutoSize = true;
            this.lblResumen.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResumen.Location = new System.Drawing.Point(10, 42);
            this.lblResumen.Name = "lblResumen";
            this.lblResumen.Size = new System.Drawing.Size(259, 13);
            this.lblResumen.TabIndex = 6;
            this.lblResumen.Text = "Coincide: 0 | Diferente: 0 | No encontrado: 0";
            // 
            // txtBuscarUuid
            // 
            this.txtBuscarUuid.Location = new System.Drawing.Point(660, 10);
            this.txtBuscarUuid.Name = "txtBuscarUuid";
            this.txtBuscarUuid.Size = new System.Drawing.Size(220, 20);
            this.txtBuscarUuid.TabIndex = 5;
            this.txtBuscarUuid.TextChanged += new System.EventHandler(this.FiltroSuperior_Changed);
            // 
            // lblBuscarUuid
            // 
            this.lblBuscarUuid.AutoSize = true;
            this.lblBuscarUuid.Location = new System.Drawing.Point(560, 14);
            this.lblBuscarUuid.Name = "lblBuscarUuid";
            this.lblBuscarUuid.Size = new System.Drawing.Size(84, 13);
            this.lblBuscarUuid.TabIndex = 4;
            this.lblBuscarUuid.Text = "UUID / Archivo:";
            // 
            // cmbResultado
            // 
            this.cmbResultado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbResultado.FormattingEnabled = true;
            this.cmbResultado.Location = new System.Drawing.Point(365, 10);
            this.cmbResultado.Name = "cmbResultado";
            this.cmbResultado.Size = new System.Drawing.Size(180, 21);
            this.cmbResultado.TabIndex = 3;
            this.cmbResultado.SelectedIndexChanged += new System.EventHandler(this.FiltroSuperior_Changed);
            // 
            // lblResultadoFiltro
            // 
            this.lblResultadoFiltro.AutoSize = true;
            this.lblResultadoFiltro.Location = new System.Drawing.Point(300, 14);
            this.lblResultadoFiltro.Name = "lblResultadoFiltro";
            this.lblResultadoFiltro.Size = new System.Drawing.Size(58, 13);
            this.lblResultadoFiltro.TabIndex = 2;
            this.lblResultadoFiltro.Text = "Resultado:";
            // 
            // cmbFuente
            // 
            this.cmbFuente.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFuente.FormattingEnabled = true;
            this.cmbFuente.Location = new System.Drawing.Point(65, 10);
            this.cmbFuente.Name = "cmbFuente";
            this.cmbFuente.Size = new System.Drawing.Size(220, 21);
            this.cmbFuente.TabIndex = 1;
            this.cmbFuente.SelectedIndexChanged += new System.EventHandler(this.FiltroSuperior_Changed);
            // 
            // lblFuenteFiltro
            // 
            this.lblFuenteFiltro.AutoSize = true;
            this.lblFuenteFiltro.Location = new System.Drawing.Point(10, 14);
            this.lblFuenteFiltro.Name = "lblFuenteFiltro";
            this.lblFuenteFiltro.Size = new System.Drawing.Size(43, 13);
            this.lblFuenteFiltro.TabIndex = 0;
            this.lblFuenteFiltro.Text = "Fuente:";
            // 
            // panelFiltrosColumnas
            // 
            this.panelFiltrosColumnas.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFiltrosColumnas.Location = new System.Drawing.Point(0, 66);
            this.panelFiltrosColumnas.Name = "panelFiltrosColumnas";
            this.panelFiltrosColumnas.Size = new System.Drawing.Size(1284, 26);
            this.panelFiltrosColumnas.TabIndex = 1;
            // 
            // gridComparacion
            // 
            this.gridComparacion.AllowUserToAddRows = false;
            this.gridComparacion.AllowUserToDeleteRows = false;
            this.gridComparacion.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridComparacion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridComparacion.Location = new System.Drawing.Point(0, 92);
            this.gridComparacion.Name = "gridComparacion";
            this.gridComparacion.ReadOnly = true;
            this.gridComparacion.RowHeadersWidth = 51;
            this.gridComparacion.RowTemplate.Height = 24;
            this.gridComparacion.Size = new System.Drawing.Size(1284, 869);
            this.gridComparacion.TabIndex = 2;
            this.gridComparacion.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gridComparacion_CellFormatting);
            this.gridComparacion.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.gridComparacion_DataBindingComplete);
            // 
            // FormComparacion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 961);
            this.Controls.Add(this.gridComparacion);
            this.Controls.Add(this.panelFiltrosColumnas);
            this.Controls.Add(this.panelSuperior);
            this.MinimumSize = new System.Drawing.Size(760, 420);
            this.Name = "FormComparacion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Comparación XML vs Excel";
            this.panelSuperior.ResumeLayout(false);
            this.panelSuperior.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridComparacion)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelSuperior;
        private System.Windows.Forms.Label lblFuenteFiltro;
        private System.Windows.Forms.ComboBox cmbFuente;
        private System.Windows.Forms.Label lblResultadoFiltro;
        private System.Windows.Forms.ComboBox cmbResultado;
        private System.Windows.Forms.Label lblBuscarUuid;
        private System.Windows.Forms.TextBox txtBuscarUuid;
        private System.Windows.Forms.Label lblResumen;
        private System.Windows.Forms.Panel panelFiltrosColumnas;
        private System.Windows.Forms.DataGridView gridComparacion;
    }
}