
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public interface ILevelConfiguration
	{
		IBackground GetBackground();

		ITilemap GetTilemap(int? tuxX, int? tuxY, int windowWidth, int windowHeight);
	}
}
