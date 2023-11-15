// Copyright Matthew Kolbe (2022)

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;


namespace LitMath
{
    internal static class Loop
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Four(int vsz)
        {
            return 4 * vsz - 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int One(int vsz)
        {
            return vsz - 1;
        }
    }

    public static class Util
    {

        /// <summary>
        /// Copies data from one n-sized Span to another
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(in Span<double> from, ref Span<double> to)
        {
            ref var fr = ref MemoryMarshal.GetReference(from);
            ref var t = ref MemoryMarshal.GetReference(to);
            Copy(in fr, ref t, from.Length);
        }


        /// <summary>
        /// Copies data from one n-sized Span to another
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(in Span<int> from, ref Span<int> to)
        {
            ref var fr = ref MemoryMarshal.GetReference(from);
            ref var t = ref MemoryMarshal.GetReference(to);
            Copy(in fr, ref t, from.Length);
        }


        /// <summary>
        /// Copies data from one n-sized Span to another
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(in Span<float> from, ref Span<float> to)
        {
            ref var fr = ref MemoryMarshal.GetReference(from);
            ref var t = ref MemoryMarshal.GetReference(to);
            Copy(in fr, ref t, from.Length);
        }


        /// <summary>
        /// Fills a Span with the specified value
        /// </summary>
        /// <param name="v"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Apply(ref Span<double> v, double value)
        {
            ref var vv = ref MemoryMarshal.GetReference(v);
            Apply(ref vv, v.Length, value);
        }


        /// <summary>
        /// Does an absolute value of each element of a Span
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Abs(in Span<double> v, ref Span<double> r)
        {
            ref var vv = ref MemoryMarshal.GetReference(v);
            ref var rr = ref MemoryMarshal.GetReference(r);
            Abs(in vv, ref rr, r.Length);
        }

        /// <summary>
        /// Converts doubles to ints
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConvertDoubleToInt(in Span<double> v, ref Span<int> r)
        {
            ref var vv = ref MemoryMarshal.GetReference(v);
            ref var rr = ref MemoryMarshal.GetReference(r);
            ConvertDoubleToInt(in vv, ref rr, r.Length);
        }

        /// <summary>
        /// Calculates the max between indicies of two Spans
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Max(in Span<int> a, in Span<int> b, ref Span<int> r)
        {
            ref var aa = ref MemoryMarshal.GetReference(a);
            ref var bb = ref MemoryMarshal.GetReference(b);
            ref var rr = ref MemoryMarshal.GetReference(r);
            Max(in aa, in bb, ref rr, r.Length);
        }

        /// <summary>
        /// Calculates the min between indicies of two Spans
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Min(in Span<int> a, in Span<int> b, ref Span<int> r)
        {
            ref var aa = ref MemoryMarshal.GetReference(a);
            ref var bb = ref MemoryMarshal.GetReference(b);
            ref var rr = ref MemoryMarshal.GetReference(r);
            Min(in aa, in bb, ref rr, r.Length);
        }

        /// <summary>
        /// Calculates the sign of the span element
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sign(in Span<int> a, ref Span<int> r)
        {
            ref var aa = ref MemoryMarshal.GetReference(a);
            ref var rr = ref MemoryMarshal.GetReference(r);
            Sign(in aa, ref rr, r.Length);
        }

        /// <summary>
        /// Calculates the sign of the span element
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sign(in Span<double> a, ref Span<double> r)
        {
            ref var aa = ref MemoryMarshal.GetReference(a);
            ref var rr = ref MemoryMarshal.GetReference(r);
            Sign(in aa, ref rr, r.Length);
        }

        /// <summary>
        /// Gathers values from a[indexes[i]], and puts them in r[i].
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Gather(in Span<double> a, in Span<int> indexes, ref Span<double> r)
        {
            ref var aa = ref MemoryMarshal.GetReference(a);
            ref var ind = ref MemoryMarshal.GetReference(indexes);
            ref var rr = ref MemoryMarshal.GetReference(r);
            Gather(in aa, in ind, ref rr, r.Length);
        }

        /// <summary>
        /// Copies data from one n-sized array to another
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="n">Number of data pieces to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(in double from, ref double to, int n)
        {
            int i = 0;

            while (i < (n - 15))
            {
                StoreV256(ref to, i, LoadV256(in from, i));
                i += 4;
                StoreV256(ref to, i, LoadV256(in from, i));
                i += 4;
                StoreV256(ref to, i, LoadV256(in from, i));
                i += 4;
                StoreV256(ref to, i, LoadV256(in from, i));
                i += 4;
            }

            for (; i < (n - 3); i += 4)
                StoreV256(ref to, i, LoadV256(in from, i));

            // Cleans up the residual
            for (; i < n; i++)
                Unsafe.Add(ref to, i) = Unsafe.Add(ref Unsafe.AsRef(in from), i);
        }


        /// <summary>
        /// Copies data from one n-sized array to another
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="n">Number of data pieces to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(in int from, ref int to, int n)
        {
            int i = 0;

            while (i < (n - 31))
            {
                StoreV256(ref to, i, LoadV256(in from, i));
                i += 8;
                StoreV256(ref to, i, LoadV256(in from, i));
                i += 8;
                StoreV256(ref to, i, LoadV256(in from, i));
                i += 8;
                StoreV256(ref to, i, LoadV256(in from, i));
                i += 8;
            }

            while (i < (n - 7))
            {
                StoreV256(ref to, i, LoadV256(in from, i));
                i += 8;
            }

            // Cleans up the residual
            for (; i < n; i++)
                Unsafe.Add(ref to, i) = Unsafe.Add(ref Unsafe.AsRef(in from), i);
        }


        /// <summary>
        /// Copies data from one n-sized array to another
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="n">Number of data pieces to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(in float from, ref float to, float n)
        {
            int i = 0;

            while (i < (n - 31))
            {
                StoreV256(ref to, i, LoadV256(in from, i));
                i += 8;
                StoreV256(ref to, i, LoadV256(in from, i));
                i += 8;
                StoreV256(ref to, i, LoadV256(in from, i));
                i += 8;
                StoreV256(ref to, i, LoadV256(in from, i));
                i += 8;
            }

            for (; i < (n - 7); i += 8)
                StoreV256(ref to, i, LoadV256(in from, i));

            // Cleans up the residual
            for (; i < n; i++)
                Unsafe.Add(ref to, i) = Unsafe.Add(ref Unsafe.AsRef(in from), i);
        }


        /// <summary>
        /// Fills an array with the specified value
        /// </summary>
        /// <param name="v"></param>
        /// <param name="n">Number of data pieces to broadcast</param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Apply(ref double v, int n, double value)
        {
            int i = 0;
            var x = Vector256.Create(value);

            while (i < (n - 15))
            {
                StoreV256(ref v, i, x);
                i += 4;
                StoreV256(ref v, i, x);
                i += 4;
                StoreV256(ref v, i, x);
                i += 4;
                StoreV256(ref v, i, x);
                i += 4;
            }

            for (; i < (n - 3); i += 4)
                StoreV256(ref v, i, x);


            // Cleans up the residual
            for (; i < n; i++)
                Unsafe.Add(ref v, i) = value;
        }


        /// <summary>
        /// Fills an array with the specified value
        /// </summary>
        /// <param name="v"></param>
        /// <param name="n">Number of data pieces to broadcast</param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Apply(ref float v, int n, float value)
        {
            int i = 0;
            var x = Vector256.Create(value);

            while (i < (n - 31))
            {
                StoreV256(ref v, i, x);
                i += 8;
                StoreV256(ref v, i, x);
                i += 8;
                StoreV256(ref v, i, x);
                i += 8;
                StoreV256(ref v, i, x);
                i += 8;
            }

            for (; i < (n - 7); i += 8)
                StoreV256(ref v, i, x);

            // Cleans up the residual
            for (; i < n; i++)
                Unsafe.Add(ref v, i) = value;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Apply(ref byte v, uint n, byte value)
        {
            Unsafe.InitBlock(ref v, value, n);
        }


        /// <summary>
        /// Returns the sum of absolute values of an array
        /// </summary>
        /// <param name="v">Pointer to the head of the array</param>
        /// <param name="n">The size of the array</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double AbsSum(in Span<double> v)
        {
            const int VSZ = 4;
            var n = v.Length;
            ref var vv = ref MemoryMarshal.GetReference(v);
            int i = 0;

            if (n < VSZ)
            {
                var ret = 0.0;
                for (; i < n; ++i)
                    ret += System.Math.Abs(v[i]);

                return ret;
            }

            var r = Vector256.Create(0.0);
            var r2 = Vector256.Create(0.0);

            while (i < (n - 15))
            {
                r = Avx.Add(Abs(LoadV256(in vv, i)), r);
                i += VSZ;
                r2 = Avx.Add(Abs(LoadV256(in vv, i)), r2);
                i += VSZ;
                r = Avx.Add(Abs(LoadV256(in vv, i)), r);
                i += VSZ;
                r2 = Avx.Add(Abs(LoadV256(in vv, i)), r2);
                i += VSZ;
            }

            r = Avx.Add(r, r2);

            for (; i < (n - 3); i += VSZ)
                r = Avx.Add(Abs(LoadV256(in vv, i)), r);


            var rr = Lit.Aggregate(in r);

            // Cleans up the residual
            for (; i < n; i++)
                rr += System.Math.Abs(v[i]);

            return rr;
        }

        /// <summary>
        /// Returns the absolute value of each element of an array
        /// </summary>
        /// <param name="v">Input</param>
        /// <param name="r">The return value (can be the same as v if you so desire this to happen in-place)</param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Abs(in double v, ref double r, int n)
        {
            int i = 0;

            while (i < (n - 15))
            {
                Util.StoreV256(ref r, i, Abs(Util.LoadV256(in v, i)));
                i += 4;
                Util.StoreV256(ref r, i, Abs(Util.LoadV256(in v, i)));
                i += 4;
                Util.StoreV256(ref r, i, Abs(Util.LoadV256(in v, i)));
                i += 4;
                Util.StoreV256(ref r, i, Abs(Util.LoadV256(in v, i)));
                i += 4;
            }

            for (; i < (n - 3); i += 4)
                Util.StoreV256(ref r, i, Abs(Util.LoadV256(in v, i)));

            // clean up the residual
            for (; i < n; i++)
                Unsafe.Add(ref r, i) = Math.Abs(Unsafe.Add(ref Unsafe.AsRef(in v), i));
        }

        /// <summary>
        /// Gathers values from a[indexes[i]], and puts them in r[i].
        /// </summary>
        /// <param name="a">The potential range of values to gather</param>
        /// <param name="indexes">The indexes to gather</param>
        /// <param name="r">The return value (can be the same as v if you so desire this to happen in-place)</param>
        /// <param name="n">Size of the return and index arrays</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Gather(in double a, in int indexes, ref double r, int n)
        {
            unsafe
            {
                fixed (double* aa = &a)
                {
                    int i = 0;
                    while (i < (n - 15))
                    {
                        Util.StoreV256(ref r, i, Avx2.GatherVector256(aa, Util.LoadV128(in indexes, i), 8));
                        i += 4;
                        Util.StoreV256(ref r, i, Avx2.GatherVector256(aa, Util.LoadV128(in indexes, i), 8));
                        i += 4;
                        Util.StoreV256(ref r, i, Avx2.GatherVector256(aa, Util.LoadV128(in indexes, i), 8));
                        i += 4;
                        Util.StoreV256(ref r, i, Avx2.GatherVector256(aa, Util.LoadV128(in indexes, i), 8));
                        i += 4;
                    }

                    for (; i < (n - 3); i += 4)
                        Util.StoreV256(ref r, i, Avx2.GatherVector256(aa, Util.LoadV128(in indexes, i), 8));

                    // clean up the residual
                    // TODO: masked gather to clean up
                    for (; i < n; i++)
                        Unsafe.Add(ref r, i) = *(aa + Unsafe.Add(ref Unsafe.AsRef(in indexes), i));
                }
            }


        }

        /// <summary>
        /// Returns the casted int of a double
        /// </summary>
        /// <param name="v">Input</param>
        /// <param name="r">The return value</param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConvertDoubleToInt(in double v, ref int r, int n)
        {
            int i = 0;

            while (i < (n - 15))
            {
                Util.StoreV128(ref r, i, ConvertDoubleToInt(Util.LoadV256(in v, i)));
                i += 4;
                Util.StoreV128(ref r, i, ConvertDoubleToInt(Util.LoadV256(in v, i)));
                i += 4;
                Util.StoreV128(ref r, i, ConvertDoubleToInt(Util.LoadV256(in v, i)));
                i += 4;
                Util.StoreV128(ref r, i, ConvertDoubleToInt(Util.LoadV256(in v, i)));
                i += 4;
            }

            for (; i < (n - 3); i += 4)
                Util.StoreV128(ref r, i, ConvertDoubleToInt(Util.LoadV256(in v, i)));

            // clean up the residual
            for (; i < n; i++)
                Unsafe.Add(ref r, i) = (int)(Unsafe.Add(ref Unsafe.AsRef(in v), i));
        }

        /// <summary>
        /// Returns the casted int of a double
        /// </summary>
        /// <param name="v">Input</param>
        /// <param name="r">The return value</param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Max(in double a, in double b, ref double r, int n)
        {
            int i = 0;

            while (i < (n - 15))
            {
                Util.StoreV256(ref r, i, Max(Util.LoadV256(in a, i), Util.LoadV256(in b, i)));
                i += 4;
                Util.StoreV256(ref r, i, Max(Util.LoadV256(in a, i), Util.LoadV256(in b, i)));
                i += 4;
                Util.StoreV256(ref r, i, Max(Util.LoadV256(in a, i), Util.LoadV256(in b, i)));
                i += 4;
                Util.StoreV256(ref r, i, Max(Util.LoadV256(in a, i), Util.LoadV256(in b, i)));
                i += 4;
            }

            for (; i < (n - 3); i += 4)
                Util.StoreV256(ref r, i, Max(Util.LoadV256(in a, i), Util.LoadV256(in b, i)));

            // clean up the residual
            for (; i < n; i++)
                Unsafe.Add(ref r, i) = Math.Max(Unsafe.Add(ref Unsafe.AsRef(in a), i), Unsafe.Add(ref Unsafe.AsRef(in b), i));
        }

        /// <summary>
        /// Returns the min of two arrays
        /// </summary>
        /// <param name="v">Input</param>
        /// <param name="r">The return value</param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Min(in int a, in int b, ref int r, int n)
        {
            int i = 0;

            while (i < (n - 31))
            {
                Util.StoreV256(ref r, i, Min(Util.LoadV256(in a, i), Util.LoadV256(in b, i)));
                i += 8;
                Util.StoreV256(ref r, i, Min(Util.LoadV256(in a, i), Util.LoadV256(in b, i)));
                i += 8;
                Util.StoreV256(ref r, i, Min(Util.LoadV256(in a, i), Util.LoadV256(in b, i)));
                i += 8;
                Util.StoreV256(ref r, i, Min(Util.LoadV256(in a, i), Util.LoadV256(in b, i)));
                i += 8;
            }

            for (; i < (n - 7); i += 8)
                Util.StoreV256(ref r, i, Min(Util.LoadV256(in a, i), Util.LoadV256(in b, i)));

            // clean up the residual
            for (; i < n; i++)
                Unsafe.Add(ref r, i) = Math.Min(Unsafe.Add(ref Unsafe.AsRef(in a), i), Unsafe.Add(ref Unsafe.AsRef(in b), i));
        }

        /// <summary>
        /// Returns the max of two arrays
        /// </summary>
        /// <param name="v">Input</param>
        /// <param name="r">The return value</param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Max(in int a, in int b, ref int r, int n)
        {
            int i = 0;

            while (i < (n - 31))
            {
                Util.StoreV256(ref r, i, Max(Util.LoadV256(in a, i), Util.LoadV256(in b, i)));
                i += 8;
                Util.StoreV256(ref r, i, Max(Util.LoadV256(in a, i), Util.LoadV256(in b, i)));
                i += 8;
                Util.StoreV256(ref r, i, Max(Util.LoadV256(in a, i), Util.LoadV256(in b, i)));
                i += 8;
                Util.StoreV256(ref r, i, Max(Util.LoadV256(in a, i), Util.LoadV256(in b, i)));
                i += 8;
            }

            for (; i < (n - 7); i += 8)
                Util.StoreV256(ref r, i, Max(Util.LoadV256(in a, i), Util.LoadV256(in b, i)));

            // clean up the residual
            for (; i < n; i++)
                Unsafe.Add(ref r, i) = Math.Max(Unsafe.Add(ref Unsafe.AsRef(in a), i), Unsafe.Add(ref Unsafe.AsRef(in b), i));
        }

        /// <summary>
        /// Returns the sign of the array element
        /// note: zero returns 1
        /// </summary>
        /// <param name="v">Input</param>
        /// <param name="r">The return value</param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sign(in int a, ref int r, int n)
        {
            if (n < 8)
            {
                Span<int> tmp = new int[8];
                ref var tmpx = ref MemoryMarshal.GetReference(tmp);
                for (int j = 0; j < n; j++)
                    Unsafe.Add(ref tmpx, j) = Unsafe.Add(ref Unsafe.AsRef(in a), j);

                Util.StoreV256(ref tmpx, 0, Sign(Util.LoadV256(in tmpx, 0)));

                for (int j = 0; j < n; ++j)
                    Unsafe.Add(ref r, j) = Unsafe.Add(ref tmpx, j);
            }

            int i = 0;

            while (i < (n - 31))
            {
                Util.StoreV256(ref r, i, Sign(Util.LoadV256(in a, i)));
                i += 8;
                Util.StoreV256(ref r, i, Sign(Util.LoadV256(in a, i)));
                i += 8;
                Util.StoreV256(ref r, i, Sign(Util.LoadV256(in a, i)));
                i += 8;
                Util.StoreV256(ref r, i, Sign(Util.LoadV256(in a, i)));
                i += 8;
            }

            for (; i < (n - 7); i += 8)
                Util.StoreV256(ref r, i, Sign(Util.LoadV256(in a, i)));

            // Cleans up any excess individual values (if n%8 != 0)
            if (i != n)
            {
                i = n - 8;
                Util.StoreV256(ref r, i, Sign(Util.LoadV256(in a, i)));
            }
        }

        /// <summary>
        /// Returns the sign of the array element
        /// note: zero returns 1
        /// </summary>
        /// <param name="v">Input</param>
        /// <param name="r">The return value</param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sign(in double a, ref double r, int n)
        {

            if (n < 4)
            {
                Span<double> tmp = stackalloc double[4];
                ref var tmpx = ref MemoryMarshal.GetReference(tmp);
                for (int j = 0; j < n; j++)
                    Unsafe.Add(ref tmpx, j) = Unsafe.Add(ref Unsafe.AsRef(in a), j);

                Util.StoreV256(ref tmpx, 0, Sign(Util.LoadV256(in tmpx, 0)));

                for (int j = 0; j < n; ++j)
                    Unsafe.Add(ref r, j) = Unsafe.Add(ref tmpx, j);
            }

            int i = 0;

            while (i < (n - 15))
            {
                Util.StoreV256(ref r, i, Sign(Util.LoadV256(in a, i)));
                i += 4;
                Util.StoreV256(ref r, i, Sign(Util.LoadV256(in a, i)));
                i += 4;
                Util.StoreV256(ref r, i, Sign(Util.LoadV256(in a, i)));
                i += 4;
                Util.StoreV256(ref r, i, Sign(Util.LoadV256(in a, i)));
                i += 4;
            }

            for (; i < (n - 3); i += 4)
                Util.StoreV256(ref r, i, Sign(Util.LoadV256(in a, i)));

            // Cleans up any excess individual values (if n%4 != 0)
            if (i != n)
            {
                i = n - 4;
                Util.StoreV256(ref r, i, Sign(Util.LoadV256(in a, i)));
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Abs(ref Vector256<double> x, ref Vector256<double> y)
        {
            y = Avx.AndNot(Lit.Double.Util.NEGATIVE_ZERO, x);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Abs(ref Vector256<double> x)
        {
            return Avx.AndNot(Lit.Double.Util.NEGATIVE_ZERO, x);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Abs(Vector256<double> x)
        {
            return Avx.AndNot(Lit.Double.Util.NEGATIVE_ZERO, x);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConvertLongToDouble(in Vector256<long> x, ref Vector256<double> y)
        {
            y = Avx.Subtract(Vector256.AsDouble(Avx2.Add(x, Lit.Long.MAGIC_LONG_DOUBLE_ADD)), Lit.Double.Util.MAGIC_LONG_DOUBLE_ADD);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> ConvertLongToDouble(in Vector256<long> x)
        {
            return Avx.Subtract(Vector256.AsDouble(Avx2.Add(x, Lit.Long.MAGIC_LONG_DOUBLE_ADD)), Lit.Double.Util.MAGIC_LONG_DOUBLE_ADD);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConvertDoubleToInt(in Vector256<double> x, ref Vector128<int> y)
        {
            y = Avx.ConvertToVector128Int32WithTruncation(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<int> ConvertDoubleToInt(in Vector256<double> x)
        {
            return Avx.ConvertToVector128Int32WithTruncation(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<int> ConvertDoubleToInt(Vector256<double> x)
        {
            return Avx.ConvertToVector128Int32WithTruncation(x);
        }


        /// <summary>
        /// Converts x to 2^x
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConvertXtoPower(in Vector256<double> x, ref Vector256<double> y)
        {
            y = Avx.Add(x, Lit.Double.Util.MAGIC_LONG_DOUBLE_ADD);
            var z = Vector256.AsInt64(y);
            z = Avx2.Add(z, Lit.Long.ONE_THOUSAND_TWENTY_THREE);
            z = Avx2.ShiftLeftLogical(z, 52);
            y = Vector256.AsDouble(z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> IfLessThan(Vector256<double> x, Vector256<double> condition,
            Vector256<double> trueval, Vector256<double> falseval)
        {
            return IfElse(Avx.CompareLessThan(x, condition), trueval, falseval);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> IfGreaterThan(Vector256<double> x, Vector256<double> condition,
             Vector256<double> trueval, Vector256<double> falseval)
        {
            return IfElse(Avx.CompareGreaterThan(x, condition), trueval, falseval);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> IfElse(Vector256<double> mask, Vector256<double> trueval, Vector256<double> falseval)
        {
            return Avx.BlendVariable(falseval, trueval, mask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<int> IfElse(Vector256<int> mask, Vector256<int> trueval, Vector256<int> falseval)
        {
            return Avx2.BlendVariable(falseval, trueval, mask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Max(in Vector256<double> x, Vector256<double> y)
        {
            return IfElse(Avx.CompareGreaterThanOrEqual(x, y), x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Min(in Vector256<double> x, in Vector256<double> y)
        {
            return IfElse(Avx.CompareLessThanOrEqual(x, y), x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Max(Vector256<double> x, Vector256<double> y)
        {
            return IfElse(Avx.CompareGreaterThanOrEqual(x, y), x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Min(Vector256<double> x, Vector256<double> y)
        {
            return IfElse(Avx.CompareLessThanOrEqual(x, y), x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Sign(Vector256<double> x)
        {
            return IfElse(Avx.CompareGreaterThanOrEqual(x, Lit.Double.Util.ZERO),
                Lit.Double.Util.ONE,
                Lit.Double.Util.NEGONE);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<int> Sign(Vector256<int> x)
        {
            return Avx2.MultiplyLow(Avx2.Add(Avx2.MultiplyLow(
                Avx2.CompareGreaterThan(x, Lit.Int.NEGONE),
                Lit.Int.TWO),
                Lit.Int.ONE),
                Lit.Int.NEGONE);
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
            return IfElse(Avx.CompareLessThan(x, condition), trueval, falseval);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<float> IfElse(Vector256<float> mask, Vector256<float> trueval, Vector256<float> falseval)
        {
            return Avx2.BlendVariable(falseval, trueval, mask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<float> Max(in Vector256<float> x, in Vector256<float> y)
        {
            return IfElse(Avx.CompareGreaterThanOrEqual(x, y), x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<float> Min(in Vector256<float> x, in Vector256<float> y)
        {
            return IfElse(Avx.CompareLessThanOrEqual(x, y), x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<float> Max(Vector256<float> x, Vector256<float> y)
        {
            return IfElse(Avx.CompareGreaterThanOrEqual(x, y), x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<float> Min(Vector256<float> x, Vector256<float> y)
        {
            return IfElse(Avx.CompareLessThanOrEqual(x, y), x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<int> Max(Vector256<int> x, Vector256<int> y)
        {
            return IfElse(Avx2.CompareGreaterThan(x, y), x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<int> Min(Vector256<int> x, Vector256<int> y)
        {
            return IfElse(Avx2.CompareGreaterThan(x, y), y, x);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<int> IfLessThan(Vector128<int> x, Vector128<int> condition,
            Vector128<int> trueval, Vector128<int> falseval)
        {
            return Avx.Or(
                Avx.And(Avx.CompareLessThan(x, condition), trueval),
                Avx.And(Avx.Or(Avx.CompareEqual(x, condition), Avx.CompareGreaterThan(x, condition)), falseval));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<int> IfElse(Vector128<int> mask, Vector128<int> trueval, Vector128<int> falseval)
        {
            return Avx2.BlendVariable(falseval, trueval, mask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<int> Max(in Vector128<int> x, in Vector128<int> y)
        {
            return IfElse(Avx2.CompareGreaterThan(x, y), x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<int> Min(in Vector128<int> x, in Vector128<int> y)
        {
            return IfElse(Avx.CompareLessThan(x, y), x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<int> Max(Vector128<int> x, Vector128<int> y)
        {
            return IfElse(Avx2.CompareGreaterThan(x, y), x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<int> Min(Vector128<int> x, Vector128<int> min)
        {
            return IfElse(Avx.CompareLessThan(x, min), x, min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<T> LoadV256<T>(in T ptr, int offset) where T : unmanaged
        {
            return Unsafe.As<T, Vector256<T>>(ref Unsafe.Add(ref Unsafe.AsRef(in ptr), offset));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> CreateMaskDouble(int mask)
        {
            return Vector256.Create((mask & 1) == 0 ? 0 : ~0, (mask & 2) == 0 ? 0 : ~0, (mask & 4) == 0 ? 0 : ~0, (mask & 8) == 0 ? 0 : ~0).AsDouble();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<float> CreateMaskFloat(int mask)
        {
            return Vector256.Create((mask & 1) == 0 ? 0 : ~0, (mask & 2) == 0 ? 0 : ~0, (mask & 4) == 0 ? 0 : ~0, (mask & 8) == 0 ? 0 : ~0,
                                    (mask & 16) == 0 ? 0 : ~0, (mask & 32) == 0 ? 0 : ~0, (mask & 64) == 0 ? 0 : ~0, (mask & 128) == 0 ? 0 : ~0).AsSingle();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> LoadMaskedV256(in double ptr, int offset, in Vector256<double> mask)
        {
            unsafe
            {
                fixed (double* ptr_ = &Unsafe.Add(ref Unsafe.AsRef(in ptr), offset))
                    return Avx.MaskLoad(ptr_, mask);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<float> LoadMaskedV256(in float ptr, int offset, in Vector256<float> mask)
        {
            unsafe
            {
                fixed (float* ptr_ = &Unsafe.Add(ref Unsafe.AsRef(in ptr), offset))
                    return Avx.MaskLoad(ptr_, mask);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StoreV256<T>(ref T ptr, int offset, Vector256<T> value) where T : unmanaged
        {
            Unsafe.As<T, Vector256<T>>(ref Unsafe.Add(ref ptr, offset)) = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StoreMaskedV256(ref double ptr, int offset, Vector256<double> value, in Vector256<double> mask)
        {
            unsafe
            {
                fixed (double* ptr_ = &Unsafe.Add(ref Unsafe.AsRef(in ptr), offset))
                    Avx.MaskStore(ptr_, mask, value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StoreMaskedV256(ref float ptr, int offset, Vector256<float> value, in Vector256<float> mask)
        {
            unsafe
            {
                fixed (float* ptr_ = &Unsafe.Add(ref Unsafe.AsRef(in ptr), offset))
                    Avx.MaskStore(ptr_, mask, value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<T> LoadV128<T>(in T ptr, int offset) where T : unmanaged
        {
            return Unsafe.As<T, Vector128<T>>(ref Unsafe.Add(ref Unsafe.AsRef(in ptr), offset));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StoreV128<T>(ref T ptr, int offset, Vector128<T> value) where T : unmanaged
        {
            Unsafe.As<T, Vector128<T>>(ref Unsafe.Add(ref ptr, offset)) = value;
        }
    }

#if NET8_0_OR_GREATER
    public static class Util512
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector512<double> IfElse(Vector512<double> mask, Vector512<double> trueval, Vector512<double> falseval)
        {
            return Avx512F.BlendVariable(falseval, trueval, mask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector512<T> LoadV512<T>(in T ptr, int offset) where T : unmanaged
        {
            return Unsafe.As<T, Vector512<T>>(ref Unsafe.Add(ref Unsafe.AsRef(in ptr), offset));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StoreV512<T>(ref T ptr, int offset, Vector512<T> value) where T : unmanaged
        {
            Unsafe.As<T, Vector512<T>>(ref Unsafe.Add(ref ptr, offset)) = value;
        }
    }
#endif
}