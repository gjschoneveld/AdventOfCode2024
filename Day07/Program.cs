using Equation = (long Result, System.Collections.Generic.List<long> Operands);
 
var input = File.ReadAllLines("input.txt");
var equations = input.Select(Parse).ToList();

var answer1 = equations.Where(e => CanBeMadeTrue(e.Result, e.Operands)).Sum(e => e.Result);
Console.WriteLine($"Answer 1: {answer1}");

var answer2 = equations.Where(e => CanBeMadeTrue(e.Result, e.Operands, true)).Sum(e => e.Result);
Console.WriteLine($"Answer 2: {answer2}");

bool CanBeMadeTrue(long result, List<long> operands, bool withConcatenation = false)
{
    if (operands.Count == 1)
    {
        return operands[0] == result;
    }

    var last = operands[^1];
    var remaining = operands[0..^1];
    var divisor = Divisor(last);

    var addition = result - last >= 0 && CanBeMadeTrue(result - last, remaining, withConcatenation);
    var multiplication = result % last == 0 && CanBeMadeTrue(result / last, remaining, withConcatenation);
    var concatenation = withConcatenation && result % divisor == last && CanBeMadeTrue(result / divisor, remaining, withConcatenation);

    return addition || multiplication || concatenation;
}

long Divisor(long value)
{
    var result = 1;

    while (value > 0)
    {
        result *= 10;
        value /= 10;
    }

    return result;
}

Equation Parse(string line)
{
    var values = line.Split([':', ' '], StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();

    return (values[0], values[1..]);
}
