using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;

namespace Prob_CMD
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Begin");
            var name = "Sound\\Я Дмитрий.wav";
            var reader = new WaveFileReader(name);
            var k = FUR(reader);
            Console.WriteLine(name);
            foreach (var item in k)
            {
                
                Console.Write($"{item};");
            }
            Console.WriteLine();
            Console.WriteLine();

            name = "Sound\\Я Дмитрий1.wav";
            reader = new WaveFileReader(name);
            k = FUR(reader);
            Console.WriteLine(name);
            foreach (var item in k)
            {
                Console.Write($"{item};");
            }
            Console.WriteLine();
            Console.WriteLine();

            name = "Sound\\Я Матвеев Дмитрий Владимирович.wav";
            reader = new WaveFileReader(name);
            k = FUR(reader);
            Console.WriteLine(name);
            foreach (var item in k)
            {
                Console.Write($"{item};");
            }
            Console.WriteLine();
            Console.WriteLine("End");
            Console.ReadLine();
        }

        private static List<double> FUR(WaveFileReader wr)
        {
            //int K = wr.WaveFormat.AverageBytesPerSecond / 1000 * 64;
            var k = Filtr(wr, 128);
            var m = Math.Sqrt(k.Count);
            m = Math.Ceiling(m);
            int z = k.Count;
            for (int i = z; i < Math.Pow(2,m); i++)
            {
                k.Add(0);
            }

            List<double> data = new List<double>();
         
            var f = new FFT();

            var zz = FFT.fft(k.Select(y => new Complex(y, 0)).ToArray());
            data.AddRange(zz.Select(y => y.Real));
            return data;
        }

        static List<byte> Filtr(WaveFileReader wr, int ms)
        {
            int K = wr.WaveFormat.AverageBytesPerSecond / 1000 * ms;
            byte[] ret = new byte[wr.Length / K + 1];
            byte[] b = new byte[K];
            int r;
            int j = 0;
            do
            {
                r = wr.Read(b, 0, K);
                if (r==0)
                {
                    break;
                }
                int coun = 0;
                for (int i = 0; i < r; i++)
                {
                    coun +=b[i];
                }
                ret[j] = (byte)(coun / r);
                j++;
            } while (true);
            return ret.ToList();

        }



    }
}

