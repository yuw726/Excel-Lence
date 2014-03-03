using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Kurs
{
    public partial class TypeForm : Form
    {
        GraphicsForm newGraphics;
        
        public TypeForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (newGraphics != null)            
            {
                newGraphics.Close();
                newGraphics.Dispose();                
            }
            try
            {
                newGraphics = new GraphicsForm(type, volume);
                newGraphics.Show();
            }
            catch { }
            this.Close();
            this.Dispose();
        }

        private int type
        {
            get
            {
                if (radioButton1.Checked == true)
                {
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
        }

        private bool volume
        {
            get
            {
                if (radioButton3.Checked == true)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == false)
            {
                groupBox2.Enabled = true;
            }
            else
            {
                groupBox2.Enabled = false;
            }
        }
    }
}