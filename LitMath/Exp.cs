// Copyright Matthew Kolbe (2022)

using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace LitMath
{
    public static class LitExp
    {
        /// <summary>
        /// Calculates n exponentials on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="x">A Span to the first argument</param>
        /// <param name="y">The return values</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void E(ref Span<double> x, ref Span<double> y)
        {
            unsafe
            {
                fixed (double* xx = x) fixed (double* yy = y)
                    E(xx, yy, x.Length);
            }
        }


        /// <summary>
        /// Calculates n exponentials on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="x">A Span to the first argument</param>
        /// <param name="y">The return values</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void E(ref Span<float> x, ref Span<float> y)
        {
            unsafe
            {
                fixed (float* xx = x) fixed (float* yy = y)
                    E(xx, yy, x.Length);
            }
        }


        /// <summary>
        /// Calculates n exponentials on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="xx">A pointer to the first argument</param>
        /// <param name="yy">The return values</param>
        /// <param name="n">The number of xx values to take an exponential of</param>
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void E(double* xx, double* yy, int n)
        {
            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - 15))
            {
                E(xx + i, yy + i);
                i += 4;
                E(xx + i, yy + i);
                i += 4;
                E(xx + i, yy + i);
                i += 4;
                E(xx + i, yy + i);
                i += 4;
            }

            // Calculautes the remaining sets of 4 values in a standard loop
            for (; i < (n - 3); i += 4)
                E(xx + i, yy + i);

            // Gets the last index we left off at 
            var nn = n & LitConstants.Int.MAX_MINUS_THREE;
            
            // Cleans up any excess individual values (if n%4 != 0)
            if (nn != n)
            {
                var tmpx = stackalloc double[4];

                for (int j = 0; j < 4; ++j)
                    tmpx[j] = xx[j+i];

                var x = Avx.LoadVector256(tmpx);
                E(ref x, ref x);

                for (; i < n; ++i)
                    yy[i] = x.GetElement(i - nn);
            }
        }


        /// <summary>
        /// Calculates n exponentials on floats via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="xx">A pointer to the first argument</param>
        /// <param name="yy">The return values</param>
        /// <param name="n">The number of xx values to take an exponential of</param>
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void E(float* xx, float* yy, int n)
        {
            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - 31))
            {
                E(xx + i, yy + i);
                i += 8;
                E(xx + i, yy + i);
                i += 8;
                E(xx + i, yy + i);
                i += 8;
                E(xx + i, yy + i);
                i += 8;
            }

            // Calculautes the remaining sets of 8 values in a standard loop
            for (; i < (n - 7); i += 8)
                E(xx + i, yy + i);

            // Cleans up any excess individual values (if n%8 != 0)
            if (i != n)
            {
                var nn = i;
                var tmpx = stackalloc float[8];

                for (int j = 0; j < 8; ++j)
                    tmpx[j] = xx[j + i];

                var x = Avx.LoadVector256(tmpx);
                E(ref x, ref x);

                for (; i < n; ++i)
                    yy[i] = x.GetElement(i - nn);
            }
        }


        /// <summary>
        /// Calculates 4 exponentials on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="xx">A pointer to the first of 4 arguments</param>
        /// <param name="yy">The return values/param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void E(double* xx, double* yy)
        {
            var x = Avx.LoadVector256(xx);
            E(ref x, ref x);
            Avx.Store(yy, x);
        }


        /// <summary>
        /// Calculates 8 exponentials on floats via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="xx">A pointer to the first of 8 arguments</param>
        /// <param name="yy">The return values</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void E(float* xx, float* yy)
        {
            var x = Avx.LoadVector256(xx);
            E(ref x, ref x);
            Avx.Store(yy, x);
        }


        /// <summary>
        /// Calculates 4 exponentials on doubles via 256-bit SIMD intrinsics.
        /// </summary>
        /// <param name="x">A reference to the 4 arguments</param>
        /// <param name="y">The 4 results</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void E(ref Vector256<double> x, ref Vector256<double> y)
        {
            // Checks if x is greater than the highest acceptable argument. Stores the information for later to
            // modify the result. If, for example, only x[1] > EXP_HIGH, then end[1] will be infinity, and the rest
            // zero. We add this to the result at the end, which will force y[1] to be infinity.
            var end = Avx.And(Avx.CompareGreaterThanOrEqual(x, LitConstants.Double.Exp.HIGH), LitConstants.Double.Exp.POSITIVE_INFINITY);

            // Avx.CompareNotEqual(x, x) is a hack to determine which values of x are NaN, since NaN is the only
            // value that doesn't equal itself. If any are NaN, we make the corresponding element of 'end' NaN, and
            // it acts like the infinity adjustment.
            end = Avx.Add(Avx.CompareNotEqual(x, x), end);

            // Bound x by the maximum and minimum values this algorithm will handle.
            var xx = Avx.Min(x, LitConstants.Double.Exp.HIGH);
            xx = Avx.Max(xx, LitConstants.Double.Exp.LOW);

            // Expresses exp(x) as exp(g)*exp(n*log(2)) = exp(g) * 2^n. Variable names do not match, since I reuse
            // variables to save allocations.
            var fx = Avx.Multiply(xx, LitConstants.Double.Exp.LOG2EF);

            // This is n
            fx = Avx.RoundToNearestInteger(fx);

            // This section gets a series approximation for exp(g) in (1, 2) since that is g's range.
            y = Vector256.Create(1.9875691500E-4);
            var z = Avx.Multiply(fx, LitConstants.Double.Exp.INVERSE_LOG2EF);
            xx = Avx.Subtract(xx, z);
            z = Avx.Multiply(xx, xx);
            y = Avx.Multiply(y, xx);
            y = Avx.Add(y, LitConstants.Double.Exp.P1);
            y = Avx.Multiply(y, xx);
            y = Avx.Add(y, LitConstants.Double.Exp.P2);
            y = Avx.Multiply(y, xx);
            y = Avx.Add(y, LitConstants.Double.Exp.P3);
            y = Avx.Multiply(y, xx);
            y = Avx.Add(y, LitConstants.Double.Exp.P4);
            y = Avx.Multiply(y, xx);
            y = Avx.Add(y, LitConstants.Double.Exp.P5);
            y = Avx.Multiply(y, z);
            y = Avx.Add(y, xx);
            y = Avx.Add(y, LitConstants.Double.Exp.ONE);

            // Converts n to 2^n. There is no Avx2.ConvertToVector256Int64(fx) intrinsic, so we convert to int's,
            // since the exponent of a double will never be more than a max int, then from int to long.
            fx = Avx2.Add(fx, LitConstants.Double.Exp.MAGIC_LONG_DOUBLE_ADD);
            var fxlong = Vector256.AsInt64(fx);
            fxlong = Avx2.Add(fxlong, LitConstants.Long.ONE_THOUSAND_TWENTY_THREE);
            fxlong = Avx2.ShiftLeftLogical(fxlong, 52);
            fx = Vector256.AsDouble(fxlong);

            // Combines the two exponentials and the end adjustments into the result.
            y = Avx.Multiply(y, fx);
            y = Avx.Add(y, end);
        }


        /// <summary>
        /// Calculates 8 exponentials on floats via 256-bit SIMD intrinsics.
        /// </summary>
        /// <param name="x">A reference to the 8 arguments</param>
        /// <param name="y">The 8 results</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void E(ref Vector256<float> x, ref Vector256<float> y)
        {
            

            // Checks if x is greater than the highest acceptable argument. Stores the information for later to
            // modify the result. If, for example, only x[1] > EXP_HIGH, then end[1] will be infinity, and the rest
            // zero. We add this to the result at the end, which will force y[1] to be infinity.
            var end = Avx.And(Avx.CompareGreaterThanOrEqual(x, LitConstants.Float.Exp.HIGH), LitConstants.Float.Exp.POSITIVE_INFINITY);

            // Avx.CompareNotEqual(x, x) is a hack to determine which values of x are NaN, since NaN is the only
            // value that doesn't equal itself. If any are NaN, we make the corresponding element of 'end' NaN, and
            // it acts like the infinity adjustment.
            end = Avx.Add(Avx.CompareNotEqual(x, x), end);

            // Bound x by the maximum and minimum values this algorithm will handle.
            var xx = Avx.Min(x, LitConstants.Float.Exp.HIGH);
            xx = Avx.Max(xx, LitConstants.Float.Exp.LOW);

            // Expresses exp(x) as exp(g)*exp(n*log(2)) = exp(g) * 2^n. Variable names do not match, since I reuse
            // variables to save allocations.
            var fx = Avx.Multiply(xx, LitConstants.Float.Exp.LOG2EF);
            
            // This is n
            fx = Avx.RoundToNearestInteger(fx);

            // This section gets a series approximation for exp(g) in (1, 2) since that is g's range.
            var z = Avx.Multiply(fx, LitConstants.Float.Exp.INVERSE_LOG2EF);
            xx = Avx.Subtract(xx, z);
            z = Avx.Multiply(xx, xx);
            y = Vector256.Create(1.9875691500E-4f);
            y = Avx.Multiply(y, xx);
            y = Avx.Add(y, LitConstants.Float.Exp.P1);
            y = Avx.Multiply(y, xx);
            y = Avx.Add(y, LitConstants.Float.Exp.P2);
            y = Avx.Multiply(y, xx);
            y = Avx.Add(y, LitConstants.Float.Exp.P3);
            y = Avx.Multiply(y, xx);
            y = Avx.Add(y, LitConstants.Float.Exp.P4);
            y = Avx.Multiply(y, xx);
            y = Avx.Add(y, LitConstants.Float.Exp.P5);
            y = Avx.Multiply(y, z);
            y = Avx.Add(y, xx);
            y = Avx.Add(y, LitConstants.Float.Exp.ONE);

            // Converts n to 2^n
            var fxint = Avx2.ConvertToVector256Int32(fx);
            fxint = Avx2.Add(fxint, LitConstants.Int.ONE_HUNDRED_TWENTY_SEVEN);
            fxint = Avx2.ShiftLeftLogical(fxint, 23);
            fx = Vector256.AsSingle(fxint);

            // Combines the two exponentials and the end adjustments into the result.
            y = Avx.Add(Avx.Multiply(y, fx), end);
        }
    }
}
