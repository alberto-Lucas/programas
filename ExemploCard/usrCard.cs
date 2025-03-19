using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExemploCard
{
    public partial class usrCard : UserControl
    {
        public usrCard(Card card)
        {
            InitializeComponent();
            lblPaciente.Text = card.Paciente;
            lblMedico.Text = card.Medico;
            lblStatus.Text = card.Status;
            lblDtConsulta.Text = card.DtConsulta.ToShortDateString();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
