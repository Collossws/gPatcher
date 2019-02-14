using Ginpo;
using Ginpo.gPatcher;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace gPatcher.Helpers.GlobalDataHolding
{
	public class ModFile : INotifyPropertyChanged
	{
		public readonly ElswordFile Base;

		private readonly ModPack _modPack;

		private ModFile.Actions _action = ModFile.Actions.None;

		private string _newFilePath;

		private ModFile.Actions Action
		{
			get
			{
				return this._action;
			}
			set
			{
				ModFile.Actions action = this._action;
				this._action = value;
				if (action == this._action)
				{
					return;
				}
				this.OnPropertyChanged("Enabled");
			}
		}

		public bool Blocked
		{
			get
			{
				return this.Base.Blocked;
			}
		}

		public string BlockedServers
		{
			get
			{
				return this.Base.BlockedServers;
			}
		}

		public string Description
		{
			get
			{
				return this.Base.Description;
			}
		}

		public bool Enabled
		{
			get
			{
				if (this.Action == ModFile.Actions.Delete)
				{
					return false;
				}
				if (this.Action == ModFile.Actions.AddOrReplace)
				{
					return true;
				}
				return this._modPack.Files.Contains(this);
			}
		}

		public string FileName
		{
			get
			{
				return this.Base.FileName;
			}
		}

		public string FilePath
		{
			get
			{
				if (this.Action != ModFile.Actions.AddOrReplace)
				{
					return this.OriginalFilePath;
				}
				return this._newFilePath;
			}
		}

		public ModTypes ModType
		{
			get
			{
				return this.Base.ModType;
			}
		}

		private string OriginalFilePath
		{
			get
			{
				return Path.Combine(this._modPack.FullName, this.FileName);
			}
		}

		public ModFile(ModPack modPack, ElswordFile file)
		{
			this.Base = file;
			this._modPack = modPack;
		}

		public void Cancel()
		{
			this.Action = ModFile.Actions.None;
			this._newFilePath = null;
		}

		public void Delete()
		{
			this.Action = ModFile.Actions.Delete;
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler propertyChangedEventHandler = this.PropertyChanged;
			if (propertyChangedEventHandler != null)
			{
				propertyChangedEventHandler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public void Replace(string newFilePath)
		{
			this._newFilePath = newFilePath;
			this.Action = ModFile.Actions.AddOrReplace;
		}

		public void SaveChanges()
		{
			if (!this.Enabled)
			{
				FileEx.Delete(this.FilePath);
				return;
			}
			if (this.Action != ModFile.Actions.AddOrReplace || !File.Exists(this._newFilePath))
			{
				return;
			}
			File.Copy(this._newFilePath, this.OriginalFilePath, true);
			this.Cancel();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private enum Actions
		{
			AddOrReplace,
			Delete,
			None
		}
	}
}