
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public interface ILevelConfiguration
	{
		IBackground GetBackground();

		ITilemap GetTilemap(
			int? tuxX, 
			int? tuxY, 
			int windowWidth, 
			int windowHeight, 
			IReadOnlyList<string> levelFlags,
			MapKeyState mapKeyState);

		IReadOnlyDictionary<string, string> GetCustomLevelInfo();

		CameraState GetCameraState(
			int tuxXMibi,
			int tuxYMibi,
			Tuple<int, int> tuxTeleportStartingLocation,
			int? tuxTeleportInProgressElapsedMicros,
			ITilemap tilemap,
			int windowWidth,
			int windowHeight,
			IReadOnlyList<string> levelFlags);
	}
}
