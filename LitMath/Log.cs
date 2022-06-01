// Copyright Matthew Kolbe (2022)

using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace LitMath
{

    public static class LitLog
    {
        /// <summary>
        /// Calculates 4 log base 2's on doubles via 256-bit SIMD intriniscs.
        /// </summary>
        /// <param name="x">A reference to the 4 arguemnts</param>
        /// <param name="y">The 4 results</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log2(ref Vector256<double> x, ref Vector256<double> y)
        {
            // Checks if x is lower than zero. Stores the information for later to modify the result. If, for
            // example, only x[1] < 0, then end[1] will be NaN, and the rest zero. We add this to the result at
            // the end, which will force y[1] to be NaN.
            var end = Avx.CompareLessThanOrEqual(x, LitConstants.Double.Log.ZERO);

            // Handles positive infinity as a special case where log2(infinity)=infinity. Uses the same trick at
            // the end.
            end = Avx.Add(Avx.And(Avx.CompareEqual(x, LitConstants.Double.Log.POSITIVE_INFINITY), LitConstants.Double.Log.POSITIVE_INFINITY), end);

            // Avx.CompareNotEqual(x, x) is a hack to determine which values of x are NaN, since NaN is the only
            // value that doesn't equal itself. If any are NaN, we make the corresponding element of 'end' NaN, and
            // it acts like the infinity adjustment.
            end = Avx.Add(Avx.CompareNotEqual(x, x), end);

            // This algorithm uses the properties of floating point number to transform x into d*2^m, so log(x)
            // becomes log(d)+m, where d is in [1, 2]. Then it uses a series approximation of log to approximate 
            // the value in [1, 2]

            var xl = Vector256.AsInt64(Avx.Max(x, LitConstants.Double.Log.ZERO));
            var mantissa = Avx2.Subtract(Avx2.ShiftRightLogical(xl, 52), LitConstants.Long.ONE_THOUSAND_TWENTY_THREE);

            LitUtilities.ConvertLongToDouble(ref mantissa, ref y);

            xl = Avx2.Or(Avx2.And(xl, LitConstants.Long.DECIMAL_MASK_FOR_DOUBLE), LitConstants.Long.EXPONENT_MASK_FOR_DOUBLE);

            var d = Avx.Multiply(Avx.Or(Vector256.AsDouble(xl), LitConstants.Double.Log.ONE), LitConstants.Double.Log.TWO_THIRDS);

            LogApprox(ref d, ref d);

            y = Avx.Add(Avx.Add(Avx.Multiply(d, LitConstants.Double.Log.LOG2EF), LitConstants.Double.Log.LOG_ONE_POINT_FIVE), y);
            y = Avx.Add(end, y);
        }


        /// <summary>
        /// Calculates 8 log base 2's on doubles via 256-bit SIMD intriniscs.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log2(ref Vector256<float> x, ref Vector256<float> y)
        {
            var end = Avx.CompareLessThanOrEqual(x, LitConstants.Float.Log.ZERO);
            end = Avx.Add(Avx.And(Avx.CompareEqual(x, LitConstants.Float.Log.POSITIVE_INFINITY), LitConstants.Float.Log.POSITIVE_INFINITY), end);
            end = Avx.Add(Avx.CompareNotEqual(x, x), end);

            var xl = Vector256.AsInt32(Avx.Max(x, LitConstants.Float.Log.ZERO));
            var m = Avx2.Subtract(Avx2.ShiftRightLogical(xl, 23), LitConstants.Int.ONE_HUNDRED_TWENTY_SEVEN);

            y = Avx.ConvertToVector256Single(m);

            xl = Avx2.Or(Avx2.And(xl, LitConstants.Int.DECIMAL_MASK_FOR_FLOAT), LitConstants.Int.EXPONENT_MASK_FOR_FLOAT);

            var d = Avx.Multiply(Avx.Or(Vector256.AsSingle(xl), LitConstants.Float.Log.ONE), LitConstants.Float.Log.TWO_THIRDS);

            LogApprox(ref d, ref d);

            y = Avx.Add(Avx.Add(Avx.Multiply(d, LitConstants.Float.Log.LOG2EF), LitConstants.Float.Log.LOG_ONE_POINT_FIVE), y);
            y = Avx.Add(end, y);
        }

        
        /// <summary>
        /// A Taylor Series approximation of ln(x) that relies on the identity that ln(x) = 2*atan((x-1)/x(+1)).
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogApprox(ref Vector256<double> x, ref Vector256<double> y)
        {
            y = Avx.Divide(Avx.Subtract(x, LitConstants.Double.Log.ONE), Avx.Add(x, LitConstants.Double.Log.ONE));
            var ysq = Avx.Multiply(y, y);

            var rx = Avx.Multiply(ysq, LitConstants.Double.Log.ONE_THIRTEENTH);
            rx = Avx.Add(rx, LitConstants.Double.Log.ONE_ELEVENTH);
            rx = Avx.Multiply(ysq, rx);
            rx = Avx.Add(rx, LitConstants.Double.Log.ONE_NINTH);
            rx = Avx.Multiply(ysq, rx);
            rx = Avx.Add(rx, LitConstants.Double.Log.ONE_SEVENTH);
            rx = Avx.Multiply(ysq, rx);
            rx = Avx.Add(rx, LitConstants.Double.Log.ONE_FIFTH);
            rx = Avx.Multiply(ysq, rx);
            rx = Avx.Add(rx, LitConstants.Double.Log.ONE_THIRD);
            rx = Avx.Multiply(ysq, rx);
            rx = Avx.Add(rx, LitConstants.Double.Log.ONE);
            rx = Avx.Multiply(y, rx);
            y = Avx.Multiply(rx, LitConstants.Double.Log.TWO);
        }


        /// <summary>
        /// A Taylor Series approximation of ln(x) that relies on the identity that ln(x) = 2*atan((x-1)/x(+1)).
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void LogApprox(ref Vector256<float> x, ref Vector256<float> y)
        {
            y = Avx.Divide(Avx.Subtract(x, LitConstants.Float.Log.ONE), Avx.Add(x, LitConstants.Float.Log.ONE));
            var ysq = Avx.Multiply(y, y);

            var rx = Avx.Multiply(ysq, LitConstants.Float.Log.ONE_ELEVENTH);
            rx = Avx.Add(rx, LitConstants.Float.Log.ONE_NINTH);
            rx = Avx.Multiply(ysq, rx);
            rx = Avx.Add(rx, LitConstants.Float.Log.ONE_SEVENTH);
            rx = Avx.Multiply(ysq, rx);
            rx = Avx.Add(rx, LitConstants.Float.Log.ONE_FIFTH);
            rx = Avx.Multiply(ysq, rx);
            rx = Avx.Add(rx, LitConstants.Float.Log.ONE_THIRD);
            rx = Avx.Multiply(ysq, rx);
            rx = Avx.Add(rx, LitConstants.Float.Log.ONE);
            rx = Avx.Multiply(y, rx);
            y = Avx.Multiply(rx, LitConstants.Float.Log.TWO);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ln(ref Vector256<double> x, ref Vector256<double> y)
        {
            Log2(ref x, ref y);
            y = Avx.Multiply(LitConstants.Double.Log.LN2, y);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ln(ref Vector256<float> x, ref Vector256<float> y)
        {
            Log2(ref x, ref y);
            y = Avx.Multiply(LitConstants.Float.Log.LN2, y);
        }


        /// <summary>
        /// Calculates the natural log of 4 doubles
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Ln(double* xx, double* yy)
        {
            var x = Avx.LoadVector256(xx);
            Ln(ref x, ref x);
            Avx.Store(yy, x);
        }


        /// <summary>
        /// Calculates n natrural logs on doubles via 256-bit SIMD intriniscs. 
        /// </summary>
        /// <param name="x">A Span to the first argument</param>
        /// <param name="y">The return values</param>
        /// <param name="n">The number of xx values to take an natural log of</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ln(ref Span<double> x, ref Span<double> y)
        {
            unsafe
            {
                fixed (double* xx = x) fixed (double* yy = y)
                    Ln(xx, yy, x.Length);
            }
        }


        /// <summary>
        /// Calculates n natrural logs on doubles via 256-bit SIMD intriniscs. 
        /// </summary>
        /// <param name="x">A Span to the first argument</param>
        /// <param name="y">The return values</param>
        /// <param name="n">The number of xx values to take an natural log of</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ln(ref Span<float> x, ref Span<float> y, int n)
        {
            unsafe
            {
                fixed (float* xx = x) fixed (float* yy = y)
                    Ln(xx, yy, n);
            }
        }


        /// <summary>
        /// Calculates n natrural logs on doubles via 256-bit SIMD intriniscs. 
        /// </summary>
        /// <param name="xx">A pointer to the first argument</param>
        /// <param name="yy">The return values</param>
        /// <param name="n">The number of xx values to take an natural log of</param>
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Ln(double* xx, double* yy, int n)
        {
            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - 15))
            {
                Ln(xx + i, yy + i);
                i += 4;
                Ln(xx + i, yy + i);
                i += 4;
                Ln(xx + i, yy + i);
                i += 4;
                Ln(xx + i, yy + i);
                i += 4;
            }

            // Calculautes the remaining sets of 4 values in a standard loop
            for (; i < (n - 3); i += 4)
                Ln(xx + i, yy + i);


            var nn = n & LitConstants.Int.MAX_MINUS_THREE;

            // Cleans up any excess individual values (if n%4 != 0)
            if (nn != n)
            {
                var tmpx = stackalloc double[4];

                for (int j = 0; j < 4; ++j)
                    tmpx[j] = xx[j + i];

                var x = Avx.LoadVector256(tmpx);

                Ln(ref x, ref x);

                for (; i < n; ++i)
                    yy[i] = x.GetElement(i - nn);
            }
        }


        /// <summary>
        /// Calculates 4 natrural logs on doubles via 256-bit SIMD intriniscs. 
        /// </summary>
        /// <param name="xx">A pointer to the first argument</param>
        /// <param name="yy">The return values</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Ln(float* xx, float* yy)
        {
            var x = Avx.LoadVector256(xx);
            Ln(ref x, ref x);
            Avx.Store(yy, x);
        }


        /// <summary>
        /// Calculates n natrural logs on doubles via 256-bit SIMD intriniscs. 
        /// </summary>
        /// <param name="xx">A pointer to the first argument</param>
        /// <param name="yy">The return values</param>
        /// <param name="n">The number of xx values to take an natural log of</param>
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Ln(float* xx, float* yy, int n)
        {
            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - 31))
            {
                Ln(xx + i, yy + i);
                i += 8;
                Ln(xx + i, yy + i);
                i += 8;
                Ln(xx + i, yy + i);
                i += 8;
                Ln(xx + i, yy + i);
                i += 8;
            }

            // Calculautes the remaining sets of 8 values in a standard loop
            for (; i < (n - 7); i += 8)
                Ln(xx + i, yy + i);

            // Cleans up any excess individual values (if n%8 != 0)
            if (i != n)
            {
                var nn = i;
                var tmpx = stackalloc float[8];

                for (int j = 0; j < 8; ++j)
                    tmpx[j] = xx[j + i];

                var x = Avx.LoadVector256(tmpx);
                Ln(ref x, ref x);

                for (; i < n; ++i)
                    yy[i] = x.GetElement(i - nn);
            }
        }
    }
}
