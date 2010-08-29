using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using CG.Common.Attributes;

namespace CG.Common.Converters
{
	/// <summary>
	/// Properties custom sort (usefull in propertygrid)
	/// </summary>
	/// <example>
	/// [TypeConverter(typeof(CustomSortTypeConverter))]
	/// class MyClass
	/// {
	///	[PropertyOrderAttribute(1)]
	///	Value1,
	///	[PropertyOrderAttribute(2)]
	///	Value2
	/// }
	/// </example>
	public class CustomSortTypeConverter : ExpandableObjectConverter
	{
		private static PropertyDescriptorCollection SortProperties(PropertyDescriptorCollection pdc)
		{
			List<PropertyOrderPair> orderedProperties = new List<PropertyOrderPair>();
			foreach (PropertyDescriptor pd in pdc)
			{
				Attribute attribute = pd.Attributes[typeof(PropertyOrderAttribute)];
				if (attribute != null)
				{
					//
					// If the attribute is found, then create an pair object to hold it
					//
					PropertyOrderAttribute poa = (PropertyOrderAttribute)attribute;
					orderedProperties.Add(new PropertyOrderPair(pd.Name, poa.Order));
				}
				else
				{
					//
					// If no order attribute is specifed then given it an order of 0
					//
					orderedProperties.Add(new PropertyOrderPair(pd.Name, 0));
				}
			}
			//
			// Perform the actual order using the value PropertyOrderPair classes
			// implementation of IComparable to sort
			//
			orderedProperties.Sort();
			//
			// Build a string list of the ordered names
			//
			List<string> propertyNames = new List<string>();
			foreach (PropertyOrderPair pop in orderedProperties)
			{
				propertyNames.Add(pop.Name);
			}
			//
			// Pass in the ordered list for the PropertyDescriptorCollection to sort by
			//
			return pdc.Sort(propertyNames.ToArray());
		}


		#region Methods

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			//
			// This override returns a list of properties in order
			//
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(value, attributes);

			return SortProperties(pdc);
		}
		#endregion


		#region Helper Class - PropertyOrderPair
		private class PropertyOrderPair : IComparable<PropertyOrderPair>
		{
			private int _order;
			private string _name;

			public string Name
			{
				get { return _name; }
			}

			public PropertyOrderPair(string name, int order)
			{
				_order = order;
				_name = name;
			}

			public int CompareTo(PropertyOrderPair other)
			{
				if (other._order == _order)
				{
					//
					// If order not specified, sort by name
					//
					return string.Compare(_name, other._name);
				}
				else if (other._order > _order)
				{
					return -1;
				}

				return 1;
			}
		}

		#endregion
	}
}
