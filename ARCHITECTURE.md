# Architecture Overview

## Solution Architecture

```
┌─────────────────────────────────────────────────────────────────────┐
│                        React Frontend                               │
│              (React + TypeScript + Vite + TailwindCSS)              │
│                     (Runs on Port 5173)                             │
└─────────────────┬───────────────────────────────────────────────────┘
                  │ HTTPS / CORS
                  │
┌─────────────────▼───────────────────────────────────────────────────┐
│                      AWS API Gateway                                │
│                   (HTTP API Integration)                            │
└─────────────────┬───────────────────────────────────────────────────┘
                  │
                  │
┌─────────────────▼───────────────────────────────────────────────────┐
│                    AWS Lambda Runtime                               │
│           (Hosts ASP.NET Core Application)                          │
│                                                                      │
│  ┌────────────────────────────────────────────────────────────────┐ │
│  │                   ASP.NET Core API                            │ │
│  │              (Minimal APIs Architecture)                      │ │
│  │                                                                │ │
│  │  ┌──────────────────────────────────────────────────────────┐ │ │
│  │  │                    API Layer                            │ │ │
│  │  │         - Minimal API endpoints                         │ │ │
│  │  │         - Request validation                            │ │ │
│  │  │         - Response transformation                       │ │ │
│  │  │         - Health checks                                 │ │ │
│  │  └────────────────┬─────────────────────────────────────────┘ │ │
│  │                   │                                            │ │
│  │  ┌────────────────▼─────────────────────────────────────────┐ │ │
│  │  │                Application Layer                        │ │ │
│  │  │         - Business logic services                       │ │ │
│  │  │         - Application workflows                         │ │ │
│  │  │         - DTO mapping and transformation                │ │ │
│  │  │         - Service interfaces                            │ │ │
│  │  └────────────────┬─────────────────────────────────────────┘ │ │
│  │                   │                                            │ │
│  │  ┌────────────────▼─────────────────────────────────────────┐ │ │
│  │  │                  Domain Layer                           │ │ │
│  │  │         - Domain entities                              │ │ │
│  │  │         - Business rules                               │ │ │
│  │  │         - Value objects                                │ │ │
│  │  │         - Enumerations                                 │ │ │
│  │  │         (INDEPENDENT - no framework dependencies)      │ │ │
│  │  └────────────────┬─────────────────────────────────────────┘ │ │
│  │                   │                                            │ │
│  │  ┌────────────────▼─────────────────────────────────────────┐ │ │
│  │  │            Infrastructure Layer                         │ │ │
│  │  │         - Entity Framework Core DbContext               │ │ │
│  │  │         - Data access repositories                      │ │ │
│  │  │         - External service integrations                 │ │ │
│  │  │         - Database configurations                       │ │ │
│  │  └────────────────┬─────────────────────────────────────────┘ │ │
│  │                   │                                            │ │
│  └───────────────────┼────────────────────────────────────────────┘ │
│                      │                                              │
└──────────────────────┼──────────────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────────────┐
│                    PostgreSQL Database                             │
│         (Supabase or Amazon RDS for Production)                    │
└─────────────────────────────────────────────────────────────────────┘
```

## Project Structure

```
Stock-Tracker/
├── backend/                          # .NET 8 backend solution
│   ├── Portfolio.sln                 # Solution file
│   ├── src/
│   │   ├── Portfolio.Api/            # ASP.NET Core API layer
│   │   │   ├── Program.cs            # Application entry point
│   │   │   ├── Endpoints/            # Minimal API endpoints
│   │   │   ├── Middleware/           # Custom middleware
│   │   │   ├── Extensions/           # DI and configuration extensions
│   │   │   ├── appsettings.json      # Configuration
│   │   │   └── appsettings.Development.json
│   │   │
│   │   ├── Portfolio.Application/    # Business logic layer
│   │   │   ├── Services/             # Application services
│   │   │   ├── DTOs/                 # Data transfer objects
│   │   │   ├── Interfaces/           # Service interfaces
│   │   │   └── Extensions/           # DI setup
│   │   │
│   │   ├── Portfolio.Domain/         # Domain model (independent)
│   │   │   ├── Entities/             # Domain entities
│   │   │   ├── Enums/                # Enumerations
│   │   │   └── ValueObjects/         # Value objects
│   │   │
│   │   └── Portfolio.Infrastructure/ # Data access layer
│   │       ├── Persistence/
│   │       │   ├── ApplicationDbContext.cs
│   │       │   └── Configurations/   # EF Core model configurations
│   │       ├── Repositories/         # Data access repositories
│   │       ├── Services/             # External services
│   │       └── Extensions/           # DI setup
│   │
│   ├── tests/
│   │   ├── Portfolio.Domain.Tests/   # Domain unit tests
│   │   └── Portfolio.Application.Tests/ # Application tests
│   │
│   └── .gitignore                    # Git ignore rules
│
├── frontend/                          # React + TypeScript frontend
│   ├── src/
│   │   ├── api/                      # API client utilities
│   │   ├── components/               # Reusable React components
│   │   ├── features/                 # Feature-specific modules
│   │   ├── pages/                    # Page components
│   │   ├── hooks/                    # Custom React hooks
│   │   ├── types/                    # TypeScript types
│   │   ├── App.tsx                   # Root component
│   │   └── main.tsx                  # Entry point
│   ├── index.html                    # HTML template
│   ├── package.json                  # Dependencies
│   ├── vite.config.ts                # Vite configuration
│   ├── tsconfig.json                 # TypeScript configuration
│   ├── .env.example                  # Environment template
│   └── .gitignore                    # Git ignore rules
│
├── infrastructure/                    # Infrastructure as Code
│   └── README.md                     # IaC documentation
│
├── README.md                         # Project documentation
└── .gitignore                        # Root git ignore rules
```

## Dependency Direction

The architecture follows clean architecture principles with dependencies flowing inward:

```
External Frameworks & Libraries
    ↓
API Layer (ASP.NET Core, Minimal APIs, HTTP)
    ↓
Application Layer (Business Logic, DTOs)
    ↓
Domain Layer (Business Rules, Entities)
    ↓
Infrastructure Layer (Data Access, External Services)
```

### Layer Responsibilities

1. **API Layer** (`Portfolio.Api`)
   - HTTP request/response handling
   - Minimal API endpoint definitions
   - Input validation
   - Error responses
   - CORS and middleware configuration
   - Dependency injection setup

2. **Application Layer** (`Portfolio.Application`)
   - Business logic and workflows
   - Service interfaces and implementations
   - DTO definitions and mapping
   - Use case orchestration
   - Input/output transformation

3. **Domain Layer** (`Portfolio.Domain`)
   - Core business entities
   - Business rules and validations
   - Value objects
   - Enumerations
   - **NO dependencies on frameworks or external libraries**

4. **Infrastructure Layer** (`Portfolio.Infrastructure`)
   - Entity Framework Core integration
   - Database context and configurations
   - Repository implementations
   - External service integrations
   - Configuration providers

## Key Principles

### Separation of Concerns

- Each layer has clear responsibilities
- API concerns don't leak into business logic
- Business logic doesn't depend on HTTP
- Domain entities are framework-agnostic

### Dependency Inversion

- High-level modules don't depend on low-level modules
- Both depend on abstractions (interfaces)
- Concrete implementations are registered via DI

### Serverless Deployment

- Application designed for Lambda but can run anywhere
- No hard dependencies on AWS SDKs in business logic
- AWS Lambda integration is at the hosting layer only
- Local development doesn't require AWS

### Modular but Monolithic

- Single deployable unit (not microservices)
- Clean module boundaries via project structure
- Can be split into services later if needed
- Shared domain model for consistency

## Technology Stack

### Backend

- **.NET 8** - Latest stable LTS version
- **ASP.NET Core** - Web framework
- **Entity Framework Core 8** - ORM for data access
- **Npgsql** - PostgreSQL provider for EF Core
- **AWS Lambda Hosting** - Serverless deployment

### Frontend

- **React 18** - UI framework
- **TypeScript** - Static typing
- **Vite** - Build tool and dev server
- **TailwindCSS** - Utility-first CSS (to be added)

### Cloud & Database

- **AWS Lambda** - Compute
- **AWS API Gateway** - HTTP API gateway
- **PostgreSQL** - Relational database
- **Supabase** - PostgreSQL hosting + auth (optional)

## Deployment Model

The application uses a **modular monolith deployed as serverless**:

- Single ASP.NET Core application deployed to Lambda
- API Gateway handles HTTP routing
- All endpoints go through the same application instance
- Scales automatically via Lambda
- Database pooling handled by .NET

## Development Workflow

### Local Development

1. Backend: `dotnet run` from `src/Portfolio.Api`
2. Frontend: `npm run dev` from `frontend/`
3. Backend runs on `https://localhost:5001`
4. Frontend runs on `http://localhost:5173`
5. API proxy configured in Vite for `/api` calls

### Production Deployment

1. Build: `dotnet publish -c Release`
2. Docker image created with ASP.NET Core runtime
3. Deploy to AWS Lambda
4. API Gateway configured with Lambda integration
5. CloudWatch for logging

## Security & Authentication

- **External Auth**: Supabase Auth or JWT-compatible provider
- **JWT Validation**: Middleware in API layer
- **CORS**: Configured for frontend domain
- **HTTPS**: Required in production
- **Secrets**: Environment variables, not hardcoded
- **Database**: Connection strings from environment

## Future Enhancements

- [x] Basic project structure
- [ ] Domain model implementation
- [ ] Portfolio CRUD operations
- [ ] Holdings management
- [ ] Exposure calculations
- [ ] ETF constituent lookup
- [ ] Authentication integration
- [ ] Data validation
- [ ] Error handling improvements
- [ ] Database migrations
- [ ] Performance optimization
- [ ] Comprehensive testing
- [ ] API documentation (Swagger)
- [ ] CI/CD pipelines
- [ ] Monitoring and alerting
