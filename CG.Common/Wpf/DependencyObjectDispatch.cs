using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CG.Common.Wpf
{
	public class DependencyObjectDispatch : DependencyObject
	{
		[System.Diagnostics.DebuggerStepThrough()]
		public object Dispatch(Func<object> f)
		{
			if (this.Dispatcher.CheckAccess())
			{
				return f();
			}
			else
			{
				return this.Dispatcher.Invoke(f, null);
			}
		}

		[System.Diagnostics.DebuggerStepThrough()]
		public void Dispatch(Action f)
		{
			this.Dispatch(() => { f(); return null; });
		}
	}
}
