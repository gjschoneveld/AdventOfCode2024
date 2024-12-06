using Point = (int X, int Y);

var map = File.ReadAllLines("input.txt");

var obstructions = Find(map, '#');
var guard = (Position: Find(map, '^').Single(), Direction: Direction.Up);

var route = Walk(map, obstructions, guard);

var answer1 = route.Visited;
Console.WriteLine($"Answer 1: {answer1}");

var positions = 0;

for (int y = 0; y < map.Length; y++)
{
    for (int x = 0; x < map[y].Length; x++)
    {
        if (map[y][x] != '.')
        {
            continue;
        }

        obstructions.Add((x, y));

        route = Walk(map, obstructions, guard);

        if (route.Loop)
        {
            positions++;
        }

        obstructions.Remove((x, y));
    }
}

var answer2 = positions;
Console.WriteLine($"Answer 2: {answer2}");

(bool Loop, int Visited) Walk(string[] map, HashSet<Point> obstructions, (Point Position, Direction Direction) guard)
{
    var visited = new HashSet<(Point Position, Direction Direction)>();

    while (IsValid(map, guard.Position) && !visited.Contains(guard))
    {
        visited.Add(guard);

        var next = Step(guard.Position, guard.Direction);

        if (obstructions.Contains(next))
        {
            guard = (guard.Position, TurnRight(guard.Direction));

            continue;
        }

        guard = (next, guard.Direction);
    }

    return (visited.Contains(guard), visited.Select(g => g.Position).Distinct().Count());
}

HashSet<Point> Find(string[] map, char type)
{
    var result = new HashSet<Point>();

    for (int y = 0; y < map.Length; y++)
    {
        for (int x = 0; x < map[y].Length; x++)
        {
            if (map[y][x] == type)
            {
                result.Add((x, y));
            }
        }
    }

    return result;
}

bool IsValid(string[] map, Point position)
{
    var lengthX = map[0].Length;
    var lengthY = map.Length;

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
