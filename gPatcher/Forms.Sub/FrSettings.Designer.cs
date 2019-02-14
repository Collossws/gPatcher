using Ginpo;
using Ginpo.Controls;
using gPatcher;
using gPatcher.Controls.General;
using gPatcher.Helpers;
using gPatcher.Helpers.GlobalDataHolding;
using gPatcher.Localization;
using gPatcher.Properties;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Windows.Forms;

namespace gPatcher.Forms.Sub
{
	public class FrSettings : MyForm
	{
		public bool CanClose = true;

		public bool ShowEmptyElswordDirWarning;

		public bool ShowInvalidElswordDirWarning;

		private bool _canSetElswordFolder = true;

		private IContainer components;

		private CheckBox chkEnableBackground;

		private TabPage tabProgram;

		private GroupBox gbxBackground;

		private TabControl tabControl;

		private TableLayoutPanel tlpLanguage;

		private ComboBox cbxLanguage;

		private Label lblLanguage;

		private PictureBox imgBackground;

		private GroupBox gbxTrayIcon;

		private PictureBox imgTrayIcon;

		private CheckBox chkStartMinimized;

		private CheckBox chkTrayIconEnabled;

		private Panel panLanguage;

		private TabPage tabElsword;

		private GroupBox gbxPaths;

		private PictureBox imgPaths;

		private Button btnElswordDir;

		private WaterMarkTextBox tbxElswordDir;

		private GroupBox gbxLaunchOptions;

		private PictureBox imgLaunchOptions;

		private CheckBox chkWebLogin;

		private Label lblWebLogin;

		private GroupBox gbxSecurity;

		private Label lblBlockLogs;

		private CheckBox chkBlockLogs;

		private PictureBox imgSecurity;

		private MenuInferior menuInferior;

		private CheckBox chkSkipLauncher;

		private WaterMarkTextBox tbxX2Args;

		private GroupBox gbxUpdates;

		private PictureBox imgUpdates;

		private CheckBox chkBetaReleases;

		private CheckBox chkProgramUpdates;

		private ToolTip toolTip;

		public bool CanSetElswordFolder
		{
			get
			{
				return this._canSetElswordFolder;
			}
			set
			{
				bool flag = value;
				bool flag1 = flag;
				this.btnElswordDir.Enabled = flag;
				this._canSetElswordFolder = flag1;
			}
		}

		public FrSettings()
		{
			this.InitializeComponent();
			this.cbxLanguage.DataSource = GlobalData.LocaleManager.SupportedCultures;
			this.cbxLanguage.DisplayMember = "NativeName";
			this.cbxLanguage.ValueMember = "Name";
			Ginpo.Events.Add(this.tabControl.Controls, new EventHandler(this.EnableApplyButton));
		}

		private void btn_Apply_Click(object sender, EventArgs e)
		{
			Settings.Default.EnableBackgroundImages = this.chkEnableBackground.Checked;
			Settings.Default.TrayIconEnabled = this.chkTrayIconEnabled.Checked;
			Settings.Default.StartHidden = this.chkStartMinimized.Checked;
			Settings.Default.ElswordDirectory = this.tbxElswordDir.Text;
			Settings.Default.WebLoginNeeded = this.chkWebLogin.Checked;
			Settings.Default.BlockLogs = this.chkBlockLogs.Checked;
			Settings.Default.Culture = (string)this.cbxLanguage.SelectedValue;
			Settings.Default.SkipLauncher = this.chkSkipLauncher.Checked;
			Settings.Default.X2Args = this.tbxX2Args.Text;
			this.UpdateLanguage();
			this.menuInferior.ButtonApplyEnabled = false;
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}

		private void btn_ElswordDir_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog()
			{
				Title = gPatcher.Localization.Text.Action_SelectElswordLauncherTitle,
				Filter = gPatcher.Localization.Text.Window_SettingsForm_SelectElswordLauncherFilter
			};
			OpenFileDialog openFileDialog1 = openFileDialog;
			if (openFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
			{
				return;
			}
			if (!"elsword.exe".Equals(openFileDialog1.SafeFileName, StringComparison.OrdinalIgnoreCase))
			{
				MsgBox.Error(gPatcher.Localization.Text.Error_InvalidLauncher);
				return;
			}
			string directoryName = Path.GetDirectoryName(openFileDialog1.FileName);
			if (!Paths.Elsword.IsValidElswordDir(directoryName))
			{
				MsgBox.Error(gPatcher.Localization.Text.Error_InvalidDirectory);
				return;
			}
			this.tbxElswordDir.Text = directoryName;
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			if (this.menuInferior.ButtonApplyEnabled)
			{
				this.btn_Apply_Click(sender, e);
			}
			base.DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void cbxLanguage_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.cbxLanguage.SelectedIndex == -1)
			{
				this.cbxLanguage.SelectedIndex = 0;
			}
		}

		private void chkProgramUpdates_CheckedChanged(object sender, EventArgs e)
		{
			this.chkBetaReleases.Enabled = this.chkProgramUpdates.Checked;
		}

		private void chkSkipLauncher_CheckedChanged(object sender, EventArgs e)
		{
			this.tbxX2Args.Enabled = this.chkSkipLauncher.Checked;
		}

		private void chkTrayIconEnabled_CheckedChanged(object sender, EventArgs e)
		{
			this.chkStartMinimized.Enabled = this.chkTrayIconEnabled.Checked;
		}

		private void chkWebLogin_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.chkWebLogin.Checked)
			{
				this.chkSkipLauncher.Enabled = true;
				return;
			}
			CheckBox checkBox = this.chkSkipLauncher;
			int num = 0;
			bool flag = (bool)num;
			this.chkSkipLauncher.Checked = (bool)num;
			checkBox.Enabled = flag;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void EnableApplyButton(object obj, EventArgs e)
		{
			this.menuInferior.ButtonApplyEnabled = true;
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FrSettings));
			this.chkEnableBackground = new CheckBox();
			this.tabProgram = new TabPage();
			this.gbxUpdates = new GroupBox();
			this.imgUpdates = new PictureBox();
			this.chkBetaReleases = new CheckBox();
			this.chkProgramUpdates = new CheckBox();
			this.panLanguage = new Panel();
			this.tlpLanguage = new TableLayoutPanel();
			this.cbxLanguage = new ComboBox();
			this.lblLanguage = new Label();
			this.gbxTrayIcon = new GroupBox();
			this.imgTrayIcon = new PictureBox();
			this.chkStartMinimized = new CheckBox();
			this.chkTrayIconEnabled = new CheckBox();
			this.gbxBackground = new GroupBox();
			this.imgBackground = new PictureBox();
			this.tabControl = new TabControl();
			this.tabElsword = new TabPage();
			this.gbxSecurity = new GroupBox();
			this.lblBlockLogs = new Label();
			this.chkBlockLogs = new CheckBox();
			this.imgSecurity = new PictureBox();
			this.gbxLaunchOptions = new GroupBox();
			this.tbxX2Args = new WaterMarkTextBox();
			this.chkSkipLauncher = new CheckBox();
			this.lblWebLogin = new Label();
			this.chkWebLogin = new CheckBox();
			this.imgLaunchOptions = new PictureBox();
			this.gbxPaths = new GroupBox();
			this.imgPaths = new PictureBox();
			this.btnElswordDir = new Button();
			this.tbxElswordDir = new WaterMarkTextBox();
			this.toolTip = new ToolTip(this.components);
			this.menuInferior = new MenuInferior();
			this.tabProgram.SuspendLayout();
			this.gbxUpdates.SuspendLayout();
			((ISupportInitialize)this.imgUpdates).BeginInit();
			this.panLanguage.SuspendLayout();
			this.tlpLanguage.SuspendLayout();
			this.gbxTrayIcon.SuspendLayout();
			((ISupportInitialize)this.imgTrayIcon).BeginInit();
			this.gbxBackground.SuspendLayout();
			((ISupportInitialize)this.imgBackground).BeginInit();
			this.tabControl.SuspendLayout();
			this.tabElsword.SuspendLayout();
			this.gbxSecurity.SuspendLayout();
			((ISupportInitialize)this.imgSecurity).BeginInit();
			this.gbxLaunchOptions.SuspendLayout();
			((ISupportInitialize)this.imgLaunchOptions).BeginInit();
			this.gbxPaths.SuspendLayout();
			((ISupportInitialize)this.imgPaths).BeginInit();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.chkEnableBackground, "chkEnableBackground");
			this.chkEnableBackground.Name = "chkEnableBackground";
			this.toolTip.SetToolTip(this.chkEnableBackground, componentResourceManager.GetString("chkEnableBackground.ToolTip"));
			this.chkEnableBackground.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.tabProgram, "tabProgram");
			this.tabProgram.Controls.Add(this.gbxUpdates);
			this.tabProgram.Controls.Add(this.panLanguage);
			this.tabProgram.Controls.Add(this.gbxTrayIcon);
			this.tabProgram.Controls.Add(this.gbxBackground);
			this.tabProgram.Name = "tabProgram";
			this.toolTip.SetToolTip(this.tabProgram, componentResourceManager.GetString("tabProgram.ToolTip"));
			this.tabProgram.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.gbxUpdates, "gbxUpdates");
			this.gbxUpdates.Controls.Add(this.imgUpdates);
			this.gbxUpdates.Controls.Add(this.chkBetaReleases);
			this.gbxUpdates.Controls.Add(this.chkProgramUpdates);
			this.gbxUpdates.Name = "gbxUpdates";
			this.gbxUpdates.TabStop = false;
			this.toolTip.SetToolTip(this.gbxUpdates, componentResourceManager.GetString("gbxUpdates.ToolTip"));
			componentResourceManager.ApplyResources(this.imgUpdates, "imgUpdates");
			this.imgUpdates.Image = Images32px.Update;
			this.imgUpdates.Name = "imgUpdates";
			this.imgUpdates.TabStop = false;
			this.toolTip.SetToolTip(this.imgUpdates, componentResourceManager.GetString("imgUpdates.ToolTip"));
			componentResourceManager.ApplyResources(this.chkBetaReleases, "chkBetaReleases");
			this.chkBetaReleases.Name = "chkBetaReleases";
			this.toolTip.SetToolTip(this.chkBetaReleases, componentResourceManager.GetString("chkBetaReleases.ToolTip"));
			this.chkBetaReleases.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.chkProgramUpdates, "chkProgramUpdates");
			this.chkProgramUpdates.Name = "chkProgramUpdates";
			this.toolTip.SetToolTip(this.chkProgramUpdates, componentResourceManager.GetString("chkProgramUpdates.ToolTip"));
			this.chkProgramUpdates.UseVisualStyleBackColor = true;
			this.chkProgramUpdates.CheckedChanged += new EventHandler(this.chkProgramUpdates_CheckedChanged);
			componentResourceManager.ApplyResources(this.panLanguage, "panLanguage");
			this.panLanguage.Controls.Add(this.tlpLanguage);
			this.panLanguage.Name = "panLanguage";
			this.toolTip.SetToolTip(this.panLanguage, componentResourceManager.GetString("panLanguage.ToolTip"));
			componentResourceManager.ApplyResources(this.tlpLanguage, "tlpLanguage");
			this.tlpLanguage.Controls.Add(this.cbxLanguage, 1, 0);
			this.tlpLanguage.Controls.Add(this.lblLanguage, 0, 0);
			this.tlpLanguage.Name = "tlpLanguage";
			this.toolTip.SetToolTip(this.tlpLanguage, componentResourceManager.GetString("tlpLanguage.ToolTip"));
			componentResourceManager.ApplyResources(this.cbxLanguage, "cbxLanguage");
			this.cbxLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbxLanguage.FormattingEnabled = true;
			this.cbxLanguage.Name = "cbxLanguage";
			this.toolTip.SetToolTip(this.cbxLanguage, componentResourceManager.GetString("cbxLanguage.ToolTip"));
			this.cbxLanguage.SelectedIndexChanged += new EventHandler(this.cbxLanguage_SelectedIndexChanged);
			componentResourceManager.ApplyResources(this.lblLanguage, "lblLanguage");
			this.lblLanguage.Name = "lblLanguage";
			this.toolTip.SetToolTip(this.lblLanguage, componentResourceManager.GetString("lblLanguage.ToolTip"));
			componentResourceManager.ApplyResources(this.gbxTrayIcon, "gbxTrayIcon");
			this.gbxTrayIcon.Controls.Add(this.imgTrayIcon);
			this.gbxTrayIcon.Controls.Add(this.chkStartMinimized);
			this.gbxTrayIcon.Controls.Add(this.chkTrayIconEnabled);
			this.gbxTrayIcon.Name = "gbxTrayIcon";
			this.gbxTrayIcon.TabStop = false;
			this.toolTip.SetToolTip(this.gbxTrayIcon, componentResourceManager.GetString("gbxTrayIcon.ToolTip"));
			componentResourceManager.ApplyResources(this.imgTrayIcon, "imgTrayIcon");
			this.imgTrayIcon.Image = Images32px.Tray;
			this.imgTrayIcon.Name = "imgTrayIcon";
			this.imgTrayIcon.TabStop = false;
			this.toolTip.SetToolTip(this.imgTrayIcon, componentResourceManager.GetString("imgTrayIcon.ToolTip"));
			componentResourceManager.ApplyResources(this.chkStartMinimized, "chkStartMinimized");
			this.chkStartMinimized.Name = "chkStartMinimized";
			this.toolTip.SetToolTip(this.chkStartMinimized, componentResourceManager.GetString("chkStartMinimized.ToolTip"));
			this.chkStartMinimized.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.chkTrayIconEnabled, "chkTrayIconEnabled");
			this.chkTrayIconEnabled.Name = "chkTrayIconEnabled";
			this.toolTip.SetToolTip(this.chkTrayIconEnabled, componentResourceManager.GetString("chkTrayIconEnabled.ToolTip"));
			this.chkTrayIconEnabled.UseVisualStyleBackColor = true;
			this.chkTrayIconEnabled.CheckedChanged += new EventHandler(this.chkTrayIconEnabled_CheckedChanged);
			componentResourceManager.ApplyResources(this.gbxBackground, "gbxBackground");
			this.gbxBackground.Controls.Add(this.imgBackground);
			this.gbxBackground.Controls.Add(this.chkEnableBackground);
			this.gbxBackground.Name = "gbxBackground";
			this.gbxBackground.TabStop = false;
			this.toolTip.SetToolTip(this.gbxBackground, componentResourceManager.GetString("gbxBackground.ToolTip"));
			componentResourceManager.ApplyResources(this.imgBackground, "imgBackground");
			this.imgBackground.Image = Images32px.Background;
			this.imgBackground.Name = "imgBackground";
			this.imgBackground.TabStop = false;
			this.toolTip.SetToolTip(this.imgBackground, componentResourceManager.GetString("imgBackground.ToolTip"));
			componentResourceManager.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Controls.Add(this.tabElsword);
			this.tabControl.Controls.Add(this.tabProgram);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.toolTip.SetToolTip(this.tabControl, componentResourceManager.GetString("tabControl.ToolTip"));
			componentResourceManager.ApplyResources(this.tabElsword, "tabElsword");
			this.tabElsword.Controls.Add(this.gbxSecurity);
			this.tabElsword.Controls.Add(this.gbxLaunchOptions);
			this.tabElsword.Controls.Add(this.gbxPaths);
			this.tabElsword.Name = "tabElsword";
			this.toolTip.SetToolTip(this.tabElsword, componentResourceManager.GetString("tabElsword.ToolTip"));
			this.tabElsword.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.gbxSecurity, "gbxSecurity");
			this.gbxSecurity.Controls.Add(this.lblBlockLogs);
			this.gbxSecurity.Controls.Add(this.chkBlockLogs);
			this.gbxSecurity.Controls.Add(this.imgSecurity);
			this.gbxSecurity.Name = "gbxSecurity";
			this.gbxSecurity.TabStop = false;
			this.toolTip.SetToolTip(this.gbxSecurity, componentResourceManager.GetString("gbxSecurity.ToolTip"));
			componentResourceManager.ApplyResources(this.lblBlockLogs, "lblBlockLogs");
			this.lblBlockLogs.Name = "lblBlockLogs";
			this.toolTip.SetToolTip(this.lblBlockLogs, componentResourceManager.GetString("lblBlockLogs.ToolTip"));
			componentResourceManager.ApplyResources(this.chkBlockLogs, "chkBlockLogs");
			this.chkBlockLogs.BackColor = Color.Transparent;
			this.chkBlockLogs.Name = "chkBlockLogs";
			this.toolTip.SetToolTip(this.chkBlockLogs, componentResourceManager.GetString("chkBlockLogs.ToolTip"));
			this.chkBlockLogs.UseVisualStyleBackColor = false;
			componentResourceManager.ApplyResources(this.imgSecurity, "imgSecurity");
			this.imgSecurity.Image = Images32px.Security;
			this.imgSecurity.Name = "imgSecurity";
			this.imgSecurity.TabStop = false;
			this.toolTip.SetToolTip(this.imgSecurity, componentResourceManager.GetString("imgSecurity.ToolTip"));
			componentResourceManager.ApplyResources(this.gbxLaunchOptions, "gbxLaunchOptions");
			this.gbxLaunchOptions.Controls.Add(this.tbxX2Args);
			this.gbxLaunchOptions.Controls.Add(this.chkSkipLauncher);
			this.gbxLaunchOptions.Controls.Add(this.lblWebLogin);
			this.gbxLaunchOptions.Controls.Add(this.chkWebLogin);
			this.gbxLaunchOptions.Controls.Add(this.imgLaunchOptions);
			this.gbxLaunchOptions.Name = "gbxLaunchOptions";
			this.gbxLaunchOptions.TabStop = false;
			this.toolTip.SetToolTip(this.gbxLaunchOptions, componentResourceManager.GetString("gbxLaunchOptions.ToolTip"));
			componentResourceManager.ApplyResources(this.tbxX2Args, "tbxX2Args");
			this.tbxX2Args.Name = "tbxX2Args";
			this.toolTip.SetToolTip(this.tbxX2Args, componentResourceManager.GetString("tbxX2Args.ToolTip"));
			this.tbxX2Args.WaterMarkColor = Color.Gray;
			componentResourceManager.ApplyResources(this.chkSkipLauncher, "chkSkipLauncher");
			this.chkSkipLauncher.Name = "chkSkipLauncher";
			this.toolTip.SetToolTip(this.chkSkipLauncher, componentResourceManager.GetString("chkSkipLauncher.ToolTip"));
			this.chkSkipLauncher.UseVisualStyleBackColor = true;
			this.chkSkipLauncher.CheckedChanged += new EventHandler(this.chkSkipLauncher_CheckedChanged);
			componentResourceManager.ApplyResources(this.lblWebLogin, "lblWebLogin");
			this.lblWebLogin.Name = "lblWebLogin";
			this.toolTip.SetToolTip(this.lblWebLogin, componentResourceManager.GetString("lblWebLogin.ToolTip"));
			componentResourceManager.ApplyResources(this.chkWebLogin, "chkWebLogin");
			this.chkWebLogin.BackColor = Color.Transparent;
			this.chkWebLogin.Name = "chkWebLogin";
			this.toolTip.SetToolTip(this.chkWebLogin, componentResourceManager.GetString("chkWebLogin.ToolTip"));
			this.chkWebLogin.UseVisualStyleBackColor = false;
			this.chkWebLogin.CheckedChanged += new EventHandler(this.chkWebLogin_CheckedChanged);
			componentResourceManager.ApplyResources(this.imgLaunchOptions, "imgLaunchOptions");
			this.imgLaunchOptions.Image = Images32px.Cmd;
			this.imgLaunchOptions.Name = "imgLaunchOptions";
			this.imgLaunchOptions.TabStop = false;
			this.toolTip.SetToolTip(this.imgLaunchOptions, componentResourceManager.GetString("imgLaunchOptions.ToolTip"));
			componentResourceManager.ApplyResources(this.gbxPaths, "gbxPaths");
			this.gbxPaths.Controls.Add(this.imgPaths);
			this.gbxPaths.Controls.Add(this.btnElswordDir);
			this.gbxPaths.Controls.Add(this.tbxElswordDir);
			this.gbxPaths.Name = "gbxPaths";
			this.gbxPaths.TabStop = false;
			this.toolTip.SetToolTip(this.gbxPaths, componentResourceManager.GetString("gbxPaths.ToolTip"));
			componentResourceManager.ApplyResources(this.imgPaths, "imgPaths");
			this.imgPaths.Image = Images32px.Elsword;
			this.imgPaths.Name = "imgPaths";
			this.imgPaths.TabStop = false;
			this.toolTip.SetToolTip(this.imgPaths, componentResourceManager.GetString("imgPaths.ToolTip"));
			componentResourceManager.ApplyResources(this.btnElswordDir, "btnElswordDir");
			this.btnElswordDir.Image = Images16px.Search;
			this.btnElswordDir.Name = "btnElswordDir";
			this.toolTip.SetToolTip(this.btnElswordDir, componentResourceManager.GetString("btnElswordDir.ToolTip"));
			this.btnElswordDir.UseVisualStyleBackColor = true;
			this.btnElswordDir.Click += new EventHandler(this.btn_ElswordDir_Click);
			componentResourceManager.ApplyResources(this.tbxElswordDir, "tbxElswordDir");
			this.tbxElswordDir.Name = "tbxElswordDir";
			this.toolTip.SetToolTip(this.tbxElswordDir, componentResourceManager.GetString("tbxElswordDir.ToolTip"));
			this.tbxElswordDir.WaterMarkColor = Color.Gray;
			componentResourceManager.ApplyResources(this.menuInferior, "menuInferior");
			this.menuInferior.Name = "menuInferior";
			this.toolTip.SetToolTip(this.menuInferior, componentResourceManager.GetString("menuInferior.ToolTip"));
			this.menuInferior.ButtonOkClick += new EventHandler(this.btn_OK_Click);
			this.menuInferior.ButtonCancelClick += new EventHandler(this.btn_Cancel_Click);
			this.menuInferior.ButtonApplyClick += new EventHandler(this.btn_Apply_Click);
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.menuInferior);
			base.Controls.Add(this.tabControl);
			this.DoubleBuffered = true;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.KeyPreview = true;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "FrSettings";
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.toolTip.SetToolTip(this, componentResourceManager.GetString("$this.ToolTip"));
			base.Activated += new EventHandler(this.SettingsForm_Activated);
			base.Load += new EventHandler(this.SettingsForm_Load);
			base.VisibleChanged += new EventHandler(this.SettingsForm_VisibleChanged);
			base.KeyDown += new KeyEventHandler(this.SettingsForm_KeyDown);
			this.tabProgram.ResumeLayout(false);
			this.gbxUpdates.ResumeLayout(false);
			this.gbxUpdates.PerformLayout();
			((ISupportInitialize)this.imgUpdates).EndInit();
			this.panLanguage.ResumeLayout(false);
			this.panLanguage.PerformLayout();
			this.tlpLanguage.ResumeLayout(false);
			this.tlpLanguage.PerformLayout();
			this.gbxTrayIcon.ResumeLayout(false);
			this.gbxTrayIcon.PerformLayout();
			((ISupportInitialize)this.imgTrayIcon).EndInit();
			this.gbxBackground.ResumeLayout(false);
			this.gbxBackground.PerformLayout();
			((ISupportInitialize)this.imgBackground).EndInit();
			this.tabControl.ResumeLayout(false);
			this.tabElsword.ResumeLayout(false);
			this.gbxSecurity.ResumeLayout(false);
			this.gbxSecurity.PerformLayout();
			((ISupportInitialize)this.imgSecurity).EndInit();
			this.gbxLaunchOptions.ResumeLayout(false);
			this.gbxLaunchOptions.PerformLayout();
			((ISupportInitialize)this.imgLaunchOptions).EndInit();
			this.gbxPaths.ResumeLayout(false);
			this.gbxPaths.PerformLayout();
			((ISupportInitialize)this.imgPaths).EndInit();
			base.ResumeLayout(false);
		}

		private void SettingsForm_Activated(object sender, EventArgs e)
		{
			if (this.ShowInvalidElswordDirWarning)
			{
				this.ShowInvalidElswordDirWarningToolTip();
				return;
			}
			if (this.ShowEmptyElswordDirWarning)
			{
				this.ShowEmptyElswordDirWarningToolTip();
			}
		}

		private void SettingsForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (!e.Control || e.KeyCode != Keys.Tab)
			{
				return;
			}
			if (this.tabControl.SelectedIndex + 1 != this.tabControl.TabCount)
			{
				TabControl selectedIndex = this.tabControl;
				selectedIndex.SelectedIndex = selectedIndex.SelectedIndex + 1;
			}
			else
			{
				this.tabControl.SelectedIndex = 0;
			}
			e.Handled = true;
		}

		private void SettingsForm_Load(object sender, EventArgs e)
		{
			this.chkEnableBackground.Checked = Settings.Default.EnableBackgroundImages;
			this.chkTrayIconEnabled.Checked = Settings.Default.TrayIconEnabled;
			this.chkStartMinimized.Checked = Settings.Default.StartHidden;
			this.tbxElswordDir.Text = Settings.Default.ElswordDirectory;
			this.chkWebLogin.Checked = Settings.Default.WebLoginNeeded;
			this.chkBlockLogs.Checked = Settings.Default.BlockLogs;
			this.cbxLanguage.SelectedValue = CultureInfo.CurrentCulture.Name;
			this.chkSkipLauncher.Checked = Settings.Default.SkipLauncher;
			this.gbxUpdates.Enabled = !Settings.Default.IsBetaRelease;
			this.chkProgramUpdates.Checked = Settings.Default.CheckForProgramUpdates;
			this.chkBetaReleases.Checked = Settings.Default.IgnoreBetaReleases;
			this.tbxX2Args.Text = Settings.Default.X2Args;
		}

		private void SettingsForm_VisibleChanged(object sender, EventArgs e)
		{
			if (base.Visible)
			{
				this.menuInferior.ButtonApplyEnabled = false;
			}
		}

		private void ShowEmptyElswordDirWarningToolTip()
		{
			this.toolTip.ToolTipIcon = ToolTipIcon.None;
			this.toolTip.Show(string.Empty, this.tbxElswordDir);
			this.toolTip.Show(gPatcher.Localization.Text.Action_SelectElswordLauncherText, this.tbxElswordDir, 10000);
			this.ShowEmptyElswordDirWarning = false;
		}

		private void ShowInvalidElswordDirWarningToolTip()
		{
			this.toolTip.ToolTipIcon = ToolTipIcon.Warning;
			this.toolTip.ToolTipTitle = gPatcher.Localization.Text.Error_InvalidDirectoryTitle;
			this.toolTip.IsBalloon = true;
			this.toolTip.Show(string.Empty, this.tbxElswordDir);
			this.toolTip.Show(gPatcher.Localization.Text.Error_InvalidDirectory, this.tbxElswordDir, 10000);
			this.ShowInvalidElswordDirWarning = false;
		}

		private void UpdateLanguage()
		{
			if (CultureInfo.CurrentCulture.Name.Equals(Settings.Default.Culture))
			{
				return;
			}
			if (!this.CanClose)
			{
				MsgBox.Information(gPatcher.Localization.Text.Warning_RestartToApply);
				return;
			}
			Settings.Default.Save();
			Program.Restart();
		}
	}
}