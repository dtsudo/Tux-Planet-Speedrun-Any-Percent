
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;

	public class Credits_DesignAndCoding
	{
		private static string GetWebBrowserVersionText()
		{
			return "Design and coding by dtsudo: \n"
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

		public static bool IsHoverOverGitHubUrl(IMouse mouse, bool isWebBrowserVersion, int width, int height)
		{
			if (!isWebBrowserVersion)
				return false;

			int mouseX = mouse.GetX();
			int mouseY = mouse.GetY();

			return 394 <= mouseX && mouseX <= 394 + 351
				&& height - 38 <= mouseY && mouseY <= height - 13;
		}

		public static void Render(
			IDisplayOutput<GameImage, GameFont> displayOutput, 
			bool isHoverOverGitHubUrl,
			bool isWebBrowserVersion, 
			int width, 
			int height)
		{
			if (isWebBrowserVersion)
			{
				string text = GetWebBrowserVersionText();

				displayOutput.DrawText(
					x: 10,
					y: height - 10,
					text: text,
					font: GameFont.DTSimpleFont20Pt,
					color: DTColor.Black());

				displayOutput.DrawText(
					x: 395,
					y: height - 10,
					text: "https://github.com/dtsudo",
					font: GameFont.DTSimpleFont20Pt,
					color: isHoverOverGitHubUrl ? new DTColor(0, 0, 255) : DTColor.Black());
			}
			else
			{
				string text = GetDesktopVersionText();

				displayOutput.DrawText(
					x: 10,
					y: height - 10,
					text: text,
					font: GameFont.DTSimpleFont20Pt,
					color: DTColor.Black());
			}
		}
	}
}
