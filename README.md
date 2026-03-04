Performance Profiler AI Agent




Run the Performance Profiler Agent locally:

Use tools/run_local_litellm.bat or perform steps provided below maually.

1. Run LlmMockServer:

dotnet run --project LlmMockServer

2. Set environment variables:

set REMOTE_LITELLM_URL=http://localhost:5000/v1/generate
set REMOTE_LITELLM_API_KEY=your_api_key_here

3. Run ProfilerAgent

dotnet run --project src/ProfilerAgent -- -i test_files\Part1.stp -n 3 -m baseline

Change DLL in project converter.exe for patched version, then:

dotnet run --project src/ProfilerAgent -- -i test_files\Part1.stp -n 3 -m patched





