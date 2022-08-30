
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class OverworldGameMap
	{
		public enum TileType
		{
			Path,
			Level,
			NonPath
		}

		public class Tile
		{
			public Tile(TileType type, Level? level)
			{
				this.Type = type;
				this.Level = level;
			}

			public TileType Type { get; private set; }
			public Level? Level { get; private set; }
		}

		public IReadOnlyList<IReadOnlyList<Tile>> Tilemap { get; private set; }

		public Tuple<int, int> StartingLocation { get; private set; }

		public static IReadOnlyList<Level> GetWaterLevels()
		{
			return new List<Level>();
		}

		public static IReadOnlyList<Level> GetMountainLevels()
		{
			return new List<Level>() { Level.Level4 };
		}

		public static IReadOnlyList<Level> GetFortressLevels()
		{
			return new List<Level>() { Level.Level6 };
		}

		public static OverworldGameMap GenerateOverworldGameMap(int windowWidth, int windowHeight, IDTDeterministicRandom random)
		{
			Tile[][] tilemap = OverworldGameMapGenerator.GenerateOverworldGameMapTileArray(
				windowWidth: windowWidth, 
				windowHeight: windowHeight,
				waterLevels: GetWaterLevels(),
				mountainLevels: GetMountainLevels(),
				fortressLevels: GetFortressLevels(),
				random: random);

			return new OverworldGameMap(
				tilemap: tilemap);
		}

		private OverworldGameMap(Tile[][] tilemap)
		{
			List<IReadOnlyList<Tile>> list = new List<IReadOnlyList<Tile>>();

			for (int i = 0; i < tilemap.Length; i++)
				list.Add(new List<Tile>(tilemap[i]));

			this.Tilemap = list;
			this.StartingLocation = null;

			for (int i = 0; i < tilemap.Length; i++)
			{
				for (int j = 0; j < tilemap[i].Length; j++)
				{
					if (tilemap[i][j].Type == TileType.Level && tilemap[i][j].Level.Value == Level.Level1)
					{
						this.StartingLocation = new Tuple<int, int>(i, j);
						break;
					}
				}

				if (this.StartingLocation != null)
					break;
			}

			if (this.StartingLocation == null)
				throw new Exception();
		}
	}
}
