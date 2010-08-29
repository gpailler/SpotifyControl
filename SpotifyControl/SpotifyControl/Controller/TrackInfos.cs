using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using CG.Common.Extensions;

namespace CG.SpotifyControl.Controller
{
	public class TrackInfos : INotifyPropertyChanged
	{
		private Metadata.MetadataInfos _metadataInfos;

		internal void UpdateInfos(string artistName, string trackName)
		{
			this.ArtistName = artistName;
			this.TrackName = trackName;

			if (Metadata.MetadataResolverManager.ResolverInstance == null)
			{
				_metadataInfos = null;
			}
			else
			{
				_metadataInfos = Metadata.MetadataResolverManager.ResolverInstance.Search(artistName, trackName);
			}
			PropertyChanged.NotifyAll(this);
		}

		internal void ClearInfos()
		{
			if (this.ArtistName == null || this.TrackName == null)
				return;

			this.ArtistName = null;
			this.TrackName = null;

			_metadataInfos = null;
			PropertyChanged.NotifyAll(this);
		}


		public string TrackName
		{
			get;
			private set;
		}

		public string ArtistName
		{
			get;
			private set;
		}

		public string AlbumName
		{
			get
			{
				return (_metadataInfos == null) ? null : _metadataInfos.AlbumName;
			}
		}

		public int AlbumYear
		{
			get
			{
				return (_metadataInfos == null) ? Metadata.MetadataInfos.YEAR_DEFAULT : _metadataInfos.AlbumYear;
			}
		}

		public TimeSpan TrackLength
		{
			get
			{
				return (_metadataInfos == null) ? Metadata.MetadataInfos.LENGTH_DEFAULT : _metadataInfos.TrackLength;
			}
		}

		public Bitmap AlbumArt
		{
			get
			{
				return (_metadataInfos == null) ? null : _metadataInfos.AlbumArt;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
