CREATE TABLE Currencies (
CurrencyID INT PRIMARY KEY IDENTITY(1,1),
CurrencyCode NVARCHAR(10) UNIQUE NOT NULL,
CurrencyNameAr NVARCHAR(100) NOT NULL,
CurrencyNameEn NVARCHAR(100),
Symbol NVARCHAR(10),
SubUnitNameAr NVARCHAR(50),
SubUnitNameEn NVARCHAR(50),
DecimalPlaces INT DEFAULT 2,
IsActive BIT DEFAULT 1,
CreatedDate DATETIME DEFAULT GETDATE()
);
CREATE TABLE Users (
UserID INT PRIMARY KEY IDENTITY(1,1),
Username NVARCHAR(50) UNIQUE NOT NULL,
PasswordHash NVARCHAR(255) NOT NULL,
FullNameAr NVARCHAR(200) NOT NULL,
FullNameEn NVARCHAR(200),
Email NVARCHAR(100),
Phone NVARCHAR(50),
IsActive BIT DEFAULT 1,
LastLogin DATETIME,
CreatedDate DATETIME DEFAULT GETDATE(),
ModifiedDate DATETIME
);
CREATE TABLE Roles (
RoleID INT PRIMARY KEY IDENTITY(1,1),
RoleCode NVARCHAR(50) UNIQUE NOT NULL,
RoleNameAr NVARCHAR(100) NOT NULL,
RoleNameEn NVARCHAR(100),
Description NVARCHAR(500),
IsActive BIT DEFAULT 1,
CreatedDate DATETIME DEFAULT GETDATE()
);
CREATE TABLE Forms (
FormID INT PRIMARY KEY IDENTITY(1,1),
FormCode NVARCHAR(50) UNIQUE NOT NULL,
FormNameAr NVARCHAR(100) NOT NULL,
FormNameEn NVARCHAR(100),
ModuleName NVARCHAR(50),
Description NVARCHAR(500),
IsActive BIT DEFAULT 1
);
CREATE TABLE FormActions (
ActionID INT PRIMARY KEY IDENTITY(1,1),
FormID INT NOT NULL,
ActionCode NVARCHAR(50) NOT NULL,
ActionNameAr NVARCHAR(100) NOT NULL,
ActionNameEn NVARCHAR(100),
ActionType NVARCHAR(20),
IsActive BIT DEFAULT 1,
CONSTRAINT FK_FormAction_Form FOREIGN KEY (FormID) REFERENCES Forms(FormID)
);
CREATE TABLE CostCenters (
CostCenterID INT PRIMARY KEY IDENTITY(1,1),
CostCenterCode NVARCHAR(50) NOT NULL,
CostCenterNameAr NVARCHAR(200) NOT NULL,
CostCenterNameEn NVARCHAR(200),
ParentCostCenterID INT NULL,
CostCenterLevel INT DEFAULT 1,
CostCenterType NVARCHAR(50),
Description NVARCHAR(500),
IsActive BIT DEFAULT 1,
CreatedBy INT,
CreatedDate DATETIME DEFAULT GETDATE(),
ModifiedBy INT,
ModifiedDate DATETIME,
CONSTRAINT FK_CostCenter_Parent FOREIGN KEY (ParentCostCenterID) REFERENCES CostCenters(CostCenterID)
);
ALTER TABLE CostCenters ADD CONSTRAINT FK_CostCenter_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
ALTER TABLE CostCenters ADD CONSTRAINT FK_CostCenter_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserID);
CREATE TABLE Companies (
CompanyID INT PRIMARY KEY IDENTITY(1,1),
CompanyCode NVARCHAR(20) UNIQUE NOT NULL,
CompanyNameAr NVARCHAR(200) NOT NULL,
CompanyNameEn NVARCHAR(200),
TaxNumber NVARCHAR(50),
CommercialRegister NVARCHAR(50),
Address NVARCHAR(500),
Phone NVARCHAR(50),
Email NVARCHAR(100),
Logo VARBINARY(MAX),
BaseCurrencyID INT,
FiscalYearStart DATE,
FiscalYearEnd DATE,
IsActive BIT DEFAULT 1,
CreatedBy INT,
CreatedDate DATETIME DEFAULT GETDATE(),
ModifiedBy INT,
ModifiedDate DATETIME,
CONSTRAINT FK_Company_Currency FOREIGN KEY (BaseCurrencyID) REFERENCES Currencies(CurrencyID)
);
ALTER TABLE Companies ADD CONSTRAINT FK_Company_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
ALTER TABLE Companies ADD CONSTRAINT FK_Company_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserID);
CREATE TABLE UserCompanyPermissions (
PermissionID INT PRIMARY KEY IDENTITY(1,1),
UserID INT NOT NULL,
CompanyID INT NOT NULL,
CanView BIT DEFAULT 0,
CanAdd BIT DEFAULT 0,
CanEdit BIT DEFAULT 0,
CanDelete BIT DEFAULT 0,
CanApprove BIT DEFAULT 0,
CanPost BIT DEFAULT 0,
IsActive BIT DEFAULT 1,
CreatedBy INT,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_UserCompPerm_User FOREIGN KEY (UserID) REFERENCES Users(UserID),
CONSTRAINT FK_UserCompPerm_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID)
);
ALTER TABLE UserCompanyPermissions ADD CONSTRAINT FK_UserCompPerm_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
CREATE TABLE UserRoles (
UserRoleID INT PRIMARY KEY IDENTITY(1,1),
UserID INT NOT NULL,
RoleID INT NOT NULL,
CompanyID INT NOT NULL,
IsActive BIT DEFAULT 1,
AssignedBy INT,
AssignedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_UserRole_User FOREIGN KEY (UserID) REFERENCES Users(UserID),
CONSTRAINT FK_UserRole_Role FOREIGN KEY (RoleID) REFERENCES Roles(RoleID),
CONSTRAINT FK_UserRole_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID)
);
ALTER TABLE UserRoles ADD CONSTRAINT FK_UserRole_AssignedBy FOREIGN KEY (AssignedBy) REFERENCES Users(UserID);
CREATE TABLE RoleFormPermissions (
PermissionID INT PRIMARY KEY IDENTITY(1,1),
RoleID INT NOT NULL,
FormID INT NOT NULL,
ActionID INT NULL,
CompanyID INT NOT NULL,
HasPermission BIT DEFAULT 0,
CreatedBy INT,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_RoleFormPerm_Role FOREIGN KEY (RoleID) REFERENCES Roles(RoleID),
CONSTRAINT FK_RoleFormPerm_Form FOREIGN KEY (FormID) REFERENCES Forms(FormID),
CONSTRAINT FK_RoleFormPerm_Action FOREIGN KEY (ActionID) REFERENCES FormActions(ActionID),
CONSTRAINT FK_RoleFormPerm_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID)
);
ALTER TABLE RoleFormPermissions ADD CONSTRAINT FK_RoleFormPerm_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
CREATE TABLE CompanyCostCenters (
CompanyCostCenterID INT PRIMARY KEY IDENTITY(1,1),
CompanyID INT NOT NULL,
CostCenterID INT NOT NULL,
IsDefault BIT DEFAULT 0,
IsActive BIT DEFAULT 1,
ActivationDate DATE,
DeactivationDate DATE,
CreatedBy INT,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_CompCostCenter_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_CompCostCenter_CostCenter FOREIGN KEY (CostCenterID) REFERENCES CostCenters(CostCenterID),
CONSTRAINT UQ_CompanyCostCenter UNIQUE(CompanyID, CostCenterID)
);
ALTER TABLE CompanyCostCenters ADD CONSTRAINT FK_CompCostCenter_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
CREATE TABLE ChartOfAccounts (
AccountID INT PRIMARY KEY IDENTITY(1,1),
AccountCode NVARCHAR(50) UNIQUE NOT NULL,
AccountNameAr NVARCHAR(200) NOT NULL,
AccountNameEn NVARCHAR(200),
ParentAccountID INT NULL,
AccountLevel INT DEFAULT 1,
AccountType NVARCHAR(50),
AccountNature NVARCHAR(20),
IsParent BIT DEFAULT 0,
AllowPosting BIT DEFAULT 1,
RequireCostCenter BIT DEFAULT 0,
RequireProject BIT DEFAULT 0,
RequireEmployee BIT DEFAULT 0,
CurrencyID INT NULL,
Description NVARCHAR(500),
IsActive BIT DEFAULT 1,
CreatedBy INT,
CreatedDate DATETIME DEFAULT GETDATE(),
ModifiedBy INT,
ModifiedDate DATETIME,
CONSTRAINT FK_Account_Parent FOREIGN KEY (ParentAccountID) REFERENCES ChartOfAccounts(AccountID),
CONSTRAINT FK_Account_Currency FOREIGN KEY (CurrencyID) REFERENCES Currencies(CurrencyID)
);
ALTER TABLE ChartOfAccounts ADD CONSTRAINT FK_Account_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
ALTER TABLE ChartOfAccounts ADD CONSTRAINT FK_Account_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserID);
CREATE TABLE CompanyAccounts (
CompanyAccountID INT PRIMARY KEY IDENTITY(1,1),
CompanyID INT NOT NULL,
AccountID INT NOT NULL,
CompanySpecificCode NVARCHAR(50),
IsActive BIT DEFAULT 1,
ActivationDate DATE,
DeactivationDate DATE,
OpeningBalance DECIMAL(18, 4) DEFAULT 0,
OpeningBalanceDate DATE,
CreatedBy INT,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_CompAccount_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_CompAccount_Account FOREIGN KEY (AccountID) REFERENCES ChartOfAccounts(AccountID),
CONSTRAINT UQ_CompanyAccount UNIQUE(CompanyID, AccountID)
);
ALTER TABLE CompanyAccounts ADD CONSTRAINT FK_CompAccount_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
CREATE TABLE AccountCostCenters (
AccountCostCenterID INT PRIMARY KEY IDENTITY(1,1),
AccountID INT NOT NULL,
CostCenterID INT NOT NULL,
CompanyID INT NOT NULL,
AllocationPercentage DECIMAL(5, 2) DEFAULT 100.00,
IsActive BIT DEFAULT 1,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_AccCostCenter_Account FOREIGN KEY (AccountID) REFERENCES ChartOfAccounts(AccountID),
CONSTRAINT FK_AccCostCenter_CostCenter FOREIGN KEY (CostCenterID) REFERENCES CostCenters(CostCenterID),
CONSTRAINT FK_AccCostCenter_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID)
);
CREATE TABLE Warehouses (
WarehouseID INT PRIMARY KEY IDENTITY(1,1),
WarehouseCode NVARCHAR(50) NOT NULL,
WarehouseNameAr NVARCHAR(200) NOT NULL,
WarehouseNameEn NVARCHAR(200),
CompanyID INT NOT NULL,
WarehouseType NVARCHAR(50),
Location NVARCHAR(500),
City NVARCHAR(100),
Country NVARCHAR(100),
Phone NVARCHAR(50),
ManagerName NVARCHAR(200),
ManagerPhone NVARCHAR(50),
IsActive BIT DEFAULT 1,
AllowNegativeStock BIT DEFAULT 0,
CostCenterID INT NULL,
InventoryAccountID INT NULL,
CreatedBy INT,
CreatedDate DATETIME DEFAULT GETDATE(),
ModifiedBy INT,
ModifiedDate DATETIME,
CONSTRAINT FK_Warehouse_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_Warehouse_CostCenter FOREIGN KEY (CostCenterID) REFERENCES CostCenters(CostCenterID),
CONSTRAINT FK_Warehouse_Account FOREIGN KEY (InventoryAccountID) REFERENCES ChartOfAccounts(AccountID),
CONSTRAINT UQ_WarehouseCode_Company UNIQUE(WarehouseCode, CompanyID)
);
ALTER TABLE Warehouses ADD CONSTRAINT FK_Warehouse_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
ALTER TABLE Warehouses ADD CONSTRAINT FK_Warehouse_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserID);
CREATE TABLE StorageLocations (
LocationID INT PRIMARY KEY IDENTITY(1,1),
LocationCode NVARCHAR(50) NOT NULL,
LocationNameAr NVARCHAR(200) NOT NULL,
LocationNameEn NVARCHAR(200),
WarehouseID INT NOT NULL,
ParentLocationID INT NULL,
LocationType NVARCHAR(50),
Capacity DECIMAL(18, 4),
UnitOfMeasure NVARCHAR(50),
IsActive BIT DEFAULT 1,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_Location_Warehouse FOREIGN KEY (WarehouseID) REFERENCES Warehouses(WarehouseID),
CONSTRAINT FK_Location_Parent FOREIGN KEY (ParentLocationID) REFERENCES StorageLocations(LocationID)
);
CREATE TABLE ItemCategories (
CategoryID INT PRIMARY KEY IDENTITY(1,1),
CategoryCode NVARCHAR(50) UNIQUE NOT NULL,
CategoryNameAr NVARCHAR(200) NOT NULL,
CategoryNameEn NVARCHAR(200),
ParentCategoryID INT NULL,
CategoryLevel INT DEFAULT 1,
Description NVARCHAR(500),
IsActive BIT DEFAULT 1,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_Category_Parent FOREIGN KEY (ParentCategoryID) REFERENCES ItemCategories(CategoryID)
);
CREATE TABLE UnitsOfMeasure (
UnitID INT PRIMARY KEY IDENTITY(1,1),
UnitCode NVARCHAR(20) UNIQUE NOT NULL,
UnitNameAr NVARCHAR(100) NOT NULL,
UnitNameEn NVARCHAR(100),
UnitType NVARCHAR(50),
DecimalPlaces INT DEFAULT 2,
IsActive BIT DEFAULT 1,
CreatedDate DATETIME DEFAULT GETDATE()
);
CREATE TABLE UnitConversions (
ConversionID INT PRIMARY KEY IDENTITY(1,1),
FromUnitID INT NOT NULL,
ToUnitID INT NOT NULL,
ConversionFactor DECIMAL(18, 6) NOT NULL,
IsActive BIT DEFAULT 1,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_Conversion_FromUnit FOREIGN KEY (FromUnitID) REFERENCES UnitsOfMeasure(UnitID),
CONSTRAINT FK_Conversion_ToUnit FOREIGN KEY (ToUnitID) REFERENCES UnitsOfMeasure(UnitID)
);
CREATE TABLE Items (
ItemID INT PRIMARY KEY IDENTITY(1,1),
ItemCode NVARCHAR(50) UNIQUE NOT NULL,
ItemNameAr NVARCHAR(300) NOT NULL,
ItemNameEn NVARCHAR(300),
BarCode NVARCHAR(100),
CategoryID INT NOT NULL,
ItemType NVARCHAR(50),
BaseUnitID INT NOT NULL,
IsBatchTracked BIT DEFAULT 0,
IsSerialTracked BIT DEFAULT 0,
IsExpiryTracked BIT DEFAULT 0,
ShelfLife INT NULL,
MinStockLevel DECIMAL(18, 4) DEFAULT 0,
MaxStockLevel DECIMAL(18, 4) DEFAULT 0,
ReorderPoint DECIMAL(18, 4) DEFAULT 0,
ReorderQuantity DECIMAL(18, 4) DEFAULT 0,
StandardCost DECIMAL(18, 4) DEFAULT 0,
AverageCost DECIMAL(18, 4) DEFAULT 0,
LastPurchasePrice DECIMAL(18, 4) DEFAULT 0,
LastPurchaseDate DATE,
CostingMethod NVARCHAR(20),
DefaultInventoryAccountID INT NULL,
DefaultCOGSAccountID INT NULL,
DefaultSalesAccountID INT NULL,
DefaultPurchaseAccountID INT NULL,
TaxCategoryID INT NULL,
Weight DECIMAL(18, 4),
WeightUnitID INT,
Dimensions NVARCHAR(100),
ImagePath NVARCHAR(500),
Notes NVARCHAR(MAX),
IsActive BIT DEFAULT 1,
IsDiscontinued BIT DEFAULT 0,
CreatedBy INT,
CreatedDate DATETIME DEFAULT GETDATE(),
ModifiedBy INT,
ModifiedDate DATETIME,
CONSTRAINT FK_Item_Category FOREIGN KEY (CategoryID) REFERENCES ItemCategories(CategoryID),
CONSTRAINT FK_Item_BaseUnit FOREIGN KEY (BaseUnitID) REFERENCES UnitsOfMeasure(UnitID),
CONSTRAINT FK_Item_InventoryAccount FOREIGN KEY (DefaultInventoryAccountID) REFERENCES ChartOfAccounts(AccountID),
CONSTRAINT FK_Item_COGSAccount FOREIGN KEY (DefaultCOGSAccountID) REFERENCES ChartOfAccounts(AccountID),
CONSTRAINT FK_Item_SalesAccount FOREIGN KEY (DefaultSalesAccountID) REFERENCES ChartOfAccounts(AccountID),
CONSTRAINT FK_Item_PurchaseAccount FOREIGN KEY (DefaultPurchaseAccountID) REFERENCES ChartOfAccounts(AccountID)
);
ALTER TABLE Items ADD CONSTRAINT FK_Item_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
ALTER TABLE Items ADD CONSTRAINT FK_Item_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserID);
CREATE TABLE ItemUnits (
ItemUnitID INT PRIMARY KEY IDENTITY(1,1),
ItemID INT NOT NULL,
UnitID INT NOT NULL,
ConversionFactor DECIMAL(18, 6) NOT NULL,
IsDefaultPurchaseUnit BIT DEFAULT 0,
IsDefaultSalesUnit BIT DEFAULT 0,
BarCode NVARCHAR(100),
Price DECIMAL(18, 4),
IsActive BIT DEFAULT 1,
CreatedDate DATETIME DEFAULT GETDATE(),
CreatedBy INT NULL,
ModifiedBy INT NULL,
ModifiedDate DATETIME NULL,
CONSTRAINT FK_ItemUnit_Item FOREIGN KEY (ItemID) REFERENCES Items(ItemID),
CONSTRAINT FK_ItemUnit_Unit FOREIGN KEY (UnitID) REFERENCES UnitsOfMeasure(UnitID),
CONSTRAINT UQ_ItemUnit UNIQUE(ItemID, UnitID)
);
ALTER TABLE ItemUnits ADD CONSTRAINT FK_ItemUnits_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
ALTER TABLE ItemUnits ADD CONSTRAINT FK_ItemUnits_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserID);
CREATE TABLE CompanyItems (
CompanyItemID INT PRIMARY KEY IDENTITY(1,1),
CompanyID INT NOT NULL,
ItemID INT NOT NULL,
CompanyItemCode NVARCHAR(50),
IsActive BIT DEFAULT 1,
ActivationDate DATE,
CustomCostingMethod NVARCHAR(20),
CustomInventoryAccountID INT NULL,
CustomCOGSAccountID INT NULL,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_CompItem_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_CompItem_Item FOREIGN KEY (ItemID) REFERENCES Items(ItemID),
CONSTRAINT UQ_CompanyItem UNIQUE(CompanyID, ItemID)
);
CREATE TABLE InventoryTransactionTypes (
TransactionTypeID INT PRIMARY KEY IDENTITY(1,1),
TransactionTypeCode NVARCHAR(50) UNIQUE NOT NULL,
TransactionTypeNameAr NVARCHAR(100) NOT NULL,
TransactionTypeNameEn NVARCHAR(100),
TransactionNature NVARCHAR(20),
AffectsFinancials BIT DEFAULT 1,
RequiresApproval BIT DEFAULT 0,
IsActive BIT DEFAULT 1
);
CREATE TABLE InventoryTransactions (
TransactionID INT PRIMARY KEY IDENTITY(1,1),
TransactionNumber NVARCHAR(50) NOT NULL,
TransactionTypeID INT NOT NULL,
TransactionDate DATE NOT NULL,
CompanyID INT NOT NULL,
WarehouseID INT NOT NULL,
ToWarehouseID INT NULL,
CostCenterID INT NULL,
ReferenceNumber NVARCHAR(100),
ReferenceType NVARCHAR(50),
ReferenceID INT NULL,
TotalQuantity DECIMAL(18, 4),
TotalValue DECIMAL(18, 4),
CurrencyID INT NOT NULL,
ExchangeRate DECIMAL(18, 6) DEFAULT 1,
Status NVARCHAR(20),
Notes NVARCHAR(MAX),
ApprovedBy INT NULL,
ApprovedDate DATETIME NULL,
PostedBy INT NULL,
PostedDate DATETIME NULL,
CreatedBy INT NOT NULL,
CreatedDate DATETIME DEFAULT GETDATE(),
ModifiedBy INT,
ModifiedDate DATETIME,
CONSTRAINT FK_InvTrans_Type FOREIGN KEY (TransactionTypeID) REFERENCES InventoryTransactionTypes(TransactionTypeID),
CONSTRAINT FK_InvTrans_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_InvTrans_Warehouse FOREIGN KEY (WarehouseID) REFERENCES Warehouses(WarehouseID),
CONSTRAINT FK_InvTrans_ToWarehouse FOREIGN KEY (ToWarehouseID) REFERENCES Warehouses(WarehouseID),
CONSTRAINT FK_InvTrans_CostCenter FOREIGN KEY (CostCenterID) REFERENCES CostCenters(CostCenterID),
CONSTRAINT FK_InvTrans_Currency FOREIGN KEY (CurrencyID) REFERENCES Currencies(CurrencyID),
CONSTRAINT UQ_TransactionNumber_Company UNIQUE(TransactionNumber, CompanyID)
);
ALTER TABLE InventoryTransactions ADD CONSTRAINT FK_InvTrans_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
ALTER TABLE InventoryTransactions ADD CONSTRAINT FK_InvTrans_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserID);
CREATE TABLE InventoryTransactionDetails (
DetailID INT PRIMARY KEY IDENTITY(1,1),
TransactionID INT NOT NULL,
LineNumber INT NOT NULL,
ItemID INT NOT NULL,
UnitID INT NOT NULL,
Quantity DECIMAL(18, 4) NOT NULL,
UnitCost DECIMAL(18, 4) NOT NULL,
TotalCost DECIMAL(18, 4) NOT NULL,
LocationID INT NULL,
ToLocationID INT NULL,
BatchNumber NVARCHAR(100),
SerialNumber NVARCHAR(100),
ExpiryDate DATE,
ManufacturingDate DATE,
Notes NVARCHAR(500),
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_InvTransDet_Transaction FOREIGN KEY (TransactionID) REFERENCES InventoryTransactions(TransactionID) ON DELETE CASCADE,
CONSTRAINT FK_InvTransDet_Item FOREIGN KEY (ItemID) REFERENCES Items(ItemID),
CONSTRAINT FK_InvTransDet_Unit FOREIGN KEY (UnitID) REFERENCES UnitsOfMeasure(UnitID),
CONSTRAINT FK_InvTransDet_Location FOREIGN KEY (LocationID) REFERENCES StorageLocations(LocationID),
CONSTRAINT FK_InvTransDet_ToLocation FOREIGN KEY (ToLocationID) REFERENCES StorageLocations(LocationID)
);
CREATE TABLE ItemBatches (
BatchID INT PRIMARY KEY IDENTITY(1,1),
BatchNumber NVARCHAR(100) NOT NULL,
ItemID INT NOT NULL,
WarehouseID INT NOT NULL,
ManufacturingDate DATE,
ExpiryDate DATE,
SupplierID INT NULL,
PurchaseOrderNumber NVARCHAR(50),
InitialQuantity DECIMAL(18, 4),
CurrentQuantity DECIMAL(18, 4),
UnitCost DECIMAL(18, 4),
Status NVARCHAR(20),
Notes NVARCHAR(500),
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_Batch_Item FOREIGN KEY (ItemID) REFERENCES Items(ItemID),
CONSTRAINT FK_Batch_Warehouse FOREIGN KEY (WarehouseID) REFERENCES Warehouses(WarehouseID),
CONSTRAINT UQ_BatchNumber_Item_Warehouse UNIQUE(BatchNumber, ItemID, WarehouseID)
);
CREATE TABLE ItemSerials (
SerialID INT PRIMARY KEY IDENTITY(1,1),
SerialNumber NVARCHAR(100) NOT NULL,
ItemID INT NOT NULL,
WarehouseID INT NOT NULL,
LocationID INT NULL,
BatchNumber NVARCHAR(100),
Status NVARCHAR(20),
PurchaseDate DATE,
PurchasePrice DECIMAL(18, 4),
WarrantyExpiryDate DATE,
Notes NVARCHAR(500),
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_Serial_Item FOREIGN KEY (ItemID) REFERENCES Items(ItemID),
CONSTRAINT FK_Serial_Warehouse FOREIGN KEY (WarehouseID) REFERENCES Warehouses(WarehouseID),
CONSTRAINT FK_Serial_Location FOREIGN KEY (LocationID) REFERENCES StorageLocations(LocationID),
CONSTRAINT UQ_SerialNumber_Item UNIQUE(SerialNumber, ItemID)
);
CREATE TABLE StockBalances (
BalanceID INT PRIMARY KEY IDENTITY(1,1),
CompanyID INT NOT NULL,
WarehouseID INT NOT NULL,
ItemID INT NOT NULL,
LocationID INT NULL,
BatchNumber NVARCHAR(100),
UnitID INT NOT NULL,
QuantityOnHand DECIMAL(18, 4) DEFAULT 0,
QuantityReserved DECIMAL(18, 4) DEFAULT 0,
QuantityAvailable DECIMAL(18, 4) DEFAULT 0,
QuantityOnOrder DECIMAL(18, 4) DEFAULT 0,
AverageCost DECIMAL(18, 4) DEFAULT 0,
TotalValue DECIMAL(18, 4) DEFAULT 0,
LastTransactionDate DATETIME,
LastUpdated DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_Balance_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_Balance_Warehouse FOREIGN KEY (WarehouseID) REFERENCES Warehouses(WarehouseID),
CONSTRAINT FK_Balance_Item FOREIGN KEY (ItemID) REFERENCES Items(ItemID),
CONSTRAINT FK_Balance_Location FOREIGN KEY (LocationID) REFERENCES StorageLocations(LocationID),
CONSTRAINT FK_Balance_Unit FOREIGN KEY (UnitID) REFERENCES UnitsOfMeasure(UnitID),
CONSTRAINT UQ_StockBalance UNIQUE(CompanyID, WarehouseID, ItemID, LocationID, BatchNumber, UnitID)
);
CREATE TABLE InventoryLedger (
LedgerID INT PRIMARY KEY IDENTITY(1,1),
TransactionID INT NOT NULL,
DetailID INT NOT NULL,
CompanyID INT NOT NULL,
WarehouseID INT NOT NULL,
ItemID INT NOT NULL,
TransactionDate DATETIME NOT NULL,
TransactionType NVARCHAR(20),
Quantity DECIMAL(18, 4) NOT NULL,
UnitCost DECIMAL(18, 4) NOT NULL,
TotalCost DECIMAL(18, 4) NOT NULL,
BalanceQuantity DECIMAL(18, 4),
BalanceValue DECIMAL(18, 4),
BatchNumber NVARCHAR(100),
SerialNumber NVARCHAR(100),
LocationID INT NULL,
CostCenterID INT NULL,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_Ledger_Transaction FOREIGN KEY (TransactionID) REFERENCES InventoryTransactions(TransactionID),
CONSTRAINT FK_Ledger_Detail FOREIGN KEY (DetailID) REFERENCES InventoryTransactionDetails(DetailID),
CONSTRAINT FK_Ledger_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_Ledger_Warehouse FOREIGN KEY (WarehouseID) REFERENCES Warehouses(WarehouseID),
CONSTRAINT FK_Ledger_Item FOREIGN KEY (ItemID) REFERENCES Items(ItemID),
CONSTRAINT FK_Ledger_Location FOREIGN KEY (LocationID) REFERENCES StorageLocations(LocationID),
CONSTRAINT FK_Ledger_CostCenter FOREIGN KEY (CostCenterID) REFERENCES CostCenters(CostCenterID)
);
CREATE TABLE CostingMethods (
MethodID INT PRIMARY KEY IDENTITY(1,1),
MethodCode NVARCHAR(20) UNIQUE NOT NULL,
MethodNameAr NVARCHAR(100) NOT NULL,
MethodNameEn NVARCHAR(100),
Description NVARCHAR(500),
IsActive BIT DEFAULT 1
);
CREATE TABLE InventoryCostLayers (
LayerID INT PRIMARY KEY IDENTITY(1,1),
CompanyID INT NOT NULL,
WarehouseID INT NOT NULL,
ItemID INT NOT NULL,
TransactionID INT NOT NULL,
TransactionDate DATETIME NOT NULL,
BatchNumber NVARCHAR(100),
LayerSequence INT NOT NULL,
QuantityReceived DECIMAL(18, 4) NOT NULL,
QuantityRemaining DECIMAL(18, 4) NOT NULL,
UnitCost DECIMAL(18, 4) NOT NULL,
TotalCost DECIMAL(18, 4) NOT NULL,
CurrencyID INT NOT NULL,
ExchangeRate DECIMAL(18, 6) DEFAULT 1,
IsFullyConsumed BIT DEFAULT 0,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_CostLayer_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_CostLayer_Warehouse FOREIGN KEY (WarehouseID) REFERENCES Warehouses(WarehouseID),
CONSTRAINT FK_CostLayer_Item FOREIGN KEY (ItemID) REFERENCES Items(ItemID),
CONSTRAINT FK_CostLayer_Transaction FOREIGN KEY (TransactionID) REFERENCES InventoryTransactions(TransactionID),
CONSTRAINT FK_CostLayer_Currency FOREIGN KEY (CurrencyID) REFERENCES Currencies(CurrencyID)
);
CREATE TABLE CostLayerConsumption (
ConsumptionID INT PRIMARY KEY IDENTITY(1,1),
LayerID INT NOT NULL,
OutTransactionID INT NOT NULL,
OutDetailID INT NOT NULL,
QuantityConsumed DECIMAL(18, 4) NOT NULL,
UnitCost DECIMAL(18, 4) NOT NULL,
TotalCost DECIMAL(18, 4) NOT NULL,
ConsumptionDate DATETIME NOT NULL,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_Consumption_Layer FOREIGN KEY (LayerID) REFERENCES InventoryCostLayers(LayerID),
CONSTRAINT FK_Consumption_Transaction FOREIGN KEY (OutTransactionID) REFERENCES InventoryTransactions(TransactionID),
CONSTRAINT FK_Consumption_Detail FOREIGN KEY (OutDetailID) REFERENCES InventoryTransactionDetails(DetailID)
);
CREATE TABLE StockCounts (
CountID INT PRIMARY KEY IDENTITY(1,1),
CountNumber NVARCHAR(50) NOT NULL,
CountDate DATE NOT NULL,
CompanyID INT NOT NULL,
WarehouseID INT NOT NULL,
CountType NVARCHAR(20),
Status NVARCHAR(20),
ScheduledStartDate DATETIME,
ScheduledEndDate DATETIME,
ActualStartDate DATETIME,
ActualEndDate DATETIME,
Notes NVARCHAR(MAX),
CreatedBy INT,
CreatedDate DATETIME DEFAULT GETDATE(),
ApprovedBy INT,
ApprovedDate DATETIME,
PostedBy INT,
PostedDate DATETIME,
CONSTRAINT FK_Count_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_Count_Warehouse FOREIGN KEY (WarehouseID) REFERENCES Warehouses(WarehouseID),
CONSTRAINT UQ_CountNumber_Company UNIQUE(CountNumber, CompanyID)
);
ALTER TABLE StockCounts ADD CONSTRAINT FK_StockCounts_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
CREATE TABLE StockCountDetails (
CountDetailID INT PRIMARY KEY IDENTITY(1,1),
CountID INT NOT NULL,
ItemID INT NOT NULL,
LocationID INT NULL,
BatchNumber NVARCHAR(100),
SerialNumber NVARCHAR(100),
SystemQuantity DECIMAL(18, 4),
CountedQuantity DECIMAL(18, 4),
VarianceQuantity DECIMAL(18, 4),
UnitID INT NOT NULL,
UnitCost DECIMAL(18, 4),
VarianceValue DECIMAL(18, 4),
CountedBy INT,
CountedDate DATETIME,
VerifiedBy INT,
VerifiedDate DATETIME,
Notes NVARCHAR(500),
CONSTRAINT FK_CountDet_Count FOREIGN KEY (CountID) REFERENCES StockCounts(CountID) ON DELETE CASCADE,
CONSTRAINT FK_CountDet_Item FOREIGN KEY (ItemID) REFERENCES Items(ItemID),
CONSTRAINT FK_CountDet_Location FOREIGN KEY (LocationID) REFERENCES StorageLocations(LocationID),
CONSTRAINT FK_CountDet_Unit FOREIGN KEY (UnitID) REFERENCES UnitsOfMeasure(UnitID)
);
CREATE TABLE StockAdjustments (
AdjustmentID INT PRIMARY KEY IDENTITY(1,1),
AdjustmentNumber NVARCHAR(50) NOT NULL,
AdjustmentDate DATE NOT NULL,
CompanyID INT NOT NULL,
WarehouseID INT NOT NULL,
AdjustmentType NVARCHAR(50),
ReasonCode NVARCHAR(50),
ReasonDescription NVARCHAR(500),
ReferenceNumber NVARCHAR(100),
ReferenceType NVARCHAR(50),
ReferenceID INT NULL,
CostCenterID INT NULL,
TotalAdjustmentValue DECIMAL(18, 4),
Status NVARCHAR(20),
Notes NVARCHAR(MAX),
CreatedBy INT,
CreatedDate DATETIME DEFAULT GETDATE(),
ApprovedBy INT,
ApprovedDate DATETIME,
PostedBy INT,
PostedDate DATETIME,
CONSTRAINT FK_Adjustment_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_Adjustment_Warehouse FOREIGN KEY (WarehouseID) REFERENCES Warehouses(WarehouseID),
CONSTRAINT FK_Adjustment_CostCenter FOREIGN KEY (CostCenterID) REFERENCES CostCenters(CostCenterID),
CONSTRAINT UQ_AdjustmentNumber_Company UNIQUE(AdjustmentNumber, CompanyID)
);
ALTER TABLE StockAdjustments ADD CONSTRAINT FK_StockAdjustments_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
CREATE TABLE StockAdjustmentDetails (
AdjustmentDetailID INT PRIMARY KEY IDENTITY(1,1),
AdjustmentID INT NOT NULL,
ItemID INT NOT NULL,
LocationID INT NULL,
BatchNumber NVARCHAR(100),
SerialNumber NVARCHAR(100),
UnitID INT NOT NULL,
AdjustmentQuantity DECIMAL(18, 4),
UnitCost DECIMAL(18, 4),
TotalAdjustmentValue DECIMAL(18, 4),
OldQuantity DECIMAL(18, 4),
NewQuantity DECIMAL(18, 4),
Notes NVARCHAR(500),
CONSTRAINT FK_AdjDet_Adjustment FOREIGN KEY (AdjustmentID) REFERENCES StockAdjustments(AdjustmentID) ON DELETE CASCADE,
CONSTRAINT FK_AdjDet_Item FOREIGN KEY (ItemID) REFERENCES Items(ItemID),
CONSTRAINT FK_AdjDet_Location FOREIGN KEY (LocationID) REFERENCES StorageLocations(LocationID),
CONSTRAINT FK_AdjDet_Unit FOREIGN KEY (UnitID) REFERENCES UnitsOfMeasure(UnitID)
);
CREATE TABLE InventoryJournalEntries (
JournalEntryID INT PRIMARY KEY IDENTITY(1,1),
EntryNumber NVARCHAR(50) NOT NULL,
EntryDate DATE NOT NULL,
CompanyID INT NOT NULL,
TransactionID INT NULL,
AdjustmentID INT NULL,
CurrencyID INT NOT NULL,
ExchangeRate DECIMAL(18, 6) DEFAULT 1,
Description NVARCHAR(500),
Status NVARCHAR(20),
PostedBy INT,
PostedDate DATETIME,
CreatedBy INT,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_JrnlEntry_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_JrnlEntry_Transaction FOREIGN KEY (TransactionID) REFERENCES InventoryTransactions(TransactionID),
CONSTRAINT FK_JrnlEntry_Adjustment FOREIGN KEY (AdjustmentID) REFERENCES StockAdjustments(AdjustmentID),
CONSTRAINT FK_JrnlEntry_Currency FOREIGN KEY (CurrencyID) REFERENCES Currencies(CurrencyID)
);
ALTER TABLE InventoryJournalEntries ADD CONSTRAINT FK_InventoryJournalEntries_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
CREATE TABLE JournalEntryLines (
LineID INT PRIMARY KEY IDENTITY(1,1),
JournalEntryID INT NOT NULL,
LineNumber INT NOT NULL,
AccountID INT NOT NULL,
CostCenterID INT NULL,
DebitAmount DECIMAL(18, 4) DEFAULT 0,
CreditAmount DECIMAL(18, 4) DEFAULT 0,
Description NVARCHAR(500),
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_JrnlLine_Entry FOREIGN KEY (JournalEntryID) REFERENCES InventoryJournalEntries(JournalEntryID) ON DELETE CASCADE,
CONSTRAINT FK_JrnlLine_Account FOREIGN KEY (AccountID) REFERENCES ChartOfAccounts(AccountID),
CONSTRAINT FK_JrnlLine_CostCenter FOREIGN KEY (CostCenterID) REFERENCES CostCenters(CostCenterID)
);
CREATE TABLE AuditLog (
AuditID BIGINT PRIMARY KEY IDENTITY(1,1),
TableName NVARCHAR(100) NOT NULL,
RecordID INT NOT NULL,
ActionType NVARCHAR(20) NOT NULL,
FieldName NVARCHAR(100),
OldValue NVARCHAR(MAX),
NewValue NVARCHAR(MAX),
CompanyID INT,
UserID INT NOT NULL,
ActionDate DATETIME DEFAULT GETDATE(),
IPAddress NVARCHAR(50),
CONSTRAINT FK_Audit_User FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
CREATE TABLE AccountingTemplates (
TemplateID INT PRIMARY KEY IDENTITY(1,1),
TemplateCode NVARCHAR(50) UNIQUE NOT NULL,
TemplateNameAr NVARCHAR(200) NOT NULL,
TemplateNameEn NVARCHAR(200),
TransactionTypeID INT NOT NULL,
CompanyID INT NULL,
IsActive BIT DEFAULT 1,
Description NVARCHAR(500),
CreatedBy INT,
CreatedDate DATETIME DEFAULT GETDATE(),
ModifiedBy INT,
ModifiedDate DATETIME,
CONSTRAINT FK_Template_TransactionType FOREIGN KEY (TransactionTypeID) REFERENCES InventoryTransactionTypes(TransactionTypeID),
CONSTRAINT FK_Template_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID)
);
ALTER TABLE AccountingTemplates ADD CONSTRAINT FK_AccountingTemplates_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
ALTER TABLE AccountingTemplates ADD CONSTRAINT FK_AccountingTemplates_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserID);
CREATE TABLE AccountingTemplateLines (
TemplateLineID INT PRIMARY KEY IDENTITY(1,1),
TemplateID INT NOT NULL,
LineNumber INT NOT NULL,
AccountType NVARCHAR(50),
AccountID INT NULL,
AccountSource NVARCHAR(50),
DebitCreditIndicator NVARCHAR(10),
AmountSource NVARCHAR(50),
RequiresCostCenter BIT DEFAULT 0,
CostCenterSource NVARCHAR(50),
IsActive BIT DEFAULT 1,
Notes NVARCHAR(500),
CONSTRAINT FK_TemplateLine_Template FOREIGN KEY (TemplateID) REFERENCES AccountingTemplates(TemplateID) ON DELETE CASCADE,
CONSTRAINT FK_TemplateLine_Account FOREIGN KEY (AccountID) REFERENCES ChartOfAccounts(AccountID)
);
CREATE TABLE PostingStatus (
PostingStatusID INT PRIMARY KEY IDENTITY(1,1),
TransactionID INT NULL,
AdjustmentID INT NULL,
CountID INT NULL,
DocumentType NVARCHAR(50),
PostingDate DATETIME,
JournalEntryID INT NULL,
Status NVARCHAR(20),
ErrorMessage NVARCHAR(MAX),
PostedBy INT,
AttemptCount INT DEFAULT 0,
LastAttemptDate DATETIME,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_PostStatus_Transaction FOREIGN KEY (TransactionID) REFERENCES InventoryTransactions(TransactionID),
CONSTRAINT FK_PostStatus_Adjustment FOREIGN KEY (AdjustmentID) REFERENCES StockAdjustments(AdjustmentID),
CONSTRAINT FK_PostStatus_Count FOREIGN KEY (CountID) REFERENCES StockCounts(CountID),
CONSTRAINT FK_PostStatus_JournalEntry FOREIGN KEY (JournalEntryID) REFERENCES InventoryJournalEntries(JournalEntryID)
);
CREATE TABLE PostingSettings (
SettingID INT PRIMARY KEY IDENTITY(1,1),
CompanyID INT NOT NULL,
SettingKey NVARCHAR(100) NOT NULL,
SettingValue NVARCHAR(500),
DataType NVARCHAR(20),
Description NVARCHAR(500),
ModifiedBy INT,
ModifiedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_PostingSetting_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT UQ_PostingSetting UNIQUE(CompanyID, SettingKey)
);
ALTER TABLE PostingSettings ADD CONSTRAINT FK_PostingSettings_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserID);
CREATE TABLE PostingValidationRules (
RuleID INT PRIMARY KEY IDENTITY(1,1),
RuleCode NVARCHAR(50) UNIQUE NOT NULL,
RuleNameAr NVARCHAR(200) NOT NULL,
RuleNameEn NVARCHAR(200),
DocumentType NVARCHAR(50),
ValidationQuery NVARCHAR(MAX),
ErrorMessageAr NVARCHAR(500),
ErrorMessageEn NVARCHAR(500),
Severity NVARCHAR(20),
IsActive BIT DEFAULT 1,
ExecutionOrder INT DEFAULT 0,
CreatedDate DATETIME DEFAULT GETDATE()
);
CREATE TABLE PostingErrorLog (
ErrorID BIGINT PRIMARY KEY IDENTITY(1,1),
DocumentType NVARCHAR(50),
DocumentID INT,
ErrorCode NVARCHAR(50),
ErrorMessage NVARCHAR(MAX),
StackTrace NVARCHAR(MAX),
ErrorDate DATETIME DEFAULT GETDATE(),
UserID INT,
IsResolved BIT DEFAULT 0,
ResolvedBy INT,
ResolvedDate DATETIME,
ResolutionNotes NVARCHAR(MAX)
);
CREATE TABLE NumberSequences (
SequenceID INT PRIMARY KEY IDENTITY(1,1),
SequenceCode NVARCHAR(100) UNIQUE NOT NULL,
CurrentValue BIGINT NOT NULL DEFAULT 0,
IncrementBy INT NOT NULL DEFAULT 1,
Padding INT NOT NULL DEFAULT 5,
Prefix NVARCHAR(50) NULL,
Suffix NVARCHAR(50) NULL,
ResetPolicy NVARCHAR(20) DEFAULT 'Never',
LastResetDate DATE NULL,
IsActive BIT DEFAULT 1,
CreatedDate DATETIME DEFAULT GETDATE()
);
CREATE TABLE NumberSequenceFormats (
FormatID INT PRIMARY KEY IDENTITY(1,1),
SequenceID INT NOT NULL,
FormatKey NVARCHAR(100) NOT NULL,
FormatPattern NVARCHAR(200) NOT NULL,
CONSTRAINT FK_NS_Format_Sequence FOREIGN KEY (SequenceID) REFERENCES NumberSequences(SequenceID) ON DELETE CASCADE,
CONSTRAINT UQ_NS_Format UNIQUE(SequenceID, FormatKey)
);
CREATE TABLE FormAccountMappings (
MappingID INT PRIMARY KEY IDENTITY(1,1),
FormCode NVARCHAR(100) NOT NULL,
CompanyID INT NULL,
ActionCode NVARCHAR(100) NULL,
AccountPurpose NVARCHAR(100) NOT NULL,
AccountID INT NULL,
AccountSource NVARCHAR(50) DEFAULT 'Fixed',
IsActive BIT DEFAULT 1,
CreatedBy INT,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_FormAccount_Account FOREIGN KEY (AccountID) REFERENCES ChartOfAccounts(AccountID)
);
ALTER TABLE FormAccountMappings ADD CONSTRAINT FK_FormAccountMappings_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
CREATE TABLE PaymentTerms (
PaymentTermID INT PRIMARY KEY IDENTITY(1,1),
TermCode NVARCHAR(50) UNIQUE NOT NULL,
TermNameAr NVARCHAR(200) NOT NULL,
TermNameEn NVARCHAR(200),
NumberOfDays INT NOT NULL,
IsActive BIT DEFAULT 1,
CreatedDate DATETIME DEFAULT GETDATE()
);
CREATE TABLE Suppliers (
SupplierID INT PRIMARY KEY IDENTITY(1,1),
SupplierCode NVARCHAR(50) UNIQUE NOT NULL,
SupplierNameAr NVARCHAR(300) NOT NULL,
SupplierNameEn NVARCHAR(300),
CompanyID INT NOT NULL,
TaxNumber NVARCHAR(100),
CommercialRegister NVARCHAR(100),
DefaultCurrencyID INT NULL,
PaymentTermID INT NULL,
CreditLimit DECIMAL(18,4),
IsActive BIT DEFAULT 1,
CreatedBy INT,
CreatedDate DATETIME DEFAULT GETDATE(),
ModifiedBy INT,
ModifiedDate DATETIME,
CONSTRAINT FK_Supplier_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_Supplier_Currency FOREIGN KEY (DefaultCurrencyID) REFERENCES Currencies(CurrencyID),
CONSTRAINT FK_Supplier_PaymentTerm FOREIGN KEY (PaymentTermID) REFERENCES PaymentTerms(PaymentTermID)
);
ALTER TABLE Suppliers ADD CONSTRAINT FK_Suppliers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
ALTER TABLE Suppliers ADD CONSTRAINT FK_Suppliers_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserID);
CREATE TABLE SupplierContacts (
ContactID INT PRIMARY KEY IDENTITY(1,1),
SupplierID INT NOT NULL,
ContactName NVARCHAR(200),
Position NVARCHAR(100),
Phone NVARCHAR(50),
Mobile NVARCHAR(50),
Email NVARCHAR(100),
Notes NVARCHAR(500),
CONSTRAINT FK_SupplierContact_Supplier FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID) ON DELETE CASCADE
);
CREATE TABLE SupplierAddresses (
AddressID INT PRIMARY KEY IDENTITY(1,1),
SupplierID INT NOT NULL,
AddressType NVARCHAR(50),
Country NVARCHAR(100),
City NVARCHAR(100),
Street NVARCHAR(300),
BuildingNo NVARCHAR(50),
PostalCode NVARCHAR(20),
IsDefault BIT DEFAULT 0,
CONSTRAINT FK_SupplierAddress_Supplier FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID) ON DELETE CASCADE
);
CREATE TABLE Customers (
CustomerID INT PRIMARY KEY IDENTITY(1,1),
CustomerCode NVARCHAR(50) UNIQUE NOT NULL,
CustomerNameAr NVARCHAR(300) NOT NULL,
CustomerNameEn NVARCHAR(300),
CompanyID INT NOT NULL,
TaxNumber NVARCHAR(100),
DefaultCurrencyID INT NULL,
PaymentTermID INT NULL,
CreditLimit DECIMAL(18,4),
IsActive BIT DEFAULT 1,
CreatedBy INT,
CreatedDate DATETIME DEFAULT GETDATE(),
ModifiedBy INT,
ModifiedDate DATETIME,
CONSTRAINT FK_Customer_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_Customer_Currency FOREIGN KEY (DefaultCurrencyID) REFERENCES Currencies(CurrencyID),
CONSTRAINT FK_Customer_PaymentTerm FOREIGN KEY (PaymentTermID) REFERENCES PaymentTerms(PaymentTermID)
);
ALTER TABLE Customers ADD CONSTRAINT FK_Customers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
ALTER TABLE Customers ADD CONSTRAINT FK_Customers_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserID);
CREATE TABLE CustomerContacts (
ContactID INT PRIMARY KEY IDENTITY(1,1),
CustomerID INT NOT NULL,
ContactName NVARCHAR(200),
Position NVARCHAR(100),
Phone NVARCHAR(50),
Mobile NVARCHAR(50),
Email NVARCHAR(100),
Notes NVARCHAR(500),
CONSTRAINT FK_CustomerContact_Customer FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID) ON DELETE CASCADE
);
CREATE TABLE CustomerAddresses (
AddressID INT PRIMARY KEY IDENTITY(1,1),
CustomerID INT NOT NULL,
AddressType NVARCHAR(50),
Country NVARCHAR(100),
City NVARCHAR(100),
Street NVARCHAR(300),
BuildingNo NVARCHAR(50),
PostalCode NVARCHAR(20),
IsDefault BIT DEFAULT 0,
CONSTRAINT FK_CustomerAddress_Customer FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID) ON DELETE CASCADE
);
CREATE TABLE TaxCodes (
TaxCodeID INT PRIMARY KEY IDENTITY(1,1),
TaxCode NVARCHAR(50) UNIQUE NOT NULL,
TaxNameAr NVARCHAR(200),
TaxNameEn NVARCHAR(200),
Description NVARCHAR(500),
IsActive BIT DEFAULT 1
);
CREATE TABLE TaxRates (
TaxRateID INT PRIMARY KEY IDENTITY(1,1),
TaxCodeID INT NOT NULL,
Rate DECIMAL(18,4) NOT NULL,
EffectiveDate DATE NOT NULL,
EndDate DATE,
CompanyID INT NULL,
IsActive BIT DEFAULT 1,
CONSTRAINT FK_TaxRate_TaxCode FOREIGN KEY (TaxCodeID) REFERENCES TaxCodes(TaxCodeID),
CONSTRAINT FK_TaxRate_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID)
);
CREATE TABLE ItemTaxGroups (
ItemTaxGroupID INT PRIMARY KEY IDENTITY(1,1),
ItemID INT NOT NULL,
TaxCodeID INT NOT NULL,
IsActive BIT DEFAULT 1,
CONSTRAINT FK_ItemTax_Item FOREIGN KEY (ItemID) REFERENCES Items(ItemID),
CONSTRAINT FK_ItemTax_TaxCode FOREIGN KEY (TaxCodeID) REFERENCES TaxCodes(TaxCodeID),
CONSTRAINT UQ_ItemTax UNIQUE (ItemID, TaxCodeID)
);
CREATE TABLE DocumentTaxes (
DocumentTaxID INT PRIMARY KEY IDENTITY(1,1),
DocumentType NVARCHAR(50),
DocumentID INT,
TaxCodeID INT,
TaxRate DECIMAL(18,4),
TaxAmount DECIMAL(18,4),
BaseAmount DECIMAL(18,4),
CompanyID INT,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_DocTax_TaxCode FOREIGN KEY (TaxCodeID) REFERENCES TaxCodes(TaxCodeID)
);
CREATE TABLE PurchaseOrders (
PurchaseOrderID INT PRIMARY KEY IDENTITY(1,1),
PurchaseOrderNumber NVARCHAR(50) NOT NULL,
CompanyID INT NOT NULL,
SupplierID INT NOT NULL,
OrderDate DATE NOT NULL,
ExpectedDeliveryDate DATE NULL,
CurrencyID INT NOT NULL,
ExchangeRate DECIMAL(18,6) DEFAULT 1,
PaymentTermID INT NULL,
Status NVARCHAR(20) DEFAULT 'Draft',
TotalAmount DECIMAL(18,4) DEFAULT 0,
TotalTax DECIMAL(18,4) DEFAULT 0,
Notes NVARCHAR(MAX),
CreatedBy INT,
CreatedDate DATETIME DEFAULT GETDATE(),
ModifiedBy INT,
ModifiedDate DATETIME,
CONSTRAINT UQ_PO_Number_Company UNIQUE(PurchaseOrderNumber, CompanyID),
CONSTRAINT FK_PO_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_PO_Supplier FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID),
CONSTRAINT FK_PO_Currency FOREIGN KEY (CurrencyID) REFERENCES Currencies(CurrencyID),
CONSTRAINT FK_PO_PaymentTerm FOREIGN KEY (PaymentTermID) REFERENCES PaymentTerms(PaymentTermID)
);
ALTER TABLE PurchaseOrders ADD CONSTRAINT FK_PurchaseOrders_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
ALTER TABLE PurchaseOrders ADD CONSTRAINT FK_PurchaseOrders_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserID);
CREATE TABLE PurchaseOrderDetails (
PODetailID INT PRIMARY KEY IDENTITY(1,1),
PurchaseOrderID INT NOT NULL,
LineNumber INT NOT NULL,
ItemID INT NOT NULL,
UnitID INT NOT NULL,
Quantity DECIMAL(18,4) NOT NULL,
UnitPrice DECIMAL(18,4) NOT NULL,
LineTotal DECIMAL(18,4) NOT NULL,
TaxCodeID INT NULL,
Notes NVARCHAR(500),
CONSTRAINT FK_POD_PO FOREIGN KEY (PurchaseOrderID) REFERENCES PurchaseOrders(PurchaseOrderID) ON DELETE CASCADE,
CONSTRAINT FK_POD_Item FOREIGN KEY (ItemID) REFERENCES Items(ItemID),
CONSTRAINT FK_POD_Unit FOREIGN KEY (UnitID) REFERENCES UnitsOfMeasure(UnitID),
CONSTRAINT FK_POD_TaxCode FOREIGN KEY (TaxCodeID) REFERENCES TaxCodes(TaxCodeID)
);
CREATE TABLE GoodsReceipts (
GRNID INT PRIMARY KEY IDENTITY(1,1),
GRNNumber NVARCHAR(50) NOT NULL,
CompanyID INT NOT NULL,
PurchaseOrderID INT NULL,
SupplierID INT NOT NULL,
WarehouseID INT NOT NULL,
ReceiptDate DATE NOT NULL,
ReferenceNumber NVARCHAR(100),
Status NVARCHAR(20) DEFAULT 'Received',
TotalQuantity DECIMAL(18,4) DEFAULT 0,
TotalValue DECIMAL(18,4) DEFAULT 0,
CreatedBy INT,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT UQ_GRN_Number_Company UNIQUE(GRNNumber, CompanyID),
CONSTRAINT FK_GRN_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_GRN_PO FOREIGN KEY (PurchaseOrderID) REFERENCES PurchaseOrders(PurchaseOrderID),
CONSTRAINT FK_GRN_Supplier FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID),
CONSTRAINT FK_GRN_Warehouse FOREIGN KEY (WarehouseID) REFERENCES Warehouses(WarehouseID)
);
ALTER TABLE GoodsReceipts ADD CONSTRAINT FK_GoodsReceipts_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
CREATE TABLE GoodsReceiptDetails (
GRNDetailID INT PRIMARY KEY IDENTITY(1,1),
GRNID INT NOT NULL,
LineNumber INT NOT NULL,
ItemID INT NOT NULL,
UnitID INT NOT NULL,
Quantity DECIMAL(18,4) NOT NULL,
UnitCost DECIMAL(18,4) NOT NULL,
TotalCost DECIMAL(18,4) NOT NULL,
BatchNumber NVARCHAR(100),
ExpiryDate DATE NULL,
Notes NVARCHAR(500),
CONSTRAINT FK_GRNDet_GRN FOREIGN KEY (GRNID) REFERENCES GoodsReceipts(GRNID) ON DELETE CASCADE,
CONSTRAINT FK_GRNDet_Item FOREIGN KEY (ItemID) REFERENCES Items(ItemID),
CONSTRAINT FK_GRNDet_Unit FOREIGN KEY (UnitID) REFERENCES UnitsOfMeasure(UnitID)
);
CREATE TABLE PurchaseInvoices (
PurchaseInvoiceID INT PRIMARY KEY IDENTITY(1,1),
InvoiceNumber NVARCHAR(50) NOT NULL,
CompanyID INT NOT NULL,
SupplierID INT NOT NULL,
InvoiceDate DATE NOT NULL,
DueDate DATE NULL,
CurrencyID INT NOT NULL,
ExchangeRate DECIMAL(18,6) DEFAULT 1,
PurchaseOrderID INT NULL,
GRNID INT NULL,
TotalAmount DECIMAL(18,4) DEFAULT 0,
TotalTax DECIMAL(18,4) DEFAULT 0,
Status NVARCHAR(20) DEFAULT 'Draft',
CreatedBy INT,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT UQ_PInvoice_Number_Company UNIQUE(InvoiceNumber, CompanyID),
CONSTRAINT FK_PInv_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_PInv_Supplier FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID),
CONSTRAINT FK_PInv_Currency FOREIGN KEY (CurrencyID) REFERENCES Currencies(CurrencyID),
CONSTRAINT FK_PInv_PO FOREIGN KEY (PurchaseOrderID) REFERENCES PurchaseOrders(PurchaseOrderID),
CONSTRAINT FK_PInv_GRN FOREIGN KEY (GRNID) REFERENCES GoodsReceipts(GRNID)
);
ALTER TABLE PurchaseInvoices ADD CONSTRAINT FK_PurchaseInvoices_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
CREATE TABLE PurchaseInvoiceLines (
PILineID INT PRIMARY KEY IDENTITY(1,1),
PurchaseInvoiceID INT NOT NULL,
LineNumber INT NOT NULL,
ItemID INT NULL,
Description NVARCHAR(500),
UnitID INT NULL,
Quantity DECIMAL(18,4) DEFAULT 0,
UnitPrice DECIMAL(18,4) DEFAULT 0,
LineAmount DECIMAL(18,4) DEFAULT 0,
TaxCodeID INT NULL,
CONSTRAINT FK_PILine_Invoice FOREIGN KEY (PurchaseInvoiceID) REFERENCES PurchaseInvoices(PurchaseInvoiceID) ON DELETE CASCADE,
CONSTRAINT FK_PILine_Item FOREIGN KEY (ItemID) REFERENCES Items(ItemID),
CONSTRAINT FK_PILine_Unit FOREIGN KEY (UnitID) REFERENCES UnitsOfMeasure(UnitID),
CONSTRAINT FK_PILine_Tax FOREIGN KEY (TaxCodeID) REFERENCES TaxCodes(TaxCodeID)
);
CREATE TABLE SalesOrders (
SalesOrderID INT PRIMARY KEY IDENTITY(1,1),
SalesOrderNumber NVARCHAR(50) NOT NULL,
CompanyID INT NOT NULL,
CustomerID INT NOT NULL,
OrderDate DATE NOT NULL,
ExpectedShipDate DATE NULL,
CurrencyID INT NOT NULL,
ExchangeRate DECIMAL(18,6) DEFAULT 1,
PaymentTermID INT NULL,
Status NVARCHAR(20) DEFAULT 'Draft',
TotalAmount DECIMAL(18,4) DEFAULT 0,
TotalTax DECIMAL(18,4) DEFAULT 0,
Notes NVARCHAR(MAX),
CreatedBy INT,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT UQ_SO_Number_Company UNIQUE(SalesOrderNumber, CompanyID),
CONSTRAINT FK_SO_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_SO_Customer FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID),
CONSTRAINT FK_SO_Currency FOREIGN KEY (CurrencyID) REFERENCES Currencies(CurrencyID),
CONSTRAINT FK_SO_PaymentTerm FOREIGN KEY (PaymentTermID) REFERENCES PaymentTerms(PaymentTermID)
);
ALTER TABLE SalesOrders ADD CONSTRAINT FK_SalesOrders_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
CREATE TABLE SalesOrderDetails (
SODetailID INT PRIMARY KEY IDENTITY(1,1),
SalesOrderID INT NOT NULL,
LineNumber INT NOT NULL,
ItemID INT NOT NULL,
UnitID INT NOT NULL,
Quantity DECIMAL(18,4) NOT NULL,
UnitPrice DECIMAL(18,4) NOT NULL,
LineTotal DECIMAL(18,4) NOT NULL,
TaxCodeID INT NULL,
CONSTRAINT FK_SOD_SO FOREIGN KEY (SalesOrderID) REFERENCES SalesOrders(SalesOrderID) ON DELETE CASCADE,
CONSTRAINT FK_SOD_Item FOREIGN KEY (ItemID) REFERENCES Items(ItemID),
CONSTRAINT FK_SOD_Unit FOREIGN KEY (UnitID) REFERENCES UnitsOfMeasure(UnitID),
CONSTRAINT FK_SOD_TaxCode FOREIGN KEY (TaxCodeID) REFERENCES TaxCodes(TaxCodeID)
);
CREATE TABLE SalesInvoices (
SalesInvoiceID INT PRIMARY KEY IDENTITY(1,1),
InvoiceNumber NVARCHAR(50) NOT NULL,
CompanyID INT NOT NULL,
CustomerID INT NOT NULL,
InvoiceDate DATE NOT NULL,
DueDate DATE NULL,
CurrencyID INT NOT NULL,
ExchangeRate DECIMAL(18,6) DEFAULT 1,
SalesOrderID INT NULL,
TotalAmount DECIMAL(18,4) DEFAULT 0,
TotalTax DECIMAL(18,4) DEFAULT 0,
Status NVARCHAR(20) DEFAULT 'Draft',
CreatedBy INT,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT UQ_SInvoice_Number_Company UNIQUE(InvoiceNumber, CompanyID),
CONSTRAINT FK_SInv_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_SInv_Customer FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID),
CONSTRAINT FK_SInv_Currency FOREIGN KEY (CurrencyID) REFERENCES Currencies(CurrencyID),
CONSTRAINT FK_SInv_SO FOREIGN KEY (SalesOrderID) REFERENCES SalesOrders(SalesOrderID)
);
ALTER TABLE SalesInvoices ADD CONSTRAINT FK_SalesInvoices_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
CREATE TABLE SalesInvoiceLines (
SILineID INT PRIMARY KEY IDENTITY(1,1),
SalesInvoiceID INT NOT NULL,
LineNumber INT NOT NULL,
ItemID INT NULL,
Description NVARCHAR(500),
UnitID INT NULL,
Quantity DECIMAL(18,4) DEFAULT 0,
UnitPrice DECIMAL(18,4) DEFAULT 0,
LineAmount DECIMAL(18,4) DEFAULT 0,
TaxCodeID INT NULL,
CONSTRAINT FK_SILine_Invoice FOREIGN KEY (SalesInvoiceID) REFERENCES SalesInvoices(SalesInvoiceID) ON DELETE CASCADE,
CONSTRAINT FK_SILine_Item FOREIGN KEY (ItemID) REFERENCES Items(ItemID),
CONSTRAINT FK_SILine_Unit FOREIGN KEY (UnitID) REFERENCES UnitsOfMeasure(UnitID),
CONSTRAINT FK_SILine_Tax FOREIGN KEY (TaxCodeID) REFERENCES TaxCodes(TaxCodeID)
);
CREATE TABLE GL_FiscalPeriods (
PeriodID INT PRIMARY KEY IDENTITY(1,1),
CompanyID INT NOT NULL,
PeriodCode NVARCHAR(20) NOT NULL,
StartDate DATE NOT NULL,
EndDate DATE NOT NULL,
IsOpen BIT DEFAULT 1,
CONSTRAINT UQ_GLPeriod UNIQUE(CompanyID, PeriodCode),
CONSTRAINT FK_GLPeriod_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID)
);
CREATE TABLE GL_Journals (
JournalID INT PRIMARY KEY IDENTITY(1,1),
JournalNumber NVARCHAR(50) NOT NULL,
CompanyID INT NOT NULL,
JournalDate DATE NOT NULL,
PeriodID INT NULL,
Description NVARCHAR(500),
Status NVARCHAR(20) DEFAULT 'Draft',
CreatedBy INT,
CreatedDate DATETIME DEFAULT GETDATE(),
PostedBy INT,
PostedDate DATETIME,
CONSTRAINT UQ_Journal_Number_Company UNIQUE(JournalNumber, CompanyID),
CONSTRAINT FK_Journal_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_Journal_Period FOREIGN KEY (PeriodID) REFERENCES GL_FiscalPeriods(PeriodID)
);
ALTER TABLE GL_Journals ADD CONSTRAINT FK_GL_Journals_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
CREATE TABLE GL_JournalLines (
GLLineID INT PRIMARY KEY IDENTITY(1,1),
JournalID INT NOT NULL,
LineNumber INT NOT NULL,
AccountID INT NOT NULL,
CostCenterID INT NULL,
DebitAmount DECIMAL(18,4) DEFAULT 0,
CreditAmount DECIMAL(18,4) DEFAULT 0,
Description NVARCHAR(500),
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_GLLine_Journal FOREIGN KEY (JournalID) REFERENCES GL_Journals(JournalID) ON DELETE CASCADE,
CONSTRAINT FK_GLLine_Account FOREIGN KEY (AccountID) REFERENCES ChartOfAccounts(AccountID),
CONSTRAINT FK_GLLine_CostCenter FOREIGN KEY (CostCenterID) REFERENCES CostCenters(CostCenterID)
);
CREATE TABLE GL_Settings (
SettingID INT PRIMARY KEY IDENTITY(1,1),
CompanyID INT NOT NULL,
SettingKey NVARCHAR(100) NOT NULL,
SettingValue NVARCHAR(500),
Description NVARCHAR(500),
ModifiedBy INT,
ModifiedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT UQ_GLSetting UNIQUE(CompanyID, SettingKey),
CONSTRAINT FK_GLSetting_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID)
);
ALTER TABLE GL_Settings ADD CONSTRAINT FK_GL_Settings_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserID);
CREATE TABLE SupplierPayments (
PaymentID INT PRIMARY KEY IDENTITY(1,1),
PaymentNumber NVARCHAR(50) NOT NULL,
CompanyID INT NOT NULL,
SupplierID INT NOT NULL,
PaymentDate DATE NOT NULL,
PaymentMethod NVARCHAR(50),
BankAccountID INT NULL,
CheckNumber NVARCHAR(100),
CheckDate DATE NULL,
Amount DECIMAL(18,4) NOT NULL,
CurrencyID INT NOT NULL,
ExchangeRate DECIMAL(18,6) DEFAULT 1,
ReferenceNumber NVARCHAR(100),
Description NVARCHAR(500),
Status NVARCHAR(20) DEFAULT 'Draft',
IsReconciled BIT DEFAULT 0,
ReconciledDate DATE NULL,
CreatedBy INT NOT NULL,
CreatedDate DATETIME DEFAULT GETDATE(),
PostedBy INT NULL,
PostedDate DATETIME NULL,
ModifiedBy INT NULL,
ModifiedDate DATETIME NULL,
CONSTRAINT UQ_SupplierPayment_Number UNIQUE(PaymentNumber, CompanyID),
CONSTRAINT FK_SupplierPayment_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_SupplierPayment_Supplier FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID),
CONSTRAINT FK_SupplierPayment_Currency FOREIGN KEY (CurrencyID) REFERENCES Currencies(CurrencyID),
CONSTRAINT FK_SupplierPayment_BankAccount FOREIGN KEY (BankAccountID) REFERENCES ChartOfAccounts(AccountID)
);
ALTER TABLE SupplierPayments ADD CONSTRAINT FK_SupplierPayments_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
CREATE TABLE SupplierPaymentAllocations (
AllocationID INT PRIMARY KEY IDENTITY(1,1),
PaymentID INT NOT NULL,
PurchaseInvoiceID INT NOT NULL,
AllocatedAmount DECIMAL(18,4) NOT NULL,
DiscountAmount DECIMAL(18,4) DEFAULT 0,
WriteOffAmount DECIMAL(18,4) DEFAULT 0,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_PaymentAlloc_Payment FOREIGN KEY (PaymentID) REFERENCES SupplierPayments(PaymentID) ON DELETE CASCADE,
CONSTRAINT FK_PaymentAlloc_Invoice FOREIGN KEY (PurchaseInvoiceID) REFERENCES PurchaseInvoices(PurchaseInvoiceID),
CONSTRAINT UQ_PaymentAllocation UNIQUE(PaymentID, PurchaseInvoiceID)
);
CREATE TABLE CustomerPayments (
PaymentID INT PRIMARY KEY IDENTITY(1,1),
PaymentNumber NVARCHAR(50) NOT NULL,
CompanyID INT NOT NULL,
CustomerID INT NOT NULL,
PaymentDate DATE NOT NULL,
PaymentMethod NVARCHAR(50),
BankAccountID INT NULL,
CheckNumber NVARCHAR(100),
CheckDate DATE NULL,
Amount DECIMAL(18,4) NOT NULL,
CurrencyID INT NOT NULL,
ExchangeRate DECIMAL(18,6) DEFAULT 1,
ReferenceNumber NVARCHAR(100),
Description NVARCHAR(500),
Status NVARCHAR(20) DEFAULT 'Draft',
IsReconciled BIT DEFAULT 0,
ReconciledDate DATE NULL,
CreatedBy INT NOT NULL,
CreatedDate DATETIME DEFAULT GETDATE(),
PostedBy INT NULL,
PostedDate DATETIME NULL,
ModifiedBy INT NULL,
ModifiedDate DATETIME NULL,
CONSTRAINT UQ_CustomerPayment_Number UNIQUE(PaymentNumber, CompanyID),
CONSTRAINT FK_CustomerPayment_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_CustomerPayment_Customer FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID),
CONSTRAINT FK_CustomerPayment_Currency FOREIGN KEY (CurrencyID) REFERENCES Currencies(CurrencyID),
CONSTRAINT FK_CustomerPayment_BankAccount FOREIGN KEY (BankAccountID) REFERENCES ChartOfAccounts(AccountID)
);
ALTER TABLE CustomerPayments ADD CONSTRAINT FK_CustomerPayments_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
CREATE TABLE CustomerPaymentAllocations (
AllocationID INT PRIMARY KEY IDENTITY(1,1),
PaymentID INT NOT NULL,
SalesInvoiceID INT NOT NULL,
AllocatedAmount DECIMAL(18,4) NOT NULL,
DiscountAmount DECIMAL(18,4) DEFAULT 0,
WriteOffAmount DECIMAL(18,4) DEFAULT 0,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_CustPaymentAlloc_Payment FOREIGN KEY (PaymentID) REFERENCES CustomerPayments(PaymentID) ON DELETE CASCADE,
CONSTRAINT FK_CustPaymentAlloc_Invoice FOREIGN KEY (SalesInvoiceID) REFERENCES SalesInvoices(SalesInvoiceID),
CONSTRAINT UQ_CustPaymentAllocation UNIQUE(PaymentID, SalesInvoiceID)
);
CREATE TABLE ExpensePayments (
ExpenseID INT PRIMARY KEY IDENTITY(1,1),
ExpenseNumber NVARCHAR(50) NOT NULL,
CompanyID INT NOT NULL,
ExpenseDate DATE NOT NULL,
SupplierID INT NULL,
ExpenseType NVARCHAR(100),
Description NVARCHAR(500),
Amount DECIMAL(18,4) NOT NULL,
TaxAmount DECIMAL(18,4) DEFAULT 0,
TotalAmount DECIMAL(18,4) NOT NULL,
CurrencyID INT NOT NULL,
ExchangeRate DECIMAL(18,6) DEFAULT 1,
PaymentMethod NVARCHAR(50),
BankAccountID INT NULL,
CostCenterID INT NULL,
Status NVARCHAR(20) DEFAULT 'Draft',
CreatedBy INT NOT NULL,
CreatedDate DATETIME DEFAULT GETDATE(),
PostedBy INT NULL,
PostedDate DATETIME NULL,
CONSTRAINT UQ_Expense_Number UNIQUE(ExpenseNumber, CompanyID),
CONSTRAINT FK_Expense_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_Expense_Supplier FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID),
CONSTRAINT FK_Expense_Currency FOREIGN KEY (CurrencyID) REFERENCES Currencies(CurrencyID),
CONSTRAINT FK_Expense_BankAccount FOREIGN KEY (BankAccountID) REFERENCES ChartOfAccounts(AccountID),
CONSTRAINT FK_Expense_CostCenter FOREIGN KEY (CostCenterID) REFERENCES CostCenters(CostCenterID)
);
ALTER TABLE ExpensePayments ADD CONSTRAINT FK_ExpensePayments_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
CREATE TABLE ErrorHandlingSettings (
SettingID INT PRIMARY KEY IDENTITY(1,1),
CompanyID INT NOT NULL,
ErrorCode NVARCHAR(50) NOT NULL,
ErrorSeverity NVARCHAR(20),
AutoRetry BIT DEFAULT 0,
MaxRetryCount INT DEFAULT 3,
NotificationEmail NVARCHAR(200),
IsActive BIT DEFAULT 1,
CONSTRAINT FK_ErrorSetting_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID)
);
CREATE TABLE ReportCache (
CacheID BIGINT PRIMARY KEY IDENTITY(1,1),
ReportType NVARCHAR(100) NOT NULL,
CompanyID INT NOT NULL,
Parameters NVARCHAR(1000) NOT NULL,
ReportData NVARCHAR(MAX) NOT NULL,
CreatedBy INT NOT NULL,
CreatedDate DATETIME DEFAULT GETDATE(),
ExpiryDate DATETIME NOT NULL,
IsCompressed BIT DEFAULT 0,
CONSTRAINT UQ_ReportCache UNIQUE (ReportType, CompanyID, Parameters)
);
ALTER TABLE ReportCache ADD CONSTRAINT FK_ReportCache_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
CREATE TABLE QueryPerformanceLog (
LogID BIGINT PRIMARY KEY IDENTITY(1,1),
QueryName NVARCHAR(200) NOT NULL,
CompanyID INT NULL,
ExecutionTimeMs INT NOT NULL,
RowCount INT NOT NULL,
Parameters NVARCHAR(500),
ExecutionDate DATETIME DEFAULT GETDATE(),
UserID INT NULL
);
CREATE TABLE PerformanceSettings (
SettingID INT PRIMARY KEY IDENTITY(1,1),
CompanyID INT NOT NULL,
SettingKey NVARCHAR(100) NOT NULL,
SettingValue NVARCHAR(500),
Description NVARCHAR(500),
ModifiedBy INT,
ModifiedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT UQ_PerformanceSetting UNIQUE(CompanyID, SettingKey)
);
ALTER TABLE PerformanceSettings ADD CONSTRAINT FK_PerformanceSettings_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserID);
CREATE TABLE BackupHistory (
BackupID INT PRIMARY KEY IDENTITY(1,1),
BackupType NVARCHAR(20) NOT NULL,
BackupPath NVARCHAR(500) NOT NULL,
BackupSizeMB DECIMAL(18,2),
StartTime DATETIME NOT NULL,
EndTime DATETIME NULL,
Status NVARCHAR(20) NOT NULL,
CreatedBy INT NOT NULL,
CONSTRAINT CHK_BackupType CHECK (BackupType IN ('FULL', 'DIFFERENTIAL', 'LOG'))
);
ALTER TABLE BackupHistory ADD CONSTRAINT FK_BackupHistory_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
CREATE TABLE Projects (
ProjectID INT PRIMARY KEY IDENTITY(1,1),
ProjectCode NVARCHAR(50) UNIQUE NOT NULL,
ProjectNameAr NVARCHAR(200) NOT NULL,
ProjectNameEn NVARCHAR(200),
CompanyID INT NOT NULL,
StartDate DATE,
EndDate DATE,
Budget DECIMAL(18,4) DEFAULT 0,
ActualCost DECIMAL(18,4) DEFAULT 0,
Status NVARCHAR(20) DEFAULT 'Active',
ProjectManagerID INT NULL,
CostCenterID INT NULL,
Description NVARCHAR(500),
IsActive BIT DEFAULT 1,
CreatedBy INT,
CreatedDate DATETIME DEFAULT GETDATE(),
ModifiedBy INT,
ModifiedDate DATETIME,
CONSTRAINT FK_Project_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_Project_CostCenter FOREIGN KEY (CostCenterID) REFERENCES CostCenters(CostCenterID)
);
ALTER TABLE Projects ADD CONSTRAINT FK_Projects_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);
ALTER TABLE Projects ADD CONSTRAINT FK_Projects_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserID);
CREATE TABLE Employees (
EmployeeID INT PRIMARY KEY IDENTITY(1,1),
EmployeeCode NVARCHAR(50) UNIQUE NOT NULL,
EmployeeNameAr NVARCHAR(200) NOT NULL,
EmployeeNameEn NVARCHAR(200),
CompanyID INT NOT NULL,
DepartmentID INT NULL,
Position NVARCHAR(100),
HireDate DATE,
Salary DECIMAL(18,4) DEFAULT 0,
CurrencyID INT NULL,
CostCenterID INT NULL,
Email NVARCHAR(100),
Phone NVARCHAR(50),
Mobile NVARCHAR(50),
Address NVARCHAR(500),
IsActive BIT DEFAULT 1,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_Employee_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_Employee_Currency FOREIGN KEY (CurrencyID) REFERENCES Currencies(CurrencyID),
CONSTRAINT FK_Employee_CostCenter FOREIGN KEY (CostCenterID) REFERENCES CostCenters(CostCenterID)
);
CREATE TABLE Departments (
DepartmentID INT PRIMARY KEY IDENTITY(1,1),
DepartmentCode NVARCHAR(50) UNIQUE NOT NULL,
DepartmentNameAr NVARCHAR(200) NOT NULL,
DepartmentNameEn NVARCHAR(200),
ParentDepartmentID INT NULL,
CostCenterID INT NULL,
ManagerID INT NULL,
IsActive BIT DEFAULT 1,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_Department_Parent FOREIGN KEY (ParentDepartmentID) REFERENCES Departments(DepartmentID),
CONSTRAINT FK_Department_CostCenter FOREIGN KEY (CostCenterID) REFERENCES CostCenters(CostCenterID)
);
CREATE TABLE FixedAssets (
AssetID INT PRIMARY KEY IDENTITY(1,1),
AssetCode NVARCHAR(50) UNIQUE NOT NULL,
AssetNameAr NVARCHAR(200) NOT NULL,
AssetNameEn NVARCHAR(200),
CompanyID INT NOT NULL,
AssetCategoryID INT NOT NULL,
PurchaseDate DATE,
PurchaseCost DECIMAL(18,4) NOT NULL,
CurrentValue DECIMAL(18,4) NOT NULL,
DepreciationMethod NVARCHAR(50),
UsefulLife INT,
SalvageValue DECIMAL(18,4) DEFAULT 0,
Location NVARCHAR(200),
CustodianID INT NULL,
Status NVARCHAR(20) DEFAULT 'Active',
IsActive BIT DEFAULT 1,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_Asset_Company FOREIGN KEY (CompanyID) REFERENCES Companies(CompanyID),
CONSTRAINT FK_Asset_Employee FOREIGN KEY (CustodianID) REFERENCES Employees(EmployeeID)
);
CREATE TABLE AssetCategories (
CategoryID INT PRIMARY KEY IDENTITY(1,1),
CategoryCode NVARCHAR(50) UNIQUE NOT NULL,
CategoryNameAr NVARCHAR(200) NOT NULL,
CategoryNameEn NVARCHAR(200),
DepreciationRate DECIMAL(5,2),
GLAccountID INT NULL,
IsActive BIT DEFAULT 1
);
-- تحسينات Partitioning للأداء
ALTER DATABASE CURRENT ADD FILEGROUP [FG_Inventory_Current];
ALTER DATABASE CURRENT ADD FILEGROUP [FG_Inventory_History];
ALTER DATABASE CURRENT ADD FILEGROUP [FG_Audit_Current];
ALTER DATABASE CURRENT ADD FILEGROUP [FG_Audit_History];
CREATE PARTITION FUNCTION pf_InventoryDate (DATE)
AS RANGE RIGHT FOR VALUES (
DATEADD(MONTH, -6, GETDATE()),
GETDATE()
);
CREATE PARTITION SCHEME ps_InventoryDate
AS PARTITION pf_InventoryDate
TO ([FG_Inventory_Current], [FG_Inventory_Current], [FG_Inventory_History]);
CREATE TABLE InventoryLedger_Partitioned (
LedgerID BIGINT PRIMARY KEY IDENTITY(1,1),
TransactionID INT NOT NULL,
DetailID INT NOT NULL,
CompanyID INT NOT NULL,
WarehouseID INT NOT NULL,
ItemID INT NOT NULL,
TransactionDate DATETIME NOT NULL,
TransactionType NVARCHAR(20),
Quantity DECIMAL(18,4) NOT NULL,
UnitCost DECIMAL(18,4) NOT NULL,
TotalCost DECIMAL(18,4) NOT NULL,
BalanceQuantity DECIMAL(18,4),
BalanceValue DECIMAL(18,4),
BatchNumber NVARCHAR(100),
SerialNumber NVARCHAR(100),
LocationID INT NULL,
CostCenterID INT NULL,
CreatedDate DATETIME DEFAULT GETDATE(),
PartitionKey AS CONVERT(DATE, TransactionDate) PERSISTED
) ON ps_InventoryDate(PartitionKey);
CREATE COLUMNSTORE INDEX IX_InventoryLedger_Columnstore
ON InventoryLedger_Partitioned (
CompanyID, WarehouseID, ItemID, TransactionDate,
Quantity, UnitCost, TotalCost, TransactionType
);
-- فهارس إضافية محسنة
CREATE INDEX IX_InventoryTransactions_Complex ON InventoryTransactions
(CompanyID, TransactionDate, Status)
INCLUDE (TotalQuantity, TotalValue, WarehouseID);
CREATE INDEX IX_StockBalances_Performance ON StockBalances
(CompanyID, ItemID, WarehouseID)
INCLUDE (QuantityOnHand, QuantityAvailable, AverageCost);
CREATE INDEX IX_InventoryLedger_Reporting ON InventoryLedger
(TransactionDate, CompanyID, ItemID, WarehouseID)
INCLUDE (Quantity, UnitCost, TotalCost, TransactionType);
CREATE INDEX IX_Items_Search ON Items
(ItemCode, ItemNameAr, ItemNameEn, IsActive);
CREATE INDEX IX_Customers_Search ON Customers
(CustomerCode, CustomerNameAr, CustomerNameEn, CompanyID);
-- إجراءات مخزنة للأداء
CREATE PROCEDURE sp_CleanupHistoricalData
@RetentionMonths INT = 24
AS
BEGIN
DELETE FROM InventoryLedger
WHERE TransactionDate < DATEADD(MONTH, -@RetentionMonths, GETDATE());
DELETE FROM AuditLog
WHERE ActionDate < DATEADD(MONTH, -@RetentionMonths, GETDATE());
DELETE FROM QueryPerformanceLog
WHERE ExecutionDate < DATEADD(MONTH, -@RetentionMonths, GETDATE());
END;
CREATE PROCEDURE sp_RebuildIndexes
@FragmentationThreshold DECIMAL(5,2) = 30.0
AS
BEGIN
DECLARE @TableName NVARCHAR(255)
DECLARE @IndexName NVARCHAR(255)
DECLARE @Fragmentation DECIMAL(5,2)
DECLARE index_cursor CURSOR FOR
SELECT
OBJECT_NAME(ips.object_id) AS TableName,
si.name AS IndexName,
ips.avg_fragmentation_in_percent
FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') ips
INNER JOIN sys.indexes si ON ips.object_id = si.object_id AND ips.index_id = si.index_id
WHERE ips.avg_fragmentation_in_percent > @FragmentationThreshold
AND si.name IS NOT NULL
OPEN index_cursor
FETCH NEXT FROM index_cursor INTO @TableName, @IndexName, @Fragmentation
WHILE @@FETCH_STATUS = 0
BEGIN
IF @Fragmentation > 50
EXEC('ALTER INDEX [' + @IndexName + '] ON [' + @TableName + '] REBUILD')
ELSE
EXEC('ALTER INDEX [' + @IndexName + '] ON [' + @TableName + '] REORGANIZE')
FETCH NEXT FROM index_cursor INTO @TableName, @IndexName, @Fragmentation
END
CLOSE index_cursor
DEALLOCATE index_cursor
END;
-- تحسينات الأمان
CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'StrongPassword123!';
CREATE CERTIFICATE MyCertificate WITH SUBJECT = 'Data Encryption Certificate';
CREATE SYMMETRIC KEY MySymmetricKey
WITH ALGORITHM = AES_256
ENCRYPTION BY CERTIFICATE MyCertificate;
ALTER TABLE Users ADD
EncryptedEmail VARBINARY(256),
EncryptedPhone VARBINARY(256);
CREATE TABLE SecurityBreachAttempts (
AttemptID BIGINT PRIMARY KEY IDENTITY(1,1),
IPAddress NVARCHAR(50) NOT NULL,
Username NVARCHAR(50) NULL,
AttemptType NVARCHAR(50) NOT NULL,
AttemptDate DATETIME DEFAULT GETDATE(),
Details NVARCHAR(1000)
);
-- نظام المهام والتقويم
CREATE TABLE Tasks (
TaskID INT PRIMARY KEY IDENTITY(1,1),
TaskTitle NVARCHAR(200) NOT NULL,
TaskDescription NVARCHAR(1000),
AssignedTo INT NOT NULL,
AssignedBy INT NOT NULL,
CompanyID INT NOT NULL,
DueDate DATETIME,
Priority NVARCHAR(20) DEFAULT 'Medium',
Status NVARCHAR(20) DEFAULT 'Pending',
RelatedTable NVARCHAR(100),
RelatedRecordID INT,
CompletionDate DATETIME NULL,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_Task_AssignedTo FOREIGN KEY (AssignedTo) REFERENCES Users(UserID),
CONSTRAINT FK_Task_AssignedBy FOREIGN KEY (AssignedBy) REFERENCES Users(UserID)
);
CREATE TABLE CalendarEvents (
EventID INT PRIMARY KEY IDENTITY(1,1),
EventTitle NVARCHAR(200) NOT NULL,
EventDescription NVARCHAR(1000),
StartDate DATETIME NOT NULL,
EndDate DATETIME NOT NULL,
CompanyID INT NOT NULL,
CreatedBy INT NOT NULL,
IsAllDay BIT DEFAULT 0,
EventType NVARCHAR(50),
Location NVARCHAR(200),
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT FK_Event_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserID)
);
-- تحسينات إضافية
CREATE TABLE UsageStatistics (
StatID BIGINT PRIMARY KEY IDENTITY(1,1),
UserID INT NOT NULL,
CompanyID INT NOT NULL,
FormName NVARCHAR(100) NOT NULL,
ActionType NVARCHAR(50) NOT NULL,
DurationMs INT,
RecordCount INT,
AccessDate DATETIME DEFAULT GETDATE()
);
CREATE TABLE CustomizationSettings (
CustomizationID INT PRIMARY KEY IDENTITY(1,1),
CompanyID INT NOT NULL,
UserID INT NULL,
FormName NVARCHAR(100) NOT NULL,
CustomizationType NVARCHAR(50) NOT NULL,
CustomizationData NVARCHAR(MAX) NOT NULL,
IsActive BIT DEFAULT 1,
CreatedDate DATETIME DEFAULT GETDATE(),
CONSTRAINT UQ_Customization UNIQUE(CompanyID, UserID, FormName, CustomizationType)
);