
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;

	public class BackgroundUtil
	{
		public static IBackground GetRandomBackground(IDTDeterministicRandom random)
		{
			switch (random.NextInt(2))
			{
				case 0: return new Background_Ocean();
				case 1: return new Background_Arctis();
				default: throw new Exception();
			}
		}
	}
}
