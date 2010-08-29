using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Windows;
using System.ComponentModel;
using CG.Common;
using CG.Common.Extensions;

namespace CG.SpotifyControl.Controller
{
	class SpotifyInfos : ISpotifyActions, INotifyPropertyChanged
	{
		// Based on http://spotifycontrol.googlecode.com/svn/trunk/SpotifyControl/ControllerClass.vb

		private int _processId = 0;
		private IntPtr _mainWindow = IntPtr.Zero;
		private string _windowName = null;
		private readonly TrackInfos _trackInfos = new TrackInfos();

		// !! The dash symbol is not standard, it's a long dash
		private static readonly Regex _trackRegex = new Regex(string.Format(@"^{0}\s\-\s(?<Artist>.*)\s–\s(?<Title>.*)$", SpotifyController.SPOTIFY_WINDOW_NAME));

		public void Update(int processId)
		{
			this.ProcessIdChanged = (this.ProcessId != processId);

			this.ProcessId = processId;
			_mainWindow = IntPtr.Zero;

			if (this.ProcessId != 0)
			{
				//We use pinvoke because Process infos from Process class are not usable
				_mainWindow = Win32PInvoke.FindWindow(SpotifyController.SPOTIFY_WINDOW_CLASS_NAME, null);
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
				//We check if track has changed
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


		#region Handle changes

		public bool ProcessIdChanged
		{
			get;
			private set;
		}

		public bool WindowNameChanged
		{
			get;
			private set;
		}

		public bool IsPlayingChanged
		{
			get;
			private set;
		}

		#endregion


		public int ProcessId
		{
			get { return _processId; }
			private set
			{
				if (_processId == value)
					return;

				_processId = value;
				this.PropertyChanged.Notify(() => IsSpotifyRunning);
			}
		}

		public string WindowName
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
		}

		public void PlayPause()
		{
			if (!this.IsSpotifyRunning) return;
			Win32PInvoke.PostMessage(_mainWindow, 0x319, IntPtr.Zero, new IntPtr(0xE0000));
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

		public bool IsSpotifyVisible
		{
			get
			{
				if (!this.IsSpotifyRunning) return false;
				return Win32PInvoke.IsWindowVisible(_mainWindow);
			}
		}

		public TrackInfos TrackInfos
		{
			get { return _trackInfos; }
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
