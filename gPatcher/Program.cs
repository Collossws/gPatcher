using gPatcher.Components.Patching;
using gPatcher.Forms.Main;
using gPatcher.Helpers;
using gPatcher.Helpers.GlobalDataHolding;
using gPatcher.Localization;
using gPatcher.Properties;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.IO;
using System.Windows.Forms;

namespace gPatcher
{
	internal class Program : WindowsFormsApplicationBase
	{
		private static Program _instance;

		public Program()
		{
			base.MainForm = new FrHome();
			base.IsSingleInstance = true;
		}

		private static bool AgreeToBeta()
		{
			if (Settings.Default.BetaFirstLaunch && MsgBox.Question(Text.BetaWarning) == DialogResult.No)
			{
				return false;
			}
			Settings.Default.BetaFirstLaunch = false;
			return true;
		}

		[STAThread]
		public static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			SettingsHelper.Load();
			Directory.SetCurrentDirectory(Paths.Main.Root);
			try
			{
				GlobalData.Init();
			}
			catch (Exception exception)
			{
				MsgBox.Error(Text.Error_GlobalDataLoadFailed, exception);
				return;
			}
			BackgroundFilePatcher.CheckForBackups();
			if (Program.AgreeToBeta())
			{
				Program._instance = new Program();
				Program._instance.Run(args);
				SettingsHelper.Save();
				return;
			}
		}

		protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
		{
			((FrHome)Program._instance.MainForm).Restore();
		}

		public static void Restart()
		{
			Program._instance.IsSingleInstance = false;
			Application.Restart();
		}
	}
}