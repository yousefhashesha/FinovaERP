using System;
using Finova.Presentation.WinForms.Modules.Accounting.Contracts;

namespace Finova.Presentation.WinForms.Modules.Accounting.Presenters
{
    public sealed class AccountingHomePresenter
    {
        private readonly IAccountingView _view;

        public event EventHandler? ChartRequested;
        public event EventHandler? JournalEntryRequested;
        public event EventHandler? JournalListRequested;

        public AccountingHomePresenter(IAccountingView view)
        {
            _view = view;
            _view.OpenChartRequested += (_, __) => ChartRequested?.Invoke(this, EventArgs.Empty);
            _view.OpenJournalEntryRequested += (_, __) => JournalEntryRequested?.Invoke(this, EventArgs.Empty);
            _view.OpenJournalListRequested += (_, __) => JournalListRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
