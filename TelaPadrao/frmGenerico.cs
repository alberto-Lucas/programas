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
        #region Perfumaria
        #region Mover Tela
        //-----------------------------------------------------------------------------------------------------
        //Mover a tela
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void pnlTopo_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (IsMaximizado())
                    CentralizarTela(); //Volta tela ao tamanho original ao mover o form
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        #endregion

        #region Maximizar tela sem sobrepor barra de tarefas
        //-----------------------------------------------------------------------------------------------------
        //Maximizar tela sem sobrepor barra de tarefas

        Size SizeForm;

        bool IsMaximizado()
        {
            return this.Size != SizeForm;
        }
        void CentralizarTela()
        {
            Rectangle workArea = Screen.PrimaryScreen.WorkingArea;

            this.Size = SizeForm;
            // Calcula a posição central baseada na área de trabalho
            int posX = workArea.X + (workArea.Width - this.Width) / 2;
            int posY = workArea.Y + (workArea.Height - this.Height) / 2;

            this.Location = new Point(posX, posY); // Aplica a posição centralizada
        }
        void MaximizarTela()
        {
            Rectangle workArea = Screen.PrimaryScreen.WorkingArea; // Obtém a área útil da tela (sem a barra de tarefas)

            if (IsMaximizado())
                CentralizarTela();
            else
            {
                this.Size = new Size(workArea.Width, workArea.Height); // Define o tamanho da janela para ocupar apenas essa área
                this.Location = new Point(workArea.X, workArea.Y); // Ajusta a posição da janela para alinhar com a área útil
            }
        }
        //-----------------------------------------------------------------------------------------------------
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


        public frmGenerico()
        {
            InitializeComponent();

            SizeForm = this.Size;
        }
    }
}
