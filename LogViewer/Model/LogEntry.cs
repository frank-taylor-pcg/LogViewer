using System.Collections.Generic;

namespace LogViewer.Model
{
	public class LogEntry
	{
		public LineEntry Primary { get; set; }

		public List<LineEntry> Extra { get; set; } = new();

		public List<LogEntry> Duplicates { get; set; } = new();
	}
}
