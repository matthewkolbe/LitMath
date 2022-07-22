// Copyright Matthew Kolbe (2022)

using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;


namespace LitMath
{
    public static class LitUtilities
    {

        /// <summary>
        /// Copies data from one n-sized Span to another
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(ref Span<double> from, ref Span<double> to)
        {
            unsafe
            {
                fixed (double* f = from) fixed (double* t = to)
                    Copy(f, t, from.Length);
            }
        }


        /// <summary>
        /// Copies data from one n-sized Span to another
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(ref Span<int> from, ref Span<int> to)
        {
            unsafe
            {
                fixed (int* f = from) fixed (int* t = to)
                    Copy(f, t, from.Length);
            }
        }


        /// <summary>
        /// Copies data from one n-sized Span to another
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(ref Span<float> from, ref Span<float> to)
        {
            unsafe
            {
                fixed (float* f = from) fixed (float* t = to)
                    Copy(f, t, from.Length);
            }
        }


        /// <summary>
        /// Fills a Span with the specified value
        /// </summary>
        /// <param name="v"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Apply(ref Span<double> v, double value)
        {
            unsafe
            {
                fixed (double* vv = v)
                    Apply(vv, v.Length, value);
            }
        }


        /// <summary>
        /// Returns the sum of absolute values of a Span
        /// </summary>
        /// <param name="v">Pointer to the head of the array</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double AbsSum(ref Span<double> v)
        {
            unsafe
            {
                fixed (double* vv = v)
                    return AbsSum(vv, v.Length);
            }
        }


        /// <summary>
        /// Copies data from one n-sized array to another
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="n">Number of data pieces to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Copy(double* from, double* to, int n)
        {
            int i = 0;

            while (i < (n - 15))
            {
                Avx.Store(to + i, Avx.LoadVector256(from + i));
                i += 4;
                Avx.Store(to + i, Avx.LoadVector256(from + i));
                i += 4;
                Avx.Store(to + i, Avx.LoadVector256(from + i));
                i += 4;
                Avx.Store(to + i, Avx.LoadVector256(from + i));
                i += 4;
            }

            for (; i < (n - 3); i += 4)
                Avx.Store(to + i, Avx.LoadVector256(from + i));

            // Cleans up the residual
            for (; i < n; i++)
                to[i] = from[i];
        }


        /// <summary>
        /// Copies data from one n-sized array to another
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="n">Number of data pieces to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Copy(int* from, int* to, int n)
        {
            int i = 0;

            while (i < (n - 31))
            {
                Avx.Store(to + i, Avx.LoadVector256(from + i));
                i += 8;
                Avx.Store(to + i, Avx.LoadVector256(from + i));
                i += 8;
                Avx.Store(to + i, Avx.LoadVector256(from + i));
                i += 8;
                Avx.Store(to + i, Avx.LoadVector256(from + i));
                i += 8;
            }

            while (i < (n - 7))
            {
                Avx.Store(to + i, Avx.LoadVector256(from + i));
                i += 8;
            }

            // Cleans up the residual
            for (; i < n; i++)
                to[i] = from[i];
        }


        /// <summary>
        /// Copies data from one n-sized array to another
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="n">Number of data pieces to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Copy(float* from, float* to, float n)
        {
            int i = 0;

            while (i < (n - 31))
            {
                Avx.Store(to + i, Avx.LoadVector256(from + i));
                i += 8;
                Avx.Store(to + i, Avx.LoadVector256(from + i));
                i += 8;
                Avx.Store(to + i, Avx.LoadVector256(from + i));
                i += 8;
                Avx.Store(to + i, Avx.LoadVector256(from + i));
                i += 8;
            }

            for (; i < (n - 7); i += 8)
                Avx.Store(to + i, Avx.LoadVector256(from + i));

            // Cleans up the residual
            for (; i < n; i++)
                to[i] = from[i];
        }


        /// <summary>
        /// Fills an array with the specified value
        /// </summary>
        /// <param name="v"></param>
        /// <param name="n">Number of data pieces to broadcast</param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Apply(double* v, int n, double value)
        {
            int i = 0;
            var x = Avx.BroadcastScalarToVector256(&value);

            while (i < (n - 15))
            {
                Avx.Store(v + i, x);
                i += 4;
                Avx.Store(v + i, x);
                i += 4;
                Avx.Store(v + i, x);
                i += 4;
                Avx.Store(v + i, x);
                i += 4;
            }

            for (; i < (n - 3); i += 4)
                Avx.Store(v + i, x);


            // Cleans up the residual
            for (; i < n; i++)
                v[i] = value;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Apply(byte* v, uint n, byte value)
        {
            Unsafe.InitBlock(v, value, n);
        }


        /// <summary>
        /// Returns the sum of absolute values of an array
        /// </summary>
        /// <param name="v">Pointer to the head of the array</param>
        /// <param name="n">The size of the array</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe double AbsSum(double* v, int n)
        {
            var r = Vector256.Create(0.0);
            int i = 0;

            while (i < (n - 15))
            {
                r = Avx.Add(Abs(Avx.LoadVector256(v + i)), r);
                i += 4;
                r = Avx.Add(Abs(Avx.LoadVector256(v + i)), r);
                i += 4;
                r = Avx.Add(Abs(Avx.LoadVector256(v + i)), r);
                i += 4;
                r = Avx.Add(Abs(Avx.LoadVector256(v + i)), r);
                i += 4;
            }

            for (; i < (n - 3); i += 4)
                r = Avx.Add(Abs(Avx.LoadVector256(v + i)), r);


            var rr = LitBasics.Aggregate(ref r);

            // Cleans up the residual
            for (; i < n; i++)
                rr += Math.Abs(v[i]);

            return rr;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Abs(ref Vector256<double> x, ref Vector256<double> y)
        {
            y = Avx.AndNot(LitConstants.Double.Util.NEGATIVE_ZERO, x);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Abs(ref Vector256<double> x)
        {
            return Avx.AndNot(LitConstants.Double.Util.NEGATIVE_ZERO, x);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Abs(Vector256<double> x)
        {
            return Avx.AndNot(LitConstants.Double.Util.NEGATIVE_ZERO, x);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConvertLongToDouble(ref Vector256<long> x, ref Vector256<double> y)
        {
            y = Avx.Subtract(Vector256.AsDouble(Avx2.Add(x, LitConstants.Long.MAGIC_LONG_DOUBLE_ADD)), LitConstants.Double.Util.MAGIC_LONG_DOUBLE_ADD);
        }


        /// <summary>
        /// Converts x to 2^x
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConvertXtoPower(ref Vector256<double> x, ref Vector256<double> y)
        {
            y = Avx2.Add(x, LitConstants.Double.Util.MAGIC_LONG_DOUBLE_ADD);
            var z = Vector256.AsInt64(y);
            z = Avx2.Add(z, LitConstants.Long.ONE_THOUSAND_TWENTY_THREE);
            z = Avx2.ShiftLeftLogical(z, 52);
            y = Vector256.AsDouble(z);
        }
    }
}
