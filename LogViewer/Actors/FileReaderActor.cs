using Akka.Actor;
using System;
using System.IO;

namespace LogViewer.Actors
{
	public class FileReaderActor : ReceiveActor
	{
		private FileStream fs;
		private StreamReader sr;

		/// <summary>
		/// ReceiveActor subclass for file reading actions.  Add message handlers to the constructor.
		/// Currently handles messages of type:
		/// <see cref="OpenFile" />
		/// <see cref="CloseFile" />
		/// <see cref="ContinueReading" />
		/// </summary>
		public FileReaderActor()
		{
			// Set up message handlers
			Receive<OpenFile>((x) => HandleOpenFile(x));
			Receive<CloseFile>((_) => HandleCloseFile());
			Receive<ContinueReading>((_) => HandleContinueReading());
		}

		// Opens a file and assigns it to the FileStream. Does not check if the FileStream is already assigned.
		private void HandleOpenFile(OpenFile msg)
		{
			try
			{
				// Open the file as read-only and don't prevent other applications from accessing it
				// This is necessary when viewing logs if they are being actively written while viewing.
				fs = File.Open(msg.Filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

				// Open a new StreamReader on the FileStream
				sr = new(fs);
			}
			catch (Exception ex)
			{
				Sender.Tell(new OperationResult(false, typeof(OpenFile), ex.Message));
			}

			// Signal that we're ready to start processing the file
			Sender.Tell(new OperationResult(true, typeof(OpenFile), $"{msg.Filename} opened successfully"));
		}

		// By handling this as a separate message, it might allow me to easily continue reading if the file
		// is appended to after we reached the end of the original stream.
		private void HandleCloseFile()
		{
			fs.Close();
			sr.Close();

			// Signal that we successfully closed the file
			Sender.Tell(new OperationResult(true, typeof(CloseFile), $"File closed successfully"));
		}

		// Read the next line of the file when told to
		private void HandleContinueReading()
		{
			ReadLineFromFile();
		}

		// Reads a single line from the file
		private void ReadLineFromFile()
		{
			string line;
			if ((line = sr.ReadLine()) != null)
			{
				// Since StreamReader.ReadLine strips the newline, append it, then send the message back
				Sender.Tell(new LineRead($"{line}{Environment.NewLine}"));
			}
			else
			{
				Sender.Tell(new OperationResult(false, typeof(ContinueReading), "Reached end of stream"));
			}
		}
	}
}
