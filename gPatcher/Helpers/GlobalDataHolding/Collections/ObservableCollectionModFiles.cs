using gPatcher.Helpers.GlobalDataHolding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace gPatcher.Helpers.GlobalDataHolding.Collections
{
	public class ObservableCollectionModFiles : ObservableCollection<ModFile>
	{
		public ObservableCollectionModFiles()
		{
		}

		public ObservableCollectionModFiles(List<ModFile> list) : base(list)
		{
		}

		public ObservableCollectionModFiles(IEnumerable<ModFile> collection) : base(collection)
		{
		}
	}
}