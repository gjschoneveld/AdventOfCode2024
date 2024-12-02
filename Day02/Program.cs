var input = File.ReadAllLines("input.txt");
var reports = input.Select(Parse).ToList();

var answer1 = reports.Count(IsSafe);
Console.WriteLine($"Answer 1: {answer1}");

var answer2 = reports.Count(IsSafeUsingProblemDampener);
Console.WriteLine($"Answer 2: {answer2}");

List<int> Parse(string line)
{
    return line.Split(' ').Select(int.Parse).ToList();
}

(Direction Direction, int Step) Compare(int a, int b)
{
    var direction = a > b ? Direction.Descending : Direction.Ascending;
    var step = Math.Abs(a - b);

    return (direction, step);
}

bool IsSafe(List<int> report)
{
    var compared = report.Zip(report.Skip(1), Compare).ToList();

    var directions = compared.GroupBy(x => x.Direction).Count();
    var min = compared.Min(x => x.Step);
    var max = compared.Max(x => x.Step);

    return directions == 1 && min >= 1 && max <= 3;
}

bool IsSafeUsingProblemDampener(List<int> report)
{
    if (IsSafe(report))
    {
        return true;
    }

    for (int i = 0; i < report.Count; i++)
    {
        var candidate = report.ToList();
        candidate.RemoveAt(i);

        if (IsSafe(candidate))
        {
            return true;
        }
    }

    return false;
}

enum Direction
{
    Ascending,
    Descending
}
