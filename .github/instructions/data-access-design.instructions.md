# Document 4: Data Access and Design Patterns

## Overview

A well-designed API should abstract database interactions and follow established design patterns.

---

## Repository Pattern

Separates **data access logic from business logic**.

Example interface

```
IUserRepository
  GetByIdAsync()
  AddAsync()
  UpdateAsync()
  DeleteAsync()
```

Implementation

```
UserRepository : IUserRepository
```

Benefits

* Decouples database
* Simplifies testing
* Centralizes data logic

---

## Unit of Work Pattern

Ensures multiple database operations execute in a **single transaction**.

Example

```
OrderService
  CreateOrder()
  UpdateInventory()
  SaveInvoice()
```

All operations should commit together.

---

## CQRS Pattern

Command Query Responsibility Segregation separates **write operations from read operations**.

Commands

```
CreateUserCommand
UpdateUserCommand
DeleteUserCommand
```

Queries

```
GetUserByIdQuery
GetUsersQuery
```

Handlers

```
CreateUserCommandHandler
GetUserByIdQueryHandler
```

Benefits

* Clear separation of concerns
* Easier optimization
* Better scalability

---

## DTO Pattern

Never expose database entities directly.

Bad

```
return UserEntity
```

Good

```
return UserResponseDto
```

Benefits

* Prevents data leakage
* Allows API evolution
* Decouples domain model

---

## Dependency Injection

Register services in the DI container.

Example

```
builder.Services.AddScoped<IUserService, UserService>();
```

Benefits

* Loose coupling
* Easy testing
* Clean architecture
