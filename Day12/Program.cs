using Map = string[];
using Point = (int X, int Y);

Map map = File.ReadAllLines("input.txt");

var totalPrice = 0; 
var visited = new HashSet<Point>();

for (int y = 0; y < map.Length; y++)
{
    for (int x = 0; x < map[y].Length; x++)
    {
        if (visited.Contains((x, y)))
        {
            continue;
        }

        var symbol = map[y][x];
        var area = 0;
        var perimeter = 0;

        var toVisit = new Queue<Point>();
        toVisit.Enqueue((x, y));

        while(toVisit.Count > 0)
        {
            var current = toVisit.Dequeue();
            visited.Add(current);

            var neighbours = Neighbours(current).Where(nb => IsValid(map, nb) && map[nb.Y][nb.X] == symbol).ToList();
            perimeter += 4 - neighbours.Count;
            area++;

            foreach (var nb in neighbours.Where(nb => !visited.Contains(nb) && !toVisit.Contains(nb)))
            {
                toVisit.Enqueue(nb);
            }
        }

        totalPrice += area * perimeter;
    }
}

var answer1 = totalPrice;
Console.WriteLine($"Answer 1: {answer1}");


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