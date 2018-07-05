using BenchmarkDotNet.Running;

namespace SIMDBenchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //BenchmarkRunner.Run<Floating>();
            //BenchmarkRunner.Run<Integer>();
            BenchmarkRunner.Run<Mandelbrot>();
        }
    }
}
