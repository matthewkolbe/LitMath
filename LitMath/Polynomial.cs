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
        /// Returns the value of 4 polynomials of order n using AVX-256 intrinsics
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PolynomialValue(ref Vector256<double> x, ref Vector256<double>[] p, ref Vector256<double> r, int n)
        {
            r = p[n - 1];

            for (int i = n - 2; i >= 0; i--)
                r = Fma.MultiplyAdd(r, x, p[i]);

        }


        /// <summary>
        /// Returns the value of 4 polynomials of order p.Length using AVX-256 intrinsics
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PolynomialValue(ref Vector256<double> x, ref Span<Vector256<double>> p, ref Vector256<double> r)
        {
            r = p[p.Length - 1];

            for (int i = p.Length - 2; i >= 0; i--)
                r = Fma.MultiplyAdd(r, x, p[i]);

        }


        /// <summary>
        /// Returns the value of 4 polynomials of order p.Length using AVX-256 intrinsics
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> PolynomialValue(ref Vector256<double> x, ref Span<Vector256<double>> p)
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
        public static void PolynomialDerivative(ref Vector256<double> x, ref Vector256<double>[] p, ref Vector256<double> r, int n)
        {
            r = Avx.Multiply(Vector256.Create((double)(n - 1)), p[p.Length - 1]);

            for (int i = n - 2; i >= 1; i--)
                r = Fma.MultiplyAdd(r, x, Avx.Multiply(Vector256.Create((double)i), p[i]));
        }


        /// <summary>
        /// Returns the derivative of 4 polynomials of order p.Length using AVX-256 intrinsics
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PolynomialDerivative(ref Vector256<double> x, ref Span<Vector256<double>> p, ref Vector256<double> r)
        {
            r = Avx.Multiply(Vector256.Create((double)(p.Length - 1)), p[p.Length - 1]);

            for (int i = p.Length - 2; i >= 1; i--)
                r = Fma.MultiplyAdd(r, x, Avx.Multiply(Vector256.Create((double)i), p[i]));
        }


        /// <summary>
        /// Returns the derivative of 4 polynomials of order p.Length using AVX-256 intrinsics
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ChebyshevSecondKindExpExtrapolation(ref Span<double> xx, ref Span<double> p, ref Span<double> rr)
        {
            const int VSZ = 4;
            var n = xx.Length;
            ref var x = ref MemoryMarshal.GetReference(xx);
            ref var y = ref MemoryMarshal.GetReference(rr);
            Vector256<double> oy, inx;

            // if n < 4, then we handle the special case by creating a 4 element array to work with
            if (n < VSZ)
            {
                Span<double> tmp = stackalloc double[VSZ];
                ref var tmpx = ref MemoryMarshal.GetReference(tmp);
                for (int j = 0; j < n; j++)
                    Unsafe.Add(ref tmpx, j) = Unsafe.Add(ref x, j);

                inx = Util.LoadV256(ref tmpx, 0);
                oy = Vector256.Create(0.0);
                ChebyshevSecondKindExpExtrapolation(ref inx, ref oy, ref p);
                Util.StoreV256(ref tmpx, 0, oy);

                for (int j = 0; j < n; ++j)
                    Unsafe.Add(ref y, j) = Unsafe.Add(ref tmpx, j);

                return;
            }

            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - 15))
            {
                inx = Util.LoadV256(ref x, i);
                oy = Vector256.Create(0.0);
                ChebyshevSecondKindExpExtrapolation(ref inx, ref oy, ref p);
                Util.StoreV256(ref y, i, oy);
                i += VSZ;

                inx = Util.LoadV256(ref x, i);
                oy = Vector256.Create(0.0);
                ChebyshevSecondKindExpExtrapolation(ref inx, ref oy, ref p);
                Util.StoreV256(ref y, i, oy);
                i += VSZ;

                inx = Util.LoadV256(ref x, i);
                oy = Vector256.Create(0.0);
                ChebyshevSecondKindExpExtrapolation(ref inx, ref oy, ref p);
                Util.StoreV256(ref y, i, oy);
                i += VSZ;

                inx = Util.LoadV256(ref x, i);
                oy = Vector256.Create(0.0);
                ChebyshevSecondKindExpExtrapolation(ref inx, ref oy, ref p);
                Util.StoreV256(ref y, i, oy);
                i += VSZ;
            }

            // Calculates the remaining sets of 4 values in a standard loop
            for (; i < (n - 3); i += VSZ)
            {
                inx = Util.LoadV256(ref x, i);
                oy = Vector256.Create(0.0);
                ChebyshevSecondKindExpExtrapolation(ref inx, ref oy, ref p);
                Util.StoreV256(ref y, i, oy);
                i += VSZ;
            }

            // Cleans up any excess individual values (if n%4 != 0)
            if (i != n)
            {
                i = n - VSZ;
                inx = Util.LoadV256(ref x, i);
                oy = Vector256.Create(0.0);
                ChebyshevSecondKindExpExtrapolation(ref inx, ref oy, ref p);
                Util.StoreV256(ref y, i, oy);
            }
        }



        /// <summary>
        /// Returns the derivative of 4 polynomials of order p.Length using AVX-256 intrinsics
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector256<double> PolynomialDerivative(ref Vector256<double> x, ref Span<Vector256<double>> p)
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
        public static void ChebyshevFirst(ref Vector256<double> x, ref Vector256<double> r, ref Vector256<double> d, ref ReadOnlySpan<double> coefficients)
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
                dd[i] = Avx.Subtract(Fma.MultiplyAdd(Double.Polynomial.TWO, Avx.Multiply(dd[i - 1], x), rr[i - 1]), dd[i - 2]);
            }

            Lit.Dot(ref cc, ref rr, ref r);
            Lit.Dot(ref cc, ref dd, ref d);
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
                dd[i] = Avx.Subtract(Fma.MultiplyAdd(Double.Polynomial.TWO, Avx.Multiply(dd[i - 1], x), rr[i - 1]), dd[i - 2]);
            }

            Lit.Dot(ref cc, ref rr, ref r);
            Lit.Dot(ref cc, ref dd, ref d);
        }


        /// <summary>
        /// Returns the result of the Chebyshev Second Kind polynomial evaluated at x, but with extrapolation of and exponential type
        /// for values outside of[-1, 1]
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ChebyshevSecondKindExpExtrapolation(Vector256<double> x, ref Vector256<double> r, ref Span<double> coefficients)
        {
            Vector256<double> d = Vector256.Create(0.0);
            var mask = Avx.Or(
                            Avx.CompareLessThanOrEqual(x, Double.Polynomial.NEGONE),
                            Avx.CompareGreaterThanOrEqual(x, Double.Polynomial.ONE));

            var xx = Util.Min(Util.Max(x, Double.Polynomial.NEGONE), Double.Polynomial.ONE);

            ChebyshevSecond(ref xx, ref r, ref d, ref coefficients);

            var out_range_r = r;

            // compute the out of range value
            var sign = Util.Sign(Avx.Multiply(Double.Polynomial.NEGONE, x));
            var arg = Avx.Add(Avx.Multiply(Util.Abs(Avx.Subtract(x, xx)), Double.Polynomial.NEGONE), Double.Polynomial.NEGONE);
            out_range_r = Avx.Add(Avx.Multiply(Avx.Multiply(d, Lit.Exp(arg)), sign), out_range_r);

            r = Util.IfElse(mask, out_range_r, r);
        }

       /// <summary>
       /// Returns the result of the Chebyshev Second Kind polynomial evaluated at x, but with extrapolation of and exponential type
       /// for values outside of[-1, 1]
       /// </summary>
       [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ChebyshevSecondKindExpExtrapolation(ref Vector256<double> x, ref Vector256<double> r, ref Span<double> coefficients)
        {
            Vector256<double> d = Vector256.Create(0.0);
            var mask = Avx.Or(
                            Avx.CompareLessThanOrEqual(x, Double.Polynomial.NEGONE),
                            Avx.CompareGreaterThanOrEqual(x, Double.Polynomial.ONE));

            var xx = Util.Min(Util.Max(x, Double.Polynomial.NEGONE), Double.Polynomial.ONE);

            ChebyshevSecond(ref xx, ref r, ref d, ref coefficients);

            var out_range_r = r;

            // compute the out of range value
            var sign = Util.Sign(Avx.Multiply(Double.Polynomial.NEGONE, x));
            var arg = Avx.Add(Avx.Multiply(Util.Abs(Avx.Subtract(x, xx)), Double.Polynomial.NEGONE), Double.Polynomial.NEGONE);
            out_range_r = Avx.Add(Avx.Multiply(Avx.Multiply(d, Lit.Exp(arg)), sign), out_range_r);

            r = Util.IfElse(mask, out_range_r, r);
        }


        /// <summary>
        /// Returns the result of the Chebyshev First Kind polynomial evaluated at x, but with extrapolation of and exponential type
        /// for values outside of[-1, 1]
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ChebyshevFirstKindExpExtrapolation(Vector256<double> x, Vector256<double> r, ref ReadOnlySpan<double> coefficients)
        {
            Vector256<double> d = Vector256.Create(0.0);
            var mask = Avx.Or(
                            Avx.CompareLessThanOrEqual(x, Double.Polynomial.NEGONE),
                            Avx.CompareGreaterThanOrEqual(x, Double.Polynomial.ONE));

            var xx = Util.Min(Util.Max(x, Double.Polynomial.NEGONE), Double.Polynomial.ONE);

            ChebyshevFirst(ref xx, ref r, ref d, ref coefficients);

            var out_range_r = r;

            // compute the out of range value
            var sign = Util.Sign(Avx.Multiply(Double.Polynomial.NEGONE, x));
            var arg = Avx.Add(Avx.Multiply(Util.Abs(Avx.Subtract(x, xx)), Double.Polynomial.NEGONE), Double.Polynomial.NEGONE);
            out_range_r = Avx.Add(Avx.Multiply(Avx.Multiply(d, Lit.Exp(arg)), sign), out_range_r);

            r = Util.IfElse(mask, out_range_r, r);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void PolynomialValue(ref Vector256<float> x, ref Vector256<float>[] p, ref Vector256<float> r, int n)
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PolynomialValue(ref Span<double> x, ref Span<double> p, ref Span<double> y)
        {
            ref var xx = ref MemoryMarshal.GetReference(x);
            ref var pp = ref MemoryMarshal.GetReference(p);
            ref var yy = ref MemoryMarshal.GetReference(y);
            PolynomialValue(ref xx, ref pp, ref yy, x.Length, p.Length);
        }

        /// <summary>
        /// Computes a polynomial derivative of order p.Length via AVX-256 intrinsics
        /// </summary>
        /// <param name="x">Arguments</param>
        /// <param name="p">The polynomial coefficients where the index corresponds to the order</param>
        /// <param name="y">The result</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PolynomialDerivative(ref Span<double> x, ref Span<double> p, ref Span<double> y)
        {
            ref var xx = ref MemoryMarshal.GetReference(x);
            ref var pp = ref MemoryMarshal.GetReference(p);
            ref var yy = ref MemoryMarshal.GetReference(y);
            PolynomialDerivative(ref xx, ref pp, ref yy, x.Length, p.Length);
        }


        /// <summary>
        /// Computes a polynomial of order p.Length via AVX-256 intrinsics
        /// </summary>
        /// <param name="x">Arguments</param>
        /// <param name="p">The polynomial coefficients where the index corresponds to the order</param>
        /// <param name="y">The result</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<double> PolynomialValue(ref Span<double> x, ref Span<double> p)
        {
            var y = new Span<double>(GC.AllocateUninitializedArray<double>(x.Length));
            ref var xx = ref MemoryMarshal.GetReference(x);
            ref var pp = ref MemoryMarshal.GetReference(p);
            ref var yy = ref MemoryMarshal.GetReference(y);
            PolynomialValue(ref xx, ref pp, ref yy, x.Length, p.Length);
            return y;
        }

        /// <summary>
        /// Computes a polynomial derivative of order p.Length via AVX-256 intrinsics
        /// </summary>
        /// <param name="x">Arguments</param>
        /// <param name="p">The polynomial coefficients where the index corresponds to the order</param>
        /// <param name="y">The result</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<double> PolynomialDerivative(ref Span<double> x, ref Span<double> p)
        {
            var y = new Span<double>(GC.AllocateUninitializedArray<double>(x.Length));
            ref var xx = ref MemoryMarshal.GetReference(x);
            ref var pp = ref MemoryMarshal.GetReference(p);
            ref var yy = ref MemoryMarshal.GetReference(y);
            PolynomialDerivative(ref xx, ref pp, ref yy, x.Length, p.Length);
            return y;
        }


        /// <summary>
        /// Computes a polynomial of order p.Length via AVX-256 intrinsics
        /// </summary>
        /// <param name="x">Arguments</param>
        /// <param name="p">The polynomial coefficients where the index corresponds to the order</param>
        /// <param name="y">The result</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PolynomialValue(ref Span<float> x, ref Span<float> p, ref Span<float> y)
        {
            ref var xx = ref MemoryMarshal.GetReference(x);
            ref var pp = ref MemoryMarshal.GetReference(p);
            ref var yy = ref MemoryMarshal.GetReference(y);
            PolynomialValue(ref xx, ref pp, ref yy, x.Length, p.Length);
        }


        /// <summary>
        /// Computes a polynomial of order order via AVX-256 intrinsics
        /// </summary>
        /// <param name="x">Arguments</param>
        /// <param name="p">The polynomial coefficients where the index corresponds to the order</param>
        /// <param name="r">The result</param>
        /// <param name="N">Length of x (must be mod4)</param>
        /// <param name="order">Length of p</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PolynomialValue(ref double x, ref double p, ref double r, int N, int order)
        {
            var pp = GC.AllocateUninitializedArray<Vector256<double>>(order);

            for (int i = 0; i < order; ++i)
                pp[i] = Vector256.Create(Unsafe.Add(ref p, i));

            var rr = Vector256.Create(0.0);
            Vector256<double> xx;

            for (int i = 0; i < N; i += 4)
            {
                xx = Util.LoadV256(ref x, i);
                PolynomialValue(ref xx, ref pp, ref rr, order);
                Util.StoreV256(ref r, i, rr);
            }
        }


        /// <summary>
        /// Computes a polynomial derivative of order order via AVX-256 intrinsics
        /// </summary>
        /// <param name="x">Arguments</param>
        /// <param name="p">The polynomial coefficients where the index corresponds to the order</param>
        /// <param name="y">The result</param>
        /// <param name="N">Length of x (must be mod4)</param>
        /// <param name="order">Length of p</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PolynomialDerivative(ref double x, ref double p, ref double r, int N, int order)
        {
            var pp = GC.AllocateUninitializedArray<Vector256<double>>(order);

            for (int i = 0; i < order; ++i)
                pp[i] = Vector256.Create(Unsafe.Add(ref p, i));

            var rr = Vector256.Create(0.0);
            Vector256<double> xx;

            for (int i = 0; i < N; i += 4)
            {
                xx = Util.LoadV256(ref x, i);
                PolynomialDerivative(ref xx, ref pp, ref rr, order);
                Util.StoreV256(ref r, i, rr);
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
        public static void PolynomialValue(ref float x, ref float p, ref float r, int N, int order)
        {
            var pp = GC.AllocateUninitializedArray<Vector256<float>>(order);

            for (int i = 0; i < order; ++i)
                pp[i] = Vector256.Create(Unsafe.Add(ref p, i));

            var rr = Vector256.Create(0.0f);
            Vector256<float> xx;

            for (int i = 0; i < N; i += 8)
            {
                xx = Util.LoadV256(ref x, i);
                PolynomialValue(ref xx, ref pp, ref rr, order);
                Util.StoreV256(ref r, i, rr);
            }
        }
    }
}
