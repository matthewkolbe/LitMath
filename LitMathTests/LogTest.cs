// Copyright Matthew Kolbe (2022)

using LitMath;

namespace LitMathTests
{
    class LogTest
    {
        [Test]
        public unsafe void AvxLogAccuracyDouble()
        {
            var a = new double[121];
            var b = stackalloc double[121];

            for (int i = 0; i < 121; ++i)
                a[i] = 0.5*i + 0.5;

            fixed (double* aa = a)
                LitLog.Ln(aa, b, 121);

            for (int i = 0; i < 121; ++i)
                if (a[i] != 1.0)
                    Assert.AreEqual(1.0, Math.Log(a[i]) / b[i], 1e-9);
        }

        [Test]
        public void AvxLogDoubleAccuracySpan()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62 })
            {
                Span<double> a = stackalloc double[n];
                Span<double> b = stackalloc double[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                    a[i] = i + r.NextDouble();


                LitLog.Ln(ref a, ref b);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(1.0, Math.Log(a[i]) / b[i], 1e-8);
            }
        }

        [Test]
        public unsafe void AvxLogDoubleNegsAreNaN()
        {
            var a = new double[4];
            var b = stackalloc double[4];

            for (int i = 0; i < 4; ++i)
                a[i] = -i;

            fixed (double* aa = a)
                LitLog.Ln(aa, b, 4);

            for (int i = 0; i < 4; ++i)
                Assert.AreEqual(b[i], double.NaN);
        }

        [Test]
        public unsafe void AvxLogDoubleInfIsInf()
        {
            var a = new double[2];
            var b = stackalloc double[2];

            for (int i = 0; i < 2; ++i)
                a[i] = double.PositiveInfinity;

            fixed (double* aa = a)
                LitLog.Ln(aa, b, 2);

            Assert.AreEqual(b[0], double.PositiveInfinity);
        }

        [Test]
        public unsafe void AvxLogDoubleNanIsNan()
        {
            var a = new double[1];
            var b = stackalloc double[1];

            a[0] = double.NaN;

            fixed (double* aa = a)
                LitLog.Ln(aa, b, 1);
                
            Assert.IsTrue(double.IsNaN(b[0]));
        }


        [Test]
        public unsafe void AvxLogAccuracyFloats()
        {
            var a = new float[1001];
            var b = stackalloc float[1001];

            for (int i = 0; i < 1001; ++i)
                a[i] = 0.5f * i + 0.5f;

            fixed (float* aa = a)
                LitLog.Ln(aa, b, 1001);

            for (int i = 0; i < 1001; ++i)
                if (a[i] != 1.0f)
                    Assert.AreEqual(1.0, Math.Log(a[i]) / b[i], 1e-6);
        }

        [Test]
        public void AvxLogFloatAccuracySpan()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62 })
            {
                Span<float> a = stackalloc float[n];
                Span<float> b = stackalloc float[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                    a[i] = (float)(i + r.NextDouble());


                LitLog.Ln(ref a, ref b, n);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(1.0, Math.Log(a[i]) / b[i], 1e-5);
            }
        }

        [Test]
        public unsafe void AvxLogFloatsNegsAreNaN()
        {
            var a = new float[8];
            var b = stackalloc float[8];

            for (int i = 0; i < 4; ++i)
                a[i] = -i;

            fixed (float* aa = a)
                LitLog.Ln(aa, b, 8);

            for (int i = 0; i < 8; ++i)
                Assert.AreEqual(b[i], double.NaN);
        }

        [Test]
        public unsafe void AvxLogFloatInfIsInf()
        {
            var a = new float[1];
            var b = stackalloc float[1];

            a[0] = float.PositiveInfinity;

            fixed (float* aa = a)
                LitLog.Ln(aa, b, 1);

            Assert.AreEqual(b[0], float.PositiveInfinity);
        }

        [Test]
        public unsafe void AvxLogFloatNanIsNan()
        {
            var a = new float[1];
            var b = stackalloc float[1];

            a[0] = float.NaN;

            fixed (float* aa = a)
                LitLog.Ln(aa, b, 1);

            Assert.IsTrue(float.IsNaN(b[0]));
        }
    }
}
