﻿// Copyright Matthew Kolbe (2022)

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace LitMath
{

    public static partial class Lit
    {
        /// <summary>
        /// Calculates 4 log base 2's on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="x">A reference to the 4 arguments</param>
        /// <param name="y">The 4 results</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log2(in Vector256<double> x, ref Vector256<double> y)
        {
            // This algorithm uses the properties of floating point number to transform x into d*2^m, so log(x)
            // becomes log(d)+m, where d is in [1, 2]. Then it uses a series approximation of log to approximate 
            // the value in [1, 2]

            var xl = Vector256.AsInt64(Avx.Max(x, Double.Log.ZERO));
            var mantissa = Avx2.Subtract(Avx2.ShiftRightLogical(xl, 52), Lit.Long.ONE_THOUSAND_TWENTY_THREE);

            Util.ConvertLongToDouble(in mantissa, ref y);

            xl = Avx2.Or(Avx2.And(xl, Lit.Long.DECIMAL_MASK_FOR_DOUBLE), Lit.Long.EXPONENT_MASK_FOR_DOUBLE);

            var d = Avx.Multiply(Avx.Or(Vector256.AsDouble(xl), Double.Log.ONE), Double.Log.TWO_THIRDS);

            LogApprox(in d, ref d);

            y = Avx.Add(Fma.MultiplyAdd(d, Double.Log.LOG2EF, Double.Log.LOG_ONE_POINT_FIVE), y);
            //y = Avx.Add(Avx.Add(Avx.Multiply(d, LitConstants.Double.Log.LOG2EF), LitConstants.Double.Log.LOG_ONE_POINT_FIVE), y);
            y = Util.IfElse(Avx.CompareLessThan(x, Double.Log.ZERO), Double.Log.NAN, y);
            y = Util.IfElse(Avx.CompareEqual(x, Double.Log.ZERO), Double.Log.NEGATIVE_INFINITY, y);
            y = Util.IfElse(Avx.CompareEqual(x, Double.Log.POSITIVE_INFINITY), Double.Log.POSITIVE_INFINITY, y);
            y = Util.IfElse(Avx.CompareNotEqual(x, x), Double.Log.NAN, y);
        }

#if NET8_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log2(in Vector512<double> x, ref Vector512<double> y)
        {
            // This algorithm uses the properties of floating point number to transform x into d*2^m, so log(x)
            // becomes log(d)+m, where d is in [1, 2]. Then it uses a series approximation of log to approximate 
            // the value in [1, 2]

            var d = Avx512F.GetMantissa(x, 0); // 0 = _MM_MANT_NORM_1_2, which scales the mantissa in [1,2]
            y = Avx512F.GetExponent(x);

            LogApprox(in d, ref d);

            y = Avx512F.FusedMultiplyAdd(d, Double512.Log.LOG2EF, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ln(in Vector512<double> x, ref Vector512<double> y)
        {
            // This algorithm uses the properties of floating point number to transform x into d*2^m, so log(x)
            // becomes log(d)+m, where d is in [1, 2]. Then it uses a series approximation of log to approximate 
            // the value in [1, 2]

            var d = Avx512F.GetMantissa(x, 0); // 0 = _MM_MANT_NORM_1_2, which scales the mantissa in [1,2]
            y = Avx512F.GetExponent(x);

            LogApprox(in d, ref d);

            y = Avx512F.FusedMultiplyAdd(Double512.Log.LN2, y, d);
        }
#endif

        /// <summary>
        /// Calculates 8 log base 2's on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log2(in Vector256<float> x, ref Vector256<float> y)
        {
            var end = Avx.CompareLessThanOrEqual(x, Float.Log.ZERO);
            end = Avx.Add(Avx.And(Avx.CompareEqual(x, Float.Log.POSITIVE_INFINITY), Float.Log.POSITIVE_INFINITY), end);
            end = Avx.Add(Avx.CompareNotEqual(x, x), end);

            var xl = Vector256.AsInt32(Avx.Max(x, Float.Log.ZERO));
            var m = Avx2.Subtract(Avx2.ShiftRightLogical(xl, 23), Lit.Int.ONE_HUNDRED_TWENTY_SEVEN);

            y = Avx.ConvertToVector256Single(m);

            xl = Avx2.Or(Avx2.And(xl, Lit.Int.DECIMAL_MASK_FOR_FLOAT), Lit.Int.EXPONENT_MASK_FOR_FLOAT);

            var d = Avx.Multiply(Avx.Or(Vector256.AsSingle(xl), Float.Log.ONE), Float.Log.TWO_THIRDS);

            LogApprox(in d, ref d);


            //y = Avx.Add(Fma.MultiplyAdd(d, LitConstants.Float.Log.LOG2EF, LitConstants.Float.Log.LOG_ONE_POINT_FIVE), y);
            y = Avx.Add(Avx.Add(Avx.Multiply(d, Float.Log.LOG2EF), Float.Log.LOG_ONE_POINT_FIVE), y);
            y = Avx.Add(end, y);
        }

        /// <summary>
        /// A Taylor Series approximation of ln(x) that relies on the identity that ln(x) = 2*atan((x-1)/(x+1)).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogApprox(in Vector256<double> x, ref Vector256<double> y)
        {
            y = Avx.Divide(Avx.Subtract(x, Double.Log.ONE), Avx.Add(x, Double.Log.ONE));
            var ysq = Avx.Multiply(y, y);

            var rx = Fma.MultiplyAdd(ysq, Double.Log.L8, Double.Log.L7);
            rx = Fma.MultiplyAdd(ysq, rx, Double.Log.L6);
            rx = Fma.MultiplyAdd(ysq, rx, Double.Log.L5);
            rx = Fma.MultiplyAdd(ysq, rx, Double.Log.L4);
            rx = Fma.MultiplyAdd(ysq, rx, Double.Log.L3);
            rx = Fma.MultiplyAdd(ysq, rx, Double.Log.L2);
            rx = Fma.MultiplyAdd(ysq, rx, Double.Log.L1);
            rx = Fma.MultiplyAdd(ysq, rx, Double.Log.TWO);

            y = Avx.Multiply(y, rx);
        }

#if NET8_0_OR_GREATER
        /// <summary>
        /// A Taylor Series approximation of ln(x) that relies on the identity that ln(x) = 2*atan((x-1)/(x+1)).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogApprox(in Vector512<double> x, ref Vector512<double> y)
        {
            y = Avx512F.Divide(Avx512F.Subtract(x, Double512.Log.ONE), Avx512F.Add(x, Double512.Log.ONE));
            var ysq = Avx512F.Multiply(y, y);

            var rx = Avx512F.FusedMultiplyAdd(ysq, Double512.Log.L8, Double512.Log.L7);
            rx = Avx512F.FusedMultiplyAdd(ysq, rx, Double512.Log.L6);
            rx = Avx512F.FusedMultiplyAdd(ysq, rx, Double512.Log.L5);
            rx = Avx512F.FusedMultiplyAdd(ysq, rx, Double512.Log.L4);
            rx = Avx512F.FusedMultiplyAdd(ysq, rx, Double512.Log.L3);
            rx = Avx512F.FusedMultiplyAdd(ysq, rx, Double512.Log.L2);
            rx = Avx512F.FusedMultiplyAdd(ysq, rx, Double512.Log.L1);
            rx = Avx512F.FusedMultiplyAdd(ysq, rx, Double512.Log.TWO);

            y = Avx512F.Multiply(y, rx);
        }
#endif

        /// <summary>
        /// A Taylor Series approximation of ln(x) that relies on the identity that ln(x) = 2*atan((x-1)/(x+1)).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void LogApprox(in Vector256<float> x, ref Vector256<float> y)
        {
            y = Avx.Divide(Avx.Subtract(x, Float.Log.ONE), Avx.Add(x, Float.Log.ONE));
            var ysq = Avx.Multiply(y, y);

            var rx = Avx.Multiply(ysq, Float.Log.ONE_ELEVENTH);
            rx = Avx.Add(rx, Float.Log.ONE_NINTH);
            rx = Avx.Multiply(ysq, rx);
            rx = Avx.Add(rx, Float.Log.ONE_SEVENTH);
            rx = Avx.Multiply(ysq, rx);
            rx = Avx.Add(rx, Float.Log.ONE_FIFTH);
            rx = Avx.Multiply(ysq, rx);
            rx = Avx.Add(rx, Float.Log.ONE_THIRD);
            rx = Avx.Multiply(ysq, rx);
            rx = Avx.Add(rx, Float.Log.ONE);
            rx = Avx.Multiply(y, rx);
            y = Avx.Multiply(rx, Float.Log.TWO);
        }


        /// <summary>
        /// Computes the natural log on 4 doubles
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ln(in Vector256<double> x, ref Vector256<double> y)
        {
            Log2(in x, ref y);
            y = Avx.Multiply(Double.Log.LN2, y);
        }

        /// <summary>
        /// Computes the natural log on 4 doubles
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Ln(in Vector256<double> x)
        {
            var y = Vector256.Create(0.0);
            Log2(in x, ref y);
            return Avx.Multiply(Double.Log.LN2, y);
        }

        /// <summary>
        /// Computes the natural log on 4 doubles
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Ln(Vector256<double> x)
        {
            Log2(in x, ref x);
            return Avx.Multiply(Double.Log.LN2, x);
        }

        /// <summary>
        /// Computes the natural log on 8 floats
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ln(in Vector256<float> x, ref Vector256<float> y)
        {
            Log2(in x, ref y);
            y = Avx.Multiply(Float.Log.LN2, y);
        }

        /// <summary>
        /// Computes the natural log on 8 floats
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<float> Ln(in Vector256<float> x)
        {
            var y = Vector256.Create(0.0f);
            Log2(in x, ref y);
            return Avx.Multiply(Float.Log.LN2, y);
        }

        /// <summary>
        /// Computes the natural log on 8 floats
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<float> Ln(Vector256<float> x)
        {
            Log2(in x, ref x);
            return Avx.Multiply(Float.Log.LN2, x);
        }


        /// <summary>
        /// Calculates the natural log of 4 doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        /// <param name="index">The offset index to calculate on</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ln(in double xx, ref double yy, int index)
        {
#if NET8_0_OR_GREATER
            if (Avx512F.IsSupported)
            {
                var x = Util512.LoadV512(in xx, index);
                var y = Vector512<double>.Zero;
                Ln(in x, ref y);
                Util512.StoreV512(ref yy, index, y);
            } else {
#endif
                var x = Util.LoadV256(in xx, index);
                var y = Vector256<double>.Zero;
                Ln(in x, ref y);
                Util.StoreV256(ref yy, index, y);
#if NET8_0_OR_GREATER
            }
#endif
        }

        /// <summary>
        /// Calculates the natural log of 8 floats via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        /// <param name="index">The offset index to calculate on</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ln(in float xx, ref float yy, int index)
        {
            var x = Util.LoadV256(in xx, index);
            Ln(in x, ref x);
            Util.StoreV256(ref yy, index, x);
        }


        /// <summary>
        /// Calculates n natural logs on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="x">A Span to the first argument</param>
        /// <param name="y">The return values</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<double> Ln(in Span<double> x)
        {
            var y = new Span<double>(GC.AllocateUninitializedArray<double>(x.Length));
            Ln(in x, ref y);
            return y;
        }

        /// <summary>
        /// Calculates n natural logs on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="x">A Span to the first argument</param>
        /// <param name="y">The return values</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<float> Ln(in Span<float> x)
        {
            var y = new Span<float>(GC.AllocateUninitializedArray<float>(x.Length));
            Ln(in x, ref y);
            return y;
        }

        /// <summary>
        /// Calculates n natural logs on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="xx">A pointer to the first argument</param>
        /// <param name="yy">The return values</param>
        /// <param name="n">The number of xx values to take an natural log of</param>
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ln(in Span<double> xx, ref Span<double> yy)
        {
            int VSZ = 4;

#if NET8_0_OR_GREATER
            if (Avx512F.IsSupported)
                VSZ = 8;
#endif

            var n = xx.Length;
            ref var x = ref MemoryMarshal.GetReference(xx);
            ref var y = ref MemoryMarshal.GetReference(yy);

            if (n< VSZ)
            {
                var mask = Util.CreateMaskDouble(~(int.MaxValue << n));
                var xv = Util.LoadMaskedV256(in x, 0, mask);
                var yv = Vector256.Create(0.0);
                Ln(in xv, ref yv);
                Util.StoreMaskedV256(ref y, 0, yv, mask);

                return;
            }

            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - Loop.Four(VSZ)))
            {
                Ln(in x, ref y, i);
                i += VSZ;
                Ln(in x, ref y, i);
                i += VSZ;
                Ln(in x, ref y, i);
                i += VSZ;
                Ln(in x, ref y, i);
                i += VSZ;
            }

            // Calculates the remaining sets of 4 values in a standard loop
            for (; i < (n - Loop.One(VSZ)); i += VSZ)
                Ln(in x, ref y, i);


            // Cleans up any excess individual values (if n%4 != 0)
            if (i != n)
            {
                i = n - VSZ;
                Ln(in x, ref y, i);
            }
        }

        /// <summary>
        /// Calculates n natural logs on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="xx">A pointer to the first argument</param>
        /// <param name="yy">The return values</param>
        /// <param name="n">The number of xx values to take an natural log of</param>
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ln(in Span<float> xx, ref Span<float> yy)
        {
            const int VSZ = 8;
            var n = xx.Length;
            ref var x = ref MemoryMarshal.GetReference(xx);
            ref var y = ref MemoryMarshal.GetReference(yy);

            if (n < VSZ)
            {
                var mask = Util.CreateMaskFloat(~(int.MaxValue << n));
                var xv = Util.LoadMaskedV256(in x, 0, mask);
                var yv = Vector256.Create(0.0f);
                Ln(in xv, ref yv);
                Util.StoreMaskedV256(ref y, 0, yv, mask);

                return;
            }

            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - 31))
            {
                Ln(in x, ref y, i);
                i += VSZ;
                Ln(in x, ref y, i);
                i += VSZ;
                Ln(in x, ref y, i);
                i += VSZ;
                Ln(in x, ref y, i);
                i += VSZ;
            }

            // Calculates the remaining sets of 8 values in a standard loop
            for (; i < (n - 7); i += VSZ)
                Ln(in x, ref y, i);

            // Cleans up any excess individual values (if n%8 != 0)
            if (i != n)
            {
                i = n - 8;
                Ln(in x, ref y, i);
            }
        }
    }
}
