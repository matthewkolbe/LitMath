// Copyright Matthew Kolbe (2022)

using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace LitMath
{
    public static class LitNormDist
    {

        /// <summary>
        /// Calculates the CDF of a normal distribution via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="mean">The distribution's means</param>
        /// <param name="sigma">The distribution's sigmas</param>
        /// <param name="x">A Span to the first sample value</param>
        /// <param name="y">The return values</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CDF(ref Span<double> mean, ref Span<double> sigma, ref Span<double> x, ref Span<double> y)
        {
            unsafe
            {
                fixed(double* m = mean) fixed(double* s = sigma) fixed (double* xx = x) fixed (double* yy = y)
                    CDF(m, s, xx, yy, x.Length);
            }
        }


        /// <summary>
        /// Calculates the CDF of a normal distribution via 256-bit SIMD intrinsics.  
        /// </summary>
        /// <param name="mean">The distribution's means</param>
        /// <param name="sigma">The distribution's sigmas</param>
        /// <param name="x">A Span to the first sample value</param>
        /// <param name="y">The return values</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CDF(ref Span<float> mean, ref Span<float> sigma, ref Span<float> x, ref Span<float> y)
        {
            unsafe
            {
                fixed (float* m = mean) fixed (float* s = sigma) fixed (float* xx = x) fixed (float* yy = y)
                    CDF(m, s, xx, yy, x.Length);
            }
        }


        /// <summary>
        /// Calculates the CDF of a normal distribution via 256-bit SIMD intrinsics. . 
        /// </summary>
        /// <param name="mean">The distribution's mean</param>
        /// <param name="sigma">The distribution's sigma</param>
        /// <param name="x">A Span to the first sample value</param>
        /// <param name="y">The return values</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CDF(double mean, double sigma, ref Span<double> x, ref Span<double> y)
        {
            unsafe
            {
                fixed (double* xx = x) fixed (double* yy = y)
                    CDF(mean, sigma, xx, yy, x.Length);
            }
        }


        /// <summary>
        /// Calculates the CDF of a normal distribution via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="mean">The distribution's mean</param>
        /// <param name="sigma">The distribution's sigma</param>
        /// <param name="x">A Span to the first sample value</param>
        /// <param name="y">The return values</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CDF(float mean, float sigma, ref Span<float> x, ref Span<float> y)
        {
            unsafe
            {
                fixed (float* xx = x) fixed (float* yy = y)
                    CDF(mean, sigma, xx, yy, x.Length);
            }
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
        public static unsafe void CDF(double* mean, double* sigma, double*x, double* y, int n)
        {
            const int VSZ = 4;

            int i = 0;

            while (i < (n - 15))
            {
                var m = Avx.LoadVector256(mean + i);
                var s = Avx.LoadVector256(sigma + i);
                var xx = Avx.LoadVector256(x + i);
                CDF(ref m, ref s, ref xx, ref s);
                Avx.Store(y + i, s);
                i += VSZ;

                m = Avx.LoadVector256(mean + i);
                s = Avx.LoadVector256(sigma + i);
                xx = Avx.LoadVector256(x + i);
                CDF(ref m, ref s, ref xx, ref s);
                Avx.Store(y + i, s);
                i += VSZ;

                m = Avx.LoadVector256(mean + i);
                s = Avx.LoadVector256(sigma + i);
                xx = Avx.LoadVector256(x + i);
                CDF(ref m, ref s, ref xx, ref s);
                Avx.Store(y + i, s);
                i += VSZ;

                m = Avx.LoadVector256(mean + i);
                s = Avx.LoadVector256(sigma + i);
                xx = Avx.LoadVector256(x + i);
                CDF(ref m, ref s, ref xx, ref s);
                Avx.Store(y + i, s);
                i += VSZ;
            }

            for (; i < (n - 3); i+= 4)
            {
                var m = Avx.LoadVector256(mean + i);
                var s = Avx.LoadVector256(sigma + i);
                var xx = Avx.LoadVector256(x + i);
                CDF(ref m, ref s, ref xx, ref s);
                Avx.Store(y + i, s);
            }

            if (i != n)
            {
                var nn = i;
                var tmpm = stackalloc double[4];
                var tmps = stackalloc double[4];
                var tmpx = stackalloc double[4];

                for (int j = 0; j < (n - i); ++j)
                {
                    tmpm[j] = mean[j + i];
                    tmps[j] = sigma[j + i];
                    tmpx[j] = x[j + i];
                }

                var m = Avx.LoadVector256(tmpm);
                var s = Avx.LoadVector256(tmps);
                var xx = Avx.LoadVector256(tmpx);

                CDF(ref m, ref s, ref xx, ref s);

                Avx.Store(tmpm, s);

                for (; i < n; ++i)
                    y[i] = tmpm[i-nn];
            }
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
        public static unsafe void CDF(float* mean, float* sigma, float* x, float* y, int n)
        {
            int i = 0;

            for (; i < (n - 31); i += 32)
            {
                var m = Avx.LoadVector256(mean + i);
                var s = Avx.LoadVector256(sigma + i);
                var xx = Avx.LoadVector256(x + i);
                CDF(ref m, ref s, ref xx, ref s);
                Avx.Store(y + i, s);

                m = Avx.LoadVector256(mean + i + 8);
                s = Avx.LoadVector256(sigma + i + 8);
                xx = Avx.LoadVector256(x + i + 8);
                CDF(ref m, ref s, ref xx, ref s);
                Avx.Store(y + i + 8, s);

                m = Avx.LoadVector256(mean + i + 16);
                s = Avx.LoadVector256(sigma + i + 16);
                xx = Avx.LoadVector256(x + i + 16);
                CDF(ref m, ref s, ref xx, ref s);
                Avx.Store(y + i + 16, s);

                m = Avx.LoadVector256(mean + i + 24);
                s = Avx.LoadVector256(sigma + i + 24);
                xx = Avx.LoadVector256(x + i + 24);
                CDF(ref m, ref s, ref xx, ref s);
                Avx.Store(y + i + 24, s);
            }

            for (; i < (n - 7); i += 8)
            {
                var m = Avx.LoadVector256(mean + i);
                var s = Avx.LoadVector256(sigma + i);
                var xx = Avx.LoadVector256(x + i);
                CDF(ref m, ref s, ref xx, ref s);
                Avx.Store(y + i, s);
            }

            if (i != n)
            {
                var nn = i;
                var tmpm = stackalloc float[8];
                var tmps = stackalloc float[8];
                var tmpx = stackalloc float[8];

                for (int j = 0; j < (n - i); ++j)
                {
                    tmpm[j] = mean[j + i];
                    tmps[j] = sigma[j + i];
                    tmpx[j] = x[j + i];
                }

                var m = Avx.LoadVector256(tmpm);
                var s = Avx.LoadVector256(tmps);
                var xx = Avx.LoadVector256(tmpx);

                CDF(ref m, ref s, ref xx, ref s);

                Avx.Store(tmpm, s);

                for (; i < n; ++i)
                    y[i] = tmpm[i - nn];
            }
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
        public static unsafe void CDF(double mean, double sigma, double* x, double* y, int n)
        {
            int i = 0;
            var m = Vector256.Create(mean);
            var s = Vector256.Create(sigma);
            var o = Vector256<double>.Zero;

            for (; i < (n - 15); i += 16)
            {
                var xx = Avx.LoadVector256(x + i);
                CDF(ref m, ref s, ref xx, ref o);
                Avx.Store(y + i, o);

                xx = Avx.LoadVector256(x + i + 4);
                CDF(ref m, ref s, ref xx, ref o);
                Avx.Store(y + i + 4, o);

                xx = Avx.LoadVector256(x + i + 8);
                CDF(ref m, ref s, ref xx, ref o);
                Avx.Store(y + i + 8, o);

                xx = Avx.LoadVector256(x + i + 12);
                CDF(ref m, ref s, ref xx, ref o);
                Avx.Store(y + i + 12, o);
            }

            for (; i < (n - 3); i += 4)
            {
                var xx = Avx.LoadVector256(x + i);
                CDF(ref m, ref s, ref xx, ref o);
                Avx.Store(y + i, o);
            }

            if (i != n)
            {
                var nn = i;
                var tmpx = stackalloc double[4];

                for (int j = 0; j < 4; ++j)
                    tmpx[j] = x[j + i];

                var xx = Avx.LoadVector256(tmpx);

                CDF(ref m, ref s, ref xx, ref o);

                Avx.Store(tmpx, o);

                for (; i < n; ++i)
                    y[i] = tmpx[i - nn];
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
        public static unsafe void CDF(float mean, float sigma, float* x, float* y, int n)
        {
            int i = 0;
            var m = Vector256.Create(mean);
            var s = Vector256.Create(sigma);
            var o = Vector256<float>.Zero;

            const int VSZ = 8;

            if(n < VSZ)
            {
                var tmpx = stackalloc float[VSZ];
                for (int j = 0; j < n; j++)
                    tmpx[j] = x[j];

                var xx = Avx.LoadVector256(tmpx);

                CDF(ref m, ref s, ref xx, ref xx);

                Avx.Store(tmpx, xx);

                for (int j = 0; j < n; ++j)
                    y[j] = tmpx[j];

                return;
            }


            for (; i < (n - 31);)
            {
                var x1 = Avx.LoadVector256(x + i);
                CDF(ref m, ref s, ref x1, ref o);
                Avx.Store(y + i, o);
                i += VSZ;

                var x2 = Avx.LoadVector256(x + i);
                CDF(ref m, ref s, ref x2, ref o);
                Avx.Store(y + i, o);
                i += VSZ;

                var x3 = Avx.LoadVector256(x + i);
                CDF(ref m, ref s, ref x3, ref o);
                Avx.Store(y + i, o);
                i += VSZ;

                var x4 = Avx.LoadVector256(x + i);
                CDF(ref m, ref s, ref x4, ref o);
                Avx.Store(y + i, o);
                i += VSZ;
            }

            for (; i < (n-7);)
            {
                var xx = Avx.LoadVector256(x + i);
                CDF(ref m, ref s, ref xx, ref o);
                Avx.Store(y + i, o);
                i += VSZ;
            }

            if (i != n)
            {
                var nn = i;
                var tmpx = stackalloc float[8];

                for (int j = i; j < n - i; j++)
                    tmpx[j - i] = x[j];

                var xx = Avx.LoadVector256(tmpx);

                CDF(ref m, ref s, ref xx, ref o);

                Avx.Store(tmpx, o);

                for (; i < n; i++)
                    y[i] = tmpx[i - nn];
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CDF(ref Vector256<double> mean, ref Vector256<double> sigma, ref Vector256<double> x, ref Vector256<double> y)
        {
            var s = Avx.Multiply(sigma, LitConstants.Double.NormDist.SQRT2);
            var m = Avx.Subtract(x, mean);
            m = Avx.Divide(m, s);
            Erf(ref m, ref y);
            y = Avx.Add(y, LitConstants.Double.NormDist.ONE);
            y = Avx.Multiply(y, LitConstants.Double.NormDist.HALF);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CDF(ref Vector256<float> mean, ref Vector256<float> sigma, ref Vector256<float> x, ref Vector256<float> y)
        {
            var s = Avx.Multiply(sigma, LitConstants.Float.NormDist.SQRT2);
            var m = Avx.Subtract(x, mean);
            m = Avx.Divide(m, s);
            //LitUtilities.Max(ref m, LitConstants.Float.NormDist.MAX);
            //LitUtilities.Min(ref m, LitConstants.Float.NormDist.MIN);
            Erf(ref m, ref y);
            y = Avx.Add(y, LitConstants.Float.NormDist.ONE);
            y = Avx.Multiply(y, LitConstants.Float.NormDist.HALF);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Erf(ref Vector256<double> x, ref Vector256<double> y)
        {

            var sign = Avx.And(LitConstants.Double.NormDist.NEGATIVE_ZERO, x);
            sign = Avx.Or(sign, LitConstants.Double.NormDist.ONE);
            var xx = Avx.AndNot(LitConstants.Double.NormDist.NEGATIVE_ZERO, x);

            var t = Fma.MultiplyAdd(LitConstants.Double.NormDist.ONE_OVER_PI, xx, LitConstants.Double.NormDist.ONE);
            t = Avx.Divide(LitConstants.Double.NormDist.ONE, t);

            var yy = Fma.MultiplyAdd(LitConstants.Double.NormDist.E12, t, LitConstants.Double.NormDist.E11);
            yy = Fma.MultiplyAdd(yy, t, LitConstants.Double.NormDist.E10);
            yy = Fma.MultiplyAdd(yy, t, LitConstants.Double.NormDist.E9);
            yy = Fma.MultiplyAdd(yy, t, LitConstants.Double.NormDist.E8);
            yy = Fma.MultiplyAdd(yy, t, LitConstants.Double.NormDist.E7);
            yy = Fma.MultiplyAdd(yy, t, LitConstants.Double.NormDist.E6);
            yy = Fma.MultiplyAdd(yy, t, LitConstants.Double.NormDist.E5);
            yy = Fma.MultiplyAdd(yy, t, LitConstants.Double.NormDist.E4);
            yy = Fma.MultiplyAdd(yy, t, LitConstants.Double.NormDist.E3);
            yy = Fma.MultiplyAdd(yy, t, LitConstants.Double.NormDist.E2);
            yy = Fma.MultiplyAdd(yy, t, LitConstants.Double.NormDist.E1);
            yy = Avx.Multiply(yy, t);

            var exsq = Avx.Multiply(Avx.Multiply(xx, LitConstants.Double.NormDist.NEGONE), xx);

            LitExp.Exp(ref exsq, ref exsq);

            yy = Avx.Multiply(yy, exsq);
            yy = Avx.Add(LitConstants.Double.NormDist.ONE, yy);
            y = Avx.Multiply(yy, sign);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Erf(ref Vector256<float> x, ref Vector256<float> y)
        {
            var sign = Avx.And(LitConstants.Float.NormDist.NEGATIVE_ZERO, x);
            sign = Avx.Or(sign, LitConstants.Float.NormDist.ONE);
            var xx = Avx.AndNot(LitConstants.Float.NormDist.NEGATIVE_ZERO, x);

            var t = Fma.MultiplyAdd(LitConstants.Float.NormDist.ONE_OVER_PI, xx, LitConstants.Float.NormDist.ONE);
            t = Avx.Divide(LitConstants.Float.NormDist.ONE, t);

            var yy = Fma.MultiplyAdd(LitConstants.Float.NormDist.E12, t, LitConstants.Float.NormDist.E11);
            yy = Fma.MultiplyAdd(yy, t, LitConstants.Float.NormDist.E10);
            yy = Fma.MultiplyAdd(yy, t, LitConstants.Float.NormDist.E9);
            yy = Fma.MultiplyAdd(yy, t, LitConstants.Float.NormDist.E8);
            yy = Fma.MultiplyAdd(yy, t, LitConstants.Float.NormDist.E7);
            yy = Fma.MultiplyAdd(yy, t, LitConstants.Float.NormDist.E6);
            yy = Fma.MultiplyAdd(yy, t, LitConstants.Float.NormDist.E5);
            yy = Fma.MultiplyAdd(yy, t, LitConstants.Float.NormDist.E4);
            yy = Fma.MultiplyAdd(yy, t, LitConstants.Float.NormDist.E3);
            yy = Fma.MultiplyAdd(yy, t, LitConstants.Float.NormDist.E2);
            yy = Fma.MultiplyAdd(yy, t, LitConstants.Float.NormDist.E1);
            yy = Avx.Multiply(yy, t);

            var exsq = Avx.Multiply(Avx.Multiply(xx, LitConstants.Float.NormDist.NEGONE), xx);

            LitExp.Exp(ref exsq, ref exsq);

            yy = Avx.Multiply(yy, exsq); 
            yy = Avx.Add(LitConstants.Float.NormDist.ONE, yy);
            y = Avx.Multiply(yy, sign);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Erf(double* xx, double* yy)
        {
            var x = Avx.LoadVector256(xx);
            var y = Vector256<double>.Zero;
            Erf(ref x, ref y);
            Avx.Store(yy, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Erf(double* xx, double* yy, int n)
        {
            const int VSZ = 4;

            // if n < 4, then we handle the special case by creating a 4 element array to work with
            if (n < VSZ)
            {
                var tmpx = stackalloc double[VSZ];
                for (int j = 0; j < n; j++)
                    tmpx[j] = xx[j];

                Erf(tmpx, tmpx);

                for (int j = 0; j < n; ++j)
                    yy[j] = tmpx[j];

                return;
            }

            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - 15))
            {
                Erf(xx + i, yy + i);
                i += VSZ;
                Erf(xx + i, yy + i);
                i += VSZ;
                Erf(xx + i, yy + i);
                i += VSZ;
                Erf(xx + i, yy + i);
                i += VSZ;
            }

            // Calculates the remaining sets of 4 values in a standard loop
            for (; i < (n - 3); i += VSZ)
                Erf(xx + i, yy + i);

            // Cleans up any excess individual values (if n%4 != 0)
            if (i != n)
            {
                i = n - VSZ;
                Erf(xx + i, yy + i);
            }
        }
    }
}
