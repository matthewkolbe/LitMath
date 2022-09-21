﻿// Copyright Matthew Kolbe (2022)

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


        /// <summary>
        /// Fills an array with the specified value
        /// </summary>
        /// <param name="v"></param>
        /// <param name="n">Number of data pieces to broadcast</param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Apply(float* v, int n, float value)
        {
            int i = 0;
            var x = Avx.BroadcastScalarToVector256(&value);

            while (i < (n - 31))
            {
                Avx.Store(v + i, x);
                i += 8;
                Avx.Store(v + i, x);
                i += 8;
                Avx.Store(v + i, x);
                i += 8;
                Avx.Store(v + i, x);
                i += 8;
            }

            for (; i < (n - 7); i += 8)
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
            const int VSZ = 4;
            var r = Vector256.Create(0.0);
            int i = 0;

            while (i < (n - 15))
            {
                r = Avx.Add(Abs(Avx.LoadVector256(v + i)), r);
                i += VSZ;
                r = Avx.Add(Abs(Avx.LoadVector256(v + i)), r);
                i += VSZ;
                r = Avx.Add(Abs(Avx.LoadVector256(v + i)), r);
                i += VSZ;
                r = Avx.Add(Abs(Avx.LoadVector256(v + i)), r);
                i += VSZ;
            }

            for (; i < (n - 3); i += VSZ)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> ConvertLongToDouble(ref Vector256<long> x)
        {
            return Avx.Subtract(Vector256.AsDouble(Avx2.Add(x, LitConstants.Long.MAGIC_LONG_DOUBLE_ADD)), LitConstants.Double.Util.MAGIC_LONG_DOUBLE_ADD);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> IfLessThan(Vector256<double> x, Vector256<double> condition,
            Vector256<double> trueval, Vector256<double> falseval)
        {
            return Avx.Add(
                    Avx.And(Avx.CompareLessThan(x, condition), trueval),
                    Avx.And(Avx.CompareGreaterThanOrEqual(x, condition), falseval));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> IfGreaterThan(Vector256<double> x, Vector256<double> condition,
             Vector256<double> trueval, Vector256<double> falseval)
        {
            return Avx.Add(
                    Avx.And(Avx.CompareGreaterThan(x, condition), trueval),
                    Avx.And(Avx.CompareLessThanOrEqual(x, condition), falseval));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> IfElse(Vector256<double> mask, Vector256<double> trueval, Vector256<double> falseval)
        {
            return Avx.Add(
                    Avx.And(mask, trueval),
                    Avx.AndNot(mask, falseval));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Max(ref Vector256<double> x, Vector256<double> max)
        {
            x = IfElse(Avx.CompareGreaterThanOrEqual(x, max), x, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Min(ref Vector256<double> x, Vector256<double> min)
        {
            x = IfElse(Avx.CompareLessThanOrEqual(x, min), x, min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Max(Vector256<double> x, Vector256<double> max)
        {
            return IfElse(Avx.CompareGreaterThanOrEqual(x, max), x, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Min(Vector256<double> x, Vector256<double> min)
        {
            return IfElse(Avx.CompareLessThanOrEqual(x, min), x, min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Sign(Vector256<double> x)
        {
            return IfElse(Avx.CompareGreaterThanOrEqual(x, LitConstants.Double.Util.ZERO),
                LitConstants.Double.Util.ONE,
                LitConstants.Double.Util.NEGONE);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Mult(Vector256<double> a, Vector256<double> b)
        {
            return Avx.Multiply(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Mult(Vector256<double> a, Vector256<double> b, Vector256<double> c)
        {
            return Avx.Multiply(Avx.Multiply(a, b), c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Mult(Vector256<double> a, Vector256<double> b, Vector256<double> c, Vector256<double> d)
        {
            return Avx.Multiply(Avx.Multiply(a, b), Avx.Multiply(c, d));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Mult(Vector256<double> a, Vector256<double> b, Vector256<double> c, Vector256<double> d, Vector256<double> e)
        {
            return Avx.Multiply(Avx.Multiply(Avx.Multiply(a, b), Avx.Multiply(c, d)), e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<float> IfLessThan(Vector256<float> x, Vector256<float> condition,
            Vector256<float> trueval, Vector256<float> falseval)
        {
            return Avx.Add(
                    Avx.And(Avx.CompareLessThan(x, condition), trueval),
                    Avx.And(Avx.CompareGreaterThanOrEqual(x, condition), falseval));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<float> IfElse(Vector256<float> mask, Vector256<float> trueval, Vector256<float> falseval)
        {
            return Avx.Add(
                    Avx.And(mask, trueval),
                    Avx.AndNot(mask, falseval));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Max(ref Vector256<float> x, Vector256<float> max)
        {
            x = IfElse(Avx.CompareGreaterThanOrEqual(x, max), x, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Min(ref Vector256<float> x, Vector256<float> min)
        {
            x = IfElse(Avx.CompareLessThanOrEqual(x, min), x, min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<float> Max(Vector256<float> x, Vector256<float> max)
        {
            return IfElse(Avx.CompareGreaterThanOrEqual(x, max), x, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<float> Min(Vector256<float> x, Vector256<float> min)
        {
            return IfElse(Avx.CompareLessThanOrEqual(x, min), x, min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<float> Mult(Vector256<float> a, Vector256<float> b)
        {
            return Avx.Multiply(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<float> Mult(Vector256<float> a, Vector256<float> b, Vector256<float> c)
        {
            return Avx.Multiply(Avx.Multiply(a, b), c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<float> Mult(Vector256<float> a, Vector256<float> b, Vector256<float> c, Vector256<float> d)
        {
            return Avx.Multiply(Avx.Multiply(a, b), Avx.Multiply(c, d));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<float> Mult(Vector256<float> a, Vector256<float> b, Vector256<float> c, Vector256<float> d, Vector256<float> e)
        {
            return Avx.Multiply(Avx.Multiply(Avx.Multiply(a, b), Avx.Multiply(c, d)), e);
        }
    }
}
