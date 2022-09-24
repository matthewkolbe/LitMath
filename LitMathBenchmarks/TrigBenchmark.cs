// Copyright Matthew Kolbe (2022)

using BenchmarkDotNet.Attributes;
using System;

namespace LitMathBenchmarks
{
    public class TrigBenchmark
    {
        double[] x, results;

        [Params(32, 50000)]
        public int N;
        double temp = 0.0;

        [GlobalSetup]
        public void SetUp()
        {
            x = new double[N];
            results = new double[N];

            for (int i = 0; i < N; i++)
                x[i] = Math.Exp((i - N / 2) / 100);

        }

        [Benchmark]
        public void NaiveSinDouble()
        {
            for (int i = 0; i < N; i++)
                temp = Math.Sin(x[i]);
        }

        [Benchmark]
        public void LitSinDouble()
        {
            var xx = x.AsSpan();
            var rr = results.AsSpan();
            Lit.Sin(ref xx, ref rr);
        }

        [Benchmark]
        public void NaiveCosDouble()
        {
            for (int i = 0; i < N; i++)
                temp = System.Math.Cos(x[i]);
        }

        [Benchmark]
        public void LitCosDouble()
        {
            var xx = x.AsSpan();
            var rr = results.AsSpan();
            Lit.Cos(ref xx, ref rr);

        }

        [Benchmark]
        public void NaiveTanDouble()
        {
            for (int i = 0; i < N; i++)
                temp = System.Math.Tan(x[i]);
        }

        [Benchmark]
        public void LitTanDouble()
        {
            var xx = x.AsSpan();
            var rr = results.AsSpan();
            Lit.Tan(ref xx, ref rr);
        }

        [Benchmark]
        public void NaiveATanDouble()
        {
            for (int i = 0; i < N; i++)
                temp = System.Math.Atan(x[i]);
        }

        [Benchmark]
        public void LitATanDouble()
        {
            var xx = x.AsSpan();
            var rr = results.AsSpan();
            Lit.ATan(ref xx, ref rr);
        }
    }
}
