using System;
using System.Runtime.CompilerServices;

namespace gPatcher.Components.Downloading
{
	public class DownloadCompletedArgs : EventArgs
	{
		public bool Cancelled
		{
			get;
			private set;
		}

		public Exception Error
		{
			get;
			private set;
		}

		public DownloadCompletedArgs(bool cancelled)
		{
			this.Cancelled = cancelled;
		}

		public DownloadCompletedArgs(bool cancelled, Exception error) : this(cancelled)
		{
			this.Error = error;
		}
	}
}