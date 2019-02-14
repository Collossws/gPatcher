using Ginpo;
using Ginpo.Extensions;
using Ginpo.gPatcher;
using gPatcher;
using gPatcher.Controls.General;
using gPatcher.Helpers;
using gPatcher.Helpers.GlobalDataHolding;
using gPatcher.Helpers.GlobalDataHolding.Collections;
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
using System.Windows.Forms;

namespace gPatcher.Controls.Modding
{
	public class ModsSelector : UserControl
	{
		private readonly static ModPack DisabledItem;

		private ModTypes _modType;

		private IContainer components;

		private MyDataGridView gridView;

		private DataGridViewImageColumn colImage;

		private DataGridViewTextBoxColumn colFile;

		private DataGridViewTextBoxColumn colDesc;

		private DataGridViewImageColumn colPlay;

		private DataGridViewComboBoxColumn colMod;

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

		public ModTypes ModType
		{
			get
			{
				return this._modType;
			}
			set
			{
				this._modType = value;
				switch (this.ModType)
				{
					case ModTypes.General:
					{
						this.colPlay.Visible = false;
						this.colImage.Visible = true;
						this.colDesc.DividerWidth = 0;
						this.colFile.FillWeight = 25f;
						break;
					}
					case ModTypes.BGM:
					{
						this.colPlay.Visible = true;
						this.colImage.Visible = false;
						this.colDesc.DividerWidth = 1;
						this.colFile.FillWeight = 40f;
						break;
					}
					case ModTypes.Video:
					{
						this.colPlay.Visible = true;
						this.colImage.Visible = false;
						this.colDesc.DividerWidth = 1;
						this.colFile.FillWeight = 40f;
						break;
					}
				}
				if (this.DesignMode)
				{
					return;
				}
				this.gridView.Rows.Clear();
				foreach (ModPack modPack in 
					from m in GlobalData.ModPacksManager
					where !m.IsDummy
					select m)
				{
					this.AddAllFiles(modPack);
				}
			}
		}

		static ModsSelector()
		{
			ModsSelector.DisabledItem = new ModPack(gPatcher.Localization.Text.Window_ManagerForm_ComboBox_Disabled, true);
		}

		public ModsSelector()
		{
			this.InitializeComponent();
			if (this.DesignMode)
			{
				return;
			}
			GlobalData.ModPacksManager.ModPackAdded += new ModPacksManager.ModPackAddedEventHandler(this.ModPacksManagerModPackManagerAdded);
			GlobalData.ModPacksManager.ModPackRemoved += new ModPacksManager.ModPackRemovedEventHandler(this.ModPacksManagerModPackManagerRemoved);
		}

		protected void AddAllFiles(ModPack pack)
		{
			IEnumerable<ModFile> files = 
				from f in pack.Files
				where f.ModType == this.ModType
				select f;
			foreach (ModFile file in files)
			{
				this.AddFile(file, pack);
			}
			pack.FileAdded -= new FileAddedEventHandler(this.ModPack_FileAdded);
			pack.FileRemoved -= new FileRemovedEventHandler(this.ModPack_FileRemoved);
			pack.FileAdded += new FileAddedEventHandler(this.ModPack_FileAdded);
			pack.FileRemoved += new FileRemovedEventHandler(this.ModPack_FileRemoved);
		}

		private void AddFile(ModFile file, ModPack pack)
		{
			DataGridViewRow item = this.gridView.Rows.Cast<DataGridViewRow>().FirstOrDefault<DataGridViewRow>((DataGridViewRow r) => ModsSelector.GetFileName(r).Equals(file.FileName));
			if (item == null)
			{
				DataGridViewRowCollection rows = this.gridView.Rows;
				object[] objArray = new object[] { (file.Blocked ? Images16px.Warning : new Bitmap(1, 1)), file.FileName, file.Description, null, ModsSelector.DisabledItem };
				int num = rows.Add(objArray);
				item = this.gridView.Rows[num];
				if (file.Blocked)
				{
					DataGridViewImageCell imageCell = ModsSelector.GetImageCell(item);
					imageCell.ToolTipText = string.Format(gPatcher.Localization.Text.ModsSelector_FileBlockedInServers, ModsSelector.GetFileName(item), file.BlockedServers);
				}
				ModsSelector.GetPlayCell(item).ValueIsIcon = false;
				DataGridViewComboBoxCell comboBoxCell = ModsSelector.GetComboBoxCell(item);
				SortableBindingList<ModPack> sortableBindingList = new SortableBindingList<ModPack>()
				{
					ModsSelector.DisabledItem,
					pack
				};
				sortableBindingList.Sort<ModPack>((ModPack t) => t);
				comboBoxCell.DisplayMember = "Name";
				comboBoxCell.ValueMember = "Self";
				comboBoxCell.ValueType = typeof(ModPack);
				comboBoxCell.DataSource = sortableBindingList;
				this.Sort();
			}
			else
			{
				BindingList<ModPack> dataSource = ModsSelector.GetDataSource(item);
				if (!dataSource.Contains(pack))
				{
					dataSource.AddSorted<ModPack>(pack);
					return;
				}
			}
		}

		public void CancelChanges()
		{
			foreach (DataGridViewRow dataGridViewRow in this.gridView.Rows.Cast<DataGridViewRow>())
			{
				bool flag = true;
				using (IEnumerator<Preset> enumerator = (
					from u in GlobalData.PresetsManager
					where ModsSelector.GetFileName(dataGridViewRow).Equals(u.File.FileName, StringComparison.OrdinalIgnoreCase)
					select u).GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						this.SetModOn(dataGridViewRow, enumerator.Current.Mod);
						flag = false;
					}
				}
				if (!flag)
				{
					continue;
				}
				this.DisableRow(dataGridViewRow);
			}
		}

		public void ClearSelection()
		{
			this.gridView.ClearSelection();
		}

		public void DisableAll()
		{
			foreach (DataGridViewRow row in (IEnumerable)this.gridView.Rows)
			{
				this.DisableRow(row);
			}
		}

		public void DisableModOnAllRows(string modName)
		{
			IEnumerable<DataGridViewComboBoxCell> rows = 
				from DataGridViewRow row in this.gridView.Rows
				select ModsSelector.GetComboBoxCell(row) into comboBox
				let dataSource = ModsSelector.GetDataSource(comboBox)
				where dataSource.Any<ModPack>((ModPack p) => p.Name == modName)
				select comboBox;
			foreach (DataGridViewComboBoxCell disabledItem in rows)
			{
				disabledItem.Value = ModsSelector.DisabledItem;
			}
		}

		protected void DisableRow(DataGridViewRow row)
		{
			ModsSelector.GetComboBoxCell(row).Value = ModsSelector.DisabledItem;
		}

		public void DisableSelectedRows()
		{
			foreach (DataGridViewRow dataGridViewRow in 
				from DataGridViewRow row in this.gridView.SelectedRows
				where row.Index != -1
				select row)
			{
				this.DisableRow(dataGridViewRow);
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
			this.gridView.RowFilter = (DataGridViewRow row) => (new string[] { ModsSelector.GetFileName(row), ModsSelector.GetDescription(row) }).Any<string>((string s) => s.Contains(filter, true));
		}

		private static DataGridViewComboBoxCell GetComboBoxCell(DataGridViewRow row)
		{
			return (DataGridViewComboBoxCell)row.Cells[4];
		}

		private static BindingList<ModPack> GetDataSource(DataGridViewComboBoxCell comboBox)
		{
			return (BindingList<ModPack>)comboBox.DataSource;
		}

		private static BindingList<ModPack> GetDataSource(DataGridViewRow row)
		{
			return ModsSelector.GetDataSource(ModsSelector.GetComboBoxCell(row));
		}

		private static string GetDescription(DataGridViewRow row)
		{
			return ModsSelector.GetString(row, ModsSelector.Columns.Description);
		}

		private static string GetFileName(DataGridViewRow row)
		{
			return ModsSelector.GetString(row, ModsSelector.Columns.Name);
		}

		private static DataGridViewImageCell GetImageCell(DataGridViewRow row)
		{
			return (DataGridViewImageCell)row.Cells[0];
		}

		private static DataGridViewImageCell GetPlayCell(DataGridViewRow row)
		{
			return (DataGridViewImageCell)row.Cells[3];
		}

		public IEnumerable<Preset> GetPreset()
		{
			return (
				from DataGridViewRow row in this.gridView.Rows
				where !this.IsDisabled(row)
				let m = ModsSelector.GetSelectedPack(row)
				where !m.IsDummy
				select new Preset()
				{
					File = m.Files.FindByName(ModsSelector.GetFileName(row)),
					Mod = GlobalData.ModPacksManager.FindByName(ModsSelector.GetSelectedName(row))
				}).ToList<Preset>();
		}

		private static string GetSelectedName(DataGridViewRow row)
		{
			return ModsSelector.GetSelectedPack(row).Name;
		}

		private static ModPack GetSelectedPack(DataGridViewRow row)
		{
			return (ModPack)row.Cells[4].Value;
		}

		private static string GetString(DataGridViewRow row, ModsSelector.Columns column)
		{
			return row.Cells[(int)column].Value.ToString();
		}

		private void gridView_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex != 3 || e.RowIndex == -1)
			{
				return;
			}
			DataGridViewRow item = this.gridView.Rows[e.RowIndex];
			string fileName = ModsSelector.GetFileName(item);
			ModPack selectedPack = ModsSelector.GetSelectedPack(item);
			try
			{
				if (!this.IsDisabled(item))
				{
					ModPlayer.Play(selectedPack.Files.FindByName(fileName));
				}
				else
				{
					ModPlayer.Play(GlobalData.ElswordFilesInfo.FindByName(fileName));
				}
			}
			catch (Exception exception)
			{
				MsgBox.Error(exception.Message);
			}
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ModsSelector));
			DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
			DataGridViewCellStyle control = new DataGridViewCellStyle();
			this.gridView = new MyDataGridView();
			this.colImage = new DataGridViewImageColumn();
			this.colFile = new DataGridViewTextBoxColumn();
			this.colDesc = new DataGridViewTextBoxColumn();
			this.colPlay = new DataGridViewImageColumn();
			this.colMod = new DataGridViewComboBoxColumn();
			((ISupportInitialize)this.gridView).BeginInit();
			base.SuspendLayout();
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
			DataGridViewColumn[] dataGridViewColumnArray = new DataGridViewColumn[] { this.colImage, this.colFile, this.colDesc, this.colPlay, this.colMod };
			columns.AddRange(dataGridViewColumnArray);
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
			this.colImage.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			this.colImage.Frozen = true;
			componentResourceManager.ApplyResources(this.colImage, "colImage");
			this.colImage.Name = "colImage";
			this.colFile.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			this.colFile.FillWeight = 40f;
			componentResourceManager.ApplyResources(this.colFile, "colFile");
			this.colFile.Name = "colFile";
			this.colFile.ReadOnly = true;
			this.colFile.Resizable = DataGridViewTriState.True;
			this.colDesc.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			this.colDesc.FillWeight = 60f;
			componentResourceManager.ApplyResources(this.colDesc, "colDesc");
			this.colDesc.Name = "colDesc";
			this.colDesc.ReadOnly = true;
			this.colDesc.Resizable = DataGridViewTriState.True;
			this.colPlay.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			control.Alignment = DataGridViewContentAlignment.MiddleCenter;
			control.BackColor = SystemColors.Control;
			control.NullValue = componentResourceManager.GetObject("dataGridViewCellStyle1.NullValue");
			this.colPlay.DefaultCellStyle = control;
			componentResourceManager.ApplyResources(this.colPlay, "colPlay");
			this.colPlay.Image = (Image)componentResourceManager.GetObject("colPlay.Image");
			this.colPlay.Name = "colPlay";
			this.colMod.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			this.colMod.FillWeight = 40f;
			componentResourceManager.ApplyResources(this.colMod, "colMod");
			this.colMod.Name = "colMod";
			this.colMod.SortMode = DataGridViewColumnSortMode.Automatic;
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.gridView);
			base.Name = "ModsSelector";
			((ISupportInitialize)this.gridView).EndInit();
			base.ResumeLayout(false);
		}

		public bool IsDisabled(DataGridViewRow row)
		{
			return ModsSelector.GetComboBoxCell(row).Value == ModsSelector.DisabledItem;
		}

		protected void ModPack_FileAdded(object sender, ModFile file)
		{
			if (file.ModType != this.ModType)
			{
				return;
			}
			base.BeginInvoke(new MethodInvoker(() => this.AddFile(file, (ModPack)sender)));
		}

		protected void ModPack_FileRemoved(object sender, ModFile file)
		{
			if (file.ModType != this.ModType)
			{
				return;
			}
			base.BeginInvoke(new MethodInvoker(() => this.Remove(file.FileName, (ModPack)sender)));
		}

		protected void ModPacksManagerModPackManagerAdded(object sender, ModPack pack)
		{
			if (!pack.IsDummy)
			{
				base.BeginInvoke(new MethodInvoker(() => this.AddAllFiles(pack)));
			}
		}

		protected void ModPacksManagerModPackManagerRemoved(object sender, ModPack pack)
		{
			if (!pack.IsDummy)
			{
				base.BeginInvoke(new MethodInvoker(() => this.RemoveAll(pack)));
			}
		}

		private void Remove(string fileName, ModPack pack)
		{
			DataGridViewRow dataGridViewRow = this.gridView.Rows.Cast<DataGridViewRow>().FirstOrDefault<DataGridViewRow>((DataGridViewRow r) => ModsSelector.GetFileName(r).Equals(fileName));
			if (dataGridViewRow == null)
			{
				return;
			}
			BindingList<ModPack> dataSource = ModsSelector.GetDataSource(dataGridViewRow);
			dataSource.Remove(pack);
			if (dataSource.Count == 1)
			{
				this.gridView.Rows.Remove(dataGridViewRow);
			}
		}

		private void RemoveAll(ModPack pack)
		{
			foreach (string str in 
				from f in pack.Files
				select f.FileName)
			{
				this.Remove(str, pack);
			}
		}

		public void SetModOn(DataGridViewRow row, ModPack mod)
		{
			DataGridViewComboBoxCell comboBoxCell = ModsSelector.GetComboBoxCell(row);
			if (ModsSelector.GetDataSource(comboBoxCell).Any<ModPack>((ModPack m) => m == mod))
			{
				comboBoxCell.Value = mod;
			}
		}

		public void SetModOnAllRows(ModPack mod)
		{
			foreach (DataGridViewRow row in (IEnumerable)this.gridView.Rows)
			{
				this.SetModOn(row, mod);
			}
		}

		public void SetModOnSelectedRows(ModPack mod)
		{
			foreach (DataGridViewRow dataGridViewRow in 
				from DataGridViewRow row in this.gridView.SelectedRows
				where row.Index != -1
				select row)
			{
				this.SetModOn(dataGridViewRow, mod);
			}
		}

		private void Sort()
		{
			MyDataGridView myDataGridView = this.gridView;
			DataGridViewColumn sortedColumn = this.gridView.SortedColumn ?? this.gridView.Columns[1];
			object sortOrder = this.gridView.SortOrder;
			object[] objArray = new object[] { SortOrder.Ascending, SortOrder.None };
			myDataGridView.Sort(sortedColumn, (sortOrder.IsAny(objArray) ? ListSortDirection.Ascending : ListSortDirection.Descending));
		}

		public event DataGridViewCellEventHandler CellValueChanged
		{
			add
			{
				this.gridView.CellValueChanged += value;
			}
			remove
			{
				this.gridView.CellValueChanged -= value;
			}
		}

		public event MouseEventHandler MouseUp
		{
			add
			{
				this.gridView.MouseUp += value;
			}
			remove
			{
				this.gridView.MouseUp -= value;
			}
		}

		public enum Columns
		{
			Icon,
			Name,
			Description,
			Open,
			ModPacks
		}
	}
}