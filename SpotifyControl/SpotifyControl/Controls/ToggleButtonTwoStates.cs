using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.ComponentModel;

namespace CG.SpotifyControl.Controls
{
	public class ToggleButtonTwoStates : ToggleButton
	{
		public static readonly DependencyProperty ButtonStateCheckedProperty;
		public static readonly DependencyProperty ButtonStateUncheckedProperty;

		static ToggleButtonTwoStates()
		{
			ButtonStateCheckedProperty = DependencyProperty.Register("ButtonStateChecked", typeof(FrameworkElement), typeof(ToggleButtonTwoStates));
			ButtonStateUncheckedProperty = DependencyProperty.Register("ButtonStateUnchecked", typeof(FrameworkElement), typeof(ToggleButtonTwoStates));
		}

		[Category("Appearance"), Bindable(true)]
		public FrameworkElement ButtonStateChecked
		{
			get { return (FrameworkElement)this.GetValue(ButtonStateCheckedProperty); }
			set { this.SetValue(ButtonStateCheckedProperty, value); }
		}

		[Category("Appearance"), Bindable(true)]
		public FrameworkElement ButtonStateUnchecked
		{
			get { return (FrameworkElement)this.GetValue(ButtonStateUncheckedProperty); }
			set { this.SetValue(ButtonStateUncheckedProperty, value); }
		}


		protected override void OnToggle()
		{
			base.OnToggle();

			// Workaround to update toggle button correctly (to update if change is not applied on binding)
			this.GetBindingExpression(IsCheckedProperty).UpdateTarget();
		}
	}
}
