
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class OverworldGameMapGenerator
	{
		private class BacktrackCounter
		{
			private int? maxNumBacktracks;
			private int numBacktracks;

			public BacktrackCounter(int? maxNumBacktracks)
			{
				this.maxNumBacktracks = maxNumBacktracks;
				this.numBacktracks = 0;
			}

			public void Increment()
			{
				this.numBacktracks += 1;

				if (this.maxNumBacktracks != null && this.numBacktracks > this.maxNumBacktracks.Value)
					throw new MapGenerationFailureException();
			}
		}

		public static OverworldGameMap.Tile[][] GenerateOverworldGameMapTileArray(
			int windowWidth,
			int windowHeight,
			IReadOnlyList<Level> waterLevels,
			IReadOnlyList<Level> mountainLevels,
			IReadOnlyList<Level> fortressLevels,
			IDTDeterministicRandom random)
		{
			int pathLength = 60;

			int level1Index = 0;
			int level2Index = 10 + random.NextInt(4);

			int level5Index = 49;
			int level4Index = 35 + random.NextInt(4);

			int level3Index = (level2Index + level4Index) / 2 + random.NextInt(4) - 2;

			int level6Index = 59;

			Dictionary<Level, int> levelToPathIndexMapping = new Dictionary<Level, int>();
			levelToPathIndexMapping[Level.Level1] = level1Index;
			levelToPathIndexMapping[Level.Level2] = level2Index;
			levelToPathIndexMapping[Level.Level3] = level3Index;
			levelToPathIndexMapping[Level.Level4] = level4Index;
			levelToPathIndexMapping[Level.Level5] = level5Index;
			levelToPathIndexMapping[Level.Level6] = level6Index;

			List<Tuple<int, int>> path = GeneratePath(
					pathLength: pathLength,
					levelToPathIndexMapping: levelToPathIndexMapping,
					waterLevels: waterLevels,
					mountainLevels: mountainLevels,
					fortressLevels: fortressLevels,
					random: random);

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

			tilemap[path[level1Index].Item1][path[level1Index].Item2] = new OverworldGameMap.Tile(type: OverworldGameMap.TileType.Level, level: Level.Level1);
			tilemap[path[level2Index].Item1][path[level2Index].Item2] = new OverworldGameMap.Tile(type: OverworldGameMap.TileType.Level, level: Level.Level2);
			tilemap[path[level3Index].Item1][path[level3Index].Item2] = new OverworldGameMap.Tile(type: OverworldGameMap.TileType.Level, level: Level.Level3);
			tilemap[path[level4Index].Item1][path[level4Index].Item2] = new OverworldGameMap.Tile(type: OverworldGameMap.TileType.Level, level: Level.Level4);
			tilemap[path[level5Index].Item1][path[level5Index].Item2] = new OverworldGameMap.Tile(type: OverworldGameMap.TileType.Level, level: Level.Level5);
			tilemap[path[level6Index].Item1][path[level6Index].Item2] = new OverworldGameMap.Tile(type: OverworldGameMap.TileType.Level, level: Level.Level6);

			return tilemap;
		}

		private static List<Tuple<int, int>> GeneratePath(
			int pathLength,
			Dictionary<Level, int> levelToPathIndexMapping,
			IReadOnlyList<Level> waterLevels,
			IReadOnlyList<Level> mountainLevels,
			IReadOnlyList<Level> fortressLevels,
			IDTDeterministicRandom random)
		{
			HashSet<int> indexesOfSpecialLevels = new HashSet<int>();
			List<int> indexesOfMountainLevels = new List<int>();

			foreach (Level waterLevel in waterLevels)
			{
				indexesOfSpecialLevels.Add(levelToPathIndexMapping[waterLevel]);
			}
			foreach (Level mountainLevel in mountainLevels)
			{
				indexesOfSpecialLevels.Add(levelToPathIndexMapping[mountainLevel]);
				indexesOfMountainLevels.Add(levelToPathIndexMapping[mountainLevel]);
			}
			foreach (Level fortressLevel in fortressLevels)
			{
				indexesOfSpecialLevels.Add(levelToPathIndexMapping[fortressLevel]);
			}

			indexesOfMountainLevels.Sort();

			bool[] doesPathIndexHaveRestrictions = new bool[pathLength];

			for (int i = 0; i < pathLength; i++)
				doesPathIndexHaveRestrictions[i] = false;

			HashSet<int> straightPathIndexes = new HashSet<int>();
			HashSet<int> mountainPathIndexes = new HashSet<int>();

			foreach (Level waterLevel in waterLevels)
			{
				int waterLevelIndex = levelToPathIndexMapping[waterLevel];

				List<int> relevantIndexes = new List<int>();
				if (waterLevelIndex >= 3)
					relevantIndexes.Add(waterLevelIndex - 3);
				if (waterLevelIndex >= 2)
					relevantIndexes.Add(waterLevelIndex - 2);
				if (waterLevelIndex >= 1)
					relevantIndexes.Add(waterLevelIndex - 1);
				relevantIndexes.Add(waterLevelIndex);
				if (waterLevelIndex < pathLength - 1)
					relevantIndexes.Add(waterLevelIndex + 1);
				if (waterLevelIndex < pathLength - 2)
					relevantIndexes.Add(waterLevelIndex + 2);
				if (waterLevelIndex < pathLength - 3)
					relevantIndexes.Add(waterLevelIndex + 3);

				if (relevantIndexes.All(x => !doesPathIndexHaveRestrictions[x]))
				{
					foreach (int relevantIndex in relevantIndexes)
					{
						straightPathIndexes.Add(relevantIndex);
						doesPathIndexHaveRestrictions[relevantIndex] = true;
					}

					straightPathIndexes.Remove(waterLevelIndex + 1);
				}
			}

			foreach (Level mountainLevel in mountainLevels)
			{
				int mountainLevelIndex = levelToPathIndexMapping[mountainLevel];

				List<int> relevantIndexes = new List<int>();
				if (mountainLevelIndex >= 3)
					relevantIndexes.Add(mountainLevelIndex - 3);
				if (mountainLevelIndex >= 2)
					relevantIndexes.Add(mountainLevelIndex - 2);
				if (mountainLevelIndex >= 1)
					relevantIndexes.Add(mountainLevelIndex - 1);
				relevantIndexes.Add(mountainLevelIndex);
				if (mountainLevelIndex < pathLength - 1)
					relevantIndexes.Add(mountainLevelIndex + 1);
				if (mountainLevelIndex < pathLength - 2)
					relevantIndexes.Add(mountainLevelIndex + 2);
				if (mountainLevelIndex < pathLength - 3)
					relevantIndexes.Add(mountainLevelIndex + 3);

				if (relevantIndexes.All(x => !doesPathIndexHaveRestrictions[x]))
				{
					foreach (int relevantIndex in relevantIndexes)
					{
						mountainPathIndexes.Add(relevantIndex);
						doesPathIndexHaveRestrictions[relevantIndex] = true;
					}
				}
			}

			foreach (Level fortressLevel in fortressLevels)
			{
				int fortressLevelIndex = levelToPathIndexMapping[fortressLevel];

				List<int> relevantIndexes = new List<int>();
				if (fortressLevelIndex >= 3)
					relevantIndexes.Add(fortressLevelIndex - 3);
				if (fortressLevelIndex >= 2)
					relevantIndexes.Add(fortressLevelIndex - 2);
				if (fortressLevelIndex >= 1)
					relevantIndexes.Add(fortressLevelIndex - 1);
				relevantIndexes.Add(fortressLevelIndex);
				if (fortressLevelIndex < pathLength - 1)
					relevantIndexes.Add(fortressLevelIndex + 1);
				if (fortressLevelIndex < pathLength - 2)
					relevantIndexes.Add(fortressLevelIndex + 2);
				if (fortressLevelIndex < pathLength - 3)
					relevantIndexes.Add(fortressLevelIndex + 3);

				if (relevantIndexes.All(x => !doesPathIndexHaveRestrictions[x]))
				{
					foreach (int relevantIndex in relevantIndexes)
					{
						straightPathIndexes.Add(relevantIndex);
						doesPathIndexHaveRestrictions[relevantIndex] = true;
					}

					straightPathIndexes.Remove(fortressLevelIndex + 1);
				}
			}

			List<Tuple<int, int>> path = new List<Tuple<int, int>>();

			path.Add(new Tuple<int, int>(0, 0));

			Func<Tuple<int, int>, Tuple<int, int>, int, List<Tuple<int, int>>> getPotentialNextSteps = (previousLocation, currentLocation, i) =>
			{
				if (straightPathIndexes.Contains(i))
				{
					if (previousLocation == null)
						return null;

					return new List<Tuple<int, int>>()
					{
						new Tuple<int, int>(
							item1: currentLocation.Item1 + (currentLocation.Item1 - previousLocation.Item1),
							item2: currentLocation.Item2 + (currentLocation.Item2 - previousLocation.Item2))
					};
				}

				if (mountainPathIndexes.Contains(i))
				{
					return new List<Tuple<int, int>>()
					{
						new Tuple<int, int>(currentLocation.Item1 - 1, currentLocation.Item2),
						new Tuple<int, int>(currentLocation.Item1 + 1, currentLocation.Item2)
					};
				}

				return null;
			};

			Func<IReadOnlyList<Tuple<int, int>>, bool> additionalValidationFunc = currentPath =>
			{
				int index = currentPath.Count - 1;

				bool shouldCheckLevelCollision = indexesOfSpecialLevels.Contains(index) || index == pathLength - 1;
				bool shouldCheckMountainAndPathCollision = indexesOfSpecialLevels.Contains(index) || index == pathLength - 1 || index % 3 == 0;

				if (shouldCheckLevelCollision)
				{
					List<Tuple<int, int>> specialLevelLocations = new List<Tuple<int, int>>();

					foreach (int s in indexesOfSpecialLevels)
					{
						if (s < currentPath.Count)
							specialLevelLocations.Add(currentPath[s]);
					}

					for (int i = 0; i < specialLevelLocations.Count; i++)
					{
						for (int j = i + 1; j < specialLevelLocations.Count; j++)
						{
							Tuple<int, int> levelA = specialLevelLocations[i];
							Tuple<int, int> levelB = specialLevelLocations[j];

							if (Math.Abs(levelA.Item1 - levelB.Item1) + Math.Abs(levelA.Item2 - levelB.Item2) <= 6)
								return false;
						}
					}
				}

				if (shouldCheckMountainAndPathCollision)
				{
					HashSet<Tuple<int, int>> pathSet = new HashSet<Tuple<int, int>>(currentPath, new IntTupleEqualityComparer());

					foreach (int mountainLevelIndex in indexesOfMountainLevels)
					{
						if (mountainLevelIndex < currentPath.Count)
						{
							Tuple<int, int> mountainLevelLocation = currentPath[mountainLevelIndex];

							if (pathSet.Contains(new Tuple<int, int>(mountainLevelLocation.Item1 - 1, mountainLevelLocation.Item2 + 2))
									|| pathSet.Contains(new Tuple<int, int>(mountainLevelLocation.Item1, mountainLevelLocation.Item2 + 2))
									|| pathSet.Contains(new Tuple<int, int>(mountainLevelLocation.Item1 + 1, mountainLevelLocation.Item2 + 2)))
								return false;
						}
					}
				}

				return true;
			};

			List<Tuple<int, int>> returnVal;

			int numTries = 0;
			while (true)
			{
				try
				{
					returnVal = GeneratePathHelper(
						path: path,
						getPotentialNextSteps: getPotentialNextSteps,
						additionalValidationFunc: additionalValidationFunc,
						pathLength: pathLength,
						backtrackCounter: new BacktrackCounter(maxNumBacktracks: 5000),
						random: random);
					break;
				}
				catch (MapGenerationFailureException)
				{
					numTries++;
				}

				if (numTries == 10)
				{
					returnVal = GeneratePathHelper(
						path: path,
						getPotentialNextSteps: (previousLocation, currentLocation, i) =>
						{
							return new List<Tuple<int, int>>()
							{
								new Tuple<int, int>(currentLocation.Item1 + 1, currentLocation.Item2)
							};
						},
						additionalValidationFunc: additionalValidationFunc,
						pathLength: pathLength,
						backtrackCounter: new BacktrackCounter(maxNumBacktracks: null),
						random: random);
					break;
				}
			}

			return new List<Tuple<int, int>>(returnVal);
		}

		private static List<Tuple<int, int>> GeneratePathHelper(
			IReadOnlyList<Tuple<int, int>> path,
			Func<Tuple<int, int>, Tuple<int, int>, int, List<Tuple<int, int>>> getPotentialNextSteps,
			Func<IReadOnlyList<Tuple<int, int>>, bool> additionalValidationFunc,
			int pathLength,
			BacktrackCounter backtrackCounter,
			IDTDeterministicRandom random)
		{
			if (path.Count == pathLength)
				return new List<Tuple<int, int>>(path);

			HashSet<Tuple<int, int>> occupiedSpaces = new HashSet<Tuple<int, int>>(path, new IntTupleEqualityComparer());

			Tuple<int, int> currentLocation = path[path.Count - 1];
			Tuple<int, int> previousLocation = path.Count == 1 ? null : path[path.Count - 2];

			List<Tuple<int, int>> potentialNextSteps = null;

			if (getPotentialNextSteps != null)
				potentialNextSteps = getPotentialNextSteps(previousLocation, currentLocation, path.Count);

			if (potentialNextSteps == null)
			{
				potentialNextSteps = new List<Tuple<int, int>>()
				{
					new Tuple<int, int>(currentLocation.Item1 - 1, currentLocation.Item2),
					new Tuple<int, int>(currentLocation.Item1 + 1, currentLocation.Item2),
					new Tuple<int, int>(currentLocation.Item1, currentLocation.Item2 - 1),
					new Tuple<int, int>(currentLocation.Item1, currentLocation.Item2 + 1)
				};
			}

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

				if (!additionalValidationFunc(newList))
					continue;

				newList = GeneratePathHelper(
					path: newList, 
					getPotentialNextSteps: getPotentialNextSteps,
					additionalValidationFunc: additionalValidationFunc,
					pathLength: pathLength,
					backtrackCounter: backtrackCounter,
					random: random);

				if (newList != null)
					return newList;
			}

			backtrackCounter.Increment();
			return null;
		}
	}
}
