
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	public class ArrayUtil
	{
		public static bool[][] CopyBoolArray(bool[][] array)
		{
			bool[][] newArray = new bool[array.Length][];

			for (int i = 0; i < newArray.Length; i++)
			{
				newArray[i] = new bool[array[i].Length];
				for (int j = 0; j < newArray[i].Length; j++)
					newArray[i][j] = array[i][j];
			}

			return newArray;
		}

		public static bool[][] EmptyBoolArray(int length1, int length2)
		{
			bool[][] array = new bool[length1][];

			for (int i = 0; i < length1; i++)
			{
				array[i] = new bool[length2];
				for (int j = 0; j < length2; j++)
					array[i][j] = false;
			}

			return array;
		}
	}
}
