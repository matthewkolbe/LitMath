using LitMath;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

var r = Fma.MultiplyAdd(Vector256.Create(1.0, 2.0, 3.0, 4.0), Vector256.Create(1.0), Vector256.Create(1.0));

Console.WriteLine(r);
Console.WriteLine("Use me to play around");