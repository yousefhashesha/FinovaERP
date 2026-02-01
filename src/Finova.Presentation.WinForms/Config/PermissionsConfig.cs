using System.Collections.Generic;

namespace Finova.Presentation.WinForms.Config
{
    public sealed class PermissionsConfig
    {
        public List<PermissionItem> Permissions { get; set; } = new();
    }

    public sealed class PermissionItem
    {
        public string Key { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
