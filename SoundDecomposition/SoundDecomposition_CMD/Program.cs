using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundDecomposition_CMD
{
    class Program
    {
        static void Main(string[] args)
        {
            var k = ReadAmplitudeValues(false);
            foreach (var item in k)
            {
                if (item !=0)
                {

                }
            }
            var file = System.IO.File.OpenRead("Я Дмитрий.wav");
            Console.ReadLine();

        }

        public static double[] ReadAmplitudeValues(bool isBigEndian)
        {
            var file = System.IO.File.OpenRead("Я Дмитрий.wav");
            int MSB, LSB; // старший и младший байты
            byte[] buffer = new byte[file.Length];//Читаем данные откуда-нибудь
            file.Read(buffer, 0, buffer.Length);
            double[] data = new double[buffer.Length / 2];

            for (int i = 0; i < data.Length; i += 2)
            {
                if (isBigEndian) // задает порядок байтов во входном сигнале
                {
                    // первым байтом будет MSB
                    MSB = buffer[2 * i];
                    // вторым байтом будет LSB
                    LSB = buffer[2 * i + 1];
                }
                else
                {
                    // наоборот
                    LSB = buffer[2 * i];
                    MSB = buffer[2 * i + 1];
                }
                // склеиваем два байта, чтобы получить 16-битное вещественное число
                // все значения делятся на максимально возможное - 2^15
                data[i] = ((MSB << 8) | LSB) / 32768;
            }

            return data;
        }
    }
}
