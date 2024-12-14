using Point = (int X, int Y);

var input = File.ReadAllLines("input.txt");
var robots = input.Select(Robot.Parse).ToList();
var example = robots.Count == 12;

var width = example ? 11 : 101;
var height = example ? 7 : 103;

int second = 1;

while (second <= 100)
{
    robots.ForEach(r => r.Step(width, height));
    second++;
}

var answer1 = robots
    .Select(r => FindQuadrant(r.Position, width, height))
    .Where(q => q != Quadrant.Unknown)
    .GroupBy(q => q)
    .Select(g => g.Count())
    .Aggregate((a, b) => a * b);

Console.WriteLine($"Answer 1: {answer1}");

// While printing I noticed some grouping when second % 101 == 11
// Found answer manually by looking at the printed state
var answer2 = 7687;

while (second <= answer2)
{
    robots.ForEach(r => r.Step(width, height));
    second++;
}

Console.WriteLine($"Answer 2: {answer2}");
Print(robots, width, height);

void Print(List<Robot> robots, int width, int height)
{
    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            var count = robots.Count(r => r.Position == (x, y));

            if (count == 0)
            {
                Console.Write(".");
            }
            else if (count <= 9)
            {
                Console.Write(count);
            }
            else
            {
                Console.Write("#");
            }
        }

        Console.WriteLine();
    }
}

Quadrant FindQuadrant(Point position, int width, int height)
{
    var centerX = width / 2;
    var centerY = height / 2;

    if (position.X < centerX && position.Y < centerY)
    {
        return Quadrant.TopLeft;
    }

    if (position.X > centerX && position.Y < centerY)
    {
        return Quadrant.TopRight;
    }

    if (position.X < centerX && position.Y > centerY)
    {
        return Quadrant.BottomLeft;
    }

    if (position.X > centerX && position.Y > centerY)
    {
        return Quadrant.BottomRight;
    }

    return Quadrant.Unknown;
}

class Robot
{
    public Point Position { get; set; }
    public Point Velocity { get; set; }

    public void Step(int width, int height)
    {
        Position = (Modulo(Position.X + Velocity.X, width), Modulo(Position.Y + Velocity.Y, height));
    }

    public int Modulo(int x, int mod)
    {
        return (x + mod) % mod;
    }

    public static Robot Parse(string line)
    {
        var values = line.Split(['p', 'v', '=', ',', ' '], StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

        return new()
        {
            Position = (values[0], values[1]),
            Velocity = (values[2], values[3])
        };
    }
}

enum Quadrant
{
    Unknown,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}
