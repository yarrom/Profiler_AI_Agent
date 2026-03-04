using System;
using System.Threading.Tasks;
using ConverterLib;

class Program
{
    static async Task<int> Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: converter.exe <input> <output> [optimize=true|false] [tess=1]");
            return 1;
        }

        Console.WriteLine("Converter args:");
        foreach (var arg in args)
        {
            Console.WriteLine($"arg: {arg}");
        }

        string input = args[0];
        string output = args[1];
        bool optimize = args.Length > 2 && bool.TryParse(args[2], out var o) && o;
        int tess = 1;
        if (args.Length > 3) int.TryParse(args[3], out tess);

        var converter = new Converter();
        var result = await converter.ConvertAsync(input, output, new ConverterOptions(optimize, tess));
        Console.WriteLine(result.Message);
        return result.Success ? 0 : 2;
    }
}
