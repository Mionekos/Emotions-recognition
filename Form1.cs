using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using System.IO;
using System.Numerics;
using System.Data.SqlClient;
using Microsoft.VisualBasic;
using System.Threading;
namespace anicore
{
    public partial class Form1 : Form
    {
        public static string voice = "";
        public static double sum=0;
        public static double summa=0;
        public static double[] massAll = new double[20];
        public static OpenFileDialog openFileDialog = new OpenFileDialog();//экземпляр класса OpenFileDialog
        public static SoundToAmplitudes Amplitude = new SoundToAmplitudes();//экземпляр класса SoundToAmplitudes
        //создание открытого подключения
        public static string ConnectionString = @"Data Source = LAPTOP-OE4CANNK; Initial Catalog = emotions; Integrated Security = True";

        public Form1()
        {
            InitializeComponent();
        }

        //загрузка аудиофайла и дальнейшие распознавание
        private void buttonDownload_Click(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog = new OpenFileDialog();//экземпляр класса OpenFileDialog
            
            string[] args;
            Transformation converting = new Transformation();
            openFileDialog.Filter = "wav files (*.wav)|*.wav";

            if (openFileDialog.ShowDialog() == DialogResult.OK)  // Or this; I was just being thorough.
            {
                var soundFile = new FileInfo(openFileDialog.FileName);
                args = new[] { Convert.ToString(soundFile) };
                Amplitude.Sound(args);
            }

            SimpleDFT Furier = new SimpleDFT(); //экземпляр класса SimpleDFT
            //отправляем массив коммплексных чисел
            Complex[] massForFurier = Furier.FFT(converting.normalizationWoH(Amplitude.list));
            mfcc MFCC = new mfcc();//экземпляр класса
            double[] mass_mfcc = MFCC.MFCC_20_calculation(converting.transformationToMell(massForFurier));

            for (int i = 0; i < mass_mfcc.Length; i++)
            {
                sum = sum + mass_mfcc[i];
            }

            voice = String.Join(" ", mass_mfcc);

            List<double> compareEmotion = new List<double>();
            List<string> emotion = new List<string>();
            
            using (SqlConnection cn = new SqlConnection())
            {
                cn.ConnectionString = @"Data Source = LAPTOP-OE4CANNK; Initial Catalog = emotions; Integrated Security = True";
                cn.Open();
                // Создание объекта команды с помощью конструктора
                string strSQL = "Select * From mffc";
                SqlCommand myCommand = new SqlCommand(strSQL, cn);
                SqlDataReader dr = myCommand.ExecuteReader();
                while (dr.Read())
                {
                    //добавление коэффициентов из бд в список
                    emotion.Add(Convert.ToString(dr[1]));
                }
                cn.Close();
            }

            double[] sadFemale = emotion[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(num => double.Parse(num)).ToArray();
            double[] happyFemale = emotion[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(num => double.Parse(num)).ToArray();
            double[] evilFemale = emotion[2].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(num => double.Parse(num)).ToArray();
            double[] supriseFemale = emotion[3].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(num => double.Parse(num)).ToArray();

            double[] sadMale = emotion[4].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(num => double.Parse(num)).ToArray();
            double[] happyMale = emotion[5].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(num => double.Parse(num)).ToArray();
            double[] evilMale = emotion[6].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(num => double.Parse(num)).ToArray();
            double[] supriseMale = emotion[7].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(num => double.Parse(num)).ToArray();

            double koef = 0;

            for (int i = 0; i < mass_mfcc.Length; i++)
            {
                koef += (mass_mfcc[i] - happyFemale[i]);
            }

            compareEmotion.Add(koef);
            koef = 0;

            for (int i = 0; i < mass_mfcc.Length; i++)
            {
                koef += (mass_mfcc[i] - evilFemale[i]);
            }

            compareEmotion.Add(koef);
            koef = 0;

            for (int i = 0; i < mass_mfcc.Length; i++)
            {
                koef += (mass_mfcc[i] - supriseFemale[i]);
            }

            compareEmotion.Add(koef);
            koef = 0;
            
            for (int i = 0; i < mass_mfcc.Length; i++)
            {
                koef += (mass_mfcc[i] - sadMale[i]);
            }

            compareEmotion.Add(koef);
            koef = 0;
            for (int i = 0; i < mass_mfcc.Length; i++)
            {
                koef += (mass_mfcc[i] - happyMale[i]);
            }

            compareEmotion.Add(koef);
            koef = 0;
            for (int i = 0; i < mass_mfcc.Length; i++)
            {
                koef += (mass_mfcc[i] - evilMale[i]);
            }

            compareEmotion.Add(koef);
            koef = 0;

            for (int i = 0; i < mass_mfcc.Length; i++)
            {
                koef += (mass_mfcc[i] - supriseMale[i]);
            }

            compareEmotion.Add(koef);

            double minEmote = compareEmotion[0];
            //нахождение минимальной разницы
            foreach (double val in compareEmotion)
            {
                if (val < minEmote)
                {
                    minEmote = val;

                }
            }

            int caseEmote = 0;
            for (int i = 0; i < compareEmotion.Count; i++)
            {
                if (minEmote == compareEmotion[i])
                {
                    caseEmote = i;
                    break;
                }
            }

            kmeans average_k = new kmeans();
            emotionsDAL data = new emotionsDAL();
            double summa = 0;

            switch (caseEmote)
            {
                case 0:
                    textBoxNameOfEmo.Text = "Грусть. Женский";
                    Bitmap image1 = new Bitmap(@"C:\Users\Azure\Documents\Visual Studio 2017\Projects\anicore 3.0\anicore\bin\Debug\emotions\sad.bmp");
                    pictureBox2.Location = new System.Drawing.Point(213, 123);//left,top
                    pictureBox2.Size = new System.Drawing.Size(100, 100);
                    pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
                    pictureBox2.BorderStyle = BorderStyle.Fixed3D;
                    pictureBox2.Image = image1;

                    if (MessageBox.Show("Вы хотите сохранить данные?", "Сохранение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        //усредненный массив
                        double[] m = average_k.kAverage(sadFemale);

                        for (int i = 0; i < m.Length; i++)
                        {
                            summa = summa + m[i];
                        }
                        string downMass = "";
                        downMass = String.Join(" ", m);

                        //включаем бд
                        data.OpenConnection(ConnectionString);
                        //обновление усредненных mffc

                        data.updateMFFC(downMass, Convert.ToString(summa), 1);
                        //добавление усредненных mffc
                        data.insertK_average(downMass, Convert.ToString(summa), 1);
                        //закрытие бд
                        data.CloseConnection();
                    }
                    else
                    {
                        string result = Microsoft.VisualBasic.Interaction.InputBox("Введите id текущей эмоции", "Изменение", string.Empty, 100, 100);
                        //усредненный массив
                        double[] m = average_k.kAverage(sadFemale);

                        for (int i = 0; i < m.Length; i++)
                        {
                            summa = summa + m[i];
                        }
                        string downMass = "";
                        downMass = String.Join(" ", m);
                        //включаем бд
                        data.OpenConnection(ConnectionString);
                        //обновление усредненных mffc

                        data.updateMFFC(downMass, Convert.ToString(summa), Convert.ToInt32(result));
                        //добавление усредненных mffc
                        data.insertK_average(downMass, Convert.ToString(summa), Convert.ToInt32(result));
                        //закрытие бд
                        data.CloseConnection();
                    };

                    break;
                case 1:
                    textBoxNameOfEmo.Text = "Радость. Женский";
                    Bitmap image2 = new Bitmap(@"C:\Users\Azure\Documents\Visual Studio 2017\Projects\anicore 3.0\anicore\bin\Debug\emotions\happy.bmp");
                    pictureBox2.Location = new System.Drawing.Point(213, 123);//left,top
                    pictureBox2.Size = new System.Drawing.Size(100, 100);
                    pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
                    pictureBox2.BorderStyle = BorderStyle.Fixed3D;
                    pictureBox2.Image = image2;
                    if (MessageBox.Show("Вы хотите сохранить данные?", "Сохранение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        //усредненный массив
                        double[] m = average_k.kAverage(happyFemale);

                        for (int i = 0; i < m.Length; i++)
                        {
                            summa = summa + m[i];
                        }

                        string downMass = "";
                        downMass = String.Join(" ", m);

                        //включаем бд
                        data.OpenConnection(ConnectionString);
                        //обновление усредненных mffc

                        data.updateMFFC(downMass, Convert.ToString(summa), 2);
                        //добавление усредненных mffc
                        data.insertK_average(downMass, Convert.ToString(summa), 2);
                        //закрытие бд
                        data.CloseConnection();
                    }
                    else
                    {
                        string result = Microsoft.VisualBasic.Interaction.InputBox("Введите id текущей эмоции", "Изменение", string.Empty, 100, 100);
                        //усредненный массив
                        double[] m = average_k.kAverage(happyFemale);

                        for (int i = 0; i < m.Length; i++)
                        {
                            summa = summa + m[i];
                        }

                        string downMass = "";
                        downMass = String.Join(" ", m);
                        //включаем бд
                        data.OpenConnection(ConnectionString);
                        //обновление усредненных mffc

                        data.updateMFFC(downMass, Convert.ToString(summa), Convert.ToInt32(result));
                        //добавление усредненных mffc
                        data.insertK_average(downMass, Convert.ToString(summa), Convert.ToInt32(result));
                        //закрытие бд
                        data.CloseConnection();
                    };
                    break;
                case 2:
                    textBoxNameOfEmo.Text = "Злость. Женский";
                    Bitmap image3 = new Bitmap(@"C:\Users\Azure\Documents\Visual Studio 2017\Projects\anicore 3.0\anicore\bin\Debug\emotions\devil.bmp");
                    pictureBox2.Location = new System.Drawing.Point(213, 123);//left,top
                    pictureBox2.Size = new System.Drawing.Size(100, 100);
                    pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
                    pictureBox2.BorderStyle = BorderStyle.Fixed3D;
                    pictureBox2.Image = image3;

                    if (MessageBox.Show("Вы хотите сохранить данные?", "Сохранение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {

                        //усредненный массив
                        double[] m = average_k.kAverage(evilFemale);

                        for (int i = 0; i < m.Length; i++)
                        {
                            summa = summa + m[i];
                        }

                        string downMass = "";
                        downMass = String.Join(" ", m);

                        //включаем бд
                        data.OpenConnection(ConnectionString);
                        //обновление усредненных mffc

                        data.updateMFFC(downMass, Convert.ToString(summa), 3);
                        //добавление усредненных mffc
                        data.insertK_average(downMass, Convert.ToString(summa), 3);
                        //закрытие бд
                        data.CloseConnection();
                    }
                    else
                    {
                        string result = Microsoft.VisualBasic.Interaction.InputBox("Введите id текущей эмоции", "Изменение", string.Empty, 100, 100);
                        //MessageBox.Show(result);
                        //включаем бд
                        double[] m = average_k.kAverage(evilFemale);

                        for (int i = 0; i < m.Length; i++)
                        {
                            summa = summa + m[i];
                        }

                        string downMass = "";
                        downMass = String.Join(" ", m);
                        data.OpenConnection(ConnectionString);
                        //обновление усредненных mffc

                        data.updateMFFC(downMass, Convert.ToString(summa), Convert.ToInt32(result));
                        //добавление усредненных mffc
                        data.insertK_average(downMass, Convert.ToString(summa), Convert.ToInt32(result));
                        //закрытие бд
                        data.CloseConnection();
                    };
                    break;
                case 3:
                    textBoxNameOfEmo.Text = "Удивление. Женский";
                    Bitmap image4 = new Bitmap(@"C:\Users\Azure\Documents\Visual Studio 2017\Projects\anicore 3.0\anicore\bin\Debug\emotions\surprised.bmp");
                    pictureBox2.Location = new System.Drawing.Point(213, 123);//left,top
                    pictureBox2.Size = new System.Drawing.Size(100, 100);
                    pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
                    pictureBox2.BorderStyle = BorderStyle.Fixed3D;
                    pictureBox2.Image = image4;
                    if (MessageBox.Show("Вы хотите сохранить данные?", "Сохранение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        //усредненный массив
                        double[] m = average_k.kAverage(supriseFemale);

                        for (int i = 0; i < m.Length; i++)
                        {
                            summa = summa + m[i];
                        }

                        string downMass = "";
                        downMass = String.Join(" ", m);

                        //включаем бд
                        data.OpenConnection(ConnectionString);
                        //обновление усредненных mffc

                        data.updateMFFC(downMass, Convert.ToString(summa), 4);
                        //добавление усредненных mffc
                        data.insertK_average(downMass, Convert.ToString(summa), 4);
                        //закрытие бд
                        data.CloseConnection();
                    }
                    else
                    {
                        string result = Microsoft.VisualBasic.Interaction.InputBox("Введите id текущей эмоции", "Изменение", string.Empty, 100, 100);
                        //усредненный массив
                        double[] m = average_k.kAverage(supriseFemale);

                        for (int i = 0; i < m.Length; i++)
                        {
                            summa = summa + m[i];
                        }

                        string downMass = "";
                        downMass = String.Join(" ", m);
                        //включаем бд
                        data.OpenConnection(ConnectionString);
                        //обновление усредненных mffc

                        data.updateMFFC(downMass, Convert.ToString(summa), Convert.ToInt32(result));
                        //добавление усредненных mffc
                        data.insertK_average(downMass, Convert.ToString(summa), Convert.ToInt32(result));
                        //закрытие бд
                        data.CloseConnection();
                    };
                    break;
                    case 4:
                        textBoxNameOfEmo.Text = "Грусть. Мужской";
                        Bitmap image5 = new Bitmap(@"C:\Users\Azure\Documents\Visual Studio 2017\Projects\anicore 3.0\anicore\bin\Debug\emotions\sad.bmp");
                        pictureBox2.Location = new System.Drawing.Point(213, 123);//left,top
                        pictureBox2.Size = new System.Drawing.Size(100, 100);
                        pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
                        pictureBox2.BorderStyle = BorderStyle.Fixed3D;
                       // pictureBox2.Image = image5;
                        if (MessageBox.Show("Вы хотите сохранить данные?", "Сохранение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {

                            //усредненный массив
                            double[] m = average_k.kAverage(sadMale);

                            for (int i = 0; i < m.Length; i++)
                            {
                                summa = summa + m[i];
                            }

                            string downMass = "";
                            downMass = String.Join(" ", m);

                            //включаем бд
                            data.OpenConnection(ConnectionString);
                            //обновление усредненных mffc

                            data.updateMFFC(downMass, Convert.ToString(summa), 5);
                            //добавление усредненных mffc
                            data.insertK_average(downMass,Convert.ToString(summa), 5);
                            //закрытие бд
                            data.CloseConnection();
                        }
                        else
                        {
                            string result = Microsoft.VisualBasic.Interaction.InputBox("Введите id текущей эмоции", "Изменение", string.Empty, 100, 100);
                            //MessageBox.Show(result);
                            //включаем бд
                            double[] m = average_k.kAverage(sadMale );

                            for (int i = 0; i < m.Length; i++)
                            {
                                summa = summa + m[i];
                            }

                            string downMass = "";
                            downMass = String.Join(" ", m);
                            data.OpenConnection(ConnectionString);
                            //обновление усредненных mffc

                            data.updateMFFC(downMass, Convert.ToString(summa), Convert.ToInt32(result));
                            //добавление усредненных mffc
                            data.insertK_average(downMass, Convert.ToString(summa), Convert.ToInt32(result));
                            //закрытие бд
                            data.CloseConnection();
                        };
                        break;
                    case 5:
                        textBoxNameOfEmo.Text = "Радость. Мужской";
                        Bitmap image6 = new Bitmap(@"C:\Users\Azure\Documents\Visual Studio 2017\Projects\anicore 3.0\anicore\bin\Debug\emotions\happy.bmp");
                        pictureBox2.Location = new System.Drawing.Point(213, 123);//left,top
                        pictureBox2.Size = new System.Drawing.Size(100, 100);
                        pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
                        pictureBox2.BorderStyle = BorderStyle.Fixed3D;
                        pictureBox2.Image = image6;
                        if (MessageBox.Show("Вы хотите сохранить данные?", "Сохранение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {

                            //усредненный массив
                            double[] m = average_k.kAverage(happyMale);
                           
                            for (int i = 0; i < m.Length; i++)
                            {
                                summa = summa + m[i];
                            }

                            string downMass = "";
                            downMass = String.Join(" ", m);

                            //включаем бд
                            data.OpenConnection(ConnectionString);
                            //обновление усредненных mffc

                            data.updateMFFC(downMass,Convert.ToString(summa), 6);
                            //добавление усредненных mffc
                            data.insertK_average(downMass,Convert.ToString(summa), 6);
                            //закрытие бд
                            data.CloseConnection();
                        }
                        else
                        {
                            string result = Microsoft.VisualBasic.Interaction.InputBox("Введите id текущей эмоции", "Изменение", string.Empty, 100, 100);
                            //MessageBox.Show(result);
                            //включаем бд
                            double[] m = average_k.kAverage(happyMale);

                            for (int i = 0; i < m.Length; i++)
                            {
                                summa = summa + m[i];
                            }

                            string downMass = "";
                            downMass = String.Join(" ", m);
                            data.OpenConnection(ConnectionString);
                            //обновление усредненных mffc

                            data.updateMFFC(downMass, Convert.ToString(summa), Convert.ToInt32(result));
                            //добавление усредненных mffc
                            data.insertK_average(downMass, Convert.ToString(summa), Convert.ToInt32(result));
                            //закрытие бд
                            data.CloseConnection();
                        };
                        break;
                    case 6:
                        textBoxNameOfEmo.Text = "Злость. Мужской";
                        Bitmap image7 = new Bitmap(@"C:\Users\Azure\Documents\Visual Studio 2017\Projects\anicore 3.0\anicore\bin\Debug\emotions\devil.bmp");
                        pictureBox2.Location = new System.Drawing.Point(213, 123);//left,top
                        pictureBox2.Size = new System.Drawing.Size(100, 100);
                        pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
                        pictureBox2.BorderStyle = BorderStyle.Fixed3D;
                        pictureBox2.Image = image7;
                        if (MessageBox.Show("Вы хотите сохранить данные?", "Сохранение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            //усредненный массив
                            double[] m = average_k.kAverage(evilMale);
                            for (int i = 0; i < m.Length; i++)
                            {
                                summa = summa + m[i];
                            }
                            string downMass = "";
                            downMass = String.Join(" ", m);

                            //включаем бд
                            data.OpenConnection(ConnectionString);
                            //обновление усредненных mffc

                            data.updateMFFC(downMass, Convert.ToString(summa), 7);
                            //добавление усредненных mffc
                            data.insertK_average(downMass,Convert.ToString(summa), 7);
                            //закрытие бд
                            data.CloseConnection();
                        }
                        else
                        {
                            string result = Microsoft.VisualBasic.Interaction.InputBox("Введите id текущей эмоции", "Изменение", string.Empty, 100, 100);
                            //MessageBox.Show(result);
                            //включаем бд
                            double[] m = average_k.kAverage(evilMale);

                            for (int i = 0; i < m.Length; i++)
                            {
                                summa = summa + m[i];
                            }

                            string downMass = "";
                            downMass = String.Join(" ", m);
                            data.OpenConnection(ConnectionString);
                            //обновление усредненных mffc

                            data.updateMFFC(downMass, Convert.ToString(summa), Convert.ToInt32(result));
                            //добавление усредненных mffc
                            data.insertK_average(downMass, Convert.ToString(summa), Convert.ToInt32(result));
                            //закрытие бд
                            data.CloseConnection();
                        };
                    break;
                    case 7:
                        textBoxNameOfEmo.Text = "Удивление. Мужской";
                        Bitmap image8 = new Bitmap(@"C:\Users\Azure\Documents\Visual Studio 2017\Projects\anicore 3.0\anicore\bin\Debug\emotions\surprised.bmp");
                        pictureBox2.Location = new System.Drawing.Point(213, 123);//left,top
                        pictureBox2.Size = new System.Drawing.Size(100, 100);
                        pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
                        pictureBox2.BorderStyle = BorderStyle.Fixed3D;
                        pictureBox2.Image = image8;
                        if (MessageBox.Show("Вы хотите сохранить данные?", "Сохранение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            //усредненный массив
                            double[] m = average_k.kAverage(supriseMale);

                            for (int i = 0; i < m.Length; i++)
                                    {
                                        summa = summa + m[i];
                                    }
                            string downMass = "";
                            downMass = String.Join(" ", m);

                            //включаем бд
                            data.OpenConnection(ConnectionString);
                            //обновление усредненных mffc

                            data.updateMFFC(downMass, Convert.ToString(summa), 8);
                            //добавление усредненных mffc
                            data.insertK_average(downMass,Convert.ToString(summa), 8);
                            //закрытие бд
                            data.CloseConnection();
                        }
                        else
                        {
                        string result = Microsoft.VisualBasic.Interaction.InputBox("Введите id текущей эмоции", "Изменение", string.Empty, 100, 100);
                        //MessageBox.Show(result);
                        //включаем бд
                        double[] m = average_k.kAverage(supriseMale);

                        for (int i = 0; i < m.Length; i++)
                        {
                            summa = summa + m[i];
                        }

                        string downMass = "";
                        downMass = String.Join(" ", m);
                        data.OpenConnection(ConnectionString);
                        //обновление усредненных mffc

                        data.updateMFFC(downMass, Convert.ToString(summa), Convert.ToInt32(result));
                        //добавление усредненных mffc
                        data.insertK_average(downMass, Convert.ToString(summa), Convert.ToInt32(result));
                        //закрытие бд
                        data.CloseConnection();
                    };
                    break;
            }

        }

        private void buttonAddMffc_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();//экземпляр класса OpenFileDialog
            SoundToAmplitudes Amplitude = new SoundToAmplitudes();//экземпляр класса SoundToAmplitudes
            string[] args;
            Transformation converting = new Transformation();
            openFileDialog.Filter = "wav files (*.wav)|*.wav";

            int index = Convert.ToInt32(textBoxAddMffc.Text);

            if (openFileDialog.ShowDialog() == DialogResult.OK)  // Or this; I was just being thorough.
            {
                var soundFile = new FileInfo(openFileDialog.FileName);
                args = new[] { Convert.ToString(soundFile) };
                Amplitude.Sound(args);
            }

            SimpleDFT Furier = new SimpleDFT(); //экземпляр класса SimpleDFT
            //отправляем массив коммплексных чисел
            Complex[] massForFurier = Furier.FFT(converting.normalizationWoH(Amplitude.list));
            mfcc MFCC = new mfcc();//экземпляр класса
            kmeans average_k = new kmeans();
            emotionsDAL data = new emotionsDAL();
            double[] mass_mfcc = MFCC.MFCC_20_calculation(converting.transformationToMell(massForFurier));
            
            for (int i = 0; i < mass_mfcc.Length; i++)
            {
                sum = sum + mass_mfcc[i];
            }

            voice = String.Join(" ", mass_mfcc);

            double[] m = average_k.kAverage(mass_mfcc);
            for (int i = 0; i < m.Length; i++)
            {
                summa = summa + m[i];
            }
            string downMass = "";
            downMass = String.Join(" ", m);

            //включаем бд
            data.OpenConnection(ConnectionString);
            //обновление усредненных mffc

            data.insertMFFC(downMass, Convert.ToString(sum), index);
            //добавление усредненных mffc
            data.insertK_average(downMass, Convert.ToString(summa), index);
            //закрытие бд
            data.CloseConnection();

        }

        private void buttonAgain_Click(object sender, EventArgs e)
        {
            Thread myThread = new Thread((ThreadStart) delegate { Application.Restart(); });
            //очищает текст боксы
            textBoxAddMffc.Clear();
            textBoxNameOfEmo.Clear();
            //убирает картинку
            pictureBox2.SendToBack();
        }

        //Класс преобразует из байт в амплитуды
        public class SoundToAmplitudes
        {
            public List<float> list = new List<float>();//объявляем list для того, чтобы заполнить его амплитудами
            public void Sound(string[] args)
            {
                string fileName = args[0];//содержит путь к файлу
                var soundFile = new FileInfo(fileName);//инициализирует новый экземпляр класса FileInfo, 
                                                       //который выполняет роль оболочки для пути файла
                foreach (float s in AmplitudesFromFile(soundFile))
                {
                    list.Add(s);//записываем амплитуды в list
                }
            }

            public static IEnumerable<float> AmplitudesFromFile(FileInfo soundFile)//IEnumerable-перечислитель, который содержит простой перебор
            {
            
                var reader = new AudioFileReader(soundFile.FullName);//объявляется экземпляр класса AudioFileReader, который упрощает открытие аудиофайла с помощью NAudio
                                                                     //получает на вход путь к файлу
                int count = 4096; // произвольное число, максимальное количество байтов, которые могут быть считаны с текущего потока
                float[] buffer = new float[count];//объявление массива байтов
                int offset = 0;//смещение
                int numRead = 0;

                while ((numRead = reader.Read(buffer, offset, count)) > 0)//выполняет чтение байтов из reader и запись данные в заданный буфер buffer
                {
                    foreach (float amp in buffer.Take(numRead))
                    {
                        yield return amp;//для возврата каждого элемента по одному
                    }
                }
            }
        }

        public class Transformation
        {
            public Complex[] normalizationWoH(List<float> amplitudes)
            {

                List<double> AmplitudeDouble = new List<double>();//перевод из float в double
                StreamWriter sw1 = new StreamWriter("Amplitude.txt"); //сохранение амплитуд в текстовый файл

                foreach (float val in amplitudes)
                {
                    AmplitudeDouble.Add(Convert.ToDouble(val));
                    sw1.WriteLine(Convert.ToDouble(val));
                }

                double max_value = 0;//находим макисмальную амплитуду
                foreach (double val in AmplitudeDouble) { if (val > max_value) max_value = val; }

                List<double> windowOfHemming = new List<double>();
                StreamWriter sw2 = new StreamWriter("WindowOfHemming.txt");
                //нормализация и окно хемминга!!!
                int iter = 0;
                foreach (double i in AmplitudeDouble)
                {
                    double normalization = i / max_value;
                    windowOfHemming.Add((0.53836 - 0.46164 * Math.Cos(2 * Math.PI * iter / (AmplitudeDouble.Count - 1))) * normalization);
                    sw2.WriteLine((0.53836 - 0.46164 * Math.Cos(2 * Math.PI * iter / (AmplitudeDouble.Count - 1))) * normalization);
                    iter++;
                }
                //число степени двойки для fft
                double N = 2;
                double exp = 1;
                double n = 1;
                while (n < windowOfHemming.Count)
                {
                    n = Math.Pow(N, exp);
                    exp++;
                }
                if (n > windowOfHemming.Count) n = Math.Pow(N, exp - 2);

                //задаем комплексный массив
                Complex[] complexArray = new Complex[Convert.ToInt32(n)];

                //преобразования массива окна хемминга в массив комплексных чисел
                for (int j = 0; j < n; j++)
                {
                    if (j < windowOfHemming.Count)
                    {
                        Complex c1 = windowOfHemming[j];
                        complexArray[j] = c1;
                    }

                }
                return complexArray;
            }
            public List<double> transformationToMell(Complex[] fur)
            {
                List<double> mell = new List<double>();
                //StreamWriter sw3 = new StreamWriter("mell.txt");

                foreach (Complex val in fur)
                {
                    var expon = Math.Pow(val.Real, 2);//Возводим каждое число в степень двойки для дальнейшего логарифмирования
                    var m = 2585 * Math.Log10(1 + expon / 700); //перевод в меллы
                    mell.Add(m);
                    //sw3.WriteLine(m);
                }
                return mell;
            }
        }

        //Быстрое преобразование Фурье
        public class SimpleDFT
        {
            //вычисляет e^
            public Complex e(int k, int N)
            {
                if (k % N == 0) return 1;
                double arg = -2 * Math.PI * k / N;

                return new Complex(Math.Cos(arg), Math.Sin(arg));
            }

            // Метод, осуществляющий быстрое преобразование Фурье        
            public Complex[] FFT(Complex[] x)
            {
                Complex[] X;
                int N = x.Length;
                if (N == 2)
                {
                    X = new Complex[2];
                    X[0] = x[0] + x[1];
                    X[1] = x[0] - x[1];
                }
                else
                {
                    Complex[] x_even = new Complex[N / 2];
                    Complex[] x_odd = new Complex[N / 2];
                    for (int i = 0; i < N / 2; i++)
                    {
                        x_even[i] = x[2 * i];
                        x_odd[i] = x[2 * i + 1];
                    }
                    Complex[] X_even = FFT(x_even);
                    Complex[] X_odd = FFT(x_odd);
                    X = new Complex[N];
                    for (int i = 0; i < N / 2; i++)
                    {
                        X[i] = X_even[i] + e(i, N) * X_odd[i];
                        X[i + N / 2] = X_even[i] - e(i, N) * X_odd[i];
                    }
                }
                //возвращает массив с преобразование Фурье, комплекс числами
                return X;
            }
        }
        //класс, в котором вычисляются мелл-кепстральные коэффициенты
        public class mfcc
        {
            public double[] MFCC_20_calculation(List<double> wav_PCM)
            {
                int count_frames = (wav_PCM.Count * 2 / 2048) + 1; //количество отрезков в сигнале

                double[,] MFCC_mass = new double[count_frames, 20];  //массив наборов MFCC для каждого фрейма
                int[] filter_points = {6,18,31,46,63,82,103,127,154,184,218,
                              257,299,348,402,463,531,608,695,792,901,1023};//массив опорных точек для фильтрации спекрта фрейма
                double[,] H = new double[20, 1024];     //массив из 20-ти фильтров для каждого MFCC

                double[] MFCC = new double[20];     //массив MFCC для данной речевой выборки   <<<<<<<<<<<<<<<<<<<<
                                                    //***********   Расчет гребенчатых фильтров спектра:    *************
                for (int i = 0; i < 20; i++)
                    for (int j = 0; j < 1024; j++)
                    {
                        if (j < filter_points[i]) H[i, j] = 0;
                        if ((filter_points[i] <= j) & (j <= filter_points[i + 1]))
                            H[i, j] = (j - filter_points[i]) / (filter_points[i + 1] - filter_points[i]);
                        if ((filter_points[i + 1] <= j) & (j <= filter_points[i + 2]))
                            H[i, j] = (filter_points[i + 2] - j) / (filter_points[i + 2] - filter_points[i + 1]);
                        if (j > filter_points[i + 2]) H[i, j] = 0;
                    }

                for (int k = 0; k < count_frames; k++)
                {
                    //**********    Применение фильтров и логарифмирование энергии спектра для каждого фрейма   ***********
                    double[] S = new double[20];
                    for (int i = 0; i < 20; i++)
                    {
                        for (int j = 0; j < 1024; j++)
                        {
                            S[i] += Math.Pow(wav_PCM[j], 2) * H[i, j];
                        }
                        if (S[i] != 0) S[i] = Math.Log(S[i], Math.E);
                    }

                    //**********    DCT и массив MFCC для каждого фрейма на выходе     ***********
                    for (int l = 0; l < 20; l++)
                        for (int i = 0; i < 20; i++) MFCC_mass[k, l] += S[i] * Math.Cos(Math.PI * l * ((i * 0.5) / 20));
                }

                //***********   Рассчет конечных MFCC для всей речевой выборки    ***********       
                for (int i = 0; i < 20; i++)
                {
                    for (int k = 0; k < count_frames; k++) MFCC[i] += MFCC_mass[k, i];
                    MFCC[i] = MFCC[i] / count_frames;
                    //MessageBox.Show(Convert.ToString(MFCC[i]));
                }

                return MFCC;
            }
        }

        public class kmeans
        {
            public double[] kAverage(double[] voice)
            {
                List<double> setCluster = new List<double>();
                List<double> euclid = new List<double>();

                //количество кластеров
                int numClaster = 5;
                //отбор кластеров 4-х кластеров
                for (int i = 0; i < voice.Length; i++)
                {
                    if (i == 0 || i == 1 || i == 2 || i == 3) setCluster.Add((voice[i] + voice[i + 4] + voice[i + 8] + voice[i + 12] + voice[i + 16]) / numClaster);
                }
                // вычисление евклидового расстояние и нахождение 5 - го кластера
                double max = 0;
                int index = 0;
                for (int i = 0; i < setCluster.Count; i++)
                {
                    for (int j = 0; j < voice.Length; j++)
                    {
                        euclid.Add(Math.Sqrt(Math.Pow(voice[j] - setCluster[i], 2)));
                        if (Math.Sqrt(Math.Pow(voice[j] - setCluster[i], 2)) > max) { max = Math.Sqrt(Math.Pow(voice[j] - setCluster[i], 2)); index = j; }
                    }
                }
                //добавление 5-го кластера
                setCluster.Add(voice[index]);
                //сложение кластеры
                double sumCluster = 0;
                for (int i = 0; i < setCluster.Count; i++)
                {
                    sumCluster += setCluster[i];
                }

                for (int i = 0; i < setCluster.Count; i++)
                {
                    setCluster[i] = setCluster[i] / sumCluster;

                }
                Random rand = new Random();
                double p;
                p = Convert.ToDouble(rand.Next(100)) / 100;
                //суммирование евклидового расстояния
                double sumSquad = 0;
                for (int i = 0; i < euclid.Count; i++)
                {
                    sumSquad += euclid[i];

                }
                List<double> used = new List<double>();
                //накопления
                double cumulative = 0.0;
                //итерация
                int ii = 0;
                int sanity = 0;
                int newMean = -1;
                while (sanity < voice.Length * 2)
                {
                    //накопление вероятностей
                    cumulative += euclid[ii] / sumSquad;

                    if (cumulative >= p && euclid.Contains(ii) == false)
                    {
                        newMean = ii; // выбранный индекс
                        used.Add(newMean); // не выбираем повторно
                        break;
                    }
                    ++ii; // следующий кандидат
                    if (ii >= euclid.Count) ii = 0; // мимо конца
                    ++sanity;
                }

                //List<double> averaging = new List<double>();
                double[] averaging = voice;
                for (int j = 0; j < 20; j++)
                {
                    averaging[j] = averaging[j] * cumulative;
                    // MessageBox.Show(Convert.ToString(j));
                }
                return averaging;
            }

        }
        //класс, в котором подключается бд и производится обновление/добавление данных
        public class emotionsDAL
        {
            private SqlConnection connect = null;

            public void OpenConnection(string connectionString)
            {
                connect = new SqlConnection(connectionString);
                connect.Open();
            }
            public void CloseConnection()
            {
                connect.Close();
            }
            //добавление к-средних 
            public void insertK_average(string koef_mass, string koef, int emotionName_id)
            {
                string sql = string.Format("Insert Into k_average" + "(koef_mass, koef, emotionName_id) Values(@koef_mass, @koef, @emotionName_id)");
                using (SqlCommand cmd = new SqlCommand(sql, this.connect))
                {

                    //cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@koef_mass", koef_mass);
                    cmd.Parameters.AddWithValue("@koef", koef);
                    cmd.Parameters.AddWithValue("@emotionName_id", emotionName_id);
                    //ExecuteNonQuery()выполняет команду, но не возвращает вывода;
                    cmd.ExecuteNonQuery();
                }
            }
            //обновление данных о mffc после их усреднения
            public void updateMFFC(string mffc_mass, string mffc_koef, int emotionName_id)
            {
                List<string> allString = new List<string>();
                using (SqlConnection cn = new SqlConnection())
                {
                    cn.ConnectionString = @"Data Source = LAPTOP-OE4CANNK; Initial Catalog = emotions; Integrated Security = True";
                    cn.Open();
                    string strSQL = "Select * From k_average";
                    SqlCommand myCommand = new SqlCommand(strSQL, cn);
                    SqlDataReader dr = myCommand.ExecuteReader();
                    while (dr.Read())
                    {
                        if (Convert.ToInt32(dr[3]) == emotionName_id)
                        {
                            //добавление коэффициентов из бд в список
                            allString.Add(Convert.ToString(dr[1]));
                            //MessageBox.Show(Convert.ToString(dr[1]));
                        }
                    }
                    cn.Close();
                }
                List<double> sum_ave = new List<double>();
                
                Dictionary<int, double> massMffcAve = new Dictionary<int, double>();
                //MessageBox.Show(Convert.ToString(allString.Count));
                int j = 0;
               
                foreach (string val in allString)
                {
                    double[] vak= val.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(num => double.Parse(num)).ToArray();
                    if (vak != null)
                    {
                        for (int i = 0; i < vak.Length; i++)
                        {
                            
                            massAll[i] =massAll[i]+ vak[i];
                        }
                    }

                }
                //MessageBox.Show(Convert.ToString(j));
                double sumMffc = 0;
                string downMass = "";
                
                for (int i = 0; i < massAll.Length; i++)
                {
                    massAll[i]=massAll[i]/20;
                    sumMffc += massAll[i];
                }
                downMass = String.Join(" ", massAll);
                string sql = string.Format("Update mffc Set mffc_mass = '{0}', mffc_koef='{1}' Where emotionName_id='{2}'", downMass, Convert.ToString(sumMffc), emotionName_id);
                using (SqlCommand cmd = new SqlCommand(sql, this.connect))
                {
                    //просто выполняет sql-выражение и возвращает количество измененных записей. Подходит для sql-выражений INSERT, UPDATE, DELETE.
                    cmd.ExecuteNonQuery();
                }
            }

            public void insertMFFC(string voice, string sum, int id)
            {
                using (SqlConnection cn = new SqlConnection())
                {
                    cn.ConnectionString = @"Data Source = LAPTOP-OE4CANNK; Initial Catalog = emotions; Integrated Security = True";
                    try
                    {
                        //Открыть подключение
                        cn.Open();
                        SqlCommand command = new SqlCommand();
                        command.Connection = cn;
                        command.CommandText = @"INSERT INTO mffc VALUES(@mffc_mass, @mffc_koef, @emotionName_id)";
                        command.Parameters.Add("@mffc_mass", System.Data.SqlDbType.VarChar, 1000000000);
                        command.Parameters.Add("@mffc_koef", System.Data.SqlDbType.VarChar, 1000000000);
                        command.Parameters.Add("@emotionName_id", System.Data.SqlDbType.Int, 1000000000);

                        command.Parameters["@mffc_mass"].Value = voice;
                        command.Parameters["@mffc_koef"].Value = sum;
                        command.Parameters["@emotionName_id"].Value = id;
                        command.ExecuteNonQuery();
                        cn.Close();
                    }
                    catch { }
                    finally { }

                }


            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
        
