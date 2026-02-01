using System;
using System.Windows.Forms;

namespace Finova.Presentation.WinForms.Modules.Shell.Views
{
    public sealed partial class ShellForm
    {
        public void OpenTab(string key, string title, Control content, bool activate = true)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Tab key is required.", nameof(key));

            if (_tabByKey.TryGetValue(key, out var existing))
            {
                if (content != null)
                {
                    existing.Controls.Clear();
                    content.Dock = DockStyle.Fill;
                    existing.Controls.Add(content);
                }

                if (activate)
                    _tabs.SelectedTab = existing;

                return;
            }

            var page = new TabPage(title ?? key) { Name = key };

            if (content != null)
            {
                content.Dock = DockStyle.Fill;
                page.Controls.Add(content);
            }

            _tabs.TabPages.Add(page);
            _tabByKey[key] = page;

            if (activate)
                _tabs.SelectedTab = page;
        }
    }
}
