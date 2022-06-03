﻿using LitMath;
namespace LitMathTests
{
    public class RootTests
    {
        [Test]
        public void SqrtDoubleAccuracySpan()
        {
            foreach (var n in new[] { 1, 3, 9, 15, 33, 62, 255, 1000 })
            {
                Span<double> a = stackalloc double[n];
                Span<double> b = stackalloc double[n];
                var r = new Random();

                for (int i = 0; i < n; ++i)
                    a[i] = (i + 1) + r.NextDouble();


                LitRoot.Sqrt(ref a, ref b);

                for (int i = 0; i < n; ++i)
                    Assert.AreEqual(1.0, Math.Sqrt(a[i]) / b[i], 1e-10);
            }
        }
    }
}
