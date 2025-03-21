using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TelaPadrao
{
    public partial class frmGenerico : Form
    {
        Size SizeForm;
        #region Vincular UtilForm

        bool IsMaximizado()
        {
            return UtilForm.IsMaximizado(this, SizeForm);
        }

        void MaximizarTela()
        {
            UtilForm.MaximizarForm(this, SizeForm);
        }

        void VincularEventosUtilForm()
        {
            pnlTopo.MouseDown += (sender, e) => UtilForm.MoverFormCentralizar(sender, e, this, SizeForm);
        }
        #endregion

        #region Alterar Icon Botao Form
        private void ptbClose_MouseLeave(object sender, EventArgs e)
        {
            AlternarIcon(sender as PictureBox, true);
        }

        private void ptbClose_MouseMove(object sender, MouseEventArgs e)
        {
            AlternarIcon(sender as PictureBox, false);
        }

        void AlternarIcon(PictureBox ButtonForm, bool IsLeave)
        {
            if (ButtonForm.Name == "ptbClose")
                if (IsLeave)
                    ButtonForm.Image = global::TelaPadrao.Properties.Resources.icons8_Close_Window_28px;
                else
                    ButtonForm.Image = global::TelaPadrao.Properties.Resources.icons8_Close_Window_28px_1;

            if (ButtonForm.Name == "ptbMaximaze")
                if (IsMaximizado())
                    if (IsLeave)
                        ButtonForm.Image = global::TelaPadrao.Properties.Resources.icons8_Restore_Window_28px;
                    else
                        ButtonForm.Image = global::TelaPadrao.Properties.Resources.icons8_Restore_Window_28px_3;
                else
                    if (IsLeave)
                        ButtonForm.Image = global::TelaPadrao.Properties.Resources.icons8_Maximize_Window_28px;
                    else
                        ButtonForm.Image = global::TelaPadrao.Properties.Resources.icons8_Maximize_Window_28px_1;
            

            if (ButtonForm.Name == "ptbMinimaze")
                if (IsLeave)
                    ButtonForm.Image = global::TelaPadrao.Properties.Resources.icons8_Minimize_Window_28px;
                else
                    ButtonForm.Image = global::TelaPadrao.Properties.Resources.icons8_Minimize_Window_28px_1;
        }
        #endregion

        #region Botoes da Tela
        private void ptbClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ptbMaximaze_Click(object sender, EventArgs e)
        {
            MaximizarTela();
        }

        private void ptbMinimaze_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        #endregion

        #region Carregar Form
        void CarregarForm(Form Form)
        {
            this.Size = Form.Size;
            lblTituloTela.Text = Form.Text;
            ptbMaximaze.Visible = Form.MaximizeBox;
            ptbMinimaze.Visible = Form.MinimizeBox;
            this.WindowState = Form.WindowState;
            ptbIcon.Image = Form.Icon.ToBitmap();

            Form.TopLevel = false;
            Form.FormBorderStyle = FormBorderStyle.None;
            Form.Dock = DockStyle.Fill;

            pnlCentral.Controls.Clear();
            pnlCentral.Controls.Add(Form);
            Form.Show();
        }
        #endregion

        public frmGenerico(Form Form = null)
        {
            InitializeComponent();

            if (Form != null)
                CarregarForm(Form);

            SizeForm = this.Size;
            VincularEventosUtilForm();
        }
    }
}
