using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using CG.Common;
using CG.Common.Extensions;

namespace CG.SpotifyControl.Controller
{
	public class SpotifyController : IDisposable, ISpotifyActions, INotifyPropertyChanged
	{
		private const uint WATCHING_INTERVAL = 100;
		private const string SPOTIFY_PROCESS_NAME = "spotify";
		internal const string SPOTIFY_WINDOW_NAME = "Spotify";
		internal const string SPOTIFY_WINDOW_CLASS_NAME = "SpotifyMainWindow";

		private volatile bool _isUpdatingSpotifyInfos = false;
		private readonly System.Threading.Timer _spotifyWatchingTimer = null;
		private readonly SpotifyInfos _spotifyInfos = new SpotifyInfos();
		private readonly SpotifyCommands _commands = null;


		#region Logic

		public SpotifyController()
		{
			//Initialize commands
			_commands = new SpotifyCommands(this);

			_spotifyInfos.PropertyChanged += new PropertyChangedEventHandler(_spotifyInfos_PropertyChanged);

			// Initialize infos
			this.UpdateSpotifyInfos(false);

			// Start a timer to check spotify infos periodically
			_spotifyWatchingTimer = new System.Threading.Timer(
				(data) => UpdateSpotifyInfos(true),
				null,
				WATCHING_INTERVAL,
				WATCHING_INTERVAL);
		}

		void _spotifyInfos_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, e);
		}

		public void Dispose()
		{
			_spotifyWatchingTimer.Dispose();
		}

		private void UpdateSpotifyInfos(bool raiseEvent)
		{
			//If update take more time than timer tick, quit immediately
			if (_isUpdatingSpotifyInfos)
				return;

			_isUpdatingSpotifyInfos = true;

			try
			{
				//Retrieve spotify process id
				Process[] processes = Process.GetProcessesByName(SPOTIFY_PROCESS_NAME);
				int processId = (processes.Length == 0) ? 0 : processes[0].Id;

				_spotifyInfos.Update(processId);

				if (_spotifyInfos.ProcessIdChanged)
				{
					if (raiseEvent)
						RaiseSpotifyStatusChanged(this._spotifyInfos.IsSpotifyRunning ? eSpotifyStatus.Running : eSpotifyStatus.Closed);
					return;
				}

				if (_spotifyInfos.WindowNameChanged)
				{
					if (_spotifyInfos.IsPlayingChanged)
					{
						if (raiseEvent)
							RaiseSpotifyStatusChanged(this._spotifyInfos.IsPlaying ? eSpotifyStatus.Playing : eSpotifyStatus.Stopped);
					}
					else
					{
						if (raiseEvent)
							RaiseSpotifyStatusChanged(eSpotifyStatus.TrackChanged);
					}
					
					return;
				}
			}
			finally
			{
				_isUpdatingSpotifyInfos = false;
			}
		}

		#endregion


		#region Forward => SpotifyInfos

		public bool IsSpotifyRunning
		{
			get { return _spotifyInfos.IsSpotifyRunning; }
		}
		
		public bool IsPlaying
		{
			get { return _spotifyInfos.IsPlaying; }
			set
			{
				if (this.IsPlaying == value)
					return;

				this.PlayPause();
			}
		}

		public void PlayPause()
		{
			_spotifyInfos.PlayPause();
			PropertyChanged.Notify(() => IsPlaying);
		}

		public void PlayPrev()
		{
			_spotifyInfos.PlayPrev();
		}

		public void PlayNext()
		{
			_spotifyInfos.PlayNext();
		}

		public void Mute()
		{
			_spotifyInfos.Mute();
		}

		public void VolumeUp()
		{
			_spotifyInfos.VolumeUp();
		}

		public void VolumeDown()
		{
			_spotifyInfos.VolumeDown();
		}

		public bool IsSpotifyVisible
		{
			get { return _spotifyInfos.IsSpotifyVisible; }
		}

		public void BringToTop()
		{
			_spotifyInfos.BringToTop();
		}

		public TrackInfos TrackInfos
		{
			get { return _spotifyInfos.TrackInfos; }
		}

		#endregion
		

		#region Status event

		public event Action<SpotifyController, eSpotifyStatus> SpotifyStatusChanged;

		private void RaiseSpotifyStatusChanged(eSpotifyStatus status)
		{
			if (SpotifyStatusChanged != null)
				SpotifyStatusChanged(this, status);
		}

		public enum eSpotifyStatus
		{
			Running,
			Closed,
			Playing,
			Stopped,
			TrackChanged
		}

		#endregion

		public event PropertyChangedEventHandler PropertyChanged;

		public SpotifyCommands Commands
		{
			get { return _commands; }
		}
	}
}
