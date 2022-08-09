
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class PauseMenuFrame : IFrame<GameImage, GameFont, GameSound, GameMusic>
	{
		private GlobalState globalState;
		private SessionState sessionState;

		private int selectedOption;

		private enum Option
		{
			Continue,
			RestartLevel,
			BackToMapScreen,
			BackToTitleScreen
		}

		private List<Option> options;

		private SoundAndMusicVolumePicker volumePicker;

		private IFrame<GameImage, GameFont, GameSound, GameMusic> underlyingFrame;

		public PauseMenuFrame(
			GlobalState globalState, 
			SessionState sessionState, 
			IFrame<GameImage, GameFont, GameSound, GameMusic> underlyingFrame, 
			bool showRestartLevelOption,
			bool showBackToMapOption)
		{
			this.globalState = globalState;
			this.sessionState = sessionState;
			this.volumePicker = null;
			this.underlyingFrame = underlyingFrame;

			this.selectedOption = 0;

			this.options = new List<Option>();
			this.options.Add(Option.Continue);
			if (showRestartLevelOption)
				this.options.Add(Option.RestartLevel);
			if (showBackToMapOption)
				this.options.Add(Option.BackToMapScreen);
			this.options.Add(Option.BackToTitleScreen);
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
			if (this.volumePicker == null)
				this.volumePicker = new SoundAndMusicVolumePicker(
					xPos: 0,
					yPos: 0,
					initialSoundVolume: soundOutput.GetSoundVolume(),
					initialMusicVolume: this.globalState.MusicVolume,
					elapsedMicrosPerFrame: this.globalState.ElapsedMicrosPerFrame,
					color: SoundAndMusicVolumePicker.Color.White);

			this.volumePicker.ProcessFrame(mouseInput: mouseInput, previousMouseInput: previousMouseInput);
			soundOutput.SetSoundVolume(volume: this.volumePicker.GetCurrentSoundVolume());
			this.globalState.MusicVolume = this.volumePicker.GetCurrentMusicVolume();

			if (keyboardInput.IsPressed(Key.Esc) && !previousKeyboardInput.IsPressed(Key.Esc))
			{
				soundOutput.PlaySound(GameSound.Click);
				this.globalState.SaveData(sessionState: this.sessionState, soundVolume: soundOutput.GetSoundVolume());
				return this.underlyingFrame;
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
					case Option.Continue:
						this.globalState.SaveData(sessionState: this.sessionState, soundVolume: soundOutput.GetSoundVolume());
						return this.underlyingFrame;
					case Option.RestartLevel:
						this.sessionState.StartLevel(
							level: this.sessionState.CurrentLevel.Value,
							windowWidth: this.globalState.WindowWidth,
							windowHeight: this.globalState.WindowHeight,
							mapInfo: this.globalState.MapInfo);
						this.globalState.SaveData(sessionState: this.sessionState, soundVolume: soundOutput.GetSoundVolume());
						return GameFrame.GetGameFrame(
							globalState: this.globalState,
							sessionState: this.sessionState,
							displayProcessing: displayProcessing,
							soundOutput: soundOutput,
							musicProcessing: musicProcessing);
					case Option.BackToMapScreen:
						this.sessionState.SetGameLogic(gameLogicState: null);
						this.globalState.SaveData(sessionState: this.sessionState, soundVolume: soundOutput.GetSoundVolume());
						return new OverworldFrame(globalState: this.globalState, sessionState: this.sessionState);
					case Option.BackToTitleScreen:
						this.globalState.SaveData(sessionState: this.sessionState, soundVolume: soundOutput.GetSoundVolume());
						return new TitleScreenFrame(globalState: this.globalState, sessionState: this.sessionState);
					default:
						throw new Exception();
				}
			}

			return this;
		}

		public void ProcessExtraTime(int milliseconds)
		{
		}

		public string GetClickUrl()
		{
			return null;
		}

		public void ProcessMusic()
		{
			this.globalState.ProcessMusic();
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			this.underlyingFrame.Render(displayOutput);

			displayOutput.DrawRectangle(
				x: 0,
				y: 0,
				width: this.globalState.WindowWidth,
				height: this.globalState.WindowHeight,
				color: new DTColor(0, 0, 0, 215),
				fill: true);

			displayOutput.DrawText(
				x: this.globalState.WindowWidth / 2 - 73,
				y: 650,
				text: "Paused",
				font: GameFont.DTSimpleFont32Pt,
				color: DTColor.White());

			if (this.volumePicker != null)
				this.volumePicker.Render(displayOutput: displayOutput);

			DTColor selectedColor = new DTColor(200, 255, 255);
			DTColor notSelectedColor = new DTColor(200, 200, 200);

			for (int i = 0; i < this.options.Count; i++)
			{
				int x = this.globalState.WindowWidth / 2 - 105;
				int y = 350 - 50 * i;
				string text;

				switch (this.options[i])
				{
					case Option.Continue:
						text = "Continue";
						break;
					case Option.RestartLevel:
						text = "Restart level";
						break;
					case Option.BackToMapScreen:
						text = "Quit level and return to map";
						break;
					case Option.BackToTitleScreen:
						text = "Back to title screen";
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

		public void RenderMusic(IMusicOutput<GameMusic> musicOutput)
		{
			this.globalState.RenderMusic(musicOutput);
		}
	}
}
