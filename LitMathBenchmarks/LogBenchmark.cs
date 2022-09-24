// Copyright Matthew Kolbe (2022)

using BenchmarkDotNet.Attributes;
using System;

namespace LitMathBenchmarks
{
    public class LogBenchmark
    {
        double[] logs, results;
        float[] flogs, fresults;

        [Params(64, 100000)]
        public int N;
        double temp = 0.0;

        [GlobalSetup]
        public void SetUp()
        {
            logs = new double[N];
            results = new double[N];
            flogs = new float[N];
            fresults = new float[N];

            for (int i = 0; i < N; i++)
            {
                logs[i] = System.Math.Exp((i - N / 2) / 100);
                flogs[i] = (float)System.Math.Exp((i - N / 2) / 100);
            }
        }

        [Benchmark]
        public void NaiveLogDouble()
        {
            for (int i = 0; i < N; i++)
                temp = System.Math.Log(logs[i]);
        }

        [Benchmark]
        public void LitLogDouble()
        {
            var lg = logs.AsSpan();
            var r = results.AsSpan();
            Lit.Ln(ref lg, ref r);
            
        }

        [Benchmark]
        public void LitLogFloat()
        {
            var lg = flogs.AsSpan();
            var r = fresults.AsSpan();
            Lit.Ln(ref lg, ref r);
        }

    }
}
