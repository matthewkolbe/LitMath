// Copyright Matthew Kolbe (2022)

using LitMath;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;

namespace LitMathTests
{
    public class NormDistTest
    {
        [Test]
        public unsafe void ErfDoubleAccuracy()
        {
            var n = 1000;
            var x = new double[n];
            var y = new double[n];
            var r = new Random(10);

            for (int i = 0; i < n; i++)
                x[i] = 30.0*(r.NextDouble()-0.5);


            fixed (double* xx = x) fixed (double* yy = y) 
                LitNormDist.Erf(xx, yy, n);

            for (int i = 0; i < n; ++i)
                Assert.AreEqual(y[i], SpecialFunctions.Erf(x[i]), 1e-13);
        }

        [Test]
        public unsafe void CdfDoubleAccuracy()
        {
            var n = 1000;
            var x = new double[n];
            var y = new double[n];
            var m = new double[n];
            var s = new double[n];

            for (int i = 0; i < n; i++)
            {
                x[i] = 10 * (-0.5 * n + i) / n;
                m[i] = (-0.05 * (n + i) / n);
                s[i] = (i % 10) + 0.5;
            }

            fixed (double* xx = x) fixed (double* yy = y) fixed (double* mm = m) fixed (double* ss = s)
                LitNormDist.CDF(mm, ss, xx, yy, n);

            for (int i = 0; i < n; ++i)
                Assert.AreEqual(y[i], Normal.CDF(m[i], s[i], x[i]), 1e-13);
        }

        [Test]
        public unsafe void CdfDoubleStandardAccuracyBig()
        {
            var n = 1000;
            var x = new double[n];
            var y = new double[n];
            var really = new double[n];
            var m = 0.0;
            var s = 1.0;

            for (int i = 0; i < n; i++)
            {
                x[i] = 20 * (-0.5 * n + i) / n;
                really[i] = Normal.CDF(m, s, x[i]);
            }

            fixed (double* xx = x) fixed (double* yy = y)
                LitNormDist.CDF(m, s, xx, yy, n);

            for (int i = 0; i < n; ++i)
                Assert.AreEqual(y[i], really[i], 1e-9);
        }

        [Test]
        public unsafe void CdfDoubleStandardAccuracy()
        {
            var n = 4;
            var x = new double[n];
            var y = new double[n];
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

            fixed (double* xx = x) fixed (double* yy = y)
                LitNormDist.CDF(m, s, xx, yy, n);

            for (int i = 0; i < n; ++i)
                Assert.AreEqual(y[i], really[i], 1e-13);
        }

        [Test]
        public unsafe void CdfDoubleInfNansAreRight()
        {
            var n = 3;
            var x = new double[n];
            var y = new double[n];
            var m = 0.0;
            var s = 1.0;

            x[0] = double.NaN;
            x[1] = double.PositiveInfinity;
            x[2] = double.NegativeInfinity;
            

            fixed (double* xx = x) fixed (double* yy = y) 
                LitNormDist.CDF(m, s, xx, yy, n);

            Assert.True(double.IsNaN(y[0]));
            Assert.AreEqual(y[1], 1.0, 1e-10);
            Assert.AreEqual(y[2], 0.0, 1e-10);
        }

        [Test]
        public unsafe void CdfFloatAccuracy()
        {
            var n = 1000;
            var x = new float[n];
            var y = new float[n];
            var m = new float[n];
            var s = new float[n];

            for (int i = 0; i < n; i++)
            {
                x[i] = 10f * (-0.5f * n + i) / n;
                m[i] = (-0.05f * (n + i) / n);
                s[i] = (i % 10) + 0.5f;
            }

            fixed (float* xx = x) fixed (float* yy = y) fixed (float* mm = m) fixed (float* ss = s)
                LitNormDist.CDF(mm, ss, xx, yy, n);

            for (int i = 0; i < n; ++i)
                Assert.AreEqual(y[i], Normal.CDF(m[i], s[i], x[i]), 1e-6);
        }

        [Test]
        public unsafe void CdffloatNonStandardAccuracy()
        {
            var n = 1000;
            var x = new float[n];
            var y = new float[n];
            var realy = new float[n];
            var m = 0.2f;
            var s = 1.03f;

            for (int i = 0; i < n; i++)
            {
                x[i] = 30f * (-0.5f * n + i) / n;
                realy[i] = (float)Normal.CDF((double)m, (double)s, (double)x[i]);
            }

            fixed (float* xx = x) fixed (float* yy = y)
                LitNormDist.CDF(m, s, xx, yy, n);

            for (int i = 0; i < n; ++i)
                Assert.AreEqual(y[i], realy[i], 1e-5);
        }

        [Test]
        public unsafe void CdffloatInfNansAreRight()
        {
            var n = 3;
            var x = new float[n];
            var y = new float[n];
            var m = 0.0f;
            var s = 1.0f;

            x[0] = float.NaN;
            x[1] = float.PositiveInfinity;
            x[2] = float.NegativeInfinity;


            fixed (float* xx = x) fixed (float* yy = y)
                LitNormDist.CDF(m, s, xx, yy, n);

            Assert.True(float.IsNaN(y[0]));
            Assert.AreEqual(y[1], 1.0, 1e-10);
            Assert.AreEqual(y[2], 0.0, 1e-10);
        }
    }
}
