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
        public static void CDF(ref Span<double> mean, ref Span<double> sigma, ref Span<double> x, ref Span<double> y)
        {
            const int VSZ = 4;
            ref var m0 = ref MemoryMarshal.GetReference(mean);
            ref var s0 = ref MemoryMarshal.GetReference(sigma);
            ref var x0 = ref MemoryMarshal.GetReference(x);
            ref var y0 = ref MemoryMarshal.GetReference(y);

            var n = x.Length;

            if(n < VSZ)
            {
                Span<double> tmpx = stackalloc double[VSZ];
                ref var tmpx0 = ref MemoryMarshal.GetReference(tmpx);
                Span<double> tmpm = stackalloc double[VSZ];
                ref var tmpm0 = ref MemoryMarshal.GetReference(tmpm);
                Span<double> tmps = stackalloc double[VSZ];
                ref var tmps0 = ref MemoryMarshal.GetReference(tmps);


                for (int j = 0; j < n; j++)
                {
                    Unsafe.Add(ref tmpx0, j) = Unsafe.Add(ref x0, j);
                    Unsafe.Add(ref tmpm0, j) = Unsafe.Add(ref m0, j);
                    Unsafe.Add(ref tmps0, j) = Unsafe.Add(ref s0, j);
                }

                CDF(ref m0, ref s0, ref x0, ref s0, 0);

                for (int j = 0; j < n; ++j)
                    Unsafe.Add(ref y0, j) = Unsafe.Add(ref tmpx0, j);

                return;
            }

            int i = 0;

            while (i < (n - 15))
            {
                CDF(ref m0, ref s0, ref x0, ref y0, i);
                i += VSZ;
                CDF(ref m0, ref s0, ref x0, ref y0, i);
                i += VSZ;
                CDF(ref m0, ref s0, ref x0, ref y0, i);
                i += VSZ;
                CDF(ref m0, ref s0, ref x0, ref y0, i);
                i += VSZ;
            }

            for (; i < (n - 3);)
            {
                CDF(ref m0, ref s0, ref x0, ref y0, i);
                i += VSZ;
            }

            if (i != n)
            {
                i = n - VSZ;
                CDF(ref m0, ref s0, ref x0, ref y0, i);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CDF(ref double mean, ref double sigma, ref double x, ref double y, int offset)
        {
            var m = Util.LoadV256(ref mean, offset);
            var s = Util.LoadV256(ref sigma, offset);
            var xx = Util.LoadV256(ref x, offset);
            var yy = Util.LoadV256(ref y, offset);
            CDF(ref m, ref s, ref xx, ref yy);
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
        public static void CDF(ref Span<float> mean, ref Span<float> sigma, ref Span<float> x, ref Span<float> y)
        {
            const int VSZ = 8;
            ref var m0 = ref MemoryMarshal.GetReference(mean);
            ref var s0 = ref MemoryMarshal.GetReference(sigma);
            ref var x0 = ref MemoryMarshal.GetReference(x);
            ref var y0 = ref MemoryMarshal.GetReference(y);
            Vector256<double> m, s, xx;

            var n = x.Length;

            if (n < VSZ)
            {
                Span<float> tmpx = stackalloc float[VSZ];
                ref var tmpx0 = ref MemoryMarshal.GetReference(tmpx);
                Span<float> tmpm = stackalloc float[VSZ];
                ref var tmpm0 = ref MemoryMarshal.GetReference(tmpm);
                Span<float> tmps = stackalloc float[VSZ];
                ref var tmps0 = ref MemoryMarshal.GetReference(tmps);


                for (int j = 0; j < n; j++)
                {
                    Unsafe.Add(ref tmpx0, j) = Unsafe.Add(ref x0, j);
                    Unsafe.Add(ref tmpm0, j) = Unsafe.Add(ref m0, j);
                    Unsafe.Add(ref tmps0, j) = Unsafe.Add(ref s0, j);
                }

                CDF(ref m0, ref s0, ref x0, ref s0, 0);

                for (int j = 0; j < n; ++j)
                    Unsafe.Add(ref y0, j) = Unsafe.Add(ref tmpx0, j);

                return;
            }

            int i = 0;

            while (i < (n - 15))
            {
                CDF(ref m0, ref s0, ref x0, ref s0, i);
                i += VSZ;
                CDF(ref m0, ref s0, ref x0, ref s0, i);
                i += VSZ;
                CDF(ref m0, ref s0, ref x0, ref s0, i);
                i += VSZ;
                CDF(ref m0, ref s0, ref x0, ref s0, i);
                i += VSZ;
            }

            for (; i < (n - 3);)
            {
                CDF(ref m0, ref s0, ref x0, ref s0, i);
                i += VSZ;
            }

            if (i != n)
            {
                i = n - VSZ;
                CDF(ref m0, ref s0, ref x0, ref s0, i);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CDF(ref float mean, ref float sigma, ref float x, ref float y, int offset)
        {
            var m = Util.LoadV256(ref mean, offset);
            var s = Util.LoadV256(ref sigma, offset);
            var xx = Util.LoadV256(ref x, offset);
            CDF(ref m, ref s, ref xx, ref s);
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
        public static void CDF(double mean, double sigma, ref Span<double> x, ref Span<double> y)
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

                xx = Util.LoadV256(ref tmpx, i);
                CDF(ref m, ref s, ref xx, ref o);
                Util.StoreV256(ref tmpx, i, o);

                for (int j = 0; j < n; ++j)
                    Unsafe.Add(ref y0, j) = Unsafe.Add(ref tmpx, j);

                return;
            }

            for (; i < (n - 15); )
            {
                xx = Util.LoadV256(ref x0, i);
                CDF(ref m, ref s, ref xx, ref o);
                Util.StoreV256(ref y0, i, o);
                i += VSZ;

                xx = Util.LoadV256(ref x0, i);
                CDF(ref m, ref s, ref xx, ref o);
                Util.StoreV256(ref y0, i, o);
                i += VSZ;

                xx = Util.LoadV256(ref x0, i);
                CDF(ref m, ref s, ref xx, ref o);
                Util.StoreV256(ref y0, i, o);
                i += VSZ;

                xx = Util.LoadV256(ref x0, i);
                CDF(ref m, ref s, ref xx, ref o);
                Util.StoreV256(ref y0, i, o);
                i += VSZ;
            }

            for (; i < (n - 3); i += VSZ)
            {
                xx = Util.LoadV256(ref x0, i);
                CDF(ref m, ref s, ref xx, ref o);
                Util.StoreV256(ref y0, i, o);
            }

            if (i != n)
            {
                i = n - VSZ;
                xx = Util.LoadV256(ref x0, i);
                CDF(ref m, ref s, ref xx, ref o);
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
        public static void CDF(float mean, float sigma, ref Span<float> x, ref Span<float> y)
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

                xx = Util.LoadV256(ref tmpx, i);
                CDF(ref m, ref s, ref xx, ref o);
                Util.StoreV256(ref tmpx, i, o);

                for (int j = 0; j < n; ++j)
                    Unsafe.Add(ref y0, j) = Unsafe.Add(ref tmpx, j);

                return;
            }

            for (; i < (n - 15);)
            {
                xx = Util.LoadV256(ref x0, i);
                CDF(ref m, ref s, ref xx, ref o);
                Util.StoreV256(ref y0, i, o);
                i += VSZ;

                xx = Util.LoadV256(ref x0, i);
                CDF(ref m, ref s, ref xx, ref o);
                Util.StoreV256(ref y0, i, o);
                i += VSZ;

                xx = Util.LoadV256(ref x0, i);
                CDF(ref m, ref s, ref xx, ref o);
                Util.StoreV256(ref y0, i, o);
                i += VSZ;

                xx = Util.LoadV256(ref x0, i);
                CDF(ref m, ref s, ref xx, ref o);
                Util.StoreV256(ref y0, i, o);
                i += VSZ;
            }

            for (; i < (n - 3); i += VSZ)
            {
                xx = Util.LoadV256(ref x0, i);
                CDF(ref m, ref s, ref xx, ref o);
                Util.StoreV256(ref y0, i, o);
            }

            if (i != n)
            {
                i = n - VSZ;
                xx = Util.LoadV256(ref x0, i);
                CDF(ref m, ref s, ref xx, ref o);
                Util.StoreV256(ref y0, i, o);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CDF(ref Vector256<double> mean, ref Vector256<double> sigma, ref Vector256<double> x, ref Vector256<double> y)
        {
            var s = Avx.Multiply(sigma, Double.NormDist.SQRT2);
            var m = Avx.Subtract(x, mean);
            m = Avx.Divide(m, s);
            Erf(ref m, ref y);
            y = Avx.Add(y, Double.NormDist.ONE);
            y = Avx.Multiply(y, Double.NormDist.HALF);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CDF(ref Vector256<float> mean, ref Vector256<float> sigma, ref Vector256<float> x, ref Vector256<float> y)
        {
            var s = Avx.Multiply(sigma, Float.NormDist.SQRT2);
            var m = Avx.Subtract(x, mean);
            m = Avx.Divide(m, s);
            //LitUtilities.Max(ref m, LitConstants.Float.NormDist.MAX);
            //LitUtilities.Min(ref m, LitConstants.Float.NormDist.MIN);
            Erf(ref m, ref y);
            y = Avx.Add(y, Float.NormDist.ONE);
            y = Avx.Multiply(y, Float.NormDist.HALF);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Erf(ref Vector256<double> x, ref Vector256<double> y)
        {

            var sign = Avx.And(Double.NormDist.NEGATIVE_ZERO, x);
            sign = Avx.Or(sign, Double.NormDist.ONE);
            var xx = Avx.AndNot(Double.NormDist.NEGATIVE_ZERO, x);

            var t = Fma.MultiplyAdd(Double.NormDist.ONE_OVER_PI, xx, Double.NormDist.ONE);
            t = Avx.Divide(Double.NormDist.ONE, t);

            var yy = Fma.MultiplyAdd(Double.NormDist.E12, t, Double.NormDist.E11);
            yy = Fma.MultiplyAdd(yy, t, Double.NormDist.E10);
            yy = Fma.MultiplyAdd(yy, t, Double.NormDist.E9);
            yy = Fma.MultiplyAdd(yy, t, Double.NormDist.E8);
            yy = Fma.MultiplyAdd(yy, t, Double.NormDist.E7);
            yy = Fma.MultiplyAdd(yy, t, Double.NormDist.E6);
            yy = Fma.MultiplyAdd(yy, t, Double.NormDist.E5);
            yy = Fma.MultiplyAdd(yy, t, Double.NormDist.E4);
            yy = Fma.MultiplyAdd(yy, t, Double.NormDist.E3);
            yy = Fma.MultiplyAdd(yy, t, Double.NormDist.E2);
            yy = Fma.MultiplyAdd(yy, t, Double.NormDist.E1);
            yy = Avx.Multiply(yy, t);

            var exsq = Avx.Multiply(Avx.Multiply(xx, Double.NormDist.NEGONE), xx);

            Lit.Exp(ref exsq, ref exsq);

            yy = Avx.Multiply(yy, exsq);
            yy = Avx.Add(Double.NormDist.ONE, yy);
            y = Avx.Multiply(yy, sign);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Erf(ref Vector256<float> x, ref Vector256<float> y)
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

            Lit.Exp(ref exsq, ref exsq);

            yy = Avx.Multiply(yy, exsq); 
            yy = Avx.Add(Float.NormDist.ONE, yy);
            y = Avx.Multiply(yy, sign);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Erf(ref double xx, ref double yy, int index)
        {
            var x = Util.LoadV256(ref xx, index);
            var y = Vector256<double>.Zero;
            Erf(ref x, ref y);
            Util.StoreV256(ref yy, index, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Erf(ref Span<double> xx, ref Span<double> yy)
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

                Erf(ref tmpx, ref tmpx, 0);

                for (int j = 0; j < n; ++j)
                    Unsafe.Add(ref y0, j) = Unsafe.Add(ref tmpx, j);

                return;
            }

            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - 15))
            {
                Erf(ref x0, ref y0, i);
                i += VSZ;
                Erf(ref x0, ref y0, i);
                i += VSZ;
                Erf(ref x0, ref y0, i);
                i += VSZ;
                Erf(ref x0, ref y0, i);
                i += VSZ;
            }

            // Calculates the remaining sets of 4 values in a standard loop
            for (; i < (n - 3); i += VSZ)
                Erf(ref x0, ref y0, i);

            // Cleans up any excess individual values (if n%4 != 0)
            if (i != n)
            {
                i = n - VSZ;
                Erf(ref x0, ref y0, i);
            }
        }
    }
}
