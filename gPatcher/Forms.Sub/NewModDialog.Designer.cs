using Ginpo.Controls;
using gPatcher;
using gPatcher.Controls.General;
using gPatcher.Helpers;
using gPatcher.Helpers.GlobalDataHolding;
using gPatcher.Localization;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace gPatcher.Forms.Sub
{
	public class NewModDialog : AutoBgForm
	{
		private IContainer components;

		private TextBox textBox;

		private Label lblName;

		private MenuInferior menuInferior;

		public gPatcher.Helpers.GlobalDataHolding.ModPack ModPack
		{
			get;
			private set;
		}

		public NewModDialog()
		{
			this.InitializeComponent();
			base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(this.textBox.Text))
			{
				return;
			}
			try
			{
				this.ModPack = GlobalData.ModPacksManager.CreateNew(this.textBox.Text);
			}
			catch (Exception exception)
			{
				MsgBox.Error(gPatcher.Localization.Text.Error_NewMod, exception);
				return;
			}
			base.DialogResult = System.Windows.Forms.DialogResult.OK;
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

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(NewModDialog));
			this.textBox = new TextBox();
			this.lblName = new Label();
			this.menuInferior = new MenuInferior();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.textBox, "textBox");
			this.textBox.Name = "textBox";
			this.textBox.KeyDown += new KeyEventHandler(this.textBox_KeyDown);
			componentResourceManager.ApplyResources(this.lblName, "lblName");
			this.lblName.BackColor = Color.Transparent;
			this.lblName.Name = "lblName";
			componentResourceManager.ApplyResources(this.menuInferior, "menuInferior");
			this.menuInferior.ButtonApplyVisible = false;
			this.menuInferior.Name = "menuInferior";
			this.menuInferior.ButtonOkClick += new EventHandler(this.btn_OK_Click);
			this.menuInferior.ButtonCancelClick += new EventHandler(this.btn_Cancel_Click);
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoBackgroundImage = BackgroundImages.Aisha;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.menuInferior);
			base.Controls.Add(this.textBox);
			base.Controls.Add(this.lblName);
			this.DoubleBuffered = true;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "NewModDialog";
			base.ShowInTaskbar = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void textBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				this.btn_OK_Click(sender, e);
			}
		}
	}
}