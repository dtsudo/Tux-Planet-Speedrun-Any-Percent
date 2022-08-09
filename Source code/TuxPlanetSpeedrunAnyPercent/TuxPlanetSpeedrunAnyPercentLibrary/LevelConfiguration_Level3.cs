
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class LevelConfiguration_Level3 : ILevelConfiguration
	{
		private List<CompositeTilemap.TilemapWithOffset> normalizedTilemaps;

		public LevelConfiguration_Level3(
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			bool canAlreadyUseTeleport,
			IDTDeterministicRandom random)
		{
			List<CompositeTilemap.TilemapWithOffset> unnormalizedTilemaps = ConstructUnnormalizedTilemaps(
				mapInfo: mapInfo,
				canAlreadyUseTeleport: canAlreadyUseTeleport,
				random: random);

			if (canAlreadyUseTeleport)
				unnormalizedTilemaps.Add(new CompositeTilemap.TilemapWithOffset(
					tilemap: new SpawnRemoveKonqiTilemap(),
					xOffset: 0,
					yOffset: 0,
					alwaysIncludeTilemap: true));

			this.normalizedTilemaps = new List<CompositeTilemap.TilemapWithOffset>(CompositeTilemap.NormalizeTilemaps(tilemaps: unnormalizedTilemaps));
		}

		private static List<CompositeTilemap.TilemapWithOffset> ConstructUnnormalizedTilemaps(
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			bool canAlreadyUseTeleport,
			IDTDeterministicRandom random)
		{
			GameMusic gameMusic = LevelConfigurationHelper.GetRandomGameMusic(random: random);

			EnemyIdGenerator enemyIdGenerator = new EnemyIdGenerator();

			List<CompositeTilemap.TilemapWithOffset> list = new List<CompositeTilemap.TilemapWithOffset>();

			ITilemap startTilemap = MapDataTilemapGenerator.GetTilemap(
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

			ITilemap dropTilemap = MapDataTilemapGenerator.GetTilemap(
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

			ITilemap cutsceneTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level3C_Cutscene"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: CutsceneProcessing.TELEPORT_CUTSCENE,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);

			CompositeTilemap.TilemapWithOffset cutsceneTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: canAlreadyUseTeleport ? new NoCutsceneWrappedTilemap(tilemap: cutsceneTilemap) : cutsceneTilemap,
				xOffset: dropTilemapWithOffset.XOffset + dropTilemap.GetWidth() + random.NextInt(10) * 16 * 3,
				yOffset: dropTilemapWithOffset.YOffset - 30 * 16 * 3,
				alwaysIncludeTilemap: false);

			list.Add(cutsceneTilemapWithOffset);

			ITilemap finishTilemap = MapDataTilemapGenerator.GetTilemap(
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

		public IBackground GetBackground()
		{
			return new Background_Ocean();
		}

		public ITilemap GetTilemap(int? tuxX, int? tuxY, int windowWidth, int windowHeight)
		{
			return LevelConfigurationHelper.GetTilemap(
				normalizedTilemaps: this.normalizedTilemaps,
				tuxX: tuxX,
				tuxY: tuxY,
				windowWidth: windowWidth,
				windowHeight: windowHeight);
		}
	}
}
