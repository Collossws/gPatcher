using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace gPatcher.Helpers.PatchInfoReading
{
	public class PatchInfoEntry
	{
		public readonly int Checksum;

		public readonly uint? ChecksumCount;

		public readonly uint? ChecksumSize;

		public readonly string FileName;

		public readonly int FileTime;

		public readonly long Size;

		public PatchInfoEntry(string fileName, long size, int checksum, int fileTime, uint? checksumSize, uint? checksumCount)
		{
			this.FileName = fileName;
			this.Size = size;
			this.Checksum = checksum;
			this.FileTime = fileTime;
			this.ChecksumSize = checksumSize;
			this.ChecksumCount = checksumCount;
		}

		public bool Equals(string filePath)
		{
			bool flag;
			using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
			{
				long length = fileStream.Length;
				if (this.Size == length)
				{
					if (filePath.EndsWith(".kom"))
					{
						BinaryReader binaryReader = new BinaryReader(fileStream);
						fileStream.Seek((long)60, SeekOrigin.Begin);
						int num = binaryReader.ReadInt32();
						if (this.FileTime == num)
						{
							fileStream.Seek((long)64, SeekOrigin.Begin);
							int num1 = binaryReader.ReadInt32();
							if (this.Checksum == num1)
							{
								fileStream.Seek((long)68, SeekOrigin.Begin);
								int num2 = binaryReader.ReadInt32();
								uint? checksumSize = this.ChecksumSize;
								long num3 = (long)num2;
								if (((ulong)checksumSize.GetValueOrDefault() != num3 ? false : checksumSize.HasValue))
								{
									MemoryStream memoryStream = new MemoryStream(binaryReader.ReadBytes(num2));
									XDocument xDocument = XDocument.Load(memoryStream);
									int num4 = xDocument.Descendants("Files").Elements<XElement>("File").Count<XElement>();
									uint? checksumCount = this.ChecksumCount;
									long num5 = (long)num4;
									if (((ulong)checksumCount.GetValueOrDefault() != num5 ? true : !checksumCount.HasValue))
									{
										flag = false;
										return flag;
									}
								}
								else
								{
									flag = false;
									return flag;
								}
							}
							else
							{
								flag = false;
								return flag;
							}
						}
						else
						{
							flag = false;
							return flag;
						}
					}
					else
					{
						int num6 = Adler32.ComputeChecksum(fileStream);
						if (this.Checksum != num6)
						{
							flag = false;
							return flag;
						}
					}
					return true;
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}
	}
}