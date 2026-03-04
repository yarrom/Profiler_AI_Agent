namespace ProfilerAgent.Models
{
    public class PatchTestResult
    {
        public bool RegressionDetected { get; set; }
        public string Summary { get; set; }
        public string LlmAnalysis { get; set; }
    }
}
