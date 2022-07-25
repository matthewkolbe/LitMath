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
            var b = LitUtilities.Abs(ref a);

            Assert.AreEqual(b.GetElement(0), Math.Abs(a.GetElement(0)));
            Assert.AreEqual(b.GetElement(1), Math.Abs(a.GetElement(1)));
            Assert.AreEqual(b.GetElement(2), Math.Abs(a.GetElement(2)));
            Assert.AreEqual(b.GetElement(3), Math.Abs(a.GetElement(3)));
        }

        [Test]
        public unsafe void AbsSumTest()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                var a = stackalloc double[n];
                var b = new double[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                {
                    a[i] = r.NextDouble() - 0.5;
                    b[i] = Math.Abs(a[i]);
                }

                var result = LitUtilities.AbsSum(a, n);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(result, b.Sum(), 1e-8);
            }
        }

        [Test]
        public unsafe void ApplyDoubleTest()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                var a = stackalloc double[n];

                LitUtilities.Apply(a, n, 10.0);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(a[i], 10.0);
            }
        }

        [Test]
        public unsafe void CopyDoubleTest()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                var a = stackalloc double[n];
                var b = stackalloc double[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                    a[i] = r.NextDouble() - 0.5;

                LitUtilities.Copy(a, b, n);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(a[i], b[i]);
            }
        }

        [Test]
        public unsafe void CopyIntTest()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                var a = stackalloc int[n];
                var b = stackalloc int[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                    a[i] = i;

                LitUtilities.Copy(a, b, n);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(a[i], b[i]);
            }
        }

        [Test]
        public unsafe void CopyFloatTest()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                var a = stackalloc float[n];
                var b = stackalloc float[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                    a[i] = (float)(r.NextDouble() - 0.5);

                LitUtilities.Copy(a, b, n);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(a[i], b[i]);
            }
        }

        [Test]
        public unsafe void ConvertLongToDoubleTest()
        {
            var a = Vector256.Create(0L, -50L, 256L, 1000000L);
            var b = Vector256.Create(0.0);
            LitUtilities.ConvertLongToDouble(ref a, ref b);

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
            var b = LitUtilities.IfElse(Avx.CompareGreaterThanOrEqual(a, zero), a, zero);

            Assert.AreEqual(b.GetElement(0), 0.0);
            Assert.AreEqual(b.GetElement(1), 1.0);
            Assert.AreEqual(b.GetElement(2), 0.0);
            Assert.AreEqual(b.GetElement(3), 0.0);
        }

    }

}
