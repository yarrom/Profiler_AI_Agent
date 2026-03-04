using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ProfilerAgent.Models;

namespace ProfilerAgent
{
    public class ProfileRunner
    {
        private readonly string _converterPath;

        public ProfileRunner(string converterPath)
        {
            _converterPath = converterPath;
        }

        public async Task<List<ProfileResult>> RunProfilesAsync(string inputFile, int iterations, List<(string name, bool optimize, int tess)> configs, string outputDir)
        {
            var results = new List<ProfileResult>();
            Directory.CreateDirectory(outputDir);
            foreach(var cfg in configs)
            {
                for (int i = 0; i < iterations; i++)
                {
                    string outFile = Path.Combine(outputDir, $"{Path.GetFileNameWithoutExtension(inputFile)}_{cfg.name}_{i}.out");
                    var sw = Stopwatch.StartNew();
                    var psi = new ProcessStartInfo
                    {
                        FileName = _converterPath,
                        Arguments = $"\"{inputFile}\" \"{outFile}\" {cfg.optimize} {cfg.tess}",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    var proc = Process.Start(psi);
                    if (proc == null) throw new Exception("Failed to start converter process");
                    string stdout = await proc.StandardOutput.ReadToEndAsync();
                    string stderr = await proc.StandardError.ReadToEndAsync();
                    await proc.WaitForExitAsync();
                    sw.Stop();

                    long processed = 0;
                    if (File.Exists(outFile)) processed = new FileInfo(outFile).Length;

                    results.Add(new ProfileResult
                    {
                        ConfigName = cfg.name,
                        Milliseconds = sw.ElapsedMilliseconds,
                        ProcessedBytes = processed
                    });
                }
            }
            return results;
        }
    }
}
