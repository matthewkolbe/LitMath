// Copyright Matthew Kolbe (2022)

using LitMath;

namespace LitMathTests
{
    internal class TrigTests
    {

        [Test]
        public void AvxSinDoubleAccuracySpan()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 127, 128, 129 })
            {
                Span<double> a = stackalloc double[n];
                Span<double> b = stackalloc double[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                    a[i] = i + r.NextDouble();


                LitTrig.Sin(ref a, ref b);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(1.0, Math.Sin(a[i]) / b[i], 1e-10);
            }
        }

        [Test]
        public unsafe void AvxSineDoubleAccuracyLarger()
        {
            var a = new double[113];
            var b = stackalloc double[113];
            var r = new Random(10);

            for (int i = 0; i < 113; ++i)
                a[i] = 100000.0 * r.NextDouble() - 50000.0;

            fixed (double* aa = a)
                LitTrig.Sin(aa, b, 113);

            for (int i = 0; i < 113; ++i)
                Assert.AreEqual(1.0, Math.Sin(a[i]) / b[i], 1e-8);
        }

        [Test]
        public unsafe void AvxSineDoubleNaNAndInfAreNan()
        {
            var a = new double[4];
            var b = stackalloc double[4];

            a[0] = double.NaN;
            a[1] = double.PositiveInfinity;
            a[2] = double.NegativeInfinity;
            a[3] = double.NaN;

            fixed (double* aa = a)
                LitTrig.Sin(aa, b, 4);

            for (int i = 0; i < 4; ++i)
                Assert.AreEqual(double.NaN, b[i]);
        }

        [Test]
        public void AvxCosDoubleAccuracySpan()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 127, 128, 129 })
            {
                Span<double> a = stackalloc double[n];
                Span<double> b = stackalloc double[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                    a[i] = i + r.NextDouble();


                LitTrig.Cos(ref a, ref b);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(1.0, Math.Cos(a[i]) / b[i], 1e-10);
            }
        }

        [Test]
        public unsafe void AvxCosDoubleAccuracyLarger()
        {
            var a = new double[113];
            var b = stackalloc double[113];
            var r = new Random(10);

            for (int i = 0; i < 113; ++i)
                a[i] = 100000.0 * r.NextDouble() - 50000.0;

            fixed (double* aa = a)
                LitTrig.Cos(aa, b, 113);

            for (int i = 0; i < 113; ++i)
                Assert.AreEqual(1.0, Math.Cos(a[i]) / b[i], 1e-8);
        }

        [Test]
        public unsafe void AvxCosDoubleNaNAndInfAreNan()
        {
            var a = new double[4];
            var b = stackalloc double[4];

            a[0] = double.NaN;
            a[1] = double.PositiveInfinity;
            a[2] = double.NegativeInfinity;
            a[3] = double.NaN;

            fixed (double* aa = a)
                LitTrig.Cos(aa, b, 4);

            for (int i = 0; i < 4; ++i)
                Assert.AreEqual(double.NaN, b[i]);
        }




        [Test]
        public unsafe void AvxTanDoubleAccuracy()
        {
            var a = new double[1000];
            var b = stackalloc double[1000];
            var r = new Random(10);

            for (int i = 0; i < 1000; ++i)
                a[i] = 1000.0 * r.NextDouble() - 500.0;

            fixed (double* aa = a)
                LitTrig.Tan(aa, b, 1000);

            for (int i = 0; i < 1000; ++i)
                Assert.AreEqual(1.0, Math.Tan(a[i]) / b[i], 1e-10);
        }

        [Test]
        public void AvxTanDoubleAccuracySpan()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 127, 128, 129 })
            {
                Span<double> a = stackalloc double[n];
                Span<double> b = stackalloc double[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                    a[i] = i + r.NextDouble();


                LitTrig.Tan(ref a, ref b);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(1.0, Math.Tan(a[i]) / b[i], 1e-10);
            }
        }

        [Test]
        public unsafe void AvxTanDoubleAccuracyLarger()
        {
            var a = new double[113];
            var b = stackalloc double[113];
            var r = new Random(10);

            for (int i = 0; i < 113; ++i)
                a[i] = 100000.0 * r.NextDouble() - 50000.0;

            fixed (double* aa = a)
                LitTrig.Tan(aa, b, 113);

            for (int i = 0; i < 113; ++i)
                Assert.AreEqual(1.0, Math.Tan(a[i]) / b[i], 1e-7);
        }

        [Test]
        public unsafe void AvxTanDoubleNaNAndInfAreNan()
        {
            var a = new double[4];
            var b = stackalloc double[4];

            a[0] = double.NaN;
            a[1] = double.PositiveInfinity;
            a[2] = double.NegativeInfinity;
            a[3] = double.NaN;

            fixed (double* aa = a)
                LitTrig.Tan(aa, b, 4);

            for (int i = 0; i < 4; ++i)
                Assert.AreEqual(double.NaN, b[i]);
        }
    }
}
