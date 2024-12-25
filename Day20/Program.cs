using Point = (int X, int Y);
using Map = string[];

Map map = File.ReadAllLines("input.txt");
var start = Find(map, 'S');
var end = Find(map, 'E');

var current = start;
var steps = 0;

var distances = new Dictionary<Point, int>
{
    [start] = 0
};

while (current != end)
{
    current = Neighbours(current).Single(nb => IsValid(map, nb) && !distances.ContainsKey(nb));
    steps++;
    distances[current] = steps;
}

var cheats = distances.Keys.SelectMany(p => FindCheats(distances, p, 2)).ToList();
var answer1 = cheats.Count(x => x >= 100);
Console.WriteLine($"Answer 1: {answer1}");

cheats = distances.Keys.SelectMany(p => FindCheats(distances, p, 20)).ToList();
var answer2 = cheats.Count(x => x >= 100);
Console.WriteLine($"Answer 2: {answer2}");

List<int> FindCheats(Dictionary<Point, int> distances, Point start, int steps)
{
    var result = new List<int>();

    var current = new List<Point>
    {
        start
    };

    var visited = new HashSet<Point>();

    for (int step = 1; step <= steps; step++)
    {
        visited.UnionWith(current);

        current = current
            .SelectMany(Neighbours)
            .Distinct()
            .Where(p => !visited.Contains(p))
            .ToList();

        var cheats = current.Where(distances.ContainsKey)
            .Select(p => distances[p] - distances[start] - step)
            .Where(x => x > 0)
            .ToList();

        result.AddRange(cheats);
    }

    return result;
}

Point Find(Map map, char symbol)
{
    for (int y = 0; y < map.Length; y++)
    {
        for (int x = 0; x < map[y].Length; x++)
        {
            if (map[y][x] == symbol)
            {
                return (x, y);
            }
        }
    }

    throw new();
}

bool IsValid(Map map, Point position)
{
    var valid = new List<char>
    {
        '.',
        'S',
        'E'
    };

    return 0 <= position.X && position.X <= map[0].Length &&
        0 <= position.Y && position.Y <= map.Length &&
        valid.Contains(map[position.Y][position.X]);
}

List<Point> Neighbours(Point position)
{
    return
    [
        (position.X, position.Y - 1),
        (position.X, position.Y + 1),
        (position.X - 1, position.Y),
        (position.X + 1, position.Y)
    ];
}
