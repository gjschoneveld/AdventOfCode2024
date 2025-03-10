﻿var input = File.ReadAllLines("input.txt");
var processor = Processor.Parse(input);

processor.Run();

var answer1 = string.Join(',', processor.Output);
Console.WriteLine($"Answer 1: {answer1}");

var a = 0L;

foreach (var value in processor.Expected.Reverse<long>())
{
    a *= 8;

    while (true)
    {
        processor.Run(a);

        if (processor.Output[0] == value)
        {
            break;
        }

        a++;
    }
}

var answer2 = a;
Console.WriteLine($"Answer 2: {answer2}");

class Processor
{
    public int InstructionPointer { get; set; }
    public required Dictionary<char, long> Registers { get; set; }
    public required List<Instruction> Instructions { get; init; }

    public required List<long> Expected { get; init; }
    public List<long> Output { get; set; } = [];

    public void Run(long? a = null)
    {
        if (a != null)
        {
            Registers = new Dictionary<char, long>
            {
                ['A'] = (long)a,
                ['B'] = 0,
                ['C'] = 0
            };
        }

        InstructionPointer = 0;
        Output = [];

        while (0 <= InstructionPointer && InstructionPointer < Instructions.Count)
        {
            var (next, output) = Instructions[InstructionPointer].Execute(Registers);

            if (output != null)
            {
                Output.Add((long)output);
            }

            InstructionPointer = next ?? InstructionPointer + 1;
        }
    }

    public static Processor Parse(string[] input)
    {
        var interesting = input.Select(x => x.Split(' ')[^1]).ToList();

        var registers = new Dictionary<char, long>()
        {
            ['A'] = long.Parse(interesting[0]),
            ['B'] = long.Parse(interesting[1]),
            ['C'] = long.Parse(interesting[2])
        };

        var values = interesting[^1]
            .Split(',')
            .Select(long.Parse)
            .ToList();

        var instructions = values
            .Chunk(2)
            .Select(c => Instruction.Parse([.. c]))
            .ToList();

        return new()
        {
            Registers = registers,
            Instructions = instructions,
            Expected = values
        };
    }
}

abstract class Instruction
{
    public required long Operand { get; init; }

    public long Combo(Dictionary<char, long> registers)
    {
        return Operand switch
        {
            <= 3 => Operand,
            4 => registers['A'],
            5 => registers['B'],
            6 => registers['C'],
            _ => throw new()
        };
    }

    public abstract (int? Next, long? Output) Execute(Dictionary<char, long> registers);

    public static Instruction Parse(List<long> values)
    {
        return values[0] switch
        {
            0 => new Adv { Operand = values[1] },
            1 => new Bxl { Operand = values[1] },
            2 => new Bst { Operand = values[1] },
            3 => new Jnz { Operand = values[1] },
            4 => new Bxc { Operand = values[1] },
            5 => new Out { Operand = values[1] },
            6 => new Bdv { Operand = values[1] },
            7 => new Cdv { Operand = values[1] },
            _ => throw new()
        };
    }
}

class Adv : Instruction
{
    public override (int? Next, long? Output) Execute(Dictionary<char, long> registers)
    {
        var numerator = registers['A'];
        var denominator = 1L << (int)Combo(registers);

        registers['A'] = numerator / denominator;

        return (null, null);
    }
}

class Bxl : Instruction
{
    public override (int? Next, long? Output) Execute(Dictionary<char, long> registers)
    {
        registers['B'] ^= Operand;

        return (null, null);
    }
}

class Bst : Instruction
{
    public override (int? Next, long? Output) Execute(Dictionary<char, long> registers)
    {
        registers['B'] = Combo(registers) % 8;

        return (null, null);
    }
}

class Jnz : Instruction
{
    public override (int? Next, long? Output) Execute(Dictionary<char, long> registers)
    {
        if (registers['A'] != 0)
        {
            return ((int)Operand, null);
        }

        return (null, null);
    }
}

class Bxc : Instruction
{
    public override (int? Next, long? Output) Execute(Dictionary<char, long> registers)
    {
        registers['B'] ^= registers['C'];

        return (null, null);
    }
}

class Out : Instruction
{
    public override (int? Next, long? Output) Execute(Dictionary<char, long> registers)
    {
        return (null, Combo(registers) % 8);
    }
}

class Bdv : Instruction
{
    public override (int? Next, long? Output) Execute(Dictionary<char, long> registers)
    {
        var numerator = registers['A'];
        var denominator = 1L << (int)Combo(registers);

        registers['B'] = numerator / denominator;

        return (null, null);
    }
}

class Cdv : Instruction
{
    public override (int? Next, long? Output) Execute(Dictionary<char, long> registers)
    {
        var numerator = registers['A'];
        var denominator = 1L << (int)Combo(registers);

        registers['C'] = numerator / denominator;

        return (null, null);
    }
}
