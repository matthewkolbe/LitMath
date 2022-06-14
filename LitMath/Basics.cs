// Copyright Matthew Kolbe (2022)

using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;


namespace LitMath
{
    public static class LitBasics
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
            unsafe
            {
                fixed (double* vv = v) fixed (double* rr = r)
                    Multiply(vv, consant, rr, r.Length);
            }
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
            unsafe
            {
                fixed (float* vv = v) fixed (float* rr = r)
                    Multiply(vv, consant, rr, r.Length);
            }
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
            unsafe
            {
                fixed (double* xx = x) fixed (double* yy = y) fixed (double* rr = r)
                    Add(xx, yy, rr, r.Length);
            }
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
            unsafe
            {
                fixed (double* xx = x) fixed (double* yy = y) fixed (double* rr = r)
                    Subtract(xx, yy, rr, r.Length);
            }
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
            unsafe
            {
                fixed (double* xx = x) fixed (double* yy = y) fixed (double* rr = r)
                    Multiply(xx, yy, rr, r.Length);
            }
        }


        /// <summary>
        /// Does a dot product between two Spans
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="y"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Dot(ref Span<double> x, ref Span<double> y)
        {
            unsafe
            {
                fixed (double* xx = x) fixed (double* yy = y)
                    return Dot(xx, yy, x.Length);
            }
        }


        /// <summary>
        /// Does a dot product between two Spans
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="y"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(ref Span<float> x, ref Span<float> y)
        {
            unsafe
            {
                fixed (float* xx = x) fixed (float* yy = y)
                    return Dot(xx, yy, x.Length);
            }
        }


        /// <summary>
        /// Multiplies every element of an array by a constant
        /// </summary>
        /// <param name="v">Input</param>
        /// <param name="constant"></param>
        /// <param name="r">The return value (can be the same as v if you so desire this to happen in-place)</param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Multiply( double* v, double constant, double* r, int n)
        {
            var c = Vector256.Create(constant);
            int i = 0;

            // Unroll the loop if n > 16
            while (i < (n - 15))
            {
                Avx.Store(r + i, Avx.Multiply(Avx.LoadVector256(v + i), c));
                i += 4;
                Avx.Store(r + i, Avx.Multiply(Avx.LoadVector256(v + i), c));
                i += 4;
                Avx.Store(r + i, Avx.Multiply(Avx.LoadVector256(v + i), c));
                i += 4;
                Avx.Store(r + i, Avx.Multiply(Avx.LoadVector256(v + i), c));
                i += 4;
            }

            // Loop through the AVX instructions
            for (; i < (n - 3); i += 4)
                Avx.Store(r + i, Avx.Multiply(Avx.LoadVector256(v + i), c));


            // clean up the residual
            for (; i < n; i++)
                r[i] = v[i] * constant;
        }


        /// <summary>
        /// Multiplies every element of an array by a constant
        /// </summary>
        /// <param name="v">Input</param>
        /// <param name="constant"></param>
        /// <param name="r">The return value (can be the same as v if you so desire this to happen in-place)</param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Multiply( float* v, float constant, float* r, int n)
        {
            var c = Vector256.Create(constant);
            int i = 0;

            while (i < (n - 31))
            {
                Avx.Store(r + i, Avx.Multiply(Avx.LoadVector256(v + i), c));
                i += 8;
                Avx.Store(r + i, Avx.Multiply(Avx.LoadVector256(v + i), c));
                i += 8;
                Avx.Store(r + i, Avx.Multiply(Avx.LoadVector256(v + i), c));
                i += 8;
                Avx.Store(r + i, Avx.Multiply(Avx.LoadVector256(v + i), c));
                i += 8;
            }

            for (; i < (n - 7); i += 8)
                Avx.Store(r + i, Avx.Multiply(Avx.LoadVector256(v + i), c));

            // clean up the residual
            for (; i < n; i++)
                r[i] = v[i] * constant;
        }


        /// <summary>
        /// Computes the sum of all elements in a Vector256
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe double Aggregate(ref Vector256<double> v)
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
        public static unsafe float Aggregate(ref Vector256<float> v)
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
        public static unsafe void Add(double* x, double* y, double* r, int n)
        {
            int i = 0;

            for (; i < (n - 15); )
            {
                Avx.Store(r + i, Avx.Add(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i)));
                i += 4;
                Avx.Store(r + i, Avx.Add(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i)));
                i += 4;
                Avx.Store(r + i, Avx.Add(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i)));
                i += 4;
                Avx.Store(r + i, Avx.Add(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i)));
                i += 4;
            }

            for (; i < (n - 3); i += 4)
                Avx.Store(r + i, Avx.Add(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i)));

            // clean up the residual
            for (; i < n; i++)
                r[i] = x[i] + y[i];
        }


        /// <summary>
        /// Takes the difference between two arrays
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="y"></param>
        /// <param name="r">The return value (can be the same as x or y if you so desire this to happen in-place)</param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Subtract(double* x, double* y, double* r, int n)
        {
            int i = 0;

            for (; i < (n - 15);)
            {
                Avx.Store(r + i, Avx.Subtract(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i)));
                i += 4;
                Avx.Store(r + i, Avx.Subtract(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i)));
                i += 4;
                Avx.Store(r + i, Avx.Subtract(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i)));
                i += 4;
                Avx.Store(r + i, Avx.Subtract(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i)));
                i += 4;
            }

            for (; i < (n - 3); i += 4)
                Avx.Store(r + i, Avx.Subtract(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i)));

            // clean up the residual
            for (; i < n; i++)
                r[i] = x[i] - y[i];
        }


        /// <summary>
        /// Subtracts every element of an array by a constant
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="constant"></param>
        /// <param name="r">The return value (can be the same as x if you so desire this to happen in-place)</param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Subtract(double* x, double constant, double* r, int n)
        {
            int i = 0;
            var yy = Vector256.Create(constant);

            for (; i < (n - 15);)
            {
                Avx.Store(r + i, Avx.Subtract(Avx.LoadVector256(x + i), yy));
                i += 4;
                Avx.Store(r + i, Avx.Subtract(Avx.LoadVector256(x + i), yy));
                i += 4;
                Avx.Store(r + i, Avx.Subtract(Avx.LoadVector256(x + i), yy));
                i += 4;
                Avx.Store(r + i, Avx.Subtract(Avx.LoadVector256(x + i), yy));
                i += 4;
            }

            for (; i < (n - 3); i += 4)
                Avx.Store(r + i, Avx.Subtract(Avx.LoadVector256(x + i), yy));

            // clean up the residual
            for (; i < n; i++)
                r[i] = x[i] - constant;
        }


        /// <summary>
        /// Does an elementwise multiply between two arrays
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="y"></param>
        /// <param name="r">The return value (can be the same as x or y if you so desire this to happen in-place)</param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Multiply(double* x, double* y, double* r, int n)
        {
            int i = 0;

            while (i < (n - 15))
            {
                Avx.Store(r + i, Avx.Multiply(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i)));
                i += 4;

                Avx.Store(r + i, Avx.Multiply(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i)));
                i += 4;

                Avx.Store(r + i, Avx.Multiply(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i)));
                i += 4;

                Avx.Store(r + i, Avx.Multiply(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i)));
                i += 4;
            }

            for (; i < (n - 3); i += 4)
                Avx.Store(r + i, Avx.Multiply(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i)));

            // clean up the residual
            for (; i < n; i++)
                r[i] = x[i] * y[i];
        }


        /// <summary>
        /// Does a dot product between two array using fused multiply add. 
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="y"></param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe double Dot(double* x, double* y, int n)
        {
            int i = 0;
            var vr1 = Vector256<double>.Zero;

            if (n > 31)
            {
                var vr2 = Vector256<double>.Zero;
                var vr3 = Vector256<double>.Zero;
                var vr4 = Vector256<double>.Zero;

                while (i < (n - 31))
                {
                    vr1 = Fma.MultiplyAdd(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i), vr1);
                    i += 4;
                    vr2 = Fma.MultiplyAdd(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i), vr2);
                    i += 4;
                    vr3 = Fma.MultiplyAdd(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i), vr3);
                    i += 4;
                    vr4 = Fma.MultiplyAdd(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i), vr4);
                    i += 4;
                    vr1 = Fma.MultiplyAdd(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i), vr1);
                    i += 4;
                    vr2 = Fma.MultiplyAdd(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i), vr2);
                    i += 4;
                    vr3 = Fma.MultiplyAdd(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i), vr3);
                    i += 4;
                    vr4 = Fma.MultiplyAdd(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i), vr4);
                    i += 4;
                }

                vr3 = Avx.Add(vr3, vr4);
                vr1 = Avx.Add(vr1, vr2);
                vr1 = Avx.Add(vr1, vr3);
            }

            for (; i < (n - 3); i += 4)
                vr1 = Fma.MultiplyAdd(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i), vr1);

            var r = Aggregate(ref vr1);

            // clean up the residual without AVX
            for (; i < n; i++)
                r += x[i] * y[i];

            return r;
        }


        /// <summary>
        /// Does a dot product between two arrays
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="y"></param>
        /// <param name="n">Size of the array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe float Dot(float* x, float* y, int n)
        {
            int i = 0;
            var vr1 = Vector256<float>.Zero;

            if (n > 63)
            {
                var vr2 = Vector256<float>.Zero;
                var vr3 = Vector256<float>.Zero;
                var vr4 = Vector256<float>.Zero;

                while (i < (n - 63))
                {
                    vr1 = Fma.MultiplyAdd(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i), vr1);
                    i += 8;
                    vr2 = Fma.MultiplyAdd(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i), vr2);
                    i += 8;
                    vr3 = Fma.MultiplyAdd(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i), vr3);
                    i += 8;
                    vr4 = Fma.MultiplyAdd(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i), vr4);
                    i += 8;
                    vr1 = Fma.MultiplyAdd(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i), vr1);
                    i += 8;
                    vr2 = Fma.MultiplyAdd(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i), vr2);
                    i += 8;
                    vr3 = Fma.MultiplyAdd(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i), vr3);
                    i += 8;
                    vr4 = Fma.MultiplyAdd(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i), vr4);
                    i += 8;
                }

                vr3 = Avx.Add(vr3, vr4);
                vr1 = Avx.Add(vr1, vr2);
                vr1 = Avx.Add(vr1, vr3);
            }

            for (; i < (n - 7); i += 8)
                vr1 = Avx.Add(Avx.Multiply(Avx.LoadVector256(x + i), Avx.LoadVector256(y + i)), vr1);

            var r = Aggregate(ref vr1);

            // clean up the residual without AVX
            for (; i < n; i++)
                r += x[i] * y[i];

            return r;
        }

    }
}
