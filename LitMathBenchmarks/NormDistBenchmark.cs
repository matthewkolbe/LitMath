using BenchmarkDotNet.Attributes;
using MathNet.Numerics;
using System;
using MathNet.Numerics.Distributions;

namespace LitMathBenchmarks
{
    public class NormDistBenchmark
    {
        double[] x, y;
        float[] xf, yf;

        [Params(1000)]
        public int N;
        double temp = 0.0;

        [GlobalSetup]
        public void SetUp()
        {
            x = new double[N];
            y = new double[N];
            xf = new float[N];
            yf = new float[N];
            var r = new Random(10);

            for (int i = 0; i < N; i++)
            {
                x[i] = 20.0 * (r.NextDouble() - 0.5);
                xf[i] = (float)x[i];
            }
        }

        [Benchmark]
        public void LitErfDouble()
        {
            var xx = x.AsSpan();
            var yy = y.AsSpan();

            Lit.Erf(ref xx, ref yy);
        }

        [Benchmark]
        public void NaiveErfDouble()
        {
            for (int i = 0; i < N; i++)
                y[i] = SpecialFunctions.Erf(x[i]);
        }

        [Benchmark]
        public void LitCdfDouble()
        {
            var xx = x.AsSpan();
            var yy = y.AsSpan();

            Lit.CDF(0.0, 1.0, ref xx, ref yy);
        }

        [Benchmark]
        public void LitCdfFloat()
        {
            var xx = xf.AsSpan();
            var yy = yf.AsSpan();
            Lit.CDF(0.0f, 1.0f, ref xx, ref yy);
        }

        [Benchmark]
        public void NaiveCdfDouble()
        {
            for (int i = 0; i < N; i++)
                y[i] = Normal.CDF(0.0, 1.0, x[i]);
        }
    }
}
