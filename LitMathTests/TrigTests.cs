// Copyright Matthew Kolbe (2022)

using LitMath;

namespace LitMathTests
{
    internal class TrigTests
    {

        [Test]
        public void AvxSinDoubleAccuracySpan()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 127, 128, 129, 2000 })
            {
                Span<double> a = stackalloc double[n];
                Span<double> b = stackalloc double[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                    a[i] = 0.5* Math.PI*r.NextDouble();


                LitTrig.Sin(ref a, ref b);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(Math.Sin(a[i]), b[i], 1e-15);
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
                Assert.AreEqual(1.0, Math.Sin(a[i]) / b[i], 1e-10);
        }

        [Test]
        public unsafe void AvxSineDoubleNaNAndInfAreNan()
        {
            var a = new double[4];
            var b = stackalloc double[4];

            a[0] = double.NaN;
            a[1] = double.PositiveInfinity;
            a[2] = double.NegativeInfinity;
            a[3] = 0.0;

            fixed (double* aa = a)
                LitTrig.Sin(aa, b, 4);

            for (int i = 0; i < 3; ++i)
                Assert.AreEqual(double.NaN, b[i]);

            Assert.AreEqual(0.0, b[3]);
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
                    a[i] = 8 * Math.PI * (r.NextDouble() - 0.5);


                LitTrig.Cos(ref a, ref b);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(Math.Cos(a[i]), b[i], 1e-15);
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
                Assert.AreEqual(1.0, Math.Cos(a[i]) / b[i], 1e-9);
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
            int N = 10000;
            Span<double> a = stackalloc double[N];
            Span<double> b = stackalloc double[N];
            var r = new Random(10);

            for (int i = 0; i < N; ++i)
                a[i] = 0.25* Math.PI * r.NextDouble();

            LitTrig.Tan(ref a, ref b);

            for (int i = 0; i < N; ++i)
                Assert.AreEqual(Math.Tan(a[i]), b[i], Math.Max(b[i] * 5e-15, 7e-17));
        }

        [Test]
        public unsafe void AvxTanContinuousAt007()
        {
            int N = 4;
            Span<double> a = stackalloc double[N];
            Span<double> b = stackalloc double[N];
            a[0] = 0.07 - 1e-12;
            a[1] = 0.07 + 1e-12;

            LitTrig.Tan(ref a, ref b);

            Assert.Greater(b[1], b[0]);
        }

        [Test]
        public void AvxTanDoubleAccuracySpan()
        {
            var r = new Random(10);

            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 127, 128, 129, 2000 })
            {
                Span<double> a = stackalloc double[n];
                Span<double> b = stackalloc double[n];
                
                for (int i = 0; i < n; ++i)
                    a[i] = 0.5 * Math.PI * r.NextDouble();

                LitTrig.Tan(ref a, ref b);

                for (int i = 0; i < n; ++i)
                    if (b[i] > 1)
                        Assert.AreEqual(Math.Tan(a[i]), b[i], b[i] * b[i] * 1e-15);
                    else
                        Assert.AreEqual(Math.Tan(a[i]), b[i], 4e-16);
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

        [Test]
        public unsafe void AvxATanDoubleAccuracy()
        {
            Span<double> a = stackalloc double[1000];
            Span<double> b = stackalloc double[1000];
            var r = new Random(10);

            for (int i = 0; i < 1000; ++i)
                a[i] = 10.0*r.NextDouble() - 5.0;


            LitTrig.ATan(ref a, ref b);

            for (int i = 0; i < 1000; ++i)
                Assert.AreEqual(Math.Atan(a[i]), b[i], Math.Abs(b[i]) * 1e-10);
        }

        [Test]
        public void AvxATanDoubleAccuracySpan()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 127, 128, 129 })
            {
                Span<double> a = stackalloc double[n];
                Span<double> b = stackalloc double[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                    a[i] = i + r.NextDouble() - n/2;


                LitTrig.ATan(ref a, ref b);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(1.0, Math.Atan(a[i]) / b[i], 1e-10);
            }
        }

        [Test]
        public unsafe void AvxATanDoubleAccuracyLarger()
        {
            var a = new double[113];
            var b = stackalloc double[113];
            var r = new Random(10);

            for (int i = 0; i < 113; ++i)
                a[i] = 100000.0 * r.NextDouble() - 50000.0;

            fixed (double* aa = a)
                LitTrig.ATan(aa, b, 113);

            for (int i = 0; i < 113; ++i)
                Assert.AreEqual(1.0, Math.Atan(a[i]) / b[i], 1e-10);
        }

        [Test]
        public unsafe void AvxATanDoubleNaNAndInfAreRight()
        {
            var a = new double[4];
            var b = stackalloc double[4];

            a[0] = double.NaN;
            a[1] = double.PositiveInfinity;
            a[2] = double.NegativeInfinity;
            a[3] = double.NaN;

            fixed (double* aa = a)
                LitTrig.ATan(aa, b, 4);

            Assert.AreEqual(double.NaN, b[0]);
            Assert.AreEqual(0.5*Math.PI, b[1], 1e-9);
            Assert.AreEqual(-0.5*Math.PI, b[2], 1e-9);
        }
    }
}
