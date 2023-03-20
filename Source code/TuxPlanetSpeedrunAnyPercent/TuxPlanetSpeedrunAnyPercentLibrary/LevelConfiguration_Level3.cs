
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class LevelConfiguration_Level3 : ILevelConfiguration
	{
		private List<CompositeTilemap.TilemapWithOffset> normalizedTilemaps;
		private IBackground background;

		private const string LEVEL_SUBFOLDER = "Level3/";

		public LevelConfiguration_Level3(
			Difficulty difficulty,
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			IDTDeterministicRandom random)
		{
			List<CompositeTilemap.TilemapWithOffset> unnormalizedTilemaps = ConstructUnnormalizedTilemaps(
				difficulty: difficulty,
				mapInfo: mapInfo, 
				random: random);

			this.normalizedTilemaps = new List<CompositeTilemap.TilemapWithOffset>(CompositeTilemap.NormalizeTilemaps(tilemaps: unnormalizedTilemaps));

			this.background = new Background_Ocean();
		}

		private static List<CompositeTilemap.TilemapWithOffset> ConstructUnnormalizedTilemaps(
			Difficulty difficulty,
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			IDTDeterministicRandom random)
		{
			GameMusic gameMusic = GameMusic.Chipdisko;

			EnemyIdGenerator enemyIdGenerator = new EnemyIdGenerator();

			string difficultySuffix;

			switch (difficulty)
			{
				case Difficulty.Easy:
					difficultySuffix = "_Easy";
					break;
				case Difficulty.Normal:
					difficultySuffix = "_Normal";
					break;
				case Difficulty.Hard:
					difficultySuffix = "_Hard";
					break;
				default:
					throw new Exception();
			}

			List<CompositeTilemap.TilemapWithOffset> list = new List<CompositeTilemap.TilemapWithOffset>();

			CompositeTilemap.TilemapWithOffset startTilemap = new CompositeTilemap.TilemapWithOffset(
				tilemap: MapDataTilemapGenerator.GetTilemap(
					data: mapInfo[LEVEL_SUBFOLDER + "A_Start" + difficultySuffix],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic),
				xOffset: 0,
				yOffset: 0,
				alwaysIncludeTilemap: false);

			list.Add(startTilemap);

			List<MapDataHelper.Map> B_obstaclesMaps = new List<MapDataHelper.Map>()
			{
				mapInfo[LEVEL_SUBFOLDER + "B_Obstacles1" + difficultySuffix],
				mapInfo[LEVEL_SUBFOLDER + "B_Obstacles2" + difficultySuffix],
				mapInfo[LEVEL_SUBFOLDER + "B_Obstacles3" + difficultySuffix]
			};

			B_obstaclesMaps.Shuffle(random: random);

			CompositeTilemap.TilemapWithOffset B_obstacles1Tilemap = new CompositeTilemap.TilemapWithOffset(
				tilemap: MapDataTilemapGenerator.GetTilemap(
					data: B_obstaclesMaps[0],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic),
				xOffset: startTilemap.XOffset + startTilemap.Tilemap.GetWidth(),
				yOffset: 0,
				alwaysIncludeTilemap: false);

			list.Add(B_obstacles1Tilemap);

			CompositeTilemap.TilemapWithOffset B_obstacles2Tilemap = new CompositeTilemap.TilemapWithOffset(
				tilemap: MapDataTilemapGenerator.GetTilemap(
					data: B_obstaclesMaps[1],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic),
				xOffset: B_obstacles1Tilemap.XOffset + B_obstacles1Tilemap.Tilemap.GetWidth(),
				yOffset: 0,
				alwaysIncludeTilemap: false);

			list.Add(B_obstacles2Tilemap);

			CompositeTilemap.TilemapWithOffset B_obstacles3Tilemap = new CompositeTilemap.TilemapWithOffset(
				tilemap: MapDataTilemapGenerator.GetTilemap(
					data: B_obstaclesMaps[2],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic),
				xOffset: B_obstacles2Tilemap.XOffset + B_obstacles2Tilemap.Tilemap.GetWidth(),
				yOffset: 0,
				alwaysIncludeTilemap: false);

			list.Add(B_obstacles3Tilemap);

			List<MapDataHelper.Map> C_obstaclesMaps = new List<MapDataHelper.Map>()
			{
				mapInfo[LEVEL_SUBFOLDER + "C_Obstacles1" + difficultySuffix],
				mapInfo[LEVEL_SUBFOLDER + "C_Obstacles2" + difficultySuffix]
			};

			C_obstaclesMaps.Shuffle(random: random);

			CompositeTilemap.TilemapWithOffset C_obstacles1Tilemap = new CompositeTilemap.TilemapWithOffset(
				tilemap: MapDataTilemapGenerator.GetTilemap(
					data: C_obstaclesMaps[0],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic),
				xOffset: B_obstacles3Tilemap.XOffset + B_obstacles3Tilemap.Tilemap.GetWidth(),
				yOffset: 0,
				alwaysIncludeTilemap: false);

			list.Add(C_obstacles1Tilemap);

			CompositeTilemap.TilemapWithOffset C_obstacles2Tilemap = new CompositeTilemap.TilemapWithOffset(
				tilemap: MapDataTilemapGenerator.GetTilemap(
					data: C_obstaclesMaps[1],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic),
				xOffset: C_obstacles1Tilemap.XOffset + C_obstacles1Tilemap.Tilemap.GetWidth(),
				yOffset: 0,
				alwaysIncludeTilemap: false);

			list.Add(C_obstacles2Tilemap);

			CompositeTilemap.TilemapWithOffset finishTilemap = new CompositeTilemap.TilemapWithOffset(
				tilemap: MapDataTilemapGenerator.GetTilemap(
					data: mapInfo[LEVEL_SUBFOLDER + "D_Finish"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic),
				xOffset: C_obstacles2Tilemap.XOffset + C_obstacles2Tilemap.Tilemap.GetWidth(),
				yOffset: 0,
				alwaysIncludeTilemap: false);

			list.Add(finishTilemap);

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
				tuxXMibi: tuxXMibi + 200 * 1024,
				tuxYMibi: tuxYMibi,
				tuxTeleportStartingLocation: tuxTeleportStartingLocation != null
					? new Tuple<int, int>(tuxTeleportStartingLocation.Item1 + 200 * 1024, tuxTeleportStartingLocation.Item2)
					: null,
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
