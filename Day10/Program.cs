using Map = string[];
using Point = (int X, int Y);

var map = File.ReadAllLines("input.txt");
var trailheads = GetTrailHeads(map);

var answer1 = trailheads.Sum(th => GetScore(map, th, false));
Console.WriteLine($"Answer 1: {answer1}");

var answer2 = trailheads.Sum(th => GetScore(map, th, true));
Console.WriteLine($"Answer 2: {answer2}");

List<Point> GetTrailHeads(Map map)
{
    var result = new List<Point>();

	for (int y = 0; y < map.Length; y++)
	{
		for (int x = 0; x < map[y].Length; x++)
		{
			if (map[y][x] == '0')
			{
				result.Add((x, y));
			}
		}
	}

	return result;
}

int GetScore(Map map, Point start, bool part2)
{
    var symbol = map[start.Y][start.X];

    var positions = new List<Point>
    {
        start
    };

    while (symbol != '9')
    {
        symbol++;

        positions = positions
            .SelectMany(Neighbours)
            .Where(nb => IsValid(map, nb) && map[nb.Y][nb.X] == symbol)
            .ToList();

		if (!part2)
		{
			positions = positions.Distinct().ToList();
		}
    }

    return positions.Count;
}

List<Point> Neighbours(Point position)
{
	return [
		(position.X - 1, position.Y),
        (position.X + 1, position.Y),
        (position.X, position.Y - 1),
        (position.X, position.Y + 1)
	];
}

bool IsValid(Map map, Point position)
{
    var lengthX = map[0].Length;
    var lengthY = map.Length;

    return 0 <= position.X && position.X < lengthX &&
        0 <= position.Y && position.Y < lengthY;
}
