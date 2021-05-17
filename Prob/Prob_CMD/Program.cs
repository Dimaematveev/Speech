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
            NewMethod("Sound\\Я Дмитрий (1).wav");
            Console.WriteLine();
            NewMethod("Sound\\Я Дмитрий (2).wav");
            Console.WriteLine();
            NewMethod("Sound\\Я Дмитрий (3).wav");
            Console.WriteLine();
            Console.WriteLine("End");
            Console.ReadLine();
        }

        private static void NewMethod(string name)
        {
            var reader = new WaveFileReader(name);
            var k = FUR(reader);
            Console.WriteLine(name);
            Console.WriteLine(k.Sum());

            foreach (var item in k)
            {
                if (item >= 0)
                {
                    Console.Write($" ");
                }
                Console.Write($"{item:0000.000}; ");
            }
            Console.WriteLine();
        }

        private static List<double> FUR(WaveFileReader wr)
        {
            //int K = wr.WaveFormat.AverageBytesPerSecond / 1000 * 64;
            var k = Filtr(wr, 64);
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
            //var zz = FFT.nfft(zz1);
            data.AddRange(zz.Select(y => y.Real));
            return data;
        }

        static List<byte> Filtr(WaveFileReader wr, int ms)
        {
            int K = wr.WaveFormat.AverageBytesPerSecond / 1000 * ms;
            var r1 = (int)Math.Ceiling(Math.Log10( wr.Length / K)/Math.Log10(2));
            var r2 = (int)Math.Pow(2, r1);
            byte[] ret = new byte[r2];
            var r3 = wr.Length / (r2);
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
                int k11 = coun / r;
                ret[j] = (byte)(k11);
                j++;
                if (j== r2)
                {
                    break;
                }
            } while (true);
            return ret.ToList();

        }



    }
}

