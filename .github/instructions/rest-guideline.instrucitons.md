# Document 3: REST API Design Guidelines

## Overview

RESTful APIs should follow consistent standards for **resource naming, HTTP methods, and response handling**.

---

## Resource Naming

Use **plural nouns** for resources.

Good

```
/users
/orders
/products
```

Bad

```
/getUsers
/createUser
```

---

## HTTP Method Usage

| Method | Purpose           |
| ------ | ----------------- |
| GET    | Retrieve resource |
| POST   | Create resource   |
| PUT    | Replace resource  |
| PATCH  | Partial update    |
| DELETE | Remove resource   |

Example

```
GET    /users
GET    /users/{id}
POST   /users
PUT    /users/{id}
DELETE /users/{id}
```

---

## Status Codes

Use proper HTTP status codes.

```
200 OK
201 Created
204 NoContent
400 BadRequest
401 Unauthorized
403 Forbidden
404 NotFound
500 InternalServerError
```

---

## Idempotency

Certain operations must produce **the same result when repeated**.

Idempotent methods:

```
GET
PUT
DELETE
```

Example

```
PUT /users/1
```

Calling it multiple times should not create duplicates.

---

## API Versioning

Version APIs to support backward compatibility.

Example

```
/api/v1/users
/api/v2/users
```

Versioning methods

* URL versioning
* Header versioning
* Query parameter versioning

---

## Response Structure

Use consistent response formats.

Example

```
{
  "success": true,
  "data": {},
  "errors": []
}
```

---

## Pagination

For large datasets always implement pagination.

Example

```
GET /users?page=1&pageSize=20
```

---

## Filtering and Sorting

```
GET /users?role=admin
GET /users?sort=name
```

---

## Key Takeaway

RESTful APIs should be:

* Predictable
* Consistent
* Easy to consume
