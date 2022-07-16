
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	public class Sprite
	{
		public Sprite(
			GameImage image,
			int x,
			int y,
			int width,
			int height,
			int scalingFactorScaled)
		{
			this.Image = image;
			this.X = x;
			this.Y = y;
			this.Width = width;
			this.Height = height;
			this.ScalingFactorScaled = scalingFactorScaled;
		}

		public GameImage Image { get; private set; }
		public int X { get; private set; }
		public int Y { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public int ScalingFactorScaled { get; private set; }
	}
}
