
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class OverworldMapGenerator
	{
		public class Result
		{
			public Result(Sprite[][] foregroundTiles, Sprite[][] backgroundTiles, IReadOnlyList<Level> levelsWithCustomSprite)
			{
				this.ForegroundTiles = foregroundTiles;
				this.BackgroundTiles = backgroundTiles;
				this.LevelsWithCustomSprite = new List<Level>(levelsWithCustomSprite);
			}

			public Sprite[][] ForegroundTiles { get; private set; }
			public Sprite[][] BackgroundTiles { get; private set; }

			public IReadOnlyList<Level> LevelsWithCustomSprite { get; private set; }
		}

		public static Result GenerateSpriteTilemap(
			OverworldGameMap map,
			IReadOnlyList<Level> waterLevels,
			IReadOnlyList<Level> mountainLevels,
			IReadOnlyList<Level> fortressLevels,
			IDTDeterministicRandom random)
		{
			int length1 = map.Tilemap.Count;
			int length2 = map.Tilemap[0].Count;

			Sprite[][] foregroundTilemap = new Sprite[length1][];
			bool?[][] isWaterArray = new bool?[length1][];
			bool[][] hasCustomForegroundSpriteArray = new bool[length1][];

			for (int i = 0; i < length1; i++)
			{
				foregroundTilemap[i] = new Sprite[length2];
				isWaterArray[i] = new bool?[length2];
				hasCustomForegroundSpriteArray[i] = new bool[length2];

				for (int j = 0; j < length2; j++)
				{
					foregroundTilemap[i][j] = null;

					if (map.Tilemap[i][j].Type != OverworldGameMap.TileType.NonPath)
						isWaterArray[i][j] = false;
					else
						isWaterArray[i][j] = null;

					hasCustomForegroundSpriteArray[i][j] = false;
				}
			}

			AddPathToForegroundTilemap(
				map: map,
				foregroundTilemap: foregroundTilemap);

			List<Level> levelsWithCustomSprite = new List<Level>();

			IReadOnlyDictionary<Level, Tuple<int, int>> levelLocations = GetLevelLocations(map: map);

			MarkWaterLevels(
				isWaterArray: isWaterArray,
				waterLevels: waterLevels,
				levelLocations: levelLocations);

			MarkMountainLevels(
				map: map,
				isWaterArray: isWaterArray,
				hasCustomForegroundSpriteArray: hasCustomForegroundSpriteArray,
				foregroundTilemap: foregroundTilemap,
				levelsWithCustomSprite: levelsWithCustomSprite,
				mountainLevels: mountainLevels,
				levelLocations: levelLocations);

			MarkFortressLevels(
				map: map,
				isWaterArray: isWaterArray,
				hasCustomForegroundSpriteArray: hasCustomForegroundSpriteArray,
				foregroundTilemap: foregroundTilemap,
				levelsWithCustomSprite: levelsWithCustomSprite,
				fortressLevels: fortressLevels,
				levelLocations: levelLocations);

			FillOutWaterArray(
				isWaterArray: isWaterArray,
				random: random);

			bool[][] nonNullIsWaterArray = new bool[length1][];
			for (int i = 0; i < length1; i++)
			{
				nonNullIsWaterArray[i] = new bool[length2];
				for (int j = 0; j < length2; j++)
					nonNullIsWaterArray[i][j] = isWaterArray[i][j].Value;
			}

			AddScenery(
				foregroundTilemap: foregroundTilemap,
				isWaterArray: nonNullIsWaterArray,
				random: random);

			return new Result(
				foregroundTiles: foregroundTilemap,
				backgroundTiles: GenerateBackgroundTiles(numColumns: length1, numRows: length2, isWaterArray: nonNullIsWaterArray, random: random),
				levelsWithCustomSprite: levelsWithCustomSprite);
		}

		private static void AddPathToForegroundTilemap(OverworldGameMap map, Sprite[][] foregroundTilemap)
		{
			int numColumns = map.Tilemap.Count;
			int numRows = map.Tilemap[0].Count;

			for (int i = 0; i < numColumns; i++)
			{
				for (int j = 0; j < numRows; j++)
				{
					int? spriteX;
					int? spriteY;

					switch (map.Tilemap[i][j].Type)
					{
						case OverworldGameMap.TileType.Path:
						case OverworldGameMap.TileType.Level:
							bool pathOnLeft = i > 0 && map.Tilemap[i - 1][j].Type != OverworldGameMap.TileType.NonPath;
							bool pathOnRight = i < numColumns - 1 && map.Tilemap[i + 1][j].Type != OverworldGameMap.TileType.NonPath;
							bool pathOnBottom = j > 0 && map.Tilemap[i][j - 1].Type != OverworldGameMap.TileType.NonPath;
							bool pathOnTop = j < numRows - 1 && map.Tilemap[i][j + 1].Type != OverworldGameMap.TileType.NonPath;

							if (!pathOnLeft && !pathOnRight && !pathOnBottom && !pathOnTop)
							{
								spriteX = 2;
								spriteY = 3;
							}
							else if (!pathOnLeft && !pathOnRight && !pathOnBottom && pathOnTop)
							{
								spriteX = 0;
								spriteY = 8;
							}
							else if (!pathOnLeft && !pathOnRight && pathOnBottom && !pathOnTop)
							{
								spriteX = 0;
								spriteY = 7;
							}
							else if (!pathOnLeft && !pathOnRight && pathOnBottom && pathOnTop)
							{
								spriteX = 2;
								spriteY = 4;
							}
							else if (!pathOnLeft && pathOnRight && !pathOnBottom && !pathOnTop)
							{
								spriteX = 1;
								spriteY = 7;
							}
							else if (!pathOnLeft && pathOnRight && !pathOnBottom && pathOnTop)
							{
								spriteX = 0;
								spriteY = 2;
							}
							else if (!pathOnLeft && pathOnRight && pathOnBottom && !pathOnTop)
							{
								spriteX = 0;
								spriteY = 0;
							}
							else if (!pathOnLeft && pathOnRight && pathOnBottom && pathOnTop)
							{
								spriteX = 0;
								spriteY = 1;
							}
							else if (pathOnLeft && !pathOnRight && !pathOnBottom && !pathOnTop)
							{
								spriteX = 2;
								spriteY = 7;
							}
							else if (pathOnLeft && !pathOnRight && !pathOnBottom && pathOnTop)
							{
								spriteX = 2;
								spriteY = 2;
							}
							else if (pathOnLeft && !pathOnRight && pathOnBottom && !pathOnTop)
							{
								spriteX = 2;
								spriteY = 0;
							}
							else if (pathOnLeft && !pathOnRight && pathOnBottom && pathOnTop)
							{
								spriteX = 2;
								spriteY = 1;
							}
							else if (pathOnLeft && pathOnRight && !pathOnBottom && !pathOnTop)
							{
								spriteX = 2;
								spriteY = 3;
							}
							else if (pathOnLeft && pathOnRight && !pathOnBottom && pathOnTop)
							{
								spriteX = 1;
								spriteY = 2;
							}
							else if (pathOnLeft && pathOnRight && pathOnBottom && !pathOnTop)
							{
								spriteX = 1;
								spriteY = 0;
							}
							else if (pathOnLeft && pathOnRight && pathOnBottom && pathOnTop)
							{
								spriteX = 1;
								spriteY = 1;
							}
							else
								throw new Exception();

							break;
						case OverworldGameMap.TileType.NonPath:
							spriteX = null;
							spriteY = null;
							break;
						default:
							throw new Exception();
					}

					if (spriteX != null)
						foregroundTilemap[i][j] = new Sprite(
							image: GameImage.PathDirt,
							x: spriteX.Value << 4,
							y: spriteY.Value << 4,
							width: 16,
							height: 16,
							scalingFactorScaled: 3 * 128);
				}
			}
		}

		private static IReadOnlyDictionary<Level, Tuple<int, int>> GetLevelLocations(OverworldGameMap map)
		{
			int length1 = map.Tilemap.Count;
			int length2 = map.Tilemap[0].Count;

			Dictionary<Level, Tuple<int, int>> levelLocations = new Dictionary<Level, Tuple<int, int>>();

			for (int i = 0; i < length1; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					if (map.Tilemap[i][j].Type == OverworldGameMap.TileType.Level)
						levelLocations[map.Tilemap[i][j].Level.Value] = new Tuple<int, int>(i, j);
				}
			}

			return levelLocations;
		}

		private static void MarkWaterLevels(
			bool?[][] isWaterArray,
			IReadOnlyList<Level> waterLevels,
			IReadOnlyDictionary<Level, Tuple<int, int>> levelLocations)
		{
			int length1 = isWaterArray.Length;
			int length2 = isWaterArray[0].Length;

			foreach (Level level in waterLevels)
			{
				Tuple<int, int> levelLocation = levelLocations[level];

				List<Tuple<int, int>> levelAndAdjacentSquares = new List<Tuple<int, int>>()
				{
					new Tuple<int, int>(levelLocation.Item1 - 1, levelLocation.Item2 - 1),
					new Tuple<int, int>(levelLocation.Item1 - 1, levelLocation.Item2),
					new Tuple<int, int>(levelLocation.Item1 - 1, levelLocation.Item2 + 1),
					new Tuple<int, int>(levelLocation.Item1, levelLocation.Item2 - 1),
					new Tuple<int, int>(levelLocation.Item1, levelLocation.Item2),
					new Tuple<int, int>(levelLocation.Item1, levelLocation.Item2 + 1),
					new Tuple<int, int>(levelLocation.Item1 + 1, levelLocation.Item2 - 1),
					new Tuple<int, int>(levelLocation.Item1 + 1, levelLocation.Item2),
					new Tuple<int, int>(levelLocation.Item1 + 1, levelLocation.Item2 + 1)
				};

				foreach (Tuple<int, int> square in levelAndAdjacentSquares)
				{
					if (square.Item1 >= 0 && square.Item1 < length1 && square.Item2 >= 0 && square.Item2 < length2)
						isWaterArray[square.Item1][square.Item2] = true;
				}
			}
		}

		private static void MarkMountainLevels(
			OverworldGameMap map,
			bool?[][] isWaterArray,
			bool[][] hasCustomForegroundSpriteArray,
			Sprite[][] foregroundTilemap,
			List<Level> levelsWithCustomSprite,
			IReadOnlyList<Level> mountainLevels,
			IReadOnlyDictionary<Level, Tuple<int, int>> levelLocations)
		{
			int length1 = isWaterArray.Length;
			int length2 = isWaterArray[0].Length;

			foreach (Level level in mountainLevels)
			{
				Tuple<int, int> levelLocation = levelLocations[level];

				if (levelLocation.Item1 - 1 >= 0 && levelLocation.Item1 + 1 < length1 && levelLocation.Item2 + 2 < length2)
				{
					List<Tuple<int, int>> relevantSquares = new List<Tuple<int, int>>()
					{
						new Tuple<int, int>(levelLocation.Item1 - 1, levelLocation.Item2),
						new Tuple<int, int>(levelLocation.Item1 - 1, levelLocation.Item2 + 1),
						new Tuple<int, int>(levelLocation.Item1 - 1, levelLocation.Item2 + 2),
						new Tuple<int, int>(levelLocation.Item1, levelLocation.Item2),
						new Tuple<int, int>(levelLocation.Item1, levelLocation.Item2 + 1),
						new Tuple<int, int>(levelLocation.Item1, levelLocation.Item2 + 2),
						new Tuple<int, int>(levelLocation.Item1 + 1, levelLocation.Item2),
						new Tuple<int, int>(levelLocation.Item1 + 1, levelLocation.Item2 + 1),
						new Tuple<int, int>(levelLocation.Item1 + 1, levelLocation.Item2 + 2)
					};

					bool isNotInWater = relevantSquares.All(x => isWaterArray[x.Item1][x.Item2] == null || isWaterArray[x.Item1][x.Item2].Value == false);
					bool isNotAlreadyOccupied = relevantSquares.All(x => hasCustomForegroundSpriteArray[x.Item1][x.Item2] == false);

					if (isNotInWater && isNotAlreadyOccupied)
					{
						levelsWithCustomSprite.Add(level);

						foreach (Tuple<int, int> relevantSquare in relevantSquares)
						{
							isWaterArray[relevantSquare.Item1][relevantSquare.Item2] = false;
							hasCustomForegroundSpriteArray[relevantSquare.Item1][relevantSquare.Item2] = true;
						}

						Func<int, int, Sprite> getMountainSprite = (x, y) =>
						{
							return new Sprite(
								image: GameImage.Mountains,
								x: x * 16,
								y: y * 16,
								width: 16,
								height: 16,
								scalingFactorScaled: 3 * 128);
						};

						foregroundTilemap[levelLocation.Item1 - 1][levelLocation.Item2] = getMountainSprite(0, 2);
						foregroundTilemap[levelLocation.Item1 - 1][levelLocation.Item2 + 1] = getMountainSprite(0, 1);
						foregroundTilemap[levelLocation.Item1 - 1][levelLocation.Item2 + 2] = getMountainSprite(0, 0);
						foregroundTilemap[levelLocation.Item1][levelLocation.Item2] = getMountainSprite(1, 2);
						foregroundTilemap[levelLocation.Item1][levelLocation.Item2 + 1] = getMountainSprite(1, 1);
						foregroundTilemap[levelLocation.Item1][levelLocation.Item2 + 2] = getMountainSprite(1, 0);
						foregroundTilemap[levelLocation.Item1 + 1][levelLocation.Item2] = getMountainSprite(2, 2);
						foregroundTilemap[levelLocation.Item1 + 1][levelLocation.Item2 + 1] = getMountainSprite(2, 1);
						foregroundTilemap[levelLocation.Item1 + 1][levelLocation.Item2 + 2] = getMountainSprite(2, 0);

						Func<int, int, bool> isPath = (x, y) =>
						{
							if (x < 0 || x >= length1 || y < 0 || y >= length2)
								return false;
							return map.Tilemap[x][y].Type != OverworldGameMap.TileType.NonPath;
						};

						if (isPath(levelLocation.Item1 - 2, levelLocation.Item2)
							&& isPath(levelLocation.Item1 - 3, levelLocation.Item2)
							&& isPath(levelLocation.Item1 - 1, levelLocation.Item2)
							&& !isPath(levelLocation.Item1 - 2, levelLocation.Item2 - 1)
							&& !isPath(levelLocation.Item1 - 2, levelLocation.Item2 + 1))
						{
							foregroundTilemap[levelLocation.Item1 - 2][levelLocation.Item2] = new Sprite(
								image: GameImage.PathDirt,
								x: 2 * 16,
								y: 7 * 16,
								width: 16,
								height: 16,
								scalingFactorScaled: 3 * 128);
						}

						if (isPath(levelLocation.Item1 + 2, levelLocation.Item2)
							&& isPath(levelLocation.Item1 + 3, levelLocation.Item2)
							&& isPath(levelLocation.Item1 + 1, levelLocation.Item2)
							&& !isPath(levelLocation.Item1 + 2, levelLocation.Item2 - 1)
							&& !isPath(levelLocation.Item1 + 2, levelLocation.Item2 + 1))
						{
							foregroundTilemap[levelLocation.Item1 + 2][levelLocation.Item2] = new Sprite(
								image: GameImage.PathDirt,
								x: 1 * 16,
								y: 7 * 16,
								width: 16,
								height: 16,
								scalingFactorScaled: 3 * 128);
						}
					}
				}
			}
		}

		private static void MarkFortressLevels(
			OverworldGameMap map,
			bool?[][] isWaterArray,
			bool[][] hasCustomForegroundSpriteArray,
			Sprite[][] foregroundTilemap,
			List<Level> levelsWithCustomSprite,
			IReadOnlyList<Level> fortressLevels,
			IReadOnlyDictionary<Level, Tuple<int, int>> levelLocations)
		{
			int length1 = isWaterArray.Length;
			int length2 = isWaterArray[0].Length;

			foreach (Level level in fortressLevels)
			{
				Tuple<int, int> levelLocation = levelLocations[level];

				if (levelLocation.Item1 - 1 >= 0 && levelLocation.Item1 + 1 < length1 && levelLocation.Item2 - 1 >= 0 && levelLocation.Item2 + 1 < length2)
				{
					List<Tuple<int, int>> relevantSquares = new List<Tuple<int, int>>()
					{
						new Tuple<int, int>(levelLocation.Item1 - 1, levelLocation.Item2 - 1),
						new Tuple<int, int>(levelLocation.Item1 - 1, levelLocation.Item2),
						new Tuple<int, int>(levelLocation.Item1 - 1, levelLocation.Item2 + 1),
						new Tuple<int, int>(levelLocation.Item1, levelLocation.Item2 - 1),
						new Tuple<int, int>(levelLocation.Item1, levelLocation.Item2),
						new Tuple<int, int>(levelLocation.Item1, levelLocation.Item2 + 1),
						new Tuple<int, int>(levelLocation.Item1 + 1, levelLocation.Item2 - 1),
						new Tuple<int, int>(levelLocation.Item1 + 1, levelLocation.Item2),
						new Tuple<int, int>(levelLocation.Item1 + 1, levelLocation.Item2 + 1)
					};

					bool isNotInWater = relevantSquares.All(x => isWaterArray[x.Item1][x.Item2] == null || isWaterArray[x.Item1][x.Item2].Value == false);
					bool isNotAlreadyOccupied = relevantSquares.All(x => hasCustomForegroundSpriteArray[x.Item1][x.Item2] == false);

					if (isNotInWater && isNotAlreadyOccupied)
					{
						levelsWithCustomSprite.Add(level);

						foreach (Tuple<int, int> relevantSquare in relevantSquares)
						{
							isWaterArray[relevantSquare.Item1][relevantSquare.Item2] = false;
							hasCustomForegroundSpriteArray[relevantSquare.Item1][relevantSquare.Item2] = true;
						}

						Func<int, int, Sprite> getFortressSprite = (x, y) =>
						{
							return new Sprite(
								image: GameImage.Towns,
								x: 15 * 16 + x * 16,
								y: y * 16,
								width: 16,
								height: 16,
								scalingFactorScaled: 3 * 128);
						};

						foregroundTilemap[levelLocation.Item1 - 1][levelLocation.Item2 - 1] = getFortressSprite(0, 2);
						foregroundTilemap[levelLocation.Item1 - 1][levelLocation.Item2] = getFortressSprite(0, 1);
						foregroundTilemap[levelLocation.Item1 - 1][levelLocation.Item2 + 1] = getFortressSprite(0, 0);
						foregroundTilemap[levelLocation.Item1][levelLocation.Item2 - 1] = getFortressSprite(1, 2);
						foregroundTilemap[levelLocation.Item1][levelLocation.Item2] = getFortressSprite(1, 1);
						foregroundTilemap[levelLocation.Item1][levelLocation.Item2 + 1] = getFortressSprite(1, 0);
						foregroundTilemap[levelLocation.Item1 + 1][levelLocation.Item2 - 1] = getFortressSprite(2, 2);
						foregroundTilemap[levelLocation.Item1 + 1][levelLocation.Item2] = getFortressSprite(2, 1);
						foregroundTilemap[levelLocation.Item1 + 1][levelLocation.Item2 + 1] = getFortressSprite(2, 0);

						Func<int, int, bool> isPath = (x, y) =>
						{
							if (x < 0 || x >= length1 || y < 0 || y >= length2)
								return false;
							return map.Tilemap[x][y].Type != OverworldGameMap.TileType.NonPath;
						};

						if (isPath(levelLocation.Item1 - 2, levelLocation.Item2)
							&& isPath(levelLocation.Item1 - 3, levelLocation.Item2)
							&& isPath(levelLocation.Item1 - 1, levelLocation.Item2)
							&& !isPath(levelLocation.Item1 - 2, levelLocation.Item2 - 1)
							&& !isPath(levelLocation.Item1 - 2, levelLocation.Item2 + 1))
						{
							foregroundTilemap[levelLocation.Item1 - 2][levelLocation.Item2] = new Sprite(
								image: GameImage.PathDirt,
								x: 2 * 16,
								y: 7 * 16,
								width: 16,
								height: 16,
								scalingFactorScaled: 3 * 128);
						}

						if (isPath(levelLocation.Item1 + 2, levelLocation.Item2)
							&& isPath(levelLocation.Item1 + 3, levelLocation.Item2)
							&& isPath(levelLocation.Item1 + 1, levelLocation.Item2)
							&& !isPath(levelLocation.Item1 + 2, levelLocation.Item2 - 1)
							&& !isPath(levelLocation.Item1 + 2, levelLocation.Item2 + 1))
						{
							foregroundTilemap[levelLocation.Item1 + 2][levelLocation.Item2] = new Sprite(
								image: GameImage.PathDirt,
								x: 1 * 16,
								y: 7 * 16,
								width: 16,
								height: 16,
								scalingFactorScaled: 3 * 128);
						}

						if (isPath(levelLocation.Item1, levelLocation.Item2 - 2)
							&& isPath(levelLocation.Item1, levelLocation.Item2 - 3)
							&& isPath(levelLocation.Item1, levelLocation.Item2 - 1)
							&& !isPath(levelLocation.Item1 - 1, levelLocation.Item2 - 2)
							&& !isPath(levelLocation.Item1 + 1, levelLocation.Item2 - 2))
						{
							foregroundTilemap[levelLocation.Item1][levelLocation.Item2 - 2] = new Sprite(
								image: GameImage.PathDirt,
								x: 0,
								y: 7 * 16,
								width: 16,
								height: 16,
								scalingFactorScaled: 3 * 128);
						}

						if (isPath(levelLocation.Item1, levelLocation.Item2 + 2)
							&& isPath(levelLocation.Item1, levelLocation.Item2 + 3)
							&& isPath(levelLocation.Item1, levelLocation.Item2 + 1)
							&& !isPath(levelLocation.Item1 - 1, levelLocation.Item2 + 2)
							&& !isPath(levelLocation.Item1 + 1, levelLocation.Item2 + 2))
						{
							foregroundTilemap[levelLocation.Item1][levelLocation.Item2 + 2] = new Sprite(
								image: GameImage.PathDirt,
								x: 0,
								y: 8 * 16,
								width: 16,
								height: 16,
								scalingFactorScaled: 3 * 128);
						}
					}
				}
			}
		}

		private static void AddScenery(
			Sprite[][] foregroundTilemap, 
			bool[][] isWaterArray, 
			IDTDeterministicRandom random)
		{
			int length1 = foregroundTilemap.Length;
			int length2 = foregroundTilemap[0].Length;

			int numLandTiles = 0;

			for (int i = 0; i < isWaterArray.Length; i++)
			{
				for (int j = 0; j < isWaterArray[i].Length; j++)
				{
					if (!isWaterArray[i][j])
						numLandTiles++;
				}
			}

			int maxNumTries = numLandTiles / 20 + random.NextInt(numLandTiles / 20 + 1);

			for (int numTries = 0; numTries < maxNumTries; numTries++)
			{
				int i = random.NextInt(length1);
				int j = random.NextInt(length2);

				if (isWaterArray[i][j])
					continue;

				if (foregroundTilemap[i][j] != null)
					continue;

				Sprite sprite;

				switch (random.NextInt(2))
				{
					case 0:
						sprite = new Sprite(
							image: GameImage.ForestSnowy,
							x: 0,
							y: random.NextBool() ? 0 : 16,
							width: 16,
							height: 16,
							scalingFactorScaled: 3 * 128);
						break;
					case 1:
						sprite = new Sprite(
							image: GameImage.RocksSnow,
							x: random.NextBool() ? 16 : 32,
							y: 16,
							width: 16,
							height: 16,
							scalingFactorScaled: 3 * 128);
						break;
					default:
						throw new Exception();
				}

				foregroundTilemap[i][j] = sprite;
			}
		}

		private static void FillOutWaterArray(
			bool?[][] isWaterArray,
			IDTDeterministicRandom random)
		{
			int length1 = isWaterArray.Length;
			int length2 = isWaterArray[0].Length;

			List<Tuple<int, int>> waterTiles = new List<Tuple<int, int>>();

			for (int i = 0; i < length1; i++)
			{
				Func<int, int, bool> isWater = (x, y) =>
				{
					if (x < 0 || x >= length1 || y < 0 || y >= length2)
						return false;
					return isWaterArray[x][y].HasValue && isWaterArray[x][y].Value;
				};

				for (int j = 0; j < length2; j++)
				{
					if (isWater(i, j))
					{
						int numAdjacentWater = 0;
						if (isWater(i - 1, j))
							numAdjacentWater++;
						if (isWater(i + 1, j))
							numAdjacentWater++;
						if (isWater(i, j - 1))
							numAdjacentWater++;
						if (isWater(i, j + 1))
							numAdjacentWater++;

						if (numAdjacentWater >= 3)
							waterTiles.Add(new Tuple<int, int>(i, j));
					}
				}
			}

			int numWaterTilesAdded = 0;

			int numWaterTilesToAdd = waterTiles.Count * 30 + random.NextInt(waterTiles.Count * 8 + 1);

			while (true)
			{
				if (waterTiles.Count == 0)
					break;

				if (numWaterTilesAdded >= numWaterTilesToAdd)
					break;

				int randomIndex = random.NextInt(waterTiles.Count);

				Tuple<int, int> temp = waterTiles[randomIndex];
				waterTiles[randomIndex] = waterTiles[waterTiles.Count - 1];
				waterTiles[waterTiles.Count - 1] = temp;

				Tuple<int, int> toBeProcessed = waterTiles[waterTiles.Count - 1];
				waterTiles.RemoveAt(waterTiles.Count - 1);

				List<Tuple<int, int>> adjacentTiles = new List<Tuple<int, int>>()
				{
					new Tuple<int, int>(toBeProcessed.Item1 - 1, toBeProcessed.Item2 - 1),
					new Tuple<int, int>(toBeProcessed.Item1 - 1, toBeProcessed.Item2),
					new Tuple<int, int>(toBeProcessed.Item1 - 1, toBeProcessed.Item2 + 1),
					new Tuple<int, int>(toBeProcessed.Item1, toBeProcessed.Item2 - 1),
					new Tuple<int, int>(toBeProcessed.Item1, toBeProcessed.Item2),
					new Tuple<int, int>(toBeProcessed.Item1, toBeProcessed.Item2 + 1),
					new Tuple<int, int>(toBeProcessed.Item1 + 1, toBeProcessed.Item2 - 1),
					new Tuple<int, int>(toBeProcessed.Item1 + 1, toBeProcessed.Item2),
					new Tuple<int, int>(toBeProcessed.Item1 + 1, toBeProcessed.Item2 + 1)
				};
				adjacentTiles = adjacentTiles.Where(x => x.Item1 >= 0 && x.Item1 < length1 && x.Item2 >= 0 && x.Item2 < length2).ToList();

				if (adjacentTiles.Any(x => isWaterArray[x.Item1][x.Item2].HasValue && !isWaterArray[x.Item1][x.Item2].Value))
					continue;
				if (adjacentTiles.All(x => isWaterArray[x.Item1][x.Item2].HasValue && isWaterArray[x.Item1][x.Item2].Value))
					continue;

				List<Tuple<int, int>> waterTilesToAdd = adjacentTiles.Where(x => isWaterArray[x.Item1][x.Item2] == null).ToList();

				bool shouldNotAddTheseTiles = waterTilesToAdd.Any(x =>
					{
						List<Tuple<int, int>> tilesAdjacentToThisWaterTile = new List<Tuple<int, int>>()
							{
								new Tuple<int, int>(x.Item1 - 1, x.Item2 - 1),
								new Tuple<int, int>(x.Item1 - 1, x.Item2),
								new Tuple<int, int>(x.Item1 - 1, x.Item2 + 1),
								new Tuple<int, int>(x.Item1, x.Item2 - 1),
								new Tuple<int, int>(x.Item1, x.Item2 + 1),
								new Tuple<int, int>(x.Item1 + 1, x.Item2 - 1),
								new Tuple<int, int>(x.Item1 + 1, x.Item2),
								new Tuple<int, int>(x.Item1 + 1, x.Item2 + 1)
							};
						tilesAdjacentToThisWaterTile = tilesAdjacentToThisWaterTile.Where(t => t.Item1 >= 0 && t.Item1 < length1 && t.Item2 >= 0 && t.Item2 < length2).ToList();

						List<Tuple<int, int>> tilesToCheck = tilesAdjacentToThisWaterTile.Where(
							t => Math.Abs(t.Item1 - toBeProcessed.Item1) == 2 && Math.Abs(t.Item2 - toBeProcessed.Item2) == 2
								|| Math.Abs(t.Item1 - toBeProcessed.Item1) == 2 && Math.Abs(t.Item2 - toBeProcessed.Item2) == 1
								|| Math.Abs(t.Item1 - toBeProcessed.Item1) == 1 && Math.Abs(t.Item2 - toBeProcessed.Item2) == 2).ToList();

						bool isBadTile = tilesToCheck.Any(t => isWaterArray[t.Item1][t.Item2].HasValue && isWaterArray[t.Item1][t.Item2].Value);

						return isBadTile;
					});

				if (shouldNotAddTheseTiles)
					continue;

				foreach (Tuple<int, int> adjacentTile in adjacentTiles)
				{
					if (isWaterArray[adjacentTile.Item1][adjacentTile.Item2] == null)
					{
						isWaterArray[adjacentTile.Item1][adjacentTile.Item2] = true;
						numWaterTilesAdded++;
					}

					if (Math.Abs(adjacentTile.Item1 - toBeProcessed.Item1) + Math.Abs(adjacentTile.Item2 - toBeProcessed.Item2) == 1)
						waterTiles.Add(adjacentTile);
				}
			}

			for (int i = 0; i < length1; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					if (isWaterArray[i][j] == null)
						isWaterArray[i][j] = false;
				}
			}
		}

		private static Sprite[][] GenerateBackgroundTiles(
			int numColumns, 
			int numRows, 
			bool[][] isWaterArray, 
			IDTDeterministicRandom random)
		{
			Sprite[][] tilemap = new Sprite[numColumns][];

			Func<int, int, bool> isLand = (i, j) =>
			{
				return i >= 0 && i < isWaterArray.Length && j >= 0 && j < isWaterArray[i].Length && !isWaterArray[i][j];
			};

			for (int i = 0; i < numColumns; i++)
			{
				tilemap[i] = new Sprite[numRows];

				for (int j = 0; j < numRows; j++)
				{
					Sprite sprite;

					if (isWaterArray[i][j])
					{
						int x = random.NextInt(3);
						int y = 5;

						if (isLand(i - 1, j) && isLand(i, j - 1))
						{
							x = 1;
							y = 1;
						}
						else if (isLand(i - 1, j) && isLand(i, j + 1))
						{
							x = 1;
							y = 0;
						}
						else if (isLand(i + 1, j) && isLand(i, j - 1))
						{
							x = 2;
							y = 1;
						}
						else if (isLand(i + 1, j) && isLand(i, j + 1))
						{
							x = 2;
							y = 0;
						}
						else if (isLand(i - 1, j))
						{
							x = 2;
							y = 3;
						}
						else if (isLand(i + 1, j))
						{
							x = 0;
							y = 3;
						}
						else if (isLand(i, j - 1))
						{
							x = 1;
							y = 2;
						}
						else if (isLand(i, j + 1))
						{
							x = 1;
							y = 4;
						}
						else if (isLand(i - 1, j - 1))
						{
							x = 2;
							y = 2;
						}
						else if (isLand(i - 1, j + 1))
						{
							x = 2;
							y = 4;
						}
						else if (isLand(i + 1, j - 1))
						{
							x = 0;
							y = 2;
						}
						else if (isLand(i + 1, j + 1))
						{
							x = 0;
							y = 4;
						}

						sprite = new Sprite(
							image: GameImage.WaterCliffSnow,
							x: x << 4,
							y: y << 4,
							width: 16,
							height: 16,
							scalingFactorScaled: 3 * 128);
					}
					else
						sprite = new Sprite(
							image: GameImage.Snow,
							x: random.NextInt(3) << 4,
							y: 5 * 16,
							width: 16,
							height: 16,
							scalingFactorScaled: 3 * 128);

					tilemap[i][j] = sprite;
				}
			}

			return tilemap;
		}
	}
}
