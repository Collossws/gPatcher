using Ginpo;
using Ginpo.Extensions;
using Ginpo.gPatcher;
using gPatcher.Components.General;
using gPatcher.Helpers;
using gPatcher.Helpers.PatchInfoReading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;

namespace gPatcher.Components.Downloading
{
	public class PacksDownloader : Component
	{
		private readonly List<Download> _downloadQueue = new List<Download>();

		private readonly Stopwatch _stopwatch = new Stopwatch();

		private readonly MyWebClient _webClient;

		private readonly BackgroundWorker _worker;

		public BindingList<VoicePack> PacksQueue = new BindingList<VoicePack>();

		private int _current;

		private long _currentBytes;

		private int _downloadCount;

		private long _totalBytes;

		public bool IsBusy
		{
			get;
			private set;
		}

		public PacksDownloader()
		{
			this.IsBusy = false;
			this._webClient = new MyWebClient();
			this._webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.Download_Completed);
			this._webClient.DownloadProgressChanged += new System.Net.DownloadProgressChangedEventHandler(this.Download_ProgressChanged);
			this._worker = new BackgroundWorker()
			{
				WorkerSupportsCancellation = true
			};
			this._worker.DoWork += new DoWorkEventHandler(this._worker_DoWork);
			this._worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this._worker_RunWorkerCompleted);
		}

		private void _worker_DoWork(object sender, DoWorkEventArgs e)
		{
			this._currentBytes = (long)0;
			this._current = 0;
			this._totalBytes = (long)0;
			this._downloadQueue.Clear();
			for (int i = this.PacksQueue.Count - 1; i >= 0; i--)
			{
				IEnumerable<Download> downloadList = PacksDownloader.GetDownloadList(this.PacksQueue[i]);
				Download[] array = downloadList as Download[] ?? downloadList.ToArray<Download>();
				if (array.Any<Download>())
				{
					Download[] downloadArray = array;
					for (int j = 0; j < (int)downloadArray.Length; j++)
					{
						Download download = downloadArray[j];
						this._downloadQueue.Add(download);
					}
				}
				else
				{
					this.PacksQueue.RemoveAt(i);
				}
				if (this._worker.CancellationPending)
				{
					e.Cancel = true;
					return;
				}
			}
			this._downloadQueue.Reverse();
			foreach (Download download1 in this._downloadQueue)
			{
				PacksDownloader fileSize = this;
				fileSize._totalBytes = fileSize._totalBytes + download1.FileSize;
			}
			this._downloadCount = this._downloadQueue.Count;
		}

		private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				this.OnDownloadCompleted(this, new DownloadCompletedArgs(false, e.Error));
				return;
			}
			if (e.Cancelled)
			{
				this.OnDownloadCompleted(this, new DownloadCompletedArgs(true));
				return;
			}
			if (this._downloadCount != 0)
			{
				this.StartNext();
				return;
			}
			this.OnDownloadCompleted(null, new DownloadCompletedArgs(false));
		}

		public void AddToQueue(VoicePack pack)
		{
			this.PacksQueue.AddIfNew<VoicePack>(pack);
		}

		public void Cancel()
		{
			if (this._worker != null)
			{
				this._worker.CancelAsync();
			}
			if (this._webClient != null)
			{
				this._webClient.CancelAsync();
			}
		}

		private void Download_Completed(object sender, AsyncCompletedEventArgs e)
		{
			Download userState = (Download)e.UserState;
			bool error = e.Error != null;
			if (e.Cancelled || error)
			{
				try
				{
					FileEx.Delete(userState.Destination);
				}
				catch
				{
				}
				if (e.Cancelled)
				{
					this.OnDownloadCompleted(sender, new DownloadCompletedArgs(true));
					return;
				}
				if (error)
				{
					this.OnDownloadCompleted(sender, new DownloadCompletedArgs(false, e.Error));
				}
				return;
			}
			this._downloadQueue.RemoveAt(0);
			if (this._downloadQueue.All<Download>((Download d) => d.VoicePack != userState.VoicePack))
			{
				this.PacksQueue.RemoveAll<VoicePack>((VoicePack d) => d == userState.VoicePack);
			}
			string filePathWithoutExtension = PathEx.GetFilePathWithoutExtension(userState.Destination);
			FileEx.Move(userState.Destination, filePathWithoutExtension, true);
			if (!this._downloadQueue.Any<Download>())
			{
				this.OnDownloadCompleted(sender, new DownloadCompletedArgs(false));
				return;
			}
			PacksDownloader fileSize = this;
			fileSize._currentBytes = fileSize._currentBytes + userState.FileSize;
			this.StartNext();
		}

		private void Download_ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			int num = (this._totalBytes == (long)0 ? 0 : (int)((long)100 * (this._currentBytes + e.BytesReceived) / this._totalBytes));
			DownloadProgressChangedArgs downloadProgressChangedArg = new DownloadProgressChangedArgs()
			{
				CurrentDownload = this._current
			};
			double bytesReceived = (double)e.BytesReceived;
			TimeSpan elapsed = this._stopwatch.Elapsed;
			downloadProgressChangedArg.DownloadSpeed = PacksDownloader.FormatDownloadSpeed(bytesReceived / elapsed.TotalSeconds);
			downloadProgressChangedArg.CurrentPercentage = e.ProgressPercentage;
			downloadProgressChangedArg.TotalPercentage = num;
			downloadProgressChangedArg.TotalDownloads = this._downloadCount;
			downloadProgressChangedArg.FileName = this._downloadQueue[0].FileName;
			downloadProgressChangedArg.PackName = this._downloadQueue[0].VoicePack.PackName;
			this.OnProgressChanged(sender, downloadProgressChangedArg);
		}

		public static string FormatDownloadSpeed(double bitsPerSecond)
		{
			short i;
			string[] empty = new string[] { string.Empty, "K", "M", "G", "T", "P", "E", "Z", "Y" };
			string[] strArrays = empty;
			for (i = 0; bitsPerSecond > 1024 && i < (int)strArrays.Length; i = (short)(i + 1))
			{
				bitsPerSecond = bitsPerSecond / 1024;
			}
			return string.Format("{0:F2} {1}B/s", bitsPerSecond, strArrays[i]);
		}

		private static IEnumerable<Download> GetDownloadList(VoicePack pack)
		{
			string patchPathFromUrl = PacksDownloader.GetPatchPathFromUrl(pack.BaseUrl);
			PatchInfoReader patchInfoReader = PatchInfoReader.Download(Path.Combine(patchPathFromUrl, "patchinfo.xml"));
			foreach (string kom in pack.Koms)
			{
				IEnumerable<PatchInfoEntry> entries = patchInfoReader.Entries;
				PatchInfoEntry patchInfoEntry = entries.FirstOrDefault<PatchInfoEntry>((PatchInfoEntry e) => e.FileName.Equals(kom, StringComparison.OrdinalIgnoreCase));
				if (patchInfoEntry == null)
				{
					continue;
				}
				string str = Path.Combine(Paths.Main.Packs, pack.PackName, kom);
				if (File.Exists(str) && patchInfoEntry.Equals(str))
				{
					continue;
				}
				str = string.Concat(str, ".tmp");
				Uri uri = new Uri(Path.Combine(patchPathFromUrl, "data", kom));
				yield return new Download(uri, patchInfoEntry.Size, str, pack);
			}
		}

		private static string GetPatchPathFromUrl(string url)
		{
			if (!url.EndsWith("PatchPath.dat"))
			{
				return url;
			}
			MyWebClient myWebClient = new MyWebClient();
			Retry.Do(() => url = myWebClient.DownloadString(url), TimeSpan.FromSeconds(3), new int?(3));
			string str = url;
			string[] strArrays = new string[] { " ", "<", ">" };
			return str.Replace(strArrays, "");
		}

		public static bool HasUpdates(VoicePack pack)
		{
			return PacksDownloader.GetDownloadList(pack).Any<Download>();
		}

		public virtual void OnDownloadCompleted(object sender, DownloadCompletedArgs e)
		{
			this.IsBusy = false;
			if (this.DownloadCompleted != null)
			{
				this.DownloadCompleted(sender, e);
			}
		}

		protected virtual void OnDownloadStarted(object sender, EventArgs e)
		{
			this.IsBusy = true;
			if (this.DownloadStarted != null)
			{
				this.DownloadStarted(sender, e);
			}
		}

		public virtual void OnProgressChanged(object sender, DownloadProgressChangedArgs e)
		{
			if (this.ProgressChanged != null)
			{
				this.ProgressChanged(sender, e);
			}
		}

		public void RemoveFromQueue(VoicePack pack)
		{
			this._downloadQueue.RemoveAll((Download d) => d.VoicePack == pack);
			this.PacksQueue.Remove(pack);
		}

		public void RemoveFromQueue(IEnumerable<VoicePack> packs)
		{
			foreach (VoicePack list in packs.ToList<VoicePack>())
			{
				this.RemoveFromQueue(list);
			}
		}

		public void StartDownload()
		{
			this.OnDownloadStarted(this, new EventArgs());
			this._worker.RunWorkerAsync();
		}

		private void StartNext()
		{
			Download item = this._downloadQueue[0];
			if (!Directory.Exists(item.DestinationFolder))
			{
				Directory.CreateDirectory(item.DestinationFolder);
			}
			PathEx.Delete(item.Destination);
			PacksDownloader packsDownloader = this;
			packsDownloader._current = packsDownloader._current + 1;
			this._webClient.DownloadFileAsync(item.FileUrl, item.Destination, item);
			this._stopwatch.Reset();
			this._stopwatch.Start();
		}

		public event DownloadCompletedEventHandler DownloadCompleted;

		public event DownloadStartedEventHandler DownloadStarted;

		public event gPatcher.Components.Downloading.DownloadProgressChangedEventHandler ProgressChanged;
	}
}