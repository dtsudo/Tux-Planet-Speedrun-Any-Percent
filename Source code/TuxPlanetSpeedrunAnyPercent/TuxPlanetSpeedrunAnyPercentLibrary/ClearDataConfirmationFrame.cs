
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class ClearDataConfirmationFrame : IFrame<GameImage, GameFont, GameSound, GameMusic>
	{
		private GlobalState globalState;
		private SessionState sessionState;
		private IFrame<GameImage, GameFont, GameSound, GameMusic> underlyingFrame;

		private Button confirmButton;
		private Button cancelButton;

		private int panelX;
		private int panelY;

		private const int PANEL_WIDTH = 480;
		private const int PANEL_HEIGHT = 150;

		public ClearDataConfirmationFrame(GlobalState globalState, SessionState sessionState, IFrame<GameImage, GameFont, GameSound, GameMusic> underlyingFrame)
		{
			this.globalState = globalState;
			this.sessionState = sessionState;
			this.underlyingFrame = underlyingFrame;

			this.panelX = (globalState.WindowWidth - PANEL_WIDTH) / 2;
			this.panelY = (globalState.WindowHeight - PANEL_HEIGHT) / 2;

			int buttonWidth = 150;
			int buttonHeight = 40;

			this.confirmButton = new Button(
				x: this.panelX + 80,
				y: this.panelY + 20,
				width: buttonWidth,
				height: buttonHeight,
				backgroundColor: Button.GetStandardSecondaryBackgroundColor(),
				hoverColor: Button.GetStandardHoverColor(),
				clickColor: Button.GetStandardClickColor(),
				text: "Yes",
				textXOffset: 47,
				textYOffset: 8,
				font: GameFont.DTSimpleFont20Pt);

			this.cancelButton = new Button(
				x: this.panelX + 250,
				y: this.panelY + 20,
				width: buttonWidth,
				height: buttonHeight,
				backgroundColor: Button.GetStandardSecondaryBackgroundColor(),
				hoverColor: Button.GetStandardHoverColor(),
				clickColor: Button.GetStandardClickColor(),
				text: "No",
				textXOffset: 55,
				textYOffset: 8,
				font: GameFont.DTSimpleFont20Pt);
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
			return new HashSet<string>();
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
			if (keyboardInput.IsPressed(Key.Esc) && !previousKeyboardInput.IsPressed(Key.Esc))
			{
				soundOutput.PlaySound(GameSound.Click);
				return new TitleScreenFrame(globalState: this.globalState, sessionState: this.sessionState);
			}

			bool isConfirmClicked = this.confirmButton.ProcessFrame(
				mouseInput: mouseInput,
				previousMouseInput: previousMouseInput);

			bool isCancelClicked = this.cancelButton.ProcessFrame(
				mouseInput: mouseInput,
				previousMouseInput: previousMouseInput);

			if (isConfirmClicked)
			{
				this.sessionState.ClearData(windowWidth: this.globalState.WindowWidth, windowHeight: this.globalState.WindowHeight);
				this.globalState.SaveData(sessionState: this.sessionState, soundVolume: soundOutput.GetSoundVolume());
				soundOutput.PlaySound(GameSound.Click);
				return new TitleScreenFrame(globalState: this.globalState, sessionState: this.sessionState);
			}

			if (isCancelClicked)
			{
				soundOutput.PlaySound(GameSound.Click);
				return new TitleScreenFrame(globalState: this.globalState, sessionState: this.sessionState);
			}

			return this;
		}

		public void ProcessMusic()
		{
			this.underlyingFrame.ProcessMusic();
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			this.underlyingFrame.Render(display: displayOutput);

			displayOutput.DrawRectangle(
				x: 0,
				y: 0,
				width: this.globalState.WindowWidth,
				height: this.globalState.WindowHeight,
				color: new DTColor(r: 0, g: 0, b: 0, alpha: 64),
				fill: true);

			displayOutput.DrawRectangle(
				x: this.panelX,
				y: this.panelY,
				width: PANEL_WIDTH - 1,
				height: PANEL_HEIGHT - 1,
				color: DTColor.White(),
				fill: true);

			displayOutput.DrawRectangle(
				x: this.panelX,
				y: this.panelY,
				width: PANEL_WIDTH,
				height: PANEL_HEIGHT,
				color: DTColor.Black(),
				fill: false);

			displayOutput.DrawText(
				x: this.panelX + 27,
				y: this.panelY + 132,
				text: "Are you sure you want to reset" + "\n" + "your progress?",
				font: GameFont.DTSimpleFont20Pt,
				color: DTColor.Black());

			this.confirmButton.Render(displayOutput: displayOutput);
			this.cancelButton.Render(displayOutput: displayOutput);
		}

		public void RenderMusic(IMusicOutput<GameMusic> musicOutput)
		{
			this.underlyingFrame.RenderMusic(musicOutput: musicOutput);
		}
	}
}
