using Anthropic;
using DerivativesDesk.AI.Services;
using DerivativesDesk.Core.Interfaces;
using DerivativesDesk.ETL;
using DerivativesDesk.ETL.Pipelines;
using DerivativesDesk.Infrastructure.Dapper;
using DerivativesDesk.Infrastructure.Dapper.Repositories;
using DerivativesDesk.Infrastructure.Redis;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;

DotNetEnv.Env.TraversePath().Load();

var builder = Host.CreateApplicationBuilder(args);

// ── Dapper ────────────────────────────────────────────────────────────────
builder.Services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IFuturesRepository, FuturesRepository>();
builder.Services.AddScoped<IRolloverRepository, RolloverRepository>();
builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();

// ── Redis ─────────────────────────────────────────────────────────────────
builder.Services.AddSingleton<IVectorStoreService, RedisVectorStoreService>();

// ── Semantic Kernel + OpenAI embeddings ───────────────────────────────────
var openAiKey      = builder.Configuration["OPENAI_API_KEY"]!;
var embeddingModel = builder.Configuration["OPENAI_EMBEDDING_MODEL"] ?? "text-embedding-3-small";

builder.Services.AddKernel()
    .AddOpenAITextEmbeddingGeneration(embeddingModel, openAiKey);

builder.Services.AddSingleton<ITextEmbeddingGenerationService>(sp =>
    sp.GetRequiredService<Kernel>().GetRequiredService<ITextEmbeddingGenerationService>());

// ── Anthropic (needed by ChatService but ETL doesn't use it directly) ─────
var anthropicKey = builder.Configuration["ANTHROPIC_API_KEY"]!;
builder.Services.AddSingleton(new AnthropicClient(new Anthropic.Core.ClientOptions { ApiKey = anthropicKey }));

// ── AI + ETL services ─────────────────────────────────────────────────────
builder.Services.AddSingleton<IEmbeddingService, EmbeddingService>();
builder.Services.AddScoped<IEtlPipeline, VectorRefreshPipeline>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
