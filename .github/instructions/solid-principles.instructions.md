# Document 1: SOLID Principles for .NET API Development

## Overview

SOLID principles ensure that the codebase remains **maintainable, scalable, and testable**. These principles are fundamental to designing clean and modular APIs.

---

## 1. Single Responsibility Principle (SRP)

A class should have **only one reason to change**.

### Guidelines

* Controllers should only handle **HTTP request/response logic**
* Business logic should be implemented in **services or handlers**
* Data access should be handled in **repositories**

### Example

Bad

```
UserService
 ├─ CreateUser()
 ├─ SendEmail()
 ├─ SaveToDatabase()
```

Good

```
UserController → handles HTTP
UserService → business logic
EmailService → email notifications
UserRepository → database operations
```

---

## 2. Open Closed Principle (OCP)

Software entities should be:

* Open for extension
* Closed for modification

### Guidelines

* Prefer **interfaces and abstractions**
* Avoid modifying existing classes when adding new features
* Use strategy or factory patterns when behavior varies

Example

Bad

```
if(paymentType == "Card")
if(paymentType == "UPI")
```

Better

```
IPaymentProcessor
CardPaymentProcessor
UpiPaymentProcessor
```

---

## 3. Liskov Substitution Principle (LSP)

Derived classes must be **interchangeable with base classes** without breaking behavior.

### Guidelines

* Ensure implementations follow interface contracts
* Avoid changing expected behavior in derived classes

Example

```
IStorageService
  Upload()

AzureBlobStorageService
MinioStorageService
S3StorageService
```

---

## 4. Interface Segregation Principle (ISP)

Clients should not depend on **methods they do not use**.

### Guidelines

* Create **small and focused interfaces**
* Avoid “fat interfaces”

Example

Bad

```
IUserService
CreateUser()
DeleteUser()
SendEmail()
GenerateReport()
```

Better

```
IUserCommandService
IUserQueryService
IEmailService
```

---

## 5. Dependency Inversion Principle (DIP)

High level modules should depend on **abstractions, not concrete implementations**.

### Guidelines

* Always depend on interfaces
* Use dependency injection

Example

Bad

```
UserService → SqlUserRepository
```

Good

```
UserService → IUserRepository
SqlUserRepository → IUserRepository
```

---

## Key Takeaway

Following SOLID principles results in:

* Loose coupling
* High testability
* Better maintainability
* Easier extensibility
