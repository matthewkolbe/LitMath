// Copyright Matthew Kolbe (2022)

using LitMath;
using System.Runtime.Intrinsics;

namespace LitMathTests
{
    class BasicsTests
    {
        [Test]
        public void DotProductDouble()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                Span<double> a = stackalloc double[n];
                Span<double> b = stackalloc double[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                {
                    a[i] = 1.0;
                    b[i] = 1+i;
                }

                var result = Lit.Dot(in a, in b);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(result, n*(n+1)*0.5, 1e-8);
            }
        }

        [Test]
        public void DotProductConcurrentDouble()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                Span<Vector256<double>> a = stackalloc Vector256<double>[n];
                Span<Vector256<double>> b = stackalloc Vector256<double>[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                {
                    a[i] = Vector256.Create(1.0);
                    b[i] = Vector256.Create((double)(1 + i));
                }

                var result = Lit.Dot(a, b);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(result.GetElement(2), n * (n + 1) * 0.5, 1e-8);
            }
        }

        [Test]
        public void DotProductFloat()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                Span<float> a = stackalloc float[n];
                Span<float> b = stackalloc float[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                {
                    a[i] = 1.0f;
                    b[i] = 1 + i;
                }

                var result = Lit.Dot(in a, in b);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(result, n * (n + 1) * 0.5f, 1e-8);
            }
        }

        [Test]
        public void ElementwiseMultiplyDouble()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                Span<double> a = stackalloc double[n];
                Span<double> b = stackalloc double[n];
                Span<double> r = stackalloc double[n];

                for (int i = 0; i < n; ++i)
                {
                    a[i] = 1.0;
                    b[i] = 1 + i;
                }

                Lit.Multiply(in a, in b, ref r);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(r[i], i + 1, 1e-8);
            }
        }

        [Test]
        public void SubtractDouble()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                Span<double> a = stackalloc double[n];
                Span<double> b = stackalloc double[n];
                Span<double> r = stackalloc double[n];

                for (int i = 0; i < n; ++i)
                {
                    a[i] = 1 + i;
                    b[i] = 1.0;
                }

                Lit.Subtract(in a, in b, ref r);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(r[i], i, 1e-8);
            }
        }

        [Test]
        public void SubtractConstDouble()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                Span<double> a = stackalloc double[n];
                Span<double> r = stackalloc double[n];

                for (int i = 0; i < n; ++i)
                    a[i] = 1 + i;

                Lit.Subtract(in a[0], 1.0, ref r[0], n);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(r[i], i, 1e-8);
            }
        }

        [Test]
        public void ConstMultiplyFloat()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                Span<float> a = stackalloc float[n];
                Span<float> r = stackalloc float[n];

                for (int i = 0; i < n; ++i)
                    a[i] = i;


                Lit.Multiply(in a, 2f, ref r);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(r[i], 2f*i, 1e-8f);
            }
        }

        [Test]
        public void ConstMultiplyDouble()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                Span<double> a = stackalloc double[n];
                Span<double> r = stackalloc double[n];

                for (int i = 0; i < n; ++i)
                    a[i] = i;


                Lit.Multiply(in a, 2, ref r);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(r[i], 2 * i, 1e-8);
            }
        }

        [Test]
        public void ConstFmaDouble()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                Span<double> a = stackalloc double[n];
                Span<double> r = stackalloc double[n];

                for (int i = 0; i < n; ++i)
                    a[i] = i;


                Lit.FusedMultiplyAdd(in a, 2, 1, ref r);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(r[i], 2 * i + 1, 1e-8);
            }
        }

        [Test]
        public void ConstFmaFloat()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                Span<float> a = stackalloc float[n];
                Span<float> r = stackalloc float[n];

                for (int i = 0; i < n; ++i)
                    a[i] = i;


                Lit.FusedMultiplyAdd(in a, 2, 1, ref r);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(r[i], 2 * i + 1, 1e-8);
            }
        }
    }
}
