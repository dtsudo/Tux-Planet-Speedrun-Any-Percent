
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class LevelCompleteFrame : IFrame<GameImage, GameFont, GameSound, GameMusic>
	{
		private GlobalState globalState;
		private SessionState sessionState;
		private Level level;
		private Difficulty difficulty;
		private Replay replay;
		private IFrame<GameImage, GameFont, GameSound, GameMusic> gameFrame;
		private IFrame<GameImage, GameFont, GameSound, GameMusic> newFrame;

		private int fadeOutElapsedMicros;
		private int fadeInElapsedMicros;

		private bool isFadingOut;
		private bool isFadingIn;

		private enum Option
		{
			Continue,
			WatchReplay,
			RestartLevel
		}

		private List<Option> options;
		private int selectedOption;

		private const int DURATION_OF_FADE_OUT = 1000 * 1000 / 2;
		private const int DURATION_OF_FADE_IN = 1000 * 1000 / 2;

		public LevelCompleteFrame(
			GlobalState globalState, 
			SessionState sessionState,
			Level level,
			Difficulty difficulty,
			Replay replay,
			IFrame<GameImage, GameFont, GameSound, GameMusic> gameFrame)
		{
			this.globalState = globalState;
			this.sessionState = sessionState;
			this.level = level;
			this.difficulty = difficulty;
			this.replay = replay;
			this.gameFrame = gameFrame;
			this.fadeOutElapsedMicros = 0;
			this.fadeInElapsedMicros = 0;

			this.isFadingOut = true;
			this.isFadingIn = false;

			this.options = new List<Option>()
			{
				Option.Continue,
				Option.WatchReplay,
				Option.RestartLevel
			};

			this.selectedOption = 0;

			this.newFrame = null;
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
			return Achievements.GetCompletedAchievements(numCompletedLevels: this.sessionState.Overworld.GetNumCompletedLevels());
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
			if (this.isFadingOut)
			{
				this.fadeOutElapsedMicros += this.globalState.ElapsedMicrosPerFrame;
				if (this.fadeOutElapsedMicros >= DURATION_OF_FADE_OUT)
					this.isFadingOut = false;
			}

			if (this.isFadingOut)
			{
				this.gameFrame = this.gameFrame.GetNextFrame(
					keyboardInput: new EmptyKeyboard(),
					mouseInput: new EmptyMouse(),
					previousKeyboardInput: new EmptyKeyboard(),
					previousMouseInput: new EmptyMouse(),
					displayProcessing: displayProcessing,
					soundOutput: soundOutput,
					musicProcessing: musicProcessing);
			}

			if (this.isFadingIn)
			{
				this.fadeInElapsedMicros += this.globalState.ElapsedMicrosPerFrame;
				if (this.fadeInElapsedMicros >= DURATION_OF_FADE_IN)
					return this.newFrame;

				this.newFrame = this.newFrame.GetNextFrame(
					keyboardInput: keyboardInput,
					mouseInput: mouseInput,
					previousKeyboardInput: previousKeyboardInput,
					previousMouseInput: previousMouseInput,
					displayProcessing: displayProcessing,
					soundOutput: soundOutput,
					musicProcessing: musicProcessing);

				if (this.newFrame == null)
					return null;
			}

			if (!this.isFadingOut && !this.isFadingIn)
			{
				if (keyboardInput.IsPressed(Key.UpArrow) && !previousKeyboardInput.IsPressed(Key.UpArrow))
				{
					this.selectedOption--;
					if (this.selectedOption == -1)
						this.selectedOption = this.options.Count - 1;
				}

				if (keyboardInput.IsPressed(Key.DownArrow) && !previousKeyboardInput.IsPressed(Key.DownArrow))
				{
					this.selectedOption++;
					if (this.selectedOption == this.options.Count)
						this.selectedOption = 0;
				}

				if (keyboardInput.IsPressed(Key.Enter) && !previousKeyboardInput.IsPressed(Key.Enter)
								|| keyboardInput.IsPressed(Key.Space) && !previousKeyboardInput.IsPressed(Key.Space)
								|| keyboardInput.IsPressed(Key.Z) && !previousKeyboardInput.IsPressed(Key.Z))
				{
					soundOutput.PlaySound(GameSound.Click);

					switch (this.options[this.selectedOption])
					{
						case Option.Continue:
							this.newFrame = new OverworldFrame(globalState: this.globalState, sessionState: this.sessionState);
							this.isFadingIn = true;
							break;
						case Option.WatchReplay:
							this.newFrame = new ReplayFrame(globalState: this.globalState, sessionState: this.sessionState, replay: this.replay);
							this.isFadingIn = true;
							break;
						case Option.RestartLevel:
							GameLogicState gameLogicState = this.sessionState.StartLevel(
								level: this.level,
								difficulty: this.difficulty,
								windowWidth: this.globalState.WindowWidth,
								windowHeight: this.globalState.WindowHeight,
								mapInfo: this.globalState.MapInfo);
							this.globalState.SaveData(sessionState: this.sessionState, soundVolume: soundOutput.GetSoundVolume());
							this.newFrame = GameFrame.GetGameFrame(
								globalState: this.globalState,
								sessionState: this.sessionState,
								gameLogicState: gameLogicState,
								displayProcessing: displayProcessing,
								soundOutput: soundOutput,
								musicProcessing: musicProcessing);
							this.isFadingIn = true;
							break;
						default:
							throw new Exception();
					}
				}
			}

			return this;
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			if (this.isFadingOut)
			{
				this.gameFrame.Render(displayOutput);

				long elapsedMicrosLong = this.fadeOutElapsedMicros;

				int alpha = (int)(elapsedMicrosLong * 255L / DURATION_OF_FADE_OUT);

				if (alpha > 255)
					alpha = 255;
				if (alpha < 0)
					alpha = 0;

				displayOutput.DrawRectangle(
					x: 0,
					y: 0,
					width: this.globalState.WindowWidth,
					height: this.globalState.WindowHeight,
					color: new DTColor(0, 0, 0, alpha),
					fill: true);
			}
			else if (this.isFadingIn)
			{
				this.newFrame.Render(displayOutput);

				long elapsed = this.fadeInElapsedMicros;

				int alpha = (int)(255L - elapsed * 255L / DURATION_OF_FADE_IN);

				if (alpha > 255)
					alpha = 255;
				if (alpha < 0)
					alpha = 0;

				displayOutput.DrawRectangle(
					x: 0,
					y: 0,
					width: this.globalState.WindowWidth,
					height: this.globalState.WindowHeight,
					color: new DTColor(0, 0, 0, alpha),
					fill: true);
			}
			else
			{
				displayOutput.DrawRectangle(
					x: 0,
					y: 0,
					width: this.globalState.WindowWidth,
					height: this.globalState.WindowHeight,
					color: new DTColor(0, 0, 0, 255),
					fill: true);

				displayOutput.DrawText(
					x: this.globalState.WindowWidth / 2 - 164,
					y: 650,
					text: "Level Complete",
					font: GameFont.DTSimpleFont32Pt,
					color: DTColor.White());

				DTColor selectedColor = new DTColor(200, 255, 255);
				DTColor notSelectedColor = new DTColor(200, 200, 200);

				for (int i = 0; i < this.options.Count; i++)
				{
					int x = this.globalState.WindowWidth / 2 - 75;
					int y = 350 - 50 * i;
					string text;

					switch (this.options[i])
					{
						case Option.Continue:
							text = "Continue";
							break;
						case Option.WatchReplay:
							text = "Watch replay";
							break;
						case Option.RestartLevel:
							text = "Restart level";
							break;
						default:
							throw new Exception();
					}

					displayOutput.DrawText(
						x: x,
						y: y,
						text: text,
						font: GameFont.DTSimpleFont16Pt,
						color: i == this.selectedOption ? selectedColor : notSelectedColor);
				}
			}
		}
	}
}
