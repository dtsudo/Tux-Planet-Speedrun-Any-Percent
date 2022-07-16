
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public interface ITilemap
	{
		bool IsGround(int x, int y);

		bool IsKillZone(int x, int y);

		bool IsSpikes(int x, int y);

		bool IsEndOfLevel(int x, int y);

		string GetCutscene(int x, int y);

		int GetWidth();

		int GetHeight();

		Tuple<int, int> GetTuxLocation(int xOffset, int yOffset);

		IReadOnlyList<IEnemy> GetEnemies(int xOffset, int yOffset);

		GameMusic? PlayMusic();

		void RenderBackgroundTiles(IDisplayOutput<GameImage, GameFont> displayOutput, int cameraX, int cameraY, int windowWidth, int windowHeight);

		void RenderForegroundTiles(IDisplayOutput<GameImage, GameFont> displayOutput, int cameraX, int cameraY, int windowWidth, int windowHeight);
	}
}
