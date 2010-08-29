using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CG.SpotifyControl.Controller
{
	public interface ISpotifyActions
	{
		bool IsSpotifyRunning
		{
			get;
		}

		bool IsPlaying
		{
			get;
		}

		void PlayPause();

		void PlayPrev();

		void PlayNext();

		void Mute();

		void VolumeUp();

		void VolumeDown();

		void BringToTop();

		bool IsSpotifyVisible
		{
			get;
		}

		TrackInfos TrackInfos
		{
			get;
		}
	}
}
