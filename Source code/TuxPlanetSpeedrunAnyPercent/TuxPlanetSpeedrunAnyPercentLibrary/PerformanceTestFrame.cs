
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class PerformanceTestFrame : IFrame<GameImage, GameFont, GameSound, GameMusic>
	{
		private GlobalState globalState;

		private GameLogicState gameLogic;

		private long timeSpentProcessing;
		private long timeSpentRendering;

		private int numFramesProcessed;
		private int numFramesRendered;

		private PerformanceTestFrame(
			GlobalState globalState,
			GameLogicState gameLogic)
		{
			this.globalState = globalState;
			this.gameLogic = gameLogic;

			this.timeSpentProcessing = 0;
			this.timeSpentRendering = 0;
			this.numFramesProcessed = 0;
			this.numFramesRendered = 0;
		}

		public static IFrame<GameImage, GameFont, GameSound, GameMusic> GetPerformanceTestFrame(
			GlobalState globalState,
			IDisplayProcessing<GameImage> displayProcessing,
			ISoundOutput<GameSound> soundOutput,
			IMusicProcessing musicProcessing)
		{
			DTDeterministicRandom random = new DTDeterministicRandom(seed: 123);

			GameLogicState gameLogic = new GameLogicState(
				level: Level.Level10,
				windowWidth: globalState.WindowWidth,
				windowHeight: globalState.WindowHeight,
				canUseSaveStates: true,
				canUseTimeSlowdown: true,
				canUseTeleport: true,
				mapInfo: globalState.MapInfo,
				random: random);

			PerformanceTestFrame frame = new PerformanceTestFrame(globalState: globalState, gameLogic: gameLogic);
			frame.GetNextFrame(
				keyboardInput: new EmptyKeyboard(),
				mouseInput: new EmptyMouse(),
				previousKeyboardInput: new EmptyKeyboard(),
				previousMouseInput: new EmptyMouse(),
				displayProcessing: displayProcessing,
				soundOutput: soundOutput,
				musicProcessing: musicProcessing);

			return frame;
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
			if (this.gameLogic.FrameCounter >= 700)
			{
				if (keyboardInput.IsPressed(Key.Enter) && !previousKeyboardInput.IsPressed(Key.Enter)
					|| keyboardInput.IsPressed(Key.Space) && !previousKeyboardInput.IsPressed(Key.Space)
					|| keyboardInput.IsPressed(Key.Esc) && !previousKeyboardInput.IsPressed(Key.Esc)
					|| keyboardInput.IsPressed(Key.Z) && !previousKeyboardInput.IsPressed(Key.Z))
				{
					return GetPerformanceTestFrame(
						globalState: this.globalState,
						displayProcessing: displayProcessing,
						soundOutput: soundOutput,
						musicProcessing: musicProcessing);
				}

				return this;
			}

			keyboardInput = new PerformanceTestFrameKeyboard(frameCounter: this.gameLogic.FrameCounter);

			DateTime startTime = DateTime.Now;

			Move move = new Move(
				jumped: keyboardInput.IsPressed(Key.Z),
				teleported: keyboardInput.IsPressed(Key.X),
				arrowLeft: keyboardInput.IsPressed(Key.LeftArrow),
				arrowRight: keyboardInput.IsPressed(Key.RightArrow),
				arrowUp: keyboardInput.IsPressed(Key.UpArrow),
				arrowDown: keyboardInput.IsPressed(Key.DownArrow),
				respawn: keyboardInput.IsPressed(Key.Esc));

			GameLogicStateProcessing.Result result = GameLogicStateProcessing.ProcessFrame(
				gameLogicState: this.gameLogic,
				move: move,
				debugMode: false,
				debug_tuxInvulnerable: false,
				debugKeyboardInput: new EmptyKeyboard(),
				debugPreviousKeyboardInput: new EmptyKeyboard(),
				displayProcessing: displayProcessing,
				soundOutput: soundOutput,
				elapsedMicrosPerFrame: this.globalState.ElapsedMicrosPerFrame);

			this.gameLogic = result.NewGameLogicState;

			DateTime endTime = DateTime.Now;

			long elapsedTicks = endTime.Ticks - startTime.Ticks;
			this.timeSpentProcessing = this.timeSpentProcessing + elapsedTicks;
			this.numFramesProcessed++;

			return this;
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

		public void ProcessMusic()
		{
			this.globalState.ProcessMusic();
		}

		private static string DisplayTicksInMilliseconds(int ticks)
		{
			int numMillis = ticks / 10000;

			string str = numMillis.ToStringCultureInvariant();

			str += ".";

			string remainingTicks = (ticks % 10000).ToStringCultureInvariant();
			while (remainingTicks.Length < 4)
				remainingTicks = "0" + remainingTicks;

			str += remainingTicks + " ms";

			return str;
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			DateTime startTime = DateTime.Now;

			GameLogicStateProcessing.Render(
				gameLogicState: this.gameLogic, 
				displayOutput: displayOutput, 
				elapsedMillis: 1, 
				debug_showHitboxes: false);

			DateTime endTime = DateTime.Now;

			if (this.gameLogic.FrameCounter >= 700 && this.numFramesRendered > 0)
			{
				displayOutput.DrawRectangle(
					x: 50,
					y: 500,
					width: 450,
					height: 150,
					color: DTColor.White(),
					fill: true);

				string text = "Number of frames processed: " + this.numFramesProcessed.ToStringCultureInvariant() + "\n";

				text += "Number of frames rendered: " + this.numFramesRendered.ToStringCultureInvariant() + "\n";

				long timeSpentProcessingPerFrame = this.timeSpentProcessing / ((long)this.numFramesProcessed);

				text += "Time spent processing (per frame): " + DisplayTicksInMilliseconds((int)timeSpentProcessingPerFrame) + "\n";

				long timeSpentRenderingPerFrame = this.timeSpentRendering / ((long)this.numFramesRendered);

				text += "Time spent rendering (per frame): " + DisplayTicksInMilliseconds((int)timeSpentRenderingPerFrame) + "\n";

				displayOutput.DrawText(
					x: 55,
					y: 645,
					text: text,
					font: GameFont.DTSimpleFont14Pt,
					color: DTColor.Black());
			}
			else
			{
				long elapsedTicks = endTime.Ticks - startTime.Ticks;
				this.timeSpentRendering = this.timeSpentRendering + elapsedTicks;
				this.numFramesRendered++;
			}
		}

		public void RenderMusic(IMusicOutput<GameMusic> musicOutput)
		{
			this.globalState.RenderMusic(musicOutput: musicOutput);
		}
	}
}
