﻿
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class MapDataTilemapGenerator
	{
		public static ITilemap GetTilemap(
			MapDataHelper.Map data,
			EnemyIdGenerator enemyIdGenerator,
			string cutsceneName,
			int scalingFactorScaled)
		{
			IReadOnlyList<MapDataHelper.Tileset> tilesets = data.Tilesets;

			MapDataHelper.Tileset solidTileset = tilesets.Single(x => x.Name.ToUpperCaseCultureInvariant() == "SOLID");
			MapDataHelper.Tileset actorsTileset = tilesets.Single(x => x.Name.ToUpperCaseCultureInvariant() == "ACTORS");

			IReadOnlyList<MapDataHelper.Layer> layers = data.Layers;

			MapDataHelper.Layer solidLayer = layers.Single(x => x.Name.ToUpperCaseCultureInvariant() == "SOLID");
			MapDataHelper.Layer foregroundLayer = layers.Single(x => x.Name.ToUpperCaseCultureInvariant() == "FOREGROUND");
			MapDataHelper.Layer backgroundLayer = layers.Single(x => x.Name.ToUpperCaseCultureInvariant() == "BACKGROUND");

			int numberOfTileColumns = solidLayer.Width;
			int numberOfTileRows = solidLayer.Height;

			Tuple<int, int> tuxLocation = null;

			Sprite[][] backgroundSpritesArray = SpriteUtil.EmptySpriteArray(length1: numberOfTileColumns, length2: numberOfTileRows);
			Sprite[][] foregroundSpritesArray = SpriteUtil.EmptySpriteArray(length1: numberOfTileColumns, length2: numberOfTileRows);
			bool[][] isGroundArray = ArrayUtil.EmptyBoolArray(length1: numberOfTileColumns, length2: numberOfTileRows);
			bool[][] isKillZoneArray = ArrayUtil.EmptyBoolArray(length1: numberOfTileColumns, length2: numberOfTileRows);
			bool[][] isSpikesArray = ArrayUtil.EmptyBoolArray(length1: numberOfTileColumns, length2: numberOfTileRows);
			bool[][] isEndOfLevelArray = ArrayUtil.EmptyBoolArray(length1: numberOfTileColumns, length2: numberOfTileRows);
			bool[][] isCutsceneArray = ArrayUtil.EmptyBoolArray(length1: numberOfTileColumns, length2: numberOfTileRows);

			IReadOnlyList<int> solidLayerData = solidLayer.Data;
			IReadOnlyList<int> foregroundLayerData = foregroundLayer.Data;
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

					if (foregroundGid != 0)
					{
						if (foregroundGid >= actorsTileset.FirstGid && (actorTilesetLastGid == null || foregroundGid <= actorTilesetLastGid.Value))
						{
							int actorGidNormalized = foregroundGid - actorsTileset.FirstGid;
							if (actorGidNormalized == 0)
								tuxLocation = new Tuple<int, int>(
									item1: i * solidTileset.TileWidth * (scalingFactorScaled / 128),
									item2: j * solidTileset.TileHeight * (scalingFactorScaled / 128) + 16 * (scalingFactorScaled / 128));
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

			return new Tilemap(
				backgroundSpritesArray: backgroundSpritesArray,
				foregroundSpritesArray: foregroundSpritesArray,
				isGroundArray: isGroundArray,
				isKillZoneArray: isKillZoneArray,
				isSpikesArray: isSpikesArray,
				isEndOfLevelArray: isEndOfLevelArray,
				isCutsceneArray: isCutsceneArray,
				tileWidth: solidTileset.TileWidth * scalingFactorScaled / 128,
				tileHeight: solidTileset.TileHeight * scalingFactorScaled / 128,
				enemies: enemies,
				cutsceneName: cutsceneName,
				tuxLocation: tuxLocation);
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
