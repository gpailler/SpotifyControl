using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CG.SpotifyControl.Metadata.Resolvers
{
	[DisplayName("Dummy resolver"), Description("The resolver does nothing")]
	class DummyResolver : IMetadataTrackResolver
	{
		public void Initialize()
		{
		}

		public MetadataInfos Search(string artistName, string trackName)
		{
			return null;
		}

		public IMetadataTrackResolverOptions ResolverOptions
		{
			get { return null; }
		}

		public void Dispose()
		{
		}
	}
}
