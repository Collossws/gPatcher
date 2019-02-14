using Ginpo;
using Ginpo.Extensions;
using Ginpo.gPatcher;
using gPatcher;
using gPatcher.Controls.General;
using gPatcher.Helpers;
using gPatcher.Helpers.GlobalDataHolding;
using gPatcher.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;

namespace gPatcher.Controls.Modding
{
	public class ModEditor : UserControl
	{
		private SortableBindingList<ModFile> _files;

		private BindingView<ModFile> _filesView;

		private gPatcher.Helpers.GlobalDataHolding.ModPack _modPack;

		private ModTypes _modType;

		private IContainer components;

		private MyDataGridView gridView;

		private System.Windows.Forms.ContextMenuStrip gridViewContextMenu;

		private ToolStripMenuItem tsmiDeleteFile;

		private DataGridViewCheckBoxColumn colEnabled;

		private DataGridViewButtonColumn colSelect;

		private DataGridViewTextBoxColumn colFile;

		private DataGridViewTextBoxColumn colDesc;

		private DataGridViewImageColumn colOpen;

		protected new bool DesignMode
		{
			get
			{
				if (base.DesignMode)
				{
					return true;
				}
				return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
			}
		}

		public gPatcher.Helpers.GlobalDataHolding.ModPack ModPack
		{
			get
			{
				return this._modPack;
			}
			set
			{
				if (value == this._modPack || value == null)
				{
					return;
				}
				if (value.IsDummy)
				{
					throw new Exception("It's not possible to edit dummy ModPack objects.");
				}
				this._modPack = value;
				IEnumerable<ModFile> elswordFilesInfo = 
					from f in GlobalData.ElswordFilesInfo
					where f.ModType == this.ModType
					select f into e
					where this._modPack.Files.All<ModFile>((ModFile f) => f.Base != e)
					select new ModFile(this._modPack, e);
				IEnumerable<ModFile> modFiles = (
					from f in this._modPack.Files
					where f.ModType == this.ModType
					select f).Union<ModFile>(elswordFilesInfo);
				this._files = modFiles.ToSortableBindingList<ModFile>();
				this._modPack.FileAdded += new FileAddedEventHandler(this.ModPackOnFileAdded);
				this._filesView = new BindingView<ModFile>(this._files);
				this.gridView.DataSource = this._filesView;
			}
		}

		[DefaultValue(ModTypes.General)]
		public ModTypes ModType
		{
			get
			{
				return this._modType;
			}
			set
			{
				this._modType = value;
				if (value == ModTypes.General)
				{
					this.colFile.FillWeight = 20f;
					this.colOpen.Visible = false;
					return;
				}
				this.colFile.FillWeight = 40f;
				this.colOpen.HeaderText = gPatcher.Localization.Text.ModEditor_ColOpen_Media;
				this.colOpen.Image = Images16px.Play;
				this.colOpen.Visible = true;
			}
		}

		public ModEditor()
		{
			this.InitializeComponent();
			this.gridView.AutoGenerateColumns = false;
			this.colEnabled.DataPropertyName = "Enabled";
			this.colFile.DataPropertyName = "FileName";
			this.colDesc.DataPropertyName = "Description";
			this.ModType = ModTypes.General;
			base.Disposed += new EventHandler(this.OnDisposed);
		}

		public void Cancel()
		{
			foreach (ModFile file in this._modPack.Files)
			{
				file.Cancel();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		public void Filter(string filter)
		{
			this._filesView.Filter = (ModFile item) => (new string[] { item.FileName, item.Description }).Any<string>((string s) => s.Contains(filter, true));
		}

		private static ModFile GetItem(DataGridViewRow row)
		{
			return (ModFile)row.DataBoundItem;
		}

		private ModFile GetItemAt(int rowIndex)
		{
			return ModEditor.GetItem(this.gridView.Rows[rowIndex]);
		}

		private void gridView_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex == -1)
			{
				return;
			}
			int columnIndex = e.ColumnIndex;
			if (columnIndex == 1)
			{
				this.SelectFile(e.RowIndex);
				return;
			}
			if (columnIndex != 4)
			{
				return;
			}
			this.Play(e.RowIndex);
		}

		private void gridView_KeyDown(object sender, KeyEventArgs e)
		{
			Keys keyCode = e.KeyCode;
			if (keyCode != Keys.Return)
			{
				if (keyCode != Keys.Delete)
				{
					return;
				}
				this.tsmiDeleteFile.PerformClick();
				return;
			}
			int rowIndex = this.gridView.CurrentCell.RowIndex;
			this.gridView_CellClick(sender, new DataGridViewCellEventArgs(4, rowIndex));
			e.Handled = true;
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ModEditor));
			DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
			DataGridViewCellStyle control = new DataGridViewCellStyle();
			this.gridViewContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.tsmiDeleteFile = new ToolStripMenuItem();
			this.gridView = new MyDataGridView();
			this.colEnabled = new DataGridViewCheckBoxColumn();
			this.colSelect = new DataGridViewButtonColumn();
			this.colFile = new DataGridViewTextBoxColumn();
			this.colDesc = new DataGridViewTextBoxColumn();
			this.colOpen = new DataGridViewImageColumn();
			this.gridViewContextMenu.SuspendLayout();
			((ISupportInitialize)this.gridView).BeginInit();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.gridViewContextMenu, "gridViewContextMenu");
			this.gridViewContextMenu.Items.AddRange(new ToolStripItem[] { this.tsmiDeleteFile });
			this.gridViewContextMenu.Name = "gridViewContextMenu";
			componentResourceManager.ApplyResources(this.tsmiDeleteFile, "tsmiDeleteFile");
			this.tsmiDeleteFile.Image = Images16px.Cross;
			this.tsmiDeleteFile.Name = "tsmiDeleteFile";
			this.tsmiDeleteFile.Click += new EventHandler(this.tsmi_DeleteFile_Click);
			componentResourceManager.ApplyResources(this.gridView, "gridView");
			this.gridView.AllowUserToAddRows = false;
			this.gridView.AllowUserToDeleteRows = false;
			this.gridView.AllowUserToResizeColumns = false;
			this.gridView.AllowUserToResizeRows = false;
			this.gridView.BackgroundColor = SystemColors.ControlLightLight;
			this.gridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.gridView.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
			this.gridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;
			this.gridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			DataGridViewColumnCollection columns = this.gridView.Columns;
			DataGridViewColumn[] dataGridViewColumnArray = new DataGridViewColumn[] { this.colEnabled, this.colSelect, this.colFile, this.colDesc, this.colOpen };
			columns.AddRange(dataGridViewColumnArray);
			this.gridView.ContextMenuStrip = this.gridViewContextMenu;
			dataGridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle.BackColor = SystemColors.ControlLightLight;
			dataGridViewCellStyle.Font = new System.Drawing.Font("Segoe UI", 8.25f);
			dataGridViewCellStyle.ForeColor = SystemColors.ControlText;
			dataGridViewCellStyle.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle.WrapMode = DataGridViewTriState.False;
			this.gridView.DefaultCellStyle = dataGridViewCellStyle;
			this.gridView.EditMode = DataGridViewEditMode.EditOnEnter;
			this.gridView.Name = "gridView";
			this.gridView.RowFilter = null;
			this.gridView.RowHeadersVisible = false;
			this.gridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			this.gridView.CellClick += new DataGridViewCellEventHandler(this.gridView_CellClick);
			this.gridView.KeyDown += new KeyEventHandler(this.gridView_KeyDown);
			this.colEnabled.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			componentResourceManager.ApplyResources(this.colEnabled, "colEnabled");
			this.colEnabled.Name = "colEnabled";
			this.colEnabled.ReadOnly = true;
			this.colEnabled.SortMode = DataGridViewColumnSortMode.Automatic;
			this.colSelect.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			this.colSelect.DividerWidth = 1;
			componentResourceManager.ApplyResources(this.colSelect, "colSelect");
			this.colSelect.Name = "colSelect";
			this.colSelect.Text = "...";
			this.colSelect.UseColumnTextForButtonValue = true;
			this.colFile.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			this.colFile.FillWeight = 40f;
			componentResourceManager.ApplyResources(this.colFile, "colFile");
			this.colFile.Name = "colFile";
			this.colFile.ReadOnly = true;
			this.colFile.Resizable = DataGridViewTriState.True;
			this.colDesc.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			this.colDesc.DividerWidth = 1;
			this.colDesc.FillWeight = 60f;
			componentResourceManager.ApplyResources(this.colDesc, "colDesc");
			this.colDesc.Name = "colDesc";
			this.colDesc.ReadOnly = true;
			this.colDesc.Resizable = DataGridViewTriState.True;
			this.colOpen.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			control.Alignment = DataGridViewContentAlignment.MiddleCenter;
			control.BackColor = SystemColors.Control;
			control.NullValue = componentResourceManager.GetObject("dataGridViewCellStyle1.NullValue");
			this.colOpen.DefaultCellStyle = control;
			componentResourceManager.ApplyResources(this.colOpen, "colOpen");
			this.colOpen.Name = "colOpen";
			this.colOpen.Resizable = DataGridViewTriState.False;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.gridView);
			base.Name = "ModEditor";
			this.gridViewContextMenu.ResumeLayout(false);
			((ISupportInitialize)this.gridView).EndInit();
			base.ResumeLayout(false);
		}

		private void ModPackOnFileAdded(object sender, ModFile file)
		{
			for (int i = this._files.Count - 1; i >= 0; i--)
			{
				if (this._files[i].Base == file.Base)
				{
					this._files[i] = file;
				}
			}
			BindingList<ModFile> dataSource = (BindingList<ModFile>)this.gridView.DataSource;
			for (int j = dataSource.Count - 1; j >= 0; j--)
			{
				if (dataSource[j].Base == file.Base)
				{
					dataSource[j] = file;
				}
			}
		}

		private void OnDisposed(object sender, EventArgs args)
		{
			this._files.Clear();
		}

		protected void OnFileEnabledChanged()
		{
			if (this.FileEnabledChanged != null)
			{
				this.FileEnabledChanged(this, EventArgs.Empty);
			}
		}

		private void Play(int rowIndex)
		{
			ModFile itemAt = this.GetItemAt(rowIndex);
			try
			{
				ModPlayer.Play(itemAt);
			}
			catch (Exception exception)
			{
				MsgBox.Error(exception.Message);
			}
		}

		public void Save()
		{
			for (int i = this._files.Count - 1; this._files.Count > 0 && i >= 0; i--)
			{
				this._files[i].SaveChanges();
			}
		}

		private void SelectFile(int rowIndex)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog()
			{
				Title = gPatcher.Localization.Text.Window_ModEditorDialog_SelectFile_Title
			};
			switch (this.ModType)
			{
				case ModTypes.General:
				{
					openFileDialog.Filter = gPatcher.Localization.Text.Window_ModEditorDialog_SelectFileFilter_General;
					break;
				}
				case ModTypes.BGM:
				{
					openFileDialog.Filter = gPatcher.Localization.Text.Window_ModEditorDialog_SelectFileFilter_BGM;
					break;
				}
				case ModTypes.Video:
				{
					openFileDialog.Filter = gPatcher.Localization.Text.Window_ModEditorDialog_SelectFileFilter_Video;
					break;
				}
			}
			if (openFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			this.GetItemAt(rowIndex).Replace(openFileDialog.FileName);
			this.OnFileEnabledChanged();
		}

		private void tsmi_DeleteFile_Click(object sender, EventArgs e)
		{
			foreach (DataGridViewRow selectedRow in this.gridView.SelectedRows)
			{
				ModEditor.GetItem(selectedRow).Delete();
				this.OnFileEnabledChanged();
			}
		}

		public event gPatcher.Controls.Modding.FileEnabledChanged FileEnabledChanged;

		private enum Columns
		{
			Select = 1,
			Open = 4
		}
	}
}