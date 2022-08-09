
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;

	public class CameraStateProcessing
	{
		public static CameraState ComputeCameraState(
			int tuxXMibi,
			int tuxYMibi,
			Tuple<int, int> tuxTeleportStartingLocation,
			int? tuxTeleportInProgressElapsedMicros,
			ITilemap tilemap,
			int windowWidth,
			int windowHeight)
		{
			if (tuxTeleportInProgressElapsedMicros != null)
			{
				long deltaX = tuxXMibi - tuxTeleportStartingLocation.Item1;
				long deltaY = tuxYMibi - tuxTeleportStartingLocation.Item2;

				tuxXMibi = (int) (tuxTeleportStartingLocation.Item1 + deltaX * tuxTeleportInProgressElapsedMicros.Value / TuxState.TELEPORT_DURATION);
				tuxYMibi = (int) (tuxTeleportStartingLocation.Item2 + deltaY * tuxTeleportInProgressElapsedMicros.Value / TuxState.TELEPORT_DURATION);
			}

			int x = tuxXMibi >> 10;
			int y = tuxYMibi >> 10;

			int halfWindowWidth = windowWidth >> 1;
			int halfWindowHeight = windowHeight >> 1;

			int maxX = tilemap.GetWidth() - halfWindowWidth;
			int maxY = tilemap.GetHeight() - halfWindowHeight;

			if (x > maxX)
				x = maxX;
			if (x < halfWindowWidth)
				x = halfWindowWidth;

			if (y > maxY)
				y = maxY;

			if (y < halfWindowHeight)
				y = halfWindowHeight;

			return CameraState.GetCameraState(x: x, y: y);
		}
	}
}
