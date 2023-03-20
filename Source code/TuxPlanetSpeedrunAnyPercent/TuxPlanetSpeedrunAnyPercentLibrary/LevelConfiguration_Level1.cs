
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class LevelConfiguration_Level1 : ILevelConfiguration
	{
		private List<CompositeTilemap.TilemapWithOffset> normalizedTilemaps;
		private IBackground background;

		private const string LEVEL_SUBFOLDER = "Level1/";

		public LevelConfiguration_Level1(
			Difficulty difficulty,
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			IDTDeterministicRandom random)
		{
			List<CompositeTilemap.TilemapWithOffset> unnormalizedTilemaps = ConstructUnnormalizedTilemaps(difficulty: difficulty, mapInfo: mapInfo, random: random);

			this.normalizedTilemaps = new List<CompositeTilemap.TilemapWithOffset>(CompositeTilemap.NormalizeTilemaps(tilemaps: unnormalizedTilemaps));

			this.background = BackgroundUtil.GetRandomBackground(random: random);
		}

		private static List<CompositeTilemap.TilemapWithOffset> ConstructUnnormalizedTilemaps(
			Difficulty difficulty,
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo, 
			IDTDeterministicRandom random)
		{
			GameMusic gameMusic = GameMusic.Chipdisko;

			EnemyIdGenerator enemyIdGenerator = new EnemyIdGenerator();

			List<CompositeTilemap.TilemapWithOffset> list = new List<CompositeTilemap.TilemapWithOffset>();

			MapDataHelper.Map map;

			switch (difficulty)
			{
				case Difficulty.Easy:
					map = mapInfo[LEVEL_SUBFOLDER + "Level1_Easy"];
					break;
				case Difficulty.Normal:
					map = mapInfo[LEVEL_SUBFOLDER + "Level1_Normal"];
					break;
				case Difficulty.Hard:
					map = mapInfo[LEVEL_SUBFOLDER + "Level1_Hard"];
					break;
				default:
					throw new Exception();
			}

			CompositeTilemap.TilemapWithOffset level1TilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: MapDataTilemapGenerator.GetTilemap(
					data: map,
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic),
				xOffset: 0,
				yOffset: 0,
				alwaysIncludeTilemap: false);

			list.Add(level1TilemapWithOffset);

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

		public ITilemap GetTilemap(int? tuxX, int? tuxY, int? cameraX, int? cameraY, int windowWidth, int windowHeight, IReadOnlyList<string> levelFlags, MapKeyState mapKeyState)
		{
			return LevelConfigurationHelper.GetTilemap(
				normalizedTilemaps: this.normalizedTilemaps,
				tuxX: tuxX,
				tuxY: tuxY,
				cameraX: cameraX,
				cameraY: cameraY,
				mapKeyState: mapKeyState,
				windowWidth: windowWidth,
				windowHeight: windowHeight);
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
				y: windowHeight >> 1);
		}
	}
}
