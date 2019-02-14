using Ginpo.Controls;
using gPatcher.Controls.General;
using gPatcher.Helpers;
using gPatcher.Localization;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace gPatcher.Forms.ProgressDialogs
{
	public class BaseProgressDialog : MyForm
	{
		protected bool NoChange = true;

		private IContainer components;

		private TableLayoutPanel tableLayoutPanel;

		public Label lblCurrentFile;

		public ProgressBar progressBar;

		public PictureBox pictureBox;

		public BackgroundWorker backgroundWorker;

		private MenuInferior menuInferior;

		public BaseProgressDialog()
		{
			this.InitializeComponent();
		}

		public virtual void btn_Cancel_Click(object sender, EventArgs e)
		{
			this.backgroundWorker.CancelAsync();
			base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(BaseProgressDialog));
			this.tableLayoutPanel = new TableLayoutPanel();
			this.lblCurrentFile = new Label();
			this.progressBar = new ProgressBar();
			this.pictureBox = new PictureBox();
			this.backgroundWorker = new BackgroundWorker();
			this.menuInferior = new MenuInferior();
			this.tableLayoutPanel.SuspendLayout();
			((ISupportInitialize)this.pictureBox).BeginInit();
			base.SuspendLayout();
			this.tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 1;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel.Controls.Add(this.lblCurrentFile, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.progressBar, 0, 1);
			this.tableLayoutPanel.Location = new Point(49, 12);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 2;
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20f));
			this.tableLayoutPanel.Size = new System.Drawing.Size(258, 35);
			this.tableLayoutPanel.TabIndex = 0;
			this.lblCurrentFile.AutoEllipsis = true;
			this.lblCurrentFile.Dock = DockStyle.Fill;
			this.lblCurrentFile.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lblCurrentFile.Location = new Point(3, 0);
			this.lblCurrentFile.Name = "lblCurrentFile";
			this.lblCurrentFile.Size = new System.Drawing.Size(252, 13);
			this.lblCurrentFile.TabIndex = 0;
			this.progressBar.Dock = DockStyle.Fill;
			this.progressBar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.progressBar.Location = new Point(3, 16);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(252, 16);
			this.progressBar.TabIndex = 1;
			this.pictureBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.pictureBox.Location = new Point(11, 12);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(32, 32);
			this.pictureBox.TabIndex = 12;
			this.pictureBox.TabStop = false;
			this.backgroundWorker.WorkerReportsProgress = true;
			this.backgroundWorker.WorkerSupportsCancellation = true;
			BaseProgressDialog baseProgressDialog = this;
			this.backgroundWorker.DoWork += new DoWorkEventHandler(baseProgressDialog.OnDoWork);
			this.backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(this.OnProgressChanged);
			BaseProgressDialog baseProgressDialog1 = this;
			this.backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(baseProgressDialog1.OnRunWorkerCompleted);
			this.menuInferior.ButtonApplyVisible = false;
			this.menuInferior.ButtonOkVisible = false;
			this.menuInferior.Dock = DockStyle.Bottom;
			this.menuInferior.Font = new System.Drawing.Font("Segoe UI", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.menuInferior.Location = new Point(0, 57);
			this.menuInferior.MinimumSize = new System.Drawing.Size(255, 42);
			this.menuInferior.Name = "menuInferior";
			this.menuInferior.Size = new System.Drawing.Size(316, 42);
			this.menuInferior.TabIndex = 13;
			BaseProgressDialog baseProgressDialog2 = this;
			this.menuInferior.ButtonCancelClick += new EventHandler(baseProgressDialog2.btn_Cancel_Click);
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(316, 99);
			base.Controls.Add(this.menuInferior);
			base.Controls.Add(this.tableLayoutPanel);
			base.Controls.Add(this.pictureBox);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Segoe UI", 8.25f);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "BaseProgressDialog";
			base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.tableLayoutPanel.ResumeLayout(false);
			((ISupportInitialize)this.pictureBox).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		protected virtual void OnDoWork(object sender, DoWorkEventArgs e)
		{
			this.NoChange = false;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!base.DesignMode)
			{
				this.backgroundWorker.RunWorkerAsync();
			}
		}

		private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			this.progressBar.Value = e.ProgressPercentage;
			this.lblCurrentFile.Text = (string)e.UserState;
		}

		protected virtual void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (this.NoChange)
			{
				base.DialogResult = System.Windows.Forms.DialogResult.Ignore;
			}
			else if (e.Error == null)
			{
				base.DialogResult = System.Windows.Forms.DialogResult.OK;
			}
			else if (e.Error != null)
			{
				base.DialogResult = System.Windows.Forms.DialogResult.Abort;
				MsgBox.Error(gPatcher.Localization.Text.Error_ImportingFiles, e.Error);
			}
			base.Close();
		}

		protected void Run()
		{
			if (!this.backgroundWorker.IsBusy)
			{
				this.backgroundWorker.RunWorkerAsync();
			}
		}
	}
}