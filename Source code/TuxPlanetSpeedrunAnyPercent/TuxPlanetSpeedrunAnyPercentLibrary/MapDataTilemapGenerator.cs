
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class MapDataTilemapGenerator
	{
		public static Tilemap GetTilemap(
			MapDataHelper.Map data,
			EnemyIdGenerator enemyIdGenerator,
			string cutsceneName,
			int scalingFactorScaled,
			GameMusic gameMusic)
		{
			IReadOnlyList<MapDataHelper.Tileset> tilesets = data.Tilesets;

			MapDataHelper.Tileset solidTileset = tilesets.Single(x => x.Name.ToUpperCaseCultureInvariant() == "SOLID");
			MapDataHelper.Tileset actorsTileset = tilesets.Single(x => x.Name.ToUpperCaseCultureInvariant() == "ACTORS");

			IReadOnlyList<MapDataHelper.Layer> layers = data.Layers;

			MapDataHelper.Layer solidLayer = layers.Single(x => x.Name.ToUpperCaseCultureInvariant() == "SOLID");
			MapDataHelper.Layer foregroundLayer = layers.Single(x => x.Name.ToUpperCaseCultureInvariant() == "FOREGROUND");
			MapDataHelper.Layer midgroundLayer = layers.SingleOrDefault(x => x.Name.ToUpperCaseCultureInvariant() == "MIDGROUND");
			MapDataHelper.Layer backgroundLayer = layers.Single(x => x.Name.ToUpperCaseCultureInvariant() == "BACKGROUND");

			int numberOfTileColumns = solidLayer.Width;
			int numberOfTileRows = solidLayer.Height;

			Tuple<int, int> tuxLocation = null;
			Dictionary<MapKey, Tuple<int, int>> keyLocations = new Dictionary<MapKey, Tuple<int, int>>();
			foreach (MapKey mapKey in Enum.GetValues(typeof(MapKey)))
			{
				keyLocations[mapKey] = null;
			}

			Sprite[][] backgroundSpritesArray = SpriteUtil.EmptySpriteArray(length1: numberOfTileColumns, length2: numberOfTileRows);
			Sprite[][] midgroundSpritesArray = SpriteUtil.EmptySpriteArray(length1: numberOfTileColumns, length2: numberOfTileRows);
			Sprite[][] foregroundSpritesArray = SpriteUtil.EmptySpriteArray(length1: numberOfTileColumns, length2: numberOfTileRows);
			bool[][] isGroundArray = ArrayUtil.EmptyBoolArray(length1: numberOfTileColumns, length2: numberOfTileRows);
			bool[][] isKillZoneArray = ArrayUtil.EmptyBoolArray(length1: numberOfTileColumns, length2: numberOfTileRows);
			bool[][] isSpikesArray = ArrayUtil.EmptyBoolArray(length1: numberOfTileColumns, length2: numberOfTileRows);
			bool[][] isEndOfLevelArray = ArrayUtil.EmptyBoolArray(length1: numberOfTileColumns, length2: numberOfTileRows);
			bool[][] isCutsceneArray = ArrayUtil.EmptyBoolArray(length1: numberOfTileColumns, length2: numberOfTileRows);
			Dictionary<MapKey, bool[][]> isKeyTileArrays = new Dictionary<MapKey, bool[][]>();
			foreach (MapKey mapKey in Enum.GetValues(typeof(MapKey)))
			{
				isKeyTileArrays[mapKey] = ArrayUtil.EmptyBoolArray(length1: numberOfTileColumns, length2: numberOfTileRows);
			}

			IReadOnlyList<int> solidLayerData = solidLayer.Data;
			IReadOnlyList<int> foregroundLayerData = foregroundLayer.Data;
			IReadOnlyList<int> midgroundLayerData = midgroundLayer != null ? midgroundLayer.Data : null;
			IReadOnlyList<int> backgroundLayerData = backgroundLayer.Data;

			List<Tilemap.EnemySpawnLocation> enemies = new List<Tilemap.EnemySpawnLocation>();

			List<MapDataHelper.Tileset> tilesetsAfterActorsTileset = tilesets.Where(x => x.FirstGid > actorsTileset.FirstGid).ToList();
			int? actorTilesetLastGid;
			if (tilesetsAfterActorsTileset.Count == 0)
				actorTilesetLastGid = null;
			else
				actorTilesetLastGid = tilesetsAfterActorsTileset.OrderBy(x => x.FirstGid).First().FirstGid - 1;

			Dictionary<int, Sprite> gidToSpriteCache = new Dictionary<int, Sprite>();

			int dataIndex = 0;
			for (int j = numberOfTileRows - 1; j >= 0; j--)
			{
				for (int i = 0; i < numberOfTileColumns; i++)
				{
					int solidGid = solidLayerData[dataIndex];
					int foregroundGid = foregroundLayerData[dataIndex];
					int midgroundGid = midgroundLayerData != null ? midgroundLayerData[dataIndex] : 0;
					int backgroundGid = backgroundLayerData[dataIndex];
					dataIndex++;

					if (backgroundGid != 0)
					{
						if (gidToSpriteCache.ContainsKey(backgroundGid))
							backgroundSpritesArray[i][j] = gidToSpriteCache[backgroundGid];
						else
						{
							gidToSpriteCache[backgroundGid] = GetSprite(tilesets: tilesets, gid: backgroundGid, scalingFactorScaled: scalingFactorScaled);
							backgroundSpritesArray[i][j] = gidToSpriteCache[backgroundGid];
						}
					}

					if (midgroundGid != 0)
					{
						if (gidToSpriteCache.ContainsKey(midgroundGid))
							midgroundSpritesArray[i][j] = gidToSpriteCache[midgroundGid];
						else
						{
							gidToSpriteCache[midgroundGid] = GetSprite(tilesets: tilesets, gid: midgroundGid, scalingFactorScaled: scalingFactorScaled);
							midgroundSpritesArray[i][j] = gidToSpriteCache[midgroundGid];
						}
					}

					if (foregroundGid != 0)
					{
						if (foregroundGid >= actorsTileset.FirstGid && (actorTilesetLastGid == null || foregroundGid <= actorTilesetLastGid.Value))
						{
							int actorGidNormalized = foregroundGid - actorsTileset.FirstGid;
							if (actorGidNormalized == 0)
								tuxLocation = new Tuple<int, int>(
									item1: i * solidTileset.TileWidth * (scalingFactorScaled / 128),
									item2: j * solidTileset.TileHeight * (scalingFactorScaled / 128) + 16 * (scalingFactorScaled / 128));
							else if (actorGidNormalized == 48)
							{
								keyLocations[MapKey.Copper] = new Tuple<int, int>(
									item1: i * solidTileset.TileWidth * (scalingFactorScaled / 128) + 8 * (scalingFactorScaled / 128),
									item2: j * solidTileset.TileHeight * (scalingFactorScaled / 128) + 8 * (scalingFactorScaled / 128));
							}
							else if (actorGidNormalized == 49)
							{
								keyLocations[MapKey.Silver] = new Tuple<int, int>(
									item1: i * solidTileset.TileWidth * (scalingFactorScaled / 128) + 8 * (scalingFactorScaled / 128),
									item2: j * solidTileset.TileHeight * (scalingFactorScaled / 128) + 8 * (scalingFactorScaled / 128));
							}
							else if (actorGidNormalized == 50)
							{
								keyLocations[MapKey.Gold] = new Tuple<int, int>(
									item1: i * solidTileset.TileWidth * (scalingFactorScaled / 128) + 8 * (scalingFactorScaled / 128),
									item2: j * solidTileset.TileHeight * (scalingFactorScaled / 128) + 8 * (scalingFactorScaled / 128));
							}
							else if (actorGidNormalized == 51)
							{
								keyLocations[MapKey.Mythril] = new Tuple<int, int>(
									item1: i * solidTileset.TileWidth * (scalingFactorScaled / 128) + 8 * (scalingFactorScaled / 128),
									item2: j * solidTileset.TileHeight * (scalingFactorScaled / 128) + 8 * (scalingFactorScaled / 128));
							}
							else if (actorGidNormalized == 52)
								isKeyTileArrays[MapKey.Copper][i][j] = true;
							else if (actorGidNormalized == 53)
								isKeyTileArrays[MapKey.Silver][i][j] = true;
							else if (actorGidNormalized == 54)
								isKeyTileArrays[MapKey.Gold][i][j] = true;
							else if (actorGidNormalized == 55)
								isKeyTileArrays[MapKey.Mythril][i][j] = true;
							else
								enemies.Add(new Tilemap.EnemySpawnLocation(
									actorId: actorGidNormalized,
									tileI: i,
									tileJ: j,
									enemyId: enemyIdGenerator.GetNewId()));
						}
						else
						{
							if (gidToSpriteCache.ContainsKey(foregroundGid))
								foregroundSpritesArray[i][j] = gidToSpriteCache[foregroundGid];
							else
							{
								gidToSpriteCache[foregroundGid] = GetSprite(tilesets: tilesets, gid: foregroundGid, scalingFactorScaled: scalingFactorScaled);
								foregroundSpritesArray[i][j] = gidToSpriteCache[foregroundGid];
							}
						}
					}

					if (solidGid != 0)
					{
						int solidGidNormalized = solidGid - solidTileset.FirstGid;

						isGroundArray[i][j] = solidGidNormalized == 0;
						isKillZoneArray[i][j] = solidGidNormalized == 41;
						isSpikesArray[i][j] = solidGidNormalized == 43;
						isEndOfLevelArray[i][j] = solidGidNormalized == 37;
						isCutsceneArray[i][j] = solidGidNormalized == 29;
					}
				}
			}

			isKeyTileArrays = OptimizeIsKeyTileArrays(isKeyTileArrays: isKeyTileArrays);

			return new Tilemap(
				backgroundSpritesArray: backgroundSpritesArray,
				midgroundSpritesArray: midgroundSpritesArray,
				foregroundSpritesArray: foregroundSpritesArray,
				isGroundArray: isGroundArray,
				isKillZoneArray: isKillZoneArray,
				isSpikesArray: isSpikesArray,
				isEndOfLevelArray: isEndOfLevelArray,
				isCutsceneArray: isCutsceneArray,
				checkpointArray: ComputeCheckpointArray(
					numberOfTileColumns: numberOfTileColumns,
					numberOfTileRows: numberOfTileRows,
					solidLayerData: solidLayerData,
					solidTileset: solidTileset,
					actorsTileset: actorsTileset,
					scalingFactorScaled: scalingFactorScaled),
				isKeyTileArrays: isKeyTileArrays,
				tileWidth: solidTileset.TileWidth * scalingFactorScaled / 128,
				tileHeight: solidTileset.TileHeight * scalingFactorScaled / 128,
				enemies: enemies,
				cutsceneName: cutsceneName,
				tuxLocation: tuxLocation,
				keyLocations: keyLocations,
				gameMusic: gameMusic,
				extraEnemiesToSpawn: new List<Tilemap.IExtraEnemyToSpawn>());
		}

		private static bool[][] OptimizeIsKeyTileArray(bool[][] isKeyTileArray)
		{
			bool[][] newArray = new bool[isKeyTileArray.Length][];

			bool containsAtLeastOneNonNullSubArray = false;

			for (int i = 0; i < newArray.Length; i++)
			{
				bool containsAtLeastOneTrueValue = false;
				for (int j = 0; j < isKeyTileArray[i].Length; j++)
				{
					if (isKeyTileArray[i][j])
					{
						containsAtLeastOneTrueValue = true;
						break;
					}
				}

				if (containsAtLeastOneTrueValue)
				{
					containsAtLeastOneNonNullSubArray = true;

					newArray[i] = new bool[isKeyTileArray[i].Length];
					for (int j = 0; j < isKeyTileArray[i].Length; j++)
						newArray[i][j] = isKeyTileArray[i][j];
				}
				else
				{
					newArray[i] = null;
				}
			}

			if (!containsAtLeastOneNonNullSubArray)
				return null;

			return newArray;
		}

		private static Dictionary<MapKey, bool[][]> OptimizeIsKeyTileArrays(Dictionary<MapKey, bool[][]> isKeyTileArrays)
		{
			Dictionary<MapKey, bool[][]> returnValue = new Dictionary<MapKey, bool[][]>();

			foreach (KeyValuePair<MapKey, bool[][]> kvp in isKeyTileArrays)
			{
				returnValue[kvp.Key] = OptimizeIsKeyTileArray(isKeyTileArray: kvp.Value);
			}

			return returnValue;
		}

		private static Tuple<int, int>[][] ComputeCheckpointArray(
			int numberOfTileColumns,
			int numberOfTileRows,
			IReadOnlyList<int> solidLayerData,
			MapDataHelper.Tileset solidTileset,
			MapDataHelper.Tileset actorsTileset,
			int scalingFactorScaled)
		{
			Tuple<int, int>[][] checkpointArray = new Tuple<int, int>[numberOfTileColumns][];
			int[][] solidLayerGids = new int[numberOfTileColumns][];

			for (int i = 0; i < numberOfTileColumns; i++)
			{
				checkpointArray[i] = new Tuple<int, int>[numberOfTileRows];
				for (int j = 0; j < numberOfTileRows; j++)
					checkpointArray[i][j] = null;

				solidLayerGids[i] = new int[numberOfTileRows];
				for (int j = 0; j < numberOfTileRows; j++)
					solidLayerGids[i][j] = 0;
			}

			int dataIndex = 0;
			for (int j = numberOfTileRows - 1; j >= 0; j--)
			{
				for (int i = 0; i < numberOfTileColumns; i++)
				{
					int solidGid = solidLayerData[dataIndex];
					dataIndex++;

					solidLayerGids[i][j] = solidGid;
				}
			}

			for (int i = 0; i < solidLayerGids.Length; i++)
			{
				for (int j = 0; j < solidLayerGids[i].Length; j++)
				{
					if (solidLayerGids[i][j] - actorsTileset.FirstGid == 72)
					{
						SetCheckpoint(
							checkpointArray: checkpointArray,
							solidLayerGids: solidLayerGids,
							actorsTileset: actorsTileset,
							checkpointDestination: new Tuple<int, int>(
								item1: i * solidTileset.TileWidth * (scalingFactorScaled / 128) + solidTileset.TileWidth * (scalingFactorScaled / 128) / 2,
								item2: j * solidTileset.TileHeight * (scalingFactorScaled / 128) + 16 * (scalingFactorScaled / 128)),
							i: i,
							j: j);
					}
				}
			}

			return checkpointArray;
		}

		private static void SetCheckpoint(
			Tuple<int, int>[][] checkpointArray,
			int[][] solidLayerGids,
			MapDataHelper.Tileset actorsTileset,
			Tuple<int, int> checkpointDestination,
			int i,
			int j)
		{
			if (i < 0 || i >= checkpointArray.Length)
				return;

			if (j < 0 || j >= checkpointArray[i].Length)
				return;

			int normalizedGid = solidLayerGids[i][j] - actorsTileset.FirstGid;

			if (normalizedGid != 72 && normalizedGid != 32)
				return;

			if (checkpointArray[i][j] != null)
				return;

			checkpointArray[i][j] = checkpointDestination;

			SetCheckpoint(checkpointArray: checkpointArray, solidLayerGids: solidLayerGids, actorsTileset: actorsTileset, checkpointDestination: checkpointDestination, i: i, j: j - 1);
			SetCheckpoint(checkpointArray: checkpointArray, solidLayerGids: solidLayerGids, actorsTileset: actorsTileset, checkpointDestination: checkpointDestination, i: i, j: j + 1);
			SetCheckpoint(checkpointArray: checkpointArray, solidLayerGids: solidLayerGids, actorsTileset: actorsTileset, checkpointDestination: checkpointDestination, i: i - 1, j: j);
			SetCheckpoint(checkpointArray: checkpointArray, solidLayerGids: solidLayerGids, actorsTileset: actorsTileset, checkpointDestination: checkpointDestination, i: i + 1, j: j);
		}

		private static Sprite GetSprite(
			IReadOnlyList<MapDataHelper.Tileset> tilesets,
			int gid,
			int scalingFactorScaled)
		{
			Dictionary<string, GameImage> tilesetToGameImageMapping = new Dictionary<string, GameImage>();
			tilesetToGameImageMapping["Actors"] = GameImage.Actors;
			tilesetToGameImageMapping["Igloo"] = GameImage.Igloo;
			tilesetToGameImageMapping["Signpost"] = GameImage.Signpost;
			tilesetToGameImageMapping["Solid"] = GameImage.Solid;
			tilesetToGameImageMapping["Spikes"] = GameImage.Spikes;
			tilesetToGameImageMapping["TsSnow"] = GameImage.TilemapSnow;
			tilesetToGameImageMapping["TsCastle"] = GameImage.TilemapCastle;

			MapDataHelper.Tileset tileset = tilesets.Where(x => x.FirstGid <= gid)
				.OrderByDescending(x => x.FirstGid)
				.First();

			GameImage image = tilesetToGameImageMapping[tileset.Name];

			int tilesetX = 0;
			int tilesetY = 0;

			gid = gid - tileset.FirstGid;

			int numSpritesInEachRow = tileset.ImageWidth / tileset.TileWidth;

			while (gid >= numSpritesInEachRow)
			{
				gid -= numSpritesInEachRow;
				tilesetY += tileset.TileHeight;
			}

			while (gid > 0)
			{
				gid--;
				tilesetX += tileset.TileWidth;
			}

			return new Sprite(
				image: image,
				x: tilesetX,
				y: tilesetY,
				width: tileset.TileWidth,
				height: tileset.TileHeight,
				scalingFactorScaled: scalingFactorScaled);
		}
	}
}
