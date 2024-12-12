using Map = string[];
using Point = (int X, int Y);

Map map = File.ReadAllLines("input.txt");

var totalPrice = 0;
var discountedPrice = 0;
var visited = new HashSet<Point>();

for (int y = 0; y < map.Length; y++)
{
    for (int x = 0; x < map[y].Length; x++)
    {
        if (visited.Contains((x, y)))
        {
            continue;
        }

        var symbol = map[y][x];
        var area = 0;

        var toVisit = new Queue<Point>();
        toVisit.Enqueue((x, y));

        var edges = new List<(Point Position, Direction Direction)>();

        while(toVisit.Count > 0)
        {
            var current = toVisit.Dequeue();
            visited.Add(current);

            var neighbours = Neighbours(current).Where(nb => IsValid(map, nb.Position) && map[nb.Position.Y][nb.Position.X] == symbol).ToList();

            var directions = new List<Direction>
            {
                Direction.North,
                Direction.South,
                Direction.West,
                Direction.East
            };

            edges.AddRange(directions.Where(d => !neighbours.Any(nb => nb.Direction == d)).Select(d => (current, d)));
            area++;

            foreach (var nb in neighbours.Where(nb => !visited.Contains(nb.Position) && !toVisit.Contains(nb.Position)))
            {
                toVisit.Enqueue(nb.Position);
            }
        }

        totalPrice += area * edges.Count;
        discountedPrice += area * CountSides(edges);
    }
}

var answer1 = totalPrice;
Console.WriteLine($"Answer 1: {answer1}");

var answer2 = discountedPrice;
Console.WriteLine($"Answer 2: {answer2}");

int CountSides(List<(Point Position, Direction Direction)> edges)
{
    return CountSidesVertical(edges, Direction.West) +
        CountSidesVertical(edges, Direction.East) +
        CountSidesHorizontal(edges, Direction.North) +
        CountSidesHorizontal(edges, Direction.South);
}

int CountSidesVertical(List<(Point Position, Direction Direction)> edges, Direction direction)
{
    int sides = 0;

    var positions = edges.Where(e => e.Direction == direction).Select(e => e.Position).OrderBy(e => e.X).ThenBy(e => e.Y).ToList();

    Point previous = (-1, -1);

    foreach (var position in positions)
    {
        if (position.X != previous.X || position.Y - previous.Y != 1)
        {
            sides++;
        }

        previous = position;
    }

    return sides;
}

int CountSidesHorizontal(List<(Point Position, Direction Direction)> edges, Direction direction)
{
    int sides = 0;

    var positions = edges.Where(e => e.Direction == direction).Select(e => e.Position).OrderBy(e => e.Y).ThenBy(e => e.X).ToList();

    Point previous = (-1, -1);

    foreach (var position in positions)
    {
        if (position.Y != previous.Y || position.X - previous.X != 1)
        {
            sides++;
        }

        previous = position;
    }

    return sides;
}

List<(Point Position, Direction Direction)> Neighbours(Point position)
{
    return [
        ((position.X - 1, position.Y), Direction.West),
        ((position.X + 1, position.Y), Direction.East),
        ((position.X, position.Y - 1), Direction.North),
        ((position.X, position.Y + 1), Direction.South)
    ];
}

bool IsValid(Map map, Point position)
{
    var lengthX = map[0].Length;
    var lengthY = map.Length;

    return 0 <= position.X && position.X < lengthX &&
        0 <= position.Y && position.Y < lengthY;
}

enum Direction
{
    North,
    South,
    West,
    East
}
