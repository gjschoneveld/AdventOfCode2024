using Point = (int X, int Y);

var input = File.ReadAllLines("input.txt");
var positions = input.Select(Parse).ToList();
var example = input.Length == 25;
var max = example ? 6 : 70;
var count = example ? 12 : 1024;

var answer1 = Steps([.. positions[..count]], max);
Console.WriteLine($"Answer 1: {answer1}");

var left = count;
var right = positions.Count;

while (left < right)
{
    var middle = (left + right + 1) / 2;

    if (Steps([.. positions[..middle]], max) == null)
    {
        right = middle - 1;
    }
    else
    {
        left = middle;
    }
}

var answer2 = input[left];
Console.WriteLine($"Answer 2: {answer2}");

int? Steps(HashSet<Point> corrupt, int max)
{
    var steps = 0;
    var visited = new HashSet<Point>();
    var toVisit = new List<Point>
    {
        (0, 0)
    };

    while (toVisit.Count > 0 && !visited.Contains((max, max)))
    {
        var neighbours = toVisit
            .SelectMany(Neighbours)
            .Where(p => IsValid(p, max) && !visited.Contains(p) && !corrupt.Contains(p))
            .Distinct()
            .ToList();

        visited.UnionWith(neighbours);
        toVisit = neighbours;
        steps++;
    }

    return visited.Contains((max, max)) ? steps : null;
}

Point Parse(string line)
{
    var values = line.Split(',').Select(int.Parse).ToList();

    return (values[0], values[1]);
}

bool IsValid(Point position, int max)
{
    return 0 <= position.X && position.X <= max &&
        0 <= position.Y && position.Y <= max;
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
