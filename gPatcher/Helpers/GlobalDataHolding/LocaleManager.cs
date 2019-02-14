using gPatcher.Localization;
using gPatcher.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

namespace gPatcher.Helpers.GlobalDataHolding
{
	public class LocaleManager
	{
		public readonly ReadOnlyCollection<CultureInfo> SupportedCultures;

		private readonly List<CultureInfo> _supportedCultures = new List<CultureInfo>();

		public LocaleManager()
		{
			this.SupportedCultures = this._supportedCultures.AsReadOnly();
			ResourceManager resourceManager = new ResourceManager(typeof(Text));
			foreach (CultureInfo cultureInfo in 
				from c in (IEnumerable<CultureInfo>)CultureInfo.GetCultures(CultureTypes.AllCultures)
				where !string.IsNullOrWhiteSpace(c.Name)
				select c)
			{
				if (resourceManager.GetResourceSet(cultureInfo, true, false) == null)
				{
					continue;
				}
				this._supportedCultures.Add(cultureInfo);
			}
		}

		public void SetCulture(string cultureName)
		{
			CultureInfo cultureInfo = new CultureInfo(cultureName);
			Type type = typeof(CultureInfo);
			try
			{
				object[] objArray = new object[] { cultureInfo };
				type.InvokeMember("s_userDefaultCulture", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, cultureInfo, objArray);
				object[] objArray1 = new object[] { cultureInfo };
				type.InvokeMember("s_userDefaultUICulture", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, cultureInfo, objArray1);
			}
			catch
			{
			}
			try
			{
				object[] objArray2 = new object[] { cultureInfo };
				type.InvokeMember("m_userDefaultCulture", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, cultureInfo, objArray2);
				object[] objArray3 = new object[] { cultureInfo };
				type.InvokeMember("m_userDefaultUICulture", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, cultureInfo, objArray3);
			}
			catch
			{
			}
		}

		public void SetDefaultCulture()
		{
			if (!string.IsNullOrWhiteSpace(Settings.Default.Culture))
			{
				this.SetCulture(Settings.Default.Culture);
			}
		}
	}
}