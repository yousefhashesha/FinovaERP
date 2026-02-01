namespace Finova.Presentation.WinForms.Modules.Shell.Contracts
{
    /// <summary>
    /// Sidebar navigation item.
    /// </summary>
    public sealed class ShellNavItem
    {
        public ShellNavItem(string key, string title, bool enabled)
        {
            Key = key;
            Title = title;
            Enabled = enabled;
        }

        public string Key { get; }
        public string Title { get; }
        public bool Enabled { get; }
    }
}
