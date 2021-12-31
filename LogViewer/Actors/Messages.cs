using LogViewer.Model;
using System;

namespace LogViewer.Actors
{
	/// <summary>
	/// For operations that can fail passes back the result of the operation with any relevant diagnostic information.
	/// </summary>
	public record OperationResult(bool Successful, Type OperationType, string Message = null);

	/// <summary>
	/// Base message type for requests from the GUI to the back end.
	/// </summary>
	public record ClientRequest();

	/// <summary>
	/// Message requesting that a file should be opened.
	/// </summary>
	public record OpenFile(string Filename) : ClientRequest;

	/// <summary>
	/// Message requesting that the currently opened file should be closed.
	/// </summary>
	public record CloseFile() : ClientRequest;

	/// <summary>
	/// Message containing the most recently read line.
	/// </summary>
	public record LineRead(string Line);

	/// <summary>
	/// Message informing the FileReaderActor to continue reading the file.
	/// </summary>
	public record ContinueReading();

	/// <summary>
	/// Message passed back to the GUI of a single LogEntry that has been compiled from one or more lines of text in the log file.
	/// </summary>
	public record LogEntryRead(LogEntry LogEntry);
}
