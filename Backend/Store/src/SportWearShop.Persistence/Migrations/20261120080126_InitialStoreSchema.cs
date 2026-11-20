using Microsoft.EntityFrameworkCore.Migrations;

namespace SportWearShop.Persistence.Migrations;

public partial class InitialStoreSchema : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Existing Docker databases may already have this schema without an EF history table.
        // Keep the migration SQL immutable, including MySQL functional indexes.
        migrationBuilder.Sql(
            """
            CREATE TABLE IF NOT EXISTS Customer
            (
                CustomerId BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                IdentityIssuer VARCHAR(255) COLLATE utf8mb4_0900_bin NOT NULL,
                IdentitySubject VARCHAR(255) COLLATE utf8mb4_0900_bin NOT NULL,
                CreatedAtUtc DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                UNIQUE KEY UX_Customer_Identity (IdentityIssuer, IdentitySubject)
            ) ENGINE=InnoDB DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
            """,
            suppressTransaction: true);

        migrationBuilder.Sql(
            """
            CREATE TABLE IF NOT EXISTS Brand
            (
                BrandId               INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                BrandName             VARCHAR(100) NOT NULL,
                BrandCode             VARCHAR(50) NOT NULL,
                IsActive              BOOLEAN NOT NULL DEFAULT 1,
                CreatedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                UpdatedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                UNIQUE KEY UX_Brand_BrandCode (BrandCode),
                UNIQUE KEY UX_Brand_BrandName (BrandName)
            ) ENGINE=InnoDB DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
            """,
            suppressTransaction: true);

        migrationBuilder.Sql(
            """
            CREATE TABLE IF NOT EXISTS Category
            (
                CategoryId            INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                ParentCategoryId      INT NULL,
                CategoryName          VARCHAR(150) NOT NULL,
                CategoryCode          VARCHAR(50) NOT NULL,
                Description           VARCHAR(500) NULL,
                SortOrder             INT NOT NULL DEFAULT 0,
                IsActive              BOOLEAN NOT NULL DEFAULT 1,
                CreatedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                UpdatedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                CONSTRAINT FK_Category_ParentCategory
                    FOREIGN KEY (ParentCategoryId) REFERENCES Category(CategoryId),
                UNIQUE KEY UX_Category_CategoryCode (CategoryCode),
                KEY IX_Category_ParentCategoryId (ParentCategoryId)
            ) ENGINE=InnoDB DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
            """,
            suppressTransaction: true);

        migrationBuilder.Sql(
            """
            CREATE TABLE IF NOT EXISTS Product
            (
                ProductId             BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                BrandId               INT NOT NULL,
                CategoryId            INT NOT NULL,
                ProductName           VARCHAR(255) NOT NULL,
                ProductCode           VARCHAR(50) NOT NULL,
                Slug                  VARCHAR(255) NOT NULL,
                Description           LONGTEXT NULL,
                Gender                VARCHAR(20) NULL,
                BaseMaterial          VARCHAR(100) NULL,
                Status                VARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
                CreatedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                UpdatedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                CONSTRAINT FK_Product_Brand
                    FOREIGN KEY (BrandId) REFERENCES Brand(BrandId),
                CONSTRAINT FK_Product_Category
                    FOREIGN KEY (CategoryId) REFERENCES Category(CategoryId),
                CONSTRAINT CK_Product_Status
                    CHECK (Status IN ('DRAFT', 'ACTIVE', 'INACTIVE', 'DISCONTINUED')),
                CONSTRAINT CK_Product_Gender
                    CHECK (Gender IS NULL OR Gender IN ('MEN', 'WOMEN', 'UNISEX', 'KIDS')),
                UNIQUE KEY UX_Product_ProductCode (ProductCode),
                UNIQUE KEY UX_Product_Slug (Slug),
                KEY IX_Product_BrandId (BrandId),
                KEY IX_Product_CategoryId (CategoryId),
                KEY IX_Product_Status (Status)
            ) ENGINE=InnoDB DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
            """,
            suppressTransaction: true);

        migrationBuilder.Sql(
            """
            CREATE TABLE IF NOT EXISTS ProductVariant
            (
                ProductVariantId      BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                ProductId             BIGINT NOT NULL,
                Sku                   VARCHAR(80) NOT NULL,
                ColorCode             VARCHAR(50) NOT NULL,
                ColorName             VARCHAR(100) NOT NULL,
                SizeCode              VARCHAR(30) NOT NULL,
                SizeLabel             VARCHAR(30) NOT NULL,
                ListPrice             DECIMAL(18,2) NOT NULL,
                SalePrice             DECIMAL(18,2) NULL,
                WeightGrams           INT NULL,
                Status                VARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
                CreatedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                UpdatedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                UNIQUE KEY UX_ProductVariant_Product_Variant (ProductId, ProductVariantId),
                CONSTRAINT FK_ProductVariant_Product
                    FOREIGN KEY (ProductId) REFERENCES Product(ProductId),
                CONSTRAINT CK_ProductVariant_Status
                    CHECK (Status IN ('ACTIVE', 'INACTIVE')),
                CONSTRAINT CK_ProductVariant_ListPrice
                    CHECK (ListPrice >= 0),
                CONSTRAINT CK_ProductVariant_SalePrice
                    CHECK (SalePrice IS NULL OR SalePrice BETWEEN 0 AND ListPrice),
                CONSTRAINT CK_ProductVariant_WeightGrams
                    CHECK (WeightGrams IS NULL OR WeightGrams >= 0),
                UNIQUE KEY UX_ProductVariant_Sku (Sku),
                UNIQUE KEY UX_ProductVariant_Product_Color_Size (ProductId, ColorCode, SizeCode),
                KEY IX_ProductVariant_Status (Status)
            ) ENGINE=InnoDB DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
            """,
            suppressTransaction: true);

        migrationBuilder.Sql(
            """
            CREATE TABLE IF NOT EXISTS ProductImage
            (
                ProductImageId        BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                ProductId             BIGINT NOT NULL,
                ProductVariantId      BIGINT NULL,
                ImageUrl              VARCHAR(1000) NOT NULL,
                AltText               VARCHAR(255) NULL,
                SortOrder             INT NOT NULL DEFAULT 0,
                IsPrimary             BOOLEAN NOT NULL DEFAULT 0,
                CreatedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                CONSTRAINT FK_ProductImage_Product
                    FOREIGN KEY (ProductId) REFERENCES Product(ProductId),
                CONSTRAINT FK_ProductImage_ProductVariant
                    FOREIGN KEY (ProductId, ProductVariantId) REFERENCES ProductVariant(ProductId, ProductVariantId)
            ) ENGINE=InnoDB DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
            """,
            suppressTransaction: true);

        migrationBuilder.Sql(
            """
            CREATE TABLE IF NOT EXISTS UserAddress
            (
                UserAddressId         BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                CustomerId            BIGINT NOT NULL,
                RecipientName         VARCHAR(150) NOT NULL,
                PhoneNumber           VARCHAR(30) NOT NULL,
                AddressLine1          VARCHAR(255) NOT NULL,
                AddressLine2          VARCHAR(255) NULL,
                Ward                  VARCHAR(100) NULL,
                District              VARCHAR(100) NULL,
                City                  VARCHAR(100) NOT NULL,
                Province              VARCHAR(100) NULL,
                PostalCode            VARCHAR(20) NULL,
                CountryCode           VARCHAR(10) NOT NULL,
                IsDefaultShipping     BOOLEAN NOT NULL DEFAULT 0,
                IsDefaultBilling      BOOLEAN NOT NULL DEFAULT 0,
                CreatedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                UpdatedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                CONSTRAINT FK_UserAddress_Customer
                    FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId),
                KEY IX_UserAddress_CustomerId (CustomerId),
                UNIQUE KEY UX_UserAddress_DefaultShipping
                    ((CASE WHEN IsDefaultShipping = 1 THEN CustomerId ELSE NULL END)),
                UNIQUE KEY UX_UserAddress_DefaultBilling
                    ((CASE WHEN IsDefaultBilling = 1 THEN CustomerId ELSE NULL END)),
                CONSTRAINT CK_UserAddress_DefaultFlags
                    CHECK (IsDefaultShipping IN (0, 1) AND IsDefaultBilling IN (0, 1))
            ) ENGINE=InnoDB DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
            """,
            suppressTransaction: true);

        migrationBuilder.Sql(
            """
            CREATE TABLE IF NOT EXISTS ProductRating
            (
                ProductRatingId       BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                ProductId             BIGINT NOT NULL,
                CustomerId            BIGINT NOT NULL,
                RatingValue           INT NOT NULL,
                ReviewText            VARCHAR(1000) NULL,
                CreatedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                UpdatedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                CONSTRAINT FK_ProductRating_Product
                    FOREIGN KEY (ProductId) REFERENCES Product(ProductId),
                CONSTRAINT FK_ProductRating_Customer
                    FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId),
                CONSTRAINT CK_ProductRating_RatingValue
                    CHECK (RatingValue BETWEEN 1 AND 5),
                UNIQUE KEY UX_ProductRating_Product_Customer (ProductId, CustomerId)
            ) ENGINE=InnoDB DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
            """,
            suppressTransaction: true);

        migrationBuilder.Sql(
            """
            CREATE TABLE IF NOT EXISTS Cart
            (
                CartId                BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                CustomerId            BIGINT NOT NULL,
                CartStatus            VARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
                ActiveCustomerId      BIGINT GENERATED ALWAYS AS
                    (CASE WHEN CartStatus = 'ACTIVE' THEN CustomerId ELSE NULL END) STORED,
                UNIQUE KEY UX_Cart_ActiveCustomer (ActiveCustomerId),
                CreatedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                UpdatedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                CONSTRAINT FK_Cart_Customer
                    FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId),
                CONSTRAINT CK_Cart_CartStatus
                    CHECK (CartStatus IN ('ACTIVE', 'CHECKED_OUT', 'ABANDONED')),
                KEY IX_Cart_CustomerId (CustomerId)
            ) ENGINE=InnoDB DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
            """,
            suppressTransaction: true);

        migrationBuilder.Sql(
            """
            CREATE TABLE IF NOT EXISTS CartItem
            (
                CartItemId            BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                CartId                BIGINT NOT NULL,
                ProductVariantId      BIGINT NOT NULL,
                Quantity              INT NOT NULL,
                UnitPrice             DECIMAL(18,2) NOT NULL,
                CreatedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                UpdatedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                CONSTRAINT FK_CartItem_Cart
                    FOREIGN KEY (CartId) REFERENCES Cart(CartId) ON DELETE CASCADE,
                CONSTRAINT FK_CartItem_ProductVariant
                    FOREIGN KEY (ProductVariantId) REFERENCES ProductVariant(ProductVariantId),
                CONSTRAINT CK_CartItem_Quantity
                    CHECK (Quantity > 0),
                CONSTRAINT CK_CartItem_UnitPrice
                    CHECK (UnitPrice >= 0),
                UNIQUE KEY UX_CartItem_CartId_ProductVariantId (CartId, ProductVariantId)
            ) ENGINE=InnoDB DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
            """,
            suppressTransaction: true);

        migrationBuilder.Sql(
            """
            CREATE TABLE IF NOT EXISTS OrderHeader
            (
                OrderId               BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                OrderNumber           VARCHAR(30) NOT NULL,
                CustomerId            BIGINT NOT NULL,
                ShippingAddressSnapshot JSON NOT NULL,
                BillingAddressSnapshot  JSON NULL,
                OrderStatus           VARCHAR(20) NOT NULL DEFAULT 'PENDING',
                PaymentStatus         VARCHAR(20) NOT NULL DEFAULT 'UNPAID',
                SubtotalAmount        DECIMAL(18,2) NOT NULL,
                DiscountAmount        DECIMAL(18,2) NOT NULL DEFAULT 0,
                ShippingFee           DECIMAL(18,2) NOT NULL DEFAULT 0,
                TotalAmount           DECIMAL(18,2) NOT NULL,
                CurrencyCode          VARCHAR(10) NOT NULL DEFAULT 'VND',
                OrderedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                PaidAtUtc             DATETIME(6) NULL,
                CancelledAtUtc        DATETIME(6) NULL,
                CreatedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                UpdatedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                CONSTRAINT FK_OrderHeader_Customer
                    FOREIGN KEY (CustomerId) REFERENCES Customer(CustomerId),
                CONSTRAINT CK_OrderHeader_OrderStatus
                    CHECK (OrderStatus IN ('PENDING', 'CONFIRMED', 'PACKING', 'SHIPPED', 'DELIVERED', 'CANCELLED', 'RETURNED')),
                CONSTRAINT CK_OrderHeader_PaymentStatus
                    CHECK (PaymentStatus IN ('UNPAID', 'PAID', 'FAILED', 'REFUNDED', 'PARTIALLY_REFUNDED')),
                CONSTRAINT CK_OrderHeader_SubtotalAmount CHECK (SubtotalAmount >= 0),
                CONSTRAINT CK_OrderHeader_DiscountAmount CHECK (DiscountAmount >= 0),
                CONSTRAINT CK_OrderHeader_ShippingFee CHECK (ShippingFee >= 0),
                CONSTRAINT CK_OrderHeader_TotalAmount CHECK (DiscountAmount <= SubtotalAmount AND TotalAmount = SubtotalAmount - DiscountAmount + ShippingFee),
                UNIQUE KEY UX_OrderHeader_OrderNumber (OrderNumber),
                KEY IX_OrderHeader_Customer_OrderedAt (CustomerId, OrderedAtUtc, OrderId),
                KEY IX_OrderHeader_OrderedAtUtc (OrderedAtUtc),
                KEY IX_OrderHeader_OrderStatus (OrderStatus)
            ) ENGINE=InnoDB DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
            """,
            suppressTransaction: true);

        migrationBuilder.Sql(
            """
            CREATE TABLE IF NOT EXISTS OrderItem
            (
                OrderItemId           BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                OrderId               BIGINT NOT NULL,
                ProductVariantId      BIGINT NULL,
                SkuSnapshot           VARCHAR(80) NOT NULL,
                ProductNameSnapshot   VARCHAR(255) NOT NULL,
                ColorSnapshot         VARCHAR(100) NOT NULL,
                SizeSnapshot          VARCHAR(30) NOT NULL,
                ImageUrlSnapshot      VARCHAR(1000) NULL,
                Quantity              INT NOT NULL,
                UnitPrice             DECIMAL(18,2) NOT NULL,
                LineDiscountAmount    DECIMAL(18,2) NOT NULL DEFAULT 0,
                LineTotalAmount       DECIMAL(18,2) NOT NULL,
                CreatedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                CONSTRAINT FK_OrderItem_OrderHeader
                    FOREIGN KEY (OrderId) REFERENCES OrderHeader(OrderId),
                CONSTRAINT FK_OrderItem_ProductVariant
                    FOREIGN KEY (ProductVariantId) REFERENCES ProductVariant(ProductVariantId) ON DELETE SET NULL,
                CONSTRAINT CK_OrderItem_Quantity CHECK (Quantity > 0),
                CONSTRAINT CK_OrderItem_UnitPrice CHECK (UnitPrice >= 0),
                CONSTRAINT CK_OrderItem_LineDiscountAmount CHECK (LineDiscountAmount >= 0),
                CONSTRAINT CK_OrderItem_LineTotalAmount CHECK (LineDiscountAmount <= UnitPrice * Quantity
                        AND LineTotalAmount = UnitPrice * Quantity - LineDiscountAmount),
                KEY IX_OrderItem_OrderId (OrderId),
                KEY IX_OrderItem_ProductVariantId (ProductVariantId)
            ) ENGINE=InnoDB DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
            """,
            suppressTransaction: true);

        migrationBuilder.Sql(
            """
            CREATE TABLE IF NOT EXISTS PaymentTransaction
            (
                PaymentTransactionId  BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                OrderId               BIGINT NOT NULL,
                PaymentMethod         VARCHAR(30) NOT NULL,
                GatewayTransactionRef VARCHAR(100) NULL,
                Amount                DECIMAL(18,2) NOT NULL,
                TransactionStatus     VARCHAR(20) NOT NULL,
                PaidAtUtc             DATETIME(6) NULL,
                FailureReason         VARCHAR(500) NULL,
                CreatedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                UpdatedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                CONSTRAINT FK_PaymentTransaction_OrderHeader
                    FOREIGN KEY (OrderId) REFERENCES OrderHeader(OrderId),
                CONSTRAINT CK_PaymentTransaction_PaymentMethod
                    CHECK (PaymentMethod IN ('COD', 'MOMO', 'VNPAY', 'CREDIT_CARD', 'BANK_TRANSFER')),
                CONSTRAINT CK_PaymentTransaction_TransactionStatus
                    CHECK (TransactionStatus IN ('PENDING', 'SUCCESS', 'FAILED', 'CANCELLED', 'REFUNDED')),
                CONSTRAINT CK_PaymentTransaction_Amount
                    CHECK (Amount >= 0),
                KEY IX_PaymentTransaction_OrderId (OrderId),
                UNIQUE KEY UX_PaymentTransaction_Method_Ref (PaymentMethod, GatewayTransactionRef)
            ) ENGINE=InnoDB DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
            """,
            suppressTransaction: true);

        migrationBuilder.Sql(
            """
            CREATE TABLE IF NOT EXISTS InventoryStock
            (
                ProductVariantId      BIGINT PRIMARY KEY,
                QuantityOnHand        INT NOT NULL DEFAULT 0,
                QuantityReserved      INT NOT NULL DEFAULT 0,
                UpdatedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                CONSTRAINT FK_InventoryStock_ProductVariant
                    FOREIGN KEY (ProductVariantId) REFERENCES ProductVariant(ProductVariantId),
                CONSTRAINT CK_InventoryStock_QuantityOnHand CHECK (QuantityOnHand >= 0),
                CONSTRAINT CK_InventoryStock_QuantityReserved CHECK (QuantityReserved >= 0),
                CONSTRAINT CK_InventoryStock_Reserved_Not_Greater_Than_OnHand
                    CHECK (QuantityReserved <= QuantityOnHand),
                KEY IX_InventoryStock_UpdatedAtUtc (UpdatedAtUtc)
            ) ENGINE=InnoDB DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
            """,
            suppressTransaction: true);

        migrationBuilder.Sql(
            """
            CREATE TABLE IF NOT EXISTS InventoryMovement
            (
                InventoryMovementId   BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                ProductVariantId      BIGINT NOT NULL,
                MovementType          VARCHAR(30) NOT NULL,
                Quantity              INT NOT NULL,
                ReferenceType         VARCHAR(30) NULL,
                ReferenceId           BIGINT NULL,
                Note                  VARCHAR(500) NULL,
                CreatedAtUtc          DATETIME(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
                CONSTRAINT FK_InventoryMovement_ProductVariant
                    FOREIGN KEY (ProductVariantId) REFERENCES ProductVariant(ProductVariantId),
                CONSTRAINT CK_InventoryMovement_MovementType
                    CHECK (MovementType IN ('IN', 'OUT', 'RESERVE', 'RELEASE', 'ADJUSTMENT')),
                CONSTRAINT CK_InventoryMovement_Quantity
                    CHECK ((MovementType = 'ADJUSTMENT' AND Quantity <> 0)
                        OR (MovementType <> 'ADJUSTMENT' AND Quantity > 0)),
                CONSTRAINT CK_InventoryMovement_ReferenceType
                    CHECK (ReferenceType IS NULL OR ReferenceType IN ('ORDER', 'PURCHASE', 'RETURN', 'MANUAL')),
                KEY IX_InventoryMovement_ProductVariantId (ProductVariantId),
                KEY IX_InventoryMovement_ReferenceType_ReferenceId (ReferenceType, ReferenceId),
                KEY IX_InventoryMovement_CreatedAtUtc (CreatedAtUtc)
            ) ENGINE=InnoDB DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
            """,
            suppressTransaction: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
        => throw new NotSupportedException("The Store baseline may contain tables created before EF migrations.");
}
