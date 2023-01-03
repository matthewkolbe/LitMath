// Copyright Matthew Kolbe (2022)

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace LitMath
{
    public static partial class Lit
    {
        /// <summary>
        /// Calculates the CDF of a normal distribution via 256-bit SIMD intrinsics.  
        /// </summary>
        /// <param name="mean">The distribution's means</param>
        /// <param name="sigma">The distribution's sigmas</param>
        /// <param name="x">A pointer to the first sample value</param>
        /// <param name="y">The return values</param>
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CDF(in Span<double> mean, in Span<double> sigma, in Span<double> x, ref Span<double> y)
        {
            const int VSZ = 4;
            ref var m0 = ref MemoryMarshal.GetReference(mean);
            ref var s0 = ref MemoryMarshal.GetReference(sigma);
            ref var x0 = ref MemoryMarshal.GetReference(x);
            ref var y0 = ref MemoryMarshal.GetReference(y);

            var n = x.Length;

            if(n < VSZ)
            {
                var mask = Util.CreateMaskDouble(~(int.MaxValue << n));
                var m = Util.LoadMaskedV256(in m0, 0, mask);
                var s = Util.LoadMaskedV256(in s0, 0, mask);
                var xx = Util.LoadMaskedV256(in x0, 0, mask);
                var yy = Util.LoadMaskedV256(in y0, 0, mask);
                CDF(in m, in s, in xx, ref yy);
                Util.StoreMaskedV256(ref y0, 0, yy, mask);

                return;
            }

            int i = 0;

            while (i < (n - 15))
            {
                CDF(in m0, in s0, in x0, ref y0, i);
                i += VSZ;
                CDF(in m0, in s0, in x0, ref y0, i);
                i += VSZ;
                CDF(in m0, in s0, in x0, ref y0, i);
                i += VSZ;
                CDF(in m0, in s0, in x0, ref y0, i);
                i += VSZ;
            }

            for (; i < (n - 3);)
            {
                CDF(in m0, in s0, in x0, ref y0, i);
                i += VSZ;
            }

            if (i != n)
            {
                i = n - VSZ;
                CDF(in m0, in s0, in x0, ref y0, i);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CDF(in double mean, in double sigma, in double x, ref double y, int offset)
        {
            var m = Util.LoadV256(in mean, offset);
            var s = Util.LoadV256(in sigma, offset);
            var xx = Util.LoadV256(in x, offset);
            var yy = Util.LoadV256(in y, offset);
            CDF(in m, in s, in xx, ref yy);
            Util.StoreV256(ref y, offset, yy);
        }


        /// <summary>
        /// Calculates the CDF of a normal distribution via 256-bit SIMD intrinsics.  
        /// </summary>
        /// <param name="mean">The distribution's means</param>
        /// <param name="sigma">The distribution's sigmas</param>
        /// <param name="x">A pointer to the first sample value</param>
        /// <param name="y">The return values</param>
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CDF(in Span<float> mean, in Span<float> sigma, in Span<float> x, ref Span<float> y)
        {
            const int VSZ = 8;
            ref var m0 = ref MemoryMarshal.GetReference(mean);
            ref var s0 = ref MemoryMarshal.GetReference(sigma);
            ref var x0 = ref MemoryMarshal.GetReference(x);
            ref var y0 = ref MemoryMarshal.GetReference(y);

            var n = x.Length;

            if (n < VSZ)
            {
                var mask = Util.CreateMaskFloat(~(int.MaxValue << n));
                var m = Util.LoadMaskedV256(in m0, 0, mask);
                var s = Util.LoadMaskedV256(in s0, 0, mask);
                var xx = Util.LoadMaskedV256(in x0, 0, mask);
                var yy = Util.LoadMaskedV256(in y0, 0, mask);
                CDF(in m, in s, in xx, ref yy);
                Util.StoreMaskedV256(ref y0, 0, yy, mask);

                return;
            }

            int i = 0;

            while (i < (n - 15))
            {
                CDF(in m0, in s0, in x0, ref y0, i);
                i += VSZ;
                CDF(in m0, in s0, in x0, ref y0, i);
                i += VSZ;
                CDF(in m0, in s0, in x0, ref y0, i);
                i += VSZ;
                CDF(in m0, in s0, in x0, ref y0, i);
                i += VSZ;
            }

            for (; i < (n - 3);)
            {
                CDF(in m0, in s0, in x0, ref y0, i);
                i += VSZ;
            }

            if (i != n)
            {
                i = n - VSZ;
                CDF(in m0, in s0, in x0, ref y0, i);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CDF(in float mean, in float sigma, in float x, ref float y, int offset)
        {
            var m = Util.LoadV256(in mean, offset);
            var s = Util.LoadV256(in sigma, offset);
            var xx = Util.LoadV256(in x, offset);
            CDF(in m, in s, in xx, ref s);
            Util.StoreV256(ref y, offset, s);
        }



        /// <summary>
        /// Calculates the CDF of a normal distribution via 256-bit SIMD intrinsics.  
        /// </summary>
        /// <param name="mean">The distribution's mean</param>
        /// <param name="sigma">The distribution's sigma</param>
        /// <param name="x">A pointer to the first sample value</param>
        /// <param name="y">The return values</param>
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CDF(double mean, double sigma, in Span<double> x, ref Span<double> y)
        {
            const int VSZ = 4;
            int i = 0;
            var m = Vector256.Create(mean);
            var s = Vector256.Create(sigma);
            var o = Vector256<double>.Zero;
            ref var x0 = ref MemoryMarshal.GetReference(x);
            ref var y0 = ref MemoryMarshal.GetReference(y);
            Vector256<double> xx;
            var n = x.Length;

            if (n < VSZ)
            {
                Span<double> tmp = stackalloc double[VSZ];
                ref var tmpx = ref MemoryMarshal.GetReference(tmp);
                for (int j = 0; j < n; j++)
                    Unsafe.Add(ref tmpx, j) = Unsafe.Add(ref x0, j);

                xx = Util.LoadV256(in tmpx, i);
                CDF(in m, in s, in xx, ref o);
                Util.StoreV256(ref tmpx, i, o);

                for (int j = 0; j < n; ++j)
                    Unsafe.Add(ref y0, j) = Unsafe.Add(ref tmpx, j);

                return;
            }

            for (; i < (n - 15); )
            {
                xx = Util.LoadV256(in x0, i);
                CDF(in m, in s, in xx, ref o);
                Util.StoreV256(ref y0, i, o);
                i += VSZ;

                xx = Util.LoadV256(in x0, i);
                CDF(in m, in s, in xx, ref o);
                Util.StoreV256(ref y0, i, o);
                i += VSZ;

                xx = Util.LoadV256(in x0, i);
                CDF(in m, in s, in xx, ref o);
                Util.StoreV256(ref y0, i, o);
                i += VSZ;

                xx = Util.LoadV256(in x0, i);
                CDF(in m, in s, in xx, ref o);
                Util.StoreV256(ref y0, i, o);
                i += VSZ;
            }

            for (; i < (n - 3); i += VSZ)
            {
                xx = Util.LoadV256(in x0, i);
                CDF(in m, in s, in xx, ref o);
                Util.StoreV256(ref y0, i, o);
            }

            if (i != n)
            {
                i = n - VSZ;
                xx = Util.LoadV256(in x0, i);
                CDF(in m, in s, in xx, ref o);
                Util.StoreV256(ref y0, i, o);
            }
        }


        /// <summary>
        /// Calculates the CDF of a normal distribution via 256-bit SIMD intrinsics.  
        /// </summary>
        /// <param name="mean">The distribution's mean</param>
        /// <param name="sigma">The distribution's sigma</param>
        /// <param name="x">A pointer to the first sample value</param>
        /// <param name="y">The return values</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CDF(float mean, float sigma, in Span<float> x, ref Span<float> y)
        {
            const int VSZ = 8;
            int i = 0;
            var m = Vector256.Create(mean);
            var s = Vector256.Create(sigma);
            var o = Vector256<float>.Zero;
            ref var x0 = ref MemoryMarshal.GetReference(x);
            ref var y0 = ref MemoryMarshal.GetReference(y);
            Vector256<float> xx;
            var n = x.Length;

            if (n < VSZ)
            {
                Span<float> tmp = stackalloc float[4];
                ref var tmpx = ref MemoryMarshal.GetReference(tmp);
                for (int j = 0; j < n; j++)
                    Unsafe.Add(ref tmpx, j) = Unsafe.Add(ref x0, j);

                xx = Util.LoadV256(in tmpx, i);
                CDF(in m, in s, in xx, ref o);
                Util.StoreV256(ref tmpx, i, o);

                for (int j = 0; j < n; ++j)
                    Unsafe.Add(ref y0, j) = Unsafe.Add(ref tmpx, j);

                return;
            }

            for (; i < (n - 15);)
            {
                xx = Util.LoadV256(in x0, i);
                CDF(in m, in s, in xx, ref o);
                Util.StoreV256(ref y0, i, o);
                i += VSZ;

                xx = Util.LoadV256(in x0, i);
                CDF(in m, in s, in xx, ref o);
                Util.StoreV256(ref y0, i, o);
                i += VSZ;

                xx = Util.LoadV256(in x0, i);
                CDF(in m, in s, in xx, ref o);
                Util.StoreV256(ref y0, i, o);
                i += VSZ;

                xx = Util.LoadV256(in x0, i);
                CDF(in m, in s, in xx, ref o);
                Util.StoreV256(ref y0, i, o);
                i += VSZ;
            }

            for (; i < (n - 3); i += VSZ)
            {
                xx = Util.LoadV256(in x0, i);
                CDF(in m, in s, in xx, ref o);
                Util.StoreV256(ref y0, i, o);
            }

            if (i != n)
            {
                i = n - VSZ;
                xx = Util.LoadV256(in x0, i);
                CDF(in m, in s, in xx, ref o);
                Util.StoreV256(ref y0, i, o);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CDF(in Vector256<double> mean, in Vector256<double> sigma, in Vector256<double> x, ref Vector256<double> y)
        {
            var s = Avx.Multiply(sigma, Double.NormDist.SQRT2);
            var m = Avx.Subtract(x, mean);
            m = Avx.Divide(m, s);
            Erf(in m, ref y);
            y = Avx.Add(y, Double.NormDist.ONE);
            y = Avx.Multiply(y, Double.NormDist.HALF);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CDF(in Vector256<float> mean, in Vector256<float> sigma, in Vector256<float> x, ref Vector256<float> y)
        {
            var s = Avx.Multiply(sigma, Float.NormDist.SQRT2);
            var m = Avx.Subtract(x, mean);
            m = Avx.Divide(m, s);
            //LitUtilities.Max(ref m, LitConstants.Float.NormDist.MAX);
            //LitUtilities.Min(ref m, LitConstants.Float.NormDist.MIN);
            Erf(in m, ref y);
            y = Avx.Add(y, Float.NormDist.ONE);
            y = Avx.Multiply(y, Float.NormDist.HALF);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Erf(in Vector256<double> x, ref Vector256<double> y)
        {

            var sign = Avx.And(Double.NormDist.NEGATIVE_ZERO, x);
            sign = Avx.Or(sign, Double.NormDist.ONE);
            var xx = Avx.AndNot(Double.NormDist.NEGATIVE_ZERO, x);

            var t = Fma.MultiplyAdd(Double.NormDist.ONE_OVER_PI, xx, Double.NormDist.ONE);
            t = Avx.Divide(Double.NormDist.ONE, t);

            var tsq = Avx.Multiply(t, t);
            var yy = Fma.MultiplyAdd(Double.NormDist.E12, tsq, Double.NormDist.E10);
            y = Fma.MultiplyAdd(Double.NormDist.E11, tsq, Double.NormDist.E9);
            yy = Fma.MultiplyAdd(yy, tsq, Double.NormDist.E8);
            y = Fma.MultiplyAdd(y, tsq, Double.NormDist.E7);
            yy = Fma.MultiplyAdd(yy, tsq, Double.NormDist.E6);
            y = Fma.MultiplyAdd(y, tsq, Double.NormDist.E5);
            yy = Fma.MultiplyAdd(yy, tsq, Double.NormDist.E4);
            y = Fma.MultiplyAdd(y, tsq, Double.NormDist.E3);
            yy = Fma.MultiplyAdd(yy, tsq, Double.NormDist.E2);
            y = Fma.MultiplyAdd(y, tsq, Double.NormDist.E1);
            yy = Avx.Multiply(yy, tsq);
            yy = Fma.MultiplyAdd(y, t, yy);

            var exsq = Avx.Multiply(Avx.Multiply(xx, Double.NormDist.NEGONE), xx);

            Lit.Exp(in exsq, ref exsq);

            yy = Avx.Multiply(yy, exsq);
            yy = Avx.Add(Double.NormDist.ONE, yy);
            y = Avx.Multiply(yy, sign);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Erf(in Vector256<float> x, ref Vector256<float> y)
        {
            var sign = Avx.And(Float.NormDist.NEGATIVE_ZERO, x);
            sign = Avx.Or(sign, Float.NormDist.ONE);
            var xx = Avx.AndNot(Float.NormDist.NEGATIVE_ZERO, x);

            var t = Fma.MultiplyAdd(Float.NormDist.ONE_OVER_PI, xx, Float.NormDist.ONE);
            t = Avx.Divide(Float.NormDist.ONE, t);

            var yy = Fma.MultiplyAdd(Float.NormDist.E12, t, Float.NormDist.E11);
            yy = Fma.MultiplyAdd(yy, t, Float.NormDist.E10);
            yy = Fma.MultiplyAdd(yy, t, Float.NormDist.E9);
            yy = Fma.MultiplyAdd(yy, t, Float.NormDist.E8);
            yy = Fma.MultiplyAdd(yy, t, Float.NormDist.E7);
            yy = Fma.MultiplyAdd(yy, t, Float.NormDist.E6);
            yy = Fma.MultiplyAdd(yy, t, Float.NormDist.E5);
            yy = Fma.MultiplyAdd(yy, t, Float.NormDist.E4);
            yy = Fma.MultiplyAdd(yy, t, Float.NormDist.E3);
            yy = Fma.MultiplyAdd(yy, t, Float.NormDist.E2);
            yy = Fma.MultiplyAdd(yy, t, Float.NormDist.E1);
            yy = Avx.Multiply(yy, t);

            var exsq = Avx.Multiply(Avx.Multiply(xx, Float.NormDist.NEGONE), xx);

            Lit.Exp(in exsq, ref exsq);

            yy = Avx.Multiply(yy, exsq); 
            yy = Avx.Add(Float.NormDist.ONE, yy);
            y = Avx.Multiply(yy, sign);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Erf(in double xx, ref double yy, int index)
        {
            var x = Util.LoadV256(in xx, index);
            var y = Vector256<double>.Zero;
            Erf(in x, ref y);
            Util.StoreV256(ref yy, index, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Erf(in Span<double> xx, ref Span<double> yy)
        {
            const int VSZ = 4;
            ref var x0 = ref MemoryMarshal.GetReference(xx);
            ref var y0 = ref MemoryMarshal.GetReference(yy);
            var n = xx.Length;

            // if n < 4, then we handle the special case by creating a 4 element array to work with
            if (n < VSZ)
            {
                Span<double> tmp = stackalloc double[VSZ];
                ref var tmpx = ref MemoryMarshal.GetReference(tmp);

                for (int j = 0; j < n; j++)
                    Unsafe.Add(ref tmpx, j) = Unsafe.Add(ref x0, j);

                Erf(in tmpx, ref tmpx, 0);

                for (int j = 0; j < n; ++j)
                    Unsafe.Add(ref y0, j) = Unsafe.Add(ref tmpx, j);

                return;
            }

            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - 15))
            {
                Erf(in x0, ref y0, i);
                i += VSZ;
                Erf(in x0, ref y0, i);
                i += VSZ;
                Erf(in x0, ref y0, i);
                i += VSZ;
                Erf(in x0, ref y0, i);
                i += VSZ;
            }

            // Calculates the remaining sets of 4 values in a standard loop
            for (; i < (n - 3); i += VSZ)
                Erf(in x0, ref y0, i);

            // Cleans up any excess individual values (if n%4 != 0)
            if (i != n)
            {
                i = n - VSZ;
                Erf(in x0, ref y0, i);
            }
        }
    }
}
