using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LogViewer.Converters
{
	public class SeverityToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			SolidColorBrush result = Brushes.Black;

			if (value.ToString().Equals("info", StringComparison.OrdinalIgnoreCase))
			{
				result = Brushes.Cyan;
			}

			if (value.ToString().Equals("warning", StringComparison.OrdinalIgnoreCase))
			{
				result = Brushes.Orange;
			}

			if (value.ToString().Equals("error", StringComparison.OrdinalIgnoreCase))
			{
				result = Brushes.Red;
			}

			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
