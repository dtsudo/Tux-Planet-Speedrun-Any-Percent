﻿
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class LevelConfiguration_Level9 : ILevelConfiguration
	{
		private class EnemyEliteOrangeSpawn : Tilemap.IExtraEnemyToSpawn
		{
			private int xMibi;
			private int yMibi;
			private int orbitersAngleScaled;
			private bool isOrbitingClockwise;
			private string enemyId;

			public EnemyEliteOrangeSpawn(
				int xMibi, 
				int yMibi, 
				int orbitersAngleScaled,
				bool isOrbitingClockwise,
				string enemyId)
			{
				this.xMibi = xMibi;
				this.yMibi = yMibi;
				this.orbitersAngleScaled = orbitersAngleScaled;
				this.isOrbitingClockwise = isOrbitingClockwise;
				this.enemyId = enemyId;
			}

			public IEnemy GetEnemy(int xOffset, int yOffset)
			{
				return new EnemySpawnHelper(
					enemyToSpawn: EnemyEliteOrange.GetEnemyEliteOrange(
						xMibi: this.xMibi + (xOffset << 10),
						yMibi: this.yMibi + (yOffset << 10),
						isFacingRight: false,
						orbitersAngleScaled: this.orbitersAngleScaled,
						isOrbitingClockwise: this.isOrbitingClockwise,
						enemyId: this.enemyId),
					xMibi: this.xMibi + (xOffset << 10),
					yMibi: this.yMibi + (yOffset << 10),
					enemyWidth: EnemyEliteOrange.ORBITERS_RADIUS_IN_PIXELS * 2 + 48,
					enemyHeight: EnemyEliteOrange.ORBITERS_RADIUS_IN_PIXELS * 2 + 48);
			}
		}

		private class EnemyEliteFlyamanitaSpawn : Tilemap.IExtraEnemyToSpawn
		{
			private int xMibi;
			private int yMibi;
			private string rngSeed;
			private string enemyId;

			public EnemyEliteFlyamanitaSpawn(
				int xMibi,
				int yMibi,
				string rngSeed,
				string enemyId)
			{
				this.xMibi = xMibi;
				this.yMibi = yMibi;
				this.rngSeed = rngSeed;
				this.enemyId = enemyId;
			}

			public IEnemy GetEnemy(int xOffset, int yOffset)
			{
				return new EnemySpawnHelper(
					enemyToSpawn: EnemyEliteFlyamanita.GetEnemyEliteFlyamanita(
						xMibi: this.xMibi + (xOffset << 10),
						yMibi: this.yMibi + (yOffset << 10),
						rngSeed: this.rngSeed,
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
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			IDTDeterministicRandom random)
		{
			List<CompositeTilemap.TilemapWithOffset> unnormalizedTilemaps = ConstructUnnormalizedTilemaps(mapInfo: mapInfo, random: random);

			this.normalizedTilemaps = new List<CompositeTilemap.TilemapWithOffset>(CompositeTilemap.NormalizeTilemaps(tilemaps: unnormalizedTilemaps));

			this.background = new Background_Ocean();
		}

		private static List<CompositeTilemap.TilemapWithOffset> ConstructUnnormalizedTilemaps(IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo, IDTDeterministicRandom random)
		{
			GameMusic gameMusic = LevelConfigurationHelper.GetRandomGameMusic(random: random);

			EnemyIdGenerator enemyIdGenerator = new EnemyIdGenerator();

			List<CompositeTilemap.TilemapWithOffset> list = new List<CompositeTilemap.TilemapWithOffset>();

			List<Tilemap.IExtraEnemyToSpawn> tilemapAExtraEnemies = new List<Tilemap.IExtraEnemyToSpawn>()
			{
				new EnemyEliteOrangeSpawn(
					xMibi: 32 * 48 * 1024,
					yMibi: 3 * 48 * 1024,
					orbitersAngleScaled: random.NextInt(360 * 128),
					isOrbitingClockwise: random.NextBool(),
					enemyId: "tilemapA_eliteOrangeIntro")
			};

			for (int i = 0; i < 3; i++)
				tilemapAExtraEnemies.Add(new EnemyEliteOrangeSpawn(
					xMibi: ((53 + 10 * i) * 48) << 10,
					yMibi: (4 * 48) << 10,
					orbitersAngleScaled: random.NextInt(360 * 128),
					isOrbitingClockwise: random.NextBool(),
					enemyId: "tilemapA_eliteOrangeChallenge_" + i.ToStringCultureInvariant()));

			list.Add(new CompositeTilemap.TilemapWithOffset(
				tilemap: Tilemap.GetTilemapWithExtraEnemiesToSpawn(
					tilemap: MapDataTilemapGenerator.GetTilemap(
						data: mapInfo[LEVEL_SUBFOLDER + "A_Start"],
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

			for (int i = 0; i < 8; i++)
				tilemapB1ExtraEnemies.Add(new EnemyEliteOrangeSpawn(
					xMibi: ((15 + 10 * i) * 48) << 10,
					yMibi: (5 * 48) << 10,
					orbitersAngleScaled: random.NextInt(360 * 128),
					isOrbitingClockwise: random.NextBool(),
					enemyId: "tilemapB1_eliteOrange_" + i.ToStringCultureInvariant()));

			tilemapsB.Add(Tilemap.GetTilemapWithExtraEnemiesToSpawn(
				tilemap: MapDataTilemapGenerator.GetTilemap(
						data: mapInfo[LEVEL_SUBFOLDER + "B_Obstacles1"],
						enemyIdGenerator: enemyIdGenerator,
						cutsceneName: null,
						scalingFactorScaled: 3 * 128,
						gameMusic: gameMusic),
				extraEnemiesToSpawn: tilemapB1ExtraEnemies));

			tilemapsB.Add(MapDataTilemapGenerator.GetTilemap(
						data: mapInfo[LEVEL_SUBFOLDER + "B_Obstacles2"],
						enemyIdGenerator: enemyIdGenerator,
						cutsceneName: null,
						scalingFactorScaled: 3 * 128,
						gameMusic: gameMusic));

			// Only shuffle the first two tilemaps
			tilemapsB.Shuffle(random: random);

			List<Tilemap.IExtraEnemyToSpawn> tilemapB3ExtraEnemies = new List<Tilemap.IExtraEnemyToSpawn>();

			for (int i = 0; i < 6; i++)
			{
				string eliteFlyamanitaRngSeed = random.SerializeToString();
				int x = random.NextInt(3) + 2;
				for (int j = 0; j < x; j++)
					random.NextBool();

				tilemapB3ExtraEnemies.Add(new EnemyEliteFlyamanitaSpawn(
					xMibi: ((14 + 25 * i) * 48) << 10,
					yMibi: (7 * 48) << 10,
					rngSeed: eliteFlyamanitaRngSeed,
					enemyId: "tilemapB3_eliteFlyamanita_" + i.ToStringCultureInvariant()));
			}

			tilemapsB.Add(Tilemap.GetTilemapWithExtraEnemiesToSpawn(
				tilemap: MapDataTilemapGenerator.GetTilemap(
						data: mapInfo[LEVEL_SUBFOLDER + "B_Obstacles3"],
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
