# Document 2: Architecture Guidelines for .NET APIs

## Overview

A well-structured architecture ensures that APIs are:

* Maintainable
* Scalable
* Testable
* Easy to extend

The recommended architecture for modern .NET APIs is **Clean Architecture with layered separation**.

---

## Layered Architecture

```
API Layer
Application Layer
Domain Layer
Infrastructure Layer
```

---

## 1. API Layer (Presentation)

Responsibilities:

* Handle HTTP requests
* Validate incoming data
* Return HTTP responses

Components:

```
Controllers
Request DTOs
Response DTOs
Filters
Middleware
```

Rules:

* No business logic
* No database access

---

## 2. Application Layer

Responsibilities:

* Implement **application use cases**
* Coordinate domain objects
* Handle commands and queries

Components:

```
Commands
Queries
Handlers
Services
DTOs
Validators
```

Example

```
CreateUserCommand
CreateUserCommandHandler
GetUserByIdQuery
GetUserByIdQueryHandler
```

---

## 3. Domain Layer

Responsibilities:

* Core business rules
* Business entities
* Value objects

Components

```
Entities
ValueObjects
Enums
Domain Services
Interfaces
```

Rules

* No infrastructure dependency
* Pure business logic

---

## 4. Infrastructure Layer

Responsibilities

* External systems
* Database access
* File storage
* Message brokers

Components

```
Repositories
Database Context
External APIs
Storage Services
```

Example

```
UserRepository
AzureBlobStorageService
EmailService
```

---

## Dependency Rule

Dependencies should always flow **inward**.

```
Infrastructure → Application → Domain
API → Application
```

Domain must remain **independent of frameworks**.

---

## Benefits

* High modularity
* Independent testing
* Easy infrastructure replacement
* Scalable system design
