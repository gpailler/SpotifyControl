using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CG.Common.Attributes
{
	/// <summary>
	/// Specifing display name of enum value
	/// (object must be decorated with attribute [TypeConverter(typeof(DisplayableEnumConverter))])
	/// </summary>
	[AttributeUsageAttribute(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class EnumDisplayNameAttribute : DisplayNameAttribute
	{
		public EnumDisplayNameAttribute(string displayName)
			: base(displayName)
		{ }
	}
}
