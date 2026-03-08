using Anthropic;
using DerivativesDesk.AI.Services;
using DerivativesDesk.Core.Interfaces;
using DerivativesDesk.Infrastructure.Dapper;
using DerivativesDesk.Infrastructure.Dapper.Repositories;
using DerivativesDesk.Infrastructure.Redis;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env if present (dev convenience)
DotNetEnv.Env.TraversePath().Load();

// ── Controllers + Swagger ───────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Derivatives Desk AI API", Version = "v1" });
    c.IncludeXmlComments(
        Path.Combine(AppContext.BaseDirectory, "DerivativesDesk.API.xml"),
        includeControllerXmlComments: true);
});

// ── CORS ────────────────────────────────────────────────────────────────────
var corsOrigin = builder.Configuration["CORS_ALLOWED_ORIGIN"] ?? "http://localhost:5173";
builder.Services.AddCors(o => o.AddPolicy("FrontendPolicy", p =>
    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// ── Dapper repositories ─────────────────────────────────────────────────────
builder.Services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IFuturesRepository, FuturesRepository>();
builder.Services.AddScoped<IRolloverRepository, RolloverRepository>();
builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();

// ── Redis vector store ──────────────────────────────────────────────────────
builder.Services.AddSingleton<IVectorStoreService, RedisVectorStoreService>();

// ── Semantic Kernel (OpenAI embeddings) ─────────────────────────────────────
var openAiKey    = builder.Configuration["OPENAI_API_KEY"]!;
var embeddingModel = builder.Configuration["OPENAI_EMBEDDING_MODEL"] ?? "text-embedding-3-small";

builder.Services.AddKernel()
    .AddOpenAITextEmbeddingGeneration(embeddingModel, openAiKey);

builder.Services.AddSingleton<ITextEmbeddingGenerationService>(sp =>
    sp.GetRequiredService<Kernel>().GetRequiredService<ITextEmbeddingGenerationService>());

// ── Anthropic SDK (Claude chat) ─────────────────────────────────────────────
var anthropicKey = builder.Configuration["ANTHROPIC_API_KEY"]!;
builder.Services.AddSingleton(new AnthropicClient(new Anthropic.Core.ClientOptions { ApiKey = anthropicKey }));

// ── AI services ─────────────────────────────────────────────────────────────
builder.Services.AddSingleton<IEmbeddingService, EmbeddingService>();
builder.Services.AddSingleton<IChatService, ChatService>();

var app = builder.Build();

// ── Middleware ───────────────────────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Derivatives Desk AI v1"));
app.UseCors("FrontendPolicy");
app.MapControllers();

// Ensure Redis index exists on startup
using (var scope = app.Services.CreateScope())
{
    var vectorStore = scope.ServiceProvider.GetRequiredService<IVectorStoreService>();
    await vectorStore.EnsureIndexAsync();
}

app.Run();
