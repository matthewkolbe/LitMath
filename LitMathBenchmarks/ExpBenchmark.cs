// Copyright Matthew Kolbe (2022)

using BenchmarkDotNet.Attributes;
using System;
using System.Threading.Tasks;
using MKLNET;

namespace LitMathBenchmarks
{
    public class ExpBenchmark
    {
        double[] exps, results;
        float[] fexps, fresults;

        [Params(4, 8, 128, 2048, 120000)]
        public int N;
        double temp = 0.0;
        int cores = Environment.ProcessorCount;

        [GlobalSetup]
        public void SetUp()
        {
            exps = new double[N];
            results = new double[N];
            fexps = new float[N];
            fresults = new float[N];

            for (int i = 0; i < N; i++)
            {
                exps[i] = System.Math.Log(100 * (i + 1) / N);
                fexps[i] = (float)System.Math.Log(100 * (i + 1) / N);
            }
        }

        [Benchmark]
        public void NaiveExpDouble()
        {
            for (int i = 0; i < N; i++)
                temp = System.Math.Exp(exps[i]);
        }

        [Benchmark]
        public void LitExpDouble()
        {
            var ex = exps.AsSpan();
            var r = results.AsSpan();
            Lit.Exp(ref ex, ref r);
        }

        [Benchmark]
        public void LitExpDoubleParallel()
        {
            var n = N / cores;

            Parallel.For(0, cores, i =>
            {
                var ex = exps.AsSpan().Slice(i, n);
                var r = results.AsSpan().Slice(i, n);
                Lit.Exp(ref ex, ref r);
            });

        }

        [Benchmark]
        public void MklNet()
        {
            Vml.Exp(exps, results);
        }


        [Benchmark]
        public void LitExpFloat()
        {
            var ex = fexps.AsSpan();
            var r = fresults.AsSpan();
            Lit.Exp(ref ex, ref r);
        }
    }
}
