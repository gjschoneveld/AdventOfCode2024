using System.Text.RegularExpressions;
using State = (bool Enabled, int Value);

var memory = File.ReadAllText("input.txt");
var multiplications = Mul.Parse(memory);

var answer1 = Run(multiplications);
Console.WriteLine($"Answer 1: {answer1}");

var dos = Do.Parse(memory);
var donts = Dont.Parse(memory);
var all = multiplications.Concat(dos).Concat(donts).ToList();

var answer2 = Run(all);
Console.WriteLine($"Answer 2: {answer2}");

static int Run(List<Command> commands)
{
    var sorted = commands.OrderBy(c => c.Index).ToList();
    State state = (true, 0);

    foreach (var command in sorted)
    {
        state = command.Apply(state);
    }

    return state.Value;
}

abstract class Command
{
    public int Index { get; set; }

    public abstract State Apply(State state);
}

class Mul : Command
{
    public int A { get; set; }
    public int B { get; set; }

    public override (bool Enabled, int Value) Apply((bool Enabled, int Value) state)
    {
        if (!state.Enabled)
        {
            return state;
        }

        return (state.Enabled, state.Value + (A * B));
    }

    public static List<Command> Parse(string memory)
    {
        var matches = Regex.Matches(memory, @"mul\((?<a>\d+),(?<b>\d+)\)");

        return matches.Select(m => new Mul
        {
            Index = m.Index,
            A = int.Parse(m.Groups["a"].Value),
            B = int.Parse(m.Groups["b"].Value)
        }).Cast<Command>().ToList();
    }
}

class Do : Command
{
    public override (bool Enabled, int Value) Apply((bool Enabled, int Value) state)
    {
        return (true, state.Value);
    }

    public static List<Command> Parse(string memory)
    {
        var matches = Regex.Matches(memory, @"do\(\)");

        return matches.Select(m => new Do
        {
            Index = m.Index
        }).Cast<Command>().ToList();
    }
}

class Dont : Command
{
    public override (bool Enabled, int Value) Apply((bool Enabled, int Value) state)
    {
        return (false, state.Value);
    }

    public static List<Command> Parse(string memory)
    {
        var matches = Regex.Matches(memory, @"don't\(\)");

        return matches.Select(m => new Dont
        {
            Index = m.Index
        }).Cast<Command>().ToList();
    }
}
