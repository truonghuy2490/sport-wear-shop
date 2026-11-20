-- Sport Wear Shop | MySQL 8.4 | IdentityServer database bootstrap
-- Run using a MySQL account with permission to create this database.
-- Set ConnectionStrings:DefaultConnection to sportwearshop_identity.
-- Apply IdentityServer's MySQL EF Core migrations to create the ASP.NET Core
-- Identity and OpenIddict tables. Seed roles via RoleManager in IdentityServer.

CREATE DATABASE IF NOT EXISTS sportwearshop_identity
    CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
