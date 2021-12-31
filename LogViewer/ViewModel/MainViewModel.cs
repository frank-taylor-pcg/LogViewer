using LogViewer.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;

namespace LogViewer.ViewModel
{
	public class MainViewModel : ViewModelBase
	{
		private string _StatusMessage;
		public string StatusMessage
		{
			get => _StatusMessage;
			set => SetField(ref _StatusMessage, value);
		}

		public ObservableCollection<LogEntry> LogEntries { get; set; } = new();

		// This feels like a huge step backwards, but I'm overlooking something about the proper way to update the ObservableCollection as each new
		// LogEntry comes in from the FileHandlerActor
		private object _logEntriesLock = new();

		public MainViewModel()
		{
			BindingOperations.EnableCollectionSynchronization(LogEntries, _logEntriesLock);
		}

		public void AddLogEntry(LogEntry logEntry)
		{
			LogEntry last = null;

			// Assume that if the error code for the last entry matches the incoming log entry that we have a duplicate
			if (LogEntries.Any())
			{
				last = LogEntries.Last();
			}

			if (last is not null && last.Primary.ErrorCode.Equals(logEntry.Primary.ErrorCode))
			{
				last.Duplicates.Add(logEntry);
			}
			else
			{
				//Dispatcher.CurrentDispatcher.Invoke(() => LogEntries.Add(logEntry));
				LogEntries.Add(logEntry);
			}
		}
	}
}
