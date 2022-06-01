// Copyright Matthew Kolbe (2022)

using BenchmarkDotNet.Attributes;
using System;

namespace LitMathBenchmarks
{
    public class ExpBenchmark
    {
        double[] exps, results;
        float[] fexps, fresults;

        [Params(64, 100000)]
        public int N;
        double temp = 0.0;

        [GlobalSetup]
        public void SetUp()
        {
            exps = new double[N];
            results = new double[N];
            fexps = new float[N];
            fresults = new float[N];

            for (int i = 0; i < N; i++)
            {
                exps[i] = Math.Log(100 * (i + 1) / N);
                fexps[i] = (float)Math.Log(100 * (i + 1) / N);
            }
        }

        [Benchmark]
        public void NaiveExpDouble()
        {
            for (int i = 0; i < N; i++)
                temp = Math.Exp(exps[i]);
        }

        [Benchmark]
        public unsafe void LitExpDouble()
        {
            fixed (double* ex = exps) fixed(double* r = results)
            {
                LitExp.E(ex, r, N);
            }
        }

        [Benchmark]
        public unsafe void LitExpFloat()
        {
            fixed (float* ex = fexps) fixed(float* r = fresults)
            {
                LitExp.E(ex, r, N);
            }
        }
    }
}
