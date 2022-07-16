
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class GameLogicState
	{
		public const int MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS = 250;
		public const int MARGIN_FOR_TILEMAP_DESPAWN_IN_PIXELS = 400;

		public ILevelConfiguration LevelConfiguration { get; private set; }

		public IBackground Background { get; private set; }

		public ITilemap Tilemap { get; private set; }
		public TuxState Tux { get; private set; }
		public CameraState Camera { get; private set; }
		public LevelNumberDisplay LevelNumberDisplay { get; private set; }

		public IReadOnlyList<IEnemy> Enemies { get; private set; }

		public IReadOnlyList<string> KilledEnemies { get; private set; }

		public int FrameCounter { get; private set; }

		public int WindowWidth { get; private set; }
		public int WindowHeight { get; private set; }

		public int LevelNumber { get; private set; }

		public bool CanUseSaveStates { get; private set; }
		public bool CanUseTimeSlowdown { get; private set; }

		public IReadOnlyList<string> CompletedCutscenes { get; private set; }
		public ICutscene Cutscene { get; private set; }

		public GameLogicState(
			ILevelConfiguration levelConfiguration,
			IBackground background,
			ITilemap tilemap,
			TuxState tux,
			CameraState camera,
			LevelNumberDisplay levelNumberDisplay,
			IReadOnlyList<IEnemy> enemies,
			IReadOnlyList<string> killedEnemies,
			int frameCounter,
			int windowWidth,
			int windowHeight,
			int levelNumber,
			bool canUseSaveStates,
			bool canUseTimeSlowdown,
			IReadOnlyList<string> completedCutscenes,
			ICutscene cutscene)
		{
			this.LevelConfiguration = levelConfiguration;
			this.Background = background;
			this.Tilemap = tilemap;
			this.Tux = tux;
			this.Camera = camera;
			this.LevelNumberDisplay = levelNumberDisplay;
			this.Enemies = new List<IEnemy>(enemies);
			this.KilledEnemies = new List<string>(killedEnemies);
			this.FrameCounter = frameCounter;
			this.WindowWidth = windowWidth;
			this.WindowHeight = windowHeight;
			this.LevelNumber = levelNumber;
			this.CanUseSaveStates = canUseSaveStates;
			this.CanUseTimeSlowdown = canUseTimeSlowdown;
			this.CompletedCutscenes = new List<string>(completedCutscenes);
			this.Cutscene = cutscene;
		}

		public GameLogicState(
			int levelNumber,
			int windowWidth,
			int windowHeight,
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			IDTDeterministicRandom random)
		{
			ILevelConfiguration levelConfig;

			if (levelNumber == 1)
				levelConfig = new LevelConfiguration_Level1(mapInfo: mapInfo, random: random);
			else if (levelNumber == 2)
				levelConfig = new LevelConfiguration_Level2(mapInfo: mapInfo, random: random);
			else if (levelNumber == 3)
				levelConfig = new LevelConfiguration_Level3(mapInfo: mapInfo, random: random);
			else if (levelNumber == 4)
				levelConfig = new LevelConfiguration_Level4(mapInfo: mapInfo, random: random);
			else if (levelNumber == 5)
				levelConfig = new LevelConfiguration_Level5(mapInfo: mapInfo, random: random);
			else
				throw new Exception();

			this.LevelConfiguration = levelConfig;
			this.Background = this.LevelConfiguration.GetBackground();
			this.Tilemap = this.LevelConfiguration.GetTilemap(tuxX: null, tuxY: null, windowWidth: windowWidth, windowHeight: windowHeight);
			this.Tux = TuxState.GetDefaultTuxState(x: this.Tilemap.GetTuxLocation(xOffset: 0, yOffset: 0).Item1, y: this.Tilemap.GetTuxLocation(xOffset: 0, yOffset: 0).Item2);
			this.Camera = CameraStateProcessing.ComputeCameraState(
				tuxXMibi: this.Tux.XMibi,
				tuxYMibi: this.Tux.YMibi,
				tilemap: this.Tilemap,
				windowWidth: windowWidth,
				windowHeight: windowHeight);
			this.LevelNumberDisplay = LevelNumberDisplay.GetLevelNumberDisplay(levelNumber: levelNumber);
			this.Enemies = new List<IEnemy>();
			this.KilledEnemies = new List<string>();
			this.FrameCounter = 0;
			this.WindowWidth = windowWidth;
			this.WindowHeight = windowHeight;
			this.LevelNumber = levelNumber;
			this.CanUseSaveStates = levelNumber > CutsceneProcessing.LEVEL_THAT_GRANTS_SAVESTATES;
			this.CanUseTimeSlowdown = levelNumber > CutsceneProcessing.LEVEL_THAT_GRANTS_TIME_SLOWDOWN;
			this.CompletedCutscenes = new List<string>();
			this.Cutscene = null;
		}
	}
}
