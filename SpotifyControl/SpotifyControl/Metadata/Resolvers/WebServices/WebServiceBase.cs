using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;
using System.Web;
using System.Drawing;

namespace CG.SpotifyControl.Metadata.Resolvers.WebServices
{
	abstract class WebServiceBase
	{
		private const int TIMEOUT = 5000;

		private XmlDocument _Xml = new XmlDocument();
		private string _artistName;
		private string _trackName;
		private static WebClient _webClient = new WebClient();
		private WebServicesResolverOptions _options;

		public WebServiceBase(WebServicesResolverOptions options)
		{
			_options = options;
		}

		public bool Search(string artistName, string trackName)
		{
			_artistName = artistName;
			_trackName = trackName;
			_Xml = new XmlDocument();

			Stream stream = DownloadFile(GetRequestUrl(HttpUtility.UrlEncode(artistName), HttpUtility.UrlEncode(trackName)));
			if (stream != null)
			{
				_Xml.Load(stream);
				stream.Dispose();
			}

			return IsValid;
		}

		public virtual bool IsValid
		{
			get { return (_Xml != null); }
		}

		protected abstract string GetRequestUrl(string artistName, string trackName);

		protected XmlDocument Xml
		{
			get { return _Xml; }
		}

		private Stream DownloadFile(string url)
		{
			try
			{
				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
				request.Method = "GET";
				request.UserAgent = "SpotifyControl";
				request.Timeout = TIMEOUT;
				request.KeepAlive = false;
				request.ProtocolVersion = HttpVersion.Version10;
				request.ContentType = "text/xml";

				HttpWebResponse response = (HttpWebResponse)request.GetResponse();

				return response.GetResponseStream();
			}
			catch (Exception)
			{
				return null;
			}
		}

		protected Bitmap DownloadBitmap(string url)
		{
			MemoryStream stream = null;
			try
			{
				stream = new MemoryStream(_webClient.DownloadData(url));
				return (Bitmap)Image.FromStream(stream);
			}
			catch (Exception)
			{
				return null;
			}
			finally
			{
				if (stream != null)
					stream.Dispose();
			}
		}
	}
}
