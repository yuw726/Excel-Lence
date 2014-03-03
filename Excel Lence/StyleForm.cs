using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;

namespace Kurs
{
    public partial class StyleForm : Form
    {
        Graphics preview;

        public StyleForm(Color col, int data_shift, int row_shift, int type, float size)
        {
            InitializeComponent();
            preview = panel2.CreateGraphics();            
            primaryColumnColor = col;
            dataShift = data_shift;
            rowShift = row_shift;
            lineStyle = type;
            lineSize = size;
        }        

        private void ShowPreview(int x0, int y0, int x1, int y1, Color col1, Color col2)
        {
            LinearGradientBrush myBrush = new LinearGradientBrush(new Rectangle(x0, y0, x1, y1), col1, col2, LinearGradientMode.Vertical);
            preview.FillEllipse(myBrush, new Rectangle(x0 - y1 / 4, y0, y1 / 2, y1));
            preview.FillRectangle(myBrush, new Rectangle(x0, y0, x1, y1));
            myBrush = new LinearGradientBrush(new Rectangle(x0, y0, x1, y1), col1, col1, LinearGradientMode.Vertical);
            preview.FillEllipse(myBrush, new Rectangle(x1 + x0 - y1 / 4, y0, y1 / 2, y1));

            GraphicsPath columnPath = new GraphicsPath();
            columnPath.AddEllipse(x0 - y1 / 4, y0, y1 / 2, y1);
            columnPath.AddEllipse(x1 + x0 - y1 / 4, y0, y1 / 2, y1);
            columnPath.AddRectangle(new Rectangle(x0, y0, x1, y1));
            if (lineStyle != 0)
            {
                if (lineStyle == 1)
                {
                    Pen myPen = new Pen(lineColor, 1);
                    preview.DrawPath(myPen, columnPath);
                }
                else
                {
                    Pen myPen = new Pen(new HatchBrush(HatchStyle.Percent50, lineColor, Color.White), 1);
                    preview.DrawPath(myPen, columnPath);
                }
            }
        }
        
        private void OKbutton_Click(object sender, EventArgs e)
        {           
            this.Close();
            this.Dispose();
        }

        private void Cancelbutton_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        public Color primaryColumnColor
        {
            set
            {
                trackBar1.Value = value.R;
                trackBar2.Value = value.G;
                trackBar3.Value = value.B;
            }
            get { return Color.FromArgb(trackBar1.Value, trackBar2.Value, trackBar3.Value); }            
        }

        public Color secondaryColumnColor
        {            
            get 
            {
                if (radioButton1.Checked == true)
                {
                    return Color.FromArgb(trackBar1.Value, trackBar2.Value, trackBar3.Value);
                }
                else
                {
                    int one = (255 - trackBar1.Value)/2;
                    int two = (255 - trackBar2.Value) / 2;
                    int three = (255 - trackBar3.Value)/2;
                    return Color.FromArgb(trackBar1.Value + one, trackBar2.Value + two, trackBar3.Value + three);                    
                }
            }
        }

        public int dataShift
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

        public int rowShift
        {
            set
            {
                if (value > 0)
                trackBar4.Value = value;
            }
            get
            {
                return trackBar4.Value;                
            }
        }

        public int lineStyle
        {
            set
            {
                listBox1.SelectedIndex = value;
            }
            get
            {
                return listBox1.SelectedIndex;
            }
        }

        public float lineSize = 1;

        public Color lineColor = Color.Black;

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            if (radioButton1.Checked == false)
            {
                ShowPreview(20, 10, 200, 50, primaryColumnColor, secondaryColumnColor);
            }
            else
            {
                ShowPreview(20, 10, 200, 50, primaryColumnColor, primaryColumnColor);
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lineSize = listView1.Items.IndexOf(listView1.SelectedItems[0]) + (float)0.25;            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lineColor = Color.White;
            this.Refresh();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            lineColor = Color.Silver;
            this.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            lineColor = Color.Black;
            this.Refresh();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            lineColor = Color.Red;
            this.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            lineColor = Color.Blue;
            this.Refresh();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            lineColor = Color.Green;
            this.Refresh();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            lineColor = Color.Yellow;
            this.Refresh();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            lineColor = Color.Orange;
            this.Refresh();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Refresh();
        }
        
    }
}