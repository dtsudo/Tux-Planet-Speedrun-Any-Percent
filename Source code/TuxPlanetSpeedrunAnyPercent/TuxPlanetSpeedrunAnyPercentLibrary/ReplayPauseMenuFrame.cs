
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class ReplayPauseMenuFrame : IFrame<GameImage, GameFont, GameSound, GameMusic>
	{
		private GlobalState globalState;
		private SessionState sessionState;
		private Replay replay;

		private IFrame<GameImage, GameFont, GameSound, GameMusic> replayFrame;

		private enum Option
		{
			Continue,
			RestartReplay,
			BackToMapScreen
		}

		private List<Option> options;
		private int selectedOption;

		public ReplayPauseMenuFrame(
			GlobalState globalState, 
			SessionState sessionState, 
			Replay replay,
			IFrame<GameImage, GameFont, GameSound, GameMusic> replayFrame)
		{
			this.globalState = globalState;
			this.sessionState = sessionState;
			this.replay = replay;
			this.replayFrame = replayFrame;

			this.selectedOption = 0;

			this.options = new List<Option>()
			{
				Option.Continue,
				Option.RestartReplay,
				Option.BackToMapScreen
			};
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
			this.globalState.RenderMusic(musicOutput);
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
			{
				soundOutput.PlaySound(GameSound.Click);
				this.globalState.SaveData(sessionState: this.sessionState, soundVolume: soundOutput.GetSoundVolume());
				return this.replayFrame;
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
						return this.replayFrame;
					case Option.RestartReplay:
						return new ReplayFrame(
							globalState: this.globalState,
							sessionState: this.sessionState,
							replay: this.replay); 
					case Option.BackToMapScreen:
						this.globalState.SaveData(sessionState: this.sessionState, soundVolume: soundOutput.GetSoundVolume());
						return new OverworldFrame(globalState: this.globalState, sessionState: this.sessionState);
					default:
						throw new Exception();
				}
			}

			return this;
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			this.replayFrame.Render(displayOutput);

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

			DTColor selectedColor = new DTColor(200, 255, 255);
			DTColor notSelectedColor = new DTColor(200, 200, 200);

			for (int i = 0; i < this.options.Count; i++)
			{
				int x = (this.globalState.WindowWidth >> 1) - 160;
				int y = 350 - 50 * i;
				string text;

				switch (this.options[i])
				{
					case Option.Continue:
						text = "Continue";
						break;
					case Option.RestartReplay:
						text = "Restart replay";
						break;
					case Option.BackToMapScreen:
						text = "Quit replay and return to map";
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
