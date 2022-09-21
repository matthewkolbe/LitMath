using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LitMathBenchmarks
{
    public class UtilityBenchmarks
    {
        double[] to, from;

        [Params(2, 4, 8, 128, 2048)]
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
            Buffer.BlockCopy(from, 0, to, 0, N*8);
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
    }
}
