namespace FinovaERP.Shared.Constants
{
    public static class ApplicationConstants
    {
        public const string ApplicationName = "FinovaERP";
        public const string Version = "2.0.0";
        public const string DefaultLanguage = "ar-SA";
        public const string DefaultCurrency = "SAR";
        public const string DateFormat = "yyyy-MM-dd";
        public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    }

    public static class RoleConstants
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string User = "User";
        public const string Accountant = "Accountant";
        public const string InventoryManager = "InventoryManager";
    }

    public static class PermissionConstants
    {
        public const string ViewDashboard = "ViewDashboard";
        public const string ManageUsers = "ManageUsers";
        public const string ManageCompanies = "ManageCompanies";
        public const string ViewReports = "ViewReports";
        public const string ManageInventory = "ManageInventory";
        public const string ManageSales = "ManageSales";
        public const string ManagePurchasing = "ManagePurchasing";
    }
}
