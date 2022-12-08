// Copyright Matthew Kolbe (2022)

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using LitMathBenchmarks;
using System;

var config = ManualConfig.Create(DefaultConfig.Instance)
      .WithOptions(ConfigOptions.JoinSummary)
      .WithOptions(ConfigOptions.DisableLogFile);

BenchmarkRunner.Run(new[]{
            BenchmarkConverter.TypeToBenchmarks( typeof(ExpBenchmark), config),
            //BenchmarkConverter.TypeToBenchmarks( typeof(LogBenchmark), config),
            //BenchmarkConverter.TypeToBenchmarks( typeof(TrigBenchmark), config),
            //BenchmarkConverter.TypeToBenchmarks( typeof(DotProductBenchmark), config),
            //BenchmarkConverter.TypeToBenchmarks( typeof(PolynomialBenchmark), config),
            //BenchmarkConverter.TypeToBenchmarks( typeof(NormDistBenchmark), config),
            //BenchmarkConverter.TypeToBenchmarks( typeof(UtilityBenchmarks), config)
            });

Console.ReadLine();