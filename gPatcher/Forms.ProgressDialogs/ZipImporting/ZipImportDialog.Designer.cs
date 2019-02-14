using Ginpo;
using gPatcher;
using gPatcher.Forms.ProgressDialogs;
using gPatcher.Helpers;
using gPatcher.Localization;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace gPatcher.Forms.ProgressDialogs.ZipImporting
{
	public class ZipImportDialog : BaseProgressDialog
	{
		private readonly DirectoryInfo _destinationFolder;

		private readonly ZipEntry[] _zipEntries;

		private readonly ZipFile _zipFile;

		private int _currentFile;

		private IContainer components;

		public string Folder
		{
			get
			{
				return this._destinationFolder.Name;
			}
		}

		public ZipImportDialog(string zipPath)
		{
			this.InitializeComponent();
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(zipPath);
			string str = Path.Combine(Paths.Main.Packs, fileNameWithoutExtension);
			this._destinationFolder = new DirectoryInfo(str);
			this.lblCurrentFile.Text = gPatcher.Localization.Text.Window_ZipImportDialog_Text;
			this._zipFile = ZipFile.Read(zipPath);
			this._zipEntries = this._zipFile.SelectEntries(SearchCriterias.ZipCriteria).ToArray<ZipEntry>();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ZipImportDialog));
			((ISupportInitialize)this.pictureBox).BeginInit();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.pictureBox, "pictureBox");
			this.pictureBox.Image = Images32px.ZIP;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Name = "ZipImportDialog";
			((ISupportInitialize)this.pictureBox).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private static void MoveAllFilesToRoot(DirectoryInfo folder)
		{
			foreach (FileInfo fileInfo in folder.EnumerateFiles("*.*", SearchOption.AllDirectories))
			{
				string str = Path.Combine(folder.FullName, fileInfo.Name);
				if (!PathEx.Equals(str, fileInfo.FullName) && File.Exists(str))
				{
					File.Delete(str);
				}
				File.Move(fileInfo.FullName, str);
			}
			foreach (DirectoryInfo directoryInfo in folder.EnumerateDirectories())
			{
				directoryInfo.Delete();
			}
		}

		protected override void OnDoWork(object sender, DoWorkEventArgs e)
		{
			while (this._currentFile < (int)this._zipEntries.Length)
			{
				if (this.backgroundWorker.CancellationPending)
				{
					e.Cancel = true;
					return;
				}
				ZipEntry zipEntry = this._zipEntries[this._currentFile];
				this.backgroundWorker.ReportProgress(this._currentFile * 100 / (int)this._zipEntries.Length, zipEntry.FileName);
				zipEntry.Extract(this._destinationFolder.FullName, ExtractExistingFileAction.OverwriteSilently);
				ZipImportDialog zipImportDialog = this;
				zipImportDialog._currentFile = zipImportDialog._currentFile + 1;
				this.NoChange = false;
			}
			ZipImportDialog.MoveAllFilesToRoot(this._destinationFolder);
		}

		protected override void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error is BadPasswordException)
			{
				this.SetZipPassword();
				return;
			}
			this._zipFile.Dispose();
			base.OnRunWorkerCompleted(sender, e);
		}

		private void SetZipPassword()
		{
			using (BadPasswordDialog badPasswordDialog = new BadPasswordDialog(this._zipEntries[this._currentFile].FileName, Path.GetFileName(this._zipFile.Name)))
			{
				if (badPasswordDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
				{
					base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
				}
				else
				{
					this._zipFile.Password = badPasswordDialog.Password;
					base.Run();
				}
			}
		}
	}
}