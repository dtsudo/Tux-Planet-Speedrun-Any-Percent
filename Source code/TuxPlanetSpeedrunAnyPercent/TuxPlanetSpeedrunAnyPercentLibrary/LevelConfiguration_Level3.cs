
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class LevelConfiguration_Level3 : ILevelConfiguration
	{
		private List<CompositeTilemap.TilemapWithOffset> normalizedTilemaps;
		private bool shouldSpawnRemoveKonqi;
		private IBackground background;

		public LevelConfiguration_Level3(
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			bool canAlreadyUseTeleport,
			IDTDeterministicRandom random)
		{
			List<CompositeTilemap.TilemapWithOffset> unnormalizedTilemaps = ConstructUnnormalizedTilemaps(
				mapInfo: mapInfo,
				canAlreadyUseTeleport: canAlreadyUseTeleport,
				random: random);

			this.shouldSpawnRemoveKonqi = canAlreadyUseTeleport;

			this.normalizedTilemaps = new List<CompositeTilemap.TilemapWithOffset>(CompositeTilemap.NormalizeTilemaps(tilemaps: unnormalizedTilemaps));

			this.background = BackgroundUtil.GetRandomBackground(random: random);
		}

		private static List<CompositeTilemap.TilemapWithOffset> ConstructUnnormalizedTilemaps(
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			bool canAlreadyUseTeleport,
			IDTDeterministicRandom random)
		{
			GameMusic gameMusic = LevelConfigurationHelper.GetRandomGameMusic(random: random);

			EnemyIdGenerator enemyIdGenerator = new EnemyIdGenerator();

			List<CompositeTilemap.TilemapWithOffset> list = new List<CompositeTilemap.TilemapWithOffset>();

			Tilemap startTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level3A_Start"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);

			CompositeTilemap.TilemapWithOffset startTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: startTilemap,
				xOffset: 0,
				yOffset: 0,
				alwaysIncludeTilemap: false);

			list.Add(startTilemapWithOffset);

			Tilemap dropTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level3B_Drop"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);

			CompositeTilemap.TilemapWithOffset dropTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: dropTilemap,
				xOffset: startTilemap.GetWidth() + random.NextInt(10) * 16 * 3,
				yOffset: -30 * 16 * 3,
				alwaysIncludeTilemap: false);

			list.Add(dropTilemapWithOffset);

			Tilemap cutsceneTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level3C_Cutscene"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: CutsceneProcessing.TELEPORT_CUTSCENE,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);

			CompositeTilemap.TilemapWithOffset cutsceneTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: canAlreadyUseTeleport ? Tilemap.GetTilemapWithoutCutscene(tilemap: cutsceneTilemap) : cutsceneTilemap,
				xOffset: dropTilemapWithOffset.XOffset + dropTilemap.GetWidth() + random.NextInt(10) * 16 * 3,
				yOffset: dropTilemapWithOffset.YOffset - 30 * 16 * 3,
				alwaysIncludeTilemap: false);

			list.Add(cutsceneTilemapWithOffset);

			Tilemap finishTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level3D_Finish"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);

			CompositeTilemap.TilemapWithOffset finishTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: finishTilemap,
				xOffset: cutsceneTilemapWithOffset.XOffset + cutsceneTilemap.GetWidth(),
				yOffset: cutsceneTilemapWithOffset.YOffset - 29 * 16 * 3,
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
