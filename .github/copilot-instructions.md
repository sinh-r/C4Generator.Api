# C4Generator.Api – Project Structure

## 1. Purpose

The API acts as the **entry point of the platform**.
It exposes REST endpoints that allow users to:

* Register repositories
* Trigger architecture generation
* Monitor architecture generation jobs
* Retrieve generated C4 diagrams
* Retrieve architecture insights

The API **does not perform heavy processing**.
Instead, it **publishes tasks to a queue** which are processed by workers.

---

# 2. Root Structure

```
C4Generator.Api
│
├── Controllers
├── Middlewares
├── Filters
├── Extensions
├── Configurations
├── DTOs
├── Validators
├── Services
├── BackgroundJobs
├── HealthChecks
├── Authorization
├── Logging
│
├── Program.cs
├── appsettings.json
└── appsettings.Development.json
```

---

# 3. Controllers

Controllers expose REST APIs.

```
Controllers
│
├── RepositoryController.cs
├── ArchitectureController.cs
├── VisualizationController.cs
├── InsightsController.cs
├── JobController.cs
└── HealthController.cs
```

### RepositoryController

Handles repository management.

Endpoints:

```
POST /repositories
GET /repositories
GET /repositories/{id}
DELETE /repositories/{id}
```

---

### ArchitectureController

Handles architecture generation.

```
POST /architecture/generate
GET  /architecture/{id}
```

---

### JobController

Used to track generation jobs.

```
GET /jobs/{jobId}
GET /jobs
```

---

### VisualizationController

Provides data required by UI to render diagrams.

```
GET /architecture/{id}/context
GET /architecture/{id}/containers
GET /architecture/{id}/components
GET /architecture/{id}/classes
```

---

### InsightsController

Architecture diagnostics.

```
GET /architecture/{id}/insights
```

---

# 4. Middlewares

Custom middleware for cross-cutting concerns.

```
Middlewares
│
├── ExceptionHandlingMiddleware.cs
├── RequestLoggingMiddleware.cs
├── RateLimitingMiddleware.cs
└── CorrelationIdMiddleware.cs
```

Responsibilities:

* Global exception handling
* Structured request logging
* Correlation tracing
* API rate limiting

---

# 5. Filters

Filters are used for request/response processing.

```
Filters
│
├── ValidationFilter.cs
├── ApiExceptionFilter.cs
└── ResultWrapperFilter.cs
```

---

# 6. Extensions

Extension methods for clean dependency registration.

```
Extensions
│
├── ServiceCollectionExtensions.cs
├── MiddlewareExtensions.cs
└── SwaggerExtensions.cs
```

---

# 7. Configurations

Configuration bindings for external services.

```
Configurations
│
├── GitHubSettings.cs
├── QueueSettings.cs
├── AISettings.cs
├── DatabaseSettings.cs
└── AuthSettings.cs
```

---

# 8. DTOs

Request and response models.

```
DTOs
│
├── Requests
│   ├── CreateRepositoryRequest.cs
│   ├── GenerateArchitectureRequest.cs
│
├── Responses
│   ├── RepositoryResponse.cs
│   ├── ArchitectureResponse.cs
│   ├── JobStatusResponse.cs
│   └── InsightResponse.cs
```

---

# 9. Validators

Request validation.

```
Validators
│
├── CreateRepositoryValidator.cs
└── GenerateArchitectureValidator.cs
```

---

# 10. Services

Application services used by controllers.

```
Services
│
├── RepositoryService.cs
├── ArchitectureService.cs
├── JobService.cs
└── InsightService.cs
```

Responsibilities:

* Validate requests
* Publish queue events
* Query architecture metadata

---

# 11. BackgroundJobs (Optional)

Used for periodic tasks.

```
BackgroundJobs
│
├── RepositorySyncJob.cs
└── ArchitectureCleanupJob.cs
```

Examples:

* Detect new repositories
* Remove old architecture models

---

# 12. HealthChecks

Used for system monitoring.

```
HealthChecks
│
├── DatabaseHealthCheck.cs
├── QueueHealthCheck.cs
└── GitHubHealthCheck.cs
```

---

# 13. Authorization

```
Authorization
│
├── Policies
│   ├── AdminPolicy.cs
│
├── Handlers
│   └── AdminPolicyHandler.cs
```

---

# 14. Logging

```
Logging
│
├── LoggingEnricher.cs
└── RequestLogger.cs
```

---

# 15. Program.cs Responsibilities

Program.cs should configure:

* Dependency injection
* Middleware pipeline
* Authentication
* Swagger
* Logging
* Health checks
