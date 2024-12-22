using Point = (int X, int Y);
using State = ((int X, int Y) Position, Direction Direction);
using Map = string[];

Map map = File.ReadAllLines("input.txt");
var nodes = FindNodes(map);

foreach (var (state, node) in nodes)
{
    WalkToNext(map, nodes, node);
}

var answer1 = FindRoute(nodes);
Console.WriteLine($"Answer 1: {answer1}");

int FindRoute(Dictionary<State, Node> nodes)
{
    var reachable = new Dictionary<State, int>
    {
        { nodes.Single(n => n.Value.Type == Type.Start).Value.State, 0 }
    };

    while (true)
    {
        var (state, distance) = reachable.OrderBy(kv => kv.Value).First();
        nodes[state].Distance = distance;
        reachable.Remove(state);

        if (nodes[state].Type == Type.End)
        {
            return distance;
        }
        
        var toVisit = nodes[state].Adjacent.Where(kv => kv.Key.Distance == null).ToList();

        foreach (var (node, delta) in toVisit)
        {
            var newDistance = distance + delta;

            if (!reachable.ContainsKey(node.State) || reachable[node.State] > newDistance)
            {
                reachable[node.State] = newDistance;
            }
        }
    }
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

    while (!nodes.ContainsKey(current))
    {
        next = Neighbours(current.Position).FirstOrDefault(nb => nb.Position != previous.Position && IsValidPosition(map, nb.Position));

        if (next == default)
        {
            // dead end
            return;
        }

        score++;

        if (current.Direction != next.Direction)
        {
            score += 1000;
        }

        previous = current;
        current = next;
    }

    node.Adjacent[nodes[current]] = score;
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

                up.Adjacent[left] = 1000;
                up.Adjacent[right] = 1000;
                down.Adjacent[left] = 1000;
                down.Adjacent[right] = 1000;
                left.Adjacent[up] = 1000;
                left.Adjacent[down] = 1000;
                right.Adjacent[up] = 1000;
                right.Adjacent[down] = 1000;

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
    public Dictionary<Node, int> Adjacent { get; set; } = [];
    public int? Distance { get; set; }

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
