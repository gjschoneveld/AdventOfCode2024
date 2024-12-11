using Group = (long Number, long Count);

var input = File.ReadAllText("input.txt");
var numbers = input.Split(' ').Select(long.Parse).ToList();

for (int i = 0; i < 25; i++)
{
    numbers = numbers.SelectMany(Blink).ToList();
}

var answer1 = numbers.Count;
Console.WriteLine($"Answer 1: {answer1}");

var groups = numbers.Select(n => (Number: n, Count: 1L)).ToList();
groups = Merge(groups);

for (int i = 0; i < 50; i++)
{
    groups = groups.SelectMany(g => Blink(g.Number).Select(n => (Number: n, g.Count))).ToList();
    groups = Merge(groups);
}

var answer2 = groups.Sum(g => g.Count);
Console.WriteLine($"Answer 2: {answer2}");

List<long> Blink(long number)
{
    if (number == 0)
    {
        return [1];
    }

    var digits = ToDigits(number);

    if (digits.Count % 2 == 0)
    {
        var half = digits.Count / 2;

        var left = digits[..half];
        var right = digits[half..];

        return [FromDigits(left), FromDigits(right)];
    }

    return [number * 2024];
}

List<long> ToDigits(long value)
{
    return value.ToString().Select(d => (long)(d - '0')).ToList();
}

long FromDigits(List<long> digits)
{
    long value = 0;

    foreach (var digit in digits)
    {
        value = 10 * value + digit;
    }

    return value;
}

List<Group> Merge(List<Group> groups)
{
    var totals = new Dictionary<long, long>();

    foreach (var (number, count) in groups)
    {
        totals.TryAdd(number, 0);
        totals[number] += count;
    }

    return totals.Select(kv => (Number: kv.Key, Count: kv.Value)).ToList();
}
