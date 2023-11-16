// Copyright Matthew Kolbe (2022)

using LitMath;

namespace LitMathTests
{
    class LogTest
    {
        [Test]
        public void AvxLogDoubleAccuracy()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62 })
            {
                Span<double> a = stackalloc double[n];
                Span<double> b = stackalloc double[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                    a[i] = i + r.NextDouble();

                Lit.Ln(in a, ref b);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(1.0, Math.Log(a[i]) / b[i], 5e-14);
            }
        }

        [Test]
        public void AvxLogDoubleNegsAreNaN()
        {
            Span<double> a = stackalloc double[4];
            Span<double> b = stackalloc double[4];

            for (int i = 0; i < 4; ++i)
                a[i] = -i-1;

            Lit.Ln(in a, ref b);

            for (int i = 0; i < 4; ++i)
                Assert.AreEqual(b[i], double.NaN);
        }

        [Test]
        public void AvxLogDoubleInfIsInfAndNanIsNan()
        {
            Span<double> a = stackalloc double[2];
            Span<double> b = stackalloc double[2];

            a[0] = double.PositiveInfinity;
            a[1] = double.NaN;

            Lit.Ln(in a, ref b);

            Assert.AreEqual(b[0], double.PositiveInfinity);
            Assert.IsTrue(double.IsNaN(b[1]));
        }


        [Test]
        public void AvxLogFloatAccuracySpan()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1001 })
            {
                Span<float> a = stackalloc float[n];
                Span<float> b = stackalloc float[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                    a[i] = (float)(i + r.NextDouble());

                Lit.Ln(in a, ref b);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(1.0, Math.Log(a[i]) / b[i], 1e-6);
            }
        }

        [Test]
        public void AvxLogFloatsNegsAreNaN()
        {
            Span<float> a = stackalloc float[8];
            Span<float> b = stackalloc float[8];

            for (int i = 0; i < 4; ++i)
                a[i] = -i;

            Lit.Ln(in a, ref b);

            for (int i = 0; i < 8; ++i)
                Assert.AreEqual(b[i], double.NaN);
        }

        [Test]
        public void AvxLogFloatInfIsInfAndNanIsNan()
        {
            Span<float> a = stackalloc float[2];
            Span<float> b = stackalloc float[2];

            a[0] = float.PositiveInfinity;
            a[1] = float.NaN;

            Lit.Ln(in a, ref b);

            Assert.AreEqual(b[0], float.PositiveInfinity);
            Assert.IsTrue(float.IsNaN(b[1]));
        }

        [Test]
        public void AvxLogApprxDoubleAccuracy()
        {
            int n = 4;
            Span<double> a = stackalloc double[n];
            Span<double> b = stackalloc double[n];
            var r = new Random(10);

            for (int i = 0; i < n; ++i)
                a[i] = 1 + r.NextDouble();

            var aa = Util.LoadV256(a[0], 0);
            var bb = Util.LoadV256(b[0], 0);

            Lit.LogApprox(in aa, ref bb);

            Util.StoreV256(ref b[0], 0, bb);

            for (int i = 0; i < n; ++i)
                Assert.AreEqual(b[i], Math.Log(a[i]), 1e-14);
            
        }
    }
}
