using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CG.SpotifyControl.Metadata
{
	public class MetadataInfos
	{
		public const ushort YEAR_DEFAULT = ushort.MinValue;
		public static readonly TimeSpan LENGTH_DEFAULT = TimeSpan.Zero;

		internal MetadataInfos(string albumName, Bitmap albumArt, int albumYear, TimeSpan trackLength)
		{
			this.AlbumName = albumName;
			this.AlbumArt = albumArt;
			this.AlbumYear = albumYear;
			this.TrackLength = trackLength;
		}


		public string AlbumName
		{
			get;
			private set;
		}

		public Bitmap AlbumArt
		{
			get;
			private set;
		}

		public int AlbumYear
		{
			get;
			private set;
		}

		public TimeSpan TrackLength
		{
			get;
			private set;
		}
	}
}
