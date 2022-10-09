
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System.Collections.Generic;

	public class InitialLoadingScreenFrame : IFrame<GameImage, GameFont, GameSound, GameMusic>
	{
		private GlobalState globalState;
		private bool isPerformanceTest;

		public InitialLoadingScreenFrame(GlobalState globalState, bool isPerformanceTest)
		{
			this.globalState = globalState;
			this.isPerformanceTest = isPerformanceTest;
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
			var returnValue = this.GetNextFrameHelper(displayProcessing: displayProcessing, soundOutput: soundOutput, musicProcessing: musicProcessing);

			if (returnValue != null)
				return returnValue;

			returnValue = this.GetNextFrameHelper(displayProcessing: displayProcessing, soundOutput: soundOutput, musicProcessing: musicProcessing);

			if (returnValue != null)
				return returnValue;

			return this;
		}

		private IFrame<GameImage, GameFont, GameSound, GameMusic> GetNextFrameHelper(IDisplayProcessing<GameImage> displayProcessing, ISoundOutput<GameSound> soundOutput, IMusicProcessing musicProcessing)
		{
			bool isDoneLoadingImages = displayProcessing.LoadImages();

			if (!isDoneLoadingImages)
				return null;
			
			bool isDoneLoadingSounds = soundOutput.LoadSounds();

			if (!isDoneLoadingSounds)
				return null;
						
			bool isDoneLoadingMusic = musicProcessing.LoadMusic();

			if (!isDoneLoadingMusic)
				return null;

			SessionState sessionState = new SessionState(windowWidth: this.globalState.WindowWidth, windowHeight: this.globalState.WindowHeight);

			this.globalState.LoadSessionState(sessionState: sessionState);

			int? soundVolume = this.globalState.LoadSoundVolume();
			if (soundVolume.HasValue)
				soundOutput.SetSoundVolume(soundVolume.Value);

			this.globalState.LoadMusicVolume();

			if (this.isPerformanceTest)
				return PerformanceTestFrame.GetPerformanceTestFrame(
					globalState: this.globalState,
					displayProcessing: displayProcessing,
					soundOutput: soundOutput,
					musicProcessing: musicProcessing);
			else
				return new TitleScreenFrame(globalState: this.globalState, sessionState: sessionState);
		}

		public void ProcessMusic()
		{
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			displayOutput.DrawInitialLoadingScreen();
		}

		public void RenderMusic(IMusicOutput<GameMusic> musicOutput)
		{
		}
	}
}
