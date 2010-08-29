using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using CG.Common.Attributes;

namespace CG.Common.Converters
{
	/// <summary>
	/// Convert enum value to displayable representation
	/// </summary>
	/// <example>
	/// [TypeConverter(typeof(DisplayableEnumConverter))]
	/// class MyEnum
	/// {
	///	[EnumDisplayName("My value1")]
	///	Value1,
	///	[EnumDisplayName("My value2")]
	///	Value2
	/// }
	/// </example>
	public class DisplayableEnumConverter : EnumConverter
	{
		public DisplayableEnumConverter(Type type)
			: base(type)
		{
		}

		internal static string GetEnumDisplayNameAttribute(Type enumType, Enum enumValue)
		{
			if (enumType == null) throw new ArgumentNullException("enumType");
			if (enumValue == null) throw new ArgumentNullException("enumValue");
			if (!enumType.IsEnum) throw new ArgumentException("enumType is not an Enum type");
			if (!Enum.IsDefined(enumType, enumValue)) throw new ArgumentException(string.Format("'{0}' is not defined in '{1}'", enumValue.ToString(), enumType.FullName));


			FieldInfo fi = enumType.GetField(enumValue.ToString());
			EnumDisplayNameAttribute[] attributes = (EnumDisplayNameAttribute[])fi.GetCustomAttributes(typeof(EnumDisplayNameAttribute), true);
			if (attributes.Length > 0)
			{
				return attributes[0].DisplayName;
			}

			return enumValue.ToString();
		}

		private static bool TryParseEnumDisplayNameAttribute(Type enumType, string displayableValue, out Enum enumValue)
		{
			if (enumType == null) throw new ArgumentNullException("enumType");
			if (string.IsNullOrEmpty(displayableValue)) throw new ArgumentNullException("displayableValue");
			if (!enumType.IsEnum) throw new ArgumentException("enumType is not an Enum type");

			foreach (Enum value in Enum.GetValues(enumType))
			{
				if (displayableValue == GetEnumDisplayNameAttribute(enumType, value))
				{
					enumValue = value;
					return true;
				}
			}

			enumValue = null;
			return false;
		}


		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
				return true;

			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (value is Enum && destinationType == typeof(string))
				return GetEnumDisplayNameAttribute(this.EnumType, value as Enum);

			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;

			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value is string)
			{
				Enum enumValue;
				if (TryParseEnumDisplayNameAttribute(this.EnumType, value as string, out enumValue))
				{
					return enumValue;
				}
				else
				{
					throw new InvalidCastException(string.Format("Unable to parse value '{0}'", value));
				}
			}

			return base.ConvertFrom(context, culture, value);
		}
	}
}
