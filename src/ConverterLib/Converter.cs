using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConverterLib
{
    public class Converter : IConverter
    {
        public async Task<ConversionResult> ConvertAsync(string inputPath, string outputPath, ConverterOptions options)
        {
            // Workflow emulation: reading of file, conversion, saving
            if (!File.Exists(inputPath))
                return new ConversionResult(false, "Input not found");

            var sw = Stopwatch.StartNew();
            // Simulate delay depending of settings
            int delay = 100 + options.TessellationQuality * 50 + (options.OptimizeMeshes ? -20 : 0);
            await Task.Delay(delay);
            // Handling of content
            var data = await File.ReadAllBytesAsync(inputPath);
            long processed = data.Length;
            // Output storage
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ".");
            await File.WriteAllBytesAsync(outputPath, data);
            sw.Stop();
            return new ConversionResult(true, $"Converted in {sw.ElapsedMilliseconds}ms", processed);
        }
    }
}
