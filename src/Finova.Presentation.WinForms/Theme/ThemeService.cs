using System;
using System.Drawing;
using Finova.Presentation.WinForms.Config;

namespace Finova.Presentation.WinForms.Theme
{
    public sealed class ThemeService
    {
        public ThemeService(BrandingOptions branding)
        {
            Branding = branding ?? throw new ArgumentNullException(nameof(branding));
            Accent = ParseColor(Branding.AccentColor, Color.FromArgb(45, 108, 223));
        }

        public BrandingOptions Branding { get; }
        public Color Accent { get; }

        public Color SidebarBack => Color.FromArgb(18, 24, 38);
        public Color SidebarItem => Color.FromArgb(26, 32, 48);
        public Color SidebarItemActive => Color.FromArgb(32, 44, 74);
        public Color HeaderText => Color.FromArgb(25, 42, 86);
        public Color MutedText => Color.FromArgb(80, 90, 110);

        private static Color ParseColor(string hex, Color fallback)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hex)) return fallback;
                if (hex.StartsWith("#")) hex = hex[1..];
                if (hex.Length == 6)
                {
                    var r = Convert.ToInt32(hex.Substring(0, 2), 16);
                    var g = Convert.ToInt32(hex.Substring(2, 2), 16);
                    var b = Convert.ToInt32(hex.Substring(4, 2), 16);
                    return Color.FromArgb(r, g, b);
                }
                return fallback;
            }
            catch
            {
                return fallback;
            }
        }
    }
}
