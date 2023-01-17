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
 
## Speedups
Below is a Benchmark.net example that compares LitMath used serally and in parallel to the naive implementation and an invocation of the MKL for computing `Exp` on an `N` sized array.

|               Method |        N |               Mean |           Error |          StdDev |
|--------------------- |--------- |-------------------:|----------------:|----------------:|
|       NaiveExpDouble |        3 |          11.162 ns |       0.0619 ns |       0.0579 ns |
|         LitExpDouble |        3 |           6.675 ns |       0.0452 ns |       0.0377 ns |
| LitExpDoubleParallel |        3 |                 NA |              NA |              NA |
|            ExpMklNet |        3 |          29.871 ns |       0.1172 ns |       0.1039 ns |
|       NaiveExpDouble |       64 |         240.154 ns |       0.4125 ns |       0.3445 ns |
|         LitExpDouble |       64 |          71.946 ns |       0.3332 ns |       0.2954 ns |
| LitExpDoubleParallel |       64 |       4,428.705 ns |      19.1108 ns |      16.9412 ns |
|            ExpMklNet |       64 |         116.934 ns |       0.6166 ns |       0.5466 ns |
|       NaiveExpDouble |     2048 |       7,553.526 ns |      19.0518 ns |      17.8211 ns |
|         LitExpDouble |     2048 |       2,446.337 ns |       9.8982 ns |       8.7745 ns |
| LitExpDoubleParallel |     2048 |       8,950.587 ns |      43.8549 ns |      38.8763 ns |
|            ExpMklNet |     2048 |       3,239.321 ns |       7.1020 ns |       6.6432 ns |
|       NaiveExpDouble |    65536 |     232,554.102 ns |     434.3946 ns |     339.1467 ns |
|         LitExpDouble |    65536 |      82,345.836 ns |     316.1190 ns |     295.6979 ns |
| LitExpDoubleParallel |    65536 |      33,246.643 ns |     112.4273 ns |     105.1645 ns |
|            ExpMklNet |    65536 |      19,159.788 ns |     189.5683 ns |     158.2981 ns |
|       NaiveExpDouble | 32000000 | 111,572,790.000 ns | 128,706.4656 ns | 114,094.9294 ns |
|         LitExpDouble | 32000000 |  47,072,238.961 ns |  95,635.2682 ns |  84,778.1743 ns |
| LitExpDoubleParallel | 32000000 |   4,904,424.323 ns |  28,642.6496 ns |  26,792.3522 ns |
|            ExpMklNet | 32000000 |  23,692,375.938 ns | 461,596.1351 ns | 690,895.3606 ns |

 
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
