using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Kurs
{
    public partial class LegendOptionsForm : Form
    {
        public LegendOptionsForm()
        {
            InitializeComponent();
        }

        public Color backColor;

        public Color lineColor;

        public string diagrammName
        {
            set
            {
                textBox1.Text = value;
            }
            get
            {
                return textBox1.Text;
            }
        }

        public string categoryName
        {
            set
            {
                textBox2.Text = value;
            }
            get
            {
                return textBox2.Text;
            }
        }

        public string numericName
        {
            set
            {
                textBox3.Text = value;
            }
            get
            {
                return textBox3.Text;
            }
        }

        public int lineThickness
        {
            set
            {
                numericUpDown1.Value = value;
            }
            get
            {
                return (int)numericUpDown1.Value;
            }
        }

        public Font legendFont = SystemFonts.DefaultFont;

        private void button3_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == colorDialog1.ShowDialog())
            {
                lineColor = colorDialog1.Color;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == colorDialog1.ShowDialog())
            {
                backColor = colorDialog1.Color;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == fontDialog1.ShowDialog())
            {
                legendFont = fontDialog1.Font;
            }
        }
    }
}
