using Ginpo.gPatcher;
using gPatcher.Helpers.GlobalDataHolding;
using gPatcher.Localization;
using System;
using System.Diagnostics;
using System.IO;

namespace gPatcher.Helpers
{
	public static class ModPlayer
	{
		public static void Play(ModFile f)
		{
			if (!f.Enabled)
			{
				ModPlayer.Play(f.Base);
				return;
			}
			ModPlayer.Play(f.FilePath);
		}

		public static void Play(ElswordFile f)
		{
			string fileName = f.FileName;
			ModPlayer.Play(Path.Combine((fileName.EndsWith(".ogg") ? Paths.Elsword.Music : Paths.Elsword.Movie), fileName));
		}

		public static void Play(string path)
		{
			if (path.EndsWith(".kom") || path.EndsWith(".ess"))
			{
				throw new Exception(Text.Error_UnsupportedMediaFile);
			}
			if (!File.Exists(path))
			{
				throw new FileNotFoundException(string.Format(Text.Error_FileNotFound, path));
			}
			Process.Start(path);
		}
	}
}