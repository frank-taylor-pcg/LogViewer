using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace LogViewer.Converters
{
	public class ListToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			StringBuilder sb = new();

			if (value is List<string> list)
			{
				foreach (string s in list)
				{
					if (!string.IsNullOrWhiteSpace(s))
					{
						_ = sb.AppendLine(s.Trim());
					}
				}
			}

			return sb.ToString().Trim();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
