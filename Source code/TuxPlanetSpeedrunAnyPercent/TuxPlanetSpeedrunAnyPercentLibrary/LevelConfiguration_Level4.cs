
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class LevelConfiguration_Level4 : ILevelConfiguration
	{
		private List<CompositeTilemap.TilemapWithOffset> normalizedTilemaps;

		public LevelConfiguration_Level4(
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			IDTDeterministicRandom random)
		{
			List<CompositeTilemap.TilemapWithOffset> unnormalizedTilemaps = ConstructUnnormalizedTilemaps(
				mapInfo: mapInfo,
				random: random);

			this.normalizedTilemaps = new List<CompositeTilemap.TilemapWithOffset>(CompositeTilemap.NormalizeTilemaps(tilemaps: unnormalizedTilemaps));
		}

		private static List<CompositeTilemap.TilemapWithOffset> ConstructUnnormalizedTilemaps(
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			IDTDeterministicRandom random)
		{
			EnemyIdGenerator enemyIdGenerator = new EnemyIdGenerator();

			List<CompositeTilemap.TilemapWithOffset> list = new List<CompositeTilemap.TilemapWithOffset>();

			ITilemap startTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level4A_Start"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 128 * 3);

			CompositeTilemap.TilemapWithOffset startTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: startTilemap,
				xOffset: 0,
				yOffset: 0);

			list.Add(startTilemapWithOffset);

			ITilemap level4bTilemap1 = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level4B_Segment1"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 128 * 3);

			ITilemap level4bTilemap2 = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level4B_Segment2"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 128 * 3);

			CompositeTilemap.TilemapWithOffset level4bTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: random.NextBool() ? level4bTilemap1 : level4bTilemap2,
				xOffset: 0,
				yOffset: startTilemap.GetHeight());

			list.Add(level4bTilemapWithOffset);

			ITilemap cutsceneTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level4C_Cutscene"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: CutsceneProcessing.TIME_SLOWDOWN_CUTSCENE,
					scalingFactorScaled: 128 * 3);

			CompositeTilemap.TilemapWithOffset cutsceneTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: cutsceneTilemap,
				xOffset: 0,
				yOffset: level4bTilemapWithOffset.YOffset + level4bTilemap1.GetHeight());

			list.Add(cutsceneTilemapWithOffset);

			ITilemap level4dTilemap1 = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level4D_Segment1"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 128 * 3);

			ITilemap level4dTilemap2 = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level4D_Segment2"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 128 * 3);

			CompositeTilemap.TilemapWithOffset level4dTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: random.NextBool() ? level4dTilemap1 : level4dTilemap2,
				xOffset: 0,
				yOffset: cutsceneTilemapWithOffset.YOffset + cutsceneTilemap.GetHeight());

			list.Add(level4dTilemapWithOffset);

			ITilemap finishTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level4E_Finish"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 128 * 3);

			CompositeTilemap.TilemapWithOffset finishTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: finishTilemap,
				xOffset: 0,
				yOffset: level4dTilemapWithOffset.YOffset + level4dTilemap1.GetHeight());

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
