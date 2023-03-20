
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class ReplayFrame : IFrame<GameImage, GameFont, GameSound, GameMusic>
	{
		private GlobalState globalState;
		private SessionState sessionState;

		private Replay replay;
		
		private GameLogicState gameLogic;
		private bool hasStartedLevelTransition;
		private GameLogicState savedGameLogicState;

		private int numberOfSkippedFrames;

		public ReplayFrame(
			GlobalState globalState, 
			SessionState sessionState,
			Replay replay)
		{
			this.globalState = globalState;
			this.sessionState = sessionState;

			this.replay = replay;

			DTDeterministicRandom rngForGeneratingLevel = new DTDeterministicRandom();
			rngForGeneratingLevel.DeserializeFromString(replay.RngForGeneratingLevel);

			this.gameLogic = new GameLogicState(
				level: replay.Level,
				difficulty: replay.Difficulty,
				windowWidth: replay.WindowWidth,
				windowHeight: replay.WindowHeight,
				canUseSaveStates: replay.CanUseSaveStatesAtStartOfLevel,
				canUseTimeSlowdown: replay.CanUseTimeSlowdownAtStartOfLevel,
				canUseTeleport: replay.CanUseTeleportAtStartOfLevel,
				mapInfo: globalState.MapInfo,
				random: rngForGeneratingLevel);

			this.hasStartedLevelTransition = false;
			this.savedGameLogicState = null;
			this.numberOfSkippedFrames = 0;
		}

		public void ProcessExtraTime(int milliseconds)
		{
		}

		public string GetClickUrl()
		{
			return null;
		}

		public HashSet<string> GetCompletedAchievements()
		{
			return null;
		}

		public string GetScore()
		{
			return null;
		}

		public void ProcessMusic()
		{
			this.globalState.ProcessMusic();
		}

		public void RenderMusic(IMusicOutput<GameMusic> musicOutput)
		{
			this.globalState.RenderMusic(musicOutput: musicOutput);
		}

		public IFrame<GameImage, GameFont, GameSound, GameMusic> GetNextFrame(
			IKeyboard keyboardInput, 
			IMouse mouseInput, 
			IKeyboard previousKeyboardInput, 
			IMouse previousMouseInput, 
			IDisplayProcessing<GameImage> displayProcessing,
			ISoundOutput<GameSound> soundOutput,
			IMusicProcessing musicProcessing)
		{
			if (keyboardInput.IsPressed(Key.Esc) && !previousKeyboardInput.IsPressed(Key.Esc))
				return new ReplayPauseMenuFrame(
					globalState: this.globalState,
					sessionState: this.sessionState,
					replay: this.replay,
					replayFrame: this);

			bool shouldExecuteFrame;
			bool shouldEndLevel = false;

			bool isSlowingDownTime = keyboardInput.IsPressed(Key.Shift) && this.sessionState.CanUseTimeSlowdown;

			if (isSlowingDownTime)
			{
				this.numberOfSkippedFrames++;
				if (this.numberOfSkippedFrames == 5)
				{
					this.numberOfSkippedFrames = 0;
					shouldExecuteFrame = true;
				}
				else
					shouldExecuteFrame = false;
			}
			else
				shouldExecuteFrame = true;

			if (shouldExecuteFrame)
			{
				int numFramesToProcess;

				if (!isSlowingDownTime && keyboardInput.IsPressed(Key.Z))
					numFramesToProcess = 4;
				else
					numFramesToProcess = 1;

				for (int i = 0; i < numFramesToProcess; i++)
				{
					Move moveToUse;

					if (this.gameLogic.FrameCounter < this.replay.Moves.Count)
						moveToUse = this.replay.Moves[this.gameLogic.FrameCounter];
					else
						moveToUse = Move.EmptyMove();

					GameLogicStateProcessing.Result result = GameLogicStateProcessing.ProcessFrame(
						gameLogicState: this.gameLogic,
						move: moveToUse,
						debugMode: false,
						debug_tuxInvulnerable: false,
						debugKeyboardInput: new EmptyKeyboard(),
						debugPreviousKeyboardInput: new EmptyKeyboard(),
						displayProcessing: displayProcessing,
						soundOutput: soundOutput,
						elapsedMicrosPerFrame: this.globalState.ElapsedMicrosPerFrame);

					this.gameLogic = result.NewGameLogicState;

					if (result.PlayMusic != null)
						this.globalState.MusicPlayer.SetMusic(music: result.PlayMusic.Value, volume: 100);

					if (result.ShouldStopMusic)
						this.globalState.MusicPlayer.StopMusic();

					shouldEndLevel = shouldEndLevel || result.EndLevel;
				}
			}

			if (keyboardInput.IsPressed(Key.S) && !previousKeyboardInput.IsPressed(Key.S) && this.sessionState.CanUseSaveStates)
				this.savedGameLogicState = this.gameLogic;

			if (keyboardInput.IsPressed(Key.A) && !previousKeyboardInput.IsPressed(Key.A) && this.sessionState.CanUseSaveStates && !shouldEndLevel && !this.hasStartedLevelTransition)
			{
				if (this.savedGameLogicState != null)
					this.gameLogic = this.savedGameLogicState;
			}

			if (this.globalState.DebugMode)
			{
				if (keyboardInput.IsPressed(Key.H) && !previousKeyboardInput.IsPressed(Key.H))
					this.globalState.Debug_ShowHitBoxes = !this.globalState.Debug_ShowHitBoxes;
			}

			if (shouldEndLevel && !this.hasStartedLevelTransition)
			{
				this.hasStartedLevelTransition = true;

				return new LevelTransitionFrame(
					globalState: this.globalState,
					previousFrame: this,
					newFrame: new OverworldFrame(globalState: this.globalState, sessionState: this.sessionState));
			}

			return this;
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			GameLogicStateProcessing.Render(
				gameLogicState: this.gameLogic, 
				displayOutput: displayOutput, 
				elapsedMillis: this.sessionState.ElapsedMillis,
				showElapsedTime: false,
				debug_showHitboxes: this.globalState.Debug_ShowHitBoxes);
		}
	}
}
