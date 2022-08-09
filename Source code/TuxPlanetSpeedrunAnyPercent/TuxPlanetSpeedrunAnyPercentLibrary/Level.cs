
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public enum Level
	{
		Level1,
		Level2,
		Level3,
		Level4,
		Level5
	}

	public static class LevelUtil
	{
		public static bool IsLastLevel(this Level level)
		{
			switch (level)
			{
				case Level.Level1:
				case Level.Level2:
				case Level.Level3:
				case Level.Level4:
					return false;
				case Level.Level5:
					return true;
				default:
					throw new Exception();
			}
		}

		public static string GetLevelName(this Level level)
		{
			switch (level)
			{
				case Level.Level1: return "Level 1";
				case Level.Level2: return "Level 2";
				case Level.Level3: return "Level 3";
				case Level.Level4: return "Level 4";
				case Level.Level5: return "Level 5";
				default: throw new Exception();
			}
		}

		public static int ToSerializableInt(this Level level)
		{
			switch (level)
			{
				case Level.Level1: return 1;
				case Level.Level2: return 2;
				case Level.Level3: return 3;
				case Level.Level4: return 4;
				case Level.Level5: return 5;
				default: throw new Exception();
			}
		}

		public static Level FromSerializableInt(int i)
		{
			Level? level = TryFromSerializableInt(i);

			if (level == null)
				throw new Exception();

			return level.Value;
		}

		public static Level? TryFromSerializableInt(int i)
		{
			switch (i)
			{
				case 1: return Level.Level1;
				case 2: return Level.Level2;
				case 3: return Level.Level3;
				case 4: return Level.Level4;
				case 5: return Level.Level5;
				default: return null;
			}
		}
	}
}
