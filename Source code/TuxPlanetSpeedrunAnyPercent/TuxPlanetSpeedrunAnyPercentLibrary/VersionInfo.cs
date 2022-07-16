
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using System.Collections.Generic;

	public class VersionInfo
	{
		public string Version { get; private set; }
		public string AlphanumericVersionGuid { get; private set; }

		private VersionInfo(
			string version,
			string alphanumericVersionGuid)
		{
			this.Version = version;
			this.AlphanumericVersionGuid = alphanumericVersionGuid;
		}

		public static VersionInfo GetVersionInfo()
		{
			List<VersionInfo> versionHistory = GetVersionHistory();

			return versionHistory[versionHistory.Count - 1];
		}

		public static List<VersionInfo> GetVersionHistory()
		{
			List<VersionInfo> list = new List<VersionInfo>();

			list.Add(new VersionInfo(version: "1.00", alphanumericVersionGuid: "1204514613893229"));

			return list;
		}
	}
}
