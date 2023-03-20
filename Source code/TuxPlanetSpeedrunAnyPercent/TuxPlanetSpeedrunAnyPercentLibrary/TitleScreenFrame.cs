
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class TitleScreenFrame : IFrame<GameImage, GameFont, GameSound, GameMusic>
	{
		private GlobalState globalState;
		private SessionState sessionState;

		private SoundAndMusicVolumePicker volumePicker;

		private Button clearDataButton;
		private Button creditsButton;

		private string versionString;

		private int selectedOption;

		private List<Option> options;

		private enum Option
		{
			ContinueGame,
			NewGame,
			Quit
		}

		public TitleScreenFrame(GlobalState globalState, SessionState sessionState)
		{
			this.globalState = globalState;
			this.sessionState = sessionState;
			this.volumePicker = null;

			this.versionString = VersionInfo.GetVersionInfo().Version;

			this.clearDataButton = new Button(
				x: 160,
				y: 10,
				width: 200,
				height: 31,
				backgroundColor: Button.GetStandardSecondaryBackgroundColor(),
				hoverColor: Button.GetStandardHoverColor(),
				clickColor: Button.GetStandardClickColor(),
				text: "Reset data",
				textXOffset: 40,
				textYOffset: 6,
				font: GameFont.DTSimpleFont16Pt);

			this.creditsButton = new Button(
				x: globalState.WindowWidth - 105,
				y: 5,
				width: 100,
				height: 35,
				backgroundColor: Button.GetStandardSecondaryBackgroundColor(),
				hoverColor: Button.GetStandardHoverColor(),
				clickColor: Button.GetStandardClickColor(),
				text: "Credits",
				textXOffset: 15,
				textYOffset: 10,
				font: GameFont.DTSimpleFont14Pt);

			this.selectedOption = 0;

			this.options = new List<Option>();

			if (CanContinueCurrentGame(sessionState: sessionState))
				this.options.Add(Option.ContinueGame);
			else
				this.options.Add(Option.NewGame);

			if (!globalState.IsWebBrowserVersion)
				this.options.Add(Option.Quit);
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

		private static bool CanContinueCurrentGame(SessionState sessionState)
		{
			return sessionState.HasStarted();
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
			this.sessionState.AddRandomSeed(17);

			if (this.volumePicker == null)
				this.volumePicker = new SoundAndMusicVolumePicker(
					xPos: 0,
					yPos: 0,
					initialSoundVolume: soundOutput.GetSoundVolume(),
					initialMusicVolume: this.globalState.MusicVolume,
					elapsedMicrosPerFrame: this.globalState.ElapsedMicrosPerFrame,
					color: SoundAndMusicVolumePicker.Color.Black);

			this.volumePicker.ProcessFrame(mouseInput: mouseInput, previousMouseInput: previousMouseInput);
			soundOutput.SetSoundVolume(volume: this.volumePicker.GetCurrentSoundVolume());
			this.globalState.MusicVolume = this.volumePicker.GetCurrentMusicVolume();

			this.globalState.MusicPlayer.SetMusic(GameMusic.Theme, volume: 100);

			if (CanContinueCurrentGame(this.sessionState))
			{
				bool clickedClearDataButton = this.clearDataButton.ProcessFrame(mouseInput: mouseInput, previousMouseInput: previousMouseInput);
				if (clickedClearDataButton)
				{
					this.globalState.SaveData(sessionState: this.sessionState, soundVolume: soundOutput.GetSoundVolume());
					soundOutput.PlaySound(GameSound.Click);
					return new ClearDataConfirmationFrame(globalState: this.globalState, sessionState: this.sessionState, underlyingFrame: this);
				}
			}

			if (this.globalState.DebugMode)
			{
				if (keyboardInput.IsPressed(Key.T) && !previousKeyboardInput.IsPressed(Key.T))
				{
					this.globalState.SaveData(sessionState: this.sessionState, soundVolume: soundOutput.GetSoundVolume());
					return new TestingFrame(globalState: this.globalState, sessionState: this.sessionState);
				}
			}

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
					case Option.NewGame:
						this.sessionState.ClearData(windowWidth: this.globalState.WindowWidth, windowHeight: this.globalState.WindowHeight);
						this.globalState.SaveData(sessionState: this.sessionState, soundVolume: soundOutput.GetSoundVolume());
						return new InstructionsFrame(globalState: this.globalState, sessionState: this.sessionState);
					case Option.ContinueGame:
						this.globalState.SaveData(sessionState: this.sessionState, soundVolume: soundOutput.GetSoundVolume());
						return new OverworldFrame(globalState: this.globalState, sessionState: this.sessionState);
					case Option.Quit:
						this.globalState.SaveData(sessionState: this.sessionState, soundVolume: soundOutput.GetSoundVolume());
						return null;
					default:
						throw new Exception();
				}
			}

			bool clickedCreditsButton = this.creditsButton.ProcessFrame(mouseInput: mouseInput, previousMouseInput: previousMouseInput);
			if (clickedCreditsButton)
			{
				this.globalState.SaveData(sessionState: this.sessionState, soundVolume: soundOutput.GetSoundVolume());
				soundOutput.PlaySound(GameSound.Click);
				return new CreditsFrame(globalState: this.globalState, sessionState: this.sessionState);
			}

			return this;
		}

		public void ProcessMusic()
		{
			this.globalState.ProcessMusic();
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			displayOutput.DrawRectangle(
				x: 0,
				y: 0,
				width: this.globalState.WindowWidth,
				height: this.globalState.WindowHeight,
				color: new DTColor(223, 220, 217),
				fill: true);

			displayOutput.DrawText(
				x: this.globalState.WindowWidth - 42,
				y: 55,
				text: "v" + this.versionString,
				font: GameFont.DTSimpleFont12Pt,
				color: DTColor.Black());

			this.creditsButton.Render(displayOutput: displayOutput);

			if (this.volumePicker != null)
				this.volumePicker.Render(displayOutput: displayOutput);

			displayOutput.DrawText(
				x: this.globalState.WindowWidth / 2 - 280,
				y: 510,
				text: "Tux Planet Speedrun Any%",
				font: GameFont.DTSimpleFont32Pt,
				color: DTColor.Black());

			for (int i = 0; i < this.options.Count; i++)
			{
				int x = this.globalState.WindowWidth / 2 - 75;

				if (this.options.Count == 1)
					x = this.globalState.WindowWidth / 2 - 129;

				int y = 350 - 50 * i;
				string text;

				switch (this.options[i])
				{
					case Option.ContinueGame:
						text = this.options.Count == 1 ? "Continue (press enter)" : "Continue";
						break;
					case Option.NewGame:
						text = this.options.Count == 1 ? "Start (press enter)" : "Start";
						break;
					case Option.Quit:
						text = "Quit";
						break;
					default:
						throw new Exception();
				}

				displayOutput.DrawText(
					x: x,
					y: y + (i == this.selectedOption ? 3 : 0),
					text: text,
					font: i == this.selectedOption ? GameFont.DTSimpleFont20Pt : GameFont.DTSimpleFont14Pt,
					color: i == this.selectedOption ? new DTColor(0, 0, 0) : new DTColor(64, 64, 64));
			}

			if (CanContinueCurrentGame(this.sessionState))
				this.clearDataButton.Render(displayOutput: displayOutput);
		}

		public void RenderMusic(IMusicOutput<GameMusic> musicOutput)
		{
			this.globalState.RenderMusic(musicOutput: musicOutput);
		}
	}
}
