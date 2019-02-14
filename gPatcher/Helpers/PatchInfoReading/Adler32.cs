using System;
using System.IO;

namespace gPatcher.Helpers.PatchInfoReading
{
	public class Adler32
	{
		private const int Base = 65521;

		private const int NMax = 5552;

		public Adler32()
		{
		}

		public static int ComputeChecksum(int initial, byte[] data, int start, int length)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			uint num = (uint)(initial & 65535);
			uint num1 = (uint)(initial >> 16 & 65535);
			int num2 = start;
			int num3 = length;
			while (num3 > 0)
			{
				int num4 = (num3 < 5552 ? num3 : 5552);
				num3 = num3 - num4;
				for (int i = 0; i < num4; i++)
				{
					int num5 = num2;
					num2 = num5 + 1;
					num = num + data[num5];
					num1 = num1 + num;
				}
				num = num % 65521;
				num1 = num1 % 65521;
			}
			return (int)(num1 << 16 | num);
		}

		public static int ComputeChecksum(int initial, byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			return Adler32.ComputeChecksum(initial, data, 0, (int)data.Length);
		}

		public static int ComputeChecksum(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			byte[] numArray = new byte[8172];
			int num = 1;
			while (true)
			{
				int num1 = stream.Read(numArray, 0, (int)numArray.Length);
				int num2 = num1;
				if (num1 <= 0)
				{
					break;
				}
				num = Adler32.ComputeChecksum(num, numArray, 0, num2);
			}
			return num;
		}

		public static int ComputeChecksum(string path)
		{
			int num;
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				num = Adler32.ComputeChecksum(fileStream);
			}
			return num;
		}
	}
}