
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;

	public class LevelTransitionFrame : IFrame<GameImage, GameFont, GameSound, GameMusic>
	{
		private GlobalState globalState;
		private SessionState sessionState;
		private IFrame<GameImage, GameFont, GameSound, GameMusic> previousFrame;
		private IFrame<GameImage, GameFont, GameSound, GameMusic> newFrame;

		private int elapsedMicros;

		private bool showRestartLevelOptionInPauseMenuOnFadeOut;
		private bool showRestartLevelOptionInPauseMenuOnFadeIn;

		private bool isFadingIn;
		private bool hasAdvancedPreviousFrameAtLeastOnce;

		private const int DURATION_OF_FADE_OUT = 1000 * 1000 / 2;
		private const int DURATION_OF_FADE_IN = 1000 * 1000 / 2;

		public LevelTransitionFrame(
			GlobalState globalState, 
			SessionState sessionState, 
			IFrame<GameImage, GameFont, GameSound, GameMusic> previousFrame,
			IFrame<GameImage, GameFont, GameSound, GameMusic> newFrame,
			bool showRestartLevelOptionInPauseMenuOnFadeOut,
			bool showRestartLevelOptionInPauseMenuOnFadeIn)
		{
			this.globalState = globalState;
			this.sessionState = sessionState;
			this.previousFrame = previousFrame;
			this.newFrame = newFrame;
			this.elapsedMicros = 0;

			this.showRestartLevelOptionInPauseMenuOnFadeOut = showRestartLevelOptionInPauseMenuOnFadeOut;
			this.showRestartLevelOptionInPauseMenuOnFadeIn = showRestartLevelOptionInPauseMenuOnFadeIn;

			this.isFadingIn = false;
			this.hasAdvancedPreviousFrameAtLeastOnce = false;
		}

		private class KeyboardWithoutEsc : IKeyboard
		{
			private IKeyboard underlyingKeyboard;

			public KeyboardWithoutEsc(IKeyboard underlyingKeyboard)
			{
				this.underlyingKeyboard = underlyingKeyboard;
			}

			public bool IsPressed(Key key)
			{
				if (key == Key.Esc)
					return false;

				return this.underlyingKeyboard.IsPressed(key);
			}
		}

		public void ProcessExtraTime(int milliseconds)
		{
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
			this.elapsedMicros += this.globalState.ElapsedMicrosPerFrame;

			if (!this.isFadingIn)
			{
				if (this.elapsedMicros > DURATION_OF_FADE_OUT)
					this.isFadingIn = true;
			}

			if (!this.isFadingIn || !this.hasAdvancedPreviousFrameAtLeastOnce)
			{
				this.hasAdvancedPreviousFrameAtLeastOnce = true;
				this.previousFrame = this.previousFrame.GetNextFrame(
					keyboardInput: new EmptyKeyboard(),
					mouseInput: new EmptyMouse(),
					previousKeyboardInput: new EmptyKeyboard(),
					previousMouseInput: new EmptyMouse(),
					displayProcessing: displayProcessing,
					soundOutput: soundOutput,
					musicProcessing: musicProcessing);

				if (this.previousFrame == null)
					return null;
			}

			if (this.isFadingIn)
			{
				this.newFrame = this.newFrame.GetNextFrame(
					keyboardInput: new KeyboardWithoutEsc(keyboardInput),
					mouseInput: mouseInput,
					previousKeyboardInput: new KeyboardWithoutEsc(previousKeyboardInput),
					previousMouseInput: previousMouseInput,
					displayProcessing: displayProcessing,
					soundOutput: soundOutput,
					musicProcessing: musicProcessing);

				if (this.newFrame == null)
					return null;
			}

			if (this.elapsedMicros > DURATION_OF_FADE_OUT + DURATION_OF_FADE_IN)
				return this.newFrame;

			if (keyboardInput.IsPressed(Key.Esc) && !previousKeyboardInput.IsPressed(Key.Esc))
			{
				return new PauseMenuFrame(
					globalState: this.globalState, 
					sessionState: this.sessionState, 
					underlyingFrame: this, 
					showRestartLevelOption: !this.isFadingIn ? this.showRestartLevelOptionInPauseMenuOnFadeOut : this.showRestartLevelOptionInPauseMenuOnFadeIn);
			}

			return this;
		}
		
		public void ProcessMusic()
		{
			this.globalState.ProcessMusic();
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			if (!this.isFadingIn)
			{
				this.previousFrame.Render(displayOutput);

				long elapsedMicrosLong = this.elapsedMicros;

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
			else
			{
				this.newFrame.Render(displayOutput);

				long elapsed = this.elapsedMicros - DURATION_OF_FADE_OUT;

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
		}

		public void RenderMusic(IMusicOutput<GameMusic> musicOutput)
		{
			this.globalState.RenderMusic(musicOutput: musicOutput);
		}
	}
}
