// Copyright Matthew Kolbe (2022)

using BenchmarkDotNet.Attributes;
using MathNet.Numerics.LinearAlgebra.Double;
using System;

namespace LitMathBenchmarks
{
    public class DotProductBenchmark
    {
        double[] x, y;
        float[] xf, yf;
        DenseVector vx, vy;

        [Params(16, 512, 1000000)]
        public int N;
        double temp = 0.0;

        [GlobalSetup]
        public void SetUp()
        {
            x = new double[N];
            y = new double[N];
            vx = new DenseVector(N);
            vy = new DenseVector(N);
            xf = new float[N];
            yf = new float[N];

            for (int i = 0; i < N; i++)
            {
                x[i] = 1.0;
                y[i] = i / N;
                vx[i] = 1.0;
                vy[i] = i / N;
                xf[i] = 1.0f;
                yf[i] = i / N;
            }
        }

        [Benchmark]
        public void NaiveDotDouble()
        {
            temp = vx.DotProduct(vy);
        }

        [Benchmark]
        public void LitDotDouble()
        {
            temp = Lit.Dot(in x[0], in y[0], N);
        }

        [Benchmark]
        public void LitDotFloat()
        {
            var xx = xf.AsSpan();
            var yy = yf.AsSpan();
            temp = Lit.Dot(in xx, in yy);
        }
    }
}
