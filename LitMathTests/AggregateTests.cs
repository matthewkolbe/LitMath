// Copyright Matthew Kolbe (2022)

using System.Runtime.Intrinsics;
using LitMath;

namespace LitMathTests
{
    class AggregateTests
    {
        [Test]
        public unsafe void DoubleAggreation()
        {
            var x = Vector256.Create(0.5, 6.3, 10.1, -4.2);
            var y = LitBasics.Aggregate(ref x);
            var yy = 0.5 + 6.3 + 10.1 - 4.2;

            Assert.AreEqual(y, yy, 1e-10);
        }

        [Test]
        public unsafe void FloatAggreation()
        {
            var x = Vector256.Create(0.5f, 6.3f, 10.1f, -4.2f, 0.0f, 3.14159f, -0.0f, 10f);
            var y = LitBasics.Aggregate(ref x);
            var yy = 0.5 + 6.3 + 10.1 - 4.2 + 0.0f + 3.14159f - 0.0f + 10f;

            Assert.AreEqual(y, yy, 1e-5f);
        }
    }
}
