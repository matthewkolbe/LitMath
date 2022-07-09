using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace LitMath
{
    public class LitRoot
    {


        /// <summary>
        /// Calculates n square roots on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="xx">A pointer to the first argument</param>
        /// <param name="yy">The return values</param>
        /// <param name="n">The number of xx values to take a square root of</param>
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Sqrt(double* xx, double* yy, int n)
        {
            const int VSZ = 4;

            if (n < VSZ)
            {
                var tmpx = stackalloc double[VSZ];

                for (int j = 0; j < n; j++)
                    tmpx[j] = xx[j];

                Sqrt(tmpx, tmpx);

                for (int j = 0; j < n; ++j)
                    yy[j] = tmpx[j];

                return;
            }

            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - 15))
            {
                Sqrt(xx + i, yy + i);
                i += 4;
                Sqrt(xx + i, yy + i);
                i += 4;
                Sqrt(xx + i, yy + i);
                i += 4;
                Sqrt(xx + i, yy + i);
                i += 4;
            }

            // Calculates the remaining sets of 4 values in a standard loop
            for (; i < (n - 3); i += 4)
                Sqrt(xx + i, yy + i);

            // Cleans up any excess individual values (if n%4 != 0)
            if (i != n)
            {
                i = n - VSZ;
                Sqrt(xx + i, yy + i);
            }
        }


        /// <summary>
        /// Calculates n square roots on floats via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="xx">A pointer to the first argument</param>
        /// <param name="yy">The return values</param>
        /// <param name="n">The number of xx values to take a square roots of</param>
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Sqrt(float* xx, float* yy, int n)
        {
            int i = 0;

            // Calculates values in an unrolled manner if the number of values is large enough
            while (i < (n - 31))
            {
                Sqrt(xx + i, yy + i);
                i += 8;
                Sqrt(xx + i, yy + i);
                i += 8;
                Sqrt(xx + i, yy + i);
                i += 8;
                Sqrt(xx + i, yy + i);
                i += 8;
            }

            // Calculates the remaining sets of 8 values in a standard loop
            for (; i < (n - 7); i += 8)
                Sqrt(xx + i, yy + i);

            // Cleans up any excess individual values (if n%8 != 0)
            if (i != n)
            {
                var nn = i;
                var tmpx = stackalloc float[8];

                for (int j = 0; j < (n - i); j++)
                    tmpx[j] = xx[i + j];

                Sqrt(tmpx, tmpx);

                for (; i < n; ++i)
                    yy[i] = tmpx[i - nn];
            }
        }


        /// <summary>
        /// Calculates 4 square roots on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="xx">A pointer to the first of 4 arguments</param>
        /// <param name="yy">The return values</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe void Sqrt(double* xx, double* yy)
        {
            Avx.Store(yy, Avx.Sqrt(Avx.LoadVector256(xx)));
        }


        /// <summary>
        /// Calculates 8 square roots on floats via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="xx">A pointer to the first of 8 arguments</param>
        /// <param name="yy">The return values</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe void Sqrt(float* xx, float* yy)
        {
            Avx.Store(yy, Avx.Sqrt(Avx.LoadVector256(xx)));
        }

        /// <summary>
        /// Calculates 4 square roots on doubles via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="xx">A Span to the first of 4 arguments</param>
        /// <param name="yy">The return values</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Sqrt(ref Span<double> xx, ref Span<double> yy)
        {
            unsafe
            {
                fixed (double* x = xx) fixed (double* y = yy)
                    Sqrt(x, y, xx.Length);
            }
        }


        /// <summary>
        /// Calculates 8 square roots on floats via 256-bit SIMD intrinsics. 
        /// </summary>
        /// <param name="xx">A Span to the first of 8 arguments</param>
        /// <param name="yy">The return values</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Sqrt(ref Span<float> xx, ref Span<float> yy)
        {
            unsafe
            {
                fixed (float* x = xx) fixed (float* y = yy)
                    Sqrt(x, y, xx.Length);
            }
        }
    }
}
