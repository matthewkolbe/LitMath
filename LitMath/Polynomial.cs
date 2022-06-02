
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace LitMath
{
    public class LitPolynomial
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void OrderN(ref Vector256<double> x, ref Vector256<double>[] p, ref Vector256<double> r, int n)
        {
            r = p[n - 1];

            for(int i = n - 2; i >= 0; i--)
            {
                r = Avx.Multiply(r, x);
                r = Avx.Add(r, p[i]);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void OrderN(ref Vector256<float> x, ref Vector256<float>[] p, ref Vector256<float> r, int n)
        {
            r = p[n - 1];

            for (int i = n - 2; i >= 0; i--)
            {
                r = Avx.Multiply(r, x);
                r = Avx.Add(r, p[i]);
            }
        }


        /// <summary>
        /// Computes a polynomial of order p.Length via AVX-256 intrinsics
        /// </summary>
        /// <param name="x">Arguments</param>
        /// <param name="p">The polynomial coefficients where the index corresponds to the order</param>
        /// <param name="y">The result</param>
        public static void Compute(ref Span<double> x, ref Span<double> p, ref Span<double> y)
        {
            unsafe
            {
                fixed (double* xx = x) fixed (double* pp = p) fixed (double* yy = y)
                    Compute(xx, pp, yy, x.Length, p.Length);
            }
        }


        /// <summary>
        /// Computes a polynomial of order p.Length via AVX-256 intrinsics
        /// </summary>
        /// <param name="x">Arguments</param>
        /// <param name="p">The polynomial coefficients where the index corresponds to the order</param>
        /// <param name="y">The result</param>
        public static void Compute(ref Span<float> x, ref Span<float> p, ref Span<float> y)
        {
            unsafe
            {
                fixed (float* xx = x) fixed (float* pp = p) fixed (float* yy = y)
                    Compute(xx, pp, yy, x.Length, p.Length);
            }
        }


        /// <summary>
        /// Computes a polynomial of order order via AVX-256 intrinsics
        /// </summary>
        /// <param name="x">Arguments</param>
        /// <param name="p">The polynomial coefficients where the index corresponds to the order</param>
        /// <param name="y">The result</param>
        /// <param name="N">Length of x</param>
        /// <param name="order">Length of p</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Compute(double* x, double* p, double* r, int N, int order)
        {
            var pp = GC.AllocateUninitializedArray<Vector256<double>>(order);

            for(int i = 0; i < order; ++i)
                pp[i] = Vector256.Create(p[i]);

            var rr = Vector256.Create(0.0);
            Vector256<double> xx;

            for (int i = 0; i < N; i += 4)
            {
                xx = Avx.LoadVector256(x + i);
                OrderN(ref xx, ref pp, ref rr, order);
                Avx.Store(r + i, rr);
            }
        }


        /// <summary>
        /// Computes a polynomial of order order via AVX-256 intrinsics
        /// </summary>
        /// <param name="x">Arguments</param>
        /// <param name="p">The polynomial coefficients where the index corresponds to the order</param>
        /// <param name="y">The result</param>
        /// <param name="N">Length of x</param>
        /// <param name="order">Length of p</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Compute(float* x, float* p, float* r, int N, int order)
        {
            var pp = GC.AllocateUninitializedArray<Vector256<float>>(order);

            for (int i = 0; i < order; ++i)
                pp[i] = Vector256.Create(p[i]);

            var rr = Vector256.Create(0.0f);
            Vector256<float> xx;

            for (int i = 0; i < N; i += 8)
            {
                xx = Avx.LoadVector256(x + i);
                OrderN(ref xx, ref pp, ref rr, order);
                Avx.Store(r + i, rr);
            }
        }
    }
}
