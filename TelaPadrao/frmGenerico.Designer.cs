namespace TelaPadrao
{
    partial class frmGenerico
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
            this.pnlTopo = new System.Windows.Forms.Panel();
            this.lblTituloTela = new System.Windows.Forms.Label();
            this.ptbIcon = new System.Windows.Forms.PictureBox();
            this.ptbMinimaze = new System.Windows.Forms.PictureBox();
            this.ptbMaximaze = new System.Windows.Forms.PictureBox();
            this.ptbClose = new System.Windows.Forms.PictureBox();
            this.pnlCentral = new System.Windows.Forms.Panel();
            this.pnlTopo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ptbIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbMinimaze)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbMaximaze)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbClose)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTopo
            // 
            this.pnlTopo.BackColor = System.Drawing.Color.CornflowerBlue;
            this.pnlTopo.Controls.Add(this.lblTituloTela);
            this.pnlTopo.Controls.Add(this.ptbIcon);
            this.pnlTopo.Controls.Add(this.ptbMinimaze);
            this.pnlTopo.Controls.Add(this.ptbMaximaze);
            this.pnlTopo.Controls.Add(this.ptbClose);
            this.pnlTopo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopo.Location = new System.Drawing.Point(0, 0);
            this.pnlTopo.Name = "pnlTopo";
            this.pnlTopo.Size = new System.Drawing.Size(553, 35);
            this.pnlTopo.TabIndex = 1;
            // 
            // lblTituloTela
            // 
            this.lblTituloTela.AutoSize = true;
            this.lblTituloTela.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTituloTela.ForeColor = System.Drawing.Color.White;
            this.lblTituloTela.Location = new System.Drawing.Point(37, 6);
            this.lblTituloTela.Name = "lblTituloTela";
            this.lblTituloTela.Size = new System.Drawing.Size(135, 25);
            this.lblTituloTela.TabIndex = 16;
            this.lblTituloTela.Text = "Tela Genérica";
            // 
            // ptbIcon
            // 
            this.ptbIcon.BackColor = System.Drawing.Color.Transparent;
            this.ptbIcon.Image = global::TelaPadrao.Properties.Resources.icons8_Form_28px;
            this.ptbIcon.Location = new System.Drawing.Point(3, 3);
            this.ptbIcon.Name = "ptbIcon";
            this.ptbIcon.Size = new System.Drawing.Size(28, 28);
            this.ptbIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ptbIcon.TabIndex = 15;
            this.ptbIcon.TabStop = false;
            // 
            // ptbMinimaze
            // 
            this.ptbMinimaze.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ptbMinimaze.BackColor = System.Drawing.Color.Transparent;
            this.ptbMinimaze.Image = global::TelaPadrao.Properties.Resources.icons8_Minimize_Window_28px;
            this.ptbMinimaze.Location = new System.Drawing.Point(463, 3);
            this.ptbMinimaze.Name = "ptbMinimaze";
            this.ptbMinimaze.Size = new System.Drawing.Size(28, 28);
            this.ptbMinimaze.TabIndex = 14;
            this.ptbMinimaze.TabStop = false;
            this.ptbMinimaze.Click += new System.EventHandler(this.ptbMinimaze_Click);
            this.ptbMinimaze.MouseLeave += new System.EventHandler(this.ptbClose_MouseLeave);
            this.ptbMinimaze.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ptbClose_MouseMove);
            // 
            // ptbMaximaze
            // 
            this.ptbMaximaze.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ptbMaximaze.BackColor = System.Drawing.Color.Transparent;
            this.ptbMaximaze.Image = global::TelaPadrao.Properties.Resources.icons8_Maximize_Window_28px;
            this.ptbMaximaze.Location = new System.Drawing.Point(492, 3);
            this.ptbMaximaze.Name = "ptbMaximaze";
            this.ptbMaximaze.Size = new System.Drawing.Size(28, 28);
            this.ptbMaximaze.TabIndex = 13;
            this.ptbMaximaze.TabStop = false;
            this.ptbMaximaze.Click += new System.EventHandler(this.ptbMaximaze_Click);
            this.ptbMaximaze.MouseLeave += new System.EventHandler(this.ptbClose_MouseLeave);
            this.ptbMaximaze.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ptbClose_MouseMove);
            // 
            // ptbClose
            // 
            this.ptbClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ptbClose.BackColor = System.Drawing.Color.Transparent;
            this.ptbClose.Image = global::TelaPadrao.Properties.Resources.icons8_Close_Window_28px;
            this.ptbClose.Location = new System.Drawing.Point(521, 3);
            this.ptbClose.Name = "ptbClose";
            this.ptbClose.Size = new System.Drawing.Size(28, 28);
            this.ptbClose.TabIndex = 12;
            this.ptbClose.TabStop = false;
            this.ptbClose.Click += new System.EventHandler(this.ptbClose_Click);
            this.ptbClose.MouseLeave += new System.EventHandler(this.ptbClose_MouseLeave);
            this.ptbClose.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ptbClose_MouseMove);
            // 
            // pnlCentral
            // 
            this.pnlCentral.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCentral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCentral.Location = new System.Drawing.Point(0, 35);
            this.pnlCentral.Name = "pnlCentral";
            this.pnlCentral.Size = new System.Drawing.Size(553, 313);
            this.pnlCentral.TabIndex = 2;
            // 
            // frmGenerico
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 348);
            this.Controls.Add(this.pnlCentral);
            this.Controls.Add(this.pnlTopo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmGenerico";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmGenerico";
            this.pnlTopo.ResumeLayout(false);
            this.pnlTopo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ptbIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbMinimaze)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbMaximaze)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbClose)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTopo;
        private System.Windows.Forms.Label lblTituloTela;
        private System.Windows.Forms.PictureBox ptbIcon;
        private System.Windows.Forms.PictureBox ptbMinimaze;
        private System.Windows.Forms.PictureBox ptbMaximaze;
        private System.Windows.Forms.PictureBox ptbClose;
        private System.Windows.Forms.Panel pnlCentral;
    }
}