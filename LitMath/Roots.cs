using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace LitMath
{
    public static partial class Lit
    {
        /// <summary>
        /// Calculates n square roots on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="x">A pointer to the first argument</param>
        /// <param name="y">The return values</param>
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sqrt(in Span<double> x, ref Span<double> y)
        {
            int VSZ = 4;

#if NET8_0_OR_GREATER
            if (Avx512F.IsSupported)
                VSZ = 8;
#endif

            ref var xx = ref MemoryMarshal.GetReference(x);
            ref var yy = ref MemoryMarshal.GetReference(y);
            var n = x.Length;

            if (n < VSZ)
            {
                Span<double> tmpx = stackalloc double[VSZ];
                ref var t = ref MemoryMarshal.GetReference(tmpx);

                for (int j = 0; j < n; j++)
                    Unsafe.Add(ref t, j) = Unsafe.Add(ref xx, j);

                Sqrt(in t, ref t, 0);

                for (int j = 0; j < n; ++j)
                    Unsafe.Add(ref yy, j) = Unsafe.Add(ref t, j);

                return;
            }

            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - Loop.Four(VSZ)))
            {
                Sqrt(in xx, ref yy, i);
                i += VSZ;
                Sqrt(in xx, ref yy, i);
                i += VSZ;
                Sqrt(in xx, ref yy, i);
                i += VSZ;
                Sqrt(in xx, ref yy, i);
                i += VSZ;
            }

            // Calculates the remaining sets of 4 values in a standard loop
            for (; i < (n - Loop.One(VSZ)); i += VSZ)
                Sqrt(in xx, ref yy, i);

            // Cleans up any excess individual values (if n%4 != 0)
            if (i != n)
            {
                i = n - VSZ;
                Sqrt(in xx, ref yy, i);
            }
        }


        /// <summary>
        /// Calculates n square roots on floats via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="x">A pointer to the first argument</param>
        /// <param name="y">The return values</param>
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sqrt(in Span<float> x, ref Span<float> y)
        {
            int VSZ = 8;

#if NET8_0_OR_GREATER
            if (Avx512F.IsSupported)
                VSZ = 16;
#endif

            ref var xx = ref MemoryMarshal.GetReference(x);
            ref var yy = ref MemoryMarshal.GetReference(y);
            var n = x.Length;

            if (n < VSZ)
            {
                Span<float> tmpx = stackalloc float[VSZ];
                ref var t = ref MemoryMarshal.GetReference(tmpx);

                for (int j = 0; j < n; j++)
                    Unsafe.Add(ref t, j) = Unsafe.Add(ref xx, j);

                Sqrt(in t, ref t, 0);

                for (int j = 0; j < n; ++j)
                    Unsafe.Add(ref yy, j) = Unsafe.Add(ref t, j);

                return;
            }

            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - Loop.Four(VSZ)))
            {
                Sqrt(in xx, ref yy, i);
                i += VSZ;
                Sqrt(in xx, ref yy, i);
                i += VSZ;
                Sqrt(in xx, ref yy, i);
                i += VSZ;
                Sqrt(in xx, ref yy, i);
                i += VSZ;
            }

            // Calculates the remaining sets of 4 values in a standard loop
            for (; i < (n - Loop.One(VSZ)); i += VSZ)
                Sqrt(in xx, ref yy, i);

            // Cleans up any excess individual values (if n%4 != 0)
            if (i != n)
            {
                i = n - VSZ;
                Sqrt(in xx, ref yy, i);
            }
        }


        /// <summary>
        /// Calculates 4 square roots on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="xx">A pointer to the first of 4 arguments</param>
        /// <param name="yy">The return values</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Sqrt(in double xx, ref double yy, int index)
        {
#if NET8_0_OR_GREATER
            if(Avx512F.IsSupported)
                Util512.StoreV512(ref yy, index, Avx512F.Sqrt(Util512.LoadV512(in xx, index)));
            else
#endif
                Util.StoreV256(ref yy, index, Avx.Sqrt(Util.LoadV256(in xx, index)));
        }


        /// <summary>
        /// Calculates 8 square roots on floats via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="xx">A pointer to the first of 8 arguments</param>
        /// <param name="yy">The return values</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Sqrt(in float xx, ref float yy, int index)
        {
#if NET8_0_OR_GREATER
            if (Avx512F.IsSupported)
                Util512.StoreV512(ref yy, index, Avx512F.Sqrt(Util512.LoadV512(in xx, index)));
            else
#endif
                Util.StoreV256(ref yy, index, Avx.Sqrt(Util.LoadV256(in xx, index)));
        }
    }
}
