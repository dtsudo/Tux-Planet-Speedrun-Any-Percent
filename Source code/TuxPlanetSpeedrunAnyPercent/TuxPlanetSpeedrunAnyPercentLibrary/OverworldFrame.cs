
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class OverworldFrame : IFrame<GameImage, GameFont, GameSound, GameMusic>
	{
		private GlobalState globalState;
		private SessionState sessionState;

		private int extraElapsedMicros;

		private Overworld overworld;

		private bool hasStartedLevelTransition;

		public OverworldFrame(GlobalState globalState, SessionState sessionState)
		{
			this.globalState = globalState;
			this.sessionState = sessionState;

			this.extraElapsedMicros = 0;

			this.overworld = sessionState.Overworld;

			this.hasStartedLevelTransition = false;
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

		public IFrame<GameImage, GameFont, GameSound, GameMusic> GetNextFrame(
			IKeyboard keyboardInput,
			IMouse mouseInput,
			IKeyboard previousKeyboardInput,
			IMouse previousMouseInput,
			IDisplayProcessing<GameImage> displayProcessing,
			ISoundOutput<GameSound> soundOutput,
			IMusicProcessing musicProcessing)
		{
			this.globalState.MusicPlayer.SetMusic(music: GameMusic.PeaceAtLast, volume: 100);
			
			this.sessionState.AddElapsedMillis(this.globalState.ElapsedMicrosPerFrame / 1000);

			this.extraElapsedMicros = this.extraElapsedMicros + (this.globalState.ElapsedMicrosPerFrame % 1000);
			if (this.extraElapsedMicros >= 1000)
			{
				this.extraElapsedMicros -= 1000;
				this.sessionState.AddElapsedMillis(1);
			}

			if (keyboardInput.IsPressed(Key.Esc) && !previousKeyboardInput.IsPressed(Key.Esc))
				return new PauseMenuFrame(
					globalState: this.globalState, 
					sessionState: this.sessionState, 
					underlyingFrame: this, 
					showRestartLevelOption: false, 
					showBackToMapOption: false,
					showToggleInputReplayFunctionalityOption: this.sessionState.CanUseSaveStates);

			Overworld.Result result = this.overworld.ProcessFrame(
				keyboardInput: keyboardInput,
				previousKeyboardInput: previousKeyboardInput,
				windowWidth: this.globalState.WindowWidth,
				windowHeight: this.globalState.WindowHeight,
				elapsedMicrosPerFrame: this.globalState.ElapsedMicrosPerFrame);

			this.overworld = result.Overworld;
			this.sessionState.SetOverworld(this.overworld);

			Level? level = result.SelectedLevel;

			if (level != null && !this.hasStartedLevelTransition)
			{
				this.hasStartedLevelTransition = true;

				this.sessionState.StartLevel(
					level: level.Value,
					windowWidth: this.globalState.WindowWidth,
					windowHeight: this.globalState.WindowHeight,
					mapInfo: this.globalState.MapInfo);

				this.globalState.SaveData(sessionState: this.sessionState, soundVolume: soundOutput.GetSoundVolume());

				IFrame<GameImage, GameFont, GameSound, GameMusic> newFrame = GameFrame.GetGameFrame(
					globalState: this.globalState,
					sessionState: this.sessionState,
					displayProcessing: displayProcessing,
					soundOutput: soundOutput,
					musicProcessing: musicProcessing);

				return new LevelTransitionFrame(
					globalState: this.globalState,
					previousFrame: this,
					newFrame: newFrame);
			}

			return this;
		}

		public void ProcessMusic()
		{
			this.globalState.ProcessMusic();
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			this.overworld.Render(displayOutput: displayOutput);

			string elapsedTimeString = ElapsedTimeUtil.GetElapsedTimeString(elapsedMillis: this.sessionState.ElapsedMillis);
			string timerText = "Time: " + elapsedTimeString;

			displayOutput.DrawText(
				x: this.globalState.WindowWidth - 120,
				y: this.globalState.WindowHeight - 10,
				text: timerText,
				font: GameFont.DTSimpleFont14Pt,
				color: DTColor.Black());
		}

		public void RenderMusic(IMusicOutput<GameMusic> musicOutput)
		{
			this.globalState.RenderMusic(musicOutput: musicOutput);
		}
	}
}
