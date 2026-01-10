using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace FinovaERP.Presentation.Forms
{
    /// <summary>
    /// Modern dashboard with KPI cards and sidebar navigation
    /// </summary>
    public partial class DashboardForm : Form
    {
        private readonly IServiceProvider _serviceProvider;
        
        // UI Controls - initialized in SetupModernDashboard
        private Panel panelSideMenu = null!;
        private Panel panelHeader = null!;
        private Panel panelMain = null!;
        private Panel panelDashboard = null!;

        private Button btnDashboard = null!;
        private Button btnSales = null!;
        private Button btnPurchasing = null!;
        private Button btnInventory = null!;
        private Button btnAccounting = null!;
        private Button btnReports = null!;
        private Button btnSettings = null!;
        private Button btnLogout = null!;

        private Label lblHeaderTitle = null!;
        private Label lblUserInfo = null!;
        private PictureBox picUser = null!;
        private Panel panelKPIs = null!;

        public DashboardForm(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            InitializeComponent();
            SetupModernDashboard();
            LoadDashboardData();
        }

        private void SetupModernDashboard()
        {
            this.Text = "FinovaERP - Dashboard";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.Font = new Font("Segoe UI", 10F);

            CreateSideMenu();
            CreateHeader();
            CreateMainPanel();
        }

        private void CreateSideMenu()
        {
            panelSideMenu = new Panel
            {
                BackColor = Color.FromArgb(45, 45, 48),
                Dock = DockStyle.Left,
                Width = 250
            };

            var panelLogo = new Panel
            {
                BackColor = Color.FromArgb(35, 35, 38),
                Height = 80,
                Dock = DockStyle.Top
            };

            var lblLogo = new Label
            {
                Text = "FinovaERP",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                AutoSize = true,
                Location = new Point(60, 25)
            };
            panelLogo.Controls.Add(lblLogo);

            var menuItems = new[]
            {
                ("Dashboard", "Dashboard"),
                ("Sales", "Sales"),
                ("Purchasing", "Purchasing"),
                ("Inventory", "Inventory"),
                ("Accounting", "Accounting"),
                ("Reports", "Reports"),
                ("Settings", "Settings"),
                ("Logout", "Logout")
            };

            int y = 90;
            foreach (var (text, tag) in menuItems)
            {
                var btn = new Button
                {
                    Text = text,
                    Tag = tag,
                    Location = new Point(0, y),
                    Size = new Size(250, 50),
                    BackColor = Color.FromArgb(45, 45, 48),
                    ForeColor = Color.Silver,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 11F),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(20, 0, 0, 0),
                    Cursor = Cursors.Hand
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += MenuButton_Click;

                panelSideMenu.Controls.Add(btn);
                y += 55;

                switch (tag)
                {
                    case "Dashboard": btnDashboard = btn; break;
                    case "Sales": btnSales = btn; break;
                    case "Purchasing": btnPurchasing = btn; break;
                    case "Inventory": btnInventory = btn; break;
                    case "Accounting": btnAccounting = btn; break;
                    case "Reports": btnReports = btn; break;
                    case "Settings": btnSettings = btn; break;
                    case "Logout": btnLogout = btn; break;
                }
            }

            panelSideMenu.Controls.Add(panelLogo);
            this.Controls.Add(panelSideMenu);
        }

        private void CreateHeader()
        {
            panelHeader = new Panel
            {
                BackColor = Color.White,
                Height = 70,
                Dock = DockStyle.Top
            };

            lblHeaderTitle = new Label
            {
                Text = "Dashboard Overview",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 45, 48),
                AutoSize = true,
                Location = new Point(20, 20)
            };

            lblUserInfo = new Label
            {
                Text = "Welcome, Admin",
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(this.Width - 250, 25)
            };

            picUser = new PictureBox
            {
                Size = new Size(40, 40),
                Location = new Point(this.Width - 300, 15),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            var userBitmap = new Bitmap(40, 40);
            using (var g = Graphics.FromImage(userBitmap))
            {
                g.Clear(Color.Transparent);
                g.FillEllipse(new SolidBrush(Color.FromArgb(0, 123, 255)), 0, 0, 40, 40);
                g.DrawString("A", new Font("Arial", 16F, FontStyle.Bold), Brushes.White, 12, 8);
            }
            picUser.Image = userBitmap;

            panelHeader.Controls.AddRange(new Control[] { lblHeaderTitle, lblUserInfo, picUser });
            this.Controls.Add(panelHeader);
        }

        private void CreateMainPanel()
        {
            panelMain = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            CreateDashboardPanel();
            panelMain.Controls.Add(panelDashboard);
            this.Controls.Add(panelMain);
        }

        private void CreateDashboardPanel()
        {
            panelDashboard = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            var lblSectionTitle = new Label
            {
                Text = "System Overview",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 45, 48),
                AutoSize = true,
                Location = new Point(30, 30)
            };

            CreateKPICards();

            panelDashboard.Controls.Add(lblSectionTitle);
            panelDashboard.Controls.Add(panelKPIs);
        }

        private void CreateKPICards()
        {
            panelKPIs = new Panel
            {
                Location = new Point(30, 80),
                Size = new Size(1000, 200)
            };

            var kpis = new[]
            {
                ("Total Sales", ",430", Color.FromArgb(0, 123, 255)),
                ("Customers", "1,247", Color.FromArgb(40, 167, 69)),
                ("Products", "892", Color.FromArgb(255, 193, 7)),
                ("Revenue", ",650", Color.FromArgb(220, 53, 69))
            };

            int x = 0;
            foreach (var (title, value, color) in kpis)
            {
                var card = CreateKPICard(title, value, color);
                card.Location = new Point(x, 20);
                panelKPIs.Controls.Add(card);
                x += 240;
            }
        }

        private static Panel CreateKPICard(string title, string value, Color color)
        {
            var card = new Panel
            {
                Size = new Size(220, 160),
                BackColor = Color.White
            };

            card.Paint += (s, e) =>
            {
                using (var brush = new SolidBrush(Color.FromArgb(30, Color.Gray)))
                {
                    e.Graphics.FillRectangle(brush, 2, 2, card.Width - 4, card.Height - 4);
                }
            };

            var topBar = new Panel { BackColor = color, Height = 5, Dock = DockStyle.Top };
            var lblTitle = new Label { Text = title, Font = new Font("Segoe UI", 11F), ForeColor = Color.Gray, AutoSize = true, Location = new Point(20, 25) };
            var lblValue = new Label { Text = value, Font = new Font("Segoe UI", 24F, FontStyle.Bold), ForeColor = color, AutoSize = true, Location = new Point(20, 50) };

            card.Controls.AddRange(new Control[] { topBar, lblTitle, lblValue });
            return card;
        }

        private static void LoadDashboardData() { /* Placeholder */ }

        private void MenuButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button button)
            {
                var tag = button.Tag?.ToString() ?? string.Empty;

                ResetMenuButtons();
                button.BackColor = Color.FromArgb(0, 123, 255);
                button.ForeColor = Color.White;

                lblHeaderTitle.Text = GetTitle(tag);

                switch (tag)
                {
                    case "Logout":
                        if (MessageBox.Show("Are you sure you want to log out?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            this.Close();
                        break;
                    case "Dashboard":
                        break;
                    default:
                        MessageBox.Show($"Opening {tag} module...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                }
            }
        }

        private void ResetMenuButtons()
        {
            var buttons = new[] { btnDashboard, btnSales, btnPurchasing, btnInventory, btnAccounting, btnReports, btnSettings, btnLogout };
            foreach (var btn in buttons)
            {
                btn.BackColor = Color.FromArgb(45, 45, 48);
                btn.ForeColor = Color.Silver;
            }
        }

        private static string GetTitle(string tag) => tag switch
        {
            "Dashboard" => "Dashboard Overview",
            "Sales" => "Sales Management",
            "Purchasing" => "Purchasing Module",
            "Inventory" => "Inventory Control",
            "Accounting" => "Accounting System",
            "Reports" => "Reports & Analytics",
            "Settings" => "System Settings",
            _ => "Dashboard Overview"
        };
    }
}
