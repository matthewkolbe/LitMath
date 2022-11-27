// Copyright Matthew Kolbe (2022)

using LitMath;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace LitMathTests
{
    public class UtilitiyTests
    {

        [Test]
        public unsafe void AbsTest()
        {
            var a = Vector256.Create(1.0, 0.0, -3.14, double.NegativeInfinity);
            var b = Util.Abs(ref a);

            Assert.AreEqual(b.GetElement(0), Math.Abs(a.GetElement(0)));
            Assert.AreEqual(b.GetElement(1), Math.Abs(a.GetElement(1)));
            Assert.AreEqual(b.GetElement(2), Math.Abs(a.GetElement(2)));
            Assert.AreEqual(b.GetElement(3), Math.Abs(a.GetElement(3)));
        }

        [Test]
        public void AbsSumTest()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                Span<double> a = stackalloc double[n];
                double[] b = new double[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                {
                    a[i] = r.NextDouble() - 0.5;
                    b[i] = Math.Abs(a[i]);
                }

                var result = Util.AbsSum(ref a);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(result, b.Sum(), 1e-8);
            }
        }

        [Test]
        public void ApplyDoubleTest()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                Span<double> a = stackalloc double[n];

                Util.Apply(ref a, 10.0);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(a[i], 10.0);
            }
        }

        [Test]
        public void CopyDoubleTest()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                Span<double> a = stackalloc double[n];
                Span<double> b = stackalloc double[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                    a[i] = r.NextDouble() - 0.5;

                Util.Copy(ref a, ref b);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(a[i], b[i]);
            }
        }

        [Test]
        public void CopyIntTest()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                Span<int> a = stackalloc int[n];
                Span<int> b = stackalloc int[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                    a[i] = i;

                Util.Copy(ref a, ref b);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(a[i], b[i]);
            }
        }

        [Test]
        public unsafe void CopyFloatTest()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                Span<float> a = stackalloc float[n];
                Span<float> b = stackalloc float[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                    a[i] = (float)(r.NextDouble() - 0.5);

                Util.Copy(ref a, ref b);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(a[i], b[i]);
            }
        }

        [Test]
        public unsafe void ConvertDoubleToIntTest()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                Span<double> a = stackalloc double[n];
                Span<int> b = stackalloc int[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                    a[i] = 100*(r.NextDouble() - 0.5);

                Util.ConvertDoubleToInt(ref a, ref b);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual((int)a[i], b[i]);
            }
        }

        [Test]
        public unsafe void MaxIntTest()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                Span<int> a = stackalloc int[n];
                Span<int> b = stackalloc int[n];
                Span<int> rr = stackalloc int[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                {
                    a[i] = (int)(100 * (r.NextDouble() - 0.5));
                    b[i] = (int)(100 * (r.NextDouble() - 0.5));
                }

                Util.Max(ref a, ref b, ref rr);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(rr[i], Math.Max(a[i], b[i]));
            }
        }

        [Test]
        public unsafe void MinIntTest()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                Span<int> a = stackalloc int[n];
                Span<int> b = stackalloc int[n];
                Span<int> rr = stackalloc int[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                {
                    a[i] = (int)(100 * (r.NextDouble() - 0.5));
                    b[i] = (int)(100 * (r.NextDouble() - 0.5));
                }

                Util.Min(ref a, ref b, ref rr);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(rr[i], Math.Min(a[i], b[i]));
            }
        }

        [Test]
        public unsafe void SignIntTest()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                Span<int> a = stackalloc int[n];
                Span<int> rr = stackalloc int[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                    a[i] = (int)(100 * (r.NextDouble() - 0.5));


                Util.Sign(ref a, ref rr);

                for (int i = 0; i < n; ++i)
                    if (a[i] != 0)
                        Assert.AreEqual(rr[i], Math.Sign(a[i]));
            }
        }

        [Test]
        public unsafe void SignDoubleTest()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                Span<double> a = stackalloc double[n];
                Span<double> rr = stackalloc double[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                    a[i] = (100 * (r.NextDouble() - 0.5));


                Util.Sign(ref a, ref rr);

                for (int i = 0; i < n; ++i)
                    if (a[i] != 0.0)
                        Assert.AreEqual(rr[i], Math.Sign(a[i]));
            }
        }

        [Test]
        public unsafe void ConvertLongToDoubleTest()
        {
            var a = Vector256.Create(0L, -50L, 256L, 1000000L);
            var b = Vector256.Create(0.0);
            Util.ConvertLongToDouble(ref a, ref b);

            Assert.AreEqual(b.GetElement(0), 0.0);
            Assert.AreEqual(b.GetElement(1), -50.0);
            Assert.AreEqual(b.GetElement(2), 256.0);
            Assert.AreEqual(b.GetElement(3), 1000000.0);
        }

        [Test]
        public void IfElseTest()
        {
            var a = Vector256.Create(0.0, 1.0, -3.0, -20.0);
            var zero = Vector256.Create(0.0);
            var b = Util.IfElse(Avx.CompareGreaterThanOrEqual(a, zero), a, zero);

            Assert.AreEqual(b.GetElement(0), 0.0);
            Assert.AreEqual(b.GetElement(1), 1.0);
            Assert.AreEqual(b.GetElement(2), 0.0);
            Assert.AreEqual(b.GetElement(3), 0.0);
        }


        [Test]
        public void SignTest()
        {
            var a = Vector256.Create(0.0, 1.0, -3.0, -20.0);
            var b = Util.Sign(a);

            Assert.AreEqual(b.GetElement(0), 1.0);
            Assert.AreEqual(b.GetElement(1), 1.0);
            Assert.AreEqual(b.GetElement(2), -1.0);
            Assert.AreEqual(b.GetElement(3), -1.0);
        }

        [Test]
        public void MaxTest()
        {
            var a1 = Vector256.Create(0.0, 1.0, -3.0, -20.0);
            var a2 = Vector256.Create(2.0, 1.0, -1.0, -30.0);
            var b = Util.Max(a1, a2);

            Assert.AreEqual(b.GetElement(0), 2.0);
            Assert.AreEqual(b.GetElement(1), 1.0);
            Assert.AreEqual(b.GetElement(2), -1.0);
            Assert.AreEqual(b.GetElement(3), -20.0);
        }

        [Test]
        public void MinTest()
        {
            var a1 = Vector256.Create(0.0, 1.0, -3.0, -20.0);
            var a2 = Vector256.Create(2.0, 1.0, -1.0, -30.0);
            var b = Util.Min(a1, a2);

            Assert.AreEqual(b.GetElement(0), 0.0);
            Assert.AreEqual(b.GetElement(1), 1.0);
            Assert.AreEqual(b.GetElement(2), -3.0);
            Assert.AreEqual(b.GetElement(3), -30.0);
        }

    }

}
