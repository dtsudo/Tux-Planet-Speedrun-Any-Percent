
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class NoCutsceneWrappedTilemap : ITilemap
	{
		private ITilemap tilemap;

		public NoCutsceneWrappedTilemap(ITilemap tilemap)
		{
			this.tilemap = tilemap;
		}

		public bool IsGround(int x, int y)
		{
			return this.tilemap.IsGround(x: x, y: y);
		}

		public bool IsKillZone(int x, int y)
		{
			return this.tilemap.IsKillZone(x: x, y: y);
		}

		public bool IsSpikes(int x, int y)
		{
			return this.tilemap.IsSpikes(x: x, y: y);
		}

		public bool IsEndOfLevel(int x, int y)
		{
			return this.tilemap.IsEndOfLevel(x: x, y: y);
		}

		public string GetCutscene(int x, int y)
		{
			return null;
		}

		public Tuple<int, int> GetCheckpoint(int x, int y)
		{
			return this.tilemap.GetCheckpoint(x: x, y: y);
		}

		public int GetWidth()
		{
			return this.tilemap.GetWidth();
		}

		public int GetHeight()
		{
			return this.tilemap.GetHeight();
		}

		public Tuple<int, int> GetTuxLocation(int xOffset, int yOffset)
		{
			return this.tilemap.GetTuxLocation(xOffset: xOffset, yOffset: yOffset);
		}

		public IReadOnlyList<IEnemy> GetEnemies(int xOffset, int yOffset)
		{
			return this.tilemap.GetEnemies(xOffset: xOffset, yOffset: yOffset);
		}

		public GameMusic? PlayMusic()
		{
			return this.tilemap.PlayMusic();
		}

		public void RenderBackgroundTiles(IDisplayOutput<GameImage, GameFont> displayOutput, int cameraX, int cameraY, int windowWidth, int windowHeight)
		{
			this.tilemap.RenderBackgroundTiles(
				displayOutput: displayOutput,
				cameraX: cameraX,
				cameraY: cameraY,
				windowWidth: windowWidth,
				windowHeight: windowHeight);
		}

		public void RenderForegroundTiles(IDisplayOutput<GameImage, GameFont> displayOutput, int cameraX, int cameraY, int windowWidth, int windowHeight)
		{
			this.tilemap.RenderForegroundTiles(
				displayOutput: displayOutput,
				cameraX: cameraX,
				cameraY: cameraY,
				windowWidth: windowWidth,
				windowHeight: windowHeight);
		}
	}
}
