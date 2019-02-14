using Ginpo.Controls;
using gPatcher;
using gPatcher.Controls.General;
using gPatcher.Localization;
using gPatcher.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace gPatcher.Forms.Sub
{
	internal class AboutDialog : AutoBgForm
	{
		private IContainer components;

		private TableLayoutPanel tableLayoutPanel;

		private PictureBox logoPictureBox;

		private RichTextBox textBoxDescription;

		private Panel panel;

		private Button buttonOK;

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

		public AboutDialog()
		{
			this.InitializeComponent();
			this.Text = string.Format(gPatcher.Localization.Text.Window_About_Title, Application.ProductName, Application.ProductVersion);
			this.textBoxDescription.Rtf = gPatcher.Localization.Text.About;
		}

		internal override void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!Settings.Default.EnableBackgroundImages)
			{
				Panel panel = this.panel;
				RichTextBox richTextBox = this.textBoxDescription;
				Button button = this.buttonOK;
				Color defaultBackColor = Control.DefaultBackColor;
				Color color = defaultBackColor;
				button.BackColor = defaultBackColor;
				Color color1 = color;
				Color color2 = color1;
				richTextBox.BackColor = color1;
				Color color3 = color2;
				Color color4 = color3;
				panel.BackColor = color3;
				this.BackColor = color4;
				this.buttonOK.ForeColor = Control.DefaultForeColor;
				this.buttonOK.UseVisualStyleBackColor = true;
			}
			else
			{
				Panel panel1 = this.panel;
				Button button1 = this.buttonOK;
				RichTextBox richTextBox1 = this.textBoxDescription;
				Color white = Color.White;
				Color color5 = white;
				richTextBox1.BackColor = white;
				Color color6 = color5;
				Color color7 = color6;
				button1.ForeColor = color6;
				Color color8 = color7;
				Color color9 = color8;
				panel1.BackColor = color8;
				this.BackColor = color9;
				this.buttonOK.BackColor = Color.Black;
				this.buttonOK.UseVisualStyleBackColor = false;
			}
			base.Default_PropertyChanged(sender, e);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AboutDialog));
			this.tableLayoutPanel = new TableLayoutPanel();
			this.panel = new Panel();
			this.buttonOK = new Button();
			this.logoPictureBox = new PictureBox();
			this.textBoxDescription = new RichTextBox();
			this.tableLayoutPanel.SuspendLayout();
			this.panel.SuspendLayout();
			((ISupportInitialize)this.logoPictureBox).BeginInit();
			base.SuspendLayout();
			this.tableLayoutPanel.BackColor = Color.Transparent;
			componentResourceManager.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
			this.tableLayoutPanel.Controls.Add(this.panel, 1, 1);
			this.tableLayoutPanel.Controls.Add(this.logoPictureBox, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.textBoxDescription, 1, 0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.panel.Controls.Add(this.buttonOK);
			componentResourceManager.ApplyResources(this.panel, "panel");
			this.panel.Name = "panel";
			componentResourceManager.ApplyResources(this.buttonOK, "buttonOK");
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Name = "buttonOK";
			this.logoPictureBox.BorderStyle = BorderStyle.FixedSingle;
			this.logoPictureBox.Image = Images.AboutImage;
			componentResourceManager.ApplyResources(this.logoPictureBox, "logoPictureBox");
			this.logoPictureBox.Name = "logoPictureBox";
			this.tableLayoutPanel.SetRowSpan(this.logoPictureBox, 2);
			this.logoPictureBox.TabStop = false;
			this.textBoxDescription.BorderStyle = BorderStyle.FixedSingle;
			componentResourceManager.ApplyResources(this.textBoxDescription, "textBoxDescription");
			this.textBoxDescription.Name = "textBoxDescription";
			this.textBoxDescription.ReadOnly = true;
			base.AutoBackgroundImage = BackgroundImages.Add;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.tableLayoutPanel);
			this.DoubleBuffered = true;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "AboutDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.Shown += new EventHandler(this.Sobre_Shown);
			this.tableLayoutPanel.ResumeLayout(false);
			this.panel.ResumeLayout(false);
			((ISupportInitialize)this.logoPictureBox).EndInit();
			base.ResumeLayout(false);
		}

		private void Sobre_Shown(object sender, EventArgs e)
		{
			this.textBoxDescription.Focus();
			this.textBoxDescription.Select(0, 0);
		}
	}
}