using Akka.Actor;
using LogViewer.Model;

namespace LogViewer.Actors
{
	public class FileHandlerActor : ReceiveActor
	{
		private IActorRef client;
		private IActorRef fileReaderActor;

		// When a file open request is made, track the file name to avoid contamination of the new file with lines read from the previously open file.
		private string fileToOpen;

		private LogEntry logEntry;

		/// <summary>
		/// ReceiveActor subclass for file reading actions.  Add message handlers to the constructor.
		/// Currently handles messages of type:
		/// <see cref="OpenFile" />
		/// <see cref="OperationResult" />
		/// <see cref="LineRead" />
		/// </summary>
		public FileHandlerActor()
		{
			Receive<OpenFile>((x) => HandleOpenFile(x));
			Receive<OperationResult>((x) => HandleOperationResult(x));
			Receive<LineRead>((x) => HandleLineRead(x));
		}

		// Cleanly handly an OpenFile message, managing the FileReaderActor as needed if it already has a file open.
		private void HandleOpenFile(OpenFile msg)
		{
			// If the file reader actor has been created, then we'll need to close it's currently active file
			if (fileReaderActor is not null)
			{
				fileReaderActor.Tell(new CloseFile());

				// Wait for a response before passing the OpenFile message to avoid cross-contamination of the responses
				fileToOpen = msg.Filename;
			}
			else
			{
				client = Sender;
				fileReaderActor = Context.ActorOf(Props.Create(() => new FileReaderActor()));

				// Pass the OpenFile msg, unchanged to the FileReaderActor
				fileReaderActor.Tell(msg);
			}
		}

		// Respond to OperationResult messages, bubbling the response up to the GUI if necessary.
		private void HandleOperationResult(OperationResult msg)
		{
			if (msg.OperationType.Equals(typeof(OpenFile)))
			{
				if (msg.Successful)
				{
					client.Tell(msg);
					fileReaderActor.Tell(new ContinueReading());
					fileToOpen = null;
				}
				else
				{
					// Bubble the message back up to the GUI
					client.Tell(msg);
				}
			}
			// If the previous file was properly closed and we have a pending file open request
			else if (msg.OperationType.Equals(typeof(CloseFile)))
			{
				if (msg.Successful && fileToOpen is not null)
				{
					fileReaderActor.Tell(new OpenFile(fileToOpen));
				}
			}
			// If the file reader can't continue reading then send the current LogEntry
			else if (msg.OperationType.Equals(typeof(ContinueReading)))
			{
				if (logEntry != null)
				{
					client.Tell(new LogEntryRead(logEntry));
				}
			}
		}

		// Pass LineRead messages up to the GUI and tell the FileReaderActor to keep going.
		private void HandleLineRead(LineRead msg)
		{
			// Instead of passing lines back, pass completely LogEntries
			//client.Tell(msg);
			CompileLogEntries(msg.Line);
			fileReaderActor.Tell(new ContinueReading());
		}

		// Compiles a LogEntry using a timestamp as the starting boundary.
		// Note that HandleOperationResult/1 sends the current LogEntry when the end of file has been reached.
		private void CompileLogEntries(string line)
		{
			LineEntry lineEntry = new(line);
			lineEntry.Parse();

			if (lineEntry.Timestamp != null)
			{
				if (logEntry != null)
				{
					client.Tell(new LogEntryRead(logEntry));
				}
				logEntry = new();
				logEntry.Primary = lineEntry;
			}
			else
			{
				logEntry?.Extra.Add(lineEntry);
			}
		}
	}
}
