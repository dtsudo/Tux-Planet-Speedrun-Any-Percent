
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class LevelStartFrame : IFrame<GameImage, GameFont, GameSound, GameMusic>
	{
		private GlobalState globalState;
		private SessionState sessionState;

		private int extraElapsedMicros;

		private Level level;

		private Difficulty selectedDifficulty;
		private bool hasSelectedPlayGame;

		private bool hasStartedLevelTransition;

		private bool hasEasyReplay;
		private bool hasNormalReplay;
		private bool hasHardReplay;

		private IFrame<GameImage, GameFont, GameSound, GameMusic> underlyingFrame;

		public LevelStartFrame(
			GlobalState globalState, 
			SessionState sessionState,
			Level level,
			IFrame<GameImage, GameFont, GameSound, GameMusic> underlyingFrame)
		{
			this.globalState = globalState;
			this.sessionState = sessionState;

			this.extraElapsedMicros = 0;

			this.level = level;

			this.selectedDifficulty = sessionState.LastSelectedDifficulty;
			this.hasSelectedPlayGame = true;

			this.hasStartedLevelTransition = false;

			this.hasEasyReplay = sessionState.TryGetReplay(level: level, difficulty: Difficulty.Easy) != null;
			this.hasNormalReplay = sessionState.TryGetReplay(level: level, difficulty: Difficulty.Normal) != null;
			this.hasHardReplay = sessionState.TryGetReplay(level: level, difficulty: Difficulty.Hard) != null;

			this.underlyingFrame = underlyingFrame;
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
			this.sessionState.AddElapsedMillis(this.globalState.ElapsedMicrosPerFrame / 1000);

			this.extraElapsedMicros = this.extraElapsedMicros + (this.globalState.ElapsedMicrosPerFrame % 1000);
			if (this.extraElapsedMicros >= 1000)
			{
				this.extraElapsedMicros -= 1000;
				this.sessionState.AddElapsedMillis(1);
			}

			if (!this.hasStartedLevelTransition)
			{
				if (keyboardInput.IsPressed(Key.Esc) && !previousKeyboardInput.IsPressed(Key.Esc)
					|| keyboardInput.IsPressed(Key.X) && !previousKeyboardInput.IsPressed(Key.X))
				{
					return this.underlyingFrame;
				}

				if (keyboardInput.IsPressed(Key.UpArrow) && !previousKeyboardInput.IsPressed(Key.UpArrow))
					this.hasSelectedPlayGame = true;

				if (keyboardInput.IsPressed(Key.DownArrow) && !previousKeyboardInput.IsPressed(Key.DownArrow))
				{
					if (this.hasEasyReplay || this.hasNormalReplay || this.hasHardReplay)
					{
						this.hasSelectedPlayGame = false;

						if (this.sessionState.TryGetReplay(level: this.level, difficulty: this.selectedDifficulty) == null)
						{
							if (this.selectedDifficulty == Difficulty.Easy)
								this.selectedDifficulty = this.hasNormalReplay ? Difficulty.Normal : Difficulty.Hard;
							else if (this.selectedDifficulty == Difficulty.Normal)
								this.selectedDifficulty = this.hasEasyReplay ? Difficulty.Easy : Difficulty.Hard;
							else if (this.selectedDifficulty == Difficulty.Hard)
								this.selectedDifficulty = this.hasNormalReplay ? Difficulty.Normal : Difficulty.Easy;
							else
								throw new Exception();
						}
					}
				}

				if (keyboardInput.IsPressed(Key.LeftArrow) && !previousKeyboardInput.IsPressed(Key.LeftArrow))
				{
					if (this.hasSelectedPlayGame)
					{
						switch (this.selectedDifficulty)
						{
							case Difficulty.Easy:
								this.selectedDifficulty = Difficulty.Easy;
								break;
							case Difficulty.Normal:
								this.selectedDifficulty = Difficulty.Easy;
								break;
							case Difficulty.Hard:
								this.selectedDifficulty = Difficulty.Normal;
								break;
							default:
								throw new Exception();
						}
					}
					else
					{
						switch (this.selectedDifficulty)
						{
							case Difficulty.Easy:
								this.selectedDifficulty = Difficulty.Easy;
								break;
							case Difficulty.Normal:
								this.selectedDifficulty = Difficulty.Easy;

								if (!this.hasEasyReplay)
									this.hasSelectedPlayGame = true;

								break;
							case Difficulty.Hard:
								if (this.hasNormalReplay)
									this.selectedDifficulty = Difficulty.Normal;
								else if (this.hasEasyReplay)
									this.selectedDifficulty = Difficulty.Easy;
								else
								{
									this.selectedDifficulty = Difficulty.Normal;
									this.hasSelectedPlayGame = true;
								}

								break;
							default:
								throw new Exception();
						}
					}
				}

				if (keyboardInput.IsPressed(Key.RightArrow) && !previousKeyboardInput.IsPressed(Key.RightArrow))
				{
					if (this.hasSelectedPlayGame)
					{
						switch (this.selectedDifficulty)
						{
							case Difficulty.Easy:
								this.selectedDifficulty = Difficulty.Normal;
								break;
							case Difficulty.Normal:
								this.selectedDifficulty = Difficulty.Hard;
								break;
							case Difficulty.Hard:
								this.selectedDifficulty = Difficulty.Hard;
								break;
							default:
								throw new Exception();
						}
					}
					else
					{
						switch (this.selectedDifficulty)
						{
							case Difficulty.Easy:
								if (this.hasNormalReplay)
									this.selectedDifficulty = Difficulty.Normal;
								else if (this.hasHardReplay)
									this.selectedDifficulty = Difficulty.Hard;
								else
								{
									this.selectedDifficulty = Difficulty.Normal;
									this.hasSelectedPlayGame = true;
								}

								break;
							case Difficulty.Normal:
								this.selectedDifficulty = Difficulty.Hard;

								if (!this.hasHardReplay)
									this.hasSelectedPlayGame = true;

								break;
							case Difficulty.Hard:
								this.selectedDifficulty = Difficulty.Hard;
								break;
							default:
								throw new Exception();
						}
					}
				}

				if (keyboardInput.IsPressed(Key.Enter) && !previousKeyboardInput.IsPressed(Key.Enter)
					|| keyboardInput.IsPressed(Key.Space) && !previousKeyboardInput.IsPressed(Key.Space)
					|| keyboardInput.IsPressed(Key.Z) && !previousKeyboardInput.IsPressed(Key.Z))
				{
					this.hasStartedLevelTransition = true;

					if (this.hasSelectedPlayGame)
					{
						GameLogicState gameLogicState = this.sessionState.StartLevel(
							level: this.level,
							difficulty: this.selectedDifficulty,
							windowWidth: this.globalState.WindowWidth,
							windowHeight: this.globalState.WindowHeight,
							mapInfo: this.globalState.MapInfo);

						this.globalState.SaveData(sessionState: this.sessionState, soundVolume: soundOutput.GetSoundVolume());

						IFrame<GameImage, GameFont, GameSound, GameMusic> newFrame = GameFrame.GetGameFrame(
							globalState: this.globalState,
							sessionState: this.sessionState,
							gameLogicState: gameLogicState,
							displayProcessing: displayProcessing,
							soundOutput: soundOutput,
							musicProcessing: musicProcessing);

						return new LevelTransitionFrame(
							globalState: this.globalState,
							previousFrame: this,
							newFrame: newFrame);
					}
					else
					{
						IFrame<GameImage, GameFont, GameSound, GameMusic> newFrame = new ReplayFrame(
							globalState: this.globalState,
							sessionState: this.sessionState,
							replay: this.sessionState.TryGetReplay(level: this.level, difficulty: this.selectedDifficulty));

						this.globalState.SaveData(sessionState: this.sessionState, soundVolume: soundOutput.GetSoundVolume());

						return new LevelTransitionFrame(
							globalState: this.globalState,
							previousFrame: this,
							newFrame: newFrame);
					}
				}
			}

			return this;
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			this.underlyingFrame.Render(display: displayOutput);

			displayOutput.DrawRectangle(
				x: 0,
				y: 0,
				width: this.globalState.WindowWidth,
				height: this.globalState.WindowHeight,
				color: new DTColor(0, 0, 0, 215),
				fill: true);

			displayOutput.DrawRectangle(
				x: 50,
				y: 50,
				width: this.globalState.WindowWidth - 100,
				height: this.globalState.WindowHeight - 100,
				color: DTColor.White(),
				fill: true);

			displayOutput.DrawRectangle(
				x: 50,
				y: 50,
				width: this.globalState.WindowWidth - 100,
				height: this.globalState.WindowHeight - 100,
				color: DTColor.Black(),
				fill: false);

			displayOutput.DrawText(
				x: 429,
				y: this.globalState.WindowHeight - 65,
				text: this.level.GetLevelName(),
				font: GameFont.DTSimpleFont32Pt,
				color: DTColor.Black());

			LevelUtil.RenderLevelScreenshot(
				level: this.level,
				displayOutput: displayOutput,
				x: 75,
				y: 225);

			int labelX = 100;
			int easyX = 300;
			int normalX = 420;
			int hardX = 540;
			int startLevelY = 183;
			int watchReplayY = 120;

			string easyText = "    Easy";
			string normalText = " Normal";
			string hardText = "    Hard";

			displayOutput.DrawText(
				x: labelX,
				y: startLevelY,
				text: "Start level:",
				font: GameFont.DTSimpleFont20Pt,
				color: DTColor.Black());

			displayOutput.DrawText(
				x: easyX,
				y: startLevelY,
				text: easyText,
				font: GameFont.DTSimpleFont20Pt,
				color: DTColor.Black());

			displayOutput.DrawText(
				x: normalX,
				y: startLevelY,
				text: normalText,
				font: GameFont.DTSimpleFont20Pt,
				color: DTColor.Black());

			displayOutput.DrawText(
				x: hardX,
				y: startLevelY,
				text: hardText,
				font: GameFont.DTSimpleFont20Pt,
				color: DTColor.Black());

			if (this.hasEasyReplay || this.hasNormalReplay || this.hasHardReplay)
			{
				displayOutput.DrawText(
					x: labelX,
					y: watchReplayY,
					text: "Watch replay:",
					font: GameFont.DTSimpleFont20Pt,
					color: DTColor.Black());

				if (this.hasEasyReplay)
					displayOutput.DrawText(
						x: easyX,
						y: watchReplayY,
						text: easyText,
						font: GameFont.DTSimpleFont20Pt,
						color: DTColor.Black());

				if (this.hasNormalReplay)
					displayOutput.DrawText(
						x: normalX,
						y: watchReplayY,
						text: normalText,
						font: GameFont.DTSimpleFont20Pt,
						color: DTColor.Black());

				if (this.hasHardReplay)
					displayOutput.DrawText(
						x: hardX,
						y: watchReplayY,
						text: hardText,
						font: GameFont.DTSimpleFont20Pt,
						color: DTColor.Black());
			}

			displayOutput.DrawRectangle(
				x: this.selectedDifficulty == Difficulty.Easy
					? easyX
					: (this.selectedDifficulty == Difficulty.Normal ? normalX : hardX),
				y: (this.hasSelectedPlayGame ? startLevelY : watchReplayY) - 43,
				width: 120,
				height: 60,
				color: DTColor.Black(),
				fill: false);

			string elapsedTimeString = ElapsedTimeUtil.GetElapsedTimeString(elapsedMillis: this.sessionState.ElapsedMillis);
			string timerText = "Time: " + elapsedTimeString;

			displayOutput.DrawText(
				x: this.globalState.WindowWidth - 120,
				y: this.globalState.WindowHeight - 10,
				text: timerText,
				font: GameFont.DTSimpleFont14Pt,
				color: DTColor.Black());
		}
	}
}
