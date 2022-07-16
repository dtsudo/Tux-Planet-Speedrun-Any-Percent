
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;

	public class TestingFontFrame : IFrame<GameImage, GameFont, GameSound, GameMusic>
	{
		private GlobalState globalState;
		private SessionState sessionState;

		public TestingFontFrame(GlobalState globalState, SessionState sessionState)
		{
			this.globalState = globalState;
			this.sessionState = sessionState;
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
			if (keyboardInput.IsPressed(Key.Esc) && !previousKeyboardInput.IsPressed(Key.Esc))
				return new TestingFrame(globalState: this.globalState, sessionState: this.sessionState);

			return this;
		}
		
		public void ProcessMusic()
		{
			this.globalState.ProcessMusic();
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			DTColor red = new DTColor(255, 0, 0);

			displayOutput.DrawRectangle(
				x: 50,
				y: 589,
				width: 822,
				height: 60,
				color: red,
				fill: false);

			displayOutput.DrawText(
				x: 50,
				y: 650,
				text: "Line 1 ABCDEFGHIJKLMNOPQRSTUVWXYZ ABCDEFGHIJKLMNOPQRSTUVWXYZ ABCDEFGHIJKLMNOPQRSTUVWXYZ"
					+ "\n" + "Line 2"
					+ "\n" + "Line 3"
					+ "\n" + "Line 4",
				font: GameFont.DTSimpleFont12Pt,
				color: DTColor.Black());

			displayOutput.DrawRectangle(
				x: 51,
				y: 481,
				width: 637,
				height: 68,
				color: red,
				fill: false);

			displayOutput.DrawText(
				x: 50,
				y: 550,
				text: "Line 1 abcdefghijklmnopqrstuvwxyz abcdefghijklmnopqrstuvwxyz"
					+ "\n" + "Line 2"
					+ "\n" + "Line 3"
					+ "\n" + "Line 4",
				font: GameFont.DTSimpleFont14Pt,
				color: DTColor.Black());
		}

		public void RenderMusic(IMusicOutput<GameMusic> musicOutput)
		{
			this.globalState.RenderMusic(musicOutput: musicOutput);
		}
	}
}
