using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CG.SpotifyControl.Interfaces
{
	public interface ISpotifyActions
	{
		void PlayPause();

		void PlayPrev();

		void PlayNext();

		void Mute();

		void VolumeUp();

		void VolumeDown();

		void BringToTop();
	}
}
