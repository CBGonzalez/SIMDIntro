using System;
using System.Numerics;
using System.Collections.Generic;
using System.Diagnostics;

namespace BenchMarkDebug
{
    class Program
    {
        static long[] longArray;
        static long[] outLongArray;
        static int longSlots = Vector<long>.Count;
        static Stopwatch sw;

        static void Main(string[] args)
        {
            Console.WriteLine($"Hardware Accelerated: {Vector.IsHardwareAccelerated}\n");
            sw = new Stopwatch();
            sw.Start();
            Mandelbrot mn = new Mandelbrot();
            Console.WriteLine($"Calculating Mandelbrot using floats");
            long dt = sw.ElapsedMilliseconds;
            int[] floatRes = mn.CalcFloat();
            dt = sw.ElapsedMilliseconds - dt;
            Console.WriteLine($"Finished in {dt}ms");
            Console.WriteLine($"Calculating Mandelbrot using float vectors");
            dt = sw.ElapsedMilliseconds;
            int[] floatVecRes = mn.CalcVectorFloat();
            dt = sw.ElapsedMilliseconds - dt;
            Console.WriteLine($"Finished in {dt}ms");
            Console.WriteLine($"Comparing results");
            int differences = 0;
            List<int> diffLocations = new List<int>();
            if (mn.HasAllResults)
            {
                for(int i = 0; i < floatRes.Length; i++)
                {
                    if(floatRes[i] != floatVecRes[i])
                    {
                        //Console.WriteLine($"\tFound difference in {i}th element (of {floatRes.Length}): {floatRes[i]} x {floatVecRes[i]} Stopped comparing.");
                        differences++;
                        diffLocations.Add(i);
                        //break;
                    }
                }
                Console.Write($"\tFinished comparing, found {differences} differences");
                if(differences > 0)
                {
                    Console.Write($"Starting at {diffLocations[0]}: \n");
                    /*foreach(int d in diffLocations)
                    {
                        Console.Write($"{d} ");
                    }*/
                    Console.Write("\n");
                }
                else
                {
                    Console.Write("\n");
                }
            }
            Console.WriteLine($"Done.");
            //longArray = new long[1000];
            //outLongArray = new long[longArray.Length / 2];
            //MixedVectorLong();
            Console.ReadLine();
        }

        static void MixedVectorLong()
        {
            //float sum = 0.0f;
            //outFloatArray = new float[ITEMS / 2];
            //Span<int> locSpan = intArray;
            //Span<int> locOutSpan = outIntArray;
            Vector<long> two = new Vector<long>(2);
            int j = 0;
            for (int i = 0; i < longArray.Length; i += 2 * longSlots)
            {
                //outFloatArray[j] += (float) Math.Sqrt((floatArray[i] * floatArray[i + 1]) / 2.0);
                Vector<long> first = new Vector<long>(longArray, i);
                Vector<long> second = new Vector<long>(longArray, i + longSlots);
                Vector<long> res = first * second;// / two;
                res.CopyTo(outLongArray, j);
                //(Vector.SquareRoot(new Vector<long>(longArray, i) * new Vector<long>(longArray, i + longSlots)) /two).CopyTo(outLongArray, j);
                j += longSlots;
            }
        }
    }
}
