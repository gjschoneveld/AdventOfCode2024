using Map = System.Collections.Generic.List<System.Collections.Generic.List<char>>;
using Point = (int X, int Y);

var map = File.ReadAllLines("input.txt").Select(x => x.ToList()).ToList();
var guard = (Position: Find(map, '^'), Direction: Direction.Up);

var route = Walk(map, guard);
var answer1 = route.Visited.Count;
Console.WriteLine($"Answer 1: {answer1}");

var answer2 = route.Visited.Where(p => map[p.Y][p.X] == '.').Count(p => Walk(map, guard, p).Loop);
Console.WriteLine($"Answer 2: {answer2}");

(bool Loop, List<Point> Visited) Walk(Map map, (Point Position, Direction Direction) guard, Point? obstruction = null)
{
    var visited = new HashSet<(Point Position, Direction Direction)>();

    while (IsValid(map, guard.Position) && !visited.Contains(guard))
    {
        visited.Add(guard);

        var next = Step(guard.Position, guard.Direction);

        if (IsValid(map, next) && (map[next.Y][next.X] == '#' || next == obstruction))
        {
            guard = (guard.Position, TurnRight(guard.Direction));

            continue;
        }

        guard = (next, guard.Direction);
    }

    return (visited.Contains(guard), visited.Select(g => g.Position).Distinct().ToList());
}

Point Find(Map map, char symbol)
{
    for (int y = 0; y < map.Count; y++)
    {
        var x = map[y].IndexOf(symbol);

        if (x >= 0)
        {
            return (x, y);
        }
    }

    throw new();
}

bool IsValid(Map map, Point position)
{
    var lengthX = map[0].Count;
    var lengthY = map.Count;

    return 0 <= position.X && position.X < lengthX &&
        0 <= position.Y && position.Y < lengthY;
}

Point Step(Point position, Direction direction)
{
    return direction switch
    {
        Direction.Up => (position.X, position.Y - 1),
        Direction.Down => (position.X, position.Y + 1),
        Direction.Left => (position.X - 1, position.Y),
        Direction.Right => (position.X + 1, position.Y),
        _ => throw new()
    };
}

Direction TurnRight(Direction direction)
{
    return direction switch
    {
        Direction.Up => Direction.Right,
        Direction.Down => Direction.Left,
        Direction.Left => Direction.Up,
        Direction.Right => Direction.Down,
        _ => throw new()
    };
}

enum Direction
{
    Up,
    Down,
    Left,
    Right
}
