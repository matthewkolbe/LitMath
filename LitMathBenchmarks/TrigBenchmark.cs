// Copyright Matthew Kolbe (2022)

using BenchmarkDotNet.Attributes;
using System;

namespace LitMathBenchmarks
{
    public class TrigBenchmark
    {
        double[] sins, results;

        [Params(64, 100000)]
        public int N;
        double temp = 0.0;

        [GlobalSetup]
        public void SetUp()
        {
            sins = new double[N];
            results = new double[N];

            for (int i = 0; i < N; i++)
                sins[i] = Math.Exp((i - N / 2) / 100);

        }

        [Benchmark]
        public void NaiveSinDouble()
        {
            for (int i = 0; i < N; i++)
                temp = Math.Sin(sins[i]);
        }

        [Benchmark]
        public unsafe void LitSinDouble()
        {
            fixed (double* lg = sins) fixed (double* r = results)
            {
                LitTrig.Sin(lg, r, N);
            }
        }

    }
}
