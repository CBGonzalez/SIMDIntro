using System.Numerics;

namespace BenchMarkDebug
{
    class Mandelbrot
    {
        private Vector2 _bitMapSize;
        private int _maxIterations;
        //private int[] _imageColorsBW;
        private int _oneLine;
        private float _minX, _minY, _maxX, _maxY;
        private bool hasFloats, hasVectors;

        public bool HasAllResults { get => hasFloats & hasVectors; }

        public Mandelbrot()
        {
            _bitMapSize = new Vector2(512, 512);
            _oneLine = (int)_bitMapSize.X;
            _maxIterations = 1000;
            //_imageColorsBW = new int[1024 * 1024];
            _minX = -2;
            _maxX = 1;
            _minY = -1;
            _maxY = 1;
            hasFloats = hasVectors = false;
        }        

        public int[] CalcFloat()
        {
            int[] _imageColorsBW = new int[(int)(_bitMapSize.X * _bitMapSize.Y)];
            float dx = (_maxX - _minX) / _bitMapSize.X;
            float dy = (_maxY - _minY) / _bitMapSize.Y;
            float xTemp, x, y, x0, y0 = _minY;

            for(int j = 0; j < _bitMapSize.Y; j++)
            {
                //y0 = _minY + j * dy;
                //y = 0;
                x0 = _minX;
                int iter;
                for(int i = 0; i < _bitMapSize.X; i++)
                {
                    /*if(i == 46)
                    {
                        string deb = "stop";
                    }*/
                    x = y = 0;
                    iter = 0;
                    do
                    {
                        xTemp = x * x - y * y + x0;
                        y = 2.0f * x * y + y0;
                        x = xTemp;
                        iter++;
                        if(x * x + y * y > 4)
                        {
                            break;
                        }
                    }
                    while (iter < _maxIterations);
                    _imageColorsBW[j * _oneLine + i] = iter;
                    x0 += dx;// _minX + (i + 1) * dx;
                }
                y0 += dy;
            }
            hasFloats = true;
            return _imageColorsBW;
        }

        public int[] CalcVectorFloat()
        {
            int[] _imageColorsBW = new int[(int)(_bitMapSize.X * _bitMapSize.Y)];
            float dx = (_maxX - _minX) / _bitMapSize.X;
            Vector<float> dyVec = new Vector<float>((_maxY - _minY) / _bitMapSize.Y);
            Vector<float> xTemp, x, y;
            //Vector<float> xBeg = new Vector<float>(_minX);
            Vector<int> iter;
            Vector<int> iterStep;
            Vector<int> maxInterVec = new Vector<int>(_maxIterations);
            Vector<float> Two = new Vector<float>(2.0f);
            Vector<float> Four = new Vector<float>(4.0f);
            int slots = Vector<float>.Count;
            //float[] tmpArray = new float[slots];
            float[] x0Array = new float[slots];
            //bool anyGreater;
            for (int m = 0; m < slots; m++)
            {
                x0Array[m] = _minX + m * dx;
                //tmpArray[m] = (m + 1) * dx;
            }
            float[] xStepArray = new float[slots];
            for(int m = 0; m < slots; m++)
            {

            }
            Vector<float> x0Step = new Vector<float>(slots * dx);
            Vector<float> dxVec = new Vector<float>(dx);
            Vector<float> x0; // = new Vector<float>(x0Array);
            Vector<float> x0Curr;
            Vector<float> y0 = new Vector<float>(_minY);
            Vector<float> y0Curr = y0;
            Vector<float> squareDistance, prevSquareDistance;
            Vector<int> blewLimit;

            for (int j = 0; j < _bitMapSize.Y; j++)
            {                                
                y = Vector<float>.Zero;
                //x0 = dxVec;
                x0 = new Vector<float>(x0Array);
                //bool allZero;
                for (int i = 0; i < _bitMapSize.X; i += slots)
                {
                    /*if( i == 80)
                    {
                        string dum = "stop";
                    }*/
                    y0Curr = y0;
                    x0Curr = x0;
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
                        //prevSquareDistance += squareDistance;
                        blewLimit = Vector.GreaterThan(squareDistance, Four);
                        prevSquareDistance = System.Numerics.Vector.ConditionalSelect(blewLimit, squareDistance, prevSquareDistance);
                        //bail out fast if all blew the limit
                        if (Vector.GreaterThanAll(prevSquareDistance, Four))
                        {
                            break;
                        }
                        else
                        //if (Vector.GreaterThanAny(tempVec, Four))
                        {
                            //first zero out the sum vector at positions where we reached limit of interations
                            //blewLimit = Vector.GreaterThanOrEqual(iter, maxInterVec);
                            blewLimit = Vector.LessThan(iter, maxInterVec) & Vector.LessThanOrEqual(prevSquareDistance, Four);
                            iterStep = Vector.Abs(blewLimit);
                            
                            //iterStep = Vector.LessThan(iter, maxInterVec) & Vector.LessThan(squareDistance, Four);
                            //iterStep = Vector.ConditionalSelect(blewLimit, Vector<int>.Zero, iterStep);
                            //blewLimit = Vector.GreaterThan(squareDistance, Four);
                            //now zero out those where the distance exceeded limit
                            //iterStep = Vector.ConditionalSelect(blewLimit, Vector<int>.Zero, iterStep);
                            x0Curr = Vector.ConditionalSelect(blewLimit, x0Curr, Vector<float>.Zero);
                            y0Curr = Vector.ConditionalSelect(blewLimit, y0Curr,Vector<float>.Zero);
                            x = Vector.ConditionalSelect(blewLimit, x, Vector<float>.Zero);
                            y = Vector.ConditionalSelect(blewLimit, y, Vector<float>.Zero);
                        }
                        //allZero = ();
                        if(Vector.EqualsAll(iterStep, Vector<int>.Zero))
                        {
                            break;
                        }
                    }
                    while (Vector.LessThanAny(iter, maxInterVec));
                    iter.CopyTo(_imageColorsBW, j * _oneLine + i);
                    //_imageColorsBW[j * _oneLine + i] = iter;
                    x0 += x0Step;// dxVec;
                }
                y0 += dyVec;
            }
            hasVectors = true;
            return _imageColorsBW;
        }

    }
}
