
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	public class SpriteUtil
	{
		public static Sprite[][] CopySpriteArray(Sprite[][] array)
		{
			Sprite[][] newArray = new Sprite[array.Length][];

			for (int i = 0; i < newArray.Length; i++)
			{
				newArray[i] = new Sprite[array[i].Length];
				for (int j = 0; j < newArray[i].Length; j++)
					newArray[i][j] = array[i][j];
			}

			return newArray;
		}

		public static Sprite[][] EmptySpriteArray(int length1, int length2)
		{
			Sprite[][] array = new Sprite[length1][];

			for (int i = 0; i < length1; i++)
			{
				array[i] = new Sprite[length2];
				for (int j = 0; j < length2; j++)
					array[i][j] = null;
			}

			return array;
		}
	}
}
