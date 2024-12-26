var input = File.ReadAllLines("input.txt");
var seeds = input.Select(long.Parse).ToList();

var answer1 = seeds.Sum(FindSecretNumber2000);
Console.WriteLine($"Answer 1: {answer1}");

var sequencesList = seeds.Select(FindSequences).ToList();

var combined = new Dictionary<(int A, int B, int C, int D), int>();

foreach (var sequences in sequencesList)
{
    foreach (var (sequence, bananas) in sequences)
    {
        combined.TryAdd(sequence, 0);
        combined[sequence] += bananas;
    }
}

var answer2 = combined.Values.Max();
Console.WriteLine($"Answer 2: {answer2}");

Dictionary<(int A, int B, int C, int D), int> FindSequences(long number)
{
    var result = new Dictionary<(int A, int B, int C, int D), int>();

    var empty = 10;
    var sequence = (A: empty, B: empty, C: empty, D: empty);

    for (int i = 0; i < 2000; i++)
    {
        var previousBananas = Bananas(number);

        number = Next(number);
        var bananas = Bananas(number);

        var change = bananas - previousBananas;
        sequence = (sequence.B, sequence.C, sequence.D, change);

        if (sequence.A == empty)
        {
            continue;
        }

        if (!result.ContainsKey(sequence))
        {
            result[sequence] = bananas;
        }
    }

    return result;
}

int Bananas(long number)
{
    return (int)(number % 10);
}

long FindSecretNumber2000(long number)
{
    for (int i = 0; i < 2000; i++)
    {
        number = Next(number);
    }

    return number;
}

long Next (long number)
{
    number = Prune(Mix(number, number * 64));
    number = Prune(Mix(number, number / 32));
    number = Prune(Mix(number, number * 2048));

    return number;
}

long Mix(long a, long b)
{
    return a ^ b;
}

long Prune(long x)
{
    return x % 16777216;
}
