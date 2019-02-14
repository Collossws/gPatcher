using Ginpo.Controls;
using Ginpo.Extensions;
using Ginpo.gPatcher;
using gPatcher;
using gPatcher.Components.Downloading;
using gPatcher.Controls.General;
using gPatcher.Helpers;
using gPatcher.Helpers.GlobalDataHolding;
using gPatcher.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace gPatcher.Forms.Main
{
	public class DownloaderForm : AutoBgForm
	{
		private bool _showToolTip = true;

		private DownloaderForm.States _state;

		private IContainer components;

		private ComboBox cbxAddToQueue;

		private Button btnAddToQueue;

		private ListBox listBox;

		private Button btnDownload;

		private System.Windows.Forms.ContextMenuStrip listContextMenu;

		private ToolStripMenuItem tsmiRemoveFromQueue;

		private ToolStripMenuItem tsmiSelectAll;

		private PacksDownloader packsDownloader;

		private ProgressBar totalProgressBar;

		private Label lblMessage;

		private Label lblStatus;

		private GroupBox groupBox;

		private ProgressBar currentProgressBar;

		public bool IsBusy
		{
			get
			{
				return this.packsDownloader.IsBusy;
			}
		}

		private DownloaderForm.States State
		{
			get
			{
				return this._state;
			}
			set
			{
				if (this._state == value)
				{
					return;
				}
				switch (value)
				{
					case DownloaderForm.States.DownloadDisabled:
					{
						this.btnAddToQueue.Enabled = true;
						this.btnDownload.Enabled = false;
						this.btnDownload.Text = gPatcher.Localization.Text.Window_DownloadForm_btn_Download_Download;
						this.btnDownload.Image = Images32px.Download;
						this.cbxAddToQueue.Enabled = true;
						this.listBox.Enabled = true;
						break;
					}
					case DownloaderForm.States.DownloadEnabled:
					{
						this.btnAddToQueue.Enabled = true;
						this.btnDownload.Enabled = true;
						this.btnDownload.Text = gPatcher.Localization.Text.Window_DownloadForm_btn_Download_Download;
						this.btnDownload.Image = Images32px.Download;
						this.cbxAddToQueue.Enabled = true;
						this.currentProgressBar.Value = 0;
						this.lblMessage.Text = string.Empty;
						this.lblStatus.Text = string.Empty;
						this.listBox.Enabled = true;
						this.totalProgressBar.Value = 0;
						break;
					}
					default:
					{
						this.btnAddToQueue.Enabled = false;
						this.btnDownload.Enabled = true;
						this.btnDownload.Image = Images32px.Cancel;
						this.btnDownload.Text = gPatcher.Localization.Text.Window_DownloadForm_btn_Download_Cancel;
						this.cbxAddToQueue.Enabled = false;
						this.listBox.Enabled = false;
						break;
					}
				}
				this._state = value;
			}
		}

		public DownloaderForm()
		{
			this.InitializeComponent();
			this.cbxAddToQueue.DisplayMember = "PackName";
			this.cbxAddToQueue.DataSource = GlobalData.VoicePacks;
			this.listBox.DisplayMember = "PackName";
			this.listBox.DataSource = this.packsDownloader.PacksQueue;
			this.packsDownloader.PacksQueue.ListChanged += new ListChangedEventHandler(this.PacksQueueOnListChanged);
			Application.ApplicationExit += new EventHandler((object param0, EventArgs param1) => this.Cancel());
		}

		private void btn_AddToQueue_Click(object sender, EventArgs e)
		{
			VoicePack selectedItem = (VoicePack)this.cbxAddToQueue.SelectedItem;
			this.packsDownloader.AddToQueue(selectedItem);
			if (this.cbxAddToQueue.SelectedIndex + 1 >= this.cbxAddToQueue.Items.Count)
			{
				this.cbxAddToQueue.SelectedIndex = 0;
				return;
			}
			ComboBox selectedIndex = this.cbxAddToQueue;
			selectedIndex.SelectedIndex = selectedIndex.SelectedIndex + 1;
		}

		private void btn_Download_Click(object sender, EventArgs e)
		{
			if (this.btnDownload.Text == gPatcher.Localization.Text.Window_DownloadForm_btn_Download_Download)
			{
				this.StartDownload();
				return;
			}
			this.Cancel();
		}

		private void Cancel()
		{
			this.packsDownloader.Cancel();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void DownloadForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.Cancel();
		}

		private void DownloadForm_Load(object sender, EventArgs e)
		{
			this.btnDownload.Font = new System.Drawing.Font(this.Font, FontStyle.Bold);
		}

		public void DownloadPacks(IEnumerable<VoicePack> voicePacks)
		{
			foreach (VoicePack voicePack in voicePacks)
			{
				this.packsDownloader.AddToQueue(voicePack);
			}
			this.btn_Download_Click(this, EventArgs.Empty);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DownloaderForm));
			this.listContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.tsmiRemoveFromQueue = new ToolStripMenuItem();
			this.tsmiSelectAll = new ToolStripMenuItem();
			this.btnDownload = new Button();
			this.listBox = new ListBox();
			this.cbxAddToQueue = new ComboBox();
			this.btnAddToQueue = new Button();
			this.packsDownloader = new PacksDownloader();
			this.totalProgressBar = new ProgressBar();
			this.lblMessage = new Label();
			this.lblStatus = new Label();
			this.groupBox = new GroupBox();
			this.currentProgressBar = new ProgressBar();
			this.listContextMenu.SuspendLayout();
			this.groupBox.SuspendLayout();
			base.SuspendLayout();
			ToolStripItemCollection items = this.listContextMenu.Items;
			ToolStripItem[] toolStripItemArray = new ToolStripItem[] { this.tsmiRemoveFromQueue, this.tsmiSelectAll };
			items.AddRange(toolStripItemArray);
			this.listContextMenu.Name = "listContextMenu";
			componentResourceManager.ApplyResources(this.listContextMenu, "listContextMenu");
			this.tsmiRemoveFromQueue.Image = Images16px.Cross;
			this.tsmiRemoveFromQueue.Name = "tsmiRemoveFromQueue";
			componentResourceManager.ApplyResources(this.tsmiRemoveFromQueue, "tsmiRemoveFromQueue");
			this.tsmiRemoveFromQueue.Click += new EventHandler(this.tsmi_RemoveFromQueue_Click);
			this.tsmiSelectAll.Image = Images16px.SelectAll;
			this.tsmiSelectAll.Name = "tsmiSelectAll";
			componentResourceManager.ApplyResources(this.tsmiSelectAll, "tsmiSelectAll");
			this.tsmiSelectAll.Click += new EventHandler(this.tsmi_SelectAll_Click);
			componentResourceManager.ApplyResources(this.btnDownload, "btnDownload");
			this.btnDownload.Image = Images32px.Download;
			this.btnDownload.Name = "btnDownload";
			this.btnDownload.UseVisualStyleBackColor = true;
			this.btnDownload.Click += new EventHandler(this.btn_Download_Click);
			this.listBox.ContextMenuStrip = this.listContextMenu;
			this.listBox.FormattingEnabled = true;
			componentResourceManager.ApplyResources(this.listBox, "listBox");
			this.listBox.Name = "listBox";
			this.listBox.SelectionMode = SelectionMode.MultiExtended;
			this.listBox.KeyDown += new KeyEventHandler(this.listBox_KeyDown);
			this.cbxAddToQueue.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbxAddToQueue.FormattingEnabled = true;
			componentResourceManager.ApplyResources(this.cbxAddToQueue, "cbxAddToQueue");
			this.cbxAddToQueue.Name = "cbxAddToQueue";
			this.btnAddToQueue.Image = Images16px.Add;
			componentResourceManager.ApplyResources(this.btnAddToQueue, "btnAddToQueue");
			this.btnAddToQueue.Name = "btnAddToQueue";
			this.btnAddToQueue.Tag = "";
			this.btnAddToQueue.UseVisualStyleBackColor = true;
			this.btnAddToQueue.Click += new EventHandler(this.btn_AddToQueue_Click);
			this.packsDownloader.ProgressChanged += new DownloadProgressChangedEventHandler(this.packsDownloader_ProgressChanged);
			this.packsDownloader.DownloadCompleted += new DownloadCompletedEventHandler(this.packsDownloader_DownloadCompleted);
			this.packsDownloader.DownloadStarted += new DownloadStartedEventHandler(this.packsDownloader_DownloadStarted);
			componentResourceManager.ApplyResources(this.totalProgressBar, "totalProgressBar");
			this.totalProgressBar.Name = "totalProgressBar";
			componentResourceManager.ApplyResources(this.lblMessage, "lblMessage");
			this.lblMessage.Name = "lblMessage";
			componentResourceManager.ApplyResources(this.lblStatus, "lblStatus");
			this.lblStatus.Name = "lblStatus";
			this.groupBox.BackColor = Color.Transparent;
			this.groupBox.Controls.Add(this.currentProgressBar);
			this.groupBox.Controls.Add(this.lblStatus);
			this.groupBox.Controls.Add(this.lblMessage);
			this.groupBox.Controls.Add(this.totalProgressBar);
			componentResourceManager.ApplyResources(this.groupBox, "groupBox");
			this.groupBox.Name = "groupBox";
			this.groupBox.TabStop = false;
			componentResourceManager.ApplyResources(this.currentProgressBar, "currentProgressBar");
			this.currentProgressBar.Name = "currentProgressBar";
			base.AutoBackgroundImage = BackgroundImages.Chung;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.btnDownload);
			base.Controls.Add(this.listBox);
			base.Controls.Add(this.groupBox);
			base.Controls.Add(this.cbxAddToQueue);
			base.Controls.Add(this.btnAddToQueue);
			this.DoubleBuffered = true;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			base.Name = "DownloaderForm";
			base.ShouldDispose = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			base.FormClosing += new FormClosingEventHandler(this.DownloadForm_FormClosing);
			base.Load += new EventHandler(this.DownloadForm_Load);
			this.listContextMenu.ResumeLayout(false);
			this.groupBox.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		private void listBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				this.tsmiRemoveFromQueue.PerformClick();
				return;
			}
			if (e.Control && e.KeyCode == Keys.A)
			{
				this.tsmiSelectAll.PerformClick();
			}
		}

		private void packsDownloader_DownloadCompleted(object sender, DownloadCompletedArgs e)
		{
			Label label = this.lblMessage;
			Label label1 = this.lblStatus;
			string empty = string.Empty;
			string str = empty;
			label1.Text = empty;
			label.Text = str;
			if (!e.Cancelled && e.Error == null)
			{
				if (MsgBox.Question(gPatcher.Localization.Text.Question_Text_DownloadCompleted, gPatcher.Localization.Text.Question_Caption_DownloadCompleted) == System.Windows.Forms.DialogResult.Yes)
				{
					base.Close();
				}
			}
			else if (e.Error != null)
			{
				MsgBox.Error(gPatcher.Localization.Text.Error_Downloading, e.Error);
			}
			this.UpdateState();
		}

		private void packsDownloader_DownloadStarted(object sender, EventArgs e)
		{
			this.State = DownloaderForm.States.Downloading;
		}

		private void packsDownloader_ProgressChanged(object sender, DownloadProgressChangedArgs e)
		{
			this.currentProgressBar.Value = e.CurrentPercentage;
			this.totalProgressBar.Value = e.TotalPercentage;
			this.lblMessage.Text = string.Format(gPatcher.Localization.Text.Window_DownloadForm_lblMessage_Downloading, e.PackName, e.FileName);
			this.lblStatus.Text = string.Format("{0} | {1}/{2}", e.DownloadSpeed, e.CurrentDownload, e.TotalDownloads);
		}

		private void PacksQueueOnListChanged(object sender, ListChangedEventArgs listChangedEventArgs)
		{
			this.UpdateState();
			if (!this._showToolTip)
			{
				return;
			}
			ToolTip toolTip = new ToolTip()
			{
				IsBalloon = true,
				ToolTipIcon = ToolTipIcon.Info,
				UseAnimation = true,
				UseFading = true
			};
			ToolTip windowDownloadFormHowToRemoveItemsTipTitle = toolTip;
			windowDownloadFormHowToRemoveItemsTipTitle.Show(string.Empty, this.listBox);
			windowDownloadFormHowToRemoveItemsTipTitle.ToolTipTitle = gPatcher.Localization.Text.Window_DownloadForm_HowToRemoveItemsTipTitle;
			windowDownloadFormHowToRemoveItemsTipTitle.Show(gPatcher.Localization.Text.Window_DownloadForm_HowToRemoveItemsTipText, this.listBox, 6000);
			this._showToolTip = false;
		}

		private void StartDownload()
		{
			try
			{
				this.lblMessage.Text = gPatcher.Localization.Text.Window_DownloadForm_lblMessage_Preparing;
				this.packsDownloader.StartDownload();
			}
			catch (Exception exception)
			{
				MsgBox.Error(gPatcher.Localization.Text.Error_Downloading, exception);
				Label label = this.lblMessage;
				Label label1 = this.lblStatus;
				string empty = string.Empty;
				string str = empty;
				label1.Text = empty;
				label.Text = str;
			}
		}

		private void tsmi_RemoveFromQueue_Click(object sender, EventArgs e)
		{
			this.packsDownloader.RemoveFromQueue(this.listBox.SelectedItems.Cast<VoicePack>());
		}

		private void tsmi_SelectAll_Click(object sender, EventArgs e)
		{
			this.listBox.SelectAll();
		}

		private void UpdateState()
		{
			if (this.packsDownloader.IsBusy)
			{
				this.State = DownloaderForm.States.Downloading;
				return;
			}
			this.State = (this.listBox.Items.Count > 0 ? DownloaderForm.States.DownloadEnabled : DownloaderForm.States.DownloadDisabled);
		}

		private enum States
		{
			DownloadDisabled,
			DownloadEnabled,
			Downloading
		}
	}
}