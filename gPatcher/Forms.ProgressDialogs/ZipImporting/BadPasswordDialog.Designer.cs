using Ginpo.Controls;
using gPatcher.Controls.General;
using gPatcher.Localization;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace gPatcher.Forms.ProgressDialogs.ZipImporting
{
	public class BadPasswordDialog : MyForm
	{
		public string FileName;

		public string OwnerFileName;

		public string Password;

		private IContainer components;

		private Label label;

		private TextBox tbxPassword;

		private CheckBox cbxShowPassword;

		private MenuInferior menuInferior;

		public BadPasswordDialog(string fileName, string ownerFileName)
		{
			this.FileName = fileName;
			this.OwnerFileName = ownerFileName;
			this.InitializeComponent();
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(this.tbxPassword.Text))
			{
				base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
				return;
			}
			this.Password = this.tbxPassword.Text;
			base.DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void cbxShowPassword_CheckedChanged(object sender, EventArgs e)
		{
			this.tbxPassword.PasswordChar = (this.cbxShowPassword.Checked ? '*' : '\0');
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(BadPasswordDialog));
			this.label = new Label();
			this.tbxPassword = new TextBox();
			this.cbxShowPassword = new CheckBox();
			this.menuInferior = new MenuInferior();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.label, "label");
			this.label.AutoEllipsis = true;
			this.label.Name = "label";
			componentResourceManager.ApplyResources(this.tbxPassword, "tbxPassword");
			this.tbxPassword.Name = "tbxPassword";
			componentResourceManager.ApplyResources(this.cbxShowPassword, "cbxShowPassword");
			this.cbxShowPassword.Name = "cbxShowPassword";
			this.cbxShowPassword.UseVisualStyleBackColor = true;
			this.cbxShowPassword.CheckedChanged += new EventHandler(this.cbxShowPassword_CheckedChanged);
			componentResourceManager.ApplyResources(this.menuInferior, "menuInferior");
			this.menuInferior.ButtonApplyVisible = false;
			this.menuInferior.Name = "menuInferior";
			this.menuInferior.ButtonOkClick += new EventHandler(this.btn_OK_Click);
			this.menuInferior.ButtonCancelClick += new EventHandler(this.btn_Cancel_Click);
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.menuInferior);
			base.Controls.Add(this.cbxShowPassword);
			base.Controls.Add(this.tbxPassword);
			base.Controls.Add(this.label);
			this.DoubleBuffered = true;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "BadPasswordDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.label.Text = string.Format(gPatcher.Localization.Text.Window_BadPasswordDialog_Text, this.FileName, this.OwnerFileName);
		}
	}
}