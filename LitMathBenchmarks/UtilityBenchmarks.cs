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
            Util.Copy(ref fr, ref tt, N);
        }

        [Benchmark]
        public void RefArithm()
        {
            Unsafe.As<double[], Vector256<double>[]>(ref from);

            ref var fr = ref MemoryMarshal.GetArrayDataReference(from); 
            ref var tt = ref MemoryMarshal.GetArrayDataReference(to);

            for (int i = 0; i < N;)
            {
                Util.StoreV256(ref tt, i, Avx.Add(Util.LoadV256(ref fr, i), Util.LoadV256(ref tt, i)));
                i += 4;
                Util.StoreV256(ref tt, i, Avx.Add(Util.LoadV256(ref fr, i), Util.LoadV256(ref tt, i)));
                i += 4;
                Util.StoreV256(ref tt, i, Avx.Add(Util.LoadV256(ref fr, i), Util.LoadV256(ref tt, i)));
                i += 4;
                Util.StoreV256(ref tt, i, Avx.Add(Util.LoadV256(ref fr, i), Util.LoadV256(ref tt, i)));
                i += 4;
            }
        }



        [Benchmark]
        public unsafe void NormalCast()
        {
            fixed (double* fr = from) fixed (double* tt = to)
                for (int i = 0; i < N; )
                {
                    Avx.Store(tt, Avx.Add(Avx.LoadVector256(fr + i), Avx.LoadVector256(tt + i)));
                    i += 4;
                    Avx.Store(tt, Avx.Add(Avx.LoadVector256(fr + i), Avx.LoadVector256(tt + i)));
                    i += 4;
                    Avx.Store(tt, Avx.Add(Avx.LoadVector256(fr + i), Avx.LoadVector256(tt + i)));
                    i += 4;
                    Avx.Store(tt, Avx.Add(Avx.LoadVector256(fr + i), Avx.LoadVector256(tt + i)));
                    i += 4;
                }

        }
    }
}