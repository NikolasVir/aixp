# AIXP (Personal Project)

AIXP is a small personal AI assistant proof-of-concept with a .NET Core backend and Angular frontend.

## Project Structure

- `AIXP.API/` - ASP.NET Core API for document ingestion, semantic search, and generation using an LLM endpoint.
- `AIXP.UI/` - Angular SPA that talks to the API.

## Features

- Upload a document
- Semantic search across the document
- Ask questions that combine context chunks and generate answers

## Quick Start

### Prerequisites

- .NET 10 SDK
- Node.js (18+)
- Angular CLI (optional, can use npm scripts)
- Local LLM API (Ollama on `http://localhost:11434/api/chat` for current sample) or configure your own endpoint.

### Run API

1. Open terminal in `AIXP.API`
2. `dotnet run`
3. API runs by default on `https://localhost:5215` (or similar)

### Run UI

1. Open terminal in `AIXP.UI`
2. `npm install`
3. `npm start` (or `ng serve`)
4. Open `http://localhost:4200`

## Notes

- `AIXP.API/Services/GenerationService.cs` currently calls Ollama with model `llama3.2`.
- If no answer exists in context, the service prompt is designed to say so.

## Development

- Add models in `AIXP.API/Models`
- Keep API routes in `AIXP.API/Program.cs`
- Update UI logic in `AIXP.UI/src/app/`

## License

This is a personal side project.
