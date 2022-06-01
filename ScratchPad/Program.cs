using LitMath;

Span<double> x = new Span<double>(new[] { 0.0, 0.0 });
LitUtilities.Apply(ref x, 2.0);
Console.WriteLine(x[1]);

Console.WriteLine("Use me to play around");