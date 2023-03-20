
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public enum Level
	{
		Level1,
		Level2,
		Level3,
		Level4,
		Level5,
		Level6,
		Level7,
		Level8,
		Level9,
		Level10
	}

	public static class LevelUtil
	{
		public static bool IsLastLevel(this Level level)
		{
			switch (level)
			{
				case Level.Level1:
				case Level.Level2:
				case Level.Level3:
				case Level.Level4:
				case Level.Level5:
				case Level.Level6:
				case Level.Level7:
				case Level.Level8:
				case Level.Level9:
					return false;
				case Level.Level10:
					return true;
				default:
					throw new Exception();
			}
		}

		public static string GetLevelName(this Level level)
		{
			switch (level)
			{
				case Level.Level1: return "Level 1";
				case Level.Level2: return "Level 2";
				case Level.Level3: return "Level 3";
				case Level.Level4: return "Level 4";
				case Level.Level5: return "Level 5";
				case Level.Level6: return "Level 6";
				case Level.Level7: return "Level 7";
				case Level.Level8: return "Level 8";
				case Level.Level9: return "Level 9";
				case Level.Level10: return "Level 10";
				default: throw new Exception();
			}
		}

		public static int ToSerializableInt(this Level level)
		{
			switch (level)
			{
				case Level.Level1: return 1;
				case Level.Level2: return 2;
				case Level.Level3: return 3;
				case Level.Level4: return 4;
				case Level.Level5: return 5;
				case Level.Level6: return 6;
				case Level.Level7: return 7;
				case Level.Level8: return 8;
				case Level.Level9: return 9;
				case Level.Level10: return 10;
				default: throw new Exception();
			}
		}

		public static Level FromSerializableInt(int i)
		{
			Level? level = TryFromSerializableInt(i);

			if (level == null)
				throw new DTDeserializationException();

			return level.Value;
		}

		public static Level? TryFromSerializableInt(int i)
		{
			switch (i)
			{
				case 1: return Level.Level1;
				case 2: return Level.Level2;
				case 3: return Level.Level3;
				case 4: return Level.Level4;
				case 5: return Level.Level5;
				case 6: return Level.Level6;
				case 7: return Level.Level7;
				case 8: return Level.Level8;
				case 9: return Level.Level9;
				case 10: return Level.Level10;
				default: return null;
			}
		}

		public static void RenderLevelScreenshot(
			Level level,
			IDisplayOutput<GameImage, GameFont> displayOutput,
			int x,
			int y)
		{
			GameImage image;
			int imageX;
			int imageY;

			switch (level)
			{
				case Level.Level1:
					image = GameImage.Level1Screenshot;
					imageX = 75;
					imageY = 190;
					break;
				case Level.Level2:
					image = GameImage.Level2Screenshot;
					imageX = 75;
					imageY = 190;
					break;
				case Level.Level3:
					image = GameImage.Level3Screenshot;
					imageX = 35;
					imageY = 240;
					break;
				case Level.Level4:
					image = GameImage.Level4Screenshot;
					imageX = 150;
					imageY = 165;
					break;
				case Level.Level5:
					image = GameImage.Level5Screenshot;
					imageX = 150;
					imageY = 225;
					break;
				case Level.Level6:
					image = GameImage.Level6Screenshot;
					imageX = 150;
					imageY = 160;
					break;
				case Level.Level7:
					image = GameImage.Level7Screenshot;
					imageX = 100;
					imageY = 90;
					break;
				case Level.Level8:
					image = GameImage.Level8Screenshot;
					imageX = 0;
					imageY = 160;
					break;
				case Level.Level9:
					image = GameImage.Level9Screenshot;
					imageX = 150;
					imageY = 310;
					break;
				case Level.Level10:
					image = GameImage.Level10Screenshot;
					imageX = 50;
					imageY = 315;
					break;
				default:
					throw new Exception();
			}

			displayOutput.DrawImageRotatedClockwise(
				image: image,
				imageX: imageX,
				imageY: imageY,
				imageWidth: 850,
				imageHeight: 355,
				x: x,
				y: y,
				degreesScaled: 0,
				scalingFactorScaled: 128);
		}
	}
}
