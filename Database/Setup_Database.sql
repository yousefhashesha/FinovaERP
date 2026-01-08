-- FinovaERP Database Setup Script
-- This script creates the database and initial tables

USE master;
GO

-- Create database if not exists
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'FinovaERP')
BEGIN
    CREATE DATABASE FinovaERP;
    PRINT 'Database FinovaERP created successfully';
END
ELSE
BEGIN
    PRINT 'Database FinovaERP already exists';
END
GO

USE FinovaERP;
GO

-- Create tables
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Companies' AND xtype = 'U')
BEGIN
    CREATE TABLE Companies (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        TradeName NVARCHAR(200),
        TaxNumber NVARCHAR(50),
        Phone NVARCHAR(50),
        Email NVARCHAR(100),
        Website NVARCHAR(200),
        Address NVARCHAR(500),
        City NVARCHAR(100),
        State NVARCHAR(100),
        Country NVARCHAR(100),
        PostalCode NVARCHAR(20),
        LogoPath NVARCHAR(500),
        BaseCurrency NVARCHAR(10) DEFAULT 'USD',
        TimeZone NVARCHAR(100),
        FiscalYearStart DATE,
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE(),
        ModifiedDate DATETIME,
        CreatedBy NVARCHAR(100),
        ModifiedBy NVARCHAR(100)
    );
    PRINT 'Companies table created';
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Roles' AND xtype = 'U')
BEGIN
    CREATE TABLE Roles (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(50) NOT NULL UNIQUE,
        Description NVARCHAR(500),
        Level INT DEFAULT 1,
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE(),
        ModifiedDate DATETIME,
        CreatedBy NVARCHAR(100),
        ModifiedBy NVARCHAR(100)
    );
    PRINT 'Roles table created';
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Users' AND xtype = 'U')
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(50) NOT NULL UNIQUE,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(500) NOT NULL,
        FirstName NVARCHAR(50) NOT NULL,
        LastName NVARCHAR(50) NOT NULL,
        PhoneNumber NVARCHAR(50),
        LastLoginDate DATETIME,
        IsLocked BIT DEFAULT 0,
        FailedLoginAttempts INT DEFAULT 0,
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE(),
        ModifiedDate DATETIME,
        CreatedBy NVARCHAR(100),
        ModifiedBy NVARCHAR(100)
    );
    PRINT 'Users table created';
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'UserRoles' AND xtype = 'U')
BEGIN
    CREATE TABLE UserRoles (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        RoleId INT NOT NULL,
        CompanyId INT NOT NULL,
        AssignedDate DATETIME DEFAULT GETDATE(),
        AssignedBy NVARCHAR(100),
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE(),
        ModifiedDate DATETIME,
        CreatedBy NVARCHAR(100),
        ModifiedBy NVARCHAR(100),
        CONSTRAINT FK_UserRoles_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
        CONSTRAINT FK_UserRoles_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id),
        CONSTRAINT FK_UserRoles_Companies FOREIGN KEY (CompanyId) REFERENCES Companies(Id),
        CONSTRAINT UK_UserRoles_UserRoleCompany UNIQUE (UserId, RoleId, CompanyId)
    );
    PRINT 'UserRoles table created';
END

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'UserCompanies' AND xtype = 'U')
BEGIN
    CREATE TABLE UserCompanies (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        CompanyId INT NOT NULL,
        JoinedDate DATETIME DEFAULT GETDATE(),
        EmployeeId NVARCHAR(50),
        Department NVARCHAR(100),
        Position NVARCHAR(100),
        IsPrimary BIT DEFAULT 0,
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE(),
        ModifiedDate DATETIME,
        CreatedBy NVARCHAR(100),
        ModifiedBy NVARCHAR(100),
        CONSTRAINT FK_UserCompanies_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
        CONSTRAINT FK_UserCompanies_Companies FOREIGN KEY (CompanyId) REFERENCES Companies(Id),
        CONSTRAINT UK_UserCompanies_UserCompany UNIQUE (UserId, CompanyId)
    );
    PRINT 'UserCompanies table created';
END

-- Insert default data
IF NOT EXISTS (SELECT * FROM Roles WHERE Name = 'System Administrator')
BEGIN
    INSERT INTO Roles (Name, Description, Level) 
    VALUES ('System Administrator', 'Full system access with all permissions', 10);
    PRINT 'System Administrator role created';
END

IF NOT EXISTS (SELECT * FROM Roles WHERE Name = 'Company Admin')
BEGIN
    INSERT INTO Roles (Name, Description, Level) 
    VALUES ('Company Admin', 'Company administrator with management permissions', 8);
    PRINT 'Company Admin role created';
END

IF NOT EXISTS (SELECT * FROM Roles WHERE Name = 'User')
BEGIN
    INSERT INTO Roles (Name, Description, Level) 
    VALUES ('User', 'Standard user with basic permissions', 1);
    PRINT 'Standard User role created';
END

-- Create default company
IF NOT EXISTS (SELECT * FROM Companies WHERE Name = 'Default Company')
BEGIN
    INSERT INTO Companies (Name, TradeName, BaseCurrency, TimeZone, FiscalYearStart) 
    VALUES ('Default Company', 'Default Company', 'USD', 'UTC', '2024-01-01');
    PRINT 'Default company created';
END

PRINT 'Database setup completed successfully';
