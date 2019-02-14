using Ginpo.gPatcher;
using gPatcher.Helpers;
using gPatcher.Helpers.GlobalDataHolding;
using System;
using System.IO;

namespace gPatcher.Components.Patching
{
	public class PatchInfo
	{
		public readonly string BackupFile;

		public readonly string DestinationFile;

		public readonly string ModdedFile;

		public readonly string TempFile;

		public PatchInfo(Preset mod)
		{
			this.ModdedFile = mod.File.FilePath;
			this.TempFile = Path.Combine(Paths.Main.Cache, mod.File.FileName);
			switch (mod.File.ModType)
			{
				case ModTypes.General:
				{
					this.DestinationFile = Path.Combine(Paths.Elsword.Data, mod.File.FileName);
					this.BackupFile = Path.Combine(Paths.Elsword.Backup, mod.File.FileName);
					return;
				}
				case ModTypes.BGM:
				{
					this.DestinationFile = Path.Combine(Paths.Elsword.Media, mod.File.FileName);
					this.BackupFile = null;
					return;
				}
				case ModTypes.Video:
				{
					this.DestinationFile = Path.Combine(Paths.Elsword.Movie, mod.File.FileName);
					this.BackupFile = Path.Combine(Paths.Elsword.Backup, mod.File.FileName);
					return;
				}
				default:
				{
					this.DestinationFile = Path.Combine(Paths.Elsword.Media, mod.File.FileName);
					this.BackupFile = null;
					return;
				}
			}
		}
	}
}