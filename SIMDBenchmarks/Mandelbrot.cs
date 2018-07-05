using System;
using BenchmarkDotNet.Attributes;
using System.Numerics;

namespace SIMDBenchmarks
{
    [DisassemblyDiagnoser(printAsm: true, printSource: true)]
    public class Mandelbrot
    {
        private Vector2 _bitMapSize;
        const int _maxIterations = 100;
        const float imWidth = 512, imHeight = 512;
        //private int[] _imageColorsBW;
        private int _oneLine;
        const float _minX = -2.0f, _minY = -1.0f, _maxX = 1.0f, _maxY = 1.0f;

        [GlobalSetup]
        public void Setup()
        {
            _bitMapSize = new Vector2(imWidth, imHeight);
        }

        [Benchmark(Baseline = true)]
        public int[] MandelFloat()
        {
            int[] _imageColorsBW = new int[(int)(_bitMapSize.X * _bitMapSize.Y)];
            float dx = (_maxX - _minX) / _bitMapSize.X;
            float dy = (_maxY - _minY) / _bitMapSize.Y;
            float xTemp, x, y, x0, y0 = _minY;

            for (int j = 0; j < _bitMapSize.Y; j++)
            {
                x0 = _minX;
                int iter;
                for (int i = 0; i < _bitMapSize.X; i++)
                {
                    x = y = 0;
                    iter = 0;
                    do
                    {
                        xTemp = x * x - y * y + x0;
                        y = 2.0f * x * y + y0;
                        x = xTemp;
                        iter++;
                        if (x * x + y * y > 4)
                        {
                            break;
                        }
                    }
                    while (iter < _maxIterations);
                    _imageColorsBW[j * _oneLine + i] = iter;
                    x0 += dx;
                }
                y0 += dy;
            }
            return _imageColorsBW;
        }
        
        [Benchmark]
        public int[] MandelVectorFloat()
        {
            int[] _imageColorsBW = new int[(int)(_bitMapSize.X * _bitMapSize.Y)];
            Span<int> imSpan = new Span<int>(_imageColorsBW);
            float dx = (_maxX - _minX) / _bitMapSize.X;
            Vector<float> dyVec = new Vector<float>((_maxY - _minY) / _bitMapSize.Y);
            Vector<float> xTemp, x, y;
            Vector<int> iter, iterStep;
            //constant vectors
            Vector<int> maxInterVec = new Vector<int>(_maxIterations);
            Vector<float> Two = new Vector<float>(2.0f);
            Vector<float> Four = new Vector<float>(4.0f);
            //number of floats in a Vector<float> at runtime
            int slots = Vector<float>.Count;
            //Vector to hold initial positions in x Axis
            float[] x0Array = new float[slots];
            for (int m = 0; m < slots; m++)
            {
                x0Array[m] = _minX + m * dx;
            }
            //increment vector for intial x value
            Vector<float> x0Step = new Vector<float>(slots * dx);
            Vector<float> x0; 
            Vector<float> x0Curr;
            Vector<float> y0 = new Vector<float>(_minY);
            Vector<float> y0Curr = y0;
            Vector<float> squareDistance, prevSquareDistance;
            //auxiliaary vector to hold comparison result
            Vector<int> blewLimit;

            //we loop per line, then column of the final bitmap to gain a little in cache hits
            for (int j = 0; j < _bitMapSize.Y; j++)
            {
                y = Vector<float>.Zero;
                //setup the first vector on x axis
                x0 = new Vector<float>(x0Array);
                for (int i = 0; i < _bitMapSize.X; i += slots)
                {
                    //prepare for while loop
                    y0Curr = y0; // auxiliary to preserve y0
                    x0Curr = x0; //auxiliary to preserve x0
                    x = y = Vector<float>.Zero;
                    iter = Vector<int>.Zero;
                    iterStep = Vector<int>.One;
                    prevSquareDistance = Vector<float>.Zero;
                    do
                    {
                        xTemp = x * x - y * y + x0Curr;
                        y = Two * x * y + y0Curr;
                        x = xTemp;
                        iter += iterStep;
                        squareDistance = x * x + y * y;
                        blewLimit = Vector.GreaterThan(squareDistance, Four);
                        //we preserve already blown squares so comparisons below work till the end
                        prevSquareDistance = System.Numerics.Vector.ConditionalSelect(blewLimit, squareDistance, prevSquareDistance);
                        //bail out fast if all blew the limit
                        if (Vector.GreaterThanAll(prevSquareDistance, Four))
                        {
                            break;
                        }
                        else
                        {
                            //first zero out the sum vector at positions where we reached limit of interations and blew radius
                            blewLimit = Vector.LessThan(iter, maxInterVec) & Vector.LessThanOrEqual(prevSquareDistance, Four);
                            //positive comparison result will contain -1
                            iterStep = Vector.Abs(blewLimit);
                            //zero out the elements that have already passed the limits so we don´t overflow
                            x0Curr = Vector.ConditionalSelect(blewLimit, x0Curr, Vector<float>.Zero);
                            y0Curr = Vector.ConditionalSelect(blewLimit, y0Curr, Vector<float>.Zero);
                            x = Vector.ConditionalSelect(blewLimit, x, Vector<float>.Zero);
                            y = Vector.ConditionalSelect(blewLimit, y, Vector<float>.Zero);
                        }
                        //check if any left alive
                        if (Vector.EqualsAll(iterStep, Vector<int>.Zero))
                        {
                            break;
                        }
                    }
                    while (Vector.LessThanAny(iter, maxInterVec));
                    iter.CopyTo(_imageColorsBW, j * _oneLine + i);
                    //increment the starting position for this line
                    x0 += x0Step;
                }
                //go to next line
                y0 += dyVec;
            }
            return _imageColorsBW;
        }
    }
}
