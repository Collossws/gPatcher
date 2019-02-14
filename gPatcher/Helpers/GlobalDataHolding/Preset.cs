using System;
using System.Runtime.CompilerServices;

namespace gPatcher.Helpers.GlobalDataHolding
{
	public struct Preset
	{
		public ModFile File
		{
			get;
			set;
		}

		public ModPack Mod
		{
			get;
			set;
		}
	}
}