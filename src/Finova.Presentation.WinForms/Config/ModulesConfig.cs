using System.Collections.Generic;

namespace Finova.Presentation.WinForms.Config
{
    public sealed class ModulesConfig
    {
        public List<ModuleItem> Modules { get; set; } = new();
    }

    public sealed class ModuleItem
    {
        public string Key { get; set; } = "";
        public string Name { get; set; } = "";
        public bool Enabled { get; set; }
    }
}
