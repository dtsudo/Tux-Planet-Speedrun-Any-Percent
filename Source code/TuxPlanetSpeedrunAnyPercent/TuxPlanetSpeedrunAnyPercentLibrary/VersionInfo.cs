
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
			list.Add(new VersionInfo(version: "1.01", alphanumericVersionGuid: "3012096945791874"));
			list.Add(new VersionInfo(version: "1.02", alphanumericVersionGuid: "7537950542756516"));
			list.Add(new VersionInfo(version: "1.03", alphanumericVersionGuid: "3031094705805517"));
			list.Add(new VersionInfo(version: "1.04", alphanumericVersionGuid: "5271794898295337"));
			list.Add(new VersionInfo(version: "1.05", alphanumericVersionGuid: "4956416045935750"));

			return list;
		}

		/// <summary>
		/// Returns a guid that doesn't change between versions, but is unique to this game
		/// and isn't used by other games.
		/// </summary>
		public static string GetGuidForGame()
		{
			return "1754720524504623";
		}
	}
}
