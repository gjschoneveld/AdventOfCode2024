var input = File.ReadAllLines("input.txt");
var schematics = Group(input).Select(Schematic.Parse).ToList();

var locks = schematics.Where(s => s.Type == Type.Lock).ToList();
var keys = schematics.Where(s => s.Type == Type.Key).ToList();

var combinations = from l in locks
                   from k in keys
                   select (Lock: l, Key: k);

var answer = combinations.Count(c => c.Lock.Fits(c.Key));
Console.WriteLine($"Answer: {answer}");

static List<List<string>> Group(string[] input)
{
    List<List<string>> result = [];
    List<string> current = [];

    foreach (var line in input)
    {
        if (line == "")
        {
            result.Add(current);
            current = [];

            continue;
        }

        current.Add(line);
    }

    result.Add(current);

    return result;
}

class Schematic
{
    public required Type Type { get; set; }
    public required List<int> Heights { get; set; }

    public bool Fits(Schematic key)
    {
        return Heights.Zip(key.Heights, (l, k) => l + k).All(h => h <= 5);
    }

    public static Schematic Parse(List<string> input)
    {
        var type = input[0][0] == '#' ? Type.Lock : Type.Key;

        var heights = Enumerable.Range(0, input[0].Length)
            .Select(column => input.Count(line => line[column] == '#') - 1)
            .ToList();

        return new Schematic
        {
            Type = type,
            Heights = heights
        };
    }
}

enum Type
{
    Key,
    Lock
}
