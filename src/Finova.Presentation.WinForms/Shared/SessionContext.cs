namespace Finova.Presentation.WinForms.Shared
{
    /// <summary>
    /// Lightweight session state (Phase 1). Later becomes full user/company context.
    /// </summary>
    public sealed class SessionContext
    {
        public bool IsAuthenticated { get; private set; }

        public string CompanyName { get; private set; } = "Demo Company";
        public string UserDisplayName { get; private set; } = "guest";

        public void SignIn(string companyName, string userDisplayName)
        {
            CompanyName = string.IsNullOrWhiteSpace(companyName) ? "Demo Company" : companyName;
            UserDisplayName = string.IsNullOrWhiteSpace(userDisplayName) ? "user" : userDisplayName;
            IsAuthenticated = true;
        }

        public void SignOut()
        {
            IsAuthenticated = false;
            UserDisplayName = "guest";
        }
    }
}
