using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace ProfilerAgent
{
    public class LlmClient : IDisposable
    {
        private readonly HttpClient _http;
        private readonly string _url;
        private readonly string _apiKey;
        private readonly int _maxTokens;
        private readonly int _retries;
        private readonly int _retryDelayMs;

        public LlmClient(IConfiguration cfg)
        {
            var llm = cfg.GetSection("Llm");
            _url = Environment.GetEnvironmentVariable("REMOTE_LITELLM_URL") ?? llm["Url"];
            _apiKey = Environment.GetEnvironmentVariable("REMOTE_LITELLM_API_KEY") ?? llm["ApiKey"];
            _maxTokens = int.TryParse(llm["MaxTokens"], out var mt) ? mt : 1024;
            _retries = int.TryParse(llm["Retries"], out var r) ? r : 2;
            _retryDelayMs = int.TryParse(llm["RetryDelayMs"], out var rd) ? rd : 1000;

            var timeoutSec = int.TryParse(llm["TimeoutSeconds"], out var ts) ? ts : 30;
            _http = new HttpClient { Timeout = TimeSpan.FromSeconds(timeoutSec) };
        }

        public async Task<string> AnalyzeAsync(string prompt, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(_url)) throw new InvalidOperationException("LLM URL is not configured");
            var payload = new { prompt = prompt, max_tokens = _maxTokens };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            if (!string.IsNullOrEmpty(_apiKey))
                content.Headers.Add("X-API-KEY", _apiKey); // Key or Authorization header depending of API

            int attempt = 0;
            while (true)
            {
                attempt++;
                try
                {
                    var req = new HttpRequestMessage(HttpMethod.Post, _url) { Content = content };
                    if (!string.IsNullOrEmpty(_apiKey))
                        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

                    var resp = await _http.SendAsync(req, ct);
                    resp.EnsureSuccessStatusCode();
                    var body = await resp.Content.ReadAsStringAsync(ct);

                    // Flexible JSON responses handling
                    dynamic j = JsonConvert.DeserializeObject(body);
                    if (j == null) return body;
                    if (j.text != null) return (string)j.text;
                    if (j.choices != null && j.choices.HasValues) return (string)j.choices[0].text;
                    if (j.data != null && j.data.answer != null) return (string)j.data.answer;
                    return body;
                }
                catch (Exception ex) when (attempt <= _retries)
                {
                    await Task.Delay(_retryDelayMs * attempt, ct);
                }
            }
        }

        public void Dispose()
        {
            _http?.Dispose();
        }
    }
}
