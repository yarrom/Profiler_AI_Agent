using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LlmMockServer.LlmEngine;

var builder = WebApplication.CreateBuilder(args);

// Registration of an engine
builder.Services.AddSingleton<ILiteLlmEngine, MockLiteLlmEngine>();
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run("http://localhost:5000");
