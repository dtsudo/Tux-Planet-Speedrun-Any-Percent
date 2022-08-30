
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;

	public class Credits_Music
	{
		private static string GetText()
		{
			return "Music track authors: \n"
				+ "* cynicmusic \n"
				+ "* Jason Lavallee \n"
				+ "* Lukas Nystrand \n"
				+ "* migfus20 \n"
				+ "* Cal McEachern \n"
				+ "* wansti \n"
				+ "\n"
				+ "See the source code for more information (including licensing \n"
				+ "details).";
		}

		public static void Render(IDisplayOutput<GameImage, GameFont> displayOutput, int width, int height)
		{
			displayOutput.DrawText(
				x: 10,
				y: height - 10,
				text: GetText(),
				font: GameFont.DTSimpleFont20Pt,
				color: DTColor.Black());
		}
	}
}
