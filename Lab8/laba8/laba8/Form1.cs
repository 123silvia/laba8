using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace laba8
{
    public partial class Form1 : Form
    {
        Bitmap bmp; //исходная картинка
        Bitmap bmpCry; //картинка с внедренным в нее текстом
        public Form1()
        {
            InitializeComponent();
        }
        //обработка кнопки "загрузить картинку"
        private void button1_Click(object sender, EventArgs e)
        {  
            //устанавливаем фильтр для работы только с изображениями
            openFileDialog1.Filter = "Image files (*.BMP, *.JPG, *.GIF, *.TIF, *.PNG, *.ICO, *.EMF, *.WMF) | *.bmp; *.jpg; *.gif; *.tif; *.png; *.ico; *.emf; *.wmf";
            //если загрузка завершилась удачно
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //проверяем есть ли до загрузки исходны картинка
                if (bmp != null)
                {
                    //освобождаем bmp
                    bmp.Dispose();
                    //очищаем пикчербокс
                    pictureBox1.Image.Dispose();
                }
                //проверяем есть ли до загрузки уже обработанная картинка с внедренным в нее текстом
                if (bmpCry != null)
                {
                    //осовобождаем bmp
                    bmpCry.Dispose();
                    //очищаем пикчербокс
                    pictureBox2.Image.Dispose();
                }
                //создаем картинку
                Image image = Image.FromFile(openFileDialog1.FileName);
                //считываем высоту и ширину
                int width = image.Width;
                int height = image.Height;
                //создаем ее в формате bitmap
                bmp = new Bitmap(image, width, height);
                //загружаем в пикчербокс
                pictureBox1.Image = bmp;
            }
        }
        //обработка кнопки "внедрить в картинку"
        private void button2_Click(object sender, EventArgs e)
        {
            //проверяем чтобы текст не был пустыл
            if (textBox1.Text != "")
            {
                //получаем текст
                string txt = textBox1.Text;
                //находим размерность
                int len = Math.Min(txt.Length, 255);
                //клонируем картинку
                bmpCry = (Bitmap)bmp.Clone();
                //проверка что текст не пустой и не пустая исходная картинка
                if (len != 0 && bmp != null)
                {
                    //считываем ширину и высоту
                    int n = bmp.Height;
                    int m = bmp.Width;
                    //пробегаемся по изобаражению
                    for (int i=0;i<8; i++)
                    {
                        //получаем пиксель
                        Color p = bmp.GetPixel(i, n - 1);
                        //получаем составляющую альфа
                        int a = p.A;
                        //получаем составляющую blue
                        int b = p.B;
                        //получаем составляющую red
                        int r = p.R;
                        //получаем составляющую green
                        int g = p.G;
                        //изменять будем составляющую red
                        //внедрение бита в последний бит байта красного цвета
                        r = ((r & 254) | ((len & (1 << i)) > 0 ? 1 : 0));
                        //формируем цвет
                        p = Color.FromArgb(a, r, g, b);
                        //оустанавливаем ее пикселю
                        bmpCry.SetPixel(i, n - 1, p);
                    }
                    //теже действия, но прибавляется уже текст
                    int x = 8;
                    int y = n - 1;
                    for (int i=0; i<len; i++)
                    {
                        int c = txt[i];
                        for(int j=0; j<8; j++)
                        {
                            if (x >= m)
                            {
                                y--;
                                x = 0;
                            }
                            Color p = bmp.GetPixel(x, y);
                            int a = p.A;
                            int r = p.R;
                            int g = p.G;
                            int b = p.B;
                            r = ((r & 254) | ((c & (1 << j)) > 0 ? 1 : 0));
                            p = Color.FromArgb(a, r, g, b);
                            bmpCry.SetPixel(x, y, p);
                            x++;
                        }
                    }
                    //загружаем обработанную картинку в пикчербокс
                    pictureBox2.Image = bmpCry;
                }
            }
        }
        //кнопка "сохранить полученную картинку"
        private void button3_Click(object sender, EventArgs e)
        {
            //название
            saveFileDialog1.Title = "Сохранить ...";
            //задаем параметры как: сохранить как, подсказки
            saveFileDialog1.OverwritePrompt = true;
            saveFileDialog1.CheckPathExists = true;
            //устанавливаем фильтр
            saveFileDialog1.Filter = "Bitmap File(*.bmp)|*.bmp|" + "GIF File(*.gif)|*.gif|" + "JPEG File(*.jpg)|*.jpg|" + "TIF File(*.tif)|*.tif|" + "PNG File(*.png)|*.png";
            //если диалог для сохранения завершился успешно
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //получаем имя, по котором сохраняем картинку
                string FileName = saveFileDialog1.FileName;
                //сохраняем нашу обработанную картинку
                bmpCry.Save(FileName);
            }
        }
        //обработка кнопки "получить внедренный текст"
        private void button4_Click(object sender, EventArgs e)
        {
            string txt = "";
            int len = 0;
            bmpCry = (Bitmap)bmpCry.Clone();
            if (bmpCry != null)
            {
                int n = bmpCry.Height;
                int m = bmpCry.Width;
                for (int i=0; i<8; i++)
                {
                    Color p = bmpCry.GetPixel(i, n - 1);
                    int r = p.R;
                    len = len | ((r & 1) << i);
                }
                int x = 8;
                int y = n - 1;
                for (int i=0; i<len; i++)
                {
                    int c = 0;
                    for (int j=0; j<8; j++)
                    {
                        if (x >= m)
                        {
                            y--;
                            x = 0;
                        }
                        Color p = bmpCry.GetPixel(x, y);
                        int r = p.R;
                        c = c | ((r & 1) << j);
                        x++;
                    }
                    txt += (char)(c);
                }
                txt.Reverse();
                label4.Text = len.ToString() + " " + txt;
            }
        }
    }
}
