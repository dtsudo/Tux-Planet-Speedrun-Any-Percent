
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;

	public class CameraState
	{
		public const int CUTSCENE_CAMERA_SPEED = 500;

		public int X { get; private set; }
		public int Y { get; private set; }

		private CameraState(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		public static CameraState GetCameraState(int x, int y)
		{
			return new CameraState(x: x, y: y);
		}

		public static CameraState SmoothCameraState(
			int currentCameraX,
			int currentCameraY,
			int destinationCameraX, 
			int destinationCameraY,
			int elapsedMicrosPerFrame,
			int cameraSpeedInPixelsPerSecond)
		{
			int maxDistancePerFrame = (elapsedMicrosPerFrame >> 3) * cameraSpeedInPixelsPerSecond / (125 * 1000);

			if (maxDistancePerFrame <= 0)
				maxDistancePerFrame = 1;

			int newX;
			int newY;

			if (Math.Abs(currentCameraX - destinationCameraX) <= maxDistancePerFrame)
				newX = destinationCameraX;
			else
				newX = currentCameraX < destinationCameraX ? (currentCameraX + maxDistancePerFrame) : (currentCameraX - maxDistancePerFrame);

			if (Math.Abs(currentCameraY - destinationCameraY) <= maxDistancePerFrame)
				newY = destinationCameraY;
			else
				newY = currentCameraY < destinationCameraY ? (currentCameraY + maxDistancePerFrame) : (currentCameraY - maxDistancePerFrame);

			return new CameraState(x: newX, y: newY);
		}

		public static CameraState SmoothCameraState(
			CameraState currentCamera,
			int destinationCameraX,
			int destinationCameraY,
			int elapsedMicrosPerFrame,
			int cameraSpeedInPixelsPerSecond)
		{
			return SmoothCameraState(
				currentCameraX: currentCamera.X,
				currentCameraY: currentCamera.Y,
				destinationCameraX: destinationCameraX,
				destinationCameraY: destinationCameraY,
				elapsedMicrosPerFrame: elapsedMicrosPerFrame,
				cameraSpeedInPixelsPerSecond: cameraSpeedInPixelsPerSecond);
		}

		public static CameraState SmoothCameraState(
			CameraState currentCamera,
			CameraState destinationCamera,
			int elapsedMicrosPerFrame,
			int cameraSpeedInPixelsPerSecond)
		{
			return SmoothCameraState(
				currentCameraX: currentCamera.X,
				currentCameraY: currentCamera.Y,
				destinationCameraX: destinationCamera.X,
				destinationCameraY: destinationCamera.Y,
				elapsedMicrosPerFrame: elapsedMicrosPerFrame,
				cameraSpeedInPixelsPerSecond: cameraSpeedInPixelsPerSecond);
		}
	}
}
