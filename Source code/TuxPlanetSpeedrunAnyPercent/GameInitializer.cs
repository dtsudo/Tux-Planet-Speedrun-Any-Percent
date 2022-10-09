
namespace TuxPlanetSpeedrunAnyPercent
{
	using Bridge;
	using DTLibrary;
	using System.Collections.Generic;
	using TuxPlanetSpeedrunAnyPercentLibrary;

	public class GameInitializer
	{
		private static IKeyboard bridgeKeyboard;
		private static IMouse bridgeMouse;
		private static IKeyboard previousKeyboard;
		private static IMouse previousMouse;
		
		private static DTDisplay<GameImage, GameFont> display;
		private static ISoundOutput<GameSound> soundOutput;
		private static IMusic<GameMusic> music;
		
		private static DisplayLogger displayLogger;
		private static bool shouldRenderDisplayLogger;
		
		private static HashSet<string> completedAchievements;
		private static string score;
		
		private static IFrame<GameImage, GameFont, GameSound, GameMusic> frame;
		
		private static bool hasInitializedClearCanvasJavascript;

		private static string clickUrl;
		
		private static void InitializeClearCanvasJavascript()
		{
			Script.Eval(@"
				window.BridgeClearCanvasJavascript = ((function () {
					'use strict';
					
					var canvas = null;
					var context = null;
								
					var clearCanvas = function () {
						if (canvas === null) {
							canvas = document.getElementById('bridgeCanvas');
							if (canvas === null)
								return;	
							context = canvas.getContext('2d', { alpha: false });
						}
						
						context.clearRect(0, 0, canvas.width, canvas.height);
					};
					
					return {
						clearCanvas: clearCanvas
					};
				})());
			");
		}
		
		private static void ClearCanvas()
		{
			if (!hasInitializedClearCanvasJavascript)
			{
				InitializeClearCanvasJavascript();
				hasInitializedClearCanvasJavascript = true;
			}
			
			Script.Write("window.BridgeClearCanvasJavascript.clearCanvas()");
		}
		
		private static void ClearClickUrl()
		{
			Script.Eval("window.bridgeClickUrl = null;");
		}
		
		private static void UpdateClickUrl(string clickUrl)
		{
			Script.Eval("window.bridgeClickUrl = '" + clickUrl + "';");
		}
		
		private static void AddClickUrlListener()
		{
			Script.Eval(@"
				document.addEventListener('click', function (e) {
					if (window.bridgeClickUrl !== undefined
							&& window.bridgeClickUrl !== null
							&& window.bridgeClickUrl !== '')
						window.open(window.bridgeClickUrl, '_blank');
				}, false);
			");
		}
		
		private static void RemoveMarginOnBody()
		{
			Script.Eval(@"
				((function () {
					var removeMargin;
					
					removeMargin = function () {
						var bodyElement = document.body;
						
						if (!bodyElement) {
							setTimeout(removeMargin, 50);
							return;
						}
						
						bodyElement.style.margin = '0px';
					};
					
					removeMargin();
				})());
			");
		}
		
		public static void Start(
			int fps, 
			bool isWebPortalVersion,
			bool debugMode)
		{
			hasInitializedClearCanvasJavascript = false;
			
			clickUrl = null;
			
			completedAchievements = new HashSet<string>();
			score = null;
			
			ClearClickUrl();
			
			AddClickUrlListener();
			
			if (isWebPortalVersion)
				RemoveMarginOnBody();
			
			shouldRenderDisplayLogger = true;
			
			IDTLogger logger;
			if (debugMode)
			{
				displayLogger = new DisplayLogger(x: 5, y: 95);
				logger = displayLogger;
			}
			else
			{
				displayLogger = null;
				logger = new EmptyLogger();
			}

			int windowWidth = 1000;
			int windowHeight = 700;

			GlobalState globalState = new GlobalState(
					windowWidth: windowWidth,
					windowHeight: windowHeight,
					fps: fps,
					rng: new DTRandom(),
					guidGenerator: new GuidGenerator(guidString: "391523846186017403"),
					logger: logger,
					timer: new SimpleTimer(),
					fileIO: new BridgeFileIO(),
					isWebBrowserVersion: true,
					isWebPortalVersion: isWebPortalVersion,
					debugMode: debugMode,
					initialMusicVolume: null);
			
			frame = TuxPlanetSpeedrunAnyPercent.GetFirstFrame(globalState: globalState);

			bridgeKeyboard = new BridgeKeyboard(disableArrowKeyScrolling: isWebPortalVersion);
			bridgeMouse = new BridgeMouse();
						
			display = new BridgeDisplay(windowHeight: windowHeight);
			soundOutput = new BridgeSoundOutput(elapsedMicrosPerFrame: globalState.ElapsedMicrosPerFrame);
			music = new BridgeMusic();

			previousKeyboard = new EmptyKeyboard();
			previousMouse = new EmptyMouse();
			
			ClearCanvas();
			frame.Render(display);
			frame.RenderMusic(music);
			if (displayLogger != null && shouldRenderDisplayLogger)
				displayLogger.Render(displayOutput: display, font: GameFont.DTSimpleFont14Pt, color: DTColor.Black());
		}
		
		public static void ProcessExtraTime(int milliseconds)
		{
			frame.ProcessExtraTime(milliseconds: milliseconds);
		}
		
		private static void AddAchievementToJavascriptArray(string achievement)
		{
			Script.Eval("if (!window.BridgeCompletedAchievements) window.BridgeCompletedAchievements = [];");
			Script.Eval("window.BridgeCompletedAchievements.push('" + achievement + "');");
		}
		
		private static void AddScoreInJavascript(string score)
		{
			Script.Eval("window.BridgeScore = '" + score + "';");
		}

		public static void ComputeAndRenderNextFrame()
		{
			IKeyboard currentKeyboard = new CopiedKeyboard(bridgeKeyboard);
			IMouse currentMouse = new CopiedMouse(bridgeMouse);
			
			frame = frame.GetNextFrame(currentKeyboard, currentMouse, previousKeyboard, previousMouse, display, soundOutput, music);
			frame.ProcessMusic();
			soundOutput.ProcessFrame();
			ClearCanvas();
			frame.Render(display);
			frame.RenderMusic(music);

			HashSet<string> newCompletedAchievements = frame.GetCompletedAchievements();

			if (newCompletedAchievements != null)
			{
				foreach (string completedAchievement in newCompletedAchievements)
				{
					bool wasAdded = completedAchievements.Add(completedAchievement);

					if (wasAdded)
						AddAchievementToJavascriptArray(completedAchievement);
				}
			}
			
			string newScore = frame.GetScore();
			
			if (newScore != null && (score == null || newScore != score))
			{
				score = newScore;
				AddScoreInJavascript(newScore);
			}

			string newClickUrl = frame.GetClickUrl();
			
			if (clickUrl != newClickUrl)
			{
				clickUrl = newClickUrl;
				if (clickUrl == null)
					ClearClickUrl();
				else
					UpdateClickUrl(clickUrl: clickUrl);
			}
			
			if (displayLogger != null && shouldRenderDisplayLogger)
				displayLogger.Render(displayOutput: display, font: GameFont.DTSimpleFont14Pt, color: DTColor.Black());
			
			if (currentKeyboard.IsPressed(Key.L) && !previousKeyboard.IsPressed(Key.L))
				shouldRenderDisplayLogger = !shouldRenderDisplayLogger;
			
			previousKeyboard = currentKeyboard;
			previousMouse = currentMouse;
		}
	}
}
