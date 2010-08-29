using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using SWF = System.Windows.Forms;
using System.Drawing;
using System.Windows.Threading;

namespace CG.SpotifyControl
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
		}


		void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
#if !DEBUG
			MessageBox.Show(string.Format(@"Exception{0}Exception type: {1}{0}Exception message: {2}{0}Exception callstack: {3}",
						Environment.NewLine,
						e.Exception.GetType().FullName,
						e.Exception.Message,
						e.Exception.StackTrace), "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
#endif
		}
	}
}
