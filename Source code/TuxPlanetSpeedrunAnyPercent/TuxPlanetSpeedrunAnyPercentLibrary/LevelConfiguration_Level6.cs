
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class LevelConfiguration_Level6 : ILevelConfiguration
	{
		private List<CompositeTilemap.TilemapWithOffset> normalizedTilemaps;
		private bool shouldSpawnRemoveKonqi;
		private IBackground background;

		private CompositeTilemap.TilemapWithOffset tilemapA;

		private const string LEVEL_SUBFOLDER = "Level6/";

		public LevelConfiguration_Level6(
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			bool canAlreadyUseTimeSlowdown,
			IDTDeterministicRandom random)
		{
			List<CompositeTilemap.TilemapWithOffset> unnormalizedTilemaps = ConstructUnnormalizedTilemaps(
				mapInfo: mapInfo,
				canAlreadyUseTimeSlowdown: canAlreadyUseTimeSlowdown,
				random: random);

			this.shouldSpawnRemoveKonqi = canAlreadyUseTimeSlowdown;

			this.normalizedTilemaps = new List<CompositeTilemap.TilemapWithOffset>(CompositeTilemap.NormalizeTilemaps(tilemaps: unnormalizedTilemaps));

			this.tilemapA = this.normalizedTilemaps[0];

			this.background = new Background_Cave();
		}

		private static List<CompositeTilemap.TilemapWithOffset> ConstructUnnormalizedTilemaps(
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

			int xOffsetInTiles = 7;
			bool lastChangeWasToTheRight = false;
			int changeXOffsetCooldown = 4;

			for (int i = 0; i < 400; i++)
			{
				changeXOffsetCooldown--;
				if (changeXOffsetCooldown <= 0)
				{
					changeXOffsetCooldown = 4;

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

			Tilemap cutsceneTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo[LEVEL_SUBFOLDER + "C_Cutscene"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: CutsceneProcessing.TIME_SLOWDOWN_CUTSCENE,
					scalingFactorScaled: 128 * 3,
					gameMusic: gameMusic);

			list.Add(new CompositeTilemap.TilemapWithOffset(
				tilemap: canAlreadyUseTimeSlowdown ? Tilemap.GetTilemapWithoutCutscene(tilemap: cutsceneTilemap) : cutsceneTilemap,
				xOffset: tilemapA.XOffset + xOffsetInTiles * 48,
				yOffset: list[list.Count - 1].YOffset - cutsceneTilemap.GetHeight(),
				alwaysIncludeTilemap: false));

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

			xOffsetInTiles = 0;
			lastChangeWasToTheRight = false;
			changeXOffsetCooldown = 4;

			for (int i = 0; i < 400; i++)
			{
				changeXOffsetCooldown--;
				if (changeXOffsetCooldown <= 0)
				{
					changeXOffsetCooldown = 4;

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
					data: mapInfo[LEVEL_SUBFOLDER + "F_Finish"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 128 * 3,
					gameMusic: gameMusic);

			list.Add(new CompositeTilemap.TilemapWithOffset(
				tilemap: tilemapF,
				xOffset: tilemapDWithOffset.XOffset + xOffsetInTiles * 48,
				yOffset: list[list.Count - 1].YOffset - tilemapF.GetHeight(),
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

			if (this.shouldSpawnRemoveKonqi)
				tilemap = new SpawnRemoveKonqiTilemap(tilemap: tilemap);

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
			int tuxX = tuxXMibi >> 10;
			int tuxY = tuxYMibi >> 10;

			if (this.tilemapA.XOffset <= tuxX
				&& tuxX <= this.tilemapA.XOffset + this.tilemapA.Tilemap.GetWidth()
				&& this.tilemapA.YOffset <= tuxY
				&& tuxY <= this.tilemapA.YOffset + this.tilemapA.Tilemap.GetHeight())
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
					x: Math.Max(cameraState.X, this.tilemapA.XOffset + 7 * 48 + (windowWidth >> 1)),
					y: cameraState.Y);
			}

			return null;
		}
	}
}
