using System;
using System.Net;
using System.Runtime.CompilerServices;

namespace gPatcher.Components.General
{
	internal class MyWebClient : WebClient
	{
		public int Timeout
		{
			get;
			set;
		}

		public MyWebClient()
		{
			this.Timeout = 10000;
		}

		public MyWebClient(int timeout)
		{
			this.Timeout = timeout;
		}

		protected override WebRequest GetWebRequest(Uri address)
		{
			WebRequest webRequest = base.GetWebRequest(address);
			if (webRequest != null)
			{
				webRequest.Timeout = this.Timeout;
			}
			return webRequest;
		}
	}
}