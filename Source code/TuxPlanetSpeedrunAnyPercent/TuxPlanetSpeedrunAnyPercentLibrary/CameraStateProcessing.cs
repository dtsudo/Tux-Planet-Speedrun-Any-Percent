
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;

	public class CameraStateProcessing
	{
		public static CameraState ComputeCameraState(
			int tuxXMibi,
			int tuxYMibi,
			ITilemap tilemap,
			int windowWidth,
			int windowHeight)
		{
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
