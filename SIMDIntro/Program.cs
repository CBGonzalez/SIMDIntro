using System;
using System.Buffers;
using System.Numerics;

namespace SIMDIntro
{
    class Program
    {
        

        static void Main(string[] args)
        {
            Logics();
            Scratch();
            Basics();
            Transformations();
            Console.WriteLine("\nSIMD Operations:\n");
            Comparisons();
            Console.ReadLine();
        }

        static void Logics()
        {
            float[] doubArray = new float[] { 1, 2, 3, 4, 4, 3, 2, 1, -1, -2, -3, -4, -5 };
            Vector<float> left = new Vector<float>(doubArray);
            Vector<int> Ones = Vector<int>.One;
            Vector<int> lessThan = Vector.LessThan(left, new Vector<float>(3));
            Vector<int> newOnes = Vector.ConditionalSelect(lessThan, Ones, Vector<int>.Zero);
            Ones = lessThan & Ones;
            bool allEq = Vector.EqualsAll(Ones, newOnes);
        }

        static void Scratch()
        {
            double[] doubArray = new double[] { 1, 2, 3, 4, 4, 3, 2, 1, -1, -2, -3, -4, -5 };
            float[] flArray = new float[] { 1, 2, 3, 4, 4, 3, 2, 1, -1, -2, -3, -4, -5 };
            ushort[] ushArray = new ushort[] { 1, 2, 3, 4, 5, 6, 7, 8, 8, 7, 6, 5, 4, 3, 2, 1, 0, 1, 2, 3, 4 };
            ulong[] ulARray = new ulong[] { 1, 2, 3, 4, 4, 3, 2, 1 };

            Vector<double> left = new Vector<double>(doubArray); //<1, 2, 3, 4>
            Vector<double> right = new Vector<double>(doubArray, 4); //<4, 3, 2, 1>
            Vector<float> leftFl = new Vector<float>(flArray); //<1, 2, 3, 4, 4, 3, 2, 1>
            Vector<float> rightFl = new Vector<float>(flArray, 4); //<4, 3, 2, 1, -1, -2, -3, -4>
            Vector<ushort> leftUSh = new Vector<ushort>(ushArray); //<1, 2, 3, 4, 5, 6, 7, 8, 8, 7, 6, 5, 4, 3, 2, 1>
            Vector<ushort> rightUSh = new Vector<ushort>(ushArray, 2); //<3, 4, 5, 6, 7, 8, 8, 7, 6, 5, 4, 3, 2, 1, 0, 1> 
            Vector<ulong> leftUl = new Vector<ulong>(ulARray);
            Vector<ulong> rightUl = new Vector<ulong>(ulARray, 4);
            Vector<long> lessThan = Vector.LessThan(left, right);//lessThan = <-1, -1, 0, 0>
            Vector<int> lessThanInt = Vector.LessThan(leftFl, rightFl); //lessThanInt = <-1, -1, 0, 0, 0, 0, 0, 0>
            Vector<ushort> greaterEqual = Vector.GreaterThanOrEqual(leftUSh, rightUSh); //greaterEqual = <0, 0, 0, 0, 0, 0, 0, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535, 65535>
            Vector<ulong> lessThanul = Vector.LessThan(leftUl, rightUl);// lessTahUl = <18446744073709551615, 18446744073709551615, 0, 0>

            Vector<double> select = Vector.ConditionalSelect(lessThan, left, right);// select = <1, 2, 2, 1>
            Vector<double> select2 = Vector.ConditionalSelect(lessThan, left, Vector<double>.Zero);// select2 = <1, 2, 0, 0>

            Vector<double> minOne = new Vector<double>(-1);//<-1, -1, -1, -1>
            Vector<long> convToLong = Vector.ConvertToInt64(minOne);//<-1, -1, -1, -1>

            Vector<int> convInt = Vector.ConvertToInt32(Vector<float>.One);// convInt = <1, 1, 1, 1, 1, 1, 1, 1>
            Vector<int> asInt = Vector.AsVectorInt32(Vector<float>.One); //asInt = <1065353216, 1065353216, 1065353216, 1065353216, 1065353216, 1065353216, 1065353216, 1065353216>
            Vector<byte> asBytes = Vector.AsVectorByte(Vector<float>.One);
            //asBytes = <0, 0, 80, 3f, 0, 0, 80, 3f, 0, 0, 80, 3f, 0, 0, 80, 3f, 0, 0, 80, 3f, 0, 0, 80, 3f, 0, 0, 80, 3f, 0, 0, 80, 3f>

            Vector<double> allPositives = new Vector<double>(doubArray); //<1, 2, 3, 4>
            Vector<double> someNegatives = new Vector<double>(doubArray, 6); //<2, 1, -1, -2>

            bool allAreNegative = Vector.LessThanAll(allPositives, Vector<double>.Zero);//allAreNegative = false
            bool allArePositive = Vector.GreaterThanAll(allPositives, Vector<double>.Zero);//allArePositive = true;
            bool someAreNegative = Vector.LessThanAny(someNegatives, Vector<double>.Zero);//someAreNegative = true;

            Vector<double> left1 = new Vector<double>(doubArray); //<1, 2, 3, 4>
            Vector<double> right1 = new Vector<double>(doubArray, 1); //<2, 3, 4, 5>
            if(left == right)
            {
                // every element is equal to the corresponding element
            }
            if(left != right)
            {
                //at least one element is different
            }
            Vector<byte> allFF = new Vector<byte>(0b11110000);
            Vector<byte> onesComp = Vector.OnesComplement(allFF);//onesComp = <15, 15, ...> = <0b00001111, ...>

            short[] shArray = new short[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            Vector<short> sh = new Vector<short>(shArray);
            Vector<int> low = new Vector<int>();
            Vector<int> high = new Vector<int>();

            Vector.Widen(sh, out low, out high);

            sh = Vector.Narrow(low, high);            

            double[] douArray = new double[] { 1, 3, 5, 7, 2, 4, 6, 8, 9, 11, 13, 15, 10, 12, 14, 16 };
            Vector<int> Four = new Vector<int>(4);
            Vector<double> aVector = new Vector<double>(douArray); //<1, 3, 5, 7>
            Vector<double> otherVector = new Vector<double>(douArray, 4);// <2, 4, 6, 8>

            Vector<double> resVDou = aVector * otherVector;// resVDou = <2, 12, 30, 56>
            double resDou = Vector.Dot(aVector, otherVector);// resDou = 2 + 12 + 30 + 56 = 100
            Vector<int> resVInt = Vector.SquareRoot(Four);// resVint = <2, 2, 2, 2, 2, 2, 2, 2>
            resVDou = Vector.Max(aVector, new Vector<double>(4));// resVDou = <4, 4, 5, 7>
            resVDou = Vector.Negate(resVDou);//resVDou = <-4, -4, -5, -7>
            Vector<ushort> resVUSort = Vector.Negate(new Vector<ushort>(2));// resVUShort = <65534, 65534, 65534, 65534, 65534, 65534, 65534, 65534, 65534, 65534, 65534, 65534, 65534, 65534, 65534, 65534>
            int i;
            for (i = 0; i < 1000; i += 4)
            {

            }
            for(; i < doubArray.Length; i++)
            {
                Span<byte> byteSpan;
                Memory<byte> bytememory = new Memory<byte>();
                
            }
        }

        
        static void Basics()
        {
            double[] doubArray = new double[] { 1, 2, 3, 4, 4, 3, 2, 1, -1, -2, -3, -4, -5 };
            Span<double> douSpan= new Span<double>(doubArray, 8, 4);
            Vector<double> spanduo= new Vector<double>(douSpan);
            Console.WriteLine($"Basics:");
            int bitWidth = Vector<byte>.Count * 8;
            Console.WriteLine($"\tHardware accelerated: {Vector.IsHardwareAccelerated}");
            Console.WriteLine($"\tvector size: {bitWidth} bits");
            Console.WriteLine($"\tVector<double> has {Vector<double>.Count} elements.");
            Console.WriteLine($"\tVector<float> has {Vector<float>.Count} elements.");
            Console.WriteLine($"\tVector<ushort> has {Vector<ushort>.Count} elements.");
            Console.WriteLine($"\tVector<byte> has {Vector<byte>.Count} elements.");

            
        }

        static void Comparisons()
        {
            
            double[] leftArr = new double[] { 8, 4, 2, 1 };
            double[] rightArr = new double[] { 1, 3, 5, 7 };
            double[] selectArr = new double[] { 100, 100, 100, 100 };
            Vector<double> left = new Vector<double>(leftArr);
            Vector<double> right = new Vector<double>(rightArr);
            Vector<double> select = new Vector<double>(selectArr);
            //If condition is met, the equivalent position in result will be 0xFFFFFFFF (will show as -1), otherwise zero
            Vector<long> lessThan = Vector.LessThan(left, right);
            Vector<long> greaterEqual = Vector.GreaterThanOrEqual(left, right);
            Vector<long> lesThan1 = Vector.LessThan(left, Vector<double>.One);
            Console.WriteLine($"Comparisons:");
            Console.WriteLine($"\tVector.LessThan({left.ToString()}, {right.ToString()}) = {lessThan.ToString()}");
            Console.WriteLine($"\tVector.GreaterThanOrEqual({left.ToString()}, {right.ToString()}) = {greaterEqual.ToString()}");
            Console.WriteLine($"Selection:");
            Vector<int> lessThanInt= Vector.AsVectorInt32(lessThan);
            Vector<double> condSelect = Vector.ConditionalSelect(lessThan, left, select);
            Console.WriteLine($"\tVector.ConditionalSelect(condition, {left}, {select.ToString()}) = {condSelect.ToString()}");
        }

        static void Transformations()
        {
            Console.WriteLine($"Transformations:");

            double[] doubArray = new double[] { 1, -1, 128, -128 };
            Vector<double> doubles = new Vector<double>(doubArray);
            Vector<byte> bytes = Vector.AsVectorByte(doubles);            
            byte[] byExp = new byte[Vector<byte>.Count];
            bytes.CopyTo(byExp);

            Console.WriteLine($"\tVector.AsVectorByte({doubles.ToString()}) = \n{bytes.ToString("x2")}");
            PrtArray(byExp);
            Vector<ulong> ulongs = Vector.AsVectorUInt64(doubles);
            Console.WriteLine($"\tVector.AsVectorUInt64({doubles.ToString()}) = \n{ulongs.ToString()}");
            Vector<double> after = Vector.AsVectorDouble(bytes);
            after.CopyTo(doubArray);
            PrtArray(doubArray);
        }

        static void PrtArray(Array a)
        {
            //if(a.GetType() == Type.);
            //Type ty = a.GetType();
            Console.Write("[ ");
            if (a.GetType().Name == "Byte[]")
            {
                foreach (byte val in a)
                {
                    Console.Write($"{val.ToString("x2")} ");
                }
            }
            else
            {
                foreach (var val in a)
                {
                    Console.Write($"{val} ");
                }
            }
            Console.WriteLine(" ]");
        }
    }
}
