using Ginpo.gPatcher;
using gPatcher.Components.Downloading;
using gPatcher.Components.General;
using gPatcher.Helpers.GlobalDataHolding;
using gPatcher.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;

namespace gPatcher.Components.UpdateChecking
{
	public class Updater : Component
	{
		private readonly BackgroundWorker _bgWorker;

		private bool _ignoreSettings;

		public Updater()
		{
			this._bgWorker = new BackgroundWorker();
			this._bgWorker.DoWork += new DoWorkEventHandler(this.BgWorker_DoWork);
			this._bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.bgWorker_RunWorkerCompleted);
		}

		private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			if (!Settings.Default.CheckForProgramUpdates && !this._ignoreSettings)
			{
				return;
			}
			string newVersionUrl = Updater.GetNewVersionUrl();
			if (newVersionUrl != null)
			{
				e.Result = newVersionUrl;
				return;
			}
			VoicePack[] array = Updater.GetOutdatedPacks().ToArray<VoicePack>();
			if (array.Any<VoicePack>())
			{
				e.Result = array;
			}
		}

		private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				return;
			}
			string result = e.Result as string;
			VoicePack[] voicePackArray = e.Result as VoicePack[];
			if (this.UpdatesAvailable != null)
			{
				this.UpdatesAvailable(sender, new UpdateCheckFinishedArgs(result, voicePackArray));
			}
		}

		private static string GetNewVersionUrl()
		{
			string str;
			using (MyWebClient myWebClient = new MyWebClient())
			{
				myWebClient.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
				string[] strArrays = myWebClient.DownloadString(new Uri(Settings.Default.VersionInfoUrl)).Split(new char[] { '\r' });
				if ((int)strArrays.Length != 2)
				{
					str = null;
				}
				else if (strArrays[0].Last<char>() == 'B' && Settings.Default.IgnoreBetaReleases)
				{
					str = null;
				}
				else if (strArrays[0].Substring(0, strArrays[0].Length - 1).Equals(Application.ProductVersion))
				{
					str = null;
				}
				else if (Uri.IsWellFormedUriString(strArrays[1], UriKind.Absolute))
				{
					str = strArrays[1];
				}
				else
				{
					str = null;
				}
			}
			return str;
		}

		private static IEnumerable<VoicePack> GetOutdatedPacks()
		{
			return GlobalData.VoicePacks.Where<VoicePack>((VoicePack vp) => {
				if (GlobalData.ModPacksManager.FindByName(vp.PackName) == null)
				{
					return false;
				}
				return PacksDownloader.HasUpdates(vp);
			});
		}

		public void Run(bool ignoreSettings = false)
		{
			if (this._bgWorker.IsBusy)
			{
				return;
			}
			this._bgWorker.RunWorkerAsync();
			this._ignoreSettings = ignoreSettings;
		}

		public event gPatcher.Components.UpdateChecking.UpdatesAvailable UpdatesAvailable;
	}
}