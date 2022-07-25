# LitMath
 A collection of AVX-256 accelerated mathematical functions for .NET

 I rewrote `Exp`, `Log`, `Sin` and a few other useful functions using pure AVX intrinsics, so instead of doing one calculation per core, you can now do 4 doubles or 8 floats per core. I added the `Sqrt`, ERF function and a Normal Distribution CDF as well. On doubles, the following accuracies apply:
 
  - `Exp` and `Sqrt` run at double precision limits
  - `ERF` at `1e-13` 
  - `Sin` and `Cos` at `1e-15`
  - `Tan` in [0,$\pi/4$] at `2e-16`[1]
  - `ATan` at `1e-10` (working on it)

 There are examples in the benchmark and tests. But here is one to get you started anyway.

 Calculate `n` $e^x$'s in chunks of 4 and store the result in y.

 ```
int n = 40;
Span<double> x = new Span<double>(Enumerable.Range(0 , n).Select(z => (double)z/n).ToArray());
Span<double> y = new Span<double>(new double[n]);
LitExp.Exp(ref x, ref y);
 ```
 
## Parallel Processing
LitMath leverages SIMD for instruction level parallelism, but not compute cores. For array sizes large enough, it would be a really good idea to do multicore processing. There's an example called `LitExpDoubleParallel` in the ExpBenchmark.cs file to see one way to go about this. 

[1] `Tan` is a difficult function. If you need great precsion everywhere, you may want to look into using CORDIC based algorithms. Near $n*\pi/2$, Tan requires a divide by a relatively small or relatively large number that really harms precision. This is especially evident for n=odd. I am open to suggestions.
