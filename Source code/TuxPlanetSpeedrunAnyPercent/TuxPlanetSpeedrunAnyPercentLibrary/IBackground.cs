
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;

	public interface IBackground
	{
		void Render(
			int cameraX,
			int cameraY,
			int windowWidth,
			int windowHeight,
			IDisplayOutput<GameImage, GameFont> displayOutput);
	}
}
