# SportWearShop

```txt
SPORTWEARSHOP
│
├── src
│   ├── SportWearShop.APIs         # Web APIs
│   ├── SportWearShop.BusinessLogics
│   ├── SportWearShop.Repositories
│   ├── SportWearShop.Shared
│   ├── SportWearShop.Web          # Razor Pages: customer/staff
│   └── SportWearShop.Admin        # ReactJS: admin
│
│
├── .dockerignore
├── .gitignore
├── .gitattributes
├── docker-compose.yml
├── LICENSE.txt
├── README.md
└── SportWearShop.sln
```

```txt
SPORTWEARSHOP
│
├── src
│   ├── SportWearShop.APIs         # Web APIs
│   ├── SportWearShop.BusinessLogics
│   ├── SportWearShop.Repositories
│   ├── SportWearShop.Shared
│   ├── SportWearShop.Web          # Razor Pages: customer/staff
│   └── SportWearShop.Admin        # ReactJS: admin
│
│
├── .dockerignore
├── .gitignore
├── .gitattributes
├── docker-compose.yml
├── LICENSE.txt
├── README.md
└── SportWearShop.sln
```

Project reference
Controller -> Service -> Repository

Install EF CLI tool:
"
dotnet tool install --global dotnet-ef
"

Install package:
- Repository
"
dotnet add SportWearShop.Repositories package Microsoft.EntityFrameworkCore.SqlServer
dotnet add SportWearShop.Repositories package Microsoft.EntityFrameworkCore.Design
dotnet add SportWearShop.Repositories package Microsoft.EntityFrameworkCore.Tools
"
- APIs
"
dotnet add SportWearShop.APIs package Microsoft.EntityFrameworkCore.Design
"

Scafford DB: 
stay on SportWearShop cd 
"
dotnet ef dbcontext scaffold "Server=localhost,1433;Database=SportWearShopDb;User Id=sa;Password=123456;TrustServerCertificate=True;MultipleActiveResultSets=true" Microsoft.EntityFrameworkCore.SqlServer --project SportWearShop.Repositories --startup-project SportWearShop.APIs --context AppDbContext --context-dir . --output-dir Entities --force
"

Setup UserIdentity 

# Repository layer (DbContext + Identity core)
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore

Create class AppUser + AppRole

set up with Add migration 
"
dotnet ef migrations add InitIdentity --project SportWearShop.Repositories --startup-project SportWearShop.APIs --output-dir Migrations
dotnet ef migrations add UpdateProductStatusEnums --project SportWearShop.Repositories --startup-project SportWearShop.APIs --output-dir Migrations
dotnet ef migrations add UpdateProductStatusEnums --project SportWearShop.Repositories --startup-project SportWearShop.APIs --output-dir Migrations
"
"
dotnet ef dabase update --project SportWearShop.Repositories --startup-project SportWearShop.APIs 
"
Setup UserFK from AppUser to Cart, OrderHeader, UserAddress, ProductRating
"
// 👇 thêm navigation
    public virtual AppUser User { get; set; } = null!;
"
Update AddForeignKey in DbContext:  Cart, OrderHeader, UserAddress, ProductRating
"
modelBuilder.Entity<Cart>(entity =>
{
    entity.HasOne(d => d.User)
          .WithMany()
          .HasForeignKey(d => d.UserId)
          .OnDelete(DeleteBehavior.Cascade);
});
"

sportwearshop.apis add packages
"
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
