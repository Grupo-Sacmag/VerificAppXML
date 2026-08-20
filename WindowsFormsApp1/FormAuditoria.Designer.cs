namespace WindowsFormsApp1
{
    partial class FormAuditoria
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
            this.txtPatronUuid = new System.Windows.Forms.TextBox();
            this.lblPatronUuid = new System.Windows.Forms.Label();
            this.cmbEstado = new System.Windows.Forms.ComboBox();
            this.lblEstadoFiltro = new System.Windows.Forms.Label();
            this.panelFiltrosColumnas = new System.Windows.Forms.Panel();
            this.gridAuditoria = new System.Windows.Forms.DataGridView();
            this.panelSuperior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridAuditoria)).BeginInit();
            this.SuspendLayout();
            // 
            // panelSuperior
            // 
            this.panelSuperior.Controls.Add(this.txtPatronUuid);
            this.panelSuperior.Controls.Add(this.lblPatronUuid);
            this.panelSuperior.Controls.Add(this.cmbEstado);
            this.panelSuperior.Controls.Add(this.lblEstadoFiltro);
            this.panelSuperior.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSuperior.Location = new System.Drawing.Point(0, 0);
            this.panelSuperior.Name = "panelSuperior";
            this.panelSuperior.Size = new System.Drawing.Size(1284, 40);
            this.panelSuperior.TabIndex = 0;
            // 
            // txtPatronUuid
            // 
            this.txtPatronUuid.Location = new System.Drawing.Point(400, 10);
            this.txtPatronUuid.Name = "txtPatronUuid";
            this.txtPatronUuid.Size = new System.Drawing.Size(200, 20);
            this.txtPatronUuid.TabIndex = 3;
            this.txtPatronUuid.TextChanged += new System.EventHandler(this.txtPatronUuid_TextChanged);
            // 
            // lblPatronUuid
            // 
            this.lblPatronUuid.AutoSize = true;
            this.lblPatronUuid.Location = new System.Drawing.Point(260, 14);
            this.lblPatronUuid.Name = "lblPatronUuid";
            this.lblPatronUuid.Size = new System.Drawing.Size(114, 13);
            this.lblPatronUuid.TabIndex = 2;
            this.lblPatronUuid.Text = "Folio / UUID contiene:";
            // 
            // cmbEstado
            // 
            this.cmbEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEstado.FormattingEnabled = true;
            this.cmbEstado.Location = new System.Drawing.Point(65, 10);
            this.cmbEstado.Name = "cmbEstado";
            this.cmbEstado.Size = new System.Drawing.Size(180, 21);
            this.cmbEstado.TabIndex = 1;
            this.cmbEstado.SelectedIndexChanged += new System.EventHandler(this.cmbEstado_SelectedIndexChanged);
            // 
            // lblEstadoFiltro
            // 
            this.lblEstadoFiltro.AutoSize = true;
            this.lblEstadoFiltro.Location = new System.Drawing.Point(10, 14);
            this.lblEstadoFiltro.Name = "lblEstadoFiltro";
            this.lblEstadoFiltro.Size = new System.Drawing.Size(43, 13);
            this.lblEstadoFiltro.TabIndex = 0;
            this.lblEstadoFiltro.Text = "Estado:";
            // 
            // panelFiltrosColumnas
            // 
            this.panelFiltrosColumnas.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFiltrosColumnas.Location = new System.Drawing.Point(0, 40);
            this.panelFiltrosColumnas.Name = "panelFiltrosColumnas";
            this.panelFiltrosColumnas.Size = new System.Drawing.Size(1284, 26);
            this.panelFiltrosColumnas.TabIndex = 1;
            // 
            // gridAuditoria
            // 
            this.gridAuditoria.AllowUserToAddRows = false;
            this.gridAuditoria.AllowUserToDeleteRows = false;
            this.gridAuditoria.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridAuditoria.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridAuditoria.Location = new System.Drawing.Point(0, 66);
            this.gridAuditoria.Name = "gridAuditoria";
            this.gridAuditoria.ReadOnly = true;
            this.gridAuditoria.RowHeadersWidth = 51;
            this.gridAuditoria.RowTemplate.Height = 24;
            this.gridAuditoria.Size = new System.Drawing.Size(1284, 895);
            this.gridAuditoria.TabIndex = 2;
            this.gridAuditoria.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.gridAuditoria_DataBindingComplete);
            // 
            // FormAuditoria
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 961);
            this.Controls.Add(this.gridAuditoria);
            this.Controls.Add(this.panelFiltrosColumnas);
            this.Controls.Add(this.panelSuperior);
            this.MinimumSize = new System.Drawing.Size(700, 400);
            this.Name = "FormAuditoria";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Auditoría — Criterios de Inclusión/Exclusión por UUID";
            this.panelSuperior.ResumeLayout(false);
            this.panelSuperior.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridAuditoria)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelSuperior;
        private System.Windows.Forms.Label lblEstadoFiltro;
        private System.Windows.Forms.ComboBox cmbEstado;
        private System.Windows.Forms.Label lblPatronUuid;
        private System.Windows.Forms.TextBox txtPatronUuid;
        private System.Windows.Forms.Panel panelFiltrosColumnas;
        private System.Windows.Forms.DataGridView gridAuditoria;
    }
}