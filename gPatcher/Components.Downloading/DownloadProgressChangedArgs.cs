using System;
using System.Runtime.CompilerServices;

namespace gPatcher.Components.Downloading
{
	public class DownloadProgressChangedArgs : EventArgs
	{
		public int CurrentDownload
		{
			get;
			set;
		}

		public int CurrentPercentage
		{
			get;
			set;
		}

		public string DownloadSpeed
		{
			get;
			set;
		}

		public string FileName
		{
			get;
			set;
		}

		public string PackName
		{
			get;
			set;
		}

		public int TotalDownloads
		{
			get;
			set;
		}

		public int TotalPercentage
		{
			get;
			set;
		}

		public DownloadProgressChangedArgs()
		{
		}
	}
}