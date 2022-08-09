
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;

	public class LevelNameDisplay
	{
		private string levelName;
		private int elapsedMicros;

		private const int LEVEL_NAME_DISPLAY_DURATION = 3000 * 1000;

		private LevelNameDisplay(string levelName, int elapsedMicros)
		{
			this.levelName = levelName;
			this.elapsedMicros = elapsedMicros;
		}

		public static LevelNameDisplay GetLevelNameDisplay(string levelName)
		{
			return new LevelNameDisplay(levelName: levelName, elapsedMicros: 0);
		}

		public LevelNameDisplay ProcessFrame(int elapsedMicrosPerFrame)
		{
			return new LevelNameDisplay(
				levelName: this.levelName,
				elapsedMicros: Math.Min(this.elapsedMicros + elapsedMicrosPerFrame, LEVEL_NAME_DISPLAY_DURATION + 1));
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput, int windowWidth, int windowHeight)
		{
			if (this.elapsedMicros >= LEVEL_NAME_DISPLAY_DURATION)
				return;

			int alpha;

			if (this.elapsedMicros <= LEVEL_NAME_DISPLAY_DURATION / 2)
				alpha = 255;
			else
			{
				long amount = this.elapsedMicros - LEVEL_NAME_DISPLAY_DURATION / 2;
				alpha = (int) (255L - amount * 255L / (LEVEL_NAME_DISPLAY_DURATION / 2));
			}

			if (alpha < 0)
				alpha = 0;
			if (alpha > 255)
				alpha = 255;

			int backgroundAlpha = 128 * alpha / 255;

			if (backgroundAlpha < 0)
				backgroundAlpha = 0;
			if (backgroundAlpha > 255)
				backgroundAlpha = 255;

			displayOutput.DrawRectangle(
				x: 0,
				y: windowHeight / 2 + 100,
				width: windowWidth,
				height: 100,
				color: new DTColor(0, 0, 0, backgroundAlpha),
				fill: true);

			displayOutput.DrawText(
				x: 50,
				y: windowHeight / 2 + 170,
				text: this.levelName,
				font: GameFont.DTSimpleFont32Pt,
				color: new DTColor(255, 255, 255, alpha));
		}
	}
}
