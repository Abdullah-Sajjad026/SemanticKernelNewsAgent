#pragma warning disable SKEXP0070
#pragma warning disable SKEXP0001

using Microsoft.SemanticKernel;
using SemanticKernalNewsAPI.Plugins;
using SemanticKernalNewsAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging
    .ClearProviders()
    .AddConsole()
    .SetMinimumLevel(LogLevel.Information);
// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", // React default
                "http://127.0.0.1:5500", // Common Live Server port
                "https://your-production-domain.com")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });

    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("*")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.AddSingleton<SessionHistoryStore>();

// Configure Semantic Kernel
builder.Services.AddSingleton<Kernel>(serviceProvider =>
{
    var kernelBuilder = Kernel.CreateBuilder();

    kernelBuilder.AddGoogleAIGeminiChatCompletion(
        builder.Configuration["LLM:Model"],
        builder.Configuration["LLM:ApiKey"]
    );

    var kernel = kernelBuilder.Build();

    // Register the NewsPlugin
    kernel.ImportPluginFromType<NewsPlugin>();

    return kernel;
});


var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();