using System;
using BenchmarkDotNet.Attributes;
using System.Numerics;

namespace SIMDBenchmarks
{
    [DisassemblyDiagnoser(printAsm: true, printSource: true)]
    public class Integer
    {
        public static int intSlots = Vector<int>.Count;
        public static int longSlots = Vector<long>.Count;
        public const int ITEMS = 1000000;
        public int[] intArray;
        public int[] outIntArray;
        public long[] longArray;
        public long[] outLongArray;

        [GlobalSetup]
        public void Setup()
        {
            Random rd = new Random(1);
            longArray = new long[ITEMS];
            intArray = new int[ITEMS];
            outLongArray = new long[ITEMS / 2];
            outIntArray = new int[ITEMS / 2];
            for (int i = 0; i < ITEMS; i++)
            {
                intArray[i] = rd.Next(1);
                longArray[i] = intArray[i];
            }            
        }
        
        [Benchmark]
        public void SumsInt()
        {
            Span<int> locSpan = intArray;
            Span<int> locOutSpan = outIntArray;
            int j = 0;
            for (int i = 0; i < intArray.Length; i += 2)
            {
                locOutSpan[j] = locSpan[i] + locSpan[i + 1];
                j++;
            }
        }

        [Benchmark]
        public void SumsLong()
        {
            Span<long> locSpan = longArray;
            Span<long> locOutSpan = outLongArray;
            int j = 0;
            for (int i = 0; i < intArray.Length; i += 2)
            {
                locOutSpan[j] = locSpan[i] + locSpan[i + 1];
                j++;
            }
        }

        [Benchmark]
        public void MultInt()
        {
            Span<int> locSpan = intArray;
            Span<int> locOutSpan = outIntArray;
            int j = 0;
            for (int i = 0; i < intArray.Length; i += 2)
            {
                locOutSpan[j] = (int)Math.Max(locSpan[i], locSpan[i + 1]) / 2;
                j++;
            }
        }
                
        [Benchmark]
        public void MultLong()
        {
            Span<long> locSpan = longArray;
            Span<long> locOutSpan = outLongArray;
            int j = 0;
            for (int i = 0; i < intArray.Length; i += 2)
            {
                locOutSpan[j] = (locSpan[i] * locSpan[i + 1]);// / 2;
                j++;
            }
        }
        
        [Benchmark]
        public void SumsVectorInt()
        {
            int j = 0;
            for (int i = 0; i < intArray.Length; i += 2 * intSlots)
            {
                (new Vector<int>(intArray, i) + new Vector<int>(intArray, i + intSlots)).CopyTo(outIntArray, j);
                j += intSlots;
            }
        }
        
        [Benchmark]
        public void SumsVectorLong()
        {
            int j = 0;
            for (int i = 0; i < longArray.Length; i += 2 * longSlots)
            {
                (new Vector<long>(longArray, i) + new Vector<long>(longArray, i + longSlots)).CopyTo(outLongArray, j);
                j += longSlots;
            }
        }
        
        [Benchmark]
        public void MulVectorInt()
        {
            int j = 0;
            for (int i = 0; i < intArray.Length; i += 2 * intSlots)
            {
                (Vector.Max(new Vector<int>(intArray, i), new Vector<int>(intArray, i + intSlots)) / new Vector<int>(2)).CopyTo(outIntArray, j);
                j += intSlots;
            }
        }
        
        [Benchmark]
        public void MulVectorLong()
        {
            Vector<long> two = new Vector<long>(2);
            int j = 0;
            for (int i = 0; i < longArray.Length; i += 2 * longSlots)
            {
                (Vector.SquareRoot(new Vector<long>(longArray, i) * new Vector<long>(longArray, i + longSlots)) /two).CopyTo(outLongArray, j);
                j += longSlots;
            }
        }
    }
}
