namespace WindowsFormsApp1
{
    partial class FormDetalleConciliacion
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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.grpBanco = new System.Windows.Forms.GroupBox();
            this.lblResumenPaquete = new System.Windows.Forms.Label();
            this.gridMovimientosVinculados = new System.Windows.Forms.DataGridView();
            this.txtDescripcionBanco = new System.Windows.Forms.TextBox();
            this.lblDescLabel = new System.Windows.Forms.Label();
            this.lblMontoBanco = new System.Windows.Forms.Label();
            this.lblMontoLabel = new System.Windows.Forms.Label();
            this.lblRfcBanco = new System.Windows.Forms.Label();
            this.lblRfcBancoLabel = new System.Windows.Forms.Label();
            this.lblFechaCompra = new System.Windows.Forms.Label();
            this.lblFechaCompraLabel = new System.Windows.Forms.Label();
            this.lblFechaReg = new System.Windows.Forms.Label();
            this.lblFechaRegLabel = new System.Windows.Forms.Label();
            this.grpXml = new System.Windows.Forms.GroupBox();
            this.btnAsociarXml = new System.Windows.Forms.Button();
            this.btnVerCarpetaXml = new System.Windows.Forms.Button();
            this.btnAbrirXml = new System.Windows.Forms.Button();
            this.txtRutaXml = new System.Windows.Forms.TextBox();
            this.lblRutaXmlLabel = new System.Windows.Forms.Label();
            this.txtUuid = new System.Windows.Forms.TextBox();
            this.lblUuidLabel = new System.Windows.Forms.Label();
            this.lblDiferencia = new System.Windows.Forms.Label();
            this.lblDiferenciaLabel = new System.Windows.Forms.Label();
            this.lblSubtotalXml = new System.Windows.Forms.Label();
            this.lblSubtotalLabel = new System.Windows.Forms.Label();
            this.lblTotalXml = new System.Windows.Forms.Label();
            this.lblTotalXmlLabel = new System.Windows.Forms.Label();
            this.lblProveedor = new System.Windows.Forms.Label();
            this.lblProveedorLabel = new System.Windows.Forms.Label();
            this.lblRfcXml = new System.Windows.Forms.Label();
            this.lblRfcXmlLabel = new System.Windows.Forms.Label();
            this.grpPdf = new System.Windows.Forms.GroupBox();
            this.btnEncontrarPdf = new System.Windows.Forms.Button();
            this.btnVerCarpetaPdf = new System.Windows.Forms.Button();
            this.btnAbrirPdf = new System.Windows.Forms.Button();
            this.txtRutaPdf = new System.Windows.Forms.TextBox();
            this.lblRutaPdfLabel = new System.Windows.Forms.Label();
            this.lblEstadoPdf = new System.Windows.Forms.Label();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.btnDesamarrar = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            this.grpBanco.SuspendLayout();
            this.grpXml.SuspendLayout();
            this.grpPdf.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.pnlHeader.Controls.Add(this.lblSubtitulo);
            this.pnlHeader.Controls.Add(this.lblTitulo);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(784, 65);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblSubtitulo
            // 
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSubtitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.lblSubtitulo.Location = new System.Drawing.Point(18, 38);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(250, 15);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Operación #1 | Estado: Conciliado";
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(16, 12);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(431, 21);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Detalle del Movimiento Bancario y Comprobantes Fiscales";
            // 
            // grpBanco
            // 
            this.grpBanco.Controls.Add(this.lblResumenPaquete);
            this.grpBanco.Controls.Add(this.gridMovimientosVinculados);
            this.grpBanco.Controls.Add(this.txtDescripcionBanco);
            this.grpBanco.Controls.Add(this.lblDescLabel);
            this.grpBanco.Controls.Add(this.lblMontoBanco);
            this.grpBanco.Controls.Add(this.lblMontoLabel);
            this.grpBanco.Controls.Add(this.lblRfcBanco);
            this.grpBanco.Controls.Add(this.lblRfcBancoLabel);
            this.grpBanco.Controls.Add(this.lblFechaCompra);
            this.grpBanco.Controls.Add(this.lblFechaCompraLabel);
            this.grpBanco.Controls.Add(this.lblFechaReg);
            this.grpBanco.Controls.Add(this.lblFechaRegLabel);
            this.grpBanco.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpBanco.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpBanco.Location = new System.Drawing.Point(16, 72);
            this.grpBanco.Name = "grpBanco";
            this.grpBanco.Size = new System.Drawing.Size(752, 252);
            this.grpBanco.TabIndex = 1;
            this.grpBanco.TabStop = false;
            this.grpBanco.Text = "🏦 Movimientos del Estado de Cuenta que llenan esta Factura";
            // 
            // lblResumenPaquete
            // 
            this.lblResumenPaquete.AutoSize = true;
            this.lblResumenPaquete.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblResumenPaquete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.lblResumenPaquete.Location = new System.Drawing.Point(15, 78);
            this.lblResumenPaquete.Name = "lblResumenPaquete";
            this.lblResumenPaquete.Size = new System.Drawing.Size(250, 15);
            this.lblResumenPaquete.TabIndex = 10;
            this.lblResumenPaquete.Text = "Movimientos vinculados:";
            // 
            // gridMovimientosVinculados
            // 
            this.gridMovimientosVinculados.AllowUserToAddRows = false;
            this.gridMovimientosVinculados.AllowUserToDeleteRows = false;
            this.gridMovimientosVinculados.BackgroundColor = System.Drawing.Color.White;
            this.gridMovimientosVinculados.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gridMovimientosVinculados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.gridMovimientosVinculados.ColumnHeadersHeight = 26;
            this.gridMovimientosVinculados.Location = new System.Drawing.Point(15, 98);
            this.gridMovimientosVinculados.MultiSelect = false;
            this.gridMovimientosVinculados.Name = "gridMovimientosVinculados";
            this.gridMovimientosVinculados.ReadOnly = true;
            this.gridMovimientosVinculados.RowHeadersVisible = false;
            this.gridMovimientosVinculados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridMovimientosVinculados.Size = new System.Drawing.Size(722, 102);
            this.gridMovimientosVinculados.TabIndex = 11;
            this.gridMovimientosVinculados.SelectionChanged += new System.EventHandler(this.gridMovimientosVinculados_SelectionChanged);
            // 
            // txtDescripcionBanco
            // 
            this.txtDescripcionBanco.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.txtDescripcionBanco.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtDescripcionBanco.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.txtDescripcionBanco.Location = new System.Drawing.Point(100, 208);
            this.txtDescripcionBanco.Multiline = true;
            this.txtDescripcionBanco.Name = "txtDescripcionBanco";
            this.txtDescripcionBanco.ReadOnly = true;
            this.txtDescripcionBanco.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescripcionBanco.Size = new System.Drawing.Size(637, 36);
            this.txtDescripcionBanco.TabIndex = 9;
            // 
            // lblDescLabel
            // 
            this.lblDescLabel.AutoSize = true;
            this.lblDescLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblDescLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblDescLabel.Location = new System.Drawing.Point(15, 210);
            this.lblDescLabel.Name = "lblDescLabel";
            this.lblDescLabel.Size = new System.Drawing.Size(72, 15);
            this.lblDescLabel.TabIndex = 8;
            this.lblDescLabel.Text = "Descripción:";
            // 
            // lblMontoBanco
            // 
            this.lblMontoBanco.AutoSize = true;
            this.lblMontoBanco.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblMontoBanco.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(118)))), ((int)(((byte)(110)))));
            this.lblMontoBanco.Location = new System.Drawing.Point(585, 27);
            this.lblMontoBanco.Name = "lblMontoBanco";
            this.lblMontoBanco.Size = new System.Drawing.Size(78, 20);
            this.lblMontoBanco.TabIndex = 7;
            this.lblMontoBanco.Text = "$0,000.00";
            // 
            // lblMontoLabel
            // 
            this.lblMontoLabel.AutoSize = true;
            this.lblMontoLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblMontoLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblMontoLabel.Location = new System.Drawing.Point(495, 30);
            this.lblMontoLabel.Name = "lblMontoLabel";
            this.lblMontoLabel.Size = new System.Drawing.Size(81, 15);
            this.lblMontoLabel.TabIndex = 6;
            this.lblMontoLabel.Text = "Cargo / Importe:";
            // 
            // lblRfcBanco
            // 
            this.lblRfcBanco.AutoSize = true;
            this.lblRfcBanco.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblRfcBanco.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblRfcBanco.Location = new System.Drawing.Point(100, 57);
            this.lblRfcBanco.Name = "lblRfcBanco";
            this.lblRfcBanco.Size = new System.Drawing.Size(12, 15);
            this.lblRfcBanco.TabIndex = 5;
            this.lblRfcBanco.Text = "-";
            // 
            // lblRfcBancoLabel
            // 
            this.lblRfcBancoLabel.AutoSize = true;
            this.lblRfcBancoLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblRfcBancoLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblRfcBancoLabel.Location = new System.Drawing.Point(15, 57);
            this.lblRfcBancoLabel.Name = "lblRfcBancoLabel";
            this.lblRfcBancoLabel.Size = new System.Drawing.Size(66, 15);
            this.lblRfcBancoLabel.TabIndex = 4;
            this.lblRfcBancoLabel.Text = "RFC Banco:";
            // 
            // lblFechaCompra
            // 
            this.lblFechaCompra.AutoSize = true;
            this.lblFechaCompra.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblFechaCompra.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblFechaCompra.Location = new System.Drawing.Point(340, 30);
            this.lblFechaCompra.Name = "lblFechaCompra";
            this.lblFechaCompra.Size = new System.Drawing.Size(12, 15);
            this.lblFechaCompra.TabIndex = 3;
            this.lblFechaCompra.Text = "-";
            // 
            // lblFechaCompraLabel
            // 
            this.lblFechaCompraLabel.AutoSize = true;
            this.lblFechaCompraLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblFechaCompraLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblFechaCompraLabel.Location = new System.Drawing.Point(245, 30);
            this.lblFechaCompraLabel.Name = "lblFechaCompraLabel";
            this.lblFechaCompraLabel.Size = new System.Drawing.Size(86, 15);
            this.lblFechaCompraLabel.TabIndex = 2;
            this.lblFechaCompraLabel.Text = "Fecha Compra:";
            // 
            // lblFechaReg
            // 
            this.lblFechaReg.AutoSize = true;
            this.lblFechaReg.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblFechaReg.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblFechaReg.Location = new System.Drawing.Point(100, 30);
            this.lblFechaReg.Name = "lblFechaReg";
            this.lblFechaReg.Size = new System.Drawing.Size(12, 15);
            this.lblFechaReg.TabIndex = 1;
            this.lblFechaReg.Text = "-";
            // 
            // lblFechaRegLabel
            // 
            this.lblFechaRegLabel.AutoSize = true;
            this.lblFechaRegLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblFechaRegLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblFechaRegLabel.Location = new System.Drawing.Point(15, 30);
            this.lblFechaRegLabel.Name = "lblFechaRegLabel";
            this.lblFechaRegLabel.Size = new System.Drawing.Size(88, 15);
            this.lblFechaRegLabel.TabIndex = 0;
            this.lblFechaRegLabel.Text = "Fecha Registro:";
            // 
            // grpXml
            // 
            this.grpXml.Controls.Add(this.btnAsociarXml);
            this.grpXml.Controls.Add(this.btnVerCarpetaXml);
            this.grpXml.Controls.Add(this.btnAbrirXml);
            this.grpXml.Controls.Add(this.txtRutaXml);
            this.grpXml.Controls.Add(this.lblRutaXmlLabel);
            this.grpXml.Controls.Add(this.txtUuid);
            this.grpXml.Controls.Add(this.lblUuidLabel);
            this.grpXml.Controls.Add(this.lblDiferencia);
            this.grpXml.Controls.Add(this.lblDiferenciaLabel);
            this.grpXml.Controls.Add(this.lblSubtotalXml);
            this.grpXml.Controls.Add(this.lblSubtotalLabel);
            this.grpXml.Controls.Add(this.lblTotalXml);
            this.grpXml.Controls.Add(this.lblTotalXmlLabel);
            this.grpXml.Controls.Add(this.lblProveedor);
            this.grpXml.Controls.Add(this.lblProveedorLabel);
            this.grpXml.Controls.Add(this.lblRfcXml);
            this.grpXml.Controls.Add(this.lblRfcXmlLabel);
            this.grpXml.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpXml.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpXml.Location = new System.Drawing.Point(16, 334);
            this.grpXml.Name = "grpXml";
            this.grpXml.Size = new System.Drawing.Size(752, 175);
            this.grpXml.TabIndex = 2;
            this.grpXml.TabStop = false;
            this.grpXml.Text = "📄 Factura XML Amarrada";
            // 
            // btnAsociarXml
            // 
            this.btnAsociarXml.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(116)))), ((int)(((byte)(144)))));
            this.btnAsociarXml.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAsociarXml.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnAsociarXml.ForeColor = System.Drawing.Color.White;
            this.btnAsociarXml.Location = new System.Drawing.Point(365, 137);
            this.btnAsociarXml.Name = "btnAsociarXml";
            this.btnAsociarXml.Size = new System.Drawing.Size(125, 26);
            this.btnAsociarXml.TabIndex = 16;
            this.btnAsociarXml.Text = "🔎 Asociar XML...";
            this.btnAsociarXml.UseVisualStyleBackColor = false;
            this.btnAsociarXml.Click += new System.EventHandler(this.btnAsociarXml_Click);
            // 
            // btnVerCarpetaXml
            // 
            this.btnVerCarpetaXml.BackColor = System.Drawing.Color.White;
            this.btnVerCarpetaXml.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnVerCarpetaXml.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.btnVerCarpetaXml.Location = new System.Drawing.Point(620, 137);
            this.btnVerCarpetaXml.Name = "btnVerCarpetaXml";
            this.btnVerCarpetaXml.Size = new System.Drawing.Size(115, 26);
            this.btnVerCarpetaXml.TabIndex = 15;
            this.btnVerCarpetaXml.Text = "📂 Ver en Carpeta";
            this.btnVerCarpetaXml.UseVisualStyleBackColor = false;
            this.btnVerCarpetaXml.Click += new System.EventHandler(this.btnVerCarpetaXml_Click);
            // 
            // btnAbrirXml
            // 
            this.btnAbrirXml.BackColor = System.Drawing.Color.White;
            this.btnAbrirXml.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnAbrirXml.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.btnAbrirXml.Location = new System.Drawing.Point(495, 137);
            this.btnAbrirXml.Name = "btnAbrirXml";
            this.btnAbrirXml.Size = new System.Drawing.Size(115, 26);
            this.btnAbrirXml.TabIndex = 14;
            this.btnAbrirXml.Text = "🌐 Abrir XML";
            this.btnAbrirXml.UseVisualStyleBackColor = false;
            this.btnAbrirXml.Click += new System.EventHandler(this.btnAbrirXml_Click);
            // 
            // txtRutaXml
            // 
            this.txtRutaXml.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.txtRutaXml.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.txtRutaXml.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.txtRutaXml.Location = new System.Drawing.Point(100, 139);
            this.txtRutaXml.Name = "txtRutaFactura";
            this.txtRutaXml.ReadOnly = true;
            this.txtRutaXml.Size = new System.Drawing.Size(255, 23);
            this.txtRutaXml.TabIndex = 13;
            // 
            // lblRutaXmlLabel
            // 
            this.lblRutaXmlLabel.AutoSize = true;
            this.lblRutaXmlLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblRutaXmlLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblRutaXmlLabel.Location = new System.Drawing.Point(15, 142);
            this.lblRutaXmlLabel.Name = "lblRutaXmlLabel";
            this.lblRutaXmlLabel.Size = new System.Drawing.Size(59, 15);
            this.lblRutaXmlLabel.TabIndex = 12;
            this.lblRutaXmlLabel.Text = "Ruta Factura:";
            // 
            // txtUuid
            // 
            this.txtUuid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.txtUuid.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.txtUuid.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.txtUuid.Location = new System.Drawing.Point(100, 107);
            this.txtUuid.Name = "txtUuid";
            this.txtUuid.ReadOnly = true;
            this.txtUuid.Size = new System.Drawing.Size(635, 23);
            this.txtUuid.TabIndex = 11;
            // 
            // lblUuidLabel
            // 
            this.lblUuidLabel.AutoSize = true;
            this.lblUuidLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblUuidLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblUuidLabel.Location = new System.Drawing.Point(15, 110);
            this.lblUuidLabel.Name = "lblUuidLabel";
            this.lblUuidLabel.Size = new System.Drawing.Size(68, 15);
            this.lblUuidLabel.TabIndex = 10;
            this.lblUuidLabel.Text = "UUID SAT:";
            // 
            // lblDiferencia
            // 
            this.lblDiferencia.AutoSize = true;
            this.lblDiferencia.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblDiferencia.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblDiferencia.Location = new System.Drawing.Point(585, 75);
            this.lblDiferencia.Name = "lblDiferencia";
            this.lblDiferencia.Size = new System.Drawing.Size(40, 17);
            this.lblDiferencia.TabIndex = 9;
            this.lblDiferencia.Text = "$0.00";
            // 
            // lblDiferenciaLabel
            // 
            this.lblDiferenciaLabel.AutoSize = true;
            this.lblDiferenciaLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblDiferenciaLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblDiferenciaLabel.Location = new System.Drawing.Point(495, 76);
            this.lblDiferenciaLabel.Name = "lblDiferenciaLabel";
            this.lblDiferenciaLabel.Size = new System.Drawing.Size(63, 15);
            this.lblDiferenciaLabel.TabIndex = 8;
            this.lblDiferenciaLabel.Text = "Diferencia:";
            // 
            // lblSubtotalXml
            // 
            this.lblSubtotalXml.AutoSize = true;
            this.lblSubtotalXml.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblSubtotalXml.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblSubtotalXml.Location = new System.Drawing.Point(340, 75);
            this.lblSubtotalXml.Name = "lblSubtotalXml";
            this.lblSubtotalXml.Size = new System.Drawing.Size(39, 17);
            this.lblSubtotalXml.TabIndex = 7;
            this.lblSubtotalXml.Text = "$0.00";
            // 
            // lblSubtotalLabel
            // 
            this.lblSubtotalLabel.AutoSize = true;
            this.lblSubtotalLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblSubtotalLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblSubtotalLabel.Location = new System.Drawing.Point(245, 76);
            this.lblSubtotalLabel.Name = "lblSubtotalLabel";
            this.lblSubtotalLabel.Size = new System.Drawing.Size(81, 15);
            this.lblSubtotalLabel.TabIndex = 6;
            this.lblSubtotalLabel.Text = "Subtotal XML:";
            // 
            // lblTotalXml
            // 
            this.lblTotalXml.AutoSize = true;
            this.lblTotalXml.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTotalXml.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(101)))), ((int)(((byte)(52)))));
            this.lblTotalXml.Location = new System.Drawing.Point(100, 75);
            this.lblTotalXml.Name = "lblTotalXml";
            this.lblTotalXml.Size = new System.Drawing.Size(45, 19);
            this.lblTotalXml.TabIndex = 5;
            this.lblTotalXml.Text = "$0.00";
            // 
            // lblTotalXmlLabel
            // 
            this.lblTotalXmlLabel.AutoSize = true;
            this.lblTotalXmlLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblTotalXmlLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblTotalXmlLabel.Location = new System.Drawing.Point(15, 76);
            this.lblTotalXmlLabel.Name = "lblTotalXmlLabel";
            this.lblTotalXmlLabel.Size = new System.Drawing.Size(61, 15);
            this.lblTotalXmlLabel.TabIndex = 4;
            this.lblTotalXmlLabel.Text = "Total XML:";
            // 
            // lblProveedor
            // 
            this.lblProveedor.AutoSize = true;
            this.lblProveedor.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblProveedor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblProveedor.Location = new System.Drawing.Point(100, 50);
            this.lblProveedor.Name = "lblProveedor";
            this.lblProveedor.Size = new System.Drawing.Size(12, 15);
            this.lblProveedor.TabIndex = 3;
            this.lblProveedor.Text = "-";
            // 
            // lblProveedorLabel
            // 
            this.lblProveedorLabel.AutoSize = true;
            this.lblProveedorLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblProveedorLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblProveedorLabel.Location = new System.Drawing.Point(15, 50);
            this.lblProveedorLabel.Name = "lblProveedorLabel";
            this.lblProveedorLabel.Size = new System.Drawing.Size(64, 15);
            this.lblProveedorLabel.TabIndex = 2;
            this.lblProveedorLabel.Text = "Proveedor:";
            // 
            // lblRfcXml
            // 
            this.lblRfcXml.AutoSize = true;
            this.lblRfcXml.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblRfcXml.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblRfcXml.Location = new System.Drawing.Point(100, 27);
            this.lblRfcXml.Name = "lblRfcXml";
            this.lblRfcXml.Size = new System.Drawing.Size(12, 15);
            this.lblRfcXml.TabIndex = 1;
            this.lblRfcXml.Text = "-";
            // 
            // lblRfcXmlLabel
            // 
            this.lblRfcXmlLabel.AutoSize = true;
            this.lblRfcXmlLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblRfcXmlLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblRfcXmlLabel.Location = new System.Drawing.Point(15, 27);
            this.lblRfcXmlLabel.Name = "lblRfcXmlLabel";
            this.lblRfcXmlLabel.Size = new System.Drawing.Size(56, 15);
            this.lblRfcXmlLabel.TabIndex = 0;
            this.lblRfcXmlLabel.Text = "RFC XML:";
            // 
            // grpPdf
            // 
            this.grpPdf.Controls.Add(this.btnEncontrarPdf);
            this.grpPdf.Controls.Add(this.btnVerCarpetaPdf);
            this.grpPdf.Controls.Add(this.btnAbrirPdf);
            this.grpPdf.Controls.Add(this.txtRutaPdf);
            this.grpPdf.Controls.Add(this.lblRutaPdfLabel);
            this.grpPdf.Controls.Add(this.lblEstadoPdf);
            this.grpPdf.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpPdf.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.grpPdf.Location = new System.Drawing.Point(16, 517);
            this.grpPdf.Name = "grpPdf";
            this.grpPdf.Size = new System.Drawing.Size(752, 115);
            this.grpPdf.TabIndex = 3;
            this.grpPdf.TabStop = false;
            this.grpPdf.Text = "📑 Comprobante PDF Asociado";
            // 
            // btnEncontrarPdf
            // 
            this.btnEncontrarPdf.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(58)))), ((int)(((byte)(138)))));
            this.btnEncontrarPdf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEncontrarPdf.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnEncontrarPdf.ForeColor = System.Drawing.Color.White;
            this.btnEncontrarPdf.Location = new System.Drawing.Point(540, 24);
            this.btnEncontrarPdf.Name = "btnEncontrarPdf";
            this.btnEncontrarPdf.Size = new System.Drawing.Size(195, 32);
            this.btnEncontrarPdf.TabIndex = 17;
            this.btnEncontrarPdf.Text = "🔎 Encontrar / Asociar PDF...";
            this.btnEncontrarPdf.UseVisualStyleBackColor = false;
            this.btnEncontrarPdf.Click += new System.EventHandler(this.btnEncontrarPdf_Click);
            // 
            // btnVerCarpetaPdf
            // 
            this.btnVerCarpetaPdf.BackColor = System.Drawing.Color.White;
            this.btnVerCarpetaPdf.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnVerCarpetaPdf.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.btnVerCarpetaPdf.Location = new System.Drawing.Point(620, 72);
            this.btnVerCarpetaPdf.Name = "btnVerCarpetaPdf";
            this.btnVerCarpetaPdf.Size = new System.Drawing.Size(115, 26);
            this.btnVerCarpetaPdf.TabIndex = 16;
            this.btnVerCarpetaPdf.Text = "📂 Ver en Carpeta";
            this.btnVerCarpetaPdf.UseVisualStyleBackColor = false;
            this.btnVerCarpetaPdf.Click += new System.EventHandler(this.btnVerCarpetaPdf_Click);
            // 
            // btnAbrirPdf
            // 
            this.btnAbrirPdf.BackColor = System.Drawing.Color.White;
            this.btnAbrirPdf.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnAbrirPdf.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.btnAbrirPdf.Location = new System.Drawing.Point(495, 72);
            this.btnAbrirPdf.Name = "btnAbrirPdf";
            this.btnAbrirPdf.Size = new System.Drawing.Size(115, 26);
            this.btnAbrirPdf.TabIndex = 15;
            this.btnAbrirPdf.Text = "📑 Abrir PDF";
            this.btnAbrirPdf.UseVisualStyleBackColor = false;
            this.btnAbrirPdf.Click += new System.EventHandler(this.btnAbrirPdf_Click);
            // 
            // txtRutaPdf
            // 
            this.txtRutaPdf.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.txtRutaPdf.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.txtRutaPdf.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.txtRutaPdf.Location = new System.Drawing.Point(100, 74);
            this.txtRutaPdf.Name = "txtRutaPdf";
            this.txtRutaPdf.ReadOnly = true;
            this.txtRutaPdf.Size = new System.Drawing.Size(380, 23);
            this.txtRutaPdf.TabIndex = 14;
            // 
            // lblRutaPdfLabel
            // 
            this.lblRutaPdfLabel.AutoSize = true;
            this.lblRutaPdfLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblRutaPdfLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblRutaPdfLabel.Location = new System.Drawing.Point(15, 77);
            this.lblRutaPdfLabel.Name = "lblRutaPdfLabel";
            this.lblRutaPdfLabel.Size = new System.Drawing.Size(57, 15);
            this.lblRutaPdfLabel.TabIndex = 13;
            this.lblRutaPdfLabel.Text = "Ruta Estado de cuenta:";
            // 
            // lblEstadoPdf
            // 
            this.lblEstadoPdf.AutoSize = true;
            this.lblEstadoPdf.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblEstadoPdf.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblEstadoPdf.Location = new System.Drawing.Point(15, 33);
            this.lblEstadoPdf.Name = "lblEstadoPdf";
            this.lblEstadoPdf.Size = new System.Drawing.Size(12, 15);
            this.lblEstadoPdf.TabIndex = 0;
            this.lblEstadoPdf.Text = "-";
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlFooter.Controls.Add(this.btnDesamarrar);
            this.pnlFooter.Controls.Add(this.btnCerrar);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 526);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(784, 55);
            this.pnlFooter.TabIndex = 4;
            // 
            // btnDesamarrar
            // 
            this.btnDesamarrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnDesamarrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDesamarrar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDesamarrar.ForeColor = System.Drawing.Color.White;
            this.btnDesamarrar.Location = new System.Drawing.Point(15, 12);
            this.btnDesamarrar.Name = "btnDesamarrar";
            this.btnDesamarrar.Size = new System.Drawing.Size(185, 32);
            this.btnDesamarrar.TabIndex = 1;
            this.btnDesamarrar.Text = "❌ Desamarrar Factura";
            this.btnDesamarrar.UseVisualStyleBackColor = false;
            this.btnDesamarrar.Click += new System.EventHandler(this.btnDesamarrar_Click);
            // 
            // btnCerrar
            // 
            this.btnCerrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.btnCerrar.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCerrar.ForeColor = System.Drawing.Color.White;
            this.btnCerrar.Location = new System.Drawing.Point(650, 12);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(118, 32);
            this.btnCerrar.TabIndex = 0;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = false;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // FormDetalleConciliacion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(784, 695);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.grpPdf);
            this.Controls.Add(this.grpXml);
            this.Controls.Add(this.grpBanco);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDetalleConciliacion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Detalle de Conciliación";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormDetalleConciliacion_KeyDown);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.grpBanco.ResumeLayout(false);
            this.grpBanco.PerformLayout();
            this.grpXml.ResumeLayout(false);
            this.grpXml.PerformLayout();
            this.grpPdf.ResumeLayout(false);
            this.grpPdf.PerformLayout();
            this.pnlFooter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.GroupBox grpBanco;
        private System.Windows.Forms.TextBox txtDescripcionBanco;
        private System.Windows.Forms.Label lblDescLabel;
        private System.Windows.Forms.Label lblMontoBanco;
        private System.Windows.Forms.Label lblMontoLabel;
        private System.Windows.Forms.Label lblRfcBanco;
        private System.Windows.Forms.Label lblRfcBancoLabel;
        private System.Windows.Forms.Label lblFechaCompra;
        private System.Windows.Forms.Label lblFechaCompraLabel;
        private System.Windows.Forms.Label lblFechaReg;
        private System.Windows.Forms.Label lblFechaRegLabel;
        private System.Windows.Forms.Label lblResumenPaquete;
        private System.Windows.Forms.DataGridView gridMovimientosVinculados;
        private System.Windows.Forms.GroupBox grpXml;
        private System.Windows.Forms.Button btnAsociarXml;
        private System.Windows.Forms.Button btnVerCarpetaXml;
        private System.Windows.Forms.Button btnAbrirXml;
        private System.Windows.Forms.TextBox txtRutaXml;
        private System.Windows.Forms.Label lblRutaXmlLabel;
        private System.Windows.Forms.TextBox txtUuid;
        private System.Windows.Forms.Label lblUuidLabel;
        private System.Windows.Forms.Label lblDiferencia;
        private System.Windows.Forms.Label lblDiferenciaLabel;
        private System.Windows.Forms.Label lblSubtotalXml;
        private System.Windows.Forms.Label lblSubtotalLabel;
        private System.Windows.Forms.Label lblTotalXml;
        private System.Windows.Forms.Label lblTotalXmlLabel;
        private System.Windows.Forms.Label lblProveedor;
        private System.Windows.Forms.Label lblProveedorLabel;
        private System.Windows.Forms.Label lblRfcXml;
        private System.Windows.Forms.Label lblRfcXmlLabel;
        private System.Windows.Forms.GroupBox grpPdf;
        private System.Windows.Forms.Button btnEncontrarPdf;
        private System.Windows.Forms.Button btnVerCarpetaPdf;
        private System.Windows.Forms.Button btnAbrirPdf;
        private System.Windows.Forms.TextBox txtRutaPdf;
        private System.Windows.Forms.Label lblRutaPdfLabel;
        private System.Windows.Forms.Label lblEstadoPdf;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Button btnDesamarrar;
        private System.Windows.Forms.Button btnCerrar;
    }
}
