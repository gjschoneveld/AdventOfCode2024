using Point = (int X, int Y);

var input = File.ReadAllLines("input.txt");
var separator = Array.FindIndex(input, line => line == "");
var map = input[..separator];
var moves = input[separator..].SelectMany(line => line).ToList();

var walls = Find(input, '#');
var boxes = Find(input, 'O');
var robot = Find(input, '@').Single();

foreach (var move in moves)
{
	var next = Next(robot, move);

	if (walls.Contains(next))
	{
		continue;
	}

	if (boxes.Contains(next))
	{
		var behind = next;

		while (boxes.Contains(behind))
		{
			behind = Next(behind, move);
		}

        if (walls.Contains(behind))
        {
            continue;
        }

		boxes.Remove(next);
		boxes.Add(behind);
    }

    robot = next;
}

var answer1 = boxes.Sum(GpsCoordinate);
Console.WriteLine($"Answer 1: {answer1}");

HashSet<Point> Find(string[] input, char symbol)
{
	var result = new HashSet<Point>();

	for (int y = 0; y < input.Length; y++)
	{
		for (int x = 0; x < input[y].Length; x++)
		{
			if (input[y][x] == symbol)
			{
				result.Add((x, y));
			}
		}
	}

	return result;
}

Point Next(Point position, char direction)
{
	return direction switch
	{
		'<' => (position.X - 1, position.Y),
        '>' => (position.X + 1, position.Y),
        '^' => (position.X, position.Y - 1),
        'v' => (position.X, position.Y + 1),
		_ => throw new()
    };
}

int GpsCoordinate(Point position)
{
	return 100 * position.Y + position.X;
}
