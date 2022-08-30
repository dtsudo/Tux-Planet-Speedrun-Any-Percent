
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;

	public class Credits_Images
	{
		private static string GetText()
		{
			return "Image files created by: \n"
				+ "* Benjamin K. Smith, Lanea Zimmerman (AKA Sharm), Daniel Eddeland, \n"
				+ "   William.Thompsonj, Nushio, Adrix89 \n"
				+ "* FrostC \n"
				+ "* Grumbel \n"
				+ "* Jetrel \n"
				+ "* Kelvin Shadewing \n"
				+ "* Kenney \n"
				+ "* KnoblePersona \n"
				+ "* Nemisys \n"
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
