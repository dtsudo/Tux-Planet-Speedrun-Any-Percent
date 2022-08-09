
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class OverworldGameMapGenerator
	{
		public static OverworldGameMap.Tile[][] GenerateOverworldGameMapTileArray(
			int windowWidth,
			int windowHeight,
			IDTDeterministicRandom random)
		{
			List<Tuple<int, int>> path = new List<Tuple<int, int>>();

			path.Add(new Tuple<int, int>(0, 0));

			path = new List<Tuple<int, int>>(GeneratePathHelper(path: path, random: random));

			int? minX = null;
			int? minY = null;

			foreach (Tuple<int, int> tile in path)
			{
				if (minX == null || minX.Value > tile.Item1)
					minX = tile.Item1;
				if (minY == null || minY.Value > tile.Item2)
					minY = tile.Item2;
			}

			int padding = 2;

			path = path.Select(tile => new Tuple<int, int>(tile.Item1 - minX.Value + padding, tile.Item2 - minY.Value + padding)).ToList();

			int numberOfColumns = Math.Max(
				path.Select(tile => tile.Item1).Max() + 1 + padding,
				windowWidth / OverworldMap.TILE_WIDTH_IN_PIXELS + 1);
			int numberOfRows = Math.Max(
				path.Select(tile => tile.Item2).Max() + 1 + padding,
				windowHeight / OverworldMap.TILE_HEIGHT_IN_PIXELS + 1);

			OverworldGameMap.Tile[][] tilemap = new OverworldGameMap.Tile[numberOfColumns][];

			for (int i = 0; i < tilemap.Length; i++)
			{
				tilemap[i] = new OverworldGameMap.Tile[numberOfRows];

				for (int j = 0; j < tilemap[i].Length; j++)
					tilemap[i][j] = new OverworldGameMap.Tile(type: OverworldGameMap.TileType.NonPath, level: null);
			}

			for (int i = 0; i < path.Count; i++)
				tilemap[path[i].Item1][path[i].Item2] = new OverworldGameMap.Tile(type: OverworldGameMap.TileType.Path, level: null);

			int level1Index = 0;
			int level2Index = 10 + random.NextInt(4);

			int level5Index = 49;
			int level4Index = 35 + random.NextInt(4);

			int level3Index = (level2Index + level4Index) / 2 + random.NextInt(4) - 2;

			tilemap[path[level1Index].Item1][path[level1Index].Item2] = new OverworldGameMap.Tile(type: OverworldGameMap.TileType.Level, level: Level.Level1);
			tilemap[path[level2Index].Item1][path[level2Index].Item2] = new OverworldGameMap.Tile(type: OverworldGameMap.TileType.Level, level: Level.Level2);
			tilemap[path[level3Index].Item1][path[level3Index].Item2] = new OverworldGameMap.Tile(type: OverworldGameMap.TileType.Level, level: Level.Level3);
			tilemap[path[level4Index].Item1][path[level4Index].Item2] = new OverworldGameMap.Tile(type: OverworldGameMap.TileType.Level, level: Level.Level4);
			tilemap[path[level5Index].Item1][path[level5Index].Item2] = new OverworldGameMap.Tile(type: OverworldGameMap.TileType.Level, level: Level.Level5);

			return tilemap;
		}

		private static List<Tuple<int, int>> GeneratePathHelper(
			IReadOnlyList<Tuple<int, int>> path,
			IDTDeterministicRandom random)
		{
			if (path.Count == 50)
				return new List<Tuple<int, int>>(path);

			int x = path[path.Count - 1].Item1;
			int y = path[path.Count - 1].Item2;

			HashSet<Tuple<int, int>> occupiedSpaces = new HashSet<Tuple<int, int>>(path, new IntTupleEqualityComparer());

			List<Tuple<int, int>> potentialNextSteps = new List<Tuple<int, int>>()
			{
				new Tuple<int, int>(x - 1, y),
				new Tuple<int, int>(x + 1, y),
				new Tuple<int, int>(x, y - 1),
				new Tuple<int, int>(x, y + 1)
			};

			potentialNextSteps.Shuffle(random: random);

			foreach (Tuple<int, int> potentialNextStep in potentialNextSteps)
			{
				if (occupiedSpaces.Contains(potentialNextStep))
					continue;

				List<Tuple<int, int>> adjacentSpaces = new List<Tuple<int, int>>()
				{
					new Tuple<int, int>(potentialNextStep.Item1 - 1, potentialNextStep.Item2 - 1),
					new Tuple<int, int>(potentialNextStep.Item1 - 1, potentialNextStep.Item2),
					new Tuple<int, int>(potentialNextStep.Item1 - 1, potentialNextStep.Item2 + 1),
					new Tuple<int, int>(potentialNextStep.Item1, potentialNextStep.Item2 - 1),
					new Tuple<int, int>(potentialNextStep.Item1, potentialNextStep.Item2 + 1),
					new Tuple<int, int>(potentialNextStep.Item1 + 1, potentialNextStep.Item2 - 1),
					new Tuple<int, int>(potentialNextStep.Item1 + 1, potentialNextStep.Item2),
					new Tuple<int, int>(potentialNextStep.Item1 + 1, potentialNextStep.Item2 + 1)
				};

				bool isTooClose = false;

				foreach (Tuple<int, int> adjacentSpace in adjacentSpaces)
				{
					if (occupiedSpaces.Contains(adjacentSpace))
					{
						if (!adjacentSpace.Equals(path[path.Count - 1]))
						{
							if (path.Count > 1 && !adjacentSpace.Equals(path[path.Count - 2]))
							{
								isTooClose = true;
								break;
							}
						}
					}
				}

				if (isTooClose)
					continue;

				List<Tuple<int, int>> newList = new List<Tuple<int, int>>(path);
				newList.Add(potentialNextStep);

				newList = GeneratePathHelper(path: newList, random: random);

				if (newList != null)
					return newList;
			}

			return null;
		}
	}
}
