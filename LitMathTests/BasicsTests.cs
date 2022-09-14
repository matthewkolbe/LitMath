// Copyright Matthew Kolbe (2022)

using LitMath;

namespace LitMathTests
{
    class BasicsTests
    {
        [Test]
        public unsafe void DotProductDouble()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                var a = stackalloc double[n];
                var b = stackalloc double[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                {
                    a[i] = 1.0;
                    b[i] = 1+i;
                }

                var result = LitBasics.Dot(a, b, n);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(result, n*(n+1)*0.5, 1e-8);
            }
        }

        [Test]
        public unsafe void DotProductFloat()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                var a = stackalloc float[n];
                var b = stackalloc float[n];
                var r = new Random(10);

                for (int i = 0; i < n; ++i)
                {
                    a[i] = 1.0f;
                    b[i] = 1 + i;
                }

                var result = LitBasics.Dot(a, b, n);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(result, n * (n + 1) * 0.5f, 1e-8);
            }
        }

        [Test]
        public unsafe void ElementwiseMultiplyDouble()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                var a = stackalloc double[n];
                var b = stackalloc double[n];
                var r = stackalloc double[n];

                for (int i = 0; i < n; ++i)
                {
                    a[i] = 1.0;
                    b[i] = 1 + i;
                }

                LitBasics.Multiply(a, b, r, n);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(r[i], i + 1, 1e-8);
            }
        }

        [Test]
        public unsafe void SubtractDouble()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                var a = stackalloc double[n];
                var b = stackalloc double[n];
                var r = stackalloc double[n];

                for (int i = 0; i < n; ++i)
                {
                    a[i] = 1 + i;
                    b[i] = 1.0;
                }

                LitBasics.Subtract(a, b, r, n);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(r[i], i, 1e-8);
            }
        }

        [Test]
        public unsafe void SubtractConstDouble()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                var a = stackalloc double[n];
                var r = stackalloc double[n];

                for (int i = 0; i < n; ++i)
                    a[i] = 1 + i;

                LitBasics.Subtract(a, 1.0, r, n);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(r[i], i, 1e-8);
            }
        }

        [Test]
        public unsafe void ConstMultiplyFloat()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                var a = stackalloc float[n];
                var r = stackalloc float[n];

                for (int i = 0; i < n; ++i)
                    a[i] = i;


                LitBasics.Multiply(a, 2f, r, n);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(r[i], 2f*i, 1e-8f);
            }
        }

        [Test]
        public unsafe void ConstMultiplyDouble()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                var a = stackalloc double[n];
                var r = stackalloc double[n];

                for (int i = 0; i < n; ++i)
                    a[i] = i;


                LitBasics.Multiply(a, 2, r, n);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(r[i], 2 * i, 1e-8);
            }
        }

        [Test]
        public unsafe void ConstFmaDouble()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                var a = stackalloc double[n];
                var r = stackalloc double[n];

                for (int i = 0; i < n; ++i)
                    a[i] = i;


                LitBasics.FusedMultiplyAdd(a, 2, 1, r, n);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(r[i], 2 * i + 1, 1e-8);
            }
        }

        [Test]
        public unsafe void ConstFmaFloat()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 1003 })
            {
                var a = stackalloc float[n];
                var r = stackalloc float[n];

                for (int i = 0; i < n; ++i)
                    a[i] = i;


                LitBasics.FusedMultiplyAdd(a, 2, 1, r, n);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(r[i], 2 * i + 1, 1e-8);
            }
        }
    }
}
