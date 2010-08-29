using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.Windows;
using System.ComponentModel;

namespace CG.Common.Wpf.Converters
{
	[ValueConversion(typeof(Bitmap), typeof(BitmapSource))]
	public class BitmapToBitmapSource : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is Bitmap))
				return null;

			IntPtr hBitmap = ((Bitmap)value).GetHbitmap();

			try
			{
				return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
				    hBitmap,
				    IntPtr.Zero,
				    Int32Rect.Empty,
				    BitmapSizeOptions.FromEmptyOptions());
			}
			catch (Win32Exception)
			{
				return null;
			}
			finally
			{
				Win32PInvoke.DeleteObject(hBitmap);
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
 			throw new NotImplementedException();
		}
	}
}
