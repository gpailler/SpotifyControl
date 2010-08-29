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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CG.SpotifyControl.GUI
{
	/// <summary>
	/// Interaction logic for SettingsControl.xaml
	/// </summary>
	public partial class SettingsControl : UserControl
	{
		public SettingsControl()
		{
			InitializeComponent();

			this.cbResolvers.ItemsSource = Metadata.MetadataResolverManager.ResolversInfos.OrderBy((e) => e.DisplayName);

			foreach (Metadata.MetadataResolverManager.ResolverInfos resolverInfos in this.cbResolvers.ItemsSource)
			{
				if (resolverInfos.IsSelected)
				{
					this.cbResolvers.SelectedValue = resolverInfos.ResolverType;
					break;
				}
			}
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			//Save selected resolver
			Metadata.MetadataResolverManager.ResolverInfos resolverInfos = (Metadata.MetadataResolverManager.ResolverInfos)this.cbResolvers.SelectedItem;
			Properties.Settings.Default.MetadataResolver = resolverInfos.ResolverType.FullName;
			Properties.Settings.Default.Save();

			//Save resolver settings
			if (pgResolverSettings.Instance != null)
			{
				((Metadata.Resolvers.IMetadataTrackResolverOptions)pgResolverSettings.Instance).Save();
			}

			Metadata.MetadataResolverManager.UpdateResolver();
		}

		private void pgResolverSettings_InstanceChanged(object sender, EventArgs e)
		{
			if (pgResolverSettings.Instance is Metadata.Resolvers.IMetadataTrackResolverOptionsChanged)
				((Metadata.Resolvers.IMetadataTrackResolverOptionsChanged)pgResolverSettings.Instance).PropertiesChanged += new EventHandler(SettingsControl_PropertiesChanged);
		}

		void SettingsControl_PropertiesChanged(object sender, EventArgs e)
		{
			pgResolverSettings.Refresh();
		}
	}
}
