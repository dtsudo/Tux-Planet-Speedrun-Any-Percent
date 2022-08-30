
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System.Collections.Generic;

	public class Achievements
	{
		public static HashSet<string> GetCompletedAchievements(int numCompletedLevels)
		{
			HashSet<string> completedAchievements = new HashSet<string>();

			if (numCompletedLevels >= 1)
				completedAchievements.Add("completed_1_level");

			for (int i = 2; i <= numCompletedLevels; i++)
			{
				string completedLevelsString = "completed_" + i.ToStringCultureInvariant() + "_levels";
				completedAchievements.Add(completedLevelsString);
			}

			return completedAchievements;
		}
	}
}
