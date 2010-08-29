using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Linq.Expressions;
using System.Reflection;

namespace CG.Common.Wpf
{


	//http://www.ingebrigtsen.info/post/2009/01/09/DependencyProperties-why-art-though-%28so-much-hassle%29.aspx

	public static class DependencyPropertyHelper
	{
		public static DependencyProperty Register<TOwner, TResult>(Expression<Func<TOwner, TResult>> expression)
		{
			return Register<TOwner, TResult>(expression, default(TResult));
		}

		public static DependencyProperty Register<TOwner, TResult>(Expression<Func<TOwner, TResult>> expression, TResult defaultValue)
		{
			var lambda = expression as LambdaExpression;
			MemberExpression memberExpression;
			if (lambda.Body is UnaryExpression)
			{
				var unaryExpression = lambda.Body as UnaryExpression;
				memberExpression = unaryExpression.Operand as MemberExpression;
			}
			else
			{
				memberExpression = lambda.Body as MemberExpression;
			}
			var propertyInfo = memberExpression.Member as PropertyInfo;
			string dependencyPropertyName = propertyInfo.Name;

			DependencyProperty prop = DependencyProperty.Register(
				dependencyPropertyName,
				propertyInfo.PropertyType,
				typeof(TOwner),
				new PropertyMetadata(defaultValue, (o, e) =>
				{
					propertyInfo.SetValue(o, e.NewValue, null);
				}));

			return prop;
		}
	}

	public static class DependencyPropertyExtensions
	{
		public static void SetValue<T>(this DependencyObjectDispatch obj, DependencyProperty property, T value)
		{
			object oldValue = obj.GetValue<T>(property);
			if (null != oldValue && null != value)
			{
				if (oldValue.Equals(value))
				{
					return;
				}
			}
			obj.Dispatch(() => obj.SetValue(property, value));
		}

		public static T GetValue<T>(this DependencyObjectDispatch obj, DependencyProperty property)
		{
			return (T)obj.Dispatch(() => obj.GetValue(property));
		}

		public static void SetValue<T>(this DependencyObject obj, DependencyProperty property, T value)
		{
			object oldValue = obj.GetValue(property);
			if (null != oldValue && null != value)
			{
				if (oldValue.Equals(value))
				{
					return;
				}
			}
			obj.SetValue(property, value);
		}

		public static T GetValue<T>(this DependencyObject obj, DependencyProperty property)
		{
			return (T)obj.GetValue(property);
		}
	}

	public class TypeSafeDependencyProperty<TOwner, TResult>
	{
		private readonly DependencyProperty _dependencyProperty;


		private TypeSafeDependencyProperty(DependencyProperty dependencyProperty)
		{
			this._dependencyProperty = dependencyProperty;
		}


		public TResult GetValue(DependencyObjectDispatch obj)
		{
			return obj.GetValue<TResult>(this._dependencyProperty);
		}

		public void SetValue(DependencyObjectDispatch obj, TResult value)
		{
			obj.SetValue<TResult>(this._dependencyProperty, value);
		}

		public TResult GetValue(DependencyObject obj)
		{
			return obj.GetValue<TResult>(this._dependencyProperty);
		}

		public void SetValue(DependencyObject obj, TResult value)
		{
			obj.SetValue<TResult>(this._dependencyProperty, value);

		}

		public static TypeSafeDependencyProperty<TOwner, TResult> Register(Expression<Func<TOwner, TResult>> expression)
		{
			var property = DependencyPropertyHelper.Register<TOwner, TResult>(expression);

			var typeSafeProperty = new TypeSafeDependencyProperty<TOwner, TResult>(property);

			return typeSafeProperty;
		}

		public static TypeSafeDependencyProperty<TOwner, TResult> Register(Expression<Func<TOwner, TResult>> expression, TResult defaultValue)
		{
			var property = DependencyPropertyHelper.Register<TOwner, TResult>(expression, defaultValue);

			var typeSafeProperty = new TypeSafeDependencyProperty<TOwner, TResult>(property);

			return typeSafeProperty;
		}
	}
}
