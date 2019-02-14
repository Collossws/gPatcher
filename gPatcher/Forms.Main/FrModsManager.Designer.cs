using Dotnetrix.Controls;
using Ginpo.Controls;
using Ginpo.Extensions;
using Ginpo.gPatcher;
using gPatcher;
using gPatcher.Controls.General;
using gPatcher.Controls.Modding;
using gPatcher.Forms.ProgressDialogs;
using gPatcher.Forms.ProgressDialogs.ZipImporting;
using gPatcher.Forms.Sub;
using gPatcher.Helpers;
using gPatcher.Helpers.GlobalDataHolding;
using gPatcher.Localization;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace gPatcher.Forms.Main
{
	public class FrModsManager : AutoBgForm
	{
		private readonly ModsSelector[] _modsSelectors;

		private IContainer components;

		private System.Windows.Forms.ContextMenuStrip btnMenuContextMenu;

		private ToolStripMenuItem tsmiEditOngPatcher;

		private ToolStripMenuItem tsmiEditOnExplorer;

		private System.Windows.Forms.ContextMenuStrip selectorContextMenu;

		private ToolStripMenuItem tsmiDisableSelected;

		private ToolStripMenuItem tsmiApplyToSelected;

		private Button btnSetMod;

		private ComboBox cbxSetMod;

		private ToolStripSeparator tsmiSeparator;

		private ToolStripMenuItem tsmiDelete;

		private ImageList tabImageList;

		private ToolTip toolTip;

		private Dotnetrix.Controls.TabControl tabControl;

		private TabPage genTabPage;

		private ModsSelector genSelector;

		private TabPage bgmTabPage;

		private ModsSelector bgmSelector;

		private TabPage vidTabPage;

		private ModsSelector vidSelector;

		private ToolStripMenuItem tsmiNew;

		private ToolStripMenuItem tsmiExport;

		private ToolStripMenuItem tsmiImportZip;

		private ToolStripMenuItem tsmiImportFolder;

		private ToolStripSeparator toolStripSeparator1;

		private ImageTextBox tbxSearch;

		private MenuInferior menuInferior;

		private DropDownButton btnMenu;

		public FrModsManager()
		{
			this.InitializeComponent();
			ModsSelector[] modsSelectorArray = new ModsSelector[] { this.genSelector, this.bgmSelector, this.vidSelector };
			this._modsSelectors = modsSelectorArray;
			this.bgmTabPage.ToolTipText = gPatcher.Localization.Text.Window_ManagerForm_TabPagesToolTips_Bgm;
			this.vidTabPage.ToolTipText = gPatcher.Localization.Text.Window_ManagerForm_TabPagesToolTips_Vid;
			this.cbxSetMod.DataSource = GlobalData.ModPacksManager;
			ComboBox comboBox = this.cbxSetMod;
			string str = "Name";
			string str1 = str;
			this.cbxSetMod.ValueMember = str;
			comboBox.DisplayMember = str1;
			this.CancelChanges();
		}

		private void AskAndSet(string modName)
		{
			if (System.Windows.Forms.DialogResult.Yes != MsgBox.Question(gPatcher.Localization.Text.Question_SetModNow))
			{
				return;
			}
			ModPack modPack = GlobalData.ModPacksManager.FindByName(modName);
			this.cbxSetMod.SelectedItem = modPack;
			this.SetAllSelectors(modPack);
		}

		private void btn_Apply_Click(object sender, EventArgs e)
		{
			this.Save();
			this.menuInferior.ButtonApplyEnabled = false;
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			this.CancelChanges();
			base.Hide();
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			if (this.menuInferior.ButtonApplyEnabled)
			{
				this.btn_Apply_Click(sender, e);
			}
			base.Hide();
		}

		private void btn_SetMod_MouseUp(object sender, MouseEventArgs e)
		{
			if (!this.btnSetMod.Enabled || e.Button != System.Windows.Forms.MouseButtons.Right)
			{
				return;
			}
			this.GetCurrentSelector().DisableModOnAllRows(this.GetSelectedPack().Name);
		}

		private void btnExportToZip_Click(object sender, EventArgs e)
		{
			ModPack selectedPack = this.GetSelectedPack();
			SaveFileDialog saveFileDialog = new SaveFileDialog()
			{
				FileName = string.Concat(selectedPack.Name, ".zip"),
				DefaultExt = "zip",
				RestoreDirectory = true,
				Filter = gPatcher.Localization.Text.Window_ManagerForm_SelectZipFilter,
				Title = gPatcher.Localization.Text.Action_ZipSaving
			};
			using (SaveFileDialog saveFileDialog1 = saveFileDialog)
			{
				if (System.Windows.Forms.DialogResult.OK == saveFileDialog1.ShowDialog())
				{
					(new ZipExportDialog(selectedPack, saveFileDialog1.FileName)).ShowDialog();
				}
			}
		}

		private void btnSetMod_Click(object sender, EventArgs e)
		{
			ModsSelector currentSelector = this.GetCurrentSelector();
			if (this.cbxSetMod.SelectedIndex == 0)
			{
				currentSelector.DisableAll();
				return;
			}
			currentSelector.SetModOnAllRows(this.GetSelectedPack());
		}

		private void CancelChanges()
		{
			ModsSelector[] modsSelectorArray = this._modsSelectors;
			for (int i = 0; i < (int)modsSelectorArray.Length; i++)
			{
				modsSelectorArray[i].CancelChanges();
			}
		}

		private void cbx_SetMod_SelectedIndexChanged(object sender, EventArgs e)
		{
			ToolStripMenuItem toolStripMenuItem = this.tsmiEditOngPatcher;
			ToolStripMenuItem toolStripMenuItem1 = this.tsmiEditOnExplorer;
			ToolStripMenuItem toolStripMenuItem2 = this.tsmiExport;
			ToolStripSeparator toolStripSeparator = this.toolStripSeparator1;
			ToolStripSeparator toolStripSeparator1 = this.tsmiSeparator;
			bool selectedIndex = this.cbxSetMod.SelectedIndex > 0;
			bool flag = selectedIndex;
			this.tsmiDelete.Visible = selectedIndex;
			bool flag1 = flag;
			bool flag2 = flag1;
			toolStripSeparator1.Visible = flag1;
			bool flag3 = flag2;
			bool flag4 = flag3;
			toolStripSeparator.Visible = flag3;
			bool flag5 = flag4;
			bool flag6 = flag5;
			toolStripMenuItem2.Visible = flag5;
			bool flag7 = flag6;
			bool flag8 = flag7;
			toolStripMenuItem1.Visible = flag7;
			toolStripMenuItem.Visible = flag8;
		}

		private void Configuration_BackgroundImageChanged(object sender, EventArgs e)
		{
			this.tabControl.Hide();
			this.tabControl.Show();
		}

		private void Configuration_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.btn_Cancel_Click(sender, e);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private static void Edit(ModPack pack)
		{
			using (ModEditorForm modEditorForm = new ModEditorForm(pack))
			{
				modEditorForm.ShowDialog();
			}
		}

		private void EnableApplyButton(object sender, DataGridViewCellEventArgs args)
		{
			this.menuInferior.ButtonApplyEnabled = true;
		}

		private ModsSelector GetCurrentSelector()
		{
			if (this.tabControl.SelectedTab == this.genTabPage)
			{
				return this.genSelector;
			}
			if (this.tabControl.SelectedTab != this.bgmTabPage)
			{
				return this.vidSelector;
			}
			return this.bgmSelector;
		}

		private ModPack GetSelectedPack()
		{
			return (ModPack)this.cbxSetMod.SelectedItem;
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FrModsManager));
			this.btnMenuContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.tsmiNew = new ToolStripMenuItem();
			this.tsmiImportFolder = new ToolStripMenuItem();
			this.tsmiImportZip = new ToolStripMenuItem();
			this.toolStripSeparator1 = new ToolStripSeparator();
			this.tsmiEditOngPatcher = new ToolStripMenuItem();
			this.tsmiEditOnExplorer = new ToolStripMenuItem();
			this.tsmiSeparator = new ToolStripSeparator();
			this.tsmiExport = new ToolStripMenuItem();
			this.tsmiDelete = new ToolStripMenuItem();
			this.selectorContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.tsmiApplyToSelected = new ToolStripMenuItem();
			this.tsmiDisableSelected = new ToolStripMenuItem();
			this.tabImageList = new ImageList(this.components);
			this.toolTip = new ToolTip(this.components);
			this.bgmTabPage = new TabPage();
			this.bgmSelector = new ModsSelector();
			this.vidTabPage = new TabPage();
			this.vidSelector = new ModsSelector();
			this.menuInferior = new MenuInferior();
			this.tbxSearch = new ImageTextBox();
			this.btnSetMod = new Button();
			this.tabControl = new Dotnetrix.Controls.TabControl();
			this.genTabPage = new TabPage();
			this.genSelector = new ModsSelector();
			this.cbxSetMod = new ComboBox();
			this.btnMenu = new DropDownButton();
			this.btnMenuContextMenu.SuspendLayout();
			this.selectorContextMenu.SuspendLayout();
			this.bgmTabPage.SuspendLayout();
			this.vidTabPage.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.genTabPage.SuspendLayout();
			base.SuspendLayout();
			ToolStripItemCollection items = this.btnMenuContextMenu.Items;
			ToolStripItem[] toolStripItemArray = new ToolStripItem[] { this.tsmiNew, this.tsmiImportFolder, this.tsmiImportZip, this.toolStripSeparator1, this.tsmiEditOngPatcher, this.tsmiEditOnExplorer, this.tsmiSeparator, this.tsmiExport, this.tsmiDelete };
			items.AddRange(toolStripItemArray);
			this.btnMenuContextMenu.Name = "contextMenu";
			componentResourceManager.ApplyResources(this.btnMenuContextMenu, "btnMenuContextMenu");
			this.tsmiNew.Image = Images16px.PackNew;
			this.tsmiNew.Name = "tsmiNew";
			componentResourceManager.ApplyResources(this.tsmiNew, "tsmiNew");
			this.tsmiNew.Click += new EventHandler(this.tsmi_NewPackage_Click);
			this.tsmiImportFolder.Image = Images16px.Folder;
			this.tsmiImportFolder.Name = "tsmiImportFolder";
			componentResourceManager.ApplyResources(this.tsmiImportFolder, "tsmiImportFolder");
			this.tsmiImportFolder.Click += new EventHandler(this.tsmi_ImportFolder_Click);
			this.tsmiImportZip.Name = "tsmiImportZip";
			componentResourceManager.ApplyResources(this.tsmiImportZip, "tsmiImportZip");
			this.tsmiImportZip.Click += new EventHandler(this.tsmi_ImportZip_Click);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			componentResourceManager.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			this.tsmiEditOngPatcher.Image = Images16px.gPatcher;
			this.tsmiEditOngPatcher.Name = "tsmiEditOngPatcher";
			componentResourceManager.ApplyResources(this.tsmiEditOngPatcher, "tsmiEditOngPatcher");
			this.tsmiEditOngPatcher.Click += new EventHandler(this.tsmi_EditOn_gPatcher_Click);
			this.tsmiEditOnExplorer.Image = Images16px.Explorer;
			this.tsmiEditOnExplorer.Name = "tsmiEditOnExplorer";
			componentResourceManager.ApplyResources(this.tsmiEditOnExplorer, "tsmiEditOnExplorer");
			this.tsmiEditOnExplorer.Click += new EventHandler(this.tsmi_EditOn_WExplorer_Click);
			this.tsmiSeparator.Name = "tsmiSeparator";
			componentResourceManager.ApplyResources(this.tsmiSeparator, "tsmiSeparator");
			this.tsmiExport.Image = Images16px.ZIP;
			this.tsmiExport.Name = "tsmiExport";
			componentResourceManager.ApplyResources(this.tsmiExport, "tsmiExport");
			this.tsmiExport.Click += new EventHandler(this.btnExportToZip_Click);
			this.tsmiDelete.Image = Images16px.Cross;
			this.tsmiDelete.Name = "tsmiDelete";
			componentResourceManager.ApplyResources(this.tsmiDelete, "tsmiDelete");
			this.tsmiDelete.Click += new EventHandler(this.tsmi_Delete_Click);
			ToolStripItemCollection toolStripItemCollections = this.selectorContextMenu.Items;
			ToolStripItem[] toolStripItemArray1 = new ToolStripItem[] { this.tsmiApplyToSelected, this.tsmiDisableSelected };
			toolStripItemCollections.AddRange(toolStripItemArray1);
			this.selectorContextMenu.Name = "contextMenuStrip1";
			componentResourceManager.ApplyResources(this.selectorContextMenu, "selectorContextMenu");
			this.selectorContextMenu.Opening += new CancelEventHandler(this.selectorContextMenu_Opening);
			this.tsmiApplyToSelected.Image = Images16px.Tick;
			this.tsmiApplyToSelected.Name = "tsmiApplyToSelected";
			componentResourceManager.ApplyResources(this.tsmiApplyToSelected, "tsmiApplyToSelected");
			this.tsmiApplyToSelected.Click += new EventHandler(this.tsmi_ApplyToSelected_Click);
			this.tsmiDisableSelected.Image = Images16px.Cancel;
			this.tsmiDisableSelected.Name = "tsmiDisableSelected";
			componentResourceManager.ApplyResources(this.tsmiDisableSelected, "tsmiDisableSelected");
			this.tsmiDisableSelected.Click += new EventHandler(this.tsmi_DisableSelected_Click);
			this.tabImageList.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("tabImageList.ImageStream");
			this.tabImageList.TransparentColor = Color.Transparent;
			this.tabImageList.Images.SetKeyName(0, "Mods_General.png");
			this.tabImageList.Images.SetKeyName(1, "Mods_Music.png");
			this.tabImageList.Images.SetKeyName(2, "Mods_Video.png");
			this.bgmTabPage.Controls.Add(this.bgmSelector);
			componentResourceManager.ApplyResources(this.bgmTabPage, "bgmTabPage");
			this.bgmTabPage.Name = "bgmTabPage";
			this.bgmTabPage.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.bgmSelector, "bgmSelector");
			this.bgmSelector.ModType = ModTypes.BGM;
			this.bgmSelector.Name = "bgmSelector";
			this.bgmSelector.CellValueChanged += new DataGridViewCellEventHandler(this.EnableApplyButton);
			this.bgmSelector.MouseUp += new MouseEventHandler(this.ModsSelector_MouseUp);
			this.vidTabPage.Controls.Add(this.vidSelector);
			componentResourceManager.ApplyResources(this.vidTabPage, "vidTabPage");
			this.vidTabPage.Name = "vidTabPage";
			this.vidTabPage.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.vidSelector, "vidSelector");
			this.vidSelector.ModType = ModTypes.Video;
			this.vidSelector.Name = "vidSelector";
			this.vidSelector.CellValueChanged += new DataGridViewCellEventHandler(this.EnableApplyButton);
			this.vidSelector.MouseUp += new MouseEventHandler(this.ModsSelector_MouseUp);
			componentResourceManager.ApplyResources(this.menuInferior, "menuInferior");
			this.menuInferior.Name = "menuInferior";
			this.menuInferior.ButtonOkClick += new EventHandler(this.btn_OK_Click);
			this.menuInferior.ButtonCancelClick += new EventHandler(this.btn_Cancel_Click);
			this.menuInferior.ButtonApplyClick += new EventHandler(this.btn_Apply_Click);
			componentResourceManager.ApplyResources(this.tbxSearch, "tbxSearch");
			this.tbxSearch.BackColor = SystemColors.Window;
			this.tbxSearch.Image = Images16px.Search;
			this.tbxSearch.Name = "tbxSearch";
			this.tbxSearch.TextBoxTextChanged += new EventHandler(this.tbx_Search_TextChanged);
			componentResourceManager.ApplyResources(this.btnSetMod, "btnSetMod");
			this.btnSetMod.Image = Images16px.Tick;
			this.btnSetMod.Name = "btnSetMod";
			this.btnSetMod.UseVisualStyleBackColor = true;
			this.btnSetMod.Click += new EventHandler(this.btnSetMod_Click);
			this.btnSetMod.MouseUp += new MouseEventHandler(this.btn_SetMod_MouseUp);
			componentResourceManager.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Controls.Add(this.genTabPage);
			this.tabControl.Controls.Add(this.bgmTabPage);
			this.tabControl.Controls.Add(this.vidTabPage);
			this.tabControl.ImageList = this.tabImageList;
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.SizeMode = TabSizeMode.Fixed;
			this.tabControl.SelectedIndexChanged += new EventHandler(this.tabControl_SelectedIndexChanged);
			this.genTabPage.Controls.Add(this.genSelector);
			componentResourceManager.ApplyResources(this.genTabPage, "genTabPage");
			this.genTabPage.Name = "genTabPage";
			this.genTabPage.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.genSelector, "genSelector");
			this.genSelector.ModType = ModTypes.General;
			this.genSelector.Name = "genSelector";
			this.genSelector.CellValueChanged += new DataGridViewCellEventHandler(this.EnableApplyButton);
			this.genSelector.MouseUp += new MouseEventHandler(this.ModsSelector_MouseUp);
			componentResourceManager.ApplyResources(this.cbxSetMod, "cbxSetMod");
			this.cbxSetMod.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbxSetMod.FormattingEnabled = true;
			this.cbxSetMod.Name = "cbxSetMod";
			this.cbxSetMod.SelectedIndexChanged += new EventHandler(this.cbx_SetMod_SelectedIndexChanged);
			componentResourceManager.ApplyResources(this.btnMenu, "btnMenu");
			this.btnMenu.Image = Images16px.Menu;
			this.btnMenu.Menu = this.btnMenuContextMenu;
			this.btnMenu.Name = "btnMenu";
			this.btnMenu.UseVisualStyleBackColor = true;
			base.AutoBackgroundImage = BackgroundImages.Elesis;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.btnMenu);
			base.Controls.Add(this.menuInferior);
			base.Controls.Add(this.tbxSearch);
			base.Controls.Add(this.btnSetMod);
			base.Controls.Add(this.tabControl);
			base.Controls.Add(this.cbxSetMod);
			this.DoubleBuffered = true;
			base.KeyPreview = true;
			base.Name = "FrModsManager";
			base.PersistState = true;
			base.FormClosing += new FormClosingEventHandler(this.Configuration_FormClosing);
			base.BackgroundImageChanged += new EventHandler(this.Configuration_BackgroundImageChanged);
			base.VisibleChanged += new EventHandler(this.ManagerForm_VisibleChanged);
			base.KeyDown += new KeyEventHandler(this.ManagerForm_KeyDown);
			this.btnMenuContextMenu.ResumeLayout(false);
			this.selectorContextMenu.ResumeLayout(false);
			this.bgmTabPage.ResumeLayout(false);
			this.vidTabPage.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.genTabPage.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void ManagerForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.F)
			{
				this.tbxSearch.Focus();
			}
		}

		private void ManagerForm_VisibleChanged(object sender, EventArgs e)
		{
			if (base.Visible)
			{
				this.menuInferior.ButtonApplyEnabled = false;
			}
		}

		private void ModsSelector_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button != System.Windows.Forms.MouseButtons.Right)
			{
				return;
			}
			MyDataGridView myDataGridView = sender as MyDataGridView;
			if (myDataGridView == null)
			{
				return;
			}
			this.selectorContextMenu.Tag = myDataGridView.Parent;
			this.selectorContextMenu.Show(myDataGridView, e.Location);
		}

		private void Save()
		{
			GlobalData.PresetsManager.Clear();
			ModsSelector[] modsSelectorArray = this._modsSelectors;
			for (int i = 0; i < (int)modsSelectorArray.Length; i++)
			{
				ModsSelector modsSelector = modsSelectorArray[i];
				GlobalData.PresetsManager.AddRange<Preset>(modsSelector.GetPreset());
			}
			try
			{
				GlobalData.PresetsManager.Save();
			}
			catch (Exception exception)
			{
				MsgBox.Error(gPatcher.Localization.Text.Error_SavingUserMods, exception);
			}
		}

		private void selectorContextMenu_Opening(object sender, CancelEventArgs e)
		{
			if (this.cbxSetMod.SelectedIndex == 0)
			{
				this.tsmiApplyToSelected.Visible = false;
				this.tsmiApplyToSelected.Text = string.Empty;
				return;
			}
			this.tsmiApplyToSelected.Visible = true;
			string windowManagerFormSetModXOnSelectedItems = gPatcher.Localization.Text.Window_ManagerForm_SetModXOnSelectedItems;
			this.tsmiApplyToSelected.Text = string.Format(windowManagerFormSetModXOnSelectedItems, this.cbxSetMod.Text);
		}

		private void SetAllSelectors(ModPack mod)
		{
			ModsSelector[] modsSelectorArray = this._modsSelectors;
			for (int i = 0; i < (int)modsSelectorArray.Length; i++)
			{
				modsSelectorArray[i].SetModOnAllRows(mod);
			}
		}

		private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.tabControl.SelectedTab.Controls[0].Focus();
		}

		private void tbx_Search_TextChanged(object sender, EventArgs e)
		{
			ModsSelector[] modsSelectorArray = this._modsSelectors;
			for (int i = 0; i < (int)modsSelectorArray.Length; i++)
			{
				modsSelectorArray[i].Filter(this.tbxSearch.Text);
			}
		}

		private void tsmi_ApplyToSelected_Click(object sender, EventArgs e)
		{
			ModsSelector tag = (ModsSelector)this.selectorContextMenu.Tag;
			tag.SetModOnSelectedRows((ModPack)this.cbxSetMod.SelectedItem);
			tag.ClearSelection();
		}

		private void tsmi_Delete_Click(object sender, EventArgs e)
		{
			ModPack selectedPack = this.GetSelectedPack();
			if (MsgBox.Question(gPatcher.Localization.Text.Confirmation_DeleteMod) != System.Windows.Forms.DialogResult.Yes)
			{
				return;
			}
			try
			{
				selectedPack.Delete();
				MsgBox.Success(gPatcher.Localization.Text.Success_DeleteMod);
			}
			catch (Exception exception)
			{
				MsgBox.Error(gPatcher.Localization.Text.Error_ModDelete, exception);
			}
		}

		private void tsmi_DisableSelected_Click(object sender, EventArgs e)
		{
			ModsSelector tag = (ModsSelector)this.selectorContextMenu.Tag;
			tag.DisableSelectedRows();
			tag.ClearSelection();
		}

		private void tsmi_EditOn_gPatcher_Click(object sender, EventArgs e)
		{
			FrModsManager.Edit(this.GetSelectedPack());
		}

		private void tsmi_EditOn_WExplorer_Click(object sender, EventArgs e)
		{
			ModPack selectedPack = this.GetSelectedPack();
			try
			{
				Process.Start(selectedPack.FullName);
			}
			catch (Exception exception)
			{
				MsgBox.Error(gPatcher.Localization.Text.Error_OpenInExplorer, exception);
			}
		}

		private void tsmi_ImportFolder_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
			{
				Description = gPatcher.Localization.Text.Action_ImportDirectory,
				ShowNewFolderButton = false
			};
			FolderBrowserDialog folderBrowserDialog1 = folderBrowserDialog;
			if (folderBrowserDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
			{
				return;
			}
			using (FolderImportDialog folderImportDialog = new FolderImportDialog(folderBrowserDialog1.SelectedPath))
			{
				if (folderImportDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					this.AskAndSet(folderImportDialog.DirectoryName);
				}
			}
		}

		private void tsmi_ImportZip_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog()
			{
				DefaultExt = "zip",
				RestoreDirectory = true,
				Filter = gPatcher.Localization.Text.Window_ManagerForm_SelectZipFilter
			};
			using (OpenFileDialog openFileDialog1 = openFileDialog)
			{
				if (System.Windows.Forms.DialogResult.OK == openFileDialog1.ShowDialog())
				{
					using (ZipImportDialog zipImportDialog = new ZipImportDialog(openFileDialog1.FileName))
					{
						if (System.Windows.Forms.DialogResult.OK == zipImportDialog.ShowDialog())
						{
							this.AskAndSet(zipImportDialog.Folder);
						}
						else
						{
							return;
						}
					}
				}
			}
		}

		private void tsmi_NewPackage_Click(object sender, EventArgs e)
		{
			using (NewModDialog newModDialog = new NewModDialog())
			{
				if (newModDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					FrModsManager.Edit(newModDialog.ModPack);
					this.cbxSetMod.SelectedItem = newModDialog.ModPack;
				}
			}
		}
	}
}