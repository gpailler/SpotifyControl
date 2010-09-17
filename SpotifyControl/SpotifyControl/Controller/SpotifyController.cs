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
using CG.SpotifyControl.Interfaces;

namespace CG.SpotifyControl.Controller
{
	public class SpotifyController : ISpotifyState, ISpotifyActions, IDisposable, INotifyPropertyChanged
	{
		private const uint WATCHING_INTERVAL = 100;
		private const string SPOTIFY_PROCESS_NAME = "spotify";
		internal const string SPOTIFY_WINDOW_NAME = "Spotify";
		internal const string SPOTIFY_WINDOW_CLASS_NAME = "SpotifyMainWindow";

		private volatile bool _isUpdatingSpotifyState = false;
		private readonly System.Threading.Timer _spotifyWatchingTimer = null;

		//Used to monitor spotify instance
		private int _processId = 0;
		private IntPtr _mainWindow = IntPtr.Zero;
		private string _windowName = null;
		private readonly TrackInfos _trackInfos = new TrackInfos();

		// !! The dash symbol is not standard, it's a long dash
		private static readonly Regex _trackRegex = new Regex(string.Format(@"^{0}\s\-\s(?<Artist>.*)\s–\s(?<Title>.*)$", SpotifyController.SPOTIFY_WINDOW_NAME));



		public SpotifyController()
		{
			// Initialize infos
			this.UpdateSpotifyState(false);

			// Start a timer to monitor spotify instance periodically
			_spotifyWatchingTimer = new System.Threading.Timer(
				(data) => UpdateSpotifyState(true),
				null,
				WATCHING_INTERVAL,
				WATCHING_INTERVAL);
		}

		public SpotifyCommands Commands
		{
			get { return SingletonCustomCtorProvider<SpotifyCommands>.InstanceCustomCtor(() => new SpotifyCommands(this)); }
		}

		public void Dispose()
		{
			_spotifyWatchingTimer.Dispose();
		}

		public event PropertyChangedEventHandler PropertyChanged;


		#region Updates

		private void UpdateSpotifyState(bool raiseEvent)
		{
			//If update take more time than timer tick, quit immediately
			if (_isUpdatingSpotifyState)
				return;

			_isUpdatingSpotifyState = true;

			try
			{
				//Retrieve spotify process id
				Process[] processes = Process.GetProcessesByName(SPOTIFY_PROCESS_NAME);
				int processId = (processes.Length == 0) ? 0 : processes[0].Id;

				UpdateSpotifyState(processId);

				if (raiseEvent)
				{
					if (this.ProcessIdChanged)
					{
						RaiseSpotifyStatusChanged(this.IsSpotifyRunning ? SpotifyStatus.Running : SpotifyStatus.Closed);
						return;
					}

					if (this.WindowNameChanged)
					{
						if (this.IsPlayingChanged)
						{
							RaiseSpotifyStatusChanged(this.IsPlaying ? SpotifyStatus.Playing : SpotifyStatus.Stopped);
						}
						else
						{
							RaiseSpotifyStatusChanged(SpotifyStatus.TrackChanged);
						}

						return;
					}
				}
			}
			finally
			{
				_isUpdatingSpotifyState = false;
			}
		}

		private void UpdateSpotifyState(int processId)
		{
			this.ProcessIdChanged = (this.ProcessId != processId);

			this.ProcessId = processId;
			_mainWindow = IntPtr.Zero;

			if (this.ProcessId != 0)
			{
				//We use pinvoke because Process infos from Process class doesn't contains accurate informations
				_mainWindow = Win32PInvoke.FindWindow(SPOTIFY_WINDOW_CLASS_NAME, null);
				if (_mainWindow != IntPtr.Zero)
				{
					string windowName = Win32PInvoke.GetWindowText(_mainWindow);
					this.WindowNameChanged = (this.WindowName != null && windowName != this.WindowName);

					bool previousIsPlayingState = this.IsPlaying;
					this.WindowName = windowName;
					this.IsPlayingChanged = (previousIsPlayingState != this.IsPlaying);

					UpdateTrackInfos();
				}
				else
				{
					_trackInfos.ClearInfos();
				}
			}
		}

		private void UpdateTrackInfos()
		{
			Match m = _trackRegex.Match(this.WindowName);
			if (m.Success)
			{
				//We update only if track has changed
				if (_trackInfos.ArtistName != m.Groups["Artist"].Value || _trackInfos.TrackName != m.Groups["Title"].Value)
				{
					_trackInfos.UpdateInfos(m.Groups["Artist"].Value, m.Groups["Title"].Value);
				}
			}
			else
			{
				_trackInfos.ClearInfos();
			}
		}

		#endregion


		#region Properties

		private bool ProcessIdChanged
		{
			get;
			set;
		}

		private bool WindowNameChanged
		{
			get;
			set;
		}

		private bool IsPlayingChanged
		{
			get;
			set;
		}

		private int ProcessId
		{
			get { return _processId; }
			set
			{
				if (_processId == value)
					return;

				_processId = value;
				this.PropertyChanged.Notify(() => IsSpotifyRunning);
			}
		}

		private string WindowName
		{
			get { return _windowName; }
			set
			{
				if (_windowName == value)
					return;

				_windowName = value;
				this.PropertyChanged.Notify(() => IsPlaying);
			}
		}

		#endregion


		#region ISpotifyState

		public bool IsSpotifyRunning
		{
			get { return (this.ProcessId != 0); }
		}

		public bool IsPlaying
		{
			get
			{
				if (!this.IsSpotifyRunning) return false;
				return (!(string.IsNullOrEmpty(this.WindowName) || this.WindowName == SpotifyController.SPOTIFY_WINDOW_NAME));
			}

			set
			{
				if (this.IsPlaying == value)
					return;

				this.PlayPause();
			}
		}

		public bool IsSpotifyVisible
		{
			get
			{
				if (!this.IsSpotifyRunning) return false;
				return Win32PInvoke.IsWindowVisible(_mainWindow);
			}
		}

		public ITrackInfos TrackInfos
		{
			get { return _trackInfos; }
		}

		public SpotifyStatus Status
		{
			get; 
			private set;
		}

		#endregion


		#region ISpotifyActions

		// Based on http://spotifycontrol.googlecode.com/svn/trunk/SpotifyControl/ControllerClass.vb

		public void PlayPause()
		{
			if (!this.IsSpotifyRunning) return;
			Win32PInvoke.PostMessage(_mainWindow, 0x319, IntPtr.Zero, new IntPtr(0xE0000));

			PropertyChanged.Notify(() => IsPlaying);
		}

		public void PlayPrev()
		{
			if (!this.IsSpotifyRunning) return;
			Win32PInvoke.PostMessage(_mainWindow, 0x319, IntPtr.Zero, new IntPtr(0xC0000));
		}

		public void PlayNext()
		{
			if (!this.IsSpotifyRunning) return;
			Win32PInvoke.PostMessage(_mainWindow, 0x319, IntPtr.Zero, new IntPtr(0xB0000));
		}

		public void Mute()
		{
			if (!this.IsSpotifyRunning) return;

			//This will press the ctrl key and the shift key then send a the KeyDown to the spotifyHandle
			Win32PInvoke.keybd_event(Keys.ControlKey, 0x1D, Win32PInvoke.KeyEvent.None, UIntPtr.Zero);
			Win32PInvoke.keybd_event(Keys.ShiftKey, 0x1D, Win32PInvoke.KeyEvent.None, UIntPtr.Zero);
			Win32PInvoke.PostMessage(_mainWindow, 0x100, new IntPtr((int)Keys.Down), IntPtr.Zero);

			//Wait a little
			Thread.Sleep(100);

			//Release the ctrlkey and shift key
			Win32PInvoke.keybd_event(Keys.ControlKey, 0x1D, Win32PInvoke.KeyEvent.KeyUp, UIntPtr.Zero);
			Win32PInvoke.keybd_event(Keys.ShiftKey, 0x1D, Win32PInvoke.KeyEvent.KeyUp, UIntPtr.Zero);
		}

		public void VolumeUp()
		{
			if (!this.IsSpotifyRunning) return;

			//This will press the ctrl key then send a the KeyUP to the spotifyHandle
			Win32PInvoke.keybd_event(Keys.ControlKey, 0x1D, Win32PInvoke.KeyEvent.None, UIntPtr.Zero);
			Win32PInvoke.PostMessage(_mainWindow, 0x100, new IntPtr((int)Keys.Up), IntPtr.Zero);

			//Wait a little
			Thread.Sleep(100);

			//Release the ctrlkey
			Win32PInvoke.keybd_event(Keys.ControlKey, 0x1D, Win32PInvoke.KeyEvent.KeyUp, UIntPtr.Zero);
		}

		public void VolumeDown()
		{
			if (!this.IsSpotifyRunning) return;

			//This will press the ctrl key then send a the KeyDown to the spotifyHandle
			Win32PInvoke.keybd_event(Keys.ControlKey, 0x1D, Win32PInvoke.KeyEvent.None, UIntPtr.Zero);
			Win32PInvoke.PostMessage(_mainWindow, 0x100, new IntPtr((int)Keys.Down), IntPtr.Zero);

			//Wait a little
			Thread.Sleep(100);

			//Release the ctrlkey
			Win32PInvoke.keybd_event(Keys.ControlKey, 0x1D, Win32PInvoke.KeyEvent.KeyUp, UIntPtr.Zero);
		}

		public void BringToTop()
		{
			if (!this.IsSpotifyRunning) return;

			Win32PInvoke.ShowWindow(_mainWindow, Win32PInvoke.WindowShowStyle.ShowNormal);
			Win32PInvoke.SetForegroundWindow(_mainWindow);
			Win32PInvoke.SetFocus(_mainWindow);
		}

		#endregion


		#region Status event

		public event Action<SpotifyController, SpotifyStatus> SpotifyStatusChanged;

		private void RaiseSpotifyStatusChanged(SpotifyStatus status)
		{
			this.Status = status;

			Action<SpotifyController, SpotifyStatus> handler = SpotifyStatusChanged;
			if (handler != null)
				handler(this, status);
		}


		#endregion
	}
}
