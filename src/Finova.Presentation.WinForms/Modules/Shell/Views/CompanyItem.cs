using System;

namespace Finova.Presentation.WinForms.Modules.Shell.Views
{
    internal sealed class CompanyItem
    {
        public CompanyItem(Guid id, string name) { Id = id; Name = name; }
        public Guid Id { get; }
        public string Name { get; }
    }
}
