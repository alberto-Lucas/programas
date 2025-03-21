using System.Windows.Forms;

namespace TelaPadrao
{
    public partial class frmGenerico : Form
    {
        FormExtend formExtend;
        frmImage frmImage = new frmImage();

        //Método satico para q a tela seja aberta sem instanciar
        public static void FormShow(Form pForm = null, bool pIsShowDialog = false)
        {
            frmGenerico frm = new frmGenerico(pForm);
            
            if(pIsShowDialog)
                frm.ShowDialog();
            else
                frm.Show();
        }

        //Construtor
        public frmGenerico(Form Form = null)
        {
            InitializeComponent();

            if (Form != null)
                CarregarForm(Form);

            formExtend = new FormExtend(this);
            VincularEventosFormExtend();
        }

        #region Carregar Form
        void CarregarForm(Form Form)
        {
            this.Size = Form.Size;
            lblTituloTela.Text = Form.Text;

            ptbMaximaze.Enabled = Form.MaximizeBox;
            ptbMinimaze.Enabled = Form.MinimizeBox;

            if(!ptbMaximaze.Enabled && !ptbMinimaze.Enabled)
            {
                ptbMaximaze.Visible = false;
                ptbMinimaze.Visible = false;
            }

            this.WindowState = Form.WindowState;
            this.Icon = Form.Icon;
            ptbIcon.Image = Form.Icon.ToBitmap();

            Form.TopLevel = false;
            Form.FormBorderStyle = FormBorderStyle.None;
            Form.Dock = DockStyle.Fill;

            pnlCentral.Controls.Clear();
            pnlCentral.Controls.Add(Form);
            Form.Show();
        }
        #endregion

        #region Vincular FormExtend
        void VincularEventosFormExtend()
        {
            formExtend.SetMovForm(pnlTopo);

            formExtend.SetFecharForm(ptbClose);
            formExtend.SetMaximizarForm(ptbMaximaze);
            formExtend.SetMinimizarForm(ptbMinimaze);

            formExtend.SetControleImage(ptbClose, frmImage.picFormCloseFill.Image);
            formExtend.SetControleImage(ptbMaximaze, frmImage.picFormMaximizeFill.Image, frmImage.picFormMaximizeDisable.Image);
            formExtend.SetControleImage(ptbMinimaze, frmImage.picFormMinimizeFill.Image, frmImage.picFormMinimizeDisable.Image);
        }
        #endregion

    }


}
