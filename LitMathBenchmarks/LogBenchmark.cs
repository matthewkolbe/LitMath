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
                logs[i] = Math.Exp((i - N / 2) / 100);
                flogs[i] = (float)Math.Exp((i - N / 2) / 100);
            }
        }

        [Benchmark]
        public void NaiveLogDouble()
        {
            for (int i = 0; i < N; i++)
                temp = Math.Log(logs[i]);
        }

        [Benchmark]
        public unsafe void LitLogDouble()
        {
            fixed (double* lg = logs) fixed(double* r = results)
            {
                LitLog.Ln(lg, r, N);
            }
        }

        [Benchmark]
        public unsafe void LitLogFloat()
        {
            fixed (float* lg = flogs) fixed(float* r = fresults)
            {
                LitLog.Ln(lg, r, N);
            }
        }

    }
}
