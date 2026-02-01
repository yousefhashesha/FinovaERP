#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

WINFORMS="$ROOT_DIR/src/Finova.Presentation.WinForms"
CONTRACTS_APP="$ROOT_DIR/src/Finova.Application.Contracts"

echo "ROOT_DIR=$ROOT_DIR"
echo "WINFORMS=$WINFORMS"
echo

# ---------- 0) Safety checks ----------
test -d "$WINFORMS" || { echo "ERROR: WinForms project not found: $WINFORMS"; exit 1; }
test -d "$CONTRACTS_APP" || { echo "ERROR: Application.Contracts not found: $CONTRACTS_APP"; exit 1; }

# ---------- 1) Remove the duplicated CompanyDto inside WinForms (سبب مشاكل CompanyDto + XML) ----------
# إذا الملف موجود، احذفه لأن CompanyDto لازم ييجي من Finova.Application.Contracts.Context
DUP_DTO="$WINFORMS/Session/CompanyDto.cs"
if [ -f "$DUP_DTO" ]; then
  echo "Removing duplicate: $DUP_DTO"
  rm -f "$DUP_DTO"
fi

# ---------- 2) Ensure IShellView contract matches the intended session model ----------
# نعتمد على Guid للشركة (CompanyId) وليس string
mkdir -p "$WINFORMS/Modules/Shell/Contracts"
cat > "$WINFORMS/Modules/Shell/Contracts/IShellView.cs" <<'CS'
using System;
using System.Collections.Generic;

namespace Finova.Presentation.WinForms.Modules.Shell.Contracts
{
    public interface IShellView
    {
        event EventHandler<string>? NavigationRequested;
        event EventHandler? LogoutRequested;
        event EventHandler<string>? SearchRequested;

        // Company switch must be GUID (matches DB + session)
        event EventHandler<Guid>? CompanyChanged;

        void SetHeader(string companyName, string userDisplayName, string periodText);

        // Companies list uses (Id, Name) + selected Id
        void SetCompanies(IEnumerable<(Guid Id, string Name)> companies, Guid? selectedCompanyId);

        void ApplyNavigation(IEnumerable<ShellNavItem> items);

        // Tabs
        void OpenTab(string key, string title, object content);
        void ActivateTab(string key);
        void CloseTab(string key);
    }

    public sealed record ShellNavItem(string Key, string Title, bool Enabled);
}
CS

# ---------- 3) Fix AppSession in WinForms to implement the real IAppSession from Contracts ----------
# IMPORTANT: using Finova.Application.Contracts.Context.IAppSession
mkdir -p "$WINFORMS/Session"
cat > "$WINFORMS/Session/AppSession.cs" <<'CS'
using System;
using System.Collections.Generic;
using Finova.Application.Contracts.Context;

namespace Finova.Presentation.WinForms.Session
{
    public sealed class AppSession : IAppSession
    {
        public Guid UserId { get; private set; }
        public string UserName { get; private set; } = string.Empty;
        public string UserDisplayName { get; private set; } = string.Empty;

        public string PeriodText { get; private set; } = string.Empty;

        public IReadOnlyList<CompanyDto> Companies { get; private set; } = Array.Empty<CompanyDto>();
        public Guid? CurrentCompanyId { get; private set; }

        public void SetCompany(Guid companyId) => CurrentCompanyId = companyId;

        public void SetUser(Guid userId, string userName, string displayName)
        {
            UserId = userId;
            UserName = userName ?? string.Empty;
            UserDisplayName = displayName ?? string.Empty;
        }

        public void SetPeriod(string periodText) => PeriodText = periodText ?? string.Empty;

        public void SetCompanies(IReadOnlyList<CompanyDto> companies, Guid? selectedCompanyId)
        {
            Companies = companies ?? Array.Empty<CompanyDto>();
            CurrentCompanyId = selectedCompanyId;
        }

        public void Clear()
        {
            UserId = Guid.Empty;
            UserName = string.Empty;
            UserDisplayName = string.Empty;
            PeriodText = string.Empty;
            Companies = Array.Empty<CompanyDto>();
            CurrentCompanyId = null;
        }
    }
}
CS

# ---------- 4) Replace ShellForm with a CLEAN compiling version that matches IShellView ----------
# أهم جزء: إصلاح Syntax error + مطابقة Events + SetCompanies + ShellNavItem(bool enabled)
SHELLFORM="$WINFORMS/Modules/Shell/Views/ShellForm.cs"
mkdir -p "$(dirname "$SHELLFORM")"

cat > "$SHELLFORM" <<'CS'
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Finova.Presentation.WinForms.Modules.Shell.Contracts;
using Finova.Presentation.WinForms.Theme;

namespace Finova.Presentation.WinForms.Modules.Shell.Views
{
    public sealed class ShellForm : Form, IShellView
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
        public event EventHandler<Guid>? CompanyChanged;

        // Tabs UX constants
        private const int CloseBoxSize = 12;
        private const int CloseBoxPadding = 8;

        private sealed class CompanyItem
        {
            public Guid Id { get; }
            public string Name { get; }
            public CompanyItem(Guid id, string name) { Id = id; Name = name ?? string.Empty; }
            public override string ToString() => Name;
        }

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
                Width = 220,
                Font = new Font("Segoe UI", 9),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            header.Controls.Add(_cmbCompany);

            _cmbCompany.SelectedIndexChanged += (_, __) =>
            {
                if (_cmbCompany.SelectedItem is CompanyItem it)
                    CompanyChanged?.Invoke(this, it.Id);
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
        }

        public void SetHeader(string companyName, string userDisplayName, string periodText)
        {
            _lblCompany.Text = companyName ?? string.Empty;
            _lblUser.Text = userDisplayName ?? string.Empty;
            _lblPeriod.Text = periodText ?? string.Empty;
        }

        public void SetCompanies(IEnumerable<(Guid Id, string Name)> companies, Guid? selectedCompanyId)
        {
            var list = (companies ?? Array.Empty<(Guid, string)>())
                .Select(x => new CompanyItem(x.Id, x.Name))
                .ToList();

            _cmbCompany.BeginUpdate();
            _cmbCompany.Items.Clear();
            foreach (var it in list) _cmbCompany.Items.Add(it);
            _cmbCompany.EndUpdate();

            if (selectedCompanyId.HasValue)
            {
                var match = list.FirstOrDefault(x => x.Id == selectedCompanyId.Value);
                if (match != null) _cmbCompany.SelectedItem = match;
                else if (_cmbCompany.Items.Count > 0) _cmbCompany.SelectedIndex = 0;
            }
            else
            {
                if (_cmbCompany.Items.Count > 0) _cmbCompany.SelectedIndex = 0;
            }
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

            int y = 70;

            foreach (var it in (items ?? Array.Empty<ShellNavItem>()))
            {
                var btn = new Button
                {
                    Text = it.Title,
                    Tag = it.Key,
                    Width = 216,
                    Height = 40,
                    Left = 22,
                    Top = y,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(28, 36, 58),
                    Enabled = it.Enabled
                };
                btn.FlatAppearance.BorderSize = 0;

                btn.Click += (_, __) =>
                {
                    if (btn.Tag is string key)
                        NavigationRequested?.Invoke(this, key);
                };

                _sidebar.Controls.Add(btn);
                _navButtons[it.Key] = btn;
                y += 46;
            }

            _btnLogout.Left = 22;
            _btnLogout.Top = Math.Max(_sidebar.Height - 60, y + 20);
            _btnLogout.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            _sidebar.Controls.Add(_btnLogout);
        }

        public void OpenTab(string key, string title, object content)
        {
            if (string.IsNullOrWhiteSpace(key)) return;

            if (_tabByKey.TryGetValue(key, out var existing))
            {
                _tabs.SelectedTab = existing;
                return;
            }

            var page = new TabPage(title ?? key);

            if (content is Control ctl)
            {
                ctl.Dock = DockStyle.Fill;
                page.Controls.Add(ctl);
            }
            else
            {
                page.Controls.Add(new Label
                {
                    Text = "Content placeholder",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                });
            }

            _tabByKey[key] = page;
            _tabs.TabPages.Add(page);
            _tabs.SelectedTab = page;
        }

        public void ActivateTab(string key)
        {
            if (_tabByKey.TryGetValue(key, out var page))
                _tabs.SelectedTab = page;
        }

        public void CloseTab(string key)
        {
            if (_tabByKey.TryGetValue(key, out var page))
            {
                _tabByKey.Remove(key);
                _tabs.TabPages.Remove(page);
                page.Dispose();
            }
        }

        private void Tabs_DrawItem(object? sender, DrawItemEventArgs e)
        {
            var tab = _tabs.TabPages[e.Index];
            var rect = _tabs.GetTabRect(e.Index);

            using var b = new SolidBrush(Color.FromArgb(240, 240, 240));
            e.Graphics.FillRectangle(b, rect);

            TextRenderer.DrawText(
                e.Graphics,
                tab.Text,
                Font,
                new Rectangle(rect.X + 6, rect.Y + 4, rect.Width - 30, rect.Height - 6),
                Color.Black,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter
            );

            // close box
            var cx = rect.Right - CloseBoxPadding - CloseBoxSize;
            var cy = rect.Y + (rect.Height - CloseBoxSize) / 2;
            var closeRect = new Rectangle(cx, cy, CloseBoxSize, CloseBoxSize);

            using var pen = new Pen(Color.Gray, 2);
            e.Graphics.DrawLine(pen, closeRect.Left, closeRect.Top, closeRect.Right, closeRect.Bottom);
            e.Graphics.DrawLine(pen, closeRect.Right, closeRect.Top, closeRect.Left, closeRect.Bottom);
        }

        private void Tabs_MouseDown(object? sender, MouseEventArgs e)
        {
            for (int i = 0; i < _tabs.TabPages.Count; i++)
            {
                var rect = _tabs.GetTabRect(i);
                var cx = rect.Right - CloseBoxPadding - CloseBoxSize;
                var cy = rect.Y + (rect.Height - CloseBoxSize) / 2;
                var closeRect = new Rectangle(cx, cy, CloseBoxSize, CloseBoxSize);

                if (closeRect.Contains(e.Location))
                {
                    var page = _tabs.TabPages[i];
                    var key = _tabByKey.FirstOrDefault(kv => kv.Value == page).Key;
                    if (!string.IsNullOrWhiteSpace(key))
                        CloseTab(key);
                    return;
                }
            }
        }
    }
}
CS

# ---------- 5) Quick build ----------
echo
echo "=== dotnet build ==="
dotnet build "$ROOT_DIR/Finova.sln" -v minimal
echo
echo "✅ Fix Pack applied. Now run: ./run-dev.sh"
