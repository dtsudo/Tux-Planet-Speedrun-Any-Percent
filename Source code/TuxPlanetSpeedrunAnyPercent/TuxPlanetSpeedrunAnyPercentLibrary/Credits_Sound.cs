
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;

	public class Credits_Sound
	{
		private static string GetText()
		{
			return "Sound effects created by: \n"
				+ "* Basto \n"
				+ "* Kenney \n"
				+ "* Little Robot Sound Factory \n"
				+ "* SuperTux team (Some_Person, wansti) \n"
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
