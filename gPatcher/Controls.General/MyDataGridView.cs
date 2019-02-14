using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace gPatcher.Controls.General
{
	public class MyDataGridView : DataGridView
	{
		private bool _flag = true;

		private Func<DataGridViewRow, bool> _rowFilter;

		private MyDataGridView.SortingData _sortData;

		public bool HasSortData
		{
			get
			{
				return this._sortData != null;
			}
		}

		public Func<DataGridViewRow, bool> RowFilter
		{
			get
			{
				return this._rowFilter;
			}
			set
			{
				this._rowFilter = value;
				this.FilterRows();
			}
		}

		public MyDataGridView()
		{
		}

		public void BackupSorting()
		{
			if (base.Rows.Count > 0)
			{
				this._sortData = new MyDataGridView.SortingData(this);
			}
		}

		private void FilterRow(DataGridViewRow row)
		{
			if (!row.IsNewRow)
			{
				row.Visible = (this._rowFilter == null ? true : this._rowFilter(row));
			}
		}

		private void FilterRows()
		{
			if (base.Rows.Count == 0 || base.DataSource != null)
			{
				return;
			}
			DataGridViewRow[] item = new DataGridViewRow[base.RowCount];
			for (int i = 0; i < (int)item.Length; i++)
			{
				item[i] = base.Rows[i];
			}
			base.Rows.Clear();
			DataGridViewRow[] dataGridViewRowArray = item;
			for (int j = 0; j < (int)dataGridViewRowArray.Length; j++)
			{
				this.FilterRow(dataGridViewRowArray[j]);
			}
			this._flag = false;
			base.Rows.AddRange(item);
			this._flag = true;
		}

		protected override void OnCellValueChanged(DataGridViewCellEventArgs e)
		{
			if (e.RowIndex != -1)
			{
				this.FilterRow(base.Rows[e.RowIndex]);
			}
			base.OnCellValueChanged(e);
		}

		protected override void OnRowsAdded(DataGridViewRowsAddedEventArgs e)
		{
			if (this._flag && this._rowFilter != null)
			{
				for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
				{
					this.FilterRow(base.Rows[i]);
				}
			}
			base.OnRowsAdded(e);
		}

		public void RestoreSorting()
		{
			if (base.Rows.Count == 0 || this._sortData == null)
			{
				return;
			}
			if (this._sortData.SortedColumn != null)
			{
				this.Sort(this._sortData.SortedColumn, (this._sortData.SortOrder == System.Windows.Forms.SortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending));
			}
			if (this._sortData.Point != new Point(-1, -1) && base.Rows.Count > this._sortData.Point.Y)
			{
				base.CurrentCell = base[this._sortData.Point.X, this._sortData.Point.Y];
			}
			foreach (int num in 
				from index in this._sortData.SelectedIndexes
				where base.Rows.Count > index
				select index)
			{
				base.Rows[num].Selected = true;
			}
			this._sortData = null;
		}

		private class SortingData
		{
			public readonly List<int> SelectedIndexes;

			public readonly DataGridViewColumn SortedColumn;

			public readonly System.Windows.Forms.SortOrder SortOrder;

			public Point Point;

			public SortingData(DataGridView gridView)
			{
				this.SortOrder = gridView.SortOrder;
				this.SortedColumn = gridView.SortedColumn;
				this.Point = (gridView.CurrentCell != null ? new Point(gridView.CurrentCell.ColumnIndex, gridView.CurrentCell.RowIndex) : new Point(-1, -1));
				foreach (DataGridViewRow selectedRow in gridView.SelectedRows)
				{
					this.SelectedIndexes.Add(selectedRow.Index);
				}
			}
		}
	}
}