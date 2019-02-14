using gPatcher.Localization;
using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace gPatcher.Controls.General
{
	public sealed class ToggleButton : CheckBox
	{
		private readonly static Color ColorOff;

		private readonly static Color ColorOn;

		private readonly string _textOff = gPatcher.Localization.Text.ToggleButton_States_Off;

		private readonly string _textOn = gPatcher.Localization.Text.ToggleButton_States_On;

		public new System.Windows.Forms.Appearance Appearance
		{
			get;
			set;
		}

		public new Color BackColor
		{
			get;
			set;
		}

		public new string Text
		{
			get;
			set;
		}

		static ToggleButton()
		{
			ToggleButton.ColorOff = Color.FromArgb(255, 162, 164);
			ToggleButton.ColorOn = Color.FromArgb(171, 255, 162);
		}

		public ToggleButton()
		{
			base.Appearance = System.Windows.Forms.Appearance.Button;
			this.ApplyColors();
		}

		private void ApplyColors()
		{
			if (base.Checked)
			{
				base.BackColor = ToggleButton.ColorOn;
				base.Text = this._textOn;
				return;
			}
			base.BackColor = ToggleButton.ColorOff;
			base.Text = this._textOff;
		}

		protected override void OnCheckStateChanged(EventArgs e)
		{
			base.OnCheckStateChanged(e);
			this.ApplyColors();
		}

		public void Toggle()
		{
			base.Checked = !base.Checked;
		}
	}
}