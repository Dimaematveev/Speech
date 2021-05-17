using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.IO;
using NAudio.Wave;
using NAudio.FileFormats;
using NAudio.CoreAudioApi;
using NAudio;
using Sample_NAudio;
//using VisioForge.Shared.NAudio.Wave;

namespace SpeechProject
{
    public partial class Form1 : Form
    {
        
        // WaveIn - поток для записи
        WaveIn waveIn;
        //Класс для записи в файл
        WaveFileWriter writer;
        //Имя файла для записи
        string outputFilename = "имя_файла.wav";
        //Чтение из файла
        WaveFileReader wave;

        public Form1()
        {
            InitializeComponent();
        }

        //получение данных из входного буфера для распознавания
        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<WaveInEventArgs>(waveIn_DataAvailable), sender, e);
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
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler(waveIn_RecordingStopped), sender, e);
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
            for (int index = 0; index<e.BytesRecorded; index += 2)
            {
                double Tmp = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index + 0]);
                Tmp /= 32768.0;
                Sum2 += Tmp* Tmp;
                if (Tmp > porog)
                    Tr = true;
            }
            Sum2 /= Count;
            if (Tr || Sum2 > porog)
            { 
                result = true; 
            }else {
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



    }
}
