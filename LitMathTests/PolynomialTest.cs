// Copyright Matthew Kolbe (2022)

using LitMath;
using MathNet.Numerics;

namespace LitMathTests
{
    public class PolynomialTest
    {
        [Test]
        public unsafe void PolynomialDoubleAccuracy()
        {
            var n = 1000;
            var m = 10;
            var x = new double[n];
            var y = new double[n];
            var p = new double[m];
            var r = new Random(10);

            for (int i = 0; i < n; i++)
                x[i] = r.NextDouble();

            for (int i = 0; i < m; i++)
                p[i] = r.NextDouble();

            fixed (double* xx = x) fixed (double* yy = y) fixed (double* pp = p)
                LitPolynomial.Compute(xx, pp, yy, n, m);

            for (int i = 0; i < n; ++i)
            {
                var real = Polynomial.Evaluate(x[i], p);
                Assert.AreEqual(y[i], real, 1e-7);
            }
        }

        [Test]
        public unsafe void PolynomialFloatAccuracy()
        {
            var n = 1000;
            var m = 10;
            var x = new float[n];
            var y = new float[n];
            var p = new float[m];
            var pdub = new double[m];
            var r = new Random(10);

            for (int i = 0; i < n; i++)
                x[i] = (float)r.NextDouble();

            for (int i = 0; i < m; i++)
            {
                p[i] = (float)r.NextDouble();
                pdub[i] = p[i];
            }

            fixed (float* xx = x) fixed (float* yy = y) fixed (float* pp = p)
                LitPolynomial.Compute(xx, pp, yy, n, m);

            for (int i = 0; i < n; ++i)
            {
                var real = Polynomial.Evaluate(x[i], pdub);
                Assert.AreEqual(y[i], real, 1e-5);
            }
        }
    }
}
