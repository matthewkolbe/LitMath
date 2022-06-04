﻿// Copyright Matthew Kolbe (2022)

using BenchmarkDotNet.Attributes;
using MathNet.Numerics.LinearAlgebra.Double;

namespace LitMathBenchmarks
{
    public class DotProductBenchmark
    {
        double[] x, y;
        float[] xf, yf;
        DenseVector vx, vy;

        [Params(64, 512, 100000)]
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
        public unsafe void LitDotDouble()
        {
            fixed (double* xx = x) fixed (double* yy = y)
            {
                temp = LitBasics.Dot(xx, yy, N);
            }
        }

        [Benchmark]
        public unsafe void LitDotFmaDouble()
        {
            fixed (double* xx = x) fixed (double* yy = y)
            {
                temp = LitBasics.DotFMA(xx, yy, N);
            }
        }

        [Benchmark]
        public unsafe void LitDotFloat()
        {
            fixed (float* xx = xf) fixed (float* yy = yf)
            {
                temp = LitBasics.Dot(xx, yy, N);
            }
        }

        [Benchmark]
        public unsafe void LitDotFmaFloat()
        {
            fixed (float* xx = xf) fixed (float* yy = yf)
            {
                temp = LitBasics.DotFMA(xx, yy, N);
            }
        }
    }
}
