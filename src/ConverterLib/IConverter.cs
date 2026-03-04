using System.Threading.Tasks;

namespace ConverterLib
{
    public interface IConverter
    {
        Task<ConversionResult> ConvertAsync(string inputPath, string outputPath, ConverterOptions options);
    }

    public record ConverterOptions(bool OptimizeMeshes = false, int TessellationQuality = 1);
    public record ConversionResult(bool Success, string Message, long ProcessedBytes = 0);
}
