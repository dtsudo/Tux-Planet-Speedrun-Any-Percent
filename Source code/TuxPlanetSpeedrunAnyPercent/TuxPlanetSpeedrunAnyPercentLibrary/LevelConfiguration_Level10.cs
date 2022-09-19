
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class LevelConfiguration_Level10 : ILevelConfiguration
	{
		private List<CompositeTilemap.TilemapWithOffset> normalizedTilemaps;

		private IReadOnlyDictionary<string, string> customLevelInfo;

		private IBackground background;

		private const string LEVEL_SUBFOLDER = "Level10/";

		public LevelConfiguration_Level10(
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo, 
			IDTDeterministicRandom random)
		{
			Tuple<List<CompositeTilemap.TilemapWithOffset>, IReadOnlyDictionary<string, string>> result = ConstructUnnormalizedTilemaps(
				mapInfo: mapInfo,
				random: random);

			List<CompositeTilemap.TilemapWithOffset> unnormalizedTilemaps = result.Item1;

			this.normalizedTilemaps = new List<CompositeTilemap.TilemapWithOffset>(CompositeTilemap.NormalizeTilemaps(tilemaps: unnormalizedTilemaps));

			this.customLevelInfo = new Dictionary<string, string>(result.Item2);

			this.background = BackgroundUtil.GetRandomBackground(random: random);
		}

		// custom level info
		public const string BOSS_ROOM_X_OFFSET_START = "level10_bossRoomXOffsetStart";
		public const string BOSS_ROOM_X_OFFSET_END = "level10_bossRoomXOffsetEnd";
		public const string KONQI_BOSS_RNG_SEED = "level10_konqiBossRngSeed";
		public const string YETI_BOSS_RNG_SEED = "level10_yetiBossRngSeed";

		// level flags
		public const string BEGIN_KONQI_DEFEATED_CUTSCENE = "level10_beginKonqiDefeatedCutscene";

		public const string SPAWN_KONQI_BOSS_DEFEAT = "level10_spawnKonqiBossDefeat";
		public const string SPAWN_MYTHRIL_KEY = "level10_spawnMythrilKey";

		public const string LOCK_CAMERA_ON_KONQI_BOSS_ROOM = "level10_lockCameraOnKonqiBossRoom";
		public const string STOP_LOCKING_CAMERA_ON_KONQI_BOSS_ROOM = "level10_stopLockingCameraOnKonqiBossRoom";

		public const string LOCK_CAMERA_ON_KONQI_DEFEATED_BOSS_ROOM = "level10_lockCameraOnKonqiDefeatedBossRoom";
		public const string STOP_LOCKING_CAMERA_ON_KONQI_DEFEATED_BOSS_ROOM = "level10_stopLockingCameraOnKonqiDefeatedBossRoom";

		public const string LOCK_CAMERA_ON_YETI_BOSS_ROOM = "level10_lockCameraOnYetiBossRoom";
		public const string STOP_LOCKING_CAMERA_ON_YETI_BOSS_ROOM = "level10_stopLockingCameraOnYetiBossRoom";

		public const string SET_CAMERA_TO_YETI_DEFEATED_LOGIC = "level10_setCameraToYetiDefeatedLogic";

		public const string MARK_LEFT_AND_RIGHT_WALLS_OF_BOSS_ROOM_AS_GROUND = "level10_markLeftAndRightWallsOfBossRoomAsGround";
		public const string STOP_MARKING_LEFT_AND_RIGHT_WALLS_OF_BOSS_ROOM_AS_GROUND = "level10_stopMarkingLeftAndRightWallsOfBossRoomAsGround";

		public const string START_PLAYING_KONQI_BOSS_MUSIC = "level10_startPlayingKonqiBossMusic";
		public const string STOP_PLAYING_KONQI_BOSS_MUSIC = "level10_stopPlayingKonqiBossMusic";

		public const string START_PLAYING_YETI_BOSS_MUSIC = "level10_startPlayingYetiBossMusic";
		public const string STOP_PLAYING_YETI_BOSS_MUSIC = "level10_stopPlayingYetiBossMusic";

		public const string BEGIN_YETI_INTRO_CUTSCENE = "level10_beginYetiIntroCutscene";
		public const string MARK_YETI_FLOOR_AS_GROUND = "level10_markYetiFloorAsGround";
		public const string CREATE_CHECKPOINT_AFTER_DEFEATING_KONQI = "level10_createCheckpointAfterDefeatingKonqi";
		public const string BEGIN_YETI_DEFEATED_CUTSCENE = "level10_beginYetiDefeatedCutscene";
		public const string CONTINUOUSLY_RENDER_KONQI_BLOCKS = "level10_continuouslyRenderKonqiBlocks";

		public static CameraState GetKonqiBossRoomCameraState(
			IReadOnlyDictionary<string, string> customLevelInfo,
			ITilemap tilemap,
			int windowWidth,
			int windowHeight)
		{
			int bossRoomXOffset = StringUtil.ParseInt(customLevelInfo[BOSS_ROOM_X_OFFSET_START]);

			return CameraState.GetCameraState(
				x: bossRoomXOffset - 48 + (windowWidth >> 1),
				y: windowHeight >> 1);
		}

		public static CameraState GetKonqiDefeatedCameraState(
			IReadOnlyDictionary<string, string> customLevelInfo,
			ITilemap tilemap,
			int windowWidth,
			int windowHeight)
		{
			CameraState konqiBossRoomCameraState = GetKonqiBossRoomCameraState(
				customLevelInfo: customLevelInfo,
				tilemap: tilemap,
				windowWidth: windowWidth,
				windowHeight: windowHeight);

			return CameraState.GetCameraState(
				x: konqiBossRoomCameraState.X,
				y: konqiBossRoomCameraState.Y + 48 * 2);
		}

		public static CameraState GetYetiBossRoomCameraState(
			IReadOnlyDictionary<string, string> customLevelInfo,
			ITilemap tilemap,
			int windowWidth,
			int windowHeight)
		{
			CameraState konqiBossRoomCameraState = GetKonqiBossRoomCameraState(
				customLevelInfo: customLevelInfo,
				tilemap: tilemap,
				windowWidth: windowWidth,
				windowHeight: windowHeight);

			return CameraState.GetCameraState(
				x: konqiBossRoomCameraState.X,
				y: konqiBossRoomCameraState.Y + 48 * 17);
		}

		public static CameraState GetYetiBossDefeatedCameraState(
			IReadOnlyDictionary<string, string> customLevelInfo,
			ITilemap tilemap,
			int effectiveTuxXMibi,
			int effectiveTuxYMibi,
			int windowWidth,
			int windowHeight)
		{
			CameraState cameraState = CameraStateProcessing.ComputeCameraState(
				tuxXMibi: effectiveTuxXMibi,
				tuxYMibi: effectiveTuxYMibi,
				tuxTeleportStartingLocation: null,
				tuxTeleportInProgressElapsedMicros: null,
				tilemap: tilemap,
				windowWidth: windowWidth,
				windowHeight: windowHeight);

			CameraState yetiBossRoomCameraState = GetYetiBossRoomCameraState(
				customLevelInfo: customLevelInfo,
				tilemap: tilemap,
				windowWidth: windowWidth,
				windowHeight: windowHeight);

			int x = cameraState.X;
			int y = yetiBossRoomCameraState.Y;

			if (cameraState.X < yetiBossRoomCameraState.X)
				x = yetiBossRoomCameraState.X;

			int maximumCameraX = tilemap.GetWidth() - (windowWidth >> 1) - 48 * 2;

			if (x > maximumCameraX)
				x = maximumCameraX;

			return CameraState.GetCameraState(x: x, y: y);
		}

		private static Tuple<List<CompositeTilemap.TilemapWithOffset>, IReadOnlyDictionary<string, string>> ConstructUnnormalizedTilemaps(
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			IDTDeterministicRandom random)
		{
			GameMusic gameMusic = GameMusic.Jewels;

			Dictionary<string, string> customLevelInfo = new Dictionary<string, string>();

			EnemyIdGenerator enemyIdGenerator = new EnemyIdGenerator();

			List<CompositeTilemap.TilemapWithOffset> list = new List<CompositeTilemap.TilemapWithOffset>();

			Tilemap startTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo[LEVEL_SUBFOLDER + "A_Start"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);

			List<Tilemap.IExtraEnemyToSpawn> startTilemapExtraEnemies = new List<Tilemap.IExtraEnemyToSpawn>();

			for (int i = 0; i < 7; i++)
				startTilemapExtraEnemies.Add(new EnemySnail.EnemySnailSpawn(
					xMibi: (20 + 10 * i) * 48 * 1024,
					yMibi: (37 * 48 + 24) * 1024,
					isFacingRight: false,
					enemyId: "startTilemap_snail[" + i.ToStringCultureInvariant() + "]"));

			for (int i = 0; i < 7; i++)
			{
				startTilemapExtraEnemies.Add(new EnemySnail.EnemySnailSpawn(
					xMibi: (51 + 5 * i) * 48 * 1024,
					yMibi: (35 * 48 + 24) * 1024,
					isFacingRight: false,
					enemyId: "startTilemap_snailBottom[" + i.ToStringCultureInvariant() + "]"));
				startTilemapExtraEnemies.Add(new EnemySnail.EnemySnailSpawn(
					xMibi: (51 + 5 * i) * 48 * 1024,
					yMibi: (45 * 48 + 24) * 1024,
					isFacingRight: false,
					enemyId: "startTilemap_snailTop[" + i.ToStringCultureInvariant() + "]"));
			}

			for (int i = 0; i < 24; i++)
			{
				startTilemapExtraEnemies.Add(new EnemyLevel10EliteSnailPassive.EnemyLevel10EliteSnailSpawn(
					xMibi: (86 + 5 * i) * 48 * 1024,
					yMibi: (35 * 48 + 24) * 1024,
					activationRadiusInPixels: 300 + random.NextInt(250),
					activationDelayInMicroseconds: random.NextInt(200 * 1000),
					shouldInitiallyTeleportUpward: true,
					maxXMibi: startTilemap.GetWidth() << 10,
					enemyId: "startTilemap_eliteSnailBottom[" + i.ToStringCultureInvariant() + "]"));
				startTilemapExtraEnemies.Add(new EnemyLevel10EliteSnailPassive.EnemyLevel10EliteSnailSpawn(
					xMibi: (86 + 5 * i) * 48 * 1024,
					yMibi: (45 * 48 + 24) * 1024,
					activationRadiusInPixels: 400 + random.NextInt(150),
					activationDelayInMicroseconds: random.NextInt(200 * 1000),
					shouldInitiallyTeleportUpward: false,
					maxXMibi: startTilemap.GetWidth() << 10,
					enemyId: "startTilemap_eliteSnailTop[" + i.ToStringCultureInvariant() + "]"));
			}

			startTilemap = Tilemap.GetTilemapWithExtraEnemiesToSpawn(
				tilemap: startTilemap,
				extraEnemiesToSpawn: startTilemapExtraEnemies);

			CompositeTilemap.TilemapWithOffset startTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: startTilemap,
				xOffset: 0,
				yOffset: 0,
				alwaysIncludeTilemap: false);

			list.Add(startTilemapWithOffset);

			Tilemap checkpointTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo[LEVEL_SUBFOLDER + "B_Checkpoint"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: CutsceneProcessing.KONQI_BOSS_INTRO_CUTSCENE,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);

			CompositeTilemap.TilemapWithOffset checkpointTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: checkpointTilemap,
				xOffset: startTilemapWithOffset.XOffset + startTilemap.GetWidth(),
				yOffset: startTilemapWithOffset.YOffset,
				alwaysIncludeTilemap: false);

			list.Add(checkpointTilemapWithOffset);

			customLevelInfo[BOSS_ROOM_X_OFFSET_START] = (checkpointTilemapWithOffset.XOffset + checkpointTilemap.GetWidth()).ToStringCultureInvariant();

			CompositeTilemap.TilemapWithOffset bossTilemap = new CompositeTilemap.TilemapWithOffset(
				tilemap: MapDataTilemapGenerator.GetTilemap(
					data: mapInfo[LEVEL_SUBFOLDER + "C_Boss"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic),
				xOffset: checkpointTilemapWithOffset.XOffset + checkpointTilemap.GetWidth(),
				yOffset: checkpointTilemapWithOffset.YOffset,
				alwaysIncludeTilemap: false);

			list.Add(bossTilemap);

			customLevelInfo[BOSS_ROOM_X_OFFSET_END] = (bossTilemap.XOffset + bossTilemap.Tilemap.GetWidth()).ToStringCultureInvariant();

			CompositeTilemap.TilemapWithOffset finishTilemap = new CompositeTilemap.TilemapWithOffset(
				tilemap: MapDataTilemapGenerator.GetTilemap(
					data: mapInfo[LEVEL_SUBFOLDER + "D_Finish"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic),
				xOffset: bossTilemap.XOffset + bossTilemap.Tilemap.GetWidth(),
				yOffset: bossTilemap.YOffset,
				alwaysIncludeTilemap: false);

			list.Add(finishTilemap);

			DTDeterministicRandom konqiRandom = new DTDeterministicRandom(seed: 0);
			for (int i = 0; i < 40; i++)
			{
				int jEnd = random.NextInt(3) + 1;
				for (int j = 0; j < jEnd; j++)
					konqiRandom.AddSeed(random.NextInt(100));
			}
			customLevelInfo[KONQI_BOSS_RNG_SEED] = konqiRandom.SerializeToString();

			random.AddSeed(17);
			random.NextBool();
			customLevelInfo[YETI_BOSS_RNG_SEED] = random.SerializeToString();
			random.AddSeed(17);
			random.NextBool();

			return new Tuple<List<CompositeTilemap.TilemapWithOffset>, IReadOnlyDictionary<string, string>>(
				item1: list,
				item2: customLevelInfo);
		}

		public IReadOnlyDictionary<string, string> GetCustomLevelInfo()
		{
			return this.customLevelInfo;
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

			tilemap = new EnemyCreationTilemap(
				mapTilemap: tilemap,
				enemiesUnaffectedByXOffsetAndYOffset: new List<IEnemy>()
				{
					new EnemyLevel10Coordinator(
						bossRoomXOffsetStart: this.customLevelInfo[BOSS_ROOM_X_OFFSET_START].ParseAsIntCultureInvariant(),
						bossRoomXOffsetEnd: this.customLevelInfo[BOSS_ROOM_X_OFFSET_END].ParseAsIntCultureInvariant())
				});

			tilemap = new Level10CoordinatorTilemap(
				mapTilemap: tilemap,
				levelFlags: levelFlags,
				bossRoomXOffsetStart: this.customLevelInfo[BOSS_ROOM_X_OFFSET_START].ParseAsIntCultureInvariant(),
				bossRoomXOffsetEnd: this.customLevelInfo[BOSS_ROOM_X_OFFSET_END].ParseAsIntCultureInvariant());

			return tilemap;
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
			if (levelFlags.Contains(LOCK_CAMERA_ON_KONQI_BOSS_ROOM) && !levelFlags.Contains(STOP_LOCKING_CAMERA_ON_KONQI_BOSS_ROOM))
			{
				return GetKonqiBossRoomCameraState(
					customLevelInfo: this.GetCustomLevelInfo(),
					tilemap: tilemap,
					windowWidth: windowWidth,
					windowHeight: windowHeight);
			}

			if (levelFlags.Contains(LOCK_CAMERA_ON_KONQI_DEFEATED_BOSS_ROOM) && !levelFlags.Contains(STOP_LOCKING_CAMERA_ON_KONQI_DEFEATED_BOSS_ROOM))
			{
				return GetKonqiDefeatedCameraState(
					customLevelInfo: this.GetCustomLevelInfo(),
					tilemap: tilemap,
					windowWidth: windowWidth,
					windowHeight: windowHeight);
			}

			if (levelFlags.Contains(LOCK_CAMERA_ON_YETI_BOSS_ROOM) && !levelFlags.Contains(STOP_LOCKING_CAMERA_ON_YETI_BOSS_ROOM))
			{
				return GetYetiBossRoomCameraState(
					customLevelInfo: this.GetCustomLevelInfo(),
					tilemap: tilemap,
					windowWidth: windowWidth,
					windowHeight: windowHeight);
			}

			if (levelFlags.Contains(SET_CAMERA_TO_YETI_DEFEATED_LOGIC))
			{
				int effectiveTuxXMibi = tuxXMibi;
				int effectiveTuxYMibi = tuxYMibi;

				if (tuxTeleportInProgressElapsedMicros != null)
				{
					long deltaX = tuxXMibi - tuxTeleportStartingLocation.Item1;
					long deltaY = tuxYMibi - tuxTeleportStartingLocation.Item2;

					effectiveTuxXMibi = (int)(tuxTeleportStartingLocation.Item1 + deltaX * tuxTeleportInProgressElapsedMicros.Value / TuxState.TELEPORT_DURATION);
					effectiveTuxYMibi = (int)(tuxTeleportStartingLocation.Item2 + deltaY * tuxTeleportInProgressElapsedMicros.Value / TuxState.TELEPORT_DURATION);
				}

				return GetYetiBossDefeatedCameraState(
					customLevelInfo: this.GetCustomLevelInfo(),
					tilemap: tilemap,
					effectiveTuxXMibi: effectiveTuxXMibi,
					effectiveTuxYMibi: effectiveTuxYMibi,
					windowWidth: windowWidth,
					windowHeight: windowHeight);
			}

			if ((tuxXMibi >> 10) < this.normalizedTilemaps[0].Tilemap.GetWidth())
			{
				CameraState cameraState = CameraStateProcessing.ComputeCameraState(
					tuxXMibi: tuxXMibi,
					tuxYMibi: tuxYMibi,
					tuxTeleportStartingLocation: tuxTeleportStartingLocation,
					tuxTeleportInProgressElapsedMicros: tuxTeleportInProgressElapsedMicros,
					tilemap: tilemap,
					windowWidth: windowWidth,
					windowHeight: windowHeight);

				return CameraState.GetCameraState(
					x: cameraState.X,
					y: (windowHeight >> 1) + 34 * 48);
			}

			if ((tuxXMibi >> 10) < this.normalizedTilemaps[0].Tilemap.GetWidth() + 14 * 48)
			{
				CameraState cameraState = CameraStateProcessing.ComputeCameraState(
					tuxXMibi: tuxXMibi,
					tuxYMibi: tuxYMibi,
					tuxTeleportStartingLocation: tuxTeleportStartingLocation,
					tuxTeleportInProgressElapsedMicros: tuxTeleportInProgressElapsedMicros,
					tilemap: tilemap,
					windowWidth: windowWidth,
					windowHeight: windowHeight);

				int destinationCameraY = cameraState.Y;
				int originalCameraY = (windowHeight >> 1) + 34 * 48;

				int ratioTimes1000;

				int tuxY = tuxYMibi >> 10;

				if (tuxY >= 37 * 48)
					ratioTimes1000 = 0;
				else if (tuxY <= 23 * 48)
					ratioTimes1000 = 1000;
				else
					ratioTimes1000 = (37 * 48 - tuxY) * 1000 / (14 * 48);

				return CameraState.GetCameraState(
					x: cameraState.X,
					y: destinationCameraY * ratioTimes1000 / 1000 + originalCameraY * (1000 - ratioTimes1000) / 1000);
			}

			return CameraStateProcessing.ComputeCameraState(
				tuxXMibi: tuxXMibi,
				tuxYMibi: tuxYMibi,
				tuxTeleportStartingLocation: tuxTeleportStartingLocation,
				tuxTeleportInProgressElapsedMicros: tuxTeleportInProgressElapsedMicros,
				tilemap: tilemap,
				windowWidth: windowWidth,
				windowHeight: windowHeight);
		}
	}
}
