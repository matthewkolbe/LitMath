# LitMath
 A collection of AVX-256 accelerated mathematical functions for .NET

 I rewrote `Exp`, `Log`, and `Sin` using pure AVX intrinsics, so instead of doing one calculation per core, you can now do 4 doubles or 8 floats per core. I added the ERF function and a Normal Distribution CDF as well. Exp, Log and Sin run between `1e-8` and `1e-10` in worst-case accuracy for double precision.

 There are examples in the benchmark and tests. But here is one to get you started anyway.

 Calculate 4 $e^x$'s at once and store the result in y.

 ```
Span<double> x = new Span<double>(Enumerable.Range(-20, 20).Select(z => (double)z).ToArray());
Span<double> y = new Span<double>(new double[x.Length]);
LitExp.Exp(ref x, ref y);
 ```
