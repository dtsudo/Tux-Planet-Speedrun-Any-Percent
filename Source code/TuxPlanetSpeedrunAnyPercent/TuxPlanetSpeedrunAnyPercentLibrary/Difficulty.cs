
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public enum Difficulty
	{
		Easy,
		Normal,
		Hard
	}

	public static class DifficultyUtil
	{
		public static int ToSerializableInt(this Difficulty difficulty)
		{
			switch (difficulty)
			{
				case Difficulty.Easy: return 1;
				case Difficulty.Normal: return 2;
				case Difficulty.Hard: return 3;
				default: throw new Exception();
			}
		}

		public static Difficulty? TryFromSerializableInt(int i)
		{
			switch (i)
			{
				case 1: return Difficulty.Easy;
				case 2: return Difficulty.Normal;
				case 3: return Difficulty.Hard;
				default: return null;
			}
		}

		public static Difficulty FromSerializableInt(int i)
		{
			Difficulty? difficulty = TryFromSerializableInt(i);

			if (difficulty == null)
				throw new DTDeserializationException();

			return difficulty.Value;
		}
	}
}
