using Point = (int X, int Y);

var input = File.ReadAllLines("input.txt");

var answer1 = FindLowestTotalComplexity(input, 2);
Console.WriteLine($"Answer 1: {answer1}");

var answer2 = FindLowestTotalComplexity(input, 25);
Console.WriteLine($"Answer 2: {answer2}");

long FindLowestTotalComplexity(string[] input, int robots)
{
    var keypads = new List<Keypad>
    {
        new NumericKeypad()
    };

    for (int i = 0; i < robots; i++)
    {
        keypads.Add(new DirectionalKeypad());
    }

    var totalComplexity = 0L;

    foreach (var line in input)
    {
        var length = FindShortestPath([.. line], keypads);
        var complexity = length * Value(line);

        totalComplexity += complexity;
    }

    return totalComplexity;
}

long FindShortestPath(List<char> buttons, List<Keypad> keypads)
{
    if (keypads.Count == 0)
    {
        return buttons.Count;
    }

    var total = 0L;

    foreach (var button in buttons)
    {
        var minimal = long.MaxValue;

        var paths = keypads[0].Press(button);

        foreach (var path in paths)
        {
            var length = FindShortestPath(path, keypads[1..]);
            minimal = Math.Min(minimal, length);
        }

        total += minimal;
    }

    return total;
}

static int Value(string line)
{
    return int.Parse(line.TrimEnd('A'));
}

abstract class Keypad
{
    public Dictionary<char, Point> Buttons { get; init; } = [];

    public Point Position { get; set; }

    public List<List<char>> GetWays(Point start, Point end)
    {
        if (start == end)
        {
            return new List<List<char>>
            {
                new List<char>()
            };
        }

        var result = new List<List<char>>();

        if (start.X > end.X)
        {
            var innerStart = (start.X - 1, start.Y);

            if (Buttons.Values.Contains(innerStart))
            {
                var innerWays = GetWays(innerStart, end);

                foreach (var way in innerWays)
                {
                    result.Add(['<', .. way]);
                }
            }
        }

        if (start.X < end.X)
        {
            var innerStart = (start.X + 1, start.Y);

            if (Buttons.Values.Contains(innerStart))
            {
                var innerWays = GetWays(innerStart, end);

                foreach (var way in innerWays)
                {
                    result.Add(['>', .. way]);
                }
            }
        }

        if (start.Y > end.Y)
        {
            var innerStart = (start.X, start.Y - 1);

            if (Buttons.Values.Contains(innerStart))
            {
                var innerWays = GetWays(innerStart, end);

                foreach (var way in innerWays)
                {
                    result.Add(['^', .. way]);
                }
            }
        }

        if (start.Y < end.Y)
        {
            var innerStart = (start.X, start.Y + 1);

            if (Buttons.Values.Contains(innerStart))
            {
                var innerWays = GetWays(innerStart, end);

                foreach (var way in innerWays)
                {
                    result.Add(['v', .. way]);
                }
            }
        }

        return result;
    }

    public List<List<char>> Press(char button)
    {
        var ways = GetWays(Position, Buttons[button]);

        foreach (var way in ways)
        {
            way.Add('A');
        }

        Position = Buttons[button];

        return ways;
    }
}

class NumericKeypad : Keypad
{
    public NumericKeypad()
    {
        Buttons = new()
        {
            ['0'] = (1, 3),
            ['1'] = (0, 2),
            ['2'] = (1, 2),
            ['3'] = (2, 2),
            ['4'] = (0, 1),
            ['5'] = (1, 1),
            ['6'] = (2, 1),
            ['7'] = (0, 0),
            ['8'] = (1, 0),
            ['9'] = (2, 0),
            ['A'] = (2, 3)
        };

        Position = Buttons['A'];
    }
}

class DirectionalKeypad : Keypad
{
    public DirectionalKeypad()
    {
        Buttons = new()
        {
            ['^'] = (1, 0),
            ['<'] = (0, 1),
            ['v'] = (1, 1),
            ['>'] = (2, 1),
            ['A'] = (2, 0)
        };

        Position = Buttons['A'];
    }
}
