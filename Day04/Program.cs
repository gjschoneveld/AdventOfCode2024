using Point = (int X, int Y);

var grid = File.ReadAllLines("input.txt");

var answer1 = CountWord(grid, "XMAS");
Console.WriteLine($"Answer 1: {answer1}");

int CountWord(string[] grid, string word)
{
    var result = 0;

    for (int y = 0; y < grid.Length; y++)
    {
        for (int x = 0; x < grid[y].Length; x++)
        {
            var words = ExtractWords(grid, (x, y), word.Length);
            result += words.Count(w => w == word);
        }
    }

    return result;
}

List<string> ExtractWords(string[] grid, Point start, int count)
{
    List<Direction> directions =
    [
        Direction.NorthWest,
        Direction.North,
        Direction.NorthEast,
        Direction.West,
        Direction.East,
        Direction.SouthWest,
        Direction.South,
        Direction.SoutEast
    ];

    return directions.Select(d => ExtractWord(grid, start, d, count)).ToList();
}

bool IsValidPosition(string[] grid, Point position)
{
    var maxX = grid[0].Length;
    var maxY = grid.Length;

    return 0 <= position.X && position.X < maxX &&
        0 <= position.Y && position.Y < maxY;
}

string ExtractWord(string[] grid, Point start, Direction direction, int count)
{
    Point delta = direction switch
    {
        Direction.NorthWest => (-1, -1),
        Direction.North => (0, -1),
        Direction.NorthEast => (1, -1),
        Direction.West => (-1, 0),
        Direction.East => (1, 0),
        Direction.SouthWest => (-1, 1),
        Direction.South => (0, 1),
        Direction.SoutEast => (1, 1),
        _ => throw new()
    };

    var result = "";

    for (int i = 0; i < count; i++)
    {
        Point position = (start.X + i * delta.X, start.Y + i * delta.Y);

        if (!IsValidPosition(grid, position))
        {
            break;
        }

        result += grid[position.Y][position.X];
    }

    return result;
}

enum Direction
{
    NorthWest,
    North,
    NorthEast,
    West,
    East,
    SouthWest,
    South,
    SoutEast
}
