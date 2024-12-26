using System.Diagnostics.CodeAnalysis;

var input = File.ReadAllLines("input.txt");
var connections = input.Select(x => x.Split('-')).ToList();

var map = new Dictionary<string, List<string>>();

foreach (var connection in connections)
{
    map.TryAdd(connection[0], []);
    map.TryAdd(connection[1], []);

    map[connection[0]].Add(connection[1]);
    map[connection[1]].Add(connection[0]);
}

var setsOfThree = new HashSet<List<string>>(new SetComparer());

foreach (var (source, destinations) in map)
{
    for (var i = 0; i < destinations.Count; i++)
    {
        for (int j = i + 1; j < destinations.Count; j++)
        {
            if (map[destinations[i]].Contains(destinations[j]))
            {
                setsOfThree.Add([.. new List<string>
                {
                    source, destinations[i], destinations[j]
                }.Order()]);
            }
        }
    }
}

var answer1 = setsOfThree.Count(s => s.Any(c => c[0] == 't'));
Console.WriteLine($"Answer 1: {answer1}");

var lanParties = setsOfThree.Select(s => Expand(map, s)).ToHashSet(new SetComparer());
var largest = lanParties.MaxBy(p => p.Count)!;

var answer2 = string.Join(',', largest);
Console.WriteLine($"Answer 2: {answer2}");

List<string> Expand(Dictionary<string, List<string>> map, List<string> setOfThree)
{
    var toVisit = setOfThree.SelectMany(s => map[s]).ToHashSet();
    var visited = new HashSet<string>(setOfThree);
    var set = new HashSet<string>(setOfThree);

    while (toVisit.Count > 0)
    {
        var candidate = toVisit.First();
        toVisit.Remove(candidate);
        visited.Add(candidate);

        if (set.All(c => map[c].Contains(candidate)))
        {
            set.Add(candidate);
            toVisit.UnionWith(map[candidate].Where(c => !visited.Contains(c)));
        }
    }

    return [.. set.Order()];
}

class SetComparer : IEqualityComparer<List<string>>
{
    public bool Equals(List<string>? x, List<string>? y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);

        return x.SequenceEqual(y);
    }

    public int GetHashCode([DisallowNull] List<string> obj)
    {
        var hash = new HashCode();

        foreach (var x in obj)
        {
            hash.Add(x);
        }

        return hash.ToHashCode();
    }
}
