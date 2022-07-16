
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class LevelConfiguration_Level2 : ILevelConfiguration
	{
		private List<CompositeTilemap.TilemapWithOffset> normalizedTilemaps;

		public LevelConfiguration_Level2(
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

			CompositeTilemap.TilemapWithOffset startTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2A_Start"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128),
				xOffset: 0,
				yOffset: 0);

			list.Add(startTilemapWithOffset);

			ITilemap chooseAPath1TilemapA = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2B_Drop1"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128);

			ITilemap chooseAPath1TilemapB = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2B_Drop2"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128);

			CompositeTilemap.TilemapWithOffset chooseAPath1TilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: random.NextBool() ? chooseAPath1TilemapA : chooseAPath1TilemapB,
				xOffset: 0,
				yOffset: -chooseAPath1TilemapA.GetHeight());

			list.Add(chooseAPath1TilemapWithOffset);

			ITilemap level2bPlatformTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2B_Platform"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128);

			CompositeTilemap.TilemapWithOffset level2bPlatformTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: level2bPlatformTilemap,
				xOffset: 0,
				yOffset: chooseAPath1TilemapWithOffset.YOffset - level2bPlatformTilemap.GetHeight());

			list.Add(level2bPlatformTilemapWithOffset);

			ITilemap chooseAPath2TilemapA = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2B_Drop1"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128);

			ITilemap chooseAPath2TilemapB = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2B_Drop2"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128);

			CompositeTilemap.TilemapWithOffset chooseAPath2TilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: random.NextBool() ? chooseAPath2TilemapA : chooseAPath2TilemapB,
				xOffset: 0,
				yOffset: level2bPlatformTilemapWithOffset.YOffset - chooseAPath2TilemapA.GetHeight());

			list.Add(chooseAPath2TilemapWithOffset);

			ITilemap level2cLowerFloorTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2C_LowerFloor"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128);

			CompositeTilemap.TilemapWithOffset level2cLowerFloorTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: level2cLowerFloorTilemap,
				xOffset: chooseAPath2TilemapWithOffset.XOffset,
				yOffset: chooseAPath2TilemapWithOffset.YOffset - level2cLowerFloorTilemap.GetHeight());

			list.Add(level2cLowerFloorTilemapWithOffset);

			ITilemap cutsceneTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2D_Cutscene"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: CutsceneProcessing.SAVESTATE_CUTSCENE,
					scalingFactorScaled: 3 * 128);

			CompositeTilemap.TilemapWithOffset cutsceneTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: cutsceneTilemap,
				xOffset: level2cLowerFloorTilemapWithOffset.XOffset + level2cLowerFloorTilemap.GetWidth(),
				yOffset: level2cLowerFloorTilemapWithOffset.YOffset);

			list.Add(cutsceneTilemapWithOffset);

			ITilemap level2eTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2E"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128);

			CompositeTilemap.TilemapWithOffset level2eTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: level2eTilemap,
				xOffset: cutsceneTilemapWithOffset.XOffset + cutsceneTilemap.GetWidth(),
				yOffset: cutsceneTilemapWithOffset.YOffset + 20 * 16 * 3);

			list.Add(level2eTilemapWithOffset);

			ITilemap level2fTilemapA = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2F_Drop1"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128);
			ITilemap level2fTilemapB = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2F_Drop2"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128);
			ITilemap level2fTilemapC = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2F_Drop3"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128);
			ITilemap level2fTilemapD = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2F_Drop4"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128);

			CompositeTilemap.TilemapWithOffset level2fTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: random.NextBool() ? (random.NextBool() ? level2fTilemapA : level2fTilemapB) : (random.NextBool() ? level2fTilemapC : level2fTilemapD),
				xOffset: level2eTilemapWithOffset.XOffset,
				yOffset: level2eTilemapWithOffset.YOffset - level2fTilemapA.GetHeight());

			list.Add(level2fTilemapWithOffset);

			ITilemap level2fPlatformTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2F_Platform"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128);

			CompositeTilemap.TilemapWithOffset level2fPlatformTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: level2fPlatformTilemap,
				xOffset: level2fTilemapWithOffset.XOffset,
				yOffset: level2fTilemapWithOffset.YOffset - level2fPlatformTilemap.GetHeight());

			list.Add(level2fPlatformTilemapWithOffset);

			ITilemap level2fTilemapA2 = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2F_Drop1"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128);
			ITilemap level2fTilemapB2 = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2F_Drop2"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128);
			ITilemap level2fTilemapC2 = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2F_Drop3"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128);
			ITilemap level2fTilemapD2 = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2F_Drop4"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128);

			CompositeTilemap.TilemapWithOffset level2fTilemapWithOffset2 = new CompositeTilemap.TilemapWithOffset(
				tilemap: random.NextBool() ? (random.NextBool() ? level2fTilemapA2 : level2fTilemapB2) : (random.NextBool() ? level2fTilemapC2 : level2fTilemapD2),
				xOffset: level2fPlatformTilemapWithOffset.XOffset,
				yOffset: level2fPlatformTilemapWithOffset.YOffset - level2fTilemapA2.GetHeight());

			list.Add(level2fTilemapWithOffset2);

			ITilemap finishTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level2G_Finish"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128);

			list.Add(new CompositeTilemap.TilemapWithOffset(
				tilemap: finishTilemap,
				xOffset: level2fTilemapWithOffset2.XOffset,
				yOffset: level2fTilemapWithOffset2.YOffset - finishTilemap.GetHeight()));

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
