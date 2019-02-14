using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace gPatcher.Properties
{
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
	internal sealed class Settings : ApplicationSettingsBase
	{
		private static Settings defaultInstance;

		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		[UserScopedSetting]
		public bool BetaFirstLaunch
		{
			get
			{
				return (bool)this["BetaFirstLaunch"];
			}
			set
			{
				this["BetaFirstLaunch"] = value;
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("False")]
		[UserScopedSetting]
		public bool BlockLogs
		{
			get
			{
				return (bool)this["BlockLogs"];
			}
			set
			{
				this["BlockLogs"] = value;
			}
		}

		[ApplicationScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool CheckForProgramUpdates
		{
			get
			{
				return (bool)this["CheckForProgramUpdates"];
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("")]
		[UserScopedSetting]
		public string Culture
		{
			get
			{
				return (string)this["Culture"];
			}
			set
			{
				this["Culture"] = value;
			}
		}

		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("")]
		[UserScopedSetting]
		public string ElswordDirectory
		{
			get
			{
				return (string)this["ElswordDirectory"];
			}
			set
			{
				this["ElswordDirectory"] = value;
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		[UserScopedSetting]
		public bool EnableBackgroundImages
		{
			get
			{
				return (bool)this["EnableBackgroundImages"];
			}
			set
			{
				this["EnableBackgroundImages"] = value;
			}
		}

		[ApplicationScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("False")]
		public bool IgnoreBetaReleases
		{
			get
			{
				return (bool)this["IgnoreBetaReleases"];
			}
		}

		[ApplicationScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool IsBetaRelease
		{
			get
			{
				return (bool)this["IsBetaRelease"];
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		[UserScopedSetting]
		public bool IsTrayIconFirstTime
		{
			get
			{
				return (bool)this["IsTrayIconFirstTime"];
			}
			set
			{
				this["IsTrayIconFirstTime"] = value;
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		[UserScopedSetting]
		public bool ModsEnabled
		{
			get
			{
				return (bool)this["ModsEnabled"];
			}
			set
			{
				this["ModsEnabled"] = value;
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("False")]
		[UserScopedSetting]
		public bool SkipLauncher
		{
			get
			{
				return (bool)this["SkipLauncher"];
			}
			set
			{
				this["SkipLauncher"] = value;
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("False")]
		[UserScopedSetting]
		public bool StartHidden
		{
			get
			{
				return (bool)this["StartHidden"];
			}
			set
			{
				this["StartHidden"] = value;
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		[UserScopedSetting]
		public bool TrayIconEnabled
		{
			get
			{
				return (bool)this["TrayIconEnabled"];
			}
			set
			{
				this["TrayIconEnabled"] = value;
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("False")]
		[UserScopedSetting]
		public bool UpgradeRequired
		{
			get
			{
				return (bool)this["UpgradeRequired"];
			}
			set
			{
				this["UpgradeRequired"] = value;
			}
		}

		[ApplicationScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("https://gpatcher2.googlecode.com/svn/trunk/versionInfo.txt")]
		public string VersionInfoUrl
		{
			get
			{
				return (string)this["VersionInfoUrl"];
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("False")]
		[UserScopedSetting]
		public bool WebLoginNeeded
		{
			get
			{
				return (bool)this["WebLoginNeeded"];
			}
			set
			{
				this["WebLoginNeeded"] = value;
			}
		}

		[DebuggerNonUserCode]
		[DefaultSettingValue("")]
		[UserScopedSetting]
		public string X2Args
		{
			get
			{
				return (string)this["X2Args"];
			}
			set
			{
				this["X2Args"] = value;
			}
		}

		static Settings()
		{
			Settings.defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
		}

		public Settings()
		{
		}
	}
}