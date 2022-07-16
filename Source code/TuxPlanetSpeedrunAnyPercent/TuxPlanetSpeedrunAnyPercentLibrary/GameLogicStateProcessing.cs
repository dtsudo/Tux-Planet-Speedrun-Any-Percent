
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class GameLogicStateProcessing
	{
		public class Result
		{
			public Result(
				GameLogicState newGameLogicState,
				bool endLevel,
				GameMusic? playMusic,
				bool shouldStopMusic)
			{
				this.NewGameLogicState = newGameLogicState;
				this.EndLevel = endLevel;
				this.PlayMusic = playMusic;
				this.ShouldStopMusic = shouldStopMusic;
			}

			public GameLogicState NewGameLogicState { get; private set; }
			public bool EndLevel { get; private set; }
			public GameMusic? PlayMusic { get; private set; }
			public bool ShouldStopMusic { get; private set; }
		}

		public static Result ProcessFrame(
			GameLogicState gameLogicState,
			Move move,
			bool debugMode,
			IKeyboard debugKeyboardInput,
			IKeyboard debugPreviousKeyboardInput,
			IDisplayProcessing<GameImage> displayProcessing,
			ISoundOutput<GameSound> soundOutput,
			int elapsedMicrosPerFrame)
		{
			ITilemap newTilemap = gameLogicState.LevelConfiguration.GetTilemap(
				tuxX: gameLogicState.Tux.XMibi >> 10, 
				tuxY: gameLogicState.Tux.YMibi >> 10,
				windowWidth: gameLogicState.WindowWidth,
				windowHeight: gameLogicState.WindowHeight);

			LevelNumberDisplay newLevelNumberDisplay = gameLogicState.LevelNumberDisplay.ProcessFrame(elapsedMicrosPerFrame: elapsedMicrosPerFrame);

			ICutscene newCutscene = gameLogicState.Cutscene;
			List<string> newCompletedCutscenes = new List<string>(gameLogicState.CompletedCutscenes);

			bool newCanUseSaveStates = gameLogicState.CanUseSaveStates;
			bool newCanUseTimeSlowdown = gameLogicState.CanUseTimeSlowdown;

			CameraState newCamera = gameLogicState.Camera;

			List<IEnemy> newEnemies = new List<IEnemy>(gameLogicState.Enemies);

			if (newCutscene == null)
			{
				string cutsceneName = newTilemap.GetCutscene(x: gameLogicState.Tux.XMibi >> 10, y: gameLogicState.Tux.YMibi >> 10);
				if (cutsceneName != null && !newCompletedCutscenes.Contains(cutsceneName))
					newCutscene = CutsceneProcessing.GetCutscene(cutsceneName: cutsceneName);
			}

			if (newCutscene != null)
			{
				string cutsceneName = newCutscene.GetCutsceneName();
				CutsceneProcessing.Result cutsceneResult = newCutscene.ProcessFrame(
					move: move,
					tuxXMibi: gameLogicState.Tux.XMibi,
					tuxYMibi: gameLogicState.Tux.YMibi,
					cameraState: newCamera,
					elapsedMicrosPerFrame: elapsedMicrosPerFrame,
					windowWidth: gameLogicState.WindowWidth,
					windowHeight: gameLogicState.WindowHeight,
					tilemap: newTilemap);

				if (cutsceneResult.Move != null)
					move = cutsceneResult.Move;

				newCutscene = cutsceneResult.Cutscene;

				if (cutsceneResult.NewEnemies.Count > 0)
					newEnemies.AddRange(cutsceneResult.NewEnemies);

				newCamera = cutsceneResult.CameraState;

				if (cutsceneResult.ShouldGrantSaveStatePower)
					newCanUseSaveStates = true;

				if (cutsceneResult.ShouldGrantTimeSlowdownPower)
					newCanUseTimeSlowdown = true;

				if (newCutscene == null)
					newCompletedCutscenes.Add(cutsceneName);
			}

			TuxStateProcessing.Result result = TuxStateProcessing.ProcessFrame(
				tuxState: gameLogicState.Tux,
				move: move,
				debugMode: debugMode,
				debugKeyboardInput: debugKeyboardInput,
				debugPreviousKeyboardInput: debugPreviousKeyboardInput,
				displayProcessing: displayProcessing,
				soundOutput: soundOutput,
				elapsedMicrosPerFrame: elapsedMicrosPerFrame,
				tilemap: gameLogicState.Tilemap);

			TuxState newTuxState = result.TuxState;

			if (newCutscene == null)
				newCamera = CameraStateProcessing.ComputeCameraState(
					tuxXMibi: newTuxState.XMibi,
					tuxYMibi: newTuxState.YMibi,
					tilemap: gameLogicState.Tilemap,
					windowWidth: gameLogicState.WindowWidth,
					windowHeight: gameLogicState.WindowHeight);

			EnemyProcessing.Result enemyProcessingResult = EnemyProcessing.ProcessFrame(
				tilemap: newTilemap,
				cameraX: newCamera.X,
				cameraY: newCamera.Y,
				windowWidth: gameLogicState.WindowWidth,
				windowHeight: gameLogicState.WindowHeight,
				enemies: newEnemies,
				killedEnemies: gameLogicState.KilledEnemies,
				elapsedMicrosPerFrame: elapsedMicrosPerFrame);

			newEnemies = new List<IEnemy>(enemyProcessingResult.Enemies);

			List<string> newKilledEnemies = new List<string>(gameLogicState.KilledEnemies);
			newKilledEnemies.AddRange(enemyProcessingResult.NewlyKilledEnemies);

			CollisionProcessing_Tux.Result collisionResultTux = CollisionProcessing_Tux.ProcessFrame(
				tuxState: newTuxState,
				enemies: newEnemies,
				soundOutput: soundOutput);

			newTuxState = collisionResultTux.NewTuxState;
			newEnemies = new List<IEnemy>(collisionResultTux.NewEnemies);

			newKilledEnemies.AddRange(collisionResultTux.NewlyKilledEnemies);

			CollisionProcessing_Enemies.Result collisionResultEnemy = CollisionProcessing_Enemies.ProcessFrame(enemies: newEnemies);

			newEnemies = new List<IEnemy>(collisionResultEnemy.NewEnemies);

			newKilledEnemies.AddRange(collisionResultEnemy.NewlyKilledEnemies);

			if (result.HasDied)
			{
				ITilemap restartedTilemap = gameLogicState.LevelConfiguration.GetTilemap(
					tuxX: null,
					tuxY: null,
					windowWidth: gameLogicState.WindowWidth,
					windowHeight: gameLogicState.WindowHeight);

				TuxState originalTuxState = TuxState.GetDefaultTuxState(x: restartedTilemap.GetTuxLocation(0, 0).Item1, y: restartedTilemap.GetTuxLocation(0, 0).Item2);

				newCamera = CameraStateProcessing.ComputeCameraState(
					tuxXMibi: originalTuxState.XMibi,
					tuxYMibi: originalTuxState.YMibi,
					tilemap: restartedTilemap,
					windowWidth: gameLogicState.WindowWidth,
					windowHeight: gameLogicState.WindowHeight);

				return new Result(
					newGameLogicState: new GameLogicState(
						levelConfiguration: gameLogicState.LevelConfiguration,
						background: gameLogicState.LevelConfiguration.GetBackground(),
						tilemap: restartedTilemap,
						tux: originalTuxState,
						camera: newCamera, 
						levelNumberDisplay: newLevelNumberDisplay,
						enemies: new List<IEnemy>(),
						killedEnemies: new List<string>(),
						frameCounter: gameLogicState.FrameCounter + 1,
						windowWidth: gameLogicState.WindowWidth,
						windowHeight: gameLogicState.WindowHeight,
						levelNumber: gameLogicState.LevelNumber,
						canUseSaveStates: gameLogicState.LevelNumber > CutsceneProcessing.LEVEL_THAT_GRANTS_SAVESTATES,
						canUseTimeSlowdown: gameLogicState.LevelNumber > CutsceneProcessing.LEVEL_THAT_GRANTS_TIME_SLOWDOWN,
						completedCutscenes: new List<string>(),
						cutscene: null),
					endLevel: result.EndLevel,
					playMusic: restartedTilemap.PlayMusic(),
					shouldStopMusic: result.ShouldStopMusic);
			}
			else
			{
				return new Result(
					newGameLogicState: new GameLogicState(
						levelConfiguration: gameLogicState.LevelConfiguration,
						background: gameLogicState.LevelConfiguration.GetBackground(),
						tilemap: newTilemap,
						tux: newTuxState,
						camera: newCamera,
						levelNumberDisplay: newLevelNumberDisplay,
						enemies: newEnemies,
						killedEnemies: newKilledEnemies,
						frameCounter: gameLogicState.FrameCounter + 1,
						windowWidth: gameLogicState.WindowWidth,
						windowHeight: gameLogicState.WindowHeight,
						levelNumber: gameLogicState.LevelNumber,
						canUseSaveStates: newCanUseSaveStates,
						canUseTimeSlowdown: newCanUseTimeSlowdown,
						completedCutscenes: newCompletedCutscenes,
						cutscene: newCutscene),
					endLevel: result.EndLevel,
					playMusic: newTilemap.PlayMusic(),
					shouldStopMusic: result.ShouldStopMusic);
			}
		}

		public static void Render(
			GameLogicState gameLogicState, 
			IDisplayOutput<GameImage, GameFont> displayOutput, 
			int elapsedMillis,
			bool debug_showHitboxes)
		{
			CameraState camera = gameLogicState.Camera;

			gameLogicState.Background.Render(
				cameraX: camera.X,
				cameraY: camera.Y,
				windowWidth: gameLogicState.WindowWidth,
				windowHeight: gameLogicState.WindowHeight,
				displayOutput: displayOutput);

			TranslatedDisplayOutput<GameImage, GameFont> translatedDisplayOutput = new TranslatedDisplayOutput<GameImage, GameFont>(
				display: displayOutput,
				xOffsetInPixels: -(camera.X - gameLogicState.WindowWidth / 2),
				yOffsetInPixels: -(camera.Y - gameLogicState.WindowHeight / 2));

			gameLogicState.Tilemap.RenderBackgroundTiles(
				displayOutput: translatedDisplayOutput, 
				cameraX: gameLogicState.Camera.X, 
				cameraY: gameLogicState.Camera.Y, 
				windowWidth: gameLogicState.WindowWidth, 
				windowHeight: gameLogicState.WindowHeight);
			
			foreach (IEnemy enemy in gameLogicState.Enemies)
				enemy.Render(displayOutput: translatedDisplayOutput);	

			TuxStateProcessing.Render(
				tuxState: gameLogicState.Tux,
				displayOutput: displayOutput, 
				camera: gameLogicState.Camera, 
				windowWidth: gameLogicState.WindowWidth, 
				windowHeight: gameLogicState.WindowHeight);

			gameLogicState.Tilemap.RenderForegroundTiles(
				displayOutput: translatedDisplayOutput, 
				cameraX: gameLogicState.Camera.X, 
				cameraY: gameLogicState.Camera.Y, 
				windowWidth: gameLogicState.WindowWidth, 
				windowHeight: gameLogicState.WindowHeight);

			if (debug_showHitboxes)
			{
				List<Hitbox> hitboxes = new List<Hitbox>();
				hitboxes.AddRange(gameLogicState.Tux.GetHitboxes());

				foreach (IEnemy enemy in gameLogicState.Enemies)
					hitboxes.AddRange(enemy.GetHitboxes());

				foreach (Hitbox hitbox in hitboxes)
				{
					translatedDisplayOutput.DrawRectangle(
						x: hitbox.X,
						y: hitbox.Y,
						width: hitbox.Width,
						height: hitbox.Height,
						color: new DTColor(255, 0, 0, 128),
						fill: true);
				}
			}

			gameLogicState.LevelNumberDisplay.Render(
				displayOutput: displayOutput,
				windowWidth: gameLogicState.WindowWidth, 
				windowHeight: gameLogicState.WindowHeight);

			string elapsedTimeString = ElapsedTimeUtil.GetElapsedTimeString(elapsedMillis: elapsedMillis);
			string timerText = "Time: " + elapsedTimeString;

			displayOutput.DrawText(
				x: gameLogicState.WindowWidth - 120,
				y: gameLogicState.WindowHeight - 10,
				text: timerText,
				font: GameFont.DTSimpleFont14Pt,
				color: DTColor.Black());

			if (gameLogicState.Cutscene != null)
				gameLogicState.Cutscene.Render(
					displayOutput: displayOutput,
					windowWidth: gameLogicState.WindowWidth,
					windowHeight: gameLogicState.WindowHeight);
		}
	}
}
