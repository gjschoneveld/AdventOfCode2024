var input = File.ReadAllLines("input.txt");
var seeds = input.Select(long.Parse).ToList();

var answer1 = seeds.Sum(FindSecretNumber2000);
Console.WriteLine($"Answer 1: {answer1}");

var sequencesList = seeds.Select(FindSequences).ToList();

var combined = new Dictionary<(int a, int b, int c, int d), int>();

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

Dictionary<(int a, int b, int c, int d), int> FindSequences(long number)
{
    var result = new Dictionary<(int a, int b, int c, int d), int>();
    var changes = new List<int>();

    for (int i = 0; i < 2000; i++)
    {
        var previousBananas = Bananas(number);

        number = Next(number);
        var bananas = Bananas(number);

        var change = bananas - previousBananas;
        changes.Add(change);

        if (changes.Count != 4)
        {
            continue;
        }

        var sequence = (changes[0], changes[1], changes[2], changes[3]);

        if (!result.ContainsKey(sequence))
        {
            result[sequence] = bananas;
        }

        changes.RemoveAt(0);
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
