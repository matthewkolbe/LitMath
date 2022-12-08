// Copyright Matthew Kolbe (2022)

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace LitMath
{
    public static partial class Lit
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sin(in Vector256<double> x, ref Vector256<double> y)
        {
            // Since sin() is periodic around 2pi, this converts x into the range of [0, 2pi]
            var xt = Avx.Subtract(x, Avx.Multiply(Double.Trig.TWOPI, Avx.Floor(Avx.Divide(x, Double.Trig.TWOPI))));

            // Since sin() in [0, 2pi] is an odd function around pi, this converts the range to [0, pi], then stores whether
            // or not the result needs to be negated in negend.
            var negend = Avx.CompareGreaterThan(xt, Double.Trig.PI);
            xt = Avx.Subtract(xt, Avx.And(negend, Double.Trig.PI));

            negend = Avx.And(negend, Double.Trig.NEGATIVE_TWO);
            negend = Avx.Add(negend, Double.Trig.ONE);

            var nanend = Avx.CompareNotEqual(x, x);
            nanend = Avx.Add(nanend, Avx.CompareGreaterThan(x, Double.Trig.HIGH));
            nanend = Avx.Add(nanend, Avx.CompareLessThan(x, Double.Trig.LOW));

            // Since sin() on [0, pi] is an even function around pi/2, this "folds" the range into [0, pi/2]. I.e. 3pi/5 becomes 2pi/5.
            xt = Avx.Subtract(Double.Trig.HALFPI, Util.Abs(Avx.Subtract(xt, Double.Trig.HALFPI)));

            var xsq = Avx.Multiply(xt, xt);

            // This is an odd-only Taylor series approximation of sin() on [0, pi/2]. 
            var yy = Fma.MultiplyAdd(Double.Trig.P15, xsq, Double.Trig.P13);
            yy = Fma.MultiplyAdd(yy, xsq, Double.Trig.P11);
            yy = Fma.MultiplyAdd(yy, xsq, Double.Trig.P9);
            yy = Fma.MultiplyAdd(yy, xsq, Double.Trig.P7);
            yy = Fma.MultiplyAdd(yy, xsq, Double.Trig.P5);
            yy = Fma.MultiplyAdd(yy, xsq, Double.Trig.P3);
            yy = Fma.MultiplyAdd(yy, xsq, Double.Trig.ONE);
            yy = Avx.Multiply(yy, xt);

            //y = Avx.Multiply(y, LitConstants.Double.Trig.SIN_OF_QUARTERPI);

            y = Fma.MultiplyAdd(yy, negend, nanend);
        }


        /// <summary>
        /// Computes 4 sines, but requires a guarantee that all x's are in [0,pi/4]
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void SinInZeroQuarterPi(in Vector256<double> x, ref Vector256<double> y)
        {
            var xsq = Avx.Multiply(x, x);

            // This is an odd-only Taylor series approximation of sin() on [0, pi/4]. 
            var yy = Fma.MultiplyAdd(Double.Trig.SQP13, xsq, Double.Trig.SQP11);
            yy = Fma.MultiplyAdd(yy, xsq, Double.Trig.SQP9);
            yy = Fma.MultiplyAdd(yy, xsq, Double.Trig.SQP7);
            yy = Fma.MultiplyAdd(yy, xsq, Double.Trig.SQP5);
            yy = Fma.MultiplyAdd(yy, xsq, Double.Trig.SQP3);
            yy = Fma.MultiplyAdd(yy, xsq, Double.Trig.ONE);
            y = Avx.Multiply(yy, x);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Tan(in Vector256<double> x, ref Vector256<double> y)
        {
            // Calculation:
            //     Move to range [0, Pi] with no adjustments
            //     Use oddness around Pi/2 to make range [0, Pi/2] 
            //     do_inverse_mask = avx.gt(Pi/4)
            //     do_not_inverse_mask = avx.lte(Pi / 4)
            //     mirror around Pi/4
            //     calculate tan(x) = sin(x) / sqrt(1-sin(x)^2)
            //     y = and(do_inverse, 1/y) + and(no_inverse, y)

            // Since tan() is periodic around pi, this converts x into the range of [0, pi]
            var xt = Avx.Subtract(x, Avx.Multiply(Double.Trig.PI, Avx.Floor(Avx.Divide(x, Double.Trig.PI))));

            // Since tan() in [0, pi] is an odd function around pi/2, this converts the range to [0, pi/2], then stores whether
            // or not the result needs to be negated in negend.
            var negend = Avx.CompareGreaterThan(xt, Double.Trig.HALFPI);
            xt = Avx.Add(xt, Avx.And(negend, Avx.Multiply(Double.Trig.NEGATIVE_TWO, Avx.Subtract(xt, Double.Trig.HALFPI))));

            negend = Avx.And(negend, Double.Trig.NEGATIVE_TWO);
            negend = Avx.Add(negend, Double.Trig.ONE);

            var nanend = Avx.CompareNotEqual(x, x);
            nanend = Avx.Add(nanend, Avx.CompareGreaterThan(x, Double.Trig.HIGH));
            nanend = Avx.Add(nanend, Avx.CompareLessThan(x, Double.Trig.LOW));

            // Since tan() on [0, pi/2] is an inversed function around pi/4, this "folds" the range into [0, pi/4]. I.e. 3pi/10 becomes 2pi/10.
            var do_inv_mask = Avx.CompareGreaterThan(xt, Double.Trig.QUARTERPI);
            var no_inv_mask = Avx.CompareLessThanOrEqual(xt, Double.Trig.QUARTERPI);
            xt = Avx.Subtract(Double.Trig.QUARTERPI, Util.Abs(Avx.Subtract(xt, Double.Trig.QUARTERPI)));

            // tan(x) = sin(x) / sqrt(1-sin(x)^2)
            var xx = Vector256.Create(0.0);
            SinInZeroQuarterPi(in xt, ref xx);
            xt = Avx.Sqrt(Avx.Subtract(Double.Trig.ONE, Avx.Multiply(xx, xx)));

            xx = Avx.Add(
                    Avx.And(do_inv_mask, Avx.Divide(xt, xx)),
                    Avx.And(no_inv_mask, Avx.Divide(xx, xt)));

            y = Fma.MultiplyAdd(xx, negend, nanend);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TanMixedModel(in Vector256<double> x, ref Vector256<double> y)
        {
            // Calculation:
            //     Move to range [0, Pi] with no adjustments
            //     Use oddness around Pi/2 to make range [0, Pi/2] 
            //     do_inverse_mask = avx.gt(Pi/4)
            //     do_not_inverse_mask = avx.lte(Pi / 4)
            //     mirror around Pi/4
            //     calculate tan(x) = sin(x) / sqrt(1-sin(x)^2)
            //     y = and(do_inverse, 1/y) + and(no_inverse, y)

            // Since tan() is periodic around pi, this converts x into the range of [0, pi]
            var xt = Avx.Subtract(x, Avx.Multiply(Double.Trig.PI, Avx.Floor(Avx.Divide(x, Double.Trig.PI))));

            // Since tan() in [0, pi] is an odd function around pi/2, this converts the range to [0, pi/2], then stores whether
            // or not the result needs to be negated in negend.
            var negend = Avx.CompareGreaterThan(xt, Double.Trig.HALFPI);
            xt = Avx.Add(xt, Avx.And(negend, Avx.Multiply(Double.Trig.NEGATIVE_TWO, Avx.Subtract(xt, Double.Trig.HALFPI))));

            negend = Avx.And(negend, Double.Trig.NEGATIVE_TWO);
            negend = Avx.Add(negend, Double.Trig.ONE);

            var nanend = Avx.CompareNotEqual(x, x);
            nanend = Avx.Add(nanend, Avx.CompareGreaterThan(x, Double.Trig.HIGH));
            nanend = Avx.Add(nanend, Avx.CompareLessThan(x, Double.Trig.LOW));

            // Since tan() on [0, pi/2] is an inversed function around pi/4, this "folds" the range into [0, pi/4]. I.e. 3pi/10 becomes 2pi/10.
            var do_inv_mask = Avx.CompareGreaterThan(xt, Double.Trig.QUARTERPI);
            xt = Avx.Subtract(Double.Trig.QUARTERPI, Util.Abs(Avx.Subtract(xt, Double.Trig.QUARTERPI)));

            // tan(x) = sin(x) / sqrt(1-sin(x)^2)
            var xx = Vector256.Create(0.0);
            SinInZeroQuarterPi(in xt, ref xx);

            var xsq = Avx.Multiply(xt, xt);

            // This is an odd-only Taylor series approximation of tan() on [0, 0.07]. 
            var yy = Fma.MultiplyAdd(Double.Trig.CT11, xsq, Double.Trig.CT9);
            yy = Fma.MultiplyAdd(yy, xsq, Double.Trig.CT7);
            yy = Fma.MultiplyAdd(yy, xsq, Double.Trig.CT5);
            yy = Fma.MultiplyAdd(yy, xsq, Double.Trig.CT3);
            yy = Fma.MultiplyAdd(yy, xsq, Double.Trig.CT1);
            yy = Avx.Multiply(yy, xt);

            xt = Avx.Sqrt(Avx.Subtract(Double.Trig.ONE, Avx.Multiply(xx, xx)));

            xx = Util.IfElse(do_inv_mask, Avx.Divide(xt, xx), Avx.Divide(xx, xt));
            yy = Util.IfElse(do_inv_mask, Avx.Divide(Double.Trig.ONE, yy), yy);
            xx = Util.IfElse(Avx.CompareLessThan(xt, Double.Trig.SMALLCONDITION), yy, xx);

            y = Fma.MultiplyAdd(xx, negend, nanend);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sinh(in Vector256<double> x, ref Vector256<double> y)
        {
            Lit.Exp(in x, ref y);
            var iy = Avx.Divide(Double.Trig.ONE, y);
            y = Avx.Multiply(Double.Trig.HALF, Avx.Subtract(y, iy));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Cosh(in Vector256<double> x, ref Vector256<double> y)
        {
            Lit.Exp(in x, ref y);
            var iy = Avx.Divide(Double.Trig.ONE, y);
            y = Avx.Multiply(Double.Trig.HALF, Avx.Add(y, iy));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Tanh(in Vector256<double> x, ref Vector256<double> y)
        {
            Lit.Exp(in x, ref y);
            var iy = Avx.Divide(Double.Trig.ONE, y);
            y = Avx.Divide(Avx.Subtract(y, iy), Avx.Add(y, iy));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ASinh(in Vector256<double> x, ref Vector256<double> y)
        {
            y = Avx.Add(Avx.Multiply(x, x), Double.Trig.ONE);
            y = Avx.Sqrt(y);
            y = Avx.Add(x, y);
            Lit.Ln(in y, ref y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ACosh(in Vector256<double> x, ref Vector256<double> y)
        {
            y = Avx.Subtract(Avx.Multiply(x, x), Double.Trig.ONE);
            y = Avx.Sqrt(y);
            y = Avx.Add(x, y);
            Lit.Ln(in y, ref y);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ATan(in Vector256<double> x, ref Vector256<double> y)
        {
            // Idea taken from https://github.com/avrdudes/avr-libc/blob/main/libm/fplib/atan.S

            //  Algorithm:
	        //  if (x > 1)
	        //      return Pi/2 - atanf(1/x)
	        //  elif (x < -1)
	        //      return -Pi/2 - atanf(1/x)
	        //  else
	        //      return x * (1 - C1 * x**2 + ... + CN * x**2N)
             

            // Ensures NaN inputs are NaN outputs
            var nanend = Avx.CompareNotEqual(x, x);

            // The original algorithm has some branching in it, but a manageable amount. This section modifies negend to also
            // have -Pi/2 or Pi/2 added at the end if |x| > 1 (since NaN is reflected by adding NaN to the answer at the end).
            nanend = Avx.Add(nanend, Avx.And(Avx.CompareGreaterThan(x, Double.Trig.ONE), Double.Trig.HALFPI));
            var invx = Avx.Divide(Double.Trig.NEGONE, x);
            var gtmask = Avx.CompareGreaterThan(x, Double.Trig.ONE);
            var ltmask = Avx.CompareLessThanOrEqual(x, Double.Trig.ONE);
            var xx = Avx.Add(Avx.And(gtmask, invx), Avx.And(ltmask, x));
            nanend = Avx.Add(nanend, Avx.And(Avx.CompareLessThan(x, Double.Trig.NEGONE), Double.Trig.NEGHALFPI));
            gtmask = Avx.CompareGreaterThanOrEqual(x, Double.Trig.NEGONE);
            ltmask = Avx.CompareLessThan(x, Double.Trig.NEGONE);
            xx = Avx.Add(Avx.And(ltmask, invx),Avx.And(gtmask, xx));

            // This algorithm leverages the fact that ATan is an odd function, and converts negative
            // inputs to positive ones, and changes the output sign at the end.
            var negend = Avx.CompareLessThan(xx, Double.Trig.ZERO);
            negend = Avx.And(negend, Double.Trig.NEGATIVE_TWO);
            negend = Avx.Add(negend, Double.Trig.ONE);

            var xt = Avx.Multiply(xx, xx);

            // This is an odd-only Taylor series approximation of atan() on [0, 1]. 
            var yy = Fma.MultiplyAdd(Double.Trig.AT13, xt, Double.Trig.AT12);
            yy = Fma.MultiplyAdd(yy, xt, Double.Trig.AT11);
            yy = Fma.MultiplyAdd(yy, xt, Double.Trig.AT10);
            yy = Fma.MultiplyAdd(yy, xt, Double.Trig.AT9);
            yy = Fma.MultiplyAdd(yy, xt, Double.Trig.AT8);
            yy = Fma.MultiplyAdd(yy, xt, Double.Trig.AT7);
            yy = Fma.MultiplyAdd(yy, xt, Double.Trig.AT6);
            yy = Fma.MultiplyAdd(yy, xt, Double.Trig.AT5);
            yy = Fma.MultiplyAdd(yy, xt, Double.Trig.AT4);
            yy = Fma.MultiplyAdd(yy, xt, Double.Trig.AT3);
            yy = Fma.MultiplyAdd(yy, xt, Double.Trig.AT2);
            yy = Fma.MultiplyAdd(yy, xt, Double.Trig.ONE);
            yy = Avx.Multiply(yy, Avx.AndNot(Double.Trig.NEGZERO, xx));

            y = Fma.MultiplyAdd(yy, negend, nanend);
        }


        /// <summary>
        /// Computes Sine on 4 doubles
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sin(in double xx, ref double yy, int index)
        {
            var x = Util.LoadV256(in xx, index);
            Sin(in x, ref x);
            Util.StoreV256(ref yy, index, x);
        }


        /// <summary>
        /// Computes Cosine on 4 doubles
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Cos(in double xx, ref double yy, int index)
        {
            var x = Avx.Add(Util.LoadV256(in xx, index), Double.Trig.HALFPI);
            Sin(in x, ref x);
            Util.StoreV256(ref yy, index, x);
        }


        /// <summary>
        /// Computes Tangent on 4 doubles
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Tan(in double xx, ref double yy, int index)
        {
            var x = Util.LoadV256(in xx, index);
            TanMixedModel(in x, ref x);
            Util.StoreV256(ref yy, index, x);
        }


        /// <summary>
        /// Computes Inverse Tangent on 4 doubles
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ATan(in double xx, ref double yy, int index)
        {
            var x = Util.LoadV256(in xx, index);
            ATan(in x, ref x);
            Util.StoreV256(ref yy, index, x);
        }

        /// <summary>
        /// Computes Cosine on n doubles
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Cos(in Span<double> xx, ref Span<double> yy)
        {
            const int VSZ = 4;
            var n = xx.Length;
            ref var x0 = ref MemoryMarshal.GetReference(xx);
            ref var y0 = ref MemoryMarshal.GetReference(yy);

            if (n < VSZ)
            {
                Span<double> tmpx = stackalloc double[VSZ];
                ref var t0 = ref MemoryMarshal.GetReference(tmpx);
                for (int j = 0; j < n; j++)
                    Unsafe.Add(ref t0, j) = Unsafe.Add(ref x0, j);

                Cos(in t0, ref t0, 0);

                for (int j = 0; j < n; ++j)
                    Unsafe.Add(ref y0, j) = Unsafe.Add(ref t0, j);
            }

            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - 15))
            {
                Cos(in x0, ref y0, i);
                i += VSZ;
                Cos(in x0, ref y0, i);
                i += VSZ;
                Cos(in x0, ref y0, i);
                i += VSZ;
                Cos(in x0, ref y0, i);
                i += VSZ;
            }

            // Calculates the remaining sets of 4 values in a standard loop
            for (; i < (n - 3); i += VSZ)
                Cos(in x0, ref y0, i);

            // Cleans up any excess individual values (if n%4 != 0)
            if (i != n)
            {
                i = n - VSZ;
                Cos(in x0, ref y0, i);
            }
        }


        /// <summary>
        /// Computes Sine on n doubles
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sin(in Span<double> xx, ref Span<double> yy)
        {
            const int VSZ = 4;
            var n = xx.Length;
            ref var x0 = ref MemoryMarshal.GetReference(xx);
            ref var y0 = ref MemoryMarshal.GetReference(yy);

            if (n < VSZ)
            {
                Span<double> tmpx = stackalloc double[VSZ];
                ref var t0 = ref MemoryMarshal.GetReference(tmpx);
                for (int j = 0; j < n; j++)
                    Unsafe.Add(ref t0, j) = Unsafe.Add(ref x0, j);

                Sin(in t0, ref t0, 0);

                for (int j = 0; j < n; ++j)
                    Unsafe.Add(ref y0, j) = Unsafe.Add(ref t0, j);
            }

            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - 15))
            {
                Sin(in x0, ref y0, i);
                i += VSZ;
                Sin(in x0, ref y0, i);
                i += VSZ;
                Sin(in x0, ref y0, i);
                i += VSZ;
                Sin(in x0, ref y0, i);
                i += VSZ;
            }

            // Calculates the remaining sets of 4 values in a standard loop
            for (; i < (n - 3); i += VSZ)
                Sin(in x0, ref y0, i);

            // Cleans up any excess individual values (if n%4 != 0)
            if (i != n)
            {
                i = n - VSZ;
                Sin(in x0, ref y0, i);
            }
        }

        /// <summary>
        /// Computes Tangent on n doubles
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        /// <param name="n"></param>
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Tan(in Span<double> xx, ref Span<double> yy)
        {
            const int VSZ = 4;
            var n = xx.Length;
            ref var x0 = ref MemoryMarshal.GetReference(xx);
            ref var y0 = ref MemoryMarshal.GetReference(yy);

            if (n < VSZ)
            {
                Span<double> tmpx = stackalloc double[VSZ];
                ref var t0 = ref MemoryMarshal.GetReference(tmpx);
                for (int j = 0; j < n; j++)
                    Unsafe.Add(ref t0, j) = Unsafe.Add(ref x0, j);

                Tan(in t0, ref t0, 0);

                for (int j = 0; j < n; ++j)
                    Unsafe.Add(ref y0, j) = Unsafe.Add(ref t0, j);
            }

            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - 15))
            {
                Tan(in x0, ref y0, i);
                i += VSZ;
                Tan(in x0, ref y0, i);
                i += VSZ;
                Tan(in x0, ref y0, i);
                i += VSZ;
                Tan(in x0, ref y0, i);
                i += VSZ;
            }

            // Calculates the remaining sets of 4 values in a standard loop
            for (; i < (n - 3); i += VSZ)
                Tan(in x0, ref y0, i);

            // Cleans up any excess individual values (if n%4 != 0)
            if (i != n)
            {
                i = n - VSZ;
                Tan(in x0, ref y0, i);
            }
        }

        /// <summary>
        /// Computes Inverse Tangent on n doubles
        /// </summary>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        /// <param name="n"></param>
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ATan(in Span<double> xx, ref Span<double> yy)
        {
            const int VSZ = 4;
            var n = xx.Length;
            ref var x0 = ref MemoryMarshal.GetReference(xx);
            ref var y0 = ref MemoryMarshal.GetReference(yy);

            if (n < VSZ)
            {
                Span<double> tmpx = stackalloc double[VSZ];
                ref var t0 = ref MemoryMarshal.GetReference(tmpx);
                for (int j = 0; j < n; j++)
                    Unsafe.Add(ref t0, j) = Unsafe.Add(ref x0, j);

                ATan(in t0, ref t0, 0);

                for (int j = 0; j < n; ++j)
                    Unsafe.Add(ref y0, j) = Unsafe.Add(ref t0, j);
            }

            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - 15))
            {
                ATan(in x0, ref y0, i);
                i += VSZ;
                ATan(in x0, ref y0, i);
                i += VSZ;
                ATan(in x0, ref y0, i);
                i += VSZ;
                ATan(in x0, ref y0, i);
                i += VSZ;
            }

            // Calculates the remaining sets of 4 values in a standard loop
            for (; i < (n - 3); i += VSZ)
                ATan(in x0, ref y0, i);

            // Cleans up any excess individual values (if n%4 != 0)
            if (i != n)
            {
                i = n - VSZ;
                ATan(in x0, ref y0, i);
            }
        }
    }
}
