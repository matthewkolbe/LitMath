﻿// Copyright Matthew Kolbe (2022)

using MathNet.Numerics;
using BenchmarkDotNet.Attributes;
using System;

namespace LitMathBenchmarks
{
    public class PolynomialBenchmark
    {
        double[] x, p, results;

        [Params(256, 200000)]
        public int N;

        public int order = 8;

        double temp = 0.0;

        [GlobalSetup]
        public void SetUp()
        {
            x = new double[N];
            results = new double[N];
            p = new double[order];

            var r = new Random();

            for (int i = 0; i < N; i++)
                x[i] = r.NextDouble();

            for(int i = 0; i < order; i++)
                p[i] = r.NextDouble();
        }

        [Benchmark]
        public void NaivePolynomialDouble()
        {
            for (int i = 0; i < N; i++)
                temp = Polynomial.Evaluate(x[i], p);
        }

        [Benchmark]
        public void LitPlynomialDouble()
        {
            Lit.PolynomialValue(in x[0], in p[0], ref results[0], N, order);
        }
    }
}
