using Ginpo;
using gPatcher;
using gPatcher.Helpers.GlobalDataHolding;
using gPatcher.Localization;
using Ionic.Zip;
using Ionic.Zlib;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace gPatcher.Forms.ProgressDialogs
{
	public class ZipExportDialog : BaseProgressDialog
	{
		private readonly ModPack _modPack;

		private readonly string _zipPath;

		private bool _cancel;

		private IContainer components;

		public ZipExportDialog(ModPack modPack, string zipPath)
		{
			if (modPack.IsDummy)
			{
				throw new Exception("It's not possible to export dummy ModPack objects.");
			}
			this.InitializeComponent();
			this.lblCurrentFile.Text = gPatcher.Localization.Text.Window_ZipSavingDialog_Text;
			this._modPack = modPack;
			this._zipPath = zipPath;
		}

		public override void btn_Cancel_Click(object sender, EventArgs e)
		{
			this._cancel = true;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ZipExportDialog));
			((ISupportInitialize)this.pictureBox).BeginInit();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.pictureBox, "pictureBox");
			this.pictureBox.Image = Images32px.ZIP;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Name = "ZipExportDialog";
			((ISupportInitialize)this.pictureBox).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		protected override void OnDoWork(object sender, DoWorkEventArgs e)
		{
			FileEx.Delete(this._zipPath);
			using (ZipFile zipFiles = new ZipFile(this._zipPath))
			{
				zipFiles.AddFiles(
					from f in this._modPack.Files
					select f.FilePath, string.Empty);
				zipFiles.CompressionLevel = CompressionLevel.BestCompression;
				zipFiles.SaveProgress += new EventHandler<SaveProgressEventArgs>(this.SaveProgress);
				zipFiles.Save(this._zipPath);
			}
		}

		public void SaveProgress(object sender, SaveProgressEventArgs e)
		{
			if (e.EventType != ZipProgressEventType.Saving_BeforeWriteEntry)
			{
				return;
			}
			double entriesSaved = (double)e.EntriesSaved / (double)e.EntriesTotal * 100;
			this.backgroundWorker.ReportProgress((int)entriesSaved, e.CurrentEntry.FileName);
			this.NoChange = false;
			if (this._cancel)
			{
				e.Cancel = this._cancel;
			}
		}
	}
}