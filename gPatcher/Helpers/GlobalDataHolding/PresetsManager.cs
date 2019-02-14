using Ginpo.Extensions;
using gPatcher.Helpers;
using gPatcher.Helpers.GlobalDataHolding.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace gPatcher.Helpers.GlobalDataHolding
{
	public class PresetsManager : ObservableCollection<Preset>
	{
		public PresetsManager()
		{
		}

		public static PresetsManager Load()
		{
			XDocument xDocument;
			PresetsManager presetsManager;
			PresetsManager presetsManager1 = new PresetsManager();
			if (!File.Exists(Paths.UserMods))
			{
				return presetsManager1;
			}
			try
			{
				xDocument = XDocument.Load(Paths.UserMods);
				goto Label0;
			}
			catch
			{
				presetsManager = presetsManager1;
			}
			return presetsManager;
		Label0:
			XElement xElement = xDocument.Element("Presets");
			if (xElement == null)
			{
				return presetsManager1;
			}
			var collection = 
				from p in xElement.Descendants()
				select new { File = p.Attribute("File").Value, Mod = p.Attribute("Mod").Value };
			presetsManager1.AddRange<Preset>((
				from p in collection
				select new { p = p, modPack = GlobalData.ModPacksManager.FindByName(p.Mod) }).Where((argument0) => {
				if (argument0.modPack == null)
				{
					return false;
				}
				return !argument0.modPack.IsDummy;
			}).Select((argument1) => new { <>h__TransparentIdentifier0 = argument1, elsFile = argument1.modPack.Files.FindByName(argument1.p.File) }).Where((argument2) => argument2.elsFile != null).Select((argument3) => new Preset()
			{
				File = argument3.elsFile,
				Mod = argument3.<>h__TransparentIdentifier0.modPack
			}));
			return presetsManager1;
		}

		private void Mod_Deleted(object sender, EventArgs e)
		{
			this.Remove<Preset>((Preset p) => p.Mod == sender);
		}

		private void Mod_FileRemoved(object sender, ModFile file)
		{
			this.Remove<Preset>((Preset p) => p.File == file);
		}

		private void Mod_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Name")
			{
				this.Save();
			}
		}

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (Preset newItem in e.NewItems)
				{
					newItem.Mod.PropertyChanged += new PropertyChangedEventHandler(this.Mod_OnPropertyChanged);
					newItem.Mod.FileRemoved += new FileRemovedEventHandler(this.Mod_FileRemoved);
					newItem.Mod.Deleted += new EventHandler(this.Mod_Deleted);
				}
			}
			if (e.OldItems != null)
			{
				foreach (Preset oldItem in e.OldItems)
				{
					oldItem.Mod.PropertyChanged -= new PropertyChangedEventHandler(this.Mod_OnPropertyChanged);
					oldItem.Mod.FileRemoved -= new FileRemovedEventHandler(this.Mod_FileRemoved);
					oldItem.Mod.Deleted -= new EventHandler(this.Mod_Deleted);
				}
			}
			base.OnCollectionChanged(e);
		}

		public void Save()
		{
			XDocument xDocument = new XDocument();
			XElement xElement = new XElement("Presets");
			foreach (Preset preset in this)
			{
				XName xName = "Preset";
				object[] xAttribute = new object[] { new XAttribute("File", preset.File.FileName), new XAttribute("Mod", preset.Mod.Name) };
				xElement.Add(new XElement(xName, xAttribute));
			}
			xDocument.Add(xElement);
			xDocument.Save(Paths.UserMods);
		}
	}
}