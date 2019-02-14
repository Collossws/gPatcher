using System;
using System.Drawing;
using System.Windows.Forms;

namespace gPatcher.Controls.General
{
	public sealed class GrayPanel : Panel
	{
		public GrayPanel()
		{
			this.BackColor = SystemColors.ControlLight;
			this.Dock = DockStyle.Bottom;
			base.Height = 42;
		}
	}
}