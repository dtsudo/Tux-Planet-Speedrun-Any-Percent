
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;

	public class ElapsedTimeUtil
	{
		public static string GetElapsedTimeString(int elapsedMillis)
		{
			string elapsedMinutes = (elapsedMillis / 1000 / 60).ToStringCultureInvariant();
			if (elapsedMinutes.Length < 2)
				elapsedMinutes = "0" + elapsedMinutes;
			string elapsedSeconds = ((elapsedMillis / 1000) % 60).ToStringCultureInvariant();
			if (elapsedSeconds.Length < 2)
				elapsedSeconds = "0" + elapsedSeconds;
			string elapsedCentiseconds = ((elapsedMillis % 1000) / 10).ToStringCultureInvariant();
			if (elapsedCentiseconds.Length < 2)
				elapsedCentiseconds = "0" + elapsedCentiseconds;

			return elapsedMinutes + ":" + elapsedSeconds + "." + elapsedCentiseconds;
		}
	}
}
