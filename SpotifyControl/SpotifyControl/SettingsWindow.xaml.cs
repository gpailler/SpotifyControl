using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CG.SpotifyControl
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			InitializeComponent();

			//Workaround to set icon
			BitmapImage icon = (BitmapImage) Application.Current.FindResource("ApplicationIcon");
			this.Icon = BitmapFrame.Create(new Uri(icon.BaseUri, icon.UriSource));
		}
	}
}
