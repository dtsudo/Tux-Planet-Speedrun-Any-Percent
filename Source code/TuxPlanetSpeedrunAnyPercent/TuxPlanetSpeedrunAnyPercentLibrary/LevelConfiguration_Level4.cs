
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class LevelConfiguration_Level4 : ILevelConfiguration
	{
		private List<CompositeTilemap.TilemapWithOffset> normalizedTilemaps;
		private bool shouldSpawnRemoveKonqi;
		private IBackground background;

		public LevelConfiguration_Level4(
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			bool canAlreadyUseTimeSlowdown,
			IDTDeterministicRandom random)
		{
			List<CompositeTilemap.TilemapWithOffset> unnormalizedTilemaps = ConstructUnnormalizedTilemaps(
				mapInfo: mapInfo,
				canAlreadyUseTimeSlowdown: canAlreadyUseTimeSlowdown,
				random: random);

			this.shouldSpawnRemoveKonqi = canAlreadyUseTimeSlowdown;

			this.normalizedTilemaps = new List<CompositeTilemap.TilemapWithOffset>(CompositeTilemap.NormalizeTilemaps(tilemaps: unnormalizedTilemaps));

			this.background = BackgroundUtil.GetRandomBackground(random: random);
		}

		private static List<CompositeTilemap.TilemapWithOffset> ConstructUnnormalizedTilemaps(
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			bool canAlreadyUseTimeSlowdown,
			IDTDeterministicRandom random)
		{
			GameMusic gameMusic = LevelConfigurationHelper.GetRandomGameMusic(random: random);

			EnemyIdGenerator enemyIdGenerator = new EnemyIdGenerator();

			List<CompositeTilemap.TilemapWithOffset> list = new List<CompositeTilemap.TilemapWithOffset>();

			Tilemap startTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level4A_Start"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 128 * 3,
					gameMusic: gameMusic);

			CompositeTilemap.TilemapWithOffset startTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: startTilemap,
				xOffset: 0,
				yOffset: 0,
				alwaysIncludeTilemap: false);

			list.Add(startTilemapWithOffset);

			Tilemap level4bTilemap1 = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level4B_Segment1"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 128 * 3,
					gameMusic: gameMusic);

			Tilemap level4bTilemap2 = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level4B_Segment2"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 128 * 3,
					gameMusic: gameMusic);

			CompositeTilemap.TilemapWithOffset level4bTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: random.NextBool() ? level4bTilemap1 : level4bTilemap2,
				xOffset: 0,
				yOffset: startTilemap.GetHeight(),
				alwaysIncludeTilemap: false);

			list.Add(level4bTilemapWithOffset);

			Tilemap cutsceneTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level4C_Cutscene"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: CutsceneProcessing.TIME_SLOWDOWN_CUTSCENE,
					scalingFactorScaled: 128 * 3,
					gameMusic: gameMusic);

			CompositeTilemap.TilemapWithOffset cutsceneTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: canAlreadyUseTimeSlowdown ? Tilemap.GetTilemapWithoutCutscene(tilemap: cutsceneTilemap) : cutsceneTilemap,
				xOffset: 0,
				yOffset: level4bTilemapWithOffset.YOffset + level4bTilemap1.GetHeight(),
				alwaysIncludeTilemap: false);

			list.Add(cutsceneTilemapWithOffset);

			Tilemap level4dTilemap1 = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level4D_Segment1"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 128 * 3,
					gameMusic: gameMusic);

			Tilemap level4dTilemap2 = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level4D_Segment2"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 128 * 3,
					gameMusic: gameMusic);

			CompositeTilemap.TilemapWithOffset level4dTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: random.NextBool() ? level4dTilemap1 : level4dTilemap2,
				xOffset: 0,
				yOffset: cutsceneTilemapWithOffset.YOffset + cutsceneTilemap.GetHeight(),
				alwaysIncludeTilemap: false);

			list.Add(level4dTilemapWithOffset);

			Tilemap finishTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level4E_Finish"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 128 * 3,
					gameMusic: gameMusic);

			CompositeTilemap.TilemapWithOffset finishTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: finishTilemap,
				xOffset: 0,
				yOffset: level4dTilemapWithOffset.YOffset + level4dTilemap1.GetHeight(),
				alwaysIncludeTilemap: false);

			list.Add(finishTilemapWithOffset);

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
