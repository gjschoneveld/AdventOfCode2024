using Point = (int X, int Y);

var input = File.ReadAllLines("input.txt");
var separator = Array.FindIndex(input, line => line == "");
var map = input[..separator];
var moves = input[separator..].SelectMany(line => line).ToList();

var walls = Find(map, '#');
var boxes = Find(map, 'O');
var robot = Find(map, '@').Single();

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

walls = Find(map, '#').SelectMany(p => new List<Point> { (2 * p.X, p.Y), (2 * p.X + 1, p.Y) }).ToHashSet();
boxes = Find(map, 'O').Select(p => (2 * p.X, p.Y)).ToHashSet();
robot = Find(map, '@').Select(p => (2 * p.X, p.Y)).Single();

foreach (var move in moves)
{
    //PrintPart2(map, walls, boxes, robot);
    //Console.WriteLine();
    //Console.WriteLine(move);

    var next = Next(robot, move);

    if (walls.Contains(next))
    {
        continue;
    }

    var box = FindBox(boxes, next);

    if (box != null)
    {
        if (!MoveAllowed(walls, boxes, box.Value, move))
        {
            continue;
        }

        Move(walls, boxes, box.Value, move);
    }

    robot = next;
}

//PrintPart2(map, walls, boxes, robot);

var answer2 = boxes.Sum(GpsCoordinate);
Console.WriteLine($"Answer 2: {answer2}");

void PrintPart2(string[] map, HashSet<Point> walls, HashSet<Point> boxes, Point robot)
{
    for (int y = 0; y < map.Length; y++)
    {
        for (int x = 0; x < map[y].Length * 2; x++)
        {
            Point current = (x, y);
            Point left = (x - 1, y);

            if (current == robot)
            {
                Console.Write('@');
            }
            else if (walls.Contains(current))
            {
                Console.Write('#');
            }
            else if (boxes.Contains(current))
            {
                Console.Write('[');
            }
            else if (boxes.Contains(left))
            {
                Console.Write(']');
            }
            else
            {
                Console.Write('.');
            }
        }

        Console.WriteLine();
    }
}

Point? FindBox(HashSet<Point> boxes, Point position)
{
    if (boxes.Contains(position))
    {
        return position;
    }

    position = Next(position, '<');

    if (boxes.Contains(position))
    {
        return position;
    }

    return null;
}

bool MoveAllowed(HashSet<Point> walls, HashSet<Point> boxes, Point box, char direction)
{
    var left = Next(box, '<');
    var right = Next(Next(box, '>'), '>');

    var topLeft = Next(box, '^');
    var topRight = Next(Next(box, '^'), '>');

    var bottomLeft = Next(box, 'v');
    var bottomRight = Next(Next(box, 'v'), '>');

    List<Point> positionsToCheck = direction switch
    {
        '<' => [left],
        '>' => [right],
        '^' => [topLeft, topRight],
        'v' => [bottomLeft, bottomRight],
        _ => throw new()
    };

    if (positionsToCheck.Any(walls.Contains))
    {
        return false;
    }

    var boxesToCheck = positionsToCheck
        .Select(p => FindBox(boxes, p))
        .Where(p => p != null)
        .Select(p => p!.Value)
        .Distinct()
        .ToList();

    return boxesToCheck.All(p => MoveAllowed(walls, boxes, p, direction));
}

void Move(HashSet<Point> walls, HashSet<Point> boxes, Point box, char direction)
{
    var left = Next(box, '<');
    var right = Next(Next(box, '>'), '>');

    var topLeft = Next(box, '^');
    var topRight = Next(Next(box, '^'), '>');

    var bottomLeft = Next(box, 'v');
    var bottomRight = Next(Next(box, 'v'), '>');

    List<Point> positionsToMove = direction switch
    {
        '<' => [left],
        '>' => [right],
        '^' => [topLeft, topRight],
        'v' => [bottomLeft, bottomRight],
        _ => throw new()
    };

    var boxesToMove = positionsToMove
        .Select(p => FindBox(boxes, p))
        .Where(p => p != null)
        .Select(p => p!.Value)
        .Distinct()
        .ToList();

    boxesToMove.ForEach(p => Move(walls, boxes, p, direction));

    boxes.Remove(box);
    boxes.Add(Next(box, direction));
}

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
