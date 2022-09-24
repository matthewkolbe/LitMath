// Copyright Matthew Kolbe (2022)

using LitMath;
using MathNet.Numerics;

namespace LitMathTests
{
    public class PolynomialTest
    {
        [Test]
        public void PolynomialDoubleAccuracy()
        {
            var n = 1000;
            var m = 10;
            Span<double> x = new double[n];
            Span<double> y = new double[n];
            Span<double> p = new double[m];
            var r = new Random(10);

            for (int i = 0; i < n; i++)
                x[i] = r.NextDouble();

            for (int i = 0; i < m; i++)
                p[i] = r.NextDouble();

            Lit.PolynomialValue(ref x, ref p, ref y);

            for (int i = 0; i < n; ++i)
            {
                var real = Polynomial.Evaluate(x[i], p.ToArray());
                Assert.AreEqual(y[i], real, 1e-7);
            }
        }

        [Test]
        public void PolynomialFloatAccuracy()
        {
            var n = 1000;
            var m = 10;
            Span<float> x = new float[n];
            Span<float> y = new float[n];
            Span<float> p = new float[m];
            var pdub = new double[m];
            var r = new Random(10);

            for (int i = 0; i < n; i++)
                x[i] = (float)r.NextDouble();

            for (int i = 0; i < m; i++)
            {
                p[i] = (float)r.NextDouble();
                pdub[i] = p[i];
            }

            Lit.PolynomialValue(ref x, ref p, ref y);

            for (int i = 0; i < n; ++i)
            {
                var real = Polynomial.Evaluate(x[i], pdub);
                Assert.AreEqual(y[i], real, 1e-5);
            }
        }
    }
}
