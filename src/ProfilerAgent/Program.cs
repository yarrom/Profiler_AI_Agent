using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ProfilerAgent.Models;

namespace ProfilerAgent
{
    public class Options
    {
        [Option('i', "input", Required = true)]
        public string Input { get; set; }

        [Option('c', "converter", Required = false)]
        public string ConverterPath { get; set; } = "ConverterApp.exe";

        [Option('n', "iterations", Required = false)]
        public int Iterations { get; set; } = 3;

        [Option('m', "mode", Required = false)]
        public string Mode { get; set; } = "baseline"; // baseline or patched

        [Option('o', "outdir", Required = false)]
        public string OutDir { get; set; } = "profiles";
    }

    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var parser = Parser.Default.ParseArguments<Options>(args);
            var exit = await parser.MapResult(async opts =>
            {
                if (!File.Exists(opts.Input))
                {
                    Console.WriteLine("Input file not found");
                    return 1;
                }


                var runner = new ProfileRunner(opts.ConverterPath);
                var configs = new List<(string name, bool optimize, int tess)>
                {
                    ("default", false, 1),
                    ("opt_high", true, 2)
                };

                Console.WriteLine($"Running profiles ({opts.Mode})...");
                var results = await runner.RunProfilesAsync(opts.Input, opts.Iterations, configs, opts.OutDir);

                string json = JsonConvert.SerializeObject(results, Formatting.Indented);
                string outFile = Path.Combine(opts.OutDir, $"{opts.Mode}_results.json");
                File.WriteAllText(outFile, json);
                Console.WriteLine($"Saved {outFile}");

                // In patched mode compare with baseline
                if (opts.Mode == "patched")
                {
                    string baseFile = Path.Combine(opts.OutDir, "baseline_results.json");
                    if (!File.Exists(baseFile))
                    {
                        Console.WriteLine("Baseline results not found; run baseline first.");
                        return 1;
                    }

                    var before = JsonConvert.DeserializeObject<List<ProfileResult>>(File.ReadAllText(baseFile));
                    var after = results;
                    // LLM client URL configurable via env REMOTE_LITELLM_URL or default localhost
                    string llmUrl = Environment.GetEnvironmentVariable("REMOTE_LITELLM_URL") ?? "http://127.0.0.1:5000/";

                    var config = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: true)
                        .AddEnvironmentVariables()
                        .Build();

                    var llm = new LlmClient(config);

                    var tester = new PatchTester(llm);
                    var patchResult = await tester.AnalyzeAsync(before, after);
                    string reportFile = Path.Combine(opts.OutDir, "patch_report.json");
                    File.WriteAllText(reportFile, JsonConvert.SerializeObject(patchResult, Formatting.Indented));
                    Console.WriteLine($"Patch report saved to {reportFile}");
                }

                return 0;
            }, errs => Task.FromResult(1));

            return exit;
        }
    }
}
