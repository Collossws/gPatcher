using Dotnetrix.Controls;
using Ginpo.Controls;
using Ginpo.gPatcher;
using gPatcher;
using gPatcher.Controls.General;
using gPatcher.Controls.Modding;
using gPatcher.Helpers;
using gPatcher.Helpers.GlobalDataHolding;
using gPatcher.Localization;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace gPatcher.Forms.Main
{
	public class ModEditorForm : AutoBgForm
	{
		private readonly ModEditor[] _editors;

		private readonly ModPack _modPack;

		private IContainer components;

		private Label lblName;

		private TextBox tbxName;

		private PictureBox pictureBox2;

		private ImageList tabImageList;

		private Dotnetrix.Controls.TabControl tabControl;

		private TabPage genTabPage;

		private TabPage bgmTabPage;

		private TabPage vidTabPage;

		private ModEditor genEditor;

		private ModEditor bgmEditor;

		private ModEditor vidEditor;

		private MenuInferior menuInferior;

		private ImageTextBox tbxSearch;

		public sealed override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
			}
		}

		public ModEditorForm(ModPack modPack)
		{
			this.InitializeComponent();
			this._modPack = modPack;
			this.Text = string.Format(gPatcher.Localization.Text.Window_ModEditorDialog_Title, modPack.Name);
			this.tbxName.Text = modPack.Name;
			this.tbxName.TextChanged += new EventHandler(this.EnableApplyButton);
			ModEditor[] modEditorArray = new ModEditor[] { this.genEditor, this.bgmEditor, this.vidEditor };
			this._editors = modEditorArray;
			ModEditor[] modEditorArray1 = this._editors;
			for (int i = 0; i < (int)modEditorArray1.Length; i++)
			{
				ModEditor modEditor = modEditorArray1[i];
				modEditor.ModPack = modPack;
				modEditor.FileEnabledChanged += new FileEnabledChanged(this.EnableApplyButton);
			}
		}

		private void btnApply_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(this.tbxName.Text))
			{
				try
				{
					this._modPack.Rename(this.tbxName.Text);
				}
				catch (Exception exception)
				{
					MsgBox.Error(gPatcher.Localization.Text.Error_RenameMod, exception);
					return;
				}
			}
			try
			{
				ModEditor[] modEditorArray = this._editors;
				for (int i = 0; i < (int)modEditorArray.Length; i++)
				{
					modEditorArray[i].Save();
				}
			}
			catch (Exception exception1)
			{
				MsgBox.Error(gPatcher.Localization.Text.Error_SavingMod, exception1);
				return;
			}
			this.menuInferior.ButtonApplyEnabled = false;
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			base.Close();
			ModEditor[] modEditorArray = this._editors;
			for (int i = 0; i < (int)modEditorArray.Length; i++)
			{
				modEditorArray[i].Cancel();
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (this.menuInferior.ButtonApplyEnabled)
			{
				this.btnApply_Click(sender, e);
			}
			base.Close();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		public void EnableApplyButton(object sender, EventArgs e)
		{
			this.menuInferior.ButtonApplyEnabled = true;
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ModEditorForm));
			this.tbxName = new TextBox();
			this.lblName = new Label();
			this.pictureBox2 = new PictureBox();
			this.tabImageList = new ImageList(this.components);
			this.tabControl = new Dotnetrix.Controls.TabControl();
			this.genTabPage = new TabPage();
			this.genEditor = new ModEditor();
			this.bgmTabPage = new TabPage();
			this.bgmEditor = new ModEditor();
			this.vidTabPage = new TabPage();
			this.vidEditor = new ModEditor();
			this.menuInferior = new MenuInferior();
			this.tbxSearch = new ImageTextBox();
			((ISupportInitialize)this.pictureBox2).BeginInit();
			this.tabControl.SuspendLayout();
			this.genTabPage.SuspendLayout();
			this.bgmTabPage.SuspendLayout();
			this.vidTabPage.SuspendLayout();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.tbxName, "tbxName");
			this.tbxName.Name = "tbxName";
			this.tbxName.KeyDown += new KeyEventHandler(this.tbx_Name_KeyDown);
			componentResourceManager.ApplyResources(this.lblName, "lblName");
			this.lblName.BackColor = Color.Transparent;
			this.lblName.Name = "lblName";
			componentResourceManager.ApplyResources(this.pictureBox2, "pictureBox2");
			this.pictureBox2.BackColor = Color.Transparent;
			this.pictureBox2.Image = Images16px.Rename;
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.TabStop = false;
			this.tabImageList.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("tabImageList.ImageStream");
			this.tabImageList.TransparentColor = Color.Transparent;
			this.tabImageList.Images.SetKeyName(0, "Mods_General.png");
			this.tabImageList.Images.SetKeyName(1, "Mods_Music.png");
			this.tabImageList.Images.SetKeyName(2, "Mods_Video.png");
			componentResourceManager.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Controls.Add(this.genTabPage);
			this.tabControl.Controls.Add(this.bgmTabPage);
			this.tabControl.Controls.Add(this.vidTabPage);
			this.tabControl.ImageList = this.tabImageList;
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.SizeMode = TabSizeMode.Fixed;
			componentResourceManager.ApplyResources(this.genTabPage, "genTabPage");
			this.genTabPage.Controls.Add(this.genEditor);
			this.genTabPage.Name = "genTabPage";
			this.genTabPage.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.genEditor, "genEditor");
			this.genEditor.ModPack = null;
			this.genEditor.Name = "genEditor";
			componentResourceManager.ApplyResources(this.bgmTabPage, "bgmTabPage");
			this.bgmTabPage.Controls.Add(this.bgmEditor);
			this.bgmTabPage.Name = "bgmTabPage";
			this.bgmTabPage.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.bgmEditor, "bgmEditor");
			this.bgmEditor.ModPack = null;
			this.bgmEditor.ModType = ModTypes.BGM;
			this.bgmEditor.Name = "bgmEditor";
			componentResourceManager.ApplyResources(this.vidTabPage, "vidTabPage");
			this.vidTabPage.Controls.Add(this.vidEditor);
			this.vidTabPage.Name = "vidTabPage";
			this.vidTabPage.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.vidEditor, "vidEditor");
			this.vidEditor.ModPack = null;
			this.vidEditor.ModType = ModTypes.Video;
			this.vidEditor.Name = "vidEditor";
			componentResourceManager.ApplyResources(this.menuInferior, "menuInferior");
			this.menuInferior.Name = "menuInferior";
			this.menuInferior.ButtonOkClick += new EventHandler(this.btnOK_Click);
			this.menuInferior.ButtonCancelClick += new EventHandler(this.btnCancel_Click);
			this.menuInferior.ButtonApplyClick += new EventHandler(this.btnApply_Click);
			componentResourceManager.ApplyResources(this.tbxSearch, "tbxSearch");
			this.tbxSearch.BackColor = SystemColors.Window;
			this.tbxSearch.Image = Images16px.Search;
			this.tbxSearch.Name = "tbxSearch";
			this.tbxSearch.TextBoxTextChanged += new EventHandler(this.tbxSearch_TextChanged);
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoBackgroundImage = BackgroundImages.Raven;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.menuInferior);
			base.Controls.Add(this.tbxSearch);
			base.Controls.Add(this.pictureBox2);
			base.Controls.Add(this.tbxName);
			base.Controls.Add(this.lblName);
			base.Controls.Add(this.tabControl);
			this.DoubleBuffered = true;
			base.KeyPreview = true;
			base.Name = "ModEditorForm";
			base.Load += new EventHandler(this.NewModEditorDialog_Load);
			base.KeyDown += new KeyEventHandler(this.ModEditorForm_KeyDown);
			((ISupportInitialize)this.pictureBox2).EndInit();
			this.tabControl.ResumeLayout(false);
			this.genTabPage.ResumeLayout(false);
			this.bgmTabPage.ResumeLayout(false);
			this.vidTabPage.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void ModEditorForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (!e.Control)
			{
				return;
			}
			Keys keyCode = e.KeyCode;
			if (keyCode == Keys.F)
			{
				this.tbxSearch.Focus();
				this.tbxSearch.SelectAll();
				return;
			}
			if (keyCode != Keys.R)
			{
				return;
			}
			this.tbxName.Focus();
			this.tbxName.SelectAll();
		}

		private void NewModEditorDialog_Load(object sender, EventArgs e)
		{
			this.menuInferior.ButtonApplyEnabled = false;
		}

		private void tbx_Name_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				this.btnOK_Click(sender, e);
			}
		}

		private void tbxSearch_TextChanged(object sender, EventArgs e)
		{
			ModEditor[] modEditorArray = this._editors;
			for (int i = 0; i < (int)modEditorArray.Length; i++)
			{
				modEditorArray[i].Filter(this.tbxSearch.Text);
			}
		}
	}
}