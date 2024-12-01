using Line = (int Left, int Right);

var input = File.ReadAllLines("input.txt");
var lines = input.Select(Parse).ToList();

var left = lines.Select(l => l.Left).Order().ToList();
var right = lines.Select(l => l.Right).Order().ToList();

var answer1 = left.Zip(right, (l, r) => Math.Abs(l - r)).Sum();
Console.WriteLine($"Answer 1: {answer1}");

var occurrences = right.GroupBy(v => v).ToDictionary(g => g.Key, g => g.Count());

var answer2 = left.Sum(v => occurrences.ContainsKey(v) ? v * occurrences[v] : 0);
Console.WriteLine($"Answer 2: {answer2}");

Line Parse(string line)
{
    var values = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

    return (values[0], values[1]);
}
