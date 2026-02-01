using System;

namespace Finova.Presentation.WinForms.Modules.Accounting.Contracts
{
    public interface IAccountingView
    {
        event EventHandler OpenChartRequested;
        event EventHandler OpenJournalEntryRequested;
        event EventHandler OpenJournalListRequested;

        void ShowInfo(string message);
    }
}
