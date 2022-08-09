
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
		public LevelNameDisplay LevelNameDisplay { get; private set; }

		public IReadOnlyList<IEnemy> Enemies { get; private set; }

		public IReadOnlyList<string> KilledEnemies { get; private set; }

		public Move PreviousMove { get; private set; }

		public int FrameCounter { get; private set; }

		public int WindowWidth { get; private set; }
		public int WindowHeight { get; private set; }

		public Level Level { get; private set; }

		public bool CanUseSaveStates { get; private set; }
		public bool CanUseTimeSlowdown { get; private set; }
		public bool CanUseTeleport { get; private set; }

		public bool StartedLevelOrCheckpointWithSaveStates { get; private set; }
		public bool StartedLevelOrCheckpointWithTimeSlowdown { get; private set; }
		public bool StartedLevelOrCheckpointWithTeleport { get; private set; }

		public Tuple<int, int> CheckpointLocation { get; private set; }
		public IReadOnlyList<string> CompletedCutscenesAtCheckpoint { get; private set; }
		public IReadOnlyList<string> KilledEnemiesAtCheckpoint { get; private set; }

		public IReadOnlyList<string> CompletedCutscenes { get; private set; }
		public ICutscene Cutscene { get; private set; }

		public GameLogicState(
			ILevelConfiguration levelConfiguration,
			IBackground background,
			ITilemap tilemap,
			TuxState tux,
			CameraState camera,
			LevelNameDisplay levelNameDisplay,
			IReadOnlyList<IEnemy> enemies,
			IReadOnlyList<string> killedEnemies,
			Move previousMove,
			int frameCounter,
			int windowWidth,
			int windowHeight,
			Level level,
			bool canUseSaveStates,
			bool canUseTimeSlowdown,
			bool canUseTeleport,
			bool startedLevelOrCheckpointWithSaveStates,
			bool startedLevelOrCheckpointWithTimeSlowdown,
			bool startedLevelOrCheckpointWithTeleport,
			Tuple<int, int> checkpointLocation,
			IReadOnlyList<string> completedCutscenesAtCheckpoint,
			IReadOnlyList<string> killedEnemiesAtCheckpoint,
			IReadOnlyList<string> completedCutscenes,
			ICutscene cutscene)
		{
			this.LevelConfiguration = levelConfiguration;
			this.Background = background;
			this.Tilemap = tilemap;
			this.Tux = tux;
			this.Camera = camera;
			this.LevelNameDisplay = levelNameDisplay;
			this.Enemies = new List<IEnemy>(enemies);
			this.KilledEnemies = new List<string>(killedEnemies);
			this.PreviousMove = previousMove;
			this.FrameCounter = frameCounter;
			this.WindowWidth = windowWidth;
			this.WindowHeight = windowHeight;
			this.Level = level;
			this.CanUseSaveStates = canUseSaveStates;
			this.CanUseTimeSlowdown = canUseTimeSlowdown;
			this.CanUseTeleport = canUseTeleport;
			this.StartedLevelOrCheckpointWithSaveStates = startedLevelOrCheckpointWithSaveStates;
			this.StartedLevelOrCheckpointWithTimeSlowdown = startedLevelOrCheckpointWithTimeSlowdown;
			this.StartedLevelOrCheckpointWithTeleport = startedLevelOrCheckpointWithTeleport;
			this.CheckpointLocation = checkpointLocation;
			this.CompletedCutscenesAtCheckpoint = new List<string>(completedCutscenesAtCheckpoint);
			this.KilledEnemiesAtCheckpoint = new List<string>(killedEnemiesAtCheckpoint);
			this.CompletedCutscenes = new List<string>(completedCutscenes);
			this.Cutscene = cutscene;
		}

		public GameLogicState(
			Level level,
			int windowWidth,
			int windowHeight,
			bool canUseSaveStates,
			bool canUseTimeSlowdown,
			bool canUseTeleport,
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo,
			IDTDeterministicRandom random)
		{
			ILevelConfiguration levelConfig;

			if (level == Level.Level1)
				levelConfig = new LevelConfiguration_Level1(mapInfo: mapInfo, random: random);
			else if (level == Level.Level2)
				levelConfig = new LevelConfiguration_Level2(mapInfo: mapInfo, canAlreadyUseSaveStates: canUseSaveStates, random: random);
			else if (level == Level.Level3)
				levelConfig = new LevelConfiguration_Level3(mapInfo: mapInfo, canAlreadyUseTeleport: canUseTeleport, random: random);
			else if (level == Level.Level4)
				levelConfig = new LevelConfiguration_Level4(mapInfo: mapInfo, canAlreadyUseTimeSlowdown: canUseTimeSlowdown, random: random);
			else if (level == Level.Level5)
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
				tuxTeleportStartingLocation: this.Tux.TeleportStartingLocation,
				tuxTeleportInProgressElapsedMicros: this.Tux.TeleportInProgressElapsedMicros,
				tilemap: this.Tilemap,
				windowWidth: windowWidth,
				windowHeight: windowHeight);
			this.LevelNameDisplay = LevelNameDisplay.GetLevelNameDisplay(levelName: level.GetLevelName());
			this.Enemies = new List<IEnemy>();
			this.KilledEnemies = new List<string>();
			this.PreviousMove = Move.EmptyMove();
			this.FrameCounter = 0;
			this.WindowWidth = windowWidth;
			this.WindowHeight = windowHeight;
			this.Level = level;
			this.CanUseSaveStates = canUseSaveStates;
			this.CanUseTimeSlowdown = canUseTimeSlowdown;
			this.CanUseTeleport = canUseTeleport;
			this.StartedLevelOrCheckpointWithSaveStates = canUseSaveStates;
			this.StartedLevelOrCheckpointWithTimeSlowdown = canUseTimeSlowdown;
			this.StartedLevelOrCheckpointWithTeleport = canUseTeleport;
			this.CheckpointLocation = null;
			this.CompletedCutscenesAtCheckpoint = new List<string>();
			this.KilledEnemiesAtCheckpoint = new List<string>();
			this.CompletedCutscenes = new List<string>();
			this.Cutscene = null;
		}
	}
}
