﻿using BenchmarkDotNet.Attributes;
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
        public unsafe void LitErfDouble()
        {
            fixed (double* lg = x) fixed (double* r = y)
                LitNormDist.Erf(lg, r, N);
        }

        [Benchmark]
        public void NaiveErfDouble()
        {
            for (int i = 0; i < N; i++)
                y[i] = SpecialFunctions.Erf(x[i]);
        }

        [Benchmark]
        public unsafe void LitCdfDouble()
        {
            fixed (double* lg = x) fixed (double* r = y)
                LitNormDist.CDF(0.0, 1.0, lg, r, N);
        }

        [Benchmark]
        public unsafe void LitCdfFloat()
        {
            fixed (float* lg = xf) fixed (float* r = yf)
                LitNormDist.CDF(0.0f, 1.0f, lg, r, N);
        }

        [Benchmark]
        public void NaiveCdfDouble()
        {
            for (int i = 0; i < N; i++)
                y[i] = Normal.CDF(0.0, 1.0, x[i]);
        }
    }
}
