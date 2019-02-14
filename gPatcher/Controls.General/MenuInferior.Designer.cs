using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace gPatcher.Controls.General
{
	public sealed class MenuInferior : UserControl
	{
		private IContainer components;

		private GrayPanel panel;

		private Button btnOK;

		private Button btnCancel;

		private Button btnApply;

		private TableLayoutPanel tableLayout;

		[Category("Buttons")]
		[DefaultValue(false)]
		public bool ButtonApplyEnabled
		{
			get
			{
				return this.btnApply.Enabled;
			}
			set
			{
				this.btnApply.Enabled = value;
			}
		}

		[Category("Buttons")]
		[DefaultValue(true)]
		public bool ButtonApplyVisible
		{
			get
			{
				return this.btnApply.Visible;
			}
			set
			{
				this.btnApply.Visible = value;
			}
		}

		[Category("Buttons")]
		[DefaultValue(true)]
		public bool ButtonCancelEnabled
		{
			get
			{
				return this.btnCancel.Enabled;
			}
			set
			{
				this.btnCancel.Enabled = value;
			}
		}

		[Category("Buttons")]
		[DefaultValue(true)]
		public bool ButtonCancelVisible
		{
			get
			{
				return this.btnCancel.Visible;
			}
			set
			{
				this.btnCancel.Visible = value;
			}
		}

		[Category("Buttons")]
		[DefaultValue(true)]
		public bool ButtonOkEnabled
		{
			get
			{
				return this.btnOK.Enabled;
			}
			set
			{
				this.btnOK.Enabled = value;
			}
		}

		[Category("Buttons")]
		[DefaultValue(true)]
		public bool ButtonOkVisible
		{
			get
			{
				return this.btnOK.Visible;
			}
			set
			{
				this.btnOK.Visible = value;
			}
		}

		public MenuInferior()
		{
			this.InitializeComponent();
			this.Dock = DockStyle.Bottom;
			this.btnApply.VisibleChanged += new EventHandler(this.BtnVisibleChanged);
			this.btnCancel.VisibleChanged += new EventHandler(this.BtnVisibleChanged);
			this.btnOK.VisibleChanged += new EventHandler(this.BtnVisibleChanged);
		}

		private void BtnVisibleChanged(object sender, EventArgs eventArgs)
		{
			TableLayoutPanel point = this.tableLayout;
			Point location = this.tableLayout.Location;
			point.Location = new Point(base.Width - this.tableLayout.Width - 5, location.Y);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MenuInferior));
			this.panel = new GrayPanel();
			this.tableLayout = new TableLayoutPanel();
			this.btnOK = new Button();
			this.btnApply = new Button();
			this.btnCancel = new Button();
			this.panel.SuspendLayout();
			this.tableLayout.SuspendLayout();
			base.SuspendLayout();
			this.panel.BackColor = SystemColors.ControlLight;
			this.panel.Controls.Add(this.tableLayout);
			componentResourceManager.ApplyResources(this.panel, "panel");
			this.panel.Name = "panel";
			componentResourceManager.ApplyResources(this.tableLayout, "tableLayout");
			this.tableLayout.Controls.Add(this.btnOK, 0, 0);
			this.tableLayout.Controls.Add(this.btnApply, 2, 0);
			this.tableLayout.Controls.Add(this.btnCancel, 1, 0);
			this.tableLayout.Name = "tableLayout";
			componentResourceManager.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.Name = "btnOK";
			this.btnOK.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.btnApply, "btnApply");
			this.btnApply.Name = "btnApply";
			this.btnApply.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.panel);
			this.MinimumSize = new System.Drawing.Size(255, 42);
			base.Name = "MenuInferior";
			this.panel.ResumeLayout(false);
			this.panel.PerformLayout();
			this.tableLayout.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		[Category("Buttons")]
		public event EventHandler ButtonApplyClick
		{
			add
			{
				this.btnApply.Click += value;
			}
			remove
			{
				this.btnApply.Click -= value;
			}
		}

		[Category("Buttons")]
		public event EventHandler ButtonCancelClick
		{
			add
			{
				this.btnCancel.Click += value;
			}
			remove
			{
				this.btnCancel.Click -= value;
			}
		}

		[Category("Buttons")]
		public event EventHandler ButtonOkClick
		{
			add
			{
				this.btnOK.Click += value;
			}
			remove
			{
				this.btnOK.Click -= value;
			}
		}
	}
}