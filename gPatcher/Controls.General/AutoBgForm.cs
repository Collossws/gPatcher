using Ginpo.Controls;
using gPatcher.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace gPatcher.Controls.General
{
	public class AutoBgForm : MyForm
	{
		[DefaultValue(null)]
		public Image AutoBackgroundImage
		{
			get;
			set;
		}

		public AutoBgForm()
		{
		}

		internal virtual void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "EnableBackgroundImages")
			{
				this.UpdateImage();
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.UpdateImage();
			AutoBgForm autoBgForm = this;
			Settings.Default.PropertyChanged += new PropertyChangedEventHandler(autoBgForm.Default_PropertyChanged);
		}

		protected void UpdateImage()
		{
			Image autoBackgroundImage;
			if (Settings.Default.EnableBackgroundImages)
			{
				autoBackgroundImage = this.AutoBackgroundImage;
			}
			else
			{
				autoBackgroundImage = null;
			}
			this.BackgroundImage = autoBackgroundImage;
		}
	}
}