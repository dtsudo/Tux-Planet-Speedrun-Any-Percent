
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class OverworldUtil
	{
		public static List<Tuple<int, int>> GetPath(
			OverworldGameMap overworldGameMap,
			IReadOnlyList<Tuple<int, int>> reachableTiles,
			Tuple<int, int> currentLocation,
			Tuple<int, int> directionOfTravel)
		{
			List<Tuple<int, int>> returnValue = new List<Tuple<int, int>>();

			Tuple<int, int> nextTile = new Tuple<int, int>(currentLocation.Item1 + directionOfTravel.Item1, currentLocation.Item2 + directionOfTravel.Item2);

			if (!reachableTiles.Contains(nextTile, comparer: new IntTupleEqualityComparer()))
				return returnValue;

			while (true)
			{
				returnValue.Add(nextTile);

				if (overworldGameMap.Tilemap[nextTile.Item1][nextTile.Item2].Type == OverworldGameMap.TileType.Level)
					break;

				List<Tuple<int, int>> possibleNextTiles = new List<Tuple<int, int>>()
				{
					new Tuple<int, int>(nextTile.Item1 - 1, nextTile.Item2),
					new Tuple<int, int>(nextTile.Item1 + 1, nextTile.Item2),
					new Tuple<int, int>(nextTile.Item1, nextTile.Item2 - 1),
					new Tuple<int, int>(nextTile.Item1, nextTile.Item2 + 1)
				};
				possibleNextTiles = possibleNextTiles
					.Where(x => reachableTiles.Contains(x, comparer: new IntTupleEqualityComparer()))
					.Where(x => !returnValue.Contains(x, comparer: new IntTupleEqualityComparer()))
					.Where(x => !x.Equals(currentLocation))
					.ToList();

				if (possibleNextTiles.Count != 1)
					break;

				nextTile = possibleNextTiles[0];
			}

			return returnValue;
		}

		public static HashSet<Tuple<int, int>> GetReachableTiles(
			OverworldGameMap overworldGameMap,
			IReadOnlyList<Level> completedLevels)
		{
			HashSet<Level> completedLevelsSet = new HashSet<Level>(completedLevels);

			HashSet<Tuple<int, int>> returnValue = new HashSet<Tuple<int, int>>(comparer: new IntTupleEqualityComparer());

			Queue<Tuple<int, int>> toBeProcessed = new Queue<Tuple<int, int>>();
			toBeProcessed.Enqueue(overworldGameMap.StartingLocation);

			while (toBeProcessed.Count > 0)
			{
				Tuple<int, int> location = toBeProcessed.Dequeue();
				int x = location.Item1;
				int y = location.Item2;

				if (returnValue.Contains(location))
					continue;

				if (x < 0 || x >= overworldGameMap.Tilemap.Count)
					continue;
				if (y < 0 || y >= overworldGameMap.Tilemap[0].Count)
					continue;

				if (overworldGameMap.Tilemap[x][y].Type == OverworldGameMap.TileType.NonPath)
					continue;

				returnValue.Add(location);

				if (overworldGameMap.Tilemap[x][y].Type == OverworldGameMap.TileType.Level
						&& !completedLevelsSet.Contains(overworldGameMap.Tilemap[x][y].Level.Value))
					continue;

				toBeProcessed.Enqueue(new Tuple<int, int>(x - 1, y));
				toBeProcessed.Enqueue(new Tuple<int, int>(x + 1, y));
				toBeProcessed.Enqueue(new Tuple<int, int>(x, y - 1));
				toBeProcessed.Enqueue(new Tuple<int, int>(x, y + 1));
			}

			return returnValue;
		}
	}
}
