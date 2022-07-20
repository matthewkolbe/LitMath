// Copyright Matthew Kolbe (2022)

using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace LitMath
{
    public class LitTrig
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sin(ref Vector256<double> x, ref Vector256<double> y)
        {
            // Since sin() is periodic around 2pi, this converts x into the range of [0, 2pi]
            var xt = Avx.Subtract(x, Avx.Multiply(LitConstants.Double.Trig.TWOPI, Avx.Floor(Avx.Divide(x, LitConstants.Double.Trig.TWOPI))));
            
            // Since sin() in [0, 2pi] is an odd function around pi, this converts the range to [0, pi], then stores whether
            // or not the result needs to be negated in negend.
            var negend = Avx.CompareGreaterThan(xt, LitConstants.Double.Trig.PI);
            xt = Avx.Subtract(xt, Avx.And(negend, LitConstants.Double.Trig.PI));

            negend = Avx.And(negend, LitConstants.Double.Trig.NEGATIVE_TWO);
            negend = Avx.Add(negend, LitConstants.Double.Trig.ONE);

            var nanend = Avx.CompareNotEqual(x, x);
            nanend = Avx.Add(nanend, Avx.CompareGreaterThan(x, LitConstants.Double.Trig.HIGH));
            nanend = Avx.Add(nanend, Avx.CompareLessThan(x, LitConstants.Double.Trig.LOW));

            // Since sin() on [0, pi] is an even function around pi/2, this "folds" the range into [0, pi/2]. I.e. 3pi/5 becomes 2pi/5.
            xt = Avx.Subtract(LitConstants.Double.Trig.QUARTERPI, LitUtilities.Abs(Avx.Subtract(xt, LitConstants.Double.Trig.HALFPI)));

            // This is a Taylor series approximation of sin() on [0, pi/2] centered at pi/4. Centering at pi/4 actually happened on
            // the previous step where we called "Avx.Subtract(Constants.Double.QUARTERPI". That should have been Avx.Subtract(Constants.Double.HALFPI,
            // but to save computation it was rolled into a single operation.
            y = Vector256.Create(-7.64716373181981647590113198578807E-13);

            y = Fma.MultiplyAdd(y, xt, LitConstants.Double.Trig.P14);
            y = Fma.MultiplyAdd(y, xt, LitConstants.Double.Trig.P13);
            y = Fma.MultiplyAdd(y, xt, LitConstants.Double.Trig.P12);
            y = Fma.MultiplyAdd(y, xt, LitConstants.Double.Trig.P11);
            y = Fma.MultiplyAdd(y, xt, LitConstants.Double.Trig.P10);
            y = Fma.MultiplyAdd(y, xt, LitConstants.Double.Trig.P9);
            y = Fma.MultiplyAdd(y, xt, LitConstants.Double.Trig.P8);
            y = Fma.MultiplyAdd(y, xt, LitConstants.Double.Trig.P7);
            y = Fma.MultiplyAdd(y, xt, LitConstants.Double.Trig.P6);
            y = Fma.MultiplyAdd(y, xt, LitConstants.Double.Trig.P5);
            y = Fma.MultiplyAdd(y, xt, LitConstants.Double.Trig.P4);
            y = Fma.MultiplyAdd(y, xt, LitConstants.Double.Trig.P3);
            y = Fma.MultiplyAdd(y, xt, LitConstants.Double.Trig.NEGHALF);
            y = Fma.MultiplyAdd(y, xt, LitConstants.Double.Trig.ONE);
            y = Fma.MultiplyAdd(y, xt, LitConstants.Double.Trig.ONE);

            y = Avx.Multiply(y, LitConstants.Double.Trig.SIN_OF_QUARTERPI);

            y = Fma.MultiplyAdd(y, negend, nanend);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sinh(ref Vector256<double> x, ref Vector256<double> y)
        {
            LitExp.Exp(ref x, ref y);
            var iy = Avx.Divide(LitConstants.Double.Trig.ONE, y);
            y = Avx.Multiply(LitConstants.Double.Trig.HALF, Avx.Subtract(y, iy));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Cosh(ref Vector256<double> x, ref Vector256<double> y)
        {
            LitExp.Exp(ref x, ref y);
            var iy = Avx.Divide(LitConstants.Double.Trig.ONE, y);
            y = Avx.Multiply(LitConstants.Double.Trig.HALF, Avx.Add(y, iy));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Tanh(ref Vector256<double> x, ref Vector256<double> y)
        {
            LitExp.Exp(ref x, ref y);
            var iy = Avx.Divide(LitConstants.Double.Trig.ONE, y);
            y = Avx.Divide(Avx.Subtract(y, iy), Avx.Add(y, iy));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ASinh(ref Vector256<double> x, ref Vector256<double> y)
        {
            y = Avx.Add(Avx.Multiply(x, x), LitConstants.Double.Trig.ONE);
            y = Avx.Sqrt(y);
            x = Avx.Add(x, y);
            LitLog.Ln(ref x, ref y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ACosh(ref Vector256<double> x, ref Vector256<double> y)
        {
            y = Avx.Subtract(Avx.Multiply(x, x), LitConstants.Double.Trig.ONE);
            y = Avx.Sqrt(y);
            x = Avx.Add(x, y);
            LitLog.Ln(ref x, ref y);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ATan(ref Vector256<double> x, ref Vector256<double> y)
        {
            // Idea taken from https://github.com/avrdudes/avr-libc/blob/main/libm/fplib/atan.S

            //  Algorithm:
	        //  if (x > 1)
	        //      return Pi/2 - atanf(1/x)
	        //  elif (x < -1)
	        //      return -Pi/2 - atanf(1/x)
	        //  else
	        //      return x * (1 - C1 * x**2 + ... + CN * x**2N)
             

            // Ensures NaN inputs are NaN outputs
            var nanend = Avx.CompareNotEqual(x, x);

            // The original algorithm has some branching in it, but a manageable amount. This section modifies negend to also
            // have -Pi/2 or Pi/2 added at the end if |x| > 1 (since NaN is reflected by adding NaN to the answer at the end).
            nanend = Avx.Add(nanend, Avx.And(Avx.CompareGreaterThan(x, LitConstants.Double.Trig.ONE), LitConstants.Double.Trig.HALFPI));
            var invx = Avx.Divide(LitConstants.Double.Trig.NEGONE, x);
            var gtmask = Avx.CompareGreaterThan(x, LitConstants.Double.Trig.ONE);
            var ltmask = Avx.CompareLessThanOrEqual(x, LitConstants.Double.Trig.ONE);
            var xx = Avx.Add(Avx.And(gtmask, invx), Avx.And(ltmask, x));
            nanend = Avx.Add(nanend, Avx.And(Avx.CompareLessThan(x, LitConstants.Double.Trig.NEGONE), LitConstants.Double.Trig.NEGHALFPI));
            gtmask = Avx.CompareGreaterThanOrEqual(x, LitConstants.Double.Trig.NEGONE);
            ltmask = Avx.CompareLessThan(x, LitConstants.Double.Trig.NEGONE);
            xx = Avx.Add(Avx.And(ltmask, invx),Avx.And(gtmask, xx));

            // This algorithm leverages the fact that ATan is an odd function, and converts negative
            // inputs to positive ones, and changes the output sign at the end.
            var negend = Avx.CompareLessThan(xx, LitConstants.Double.Trig.ZERO);
            negend = Avx.And(negend, LitConstants.Double.Trig.NEGATIVE_TWO);
            negend = Avx.Add(negend, LitConstants.Double.Trig.ONE);

            var xt = Avx.Multiply(xx, xx);

            var yy = Vector256.Create(0.0004071627951457367);
            yy = Fma.MultiplyAdd(yy, xt, LitConstants.Double.Trig.AT12);
            yy = Fma.MultiplyAdd(yy, xt, LitConstants.Double.Trig.AT11);
            yy = Fma.MultiplyAdd(yy, xt, LitConstants.Double.Trig.AT10);
            yy = Fma.MultiplyAdd(yy, xt, LitConstants.Double.Trig.AT9);
            yy = Fma.MultiplyAdd(yy, xt, LitConstants.Double.Trig.AT8);
            yy = Fma.MultiplyAdd(yy, xt, LitConstants.Double.Trig.AT7);
            yy = Fma.MultiplyAdd(yy, xt, LitConstants.Double.Trig.AT6);
            yy = Fma.MultiplyAdd(yy, xt, LitConstants.Double.Trig.AT5);
            yy = Fma.MultiplyAdd(yy, xt, LitConstants.Double.Trig.AT4);
            yy = Fma.MultiplyAdd(yy, xt, LitConstants.Double.Trig.AT3);
            yy = Fma.MultiplyAdd(yy, xt, LitConstants.Double.Trig.AT2);
            yy = Fma.MultiplyAdd(yy, xt, LitConstants.Double.Trig.ONE);
            yy = Avx.Multiply(yy, Avx.AndNot(LitConstants.Double.Trig.NEGZERO, xx));

            y = Fma.MultiplyAdd(yy, negend, nanend);
        }


        /// <summary>
        /// Computes Sine on 4 doubles
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Sin(double* xx, double* yy)
        {
            var x = Avx.LoadVector256(xx);
            Sin(ref x, ref x);
            Avx.Store(yy, x);
        }


        /// <summary>
        /// Computes Cosine on 4 doubles
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Cos(double* xx, double* yy)
        {
            var x = Avx.Add(Avx.LoadVector256(xx), LitConstants.Double.Trig.HALFPI);
            Sin(ref x, ref x);
            Avx.Store(yy, x);
        }


        /// <summary>
        /// Computes Tangent on 4 doubles
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Tan(double* xx, double* yy)
        {
            // TODO: This for sure isn't the fastest way to do this, but it's still faster than Math.Tan()

            var sin = Avx.LoadVector256(xx);
            var cos = Avx.Add(sin, LitConstants.Double.Trig.HALFPI);
            Sin(ref cos, ref cos);
            Sin(ref sin, ref sin);
            Avx.Store(yy, Avx.Divide(sin, cos));
        }


        /// <summary>
        /// Computes Inverse Tangent on 4 doubles
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ATan(double* xx, double* yy)
        {
            var x = Avx.LoadVector256(xx);
            ATan(ref x, ref x);
            Avx.Store(yy, x);
        }


        /// <summary>
        /// Computes Sine on n doubles
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        public static void Sin(ref Span<double> xx, ref Span<double> yy)
        {
            unsafe
            {
                fixed (double* x = xx) fixed (double* y = yy)
                    Sin(x, y, xx.Length);
            }
        }


        /// <summary>
        /// Computes Cosine on n doubles
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        public static void Cos(ref Span<double> xx, ref Span<double> yy)
        {
            unsafe
            {
                fixed (double* x = xx) fixed (double* y = yy)
                    Cos(x, y, xx.Length);
            }
        }


        /// <summary>
        /// Computes Tangent on n doubles
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        public static void Tan(ref Span<double> xx, ref Span<double> yy)
        {
            unsafe
            {
                fixed (double* x = xx) fixed (double* y = yy)
                    Tan(x, y, xx.Length);
            }
        }


        /// <summary>
        /// Computes Inverse Tangent on n doubles
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        public static void ATan(ref Span<double> xx, ref Span<double> yy)
        {
            unsafe
            {
                fixed (double* x = xx) fixed (double* y = yy)
                    ATan(x, y, xx.Length);
            }
        }


        /// <summary>
        /// Computes Cosine on n doubles
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        /// <param name="n"></param>
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Cos(double* xx, double* yy, int n)
        {
            const int VSZ = 4;

            if (n < VSZ)
            {
                var tmpx = stackalloc double[VSZ];
                for (int j = 0; j < n; j++)
                    tmpx[j] = xx[j];

                Cos(tmpx, tmpx);

                for (int j = 0; j < n; ++j)
                    yy[j] = tmpx[j];
            }

            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - 15))
            {
                Cos(xx + i, yy + i);
                i += VSZ;
                Cos(xx + i, yy + i);
                i += VSZ;
                Cos(xx + i, yy + i);
                i += VSZ;
                Cos(xx + i, yy + i);
                i += VSZ;
            }

            // Calculates the remaining sets of 4 values in a standard loop
            for (; i < (n - 3); i += VSZ)
                Cos(xx + i, yy + i);

            // Cleans up any excess individual values (if n%4 != 0)
            if (i != n)
            {
                i = n - VSZ;
                Cos(xx + i, yy + i);
            }
        }


        /// <summary>
        /// Computes Sine on n doubles
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        /// <param name="n"></param>
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Sin(double* xx, double* yy, int n)
        {
            const int VSZ = 4;

            if (n < VSZ)
            {
                var tmpx = stackalloc double[VSZ];
                for (int j = 0; j < n; j++)
                    tmpx[j] = xx[j];

                Sin(tmpx, tmpx);

                for (int j = 0; j < n; ++j)
                    yy[j] = tmpx[j];
            }

            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - 15))
            {
                Sin(xx + i, yy + i);
                i += VSZ;
                Sin(xx + i, yy + i);
                i += VSZ;
                Sin(xx + i, yy + i);
                i += VSZ;
                Sin(xx + i, yy + i);
                i += VSZ;
            }

            // Calculates the remaining sets of 4 values in a standard loop
            for (; i < (n - 3); i += VSZ)
                Sin(xx + i, yy + i);

            // Cleans up any excess individual values (if n%4 != 0)
            if (i != n)
            {
                i = n - VSZ;
                Sin(xx + i, yy + i);
            }
        }

        /// <summary>
        /// Computes Tangent on n doubles
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        /// <param name="n"></param>
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Tan(double* xx, double* yy, int n)
        {
            const int VSZ = 4;

            if (n < VSZ)
            {
                var tmpx = stackalloc double[VSZ];
                for (int j = 0; j < n; j++)
                    tmpx[j] = xx[j];

                Tan(tmpx, tmpx);

                for (int j = 0; j < n; ++j)
                    yy[j] = tmpx[j];
            }

            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - 15))
            {
                Tan(xx + i, yy + i);
                i += VSZ;
                Tan(xx + i, yy + i);
                i += VSZ;
                Tan(xx + i, yy + i);
                i += VSZ;
                Tan(xx + i, yy + i);
                i += VSZ;
            }

            // Calculates the remaining sets of 4 values in a standard loop
            for (; i < (n - 3); i += VSZ)
                Tan(xx + i, yy + i);

            // Cleans up any excess individual values (if n%4 != 0)
            if (i != n)
            {
                i = n - VSZ;
                Tan(xx + i, yy + i);
            }
        }

        /// <summary>
        /// Computes Inverse Tangent on n doubles
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        /// <param name="n"></param>
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ATan(double* xx, double* yy, int n)
        {
            const int VSZ = 4;

            if (n < VSZ)
            {
                var tmpx = stackalloc double[VSZ];
                for (int j = 0; j < n; j++)
                    tmpx[j] = xx[j];

                ATan(tmpx, tmpx);

                for (int j = 0; j < n; ++j)
                    yy[j] = tmpx[j];
            }

            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - 15))
            {
                ATan(xx + i, yy + i);
                i += VSZ;
                ATan(xx + i, yy + i);
                i += VSZ;
                ATan(xx + i, yy + i);
                i += VSZ;
                ATan(xx + i, yy + i);
                i += VSZ;
            }

            // Calculates the remaining sets of 4 values in a standard loop
            for (; i < (n - 3); i += VSZ)
                ATan(xx + i, yy + i);

            // Cleans up any excess individual values (if n%4 != 0)
            if (i != n)
            {
                i = n - VSZ;
                ATan(xx + i, yy + i);
            }
        }
    }
}
