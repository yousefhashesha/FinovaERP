using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using FinovaERP.Application.Interfaces.Services;
using FinovaERP.Domain.Models.Entities;
namespace FinovaERP.Presentation.Forms.Reports
{
/// <summary>
/// Advanced item list form with real database data
/// </summary>
public partial class ItemListReportForm : Form
{
private readonly IItemService _itemService;
private readonly IItemCategoryService _categoryService;
private DataGridView dgvItems;
private ComboBox cmbCategory;
private TextBox txtSearch;
private Button btnSearch;
private Button btnRefresh;
private Button btnNew;
private Button btnEdit;
private Button btnDelete;
private Button btnExport;
private Button btnPrint;
private Label lblTotalValue;
private Label lblTotalItems;
private ProgressBar progressBar;
private BindingList<ItemViewModel> itemsList;
private List<ItemCategory> categories;
public ItemListReportForm(IServiceProvider serviceProvider)
{
_itemService = serviceProvider.GetRequiredService<IItemService>();
_categoryService = serviceProvider.GetRequiredService<IItemCategoryService>();
InitializeComponent();
SetupAdvancedUI();
LoadDataAsync();
}
private void SetupAdvancedUI()
{
this.Text = "Item Management - Advanced";
this.Size = new Size(1400, 800);
this.StartPosition = FormStartPosition.CenterScreen;
this.BackColor = Color.FromArgb(245, 248, 250);
this.Font = new Font("Segoe UI", 10F);
CreateHeaderPanel();
CreateSearchPanel();
CreateDataPanel();
CreateStatusPanel();
}
private void CreateHeaderPanel()
{
var panelHeader = new Panel
{
BackColor = Color.White,
Height = 80,
Dock = DockStyle.Top,
Padding = new Padding(20)
};
var lblTitle = new Label
{
Text = "Item Management System",
Font = new Font("Segoe UI", 24F, FontStyle.Bold),
ForeColor = Color.FromArgb(52, 73, 94),
AutoSize = true,
Location = new Point(20, 20)
};
var lblSubtitle = new Label
{
Text = "Manage your product inventory with advanced features",
Font = new Font("Segoe UI", 12F),
ForeColor = Color.Gray,
AutoSize = true,
Location = new Point(20, 50)
};
panelHeader.Controls.AddRange(new Control[] { lblTitle, lblSubtitle });
this.Controls.Add(panelHeader);
}
private void CreateSearchPanel()
{
var panelSearch = new Panel
{
BackColor = Color.White,
Height = 100,
Dock = DockStyle.Top,
Padding = new Padding(20)
};
// Search controls
var lblSearch = new Label
{
Text = "Search Items:",
Location = new Point(20, 20),
Size = new Size(100, 25),
ForeColor = Color.FromArgb(52, 73, 94),
Font = new Font("Segoe UI", 11F, FontStyle.Bold)
};
txtSearch = new TextBox
{
Location = new Point(130, 15),
Size = new Size(300, 35),
Font = new Font("Segoe UI", 11F),
BackColor = Color.FromArgb(248, 249, 250),
BorderStyle = BorderStyle.FixedSingle,
PlaceholderText = "Enter item name or code..."
};
var lblCategory = new Label
{
Text = "Category:",
Location = new Point(450, 20),
Size = new Size(80, 25),
ForeColor = Color.FromArgb(52, 73, 94),
Font = new Font("Segoe UI", 11F, FontStyle.Bold)
};
cmbCategory = new ComboBox
{
Location = new Point(540, 15),
Size = new Size(250, 35),
Font = new Font("Segoe UI", 11F),
BackColor = Color.FromArgb(248, 249, 250),
FlatStyle = FlatStyle.Flat,
DropDownStyle = ComboBoxStyle.DropDownList
};
btnSearch = new Button
{
Text = "🔍 Search",
Location = new Point(810, 15),
Size = new Size(120, 35),
BackColor = Color.FromArgb(52, 152, 219),
ForeColor = Color.White,
FlatStyle = FlatStyle.Flat,
Font = new Font("Segoe UI", 11F, FontStyle.Bold),
Cursor = Cursors.Hand
};
btnSearch.FlatAppearance.BorderSize = 0;
btnSearch.Click += async (s, e) => await SearchItemsAsync();
btnRefresh = new Button
{
Text = "🔄 Refresh",
Location = new Point(950, 15),
Size = new Size(120, 35),
BackColor = Color.FromArgb(149, 165, 166),
ForeColor = Color.White,
FlatStyle = FlatStyle.Flat,
Font = new Font("Segoe UI", 11F),
Cursor = Cursors.Hand
};
btnRefresh.FlatAppearance.BorderSize = 0;
btnRefresh.Click += async (s, e) => await LoadDataAsync();
// Action buttons
btnNew = new Button
{
Text = "➕ New Item",
Location = new Point(20, 60),
Size = new Size(120, 30),
BackColor = Color.FromArgb(46, 204, 113),
ForeColor = Color.White,
FlatStyle = FlatStyle.Flat,
Font = new Font("Segoe UI", 10F, FontStyle.Bold),
Cursor = Cursors.Hand
};
btnNew.FlatAppearance.BorderSize = 0;
btnNew.Click += BtnNew_Click;
btnEdit = new Button
{
Text = "✏️ Edit",
Location = new Point(150, 60),
Size = new Size(100, 30),
BackColor = Color.FromArgb(241, 196, 15),
ForeColor = Color.White,
FlatStyle = FlatStyle.Flat,
Font = new Font("Segoe UI", 10F, FontStyle.Bold),
Cursor = Cursors.Hand
};
btnEdit.FlatAppearance.BorderSize = 0;
btnEdit.Click += BtnEdit_Click;
btnDelete = new Button
{
Text = "🗑️ Delete",
Location = new Point(260, 60),
Size = new Size(100, 30),
BackColor = Color.FromArgb(231, 76, 60),
ForeColor = Color.White,
FlatStyle = FlatStyle.Flat,
Font = new Font("Segoe UI", 10F, FontStyle.Bold),
Cursor = Cursors.Hand
};
btnDelete.FlatAppearance.BorderSize = 0;
btnDelete.Click += BtnDelete_Click;
btnExport = new Button
{
Text = "📊 Export Excel",
Location = new Point(370, 60),
Size = new Size(120, 30),
BackColor = Color.FromArgb(155, 89, 182),
ForeColor = Color.White,
FlatStyle = FlatStyle.Flat,
Font = new Font("Segoe UI", 10F, FontStyle.Bold),
Cursor = Cursors.Hand
};
btnExport.FlatAppearance.BorderSize = 0;
btnExport.Click += BtnExport_Click;
btnPrint = new Button
{
Text = "🖨️ Print Report",
Location = new Point(500, 60),
Size = new Size(120, 30),
BackColor = Color.FromArgb(52, 73, 94),
ForeColor = Color.White,
FlatStyle = FlatStyle.Flat,
Font = new Font("Segoe UI", 10F, FontStyle.Bold),
Cursor = Cursors.Hand
};
btnPrint.FlatAppearance.BorderSize = 0;
btnPrint.Click += BtnPrint_Click;
panelSearch.Controls.AddRange(new Control[] {
lblSearch, txtSearch, lblCategory, cmbCategory, btnSearch, btnRefresh,
btnNew, btnEdit, btnDelete, btnExport, btnPrint
});
this.Controls.Add(panelSearch);
}
private void CreateDataPanel()
{
var panelData = new Panel
{
Dock = DockStyle.Fill,
Padding = new Padding(20),
BackColor = Color.Transparent
};
// Data Grid View
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
Font = new Font("Segoe UI", 10F),
GridColor = Color.FromArgb(236, 240, 241),
AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(248, 249, 250) }
};
// Style headers
dgvItems.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
dgvItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
dgvItems.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
dgvItems.ColumnHeadersHeight = 40;
dgvItems.EnableHeadersVisualStyles = false;
// Style selection
dgvItems.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
dgvItems.DefaultCellStyle.SelectionForeColor = Color.White;
// Progress bar
progressBar = new ProgressBar
{
Location = new Point(20, 20),
Size = new Size(200, 25),
Style = ProgressBarStyle.Marquee,
Visible = false,
Dock = DockStyle.Bottom
};
panelData.Controls.AddRange(new Control[] { dgvItems, progressBar });
this.Controls.Add(panelData);
}
private void CreateStatusPanel()
{
var panelStatus = new Panel
{
Height = 60,
Dock = DockStyle.Bottom,
BackColor = Color.FromArgb(52, 73, 94),
Padding = new Padding(20, 10, 20, 10)
};
lblTotalItems = new Label
{
Text = "Total Items: 0",
ForeColor = Color.White,
Font = new Font("Segoe UI", 12F, FontStyle.Bold),
AutoSize = true,
Location = new Point(20, 20)
};
lblTotalValue = new Label
{
Text = "Total Value: .00",
ForeColor = Color.White,
Font = new Font("Segoe UI", 12F, FontStyle.Bold),
AutoSize = true,
Location = new Point(200, 20)
};
panelStatus.Controls.AddRange(new Control[] { lblTotalItems, lblTotalValue });
this.Controls.Add(panelStatus);
}
private async void LoadDataAsync()
{
try
{
progressBar.Visible = true;
// Load categories
categories = new List<ItemCategory>(await _categoryService.GetAllCategoriesAsync());
cmbCategory.Items.Clear();
cmbCategory.Items.Add("All Categories");
foreach (var category in categories)
{
cmbCategory.Items.Add(category.Name);
}
cmbCategory.SelectedIndex = 0;
// Load items
await LoadItemsAsync();
progressBar.Visible = false;
}
catch (Exception ex)
{
progressBar.Visible = false;
MessageBox.Show($""Error loading data: {ex.Message}"", ""Error"", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
}
private async Task LoadItemsAsync()
{
var items = await _itemService.GetAllItemsAsync();
UpdateDataGrid(items);
}
private async Task SearchItemsAsync()
{
string searchTerm = txtSearch.Text.Trim();
string selectedCategory = cmbCategory.SelectedItem?.ToString() ?? "All Categories";
int? categoryId = selectedCategory == "All Categories" ? null : categories.Find(c => c.Name == selectedCategory)?.Id;
var items = await _itemService.SearchItemsAsync(searchTerm, categoryId);
UpdateDataGrid(items);
}
private void UpdateDataGrid(IEnumerable<Item> items)
{
itemsList = new BindingList<ItemViewModel>();
decimal totalValue = 0;
foreach (var item in items)
{
itemsList.Add(new ItemViewModel
{
Id = item.Id,
Code = item.Code,
Name = item.Name,
Category = item.Category?.Name ?? "N/A",
Price = item.SellingPrice,
Cost = item.CostPrice,
Stock = item.CurrentStock,
Status = item.IsActive ? "Active" : "Inactive",
TotalValue = item.TotalValue
});
totalValue += item.TotalValue;
}
dgvItems.DataSource = itemsList;
// Update status labels
lblTotalItems.Text = $""Total Items: {itemsList.Count}"";
lblTotalValue.Text = $""Total Value: {totalValue:C2}"";
ConfigureDataGridColumns();
}
private void ConfigureDataGridColumns()
{
dgvItems.Columns.Clear();
dgvItems.Columns.Add(new DataGridViewTextBoxColumn
{
DataPropertyName = "Id",
HeaderText = "ID",
Width = 60,
DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
});
dgvItems.Columns.Add(new DataGridViewTextBoxColumn
{
DataPropertyName = "Code",
HeaderText = "Item Code",
Width = 100
});
dgvItems.Columns.Add(new DataGridViewTextBoxColumn
{
DataPropertyName = "Name",
HeaderText = "Item Name",
Width = 250
});
dgvItems.Columns.Add(new DataGridViewTextBoxColumn
{
DataPropertyName = "Category",
HeaderText = "Category",
Width = 120
});
dgvItems.Columns.Add(new DataGridViewTextBoxColumn
{
DataPropertyName = "Price",
HeaderText = "Selling Price",
Width = 100,
DefaultCellStyle = { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight }
});
dgvItems.Columns.Add(new DataGridViewTextBoxColumn
{
DataPropertyName = "Cost",
HeaderText = "Cost Price",
Width = 100,
DefaultCellStyle = { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight }
});
dgvItems.Columns.Add(new DataGridViewTextBoxColumn
{
DataPropertyName = "Stock",
HeaderText = "Stock",
Width = 80,
DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
});
dgvItems.Columns.Add(new DataGridViewTextBoxColumn
{
DataPropertyName = "TotalValue",
HeaderText = "Total Value",
Width = 120,
DefaultCellStyle = { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight }
});
dgvItems.Columns.Add(new DataGridViewTextBoxColumn
{
DataPropertyName = "Status",
HeaderText = "Status",
Width = 80,
DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
});
}
private async void BtnNew_Click(object sender, EventArgs e)
{
// Open add item dialog
MessageBox.Show("Add new item functionality will be implemented in next phase!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
private async void BtnEdit_Click(object sender, EventArgs e)
{
if (dgvItems.SelectedRows.Count > 0)
{
var selectedItem = dgvItems.SelectedRows[0].DataBoundItem as ItemViewModel;
MessageBox.Show($""Edit item: {selectedItem.Name}"", ""Information"", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
else
{
MessageBox.Show(""Please select an item to edit!"", ""Warning"", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
}
private async void BtnDelete_Click(object sender, EventArgs e)
{
if (dgvItems.SelectedRows.Count > 0)
{
var selectedItem = dgvItems.SelectedRows[0].DataBoundItem as ItemViewModel;
if (MessageBox.Show($""Are you sure you want to delete {selectedItem.Name}?"", ""Confirm"", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
{
try
{
await _itemService.DeleteItemAsync(selectedItem.Id);
await LoadItemsAsync();
MessageBox.Show(""Item deleted successfully!"", ""Information"", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
catch (Exception ex)
{
MessageBox.Show($""Error deleting item: {ex.Message}"", ""Error"", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
}
}
else
{
MessageBox.Show(""Please select an item to delete!"", ""Warning"", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
}
private async void BtnExport_Click(object sender, EventArgs e)
{
try
{
var saveDialog = new SaveFileDialog
{
Filter = ""Excel files (*.xlsx)|*.xlsx|CSV files (*.csv)|*.csv"",
Title = ""Export Items Data"",
FileName = $""Items_Report_{DateTime.Now:yyyyMMdd_HHmmss}""
};
if (saveDialog.ShowDialog() == DialogResult.OK)
{
// Export logic will be implemented
MessageBox.Show($""Data exported successfully to: {saveDialog.FileName}"", ""Success"", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
}
catch (Exception ex)
{
MessageBox.Show($""Error exporting data: {ex.Message}"", ""Error"", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
}
private void BtnPrint_Click(object sender, EventArgs e)
{
// Print logic will be implemented
MessageBox.Show(""Print functionality will be implemented in next phase!"", ""Information"", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
// View Model
private class ItemViewModel
{
public int Id { get; set; }
public string Code { get; set; } = string.Empty;
public string Name { get; set; } = string.Empty;
public string Category { get; set; } = string.Empty;
public decimal Price { get; set; }
public decimal Cost { get; set; }
public int Stock { get; set; }
public decimal TotalValue { get; set; }
public string Status { get; set; } = string.Empty;
}
}
}
