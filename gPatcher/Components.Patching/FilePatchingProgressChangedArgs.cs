using System;

namespace gPatcher.Components.Patching
{
	public class FilePatchingProgressChangedArgs : EventArgs
	{
		public readonly BackgroundFilePatcher.States CurrentState;

		public readonly int ProgressPercentage;

		public FilePatchingProgressChangedArgs(BackgroundFilePatcher.States currentState, int progressPercentage)
		{
			this.CurrentState = currentState;
			this.ProgressPercentage = progressPercentage;
		}
	}
}