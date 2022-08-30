
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class LevelConfiguration_Level6 : ILevelConfiguration
	{
		private List<CompositeTilemap.TilemapWithOffset> normalizedTilemaps;

		private IReadOnlyDictionary<string, string> customLevelInfo;

		private IBackground background;

		public LevelConfiguration_Level6(
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
		public const string BOSS_ROOM_X_OFFSET_START = "level6_bossRoomXOffsetStart";
		public const string BOSS_ROOM_X_OFFSET_END = "level6_bossRoomXOffsetEnd";
		public const string KONQI_BOSS_RNG_SEED = "level6_konqiBossRngSeed";

		// level flags
		public const string BEGIN_BOSS_BATTLE = "level6_beginBossBattle";
		public const string BOSS_DEFEATED = "level6_bossDefeated";
		public const string DESPAWN_KONQI_AND_REMOVE_BOSS_DOORS = "level6_despawnKonqiAndRemoveBossDoors";
		public const string BOSS_DEFEATED_RESTORE_DEFAULT_CAMERA = "level6_bossDefeatedRestoreDefaultCamera";

		public static CameraState GetBossRoomCameraState(
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

		private static Tuple<List<CompositeTilemap.TilemapWithOffset>, IReadOnlyDictionary<string, string>> ConstructUnnormalizedTilemaps(
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			IDTDeterministicRandom random)
		{
			GameMusic gameMusic = LevelConfigurationHelper.GetRandomGameMusic(random: random);

			Dictionary<string, string> customLevelInfo = new Dictionary<string, string>();

			EnemyIdGenerator enemyIdGenerator = new EnemyIdGenerator();

			List<CompositeTilemap.TilemapWithOffset> list = new List<CompositeTilemap.TilemapWithOffset>();

			/*
				The camera isn't always centered on Tux, and we don't want tilemaps to despawn if they're
				too far from Tux but still in view of the camera.
			*/
			bool alwaysIncludeTilemap = true;

			CompositeTilemap.TilemapWithOffset startTilemap = new CompositeTilemap.TilemapWithOffset(
				tilemap: MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level6A_Start"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: CutsceneProcessing.BOSS_CUTSCENE,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic),
				xOffset: 0,
				yOffset: 0,
				alwaysIncludeTilemap: alwaysIncludeTilemap);

			list.Add(startTilemap);

			customLevelInfo[BOSS_ROOM_X_OFFSET_START] = startTilemap.Tilemap.GetWidth().ToStringCultureInvariant();

			CompositeTilemap.TilemapWithOffset bossTilemap = new CompositeTilemap.TilemapWithOffset(
				tilemap: MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level6B_Boss"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic),
				xOffset: startTilemap.Tilemap.GetWidth(),
				yOffset: 0,
				alwaysIncludeTilemap: alwaysIncludeTilemap);

			list.Add(bossTilemap);

			customLevelInfo[BOSS_ROOM_X_OFFSET_END] = (bossTilemap.XOffset + bossTilemap.Tilemap.GetWidth()).ToStringCultureInvariant();

			CompositeTilemap.TilemapWithOffset finishTilemap = new CompositeTilemap.TilemapWithOffset(
				tilemap: MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level6C_Finish"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic),
				xOffset: bossTilemap.XOffset + bossTilemap.Tilemap.GetWidth(),
				yOffset: 0,
				alwaysIncludeTilemap: alwaysIncludeTilemap);

			list.Add(finishTilemap);

			DTDeterministicRandom konqiRandom = new DTDeterministicRandom();
			for (int i = 0; i < 40; i++)
			{
				int jEnd = random.NextInt(3) + 1;
				for (int j = 0; j < jEnd; j++)
					konqiRandom.AddSeed(random.NextInt(100));
			}
			customLevelInfo[KONQI_BOSS_RNG_SEED] = konqiRandom.SerializeToString();

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

		public ITilemap GetTilemap(int? tuxX, int? tuxY, int windowWidth, int windowHeight, IReadOnlyList<string> levelFlags, MapKeyState mapKeyState)
		{
			if (!levelFlags.Contains(BEGIN_BOSS_BATTLE))
				return LevelConfigurationHelper.GetTilemap(
					normalizedTilemaps: this.normalizedTilemaps,
					tuxX: tuxX,
					tuxY: tuxY,
					mapKeyState: mapKeyState,
					windowWidth: windowWidth,
					windowHeight: windowHeight);

			ITilemap boundedTilemap = LevelConfigurationHelper.GetTilemap(
				normalizedTilemaps: this.normalizedTilemaps,
				tuxX: tuxX,
				tuxY: tuxY,
				mapKeyState: mapKeyState,
				windowWidth: windowWidth,
				windowHeight: windowHeight);

			if (levelFlags.Contains(BOSS_DEFEATED))
				return new Level6Tilemap_BossDefeated(
					mapTilemap: boundedTilemap,
					bossRoomXOffsetStart: this.customLevelInfo[BOSS_ROOM_X_OFFSET_START].ParseAsIntCultureInvariant(),
					bossRoomXOffsetEnd: this.customLevelInfo[BOSS_ROOM_X_OFFSET_END].ParseAsIntCultureInvariant(),
					shouldRestrictToBossRoom: !levelFlags.Contains(DESPAWN_KONQI_AND_REMOVE_BOSS_DOORS));

			return new Level6Tilemap_BossBattle(
				mapTilemap: boundedTilemap,
				bossRoomXOffsetStart: this.customLevelInfo[BOSS_ROOM_X_OFFSET_START].ParseAsIntCultureInvariant(),
				bossRoomXOffsetEnd: this.customLevelInfo[BOSS_ROOM_X_OFFSET_END].ParseAsIntCultureInvariant());
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
			if (levelFlags.Contains(BEGIN_BOSS_BATTLE) && !levelFlags.Contains(BOSS_DEFEATED_RESTORE_DEFAULT_CAMERA))
			{
				return GetBossRoomCameraState(
					customLevelInfo: this.GetCustomLevelInfo(),
					tilemap: tilemap,
					windowWidth: windowWidth,
					windowHeight: windowHeight);
			}

			CameraState cameraState = CameraStateProcessing.ComputeCameraState(
				tuxXMibi: tuxXMibi,
				tuxYMibi: tuxYMibi,
				tuxTeleportStartingLocation: tuxTeleportStartingLocation,
				tuxTeleportInProgressElapsedMicros: tuxTeleportInProgressElapsedMicros,
				tilemap: tilemap,
				windowWidth: windowWidth,
				windowHeight: windowHeight);

			int maximumCameraX = tilemap.GetWidth() - (windowWidth >> 1) - 48 * 2;

			if (cameraState.X > maximumCameraX)
				cameraState = CameraState.GetCameraState(x: maximumCameraX, y: cameraState.Y);

			return cameraState;
		}
	}
}
