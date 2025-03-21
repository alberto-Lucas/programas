using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TelaPadrao
{
    public static class UtilForm
    {
        public static void FormGenericoShow(Form AForm)
        {
            frmGenerico frm = new frmGenerico(AForm);
            frm.Show();
        }

        #region Mover Tela
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public static void MoverForm(object sender, MouseEventArgs e, Form AForm)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(AForm.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        public static void MoverFormCentralizar(object sender, MouseEventArgs e, Form AForm, Size ASize)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (IsMaximizado(AForm, ASize))
                    CentralizarTela(AForm, ASize);
                ReleaseCapture();
                SendMessage(AForm.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        #endregion

        #region Centralizar Form
        static void CentralizarTela(Form AForm, Size ASize)
        {
            Rectangle workArea = Screen.PrimaryScreen.WorkingArea;

            AForm.Size = ASize;
            // Calcula a posição central baseada na área de trabalho
            int posX = workArea.X + (workArea.Width - AForm.Width) / 2;
            int posY = workArea.Y + (workArea.Height - AForm.Height) / 2;

            AForm.Location = new Point(posX, posY); // Aplica a posição centralizada
        }
        #endregion

        #region Maximizar tela sem sobrepor barra de tarefas
        public static bool IsMaximizado(Form AForm, Size ASize)
        {
            return AForm.Size != ASize;
        }

        public static void MaximizarForm(Form AForm, Size ASize)
        {
            Rectangle workArea = Screen.PrimaryScreen.WorkingArea; // Obtém a área útil da tela (sem a barra de tarefas)

            if (IsMaximizado(AForm, ASize))
                CentralizarTela(AForm, ASize);
            else
            {
                AForm.Size = new Size(workArea.Width, workArea.Height); // Define o tamanho da janela para ocupar apenas essa área
                AForm.Location = new Point(workArea.X, workArea.Y); // Ajusta a posição da janela para alinhar com a área útil
            }
        }
        #endregion
    }
}
