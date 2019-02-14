using Ginpo;
using Ginpo.gPatcher;
using gPatcher.Localization;
using System;
using System.Collections.Generic;

namespace gPatcher.Helpers.GlobalDataHolding
{
	public static class GlobalData
	{
		public static ElswordFilesManager ElswordFilesInfo;

		public static gPatcher.Helpers.GlobalDataHolding.LocaleManager LocaleManager;

		public static gPatcher.Helpers.GlobalDataHolding.ModPacksManager ModPacksManager;

		public static gPatcher.Helpers.GlobalDataHolding.PresetsManager PresetsManager;

		public static IEnumerable<VoicePack> VoicePacks;

		public static void Init()
		{
			GlobalData.LocaleManager = new gPatcher.Helpers.GlobalDataHolding.LocaleManager();
			GlobalData.LocaleManager.SetDefaultCulture();
			GlobalData.ElswordFilesInfo = new ElswordFilesManager();
			GlobalData.VoicePacks = SerializationEx.DeserializeString<List<VoicePack>>(Text.Data_DownloadInfo);
			GlobalData.ModPacksManager = new gPatcher.Helpers.GlobalDataHolding.ModPacksManager();
			GlobalData.PresetsManager = gPatcher.Helpers.GlobalDataHolding.PresetsManager.Load();
		}
	}
}