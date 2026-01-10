using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace FinovaERP.Presentation.Forms
{
    /// <summary>
    /// Professional item list form with modern design
    /// </summary>
    public partial class ItemListForm : Form
    {
        private readonly IServiceProvider _serviceProvider;
        
        // UI Controls - initialized in SetupModernUI
        private Panel panelHeader = null!;
        private Panel panelSearch = null!;
        private Panel panelGrid = null!;
        private Panel panelActions = null!;
        
        private DataGridView dgvItems = null!;
        private TextBox txtSearch = null!;
        private ComboBox cmbCategory = null!;
        private Button btnSearch = null!;
        private Button btnNew = null!;
        private Button btnEdit = null!;
        private Button btnDelete = null!;
        private Button btnRefresh = null!;
        private Button btnExport = null!;
        
        private Label lblTitle = null!;
        private Label lblSearch = null!;
        private Label lblCategory = null!;
        
        private BindingList<ItemViewModel> itemsList = null!;

        public ItemListForm(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            InitializeComponent();
            SetupModernUI();
            LoadSampleData();
        }

        private void SetupModernUI()
        {
            this.Text = "Item Management - FinovaERP";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.Font = new Font("Segoe UI", 10F);

            CreateHeader();
            CreateSearchPanel();
            CreateGridPanel();
            CreateActionsPanel();
        }

        private void CreateHeader()
        {
            panelHeader = new Panel
            {
                BackColor = Color.White,
                Height = 80,
                Dock = DockStyle.Top,
                Padding = new Padding(20)
            };

            lblTitle = new Label
            {
                Text = "Item Management",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 45, 48),
                AutoSize = true,
                Location = new Point(20, 25)
            };

            var btnClose = new Button
            {
                Text = "×",
                Location = new Point(this.Width - 60, 20),
                Size = new Size(30, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.Gray,
                Font = new Font("Arial", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            panelHeader.Controls.AddRange(new Control[] { lblTitle, btnClose });
            this.Controls.Add(panelHeader);
        }

        private void CreateSearchPanel()
        {
            panelSearch = new Panel
            {
                BackColor = Color.White,
                Height = 80,
                Dock = DockStyle.Top,
                Padding = new Padding(20)
            };

            lblSearch = new Label
            {
                Text = "Search:",
                Location = new Point(20, 30),
                Size = new Size(60, 25),
                ForeColor = Color.Gray
            };

            txtSearch = new TextBox
            {
                Location = new Point(90, 25),
                Size = new Size(250, 35),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblCategory = new Label
            {
                Text = "Category:",
                Location = new Point(360, 30),
                Size = new Size(70, 25),
                ForeColor = Color.Gray
            };

            cmbCategory = new ComboBox
            {
                Location = new Point(440, 25),
                Size = new Size(200, 35),
                Font = new Font("Segoe UI", 11F),
                BackColor = Color.FromArgb(248, 249, 250),
                FlatStyle = FlatStyle.Flat
            };
            cmbCategory.Items.AddRange(new[] { "All Categories", "Electronics", "Clothing", "Books", "Food", "Other" });
            cmbCategory.SelectedIndex = 0;

            btnSearch = new Button
            {
                Text = "🔍 Search",
                Location = new Point(660, 25),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += BtnSearch_Click;

            btnRefresh = new Button
            {
                Text = "🔄 Refresh",
                Location = new Point(770, 25),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += BtnRefresh_Click;

            panelSearch.Controls.AddRange(new Control[] { lblSearch, txtSearch, lblCategory, cmbCategory, btnSearch, btnRefresh });
            this.Controls.Add(panelSearch);
        }

        private void CreateGridPanel()
        {
            panelGrid = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            dgvItems = new DataGridView
            {
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F)
            };

            // Style the grid
            dgvItems.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            dgvItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvItems.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            dgvItems.ColumnHeadersHeight = 40;

            dgvItems.DefaultCellStyle.BackColor = Color.White;
            dgvItems.DefaultCellStyle.ForeColor = Color.FromArgb(45, 45, 48);
            dgvItems.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 123, 255);
            dgvItems.DefaultCellStyle.SelectionForeColor = Color.White;

            dgvItems.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);

            panelGrid.Controls.Add(dgvItems);
            this.Controls.Add(panelGrid);
        }

        private void CreateActionsPanel()
        {
            panelActions = new Panel
            {
                Height = 60,
                Dock = DockStyle.Bottom,
                BackColor = Color.White,
                Padding = new Padding(20, 10, 20, 10)
            };

            btnNew = new Button
            {
                Text = "➕ New Item",
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnNew.FlatAppearance.BorderSize = 0;
            btnNew.Click += BtnNew_Click;

            btnEdit = new Button
            {
                Text = "✏️ Edit",
                Size = new Size(100, 40),
                Location = new Point(140, 10),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button
            {
                Text = "🗑️ Delete",
                Size = new Size(100, 40),
                Location = new Point(250, 10),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += BtnDelete_Click;

            btnExport = new Button
            {
                Text = "📊 Export",
                Size = new Size(100, 40),
                Location = new Point(360, 10),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnExport.FlatAppearance.BorderSize = 0;
            btnExport.Click += BtnExport_Click;

            panelActions.Controls.AddRange(new Control[] { btnNew, btnEdit, btnDelete, btnExport });
            this.Controls.Add(panelActions);
        }

        private void LoadSampleData()
        {
            itemsList = new BindingList<ItemViewModel>
            {
                new ItemViewModel { Id = 1, Code = "ITM001", Name = "Laptop Dell XPS 13", Category = "Electronics", Price = 1200.00m, Stock = 15, Status = "Active" },
                new ItemViewModel { Id = 2, Code = "ITM002", Name = "iPhone 15 Pro", Category = "Electronics", Price = 999.00m, Stock = 8, Status = "Active" },
                new ItemViewModel { Id = 3, Code = "ITM003", Name = "Samsung Galaxy S24", Category = "Electronics", Price = 899.00m, Stock = 12, Status = "Active" },
                new ItemViewModel { Id = 4, Code = "ITM004", Name = "Nike Air Max", Category = "Clothing", Price = 150.00m, Stock = 25, Status = "Active" },
                new ItemViewModel { Id = 5, Code = "ITM005", Name = "Coffee Beans 1kg", Category = "Food", Price = 25.50m, Stock = 50, Status = "Active" }
            };

            SetupDataGridView();
        }

        private void SetupDataGridView()
        {
            dgvItems.DataSource = itemsList;

            // Configure columns
            dgvItems.Columns["Id"].HeaderText = "ID";
            dgvItems.Columns["Id"].Width = 50;
            dgvItems.Columns["Id"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvItems.Columns["Code"].HeaderText = "Item Code";
            dgvItems.Columns["Code"].Width = 100;

            dgvItems.Columns["Name"].HeaderText = "Item Name";
            dgvItems.Columns["Name"].Width = 250;

            dgvItems.Columns["Category"].HeaderText = "Category";
            dgvItems.Columns["Category"].Width = 120;

            dgvItems.Columns["Price"].HeaderText = "Price";
            dgvItems.Columns["Price"].Width = 100;
            dgvItems.Columns["Price"].DefaultCellStyle.Format = "C2";
            dgvItems.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvItems.Columns["Stock"].HeaderText = "Stock";
            dgvItems.Columns["Stock"].Width = 80;
            dgvItems.Columns["Stock"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvItems.Columns["Status"].HeaderText = "Status";
            dgvItems.Columns["Status"].Width = 100;
            dgvItems.Columns["Status"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        // Event Handlers
        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            string searchText = txtSearch.Text.ToLower();
            string category = cmbCategory.SelectedItem?.ToString() ?? "All Categories";

            var filteredList = new BindingList<ItemViewModel>();

            foreach (var item in itemsList)
            {
                bool matchesSearch = string.IsNullOrWhiteSpace(searchText) || 
                                   item.Name.ToLower().Contains(searchText) || 
                                   item.Code.ToLower().Contains(searchText);

                bool matchesCategory = category == "All Categories" || item.Category == category;

                if (matchesSearch && matchesCategory)
                {
                    filteredList.Add(item);
                }
            }

            dgvItems.DataSource = filteredList;
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            SetupDataGridView();
            txtSearch.Clear();
            cmbCategory.SelectedIndex = 0;
            MessageBox.Show("Data refreshed successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnNew_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Add new item functionality will be implemented soon!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count > 0)
            {
                var selectedItem = dgvItems.SelectedRows[0].DataBoundItem as ItemViewModel;
                MessageBox.Show($"Edit item: {selectedItem.Name}", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please select an item to edit!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count > 0)
            {
                var selectedItem = dgvItems.SelectedRows[0].DataBoundItem as ItemViewModel;
                if (MessageBox.Show($"Are you sure you want to delete {selectedItem.Name}?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    itemsList.Remove(selectedItem);
                    MessageBox.Show("Item deleted successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select an item to delete!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnExport_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Export functionality will be implemented soon!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    // View Model for DataGridView
    public class ItemViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Status { get; set; } = "Active";
    }
}
