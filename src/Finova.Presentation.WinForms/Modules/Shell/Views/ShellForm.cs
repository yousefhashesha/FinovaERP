using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Finova.Presentation.WinForms.Modules.Shell.Contracts;
using Finova.Presentation.WinForms.Theme;

namespace Finova.Presentation.WinForms.Modules.Shell.Views
{
    public sealed partial class ShellForm : Form, IShellView
        private sealed class CompanyItem
        {
            public CompanyItem(System.Guid id, string name) { Id = id; Name = name; }
            public System.Guid Id { get; }
            public string Name { get; }
            public override string ToString() => Name;
        }

    {
        private readonly Label _lblCompany;
        private readonly Label _lblUser;
        private readonly Label _lblPeriod;

        private readonly ComboBox _cmbCompany;
        private readonly TextBox _txtSearch;
        private readonly Button _btnSearch;

        private readonly TabControl _tabs;

        private readonly Panel _sidebar;
        private readonly Button _btnLogout;

        private readonly Dictionary<string, TabPage> _tabByKey = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Button> _navButtons = new(StringComparer.OrdinalIgnoreCase);

        private ThemeService? _theme;

        public event EventHandler<string>? NavigationRequested;
        public event EventHandler? LogoutRequested;

        public event EventHandler<string>? SearchRequested;
        public event EventHandler<string>? CompanyChanged;

        // Tabs UX constants
        private const int CloseBoxSize = 12;
        private const int CloseBoxPadding = 8;

        public ShellForm()
        {
            Text = "Finova ERP";
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            Controls.Add(root);

            _sidebar = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(18, 24, 38) };
            root.Controls.Add(_sidebar, 0, 0);

            var right = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            right.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
            right.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.Controls.Add(right, 1, 0);

            // Header
            var header = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            right.Controls.Add(header, 0, 0);

            _lblCompany = new Label
            {
                Text = "Company",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 42, 86),
                AutoSize = true,
                Location = new Point(18, 10)
            };
            header.Controls.Add(_lblCompany);

            _lblUser = new Label
            {
                Text = "User",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(80, 90, 110),
                AutoSize = true,
                Location = new Point(18, 40)
            };
            header.Controls.Add(_lblUser);

            _lblPeriod = new Label
            {
                Text = "Period",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(110, 120, 140),
                AutoSize = true,
                Location = new Point(220, 44)
            };
            header.Controls.Add(_lblPeriod);

            // Company switch (right side)
            _cmbCompany = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 200,
                Font = new Font("Segoe UI", 9),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            header.Controls.Add(_cmbCompany);

            _cmbCompany.SelectedIndexChanged += (_, __) =>
            {
                if (_cmbCompany.SelectedItem is string s && s.Length > 0)
                    CompanyChanged?.Invoke(this, s);
            };

            // Search box (right side)
            _txtSearch = new TextBox
            {
                Width = 240,
                Font = new Font("Segoe UI", 9),
                PlaceholderText = "Search… (Stub)",
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            header.Controls.Add(_txtSearch);

            _btnSearch = new Button
            {
                Text = "Search",
                Width = 90,
                Height = 28,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _btnSearch.FlatAppearance.BorderSize = 0;
            header.Controls.Add(_btnSearch);

            _btnSearch.Click += (_, __) =>
            {
                var q = _txtSearch.Text ?? string.Empty;
                SearchRequested?.Invoke(this, q);
            };

            _txtSearch.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var q = _txtSearch.Text ?? string.Empty;
                    SearchRequested?.Invoke(this, q);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };

            header.Resize += (_, __) =>
            {
                // Layout on resize (right aligned controls)
                int rightX = header.Width - 18;

                _btnSearch.Top = 20;
                _btnSearch.Left = rightX - _btnSearch.Width;
                rightX = _btnSearch.Left - 10;

                _txtSearch.Top = 21;
                _txtSearch.Left = rightX - _txtSearch.Width;
                rightX = _txtSearch.Left - 14;

                _cmbCompany.Top = 20;
                _cmbCompany.Left = rightX - _cmbCompany.Width;
            };
            header.PerformLayout();

            // Workspace tabs
            _tabs = new TabControl { Dock = DockStyle.Fill };
            right.Controls.Add(_tabs, 0, 1);

            _tabs.DrawMode = TabDrawMode.OwnerDrawFixed;
            _tabs.Padding = new Point(18, 6);
            _tabs.DrawItem += Tabs_DrawItem;
            _tabs.MouseDown += Tabs_MouseDown;

            _btnLogout = new Button
            {
                Text = "Logout",
                Width = 216,
                Height = 38,
                FlatStyle = FlatStyle.Flat
            };
            _btnLogout.FlatAppearance.BorderSize = 0;
            _btnLogout.Click += (_, __) => LogoutRequested?.Invoke(this, EventArgs.Empty);

            // Phase 2.1 Hotfix: inject Accounting tab open without changing ShellPresenter
            WireAccountingHotfix();
        }

        public void SetHeader(string companyName, string userDisplayName, string periodText)
        {
            _lblCompany.Text = companyName;
            _lblUser.Text = userDisplayName;
            _lblPeriod.Text = periodText;
        }

        public void SetCompanies(IEnumerable<(System.Guid Id, string Name)> companies, System.Guid? selectedCompanyId)
        {
            _cmbCompany.BeginUpdate();
            _cmbCompany.Items.Clear();

            foreach (var c in companies ?? Array.Empty<string>())
                _cmbCompany.Items.Add(c);

            _cmbCompany.EndUpdate();

            if (!string.IsNullOrWhiteSpace(selectedCompany))
                _cmbCompany.SelectedItem = selectedCompany;
            else if (_cmbCompany.Items.Count > 0)
                _cmbCompany.SelectedIndex = 0;
        }

        public void ApplyNavigation(IEnumerable<ShellNavItem> items)
        {
            _sidebar.Controls.Clear();
            _navButtons.Clear();

            var brand = new Label
            {
                Text = _theme?.Branding.SystemName ?? "Finova",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(22, 18)
            };
            _sidebar.Controls.Add(brand);

            var y = 72;
            foreach (var item in items.Where(i => i.Enabled))
            {
                var btn = MakeNavButton(item.Title, item.Key, 22, y);
                _sidebar.Controls.Add(btn);
                _navButtons[item.Key] = btn;
                y += 46;
            }

            _btnLogout.Location = new Point(22, y + 18);
            _sidebar.Controls.Add(_btnLogout);

            ApplyThemeIfReady();
        }

        public void SetActiveNav(string key)
        {
            foreach (var kv in _navButtons)
                kv.Value.BackColor = _theme?.SidebarItem ?? Color.FromArgb(26, 32, 48);

            if (_navButtons.TryGetValue(key, out var active))
                active.BackColor = _theme?.SidebarItemActive ?? Color.FromArgb(32, 44, 74);
        }

        public void OpenTab(string key, string title)
        {
            if (_tabByKey.TryGetValue(key, out var existing))
            {
                _tabs.SelectedTab = existing;
                return;
            }

            var page = new TabPage(title) { BackColor = Color.White };
            page.Tag = key;

            var lbl = new Label
            {
                Text = $"{title} (Phase 1 placeholder)",
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = _theme?.HeaderText ?? Color.FromArgb(25, 42, 86),
                Location = new Point(18, 18)
            };
            page.Controls.Add(lbl);

            _tabs.TabPages.Add(page);
            _tabByKey[key] = page;
            _tabs.SelectedTab = page;

            _tabs.Invalidate();
        }

        public void ShowInfo(string message, string title = "Info")
        {
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private Button MakeNavButton(string text, string key, int x, int y)
        {
            var btn = new Button
            {
                Text = text,
                Width = 216,
                Height = 38,
                Location = new Point(x, y),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(26, 32, 48),
                TextAlign = ContentAlignment.MiddleLeft
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (_, __) => NavigationRequested?.Invoke(this, key);
            return btn;
        }

        public void InjectTheme(ThemeService theme)
        {
            _theme = theme;
            ApplyThemeIfReady();
            _tabs.Invalidate();
        }

        private void ApplyThemeIfReady()
        {
            if (_theme is null) return;

            _sidebar.BackColor = _theme.SidebarBack;

            foreach (var btn in _navButtons.Values)
                btn.BackColor = _theme.SidebarItem;

            _btnLogout.BackColor = _theme.Accent;
            _btnLogout.ForeColor = Color.White;

            _btnSearch.BackColor = Color.FromArgb(240, 240, 240);
            _btnSearch.ForeColor = Color.FromArgb(40, 40, 40);

            _lblCompany.ForeColor = _theme.HeaderText;
            _lblUser.ForeColor = _theme.MutedText;
        }

        // -------- Tabs Close X --------
        private void Tabs_DrawItem(object? sender, DrawItemEventArgs e)
        {
            var tab = _tabs.TabPages[e.Index];
            var tabRect = _tabs.GetTabRect(e.Index);
            var key = tab.Tag as string ?? tab.Text;

            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            var back = isSelected ? Color.White : Color.FromArgb(245, 247, 250);
            using var backBrush = new SolidBrush(back);
            e.Graphics.FillRectangle(backBrush, tabRect);

            var textColor = _theme?.HeaderText ?? Color.FromArgb(25, 42, 86);
            using var textBrush = new SolidBrush(textColor);

            var textRect = new Rectangle(tabRect.X + 10, tabRect.Y + 6, tabRect.Width - 20, tabRect.Height - 8);

            bool closable = !string.Equals(key, "HOME", StringComparison.OrdinalIgnoreCase);
            if (closable)
                textRect.Width -= (CloseBoxSize + CloseBoxPadding + 4);

            e.Graphics.DrawString(tab.Text, Font, textBrush, textRect);

            if (closable)
            {
                var closeRect = GetCloseRect(tabRect);
                using var pen = new Pen(Color.FromArgb(120, 120, 120), 2);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.DrawLine(pen, closeRect.Left, closeRect.Top, closeRect.Right, closeRect.Bottom);
                e.Graphics.DrawLine(pen, closeRect.Right, closeRect.Top, closeRect.Left, closeRect.Bottom);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            }
        }

        private void Tabs_MouseDown(object? sender, MouseEventArgs e)
        {
            for (int i = 0; i < _tabs.TabPages.Count; i++)
            {
                var tabRect = _tabs.GetTabRect(i);
                var tab = _tabs.TabPages[i];
                var key = tab.Tag as string ?? tab.Text;

                bool closable = !string.Equals(key, "HOME", StringComparison.OrdinalIgnoreCase);
                if (!closable) continue;

                var closeRect = GetCloseRect(tabRect);
                if (closeRect.Contains(e.Location))
                {
                    CloseTabByKey(key);
                    return;
                }
            }
        }

        private Rectangle GetCloseRect(Rectangle tabRect)
        {
            int x = tabRect.Right - CloseBoxPadding - CloseBoxSize;
            int y = tabRect.Top + (tabRect.Height - CloseBoxSize) / 2;
            return new Rectangle(x, y, CloseBoxSize, CloseBoxSize);
        }

        private void CloseTabByKey(string key)
        {
            if (!_tabByKey.TryGetValue(key, out var page))
                return;

            _tabs.TabPages.Remove(page);
            _tabByKey.Remove(key);

            if (_tabs.TabPages.Count > 0)
            {
                _tabs.SelectedIndex = 0;
                SetActiveNav("HOME");
            }

            _tabs.Invalidate();
        }



        // Phase 2.1 Hotfix: Open Accounting module without depending on ShellPresenter/IAppSession signatures
        private void WireAccountingHotfix()
        {
            try
            {
                // Hook sidebar buttons AFTER ApplyNavigation rebuilds them.
                // We attach to NavigationRequested event (already raised by sidebar buttons).
                NavigationRequested += (_, key) =>
                {
                    if (string.Equals(key, "accounting", StringComparison.OrdinalIgnoreCase))
                    {
                        // Lazy create AccountingHomeControl via reflection to avoid DI assumptions
                        var t = Type.GetType("Finova.Presentation.WinForms.Modules.Accounting.Views.AccountingHomeControl, Finova.Presentation.WinForms");
                        if (t == null) { MessageBox.Show("AccountingHomeControl type not found."); return; }
                        var control = Activator.CreateInstance(t) as Control;
                        if (control == null) { MessageBox.Show("Cannot create AccountingHomeControl."); return; }
                        OpenTabHost("accounting", "Accounting", control, true);
                    }
                };

                // Fallback: if no accounting nav exists, open it under Home on startup once
                // (This avoids being blocked by permissions while you are still wiring RBAC)
                this.Shown += (_, __) =>
                {
                    if (!_tabByKey.ContainsKey("home") && _tabs.TabPages.Count == 0)
                    {
                        OpenTabHost("home", "Home", new Label { Text = "Home", AutoSize = true });
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hotfix", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        // Adding the missing OpenTabHost method to resolve CS0103 error.
        private void OpenTabHost(string key, string title, Control content, bool closable = false)
        {
            if (_tabByKey.TryGetValue(key, out var existing))
            {
                _tabs.SelectedTab = existing;
                return;
            }

            var page = new TabPage(title) { BackColor = Color.White };
            page.Tag = key;

            content.Dock = DockStyle.Fill;
            page.Controls.Add(content);

            _tabs.TabPages.Add(page);
            _tabByKey[key] = page;
            _tabs.SelectedTab = page;

            _tabs.Invalidate();
        }
    }
}
