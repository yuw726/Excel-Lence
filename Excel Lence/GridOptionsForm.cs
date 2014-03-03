using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Kurs
{
    public partial class GridOptionsForm : Form
    {
        public GridOptionsForm()
        {
            InitializeComponent();
        }

        public bool gridEnabled
        {
            set
            {
                checkBox1.Checked = value;
                groupBox1.Enabled = value;
            }
            get
            {
                return checkBox1.Checked;
            }
        }

        public int thickness
        {
            set
            {
                comboBox1.SelectedIndex = value - 1;
            }
            get
            {
                return comboBox1.SelectedIndex + 1;
            }
        }

        public Color gridColor = Color.Black;

        public Color backColor = Color.White;

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
            {
                groupBox1.Enabled = false;
            }
            else
            {
                groupBox1.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == colorDialog1.ShowDialog())
            backColor = colorDialog1.Color;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == colorDialog1.ShowDialog())
            gridColor = colorDialog1.Color;
        }
    }
}
