# Distributed-URL-Shortener-with-Rate-Limiter
Built with .NET 9, Redis, and PostgreSQL. Handles 10K+ redirects/sec 
with a token-bucket rate limiter capping abuse at 100 req/min per client.

## Features
- POST /api/url/shorten — creates a short code
- GET /{code} — 302 redirect (Redis cache-aside, <2ms overhead)
- GET /{code}/stats — click analytics
- Token-bucket rate limiter via Redis
- Load tested with k6 / Gatling

## Run locally
docker-compose up   # spins up Redis + PostgreSQL
dotnet run --project UrlShortener.API