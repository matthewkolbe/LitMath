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

            // This is a Taylor series approximation of sin() on [0, pi/2] centered at pi/4. Centereing at pi/4 actually happened on
            // the previous step where we called "Avx.Subtract(Constants.Double.QUARTERPI". That should have been Avx.Subtract(Constants.Double.HALFPI,
            // but to save computation it was rolled into a single operation.
            y = Vector256.Create(-7.64716373181981647590113198578807E-13);

            y = Avx.Multiply(y, xt);
            y = Avx.Add(LitConstants.Double.Trig.P14, y);
            y = Avx.Multiply(y, xt);
            y = Avx.Add(LitConstants.Double.Trig.P13, y);
            y = Avx.Multiply(y, xt);
            y = Avx.Add(LitConstants.Double.Trig.P12, y);
            y = Avx.Multiply(y, xt);
            y = Avx.Add(LitConstants.Double.Trig.P11, y);
            y = Avx.Multiply(y, xt);
            y = Avx.Add(LitConstants.Double.Trig.P10, y);
            y = Avx.Multiply(y, xt);
            y = Avx.Add(LitConstants.Double.Trig.P9, y);
            y = Avx.Multiply(y, xt);
            y = Avx.Add(LitConstants.Double.Trig.P8, y);
            y = Avx.Multiply(y, xt);
            y = Avx.Add(LitConstants.Double.Trig.P7, y);
            y = Avx.Multiply(y, xt);
            y = Avx.Add(LitConstants.Double.Trig.P6, y);
            y = Avx.Multiply(y, xt);
            y = Avx.Add(LitConstants.Double.Trig.P5, y);
            y = Avx.Multiply(y, xt);
            y = Avx.Add(LitConstants.Double.Trig.P4, y);
            y = Avx.Multiply(y, xt);
            y = Avx.Add(LitConstants.Double.Trig.P3, y);
            y = Avx.Multiply(y, xt);
            y = Avx.Add(LitConstants.Double.Trig.NEGHALF, y);
            y = Avx.Multiply(y, xt);
            y = Avx.Add(LitConstants.Double.Trig.ONE, y);
            y = Avx.Multiply(y, xt);
            y = Avx.Add(LitConstants.Double.Trig.ONE, y);

            y = Avx.Multiply(y, LitConstants.Double.Trig.SIN_OF_QUARTERPI);

            
            y = Avx.Multiply(y, negend);
            y = Avx.Add(y, nanend);
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
        /// Computes Cosine on n doubles
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        /// <param name="n"></param>
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Cos(double* xx, double* yy, int n)
        {
            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            for (; i < (n - 15); i += 16)
            {
                Cos(xx + i, yy + i);
                Cos(xx + i + 4, yy + i + 4);
                Cos(xx + i + 8, yy + i + 8);
                Cos(xx + i + 12, yy + i + 12);
            }

            // Calculates the remaining sets of 4 values in a standard loop
            for (; i < (n - 3); i += 4)
                Cos(xx + i, yy + i);

            // Gets the last index we left off at 
            var nn = n & LitConstants.Int.MAX_MINUS_THREE;

            // Cleans up any excess individual values (if n%4 != 0)
            if (nn != n)
            {
                var tmpx = stackalloc double[4];

                for (int j = 0; j < 4; ++j)
                    tmpx[j] = xx[j + i];
                
                Cos(xx + i, tmpx);

                for (; i < n; ++i)
                    yy[i] = tmpx[i - nn];
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
            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            for (; i < (n - 15); i += 16)
            {
                Sin(xx + i, yy + i);
                Sin(xx + i + 4, yy + i + 4);
                Sin(xx + i + 8, yy + i + 8);
                Sin(xx + i + 12, yy + i + 12);
            }

            // Calculates the remaining sets of 4 values in a standard loop
            for (; i < (n - 3); i += 4)
                Sin(xx + i, yy + i);

            // Gets the last index we left off at 
            var nn = n & LitConstants.Int.MAX_MINUS_THREE;

            // Cleans up any excess individual values (if n%4 != 0)
            if (nn != n)
            {
                var tmpx = stackalloc double[4];

                for (int j = 0; j < 4; ++j)
                    tmpx[j] = xx[j + i];

                Sin(tmpx, tmpx);

                for (; i < n; ++i)
                    yy[i] = tmpx[i - nn];
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
            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            for (; i < (n - 15); i += 16)
            {
                Tan(xx + i, yy + i);
                Tan(xx + i + 4, yy + i + 4);
                Tan(xx + i + 8, yy + i + 8);
                Tan(xx + i + 12, yy + i + 12);
            }

            // Calculates the remaining sets of 4 values in a standard loop
            for (; i < (n - 3); i += 4)
                Tan(xx + i, yy + i);

            // Gets the last index we left off at 
            var nn = n & LitConstants.Int.MAX_MINUS_THREE;

            // Cleans up any excess individual values (if n%4 != 0)
            if (nn != n)
            {
                var tmpx = stackalloc double[4];

                for (int j = 0; j < 4; ++j)
                    tmpx[j] = xx[j + i];

                Tan(xx + i, tmpx);

                for (; i < n; ++i)
                    yy[i] = tmpx[i - nn];
            }
        }
    }
}
