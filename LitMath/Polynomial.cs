// Copyright Matthew Kolbe (2022)

using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace LitMath
{
    public class LitPolynomial
    {
        /// <summary>
        /// Returns the value of 4 polynomials of order n using AVX-256 intrinsics
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Value(ref Vector256<double> x, ref Vector256<double>[] p, ref Vector256<double> r, int n)
        {
            r = p[n - 1];

            for (int i = n - 2; i >= 0; i--)
                r = Fma.MultiplyAdd(r, x, p[i]);

        }


        /// <summary>
        /// Returns the value of 4 polynomials of order p.Length using AVX-256 intrinsics
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Value(ref Vector256<double> x, ref Span<Vector256<double>> p, ref Vector256<double> r)
        {
            r = p[p.Length - 1];

            for (int i = p.Length - 2; i >= 0; i--)
                r = Fma.MultiplyAdd(r, x, p[i]);

        }


        /// <summary>
        /// Returns the value of 4 polynomials of order p.Length using AVX-256 intrinsics
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Value(ref Vector256<double> x, ref Span<Vector256<double>> p)
        {
            var r = p[p.Length - 1];

            for (int i = p.Length - 2; i >= 0; i--)
                r = Fma.MultiplyAdd(r, x, p[i]);

            return r;
        }


        /// <summary>
        /// Returns the derivative of 4 polynomials of order n using AVX-256 intrinsics
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Derivative(ref Vector256<double> x, ref Vector256<double>[] p, ref Vector256<double> r, int n)
        {
            r = Avx.Multiply(Vector256.Create((double)(n - 1)), p[p.Length - 1]);

            for (int i = n - 2; i >= 1; i--)
                r = Fma.MultiplyAdd(r, x, Avx.Multiply(Vector256.Create((double)i), p[i]));
        }


        /// <summary>
        /// Returns the derivative of 4 polynomials of order p.Length using AVX-256 intrinsics
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Derivative(ref Vector256<double> x, ref Span<Vector256<double>> p, ref Vector256<double> r)
        {
            r = Avx.Multiply(Vector256.Create((double)(p.Length - 1)), p[p.Length - 1]);

            for (int i = p.Length - 2; i >= 1; i--)
                r = Fma.MultiplyAdd(r, x, Avx.Multiply(Vector256.Create((double)i), p[i]));
        }


        /// <summary>
        /// Returns the derivative of 4 polynomials of order p.Length using AVX-256 intrinsics
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> Derivative(ref Vector256<double> x, ref Span<Vector256<double>> p)
        {
            var r = p[p.Length - 1];

            for (int i = p.Length - 2; i >= 1; i--)
                r = Fma.MultiplyAdd(r, x, Avx.Multiply(Vector256.Create((double)i), p[i]));

            return r;
        }

        /// <summary>
        /// Returns the result and derivative of the Chebyshev First Kind polynomial evaluated at x
        /// </summary>
        /// <param name="x"></param>
        /// <param name="r">Result</param>
        /// <param name="d">Derivative</param>
        /// <param name="n"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ChebyshevFirst(ref Vector256<double> x, ref Vector256<double> r, ref Vector256<double> d, ref Span<double> coefficients)
        {
            Span<Vector256<double>> rr = stackalloc Vector256<double>[coefficients.Length];
            Span<Vector256<double>> dd = stackalloc Vector256<double>[coefficients.Length];
            Span<Vector256<double>> cc = stackalloc Vector256<double>[coefficients.Length];

            rr[0] = Vector256.Create(1.0);
            rr[1] = x;

            dd[0] = Vector256.Create(0.0);
            dd[1] = Vector256.Create(1.0);

            cc[0] = Vector256.Create(coefficients[0]);
            cc[1] = Vector256.Create(coefficients[1]);

            var x2 = Avx.Add(x, x);

            for (int i = 2; i < coefficients.Length; i++)
            {
                cc[i] = Vector256.Create(coefficients[i]);
                rr[i] = Fma.MultiplySubtract(x2, rr[i - 1], rr[i - 2]);
                dd[i] = Avx.Subtract(Fma.MultiplyAdd(LitConstants.Double.Polynomial.TWO, Avx.Multiply(dd[i - 1], x), rr[i - 1]), dd[i - 2]);
            }

            LitBasics.Dot(ref cc, ref rr, ref r);
            LitBasics.Dot(ref cc, ref dd, ref d);
        }


        /// <summary>
        /// Returns the result and derivative of the Chebyshev Second Kind polynomial evaluated at x
        /// </summary>
        /// <param name="x"></param>
        /// <param name="r">Result</param>
        /// <param name="d">Derivative</param>
        /// <param name="n"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ChebyshevSecond(ref Vector256<double> x, ref Vector256<double> r, ref Vector256<double> d, ref Span<double> coefficients)
        {
            Span<Vector256<double>> rr = stackalloc Vector256<double>[coefficients.Length];
            Span<Vector256<double>> dd = stackalloc Vector256<double>[coefficients.Length];
            Span<Vector256<double>> cc = stackalloc Vector256<double>[coefficients.Length];

            var x2 = Avx.Add(x, x);

            rr[0] = Vector256.Create(1.0);
            rr[1] = x2;

            dd[0] = Vector256.Create(0.0);
            dd[1] = Vector256.Create(2.0);

            cc[0] = Vector256.Create(coefficients[0]);
            cc[1] = Vector256.Create(coefficients[1]);

            for (int i = 2; i < coefficients.Length; i++)
            {
                cc[i] = Vector256.Create(coefficients[i]);
                rr[i] = Fma.MultiplySubtract(x2, rr[i - 1], rr[i - 2]);
                dd[i] = Avx.Subtract(Fma.MultiplyAdd(LitConstants.Double.Polynomial.TWO, Avx.Multiply(dd[i - 1], x), rr[i - 1]), dd[i - 2]);
            }

            LitBasics.Dot(ref cc, ref rr, ref r);
            LitBasics.Dot(ref cc, ref dd, ref d);
        }


        /// <summary>
        /// Returns the result of the Chebyshev Second Kind polynomial evaluated at x, but with extrapolation of and exponential type
        /// for values outside of[-1, 1]
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void SecondKindExpExtrapolation(Vector256<double> x, ref Vector256<double> r, ref Span<double> coefficients)
        {
            Vector256<double> d = Vector256.Create(0.0);
            var mask = Avx.Or(
                            Avx.CompareLessThanOrEqual(x, LitConstants.Double.Polynomial.NEGONE),
                            Avx.CompareGreaterThanOrEqual(x, LitConstants.Double.Polynomial.ONE));

            var xx = LitUtilities.Min(LitUtilities.Max(x, LitConstants.Double.Polynomial.NEGONE), LitConstants.Double.Polynomial.ONE);

            ChebyshevSecond(ref xx, ref r, ref d, ref coefficients);

            var out_range_r = r;

            // compute the out of range value
            var sign = LitUtilities.Sign(Avx.Multiply(LitConstants.Double.Polynomial.NEGONE, x));
            var arg = Avx.Add(Avx.Multiply(LitUtilities.Abs(Avx.Subtract(x, xx)), LitConstants.Double.Polynomial.NEGONE), LitConstants.Double.Polynomial.NEGONE);
            out_range_r = Avx.Add(Avx.Multiply(Avx.Multiply(d, LitExp.Exp(arg)), sign), out_range_r);

            r = LitUtilities.IfElse(mask, out_range_r, r);
        }


        /// <summary>
        /// Returns the result of the Chebyshev First Kind polynomial evaluated at x, but with extrapolation of and exponential type
        /// for values outside of[-1, 1]
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void FirstKindExpExtrapolation(Vector256<double> x, ref Vector256<double> r, ref Span<double> coefficients)
        {
            Vector256<double> d = Vector256.Create(0.0);
            var mask = Avx.Or(
                            Avx.CompareLessThanOrEqual(x, LitConstants.Double.Polynomial.NEGONE),
                            Avx.CompareGreaterThanOrEqual(x, LitConstants.Double.Polynomial.ONE));

            var xx = LitUtilities.Min(LitUtilities.Max(x, LitConstants.Double.Polynomial.NEGONE), LitConstants.Double.Polynomial.ONE);

            ChebyshevFirst(ref xx, ref r, ref d, ref coefficients);

            var out_range_r = r;

            // compute the out of range value
            var sign = LitUtilities.Sign(Avx.Multiply(LitConstants.Double.Polynomial.NEGONE, x));
            var arg = Avx.Add(Avx.Multiply(LitUtilities.Abs(Avx.Subtract(x, xx)), LitConstants.Double.Polynomial.NEGONE), LitConstants.Double.Polynomial.NEGONE);
            out_range_r = Avx.Add(Avx.Multiply(Avx.Multiply(d, LitExp.Exp(arg)), sign), out_range_r);

            r = LitUtilities.IfElse(mask, out_range_r, r);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Value(ref Vector256<float> x, ref Vector256<float>[] p, ref Vector256<float> r, int n)
        {
            r = p[n - 1];

            for (int i = n - 2; i >= 0; i--)
                r = Fma.MultiplyAdd(r, x, p[i]);
            
        }


        /// <summary>
        /// Computes a polynomial of order p.Length via AVX-256 intrinsics
        /// </summary>
        /// <param name="x">Arguments</param>
        /// <param name="p">The polynomial coefficients where the index corresponds to the order</param>
        /// <param name="y">The result</param>
        public static void Value(ref Span<double> x, ref Span<double> p, ref Span<double> y)
        {
            unsafe
            {
                fixed (double* xx = x) fixed (double* pp = p) fixed (double* yy = y)
                    Value(xx, pp, yy, x.Length, p.Length);
            }
        }

        /// <summary>
        /// Computes a polynomial derivative of order p.Length via AVX-256 intrinsics
        /// </summary>
        /// <param name="x">Arguments</param>
        /// <param name="p">The polynomial coefficients where the index corresponds to the order</param>
        /// <param name="y">The result</param>
        public static void Derivative(ref Span<double> x, ref Span<double> p, ref Span<double> y)
        {
            unsafe
            {
                fixed (double* xx = x) fixed (double* pp = p) fixed (double* yy = y)
                    Derivative(xx, pp, yy, x.Length, p.Length);
            }
        }


        /// <summary>
        /// Computes a polynomial of order p.Length via AVX-256 intrinsics
        /// </summary>
        /// <param name="x">Arguments</param>
        /// <param name="p">The polynomial coefficients where the index corresponds to the order</param>
        /// <param name="y">The result</param>
        public static Span<double> Value(ref Span<double> x, ref Span<double> p)
        {
            var y = GC.AllocateUninitializedArray<double>(x.Length);

            unsafe
            {
                fixed (double* xx = x) fixed (double* pp = p) fixed (double* yy = y)
                    Value(xx, pp, yy, x.Length, p.Length);
            }

            return y;
        }

        /// <summary>
        /// Computes a polynomial derivative of order p.Length via AVX-256 intrinsics
        /// </summary>
        /// <param name="x">Arguments</param>
        /// <param name="p">The polynomial coefficients where the index corresponds to the order</param>
        /// <param name="y">The result</param>
        public static Span<double> Derivative(ref Span<double> x, ref Span<double> p)
        {
            var y = GC.AllocateUninitializedArray<double>(x.Length);
            
            unsafe
            {
                fixed (double* xx = x) fixed (double* pp = p) fixed (double* yy = y)
                    Derivative(xx, pp, yy, x.Length, p.Length);
            }

            return y;
        }


        /// <summary>
        /// Computes a polynomial of order p.Length via AVX-256 intrinsics
        /// </summary>
        /// <param name="x">Arguments</param>
        /// <param name="p">The polynomial coefficients where the index corresponds to the order</param>
        /// <param name="y">The result</param>
        public static void Value(ref Span<float> x, ref Span<float> p, ref Span<float> y)
        {
            unsafe
            {
                fixed (float* xx = x) fixed (float* pp = p) fixed (float* yy = y)
                    Value(xx, pp, yy, x.Length, p.Length);
            }
        }


        /// <summary>
        /// Computes a polynomial of order order via AVX-256 intrinsics
        /// </summary>
        /// <param name="x">Arguments</param>
        /// <param name="p">The polynomial coefficients where the index corresponds to the order</param>
        /// <param name="y">The result</param>
        /// <param name="N">Length of x (must be mod4)</param>
        /// <param name="order">Length of p</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Value(double* x, double* p, double* r, int N, int order)
        {
            var pp = GC.AllocateUninitializedArray<Vector256<double>>(order);

            for (int i = 0; i < order; ++i)
                pp[i] = Vector256.Create(p[i]);

            var rr = Vector256.Create(0.0);
            Vector256<double> xx;

            for (int i = 0; i < N; i += 4)
            {
                xx = Avx.LoadVector256(x + i);
                Value(ref xx, ref pp, ref rr, order);
                Avx.Store(r + i, rr);
            }
        }


        /// <summary>
        /// Computes a polynomial derivatve of order order via AVX-256 intrinsics
        /// </summary>
        /// <param name="x">Arguments</param>
        /// <param name="p">The polynomial coefficients where the index corresponds to the order</param>
        /// <param name="y">The result</param>
        /// <param name="N">Length of x (must be mod4)</param>
        /// <param name="order">Length of p</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Derivative(double* x, double* p, double* r, int N, int order)
        {
            var pp = GC.AllocateUninitializedArray<Vector256<double>>(order);

            for (int i = 0; i < order; ++i)
                pp[i] = Vector256.Create(p[i]);

            var rr = Vector256.Create(0.0);
            Vector256<double> xx;

            for (int i = 0; i < N; i += 4)
            {
                xx = Avx.LoadVector256(x + i);
                Derivative(ref xx, ref pp, ref rr, order);
                Avx.Store(r + i, rr);
            }
        }


        /// <summary>
        /// Computes a polynomial of order order via AVX-256 intrinsics
        /// </summary>
        /// <param name="x">Arguments</param>
        /// <param name="p">The polynomial coefficients where the index corresponds to the order</param>
        /// <param name="y">The result</param>
        /// <param name="N">Length of x (must be mod8)</param>
        /// <param name="order">Length of p</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Value(float* x, float* p, float* r, int N, int order)
        {
            var pp = GC.AllocateUninitializedArray<Vector256<float>>(order);

            for (int i = 0; i < order; ++i)
                pp[i] = Vector256.Create(p[i]);

            var rr = Vector256.Create(0.0f);
            Vector256<float> xx;

            for (int i = 0; i < N; i += 8)
            {
                xx = Avx.LoadVector256(x + i);
                Value(ref xx, ref pp, ref rr, order);
                Avx.Store(r + i, rr);
            }
        }
    }
}
