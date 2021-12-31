using Akka.Actor;
using LogViewer.Model;
using LogViewer.ViewModel;
using System.Linq;

namespace LogViewer.Actors
{
	/// <summary>
	/// This class is a "bridge" between WPF and Akka.NET.
	/// </summary>
	public class MainViewActor : ReceiveActor
	{
		private readonly MainViewModel _viewModel;

		public MainViewActor(MainViewModel viewModel)
		{
			_viewModel = viewModel;

			Receive<ClientRequest>(x => HandleClientRequest(x));
			Receive<OperationResult>(x => HandleOperationResult(x));
			Receive<LogEntryRead>(x => HandleLogEntryRead(x));
		}

		private static void HandleClientRequest(ClientRequest msg)
		{
			Globals.FileHandler.Tell(msg);
		}

		private void HandleOperationResult(OperationResult msg)
		{
			_viewModel.StatusMessage = msg.Message;
		}

		private void HandleLogEntryRead(LogEntryRead msg)
		{
			_viewModel.AddLogEntry(msg.LogEntry);
		}
	}
}
