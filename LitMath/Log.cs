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
        /// Calculates 4 log base 2's on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="x">A reference to the 4 arguments</param>
        /// <param name="y">The 4 results</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log2(ref Vector256<double> x, ref Vector256<double> y)
        {
            // This algorithm uses the properties of floating point number to transform x into d*2^m, so log(x)
            // becomes log(d)+m, where d is in [1, 2]. Then it uses a series approximation of log to approximate 
            // the value in [1, 2]

            var xl = Vector256.AsInt64(Avx.Max(x, Double.Log.ZERO));
            var mantissa = Avx2.Subtract(Avx2.ShiftRightLogical(xl, 52), Lit.Long.ONE_THOUSAND_TWENTY_THREE);

            Util.ConvertLongToDouble(ref mantissa, ref y);

            xl = Avx2.Or(Avx2.And(xl, Lit.Long.DECIMAL_MASK_FOR_DOUBLE), Lit.Long.EXPONENT_MASK_FOR_DOUBLE);

            var d = Avx.Multiply(Avx.Or(Vector256.AsDouble(xl), Double.Log.ONE), Double.Log.TWO_THIRDS);

            LogApprox(ref d, ref d);

            y = Avx.Add(Fma.MultiplyAdd(d, Double.Log.LOG2EF, Double.Log.LOG_ONE_POINT_FIVE), y);
            //y = Avx.Add(Avx.Add(Avx.Multiply(d, LitConstants.Double.Log.LOG2EF), LitConstants.Double.Log.LOG_ONE_POINT_FIVE), y);
            y = Util.IfElse(Avx.CompareLessThan(x, Double.Log.ZERO), Double.Log.NAN, y);
            y = Util.IfElse(Avx.CompareEqual(x, Double.Log.ZERO), Double.Log.NEGATIVE_INFINITY, y);
            y = Util.IfElse(Avx.CompareEqual(x, Double.Log.POSITIVE_INFINITY), Double.Log.POSITIVE_INFINITY, y);
            y = Util.IfElse(Avx.CompareNotEqual(x, x), Double.Log.NAN, y);
        }


        /// <summary>
        /// Calculates 8 log base 2's on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log2(ref Vector256<float> x, ref Vector256<float> y)
        {
            var end = Avx.CompareLessThanOrEqual(x, Float.Log.ZERO);
            end = Avx.Add(Avx.And(Avx.CompareEqual(x, Float.Log.POSITIVE_INFINITY), Float.Log.POSITIVE_INFINITY), end);
            end = Avx.Add(Avx.CompareNotEqual(x, x), end);

            var xl = Vector256.AsInt32(Avx.Max(x, Float.Log.ZERO));
            var m = Avx2.Subtract(Avx2.ShiftRightLogical(xl, 23), Lit.Int.ONE_HUNDRED_TWENTY_SEVEN);

            y = Avx.ConvertToVector256Single(m);

            xl = Avx2.Or(Avx2.And(xl, Lit.Int.DECIMAL_MASK_FOR_FLOAT), Lit.Int.EXPONENT_MASK_FOR_FLOAT);

            var d = Avx.Multiply(Avx.Or(Vector256.AsSingle(xl), Float.Log.ONE), Float.Log.TWO_THIRDS);

            LogApprox(ref d, ref d);


            //y = Avx.Add(Fma.MultiplyAdd(d, LitConstants.Float.Log.LOG2EF, LitConstants.Float.Log.LOG_ONE_POINT_FIVE), y);
            y = Avx.Add(Avx.Add(Avx.Multiply(d, Float.Log.LOG2EF), Float.Log.LOG_ONE_POINT_FIVE), y);
            y = Avx.Add(end, y);
        }

        
        /// <summary>
        /// A Taylor Series approximation of ln(x) that relies on the identity that ln(x) = 2*atan((x-1)/x(+1)).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogApprox(ref Vector256<double> x, ref Vector256<double> y)
        {
            y = Avx.Divide(Avx.Subtract(x, Double.Log.ONE), Avx.Add(x, Double.Log.ONE));
            var ysq = Avx.Multiply(y, y);

            var rx = Fma.MultiplyAdd(ysq, Double.Log.ONE_THIRTEENTH, Double.Log.ONE_ELEVENTH);
            rx = Fma.MultiplyAdd(ysq, rx, Double.Log.ONE_NINTH);
            rx = Fma.MultiplyAdd(ysq, rx, Double.Log.ONE_SEVENTH);
            rx = Fma.MultiplyAdd(ysq, rx, Double.Log.ONE_FIFTH);
            rx = Fma.MultiplyAdd(ysq, rx, Double.Log.ONE_THIRD);
            rx = Fma.MultiplyAdd(ysq, rx, Double.Log.ONE);

            rx = Avx.Multiply(y, rx);
            y = Avx.Multiply(rx, Double.Log.TWO);
        }


        /// <summary>
        /// A Taylor Series approximation of ln(x) that relies on the identity that ln(x) = 2*atan((x-1)/x(+1)).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void LogApprox(ref Vector256<float> x, ref Vector256<float> y)
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
        public static void Ln(ref Vector256<double> x, ref Vector256<double> y)
        {
            Log2(ref x, ref y);
            y = Avx.Multiply(Double.Log.LN2, y);
        }

        /// <summary>
        /// Computes the natural log on 4 doubles
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Ln(ref Vector256<double> x)
        {
            var y = Vector256.Create(0.0);
            Log2(ref x, ref y);
            return Avx.Multiply(Double.Log.LN2, y);
        }

        /// <summary>
        /// Computes the natural log on 4 doubles
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Ln(Vector256<double> x)
        {
            Log2(ref x, ref x);
            return Avx.Multiply(Double.Log.LN2, x);
        }

        /// <summary>
        /// Computes the natural log on 8 floats
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ln(ref Vector256<float> x, ref Vector256<float> y)
        {
            Log2(ref x, ref y);
            y = Avx.Multiply(Float.Log.LN2, y);
        }

        /// <summary>
        /// Computes the natural log on 8 floats
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<float> Ln(ref Vector256<float> x)
        {
            var y = Vector256.Create(0.0f);
            Log2(ref x, ref y);
            return Avx.Multiply(Float.Log.LN2, y);
        }

        /// <summary>
        /// Computes the natural log on 8 floats
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<float> Ln(Vector256<float> x)
        {
            Log2(ref x, ref x);
            return Avx.Multiply(Float.Log.LN2, x);
        }


        /// <summary>
        /// Calculates the natural log of 4 doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        /// <param name="index">The offset index to calculate on</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ln(ref double xx, ref double yy, int index)
        {
            var x = Util.LoadV256(ref xx, index);
            var y = Vector256<double>.Zero;
            Ln(ref x, ref y);
            Util.StoreV256(ref yy, index, y);
        }

        /// <summary>
        /// Calculates the natural log of 8 floats via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        /// <param name="index">The offset index to calculate on</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ln(ref float xx, ref float yy, int index)
        {
            var x = Util.LoadV256(ref xx, index);
            Ln(ref x, ref x);
            Util.StoreV256(ref yy, index, x);
        }


        /// <summary>
        /// Calculates n natural logs on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="x">A Span to the first argument</param>
        /// <param name="y">The return values</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<double> Ln(ref Span<double> x)
        {
            var y = new Span<double>(GC.AllocateUninitializedArray<double>(x.Length));
            Ln(ref x, ref y);
            return y;
        }

        /// <summary>
        /// Calculates n natural logs on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="x">A Span to the first argument</param>
        /// <param name="y">The return values</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<double> Ln(Span<double> x)
        {
            return Ln(ref x);
        }

        /// <summary>
        /// Calculates n natural logs on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="x">A Span to the first argument</param>
        /// <param name="y">The return values</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<float> Ln(ref Span<float> x)
        {
            var y = new Span<float>(GC.AllocateUninitializedArray<float>(x.Length));
            Ln(ref x, ref y);
            return y;
        }

        /// <summary>
        /// Calculates n natural logs on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="x">A Span to the first argument</param>
        /// <param name="y">The return values</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<float> Ln(Span<float> x)
        {
            return Ln(ref x);
        }

        /// <summary>
        /// Calculates n natural logs on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="xx">A pointer to the first argument</param>
        /// <param name="yy">The return values</param>
        /// <param name="n">The number of xx values to take an natural log of</param>
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ln(ref Span<double> xx, ref Span<double> yy)
        {
            const int VSZ = 4;
            var n = xx.Length;
            ref var x = ref MemoryMarshal.GetReference(xx);
            ref var y = ref MemoryMarshal.GetReference(yy);

            if (n< VSZ)
            {
                Span<double> tmp = stackalloc double[VSZ];
                ref var tmpx = ref MemoryMarshal.GetReference(tmp);
                for (int j = 0; j < n; j++)
                    Unsafe.Add(ref tmpx, j) = Unsafe.Add(ref x, j);

                Ln(ref tmpx, ref tmpx, 0);

                for (int j = 0; j < n; ++j)
                    Unsafe.Add(ref y, j) = Unsafe.Add(ref tmpx, j);
            }

            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - 15))
            {
                Ln(ref x, ref y, i);
                i += VSZ;
                Ln(ref x, ref y, i);
                i += VSZ;
                Ln(ref x, ref y, i);
                i += VSZ;
                Ln(ref x, ref y, i);
                i += VSZ;
            }

            // Calculates the remaining sets of 4 values in a standard loop
            for (; i < (n - 3); i += VSZ)
                Ln(ref x, ref y, i);


            // Cleans up any excess individual values (if n%4 != 0)
            if (i != n)
            {
                i = n - VSZ;
                Ln(ref x, ref y, i);
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
        public static void Ln(ref Span<float> xx, ref Span<float> yy)
        {
            const int VSZ = 8;
            var n = xx.Length;
            ref var x = ref MemoryMarshal.GetReference(xx);
            ref var y = ref MemoryMarshal.GetReference(yy);

            if (n < VSZ)
            {
                Span<float> tmp = stackalloc float[VSZ];
                ref var tmpx = ref MemoryMarshal.GetReference(tmp);
                for (int j = 0; j < n; j++)
                    Unsafe.Add(ref tmpx, j) = Unsafe.Add(ref x, j);

                Ln(ref tmpx, ref tmpx, 0);

                for (int j = 0; j < n; ++j)
                    Unsafe.Add(ref y, j) = Unsafe.Add(ref tmpx, j);
            }

            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - 31))
            {
                Ln(ref x, ref y, i);
                i += VSZ;
                Ln(ref x, ref y, i);
                i += VSZ;
                Ln(ref x, ref y, i);
                i += VSZ;
                Ln(ref x, ref y, i);
                i += VSZ;
            }

            // Calculates the remaining sets of 8 values in a standard loop
            for (; i < (n - 7); i += VSZ)
                Ln(ref x, ref y, i);

            // Cleans up any excess individual values (if n%8 != 0)
            if (i != n)
            {
                i = n - 8;
                Ln(ref x, ref y, i);
            }
        }
    }
}
