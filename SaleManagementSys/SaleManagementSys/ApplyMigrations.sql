-- Run this script against your database (same as in appsettings.json ConnectionStrings:DefaultConnection)
-- to create Products table and apply migrations. Run in SSMS or: sqlcmd -S your_server -d your_db -i ApplyMigrations.sql

SET NOCOUNT ON;

-- 1) Create Products table if missing
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
    CREATE TABLE [Products] (
        [Id] int NOT NULL IDENTITY(1,1),
        [Name] nvarchar(200) NOT NULL,
        [DefaultPurchasePrice] decimal(18,2) NOT NULL,
        [Description] nvarchar(1000) NULL,
        CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
    );

    INSERT INTO Products (Name, DefaultPurchasePrice, Description) VALUES (N'Default', 0, NULL);
    PRINT 'Created Products table and Default product.';
END

-- 2) Add ProductId to SaleDetails and migrate from ProductName if needed
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SaleDetails') AND name = 'ProductId')
BEGIN
    ALTER TABLE [SaleDetails] ADD [ProductId] int NULL;
    UPDATE SaleDetails SET ProductId = 1 WHERE ProductId IS NULL;
    ALTER TABLE [SaleDetails] ALTER COLUMN [ProductId] int NOT NULL;

    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('SaleDetails') AND name = 'ProductName')
        ALTER TABLE [SaleDetails] DROP COLUMN [ProductName];

    CREATE INDEX [IX_SaleDetails_ProductId] ON [SaleDetails] ([ProductId]);
    ALTER TABLE [SaleDetails] ADD CONSTRAINT [FK_SaleDetails_Products_ProductId]
        FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION;
    PRINT 'Added ProductId to SaleDetails and dropped ProductName.';
END

-- 3) Seed baby suit products (skip if we already have more than the Default product)
IF (SELECT COUNT(*) FROM Products) <= 1
BEGIN
    INSERT INTO Products (Name, DefaultPurchasePrice, Description) VALUES
    (N'Baby Romper Suit', 8.99, N'Cotton romper suit for 0-12 months'),
    (N'Newborn Sleepsuit', 12.50, N'Soft sleepsuit with feet, 0-3 months'),
    (N'Toddler Jumpsuit', 14.99, N'Comfy jumpsuit for 1-2 years'),
    (N'Baby Bodysuit 3-Pack', 18.00, N'Pack of 3 short-sleeve bodysuits'),
    (N'Winter Baby Suit', 22.00, N'Fleece-lined suit for cold weather'),
    (N'Baby Formal Suit', 35.00, N'Dress-up suit for special occasions'),
    (N'Organic Cotton Baby Suit', 19.99, N'100% organic cotton, 0-6 months'),
    (N'Baby Pajama Suit', 15.50, N'Long-sleeve pajama with snap closure');
    PRINT 'Inserted baby suit products.';
END

-- 4) Remove Email from Sales if present
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Sales') AND name = 'Email')
BEGIN
    ALTER TABLE [Sales] DROP COLUMN [Email];
    PRINT 'Dropped Email from Sales.';
END

-- 5) Record migrations so EF Core considers them applied
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260212114625_InitialCreate')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260212114625_InitialCreate', N'8.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260213100000_AddProductAndRefactorSaleDetail')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260213100000_AddProductAndRefactorSaleDetail', N'8.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260213110000_SeedBabySuitProducts')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260213110000_SeedBabySuitProducts', N'8.0.0');
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260213120000_RemoveEmailFromSale')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260213120000_RemoveEmailFromSale', N'8.0.0');

PRINT 'Done. Database is up to date.';
