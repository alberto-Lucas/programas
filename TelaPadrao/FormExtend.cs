using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TelaPadrao
{
    public class FormExtend
    {
        Form gForm;
        Size gSizeForm;

        public FormExtend(Form pForm)
        {
            gForm = pForm;
            gSizeForm = pForm.Size;
        }

        public void SetSizeForm(Size pSize)
        {
            gSizeForm = pSize;
        }

        #region Mover Form

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        #endregion

        #region Centralizar Form
        void CentralizarTela()
        {
            Rectangle workArea = Screen.PrimaryScreen.WorkingArea;

            gForm.Size = gSizeForm;
            // Calcula a posição central baseada na área de trabalho
            int posX = workArea.X + (workArea.Width - gForm.Width) / 2;
            int posY = workArea.Y + (workArea.Height - gForm.Height) / 2;

            gForm.Location = new Point(posX, posY); // Aplica a posição centralizada
        }
        #endregion

        #region Maximizar tela sem sobrepor barra de tarefas
        public bool IsMaximizado()
        {
            return gForm.Size != gSizeForm;
        }

        public void MaximizarForm()
        {
            Rectangle workArea = Screen.PrimaryScreen.WorkingArea; // Obtém a área útil da tela (sem a barra de tarefas)

            if (IsMaximizado())
                CentralizarTela();
            else
            {
                gForm.Size = new Size(workArea.Width, workArea.Height); // Define o tamanho da janela para ocupar apenas essa área
                gForm.Location = new Point(workArea.X, workArea.Y); // Ajusta a posição da janela para alinhar com a área útil
            }
        }
        #endregion

        #region Eventos

        public void MoverFormCentralizar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (IsMaximizado())
                    CentralizarTela();
                ReleaseCapture();
                SendMessage(gForm.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void MinimizarForm_Click(object sender, EventArgs e)
        {
            gForm.WindowState = FormWindowState.Minimized;
        }

        private void MaximizarForm_Click(object sender, EventArgs e)
        {
            MaximizarForm();
        }

        private void FecharForm_Click(object sender, EventArgs e)
        {
            gForm.Close();
        }

        #endregion

        #region Vincular Eventos
        public void SetMovForm(Control pComponente)
        {
            pComponente.MouseDown += MoverFormCentralizar_MouseDown;
        }

        public void SetMinimizarForm(Control pComponente)
        {
            pComponente.Click += MinimizarForm_Click;
        }

        public void SetMaximizarForm(Control pComponente)
        {
            pComponente.Click += MaximizarForm_Click;
        }

        public void SetFecharForm(Control pComponente)
        {
            pComponente.Click += FecharForm_Click;
        }

        #endregion

        #region Controle Image PictureBox
        public void SetControleImage(PictureBox pPictureBox, Image pImgTemporaria, Image pImgDesativada = null)
        {
            Image imgFixa = pPictureBox.Image;
            Image GetImage() => pPictureBox.Enabled ? imgFixa : pImgDesativada ?? imgFixa; //Funcao Local
            pPictureBox.Image = GetImage(); //Nescessario quando o campo ja inicia desativado

            pPictureBox.MouseLeave      += (s, e) => pPictureBox.Image = GetImage();
            pPictureBox.MouseMove       += (s, e) => pPictureBox.Image = pPictureBox.Enabled ? pImgTemporaria : GetImage();
            pPictureBox.EnabledChanged  += (s, e) => pPictureBox.Image = GetImage();
        }
        #endregion

    }
}
