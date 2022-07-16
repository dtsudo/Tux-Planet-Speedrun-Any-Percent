
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
					data: mapInfo["Level3A_Start"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128);

			CompositeTilemap.TilemapWithOffset startTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: startTilemap,
				xOffset: 0,
				yOffset: 0);

			list.Add(startTilemapWithOffset);

			ITilemap dropTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level3B_Drop"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128);

			CompositeTilemap.TilemapWithOffset dropTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: dropTilemap,
				xOffset: startTilemap.GetWidth() + random.NextInt(10) * 16 * 3,
				yOffset: -30 * 16 * 3);

			list.Add(dropTilemapWithOffset);

			ITilemap finishTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level3C_Finish"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128);

			CompositeTilemap.TilemapWithOffset finishTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: finishTilemap,
				xOffset: dropTilemapWithOffset.XOffset + dropTilemap.GetWidth() + random.NextInt(10) * 16 * 3,
				yOffset: dropTilemapWithOffset.YOffset - 30 * 16 * 3);

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
