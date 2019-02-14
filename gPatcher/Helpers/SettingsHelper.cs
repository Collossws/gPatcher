using gPatcher.Helpers.GlobalDataHolding;
using gPatcher.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;

namespace gPatcher.Helpers
{
	public static class SettingsHelper
	{
		private static void DefaultOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "ElswordDirectory")
			{
				return;
			}
			foreach (ModPack modPacksManager in GlobalData.ModPacksManager)
			{
				modPacksManager.ReloadFiles();
			}
			GlobalData.ElswordFilesInfo.ReloadFiles();
		}

		public static void Load()
		{
			if (!Settings.Default.UpgradeRequired)
			{
				return;
			}
			Settings.Default.Upgrade();
			Settings.Default.UpgradeRequired = false;
			Settings.Default.PropertyChanged += new PropertyChangedEventHandler(SettingsHelper.DefaultOnPropertyChanged);
		}

		public static void Save()
		{
			Settings.Default.Save();
		}
	}
}