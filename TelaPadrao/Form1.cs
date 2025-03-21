﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TelaPadrao
{
    public partial class Form1: Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void padrãoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmGenerico.FormShow();
        }

        private void produtosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmGenerico.FormShow(new frmCadProduto());
        }

        private void testeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmGenerico.FormShow(new frmTeste());
        }

        private void clientesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmGenerico.FormShow(new frmCadCliente());
        }
    }
}
