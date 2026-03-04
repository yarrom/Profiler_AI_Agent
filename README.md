# Performance Profiler AI Agent
## Short description
A .NET C# AI agent for profiling and analyzing application behaviour with a pluggable LLM backend. Includes a local LlmMockServer for faster development. In production the agent will be wired to a real LLM provider.
## Key features
- Modular agent core that communicates with an LLM backend
- Local LlmMockServer for fast development and repeatable tests
- Configurable pipelines for profiling tasks and analysis
- Example projects and scripts to run profiling targets
## Requirements
- .NET 7.0 or later
- Docker optional for containerized runs
- Access to an LLM service or use the included LlmMockServer for local testing
## Current status
Currently a MVP of Agent is presented in the repository and Agent is in active development phase.
## Quick setup
### 1. Clone repository
```
git clone 
cd Profiler_AI_Agent
```
## Performance Profiler Agent local testing
Run `tools/run_local_litellm.bat` or perform steps provided below maually:
### 1. Run LlmMockServer
```
dotnet run --project LlmMockServer
```
### 2. Set environment variables
```
set REMOTE_LITELLM_URL=http://localhost:5000/v1/generate
set REMOTE_LITELLM_API_KEY=your_api_key_here
```

### 3. Run ProfilerAgent
```
dotnet run --project src/ProfilerAgent -- -i test_files\Part1.stp -n 3 -m baseline
```
Change DLL in project converter.exe for patched version, then:
```
dotnet run --project src/ProfilerAgent -- -i test_files\Part1.stp -n 3 -m patched
```
