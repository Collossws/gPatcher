using Ginpo.Extensions;
using gPatcher;
using gPatcher.Helpers;
using gPatcher.Localization;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace gPatcher.Forms.ProgressDialogs
{
	public class FolderImportDialog : BaseProgressDialog
	{
		private readonly string _destinationFolder;

		private readonly DirectoryInfo _folderToImport;

		private IContainer components;

		public string DirectoryName
		{
			get
			{
				return this._folderToImport.Name;
			}
		}

		private FolderImportDialog()
		{
			this.lblCurrentFile.Text = gPatcher.Localization.Text.Window_FilesImportDialog_Text;
			this.InitializeComponent();
		}

		public FolderImportDialog(string directory) : this()
		{
			this._folderToImport = new DirectoryInfo(directory);
			this._destinationFolder = Path.Combine(Paths.Main.Packs, this._folderToImport.Name);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FolderImportDialog));
			((ISupportInitialize)this.pictureBox).BeginInit();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.pictureBox, "pictureBox");
			this.pictureBox.Image = Images32px.FolderImport;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Name = "FolderImportDialog";
			((ISupportInitialize)this.pictureBox).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		protected override void OnDoWork(object sender, DoWorkEventArgs e)
		{
			FileInfo[] files = this._folderToImport.GetFiles(SearchCriterias.Patterns, SearchOption.AllDirectories);
			Directory.CreateDirectory(this._destinationFolder);
			for (int i = 0; i < (int)files.Length; i++)
			{
				if (this.backgroundWorker.CancellationPending)
				{
					e.Cancel = true;
					return;
				}
				this.backgroundWorker.ReportProgress(i * 100 / (int)files.Length, files[i].Name);
				string str = Path.Combine(this._destinationFolder, files[i].Name);
				File.Copy(files[i].FullName, str, true);
				this.NoChange = false;
			}
		}
	}
}