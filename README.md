# LitMath
 A collection of AVX-256 accelerated mathematical functions for .NET

 I rewrote `Exp`, `Log`, `Sin`, `Cos` and `Tan` using pure AVX intrinsics, so instead of doing one calculation per core, you can now do 4 doubles or 8 floats per core. I added the `Sqrt`, ERF function and a Normal Distribution CDF as well. `Exp`, `Log`, `Sin`, `Cos` and `Tan` all run between `1e-8` and `1e-10` in worst-case accuracy for double precision.

 There are examples in the benchmark and tests. But here is one to get you started anyway.

 Calculate `n` $e^x$'s in chunks of 4 and store the result in y.

 ```
int n = 40;
Span<double> x = new Span<double>(Enumerable.Range(0 , n).Select(z => (double)z/n).ToArray());
Span<double> y = new Span<double>(new double[n]);
LitExp.Exp(ref x, ref y);
 ```
 
## Parallel Processing
LitMath leverages SIMD for instruction level parallelism, but not compute cores. For array sizes large enough, this it would be a really good idea to do multicore processing. There's an example called `LitExpDoubleParallel` in the ExpBenchmark.cs file to see one way to go about this. 
 
## Warnings and Friendly Advise
The only reason you should be here is that you're trying to squeeze every ounce of performance out of your math problem. To this end, please keep in mind that `LitMath` is only one possible tool for a small subset of use cases, and I provide no guarantees or endorsements that it is the best solution for your problem. I made it because I don't have an AVX-512 enabled processor, and I need to compute these functions on small to medium sized arrays of `double`s, and I'm locked into C# for other reasons, and because this is the best I could do. Optimization is a complex topic balancing error tolerances, hardware availability, compilers, language support, and so on. From what I can tell, the gold standard for doing problems like the ones supported here are via Intel's oneAPI (C# wrapper: [MKL.NET](https://github.com/MKL-NET/MKL.NET)). Unfortunately, at the small-to-medium array sizes I'm concerned with, `DllImport`ing eats up too much time. 
