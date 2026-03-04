@echo off

REM Run LlmMockServer in separate window
start "" dotnet run --project ../LlmMockServer

REM Wait for LlmMockServer start
powershell -NoProfile -Command "for ($i=0; $i -lt 30; $i++) { try { $c = New-Object System.Net.Sockets.TcpClient('127.0.0.1',5000); $c.Close(); Write-Output 'OK'; exit 0 } catch { Start-Sleep -Seconds 1 } }; exit 1" > wait_result.txt
findstr /c:"OK" wait_result.txt >nul && echo LlmMockServer Ready || echo LlmMockServer Timeout
del wait_result.txt

REM Set environment variables
set REMOTE_LITELLM_URL=http://localhost:5000/v1/generate
set REMOTE_LITELLM_API_KEY=your_api_key_here

REM Run ProfilerAgent
dotnet run --project ../src/ProfilerAgent -- -i ../test_files/Part1.stp -n 3 -m baseline

REM Change DLL in project converter.exe for patched version, then run ProfilerAgent again
dotnet run --project ../src/ProfilerAgent -- -i ../test_files/Part1.stp -n 3 -m patched

pause
