using Ginpo;
using Ginpo.Extensions;
using gPatcher.Helpers;
using gPatcher.Helpers.GlobalDataHolding;
using gPatcher.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace gPatcher.Components.Patching
{
	public class BackgroundFilePatcher : Component
	{
		private readonly BackgroundWorker _bgwWorker;

		private readonly List<PatchInfo> _fileList = new List<PatchInfo>();

		public bool IsBusy
		{
			get
			{
				return this._bgwWorker.IsBusy;
			}
		}

		public BackgroundFilePatcher()
		{
			BackgroundWorker backgroundWorker = new BackgroundWorker()
			{
				WorkerSupportsCancellation = true,
				WorkerReportsProgress = true
			};
			this._bgwWorker = backgroundWorker;
			this._bgwWorker.DoWork += new DoWorkEventHandler(this.bgwWorker_DoWork);
			this._bgwWorker.ProgressChanged += new ProgressChangedEventHandler(this.bgwWorker_ProgressChanged);
			this._bgwWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.bgwWorker_RunWorkerCompleted);
		}

		private void bgwWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			Process exeProcess;
			this._fileList.Clear();
			this._fileList.AddRange(
				from p in GlobalData.PresetsManager
				select new PatchInfo(p));
			if (this._fileList.Count == 0)
			{
				return;
			}
			this.ReportProgress(BackgroundFilePatcher.States.PreparingFilesStarted, 0);
			for (int i = 0; i < this._fileList.Count; i++)
			{
				if (this._bgwWorker.CancellationPending)
				{
					e.Cancel = true;
					return;
				}
				PatchInfo item = this._fileList[i];
				if (!FileEx.Equals(item.ModdedFile, item.TempFile))
				{
					File.Copy(item.ModdedFile, item.TempFile, true);
				}
				this.ReportProgress(BackgroundFilePatcher.States.PreparingFiles, BackgroundFilePatcher.CalculateProgress(i, this._fileList.Count));
			}
			BackgroundFilePatcher.BlockLogs();
			if (!Settings.Default.SkipLauncher || string.IsNullOrWhiteSpace(Settings.Default.X2Args))
			{
				BackgroundFilePatcher.RunLauncher();
				this.ReportProgress(BackgroundFilePatcher.States.WaitingForX2ToOpen, 0);
				while (!this._bgwWorker.CancellationPending)
				{
					exeProcess = ProcessEx.GetExeProcess(Paths.Elsword.ClientExe);
					Thread.Sleep(1000);
					if (exeProcess != null)
					{
						this.ReportProgress(BackgroundFilePatcher.States.BackupStarted, 0);
						this.CreateBackup();
						this.ReportProgress(BackgroundFilePatcher.States.PatchingStarted, 0);
						this.PatchFiles();
						(new DirectoryInfo(Paths.Main.Cache)).Empty();
						this.ReportProgress(BackgroundFilePatcher.States.WaitingForX2ToClose, 100);
						exeProcess.WaitForExit();
						this.ReportProgress(BackgroundFilePatcher.States.RestoringBackupStarted, 0);
						this.RestoreBackup();
						BackgroundFilePatcher.ClearRegistry("Software\\ElswordINT");
						BackgroundFilePatcher.ClearRegistry("Software\\Nexon\\Elsword\\PatcherOption");
						this.ReportProgress(BackgroundFilePatcher.States.Done, 100);
						return;
					}
				}
				e.Cancel = true;
				return;
			}
			else
			{
				exeProcess = Paths.Elsword.RunClient();
			}
			this.ReportProgress(BackgroundFilePatcher.States.BackupStarted, 0);
			this.CreateBackup();
			this.ReportProgress(BackgroundFilePatcher.States.PatchingStarted, 0);
			this.PatchFiles();
			(new DirectoryInfo(Paths.Main.Cache)).Empty();
			this.ReportProgress(BackgroundFilePatcher.States.WaitingForX2ToClose, 100);
			exeProcess.WaitForExit();
			this.ReportProgress(BackgroundFilePatcher.States.RestoringBackupStarted, 0);
			this.RestoreBackup();
			BackgroundFilePatcher.ClearRegistry("Software\\ElswordINT");
			BackgroundFilePatcher.ClearRegistry("Software\\Nexon\\Elsword\\PatcherOption");
			this.ReportProgress(BackgroundFilePatcher.States.Done, 100);
		}

		private void bgwWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			this.OnProgressChanged(sender, (FilePatchingProgressChangedArgs)e.UserState);
		}

		private void bgwWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				BackgroundFilePatcher.CheckForBackups();
			}
			this.OnRunWorkerCompleted(sender, e);
		}

		private static void BlockLogs()
		{
			if (Settings.Default.BlockLogs)
			{
				Paths.Elsword.BlockLogs();
				return;
			}
			Paths.Elsword.UnblockLogs();
		}

		private static int CalculateProgress(int currentFile, int totalFiles)
		{
			return currentFile * 100 / totalFiles;
		}

		public void CancelAsync()
		{
			this._bgwWorker.CancelAsync();
		}

		public static void CheckForBackups()
		{
			string str;
			string backup = Paths.Elsword.Backup;
			if (string.IsNullOrEmpty(backup) || !Directory.Exists(backup))
			{
				return;
			}
			try
			{
				foreach (string str1 in Directory.EnumerateFiles(backup))
				{
					if (str1.EndsWith(".kom") || str1.EndsWith("general.ess"))
					{
						str = Path.Combine(Paths.Elsword.Data, Path.GetFileName(str1));
						FileEx.Move(str1, str, true);
					}
					else
					{
						if (!str1.EndsWith(".avi"))
						{
							continue;
						}
						str = Path.Combine(Paths.Elsword.Movie, Path.GetFileName(str1));
						FileEx.Move(str1, str, true);
					}
				}
			}
			catch
			{
			}
		}

		public static void ClearRegistry(string keyPath)
		{
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(keyPath, true);
			if (registryKey == null)
			{
				return;
			}
			if (registryKey.Name.EndsWith("\\MARK_INVALID"))
			{
				string[] valueNames = registryKey.GetValueNames();
				for (int i = 0; i < (int)valueNames.Length; i++)
				{
					registryKey.DeleteValue(valueNames[i]);
				}
			}
			string[] subKeyNames = registryKey.GetSubKeyNames();
			for (int j = 0; j < (int)subKeyNames.Length; j++)
			{
				string str = subKeyNames[j];
				BackgroundFilePatcher.ClearRegistry(string.Concat(keyPath, "\\", str));
			}
		}

		private void CreateBackup()
		{
			for (int i = 0; i < this._fileList.Count; i++)
			{
				PatchInfo item = this._fileList[i];
				if (item.BackupFile != null && File.Exists(item.DestinationFile))
				{
					FileEx.Move(item.DestinationFile, item.BackupFile, true);
				}
				this.ReportProgress(BackgroundFilePatcher.States.DoingBackup, BackgroundFilePatcher.CalculateProgress(i, this._fileList.Count));
			}
		}

		public virtual void OnProgressChanged(object sender, FilePatchingProgressChangedArgs e)
		{
			if (this.ProgressChanged != null)
			{
				this.ProgressChanged(sender, e);
			}
		}

		public virtual void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (this.RunWorkerCompleted != null)
			{
				this.RunWorkerCompleted(sender, e);
			}
		}

		private void PatchFiles()
		{
			for (int i = 0; i < this._fileList.Count; i++)
			{
				PatchInfo item = this._fileList[i];
				FileEx.Move(item.TempFile, item.DestinationFile, true);
				this.ReportProgress(BackgroundFilePatcher.States.Patching, BackgroundFilePatcher.CalculateProgress(i, this._fileList.Count));
			}
		}

		private void ReportProgress(BackgroundFilePatcher.States state, int percentage)
		{
			this._bgwWorker.ReportProgress(percentage, new FilePatchingProgressChangedArgs(state, percentage));
		}

		private void RestoreBackup()
		{
			for (int i = 0; i < this._fileList.Count; i++)
			{
				PatchInfo item = this._fileList[i];
				FileEx.Move(item.DestinationFile, item.TempFile, true);
				FileEx.Move(item.BackupFile, item.DestinationFile, true);
				this.ReportProgress(BackgroundFilePatcher.States.RestoringBackup, BackgroundFilePatcher.CalculateProgress(i, this._fileList.Count));
			}
			DirectoryEx.Delete(Paths.Elsword.Media);
			DirectoryEx.Delete(Paths.Elsword.Backup);
		}

		private static void RunLauncher()
		{
			if (!Settings.Default.WebLoginNeeded)
			{
				Paths.Elsword.RunLauncher();
			}
		}

		public void RunWorkerAsync()
		{
			this._bgwWorker.RunWorkerAsync();
		}

		public event BackgroundFilePatcherProgressChangedEventHandler ProgressChanged;

		public event BackgroundFilePatcherRunWorkerCompleted RunWorkerCompleted;

		public enum States
		{
			PreparingFilesStarted,
			PreparingFiles,
			WaitingForX2ToOpen,
			BackupStarted,
			DoingBackup,
			PatchingStarted,
			Patching,
			PatchingDone,
			WaitingForX2ToClose,
			RestoringBackupStarted,
			RestoringBackup,
			Done
		}
	}
}