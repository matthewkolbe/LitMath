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
        /// Multiplies every element of a Span by a constant
        /// </summary>
        /// <param name="v">Input</param>
        /// <param name="constant"></param>
        /// <param name="r">The return value (can be the same as v if you so desire this to happen in-place)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply(ref Span<double> v, double consant, ref Span<double> r)
        {
            ref var vv = ref MemoryMarshal.GetReference(v);
            ref var rr = ref MemoryMarshal.GetReference(r);
            Multiply(ref vv, consant, ref rr, r.Length);

        }


        /// <summary>
        /// Multiplies every element of a Span by a constant
        /// </summary>
        /// <param name="v">Input</param>
        /// <param name="constant"></param>
        /// <param name="r">The return value (can be the same as v if you so desire this to happen in-place)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply(ref Span<float> v, float consant, ref Span<float> r)
        {
            ref var vv = ref MemoryMarshal.GetReference(v);
            ref var rr = ref MemoryMarshal.GetReference(r);
            Multiply(ref vv, consant, ref rr, r.Length);
        }


        /// <summary>
        /// Sums two Spans
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="y">Input</param>
        /// <param name="r">The return value (can be the same as x or y if you so desire this to happen in-place)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(ref Span<double> x, ref Span<double> y, ref Span<double> r)
        {
            ref var xx = ref MemoryMarshal.GetReference(x);
            ref var yy = ref MemoryMarshal.GetReference(y);
            ref var rr = ref MemoryMarshal.GetReference(r);
            Add(ref xx, ref yy, ref rr, r.Length);
        }


        /// <summary>
        /// Takes the difference between two Spans
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="y"></param>
        /// <param name="r">The return value (can be the same as x or y if you so desire this to happen in-place)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Subtract(ref Span<double> x, ref Span<double> y, ref Span<double> r)
        {
            ref var xx = ref MemoryMarshal.GetReference(x);
            ref var yy = ref MemoryMarshal.GetReference(y);
            ref var rr = ref MemoryMarshal.GetReference(r);
            Subtract(ref xx, ref yy, ref rr, r.Length);
        }


        /// <summary>
        /// Does an elementwise multiply between two Spans
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="y"></param>
        /// <param name="r">The return value (can be the same as x or y if you so desire this to happen in-place)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply(ref Span<double> x, ref Span<double> y, ref Span<double> r)
        {
            ref var xx = ref MemoryMarshal.GetReference(x);
            ref var yy = ref MemoryMarshal.GetReference(y);
            ref var rr = ref MemoryMarshal.GetReference(r);
            Multiply(ref xx, ref yy, ref rr, r.Length);
        }


        /// <summary>
        /// Does an fused multiply add between a span and two constants
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FusedMultiplyAdd(ref Span<double> v, double mult, double add, ref Span<double> r)
        {
            ref var vv = ref MemoryMarshal.GetReference(v);
            ref var rr = ref MemoryMarshal.GetReference(r);
            FusedMultiplyAdd(ref vv, mult, add, ref rr, r.Length);
        }


        /// <summary>
        /// Does an fused multiply add between a span and two constants
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FusedMultiplyAdd(ref Span<float> v, float mult, float add, ref Span<float> r)
        {
            ref var vv = ref MemoryMarshal.GetReference(v);
            ref var rr = ref MemoryMarshal.GetReference(r);
            FusedMultiplyAdd(ref vv, mult, add, ref rr, r.Length);
        }

        /// <summary>
        /// Does a dot product between two Spans
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="y"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Dot(ref Span<double> x, ref Span<double> y)
        {
            ref var xx = ref MemoryMarshal.GetReference(x);
            ref var yy = ref MemoryMarshal.GetReference(y);
            return Dot(ref xx, ref yy, x.Length);
        }


        /// <summary>
        /// Does a dot product between two Span<Vector256<double>>. This is a little different from the other
        /// dot products in this library, where instead of chunking a single array to make a dot product faster
        /// on a single computation, it uses Avx to do four dot products at once.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Dot(Span<Vector256<double>> a, Span<Vector256<double>> b)
        {
            return Dot(ref a, ref b);
        }

        /// <summary>
        /// Does a dot product between two Span<Vector256<double>>. This is a little different from the other
        /// dot products in this library, where instead of chunking a single array to make a dot product faster
        /// on a single computation, it uses Avx to do four dot products at once.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Dot(ref Span<Vector256<double>> a, ref Span<Vector256<double>> b)
        {
            if (a.Length == 1)
                return Avx.Multiply(a[0], b[0]);

            var r1 = Avx.Multiply(a[0], b[0]);
            var r2 = Avx.Multiply(a[1], b[1]);
            int i = 2;

            for (; i < (a.Length - 1);)
            {
                r1 = Fma.MultiplyAdd(a[i], b[i], r1);
                i++;
                r2 = Fma.MultiplyAdd(a[i], b[i], r2);
                i++;
            }

            if (i != a.Length)
                r1 = Fma.MultiplyAdd(a[i], b[i], r1);

            return Avx.Add(r1, r2);
        }


        /// <summary>
        /// Does a dot product between two Span<Vector256<double>>. This is a little different from the other
        /// dot products in this library, where instead of chunking a single array to make a dot product faster
        /// on a single computation, it uses Avx to do four dot products at once.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Dot(ref Span<Vector256<double>> a, ref Span<Vector256<double>> b, ref Vector256<double> r)
        {
            r = Dot(ref a, ref b);
        }



        /// <summary>
        /// Does a dot product between two Spans
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="y"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(ref Span<float> x, ref Span<float> y)
        {
            ref var xx = ref MemoryMarshal.GetReference(x);
            ref var yy = ref MemoryMarshal.GetReference(y);
            return Dot(ref xx, ref yy, x.Length);

        }


        /// <summary>
        /// Sums all the values of x
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="y"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Aggregate(ref Span<double> x)
        {
            ref var xx = ref MemoryMarshal.GetReference(x);
            return Aggregate(ref xx, x.Length);
        }


        /// <summary>
        /// Multiplies every element of an array by a constant
        /// </summary>
        /// <param name="v">Input</param>
        /// <param name="constant"></param>
        /// <param name="r">The return value (can be the same as v if you so desire this to happen in-place)</param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply(ref double v, double constant, ref double r, int n)
        {
            var c = Vector256.Create(constant);
            int i = 0;

            // Unroll the loop if n > 16
            while (i < (n - 15))
            {
                Util.StoreV256(ref r, i, Avx.Multiply(Util.LoadV256(ref v, i), c));
                i += 4;
                Util.StoreV256(ref r, i, Avx.Multiply(Util.LoadV256(ref v, i), c));
                i += 4;
                Util.StoreV256(ref r, i, Avx.Multiply(Util.LoadV256(ref v, i), c));
                i += 4;
                Util.StoreV256(ref r, i, Avx.Multiply(Util.LoadV256(ref v, i), c));
                i += 4;
            }

            // Loop through the AVX instructions
            for (; i < (n - 3); i += 4)
                Util.StoreV256(ref r, i, Avx.Multiply(Util.LoadV256(ref v, i), c));


            // clean up the residual
            for (; i < n; i++)
                Unsafe.Add(ref r, i) = Unsafe.Add(ref v, i) * constant;
        }


        /// <summary>
        /// Multiplies every element of an array by a constant
        /// </summary>
        /// <param name="v">Input</param>
        /// <param name="constant"></param>
        /// <param name="r">The return value (can be the same as v if you so desire this to happen in-place)</param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply(ref float v, float constant, ref float r, int n)
        {
            var c = Vector256.Create(constant);
            int i = 0;

            while (i < (n - 31))
            {
                Util.StoreV256(ref r, i, Avx.Multiply(Util.LoadV256(ref v, i), c));
                i += 8;
                Util.StoreV256(ref r, i, Avx.Multiply(Util.LoadV256(ref v, i), c));
                i += 8;
                Util.StoreV256(ref r, i, Avx.Multiply(Util.LoadV256(ref v, i), c));
                i += 8;
                Util.StoreV256(ref r, i, Avx.Multiply(Util.LoadV256(ref v, i), c));
                i += 8;
            }

            for (; i < (n - 7); i += 8)
                Util.StoreV256(ref r, i, Avx.Multiply(Util.LoadV256(ref v, i), c));

            // clean up the residual
            for (; i < n; i++)
                Unsafe.Add(ref r, i) = Unsafe.Add(ref v, i) * constant;
        }

        /// <summary>
        /// Adds every element of an array by a constant
        /// </summary>
        /// <param name="v">Input</param>
        /// <param name="constant"></param>
        /// <param name="r">The return value (can be the same as v if you so desire this to happen in-place)</param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(ref double v, double constant, ref double r, int n)
        {
            var c = Vector256.Create(constant);
            int i = 0;

            // Unroll the loop if n > 16
            while (i < (n - 15))
            {
                Util.StoreV256(ref r, i, Avx.Add(Util.LoadV256(ref v, i), c));
                i += 4;
                Util.StoreV256(ref r, i, Avx.Add(Util.LoadV256(ref v, i), c));
                i += 4;
                Util.StoreV256(ref r, i, Avx.Add(Util.LoadV256(ref v, i), c));
                i += 4;
                Util.StoreV256(ref r, i, Avx.Add(Util.LoadV256(ref v, i), c));
                i += 4;
            }

            // Loop through the AVX instructions
            for (; i < (n - 3); i += 4)
                Util.StoreV256(ref r, i, Avx.Add(Util.LoadV256(ref v, i), c));


            // clean up the residual
            for (; i < n; i++)
                Unsafe.Add(ref r, i) = Unsafe.Add(ref v, i) + constant;
        }

        /// <summary>
        /// Adds every element of an array by a constant
        /// </summary>
        /// <param name="v">Input</param>
        /// <param name="constant"></param>
        /// <param name="r">The return value (can be the same as v if you so desire this to happen in-place)</param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(ref float v, float constant, ref float r, int n)
        {
            var c = Vector256.Create(constant);
            int i = 0;

            while (i < (n - 31))
            {
                Util.StoreV256(ref r, i, Avx.Add(Util.LoadV256(ref v, i), c)); ;
                i += 8;
                Util.StoreV256(ref r, i, Avx.Add(Util.LoadV256(ref v, i), c));
                i += 8;
                Util.StoreV256(ref r, i, Avx.Add(Util.LoadV256(ref v, i), c));
                i += 8;
                Util.StoreV256(ref r, i, Avx.Add(Util.LoadV256(ref v, i), c));
                i += 8;
            }

            for (; i < (n - 7); i += 8)
                Util.StoreV256(ref r, i, Avx.Add(Util.LoadV256(ref v, i), c));

            // clean up the residual
            for (; i < n; i++)
                Unsafe.Add(ref r, i) = Unsafe.Add(ref v, i) + constant;
        }

        /// <summary>
        /// Fused multiply adds every element of an array by a constant
        /// </summary>
        /// <param name="v">Input</param>
        /// <param name="constant"></param>
        /// <param name="r">The return value (can be the same as v if you so desire this to happen in-place)</param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FusedMultiplyAdd(ref double v, double mult, double add, ref double r, int n)
        {
            var m = Vector256.Create(mult);
            var a = Vector256.Create(add);
            int i = 0;

            // Unroll the loop if n > 16
            while (i < (n - 15))
            {
                Util.StoreV256(ref  r, i, Fma.MultiplyAdd(Util.LoadV256(ref v, i), m, a));
                i += 4;
                Util.StoreV256(ref r, i, Fma.MultiplyAdd(Util.LoadV256(ref v, i), m, a));
                i += 4;
                Util.StoreV256(ref r, i, Fma.MultiplyAdd(Util.LoadV256(ref v, i), m, a));
                i += 4;
                Util.StoreV256(ref r, i, Fma.MultiplyAdd(Util.LoadV256(ref v, i), m, a));
                i += 4;
            }

            // Loop through the AVX instructions
            for (; i < (n - 3); i += 4)
                Util.StoreV256(ref r, i, Fma.MultiplyAdd(Util.LoadV256(ref v, i), m, a));


            // clean up the residual
            for (; i < n; i++)
                Unsafe.Add(ref r, i) = Unsafe.Add(ref v, i) * mult + add;
        }

        /// <summary>
        /// Fused multiply adds every element of an array by a constant
        /// </summary>
        /// <param name="v">Input</param>
        /// <param name="constant"></param>
        /// <param name="r">The return value (can be the same as v if you so desire this to happen in-place)</param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FusedMultiplyAdd(ref float v, float mult, float add, ref float r, int n)
        {
            var m = Vector256.Create(mult);
            var a = Vector256.Create(add);
            int i = 0;

            while (i < (n - 31))
            {
                Util.StoreV256(ref r, i, Fma.MultiplyAdd(Util.LoadV256(ref v, i), m, a));
                i += 8;
                Util.StoreV256(ref r, i, Fma.MultiplyAdd(Util.LoadV256(ref v, i), m, a));
                i += 8;
                Util.StoreV256(ref r, i, Fma.MultiplyAdd(Util.LoadV256(ref v, i), m, a));
                i += 8;
                Util.StoreV256(ref r, i, Fma.MultiplyAdd(Util.LoadV256(ref v, i), m, a));
                i += 8;
            }

            for (; i < (n - 7); i += 8)
                Util.StoreV256(ref r, i, Fma.MultiplyAdd(Util.LoadV256(ref v, i), m, a));

            // clean up the residual
            for (; i < n; i++)
                Unsafe.Add(ref r, i) = Unsafe.Add(ref v, i) * mult + add;
        }

        /// <summary>
        /// Computes the sum of all elements in a Vector256
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Aggregate(ref Vector256<double> v)
        {
            // https://stackoverflow.com/questions/49941645/get-sum-of-values-stored-in-m256d-with-sse-avx/49943540#49943540

            var low = Vector256.GetLower(v);
            low = Avx.Add(low, Avx.ExtractVector128(v, 1));
            low = Sse2.Add(low, Sse2.UnpackHigh(low, low));

            return low.GetElement(0);
        }


        /// <summary>
        /// Computes the sum of all elements in a Vector256
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Aggregate(ref Vector256<float> v)
        {
            var low = Vector256.GetLower(v);
            low = Avx.Add(low, Avx.ExtractVector128(v, 3));

            low = Avx.HorizontalAdd(low, low);
            low = Avx.HorizontalAdd(low, low);

            return low.GetElement(0);
        }


        /// <summary>
        /// Sums two arrays
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="y">Input</param>
        /// <param name="r">The return value (can be the same as x or y if you so desire this to happen in-place)</param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(ref double x, ref double y, ref double r, int n)
        {
            int i = 0;

            for (; i < (n - 15); )
            {
                Util.StoreV256(ref r, i, Avx.Add(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i)));
                i += 4;
                Util.StoreV256(ref r, i, Avx.Add(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i)));
                i += 4;
                Util.StoreV256(ref r, i, Avx.Add(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i)));
                i += 4;
                Util.StoreV256(ref r, i, Avx.Add(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i)));
                i += 4;
            }

            for (; i < (n - 3); i += 4)
                Util.StoreV256(ref r, i, Avx.Add(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i)));

            // clean up the residual
            for (; i < n; i++)
                Unsafe.Add(ref r, i) = Unsafe.Add(ref x, i) + Unsafe.Add(ref y, i);
        }


        /// <summary>
        /// Takes the difference between two arrays
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="y"></param>
        /// <param name="r">The return value (can be the same as x or y if you so desire this to happen in-place)</param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Subtract(ref double x, ref double y, ref double r, int n)
        {
            int i = 0;

            for (; i < (n - 15);)
            {
                Util.StoreV256(ref r, i, Avx.Subtract(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i)));
                i += 4;
                Util.StoreV256(ref r, i, Avx.Subtract(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i)));
                i += 4;
                Util.StoreV256(ref r, i, Avx.Subtract(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i)));
                i += 4;
                Util.StoreV256(ref r, i, Avx.Subtract(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i)));
                i += 4;
            }

            for (; i < (n - 3); i += 4)
                Util.StoreV256(ref r, i, Avx.Subtract(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i)));

            // clean up the residual
            for (; i < n; i++)
                Unsafe.Add(ref r, i) = Unsafe.Add(ref x, i) - Unsafe.Add(ref y, i);
        }


        /// <summary>
        /// Subtracts every element of an array by a constant
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="constant"></param>
        /// <param name="r">The return value (can be the same as x if you so desire this to happen in-place)</param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Subtract(ref double x, double constant, ref double r, int n)
        {
            int i = 0;
            var yy = Vector256.Create(constant);

            for (; i < (n - 15);)
            {
                Util.StoreV256(ref r, i, Avx.Subtract(Util.LoadV256(ref x, i), yy));
                i += 4;
                Util.StoreV256(ref r, i, Avx.Subtract(Util.LoadV256(ref x, i), yy));
                i += 4;
                Util.StoreV256(ref r, i, Avx.Subtract(Util.LoadV256(ref x, i), yy));
                i += 4;
                Util.StoreV256(ref r, i, Avx.Subtract(Util.LoadV256(ref x, i), yy));
                i += 4;
            }

            for (; i < (n - 3); i += 4)
                Util.StoreV256(ref r, i, Avx.Subtract(Util.LoadV256(ref x, i), yy));

            // clean up the residual
            for (; i < n; i++)
                Unsafe.Add(ref r, i) = Unsafe.Add(ref x, i) - constant;
        }


        /// <summary>
        /// Does an elementwise multiply between two arrays
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="y"></param>
        /// <param name="r">The return value (can be the same as x or y if you so desire this to happen in-place)</param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply(ref double x, ref double y, ref double r, int n)
        {
            const int VSZ = 4;
            int i = 0;

            while (i < (n - 15))
            {
                Util.StoreV256(ref r, i, Avx.Multiply(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i)));
                i += VSZ;

                Util.StoreV256(ref r, i, Avx.Multiply(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i)));
                i += VSZ;

                Util.StoreV256(ref r, i, Avx.Multiply(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i)));
                i += VSZ;

                Util.StoreV256(ref r, i, Avx.Multiply(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i)));
                i += VSZ;
            }

            for (; i < (n - 3); i += 4)
                Util.StoreV256(ref r, i, Avx.Multiply(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i)));

            // clean up the residual
            for (; i < n; i++)
                Unsafe.Add(ref r, i) = Unsafe.Add(ref x, i) * Unsafe.Add(ref y, i);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Dot(ref Vector256<double>[] a, ref Vector256<double>[] b, ref Vector256<double> r, int n)
        {
            int i = 0;
            r = Vector256<double>.Zero;

            if (n > 7)
            {
                var vr2 = Vector256<double>.Zero;
                var vr3 = Vector256<double>.Zero;
                var vr4 = Vector256<double>.Zero;

                while (i < (n - 7))
                {
                    r = Fma.MultiplyAdd(a[i], b[i], r);
                    i++;
                    vr2 = Fma.MultiplyAdd(a[i], b[i], vr2);
                    i++;
                    vr3 = Fma.MultiplyAdd(a[i], b[i], vr3);
                    i++;
                    vr4 = Fma.MultiplyAdd(a[i], b[i], vr4);
                    i++;
                    r = Fma.MultiplyAdd(a[i], b[i], r);
                    i++;
                    vr2 = Fma.MultiplyAdd(a[i], b[i], vr2);
                    i++;
                    vr3 = Fma.MultiplyAdd(a[i], b[i], vr3);
                    i++;
                    vr4 = Fma.MultiplyAdd(a[i], b[i], vr4);
                    i++;
                }

                vr3 = Avx.Add(vr3, vr4);
                r = Avx.Add(r, vr2);
                r = Avx.Add(r, vr3);
            }

            for (; i < n; i++)
                r = Fma.MultiplyAdd(a[i], b[i], r);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Dot(ref Span<Vector256<double>> a, ref Vector256<double>[] b, ref Vector256<double> r, int n)
        {
            int i = 0;
            r = Vector256<double>.Zero;

            if (n > 7)
            {
                var vr2 = Vector256<double>.Zero;
                var vr3 = Vector256<double>.Zero;
                var vr4 = Vector256<double>.Zero;

                while (i < (n - 7))
                {
                    r = Fma.MultiplyAdd(a[i], b[i], r);
                    i++;
                    vr2 = Fma.MultiplyAdd(a[i], b[i], vr2);
                    i++;
                    vr3 = Fma.MultiplyAdd(a[i], b[i], vr3);
                    i++;
                    vr4 = Fma.MultiplyAdd(a[i], b[i], vr4);
                    i++;
                    r = Fma.MultiplyAdd(a[i], b[i], r);
                    i++;
                    vr2 = Fma.MultiplyAdd(a[i], b[i], vr2);
                    i++;
                    vr3 = Fma.MultiplyAdd(a[i], b[i], vr3);
                    i++;
                    vr4 = Fma.MultiplyAdd(a[i], b[i], vr4);
                    i++;
                }

                vr3 = Avx.Add(vr3, vr4);
                r = Avx.Add(r, vr2);
                r = Avx.Add(r, vr3);
            }

            for (; i < n; i++)
                r = Fma.MultiplyAdd(a[i], b[i], r);
        }


        /// <summary>
        /// Does a dot product between two array using fused multiply add. 
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="y"></param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Dot(ref double x, ref double y, int n)
        {
            const int VSZ = 4;
            int i = 0;
            var vr1 = Vector256<double>.Zero;

            if (n > 31)
            {
                var vr2 = Vector256<double>.Zero;
                var vr3 = Vector256<double>.Zero;
                var vr4 = Vector256<double>.Zero;

                while (i < (n - 31))
                {
                    vr1 = Fma.MultiplyAdd(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i), vr1);
                    i += VSZ;
                    vr2 = Fma.MultiplyAdd(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i), vr2);
                    i += VSZ;
                    vr3 = Fma.MultiplyAdd(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i), vr3);
                    i += VSZ;
                    vr4 = Fma.MultiplyAdd(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i), vr4);
                    i += VSZ;
                    vr1 = Fma.MultiplyAdd(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i), vr1);
                    i += VSZ;
                    vr2 = Fma.MultiplyAdd(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i), vr2);
                    i += VSZ;
                    vr3 = Fma.MultiplyAdd(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i), vr3);
                    i += VSZ;
                    vr4 = Fma.MultiplyAdd(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i), vr4);
                    i += VSZ;
                }

                vr3 = Avx.Add(vr3, vr4);
                vr1 = Avx.Add(vr1, vr2);
                vr1 = Avx.Add(vr1, vr3);
            }

            for (; i < (n - 3); i += VSZ)
                vr1 = Fma.MultiplyAdd(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i), vr1);

            var r = Aggregate(ref vr1);

            // clean up the residual without AVX
            if (i != n)
            {
                for (; i < n; ++i)
                    r += Unsafe.Add(ref x, i) * Unsafe.Add(ref y, i);
            }

            return r;
        }


        /// <summary>
        /// Does a dot product between two arrays
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="y"></param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(ref float x, ref float y, int n)
        {
            const int VSZ = 8;
            int i = 0;
            var vr1 = Vector256<float>.Zero;

            if (n > 31)
            {
                var vr2 = Vector256<float>.Zero;
                var vr3 = Vector256<float>.Zero;
                var vr4 = Vector256<float>.Zero;

                while (i < (n - 31))
                {
                    vr1 = Fma.MultiplyAdd(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i), vr1);
                    i += VSZ;
                    vr2 = Fma.MultiplyAdd(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i), vr2);
                    i += VSZ;
                    vr3 = Fma.MultiplyAdd(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i), vr3);
                    i += VSZ;
                    vr4 = Fma.MultiplyAdd(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i), vr4);
                    i += VSZ;
                }

                vr3 = Avx.Add(vr3, vr4);
                vr1 = Avx.Add(vr1, vr2);
                vr1 = Avx.Add(vr1, vr3);
            }

            for (; i < (n - 7); i += VSZ)
                vr1 = Fma.MultiplyAdd(Util.LoadV256(ref x, i), Util.LoadV256(ref y, i), vr1);

            var r = Aggregate(ref vr1);

            // clean up the residual without AVX
            if (i != n)
            {
                for (; i < n; ++i)
                    r += Unsafe.Add(ref x, i) * Unsafe.Add(ref y, i);
            }

            return r;
        }



        /// <summary>
        /// Sums all the elements of x
        /// </summary>
        /// <param name="x"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double Aggregate(ref double x, int n)
        {
            const int VSZ = 4;
            int i = 0;
            var vr1 = Vector256<double>.Zero;

            if (n > 31)
            {
                var vr2 = Vector256<double>.Zero;
                var vr3 = Vector256<double>.Zero;
                var vr4 = Vector256<double>.Zero;

                while (i < (n - 31))
                {
                    vr1 = Avx.Add(Util.LoadV256(ref x, i), vr1);
                    i += VSZ;
                    vr2 = Avx.Add(Util.LoadV256(ref x, i), vr2);
                    i += VSZ;
                    vr3 = Avx.Add(Util.LoadV256(ref x, i), vr3);
                    i += VSZ;
                    vr4 = Avx.Add(Util.LoadV256(ref x, i), vr4);
                    i += VSZ;
                    vr1 = Avx.Add(Util.LoadV256(ref x, i), vr1);
                    i += VSZ;
                    vr2 = Avx.Add(Util.LoadV256(ref x, i), vr2);
                    i += VSZ;
                    vr3 = Avx.Add(Util.LoadV256(ref x, i), vr3);
                    i += VSZ;
                    vr4 = Avx.Add(Util.LoadV256(ref x, i), vr4);
                    i += VSZ;
                }

                vr3 = Avx.Add(vr3, vr4);
                vr1 = Avx.Add(vr1, vr2);
                vr1 = Avx.Add(vr1, vr3);
            }

            for (; i < (n - 3); i += 4)
                vr1 = Avx.Add(Util.LoadV256(ref x, i), vr1);

            var r = Aggregate(ref vr1);

            // clean up the residual without AVX
            for (; i < n; i++)
                r += Unsafe.Add(ref x, i);

            return r;
        }
    }
}
