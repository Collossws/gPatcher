using Ginpo;
using Ginpo.Extensions;
using Ginpo.gPatcher;
using gPatcher.Helpers;
using gPatcher.Helpers.GlobalDataHolding.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace gPatcher.Helpers.GlobalDataHolding
{
	public class ModPack : INotifyPropertyChanged, IDisposable, IComparable<ModPack>
	{
		public readonly ReadOnlyObservableCollectionModFiles Files;

		private readonly DirectoryInfo _dirInfo;

		private readonly ObservableCollection<ModFile> _files;

		private readonly FileSystemWatcher _fileSystemWatcher;

		private readonly bool _isDummy;

		private bool _disposed;

		private string _name;

		public string FullName
		{
			get
			{
				if (this._isDummy)
				{
					return this.Name;
				}
				return this._dirInfo.FullName;
			}
		}

		public bool IsDummy
		{
			get
			{
				return this._isDummy;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
			internal set
			{
				this._name = value;
				this.OnPropertyChanged(this, "Name");
			}
		}

		public ModPack Self
		{
			get
			{
				return this;
			}
		}

		private ModPack()
		{
			this._files = new ObservableCollection<ModFile>();
			this.Files = new ReadOnlyObservableCollectionModFiles(this._files);
		}

		public ModPack(DirectoryInfo dirInfo) : this()
		{
			if (!dirInfo.Exists)
			{
				throw new DirectoryNotFoundException("The specified directory does not exist.");
			}
			this._dirInfo = dirInfo;
			this.Name = this._dirInfo.Name;
			this.LoadFiles();
			FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(dirInfo.FullName)
			{
				IncludeSubdirectories = false,
				EnableRaisingEvents = true
			};
			this._fileSystemWatcher = fileSystemWatcher;
			this._files.CollectionChanged += new NotifyCollectionChangedEventHandler(this._files_CollectionChanged);
			this._fileSystemWatcher.Created += new FileSystemEventHandler(this._fileSystemWatcher_Created);
			this._fileSystemWatcher.Deleted += new FileSystemEventHandler(this._fileSystemWatcher_Deleted);
			this._fileSystemWatcher.Renamed += new RenamedEventHandler(this._fileSystemWatcher_Renamed);
		}

		public ModPack(string name, bool isDummy = true)
		{
			this.Name = name;
			this._isDummy = isDummy;
		}

		private void _files_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				foreach (ModFile oldItem in e.OldItems)
				{
					this.OnFileRemoved(this, oldItem);
				}
			}
			if (e.NewItems != null)
			{
				foreach (ModFile newItem in e.NewItems)
				{
					this.OnFileAdded(this, newItem);
				}
			}
		}

		private void _fileSystemWatcher_Created(object sender, FileSystemEventArgs e)
		{
			ElswordFile elswordFile = GlobalData.ElswordFilesInfo.FindByName(e.Name);
			if (elswordFile != null)
			{
				this.Add(new ModFile(this, elswordFile));
			}
		}

		private void _fileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
		{
			int num = this._files.BinarySearch<ModFile, string>((ModFile f) => f.FileName, e.Name, StringComparer.OrdinalIgnoreCase);
			if (num >= 0)
			{
				this._files.RemoveAt(num);
			}
		}

		private void _fileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
		{
			int modFile = this._files.BinarySearch<ModFile, string>((ModFile f) => f.FileName, e.OldName, StringComparer.OrdinalIgnoreCase);
			ElswordFile elswordFile = GlobalData.ElswordFilesInfo.FindByName(e.Name);
			bool flag = modFile >= 0;
			bool flag1 = elswordFile != null;
			if (flag && !flag1)
			{
				this._files.RemoveAt(modFile);
				return;
			}
			if (!flag && flag1)
			{
				this.Add(new ModFile(this, elswordFile));
				return;
			}
			if (flag)
			{
				this._files[modFile] = new ModFile(this, elswordFile);
			}
		}

		private void Add(ModFile f)
		{
			this._files.AddSorted<ModFile>(f, new Comparison<ModFile>(ModPack.CompareModFile));
		}

		private static int CompareModFile(ModFile x, ModFile y)
		{
			return string.Compare(x.FileName, y.FileName, StringComparison.OrdinalIgnoreCase);
		}

		public int CompareTo(ModPack other)
		{
			if (other == null)
			{
				return 1;
			}
			if (this._isDummy && other._isDummy)
			{
				return 0;
			}
			if (other._isDummy)
			{
				return 1;
			}
			if (this._isDummy)
			{
				return -1;
			}
			return this.Name.CompareTo(other.Name);
		}

		public void Delete()
		{
			this._dirInfo.Delete(true);
			this.Dispose();
			this.OnDeleted();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed && disposing)
			{
				this._fileSystemWatcher.Dispose();
				this.Deleted = null;
				this.FileAdded = null;
				this.FileRemoved = null;
			}
			this._disposed = true;
		}

		public override bool Equals(object obj)
		{
			ModPack modPack = obj as ModPack;
			if (modPack == null)
			{
				return false;
			}
			return this.CompareTo(modPack) == 0;
		}

		private IEnumerable<ModFile> GetFiles()
		{
			return 
				from f in (IEnumerable<FileInfo>)this._dirInfo.GetFiles(SearchCriterias.Patterns)
				let elsFile = GlobalData.ElswordFilesInfo.FindByName(f.Name)
				where elsFile != null
				select new ModFile(this, elsFile);
		}

		public override int GetHashCode()
		{
			int hashCode = 17;
			bool flag = this._isDummy;
			hashCode = hashCode * 11 + flag.GetHashCode();
			return hashCode * 11 + this.Name.GetHashCode();
		}

		private void LoadFiles()
		{
			IEnumerable<ModFile> files = null;
			int? nullable = null;
			Retry.Do(() => {
				if (!this._disposed)
				{
					files = this.GetFiles();
				}
			}, TimeSpan.FromSeconds(1), nullable);
			if (files == null)
			{
				return;
			}
			foreach (ModFile modFile in 
				from f in files
				where this.Files.FindByName(f.FileName) == null
				select f)
			{
				this.Add(modFile);
			}
		}

		protected void OnDeleted()
		{
			if (this.Deleted != null)
			{
				this.Deleted(this, EventArgs.Empty);
			}
		}

		protected void OnFileAdded(object sender, ModFile file)
		{
			if (this.FileAdded != null)
			{
				this.FileAdded(sender, file);
			}
		}

		protected void OnFileRemoved(object sender, ModFile file)
		{
			if (this.FileRemoved != null)
			{
				this.FileRemoved(sender, file);
			}
		}

		private void OnPropertyChanged(object sender, string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
			}
		}

		public void ReloadFiles()
		{
			for (int i = 0; i < this._files.Count; i++)
			{
				if (GlobalData.ElswordFilesInfo.FindByName(this._files[i].FileName) == null)
				{
					this._files.RemoveAt(i);
				}
			}
			this.LoadFiles();
		}

		public void Rename(string newName)
		{
			string str;
			if (this.Name.Equals(newName, StringComparison.OrdinalIgnoreCase))
			{
				return;
			}
			if (!this._isDummy)
			{
				str = (this._dirInfo.Parent != null ? this._dirInfo.Parent.FullName : string.Empty);
				this._dirInfo.MoveTo(Path.Combine(str, newName));
			}
		}

		public override string ToString()
		{
			return this.Name;
		}

		public event EventHandler Deleted;

		public event FileAddedEventHandler FileAdded;

		public event FileRemovedEventHandler FileRemoved;

		public event PropertyChangedEventHandler PropertyChanged;
	}
}