# Derivatives Desk AI — Implementation Plan

## Context

A financial derivatives desk needs a conversational AI assistant for Portfolio Managers (PMs) and Institutional Portfolio Managers (IPMs). They work daily with futures contracts, rollover periods, and order data (placed/cancelled). The goal is a RAG-based chatbot that lets PMs ask natural-language questions like "What futures contracts are rolling over next week?" or "Show me all cancelled orders for ES contracts this month."

**Confirmed decisions:**
- Semantic Kernel (.NET 8) for AI orchestration
- **Claude Sonnet 4.6** (Anthropic) for chat/generation
- **OpenAI `text-embedding-3-small`** for vector embeddings
- **Dapper** for SQL Server data access (not EF Core)
- **SQL Server Database Project** (.sqlproj) for schema + test data
- **Redis Stack** for vector store (HNSW, dim=1536)
- No auth for MVP; SQL Server only for MVP
- Streaming responses via SSE

---

## Folder Structure

```
DerivativesDeskAI/
├── PLAN.md
├── docker-compose.yml
├── .env.example
├── database/
│   ├── DerivativesDesk.Database.sqlproj
│   ├── Tables/
│   │   ├── PortfolioManagers.sql
│   │   ├── FuturesContracts.sql
│   │   ├── RolloverPeriods.sql
│   │   └── Orders.sql
│   └── Scripts/TestData/
│       ├── 001_PortfolioManagers.sql
│       ├── 002_FuturesContracts.sql
│       ├── 003_RolloverPeriods.sql
│       └── 004_Orders.sql
├── backend/
│   ├── DerivativesDesk.sln
│   ├── DerivativesDesk.API/
│   ├── DerivativesDesk.Core/
│   ├── DerivativesDesk.Infrastructure/
│   ├── DerivativesDesk.AI/
│   └── DerivativesDesk.ETL/
└── frontend/  (React + TypeScript + Vite)
```

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | React 18, TypeScript, Vite |
| Backend API | ASP.NET Core 8, Swashbuckle (Swagger) |
| AI Orchestration | Microsoft Semantic Kernel |
| Chat Model | Claude Sonnet 4.6 (Anthropic) |
| Embeddings | OpenAI text-embedding-3-small (dim=1536) |
| Data Access | Dapper + Microsoft.Data.SqlClient |
| Vector Store | Redis Stack (RediSearch HNSW) |
| Relational DB | SQL Server |
| Streaming | Server-Sent Events (SSE) |

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/chat/stream` | SSE stream: `{sessionId, message}` |
| GET | `/api/chat/history/{sessionId}` | Message history |
| POST | `/api/etl/trigger` | Manual ETL refresh |
| GET | `/api/etl/status` | Last run stats |

Swagger UI: `http://localhost:5000/swagger`

---

## RAG Pipeline

```
User Query → OpenAI embed → Redis HNSW (top 7) → Prompt + Context → Claude Sonnet 4.6 → SSE stream
```

## ETL (Daily 6 AM)

```
SQL Server (Dapper) → text chunks → OpenAI embed → Redis upsert (dim=1536)
```

---

## Environment Variables

```env
ANTHROPIC_API_KEY=sk-ant-...
OPENAI_API_KEY=sk-...
OPENAI_EMBEDDING_MODEL=text-embedding-3-small
ConnectionStrings__DerivativesDesk=Server=.;Database=DerivativesDesk;Trusted_Connection=True;TrustServerCertificate=True;
REDIS_CONNECTION_STRING=localhost:6379
ETL_REFRESH_HOUR=6
CORS_ALLOWED_ORIGIN=http://localhost:5173
```
