
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;

	public class Credits_DesignAndCoding
	{
		private static string GetWebBrowserVersionText()
		{
			return "Design and coding by dtsudo (https://github.com/dtsudo) \n"
				+ "\n"
				+ "This game is a fangame of SuperTux and SuperTux Advance. \n"
				+ "\n"
				+ "This game is open source, licensed under the AGPL 3.0. \n"
				+ "(Code dependencies and images/font/sound/music licensed under \n"
				+ "AGPL-compatible licenses.) \n"
				+ "\n"
				+ "The source code is written in C# and transpiled to javascript using \n"
				+ "Bridge.NET. \n"
				+ "\n"
				+ "See the source code for more information (including licensing \n"
				+ "details).";
		}

		private static string GetDesktopVersionText()
		{
			return "";
		}

		public static void Render(IDisplayOutput<GameImage, GameFont> displayOutput, bool isWebBrowserVersion, int width, int height)
		{
			string text = isWebBrowserVersion ? GetWebBrowserVersionText() : GetDesktopVersionText();

			displayOutput.DrawText(
				x: 10,
				y: height - 10,
				text: text,
				font: GameFont.DTSimpleFont20Pt,
				color: DTColor.Black());
		}
	}
}
