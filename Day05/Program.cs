using Rule = (int Left, int Right);

var input = File.ReadAllLines("input.txt");
var ruleInput = input.TakeWhile(line => line != "").ToList();
var updateInput = input.SkipWhile(line => line != "").Skip(1).ToList();

var rules = ruleInput.Select(ParseRule).ToList();
var updates = updateInput.Select(line => Update.Parse(line, rules)).ToList();

var answer1 = updates.Where(u => u.IsSorted).Sum(u => u.MiddlePage);
Console.WriteLine($"Answer 1: {answer1}");

var answer2 = updates.Where(u => !u.IsSorted).Select(u => u.Sort()).Sum(u => u.MiddlePage);
Console.WriteLine($"Answer 2: {answer2}");

Rule ParseRule(string line)
{
    var values = line.Split('|').Select(int.Parse).ToList();

    return (values[0], values[1]);
}

class Update
{
    public required List<int> Pages { get; set; }
    public required List<Rule> Rules { get; set; }

    public bool IsSorted => Rules.All(r => Pages.IndexOf(r.Left) < Pages.IndexOf(r.Right));
    public int MiddlePage => Pages[Pages.Count / 2];

    public Update Sort()
    {
        var pages = Pages.ToList();
        var relevantRules = Rules.ToList();

        var sorted = new List<int>();

        while (pages.Count > 0)
        {
            var first = pages.First(p => !relevantRules.Any(r => r.Right == p));

            sorted.Add(first);
            pages.Remove(first);
            relevantRules = relevantRules.Where(r => r.Left != first).ToList();
        }

        return new()
        {
            Pages = sorted,
            Rules = Rules
        };
    }

    public static Update Parse(string line, List<Rule> rules)
    {
        var pages = line.Split(',').Select(int.Parse).ToList();
        var relevantRules = rules.Where(r => pages.Contains(r.Left) && pages.Contains(r.Right)).ToList();

        return new()
        {
            Pages = pages,
            Rules = relevantRules
        };
    }
}
