using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG.Common.Converters;

namespace CG.Common.Formatters
{
	public class EnumFormatter : IFormatProvider, ICustomFormatter
	{
		#region IFormatProvider Members

		public object GetFormat(Type formatType)
		{
			if (formatType == typeof(ICustomFormatter))
			{
				return this;
			}

			return null;
		}

		#endregion

		#region ICustomFormatter Members

		public string Format(string format, object arg, IFormatProvider formatProvider)
		{
			if (arg is Enum)
			{
				return DisplayableEnumConverter.GetEnumDisplayNameAttribute(arg.GetType(), (Enum)arg);
			}

			return string.Empty;
		}

		#endregion
	}

}
