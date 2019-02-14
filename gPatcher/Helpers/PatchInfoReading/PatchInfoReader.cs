using gPatcher.Components.General;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace gPatcher.Helpers.PatchInfoReading
{
	public class PatchInfoReader
	{
		public IEnumerable<PatchInfoEntry> Entries;

		public PatchInfoReader()
		{
		}

		public static PatchInfoReader Download(Uri uri)
		{
			PatchInfoReader patchInfoReader;
			using (MemoryStream memoryStream = new MemoryStream((new MyWebClient()).DownloadData(uri)))
			{
				patchInfoReader = PatchInfoReader.Load(XDocument.Load(memoryStream));
			}
			return patchInfoReader;
		}

		public static PatchInfoReader Download(string address)
		{
			PatchInfoReader patchInfoReader;
			using (MemoryStream memoryStream = new MemoryStream((new MyWebClient()).DownloadData(address)))
			{
				patchInfoReader = PatchInfoReader.Load(XDocument.Load(memoryStream));
			}
			return patchInfoReader;
		}

		public static PatchInfoReader Load(string fileName)
		{
			return PatchInfoReader.Load(XDocument.Load(fileName));
		}

		private static PatchInfoReader Load(XContainer xmlDocument)
		{
			PatchInfoReader patchInfoReader = new PatchInfoReader()
			{
				Entries = 
					from entry in xmlDocument.Descendants("Files").Elements<XElement>("File")
					select new PatchInfoEntry(Path.GetFileName(entry.Attribute("Name").Value), (long)entry.Attribute("Size"), Convert.ToInt32(entry.Attribute("Checksum").Value, 16), Convert.ToInt32(entry.Attribute("FileTime").Value, 16), (uint?)entry.Attribute("ChecksumSize"), (uint?)entry.Attribute("ChecksumCount"))
			};
			return patchInfoReader;
		}
	}
}