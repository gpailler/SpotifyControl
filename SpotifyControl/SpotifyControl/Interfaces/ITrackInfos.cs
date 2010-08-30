using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CG.SpotifyControl.Interfaces
{
	public interface ITrackInfos
	{
		string TrackName
		{
			get;
		}

		string ArtistName
		{
			get;
		}

		string AlbumName
		{
			get;
		}

		int AlbumYear
		{
			get;
		}

		TimeSpan TrackLength
		{
			get;
		}

		Bitmap AlbumArt
		{
			get;
		}
	}
}
