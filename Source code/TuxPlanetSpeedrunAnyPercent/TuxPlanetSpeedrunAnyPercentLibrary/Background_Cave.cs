
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;

	public class Background_Cave : IBackground
	{
		public void Render(
			int cameraX,
			int cameraY,
			int windowWidth,
			int windowHeight,
			IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			GameImage image = GameImage.CaveBackground;
			int scalingFactor = 3;

			int displacementX = -cameraX / 6;

			int imageWidth = displayOutput.GetWidth(image);

			if (displacementX >= 0)
				displacementX = displacementX % (imageWidth * scalingFactor);
			else
			{
				int multiple = (-displacementX) / (imageWidth * scalingFactor);
				displacementX = displacementX + multiple * imageWidth * scalingFactor;
				while (displacementX >= imageWidth * scalingFactor)
					displacementX = displacementX - imageWidth * scalingFactor;
				while (displacementX < 0)
					displacementX = displacementX + imageWidth * scalingFactor;
			}

			displayOutput.DrawImageRotatedClockwise(
				image: image,
				x: displacementX,
				y: 0,
				degreesScaled: 0,
				scalingFactorScaled: scalingFactor * 128);

			displayOutput.DrawImageRotatedClockwise(
				image: image,
				x: displacementX - imageWidth * scalingFactor,
				y: 0,
				degreesScaled: 0,
				scalingFactorScaled: scalingFactor * 128);
		}
	}
}
