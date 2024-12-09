var input = File.ReadAllText("input.txt");

var disk = CreateDisk(input);
CompactDisk1(disk);
var answer1 = Checksum(disk);
Console.WriteLine($"Answer 1: {answer1}");

disk = CreateDisk(input);
CompactDisk2(disk);
var answer2 = Checksum(disk);
Console.WriteLine($"Answer 2: {answer2}");

List<int?> CreateDisk(string map)
{
    var result = new List<int?>();

    var file = true;
    var id = 0;

    foreach (var symbol in map)
    {
        var length = symbol - '0';

        for (int i = 0; i < length; i++)
        {
            result.Add(file ? id : null);
        }

        if (file)
        {
            id++;
        }

        file = !file;
    }

    return result;
}

void CompactDisk1(List<int?> disk)
{
    int writer = disk.FindIndex(x => x == null);
    int reader = disk.Count - 1;

    while (writer < reader)
    {
        disk[writer] = disk[reader];
        disk[reader] = null;

        while (writer < disk.Count && disk[writer] != null)
        {
            writer++;
        }

        while (reader >= 0 && disk[reader] == null)
        {
            reader--;
        }
    }
}

void CompactDisk2(List<int?> disk)
{
    var free = new Dictionary<int, List<int>>
    {
        [1] = [],
        [2] = [],
        [3] = [],
        [4] = [],
        [5] = [],
        [6] = [],
        [7] = [],
        [8] = [],
        [9] = []
    };

    var files = new List<(int Id, int Index, int Size)>();

    int i = 0;

    while (i < disk.Count)
    {
        var id = disk[i];
        var index = i;
        var size = 0;

        while (i < disk.Count && disk[i] == id)
        {
            size++;
            i++;
        }

        if (id == null)
        {
            free[size].Add(index);

            continue;
        }

        files.Add(((int)id, index, size));
    }

    files.Reverse();

    foreach (var file in files)
    {
        var freeIndex = int.MaxValue;
        var freeSize = 0;

        var size = file.Size;

        while (free.ContainsKey(size))
        {
            if (free[size].Count > 0 && free[size][0] < freeIndex)
            {
                freeIndex = free[size][0];
                freeSize = size;
            }

            size++;
        }

        if (freeIndex > file.Index)
        {
            continue;
        }

        free[freeSize].RemoveAt(0);

        for (int j = 0; j < file.Size; j++)
        {
            disk[freeIndex + j] = file.Id;
            disk[file.Index + j] = null;
        }

        var remainderIndex = freeIndex + file.Size;
        var remainderSize = freeSize - file.Size;

        if (remainderSize == 0)
        {
            continue;
        }

        free[remainderSize].Add(remainderIndex);
        free[remainderSize].Sort();
    }
}

long Checksum(List<int?> disk)
{
    return disk.Select((v, i) => (long)(v ?? 0) * i).Sum();
}
