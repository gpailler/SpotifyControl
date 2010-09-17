using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG.SpotifyControl.Controller;

namespace CG.SpotifyControl.Interfaces
{
	public interface ISpotifyState
	{
		bool IsSpotifyRunning
		{
			get;
		}

		bool IsPlaying
		{
			get;
		}

		bool IsSpotifyVisible
		{
			get;
		}

		ITrackInfos TrackInfos
		{
			get;
		}

		SpotifyStatus Status
		{
			get;
		}
	}


	public enum SpotifyStatus
	{
		Running,
		Closed,
		Playing,
		Stopped,
		TrackChanged
	}
}
