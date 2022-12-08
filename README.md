# LitMath
 A collection of AVX-256 accelerated mathematical functions for .NET

 I rewrote `Exp`, `Log`, `Sin` and a few other useful functions using pure AVX intrinsics, so instead of doing one calculation per core, you can now do 4 doubles or 8 floats per core. I added the `Sqrt`, ERF function and a Normal Distribution CDF as well. On doubles, the following accuracies apply:
 
  - `Exp` and `Sqrt` run at double precision limits
  - `ERF` at `1e-13` 
  - `Sin` and `Cos` at `1e-15`
  - `Tan` in $[0,\pi/4]$ at `2e-16`
  - `ATan` at `1e-10` (working on it)

 There are examples in the benchmark and tests. But here is one to get you started anyway.

 Calculate `n` $e^x$'s in chunks of 4 and store the result in y.

 ```
int n = 40;
Span<double> x = new Span<double>(Enumerable.Range(0 , n).Select(z => (double)z/n).ToArray());
Span<double> y = new Span<double>(new double[n]);
Lit.Exp(in x, ref y);
 ```
 
## Parallel Processing
LitMath leverages SIMD for instruction level parallelism, but not compute cores. For array sizes large enough, it would be a really good idea to do multicore processing. There's an example called `LitExpDoubleParallel` in the ExpBenchmark.cs file to see one way to go about this. 

## Non-Math Things
Making a library like this involves reinventing the wheel so to speak on very basic concepts. The `Util` class includes methods like `Max` and `Min` and `IfElse`, which are key to many programming problems in AVX programming, because it needs to be branch-free.

## FAQ
#### Why does this exist?
The reasons I hear most for why this library is pointless is that the Intel MKL can do everything here better than I or C# can, and that if you want such extreme optimization, you shouldn't be using C# to begin with. I wholeheartedly disagree with both. C# is a great language with increasingly great compilers, and the performance I've been getting in this library is close to what you'd find in the MKL. Choosing C# to do some serious back end math with is a totally fine choice, and if you do math, then you might care about performance, or about cloud computing fees. And for these reasons, optimization to its fullest extent can matter.

The MKL is great. But marshaling objects out of C# into C in order to use the MKL is not great. I have benchmarks that compare the `exp` function running on an array in `LitMath` and the MKL, and for <2000, LitMath wins on my Zen 3 processor. And it wins by a lot (10x) when you're talking about 256 bit sized arrays. This is the most interesting thing to me. Because with `LitMath` you can chain the `Vector256` interfaced functions together to make whatever `ComplexFunction(Vector256 x)` you would like, then instead of thrashing your cache by running each `n` sized array through the MKL over and over to get to your complex function, simply run the full function over each `x` to get `y`. That is, the MKL would require you to have intermediate results for each basic function run, and these intermediate results would be run over the entire array each time. But by making your entire function a single `Vector256` to `Vector256`, you only run though the array once. The `ERF` is a good example of this. 

#### Why do you have such erratic deployments?
This library is an essential part of the code base I use at my job. When I need a new function, I add/deploy as I go, and sometimes that involves several deployments within a few hours. 