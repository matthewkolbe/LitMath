// Copyright Matthew Kolbe (2022)

using LitMath;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;

namespace LitMathTests
{
    public class NormDistTest
    {
        [Test]
        public void ErfDoubleAccuracy()
        {
            var n = 1000;
            Span<double> x = new double[n];
            Span<double> y = new double[n];
            var r = new Random(10);

            for (int i = 0; i < n; i++)
                x[i] = 30.0*(r.NextDouble()-0.5);

            Lit.Erf(in x, ref y);

            for (int i = 0; i < n; ++i)
                Assert.AreEqual(y[i], SpecialFunctions.Erf(x[i]), 1e-13);
        }

        [Test]
        public void CdfDoubleAccuracy()
        {
            var n = 1000;
            Span<double> x = new double[n];
            Span<double> y = new double[n];
            Span<double> m = new double[n];
            Span<double> s = new double[n];

            for (int i = 0; i < n; i++)
            {
                x[i] = 10 * (-0.5 * n + i) / n;
                m[i] = (-0.05 * (n + i) / n);
                s[i] = (i % 10) + 0.5;
            }

            Lit.CDF(in m, in s, in x, ref y);

            for (int i = 0; i < n; ++i)
                Assert.AreEqual(y[i], Normal.CDF(m[i], s[i], x[i]), 1e-13);
        }

        [Test]
        public void CdfDoubleStandardAccuracyBig()
        {
            var n = 1000;
            Span<double> x = new double[n];
            Span<double> y = new double[n];
            var really = new double[n];
            var m = 0.0;
            var s = 1.0;

            for (int i = 0; i < n; i++)
            {
                x[i] = 20 * (-0.5 * n + i) / n;
                really[i] = Normal.CDF(m, s, x[i]);
            }

            Lit.CDF(m, s, in x, ref y);

            for (int i = 0; i < n; ++i)
                Assert.AreEqual(y[i], really[i], 1e-9);
        }

        [Test]
        public void CdfDoubleStandardAccuracy()
        {
            var n = 4;
            Span<double> x = new double[n];
            Span<double> y = new double[n];
            var really = new double[n];
            var m = 0.0;
            var s = 1.0;

            x[0] = -3.7;
            x[1] = 0.0;
            x[2] = 1.0;
            x[3] = 7.0;
            really[0] = Normal.CDF(m, s, x[0]);
            really[1] = Normal.CDF(m, s, x[1]);
            really[2] = Normal.CDF(m, s, x[2]);
            really[3] = Normal.CDF(m, s, x[3]);

            Lit.CDF(m, s, in x, ref y);

            for (int i = 0; i < n; ++i)
                Assert.AreEqual(y[i], really[i], 1e-13);
        }

        [Test]
        public void CdfDoubleInfNansAreRight()
        {
            var n = 3;
            Span<double> x = new double[n];
            Span<double> y = new double[n];
            var m = 0.0;
            var s = 1.0;

            x[0] = double.NaN;
            x[1] = double.PositiveInfinity;
            x[2] = double.NegativeInfinity;
            
            Lit.CDF(m, s, in x, ref y);

            Assert.True(double.IsNaN(y[0]));
            Assert.AreEqual(y[1], 1.0, 1e-10);
            Assert.AreEqual(y[2], 0.0, 1e-10);
        }

        [Test]
        public void CdfFloatAccuracy()
        {
            var n = 1000;
            Span<double> x = new double[n];
            Span<double> y = new double[n];
            Span<double> m = new double[n];
            Span<double> s = new double[n];

            for (int i = 0; i < n; i++)
            {
                x[i] = 10f * (-0.5f * n + i) / n;
                m[i] = (-0.05f * (n + i) / n);
                s[i] = (i % 10) + 0.5f;
            }

            Lit.CDF(in m, in s, in x, ref y);

            for (int i = 0; i < n; ++i)
                Assert.AreEqual(y[i], Normal.CDF(m[i], s[i], x[i]), 2e-7);
        }

        [Test]
        public void CdffloatNonStandardAccuracy()
        {
            var n = 1000;
            Span<float> x = new float[n];
            Span<float> y = new float[n];
            var realy = new float[n];
            var m = 0.2f;
            var s = 1.03f;

            for (int i = 0; i < n; i++)
            {
                x[i] = 30f * (-0.5f * n + i) / n;
                realy[i] = (float)Normal.CDF((double)m, (double)s, (double)x[i]);
            }

            Lit.CDF(m, s, in x, ref y);

            for (int i = 0; i < n; ++i)
                Assert.AreEqual(y[i], realy[i], 1e-5);
        }
    }
}
