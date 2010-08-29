using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CG.Common.Attributes
{
	/// <summary>
	/// Specifing order of the property
	/// (object must be decorated with attribute [TypeConverter(typeof(CustomSortTypeConverter))])
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class PropertyOrderAttribute : Attribute
	{
		private readonly int _orderIndex;

		public PropertyOrderAttribute(int orderIndex)
		{
			_orderIndex = orderIndex;
		}

		public int Order
		{
			get
			{
				return _orderIndex;
			}
		}
	}
}
