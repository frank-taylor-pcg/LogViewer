using LogViewer.Enums;
using System;
using System.Collections.Generic;

namespace LogViewer.Model
{
	public class LineEntry
	{
		private readonly string line;
		private readonly string[] separators = { " : ", "[", "]-", "  " };

		/// <summary>Timestamp associated with the current log entry - not all lines have this</summary>
		public DateTime? Timestamp { get; set; }

		/// <summary>The error code associated with this specific message</summary>
		public int ErrorCode { get; set; }

		/// <summary>The severity of the message</summary>
		public SeverityLevel Severity { get; set; }

		/// <summary>Brief description of the problem</summary>
		public string Description { get; set; }

		/// <summary>Extra/verbose details that clutter the log.  May not always be useful.</summary>
		public List<string> Details { get; set; } = new();

		/// <summary> Constructor </summary>
		/// <param name="line">The line that will be used to create this LineEntry</param>
		public LineEntry(string line)
		{
			this.line = line;
		}

		/// <summary>
		/// Parse the provided line into its constituent parts.
		/// </summary>
		public void Parse()
		{
			try
			{
				string[] tokens = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
				int index = 0;

				// Attempts to extract a timestamp, only increments the token index if one is found
				if (ExtractTimeStamp(tokens[index]))
				{
					index++;
				}

				GetErrorCode(tokens[index++]);
				GetSeverity(tokens[index++]);
				Description = tokens[index++];

				for (int i = index; i < tokens.Length; i++)
				{
					Details.Add(tokens[i]);
				}
			}
			catch (Exception ex)
			{
				// TODO: Replace this with proper error handling
				System.Diagnostics.Debug.WriteLine($"Unexpected line format:{Environment.NewLine}{line}{Environment.NewLine}Exception message: {ex.Message}");
			}
		}

		// Parse out the timestamp if it exists
		private bool ExtractTimeStamp(string token)
		{
			bool success = DateTime.TryParse(token, out DateTime value);
			if (success)
			{
				Timestamp = value;
			}
			return success;
		}

		// Parse out the error code
		private void GetErrorCode(string token)
		{
			bool success = int.TryParse(token, out int value);
			if (success)
			{
				ErrorCode = value;
			}
		}

		// Parse out the severity indicator
		private void GetSeverity(string token)
		{
			Severity = token switch
			{
				"Info" => SeverityLevel.INFO,
				"Warning" => SeverityLevel.WARNING,
				"Error" => SeverityLevel.ERROR,
				_ => SeverityLevel.UNKNOWN,
			};
		}

		public override string ToString()
		{
			return $"{Timestamp}\t{ErrorCode}\t{Severity}\t{Description}";
		}
	}
}
