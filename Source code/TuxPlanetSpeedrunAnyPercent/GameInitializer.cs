
namespace TuxPlanetSpeedrunAnyPercent
{
	using Bridge;
	using DTLibrary;
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
		
		private static IFrame<GameImage, GameFont, GameSound, GameMusic> frame;
		
		private static bool hasInitializedClearCanvasJavascript;
		
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
							context = canvas.getContext('2d');
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
		
		public static void Start(
			int fps, 
			bool debugMode)
		{
			hasInitializedClearCanvasJavascript = false;
			
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
					debugMode: debugMode,
					initialMusicVolume: null);
			
			frame = TuxPlanetSpeedrunAnyPercent.GetFirstFrame(globalState: globalState);

			bridgeKeyboard = new BridgeKeyboard();
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
			if (displayLogger != null && shouldRenderDisplayLogger)
				displayLogger.Render(displayOutput: display, font: GameFont.DTSimpleFont14Pt, color: DTColor.Black());
			
			if (currentKeyboard.IsPressed(Key.L) && !previousKeyboard.IsPressed(Key.L))
				shouldRenderDisplayLogger = !shouldRenderDisplayLogger;
			
			previousKeyboard = new CopiedKeyboard(currentKeyboard);
			previousMouse = new CopiedMouse(currentMouse);
		}
	}
}
