// Copyright Matthew Kolbe (2022)

using BenchmarkDotNet.Attributes;
using System;

namespace LitMathBenchmarks
{
    public class TrigBenchmark
    {
        double[] x, results;

        [Params(64, 100000)]
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
        public unsafe void LitSinDouble()
        {
            fixed (double* lg = x) fixed (double* r = results)
                LitTrig.Sin(lg, r, N);
        }

        [Benchmark]
        public void NaiveCosDouble()
        {
            for (int i = 0; i < N; i++)
                temp = Math.Cos(x[i]);
        }

        [Benchmark]
        public unsafe void LitCosDouble()
        {
            fixed (double* lg = x) fixed (double* r = results)
                LitTrig.Cos(lg, r, N);

        }

        [Benchmark]
        public void NaiveTanDouble()
        {
            for (int i = 0; i < N; i++)
                temp = Math.Tan(x[i]);
        }

        [Benchmark]
        public unsafe void LitTanDouble()
        {
            fixed (double* lg = x) fixed (double* r = results)
                LitTrig.Tan(lg, r, N);

        }
    }
}
