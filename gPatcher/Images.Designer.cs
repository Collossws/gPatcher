using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace gPatcher
{
	[CompilerGenerated]
	[DebuggerNonUserCode]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	internal class Images
	{
		private static System.Resources.ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		internal static Bitmap AboutBackgroundImage
		{
			get
			{
				return (Bitmap)Images.ResourceManager.GetObject("AboutBackgroundImage", Images.resourceCulture);
			}
		}

		internal static Bitmap AboutImage
		{
			get
			{
				return (Bitmap)Images.ResourceManager.GetObject("AboutImage", Images.resourceCulture);
			}
		}

		internal static Bitmap Aisha
		{
			get
			{
				return (Bitmap)Images.ResourceManager.GetObject("Aisha", Images.resourceCulture);
			}
		}

		internal static Bitmap Chung
		{
			get
			{
				return (Bitmap)Images.ResourceManager.GetObject("Chung", Images.resourceCulture);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Images.resourceCulture;
			}
			set
			{
				Images.resourceCulture = value;
			}
		}

		internal static Bitmap Elesis
		{
			get
			{
				return (Bitmap)Images.ResourceManager.GetObject("Elesis", Images.resourceCulture);
			}
		}

		internal static Bitmap Elsword
		{
			get
			{
				return (Bitmap)Images.ResourceManager.GetObject("Elsword", Images.resourceCulture);
			}
		}

		internal static Bitmap Raven
		{
			get
			{
				return (Bitmap)Images.ResourceManager.GetObject("Raven", Images.resourceCulture);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static System.Resources.ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(Images.resourceMan, null))
				{
					Images.resourceMan = new System.Resources.ResourceManager("gPatcher.Images", typeof(Images).Assembly);
				}
				return Images.resourceMan;
			}
		}

		internal Images()
		{
		}
	}
}