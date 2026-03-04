using System.Collections.Generic;
using System.Threading.Tasks;
using ProfilerAgent.Models;

namespace ProfilerAgent
{
    public class PatchTester
    {
        private readonly LlmClient _llm;

        public PatchTester(LlmClient llm)
        {
            _llm = llm;
        }

        public async Task<PatchTestResult> AnalyzeAsync(IEnumerable<ProfileResult> before, IEnumerable<ProfileResult> after)
        {
            // Simple aggregation: average time
            double avgBefore = 0, avgAfter = 0;
            int nBefore = 0, nAfter = 0;
            foreach (var b in before) { avgBefore += b.Milliseconds; nBefore++; }
            foreach (var a in after) { avgAfter += a.Milliseconds; nAfter++; }
            avgBefore = nBefore > 0 ? avgBefore / nBefore : 0;
            avgAfter = nAfter > 0 ? avgAfter / nAfter : 0;

            bool regression = avgAfter > avgBefore * 1.05; // threshold 5%
            string summary = $"avg_before={avgBefore:F1}ms avg_after={avgAfter:F1}ms regression={regression}";

            string prompt = $"Summary: {summary}\n" +
                "Give detailed analysis of possible reasons of regression and propositions for optimization. " +
                "Point, which modules of library may be responsible ant what tests to add.";

            string analysis = await _llm.AnalyzeAsync(prompt);

            return new PatchTestResult
            {
                RegressionDetected = regression,
                Summary = summary,
                LlmAnalysis = analysis
            };
        }
    }
}
