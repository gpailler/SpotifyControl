using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CG.SpotifyControl.Metadata.Resolvers
{
	public interface IMetadataTrackResolver : IDisposable
	{
		void Initialize();
		MetadataInfos Search(string artistName, string trackName);
		IMetadataTrackResolverOptions ResolverOptions
		{
			get;
		}
	}


	public interface IMetadataTrackResolverOptions
	{
		void Save();
	}

	public interface  IMetadataTrackResolverOptionsChanged
	{
		event EventHandler PropertiesChanged;
	}
}
