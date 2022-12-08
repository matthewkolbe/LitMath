using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LitMathBenchmarks
{
    public class UtilityBenchmarks
    {
        double[] to, from;

        [Params(32, 256, 2048)]
        public int N;

        int N4;

        [GlobalSetup]
        public void SetUp()
        {
            N4 = N / 4;

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
            ref var fr = ref MemoryMarshal.GetArrayDataReference(from);
            ref var tt = ref MemoryMarshal.GetArrayDataReference(to);
            Util.Copy(in fr, ref tt, N);
        }
    }
}