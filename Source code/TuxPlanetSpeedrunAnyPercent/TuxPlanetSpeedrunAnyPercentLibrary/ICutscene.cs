
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public interface ICutscene
	{
		CutsceneProcessing.Result ProcessFrame(
			Move move,
			int tuxXMibi,
			int tuxYMibi,
			CameraState cameraState,
			int elapsedMicrosPerFrame,
			int windowWidth,
			int windowHeight,
			ITilemap tilemap,
			Difficulty difficulty,
			IReadOnlyList<IEnemy> enemies,
			IReadOnlyList<string> levelFlags);

		string GetCutsceneName();

		void Render(IDisplayOutput<GameImage, GameFont> displayOutput, int windowWidth, int windowHeight);
	}
}
