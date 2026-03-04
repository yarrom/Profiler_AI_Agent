namespace ProfilerAgent.Models
{
    public class ProfileResult
    {
        public string ConfigName { get; set; }
        public long Milliseconds { get; set; }
        public long ProcessedBytes { get; set; }
    }
}
