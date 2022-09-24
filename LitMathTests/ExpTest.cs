// Copyright Matthew Kolbe (2022)

using LitMath;

namespace LitMathTests
{
    public class ExpTest
    {
        [Test]
        public void AvxExpDoubleAccuracy()
        {
            Span<double> a = stackalloc double[1000];
            Span<double> b = stackalloc double[1000];
            var r = new Random(10);

            for (int i = 0; i < 1000; ++i)
                a[i] = 2*(r.NextDouble() - 0.5);

            Lit.Exp(ref a, ref b);

            for (int i = 0; i < 1000; ++i)
                Assert.AreEqual(Math.Exp(a[i]), b[i], 3e-16 * b[i]);
        }

        [Test]
        public void AvxExpDoubleAccuracySpan()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62 })
            {
                Span<double> a = stackalloc double[n];
                Span<double> b = stackalloc double[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                    a[i] = (i - n / 2) + r.NextDouble();


                Lit.Exp(ref a, ref b);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(b[i], Math.Exp(a[i]), Math.Max(5e-15 * b[i], 1e-20));
            }
        }

        [Test]
        public void AvxExpDoubleLargeIsInf()
        {
            Span<double> a = stackalloc double[121];
            Span<double> b = stackalloc double[121];

            a[0] = double.PositiveInfinity;

            for (int i = 1; i < 121; ++i)
                a[i] = 708 + 10.1 * i;

            Lit.Exp(ref a, ref  b);

            for (int i = 0; i < 121; ++i)
                Assert.IsTrue(double.IsInfinity(b[i]));
        }

        [Test]
        public void AvxExpDoubleNegInfZero()
        {
            Span<double> a = stackalloc double[4];
            Span<double> b = stackalloc double[4];

            a[0] = double.NegativeInfinity;
            a[1] = -1e200;
            a[2] = -1e100;
            a[3] = -1e50;

            Lit.Exp(ref a, ref b);

            for (int i = 0; i < 4; ++i)
                Assert.AreEqual(0.0, b[i]);
        }

        [Test]
        public void AvxExpDoubleNaNIsNaN()
        {
            Span<double> a = stackalloc double[4];
            Span<double> b = stackalloc double[4];

            for (int i = 0; i < 4; ++i)
                a[i] = double.NaN;

            Lit.Exp(ref a, ref b);

            for (int i = 0; i < 4; ++i)
                Assert.AreEqual(double.NaN, b[i]);
        }

        [Test]
        public void AvxExpFloatAccuracy()
        {
            Span<float> a = stackalloc float[160];
            Span<float> b = stackalloc float[160];
            var r = new Random(10);

            for (int i = 0; i < 160; ++i)
                a[i] = 6*(float)(r.NextDouble()-0.5);

            Lit.Exp(ref a, ref b);

            for (int i = 0; i < 160; ++i)
                Assert.AreEqual(b[i], Math.Exp(a[i]), 5e-7 * Math.Abs(b[i]));
        }

        [Test]
        public void AvxExpFloatAccuracySpan()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 31, 32, 33, 62 })
            {
                Span<float> a = stackalloc float[n];
                Span<float> b = stackalloc float[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                    a[i] = (float)((i - n / 2) + r.NextDouble());


                Lit.Exp(ref a, ref b);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(1.0, Math.Exp(a[i]) / b[i], 1e-5);
            }
        }

        [Test]
        public void AvxExpFloatLargeIsInf()
        {
            Span<float> a = stackalloc float[121];
            Span<float> b = stackalloc float[121];

            a[0] = float.PositiveInfinity;

            for (int i = 1; i < 121; ++i)
                a[i] = 89f + 10.1f * i;

            
            Lit.Exp(ref a, ref b);

            for (int i = 0; i < 121; ++i)
                Assert.IsTrue(float.IsInfinity(b[i]));
        }

        [Test]
        public void AvxExpFloatNegInfZero()
        {
            Span<float> a = stackalloc float[8];
            Span<float> b = stackalloc float[8];

            for (int i = 0; i < 8; ++i)
                a[i] = float.NegativeInfinity;

            Lit.Exp(ref a, ref b);

            for (int i = 0; i < 8; ++i)
                Assert.AreEqual(0.0f, b[i], 1e-10);
        }

        [Test]
        public void AvxExpFloatNaNIsNaN()
        {
            Span<float> a = stackalloc float[2];
            Span<float> b = stackalloc float[2];

            for (int i = 0; i < 2; ++i)
                a[i] = float.NaN;

            Lit.Exp(ref a, ref b);

            for (int i = 0; i < 2; ++i)
                Assert.IsTrue(float.IsNaN(b[i]));
        }
    }
}
