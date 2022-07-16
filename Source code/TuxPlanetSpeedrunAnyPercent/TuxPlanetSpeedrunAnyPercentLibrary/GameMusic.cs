
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using System;

	public enum GameMusic
	{
		Airship2,
		Theme
	}

	public static class GameMusicUtil
	{
		public static string GetMusicFilename(this GameMusic music)
		{
			switch (music)
			{
				case GameMusic.Airship2: return "JasonLavallee/airship_2.wav";
				case GameMusic.Theme: return "wansti/theme.wav";
				default: throw new Exception();
			}
		}

		// From 0 to 100 (both inclusive)
		public static int GetMusicVolume(this GameMusic music)
		{
			switch (music)
			{
				case GameMusic.Airship2: return 40;
				case GameMusic.Theme: return 10;
				default: throw new Exception();
			}
		}
	}
}
