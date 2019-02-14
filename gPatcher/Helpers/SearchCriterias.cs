using System;

namespace gPatcher.Helpers
{
	public class SearchCriterias
	{
		public readonly static string[] Patterns;

		public readonly static string ZipCriteria;

		static SearchCriterias()
		{
			string[] strArrays = new string[] { "*.kom", "general.ess", "*.avi", "*.ogg" };
			SearchCriterias.Patterns = strArrays;
			SearchCriterias.ZipCriteria = "name = *.kom OR name = general.ess OR name = *.avi OR name = *.ogg";
		}

		public SearchCriterias()
		{
		}
	}
}