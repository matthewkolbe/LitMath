// Copyright Matthew Kolbe (2022)

using BenchmarkDotNet.Attributes;
using MathNet.Numerics.LinearAlgebra.Double;

namespace LitMathBenchmarks
{
    public class DotProductBenchmark
    {
        double[] x, y;
        DenseVector vx, vy;

        [Params(64, 512, 100000)]
        public int N;
        double temp = 0.0;

        [GlobalSetup]
        public void SetUp()
        {
            x = new double[N];
            y = new double[N];
            vx = new DenseVector(N);
            vy = new DenseVector(N);

            for (int i = 0; i < N; i++)
            {
                x[i] = 1.0;
                y[i] = i / N;
                vx[i] = 1.0;
                vy[i] = i / N;
            }
        }

        [Benchmark]
        public void NaiveDotDouble()
        {
            temp = vx.DotProduct(vy);
        }

        [Benchmark]
        public unsafe void LitDotDouble()
        {
            fixed (double* xx = x) fixed (double* yy = y)
            {
                temp = LitBasics.Dot(xx, yy, N);
            }
        }

        [Benchmark]
        public unsafe void LitDot2Double()
        {
            fixed (double* xx = x) fixed (double* yy = y)
            {
                temp = LitBasics.Dot2(xx, yy, N);
            }
        }
    }
}
