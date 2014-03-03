using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.IO;

namespace Kurs
{
    public partial class GraphicsForm : Form
    {
        StyleForm newStyle;
        TypeForm newType;
        GridOptionsForm newOptions;
        LegendOptionsForm newLegend;
        DirectoryInfo programDirectory = new DirectoryInfo(".");
        static Graphics diagramm;
        static Region[] rowRegion = new System.Drawing.Region[100]; // Области, занимаемые столбцами
        Region tempRegion = new Region(); // Временный столбец, участвующий в обработке событий от пользователя
        int row_quantity = 0;     // Количество столбцов
        int data_quantity = 0;    // Количество рядов
        int row_shift = 50;       // Сдвиг (расстояние) между столбцами
        int data_shift = 20;      // Сдвиг между рядами данных
        int threeD_shift = 0;     // Сдвиг по горизонтали при построении в 3D для одного столбца
        int abs_threeD_shift = 0; // Сдвиг по горизонтали при построении в 3D для всей диаграммы
        int norm_shift = 0;       // Сдвиг для нормализованной диаграммы с накоплением
        double coef_lenght = 1;   // Коефициент уменьшения длины (по горизонтали)
        double coef_width = 1;    // Коефициент уменьшения ширины (по вертикали)
        double norm_shift_max = 1;// Максимальный сдвиг нормализации
        bool type_is_norm = false;// Тип диаграммы - нормированная
        bool grid_enabled = true; // Сетка включена
        int grid_thickness = 2;   // Толщина линий сетки
        Color grid_color = Color.Black; // Цвет линий сетки
        Color back_color = Color.White; // Фоновая заливка
        int defaultRowWidth = 50;       // Ширина столбца по умолчанию
        Region legendRegion;            // Область легенды
        Color[] defaultColors = new Color[15]; // Набор цветов столбцов по умолчанию 
        DataRow[] data_row = new DataRow[100]; // Массив рядов данных

        public GraphicsForm(int type, bool volume)
        { 
            InitializeComponent();            
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - this.Size.Width, 100);
            diagramm = this.CreateGraphics();
            if (volume == true)
            {
                row_shift = 30;
                data_shift = 40;
            }
            if (type == 2)
            {
                type_is_norm = true;
                row_shift = 0;
                data_shift = 100;
            }            

            // Задаём набор цветов столбцов по умолчанию

            Random s = new Random(4235);
            for (int l = 0; l < 15; l++)
            {
                defaultColors[l] = Color.FromArgb(s.Next(255), s.Next(255), s.Next(255));
            }

            // Чтение данных из файла

            try
            {                
                string[] cell_to_string = new string[100];
                int j = 0, k = 0;
                int i = 0, g = 0, u = 0;
                row_shift = (int)(row_shift / coef_width);

                if (File.Exists(programDirectory.FullName.ToString() + "//temp_datum.elf") == false)
                {
                    throw new FileNotFoundException();                    
                }                
                foreach (string row_data in File.ReadAllLines(programDirectory.FullName.ToString() + "//temp_datum.elf"))
                {
                    norm_shift = 0;
                    foreach (char cell_data in row_data)
                    {
                        if (cell_data != ' ')
                        {
                            cell_to_string[k] = cell_to_string[k] + cell_data;
                        }
                        else
                        {
                            while (40 + Int32.Parse(cell_to_string[k]) / coef_lenght > this.Width)
                            {
                                coef_lenght = coef_lenght + 0.25;
                            }
                            data_row[k] = new DataRow(20 + norm_shift + threeD_shift, 30 + i + g, Int32.Parse(cell_to_string[k]), defaultRowWidth, defaultColors[j], Color.FromArgb((255 + defaultColors[j].R) / 2, (255 + defaultColors[j].G) / 2, (255 + defaultColors[j].B) / 2), j, 0, Color.Black, 0);
                            //                                                       //Смещение// //--------------Длина-------// //----Ширина--// //-Осн. цвет--// //---------------------------------------Вторичный цвет заливки------------------------------------------//№ряда//Параметры контура//                        
                            if (type == 2) // Если тип диаграммы - нормированная линейная с накоплением
                            {
                                norm_shift = norm_shift + data_row[k].x1;
                                norm_shift_max = norm_shift;
                            }
                            if (volume == true) // Если диаграмма объёмная
                            {
                                threeD_shift = threeD_shift + 10;
                                abs_threeD_shift = threeD_shift;
                            }
                            ++j;
                            ++k;
                            i = i + row_shift;
                            if (j != 0) data_quantity = j;
                        }
                    }
                    if (type == 2) // Если тип диаграммы - нормированная линейная с накоплением, то приводим её к нужному масштабу сейчас
                    {
                        if (j == 0) // Если считывание не ведётся
                        {
                            break;
                        }
                        data_row[j * u].x0 = 20;
                        if ((int)(data_row[j * u].x1 * (this.Width - 40) / norm_shift_max) > 1)
                            data_row[j * u].x1 = (int)(data_row[j * u].x1 * (this.Width - 40) / norm_shift_max);
                        else data_row[j * u].x1 = 1;
                        for (int t = j * u + 1; t < k; t++)
                        {
                            data_row[t].x0 = data_row[t - 1].x0 + data_row[t - 1].x1;
                            if ((int)(data_row[t].x1 * (this.Width - 40) / norm_shift_max) > 1)
                                data_row[t].x1 = (int)(data_row[t].x1 * (this.Width - 40) / norm_shift_max);
                            else data_row[t].x1 = 1;
                        }
                        ++u;
                    }
                    j = 0;
                    threeD_shift = 0;
                    norm_shift = 0;
                    g = g + data_shift;
                }
                row_quantity = k;
                
                // Теперь приводим диаграмму к нужному масштабу

                g = 0; j = 0;
                if (type == 1) // Если тип диаграммы - обычная, линейная
                {
                    while (((row_quantity * (row_shift) + data_shift) / coef_width) > (this.Height-100))
                    {
                        coef_width = coef_width + 0.3;
                    }
                    if (data_row[0] != null)
                    {
                        if (defaultRowWidth / coef_width != 0)
                            data_row[0].y1 = (int)(defaultRowWidth / coef_width);
                        else data_row[0].y1 = 1;

                        if ((int)(data_row[0].x1 / coef_lenght) != 0)
                        {
                            data_row[0].x1 = (int)(data_row[0].x1 / coef_lenght);
                        }
                        else data_row[0].x1 = 1;

                        j = j + (int)(row_shift / coef_width);
                        for (k = 1; k < row_quantity; k++)
                        {
                            if ((int)(Int32.Parse(cell_to_string[k]) / coef_lenght) != 0)
                                data_row[k].x1 = (int)(Int32.Parse(cell_to_string[k]) / coef_lenght);
                            else data_row[k].x1 = 1;

                            if ((int)(defaultRowWidth / coef_width) != 0)
                                data_row[k].y1 = (int)(defaultRowWidth / coef_width);
                            else data_row[k].y1 = 1;

                            if (data_row[k].num_of_row == 0)
                                g = g + (int)(data_shift / coef_width);
                            data_row[k].y0 = 30 + j + g;
                            j = j + (int)(row_shift / coef_width);
                        }
                    }
                }
                if (type == 2)
                {
                    while (((row_quantity * (row_shift) + data_quantity * data_shift) / coef_width) > (this.Height - 100))
                    {
                        coef_width = coef_width + 0.3;
                    }
                    MessageBox.Show((((row_quantity * (row_shift) + data_shift) / coef_width) > (this.Height - 100)).ToString());
                    MessageBox.Show(coef_width.ToString());
                    if (data_row[0] != null)
                    {
                        if (defaultRowWidth / coef_width != 0)
                            data_row[0].y1 = (int)(defaultRowWidth / coef_width);
                        else data_row[0].y1 = 1;

                        j = j + (int)(row_shift / coef_width);
                        for (k = 1; k < row_quantity; k++)
                        {                            
                            if ((int)(defaultRowWidth / coef_width) != 0)
                                data_row[k].y1 = (int)(defaultRowWidth / coef_width);
                            else data_row[k].y1 = 1;

                            if (data_row[k].num_of_row == 0)
                                g = g + (int)(data_shift / coef_width);
                            data_row[k].y0 = 30 + j + g;
                            j = j + (int)(row_shift / coef_width);
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Файл данных отсутствует.");                    
                this.Close();
                this.Dispose();
            }
            catch
            {
                MessageBox.Show("Таблица должна содержать только цифры!");
                this.Close();
                this.Dispose();
            }
        }

        public class DataRow // Класс рядов данных (столбцов)
        {
            public int num_of_row; // Номер ряда данных
            public int x0;         // Координаты на диаграмме
            public int x1;
            public int y0;
            public int y1;
            public Color col1;     // Цвета заливки
            public Color col2;
            public int line_type;  // Тип контура
            public Color line_col; // Цвет контура
            public float line_size;  // Толщина контура

            public DataRow(int x00, int y00, int x01, int y01, Color col01, Color col02, int i, int type, Color cl, int size)
            {
                x0 = x00;
                x1 = x01;
                y0 = y00;
                y1 = y01;
                col1 = col01;
                col2 = col02;
                num_of_row = i;
                line_type = type;
                line_size = size;
                line_col = cl;
            }
            
            public void Draw() // Рисование столбца
            {
                LinearGradientBrush myBrush = new LinearGradientBrush(new Rectangle(x0, y0, x1, y1), col1, col2, LinearGradientMode.Vertical);
                diagramm.FillEllipse(myBrush, new Rectangle(x0 - y1 / 4, y0, y1 / 2, y1));
                diagramm.FillRectangle(myBrush, new Rectangle(x0, y0, x1, y1));
                myBrush = new LinearGradientBrush(new Rectangle(x0, y0, x1, y1), col1, col1, LinearGradientMode.Vertical);
                diagramm.FillEllipse(myBrush, new Rectangle(x1 + x0 - y1 / 4, y0, y1 / 2, y1));

                // Добавление столбца в область ряда данных

                GraphicsPath rowPath = new GraphicsPath();
                rowPath.AddEllipse(x0 - y1 / 4, y0, y1 / 2, y1);
                rowPath.AddEllipse(x1 + x0 - y1 / 4, y0, y1 / 2, y1);
                rowPath.AddRectangle(new Rectangle(x0, y0, x1, y1));
                if (line_type != 0)
                {
                    if (line_type == 1)
                    {
                        Pen myPen = new Pen(line_col, line_size);
                        diagramm.DrawPath(myPen, rowPath);
                    }
                    else
                    {                        
                        Pen myPen = new Pen(new HatchBrush(HatchStyle.Percent50, line_col, Color.White), line_size);
                        diagramm.DrawPath(myPen, rowPath);
                    }
                }
                if (rowRegion[num_of_row] == null)
                {
                    rowRegion[num_of_row] = new Region(rowPath);
                }
                else
                {
                    rowRegion[num_of_row].Union(rowPath);
                }
            }
        };

        private void GraphicsForm_Paint(object sender, PaintEventArgs e) // Перерисовка диаграммы
        {
            this.Activate();
            diagramm = this.CreateGraphics();
            diagramm.FillRectangle(new SolidBrush(back_color), new Rectangle(0, 0, this.Width, this.Height));
            DrawCoordLines();
            if (type_is_norm == true)
            {
                for (int i = 0; i < row_quantity; i++)
                {
                    if (data_row[i] != null)
                        data_row[i].Draw();
                }
            }
            else
            {
                for (int i = row_quantity - 1; i >= 0; i--)
                {
                    if (data_row[i] != null)
                        data_row[i].Draw();
                }
            }
            if (newLegend != null)
            {
                DrawLegend(newLegend.backColor, newLegend.lineColor, newLegend.legendFont, newLegend.lineThickness);
            }
            else
                DrawLegend(Color.White, Color.Black, SystemFonts.DefaultFont, 1);
            label1.Location = new Point(this.Width / 2, this.Height / 15);
            label2.Location = new Point(this.Width / 2, this.Height - 90);
            label3.Location = new Point(this.Width / 80, this.Height / 12);
        }
      
        private void DrawCoordLines() // Рисование координатных осей
        {
            SolidBrush myBrush = new SolidBrush(Color.Black);
            Pen myPen = new Pen(Color.Black, 2);
            myPen.EndCap = LineCap.Triangle;            
            diagramm.DrawLine(myPen, new Point(20, 10), new Point(20, this.Height - 80));
            diagramm.DrawLine(myPen, new Point(20, this.Height - 80), new Point(this.Width - 20, this.Height - 80));
            diagramm.DrawLine(myPen, new Point(20, 10), new Point(15, 25));
            diagramm.DrawLine(myPen, new Point(20, 10), new Point(25, 25));
            diagramm.DrawLine(myPen, new Point(this.Width - 20, this.Height - 80), new Point(this.Width - 35, this.Height - 85));
            diagramm.DrawLine(myPen, new Point(this.Width - 20, this.Height - 80), new Point(this.Width - 35, this.Height - 75));
            for (int i = 0; i <= 100; i++)
            {
                if (type_is_norm == false)
                {
                    diagramm.DrawLine(myPen, new Point(20 + 50 * i, this.Height - 85), new Point(20 + 50 * i, this.Height - 75));
                    diagramm.DrawString(((int)(i * 50 * coef_lenght)).ToString(), SystemFonts.DefaultFont, myBrush, 50 * i, this.Height - 65);
                }
                else
                {
                    diagramm.DrawLine(myPen, new Point(20 + (this.Width - 40) / 10 * i, this.Height - 85), new Point(20 + (this.Width - 40) / 10 * i, this.Height - 75));
                    diagramm.DrawString(((int)(i * 10)).ToString() + "%", SystemFonts.DefaultFont, myBrush, this.Width / 10 * i, this.Height - 65);
                }
            }
            if (grid_enabled == true)
            {                
                myPen = new Pen(grid_color, grid_thickness);
                for (int i = 0; i <= 100; i++)
                {
                    if (type_is_norm == false)
                    {
                        if (abs_threeD_shift != 0)
                        {
                            diagramm.DrawLine(myPen, new Point(20 + 50 * i, this.Height - 80), new Point(20 + abs_threeD_shift + 50 * i, this.Height - 80 - abs_threeD_shift));
                            diagramm.DrawLine(myPen, new Point(20 + abs_threeD_shift + 50 * i, this.Height - 80 - abs_threeD_shift), new Point(20 + abs_threeD_shift + 50 * i, 30));
                        }
                        else
                        {
                            diagramm.DrawLine(myPen, new Point(20 + 50 * i, this.Height - 75), new Point(20 + 50 * i, 30));
                        }
                    }
                    else
                    {
                        diagramm.DrawLine(myPen, new Point(20 + (this.Width - 40) / 10 * i, this.Height - 75), new Point(20 + (this.Width - 40) / 10 * i, 30));
                    }
                }
            }
        }

        private void DrawLegend(Color legendCol, Color lineCol, Font font, int lnthick) // Рисование легенды
        {
            if (data_quantity != 0)
            {
                legendRegion = new Region(new Rectangle(this.Width - 110, this.Height - 200, 100, 20 * data_quantity));
                diagramm.FillRectangle(new SolidBrush(legendCol), new Rectangle(this.Width - 110, this.Height - 200, 100, 20 * data_quantity));
                diagramm.DrawRectangle(new Pen(lineCol, lnthick), new Rectangle(this.Width - 110, this.Height - 200, 100, 20 * data_quantity));
                for (int i = 0; i < data_quantity; i++)
                {
                    diagramm.DrawRectangle(new Pen(defaultColors[i]), new Rectangle(this.Width - 100, this.Height - 200 + 20 * i, 5, 5));
                    diagramm.DrawString("Ряд № " + i, font, new SolidBrush(Color.Black), this.Width - 90, this.Height - 200 + 20 * i);
                }
            }
        }

        // ----------------------------------------------------------------
        // Проверка попадания в регион

        private void SpaceClickedCheck(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                Point mouseClickPoint = new Point(e.X, e.Y);
                for (int i = 0; i < 100; i++)
                {
                    if (rowRegion[i] != null)
                    {
                        if (rowRegion[i].IsVisible(mouseClickPoint))
                        {
                            tempRegion = rowRegion[i];
                            contextMenuStrip1.Show(this, mouseClickPoint);
                            break;
                        }
                    }                    
                    if (legendRegion.IsVisible(mouseClickPoint))
                    {
                        contextMenuStrip3.Show(this, mouseClickPoint);
                        break;
                    }
                    Region reg = new Region(new Rectangle(0, 0, this.Width, this.Height));
                    if (reg.IsVisible(mouseClickPoint))
                    {
                        contextMenuStrip2.Show(this, mouseClickPoint);                        
                    } 
                }
            }
        }

        private void GraphicsForm_SizeChanged(object sender, EventArgs e)
        {
            // Чтение данных из файла
            try
            {
                coef_lenght = 1;
                string[] cell_to_string = new string[100];
                int j = 0, k = 0;
                int i = 0, g = 0, u = 0;
                row_shift = (int)(row_shift / coef_width);
                foreach (string row_data in File.ReadAllLines(programDirectory.FullName.ToString() + "//temp_datum.elf"))
                {
                    norm_shift = 0;
                    foreach (char cell_data in row_data)
                    {
                        if (cell_data != ' ')
                        {
                            cell_to_string[k] = cell_to_string[k] + cell_data;
                        }
                        else
                        {
                            while (40 + Int32.Parse(cell_to_string[k]) / coef_lenght > this.Width)
                            {
                                coef_lenght = coef_lenght + 0.25;
                            }
                            data_row[k] = new DataRow(20 + norm_shift + threeD_shift, 30 + i + g, Int32.Parse(cell_to_string[k]), defaultRowWidth, defaultColors[j], Color.FromArgb((255 + defaultColors[j].R) / 2, (255 + defaultColors[j].G) / 2, (255 + defaultColors[j].B) / 2), j, 0, Color.Black, 0);
                            //                                                       //Смещение// //--------------Длина-------// //----Ширина--// //-Осн. цвет--// //---------------------------------------Вторичный цвет заливки------------------------------------------//№ряда//Параметры контура//                        
                            if (type_is_norm == true) // Если тип диаграммы - нормированная линейная с накоплением
                            {
                                norm_shift = norm_shift + data_row[k].x1;
                                norm_shift_max = norm_shift;
                            }
                            if (abs_threeD_shift != 0) // Если диаграмма объёмная
                            {
                                threeD_shift = threeD_shift + 10;
                                abs_threeD_shift = threeD_shift;
                            }
                            ++j;
                            ++k;
                            i = i + row_shift;
                            if (j != 0) data_quantity = j;
                        }
                    }
                    if (type_is_norm == true) // Если тип диаграммы - нормированная линейная с накоплением, то приводим её к нужному масштабу сейчас
                    {
                        if (j == 0) // Если считывание не ведётся
                        {
                            break;
                        }
                        data_row[j * u].x0 = 20;
                        if ((int)(data_row[j * u].x1 * (this.Width - 40) / norm_shift_max) > 1)
                            data_row[j * u].x1 = (int)(data_row[j * u].x1 * (this.Width - 40) / norm_shift_max);
                        else data_row[j * u].x1 = 1;
                        for (int t = j * u + 1; t < k; t++)
                        {
                            data_row[t].x0 = data_row[t - 1].x0 + data_row[t - 1].x1;
                            if ((int)(data_row[t].x1 * (this.Width - 40) / norm_shift_max) > 1)
                                data_row[t].x1 = (int)(data_row[t].x1 * (this.Width - 40) / norm_shift_max);
                            else data_row[t].x1 = 1;
                        }
                        ++u;
                    }
                    j = 0;
                    threeD_shift = 0;
                    norm_shift = 0;
                    g = g + data_shift;
                }
                row_quantity = k;                
            }
            catch { MessageBox.Show("Файл данных отсутствует."); };
            this.Refresh();
        }

        // ----------------------------------------------------------------
        // Menu Interface

        private void параметрыРядовДанныхToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int num = 0; // Номер текущего столбца
            for (int i = 0; i <= 10; i++)
            {
                if (rowRegion[i] != null)
                {
                    if (rowRegion[i] == tempRegion)
                    {
                        num = i;
                    }
                }
            }

            if (newStyle != null)
            {                
                newStyle.Close();
                newStyle.Dispose();                
            }
            newStyle = new StyleForm(data_row[num].col1, data_shift, row_shift - defaultRowWidth, data_row[num].line_type, data_row[num].line_size);            

            if (DialogResult.OK == newStyle.ShowDialog())
            {
                data_shift = newStyle.dataShift;
                if (row_shift >= defaultRowWidth)
                {
                    row_shift = defaultRowWidth + newStyle.rowShift;
                }
                else
                    if (row_shift != 0)
                    {
                        row_shift = row_shift + newStyle.rowShift;
                    }

                int j = 0, g = 0;
                j = j + (int)(row_shift / coef_width);
                for (int i = 1; i < row_quantity; i++)
                {
                    if (data_row[i].num_of_row == 0)
                        g = g + data_shift;
                    data_row[i].y0 = 30 + j + g;
                    j = j + (int)(row_shift / coef_width);
                }
                for (int i = 0; i < row_quantity; i++)
                {
                    if (data_row[i] != null)
                    {
                        if (data_row[i].num_of_row == data_row[num].num_of_row)
                        {
                            data_row[i].col1 = newStyle.primaryColumnColor;
                            data_row[i].col2 = newStyle.secondaryColumnColor;
                            data_row[i].line_type = newStyle.lineStyle;
                            data_row[i].line_size = newStyle.lineSize;
                            data_row[i].line_col = newStyle.lineColor;
                        }
                    }
                }
                this.Refresh();
            }
        }

        private void параметрыДиаграммыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (newOptions != null)
            {
                newOptions.Close();
                newOptions.Dispose();
            }
            newOptions = new GridOptionsForm();
            newOptions.thickness = grid_thickness;
            newOptions.gridEnabled = grid_enabled;
            if (DialogResult.OK == newOptions.ShowDialog())
            {
                grid_enabled = newOptions.gridEnabled;
                grid_thickness = newOptions.thickness;
                grid_color = newOptions.gridColor;
                back_color = newOptions.backColor;
                this.Refresh();
            }            
        }        

        private void выбратьТипДиаграммыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newType = new TypeForm();
            if (DialogResult.OK == newType.ShowDialog())
            {
                this.Close();
                this.Dispose();
            }
        }

        private void параметрыПодписейИЛегендыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (newLegend != null)
            {
                newLegend.Close();
                newLegend.Dispose();
            }
            newLegend = new LegendOptionsForm();
            newLegend.diagrammName = label1.Text;
            newLegend.categoryName = label3.Text;
            newLegend.numericName = label2.Text;            
            if (DialogResult.OK == newLegend.ShowDialog())
            {                                
                DrawLegend(newLegend.backColor, newLegend.lineColor, newLegend.legendFont, newLegend.lineThickness);
                if (newLegend.diagrammName != null)
                {
                    label1.Visible = true;
                    label1.Text = newLegend.diagrammName;
                }
                else
                {
                    label1.Visible = false;
                }
                if (newLegend.categoryName != null)
                {
                    label3.Visible = true;
                    label3.Text = newLegend.categoryName;
                }
                else
                {
                    label3.Visible = false;
                }
                if (newLegend.numericName != null)
                {
                    label2.Visible = true;
                    label2.Text = newLegend.numericName;
                }
                else
                {
                    label2.Visible = false;
                }
                this.Refresh();
            }
        }        
    }
}