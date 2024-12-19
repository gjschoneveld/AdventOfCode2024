var input = File.ReadAllLines("input.txt");
var patterns = input[0].Split(", ").ToList();
var designs = input[2..];

var answer1 = designs.Count(d => IsValid(patterns, [], d, 0));
Console.WriteLine($"Answer 1: {answer1}");

static bool IsValid(List<string> patterns, HashSet<int> invalid, string design, int index)
{
    if (index == design.Length)
    {
        return true;
    }

    if (invalid.Contains(index))
    {
        return false;
    }

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

        if (IsValid(patterns, invalid, design, index + pattern.Length))
        {
            return true;
        }            
    }

    invalid.Add(index);

    return false;
}
