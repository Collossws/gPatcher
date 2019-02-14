using Ginpo.Controls;
using gPatcher.Localization;
using System;
using System.Windows.Forms;

namespace gPatcher.Helpers
{
	public static class MsgBox
	{
		public static void Error(string message, Exception ex)
		{
			if (ex == null)
			{
				MsgBox.Error(message);
				return;
			}
			string stackTrace = MsgBox.GetStackTrace(ex);
			MyMessageBox.Show(message, string.Concat(ex.Message, "\r\n", stackTrace), Text.MsgBoxCaption_Error, MyMessageBox.MessageBoxTypes.Error, MyMessageBox.MessageBoxButtons.Ok);
		}

		public static void Error(string message)
		{
			MessageBox.Show(message, Text.MsgBoxCaption_Error, MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}

		private static string GetStackTrace(Exception ex)
		{
			string stackTrace = ex.StackTrace;
			if (string.IsNullOrWhiteSpace(stackTrace) && ex.InnerException != null)
			{
				stackTrace = ex.InnerException.StackTrace;
			}
			return stackTrace;
		}

		public static void Information(string message)
		{
			MessageBox.Show(message, Text.MsgBoxCaption_Information, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}

		public static DialogResult Question(string message)
		{
			return MessageBox.Show(message, Text.MsgBoxCaption_Question, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
		}

		public static DialogResult Question(string message, string caption)
		{
			return MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
		}

		public static void Success(string message)
		{
			MessageBox.Show(message, Text.MsgBoxCaption_Success, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}
	}
}