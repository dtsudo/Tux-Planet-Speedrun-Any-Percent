
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class LevelConfiguration_Level4 : ILevelConfiguration
	{
		private List<CompositeTilemap.TilemapWithOffset> normalizedTilemaps;
		private bool shouldRemoveKonqi;
		private IBackground background;

		private const string LEVEL_SUBFOLDER = "Level4";

		public LevelConfiguration_Level4(
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			bool canAlreadyUseTeleport,
			IDTDeterministicRandom random)
		{
			List<CompositeTilemap.TilemapWithOffset> unnormalizedTilemaps = ConstructUnnormalizedTilemaps(
				mapInfo: mapInfo,
				canAlreadyUseTeleport: canAlreadyUseTeleport,
				random: random);

			this.shouldRemoveKonqi = canAlreadyUseTeleport;

			this.normalizedTilemaps = new List<CompositeTilemap.TilemapWithOffset>(CompositeTilemap.NormalizeTilemaps(tilemaps: unnormalizedTilemaps));

			this.background = new Background_Cave();
		}

		private static List<CompositeTilemap.TilemapWithOffset> ConstructUnnormalizedTilemaps(
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			bool canAlreadyUseTeleport,
			IDTDeterministicRandom random)
		{
			GameMusic gameMusic = LevelConfigurationHelper.GetRandomGameMusic(random: random);

			EnemyIdGenerator enemyIdGenerator = new EnemyIdGenerator();

			List<CompositeTilemap.TilemapWithOffset> list = new List<CompositeTilemap.TilemapWithOffset>();

			Tilemap cutsceneTilemap = MapDataTilemapGenerator.GetTilemap(
					data: mapInfo[LEVEL_SUBFOLDER + "/" + "A_Start"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: CutsceneProcessing.TELEPORT_CUTSCENE,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic);

			CompositeTilemap.TilemapWithOffset cutsceneTilemapWithOffset = new CompositeTilemap.TilemapWithOffset(
				tilemap: canAlreadyUseTeleport ? Tilemap.GetTilemapWithoutCutscene(tilemap: cutsceneTilemap) : cutsceneTilemap,
				xOffset: 0,
				yOffset: 0,
				alwaysIncludeTilemap: false);

			list.Add(cutsceneTilemapWithOffset);

			CompositeTilemap.TilemapWithOffset tilemap2 = new CompositeTilemap.TilemapWithOffset(
				tilemap: MapDataTilemapGenerator.GetTilemap(
					data: mapInfo[LEVEL_SUBFOLDER + "/" + "B_Key"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic),
				xOffset: cutsceneTilemapWithOffset.XOffset + cutsceneTilemapWithOffset.Tilemap.GetWidth(),
				yOffset: cutsceneTilemapWithOffset.YOffset - 16 * 48,
				alwaysIncludeTilemap: false);

			list.Add(tilemap2);

			CompositeTilemap.TilemapWithOffset tilemap3 = new CompositeTilemap.TilemapWithOffset(
				tilemap: MapDataTilemapGenerator.GetTilemap(
					data: mapInfo[LEVEL_SUBFOLDER + "/" + "C_Ascent"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: null,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic),
				xOffset: tilemap2.XOffset + tilemap2.Tilemap.GetWidth(),
				yOffset: tilemap2.YOffset + 18 * 48,
				alwaysIncludeTilemap: false);

			list.Add(tilemap3);

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
			return null;
		}
	}
}
