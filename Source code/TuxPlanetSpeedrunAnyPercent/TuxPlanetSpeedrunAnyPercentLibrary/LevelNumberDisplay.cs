
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;

	public class LevelNumberDisplay
	{
		private int levelNumber;
		private int elapsedMicros;

		private const int LEVEL_NUMBER_DISPLAY_DURATION = 3000 * 1000;

		private LevelNumberDisplay(int levelNumber, int elapsedMicros)
		{
			this.levelNumber = levelNumber;
			this.elapsedMicros = elapsedMicros;
		}

		public static LevelNumberDisplay GetLevelNumberDisplay(int levelNumber)
		{
			return new LevelNumberDisplay(levelNumber, 0);
		}

		public LevelNumberDisplay ProcessFrame(int elapsedMicrosPerFrame)
		{
			return new LevelNumberDisplay(
				levelNumber: this.levelNumber,
				elapsedMicros: Math.Min(this.elapsedMicros + elapsedMicrosPerFrame, LEVEL_NUMBER_DISPLAY_DURATION + 1));
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput, int windowWidth, int windowHeight)
		{
			if (this.elapsedMicros >= LEVEL_NUMBER_DISPLAY_DURATION)
				return;

			int alpha;

			if (this.elapsedMicros <= LEVEL_NUMBER_DISPLAY_DURATION / 2)
				alpha = 255;
			else
			{
				long amount = this.elapsedMicros - LEVEL_NUMBER_DISPLAY_DURATION / 2;
				alpha = (int) (255L - amount * 255L / (LEVEL_NUMBER_DISPLAY_DURATION / 2));
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

			string levelNumStr = this.levelNumber.ToStringCultureInvariant();
			displayOutput.DrawText(
				x: 50,
				y: windowHeight / 2 + 170,
				text: "Level " + levelNumStr,
				font: GameFont.DTSimpleFont32Pt,
				color: new DTColor(255, 255, 255, alpha));
		}
	}
}
