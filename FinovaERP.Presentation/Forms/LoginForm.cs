using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace FinovaERP.Presentation.Forms
{
    /// <summary>
    /// Modern login form with sleek design
    /// </summary>
    public partial class LoginForm : Form
    {
        private readonly IServiceProvider _serviceProvider;
        
        // UI Controls - initialized in SetupModernUI
        private TextBox txtUsername = null!;
        private TextBox txtPassword = null!;
        private Button btnLogin = null!;
        private Button btnCancel = null!;
        private Label lblTitle = null!;
        private Label lblUsername = null!;
        private Label lblPassword = null!;
        private Panel panelHeader = null!;
        private Panel panelBody = null!;
        private PictureBox picLogo = null!;

        public LoginForm(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            InitializeComponent();
            SetupModernUI();
        }

        private void SetupModernUI()
        {
            // Basic form settings
            this.Text = "FinovaERP - Login";
            this.Size = new Size(450, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            // Create controls
            CreateHeaderPanel();
            CreateBodyPanel();
            CreateLogo();
        }

        private void CreateHeaderPanel()
        {
            panelHeader = new Panel
            {
                BackColor = Color.FromArgb(0, 123, 255),
                Dock = DockStyle.Top,
                Height = 80
            };

            lblTitle = new Label
            {
                Text = "FinovaERP Login System",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(120, 25)
            };

            // Close button
            var btnClose = new Button
            {
                Text = "×",
                Location = new Point(410, 5),
                Size = new Size(30, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                Font = new Font("Arial", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            panelHeader.Controls.AddRange(new Control[] { lblTitle, btnClose });
            this.Controls.Add(panelHeader);
        }

        private void CreateBodyPanel()
        {
            panelBody = new Panel
            {
                BackColor = Color.FromArgb(62, 62, 66),
                Dock = DockStyle.Fill,
                Location = new Point(0, 80)
            };

            // Input fields
            lblUsername = new Label
            {
                Text = "Username:",
                Location = new Point(50, 80),
                Size = new Size(120, 25),
                ForeColor = Color.Silver,
                Font = new Font("Segoe UI", 11F)
            };

            txtUsername = new TextBox
            {
                Location = new Point(50, 110),
                Size = new Size(350, 35),
                Font = new Font("Segoe UI", 12F),
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            lblPassword = new Label
            {
                Text = "Password:",
                Location = new Point(50, 160),
                Size = new Size(120, 25),
                ForeColor = Color.Silver,
                Font = new Font("Segoe UI", 11F)
            };

            txtPassword = new TextBox
            {
                Location = new Point(50, 190),
                Size = new Size(350, 35),
                Font = new Font("Segoe UI", 12F),
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                PasswordChar = '●'
            };

            // Control buttons
            btnLogin = new Button
            {
                Text = "Login",
                Location = new Point(50, 250),
                Size = new Size(170, 45),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(230, 250),
                Size = new Size(170, 45),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();

            // Add controls
            panelBody.Controls.AddRange(new Control[] {
                lblUsername, txtUsername, lblPassword, txtPassword,
                btnLogin, btnCancel
            });

            this.Controls.Add(panelBody);
        }

        private void CreateLogo()
        {
            picLogo = new PictureBox
            {
                Size = new Size(100, 100),
                Location = new Point(175, 30),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

            // Create temporary logo (can be replaced with real image later)
            var bitmap = new Bitmap(100, 100);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.Transparent);
                g.FillEllipse(new SolidBrush(Color.FromArgb(0, 123, 255)), 10, 10, 80, 80);
                g.DrawString("F", new Font("Arial", 40F, FontStyle.Bold), Brushes.White, 25, 20);
            }
            picLogo.Image = bitmap;

            panelBody.Controls.Add(picLogo);
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            // Login logic
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter username and password", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Default login credentials for testing
            if (txtUsername.Text == "admin" && txtPassword.Text == "admin123")
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
