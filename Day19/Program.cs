var input = File.ReadAllLines("input.txt");
var patterns = input[0].Split(", ").ToList();
var designs = input[2..];

var ways = designs.Select(d => CountDifferentWays(patterns, [], d, 0)).ToList();

var answer1 = ways.Count(w => w > 0);
Console.WriteLine($"Answer 1: {answer1}");

var answer2 = ways.Sum();
Console.WriteLine($"Answer 2: {answer2}");

static long CountDifferentWays(List<string> patterns, Dictionary<int, long> cache, string design, int index)
{
    if (index == design.Length)
    {
        return 1;
    }

    if (cache.ContainsKey(index))
    {
        return cache[index];
    }

    var ways = 0L;
    var maxLength = design.Length - index;

    foreach (var pattern in patterns)
    {
        if (pattern.Length > maxLength)
        {
            continue;
        }

        var substring = design.Substring(index, pattern.Length);

        if (pattern != substring)
        {
            continue;
        }

        ways += CountDifferentWays(patterns, cache, design, index + pattern.Length);        
    }

    cache[index] = ways;

    return ways;
}
