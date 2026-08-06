namespace WindowsFormsApp1
{
    partial class FormDetalles
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gridXmls = new System.Windows.Forms.DataGridView();
            this.lblLista = new System.Windows.Forms.Label();
            this.panelDetalles = new System.Windows.Forms.Panel();
            this.txtCertificado = new System.Windows.Forms.TextBox();
            this.lblCertificado = new System.Windows.Forms.Label();
            this.txtSello = new System.Windows.Forms.TextBox();
            this.lblSello = new System.Windows.Forms.Label();
            this.txtDetalleTecnico = new System.Windows.Forms.TextBox();
            this.lblDetalleTecnico = new System.Windows.Forms.Label();
            this.txtDiagnostico = new System.Windows.Forms.TextBox();
            this.lblDiagnostico = new System.Windows.Forms.Label();
            this.txtTienePdf = new System.Windows.Forms.TextBox();
            this.lblTienePdf = new System.Windows.Forms.Label();
            this.txtEsValido = new System.Windows.Forms.TextBox();
            this.lblEsValido = new System.Windows.Forms.Label();
            this.txtMetodoPago = new System.Windows.Forms.TextBox();
            this.lblMetodoPago = new System.Windows.Forms.Label();
            this.txtFormaPago = new System.Windows.Forms.TextBox();
            this.lblFormaPago = new System.Windows.Forms.Label();
            this.txtUuid = new System.Windows.Forms.TextBox();
            this.lblUuid = new System.Windows.Forms.Label();
            this.txtNombreArchivo = new System.Windows.Forms.TextBox();
            this.lblNombreArchivo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridXmls)).BeginInit();
            this.panelDetalles.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gridXmls);
            this.splitContainer1.Panel1.Controls.Add(this.lblLista);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(10);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panelDetalles);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(10);
            this.splitContainer1.Size = new System.Drawing.Size(1100, 650);
            this.splitContainer1.SplitterDistance = 380;
            this.splitContainer1.TabIndex = 0;
            // 
            // gridXmls
            // 
            this.gridXmls.AllowUserToAddRows = false;
            this.gridXmls.AllowUserToDeleteRows = false;
            this.gridXmls.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridXmls.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridXmls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridXmls.Location = new System.Drawing.Point(10, 35);
            this.gridXmls.MultiSelect = false;
            this.gridXmls.Name = "gridXmls";
            this.gridXmls.ReadOnly = true;
            this.gridXmls.RowHeadersWidth = 30;
            this.gridXmls.RowTemplate.Height = 24;
            this.gridXmls.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridXmls.Size = new System.Drawing.Size(360, 605);
            this.gridXmls.TabIndex = 1;
            this.gridXmls.SelectionChanged += new System.EventHandler(this.gridXmls_SelectionChanged);
            // 
            // lblLista
            // 
            this.lblLista.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblLista.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLista.Location = new System.Drawing.Point(10, 10);
            this.lblLista.Name = "lblLista";
            this.lblLista.Size = new System.Drawing.Size(360, 25);
            this.lblLista.TabIndex = 0;
            this.lblLista.Text = "Archivos XML Procesados:";
            this.lblLista.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelDetalles
            // 
            this.panelDetalles.AutoScroll = true;
            this.panelDetalles.Controls.Add(this.txtCertificado);
            this.panelDetalles.Controls.Add(this.lblCertificado);
            this.panelDetalles.Controls.Add(this.txtSello);
            this.panelDetalles.Controls.Add(this.lblSello);
            this.panelDetalles.Controls.Add(this.txtDetalleTecnico);
            this.panelDetalles.Controls.Add(this.lblDetalleTecnico);
            this.panelDetalles.Controls.Add(this.txtDiagnostico);
            this.panelDetalles.Controls.Add(this.lblDiagnostico);
            this.panelDetalles.Controls.Add(this.txtTienePdf);
            this.panelDetalles.Controls.Add(this.lblTienePdf);
            this.panelDetalles.Controls.Add(this.txtEsValido);
            this.panelDetalles.Controls.Add(this.lblEsValido);
            this.panelDetalles.Controls.Add(this.txtMetodoPago);
            this.panelDetalles.Controls.Add(this.lblMetodoPago);
            this.panelDetalles.Controls.Add(this.txtFormaPago);
            this.panelDetalles.Controls.Add(this.lblFormaPago);
            this.panelDetalles.Controls.Add(this.txtUuid);
            this.panelDetalles.Controls.Add(this.lblUuid);
            this.panelDetalles.Controls.Add(this.txtNombreArchivo);
            this.panelDetalles.Controls.Add(this.lblNombreArchivo);
            this.panelDetalles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDetalles.Location = new System.Drawing.Point(10, 10);
            this.panelDetalles.Name = "panelDetalles";
            this.panelDetalles.Size = new System.Drawing.Size(696, 630);
            this.panelDetalles.TabIndex = 0;
            // 
            // txtCertificado
            // 
            this.txtCertificado.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCertificado.Font = new System.Drawing.Font("Consolas", 8F);
            this.txtCertificado.Location = new System.Drawing.Point(15, 510);
            this.txtCertificado.Multiline = true;
            this.txtCertificado.Name = "txtCertificado";
            this.txtCertificado.ReadOnly = true;
            this.txtCertificado.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCertificado.Size = new System.Drawing.Size(660, 95);
            this.txtCertificado.TabIndex = 19;
            // 
            // lblCertificado
            // 
            this.lblCertificado.AutoSize = true;
            this.lblCertificado.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold);
            this.lblCertificado.Location = new System.Drawing.Point(12, 490);
            this.lblCertificado.Name = "lblCertificado";
            this.lblCertificado.Size = new System.Drawing.Size(129, 16);
            this.lblCertificado.TabIndex = 18;
            this.lblCertificado.Text = "Certificado (Base64):";
            // 
            // txtSello
            // 
            this.txtSello.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSello.Font = new System.Drawing.Font("Consolas", 8F);
            this.txtSello.Location = new System.Drawing.Point(15, 390);
            this.txtSello.Multiline = true;
            this.txtSello.Name = "txtSello";
            this.txtSello.ReadOnly = true;
            this.txtSello.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSello.Size = new System.Drawing.Size(660, 90);
            this.txtSello.TabIndex = 17;
            // 
            // lblSello
            // 
            this.lblSello.AutoSize = true;
            this.lblSello.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold);
            this.lblSello.Location = new System.Drawing.Point(12, 370);
            this.lblSello.Name = "lblSello";
            this.lblSello.Size = new System.Drawing.Size(149, 16);
            this.lblSello.TabIndex = 16;
            this.lblSello.Text = "Sello Digital (Sello CFD):";
            // 
            // txtDetalleTecnico
            // 
            this.txtDetalleTecnico.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDetalleTecnico.Location = new System.Drawing.Point(15, 305);
            this.txtDetalleTecnico.Multiline = true;
            this.txtDetalleTecnico.Name = "txtDetalleTecnico";
            this.txtDetalleTecnico.ReadOnly = true;
            this.txtDetalleTecnico.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDetalleTecnico.Size = new System.Drawing.Size(660, 55);
            this.txtDetalleTecnico.TabIndex = 15;
            // 
            // lblDetalleTecnico
            // 
            this.lblDetalleTecnico.AutoSize = true;
            this.lblDetalleTecnico.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold);
            this.lblDetalleTecnico.Location = new System.Drawing.Point(12, 285);
            this.lblDetalleTecnico.Name = "lblDetalleTecnico";
            this.lblDetalleTecnico.Size = new System.Drawing.Size(120, 16);
            this.lblDetalleTecnico.TabIndex = 14;
            this.lblDetalleTecnico.Text = "Detalle Técnico:";
            // 
            // txtDiagnostico
            // 
            this.txtDiagnostico.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDiagnostico.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.txtDiagnostico.Location = new System.Drawing.Point(15, 230);
            this.txtDiagnostico.Multiline = true;
            this.txtDiagnostico.Name = "txtDiagnostico";
            this.txtDiagnostico.ReadOnly = true;
            this.txtDiagnostico.Size = new System.Drawing.Size(660, 48);
            this.txtDiagnostico.TabIndex = 13;
            // 
            // lblDiagnostico
            // 
            this.lblDiagnostico.AutoSize = true;
            this.lblDiagnostico.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold);
            this.lblDiagnostico.Location = new System.Drawing.Point(12, 210);
            this.lblDiagnostico.Name = "lblDiagnostico";
            this.lblDiagnostico.Size = new System.Drawing.Size(180, 16);
            this.lblDiagnostico.TabIndex = 12;
            this.lblDiagnostico.Text = "Diagnóstico del Sistema:";
            // 
            // txtTienePdf
            // 
            this.txtTienePdf.Location = new System.Drawing.Point(450, 170);
            this.txtTienePdf.Name = "txtTienePdf";
            this.txtTienePdf.ReadOnly = true;
            this.txtTienePdf.Size = new System.Drawing.Size(225, 22);
            this.txtTienePdf.TabIndex = 11;
            // 
            // lblTienePdf
            // 
            this.lblTienePdf.AutoSize = true;
            this.lblTienePdf.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold);
            this.lblTienePdf.Location = new System.Drawing.Point(447, 150);
            this.lblTienePdf.Name = "lblTienePdf";
            this.lblTienePdf.Size = new System.Drawing.Size(81, 16);
            this.lblTienePdf.TabIndex = 10;
            this.lblTienePdf.Text = "Tiene PDF:";
            // 
            // txtEsValido
            // 
            this.txtEsValido.Location = new System.Drawing.Point(15, 170);
            this.txtEsValido.Name = "txtEsValido";
            this.txtEsValido.ReadOnly = true;
            this.txtEsValido.Size = new System.Drawing.Size(410, 22);
            this.txtEsValido.TabIndex = 9;
            // 
            // lblEsValido
            // 
            this.lblEsValido.AutoSize = true;
            this.lblEsValido.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold);
            this.lblEsValido.Location = new System.Drawing.Point(12, 150);
            this.lblEsValido.Name = "lblEsValido";
            this.lblEsValido.Size = new System.Drawing.Size(77, 16);
            this.lblEsValido.TabIndex = 8;
            this.lblEsValido.Text = "Es Válido:";
            // 
            // txtMetodoPago
            // 
            this.txtMetodoPago.Location = new System.Drawing.Point(450, 115);
            this.txtMetodoPago.Name = "txtMetodoPago";
            this.txtMetodoPago.ReadOnly = true;
            this.txtMetodoPago.Size = new System.Drawing.Size(225, 22);
            this.txtMetodoPago.TabIndex = 7;
            // 
            // lblMetodoPago
            // 
            this.lblMetodoPago.AutoSize = true;
            this.lblMetodoPago.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold);
            this.lblMetodoPago.Location = new System.Drawing.Point(447, 95);
            this.lblMetodoPago.Name = "lblMetodoPago";
            this.lblMetodoPago.Size = new System.Drawing.Size(104, 16);
            this.lblMetodoPago.TabIndex = 6;
            this.lblMetodoPago.Text = "Método Pago:";
            // 
            // txtFormaPago
            // 
            this.txtFormaPago.Location = new System.Drawing.Point(15, 115);
            this.txtFormaPago.Name = "txtFormaPago";
            this.txtFormaPago.ReadOnly = true;
            this.txtFormaPago.Size = new System.Drawing.Size(410, 22);
            this.txtFormaPago.TabIndex = 5;
            // 
            // lblFormaPago
            // 
            this.lblFormaPago.AutoSize = true;
            this.lblFormaPago.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold);
            this.lblFormaPago.Location = new System.Drawing.Point(12, 95);
            this.lblFormaPago.Name = "lblFormaPago";
            this.lblFormaPago.Size = new System.Drawing.Size(96, 16);
            this.lblFormaPago.TabIndex = 4;
            this.lblFormaPago.Text = "Forma Pago:";
            // 
            // txtUuid
            // 
            this.txtUuid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUuid.Location = new System.Drawing.Point(15, 65);
            this.txtUuid.Name = "txtUuid";
            this.txtUuid.ReadOnly = true;
            this.txtUuid.Size = new System.Drawing.Size(660, 22);
            this.txtUuid.TabIndex = 3;
            // 
            // lblUuid
            // 
            this.lblUuid.AutoSize = true;
            this.lblUuid.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold);
            this.lblUuid.Location = new System.Drawing.Point(12, 45);
            this.lblUuid.Name = "lblUuid";
            this.lblUuid.Size = new System.Drawing.Size(140, 16);
            this.lblUuid.TabIndex = 2;
            this.lblUuid.Text = "UUID (Folio Fiscal):";
            // 
            // txtNombreArchivo
            // 
            this.txtNombreArchivo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNombreArchivo.Location = new System.Drawing.Point(15, 20);
            this.txtNombreArchivo.Name = "txtNombreArchivo";
            this.txtNombreArchivo.ReadOnly = true;
            this.txtNombreArchivo.Size = new System.Drawing.Size(660, 22);
            this.txtNombreArchivo.TabIndex = 1;
            // 
            // lblNombreArchivo
            // 
            this.lblNombreArchivo.AutoSize = true;
            this.lblNombreArchivo.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold);
            this.lblNombreArchivo.Location = new System.Drawing.Point(12, 2);
            this.lblNombreArchivo.Name = "lblNombreArchivo";
            this.lblNombreArchivo.Size = new System.Drawing.Size(148, 16);
            this.lblNombreArchivo.TabIndex = 0;
            this.lblNombreArchivo.Text = "Archivo / Subcarpeta:";
            // 
            // FormDetalles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 650);
            this.Controls.Add(this.splitContainer1);
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "FormDetalles";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Detalles Completos del CFDI - Antigravity";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridXmls)).EndInit();
            this.panelDetalles.ResumeLayout(false);
            this.panelDetalles.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView gridXmls;
        private System.Windows.Forms.Label lblLista;
        private System.Windows.Forms.Panel panelDetalles;
        private System.Windows.Forms.TextBox txtNombreArchivo;
        private System.Windows.Forms.Label lblNombreArchivo;
        private System.Windows.Forms.TextBox txtUuid;
        private System.Windows.Forms.Label lblUuid;
        private System.Windows.Forms.TextBox txtMetodoPago;
        private System.Windows.Forms.Label lblMetodoPago;
        private System.Windows.Forms.TextBox txtFormaPago;
        private System.Windows.Forms.Label lblFormaPago;
        private System.Windows.Forms.TextBox txtTienePdf;
        private System.Windows.Forms.Label lblTienePdf;
        private System.Windows.Forms.TextBox txtEsValido;
        private System.Windows.Forms.Label lblEsValido;
        private System.Windows.Forms.TextBox txtDiagnostico;
        private System.Windows.Forms.Label lblDiagnostico;
        private System.Windows.Forms.TextBox txtDetalleTecnico;
        private System.Windows.Forms.Label lblDetalleTecnico;
        private System.Windows.Forms.TextBox txtSello;
        private System.Windows.Forms.Label lblSello;
        private System.Windows.Forms.TextBox txtCertificado;
        private System.Windows.Forms.Label lblCertificado;
    }
}
