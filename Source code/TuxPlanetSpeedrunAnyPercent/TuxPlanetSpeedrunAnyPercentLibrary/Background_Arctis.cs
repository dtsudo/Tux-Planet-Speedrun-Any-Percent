
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;

	public class Background_Arctis : IBackground
	{
		public void Render(
			int cameraX,
			int cameraY,
			int windowWidth,
			int windowHeight,
			IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			int displacement = -cameraX / 8;

			GameImage image = GameImage.Arctis2;

			int imageWidth = displayOutput.GetWidth(image);
			int scalingFactor = 1;

			if (displacement >= 0)
				displacement = displacement % (imageWidth * scalingFactor);
			else
			{
				int multiple = (-displacement) / (imageWidth * scalingFactor);
				displacement = displacement + multiple * imageWidth * scalingFactor;
				while (displacement >= imageWidth * scalingFactor)
					displacement = displacement - imageWidth * scalingFactor;
				while (displacement < 0)
					displacement = displacement + imageWidth * scalingFactor;
			}

			displayOutput.DrawImageRotatedClockwise(
				image: image,
				x: displacement,
				y: 0,
				degreesScaled: 0,
				scalingFactorScaled: scalingFactor * 128);

			displayOutput.DrawImageRotatedClockwise(
				image: image,
				x: displacement - imageWidth * scalingFactor,
				y: 0,
				degreesScaled: 0,
				scalingFactorScaled: scalingFactor * 128);
		}
	}
}
