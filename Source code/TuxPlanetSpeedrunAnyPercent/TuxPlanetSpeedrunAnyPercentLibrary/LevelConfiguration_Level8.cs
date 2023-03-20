
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class LevelConfiguration_Level8 : ILevelConfiguration
	{
		private List<CompositeTilemap.TilemapWithOffset> normalizedTilemaps;
		private IBackground background;

		private List<IEnemy> fishes;

		private const string LEVEL_SUBFOLDER = "Level8/";

		// level flags
		public const string HAS_FINISHED_CUTSCENE = "level8_hasFinishedCutscene";

		public LevelConfiguration_Level8(
			Difficulty difficulty,
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			IDTDeterministicRandom random)
		{
			List<CompositeTilemap.TilemapWithOffset> unnormalizedTilemaps = ConstructUnnormalizedTilemaps(difficulty: difficulty, mapInfo: mapInfo, random: random);

			this.normalizedTilemaps = new List<CompositeTilemap.TilemapWithOffset>(CompositeTilemap.NormalizeTilemaps(tilemaps: unnormalizedTilemaps));

			this.background = new Background_Ocean();

			this.fishes = new List<IEnemy>();

			for (int i = 0; i < 25; i++)
			{
				GameImage fishImage;
				GameImage fishImageMirrored;

				switch (random.NextInt(3))
				{
					case 0:
						fishImage = GameImage.FishBlue;
						fishImageMirrored = GameImage.FishBlueMirrored;
						break;
					case 1:
						fishImage = GameImage.FishGreen;
						fishImageMirrored = GameImage.FishGreenMirrored;
						break;
					case 2:
						fishImage = GameImage.FishRed;
						fishImageMirrored = GameImage.FishRedMirrored;
						break;
					default:
						throw new Exception();
				}

				this.fishes.Add(EnemyLevel8Fish.SpawnLevel8Fish(
					x: random.NextInt(75 * 48),
					y: random.NextInt(7 * 48),
					isFacingRight: random.NextBool(),
					fishImage: fishImage,
					fishImageMirrored: fishImageMirrored,
					enemyId: "level8Fish_" + i.ToStringCultureInvariant()));
			}
		}

		private static List<CompositeTilemap.TilemapWithOffset> ConstructUnnormalizedTilemaps(
			Difficulty difficulty,
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo, 
			IDTDeterministicRandom random)
		{
			GameMusic gameMusic = LevelConfigurationHelper.GetRandomGameMusic(random: random);

			EnemyIdGenerator enemyIdGenerator = new EnemyIdGenerator();

			List<CompositeTilemap.TilemapWithOffset> list = new List<CompositeTilemap.TilemapWithOffset>();

			CompositeTilemap.TilemapWithOffset tilemap = new CompositeTilemap.TilemapWithOffset(
				tilemap: MapDataTilemapGenerator.GetTilemap(
					data: mapInfo[LEVEL_SUBFOLDER + "Water"],
					enemyIdGenerator: enemyIdGenerator,
					cutsceneName: CutsceneProcessing.LEVEL_8_CUTSCENE,
					scalingFactorScaled: 3 * 128,
					gameMusic: gameMusic),
				xOffset: 0,
				yOffset: 0,
				alwaysIncludeTilemap: false);

			list.Add(tilemap);

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

			List<IEnemy> enemies = new List<IEnemy>(this.fishes);
			enemies.Add(new EnemyLevel8WaterAnimation(elapsedMicros: 0, enemyId: "level8waterAnimation"));

			tilemap = new EnemyCreationTilemap(
				mapTilemap: tilemap,
				enemiesUnaffectedByXOffsetAndYOffset: enemies);

			if (levelFlags.Contains(HAS_FINISHED_CUTSCENE))
				tilemap = new WinLevelTilemap(mapTilemap: tilemap);

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

			return CameraState.GetCameraState(
				x: cameraState.X,
				y: windowHeight >> 1);
		}
	}
}
