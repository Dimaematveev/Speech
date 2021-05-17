using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.Wave;
using NAudio.FileFormats;
using NAudio.CoreAudioApi;
using NAudio;
using Sample_NAudio;
using System.ComponentModel;
using System.IO;
using Microsoft.Win32;

namespace SpeechProject.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ListUser ListUser = new ListUser(AppDomain.CurrentDomain.BaseDirectory + "\\Sound1\\All.ttt");
        // WaveIn - поток для записи
        WaveIn waveIn;
        //Класс для записи в файл
        WaveFileWriter writer;
        //Имя файла для записи
        string outputFilename
        {
            get => AppDomain.CurrentDomain.BaseDirectory + "\\Sound1\\" + FileName.Text + ".wav";
        }
        //Чтение из файла
        WaveFileReader wave;

        public MainWindow()
        {
            InitializeComponent();
            NAudioEngine soundEngine = NAudioEngine.Instance;
        }





        //получение данных из входного буфера для распознавания
        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new EventHandler<WaveInEventArgs>(waveIn_DataAvailable), sender, e);
            }
            else
            {
                //переменная для обозначения начала распознавания речевого отрезка
                bool result = ProcessData(e);
                // Если записываемый отрезок содержит речь
                if (result == true)
                {
                    //Записываем данные из буфера в файл
                    writer.WriteData(e.Buffer, 0, e.BytesRecorded);
                }
                else
                {
                    // если речь не определена на звуковом отрезке
                }
            }
        }
        ////Получение данных из входного буфера 
        //void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        //{
        //    if (this.InvokeRequired)
        //    {
        //        this.BeginInvoke(new EventHandler<WaveInEventArgs>(waveIn_DataAvailable), sender, e);
        //    }
        //    else
        //    {
        //        //Записываем данные из буфера в файл
        //        writer.WriteData(e.Buffer, 0, e.BytesRecorded);
        //    }
        //}
        //Завершаем запись
        void StopRecording()
        {

            waveIn.StopRecording();
            MessageBox.Show("StopRecording");
        }

        //Окончание записи
        private void waveIn_RecordingStopped(object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new EventHandler(waveIn_RecordingStopped), sender, e);
            }
            else
            {
                waveIn.Dispose();
                waveIn = null;
                writer.Close();
                writer = null;
            }
        }

        // Обработчка речи - вычисляем, есть ли сама речь на звуковом отрезке
        private bool ProcessData(WaveInEventArgs e)
        {
            // Порог для вычисления наличия речи
            double porog = 0.02;
            bool result = false;
            bool Tr = false;
            double Sum2 = 0;
            int Count = e.BytesRecorded / 2;
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                double Tmp = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index + 0]);
                Tmp /= 32768.0;
                Sum2 += Tmp * Tmp;
                if (Tmp > porog)
                    Tr = true;
            }
            Sum2 /= Count;
            if (Tr || Sum2 > porog)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        //Начинаем запись - обработчик нажатия кнопки
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Start Recording");
                waveIn = new WaveIn();
                //Дефолтное устройство для записи (если оно имеется)
                //встроенный микрофон ноутбука имеет номер 0
                waveIn.DeviceNumber = 0;
                //Прикрепляем к событию DataAvailable обработчик, возникающий при наличии записываемых данных
                waveIn.DataAvailable += waveIn_DataAvailable;
                //Прикрепляем обработчик завершения записи
                waveIn.RecordingStopped += waveIn_RecordingStopped;
                //Формат wav-файла - принимает параметры - частоту дискретизации и количество каналов(здесь mono)
                waveIn.WaveFormat = new WaveFormat(8000, 1);
                //Инициализируем объект WaveFileWriter
                writer = new WaveFileWriter(outputFilename, waveIn.WaveFormat);
                //Начало записи
                waveIn.StartRecording();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Воспроизвести звук
        private void PlaySound()
        {

            wave = new WaveFileReader(outputFilename);
                

            WaveOutEvent player = new WaveOutEvent();

            player.Init(wave);
            player.Play();
            player.PlaybackStopped += Player_PlaybackStopped;

        }

        private void Player_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            ((WaveOutEvent)sender).Dispose();
            wave.Dispose();
        }


        //Прерываем запись - обработчик нажатия второй кнопки
        private void button2_Click(object sender, EventArgs e)
        {
            if (waveIn != null)
            {
                StopRecording();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PlaySound();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
             var user = ListUser.Find(GetData(filename));
            if (user == null)
            {
                MessageBox.Show("Нет такого пользователя!");
                return;
            }
            MessageBox.Show(user.Name);
        }
        private string filename;
        private void button6_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "(*.mp3)|*.mp3";
            openDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory +"Sound1\\";
            if (openDialog.ShowDialog() == true)
            {
                
                filename =  openDialog.FileName;
                FN.Text = filename.Substring(filename.LastIndexOf("\\"));
            }
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace( UserName.Text))
            {
                MessageBox.Show("Пользователь пуст!!");
                return;
            }

            string str = ListUser.AddUserData(UserName.Text, GetData(filename));
            if (str !=null)
            {
                MessageBox.Show(str);
            }
        }

        private float[] GetData(string filename)
        {
            NAudioEngine.Instance.OpenFile(filename);
            return NAudioEngine.Instance.WaveformData;
        }
        //private void button5_Click(object sender, RoutedEventArgs e)
        //{
        //    string[] allfiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "Sound1","*.mp3");
        //    string fileTXT = AppDomain.CurrentDomain.BaseDirectory + "Sound1\\All.txt";
        //    foreach (var fileMP3 in allfiles)
        //            {
        //        NAudioEngine.Instance.OpenFile(fileMP3);
        //        if (NAudioEngine.Instance.WaveformData != null)
        //        {
        //            string fn = fileTXT;
        //            using (StreamWriter sw = new StreamWriter(fn, true, System.Text.Encoding.Default))
        //            {
        //                sw.Write(fileMP3+";");
        //                foreach (var item in NAudioEngine.Instance.WaveformData)
        //                {
        //                    sw.Write(item);
        //                    sw.Write(";");
        //                }
        //                sw.WriteLine();

        //            }
        //        }
        //    }

        //openDialog.Filter = "(*.mp3, *.m4a, *.wav)|*.mp3;*.m4a;*.wav";
        //openDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
        //if (openDialog.ShowDialog() == true)
        //{
        //    NAudioEngine.Instance.OpenFile(openDialog.FileName);

        //}


        //private static void ReturnFile(string fileMP3, string fileTXT)
        //{
        //    NAudioEngine.Instance.OpenFile(fileMP3);

        //    if (NAudioEngine.Instance.WaveformData != null)
        //    {
        //        string fn = fileTXT;
        //        using (StreamWriter sw = new StreamWriter(fn, false, System.Text.Encoding.Default))
        //        {
        //            foreach (var item in NAudioEngine.Instance.WaveformData)
        //            {
        //                sw.Write(item);
        //                sw.Write(";");
        //            }
        //            sw.WriteLine();

        //        }
        //    }
        //}
    }
}
