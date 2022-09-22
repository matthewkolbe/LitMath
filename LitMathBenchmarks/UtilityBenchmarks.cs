using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LitMathBenchmarks
{
    public class UtilityBenchmarks
    {
        double[] to, from;

        [Params(4, 8, 32, 256, 2048)]
        public int N;

        [GlobalSetup]
        public void SetUp()
        {
            to = new double[N];
            from = new double[N];

            for (int i = 0; i < N; i++)
                from[i] = i;
        }

        [Benchmark]
        public unsafe void BlkCopy()
        {
            Buffer.BlockCopy(from, 0, to, 0, N * 8);
        }

        [Benchmark]
        public unsafe void MemCopy()
        {
            fixed (double* t = to) fixed (double* f = from)
                Buffer.MemoryCopy(f, t, 8 * N, 8 * N);
        }

        [Benchmark]
        public unsafe void LitCpy()
        {
            fixed (double* t = to) fixed (double* f = from)
                LitUtilities.Copy(f, t, N);
        }

        [Benchmark]
        public void UnsafeAs()
        {
            var fr = from.AsSpan();
            var tt = to.AsSpan();

            for (int i = 0; i < N; i += 4)
            {
                var f = Unsafe.As<double, Vector256<double>>(ref fr[i]);
                var t = Unsafe.As<double, Vector256<double>>(ref tt[i]);
                t = Avx.Add(f, t);
                Unsafe.As<double, Vector256<double>>(ref tt[i]) = t;
            }
        }

        [Benchmark]
        public unsafe void NormalCast()
        {
            fixed(double* fr = from) fixed(double* tt = to)
                for (int i = 0; i < N; i += 4)
                {
                    var f = Avx.LoadVector256(fr + i);
                    var t = Avx.LoadVector256(tt + i);
                    t = Avx.Add(f, t);
                    Avx.Store(tt, t);
                }
        }
    }
}
