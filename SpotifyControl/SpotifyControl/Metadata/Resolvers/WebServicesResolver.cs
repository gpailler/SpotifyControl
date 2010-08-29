using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CG.SpotifyControl.Metadata.Resolvers.WebServices;
using System.ComponentModel;
using CG.Common.Converters;
using CG.Common.Attributes;
using System.Reflection;

namespace CG.SpotifyControl.Metadata.Resolvers
{
	[DisplayName("Web services"), Description("Find related track informations using Spotify and Last.fm web services")]
	class WebServicesResolver : IMetadataTrackResolver
	{
		private SpotifyWebService _spotifyWebService = null;
		private LastFmWebService _lastFmWebService = null;
		private WebServicesResolverOptions _options;

		public WebServicesResolver()
		{
			_options = new WebServicesResolverOptions();
		}

		public void Initialize()
		{
			_spotifyWebService = new SpotifyWebService(_options);
			_lastFmWebService = new LastFmWebService(_options);
		}

		public void Dispose()
		{
			_spotifyWebService = null;
			_lastFmWebService = null;
		}

		public IMetadataTrackResolverOptions ResolverOptions
		{
			get { return _options; }
		}
		
		public MetadataInfos Search(string artistName, string trackName)
		{
			if (!_spotifyWebService.Search(artistName, trackName))
			{
				MainWindow.ShowCustomBalloon("Unable to retrieve track infos", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Error);
				return null;
			}


			Bitmap albumArt = null;
			if (_lastFmWebService.Search(artistName, trackName))
				albumArt = _lastFmWebService.AlbumArt;

			MetadataInfos metadataTrack = new MetadataInfos(
				_spotifyWebService.AlbumName,
				albumArt,
				_spotifyWebService.AlbumYear,
				_spotifyWebService.TrackLength);

			return metadataTrack;
		}
	}
	
	[TypeConverter(typeof(CustomSortTypeConverterFiltered))]
	class WebServicesResolverOptions : IMetadataTrackResolverOptions, IMetadataTrackResolverOptionsChanged
	{
		private eProxyType _proxyType;
		private string _address;
		private ushort _port;
		private bool _useAuthentication;
		private string _username;
		private string _password;


		public WebServicesResolverOptions()
		{
			_address = Properties.Settings.Default.WebServicesResolverOptionsProxyAddress;
			_port = Properties.Settings.Default.WebServicesResolverOptionsProxyPort;
			_proxyType = Properties.Settings.Default.WebServicesResolverOptionsProxyType;
		}

		public void Save()
		{
			Properties.Settings.Default.WebServicesResolverOptionsProxyAddress = _address;
			Properties.Settings.Default.WebServicesResolverOptionsProxyPort = _port;
			Properties.Settings.Default.WebServicesResolverOptionsProxyType = _proxyType;

			Properties.Settings.Default.Save();
		}


		public event EventHandler PropertiesChanged;

		private void RaisePropertiesChanged()
		{
			if (PropertiesChanged != null)
				PropertiesChanged(this, EventArgs.Empty);
		}

		[Browsable(false)]
		public bool UseProxy
		{
			get { return (_proxyType != eProxyType.None); }
		}

		[PropertyOrder(2)]
		[DisplayName("Proxy type")]
		public eProxyType ProxyType
		{
			get { return _proxyType; }
			set
			{
				_proxyType = value;
				this.RaisePropertiesChanged();
			}
		}

		[PropertyOrder(3)]
		[DisplayName("Proxy address")]
		[Visibility(typeof(WebServicesResolverOptions), "UseProxy")]
		public string Address
		{
			get { return _address; }
			set { _address = value; }
		}

		[PropertyOrder(4)]
		[DisplayName("Port")]
		[Visibility(typeof(WebServicesResolverOptions), "UseProxy")]
		public ushort Port
		{
			get { return _port; }
			set { _port = value; }
		}

		[PropertyOrder(5)]
		[DisplayName("Use authentication")]
		[Visibility(typeof(WebServicesResolverOptions), "UseProxy")]
		public bool UseAuthentication
		{
			get { return _useAuthentication; }
			set
			{
				_useAuthentication = value;
				this.RaisePropertiesChanged();
			}
		}

		[PropertyOrder(6)]
		[DisplayName("Username")]
		[Visibility(typeof(WebServicesResolverOptions), "UseAuthentication")]
		public string Username
		{
			get { return _username; }
			set { _username= value; }
		}

		[PropertyOrder(7)]
		[DisplayName("Password")]
		[Visibility(typeof(WebServicesResolverOptions), "UseAuthentication")]
		public string Password
		{
			get { return _password; }
			set { _password= value; }
		}




		class CustomSortTypeConverterFiltered : CustomSortTypeConverter
		{
			public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
			{
				//Construct properties list using visibility attributes

				List<PropertyDescriptor> lstProperties = new List<PropertyDescriptor>();

				PropertyDescriptorCollection properties = base.GetProperties(context, value, attributes);
				foreach (PropertyDescriptor property in properties)
				{
					VisibilityAttribute visiblityAttribute = property.Attributes[typeof(VisibilityAttribute)] as VisibilityAttribute;
					if (visiblityAttribute == null)
					{
						lstProperties.Add(property);
					}
					else
					{
						if (visiblityAttribute.CheckCondition(value))
						{
							lstProperties.Add(property);
						}
					}
					
				}

				return new PropertyDescriptorCollection(lstProperties.ToArray());
			}
		}
		

		[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
		class VisibilityAttribute : Attribute
		{
			private readonly PropertyInfo _conditionProperty;
			private readonly bool _invertCondition = false;

			public VisibilityAttribute(Type type, string conditionProperty, bool invertCondition = false)
			{
				if (type == null) throw new ArgumentNullException("type");
				if (string.IsNullOrEmpty(conditionProperty)) throw new ArgumentNullException("conditionProperty");

				PropertyInfo pi = type.GetProperty(conditionProperty, BindingFlags.Instance | BindingFlags.Public);
				if (pi == null) throw new ArgumentException(string.Format("Unable to find public property '{0}' in type '{1}'", conditionProperty, type));
				if (!pi.CanRead) throw new ArgumentException(string.Format("Unable to find getter for property '{0}'", conditionProperty));

				_conditionProperty = pi;
				_invertCondition = invertCondition;
			}

			public bool CheckCondition(object obj)
			{
				bool ret = (bool)_conditionProperty.GetValue(obj, null);
				return ret | _invertCondition;
			}
		}
	}

	[TypeConverter(typeof(DisplayableEnumConverter))]
	public enum eProxyType
	{
		[EnumDisplayName("None")]
		None,
		[EnumDisplayName("HTTP")]
		Http,
		[EnumDisplayName("SOCKS 4")]
		Socks4,
		[EnumDisplayName("SOCKS 5")]
		Socks5
	}
}
