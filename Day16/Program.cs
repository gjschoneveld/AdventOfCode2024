using Point = (int X, int Y);
using State = ((int X, int Y) Position, Direction Direction);
using Map = string[];

Map map = File.ReadAllLines("input.txt");
var nodes = FindNodes(map);

foreach (var (state, node) in nodes)
{
    WalkToNext(map, nodes, node);
}

var start = nodes.Single(n => n.Value.Type == Type.Start).Value;
var end = nodes.Where(n => n.Value.Type == Type.End).Select(n => n.Value).ToList();
var best = FindRoute(nodes, start, end);

var answer1 = best;
Console.WriteLine($"Answer 1: {answer1}");

var optimalNodes = new HashSet<Node>();
optimalNodes.UnionWith(end.Where(n => n.Score == best));
optimalNodes.Add(start);

foreach (var (_, node) in nodes.Where(kv => kv.Value.Type == Type.Regular))
{
    var route = FindRoute(nodes, start, [node]) + FindRoute(nodes, node, end);

    if (route == best)
    {
        optimalNodes.Add(node);
    }
}

// recalculate optimal scores
FindRoute(nodes, start, end);

var tiles = optimalNodes.Select(n => n.Position).Distinct().Count();

foreach (var node in optimalNodes)
{
    foreach (var (adjacent, delta) in node.Adjacent)
    {
        if (optimalNodes.Contains(adjacent) && adjacent.Score - node.Score == delta.Score)
        {
            tiles += delta.Tiles;
        }
    }
}

var answer2 = tiles;
Console.WriteLine($"Answer 2: {answer2}");

int FindRoute(Dictionary<State, Node> nodes, Node start, List<Node> end)
{
    foreach (var (_, node) in nodes)
    {
        node.Score = null;
    }

    var reachable = new PriorityQueue<State, int>();
    reachable.Enqueue(start.State, 0);

    while (reachable.Count > 0)
    {
        reachable.TryDequeue(out State state, out int score);

        if (nodes[state].Score != null)
        {
            continue;
        }

        nodes[state].Score = score;

        foreach (var (node, delta) in nodes[state].Adjacent)
        {
            reachable.Enqueue(node.State, score + delta.Score);
        }
    }

    return (int)end.Min(n => n.Score)!;
}

void WalkToNext(Map map, Dictionary<State, Node> nodes, Node node)
{
    var current = node.State;
    var next = Next(current);

    if (!IsValidPosition(map, next.Position))
    {
        // nothing in this direction
        return;
    }

    var previous = current;
    current = next;
    var score = 1;
    var tiles = 0;

    while (!nodes.ContainsKey(current))
    {
        next = Neighbours(current.Position).FirstOrDefault(nb => nb.Position != previous.Position && IsValidPosition(map, nb.Position));

        if (next == default)
        {
            // dead end
            return;
        }

        score++;
        tiles++;

        if (current.Direction != next.Direction)
        {
            score += 1000;
        }

        previous = current;
        current = next;
    }

    node.Adjacent[nodes[current]] = (score, tiles);
}

Dictionary<State, Node> FindNodes(Map map)
{
    var result = new Dictionary<State, Node>();

    for (int y = 0; y < map.Length; y++)
    {
        for (int x = 0; x < map[y].Length; x++)
        {
            var symbol = map[y][x];

            if (symbol == 'S' || symbol == 'E' || (symbol == '.' && Neighbours((x, y)).Count(nb => IsValidPosition(map, nb.Position)) >= 3))
            {
                var up = new Node
                {
                    Position = new Point(x, y),
                    Direction = Direction.Up,
                    Type = symbol == 'E' ? Type.End : Type.Regular
                };

                var down = new Node
                {
                    Position = new Point(x, y),
                    Direction = Direction.Down,
                    Type = symbol == 'E' ? Type.End : Type.Regular
                };

                var left = new Node
                {
                    Position = new Point(x, y),
                    Direction = Direction.Left,
                    Type = symbol == 'E' ? Type.End : Type.Regular
                };

                var right = new Node
                {
                    Position = new Point(x, y),
                    Direction = Direction.Right,
                    Type = symbol == 'S' ? Type.Start : symbol == 'E' ? Type.End : Type.Regular
                };

                up.Adjacent[left] = (1000, 0);
                up.Adjacent[right] = (1000, 0);
                down.Adjacent[left] = (1000, 0);
                down.Adjacent[right] = (1000, 0);
                left.Adjacent[up] = (1000, 0);
                left.Adjacent[down] = (1000, 0);
                right.Adjacent[up] = (1000, 0);
                right.Adjacent[down] = (1000, 0);

                result[(up.Position, up.Direction)] = up;
                result[(down.Position, down.Direction)] = down;
                result[(left.Position, left.Direction)] = left;
                result[(right.Position, right.Direction)] = right;
            }
        }
    }

    return result;
}

bool IsValidPosition(Map map, Point position)
{
    var symbol = map[position.Y][position.X];

    return symbol is 'S' or 'E' or '.';
}

State Next(State state)
{
    return state.Direction switch
    {
        Direction.Up => ((state.Position.X, state.Position.Y - 1), state.Direction),
        Direction.Down => ((state.Position.X, state.Position.Y + 1), state.Direction),
        Direction.Left => ((state.Position.X - 1, state.Position.Y), state.Direction),
        Direction.Right => ((state.Position.X + 1, state.Position.Y), state.Direction),
        _ => throw new(),
    };
}

List<State> Neighbours(Point position)
{
    return
    [
        Next((position, Direction.Up)),
        Next((position, Direction.Down)),
        Next((position, Direction.Left)),
        Next((position, Direction.Right))
    ];
}

class Node
{
    public Type Type;
    public Point Position { get; set; }
    public Direction Direction { get; set; }
    public Dictionary<Node, (int Score, int Tiles)> Adjacent { get; set; } = [];
    public int? Score { get; set; }

    public State State => (Position, Direction);
}

enum Direction
{
    Up,
    Down,
    Left,
    Right
}

enum Type
{
    Regular,
    Start,
    End
}
