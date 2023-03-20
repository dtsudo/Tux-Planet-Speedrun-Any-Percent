
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class LevelConfiguration_Level6 : ILevelConfiguration
	{
		private List<CompositeTilemap.TilemapWithOffset> normalizedTilemaps;
		private bool shouldRemoveKonqi;
		private IBackground background;

		private CompositeTilemap.TilemapWithOffset tilemapA;
		private CompositeTilemap.TilemapWithOffset tilemapC;
		private CompositeTilemap.TilemapWithOffset tilemapD;
		private CompositeTilemap.TilemapWithOffset tilemapF;

		private const string LEVEL_SUBFOLDER = "Level6/";

		public LevelConfiguration_Level6(
			Difficulty difficulty,
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			bool canAlreadyUseTimeSlowdown,
			IDTDeterministicRandom random)
		{
			Result result = ConstructUnnormalizedTilemaps(
				difficulty: difficulty,
				mapInfo: mapInfo,
				canAlreadyUseTimeSlowdown: canAlreadyUseTimeSlowdown,
				random: random);

			this.shouldRemoveKonqi = canAlreadyUseTimeSlowdown;

			this.normalizedTilemaps = new List<CompositeTilemap.TilemapWithOffset>(CompositeTilemap.NormalizeTilemaps(tilemaps: result.UnnormalizedTilemaps));

			this.tilemapA = this.normalizedTilemaps[result.TilemapAIndex];
			this.tilemapC = this.normalizedTilemaps[result.TilemapCIndex];
			this.tilemapD = this.normalizedTilemaps[result.TilemapDIndex];
			this.tilemapF = this.normalizedTilemaps[result.TilemapFIndex];

			this.background = new Background_Cave();
		}

		private class Result
		{
			public Result(
				List<CompositeTilemap.TilemapWithOffset> unnormalizedTilemaps,
				int tilemapAIndex,
				int tilemapCIndex,
				int tilemapDIndex,
				int tilemapFIndex)
			{
				this.UnnormalizedTilemaps = unnormalizedTilemaps;
				this.TilemapAIndex = tilemapAIndex;
				this.TilemapCIndex = tilemapCIndex;
				this.TilemapDIndex = tilemapDIndex;
				this.TilemapFIndex = tilemapFIndex;
			}

			public List<CompositeTilemap.TilemapWithOffset> UnnormalizedTilemaps { get; private set; }
			public int TilemapAIndex { get; private set; }
			public int TilemapCIndex { get; private set; }
			public int TilemapDIndex { get; private set; }
			public int TilemapFIndex { get; private set; }
		}

		private static Result ConstructUnnormalizedTilemaps(
			Difficulty difficulty,
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			bool canAlreadyUseTimeSlowdown,
			IDTDeterministicRandom random)
		{
			GameMusic gameMusic = LevelConfigurationHelper.GetRandomGameMusic(random: random);

			EnemyIdGenerator enemyIdGenerator = new EnemyIdGenerator();

			List<CompositeTilemap.TilemapWithOffset> list = new List<CompositeTilemap.TilemapWithOffset>();

			CompositeTilemap.TilemapWithOffset tilemapA = new CompositeTilemap.TilemapWithOffset(
				tilemap: MapDataTilemapGenerator.GetTilemap(
					data: mapInfo[LEVEL_SUBFOLDER + "A_Start"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 128 * 3,
					gameMusic: gameMusic),
				xOffset: 0,
				yOffset: 0,
				alwaysIncludeTilemap: false);

			list.Add(tilemapA);

			int tilemapAIndex = list.Count - 1;

			int changeXOffsetMaxCooldown;

			switch (difficulty)
			{
				case Difficulty.Easy:
					changeXOffsetMaxCooldown = 12;
					break;
				case Difficulty.Normal:
					changeXOffsetMaxCooldown = 7;
					break;
				case Difficulty.Hard:
					changeXOffsetMaxCooldown = 4;
					break;
				default:
					throw new Exception();
			}

			int xOffsetInTiles = 7;
			bool lastChangeWasToTheRight = false;
			int changeXOffsetCooldown = changeXOffsetMaxCooldown;

			for (int i = 0; i < 400; i++)
			{
				changeXOffsetCooldown--;
				if (changeXOffsetCooldown <= 0)
				{
					changeXOffsetCooldown = changeXOffsetMaxCooldown;

					int delta;

					if (random.NextInt(3) == 0)
						delta = 0;
					else if (random.NextInt(4) != 0)
						delta = lastChangeWasToTheRight ? 1 : -1;
					else
						delta = lastChangeWasToTheRight ? -1 : 1;

					xOffsetInTiles = xOffsetInTiles + delta;

					if (delta == -1)
						lastChangeWasToTheRight = false;
					if (delta == 1)
						lastChangeWasToTheRight = true;
				}

				list.Add(new CompositeTilemap.TilemapWithOffset(
					tilemap: MapDataTilemapGenerator.GetTilemap(
						data: mapInfo[LEVEL_SUBFOLDER + "B_Descent"],
						enemyIdGenerator: enemyIdGenerator,
						cutsceneName: null,
						scalingFactorScaled: 128 * 3,
						gameMusic: gameMusic),
					xOffset: tilemapA.XOffset + xOffsetInTiles * 48,
					yOffset: list[list.Count - 1].YOffset - 48,
					alwaysIncludeTilemap: false));
			}

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

			Tilemap cutsceneTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo[LEVEL_SUBFOLDER + "C_Cutscene" + difficultySuffix],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: CutsceneProcessing.TIME_SLOWDOWN_CUTSCENE,
					scalingFactorScaled: 128 * 3,
					gameMusic: gameMusic);

			list.Add(new CompositeTilemap.TilemapWithOffset(
				tilemap: canAlreadyUseTimeSlowdown ? Tilemap.GetTilemapWithoutCutscene(tilemap: cutsceneTilemap) : cutsceneTilemap,
				xOffset: tilemapA.XOffset + xOffsetInTiles * 48,
				yOffset: list[list.Count - 1].YOffset - cutsceneTilemap.GetHeight(),
				alwaysIncludeTilemap: false));

			int tilemapCIndex = list.Count - 1;

			Tilemap tilemapD = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo[LEVEL_SUBFOLDER + "D_SecondDrop"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 128 * 3,
					gameMusic: gameMusic);

			CompositeTilemap.TilemapWithOffset tilemapDWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: tilemapD,
				xOffset: list[list.Count - 1].XOffset + list[list.Count - 1].Tilemap.GetWidth(),
				yOffset: list[list.Count - 1].YOffset - 48,
				alwaysIncludeTilemap: false);

			list.Add(tilemapDWithOffset);

			int tilemapDIndex = list.Count - 1;

			xOffsetInTiles = 0;
			lastChangeWasToTheRight = false;
			changeXOffsetCooldown = changeXOffsetMaxCooldown;

			for (int i = 0; i < 400; i++)
			{
				changeXOffsetCooldown--;
				if (changeXOffsetCooldown <= 0)
				{
					changeXOffsetCooldown = changeXOffsetMaxCooldown;

					int delta;

					if (random.NextInt(3) == 0)
						delta = 0;
					else if (random.NextInt(4) != 0)
						delta = lastChangeWasToTheRight ? 1 : -1;
					else
						delta = lastChangeWasToTheRight ? -1 : 1;

					xOffsetInTiles = xOffsetInTiles + delta;

					if (delta == -1)
						lastChangeWasToTheRight = false;
					if (delta == 1)
						lastChangeWasToTheRight = true;
				}

				list.Add(new CompositeTilemap.TilemapWithOffset(
					tilemap: MapDataTilemapGenerator.GetTilemap(
						data: mapInfo[LEVEL_SUBFOLDER + "E_Descent"],
						enemyIdGenerator: enemyIdGenerator,
						cutsceneName: null,
						scalingFactorScaled: 128 * 3,
						gameMusic: gameMusic),
					xOffset: tilemapDWithOffset.XOffset + xOffsetInTiles * 48,
					yOffset: list[list.Count - 1].YOffset - 48,
					alwaysIncludeTilemap: false));
			}

			Tilemap tilemapF = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo[LEVEL_SUBFOLDER + "F_Finish" + difficultySuffix],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 128 * 3,
					gameMusic: gameMusic);

			list.Add(new CompositeTilemap.TilemapWithOffset(
				tilemap: tilemapF,
				xOffset: tilemapDWithOffset.XOffset + xOffsetInTiles * 48,
				yOffset: list[list.Count - 1].YOffset - tilemapF.GetHeight(),
				alwaysIncludeTilemap: false));

			int tilemapFIndex = list.Count - 1;

			return new Result(
				unnormalizedTilemaps: list,
				tilemapAIndex: tilemapAIndex,
				tilemapCIndex: tilemapCIndex,
				tilemapDIndex: tilemapDIndex,
				tilemapFIndex: tilemapFIndex);
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

			if (this.shouldRemoveKonqi)
				tilemap = new EnemyCreationTilemap(
					mapTilemap: tilemap,
					enemiesUnaffectedByXOffsetAndYOffset: new List<IEnemy>()
					{
						new EnemyAddLevelFlag(
							levelFlag: EnemyKonqiCutscene.SHOULD_TELEPORT_OUT_DEFAULT_LEVEL_FLAG,
							enemyId: "EnemyAddLevelFlag_shouldTeleportOut")
					});

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
				tuxXMibi: tuxXMibi,
				tuxYMibi: tuxYMibi,
				tuxTeleportStartingLocation: tuxTeleportStartingLocation,
				tuxTeleportInProgressElapsedMicros: tuxTeleportInProgressElapsedMicros,
				tilemap: tilemap,
				windowWidth: windowWidth,
				windowHeight: windowHeight);

			int effectiveTuxXMibi = tuxXMibi;
			int effectiveTuxYMibi = tuxYMibi;

			if (tuxTeleportInProgressElapsedMicros != null)
			{
				long deltaX = tuxXMibi - tuxTeleportStartingLocation.Item1;
				long deltaY = tuxYMibi - tuxTeleportStartingLocation.Item2;

				effectiveTuxXMibi = (int)(tuxTeleportStartingLocation.Item1 + deltaX * tuxTeleportInProgressElapsedMicros.Value / TuxState.TELEPORT_DURATION);
				effectiveTuxYMibi = (int)(tuxTeleportStartingLocation.Item2 + deltaY * tuxTeleportInProgressElapsedMicros.Value / TuxState.TELEPORT_DURATION);
			}

			int effectiveTuxX = effectiveTuxXMibi >> 10;
			int effectiveTuxY = effectiveTuxYMibi >> 10;

			if (this.tilemapA.XOffset <= effectiveTuxX
				&& effectiveTuxX <= this.tilemapA.XOffset + this.tilemapA.Tilemap.GetWidth()
				&& this.tilemapA.YOffset + 18 * 48 <= effectiveTuxY
				&& effectiveTuxY <= this.tilemapA.YOffset + this.tilemapA.Tilemap.GetHeight())
			{
				return CameraState.GetCameraState(
					x: Math.Max(cameraState.X, this.tilemapA.XOffset + 7 * 48 + (windowWidth >> 1)),
					y: cameraState.Y);
			}

			if (this.tilemapA.YOffset - 3 * 48 <= effectiveTuxY
				&& effectiveTuxY <= this.tilemapA.YOffset + 17 * 48)
			{
				int distance = (this.tilemapA.YOffset + 17 * 48) - effectiveTuxY;

				int ratioTimes10000 = distance * 10000 / (20 * 48);

				int cameraYOffset = ratioTimes10000 * (-windowHeight / 3) / 10000;

				return CameraState.GetCameraState(
					x: cameraState.X,
					y: cameraState.Y + cameraYOffset);
			}

			if (this.tilemapC.YOffset + 50 * 48 <= effectiveTuxY
				&& effectiveTuxY <= this.tilemapA.YOffset - 3 * 48)
			{
				int cameraYOffset = -windowHeight / 3;

				return CameraState.GetCameraState(
					x: cameraState.X,
					y: cameraState.Y + cameraYOffset);
			}

			if (this.tilemapC.YOffset <= effectiveTuxY
				&& effectiveTuxY <= this.tilemapC.YOffset + 50 * 48
				&& effectiveTuxX <= this.tilemapC.XOffset + this.tilemapC.Tilemap.GetWidth())
			{
				int ratioTimes10000;

				if (effectiveTuxX <= this.tilemapC.XOffset + 30 * 48)
					ratioTimes10000 = 10000;
				else if (effectiveTuxX <= this.tilemapC.XOffset + 45 * 48)
				{
					int distance = this.tilemapC.XOffset + 45 * 48 - effectiveTuxX;
					ratioTimes10000 = distance * 10000 / (15 * 48);
				}
				else
					ratioTimes10000 = 0;

				int cameraYOffset = ratioTimes10000 * (-windowHeight / 3) / 10000;

				return CameraState.GetCameraState(
					x: cameraState.X,
					y: cameraState.Y + cameraYOffset);
			}

			if (this.tilemapD.XOffset <= effectiveTuxX
				&& effectiveTuxX <= this.tilemapD.XOffset + this.tilemapD.Tilemap.GetWidth()
				&& this.tilemapD.YOffset <= effectiveTuxY
				&& effectiveTuxY <= this.tilemapD.YOffset + 10 * 48)
			{
				int distance = (this.tilemapD.YOffset + 10 * 48) - effectiveTuxY;

				int ratioTimes10000 = distance * 10000 / (10 * 48);

				int cameraYOffset = ratioTimes10000 * (-windowHeight / 3) / 10000;

				return CameraState.GetCameraState(
					x: cameraState.X,
					y: cameraState.Y + cameraYOffset);
			}

			if (this.tilemapF.YOffset + this.tilemapF.Tilemap.GetHeight() <= effectiveTuxY
				&& effectiveTuxY <= this.tilemapD.YOffset)
			{
				int cameraYOffset = -windowHeight / 3;

				return CameraState.GetCameraState(
					x: cameraState.X,
					y: cameraState.Y + cameraYOffset);
			}

			if (this.tilemapF.YOffset <= effectiveTuxY
				&& effectiveTuxY <= this.tilemapF.YOffset + this.tilemapF.Tilemap.GetHeight()
				&& this.tilemapF.XOffset <= effectiveTuxX
				&& effectiveTuxX <= this.tilemapF.XOffset + this.tilemapF.Tilemap.GetWidth())
			{
				int ratioTimes10000;

				if (this.tilemapF.XOffset + 70 * 48 <= effectiveTuxX)
					ratioTimes10000 = 0;
				else if (this.tilemapF.YOffset + 23 * 48 <= effectiveTuxY)
					ratioTimes10000 = 10000;
				else if (effectiveTuxX <= this.tilemapF.XOffset + 30 * 48)
					ratioTimes10000 = 10000;
				else if (effectiveTuxX <= this.tilemapF.XOffset + 45 * 48)
				{
					int distance = this.tilemapF.XOffset + 45 * 48 - effectiveTuxX;
					ratioTimes10000 = distance * 10000 / (15 * 48);
				}
				else
					ratioTimes10000 = 0;

				int cameraYOffset = ratioTimes10000 * (-windowHeight / 3) / 10000;

				return CameraState.GetCameraState(
					x: cameraState.X,
					y: cameraState.Y + cameraYOffset);
			}

			return cameraState;
		}
	}
}
