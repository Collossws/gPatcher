using Ginpo.Controls;
using Ginpo.Extensions;
using gPatcher;
using gPatcher.Components.Patching;
using gPatcher.Components.UpdateChecking;
using gPatcher.Controls.General;
using gPatcher.Forms.Sub;
using gPatcher.Helpers;
using gPatcher.Helpers.GlobalDataHolding;
using gPatcher.Localization;
using gPatcher.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace gPatcher.Forms.Main
{
	public class FrHome : AutoBgForm
	{
		private IContainer components;

		private Button btnLaunch;

		private ListBox listBox;

		private System.Windows.Forms.ContextMenuStrip cmsTray;

		private ToolStripMenuItem tsmiPatchPlay;

		private ToolStripMenuItem tsmiRestore;

		private ToolStripMenuItem settingsToolStripMenuItem;

		private ToolStripSeparator tsmiSeparator;

		private ToolStripMenuItem tsmiExit2;

		private StatusStrip statusStrip;

		private ToolStripProgressBar progressBar;

		private ToolStripStatusLabel statusLabel;

		private MenuStrip menuStrip;

		private ToolStripMenuItem tsmiFile;

		private ToolStripMenuItem tsmiExit;

		private ToolStripMenuItem tsmiSettings;

		private ToolStripMenuItem tsmiAbout;

		private Button btnManageMods;

		private ImageList imageList;

		private ToolStripMenuItem tsmiActions;

		private ToolStripMenuItem tsmiClearFileCache;

		private BackgroundFilePatcher bgwPatcher;

		private Label lblActiveMods;

		private ToggleButton chkModsEnabled;

		private ToolStripMenuItem tsmiRestart;

		private ToolStripMenuItem tsmiClearAllSettings;

		private ToolStripSeparator toolStripSeparator2;

		private ToolStripMenuItem tsmiDeleteAllMods;

		private ToolStripMenuItem tsmiOpenLauncher;

		private ToolStripSeparator toolStripSeparator4;

		private ToolStripMenuItem tsmiCheckForUpdates;

		private Updater updater;

		private Button btnDownload;

		private NotifyIcon trayIcon;

		private ToolStripMenuItem tsmiCancel;

		private readonly DownloaderForm _downloaderForm = new DownloaderForm()
		{
			ShouldDispose = false
		};

		private readonly FrModsManager _managerForm = new FrModsManager()
		{
			ShouldDispose = false
		};

		private readonly Random _random = new Random();

		private readonly Settings _settings = Settings.Default;

		private readonly FrSettings _settingsForm = new FrSettings();

		private FrHome.States _state;

		private FrHome.States State
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
					case FrHome.States.OpenLauncher:
					{
						this._settingsForm.CanClose = true;
						this._settingsForm.CanSetElswordFolder = true;
						this.btnLaunch.Enabled = true;
						this.btnLaunch.Text = gPatcher.Localization.Text.Window_MainForm_btn_Launch_Launcher;
						this.chkModsEnabled.Enabled = true;
						this.tsmiCancel.Visible = false;
						this.tsmiClearAllSettings.Enabled = true;
						this.tsmiClearFileCache.Enabled = true;
						this.tsmiDeleteAllMods.Enabled = true;
						this.tsmiOpenLauncher.Enabled = true;
						this.tsmiOpenLauncher.Visible = true;
						this.tsmiPatchPlay.Enabled = true;
						this.tsmiPatchPlay.Visible = true;
						this.tsmiSeparator.Visible = true;
						break;
					}
					case FrHome.States.Patch:
					{
						this._settingsForm.CanClose = true;
						this._settingsForm.CanSetElswordFolder = true;
						this.btnLaunch.Enabled = true;
						this.btnLaunch.Text = gPatcher.Localization.Text.Window_MainForm_btn_Launch_Default;
						this.chkModsEnabled.Enabled = true;
						this.tsmiCancel.Visible = false;
						this.tsmiClearAllSettings.Enabled = true;
						this.tsmiClearFileCache.Enabled = true;
						this.tsmiDeleteAllMods.Enabled = true;
						this.tsmiOpenLauncher.Enabled = true;
						this.tsmiOpenLauncher.Visible = true;
						this.tsmiPatchPlay.Enabled = true;
						this.tsmiPatchPlay.Visible = true;
						this.tsmiSeparator.Visible = true;
						break;
					}
					case FrHome.States.Cancel:
					{
						this._settingsForm.CanClose = true;
						this._settingsForm.CanSetElswordFolder = false;
						this.btnLaunch.Enabled = true;
						this.btnLaunch.Image = null;
						this.btnLaunch.Text = gPatcher.Localization.Text.Window_MainForm_btn_Launch_Cancel;
						this.chkModsEnabled.Enabled = false;
						this.tsmiCancel.Visible = true;
						this.tsmiClearAllSettings.Enabled = false;
						this.tsmiClearFileCache.Enabled = false;
						this.tsmiDeleteAllMods.Enabled = false;
						this.tsmiOpenLauncher.Enabled = false;
						this.tsmiOpenLauncher.Visible = false;
						this.tsmiPatchPlay.Enabled = false;
						this.tsmiPatchPlay.Visible = false;
						this.tsmiSeparator.Visible = true;
						break;
					}
					case FrHome.States.CancelDisabled:
					{
						this._settingsForm.CanClose = false;
						this._settingsForm.CanSetElswordFolder = false;
						this.btnLaunch.Enabled = false;
						this.btnLaunch.Image = null;
						this.btnLaunch.Text = gPatcher.Localization.Text.Window_MainForm_btn_Launch_Cancel;
						this.chkModsEnabled.Enabled = false;
						this.tsmiCancel.Visible = false;
						this.tsmiClearAllSettings.Enabled = false;
						this.tsmiClearFileCache.Enabled = false;
						this.tsmiDeleteAllMods.Enabled = false;
						this.tsmiOpenLauncher.Enabled = false;
						this.tsmiOpenLauncher.Visible = false;
						this.tsmiPatchPlay.Enabled = false;
						this.tsmiPatchPlay.Visible = false;
						this.tsmiSeparator.Visible = false;
						break;
					}
					case FrHome.States.Disabled:
					{
						this._settingsForm.CanClose = true;
						this._settingsForm.CanSetElswordFolder = true;
						this.btnLaunch.Enabled = false;
						this.chkModsEnabled.Enabled = false;
						this.tsmiCancel.Visible = false;
						this.tsmiClearAllSettings.Enabled = true;
						this.tsmiClearFileCache.Enabled = true;
						this.tsmiDeleteAllMods.Enabled = true;
						this.tsmiOpenLauncher.Enabled = false;
						this.tsmiOpenLauncher.Visible = true;
						this.tsmiPatchPlay.Enabled = false;
						this.tsmiPatchPlay.Visible = true;
						this.tsmiSeparator.Visible = true;
						break;
					}
				}
				this._state = value;
				this.NextIcon();
			}
		}

		public FrHome()
		{
			this.InitializeComponent();
			this.listBox.DisplayMember = "Name";
		}

		private void ApplyPatch()
		{
			if (!File.Exists(Paths.Elsword.LauncherExe))
			{
				this.ShowElswordFolderSettings();
				return;
			}
			this.State = FrHome.States.Cancel;
			this.bgwPatcher.RunWorkerAsync();
		}

		private void bgwPatcher_ProgressChanged(object sender, FilePatchingProgressChangedArgs e)
		{
			this.progressBar.Value = e.ProgressPercentage;
			string statusPreparingFiles = null;
			switch (e.CurrentState)
			{
				case BackgroundFilePatcher.States.PreparingFilesStarted:
				{
					statusPreparingFiles = gPatcher.Localization.Text.Status_PreparingFiles;
					if (statusPreparingFiles == null)
					{
						return;
					}
					this.statusLabel.Text = statusPreparingFiles.Replace("\r\n", " ");
					this.trayIcon.ShowBalloonTip(3000, null, statusPreparingFiles, ToolTipIcon.None);
					return;
				}
				case BackgroundFilePatcher.States.PreparingFiles:
				case BackgroundFilePatcher.States.DoingBackup:
				case BackgroundFilePatcher.States.Patching:
				case BackgroundFilePatcher.States.PatchingDone:
				case BackgroundFilePatcher.States.RestoringBackup:
				{
					if (statusPreparingFiles == null)
					{
						return;
					}
					this.statusLabel.Text = statusPreparingFiles.Replace("\r\n", " ");
					this.trayIcon.ShowBalloonTip(3000, null, statusPreparingFiles, ToolTipIcon.None);
					return;
				}
				case BackgroundFilePatcher.States.WaitingForX2ToOpen:
				{
					statusPreparingFiles = gPatcher.Localization.Text.Status_WaitingForElswordClient;
					if (statusPreparingFiles == null)
					{
						return;
					}
					this.statusLabel.Text = statusPreparingFiles.Replace("\r\n", " ");
					this.trayIcon.ShowBalloonTip(3000, null, statusPreparingFiles, ToolTipIcon.None);
					return;
				}
				case BackgroundFilePatcher.States.BackupStarted:
				{
					statusPreparingFiles = gPatcher.Localization.Text.Status_CreatingBackup;
					this.State = FrHome.States.CancelDisabled;
					if (statusPreparingFiles == null)
					{
						return;
					}
					this.statusLabel.Text = statusPreparingFiles.Replace("\r\n", " ");
					this.trayIcon.ShowBalloonTip(3000, null, statusPreparingFiles, ToolTipIcon.None);
					return;
				}
				case BackgroundFilePatcher.States.PatchingStarted:
				{
					statusPreparingFiles = gPatcher.Localization.Text.Status_ModifyingFiles;
					if (statusPreparingFiles == null)
					{
						return;
					}
					this.statusLabel.Text = statusPreparingFiles.Replace("\r\n", " ");
					this.trayIcon.ShowBalloonTip(3000, null, statusPreparingFiles, ToolTipIcon.None);
					return;
				}
				case BackgroundFilePatcher.States.WaitingForX2ToClose:
				{
					statusPreparingFiles = gPatcher.Localization.Text.Status_ModifyingSuccessful;
					if (!this._settings.TrayIconEnabled)
					{
						base.WindowState = FormWindowState.Minimized;
						if (statusPreparingFiles == null)
						{
							return;
						}
						this.statusLabel.Text = statusPreparingFiles.Replace("\r\n", " ");
						this.trayIcon.ShowBalloonTip(3000, null, statusPreparingFiles, ToolTipIcon.None);
						return;
					}
					else
					{
						this.ShowTrayIcon();
						if (statusPreparingFiles == null)
						{
							return;
						}
						this.statusLabel.Text = statusPreparingFiles.Replace("\r\n", " ");
						this.trayIcon.ShowBalloonTip(3000, null, statusPreparingFiles, ToolTipIcon.None);
						return;
					}
				}
				case BackgroundFilePatcher.States.RestoringBackupStarted:
				{
					statusPreparingFiles = gPatcher.Localization.Text.Status_RestoringBackup;
					if (statusPreparingFiles == null)
					{
						return;
					}
					this.statusLabel.Text = statusPreparingFiles.Replace("\r\n", " ");
					this.trayIcon.ShowBalloonTip(3000, null, statusPreparingFiles, ToolTipIcon.None);
					return;
				}
				case BackgroundFilePatcher.States.Done:
				{
					statusPreparingFiles = gPatcher.Localization.Text.Status_BackupRestored;
					if (statusPreparingFiles == null)
					{
						return;
					}
					this.statusLabel.Text = statusPreparingFiles.Replace("\r\n", " ");
					this.trayIcon.ShowBalloonTip(3000, null, statusPreparingFiles, ToolTipIcon.None);
					return;
				}
				default:
				{
					if (statusPreparingFiles == null)
					{
						return;
					}
					this.statusLabel.Text = statusPreparingFiles.Replace("\r\n", " ");
					this.trayIcon.ShowBalloonTip(3000, null, statusPreparingFiles, ToolTipIcon.None);
					return;
				}
			}
		}

		private void bgwPatcher_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				MsgBox.Error(gPatcher.Localization.Text.Error_ModifyingFiles, e.Error);
			}
			this.statusLabel.Text = gPatcher.Localization.Text.Status_Idle;
			this.UpdateState();
		}

		private void btn_Launch_Click(object sender, EventArgs e)
		{
			switch (this.State)
			{
				case FrHome.States.OpenLauncher:
				{
					this.RunElswordLauncher();
					return;
				}
				case FrHome.States.Patch:
				{
					this.ApplyPatch();
					return;
				}
				case FrHome.States.Cancel:
				{
					this.tsmiCancel_Click(sender, e);
					return;
				}
				default:
				{
					return;
				}
			}
		}

		private void btn_ManageMods_Click(object sender, EventArgs e)
		{
			this._managerForm.Restore();
		}

		private void btnDownload_Click(object sender, EventArgs e)
		{
			this._downloaderForm.Restore();
		}

		private bool CanClose()
		{
			if (this.State != FrHome.States.CancelDisabled)
			{
				return true;
			}
			MsgBox.Information(gPatcher.Localization.Text.Program_CannotClose);
			return false;
		}

		private void chk_ModsEnabled_CheckedChanged(object sender, EventArgs e)
		{
			this._settings.ModsEnabled = this.chkModsEnabled.Checked;
			this.UpdateState();
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
			this.components = new System.ComponentModel.Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FrHome));
			this.settingsToolStripMenuItem = new ToolStripMenuItem();
			this.cmsTray = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.tsmiCancel = new ToolStripMenuItem();
			this.tsmiOpenLauncher = new ToolStripMenuItem();
			this.tsmiPatchPlay = new ToolStripMenuItem();
			this.tsmiSeparator = new ToolStripSeparator();
			this.tsmiRestore = new ToolStripMenuItem();
			this.tsmiExit2 = new ToolStripMenuItem();
			this.imageList = new ImageList(this.components);
			this.btnDownload = new Button();
			this.chkModsEnabled = new ToggleButton();
			this.btnManageMods = new Button();
			this.statusStrip = new StatusStrip();
			this.progressBar = new ToolStripProgressBar();
			this.statusLabel = new ToolStripStatusLabel();
			this.menuStrip = new MenuStrip();
			this.tsmiFile = new ToolStripMenuItem();
			this.tsmiRestart = new ToolStripMenuItem();
			this.tsmiExit = new ToolStripMenuItem();
			this.tsmiActions = new ToolStripMenuItem();
			this.tsmiClearFileCache = new ToolStripMenuItem();
			this.tsmiClearAllSettings = new ToolStripMenuItem();
			this.toolStripSeparator2 = new ToolStripSeparator();
			this.tsmiDeleteAllMods = new ToolStripMenuItem();
			this.toolStripSeparator4 = new ToolStripSeparator();
			this.tsmiCheckForUpdates = new ToolStripMenuItem();
			this.tsmiSettings = new ToolStripMenuItem();
			this.tsmiAbout = new ToolStripMenuItem();
			this.lblActiveMods = new Label();
			this.btnLaunch = new Button();
			this.listBox = new ListBox();
			this.bgwPatcher = new BackgroundFilePatcher();
			this.updater = new Updater();
			this.trayIcon = new NotifyIcon(this.components);
			this.cmsTray.SuspendLayout();
			this.statusStrip.SuspendLayout();
			this.menuStrip.SuspendLayout();
			base.SuspendLayout();
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			componentResourceManager.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
			ToolStripItemCollection items = this.cmsTray.Items;
			ToolStripItem[] toolStripItemArray = new ToolStripItem[] { this.tsmiCancel, this.tsmiOpenLauncher, this.tsmiPatchPlay, this.tsmiSeparator, this.tsmiRestore, this.tsmiExit2 };
			items.AddRange(toolStripItemArray);
			this.cmsTray.Name = "trayContext";
			componentResourceManager.ApplyResources(this.cmsTray, "cmsTray");
			this.tsmiCancel.BackColor = Color.FromArgb(255, 238, 238);
			this.tsmiCancel.Name = "tsmiCancel";
			componentResourceManager.ApplyResources(this.tsmiCancel, "tsmiCancel");
			this.tsmiCancel.Click += new EventHandler(this.tsmiCancel_Click);
			this.tsmiOpenLauncher.BackColor = Color.FromArgb(255, 238, 238);
			this.tsmiOpenLauncher.Name = "tsmiOpenLauncher";
			componentResourceManager.ApplyResources(this.tsmiOpenLauncher, "tsmiOpenLauncher");
			this.tsmiOpenLauncher.Click += new EventHandler(this.tsmi_AbrirLauncher_Click);
			this.tsmiPatchPlay.BackColor = Color.FromArgb(238, 255, 255);
			this.tsmiPatchPlay.Name = "tsmiPatchPlay";
			componentResourceManager.ApplyResources(this.tsmiPatchPlay, "tsmiPatchPlay");
			this.tsmiPatchPlay.Click += new EventHandler(this.tsmi_AbrirJogo_Click);
			this.tsmiSeparator.Name = "tsmiSeparator";
			componentResourceManager.ApplyResources(this.tsmiSeparator, "tsmiSeparator");
			this.tsmiRestore.Name = "tsmiRestore";
			componentResourceManager.ApplyResources(this.tsmiRestore, "tsmiRestore");
			this.tsmiRestore.Click += new EventHandler(this.tsmi_Restore_Click);
			this.tsmiExit2.Name = "tsmiExit2";
			componentResourceManager.ApplyResources(this.tsmiExit2, "tsmiExit2");
			this.tsmiExit2.Click += new EventHandler(this.tsmi_Exit_Click);
			this.imageList.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("imageList.ImageStream");
			this.imageList.TransparentColor = Color.Transparent;
			this.imageList.Images.SetKeyName(0, "AANEW.bmp");
			this.imageList.Images.SetKeyName(1, "AddNEW.bmp");
			this.imageList.Images.SetKeyName(2, "AishaNEW.bmp");
			this.imageList.Images.SetKeyName(3, "AraNEW.bmp");
			this.imageList.Images.SetKeyName(4, "ATNEW.bmp");
			this.imageList.Images.SetKeyName(5, "BaMNEW.bmp");
			this.imageList.Images.SetKeyName(6, "BHNEW.bmp");
			this.imageList.Images.SetKeyName(7, "BMNEW.bmp");
			this.imageList.Images.SetKeyName(8, "CANEW.bmp");
			this.imageList.Images.SetKeyName(9, "CAvNEW.bmp");
			this.imageList.Images.SetKeyName(10, "CBSNEW.bmp");
			this.imageList.Images.SetKeyName(11, "CENEW.bmp");
			this.imageList.Images.SetKeyName(12, "ChiNEW.bmp");
			this.imageList.Images.SetKeyName(13, "ChungNEW.bmp");
			this.imageList.Images.SetKeyName(14, "CielNNEW.bmp");
			this.imageList.Images.SetKeyName(15, "CLNEW.bmp");
			this.imageList.Images.SetKeyName(16, "CMNEW.bmp");
			this.imageList.Images.SetKeyName(17, "CNNEW.bmp");
			this.imageList.Images.SetKeyName(18, "CRNEW.bmp");
			this.imageList.Images.SetKeyName(19, "DCNEW.bmp");
			this.imageList.Images.SetKeyName(20, "DENEW.bmp");
			this.imageList.Images.SetKeyName(21, "DKNEW.bmp");
			this.imageList.Images.SetKeyName(22, "DMNEW.bmp");
			this.imageList.Images.SetKeyName(23, "DWNEW.bmp");
			this.imageList.Images.SetKeyName(24, "ElesisNEW.bmp");
			this.imageList.Images.SetKeyName(25, "ElswordNEW.bmp");
			this.imageList.Images.SetKeyName(26, "EMNEW.bmp");
			this.imageList.Images.SetKeyName(27, "EveNEW.bmp");
			this.imageList.Images.SetKeyName(28, "FGNEW.bmp");
			this.imageList.Images.SetKeyName(29, "GANEW.bmp");
			this.imageList.Images.SetKeyName(30, "GMNEW.bmp");
			this.imageList.Images.SetKeyName(31, "HMNEW.bmp");
			this.imageList.Images.SetKeyName(32, "IPNEW.bmp");
			this.imageList.Images.SetKeyName(33, "ISNEW.bmp");
			this.imageList.Images.SetKeyName(34, "LDNEW.bmp");
			this.imageList.Images.SetKeyName(35, "LHNEW.bmp");
			this.imageList.Images.SetKeyName(36, "LKNEW.bmp");
			this.imageList.Images.SetKeyName(37, "LPNEW.bmp");
			this.imageList.Images.SetKeyName(38, "LSNEW.bmp");
			this.imageList.Images.SetKeyName(39, "LuCielNEW.bmp");
			this.imageList.Images.SetKeyName(40, "LuDNEW.bmp");
			this.imageList.Images.SetKeyName(41, "MKNEW.bmp");
			this.imageList.Images.SetKeyName(42, "MMNEW.bmp");
			this.imageList.Images.SetKeyName(43, "NWNEW.bmp");
			this.imageList.Images.SetKeyName(44, "OTNEW.bmp");
			this.imageList.Images.SetKeyName(45, "PKNEW.bmp");
			this.imageList.Images.SetKeyName(46, "PTNEW.bmp");
			this.imageList.Images.SetKeyName(47, "RavenNEW.bmp");
			this.imageList.Images.SetKeyName(48, "RenaNEW.bmp");
			this.imageList.Images.SetKeyName(49, "RFNEW.bmp");
			this.imageList.Images.SetKeyName(50, "RGNEW.bmp");
			this.imageList.Images.SetKeyName(51, "RSNEW.bmp");
			this.imageList.Images.SetKeyName(52, "SbKNEW.bmp");
			this.imageList.Images.SetKeyName(53, "SDNEW.bmp");
			this.imageList.Images.SetKeyName(54, "SGNEW.bmp");
			this.imageList.Images.SetKeyName(55, "ShGNEW.bmp");
			this.imageList.Images.SetKeyName(56, "SKNEW.bmp");
			this.imageList.Images.SetKeyName(57, "SRNEW.bmp");
			this.imageList.Images.SetKeyName(58, "StKNEW.bmp");
			this.imageList.Images.SetKeyName(59, "STNEW.bmp");
			this.imageList.Images.SetKeyName(60, "TiTNEW.bmp");
			this.imageList.Images.SetKeyName(61, "TRNEW.bmp");
			this.imageList.Images.SetKeyName(62, "TTNEW.bmp");
			this.imageList.Images.SetKeyName(63, "VCNEW.bmp");
			this.imageList.Images.SetKeyName(64, "VPNEW.bmp");
			this.imageList.Images.SetKeyName(65, "WSNEW.bmp");
			this.imageList.Images.SetKeyName(66, "WTNEW.bmp");
			this.imageList.Images.SetKeyName(67, "YRNEW.bmp");
			componentResourceManager.ApplyResources(this.btnDownload, "btnDownload");
			this.btnDownload.Image = Images16px.Download;
			this.btnDownload.Name = "btnDownload";
			this.btnDownload.UseVisualStyleBackColor = true;
			this.btnDownload.Click += new EventHandler(this.btnDownload_Click);
			componentResourceManager.ApplyResources(this.chkModsEnabled, "chkModsEnabled");
			this.chkModsEnabled.BackColor = SystemColors.Control;
			this.chkModsEnabled.Checked = true;
			this.chkModsEnabled.CheckState = CheckState.Checked;
			this.chkModsEnabled.Name = "chkModsEnabled";
			this.chkModsEnabled.UseVisualStyleBackColor = false;
			this.chkModsEnabled.CheckedChanged += new EventHandler(this.chk_ModsEnabled_CheckedChanged);
			componentResourceManager.ApplyResources(this.btnManageMods, "btnManageMods");
			this.btnManageMods.Image = Images16px.Mods;
			this.btnManageMods.Name = "btnManageMods";
			this.btnManageMods.UseVisualStyleBackColor = true;
			this.btnManageMods.Click += new EventHandler(this.btn_ManageMods_Click);
			ToolStripItemCollection toolStripItemCollections = this.statusStrip.Items;
			ToolStripItem[] toolStripItemArray1 = new ToolStripItem[] { this.progressBar, this.statusLabel };
			toolStripItemCollections.AddRange(toolStripItemArray1);
			componentResourceManager.ApplyResources(this.statusStrip, "statusStrip");
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.SizingGrip = false;
			this.progressBar.Name = "progressBar";
			componentResourceManager.ApplyResources(this.progressBar, "progressBar");
			this.statusLabel.BorderSides = ToolStripStatusLabelBorderSides.Left;
			this.statusLabel.Margin = new System.Windows.Forms.Padding(3, 3, 0, 2);
			this.statusLabel.Name = "statusLabel";
			componentResourceManager.ApplyResources(this.statusLabel, "statusLabel");
			this.menuStrip.BackColor = Color.Transparent;
			ToolStripItemCollection items1 = this.menuStrip.Items;
			ToolStripItem[] toolStripItemArray2 = new ToolStripItem[] { this.tsmiFile, this.tsmiActions, this.tsmiSettings, this.tsmiAbout };
			items1.AddRange(toolStripItemArray2);
			componentResourceManager.ApplyResources(this.menuStrip, "menuStrip");
			this.menuStrip.Name = "menuStrip";
			ToolStripItemCollection dropDownItems = this.tsmiFile.DropDownItems;
			ToolStripItem[] toolStripItemArray3 = new ToolStripItem[] { this.tsmiRestart, this.tsmiExit };
			dropDownItems.AddRange(toolStripItemArray3);
			this.tsmiFile.Name = "tsmiFile";
			componentResourceManager.ApplyResources(this.tsmiFile, "tsmiFile");
			this.tsmiRestart.Name = "tsmiRestart";
			componentResourceManager.ApplyResources(this.tsmiRestart, "tsmiRestart");
			this.tsmiRestart.Click += new EventHandler(this.tsmi_Restart_Click);
			this.tsmiExit.Name = "tsmiExit";
			componentResourceManager.ApplyResources(this.tsmiExit, "tsmiExit");
			this.tsmiExit.Click += new EventHandler(this.tsmi_Exit_Click);
			ToolStripItemCollection dropDownItems1 = this.tsmiActions.DropDownItems;
			ToolStripItem[] toolStripItemArray4 = new ToolStripItem[] { this.tsmiClearFileCache, this.tsmiClearAllSettings, this.toolStripSeparator2, this.tsmiDeleteAllMods, this.toolStripSeparator4, this.tsmiCheckForUpdates };
			dropDownItems1.AddRange(toolStripItemArray4);
			this.tsmiActions.Name = "tsmiActions";
			componentResourceManager.ApplyResources(this.tsmiActions, "tsmiActions");
			this.tsmiClearFileCache.Image = Images16px.Clear;
			this.tsmiClearFileCache.Name = "tsmiClearFileCache";
			componentResourceManager.ApplyResources(this.tsmiClearFileCache, "tsmiClearFileCache");
			this.tsmiClearFileCache.Click += new EventHandler(this.tsmi_ClearFileCache_Click);
			this.tsmiClearAllSettings.Name = "tsmiClearAllSettings";
			componentResourceManager.ApplyResources(this.tsmiClearAllSettings, "tsmiClearAllSettings");
			this.tsmiClearAllSettings.Click += new EventHandler(this.tsmi_ClearAllSettings_Click);
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			componentResourceManager.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
			this.tsmiDeleteAllMods.Image = Images16px.Cross;
			this.tsmiDeleteAllMods.Name = "tsmiDeleteAllMods";
			componentResourceManager.ApplyResources(this.tsmiDeleteAllMods, "tsmiDeleteAllMods");
			this.tsmiDeleteAllMods.Click += new EventHandler(this.tsmi_DeleteAllMods_Click);
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			componentResourceManager.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
			this.tsmiCheckForUpdates.Image = Images16px.Update;
			this.tsmiCheckForUpdates.Name = "tsmiCheckForUpdates";
			componentResourceManager.ApplyResources(this.tsmiCheckForUpdates, "tsmiCheckForUpdates");
			this.tsmiCheckForUpdates.Click += new EventHandler(this.tsmi_CheckForUpdates_Click);
			this.tsmiSettings.CheckOnClick = true;
			this.tsmiSettings.Name = "tsmiSettings";
			componentResourceManager.ApplyResources(this.tsmiSettings, "tsmiSettings");
			this.tsmiSettings.Click += new EventHandler(this.tsmi_Settings_Click);
			this.tsmiAbout.Name = "tsmiAbout";
			componentResourceManager.ApplyResources(this.tsmiAbout, "tsmiAbout");
			this.tsmiAbout.Click += new EventHandler(this.tsmi_About_Click);
			componentResourceManager.ApplyResources(this.lblActiveMods, "lblActiveMods");
			this.lblActiveMods.BackColor = Color.Transparent;
			this.lblActiveMods.Name = "lblActiveMods";
			componentResourceManager.ApplyResources(this.btnLaunch, "btnLaunch");
			this.btnLaunch.Name = "btnLaunch";
			this.btnLaunch.UseVisualStyleBackColor = true;
			this.btnLaunch.Click += new EventHandler(this.btn_Launch_Click);
			componentResourceManager.ApplyResources(this.listBox, "listBox");
			this.listBox.Name = "listBox";
			this.listBox.SelectionMode = SelectionMode.None;
			this.bgwPatcher.ProgressChanged += new BackgroundFilePatcherProgressChangedEventHandler(this.bgwPatcher_ProgressChanged);
			this.bgwPatcher.RunWorkerCompleted += new BackgroundFilePatcherRunWorkerCompleted(this.bgwPatcher_RunWorkerCompleted);
			this.updater.UpdatesAvailable += new UpdatesAvailable(this.updater_UpdatesAvailable);
			this.trayIcon.ContextMenuStrip = this.cmsTray;
			componentResourceManager.ApplyResources(this.trayIcon, "trayIcon");
			this.trayIcon.MouseDoubleClick += new MouseEventHandler(this.trayIcon_MouseDoubleClick);
			base.AutoBackgroundImage = BackgroundImages.Elsword;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.btnDownload);
			base.Controls.Add(this.chkModsEnabled);
			base.Controls.Add(this.btnManageMods);
			base.Controls.Add(this.statusStrip);
			base.Controls.Add(this.menuStrip);
			base.Controls.Add(this.lblActiveMods);
			base.Controls.Add(this.btnLaunch);
			base.Controls.Add(this.listBox);
			this.DoubleBuffered = true;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.MainMenuStrip = this.menuStrip;
			base.MaximizeBox = false;
			base.Name = "FrHome";
			base.PersistState = true;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			base.FormClosing += new FormClosingEventHandler(this.MainForm_FormClosing);
			base.Load += new EventHandler(this.MainForm_Load);
			base.Shown += new EventHandler(this.MainForm_Shown);
			base.VisibleChanged += new EventHandler(this.MainForm_VisibleChanged);
			base.Resize += new EventHandler(this.MainForm_Resize);
			this.cmsTray.ResumeLayout(false);
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.State != FrHome.States.CancelDisabled)
			{
				return;
			}
			e.Cancel = true;
			MsgBox.Error(gPatcher.Localization.Text.Program_CannotClose);
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			this.PopulateList(sender, null);
			this.updater.Run(false);
			this.UpdateState();
			GlobalData.PresetsManager.CollectionChanged += new NotifyCollectionChangedEventHandler(this.PopulateList);
			Settings.Default.PropertyChanged += new PropertyChangedEventHandler((object o, PropertyChangedEventArgs args) => {
				if (args.PropertyName.IsAny(new object[] { "ElswordDirectory", "SkipLauncher" }))
				{
					this.UpdateState();
				}
			});
		}

		private void MainForm_Resize(object sender, EventArgs e)
		{
			if (base.WindowState == FormWindowState.Minimized && this._settings.TrayIconEnabled)
			{
				this.ShowTrayIcon();
			}
		}

		private void MainForm_Shown(object sender, EventArgs e)
		{
			if (!File.Exists(Paths.Elsword.LauncherExe))
			{
				this.ShowElswordFolderSettings();
				return;
			}
			if (this._settings.StartHidden && this._settings.TrayIconEnabled)
			{
				this.ShowTrayIcon();
			}
		}

		private void MainForm_VisibleChanged(object sender, EventArgs e)
		{
			this.NextIcon();
		}

		private void NextIcon()
		{
			if (this.State == FrHome.States.OpenLauncher || this.State == FrHome.States.Patch)
			{
				this.btnLaunch.Image = this.imageList.Images[this._random.Next(this.imageList.Images.Count - 1)];
			}
		}

		public void PopulateList(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.listBox.DataSource = (
				from p in GlobalData.PresetsManager
				select p.Mod).Distinct<ModPack>().ToBindingList<ModPack>();
			this.UpdateState();
		}

		public void Restore()
		{
			this.Restore();
			this.trayIcon.Visible = false;
		}

		private void RunElswordLauncher()
		{
			if (File.Exists(Paths.Elsword.LauncherExe))
			{
				Paths.Elsword.RunLauncher();
				return;
			}
			this.ShowElswordFolderSettings();
		}

		private void ShowElswordFolderSettings()
		{
			this._settingsForm.ShowInvalidElswordDirWarning = true;
			this._settingsForm.ShowDialog();
		}

		private void ShowTrayIcon()
		{
			base.Hide();
			this.trayIcon.Visible = true;
			if (!this._settings.IsTrayIconFirstTime)
			{
				return;
			}
			this.trayIcon.ShowBalloonTip(5000, gPatcher.Localization.Text.TrayIcon_FirstTimeTitle, gPatcher.Localization.Text.TrayIcon_FirstTimeText, ToolTipIcon.Info);
			this._settings.IsTrayIconFirstTime = false;
		}

		private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				this.Restore();
			}
		}

		private void tsmi_About_Click(object sender, EventArgs e)
		{
			(new AboutDialog()).ShowDialog();
		}

		private void tsmi_AbrirJogo_Click(object sender, EventArgs e)
		{
			this.ApplyPatch();
		}

		private void tsmi_AbrirLauncher_Click(object sender, EventArgs e)
		{
			this.RunElswordLauncher();
		}

		private void tsmi_CheckForUpdates_Click(object sender, EventArgs e)
		{
			this.updater.Run(true);
		}

		private void tsmi_ClearAllSettings_Click(object sender, EventArgs e)
		{
			if (MsgBox.Question(gPatcher.Localization.Text.Confirmation_ClearAllSettings) != System.Windows.Forms.DialogResult.Yes)
			{
				return;
			}
			string str = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gintoki147");
			foreach (string str1 in Directory.EnumerateDirectories(str, "gPatcher*"))
			{
				Directory.Delete(str1, true);
			}
			Program.Restart();
		}

		private void tsmi_ClearFileCache_Click(object sender, EventArgs e)
		{
			if (MsgBox.Question(gPatcher.Localization.Text.Confirmation_ClearFileCache) != System.Windows.Forms.DialogResult.Yes)
			{
				return;
			}
			try
			{
				Directory.Delete(Paths.Main.Cache, true);
			}
			catch (Exception exception)
			{
				MsgBox.Error(gPatcher.Localization.Text.Error_ClearFileCache, exception);
				return;
			}
			MsgBox.Success(gPatcher.Localization.Text.Success_ClearFileCache);
		}

		private void tsmi_DeleteAllMods_Click(object sender, EventArgs e)
		{
			if (MsgBox.Question(gPatcher.Localization.Text.Confirmation_DeleteAllMods) != System.Windows.Forms.DialogResult.Yes)
			{
				return;
			}
			try
			{
				(new DirectoryInfo(Paths.Main.Packs)).Empty();
			}
			catch (Exception exception)
			{
				MsgBox.Error(gPatcher.Localization.Text.Error_DeleteAllMods, exception);
				return;
			}
			MsgBox.Success(gPatcher.Localization.Text.Success_DeleteAllMods);
		}

		private void tsmi_Exit_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void tsmi_Restart_Click(object sender, EventArgs e)
		{
			if (this.CanClose())
			{
				Program.Restart();
			}
		}

		private void tsmi_Restore_Click(object sender, EventArgs e)
		{
			this.Restore();
		}

		private void tsmi_Settings_Click(object sender, EventArgs e)
		{
			this._settingsForm.ShowDialog();
		}

		private void tsmiCancel_Click(object sender, EventArgs e)
		{
			this.bgwPatcher.CancelAsync();
		}

		private void updater_UpdatesAvailable(object sender, UpdateCheckFinishedArgs e)
		{
			if (e.NewVersionUrl != null && MsgBox.Question(gPatcher.Localization.Text.Question_NewVersionAvailable, gPatcher.Localization.Text.Question_Caption_UpdatesAvailable) == System.Windows.Forms.DialogResult.Yes)
			{
				try
				{
					Process.Start(e.NewVersionUrl);
				}
				catch (Exception exception)
				{
					Process.Start("iexplore", e.NewVersionUrl);
				}
			}
			else if (e.OutdatedVoicePacks != null && !this._downloaderForm.IsBusy && MsgBox.Question(gPatcher.Localization.Text.Question_VoiceUpdatesAvailable, gPatcher.Localization.Text.Question_Caption_UpdatesAvailable) == System.Windows.Forms.DialogResult.Yes)
			{
				this._downloaderForm.DownloadPacks(e.OutdatedVoicePacks);
				this.btnDownload_Click(sender, e);
			}
		}

		private void UpdateState()
		{
			if (this.bgwPatcher.IsBusy)
			{
				return;
			}
			bool count = this.listBox.Items.Count > 0;
			if (count && this.chkModsEnabled.Checked)
			{
				this.State = FrHome.States.Patch;
				return;
			}
			if (count && !this.chkModsEnabled.Checked)
			{
				this.State = FrHome.States.OpenLauncher;
				return;
			}
			this.State = FrHome.States.Disabled;
		}

		private enum States
		{
			ProgramStarted,
			OpenLauncher,
			Patch,
			Cancel,
			CancelDisabled,
			Disabled
		}
	}
}