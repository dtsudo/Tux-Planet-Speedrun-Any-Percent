
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class InstructionsFrame : IFrame<GameImage, GameFont, GameSound, GameMusic>
	{
		private GlobalState globalState;
		private SessionState sessionState;

		private bool hasStartedGame;

		public InstructionsFrame(GlobalState globalState, SessionState sessionState)
		{
			this.globalState = globalState;
			this.sessionState = sessionState;

			this.hasStartedGame = false;
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

		public IFrame<GameImage, GameFont, GameSound, GameMusic> GetNextFrame(
			IKeyboard keyboardInput,
			IMouse mouseInput,
			IKeyboard previousKeyboardInput,
			IMouse previousMouseInput,
			IDisplayProcessing<GameImage> displayProcessing,
			ISoundOutput<GameSound> soundOutput,
			IMusicProcessing musicProcessing)
		{
			if ((keyboardInput.IsPressed(Key.Enter) && !previousKeyboardInput.IsPressed(Key.Enter)
				|| keyboardInput.IsPressed(Key.Space) && !previousKeyboardInput.IsPressed(Key.Space)
				|| keyboardInput.IsPressed(Key.Z) && !previousKeyboardInput.IsPressed(Key.Z))
					&&
				!this.hasStartedGame)
			{
				this.hasStartedGame = true;

				soundOutput.PlaySound(GameSound.Click);

				this.globalState.SaveData(sessionState: this.sessionState, soundVolume: soundOutput.GetSoundVolume());

				return new OverworldFrame(globalState: this.globalState, sessionState: this.sessionState);
			}

			if (keyboardInput.IsPressed(Key.Esc) && !previousKeyboardInput.IsPressed(Key.Esc))
			{
				soundOutput.PlaySound(GameSound.Click);
				return new PauseMenuFrame(
					globalState: this.globalState, 
					sessionState: this.sessionState, 
					underlyingFrame: this, 
					showRestartLevelOption: false, 
					showBackToMapOption: false,
					showToggleInputReplayFunctionalityOption: false);
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
				x: this.globalState.WindowWidth / 2 - 130,
				y: 650,
				text: "Instructions",
				font: GameFont.DTSimpleFont32Pt,
				color: DTColor.Black());

			displayOutput.DrawText(
				x: this.globalState.WindowWidth / 2 - 442,
				y: 500,
				text: "You are an elite speedrunner. Today, you're speedrunning the \n"
					+ "platforming game Tux Planet. \n\n"
					+ "Unfortunately, the devs are complete morons and the level design \n"
					+ "is terrible. Good luck! \n\n"
					+ "Controls: \n"
					+ "    Movement: Arrow keys \n"
					+ "    Jump: Z",
				font: GameFont.DTSimpleFont20Pt,
				color: DTColor.Black());

			displayOutput.DrawText(
				x: this.globalState.WindowWidth / 2 - 129,
				y: 160,
				text: "Start (press enter)",
				font: GameFont.DTSimpleFont20Pt,
				color: DTColor.Black());
		}

		public void RenderMusic(IMusicOutput<GameMusic> musicOutput)
		{
			this.globalState.RenderMusic(musicOutput: musicOutput);
		}
	}
}
