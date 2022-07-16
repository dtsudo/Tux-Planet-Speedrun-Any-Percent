
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class BoundedTilemap : ITilemap
	{
		private ITilemap tilemap;
		private int width;
		private int height;

		public BoundedTilemap(ITilemap tilemap)
		{
			this.tilemap = tilemap;
			this.width = tilemap.GetWidth();
			this.height = tilemap.GetHeight();
		}

		public bool IsGround(int x, int y)
		{
			if (x < 0)
				return true;
			if (x >= this.width)
				return true;

			return this.tilemap.IsGround(x: x, y: y);
		}

		public bool IsSpikes(int x, int y)
		{
			return this.tilemap.IsSpikes(x: x, y: y);
		}

		public bool IsKillZone(int x, int y)
		{
			if (y < 0)
				return true;

			return this.tilemap.IsKillZone(x: x, y: y);
		}

		public bool IsEndOfLevel(int x, int y)
		{
			return this.tilemap.IsEndOfLevel(x: x, y: y);
		}

		public string GetCutscene(int x, int y)
		{
			return this.tilemap.GetCutscene(x: x, y: y);
		}

		public int GetWidth()
		{
			return this.width;
		}

		public int GetHeight()
		{
			return this.height;
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
			this.tilemap.RenderBackgroundTiles(displayOutput: displayOutput, cameraX: cameraX, cameraY: cameraY, windowWidth: windowWidth, windowHeight: windowHeight);
		}

		public void RenderForegroundTiles(IDisplayOutput<GameImage, GameFont> displayOutput, int cameraX, int cameraY, int windowWidth, int windowHeight)
		{
			this.tilemap.RenderForegroundTiles(displayOutput: displayOutput, cameraX: cameraX, cameraY: cameraY, windowWidth: windowWidth, windowHeight: windowHeight);
		}
	}
}
