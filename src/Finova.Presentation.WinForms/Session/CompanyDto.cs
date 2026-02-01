using System;

namespace Finova.Presentation.WinForms.Session
{
    /// <summary>
    /// Minimal Company DTO used by session & company switch UI.
    /// </summary>
    public sealed class CompanyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public override string ToString() => Name;
    }
}
