using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spotify;
using System.Threading;
using System.Drawing;
using System.ComponentModel;
using CG.Common.Attributes;
using CG.Common.Converters;

namespace CG.SpotifyControl.Metadata.Resolvers
{
	[DisplayName("Spotify lib"), Description("Find related track informations using Spotify library API (more accurate)")]
	class SpotifyLibResolver : IMetadataTrackResolver
	{
		#region Key

		private static byte[] _apiKey = {
			/* spotify API key */
		};

		#endregion

		private static readonly TimeSpan TIMEOUT = new TimeSpan(0, 0, 5);
		private const string QUERY = "artist:\"{0}\" AND title:\"{1}\"";

		private static ManualResetEvent loggedIn = new ManualResetEvent(false);
		private Session _session;

		private SpotifyLibResolverOptions _options;

		public SpotifyLibResolver()
		{
			_options = new SpotifyLibResolverOptions();
			_session = Session.CreateInstance(_apiKey, System.IO.Path.GetTempPath(), System.IO.Path.GetTempPath(), "SpotifyControl");
		}

		public void Initialize()
		{
			_session.LogIn(_options.Username, _options.Password);
			_session.OnLoginComplete += new SessionEventHandler(s_OnLoginComplete);
		}

		public void Dispose()
		{
			_session.LogOutSync(TIMEOUT);
			loggedIn.Reset();
		}

		public IMetadataTrackResolverOptions ResolverOptions
		{
			get
			{
				return _options;
			}
		}

		void s_OnLoginComplete(Session sender, SessionEventArgs e)
		{
			loggedIn.Set();
		}

		public MetadataInfos Search(string artistName, string trackName)
		{
			loggedIn.WaitOne();

			if (_session.ConnectionState != sp_connectionstate.LOGGED_IN)
			{
				MainWindow.ShowCustomBalloon("Unable to connect Spotify (wrong Username/Password)", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Error);
				return null;
			}

			Search search = _session.SearchSync(string.Format(QUERY, artistName, trackName), 0, 1, 0, 1, 0, 1, TIMEOUT);
			if (search.Tracks.Length == 0)
			{
				MainWindow.ShowCustomBalloon("Unable to find associate track infos", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
				return null;
			}

			Bitmap albumArt = _session.LoadImageSync(search.Tracks[0].Album.CoverId, TIMEOUT);

			MetadataInfos metadataTrack = new MetadataInfos(
				search.Tracks[0].Album.Name,
				albumArt,
				search.Tracks[0].Album.Year,
				new TimeSpan(0, 0, 0, 0, search.Tracks[0].Duration));

			return metadataTrack;
		}
	}

	[TypeConverter(typeof(CustomSortTypeConverter))]
	class SpotifyLibResolverOptions : IMetadataTrackResolverOptions
	{
		private string _username;
		private string _password;

		public SpotifyLibResolverOptions()
		{
			_username = Properties.Settings.Default.SpotifyLibResolverOptionsUsername;
			_password = Properties.Settings.Default.SpotifyLibResolverOptionsPassword;
		}

		public void Save()
		{
			Properties.Settings.Default.SpotifyLibResolverOptionsUsername = _username;
			Properties.Settings.Default.SpotifyLibResolverOptionsPassword = _password;

			Properties.Settings.Default.Save();
		}

		[PropertyOrder(1)]
		[DisplayName("Username")]
		[Description("Spotify account username to connect Spotify API")]
		[Category("Spotify account")]
		public string Username
		{
			get { return _username; }
			set { _username = value; }
		}

		[PropertyOrder(2)]
		[DisplayName("Password")]
		[Description("Spotify account password to connect Spotify API")]
		[Category("Spotify account")]
		public string Password
		{
			get { return _password; }
			set { _password = value; }
		}

	}
}
