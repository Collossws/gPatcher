using Ginpo.gPatcher;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace gPatcher.Components.Downloading
{
	public class Download
	{
		public string Destination
		{
			get;
			private set;
		}

		public string DestinationFolder
		{
			get;
			private set;
		}

		public string FileName
		{
			get;
			private set;
		}

		public long FileSize
		{
			get;
			private set;
		}

		public Uri FileUrl
		{
			get;
			private set;
		}

		public Ginpo.gPatcher.VoicePack VoicePack
		{
			get;
			private set;
		}

		public Download(Uri fileUrl, long fileSize, string destination, Ginpo.gPatcher.VoicePack voicePack)
		{
			this.FileUrl = fileUrl;
			this.FileSize = fileSize;
			this.Destination = destination;
			this.VoicePack = voicePack;
			this.FileName = Path.GetFileName(fileUrl.LocalPath);
			this.DestinationFolder = Path.GetDirectoryName(destination);
		}
	}
}