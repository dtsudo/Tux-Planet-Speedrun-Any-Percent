
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public enum MapKey
	{
		Copper,
		Silver,
		Gold,
		Mythril
	}

	public static class MapKeyUtil
	{
		public static List<MapKey> GetOrderedListOfMapKeys()
		{
			List<MapKey> list = new List<MapKey>();

			foreach (MapKey mapKey in Enum.GetValues(typeof(MapKey)))
			{
				list.Add(mapKey);
			}

			list = list.Select(x => x.ToSerializableInt()).OrderBy(x => x).Select(x => FromSerializableInt(x)).ToList();

			return list;
		}

		public static GameImage GetGameImage(this MapKey mapKey)
		{
			switch (mapKey)
			{
				case MapKey.Copper: return GameImage.KeyCopper;
				case MapKey.Silver: return GameImage.KeySilver;
				case MapKey.Gold: return GameImage.KeyGold;
				case MapKey.Mythril: return GameImage.KeyMythril;
				default: throw new Exception();
			}
		}

		public static int ToSerializableInt(this MapKey mapKey)
		{
			switch (mapKey)
			{
				case MapKey.Copper: return 1;
				case MapKey.Silver: return 2;
				case MapKey.Gold: return 3;
				case MapKey.Mythril: return 4;
				default: throw new Exception();
			}
		}

		public static MapKey FromSerializableInt(int i)
		{
			switch (i)
			{
				case 1: return MapKey.Copper;
				case 2: return MapKey.Silver;
				case 3: return MapKey.Gold;
				case 4: return MapKey.Mythril;
				default: throw new DTDeserializationException();
			}
		}
	}
}
