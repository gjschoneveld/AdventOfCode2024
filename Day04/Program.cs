using Point = (int X, int Y);

var grid = File.ReadAllLines("input.txt");

var answer1 = CountWord(grid, "XMAS");
Console.WriteLine($"Answer 1: {answer1}");

var answer2 = CountCrossWord(grid, "MAS");
Console.WriteLine($"Answer 2: {answer2}");

int CountWord(string[] grid, string word)
{
    var result = 0;

    for (int y = 0; y < grid.Length; y++)
    {
        for (int x = 0; x < grid[y].Length; x++)
        {
            result += CountWordAtPosition(grid, (x, y), word);
        }
    }

    return result;
}

int CountCrossWord(string[] grid, string word)
{
    var result = 0;

    for (int y = 0; y < grid.Length; y++)
    {
        for (int x = 0; x < grid[y].Length; x++)
        {
            if (MatchesCrossWord(grid, (x, y), word))
            {
                result++;
            }
        }
    }

    return result;
}

int CountWordAtPosition(string[] grid, Point start, string word)
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
        Direction.SouthEast
    ];

    return directions.Count(d => MatchesWord(grid, start, d, word));
}

bool MatchesCrossWord(string[] grid, Point start, string word)
{
    Point southEast = (start.X + word.Length - 1, start.Y + word.Length - 1);

    Point northEast = (start.X + word.Length - 1, start.Y);
    Point southWest = (start.X, start.Y + word.Length - 1);

    return (MatchesWord(grid, start, Direction.SouthEast, word) || MatchesWord(grid, southEast, Direction.NorthWest, word)) &&
        (MatchesWord(grid, northEast, Direction.SouthWest, word) || MatchesWord(grid, southWest, Direction.NorthEast, word));
}

bool IsValidPosition(string[] grid, Point position)
{
    var maxX = grid[0].Length;
    var maxY = grid.Length;

    return 0 <= position.X && position.X < maxX &&
        0 <= position.Y && position.Y < maxY;
}

bool MatchesWord(string[] grid, Point start, Direction direction, string word)
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
        Direction.SouthEast => (1, 1),
        _ => throw new()
    };

    for (int i = 0; i < word.Length; i++)
    {
        Point position = (start.X + i * delta.X, start.Y + i * delta.Y);

        if (!IsValidPosition(grid, position) || grid[position.Y][position.X] != word[i])
        {
            return false;
        }
    }

    return true;
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
    SouthEast
}
