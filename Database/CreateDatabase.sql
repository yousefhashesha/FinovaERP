-- FinovaERP Database Creation Script
-- Creates complete database schema with all tables and relationships
USE [master];
GO
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'FinovaERP')
BEGIN
ALTER DATABASE [FinovaERP] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE [FinovaERP];
END
GO
CREATE DATABASE [FinovaERP];
GO
USE [FinovaERP];
GO
-- Companies Table
CREATE TABLE [Companies] (
[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
[Name] NVARCHAR(200) NOT NULL,
[Code] NVARCHAR(50) NOT NULL,
[Address] NVARCHAR(500),
[Phone] NVARCHAR(50),
[Email] NVARCHAR(200),
[Website] NVARCHAR(200),
[TaxNumber] NVARCHAR(50),
[Logo] NVARCHAR(500),
[CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
[ModifiedDate] DATETIME2,
[CreatedBy] NVARCHAR(100) NOT NULL,
[ModifiedBy] NVARCHAR(100),
[IsActive] BIT NOT NULL DEFAULT 1,
CONSTRAINT [UK_Companies_Code] UNIQUE ([Code])
);
-- Users Table
CREATE TABLE [Users] (
[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
[Username] NVARCHAR(50) NOT NULL,
[Password] NVARCHAR(255) NOT NULL,
[Email] NVARCHAR(200) NOT NULL,
[FirstName] NVARCHAR(100),
[LastName] NVARCHAR(100),
[Phone] NVARCHAR(50),
[ProfilePicture] NVARCHAR(500),
[CompanyId] INT NOT NULL,
[LastLoginDate] DATETIME2,
[IsLocked] BIT NOT NULL DEFAULT 0,
[LockoutEndDate] DATETIME2,
[FailedLoginAttempts] INT NOT NULL DEFAULT 0,
[CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
[ModifiedDate] DATETIME2,
[CreatedBy] NVARCHAR(100) NOT NULL,
[ModifiedBy] NVARCHAR(100),
[IsActive] BIT NOT NULL DEFAULT 1,
CONSTRAINT [UK_Users_Username] UNIQUE ([Username]),
CONSTRAINT [UK_Users_Email] UNIQUE ([Email]),
CONSTRAINT [FK_Users_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [Companies]([Id])
);
-- Roles Table
CREATE TABLE [Roles] (
[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
[Name] NVARCHAR(50) NOT NULL,
[Description] NVARCHAR(500),
[CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
[ModifiedDate] DATETIME2,
[CreatedBy] NVARCHAR(100) NOT NULL,
[ModifiedBy] NVARCHAR(100),
[IsActive] BIT NOT NULL DEFAULT 1,
CONSTRAINT [UK_Roles_Name] UNIQUE ([Name])
);
-- UserRoles Table (Many-to-Many)
CREATE TABLE [UserRoles] (
[UserId] INT NOT NULL,
[RoleId] INT NOT NULL,
[AssignedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
[AssignedBy] NVARCHAR(100) NOT NULL,
CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId]),
CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE,
CONSTRAINT [FK_UserRoles_Roles] FOREIGN KEY ([RoleId]) REFERENCES [Roles]([Id]) ON DELETE CASCADE
);
-- Currencies Table
CREATE TABLE [Currencies] (
[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
[Code] NVARCHAR(3) NOT NULL,
[Name] NVARCHAR(100) NOT NULL,
[Symbol] NVARCHAR(10),
[ExchangeRate] DECIMAL(18,6) NOT NULL DEFAULT 1,
[IsBaseCurrency] BIT NOT NULL DEFAULT 0,
[IsActive] BIT NOT NULL DEFAULT 1,
[CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
[ModifiedDate] DATETIME2,
[CreatedBy] NVARCHAR(100) NOT NULL,
[ModifiedBy] NVARCHAR(100),
CONSTRAINT [UK_Currencies_Code] UNIQUE ([Code])
);
-- NumberSequences Table
CREATE TABLE [NumberSequences] (
[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
[Code] NVARCHAR(20) NOT NULL,
[Description] NVARCHAR(200),
[Prefix] NVARCHAR(10),
[Suffix] NVARCHAR(10),
[LastNumber] INT NOT NULL DEFAULT 0,
[MinDigits] INT NOT NULL DEFAULT 4,
[IsActive] BIT NOT NULL DEFAULT 1,
[CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
[ModifiedDate] DATETIME2,
[CreatedBy] NVARCHAR(100) NOT NULL,
[ModifiedBy] NVARCHAR(100),
CONSTRAINT [UK_NumberSequences_Code] UNIQUE ([Code])
);
-- ItemCategories Table
CREATE TABLE [ItemCategories] (
[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
[Name] NVARCHAR(100) NOT NULL,
[Description] NVARCHAR(500),
[ParentId] INT,
[IsActive] BIT NOT NULL DEFAULT 1,
[CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
[ModifiedDate] DATETIME2,
[CreatedBy] NVARCHAR(100) NOT NULL,
[ModifiedBy] NVARCHAR(100),
CONSTRAINT [UK_ItemCategories_Name] UNIQUE ([Name]),
CONSTRAINT [FK_ItemCategories_Parent] FOREIGN KEY ([ParentId]) REFERENCES [ItemCategories]([Id])
);
-- Items Table
CREATE TABLE [Items] (
[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
[Code] NVARCHAR(50) NOT NULL,
[Name] NVARCHAR(200) NOT NULL,
[Description] NVARCHAR(1000),
[CategoryId] INT NOT NULL,
[Unit] NVARCHAR(20),
[CostPrice] DECIMAL(18,2) NOT NULL DEFAULT 0,
[SellingPrice] DECIMAL(18,2) NOT NULL DEFAULT 0,
[MinimumStock] INT NOT NULL DEFAULT 0,
[CurrentStock] INT NOT NULL DEFAULT 0,
[Barcode] NVARCHAR(50),
[ImagePath] NVARCHAR(500),
[IsActive] BIT NOT NULL DEFAULT 1,
[CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
[ModifiedDate] DATETIME2,
[CreatedBy] NVARCHAR(100) NOT NULL,
[ModifiedBy] NVARCHAR(100),
CONSTRAINT [UK_Items_Code] UNIQUE ([Code]),
CONSTRAINT [FK_Items_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [ItemCategories]([Id])
);
-- Customers Table
CREATE TABLE [Customers] (
[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
[Code] NVARCHAR(50) NOT NULL,
[Name] NVARCHAR(200) NOT NULL,
[ContactPerson] NVARCHAR(200),
[Email] NVARCHAR(200),
[Phone] NVARCHAR(50),
[Mobile] NVARCHAR(50),
[Address] NVARCHAR(500),
[City] NVARCHAR(100),
[State] NVARCHAR(100),
[Country] NVARCHAR(100),
[PostalCode] NVARCHAR(20),
[TaxNumber] NVARCHAR(50),
[CreditLimit] DECIMAL(18,2) NOT NULL DEFAULT 0,
[OutstandingBalance] DECIMAL(18,2) NOT NULL DEFAULT 0,
[PaymentTerms] INT NOT NULL DEFAULT 30,
[IsActive] BIT NOT NULL DEFAULT 1,
[CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
[ModifiedDate] DATETIME2,
[CreatedBy] NVARCHAR(100) NOT NULL,
[ModifiedBy] NVARCHAR(100),
CONSTRAINT [UK_Customers_Code] UNIQUE ([Code]),
CONSTRAINT [UK_Customers_Email] UNIQUE ([Email])
);
-- Suppliers Table
CREATE TABLE [Suppliers] (
[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
[Code] NVARCHAR(50) NOT NULL,
[Name] NVARCHAR(200) NOT NULL,
[ContactPerson] NVARCHAR(200),
[Email] NVARCHAR(200),
[Phone] NVARCHAR(50),
[Mobile] NVARCHAR(50),
[Address] NVARCHAR(500),
[City] NVARCHAR(100),
[State] NVARCHAR(100),
[Country] NVARCHAR(100),
[PostalCode] NVARCHAR(20),
[TaxNumber] NVARCHAR(50),
[PaymentTerms] INT NOT NULL DEFAULT 30,
[IsActive] BIT NOT NULL DEFAULT 1,
[CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
[ModifiedDate] DATETIME2,
[CreatedBy] NVARCHAR(100) NOT NULL,
[ModifiedBy] NVARCHAR(100),
CONSTRAINT [UK_Suppliers_Code] UNIQUE ([Code]),
CONSTRAINT [UK_Suppliers_Email] UNIQUE ([Email])
);
-- SalesOrders Table
CREATE TABLE [SalesOrders] (
[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
[OrderNumber] NVARCHAR(50) NOT NULL,
[OrderDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
[CustomerId] INT NOT NULL,
[CurrencyId] INT NOT NULL,
[SubTotal] DECIMAL(18,2) NOT NULL DEFAULT 0,
[TaxAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
[TotalAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
[Status] NVARCHAR(20) NOT NULL DEFAULT 'Draft',
[Notes] NVARCHAR(1000),
[CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
[ModifiedDate] DATETIME2,
[CreatedBy] NVARCHAR(100) NOT NULL,
[ModifiedBy] NVARCHAR(100),
CONSTRAINT [UK_SalesOrders_OrderNumber] UNIQUE ([OrderNumber]),
CONSTRAINT [FK_SalesOrders_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [Customers]([Id]),
CONSTRAINT [FK_SalesOrders_Currencies] FOREIGN KEY ([CurrencyId]) REFERENCES [Currencies]([Id])
);
-- SalesOrderItems Table
CREATE TABLE [SalesOrderItems] (
[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
[SalesOrderId] INT NOT NULL,
[ItemId] INT NOT NULL,
[Quantity] INT NOT NULL,
[UnitPrice] DECIMAL(18,2) NOT NULL,
[TotalPrice] DECIMAL(18,2) NOT NULL,
[DiscountPercent] DECIMAL(5,2) NOT NULL DEFAULT 0,
[DiscountAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
[TaxPercent] DECIMAL(5,2) NOT NULL DEFAULT 0,
[TaxAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
[NetAmount] DECIMAL(18,2) NOT NULL,
[CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
[CreatedBy] NVARCHAR(100) NOT NULL,
CONSTRAINT [FK_SalesOrderItems_SalesOrders] FOREIGN KEY ([SalesOrderId]) REFERENCES [SalesOrders]([Id]) ON DELETE CASCADE,
CONSTRAINT [FK_SalesOrderItems_Items] FOREIGN KEY ([ItemId]) REFERENCES [Items]([Id])
);
-- PurchaseOrders Table
CREATE TABLE [PurchaseOrders] (
[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
[OrderNumber] NVARCHAR(50) NOT NULL,
[OrderDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
[SupplierId] INT NOT NULL,
[CurrencyId] INT NOT NULL,
[SubTotal] DECIMAL(18,2) NOT NULL DEFAULT 0,
[TaxAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
[TotalAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
[Status] NVARCHAR(20) NOT NULL DEFAULT 'Draft',
[Notes] NVARCHAR(1000),
[CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
[ModifiedDate] DATETIME2,
[CreatedBy] NVARCHAR(100) NOT NULL,
[ModifiedBy] NVARCHAR(100),
CONSTRAINT [UK_PurchaseOrders_OrderNumber] UNIQUE ([OrderNumber]),
CONSTRAINT [FK_PurchaseOrders_Suppliers] FOREIGN KEY ([SupplierId]) REFERENCES [Suppliers]([Id]),
CONSTRAINT [FK_PurchaseOrders_Currencies] FOREIGN KEY ([CurrencyId]) REFERENCES [Currencies]([Id])
);
-- PurchaseOrderItems Table
CREATE TABLE [PurchaseOrderItems] (
[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
[PurchaseOrderId] INT NOT NULL,
[ItemId] INT NOT NULL,
[Quantity] INT NOT NULL,
[UnitPrice] DECIMAL(18,2) NOT NULL,
[TotalPrice] DECIMAL(18,2) NOT NULL,
[CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
[CreatedBy] NVARCHAR(100) NOT NULL,
CONSTRAINT [FK_PurchaseOrderItems_PurchaseOrders] FOREIGN KEY ([PurchaseOrderId]) REFERENCES [PurchaseOrders]([Id]) ON DELETE CASCADE,
CONSTRAINT [FK_PurchaseOrderItems_Items] FOREIGN KEY ([ItemId]) REFERENCES [Items]([Id])
);
-- InventoryTransactions Table
CREATE TABLE [InventoryTransactions] (
[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
[ItemId] INT NOT NULL,
[TransactionType] NVARCHAR(20) NOT NULL,
[ReferenceId] INT,
[ReferenceType] NVARCHAR(50),
[Quantity] INT NOT NULL,
[UnitCost] DECIMAL(18,2) NOT NULL DEFAULT 0,
[TotalCost] DECIMAL(18,2) NOT NULL DEFAULT 0,
[TransactionDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
[Notes] NVARCHAR(500),
[CreatedBy] NVARCHAR(100) NOT NULL,
CONSTRAINT [FK_InventoryTransactions_Items] FOREIGN KEY ([ItemId]) REFERENCES [Items]([Id])
);
-- Create Indexes for better performance
CREATE INDEX [IX_Users_CompanyId] ON [Users] ([CompanyId]);
CREATE INDEX [IX_Users_Username] ON [Users] ([Username]);
CREATE INDEX [IX_UserRoles_RoleId] ON [UserRoles] ([RoleId]);
CREATE INDEX [IX_Items_CategoryId] ON [Items] ([CategoryId]);
CREATE INDEX [IX_Items_Code] ON [Items] ([Code]);
CREATE INDEX [IX_SalesOrders_CustomerId] ON [SalesOrders] ([CustomerId]);
CREATE INDEX [IX_SalesOrders_OrderDate] ON [SalesOrders] ([OrderDate]);
CREATE INDEX [IX_SalesOrderItems_SalesOrderId] ON [SalesOrderItems] ([SalesOrderId]);
CREATE INDEX [IX_SalesOrderItems_ItemId] ON [SalesOrderItems] ([ItemId]);
CREATE INDEX [IX_PurchaseOrders_SupplierId] ON [PurchaseOrders] ([SupplierId]);
CREATE INDEX [IX_PurchaseOrders_OrderDate] ON [PurchaseOrders] ([OrderDate]);
CREATE INDEX [IX_InventoryTransactions_ItemId] ON [InventoryTransactions] ([ItemId]);
CREATE INDEX [IX_InventoryTransactions_TransactionDate] ON [InventoryTransactions] ([TransactionDate]);
-- Insert initial data
INSERT INTO [Companies] ([Name], [Code], [Address], [Phone], [Email], [CreatedBy])
VALUES ('Main Company', 'MAIN', 'Main Street 123', '+1234567890', 'info@maincompany.com', 'System');
INSERT INTO [Users] ([Username], [Password], [Email], [FirstName], [LastName], [CompanyId], [CreatedBy])
VALUES ('admin', 'admin123', 'admin@finovaerp.com', 'System', 'Administrator', 1, 'System');
INSERT INTO [Roles] ([Name], [Description], [CreatedBy])
VALUES
('Administrator', 'System Administrator', 'System'),
('Manager', 'Department Manager', 'System'),
('User', 'Regular User', 'System');
INSERT INTO [UserRoles] ([UserId], [RoleId], [AssignedBy])
VALUES (1, 1, 'System');
INSERT INTO [Currencies] ([Code], [Name], [Symbol], [ExchangeRate], [CreatedBy])
VALUES
('USD', 'US Dollar', '$', 1.0, 'System'),
('EUR', 'Euro', '€', 0.85, 'System');
INSERT INTO [NumberSequences] ([Code], [Description], [Prefix], [LastNumber], [MinDigits], [CreatedBy])
VALUES
('ITEM', 'Item Numbers', 'ITM', 1000, 4, 'System'),
('CUST', 'Customer Numbers', 'CUST', 1000, 4, 'System'),
('SUPP', 'Supplier Numbers', 'SUPP', 1000, 4, 'System');
INSERT INTO [ItemCategories] ([Name], [Description], [CreatedBy])
VALUES
('Electronics', 'Electronic devices and accessories', 'System'),
('Clothing', 'Clothing and apparel', 'System'),
('Food', 'Food and beverages', 'System');
PRINT 'Database FinovaERP created successfully with all tables and relationships!';
