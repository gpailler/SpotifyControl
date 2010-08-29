using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using System.Drawing;

namespace CG.SpotifyControl.Metadata.Resolvers.WebServices
{
	class LastFmWebService : WebServiceBase
	{
		private const string LAST_FM_KEY = "" /*lastFM api key */;
		private const string LAST_FM_URL = "http://ws.audioscrobbler.com/2.0/?method=track.getinfo&api_key={0}&artist={1}&track={2}";

		public LastFmWebService(WebServicesResolverOptions options) : base(options)
		{
				
		}

		protected override string GetRequestUrl(string artistName, string trackName)
		{
			return string.Format(LAST_FM_URL, LAST_FM_KEY, artistName, trackName);
		}

		public override bool IsValid
		{
			get
			{
				if (!base.IsValid) return false;
				return (Xml.GetElementsByTagName("image").Count >= 2);
			}
		}


		#region Properties

		private string AlbumArtUrl
		{
			get
			{
				return Xml.GetElementsByTagName("image")[1].InnerText;
			}
		}

		public Bitmap AlbumArt
		{
			get
			{
				return DownloadBitmap(this.AlbumArtUrl);
			}
		}

		#endregion
	}
}
