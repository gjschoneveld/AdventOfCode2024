var input = File.ReadAllLines("input.txt");
var patterns = input[0].Split(", ").ToList();
var designs = input[2..];

var ways = designs.Select(d => CountDifferentWays(patterns, d, 0, [])).ToList();

var answer1 = ways.Count(w => w > 0);
Console.WriteLine($"Answer 1: {answer1}");

var answer2 = ways.Sum();
Console.WriteLine($"Answer 2: {answer2}");

static long CountDifferentWays(List<string> patterns, string design, int index, Dictionary<int, long> cache)
{
    if (index == design.Length)
    {
        return 1;
    }

    if (cache.ContainsKey(index))
    {
        return cache[index];
    }

    var ways = patterns
        .Where(p => index + p.Length <= design.Length && design.Substring(index, p.Length) == p)
        .Sum(p => CountDifferentWays(patterns, design, index + p.Length, cache));

    return cache[index] = ways;
}
