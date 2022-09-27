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
        /// Calculates n exponentials on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="x">A Span to the first argument</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<double> Exp(ref Span<double> x)
        {
            var y = new Span<double>(GC.AllocateUninitializedArray<double>(x.Length));
            Exp(ref x, ref y);
            return y;
        }


        /// <summary>
        /// Calculates n exponentials on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="x">A Span to the first argument</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<double> Exp(Span<double> x)
        {
            return Exp(ref x);
        }


        /// <summary>
        /// Calculates n exponentials on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="x">A Span to the first argument</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<float> Exp(ref Span<float> x)
        {
            var y = new Span<float>(GC.AllocateUninitializedArray<float>(x.Length));
            Exp(ref x, ref y);
            return y;
        }


        /// <summary>
        /// Calculates n exponentials on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="x">A Span to the first argument</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<float> Exp(Span<float> x)
        {
            return Exp(ref x);

        }

        /// <summary>
        /// Calculates n exponentials on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="xx">A pointer to the first argument</param>
        /// <param name="yy">The return values</param>
        /// <param name="n">The number of xx values to take an exponential of</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Exp(ref Span<double> xx, ref Span<double> yy)
        {
            const int VSZ = 4;
            var n = xx.Length;
            ref var x = ref MemoryMarshal.GetReference(xx);
            ref var y = ref MemoryMarshal.GetReference(yy);

            // if n < 4, then we handle the special case by creating a 4 element array to work with
            if (n < VSZ)
            {
                Span<double> tmp = stackalloc double[VSZ];
                ref var tmpx = ref MemoryMarshal.GetReference(tmp);
                for (int j = 0; j < n; j++)
                    Unsafe.Add(ref tmpx, j) = Unsafe.Add(ref x, j);

                Exp(ref tmpx, ref tmpx, 0);

                for (int j = 0; j < n; ++j)
                    Unsafe.Add(ref y, j) = Unsafe.Add(ref tmpx, j);

                return;
            }

            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - 15))
            {
                Exp(ref x, ref y, i);
                i += VSZ;
                Exp(ref x, ref y, i);
                i += VSZ;
                Exp(ref x, ref y, i);
                i += VSZ;
                Exp(ref x, ref y, i);
                i += VSZ;
            }

            // Calculates the remaining sets of 4 values in a standard loop
            for (; i < (n - 3); i += VSZ)
                Exp(ref x, ref y, i);

            // Cleans up any excess individual values (if n%4 != 0)
            if (i != n)
            {
                i = n - VSZ;
                Exp(ref x, ref y, i);
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
        public static void Exp(ref Span<float> xx, ref Span<float> yy)
        {
            const int VSZ = 8;
            var n = xx.Length;
            ref var x = ref MemoryMarshal.GetReference(xx);
            ref var y = ref MemoryMarshal.GetReference(yy);

            // if n < 8, then we handle the special case by creating a 4 element array to work with
            if (n < VSZ)
            {
                Span<float> tmp = stackalloc float[VSZ];
                ref var tmpx = ref MemoryMarshal.GetReference(tmp);
                for (int j = 0; j < n; j++)
                    Unsafe.Add(ref tmpx, j) = Unsafe.Add(ref x, j);

                Exp(ref tmpx, ref tmpx, 0);

                for (int j = 0; j < n; ++j)
                    Unsafe.Add(ref y, j) = Unsafe.Add(ref tmpx, j);

                return;
            }

            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - 31))
            {
                Exp(ref x, ref y, i);
                i += VSZ;
                Exp(ref x, ref y, i);
                i += VSZ;
                Exp(ref x, ref y, i);
                i += VSZ;
                Exp(ref x, ref y, i);
                i += VSZ;
            }

            // Calculates the remaining sets of 8 values in a standard loop
            for (; i < (n - 7); i += VSZ)
                Exp(ref x, ref y, i);

            // Cleans up any excess individual values (if n%8 != 0)
            if (i != n)
            {
                i = n - VSZ;
                Exp(ref x, ref y, i);
            }
        }


        /// <summary>
        /// Calculates 4 exponentials on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="xx">A pointer to the first of 4 arguments</param>
        /// <param name="yy">The return values</param>
        /// <param name="index">The index offset of the operation starts on</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Exp(ref double xx, ref double yy, int index)
        {
            // Instead of calculating e^x directly, calculate 2^(x / ln(2))
            var x = Util.LoadV256(ref xx, index);
            x = Avx.Multiply(x, Double.Exp.LOG2EF);
            Two(ref x, ref x);
            Util.StoreV256(ref yy, index, x);
        }


        /// <summary>
        /// Calculates 8 exponentials on floats via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="xx">A pointer to the first of 8 arguments</param>
        /// <param name="yy">The return values</param>
        /// <param name="index">The index offset of the operation starts on</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Exp(ref float xx, ref float yy, int index)
        {
            // Instead of calculating e^x directly, calculate 2^(x / ln(2))
            var x = Util.LoadV256(ref xx, index);
            x = Avx.Multiply(x, Float.Exp.LOG2EF);
            Two(ref x, ref x);
            Util.StoreV256(ref yy, index, x);
        }


        /// <summary>
        /// Calculates 4 exponentials on doubles via 256-bit SIMD intrinsics.
        /// </summary>
        /// <param name="x">A reference to the 4 arguments</param>
        /// <param name="y">The 4 results</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Exp(ref Vector256<double> x, ref Vector256<double> y)
        {
            var xx = Avx.Multiply(x, Double.Exp.LOG2EF);
            Two(ref xx, ref y);
        }


        /// <summary>
        /// Calculates 4 exponentials on doubles via 256-bit SIMD intrinsics.
        /// </summary>
        /// <param name="x">A reference to the 4 arguments</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Exp(Vector256<double> x)
        {
            var xx = Avx.Multiply(x, Double.Exp.LOG2EF);
            Two(ref xx, ref xx);
            return xx;
        }


        /// <summary>
        /// Calculates 4 exponentials on doubles via 256-bit SIMD intrinsics.
        /// </summary>
        /// <param name="x">A reference to the 4 arguments</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Exp(ref Vector256<double> x)
        {
            var xx = Avx.Multiply(x, Double.Exp.LOG2EF);
            Two(ref xx, ref xx);
            return xx;
        }


        /// <summary>
        /// Calculates 4 exponentials on doubles via 256-bit SIMD intrinsics.
        /// </summary>
        /// <param name="x">A reference to the 4 arguments</param>
        /// <param name="y">The 4 results</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Exp(ref Vector256<float> x, ref Vector256<float> y)
        {
            var xx = Avx.Multiply(x, Float.Exp.LOG2EF);
            Two(ref xx, ref y);
        }


        /// <summary>
        /// Calculates 4 exponentials on doubles via 256-bit SIMD intrinsics.
        /// </summary>
        /// <param name="x">A reference to the 4 arguments</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<float> Exp(Vector256<float> x)
        {
            var xx = Avx.Multiply(x, Float.Exp.LOG2EF);
            Two(ref xx, ref xx);
            return xx;
        }


        /// <summary>
        /// Calculates 4 exponentials on doubles via 256-bit SIMD intrinsics.
        /// </summary>
        /// <param name="x">A reference to the 4 arguments</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<float> Exp(ref Vector256<float> x)
        {
            var xx = Avx.Multiply(x, Float.Exp.LOG2EF);
            Two(ref xx, ref xx);
            return xx;
        }


        /// <summary>
        /// Calculates 4 2^x exponentials on doubles via 256-bit SIMD intrinsics.
        /// </summary>
        /// <param name="x">A reference to the 4 arguments</param>
        /// <param name="y">The 4 results</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Two(ref Vector256<double> x, ref Vector256<double> y)
        {
            // Checks if x is greater than the highest acceptable argument. Stores the information for later to
            // modify the result. If, for example, only x[1] > EXP_HIGH, then end[1] will be infinity, and the rest
            // zero. We add this to the result at the end, which will force y[1] to be infinity.
            var end = Avx.And(Avx.CompareGreaterThanOrEqual(x, Double.Exp.HIGH), Double.Exp.POSITIVE_INFINITY);

            // Bound x by the maximum and minimum values this algorithm will handle.
            var xx = Avx.Max(Avx.Min(x, Double.Exp.THIGH), Double.Exp.TLOW);

            // Avx.CompareNotEqual(x, x) is a hack to determine which values of x are NaN, since NaN is the only
            // value that doesn't equal itself. If any are NaN, we make the corresponding element of 'end' NaN, and
            // it acts like the infinity adjustment.
            end = Avx.Add(Avx.CompareNotEqual(x, x), end);

            var fx = Avx.RoundToNearestInteger(xx);

            // This section gets a series approximation for exp(g) in (-0.5, 0.5) since that is g's range.
            xx = Avx.Subtract(xx, fx);
            y = Fma.MultiplyAdd(Double.Exp.T11, xx, Double.Exp.T10);
            y = Fma.MultiplyAdd(y, xx, Double.Exp.T9);
            y = Fma.MultiplyAdd(y, xx, Double.Exp.T8);
            y = Fma.MultiplyAdd(y, xx, Double.Exp.T7);
            y = Fma.MultiplyAdd(y, xx, Double.Exp.T6);
            y = Fma.MultiplyAdd(y, xx, Double.Exp.T5);
            y = Fma.MultiplyAdd(y, xx, Double.Exp.T4);
            y = Fma.MultiplyAdd(y, xx, Double.Exp.T3);
            y = Fma.MultiplyAdd(y, xx, Double.Exp.T2);
            y = Fma.MultiplyAdd(y, xx, Double.Exp.T1);
            y = Fma.MultiplyAdd(y, xx, Double.Exp.T0);

            // Converts n to 2^n. There is no Avx2.ConvertToVector256Int64(fx) intrinsic, so we convert to int32's,
            // since the exponent of a double will never be more than a max int32, then from int to long.
            fx = Avx.Add(fx, Double.Exp.MAGIC_LONG_DOUBLE_ADD);
            fx = Avx2.ShiftLeftLogical(Avx2.Add(Vector256.AsInt64(fx), Lit.Long.ONE_THOUSAND_TWENTY_THREE), 52).AsDouble();

            // Combines the two exponentials and the end adjustments into the result.
            y = Fma.MultiplyAdd(y, fx, end);
        }


        /// <summary>
        /// Calculates 4 2^x exponentials on doubles via 256-bit SIMD intrinsics.
        /// </summary>
        /// <param name="x">A reference to the 4 arguments</param>
        /// <param name="y">The 4 results</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Two(ref Vector256<float> x, ref Vector256<float> y)
        {
            // Checks if x is greater than the highest acceptable argument. Stores the information for later to
            // modify the result. If, for example, only x[1] > EXP_HIGH, then end[1] will be infinity, and the rest
            // zero. We add this to the result at the end, which will force y[1] to be infinity.
            var end = Avx.And(Avx.CompareGreaterThanOrEqual(x, Float.Exp.HIGH), Float.Exp.POSITIVE_INFINITY);

            // Bound x by the maximum and minimum values this algorithm will handle.
            var xx = Avx.Max(Avx.Min(x, Float.Exp.THIGH), Float.Exp.TLOW);

            // Avx.CompareNotEqual(x, x) is a hack to determine which values of x are NaN, since NaN is the only
            // value that doesn't equal itself. If any are NaN, we make the corresponding element of 'end' NaN, and
            // it acts like the infinity adjustment.
            end = Avx.Add(Avx.CompareNotEqual(x, x), end);

            var fx = Avx.RoundToNearestInteger(xx);

            // This section gets a series approximation for exp(g) in (-0.5, 0.5) since that is g's range.
            xx = Avx.Subtract(xx, fx);
            y = Fma.MultiplyAdd(Float.Exp.T7, xx, Float.Exp.T6);
            y = Fma.MultiplyAdd(y, xx, Float.Exp.T5);
            y = Fma.MultiplyAdd(y, xx, Float.Exp.T4);
            y = Fma.MultiplyAdd(y, xx, Float.Exp.T3);
            y = Fma.MultiplyAdd(y, xx, Float.Exp.T2);
            y = Fma.MultiplyAdd(y, xx, Float.Exp.T1);
            y = Fma.MultiplyAdd(y, xx, Float.Exp.T0);

            // Converts n to 2^n. There is no Avx2.ConvertToVector256Int64(fx) intrinsic, so we convert to int32's,
            // since the exponent of a double will never be more than a max int32, then from int to long.
            fx = Vector256.AsSingle(Avx2.ShiftLeftLogical(Avx2.Add(Avx2.ConvertToVector256Int32(fx), Lit.Int.ONE_HUNDRED_TWENTY_SEVEN), 23));

            // Combines the two exponentials and the end adjustments into the result.
            y = Fma.MultiplyAdd(y, fx, end);
        }
    }
}
