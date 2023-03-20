
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class LevelConfiguration_Level9 : ILevelConfiguration
	{
		private class EnemyEliteFlyamanitaSpawn : Tilemap.IExtraEnemyToSpawn
		{
			private int xMibi;
			private int yMibi;
			private string rngSeed;
			private Difficulty difficulty;
			private string enemyId;

			public EnemyEliteFlyamanitaSpawn(
				int xMibi,
				int yMibi,
				string rngSeed,
				Difficulty difficulty,
				string enemyId)
			{
				this.xMibi = xMibi;
				this.yMibi = yMibi;
				this.rngSeed = rngSeed;
				this.difficulty = difficulty;
				this.enemyId = enemyId;
			}

			public IEnemy GetEnemy(int xOffset, int yOffset)
			{
				return new EnemySpawnHelper(
					enemyToSpawn: EnemyEliteFlyamanita.GetEnemyEliteFlyamanita(
						xMibi: this.xMibi + (xOffset << 10),
						yMibi: this.yMibi + (yOffset << 10),
						rngSeed: this.rngSeed,
						difficulty: this.difficulty,
						enemyId: this.enemyId),
					xMibi: this.xMibi + (xOffset << 10),
					yMibi: this.yMibi + (yOffset << 10),
					enemyWidth: EnemyEliteFlyamanita.GREATER_ORBITER_RADIUS_IN_PIXELS * 2 + EnemyEliteFlyamanita.LESSER_ORBITER_RADIUS_IN_PIXELS * 2 + 20 * 3,
					enemyHeight: EnemyEliteFlyamanita.GREATER_ORBITER_RADIUS_IN_PIXELS * 2 + EnemyEliteFlyamanita.LESSER_ORBITER_RADIUS_IN_PIXELS * 2 + 20 * 3);
			}
		}

		private List<CompositeTilemap.TilemapWithOffset> normalizedTilemaps;
		private IBackground background;

		private const string LEVEL_SUBFOLDER = "Level9/";

		public LevelConfiguration_Level9(
			Difficulty difficulty,
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			IDTDeterministicRandom random)
		{
			List<CompositeTilemap.TilemapWithOffset> unnormalizedTilemaps = ConstructUnnormalizedTilemaps(difficulty: difficulty, mapInfo: mapInfo, random: random);

			this.normalizedTilemaps = new List<CompositeTilemap.TilemapWithOffset>(CompositeTilemap.NormalizeTilemaps(tilemaps: unnormalizedTilemaps));

			this.background = new Background_Ocean();
		}

		private static List<CompositeTilemap.TilemapWithOffset> ConstructUnnormalizedTilemaps(
			Difficulty difficulty,
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo, 
			IDTDeterministicRandom random)
		{
			GameMusic gameMusic = LevelConfigurationHelper.GetRandomGameMusic(random: random);

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

			List<Tilemap.IExtraEnemyToSpawn> tilemapAExtraEnemies = new List<Tilemap.IExtraEnemyToSpawn>()
			{
				new EnemyEliteOrange.EnemyEliteOrangeSpawn(
					xMibi: 32 * 48 * 1024,
					yMibi: 3 * 48 * 1024,
					orbitersAngleScaled: random.NextInt(360 * 128),
					isOrbitingClockwise: random.NextBool(),
					difficulty: difficulty,
					enemyId: "tilemapA_eliteOrangeIntro")
			};

			for (int i = (difficulty == Difficulty.Hard ? 0 : (difficulty == Difficulty.Normal ? 1 : 2)); i < 3; i++)
				tilemapAExtraEnemies.Add(new EnemyEliteOrange.EnemyEliteOrangeSpawn(
					xMibi: ((53 + 10 * i) * 48) << 10,
					yMibi: (4 * 48) << 10,
					orbitersAngleScaled: random.NextInt(360 * 128),
					isOrbitingClockwise: random.NextBool(),
					difficulty: difficulty,
					enemyId: "tilemapA_eliteOrangeChallenge_" + i.ToStringCultureInvariant()));

			list.Add(new CompositeTilemap.TilemapWithOffset(
				tilemap: Tilemap.GetTilemapWithExtraEnemiesToSpawn(
					tilemap: MapDataTilemapGenerator.GetTilemap(
						data: mapInfo[LEVEL_SUBFOLDER + "A_Start" + difficultySuffix],
						enemyIdGenerator: enemyIdGenerator,
						cutsceneName: null,
						scalingFactorScaled: 3 * 128,
						gameMusic: gameMusic),
					extraEnemiesToSpawn: tilemapAExtraEnemies),
				xOffset: 0,
				yOffset: 0,
				alwaysIncludeTilemap: false));

			List<Tilemap> tilemapsB = new List<Tilemap>();

			List<Tilemap.IExtraEnemyToSpawn> tilemapB1ExtraEnemies = new List<Tilemap.IExtraEnemyToSpawn>();

			for (int i = (difficulty == Difficulty.Normal ? 3 : 0); i < 8; i++)
				tilemapB1ExtraEnemies.Add(new EnemyEliteOrange.EnemyEliteOrangeSpawn(
					xMibi: ((15 + 10 * i) * 48) << 10,
					yMibi: (5 * 48) << 10,
					orbitersAngleScaled: random.NextInt(360 * 128),
					isOrbitingClockwise: random.NextBool(),
					difficulty: difficulty,
					enemyId: "tilemapB1_eliteOrange_" + i.ToStringCultureInvariant()));

			tilemapsB.Add(Tilemap.GetTilemapWithExtraEnemiesToSpawn(
				tilemap: MapDataTilemapGenerator.GetTilemap(
						data: mapInfo[LEVEL_SUBFOLDER + "B_Obstacles1" + difficultySuffix],
						enemyIdGenerator: enemyIdGenerator,
						cutsceneName: null,
						scalingFactorScaled: 3 * 128,
						gameMusic: gameMusic),
				extraEnemiesToSpawn: tilemapB1ExtraEnemies));

			tilemapsB.Add(MapDataTilemapGenerator.GetTilemap(
						data: mapInfo[LEVEL_SUBFOLDER + "B_Obstacles2" + difficultySuffix],
						enemyIdGenerator: enemyIdGenerator,
						cutsceneName: null,
						scalingFactorScaled: 3 * 128,
						gameMusic: gameMusic));

			// Only shuffle the first two tilemaps
			tilemapsB.Shuffle(random: random);

			List<Tilemap.IExtraEnemyToSpawn> tilemapB3ExtraEnemies = new List<Tilemap.IExtraEnemyToSpawn>();

			for (int i = 0; i < 6; i++)
			{
				if (difficulty == Difficulty.Easy)
				{
					if (i == 1)
						continue;
					if (i == 3)
						continue;
				}

				if (difficulty == Difficulty.Normal && i == 2)
					continue;

				string eliteFlyamanitaRngSeed = random.SerializeToString();
				int x = random.NextInt(3) + 2;
				for (int j = 0; j < x; j++)
					random.NextBool();

				tilemapB3ExtraEnemies.Add(new EnemyEliteFlyamanitaSpawn(
					xMibi: ((14 + 25 * i) * 48) << 10,
					yMibi: (7 * 48) << 10,
					rngSeed: eliteFlyamanitaRngSeed,
					difficulty: difficulty,
					enemyId: "tilemapB3_eliteFlyamanita_" + i.ToStringCultureInvariant()));
			}

			tilemapsB.Add(Tilemap.GetTilemapWithExtraEnemiesToSpawn(
				tilemap: MapDataTilemapGenerator.GetTilemap(
						data: mapInfo[LEVEL_SUBFOLDER + "B_Obstacles3" + difficultySuffix],
						enemyIdGenerator: enemyIdGenerator,
						cutsceneName: null,
						scalingFactorScaled: 3 * 128,
						gameMusic: gameMusic),
				extraEnemiesToSpawn: tilemapB3ExtraEnemies));

			foreach (Tilemap tilemap in tilemapsB)
				list.Add(new CompositeTilemap.TilemapWithOffset(
					tilemap: tilemap,
					xOffset: list[list.Count - 1].XOffset + list[list.Count - 1].Tilemap.GetWidth(),
					yOffset: 0,
					alwaysIncludeTilemap: false));

			list.Add(new CompositeTilemap.TilemapWithOffset(
				tilemap: MapDataTilemapGenerator.GetTilemap(
						data: mapInfo[LEVEL_SUBFOLDER + "C_Finish"],
						enemyIdGenerator: enemyIdGenerator,
						cutsceneName: null,
						scalingFactorScaled: 3 * 128,
						gameMusic: gameMusic),
				xOffset: list[list.Count - 1].XOffset + list[list.Count - 1].Tilemap.GetWidth(),
				yOffset: 0,
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

		public ITilemap GetTilemap(int? tuxX, int? tuxY, int? cameraX, int? cameraY, int windowWidth, int windowHeight, IReadOnlyList<string> levelFlags, MapKeyState mapKeyState)
		{
			ITilemap tilemap = LevelConfigurationHelper.GetTilemap(
				normalizedTilemaps: this.normalizedTilemaps,
				tuxX: tuxX,
				tuxY: tuxY,
				cameraX: cameraX,
				cameraY: cameraY,
				mapKeyState: mapKeyState,
				windowWidth: windowWidth,
				windowHeight: windowHeight);

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
