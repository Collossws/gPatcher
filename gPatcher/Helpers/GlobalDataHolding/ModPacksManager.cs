using Ginpo;
using Ginpo.Extensions;
using gPatcher.Helpers;
using gPatcher.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace gPatcher.Helpers.GlobalDataHolding
{
	public class ModPacksManager : IListSource, IEnumerable<ModPack>, IEnumerable, IDisposable
	{
		private readonly FileSystemWatcher _directoryWatcher;

		private readonly SortableBindingList<ModPack> _modPacks = new SortableBindingList<ModPack>()
		{
			new ModPack(Text.Window_ManagerForm_ComboBox_DisableAll, true)
		};

		private bool _raiseEvents = true;

		public bool ContainsListCollection
		{
			get
			{
				return false;
			}
		}

		public ModPacksManager()
		{
			foreach (DirectoryInfo directoryInfo in (new DirectoryInfo(Paths.Main.Packs)).EnumerateDirectories())
			{
				this._modPacks.AddSorted<ModPack>(new ModPack(directoryInfo));
			}
			FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(Paths.Main.Packs)
			{
				IncludeSubdirectories = false,
				NotifyFilter = NotifyFilters.DirectoryName,
				EnableRaisingEvents = true
			};
			this._directoryWatcher = fileSystemWatcher;
			this._directoryWatcher.Created += new FileSystemEventHandler(this._directoryWatcher_Created);
			this._directoryWatcher.Deleted += new FileSystemEventHandler(this._directoryWatcher_Deleted);
			this._directoryWatcher.Renamed += new RenamedEventHandler(this._directoryWatcher_Renamed);
			this._modPacks.Sort<ModPack>((ModPack m) => m);
		}

		private void _directoryWatcher_Created(object sender, FileSystemEventArgs e)
		{
			this.Add(new ModPack(new DirectoryInfo(e.FullPath)));
		}

		private void _directoryWatcher_Deleted(object sender, FileSystemEventArgs e)
		{
			this.Remove(this.FindByName(e.Name));
		}

		private void _directoryWatcher_Renamed(object sender, RenamedEventArgs e)
		{
			ModPack name = this.FindByName(e.OldName);
			if (name == null)
			{
				this.Add(new ModPack(new DirectoryInfo(e.FullPath)));
				return;
			}
			name.Name = e.Name;
			this.DoWithoutRaisingEvents(() => this._modPacks.Sort<ModPack>(name));
		}

		private void Add(ModPack pack)
		{
			this._modPacks.AddSorted<ModPack>(pack);
			this.OnModPackAdded(this, pack);
		}

		public ModPack CreateNew(string name)
		{
			ModPack modPack;
			string str = Path.Combine(Paths.Main.Packs, name);
			Directory.CreateDirectory(str);
			while (true)
			{
				modPack = this.FindByName(name);
				if (modPack != null)
				{
					break;
				}
				Thread.Sleep(100);
			}
			return modPack;
		}

		public void Dispose()
		{
			this._directoryWatcher.Dispose();
		}

		private void DoWithoutRaisingEvents(Action a)
		{
			bool flag = this._raiseEvents;
			this._raiseEvents = false;
			a();
			this._raiseEvents = flag;
		}

		public ModPack FindByName(string name)
		{
			int num = this._modPacks.BinarySearch<ModPack>(new ModPack(name, false));
			if (num < 0)
			{
				return null;
			}
			return this._modPacks[num];
		}

		public IEnumerator<ModPack> GetEnumerator()
		{
			return this._modPacks.GetEnumerator();
		}

		public IList GetList()
		{
			return this._modPacks;
		}

		protected virtual void OnModPackAdded(object sender, ModPack pack)
		{
			if (this.ModPackAdded != null && this._raiseEvents)
			{
				this.ModPackAdded(this, pack);
			}
		}

		protected virtual void OnModPackRemoved(object sender, ModPack pack)
		{
			if (this.ModPackRemoved != null && this._raiseEvents)
			{
				this.ModPackRemoved(this, pack);
			}
		}

		private void Remove(ModPack pack)
		{
			this._modPacks.RemoveSorted<ModPack>(pack);
			pack.Dispose();
			this.OnModPackRemoved(this, pack);
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public event ModPacksManager.ModPackAddedEventHandler ModPackAdded;

		public event ModPacksManager.ModPackRemovedEventHandler ModPackRemoved;

		public delegate void ModPackAddedEventHandler(object sender, ModPack pack);

		public delegate void ModPackRemovedEventHandler(object sender, ModPack pack);
	}
}