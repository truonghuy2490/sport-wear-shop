using Microsoft.EntityFrameworkCore;
using SportWearShop.Repositories.Entities;

namespace SportWearShop.Repositories.SeedData;

public static class SeedDataInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        await SeedBrandsAsync(context);
        await SeedCategoriesAsync(context);
        await SeedProductsAsync(context);
        await SeedProductVariantsAsync(context);
    }

    private static async Task SeedBrandsAsync(AppDbContext context)
    {
        if (await context.Brands.AnyAsync()) return;

        var now = DateTime.UtcNow;

        var brands = new List<Brand>
        {
            new() { BrandCode = "NIKE",       BrandName = "Nike",        IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { BrandCode = "ADIDAS",     BrandName = "Adidas",      IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { BrandCode = "PUMA",       BrandName = "PUMA",        IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { BrandCode = "UNDERARMOUR",BrandName = "Under Armour",IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { BrandCode = "NEWBALANCE", BrandName = "New Balance", IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { BrandCode = "ASICS",      BrandName = "ASICS",       IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { BrandCode = "REEBOK",     BrandName = "Reebok",      IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { BrandCode = "FILA",       BrandName = "FILA",        IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { BrandCode = "CONVERSE",   BrandName = "Converse",    IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { BrandCode = "VANS",       BrandName = "Vans",        IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now }
        };

        await context.Brands.AddRangeAsync(brands);
        await context.SaveChangesAsync();
    }

    private static async Task SeedCategoriesAsync(AppDbContext context)
    {
        if (await context.Categories.AnyAsync()) return;

        var now = DateTime.UtcNow;

        var categories = new List<Category>
        {
            new() { CategoryCode = "RUNNING",    CategoryName = "Running Shoes",   IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { CategoryCode = "SNEAKERS",   CategoryName = "Sneakers",        IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { CategoryCode = "TRAINING",   CategoryName = "Training Shoes",  IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { CategoryCode = "BASKETBALL", CategoryName = "Basketball Shoes",IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { CategoryCode = "FOOTBALL",   CategoryName = "Football Shoes",  IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { CategoryCode = "SANDALS",    CategoryName = "Sandals",         IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { CategoryCode = "HOODIE",     CategoryName = "Hoodies",         IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { CategoryCode = "TSHIRT",     CategoryName = "T-Shirts",        IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { CategoryCode = "SHORTS",     CategoryName = "Shorts",          IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { CategoryCode = "JACKET",     CategoryName = "Jackets",         IsActive = true, CreatedAtUtc = now, UpdatedAtUtc = now }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
    }

    private static async Task SeedProductsAsync(AppDbContext context)
    {
        if (await context.Products.AnyAsync()) return;

        var now = DateTime.UtcNow;

        var brandMap = await context.Brands
            .ToDictionaryAsync(x => x.BrandCode, x => x.BrandId);

        var categoryMap = await context.Categories
            .ToDictionaryAsync(x => x.CategoryCode, x => x.CategoryId);

        var products = new List<Product>
        {
            new()
            {
                BrandId = brandMap["NIKE"],
                CategoryId = categoryMap["RUNNING"],
                ProductCode = "NIKE-AIR-ZOOM-PEGASUS-40",
                ProductName = "Nike Air Zoom Pegasus 40",
                Slug = "nike-air-zoom-pegasus-40",
                Description = "Daily running shoes with responsive cushioning.",
                Gender = "UNISEX",
                Status = "ACTIVE",
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            },
            new()
            {
                BrandId = brandMap["ADIDAS"],
                CategoryId = categoryMap["RUNNING"],
                ProductCode = "ADIDAS-ULTRABOOST-LIGHT",
                ProductName = "Adidas Ultraboost Light",
                Slug = "adidas-ultraboost-light",
                Description = "Premium running shoes with soft boost foam.",
                Gender = "UNISEX",
                Status = "ACTIVE",
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            },
            new()
            {
                BrandId = brandMap["PUMA"],
                CategoryId = categoryMap["TRAINING"],
                ProductCode = "PUMA-VELOCITY-NITRO",
                ProductName = "PUMA Velocity Nitro",
                Slug = "puma-velocity-nitro",
                Description = "Lightweight and stable shoes for gym sessions.",
                Gender = "UNISEX",
                Status = "ACTIVE",
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            },
            new()
            {
                BrandId = brandMap["UNDERARMOUR"],
                CategoryId = categoryMap["BASKETBALL"],
                ProductCode = "UA-CURRY-SPLASH",
                ProductName = "Under Armour Curry Splash",
                Slug = "under-armour-curry-splash",
                Description = "Basketball shoes inspired by quick court movement.",
                Gender = "MEN",
                Status = "ACTIVE",
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            },
            new()
            {
                BrandId = brandMap["NEWBALANCE"],
                CategoryId = categoryMap["SNEAKERS"],
                ProductCode = "NB-574-CLASSIC",
                ProductName = "New Balance 574 Classic",
                Slug = "new-balance-574-classic",
                Description = "Iconic lifestyle sneakers for daily wear.",
                Gender = "UNISEX",
                Status = "ACTIVE",
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            },
            new()
            {
                BrandId = brandMap["ASICS"],
                CategoryId = categoryMap["RUNNING"],
                ProductCode = "ASICS-GEL-KAYANO-30",
                ProductName = "ASICS Gel-Kayano 30",
                Slug = "asics-gel-kayano-30",
                Description = "Supportive running shoes with gel cushioning.",
                Gender = "UNISEX",
                Status = "ACTIVE",
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            },
            new()
            {
                BrandId = brandMap["REEBOK"],
                CategoryId = categoryMap["TRAINING"],
                ProductCode = "REEBOK-NANO-X3",
                ProductName = "Reebok Nano X3",
                Slug = "reebok-nano-x3",
                Description = "Versatile cross-training shoes for strength and cardio.",
                Gender = "UNISEX",
                Status = "ACTIVE",
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            },
            new()
            {
                BrandId = brandMap["FILA"],
                CategoryId = categoryMap["SNEAKERS"],
                ProductCode = "FILA-DISRUPTOR-II",
                ProductName = "FILA Disruptor II",
                Slug = "fila-disruptor-ii",
                Description = "Chunky sneakers with bold streetwear style.",
                Gender = "WOMEN",
                Status = "ACTIVE",
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            },
            new()
            {
                BrandId = brandMap["CONVERSE"],
                CategoryId = categoryMap["SNEAKERS"],
                ProductCode = "CONVERSE-CHUCK-70",
                ProductName = "Converse Chuck 70",
                Slug = "converse-chuck-70",
                Description = "Classic canvas sneakers with timeless design.",
                Gender = "UNISEX",
                Status = "ACTIVE",
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            },
            new()
            {
                BrandId = brandMap["VANS"],
                CategoryId = categoryMap["SNEAKERS"],
                ProductCode = "VANS-OLD-SKOOL",
                ProductName = "Vans Old Skool",
                Slug = "vans-old-skool",
                Description = "Skate-inspired sneakers for daily casual use.",
                Gender = "UNISEX",
                Status = "ACTIVE",
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
            new() { ProductId = productMap["NIKE-AIR-ZOOM-PEGASUS-40"], Sku = "NIKE-PEG40-BLK-41", ColorCode = "BLACK", ColorName = "Black", SizeCode = "41", SizeLabel = "41", ListPrice = 3200000, SalePrice = 2990000, Status = "ACTIVE", CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { ProductId = productMap["NIKE-AIR-ZOOM-PEGASUS-40"], Sku = "NIKE-PEG40-WHT-42", ColorCode = "WHITE", ColorName = "White", SizeCode = "42", SizeLabel = "42", ListPrice = 3200000, SalePrice = 3050000, Status = "ACTIVE", CreatedAtUtc = now, UpdatedAtUtc = now },

            new() { ProductId = productMap["ADIDAS-ULTRABOOST-LIGHT"], Sku = "ADI-UBL-BLK-41", ColorCode = "BLACK", ColorName = "Black", SizeCode = "41", SizeLabel = "41", ListPrice = 3500000, SalePrice = 3290000, Status = "ACTIVE", CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { ProductId = productMap["ADIDAS-ULTRABOOST-LIGHT"], Sku = "ADI-UBL-WHT-42", ColorCode = "WHITE", ColorName = "White", SizeCode = "42", SizeLabel = "42", ListPrice = 3500000, SalePrice = 3350000, Status = "ACTIVE", CreatedAtUtc = now, UpdatedAtUtc = now },

            new() { ProductId = productMap["PUMA-VELOCITY-NITRO"], Sku = "PUMA-VN-RED-40", ColorCode = "RED", ColorName = "Red", SizeCode = "40", SizeLabel = "40", ListPrice = 2800000, SalePrice = 2590000, Status = "ACTIVE", CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { ProductId = productMap["PUMA-VELOCITY-NITRO"], Sku = "PUMA-VN-BLK-41", ColorCode = "BLACK", ColorName = "Black", SizeCode = "41", SizeLabel = "41", ListPrice = 2800000, SalePrice = 2690000, Status = "ACTIVE", CreatedAtUtc = now, UpdatedAtUtc = now },

            new() { ProductId = productMap["UA-CURRY-SPLASH"], Sku = "UA-CURRY-BLU-42", ColorCode = "BLUE", ColorName = "Blue", SizeCode = "42", SizeLabel = "42", ListPrice = 3100000, SalePrice = 2890000, Status = "ACTIVE", CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { ProductId = productMap["UA-CURRY-SPLASH"], Sku = "UA-CURRY-BLK-43", ColorCode = "BLACK", ColorName = "Black", SizeCode = "43", SizeLabel = "43", ListPrice = 3100000, SalePrice = 2950000, Status = "ACTIVE", CreatedAtUtc = now, UpdatedAtUtc = now },

            new() { ProductId = productMap["NB-574-CLASSIC"], Sku = "NB574-GRY-41", ColorCode = "GREY", ColorName = "Grey", SizeCode = "41", SizeLabel = "41", ListPrice = 2600000, SalePrice = 2450000, Status = "ACTIVE", CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { ProductId = productMap["NB-574-CLASSIC"], Sku = "NB574-NVY-42", ColorCode = "NAVY", ColorName = "Navy", SizeCode = "42", SizeLabel = "42", ListPrice = 2600000, SalePrice = 2490000, Status = "ACTIVE", CreatedAtUtc = now, UpdatedAtUtc = now },

            new() { ProductId = productMap["ASICS-GEL-KAYANO-30"], Sku = "ASICS-K30-GRN-41", ColorCode = "GREEN", ColorName = "Green", SizeCode = "41", SizeLabel = "41", ListPrice = 3600000, SalePrice = 3390000, Status = "ACTIVE", CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { ProductId = productMap["ASICS-GEL-KAYANO-30"], Sku = "ASICS-K30-BLK-42", ColorCode = "BLACK", ColorName = "Black", SizeCode = "42", SizeLabel = "42", ListPrice = 3600000, SalePrice = 3450000, Status = "ACTIVE", CreatedAtUtc = now, UpdatedAtUtc = now },

            new() { ProductId = productMap["REEBOK-NANO-X3"], Sku = "REEBOK-NANO-WHT-40", ColorCode = "WHITE", ColorName = "White", SizeCode = "40", SizeLabel = "40", ListPrice = 2900000, SalePrice = 2750000, Status = "ACTIVE", CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { ProductId = productMap["REEBOK-NANO-X3"], Sku = "REEBOK-NANO-RED-41", ColorCode = "RED", ColorName = "Red", SizeCode = "41", SizeLabel = "41", ListPrice = 2900000, SalePrice = 2790000, Status = "ACTIVE", CreatedAtUtc = now, UpdatedAtUtc = now },

            new() { ProductId = productMap["FILA-DISRUPTOR-II"], Sku = "FILA-DIS-WHT-38", ColorCode = "WHITE", ColorName = "White", SizeCode = "38", SizeLabel = "38", ListPrice = 2300000, SalePrice = 2150000, Status = "ACTIVE", CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { ProductId = productMap["FILA-DISRUPTOR-II"], Sku = "FILA-DIS-PNK-39", ColorCode = "PINK", ColorName = "Pink", SizeCode = "39", SizeLabel = "39", ListPrice = 2300000, SalePrice = 2190000, Status = "ACTIVE", CreatedAtUtc = now, UpdatedAtUtc = now },

            new() { ProductId = productMap["CONVERSE-CHUCK-70"], Sku = "CONV-C70-BLK-40", ColorCode = "BLACK", ColorName = "Black", SizeCode = "40", SizeLabel = "40", ListPrice = 2100000, SalePrice = 1990000, Status = "ACTIVE", CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { ProductId = productMap["CONVERSE-CHUCK-70"], Sku = "CONV-C70-CRM-41", ColorCode = "CREAM", ColorName = "Cream", SizeCode = "41", SizeLabel = "41", ListPrice = 2100000, SalePrice = 2020000, Status = "ACTIVE", CreatedAtUtc = now, UpdatedAtUtc = now },

            new() { ProductId = productMap["VANS-OLD-SKOOL"], Sku = "VANS-OS-BLK-40", ColorCode = "BLACK", ColorName = "Black", SizeCode = "40", SizeLabel = "40", ListPrice = 2000000, SalePrice = 1890000, Status = "ACTIVE", CreatedAtUtc = now, UpdatedAtUtc = now },
            new() { ProductId = productMap["VANS-OLD-SKOOL"], Sku = "VANS-OS-BLU-41", ColorCode = "BLUE", ColorName = "Blue", SizeCode = "41", SizeLabel = "41", ListPrice = 2000000, SalePrice = 1920000, Status = "ACTIVE", CreatedAtUtc = now, UpdatedAtUtc = now }
        };

        await context.ProductVariants.AddRangeAsync(variants);
        await context.SaveChangesAsync();
    }
}