using Point = (int X, int Y);
using Map = string[];

var map = File.ReadAllLines("input.txt");
var antennas = FindAntennas(map);

var antinodes1 = FindAntinodes(map, antennas);
var answer1 = antinodes1.Count;
Console.WriteLine($"Answer 1: {answer1}");

var antinodes2 = FindAntinodes(map, antennas, true);
var answer2 = antinodes2.Count;
Console.WriteLine($"Answer 2: {answer2}");

Dictionary<char, List<Point>> FindAntennas(Map map)
{
    var result = new Dictionary<char, List<Point>>();

    for (int y = 0; y < map.Length; y++)
    {
        for (int x = 0; x < map[y].Length; x++)
        {
            if (map[y][x] == '.')
            {
                continue;
            }

            var frequency = map[y][x];
            result.TryAdd(frequency, []);
            result[frequency].Add((x, y));
        }
    }

    return result;
}

HashSet<Point> FindAntinodes(Map map, Dictionary<char, List<Point>> antennas, bool part2 = false)
{
    var result = new HashSet<Point>();

    foreach (var (type, positions) in antennas)
    {
        for (int i = 0; i < positions.Count; i++)
        {
            for (int j = i + 1; j < positions.Count; j++)
            {
                List<Point> antinodes;

                if (!part2)
                {
                    antinodes = FindAntinodesPart1(map, positions[i], positions[j]);
                }
                else
                {
                    antinodes = FindAntinodesPart2(map, positions[i], positions[j]);
                }

                result.UnionWith(antinodes);
            }
        }
    }

    return result;
}

List<Point> FindAntinodesPart1(Map map, Point a, Point b)
{
    var difference = Subtract(b, a);

    var antinodes = new List<Point>
    {
        Subtract(a, difference),
        Add(b, difference)
    };

    return antinodes.Where(p => IsValid(map, p)).ToList();
}

List<Point> FindAntinodesPart2(Map map, Point a, Point b)
{
    var difference = Subtract(b, a);

    var antinodes = new List<Point>();

    while (IsValid(map, a))
    {
        antinodes.Add(a);
        a = Subtract(a, difference);
    }

    while (IsValid(map, b))
    {
        antinodes.Add(b);
        b = Add(b, difference);
    }

    return antinodes;
}

Point Add(Point a, Point b)
{
    return (a.X + b.X, a.Y + b.Y);
}

Point Subtract(Point a, Point b)
{
    return (a.X - b.X, a.Y - b.Y);
}

bool IsValid(Map map, Point position)
{
    var lengthX = map[0].Length;
    var lengthY = map.Length;

    return 0 <= position.X && position.X < lengthX &&
        0 <= position.Y && position.Y < lengthY;
}
