using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Web;
using System.Globalization;

namespace CG.SpotifyControl.Metadata.Resolvers.WebServices
{
	class SpotifyWebService : WebServiceBase
	{
		private const string SPOTIFY_URL = "http://ws.spotify.com/search/1/track?q=artist%3a%22{0}%22+AND+title%3a%22{1}%22";

		public SpotifyWebService(WebServicesResolverOptions options) : base(options)
		{
				
		}

		protected override string GetRequestUrl(string artistName, string trackName)
		{
			return string.Format(SPOTIFY_URL, artistName, trackName);
		}

		public override bool IsValid
		{
			get
			{
				if (!base.IsValid) return false;

				int nbResults = 0;
				if (Xml.GetElementsByTagName("opensearch:totalResults").Count == 0) return false;

				int.TryParse(Xml.GetElementsByTagName("opensearch:totalResults")[0].InnerText, out nbResults);
				return (nbResults > 0);
			}
		}

		#region Properties

		public string AlbumName
		{
			get
			{
				return Xml.GetElementsByTagName("album")[0].ChildNodes[0].InnerText;
			}
		}

		public int AlbumYear
		{
			get
			{
				return int.Parse(Xml.GetElementsByTagName("album")[0].ChildNodes[1].InnerText);
			}
		}

		public TimeSpan TrackLength
		{
			get
			{
				double seconds = double.Parse(Xml.GetElementsByTagName("length")[0].InnerText, NumberStyles.AllowDecimalPoint, CultureInfo.GetCultureInfo("en-US"));
				return new TimeSpan(0, 0, (int)Math.Floor(seconds), (int)((seconds - Math.Floor(seconds)) * 1000));
			}
		}

		#endregion
	}
}
