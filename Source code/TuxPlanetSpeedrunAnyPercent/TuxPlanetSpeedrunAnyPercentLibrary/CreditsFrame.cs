
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class CreditsFrame : IFrame<GameImage, GameFont, GameSound, GameMusic>
	{
		private enum Tab
		{
			DesignAndCoding,
			Images,
			Font,
			Sound,
			Music
		}

		private class TabButton
		{
			public TabButton(
				int x,
				int y,
				int width,
				int height,
				Tab tab,
				string tabName)
			{
				this.X = x;
				this.Y = y;
				this.Width = width;
				this.Height = height;
				this.Tab = tab;
				this.TabName = tabName;
			}

			public int X { get; private set; }
			public int Y { get; private set; }
			public int Width { get; private set; }
			public int Height { get; private set; }
			public Tab Tab { get; private set; }
			public string TabName { get; private set; }
		}

		private GlobalState globalState;
		private SessionState sessionState;

		private List<TabButton> tabButtons;
		private Tab selectedTab;

		private Tab? hoverTab;
		private Tab? clickTab;

		private Button backButton;

		private bool isHoverOverGitHubUrl;

		public CreditsFrame(GlobalState globalState, SessionState sessionState)
		{
			this.globalState = globalState;
			this.sessionState = sessionState;

			this.selectedTab = Tab.DesignAndCoding;
			this.hoverTab = null;
			this.clickTab = null;

			this.tabButtons = new List<TabButton>();
			this.tabButtons.Add(new TabButton(x: 20, y: 569, width: 234, height: 40, tab: Tab.DesignAndCoding, tabName: "Design and coding"));
			this.tabButtons.Add(new TabButton(x: 254, y: 569, width: 103, height: 40, tab: Tab.Images, tabName: "Images"));
			this.tabButtons.Add(new TabButton(x: 357, y: 569, width: 82, height: 40, tab: Tab.Font, tabName: "Font"));
			this.tabButtons.Add(new TabButton(x: 439, y: 569, width: 96, height: 40, tab: Tab.Sound, tabName: "Sound"));
			this.tabButtons.Add(new TabButton(x: 535, y: 569, width: 90, height: 40, tab: Tab.Music, tabName: "Music"));

			this.backButton = new Button(
				x: 780,
				y: 20,
				width: 200,
				height: 80,
				backgroundColor: Button.GetStandardPrimaryBackgroundColor(),
				hoverColor: Button.GetStandardHoverColor(),
				clickColor: Button.GetStandardClickColor(),
				text: "Back",
				textXOffset: 67,
				textYOffset: 28,
				font: GameFont.DTSimpleFont20Pt);

			this.isHoverOverGitHubUrl = false;
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
			int mouseX = mouseInput.GetX();
			int mouseY = mouseInput.GetY();

			this.isHoverOverGitHubUrl = false;

			this.hoverTab = null;
			foreach (TabButton tabButton in this.tabButtons)
			{
				if (tabButton.X <= mouseX && mouseX <= tabButton.X + tabButton.Width && tabButton.Y <= mouseY && mouseY <= tabButton.Y + tabButton.Height)
					this.hoverTab = tabButton.Tab;
			}

			if (mouseInput.IsLeftMouseButtonPressed() && !previousMouseInput.IsLeftMouseButtonPressed())
			{
				if (this.hoverTab != null)
					this.clickTab = this.hoverTab;
			}

			if (this.clickTab != null && !mouseInput.IsLeftMouseButtonPressed() && previousMouseInput.IsLeftMouseButtonPressed())
			{
				if (this.hoverTab.HasValue && this.hoverTab.Value == this.clickTab.Value)
				{
					soundOutput.PlaySound(GameSound.Click);
					this.selectedTab = this.clickTab.Value;
				}

				this.clickTab = null;
			}

			if (keyboardInput.IsPressed(Key.Esc) && !previousKeyboardInput.IsPressed(Key.Esc))
			{
				soundOutput.PlaySound(GameSound.Click);
				return new TitleScreenFrame(globalState: this.globalState, sessionState: this.sessionState);
			}

			bool clickedBackButton = this.backButton.ProcessFrame(mouseInput: mouseInput, previousMouseInput: previousMouseInput);
			if (clickedBackButton)
			{
				soundOutput.PlaySound(GameSound.Click);
				return new TitleScreenFrame(globalState: this.globalState, sessionState: this.sessionState);
			}

			if (this.selectedTab == Tab.DesignAndCoding)
			{
				this.isHoverOverGitHubUrl = Credits_DesignAndCoding.IsHoverOverGitHubUrl(
					mouse: new TranslatedMouse(mouse: mouseInput, xOffset: -20, yOffset: -120),
					isWebBrowserVersion: this.globalState.IsWebBrowserVersion,
					width: 960,
					height: 450);
			}

			return this;
		}

		public void ProcessExtraTime(int milliseconds)
		{
		}

		public string GetClickUrl()
		{
			if (this.isHoverOverGitHubUrl)
				return "https://github.com/dtsudo";

			return null;
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
				x: 422,
				y: 675,
				text: "Credits",
				font: GameFont.DTSimpleFont32Pt,
				color: DTColor.Black());

			displayOutput.DrawRectangle(
				x: 20,
				y: 120,
				width: 960 - 1,
				height: 450 - 1,
				color: DTColor.White(),
				fill: true);

			displayOutput.DrawRectangle(
				x: 20,
				y: 120,
				width: 960,
				height: 450,
				color: DTColor.Black(),
				fill: false);

			foreach (TabButton tabButton in this.tabButtons)
			{
				DTColor backgroundColor;

				if (tabButton.Tab == this.selectedTab)
					backgroundColor = DTColor.White();
				else if (this.clickTab.HasValue && this.clickTab.Value == tabButton.Tab)
					backgroundColor = Button.GetStandardClickColor();
				else if (this.hoverTab.HasValue && this.hoverTab.Value == tabButton.Tab)
					backgroundColor = Button.GetStandardHoverColor();
				else
					backgroundColor = Button.GetStandardSecondaryBackgroundColor();

				displayOutput.DrawRectangle(
					x: tabButton.X,
					y: tabButton.Y,
					width: tabButton.Width - 1,
					height: tabButton.Height - 1,
					color: backgroundColor,
					fill: true);

				displayOutput.DrawRectangle(
					x: tabButton.X,
					y: tabButton.Y,
					width: tabButton.Width,
					height: tabButton.Height,
					color: DTColor.Black(),
					fill: false);

				if (this.selectedTab == tabButton.Tab)
					displayOutput.DrawRectangle(
						x: tabButton.X + 1,
						y: tabButton.Y - 1,
						width: tabButton.Width - 2,
						height: 3,
						color: DTColor.White(),
						fill: true);

				displayOutput.DrawText(
					x: tabButton.X + 10,
					y: tabButton.Y + tabButton.Height - 10,
					text: tabButton.TabName,
					font: GameFont.DTSimpleFont18Pt,
					color: DTColor.Black());
			}

			IDisplayOutput<GameImage, GameFont> translatedDisplay = new TranslatedDisplayOutput<GameImage, GameFont>(
				display: displayOutput,
				xOffsetInPixels: 20,
				yOffsetInPixels: 120);

			if (this.selectedTab == Tab.DesignAndCoding)
				Credits_DesignAndCoding.Render(displayOutput: translatedDisplay, isHoverOverGitHubUrl: this.isHoverOverGitHubUrl, isWebBrowserVersion: this.globalState.IsWebBrowserVersion, width: 960, height: 450);
			if (this.selectedTab == Tab.Images)
				Credits_Images.Render(displayOutput: translatedDisplay, width: 960, height: 450);
			if (this.selectedTab == Tab.Font)
				Credits_Font.Render(displayOutput: translatedDisplay, width: 960, height: 450);
			if (this.selectedTab == Tab.Sound)
				Credits_Sound.Render(displayOutput: translatedDisplay, width: 960, height: 450);
			if (this.selectedTab == Tab.Music)
				Credits_Music.Render(displayOutput: translatedDisplay, width: 960, height: 450);

			this.backButton.Render(displayOutput: displayOutput);
		}

		public void RenderMusic(IMusicOutput<GameMusic> musicOutput)
		{
			this.globalState.RenderMusic(musicOutput: musicOutput);
		}
	}
}
