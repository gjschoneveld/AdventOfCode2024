﻿using Point = (long X, long Y);

var input = File.ReadAllLines("input.txt");
var machines = Group(input).Select(Machine.Parse).ToList();

var answer1 = machines.Sum(m => m.TokensNeeded());
Console.WriteLine($"Answer 1: {answer1}");

var answer2 = machines.Sum(m => m.TokensNeededRaisedPrize());
Console.WriteLine($"Answer 2: {answer2}");

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

class Machine
{
    public Point A { get; set; }
    public Point B { get; set; }

    public Point Prize { get; set; }
    public Point RaisedPrize => (Prize.X + Raise, Prize.Y + Raise);

    public static long Raise => 10000000000000;

    public int TokensNeeded()
    {
        int result = 0;

        for (int a = 0; a <= 100; a++)
        {
            for (int b = 0; b <= 100; b++)
            {
                if (a * A.X + b * B.X != Prize.X || a * A.Y + b * B.Y != Prize.Y)
                {
                    continue;
                }

                var tokens = 3 * a + b;

                if (result == 0 || tokens < result)
                {
                    result = tokens;
                }
            }
        }

        return result;
    }

    public long TokensNeededRaisedPrize()
    {
        var numeratorA = B.X * RaisedPrize.Y - B.Y * RaisedPrize.X;
        var denominatorA = B.X * A.Y - B.Y * A.X;

        if (numeratorA % denominatorA != 0)
        {
            return 0;
        }

        var a = numeratorA / denominatorA;

        if (a < 0)
        {
            return 0;
        }

        var numeratorB = RaisedPrize.X - A.X * a;
        var denominatorB = B.X;

        if (numeratorB % denominatorB != 0)
        {
            return 0;
        }

        var b = numeratorB / denominatorB;

        if (b < 0)
        {
            return 0;
        }

        return 3 * a + b;
    }

    public static Point FindNumbers(string line)
    {
        var interesting = line.Where(c => char.IsDigit(c) || c == ',').ToArray();
        var comma = Array.IndexOf(interesting, ',');

        var x = int.Parse(new string(interesting[..comma]));
        var y = int.Parse(new string(interesting[(comma + 1)..]));

        return (x, y);
    }

    public static Machine Parse(List<string> lines)
    {
        return new()
        {
            A = FindNumbers(lines[0]),
            B = FindNumbers(lines[1]),
            Prize = FindNumbers(lines[2])
        };
    }
}
