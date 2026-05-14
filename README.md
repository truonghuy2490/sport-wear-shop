# SportWearShop Backend

Backend system for SportWearShop built with ASP.NET Core Web API using 3-layer architecture.

---

# Project Structure

```txt
SPORTWEARSHOP
│
├── src
│   ├── SportWearShop.APIs              # Presentation Layer (Controllers, Middleware)
│   ├── SportWearShop.BusinessLogics    # Business Layer (Services, Validation, Rules)
│   ├── SportWearShop.Repositories      # Data Access Layer (EF Core, DbContext, Repositories)
│   ├── SportWearShop.Shared            # Shared DTOs, constants, helpers
│   ├── SportWearShop.Web               # Razor Pages frontend
│   └── SportWearShop.Admin             # React Admin frontend
│
├── docker-compose.yml
├── SportWearShop.sln
└── README.md
```

---

# Architecture

This project follows **3-layer architecture**:

```txt
Controller Layer
      ↓
Service Layer
      ↓
Repository Layer
      ↓
Database
```

## Layer Responsibilities

### 1. APIs Layer (Presentation Layer)

Responsible for:

- HTTP request handling
- routing
- authorization
- authentication
- model binding
- API response formatting
- middleware / exception handling

Examples:

```txt
Controllers/
ExceptionHandlers/
Middlewares/
Extensions/
```

Flow example:

```txt
ProductController
    → ProductService
        → ProductRepository
            → SQL Server
```

---

### 2. BusinessLogics Layer (Service Layer)

Responsible for:

- business rules
- validation
- workflow orchestration
- DTO mapping
- exception throwing
- service abstraction

Examples:

```txt
Services/
Interfaces/
Validators/
Exceptions/
Mappings/
```

Examples of business rules:

- cannot publish product without variants
- cannot activate deleted brand
- validate duplicate category name
- refresh token validation
- stock adjustment rules

---

### 3. Repositories Layer (Data Access Layer)

Responsible for:

- database communication
- EF Core DbContext
- repository implementation
- query execution
- entity persistence

Examples:

```txt
Entities/
Repositories/
UnitOfWork/
AppDbContext.cs
```

---

### 4. Shared Layer

Responsible for shared contracts:

- request models
- response models
- enums
- constants
- pagination models

Examples:

```txt
ViewModels/
Enums/
Constants/
Helpers/
```

---

# Technology Stack

## Backend

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- ASP.NET Identity
- JWT Authentication
- FluentValidation

---

# Package Dependencies

## Repositories

```bash
dotnet add SportWearShop.Repositories package Microsoft.EntityFrameworkCore.SqlServer
dotnet add SportWearShop.Repositories package Microsoft.EntityFrameworkCore.Design
dotnet add SportWearShop.Repositories package Microsoft.EntityFrameworkCore.Tools
dotnet add SportWearShop.Repositories package Microsoft.AspNetCore.Identity.EntityFrameworkCore
```

## APIs

```bash
dotnet add SportWearShop.APIs package Microsoft.EntityFrameworkCore.Design
dotnet add SportWearShop.APIs package FluentValidation.AspNetCore
dotnet add SportWearShop.APIs package Microsoft.AspNetCore.Authentication.JwtBearer
```

## BusinessLogics

```bash
dotnet add SportWearShop.BusinessLogics package FluentValidation
```

---

# Database Setup

## Install EF Core CLI

```bash
dotnet tool install --global dotnet-ef
```

---

## Scaffold Database

Run from solution root:

```bash
dotnet ef dbcontext scaffold \
"Server=localhost,1433;Database=SportWearShopDb;User Id=sa;Password=123456;TrustServerCertificate=True;MultipleActiveResultSets=true" \
Microsoft.EntityFrameworkCore.SqlServer \
--project SportWearShop.Repositories \
--startup-project SportWearShop.APIs \
--context AppDbContext \
--context-dir . \
--output-dir Entities \
--force
```

---

# Identity Setup

Authentication uses ASP.NET Identity.

## Core Identity Entities

Custom user:

```csharp
AppUser : IdentityUser
```

Custom role:

```csharp
AppRole : IdentityRole
```

Used for:

- Admin
- Staff
- Customer

---

## Identity Migration

```bash
dotnet ef migrations add InitIdentity \
--project SportWearShop.Repositories \
--startup-project SportWearShop.APIs \
--output-dir Migrations
```

Example schema update:

```bash
dotnet ef migrations add UpdateProductStatusEnums \
--project SportWearShop.Repositories \
--startup-project SportWearShop.APIs \
--output-dir Migrations
```

Apply:

```bash
dotnet ef database update \
--project SportWearShop.Repositories \
--startup-project SportWearShop.APIs
```

---

# User Relationships

AppUser is linked to:

- Cart
- OrderHeader
- UserAddress
- ProductRating

Example:

```csharp
public virtual AppUser User { get; set; } = null!;
```

DbContext mapping:

```csharp
modelBuilder.Entity<Cart>(entity =>
{
    entity.HasOne(d => d.User)
          .WithMany()
          .HasForeignKey(d => d.UserId)
          .OnDelete(DeleteBehavior.Cascade);
});
```

---

# Authentication Architecture

JWT authentication with refresh token support.

Flow:

```txt
Login
   ↓
Validate credentials
   ↓
Generate Access Token
Generate Refresh Token
   ↓
Save Refresh Token
   ↓
Return tokens
```

Protected API request:

```txt
Client sends Access Token
   ↓
JWT middleware validates
   ↓
Controller executes
```

Expired token:

```txt
401 Unauthorized
   ↓
Client sends Refresh Token
   ↓
Backend validates refresh token
   ↓
Issue new access token
```

---

# API Design Principles

RESTful endpoints:

Examples:

```txt
GET    /api/products
GET    /api/products/{id}
POST   /api/products
PUT    /api/products/{id}
PATCH  /api/products/{id}/publish
DELETE /api/products/{id}
```

Patterns used:

- pagination
- filtering
- sorting
- soft delete
- activation/deactivation
- batch operations

---

# Exception Handling

Global exception handling for:

- BadRequestException
- NotFoundException
- UnauthorizedException
- ConflictException
- ValidationException

Benefits:

- consistent API responses
- centralized error handling
- cleaner controllers

---

# Validation

FluentValidation used for:

- request DTO validation
- business input validation

Examples:

```txt
CreateProductValidator
CreateCategoryValidator
LoginRequestValidator
```

---

# Repository Pattern

Repositories abstract EF Core access.

Example:

```txt
IProductRepository
ProductRepository
```

Benefits:

- cleaner service layer
- reusable queries
- easier testing
- separation of concerns

---

# Unit of Work

Used to coordinate transactions across repositories.

Example:

```txt
IUnitOfWork
UnitOfWork
```

Responsibilities:

- save changes
- repository access
- transaction consistency

---

# Core Features

Implemented modules:

## Authentication

- login
- refresh token
- logout
- role authorization

## Product

- CRUD
- publish
- activate/deactivate
- product detail

## Product Variant

- single create
- batch create
- update
- publish

## Category

- CRUD
- parent/child hierarchy

## Brand

- CRUD
- active/inactive

## Inventory

- stock tracking
- movement history
- adjustments

## Orders

- order management

---

# Development Run

Restore:

```bash
dotnet restore
```

Run API:

```bash
dotnet run --project SportWearShop.APIs
```

Default:

```txt
https://localhost:xxxx
```

---

# Future Improvements

- unit test coverage
- caching with Redis
- Redis refresh token storage
- dashboard analytics APIs