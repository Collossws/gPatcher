using Ginpo;
using gPatcher.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace gPatcher.Helpers
{
	public static class Paths
	{
		public static string LocalApplicationData;

		public static string UserMods
		{
			get;
			private set;
		}

		static Paths()
		{
			Paths.LocalApplicationData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.CompanyName, Application.ProductName);
			Paths.UserMods = Path.Combine(Paths.LocalApplicationData, "usrmods.xml");
		}

		public static class Elsword
		{
			public static string Backup
			{
				get
				{
					return Paths.Elsword.CreateDirectory(Path.Combine(Paths.Elsword.Root, "backup"));
				}
			}

			public static string ClientExe
			{
				get
				{
					return Path.Combine(Paths.Elsword.Data, "x2.exe");
				}
			}

			public static string Data
			{
				get
				{
					return Paths.Elsword.CreateDirectory(Path.Combine(Paths.Elsword.Root, "data"));
				}
			}

			public static string LauncherExe
			{
				get
				{
					return Path.Combine(Paths.Elsword.Root, "elsword.exe");
				}
			}

			private static IEnumerable<string> LogFiles
			{
				get
				{
					string str = Path.Combine(Paths.Elsword.Data, "Crash_ScreenShot.jpg");
					string str1 = Path.Combine(Paths.Elsword.Data, "log.htm");
					return new string[] { str, str1 };
				}
			}

			public static string Media
			{
				get
				{
					return Paths.Elsword.CreateDirectory(Path.Combine(Paths.Elsword.Data, "media"));
				}
			}

			public static string Movie
			{
				get
				{
					return Paths.Elsword.CreateDirectory(Path.Combine(Paths.Elsword.Data, "movie"));
				}
			}

			public static string Music
			{
				get
				{
					return Paths.Elsword.CreateDirectory(Path.Combine(Paths.Elsword.Data, "music"));
				}
			}

			public static string Root
			{
				get
				{
					return Settings.Default.ElswordDirectory;
				}
			}

			public static void BlockLogs()
			{
				foreach (string str in Paths.Elsword.LogFiles.Where<string>((string logFile) => {
					if (!PathEx.Exists(logFile))
					{
						return true;
					}
					return !PathEx.IsDirectory(logFile);
				}))
				{
					FileEx.Delete(str);
					Directory.CreateDirectory(str).Attributes = FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.System | FileAttributes.Directory;
				}
			}

			private static string CreateDirectory(string path)
			{
				if (!Paths.Elsword.IsValidElswordDir(Paths.Elsword.Root))
				{
					return string.Empty;
				}
				Directory.CreateDirectory(path);
				return path;
			}

			public static bool IsValidElswordDir(string dir)
			{
				string str = Path.Combine(dir, "elsword.exe");
				string str1 = Path.Combine(dir, "data");
				if (!File.Exists(str))
				{
					return false;
				}
				return Directory.Exists(str1);
			}

			public static Process RunClient()
			{
				Process process = new Process();
				ProcessStartInfo processStartInfo = new ProcessStartInfo()
				{
					FileName = Paths.Elsword.ClientExe,
					Arguments = string.Concat(" ", Settings.Default.X2Args),
					WorkingDirectory = Paths.Elsword.Data
				};
				process.StartInfo = processStartInfo;
				Process process1 = process;
				process1.Start();
				return process1;
			}

			public static void RunLauncher()
			{
				Process process = new Process();
				ProcessStartInfo processStartInfo = new ProcessStartInfo()
				{
					FileName = Paths.Elsword.LauncherExe,
					WorkingDirectory = Paths.Elsword.Root
				};
				process.StartInfo = processStartInfo;
				process.Start();
			}

			public static void UnblockLogs()
			{
				foreach (string str in Paths.Elsword.LogFiles.Where<string>((string logFile) => {
					if (!PathEx.Exists(logFile))
					{
						return true;
					}
					return !PathEx.IsFile(logFile);
				}))
				{
					PathEx.RemoveAttributes(str);
					DirectoryEx.Delete(str);
				}
			}
		}

		public static class Main
		{
			public static string Cache
			{
				get
				{
					string pathRoot = Path.GetPathRoot(Paths.Elsword.Root);
					return Paths.Main.CreateDirectory(Path.Combine(pathRoot, "gPatcher cache"));
				}
			}

			public static string Packs
			{
				get
				{
					return Paths.Main.CreateDirectory(Path.Combine(Paths.Main.Root, "packs"));
				}
			}

			public static string Root
			{
				get
				{
					return AppDomain.CurrentDomain.BaseDirectory;
				}
			}

			private static string CreateDirectory(string path)
			{
				Directory.CreateDirectory(path);
				return path;
			}
		}
	}
}