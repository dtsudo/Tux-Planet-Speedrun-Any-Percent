
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class VictoryScreenFrame : IFrame<GameImage, GameFont, GameSound, GameMusic>
	{
		private GlobalState globalState;
		private SessionState sessionState;

		private bool updatedSession;

		public VictoryScreenFrame(GlobalState globalState, SessionState sessionState)
		{
			this.globalState = globalState;
			this.sessionState = sessionState;

			this.updatedSession = false;
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
			int elapsedMillis = this.sessionState.ElapsedMillis;
			return elapsedMillis.ToStringCultureInvariant();
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
			if (!this.updatedSession)
			{
				this.updatedSession = true;
				this.sessionState.WinGame();
				this.globalState.SaveData(sessionState: this.sessionState, soundVolume: soundOutput.GetSoundVolume());
			}

			if (keyboardInput.IsPressed(Key.Z) && !previousKeyboardInput.IsPressed(Key.Z)
				|| keyboardInput.IsPressed(Key.Enter) && !previousKeyboardInput.IsPressed(Key.Enter)
				|| keyboardInput.IsPressed(Key.Space) && !previousKeyboardInput.IsPressed(Key.Space)
				|| keyboardInput.IsPressed(Key.Esc) && !previousKeyboardInput.IsPressed(Key.Esc))
			{
				soundOutput.PlaySound(GameSound.Click);
				return new TitleScreenFrame(globalState: this.globalState, sessionState: this.sessionState);
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
				x: this.globalState.WindowWidth / 2 - 78,
				y: 650,
				text: "Victory",
				font: GameFont.DTSimpleFont32Pt,
				color: DTColor.Black());

			string elapsedTimeString = ElapsedTimeUtil.GetElapsedTimeString(elapsedMillis: this.sessionState.ElapsedMillis);

			displayOutput.DrawText(
				x: this.globalState.WindowWidth / 2 - 115,
				y: 450,
				text: "You win!" + "\n" + "Final time: " + elapsedTimeString,
				font: GameFont.DTSimpleFont20Pt,
				color: DTColor.Black());

			displayOutput.DrawText(
				x: this.globalState.WindowWidth / 2 - 131,
				y: 225,
				text: "Back to title screen",
				font: GameFont.DTSimpleFont20Pt,
				color: DTColor.Black());
		}

		public void RenderMusic(IMusicOutput<GameMusic> musicOutput)
		{
			this.globalState.RenderMusic(musicOutput: musicOutput);
		}
	}
}
