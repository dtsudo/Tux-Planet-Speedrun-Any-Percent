
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class LevelConfiguration_Level5 : ILevelConfiguration
	{
		private List<CompositeTilemap.TilemapWithOffset> normalizedTilemaps;
		private int endingXMibi;
		private IBackground background;

		public LevelConfiguration_Level5(
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo, 
			IDTDeterministicRandom random)
		{
			Tuple<List<CompositeTilemap.TilemapWithOffset>, int> result = ConstructUnnormalizedTilemaps(
				mapInfo: mapInfo,
				random: random);

			List<CompositeTilemap.TilemapWithOffset> unnormalizedTilemaps = result.Item1;

			this.normalizedTilemaps = new List<CompositeTilemap.TilemapWithOffset>(CompositeTilemap.NormalizeTilemaps(tilemaps: unnormalizedTilemaps));

			this.endingXMibi = result.Item2;

			this.background = BackgroundUtil.GetRandomBackground(random: random);
		}

		private static Tuple<List<CompositeTilemap.TilemapWithOffset>, int> ConstructUnnormalizedTilemaps(
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			IDTDeterministicRandom random)
		{
			GameMusic gameMusic = LevelConfigurationHelper.GetRandomGameMusic(random: random);

			EnemyIdGenerator enemyIdGenerator = new EnemyIdGenerator();

			List<CompositeTilemap.TilemapWithOffset> list = new List<CompositeTilemap.TilemapWithOffset>();

			CompositeTilemap.TilemapWithOffset startTilemap = new CompositeTilemap.TilemapWithOffset(
				tilemap: MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level5A_Start"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic),
				xOffset: 0,
				yOffset: 0,
				alwaysIncludeTilemap: false);

			list.Add(startTilemap);

			int xOffset = startTilemap.Tilemap.GetWidth() + (3 + random.NextInt(5)) * 16 * 3;

			int yOffset = 3 * 16 * 3;

			while (true)
			{
				if (xOffset >= 400 * 16 * 3)
					break;

				int numberOfFragmentTilemaps = 12;
				string mapInfoName = "Level5B_Fragment" + (random.NextInt(numberOfFragmentTilemaps) + 1).ToStringCultureInvariant();

				Tilemap fragmentTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo[mapInfoName],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);

				CompositeTilemap.TilemapWithOffset fragmentTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
					tilemap: fragmentTilemap,
					xOffset: xOffset,
					yOffset: yOffset,
				alwaysIncludeTilemap: false);

				list.Add(fragmentTilemapWithOffset);

				xOffset += fragmentTilemap.GetWidth() + (3 + random.NextInt(5)) * 16 * 3;

				if (yOffset == 10 * 16 * 3)
					yOffset += (random.NextInt(3) - 2) * 16 * 3;
				else if (yOffset == 0)
					yOffset += random.NextInt(3) * 16 * 3;
				else
					yOffset += (random.NextInt(5) - 2) * 16 * 3;

				if (yOffset > 10 * 16 * 3)
					yOffset = 10 * 16 * 3;

				if (yOffset < 0)
					yOffset = 0;
			}

			CompositeTilemap.TilemapWithOffset finishTilemap = new CompositeTilemap.TilemapWithOffset(
				tilemap: MapDataTilemapGenerator.GetTilemap(
					data: mapInfo["Level5C_Finish"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic),
				xOffset: xOffset,
				yOffset: 0,
				alwaysIncludeTilemap: false);

			list.Add(finishTilemap);

			int endingXMibi = (finishTilemap.XOffset - 16 * 3 / 2) * 1024;

			return new Tuple<List<CompositeTilemap.TilemapWithOffset>, int>(list, endingXMibi);
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

			return new Level5Tilemap(
				mapTilemap: tilemap,
				endingXMibi: this.endingXMibi);
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
