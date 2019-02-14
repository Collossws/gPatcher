using Ginpo;
using Ginpo.Extensions;
using Ginpo.gPatcher;
using gPatcher.Helpers;
using gPatcher.Localization;
using gPatcher.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace gPatcher.Helpers.GlobalDataHolding
{
	public class ElswordFilesManager : IEnumerable<ElswordFile>, IEnumerable
	{
		private readonly List<ElswordFile> _embeddedFiles;

		private readonly List<ElswordFile> _mergedList = new List<ElswordFile>();

		public ElswordFilesManager()
		{
			this._embeddedFiles = SerializationEx.DeserializeString<List<ElswordFile>>(Text.Data_ElswordFileInfo);
			this._embeddedFiles.Sort<ElswordFile, string>((ElswordFile f) => f.FileName);
			this.ReloadFiles();
		}

		public ElswordFile FindByName(string fileName)
		{
			List<ElswordFile> elswordFiles = this._mergedList;
			Func<ElswordFile, string> func = (ElswordFile f) => f.FileName;
			StringComparer ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
			int num = elswordFiles.BinarySearch<ElswordFile, string>(func, fileName, new Comparison<string>(ordinalIgnoreCase.Compare));
			if (num < 0)
			{
				return null;
			}
			return this._mergedList[num];
		}

		public IEnumerator<ElswordFile> GetEnumerator()
		{
			return this._mergedList.GetEnumerator();
		}

		private IEnumerable<ElswordFile> LoadFilesFrom(string directory, string searchPattern)
		{
			return Directory.EnumerateFiles(directory, searchPattern).Select<string, string>(new Func<string, string>(Path.GetFileName)).Where<string>((string fileName) => this._embeddedFiles.FirstOrDefaultSorted<ElswordFile, string>((ElswordFile f) => f.FileName, fileName) == null).Select<string, ElswordFile>((string fileName) => new ElswordFile()
			{
				FileName = fileName
			});
		}

		public void ReloadFiles()
		{
			this._mergedList.Clear();
			this._mergedList.AddRange(this._embeddedFiles);
			if (!Paths.Elsword.IsValidElswordDir(Settings.Default.ElswordDirectory))
			{
				return;
			}
			this._mergedList.AddRange(this.LoadFilesFrom(Paths.Elsword.Data, "*.kom"));
			this._mergedList.AddRange(this.LoadFilesFrom(Paths.Elsword.Music, "*.ogg"));
			this._mergedList.AddRange(this.LoadFilesFrom(Paths.Elsword.Movie, "*.avi"));
			this._mergedList.Sort<ElswordFile, string>((ElswordFile f) => f.FileName);
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}