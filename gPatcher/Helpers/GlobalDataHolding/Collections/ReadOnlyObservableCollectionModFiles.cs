using Ginpo.Extensions;
using gPatcher.Helpers.GlobalDataHolding;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace gPatcher.Helpers.GlobalDataHolding.Collections
{
	public class ReadOnlyObservableCollectionModFiles : ReadOnlyObservableCollection<ModFile>
	{
		public ReadOnlyObservableCollectionModFiles(ObservableCollection<ModFile> list) : base(list)
		{
		}

		public ModFile FindByName(string name)
		{
			return this.FirstOrDefaultSorted<ModFile, string>((ModFile f) => f.FileName, name, StringComparer.OrdinalIgnoreCase);
		}
	}
}