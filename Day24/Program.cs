var input = File.ReadAllLines("input.txt");
var separator = Array.FindIndex(input, line => line == "");
var start = input[..separator].Select(ParseValue).ToList();
var gates = input[(separator + 1)..].Select(Gate.Parse).ToList();

var values = start.ToDictionary(x => x.Node, x => x.Value);
var bits = values.Count(kv => kv.Key[0] == 'x');
var x = GetValue(values, bits, 'x');
var y = GetValue(values, bits, 'y');

var answer1 = Simulate(gates, bits, x, y).Result;
Console.WriteLine($"Answer 1: {answer1}");

var swaps = new HashSet<string>();

for (int index = 0; index < bits; index++)
{
    // turns out that the 4 swaps are quite local, so we do a small test around every bit position
    var (success, involved) = Test(gates, bits, index);

    if (success)
    {
        continue;
    }

    for (int i = 0; i < involved.Count; i++)
    {
        for (int j = i + 1; j < involved.Count; j++)
        {
            (involved[i].Output, involved[j].Output) = (involved[j].Output, involved[i].Output);

            try
            {
                if (Test(gates, bits, index - 1).Success && Test(gates, bits, index).Success && Test(gates, bits, index + 1).Success)
                {
                    swaps.Add(involved[i].Output);
                    swaps.Add(involved[j].Output);
                }
            }
            catch
            {
                // we created a cycle
            }

            (involved[i].Output, involved[j].Output) = (involved[j].Output, involved[i].Output);
        }
    }
}

var answer2 = string.Join(',', swaps.OrderBy(n => n));
Console.WriteLine($"Answer 2: {answer2}");

(bool Success, List<Gate> InvolvedGates) Test(List<Gate> gates, int bits, int index)
{
    var max = (1 << Math.Min(2, bits - index)) - 1;
    var bit = 1L << index;

    var success = true;
    var involved = new List<Gate>();

    for (int x = 0; x <= max; x++)
    {
        for (int y = 0; y <= max; y++)
        {
            var (actual, values) = Simulate(gates, bits, x * bit, y * bit);
            involved.AddRange(gates.Where(g => g.Inputs.Any(i => values[i])));
            var expected = (x + y) * bit;

            if (actual != expected)
            {
                success = false;
            }
        }
    }

    return (success, involved.Distinct().ToList());
}

(long Result, Dictionary<string, bool> Internals) Simulate(List<Gate> gates, int bits, long x, long y)
{
    var values = new Dictionary<string, bool>();
    SetValue(values, bits, 'x', x);
    SetValue(values, bits, 'y', y);

    var changed = true;

    while (changed)
    {
        changed = false;

        foreach (var gate in gates)
        {
            if (!values.ContainsKey(gate.Output) && gate.MayExecute(values))
            {
                gate.Execute(values);
                changed = true;
            }
        }
    }

    return (GetValue(values, bits + 1, 'z'), values);
}

void SetValue(Dictionary<string, bool> values, int bits, char symbol, long value)
{
    for (int index = 0; index < bits; index++)
    {
        var node = $"{symbol}{index:00}";
        values[node] = ((value >> index) & 1) == 1;
    }
}

long GetValue(Dictionary<string, bool> values, int bits, char symbol)
{
    var result = 0L;

    for (var index = 0; index < bits; index++)
    {
        var node = $"{symbol}{index:00}";
        result |= values[node] ? (1L << index) : 0;
    }

    return result;
}

(string Node, bool Value) ParseValue(string line)
{
    var parts = line.Split([':', ' '], StringSplitOptions.RemoveEmptyEntries);

    return (parts[0], parts[1] == "1");
}

abstract class Gate
{
    public required List<string> Inputs { get; set; }
    public required string Output { get; set; }

    public bool MayExecute(Dictionary<string, bool> values)
    {
        return Inputs.All(values.ContainsKey);
    }

    public abstract void Execute(Dictionary<string, bool> values);

    public static Gate Parse(string line)
    {
        var parts = line.Split(['-', '>', ' '], StringSplitOptions.RemoveEmptyEntries);

        var inputs = new List<string>
        {
            parts[0],
            parts[2]
        };

        var output = parts[3];

        return parts[1] switch
        {
            "AND" => new And { Inputs = inputs, Output = output },
            "OR" => new Or { Inputs = inputs, Output = output },
            "XOR" => new Xor { Inputs = inputs, Output = output },
            _ => throw new()
        };
    }
}

class And : Gate
{
    public override void Execute(Dictionary<string, bool> values)
    {
        values[Output] = values[Inputs[0]] && values[Inputs[1]];
    }
}

class Or : Gate
{
    public override void Execute(Dictionary<string, bool> values)
    {
        values[Output] = values[Inputs[0]] || values[Inputs[1]];
    }
}

class Xor : Gate
{
    public override void Execute(Dictionary<string, bool> values)
    {
        values[Output] = values[Inputs[0]] ^ values[Inputs[1]];
    }
}
