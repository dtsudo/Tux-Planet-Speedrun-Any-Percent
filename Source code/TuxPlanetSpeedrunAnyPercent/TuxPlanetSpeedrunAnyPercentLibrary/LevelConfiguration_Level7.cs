
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class LevelConfiguration_Level7 : ILevelConfiguration
	{
		private class EnemyFlyamanitaLevel7Spawn : Tilemap.IExtraEnemyToSpawn
		{
			private int xMibi;
			private int yMibi;
			private string enemyIdPrefix;

			public EnemyFlyamanitaLevel7Spawn(int xMibi, int yMibi, string enemyIdPrefix)
			{
				this.xMibi = xMibi;
				this.yMibi = yMibi;
				this.enemyIdPrefix = enemyIdPrefix;
			}

			public IEnemy GetEnemy(int xOffset, int yOffset)
			{
				return EnemyFlyamanitaLevel7.GetEnemyFlyamanitaLevel7(
					xMibi: this.xMibi + (xOffset << 10),
					yMibi: this.yMibi + (yOffset << 10),
					enemyIdPrefix: this.enemyIdPrefix,
					enemyIdCounter: 1);
			}
		}

		private List<CompositeTilemap.TilemapWithOffset> normalizedTilemaps;
		private IBackground background;

		private int yOffsetOfKeyTilemap;

		private const string LEVEL_SUBFOLDER = "Level7/";

		// level flags
		public const string HAS_OBTAINED_KEY = "level7_hasObtainedKey";

		public LevelConfiguration_Level7(
			Difficulty difficulty,
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			IDTDeterministicRandom random)
		{
			Tuple<List<CompositeTilemap.TilemapWithOffset>, int> result = ConstructUnnormalizedTilemaps(difficulty: difficulty, mapInfo: mapInfo, random: random);

			List<CompositeTilemap.TilemapWithOffset> unnormalizedTilemaps = result.Item1;

			this.normalizedTilemaps = new List<CompositeTilemap.TilemapWithOffset>(CompositeTilemap.NormalizeTilemaps(tilemaps: unnormalizedTilemaps));

			this.yOffsetOfKeyTilemap = result.Item2;

			this.background = new Background_Cave();
		}

		private static Tuple<List<CompositeTilemap.TilemapWithOffset>, int> ConstructUnnormalizedTilemaps(
			Difficulty difficulty,
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			IDTDeterministicRandom random)
		{
			GameMusic gameMusic = GameMusic.Chipdisko;

			EnemyIdGenerator enemyIdGenerator = new EnemyIdGenerator();

			List<CompositeTilemap.TilemapWithOffset> list = new List<CompositeTilemap.TilemapWithOffset>();

			CompositeTilemap.TilemapWithOffset startTilemap = new CompositeTilemap.TilemapWithOffset(
				tilemap: MapDataTilemapGenerator.GetTilemap(
					data: mapInfo[LEVEL_SUBFOLDER + "A_Start"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic),
				xOffset: 0,
				yOffset: 0,
				alwaysIncludeTilemap: false);

			list.Add(startTilemap);

			string difficultySuffix;

			switch (difficulty)
			{
				case Difficulty.Easy:
					difficultySuffix = "_Easy";
					break;
				case Difficulty.Normal:
					difficultySuffix = "_Normal";
					break;
				case Difficulty.Hard:
					difficultySuffix = "_Hard";
					break;
				default:
					throw new Exception();
			}

			List<MapDataHelper.Map> B_fragmentMaps = new List<MapDataHelper.Map>()
			{
				mapInfo[LEVEL_SUBFOLDER + "B_Fragment1" + difficultySuffix],
				mapInfo[LEVEL_SUBFOLDER + "B_Fragment2" + difficultySuffix]
			};

			B_fragmentMaps.Shuffle(random: random);

			foreach (MapDataHelper.Map map in B_fragmentMaps)
			{
				list.Add(new CompositeTilemap.TilemapWithOffset(
					tilemap: MapDataTilemapGenerator.GetTilemap(
						data: map,
						enemyIdGenerator: enemyIdGenerator,
						cutsceneName: null,
						scalingFactorScaled: 3 * 128,
						gameMusic: gameMusic),
					xOffset: list[list.Count - 1].XOffset,
					yOffset: list[list.Count - 1].YOffset + list[list.Count - 1].Tilemap.GetHeight(),
					alwaysIncludeTilemap: false));
			}

			int numConsecutiveFlyamanita;

			switch (difficulty)
			{
				case Difficulty.Easy:
					numConsecutiveFlyamanita = 1;
					break;
				case Difficulty.Normal:
					numConsecutiveFlyamanita = 2;
					break;
				case Difficulty.Hard:
					numConsecutiveFlyamanita = 4;
					break;
				default:
					throw new Exception();
			}

			for (int i = 0; i < numConsecutiveFlyamanita; i++)
			{
				Tilemap tilemapFlyamanita = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo[LEVEL_SUBFOLDER + "Flyamanita"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);

				int flyAmanitaXMibi = (random.NextInt(19) * 48 + 48 + 24) << 10;
				int flyAmanitaYMibi = (tilemapFlyamanita.GetHeight() >> 1) << 10;

				tilemapFlyamanita = Tilemap.GetTilemapWithExtraEnemiesToSpawn(
					tilemap: tilemapFlyamanita,
					extraEnemiesToSpawn: new List<Tilemap.IExtraEnemyToSpawn>()
					{
						new EnemyFlyamanitaLevel7Spawn(xMibi: flyAmanitaXMibi, yMibi: flyAmanitaYMibi, enemyIdPrefix: "flyAmanitaSpawn[" + i.ToStringCultureInvariant() + "]")
					});

				list.Add(new CompositeTilemap.TilemapWithOffset(
					tilemap: tilemapFlyamanita,
					xOffset: list[list.Count - 1].XOffset,
					yOffset: list[list.Count - 1].YOffset + list[list.Count - 1].Tilemap.GetHeight(),
					alwaysIncludeTilemap: false));
			}

			List<MapDataHelper.Map> C_fragmentMaps = new List<MapDataHelper.Map>()
			{
				mapInfo[LEVEL_SUBFOLDER + "C_Fragment1" + difficultySuffix],
				mapInfo[LEVEL_SUBFOLDER + "C_Fragment2" + difficultySuffix]
			};

			C_fragmentMaps.Shuffle(random: random);

			foreach (MapDataHelper.Map map in C_fragmentMaps)
			{
				list.Add(new CompositeTilemap.TilemapWithOffset(
					tilemap: MapDataTilemapGenerator.GetTilemap(
						data: map,
						enemyIdGenerator: enemyIdGenerator,
						cutsceneName: null,
						scalingFactorScaled: 3 * 128,
						gameMusic: gameMusic),
					xOffset: list[list.Count - 1].XOffset,
					yOffset: list[list.Count - 1].YOffset + list[list.Count - 1].Tilemap.GetHeight(),
					alwaysIncludeTilemap: false));
			}

			for (int i = 0; i < numConsecutiveFlyamanita; i++)
			{
				Tilemap tilemapFlyamanita = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo[LEVEL_SUBFOLDER + "Flyamanita"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);

				int flyAmanitaXMibi = (random.NextInt(19) * 48 + 48 + 24) << 10;
				int flyAmanitaYMibi = (tilemapFlyamanita.GetHeight() >> 1) << 10;

				tilemapFlyamanita = Tilemap.GetTilemapWithExtraEnemiesToSpawn(
					tilemap: tilemapFlyamanita,
					extraEnemiesToSpawn: new List<Tilemap.IExtraEnemyToSpawn>()
					{
						new EnemyFlyamanitaLevel7Spawn(xMibi: flyAmanitaXMibi, yMibi: flyAmanitaYMibi, enemyIdPrefix: "flyAmanitaSpawn2[" + i.ToStringCultureInvariant() + "]")
					});

				list.Add(new CompositeTilemap.TilemapWithOffset(
					tilemap: tilemapFlyamanita,
					xOffset: list[list.Count - 1].XOffset,
					yOffset: list[list.Count - 1].YOffset + list[list.Count - 1].Tilemap.GetHeight(),
					alwaysIncludeTilemap: false));
			}

			list.Add(new CompositeTilemap.TilemapWithOffset(
				tilemap: MapDataTilemapGenerator.GetTilemap(
					data: mapInfo[LEVEL_SUBFOLDER + "D_Key" + difficultySuffix],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic),
				xOffset: list[list.Count - 1].XOffset,
				yOffset: list[list.Count - 1].YOffset + list[list.Count - 1].Tilemap.GetHeight(),
				alwaysIncludeTilemap: false));

			int yOffsetOfKeyTilemap = list[list.Count - 1].YOffset;

			return new Tuple<List<CompositeTilemap.TilemapWithOffset>, int>(list, yOffsetOfKeyTilemap);
		}

		public IReadOnlyDictionary<string, string> GetCustomLevelInfo()
		{
			return new Dictionary<string, string>();
		}

		public IBackground GetBackground()
		{
			return this.background;
		}

		public ITilemap GetTilemap(int? tuxX, int? tuxY, int? cameraX, int? cameraY, int windowWidth, int windowHeight, IReadOnlyList<string> levelFlags, MapKeyState mapKeyState)
		{
			ITilemap tilemap = LevelConfigurationHelper.GetTilemap(
				normalizedTilemaps: this.normalizedTilemaps,
				tuxX: tuxX,
				tuxY: tuxY,
				cameraX: cameraX,
				cameraY: cameraY,
				mapKeyState: mapKeyState,
				windowWidth: windowWidth,
				windowHeight: windowHeight);

			List<IEnemy> enemies = new List<IEnemy>();

			if (mapKeyState.CollectedKeys.Contains(MapKey.Gold))
				enemies.Add(new EnemyAddLevelFlag(
					levelFlag: HAS_OBTAINED_KEY,
					enemyId: "level7_addLevelFlagForDescent"));

			return new EnemyCreationTilemap(
				mapTilemap: tilemap,
				enemiesUnaffectedByXOffsetAndYOffset: enemies);
		}

		private static int GetMaximumCameraXAscending(
			int tuxYMibi,
			Tuple<int, int> tuxTeleportStartingLocation,
			int? tuxTeleportInProgressElapsedMicros,
			int windowWidth, 
			int windowHeight)
		{
			if (tuxTeleportInProgressElapsedMicros != null)
			{
				long deltaY = tuxYMibi - tuxTeleportStartingLocation.Item2;

				tuxYMibi = (int)(tuxTeleportStartingLocation.Item2 + deltaY * tuxTeleportInProgressElapsedMicros.Value / TuxState.TELEPORT_DURATION);
			}

			int y = tuxYMibi >> 10;

			int halfWindowWidth = windowWidth >> 1;

			int yStart = windowHeight;
			int yEnd = windowHeight * 3;

			if (y <= yStart)
				return windowWidth;

			if (y >= yEnd)
				return halfWindowWidth;

			int ratioTimes1000 = (yEnd - y) * 1000 / (yEnd - yStart);

			return halfWindowWidth + halfWindowWidth * ratioTimes1000 / 1000;
		}

		private static int? GetMaximumCameraXDescending(
			int tuxYMibi,
			Tuple<int, int> tuxTeleportStartingLocation,
			int? tuxTeleportInProgressElapsedMicros,
			int windowWidth,
			int windowHeight)
		{
			if (tuxTeleportInProgressElapsedMicros != null)
			{
				long deltaY = tuxYMibi - tuxTeleportStartingLocation.Item2;

				tuxYMibi = (int)(tuxTeleportStartingLocation.Item2 + deltaY * tuxTeleportInProgressElapsedMicros.Value / TuxState.TELEPORT_DURATION);
			}

			int y = tuxYMibi >> 10;

			int halfWindowWidth = windowWidth >> 1;

			int yStart = windowHeight;
			int yEnd = windowHeight * 3;

			if (y <= yStart)
				return null;

			if (y >= yEnd)
				return halfWindowWidth;

			int ratioTimes1000 = (yEnd - y) * 1000 / (yEnd - yStart);

			return halfWindowWidth + halfWindowWidth * ratioTimes1000 / 1000;
		}

		private static int GetCameraYOffsetAscending(
			int tuxYMibi,
			Tuple<int, int> tuxTeleportStartingLocation,
			int? tuxTeleportInProgressElapsedMicros,
			int yOffsetOfKeyTilemap,
			int windowWidth,
			int windowHeight)
		{
			if (tuxTeleportInProgressElapsedMicros != null)
			{
				long deltaY = tuxYMibi - tuxTeleportStartingLocation.Item2;

				tuxYMibi = (int)(tuxTeleportStartingLocation.Item2 + deltaY * tuxTeleportInProgressElapsedMicros.Value / TuxState.TELEPORT_DURATION);
			}

			int y = tuxYMibi >> 10;

			int maxCameraYOffset = windowHeight / 6;

			int yStart = windowHeight;
			int yEnd = windowHeight * 3;

			if (y <= yStart)
				return 0;

			int ratioTimes1000;

			if (y < yEnd)
			{
				ratioTimes1000 = (y - yStart) * 1000 / (yEnd - yStart);
				return maxCameraYOffset * ratioTimes1000 / 1000;
			}

			yStart = yOffsetOfKeyTilemap + 27 * 48;
			yEnd = yOffsetOfKeyTilemap + 37 * 48;

			if (y <= yStart)
				return maxCameraYOffset;

			if (y >= yEnd)
				return 0;

			ratioTimes1000 = (yEnd - y) * 1000 / (yEnd - yStart);
			return maxCameraYOffset * ratioTimes1000 / 1000;
		}

		private static int GetCameraYOffsetDescending(
			int tuxYMibi,
			Tuple<int, int> tuxTeleportStartingLocation,
			int? tuxTeleportInProgressElapsedMicros,
			int yOffsetOfKeyTilemap,
			int windowWidth,
			int windowHeight)
		{
			if (tuxTeleportInProgressElapsedMicros != null)
			{
				long deltaY = tuxYMibi - tuxTeleportStartingLocation.Item2;

				tuxYMibi = (int)(tuxTeleportStartingLocation.Item2 + deltaY * tuxTeleportInProgressElapsedMicros.Value / TuxState.TELEPORT_DURATION);
			}

			int y = tuxYMibi >> 10;

			int maxCameraYOffset = -windowHeight / 6;

			int yStart = yOffsetOfKeyTilemap + 37 * 48;
			int yEnd = yOffsetOfKeyTilemap + 27 * 48;

			if (y >= yStart)
				return 0;

			if (y > yEnd)
			{
				int ratioTimes1000 = (yStart - y) * 1000 / (yStart - yEnd);
				return maxCameraYOffset * ratioTimes1000 / 1000;
			}

			return maxCameraYOffset;
		}

		public CameraState GetCameraState(
			int tuxXMibi,
			int tuxYMibi,
			Tuple<int, int> tuxTeleportStartingLocation,
			int? tuxTeleportInProgressElapsedMicros,
			ITilemap tilemap,
			int windowWidth,
			int windowHeight,
			IReadOnlyList<string> levelFlags)
		{
			CameraState cameraState = CameraStateProcessing.ComputeCameraState(
				tuxXMibi: tuxXMibi,
				tuxYMibi: tuxYMibi,
				tuxTeleportStartingLocation: tuxTeleportStartingLocation,
				tuxTeleportInProgressElapsedMicros: tuxTeleportInProgressElapsedMicros,
				tilemap: tilemap,
				windowWidth: windowWidth,
				windowHeight: windowHeight);

			if (levelFlags.Contains(HAS_OBTAINED_KEY))
			{
				int? maximumCameraX = GetMaximumCameraXDescending(
					tuxYMibi: tuxYMibi,
					tuxTeleportStartingLocation: tuxTeleportStartingLocation,
					tuxTeleportInProgressElapsedMicros: tuxTeleportInProgressElapsedMicros,
					windowWidth: windowWidth,
					windowHeight: windowHeight);

				int cameraX;

				if (maximumCameraX.HasValue)
					cameraX = Math.Min(cameraState.X, maximumCameraX.Value);
				else
					cameraX = cameraState.X;

				int cameraYOffset = GetCameraYOffsetDescending(
					tuxYMibi: tuxYMibi,
					tuxTeleportStartingLocation: tuxTeleportStartingLocation,
					tuxTeleportInProgressElapsedMicros: tuxTeleportInProgressElapsedMicros,
					yOffsetOfKeyTilemap: this.yOffsetOfKeyTilemap,
					windowWidth: windowWidth,
					windowHeight: windowHeight);

				int cameraY = cameraState.Y + cameraYOffset;

				if (cameraY < (windowHeight >> 1))
					cameraY = windowHeight >> 1;

				return CameraState.GetCameraState(
					x: cameraX,
					y: cameraY);
			}
			else
			{
				int maximumCameraX = GetMaximumCameraXAscending(
					tuxYMibi: tuxYMibi,
					tuxTeleportStartingLocation: tuxTeleportStartingLocation,
					tuxTeleportInProgressElapsedMicros: tuxTeleportInProgressElapsedMicros,
					windowWidth: windowWidth,
					windowHeight: windowHeight);

				int cameraYOffset = GetCameraYOffsetAscending(
					tuxYMibi: tuxYMibi,
					tuxTeleportStartingLocation: tuxTeleportStartingLocation,
					tuxTeleportInProgressElapsedMicros: tuxTeleportInProgressElapsedMicros,
					yOffsetOfKeyTilemap: this.yOffsetOfKeyTilemap,
					windowWidth: windowWidth,
					windowHeight: windowHeight);

				return CameraState.GetCameraState(
					x: Math.Min(cameraState.X, maximumCameraX),
					y: cameraState.Y + cameraYOffset);
			}
		}
	}
}
