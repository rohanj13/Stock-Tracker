# portfolio-xray

portfolio-xray is a fintech portfolio-analysis tool for ETF and stock look-through analysis. It reveals a user’s true consolidated sector and geographic exposure across a full portfolio by resolving each ETF into its underlying holdings rather than treating it as a single opaque position.

## MVP Architecture

- **Backend compute**: AWS Lambda
  - Request-time API Lambda for user portfolio exposure calculations
  - Scheduled Lambda for periodic reference-data refresh
- **Database**: Supabase (Postgres)
  - Relational reference data for stocks, ETFs, and ETF constituent mappings
  - Core tables:
    - `stocks`
    - `etfs`
    - `etf_holdings` (ETF-to-constituent-stock weights)
- **Frontend**: Vercel-hosted web app

## Exposure Calculation Model

For any user-submitted mix of stock and ETF tickers:

1. Resolve direct stock positions.
2. Expand ETF positions through `etf_holdings`.
3. Aggregate effective constituent weights.
4. Compute weighted-average sector and geographic exposure.
5. Return consolidated portfolio breakdown.

## Data Refresh

A scheduled AWS Lambda job periodically refreshes:

- ETF constituent holdings
- Sector classification data
- Geographic exposure attributes

## Repository Scope (Early MVP)

- Lightweight setup only (no CI/CD workflows yet)
- Initial project documentation and repository hygiene files

## Branch Strategy

- Default branch: `main`
- Branch protection: required on `main` in repository settings
- Development flow: create short-lived `feature/*` branches from `main`, then merge back via pull requests

## Suggested Repository Topics

- `fintech`
- `etf`
- `portfolio-analysis`
- `aws-lambda`
- `supabase`
- `vercel`

## License

MIT (see [LICENSE](LICENSE)).
