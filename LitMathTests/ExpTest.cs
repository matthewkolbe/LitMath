// Copyright Matthew Kolbe (2022)

using LitMath;

namespace LitMathTests
{
    public class ExpTest
    {
        [Test]
        public unsafe void AvxExpDoubleAccuracy()
        {
            var a = new double[1000];
            var b = stackalloc double[1000];
            var r = new Random(10);

            for (int i = 0; i < 1000; ++i)
                a[i] = (i - 500) + r.NextDouble();

            fixed (double* aa = a)
            LitExp.Exp(aa, b, 1000);

            for (int i = 0; i < 1000; ++i)
                Assert.AreEqual(1.0, Math.Exp(a[i]) / b[i], 1e-8);
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


                LitExp.Exp(ref a, ref b);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(1.0, Math.Exp(a[i]) / b[i], 1e-8);
            }
        }

        [Test]
        public unsafe void AvxExpDoubleLargeIsInf()
        {
            var a = new double[121];
            var b = stackalloc double[121];

            a[0] = double.PositiveInfinity;

            for (int i = 1; i < 121; ++i)
                a[i] = 708 + 10.1 * i;

            fixed (double* aa = a)
                LitExp.Exp(aa, b, 121);

            for (int i = 0; i < 121; ++i)
                Assert.IsTrue(double.IsInfinity(b[i]));
        }

        [Test]
        public unsafe void AvxExpDoubleNegInfZero()
        {
            var a = new double[4];
            var b = stackalloc double[4];

            for (int i = 0; i < 4; ++i)
                a[i] = double.NegativeInfinity;

            fixed (double* aa = a)
                LitExp.Exp(aa, b, 4);

            for (int i = 0; i < 4; ++i)
                Assert.AreEqual(0.0, b[i]);
        }

        [Test]
        public unsafe void AvxExpDoubleNaNIsNaN()
        {
            var a = new double[4];
            var b = stackalloc double[4];

            for (int i = 0; i < 4; ++i)
                a[i] = double.NaN;

            fixed (double* aa = a)
                LitExp.Exp(aa, b, 4);

            for (int i = 0; i < 4; ++i)
                Assert.AreEqual(double.NaN, b[i]);
        }

        [Test]
        public unsafe void AvxExpFloatAccuracy()
        {
            var a = new float[160];
            var b = stackalloc float[160];
            var r = new Random(10);

            for (int i = 0; i < 160; ++i)
                a[i] = (i - 80f) + (float)r.NextDouble();

            fixed (float* aa = a)
                LitExp.Exp(aa, b, 160);

            for (int i = 0; i < 160; ++i)
                Assert.AreEqual(1.0, Math.Exp(a[i]) / b[i], 1e-5);
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


                LitExp.Exp(ref a, ref b);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(1.0, Math.Exp(a[i]) / b[i], 1e-5);
            }
        }

        [Test]
        public unsafe void AvxExpFloatLargeIsInf()
        {
            var a = new float[121];
            var b = stackalloc float[121];

            a[0] = float.PositiveInfinity;

            for (int i = 1; i < 121; ++i)
                a[i] = 89f + 10.1f * i;

            fixed (float* aa = a)
                LitExp.Exp(aa, b, 121);

            for (int i = 0; i < 121; ++i)
                Assert.IsTrue(float.IsInfinity(b[i]));
        }

        [Test]
        public unsafe void AvxExpFloatNegInfZero()
        {
            var a = new float[8];
            var b = stackalloc float[8];

            for (int i = 0; i < 8; ++i)
                a[i] = float.NegativeInfinity;

            fixed (float* aa = a)
                LitExp.Exp(aa, b, 4);

            for (int i = 0; i < 8; ++i)
                Assert.AreEqual(0.0f, b[i]);
        }

        [Test]
        public unsafe void AvxExpFloatNaNIsNaN()
        {
            var a = new float[2];
            var b = stackalloc float[2];

            for (int i = 0; i < 2; ++i)
                a[i] = float.NaN;

            fixed (float* aa = a)
                LitExp.Exp(aa, b, 2);

            for (int i = 0; i < 2; ++i)
                Assert.IsTrue(float.IsNaN(b[i]));
        }
    }
}
