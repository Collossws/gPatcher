using Ginpo.gPatcher;
using System;

namespace gPatcher.Components.UpdateChecking
{
	public class UpdateCheckFinishedArgs : EventArgs
	{
		public string NewVersionUrl;

		public VoicePack[] OutdatedVoicePacks;

		public UpdateCheckFinishedArgs(string newVersionUrl, VoicePack[] outdatedVoicePacks)
		{
			this.NewVersionUrl = newVersionUrl;
			this.OutdatedVoicePacks = outdatedVoicePacks;
		}
	}
}