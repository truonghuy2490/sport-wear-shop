using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportWearShop.Repositories.Entities;
using SportWearShop.Repositories.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportWearShop.Repositories.SeedData
{
    public static class SeedDataInitializer
    {
        public static async Task SeedAsync(
            AppDbContext context,
            RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            await context.Database.MigrateAsync();

            await SeedRolesAsync(roleManager);

            await SeedBrandsAsync(context);
            await SeedCategoriesWith3LevelsAsync(context);
            await SeedProductsAsync(context);
            await SeedProductVariantsAsync(context);
            await SeedProductImagesAsync(context);
            await SeedInventoryStocksAsync(context);
            await SeedUsersAsync(userManager);
            
            // Các table khác (Cart, Order...) để sau vì phụ thuộc User
        }


        private static async Task SeedRolesAsync(
            RoleManager<AppRole> roleManager)
        {
            var roles = new[]
            {
                "Admin",
                "Staff",
                "Customer"
            };

            foreach (var roleName in roles)
            {
                var exists = await roleManager.RoleExistsAsync(roleName);

                if (!exists)
                {
                    await roleManager.CreateAsync(new AppRole
                    {
                        Name = roleName
                    });
                }
            }
        }

        private static async Task SeedUsersAsync(
            UserManager<AppUser> userManager)
        {
            var users = new[]
            {
                new
                {
                    Email = "admin@sportwearshop.com",
                    FirstName = "System",
                    LastName = "Admin",
                    Role = "Admin"
                },
                new
                {
                    Email = "staff@sportwearshop.com",
                    FirstName = "Shop",
                    LastName = "Staff",
                    Role = "Staff"
                },
                new
                {
                    Email = "customer@sportwearshop.com",
                    FirstName = "Demo",
                    LastName = "Customer",
                    Role = "Customer"
                }
            };

            foreach (var seedUser in users)
            {
                var existingUser = await userManager.FindByEmailAsync(seedUser.Email);

                if (existingUser is not null)
                {
                    continue;
                }

                var user = new AppUser
                {
                    UserName = seedUser.Email,
                    Email = seedUser.Email,
                    FirstName = seedUser.FirstName,
                    LastName = seedUser.LastName,
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(
                    user,
                    "Password@123"
                );

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ",
                        result.Errors.Select(e => e.Description));

                    throw new Exception(errors);
                }

                await userManager.AddToRoleAsync(user, seedUser.Role);
            }
        }

        private static async Task SeedBrandsAsync(AppDbContext context)
        {
            if (await context.Brands.AnyAsync()) return;
            var now = DateTime.UtcNow;
            var brands = new List<Brand>
            {
                new() { BrandCode = "NIKE", BrandName = "Nike", BrandImage = "/images/brands/nike.jpg", IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
                new() { BrandCode = "ADIDAS", BrandName = "Adidas", BrandImage = "/images/brands/adidas.jpg", IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
                new() { BrandCode = "PUMA", BrandName = "PUMA", BrandImage = "/images/brands/puma.jpg", IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
                new() { BrandCode = "UNDERARMOUR", BrandName = "Under Armour", BrandImage = "/images/brands/underarmour.jpg", IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
                new() { BrandCode = "NEWBALANCE", BrandName = "New Balance", BrandImage = "/images/brands/newbalance.jpg", IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
                new() { BrandCode = "ASICS", BrandName = "ASICS", BrandImage = "/images/brands/asics.jpg", IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
                new() { BrandCode = "REEBOK", BrandName = "Reebok", BrandImage = "/images/brands/reebok.jpg", IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
                new() { BrandCode = "FILA", BrandName = "FILA", BrandImage = "/images/brands/fila.jpg", IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
                new() { BrandCode = "CONVERSE", BrandName = "Converse", BrandImage = "/images/brands/converse.jpg", IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
                new() { BrandCode = "VANS", BrandName = "Vans", BrandImage = "/images/brands/vans.jpg", IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now }
            };
            await context.Brands.AddRangeAsync(brands);
            await context.SaveChangesAsync();
        }

        // ==================== CATEGORIES 3 LEVELS ====================
        private static async Task SeedCategoriesWith3LevelsAsync(AppDbContext context)
        {
            if (await context.Categories.AnyAsync()) return;
            var now = DateTime.UtcNow;

            var categories = new List<Category>
            {
                // Level 0 (Root) - Parent không có
                new() { CategoryCode = "FOOTWEAR", CategoryName = "Footwear", Description = "Giày thể thao", IsActive = true, SortOrder = 1, CreatedAtUtc = now, UpdatedAtUtc = now },

                // Level 1 - Children của Footwear
                new() { CategoryCode = "RUNNING", CategoryName = "Running Shoes", Description = "Giày chạy bộ", IsActive = true, ParentCategoryId = null, SortOrder = 1, CreatedAtUtc = now, UpdatedAtUtc = now }, // sẽ set Parent sau
                new() { CategoryCode = "SNEAKERS", CategoryName = "Sneakers", Description = "Giày sneaker casual", IsActive = true, SortOrder = 2, CreatedAtUtc = now, UpdatedAtUtc = now },
                new() { CategoryCode = "TRAINING", CategoryName = "Training Shoes", Description = "Giày tập luyện", IsActive = true, SortOrder = 3, CreatedAtUtc = now, UpdatedAtUtc = now },
                new() { CategoryCode = "BASKETBALL", CategoryName = "Basketball Shoes", IsActive = true, SortOrder = 4, CreatedAtUtc = now, UpdatedAtUtc = now },
                new() { CategoryCode = "FOOTBALL", CategoryName = "Football Shoes", IsActive = true, SortOrder = 5, CreatedAtUtc = now, UpdatedAtUtc = now },

                // Level 2 - Sub categories
                new() { CategoryCode = "LIFESTYLE", CategoryName = "Lifestyle", Description = "Giày phong cách sống", IsActive = true, SortOrder = 6, CreatedAtUtc = now, UpdatedAtUtc = now },
                new() { CategoryCode = "PERFORMANCE", CategoryName = "Performance", Description = "Giày hiệu suất cao", IsActive = true, SortOrder = 7, CreatedAtUtc = now, UpdatedAtUtc = now },

                // Apparel
                new() { CategoryCode = "APPAREL", CategoryName = "Apparel", Description = "Quần áo", IsActive = true, SortOrder = 8, CreatedAtUtc = now, UpdatedAtUtc = now },
                new() { CategoryCode = "HOODIE", CategoryName = "Hoodies", IsActive = true, SortOrder = 9, CreatedAtUtc = now, UpdatedAtUtc = now },
                new() { CategoryCode = "TSHIRT", CategoryName = "T-Shirts", IsActive = true, SortOrder = 10, CreatedAtUtc = now, UpdatedAtUtc = now }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            // Set ParentCategoryId cho 3 levels
            var catDict = await context.Categories.ToDictionaryAsync(c => c.CategoryCode, c => c.CategoryId);

            // Set Parent cho Level 1 & 2
            var updates = new List<Category>();
            updates.Add(await context.Categories.FindAsync(catDict["RUNNING"]));
            updates.Last().ParentCategoryId = catDict["FOOTWEAR"];

            updates.Add(await context.Categories.FindAsync(catDict["SNEAKERS"]));
            updates.Last().ParentCategoryId = catDict["FOOTWEAR"];

            updates.Add(await context.Categories.FindAsync(catDict["TRAINING"]));
            updates.Last().ParentCategoryId = catDict["FOOTWEAR"];

            updates.Add(await context.Categories.FindAsync(catDict["BASKETBALL"]));
            updates.Last().ParentCategoryId = catDict["FOOTWEAR"];

            updates.Add(await context.Categories.FindAsync(catDict["FOOTBALL"]));
            updates.Last().ParentCategoryId = catDict["FOOTWEAR"];

            updates.Add(await context.Categories.FindAsync(catDict["LIFESTYLE"]));
            updates.Last().ParentCategoryId = catDict["FOOTWEAR"];

            updates.Add(await context.Categories.FindAsync(catDict["PERFORMANCE"]));
            updates.Last().ParentCategoryId = catDict["FOOTWEAR"];

            updates.Add(await context.Categories.FindAsync(catDict["HOODIE"]));
            updates.Last().ParentCategoryId = catDict["APPAREL"];

            updates.Add(await context.Categories.FindAsync(catDict["TSHIRT"]));
            updates.Last().ParentCategoryId = catDict["APPAREL"];

            await context.SaveChangesAsync();
        }

        // ==================== PRODUCTS, VARIANTS, IMAGES, STOCK (≥10 records mỗi table) ====================
        private static async Task SeedProductsAsync(AppDbContext context)
        {
            if (await context.Products.AnyAsync()) return;

            var now = DateTime.UtcNow;

            var brandMap = await context.Brands
                .ToDictionaryAsync(x => x.BrandCode, x => x.BrandId);

            var catMap = await context.Categories
                .ToDictionaryAsync(x => x.CategoryCode, x => x.CategoryId);

            var products = new List<Product>
            {
                new()
                {
                    BrandId = brandMap["ADIDAS"],
                    CategoryId = catMap["RUNNING"],
                    ProductCode = "ADIDAS-ADIZERO-EVO-SL",
                    ProductName = "Adidas Adizero Evo SL",
                    Slug = "adidas-adizero-evo-sl",
                    Description = "Lightweight running shoes designed for speed training and race day.",
                    Gender = ProductGender.Unisex,
                    Status = ProductStatus.Active,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                },
                new()
                {
                    BrandId = brandMap["ADIDAS"],
                    CategoryId = catMap["SNEAKERS"],
                    ProductCode = "ADIDAS-SAMBA-OG",
                    ProductName = "Adidas Samba OG",
                    Slug = "adidas-samba-og",
                    Description = "Classic lifestyle sneakers with iconic 3-Stripes design.",
                    Gender = ProductGender.Unisex,
                    Status = ProductStatus.Active,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                },
                new()
                {
                    BrandId = brandMap["ADIDAS"],
                    CategoryId = catMap["LIFESTYLE"],
                    ProductCode = "ADIDAS-GAZELLE",
                    ProductName = "Adidas Gazelle",
                    Slug = "adidas-gazelle",
                    Description = "Retro-inspired lifestyle shoes with suede upper.",
                    Gender = ProductGender.Unisex,
                    Status = ProductStatus.Active,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                },
                new()
                {
                    BrandId = brandMap["ADIDAS"],
                    CategoryId = catMap["TRAINING"],
                    ProductCode = "ADIDAS-ULTRABOOST-5X",
                    ProductName = "Adidas Ultraboost 5X",
                    Slug = "adidas-ultraboost-5x",
                    Description = "Responsive running and training shoes with Boost cushioning.",
                    Gender = ProductGender.Unisex,
                    Status = ProductStatus.Active,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                },
                new()
                {
                    BrandId = brandMap["ADIDAS"],
                    CategoryId = catMap["RUNNING"],
                    ProductCode = "ADIDAS-SUPERNOVA-RISE",
                    ProductName = "Adidas Supernova Rise",
                    Slug = "adidas-supernova-rise",
                    Description = "Daily running shoes built for comfort and stability.",
                    Gender = ProductGender.Unisex,
                    Status = ProductStatus.Active,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                },
                new()
                {
                    BrandId = brandMap["ADIDAS"],
                    CategoryId = catMap["FOOTBALL"],
                    ProductCode = "ADIDAS-PREDATOR-CLUB",
                    ProductName = "Adidas Predator Club",
                    Slug = "adidas-predator-club",
                    Description = "Football boots designed for control and precision.",
                    Gender = ProductGender.Unisex,
                    Status = ProductStatus.Active,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                },
                new()
                {
                    BrandId = brandMap["ADIDAS"],
                    CategoryId = catMap["TRAINING"],
                    ProductCode = "ADIDAS-DROPSKET-TRAINER",
                    ProductName = "Adidas Dropset Trainer",
                    Slug = "adidas-dropset-trainer",
                    Description = "Training shoes suitable for gym workouts and lifting sessions.",
                    Gender = ProductGender.Unisex,
                    Status = ProductStatus.Active,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                },
                new()
                {
                    BrandId = brandMap["ADIDAS"],
                    CategoryId = catMap["SNEAKERS"],
                    ProductCode = "ADIDAS-FORUM-LOW",
                    ProductName = "Adidas Forum Low",
                    Slug = "adidas-forum-low",
                    Description = "Basketball-inspired sneakers with a classic low-cut design.",
                    Gender = ProductGender.Unisex,
                    Status = ProductStatus.Active,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                },
                new()
                {
                    BrandId = brandMap["ADIDAS"],
                    CategoryId = catMap["LIFESTYLE"],
                    ProductCode = "ADIDAS-STAN-SMITH",
                    ProductName = "Adidas Stan Smith",
                    Slug = "adidas-stan-smith",
                    Description = "Minimal clean sneakers with timeless court style.",
                    Gender = ProductGender.Unisex,
                    Status = ProductStatus.Active,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                },
                new()
                {
                    BrandId = brandMap["ADIDAS"],
                    CategoryId = catMap["RUNNING"],
                    ProductCode = "ADIDAS-DURAMO-SL",
                    ProductName = "Adidas Duramo SL",
                    Slug = "adidas-duramo-sl",
                    Description = "Lightweight running shoes for everyday training.",
                    Gender = ProductGender.Unisex,
                    Status = ProductStatus.Active,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }

        private static async Task SeedProductVariantsAsync(AppDbContext context)
        {
            if (await context.ProductVariants.AnyAsync()) return;

            var now = DateTime.UtcNow;

            var productMap = await context.Products
                .ToDictionaryAsync(x => x.ProductCode, x => x.ProductId);

            var variants = new List<ProductVariant>
            {
                new()
                {
                    ProductId = productMap["ADIDAS-ADIZERO-EVO-SL"],
                    Sku = "ADI-EVO-BLK-41",
                    ColorCode = "BLACK",
                    ColorName = "Core Black",
                    SizeCode = "41",
                    SizeLabel = "41 EU",
                    ListPrice = 3200000,
                    SalePrice = 2890000,
                    Status = ProductVariantStatus.Active,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                },
                new()
                {
                    ProductId = productMap["ADIDAS-SAMBA-OG"],
                    Sku = "ADI-SAM-WHT-40",
                    ColorCode = "WHITE",
                    ColorName = "Cloud White/Core Black",
                    SizeCode = "40",
                    SizeLabel = "40 EU",
                    ListPrice = 2800000,
                    SalePrice = 2490000,
                    Status = ProductVariantStatus.Active,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                },
                new()
                {
                    ProductId = productMap["ADIDAS-GAZELLE"],
                    Sku = "ADI-GAZ-BLU-42",
                    ColorCode = "BLUE",
                    ColorName = "Collegiate Blue",
                    SizeCode = "42",
                    SizeLabel = "42 EU",
                    ListPrice = 2600000,
                    SalePrice = 2290000,
                    Status = ProductVariantStatus.Active,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                },
                new()
                {
                    ProductId = productMap["ADIDAS-ULTRABOOST-5X"],
                    Sku = "ADI-UB5X-BLK-42",
                    ColorCode = "BLACK",
                    ColorName = "Core Black/Grey",
                    SizeCode = "42",
                    SizeLabel = "42 EU",
                    ListPrice = 5200000,
                    SalePrice = 4790000,
                    Status = ProductVariantStatus.Active,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                },
                new()
                {
                    ProductId = productMap["ADIDAS-SUPERNOVA-RISE"],
                    Sku = "ADI-SNR-WHT-41",
                    ColorCode = "WHITE",
                    ColorName = "Cloud White/Lucid Blue",
                    SizeCode = "41",
                    SizeLabel = "41 EU",
                    ListPrice = 3600000,
                    SalePrice = 3290000,
                    Status = ProductVariantStatus.Active,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                },
                new()
                {
                    ProductId = productMap["ADIDAS-PREDATOR-CLUB"],
                    Sku = "ADI-PRE-RED-42",
                    ColorCode = "RED",
                    ColorName = "Lucid Red/Core Black",
                    SizeCode = "42",
                    SizeLabel = "42 EU",
                    ListPrice = 1800000,
                    SalePrice = 1590000,
                    Status = ProductVariantStatus.Active,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                },
                new()
                {
                    ProductId = productMap["ADIDAS-DROPSKET-TRAINER"],
                    Sku = "ADI-DROP-GRY-41",
                    ColorCode = "GREY",
                    ColorName = "Grey Six/Core Black",
                    SizeCode = "41",
                    SizeLabel = "41 EU",
                    ListPrice = 3400000,
                    SalePrice = 2990000,
                    Status = ProductVariantStatus.Active,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                },
                new()
                {
                    ProductId = productMap["ADIDAS-FORUM-LOW"],
                    Sku = "ADI-FOR-WHT-40",
                    ColorCode = "WHITE",
                    ColorName = "Cloud White/Royal Blue",
                    SizeCode = "40",
                    SizeLabel = "40 EU",
                    ListPrice = 3000000,
                    SalePrice = 2690000,
                    Status = ProductVariantStatus.Active,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                },
                new()
                {
                    ProductId = productMap["ADIDAS-STAN-SMITH"],
                    Sku = "ADI-STAN-GRN-39",
                    ColorCode = "GREEN",
                    ColorName = "Cloud White/Green",
                    SizeCode = "39",
                    SizeLabel = "39 EU",
                    ListPrice = 2700000,
                    SalePrice = 2390000,
                    Status = ProductVariantStatus.Active,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                },
                new()
                {
                    ProductId = productMap["ADIDAS-DURAMO-SL"],
                    Sku = "ADI-DUR-BLK-41",
                    ColorCode = "BLACK",
                    ColorName = "Core Black/Cloud White",
                    SizeCode = "41",
                    SizeLabel = "41 EU",
                    ListPrice = 1900000,
                    SalePrice = 1690000,
                    Status = ProductVariantStatus.Active,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                }
            };

            await context.ProductVariants.AddRangeAsync(variants);
            await context.SaveChangesAsync();
        }

        private static async Task SeedProductImagesAsync(AppDbContext context)
        {
            if (await context.ProductImages.AnyAsync()) return;

            var now = DateTime.UtcNow;

            var products = await context.Products.ToListAsync();

            var images = new List<ProductImage>();

            foreach (var product in products)
            {
                images.AddRange(new[]
                {
                    new ProductImage
                    {
                        ProductId = product.ProductId,
                        ImageUrl = $"/images/products/{product.Slug}-1.jpg",
                        IsPrimary = true,
                        SortOrder = 1,
                        AltText = $"{product.ProductName} Front View",
                        CreatedAtUtc = now
                    },
                    new ProductImage
                    {
                        ProductId = product.ProductId,
                        ImageUrl = $"/images/products/{product.Slug}-2.jpg",
                        IsPrimary = false,
                        SortOrder = 2,
                        AltText = $"{product.ProductName} Side View",
                        CreatedAtUtc = now
                    },
                    new ProductImage
                    {
                        ProductId = product.ProductId,
                        ImageUrl = $"/images/products/{product.Slug}-3.jpg",
                        IsPrimary = false,
                        SortOrder = 3,
                        AltText = $"{product.ProductName} Back View",
                        CreatedAtUtc = now
                    }
                });
            }

            await context.ProductImages.AddRangeAsync(images);
            await context.SaveChangesAsync();
        }

        private static async Task SeedInventoryStocksAsync(AppDbContext context)
        {
            if (await context.InventoryStocks.AnyAsync()) return;
            var now = DateTime.UtcNow;
            var variants = await context.ProductVariants.ToListAsync();

            var stocks = variants.Select(v => new InventoryStock
            {
                ProductVariantId = v.ProductVariantId,
                QuantityOnHand = 100,
                QuantityReserved = 10,
                UpdatedAtUtc = now
            }).ToList();

            await context.InventoryStocks.AddRangeAsync(stocks);
            await context.SaveChangesAsync();
        }

        
    }
}