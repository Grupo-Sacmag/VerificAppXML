namespace WindowsFormsApp1a1
{
    partial class FormInvalidos
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

        /// <summary>            bv 
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gridInvalidos = new System.Windows.Forms.DataGridView();
            this.lblListaInvalidos = new System.Windows.Forms.Label();
            this.rtbXmlCrudo = new System.Windows.Forms.RichTextBox();
            this.lblLeyendaError = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridInvalidos)).BeginInit();
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
            this.splitContainer1.Panel1.Controls.Add(this.gridInvalidos);
            this.splitContainer1.Panel1.Controls.Add(this.lblListaInvalidos);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(10);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.rtbXmlCrudo);
            this.splitContainer1.Panel2.Controls.Add(this.lblLeyendaError);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(10);
            this.splitContainer1.Size = new System.Drawing.Size(1100, 650);
            this.splitContainer1.SplitterDistance = 380;
            this.splitContainer1.TabIndex = 0;
            // 
            // gridInvalidos
            // 
            this.gridInvalidos.AllowUserToAddRows = false;
            this.gridInvalidos.AllowUserToDeleteRows = false;
            this.gridInvalidos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridInvalidos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridInvalidos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridInvalidos.Location = new System.Drawing.Point(10, 35);
            this.gridInvalidos.MultiSelect = false;
            this.gridInvalidos.Name = "gridInvalidos";
            this.gridInvalidos.ReadOnly = true;
            this.gridInvalidos.RowHeadersWidth = 30;
            this.gridInvalidos.RowTemplate.Height = 24;
            this.gridInvalidos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridInvalidos.Size = new System.Drawing.Size(360, 605);
            this.gridInvalidos.TabIndex = 1;
            this.gridInvalidos.SelectionChanged += new System.EventHandler(this.gridInvalidos_SelectionChanged);
            // 
            // lblListaInvalidos
            // 
            this.lblListaInvalidos.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblListaInvalidos.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblListaInvalidos.ForeColor = System.Drawing.Color.Red;
            this.lblListaInvalidos.Location = new System.Drawing.Point(10, 10);
            this.lblListaInvalidos.Name = "lblListaInvalidos";
            this.lblListaInvalidos.Size = new System.Drawing.Size(360, 25);
            this.lblListaInvalidos.TabIndex = 0;
            this.lblListaInvalidos.Text = "Facturas Inválidas / Inconsistentes:";
            this.lblListaInvalidos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // rtbXmlCrudo
            // 
            this.rtbXmlCrudo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbXmlCrudo.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.rtbXmlCrudo.Location = new System.Drawing.Point(10, 45);
            this.rtbXmlCrudo.Name = "rtbXmlCrudo";
            this.rtbXmlCrudo.ReadOnly = true;
            this.rtbXmlCrudo.Size = new System.Drawing.Size(696, 595);
            this.rtbXmlCrudo.TabIndex = 1;
            this.rtbXmlCrudo.Text = "";
            this.rtbXmlCrudo.WordWrap = false;
            // 
            // lblLeyendaError
            // 
            this.lblLeyendaError.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblLeyendaError.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLeyendaError.ForeColor = System.Drawing.Color.DarkRed;
            this.lblLeyendaError.Location = new System.Drawing.Point(10, 10);
            this.lblLeyendaError.Name = "lblLeyendaError";
            this.lblLeyendaError.Size = new System.Drawing.Size(696, 35);
            this.lblLeyendaError.TabIndex = 0;
            this.lblLeyendaError.Text = "En color rojo se resalta el error o inconsistencia detectada.";
            this.lblLeyendaError.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormInvalidos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 650);
            this.Controls.Add(this.splitContainer1);
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "FormInvalidos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Inspección de Facturas Inválidas y XML Crudo - Antigravity";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridInvalidos)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView gridInvalidos;
        private System.Windows.Forms.Label lblListaInvalidos;
        private System.Windows.Forms.RichTextBox rtbXmlCrudo;
        private System.Windows.Forms.Label lblLeyendaError;
    }
}
