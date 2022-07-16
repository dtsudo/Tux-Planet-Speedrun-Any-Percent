
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;

	public class Dialogue
	{
		private int x;
		private int y;
		private int width;
		private int height;
		private string text;
		private int elapsedMicros;
		private bool isFirst;
		private Move previousMove;

		private const int MICROS_PER_CHARACTER = 1000 * 30;

		private Dialogue(
			int x,
			int y,
			int width,
			int height,
			string text,
			int elapsedMicros,
			bool isFirst,
			Move previousMove)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
			this.text = text;
			this.elapsedMicros = elapsedMicros;
			this.isFirst = isFirst;
			this.previousMove = previousMove;
		}

		public static Dialogue GetDialogue(int x, int y, int width, int height, string text)
		{
			return new Dialogue(
				x: x,
				y: y,
				width: width,
				height: height,
				text: text,
				elapsedMicros: 0,
				isFirst: true,
				previousMove: Move.EmptyMove());
		}

		public class Result
		{
			public Result(
				Dialogue dialogue,
				bool isDone)
			{
				this.Dialogue = dialogue;
				this.IsDone = isDone;
			}

			public Dialogue Dialogue { get; private set; }
			public bool IsDone { get; private set; }
		}

		private int GetMaxElapsedMicros()
		{
			return this.text.Length * MICROS_PER_CHARACTER;
		}

		public Result ProcessFrame(
			Move move,
			int elapsedMicrosPerFrame)
		{
			int newElapsedMicros = this.elapsedMicros + elapsedMicrosPerFrame;

			bool hasFinishedRenderingText;

			if (newElapsedMicros >= this.GetMaxElapsedMicros())
			{
				newElapsedMicros = this.GetMaxElapsedMicros() + 1;
				hasFinishedRenderingText = true;
			}
			else
				hasFinishedRenderingText = false;

			bool done = false;

			if (move.Jumped && !this.previousMove.Jumped && !this.isFirst)
			{
				if (hasFinishedRenderingText)
					done = true;
				else
					newElapsedMicros = this.GetMaxElapsedMicros() + 1;
			}

			return new Result(
				dialogue: new Dialogue(
					x: this.x, 
					y: this.y, 
					width: this.width, 
					height: this.height, 
					text: this.text, 
					elapsedMicros: newElapsedMicros, 
					isFirst: false, 
					previousMove: move), 
				isDone: done);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput, int windowWidth, int windowHeight)
		{
			int numCharactersToRender = this.elapsedMicros / MICROS_PER_CHARACTER;

			string textToRender;

			if (numCharactersToRender >= this.text.Length)
				textToRender = this.text;
			else
				textToRender = this.text.Substring(0, numCharactersToRender);

			displayOutput.DrawRectangle(
				x: this.x,
				y: this.y,
				width: this.width,
				height: this.height,
				color: new DTColor(0, 0, 0, 150),
				fill: true);

			displayOutput.DrawText(
				x: this.x + 5,
				y: this.y + this.height - 5,
				text: textToRender,
				font: GameFont.DTSimpleFont20Pt,
				color: DTColor.White());
		}	
	}
}
