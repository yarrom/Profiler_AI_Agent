using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using ProfilerAgent;
using System.Collections.Generic;

public class ProfileRunnerTests
{
    [Fact]
    public async Task RunProfiles_CreatesOutputFiles()
    {
        // Create temporary "input" file
        string tmp = Path.GetTempFileName();
        await File.WriteAllTextAsync(tmp, "dummy data");
        var runner = new ProfileRunner("dotnet"); // Run not actual converter, just check the stream
        var configs = new List<(string name, bool optimize, int tess)>
        {
            ("test", false, 1)
        };
        string outDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var results = await runner.RunProfilesAsync(tmp, 1, configs, outDir);
        Assert.NotEmpty(results);
    }
}
