
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class LevelConfiguration_Level2 : ILevelConfiguration
	{
		private List<CompositeTilemap.TilemapWithOffset> normalizedTilemaps;
		private bool shouldSpawnRemoveKonqi;
		private IBackground background;

		public LevelConfiguration_Level2(
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			bool canAlreadyUseSaveStates,
			IDTDeterministicRandom random)
		{
			List<CompositeTilemap.TilemapWithOffset> unnormalizedTilemaps = ConstructUnnormalizedTilemaps(
				mapInfo: mapInfo,
				canAlreadyUseSaveStates: canAlreadyUseSaveStates,
				random: random);

			this.shouldSpawnRemoveKonqi = canAlreadyUseSaveStates;

			this.normalizedTilemaps = new List<CompositeTilemap.TilemapWithOffset>(CompositeTilemap.NormalizeTilemaps(tilemaps: unnormalizedTilemaps));

			this.background = BackgroundUtil.GetRandomBackground(random: random);
		}

		private static List<CompositeTilemap.TilemapWithOffset> ConstructUnnormalizedTilemaps(
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			bool canAlreadyUseSaveStates,
			IDTDeterministicRandom random)
		{
			GameMusic gameMusic = LevelConfigurationHelper.GetRandomGameMusic(random: random);

			EnemyIdGenerator enemyIdGenerator = new EnemyIdGenerator();

			List<CompositeTilemap.TilemapWithOffset> list = new List<CompositeTilemap.TilemapWithOffset>();

			CompositeTilemap.TilemapWithOffset startTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2A_Start"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic),
				xOffset: 0,
				yOffset: 0,
				alwaysIncludeTilemap: false);

			list.Add(startTilemapWithOffset);

			Tilemap chooseAPath1TilemapA = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2B_Drop1"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);

			Tilemap chooseAPath1TilemapB = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2B_Drop2"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);

			CompositeTilemap.TilemapWithOffset chooseAPath1TilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: random.NextBool() ? chooseAPath1TilemapA : chooseAPath1TilemapB,
				xOffset: 0,
				yOffset: -chooseAPath1TilemapA.GetHeight(),
				alwaysIncludeTilemap: false);

			list.Add(chooseAPath1TilemapWithOffset);

			Tilemap level2bPlatformTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2B_Platform"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);

			CompositeTilemap.TilemapWithOffset level2bPlatformTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: level2bPlatformTilemap,
				xOffset: 0,
				yOffset: chooseAPath1TilemapWithOffset.YOffset - level2bPlatformTilemap.GetHeight(),
				alwaysIncludeTilemap: false);

			list.Add(level2bPlatformTilemapWithOffset);

			Tilemap chooseAPath2TilemapA = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2B_Drop1"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);

			Tilemap chooseAPath2TilemapB = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2B_Drop2"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);

			CompositeTilemap.TilemapWithOffset chooseAPath2TilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: random.NextBool() ? chooseAPath2TilemapA : chooseAPath2TilemapB,
				xOffset: 0,
				yOffset: level2bPlatformTilemapWithOffset.YOffset - chooseAPath2TilemapA.GetHeight(),
				alwaysIncludeTilemap: false);

			list.Add(chooseAPath2TilemapWithOffset);

			Tilemap level2cLowerFloorTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2C_LowerFloor"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);

			CompositeTilemap.TilemapWithOffset level2cLowerFloorTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: level2cLowerFloorTilemap,
				xOffset: chooseAPath2TilemapWithOffset.XOffset,
				yOffset: chooseAPath2TilemapWithOffset.YOffset - level2cLowerFloorTilemap.GetHeight(),
				alwaysIncludeTilemap: false);

			list.Add(level2cLowerFloorTilemapWithOffset);

			Tilemap cutsceneTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2D_Cutscene"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: CutsceneProcessing.SAVESTATE_CUTSCENE,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);

			CompositeTilemap.TilemapWithOffset cutsceneTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: canAlreadyUseSaveStates ? Tilemap.GetTilemapWithoutCutscene(tilemap: cutsceneTilemap) : cutsceneTilemap,
				xOffset: level2cLowerFloorTilemapWithOffset.XOffset + level2cLowerFloorTilemap.GetWidth(),
				yOffset: level2cLowerFloorTilemapWithOffset.YOffset,
				alwaysIncludeTilemap: false);

			list.Add(cutsceneTilemapWithOffset);

			Tilemap level2eTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2E"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);

			CompositeTilemap.TilemapWithOffset level2eTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: level2eTilemap,
				xOffset: cutsceneTilemapWithOffset.XOffset + cutsceneTilemap.GetWidth(),
				yOffset: cutsceneTilemapWithOffset.YOffset + 20 * 16 * 3,
				alwaysIncludeTilemap: false);

			list.Add(level2eTilemapWithOffset);

			Tilemap level2fTilemapA = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2F_Drop1"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);
			Tilemap level2fTilemapB = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2F_Drop2"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);
			Tilemap level2fTilemapC = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2F_Drop3"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);
			Tilemap level2fTilemapD = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2F_Drop4"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);

			CompositeTilemap.TilemapWithOffset level2fTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: random.NextBool() ? (random.NextBool() ? level2fTilemapA : level2fTilemapB) : (random.NextBool() ? level2fTilemapC : level2fTilemapD),
				xOffset: level2eTilemapWithOffset.XOffset,
				yOffset: level2eTilemapWithOffset.YOffset - level2fTilemapA.GetHeight(),
				alwaysIncludeTilemap: false);

			list.Add(level2fTilemapWithOffset);

			Tilemap level2fPlatformTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2F_Platform"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);

			CompositeTilemap.TilemapWithOffset level2fPlatformTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: level2fPlatformTilemap,
				xOffset: level2fTilemapWithOffset.XOffset,
				yOffset: level2fTilemapWithOffset.YOffset - level2fPlatformTilemap.GetHeight(),
				alwaysIncludeTilemap: false);

			list.Add(level2fPlatformTilemapWithOffset);

			Tilemap level2fTilemapA2 = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2F_Drop1"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);
			Tilemap level2fTilemapB2 = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2F_Drop2"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);
			Tilemap level2fTilemapC2 = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2F_Drop3"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);
			Tilemap level2fTilemapD2 = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2F_Drop4"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);

			CompositeTilemap.TilemapWithOffset level2fTilemapWithOffset2 = new CompositeTilemap.TilemapWithOffset(
				tilemap: random.NextBool() ? (random.NextBool() ? level2fTilemapA2 : level2fTilemapB2) : (random.NextBool() ? level2fTilemapC2 : level2fTilemapD2),
				xOffset: level2fPlatformTilemapWithOffset.XOffset,
				yOffset: level2fPlatformTilemapWithOffset.YOffset - level2fTilemapA2.GetHeight(),
				alwaysIncludeTilemap: false);

			list.Add(level2fTilemapWithOffset2);

			Tilemap finishTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2G_Finish"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);

			list.Add(new CompositeTilemap.TilemapWithOffset(
				tilemap: finishTilemap,
				xOffset: level2fTilemapWithOffset2.XOffset,
				yOffset: level2fTilemapWithOffset2.YOffset - finishTilemap.GetHeight(),
				alwaysIncludeTilemap: false));

			return list;
		}

		public IReadOnlyDictionary<string, string> GetCustomLevelInfo()
		{
			return new Dictionary<string, string>();
		}

		public IBackground GetBackground()
		{
			return this.background;
		}

		public ITilemap GetTilemap(int? tuxX, int? tuxY, int windowWidth, int windowHeight, IReadOnlyList<string> levelFlags, MapKeyState mapKeyState)
		{
			ITilemap tilemap = LevelConfigurationHelper.GetTilemap(
				normalizedTilemaps: this.normalizedTilemaps,
				tuxX: tuxX,
				tuxY: tuxY,
				mapKeyState: mapKeyState,
				windowWidth: windowWidth,
				windowHeight: windowHeight);

			if (this.shouldSpawnRemoveKonqi)
				tilemap = new SpawnRemoveKonqiTilemap(tilemap: tilemap);

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
			return null;
		}
	}
}
