using System;
using BenchmarkDotNet.Attributes;
using System.Numerics;

namespace SIMDBenchmarks
{
    [DisassemblyDiagnoser(printAsm: true, printSource: true)]
    public class Floating
    {
        public static int floatSlots = Vector<float>.Count;
        public static int doubleSlots = Vector<double>.Count;
        public const int ITEMS = 1000000;
        public float[] floatArray;
        public float[] outFloatArray;
        public double[] doubleArray;
        public double[] outDoubleArray;
        public Vector<float> left, right;
        public Vector<double> leftD, righD;        

        [GlobalSetup]
        public void Setup()
        {
            Random rd = new Random(1);
            doubleArray = new double[ITEMS];
            floatArray = new float[ITEMS];
            outDoubleArray = new double[ITEMS / 2];
            outFloatArray = new float[ITEMS / 2];
            for(int i = 0; i < ITEMS; i++)
            {
                doubleArray[i] = rd.NextDouble();
                floatArray[i] = (float)doubleArray[i];
            }
        }        

        [Benchmark]
        public void SumsFloat()
        {
            Span<float> locSpan = floatArray;
            Span<float> locOutSpan = outFloatArray;
            int j = 0;
            for (int i = 0; i < floatArray.Length; i += 2)
            {
                locOutSpan[j] += locSpan[i] + locSpan[i + 1];
                j++;
            }
        }
        
        [Benchmark]
        public void MulFloat()
        {
            Span<float> locSpan = floatArray;
            Span<float> locOutSpan = outFloatArray;
            int j = 0;
            for (int i = 0; i < floatArray.Length; i += 2)
            {
                locOutSpan[j] += (float)Math.Sqrt((locSpan[i] * locSpan[i + 1]) / 2.0);
                j++;
            }
        }

       

        [Benchmark(Baseline = true)]
        public void SumsDouble()
        {
            Span<double> locSpan = doubleArray;
            Span<double> locOutSpan = outDoubleArray;
            int j = 0;
            for (int i = 0; i < doubleArray.Length; i += 2)
            {
                locOutSpan[j] = locSpan[i] + locSpan[i + 1];
                j++;
            }
        }

        [Benchmark]
        public void MulDouble()
        {
            Span<double> locSpan = doubleArray;
            Span<double> locOutSpan = outDoubleArray;
            int j = 0;
            for (int i = 0; i < doubleArray.Length; i += 2)
            {
                locOutSpan[j] = Math.Sqrt((locSpan[i] * locSpan[i + 1]) / 2.0);
                j++;
            }
        }
       
        
        [Benchmark]
        public void SumsVectorFloat()
        {
            int j = 0;
            for(int i = 0; i < floatArray.Length; i += 2 * floatSlots)
            {
                (new Vector<float>(floatArray, i) + new Vector<float>(floatArray, i + floatSlots)).CopyTo(outFloatArray, j);
                j += floatSlots;
            }
        }
        
        [Benchmark]
        public void MulVectorFloat()
        {
            Vector<float> Two = new Vector<float>(2.0f);
            int j = 0;
            for (int i = 0; i < floatArray.Length; i += 2 * floatSlots)
            {
                Vector.SquareRoot((new Vector<float>(floatArray, i) * new Vector<float>(floatArray, i + floatSlots) / Two)).CopyTo(outFloatArray, j);
                j += floatSlots;
            }
        }
       

        [Benchmark]
        public void SumsVectorDouble()
        {
            int j = 0;
            for (int i = 0; i < doubleArray.Length; i += 2 * doubleSlots)
            {
                (new Vector<double>(doubleArray, i) + new Vector<double>(doubleArray, i + doubleSlots)).CopyTo(outDoubleArray, j);
                j += doubleSlots;
            }
        }

        [Benchmark]
        public void MulVectorDouble()
        {
            Vector<double> Two = new Vector<double>(2.0);
            int j = 0;
            for (int i = 0; i < doubleArray.Length; i += 2 * doubleSlots)
            {
                Vector.SquareRoot((new Vector<double>(doubleArray, i) * new Vector<double>(doubleArray, i + doubleSlots) / Two)).CopyTo(outDoubleArray, j);
                j += doubleSlots;
            }
        }
    }
}
