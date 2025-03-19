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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Card card = new Card();

            card.Paciente = textBox1.Text;
            card.Medico = textBox2.Text;
            card.Status = textBox3.Text;
            card.DtConsulta = dateTimePicker1.Value;

            usrCard usrCard = new usrCard(card);

            flowLayoutPanel1.Controls.Add(usrCard);
        }
    }
}
