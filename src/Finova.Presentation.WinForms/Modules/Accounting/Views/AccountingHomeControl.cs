using System;
using System.Drawing;
using System.Windows.Forms;
using Finova.Presentation.WinForms.Modules.Accounting.Contracts;

namespace Finova.Presentation.WinForms.Modules.Accounting.Views
{
    public sealed class AccountingHomeControl : UserControl, IAccountingView
    {
        private readonly Button _btnCoa = new();
        private readonly Button _btnJe = new();
        private readonly Button _btnList = new();

        public event EventHandler? OpenChartRequested;
        public event EventHandler? OpenJournalEntryRequested;
        public event EventHandler? OpenJournalListRequested;

        public AccountingHomeControl()
        {
            Dock = DockStyle.Fill;

            var title = new Label
            {
                Text = "Accounting",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(16, 16)
            };
            Controls.Add(title);

            _btnCoa.Text = "Chart of Accounts";
            _btnCoa.Size = new Size(220, 40);
            _btnCoa.Location = new Point(16, 70);
            _btnCoa.Click += (_, __) => OpenChartRequested?.Invoke(this, EventArgs.Empty);
            Controls.Add(_btnCoa);

            _btnJe.Text = "Journal Entry";
            _btnJe.Size = new Size(220, 40);
            _btnJe.Location = new Point(16, 120);
            _btnJe.Click += (_, __) => OpenJournalEntryRequested?.Invoke(this, EventArgs.Empty);
            Controls.Add(_btnJe);

            _btnList.Text = "Journal List";
            _btnList.Size = new Size(220, 40);
            _btnList.Location = new Point(16, 170);
            _btnList.Click += (_, __) => OpenJournalListRequested?.Invoke(this, EventArgs.Empty);
            Controls.Add(_btnList);
        }

        public void ShowInfo(string message) => MessageBox.Show(message, "Finova", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
