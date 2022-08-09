
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class CompositeTilemap : ITilemap
	{
		public class TilemapWithOffset
		{
			public TilemapWithOffset(
				ITilemap tilemap,
				int xOffset,
				int yOffset,
				bool alwaysIncludeTilemap)
			{
				this.Tilemap = tilemap;
				this.XOffset = xOffset;
				this.YOffset = yOffset;
				this.AlwaysIncludeTilemap = alwaysIncludeTilemap;
			}

			public ITilemap Tilemap { get; private set; }
			public int XOffset { get; private set; }
			public int YOffset { get; private set; }
			public bool AlwaysIncludeTilemap { get; private set; }
		}

		private List<TilemapWithOffset> tilemaps;

		private int width;
		private int height;

		public static List<TilemapWithOffset> NormalizeTilemaps(List<TilemapWithOffset> tilemaps)
		{
			int? minX = null;
			int? minY = null;

			foreach (TilemapWithOffset tilemap in tilemaps)
			{
				int tilemapMinX = tilemap.XOffset;

				int tilemapMinY = tilemap.YOffset;

				if (minX == null || minX.Value > tilemapMinX)
					minX = tilemapMinX;

				if (minY == null || minY.Value > tilemapMinY)
					minY = tilemapMinY;
			}

			return tilemaps.Select(t => new TilemapWithOffset(
				tilemap: t.Tilemap,
				xOffset: t.XOffset - minX.Value,
				yOffset: t.YOffset - minY.Value,
				alwaysIncludeTilemap: t.AlwaysIncludeTilemap)).ToList();
		}

		public CompositeTilemap(IReadOnlyList<TilemapWithOffset> normalizedTilemaps, int width, int height)
		{
			this.tilemaps = new List<TilemapWithOffset>(normalizedTilemaps);

			this.width = width;
			this.height = height;
		}

		public GameMusic? PlayMusic()
		{
			foreach (TilemapWithOffset tilemap in this.tilemaps)
			{
				GameMusic? music = tilemap.Tilemap.PlayMusic();

				if (music != null)
					return music.Value;
			}

			return null;
		}

		public bool IsGround(int x, int y)
		{
			for (int i = 0; i < this.tilemaps.Count; i++)
			{
				TilemapWithOffset tilemap = this.tilemaps[i];
				if (tilemap.Tilemap.IsGround(x - tilemap.XOffset, y - tilemap.YOffset))
					return true;
			}

			return false;
		}

		public bool IsSpikes(int x, int y)
		{
			for (int i = 0; i < this.tilemaps.Count; i++)
			{
				TilemapWithOffset tilemap = this.tilemaps[i];
				if (tilemap.Tilemap.IsSpikes(x - tilemap.XOffset, y - tilemap.YOffset))
					return true;
			}

			return false;
		}

		public bool IsKillZone(int x, int y)
		{
			for (int i = 0; i < this.tilemaps.Count; i++)
			{
				TilemapWithOffset tilemap = this.tilemaps[i];
				if (tilemap.Tilemap.IsKillZone(x - tilemap.XOffset, y - tilemap.YOffset))
					return true;
			}

			return false;
		}

		public bool IsEndOfLevel(int x, int y)
		{
			for (int i = 0; i < this.tilemaps.Count; i++)
			{
				TilemapWithOffset tilemap = this.tilemaps[i];
				if (tilemap.Tilemap.IsEndOfLevel(x - tilemap.XOffset, y - tilemap.YOffset))
					return true;
			}

			return false;
		}

		public string GetCutscene(int x, int y)
		{
			for (int i = 0; i < this.tilemaps.Count; i++)
			{
				TilemapWithOffset tilemap = this.tilemaps[i];
				string cutscene = tilemap.Tilemap.GetCutscene(x - tilemap.XOffset, y - tilemap.YOffset);

				if (cutscene != null)
					return cutscene;
			}

			return null;
		}

		public Tuple<int, int> GetCheckpoint(int x, int y)
		{
			for (int i = 0; i < this.tilemaps.Count; i++)
			{
				TilemapWithOffset tilemap = this.tilemaps[i];
				Tuple<int, int> checkpoint = tilemap.Tilemap.GetCheckpoint(x - tilemap.XOffset, y - tilemap.YOffset);

				if (checkpoint != null)
					return new Tuple<int, int>(checkpoint.Item1 + tilemap.XOffset, checkpoint.Item2 + tilemap.YOffset);
			}

			return null;
		}

		public void RenderBackgroundTiles(IDisplayOutput<GameImage, GameFont> displayOutput, int cameraX, int cameraY, int windowWidth, int windowHeight)
		{
			foreach (TilemapWithOffset tilemap in this.tilemaps)
			{
				IDisplayOutput<GameImage, GameFont> translatedDisplayOutput = new TranslatedDisplayOutput<GameImage, GameFont>(
					display: displayOutput,
					xOffsetInPixels: tilemap.XOffset,
					yOffsetInPixels: tilemap.YOffset);

				tilemap.Tilemap.RenderBackgroundTiles(
					displayOutput: translatedDisplayOutput,
					cameraX: cameraX - tilemap.XOffset,
					cameraY: cameraY - tilemap.YOffset,
					windowWidth: windowWidth,
					windowHeight: windowHeight);
			}
		}

		public void RenderForegroundTiles(IDisplayOutput<GameImage, GameFont> displayOutput, int cameraX, int cameraY, int windowWidth, int windowHeight)
		{
			foreach (TilemapWithOffset tilemap in this.tilemaps)
			{
				IDisplayOutput<GameImage, GameFont> translatedDisplayOutput = new TranslatedDisplayOutput<GameImage, GameFont>(
					display: displayOutput,
					xOffsetInPixels: tilemap.XOffset,
					yOffsetInPixels: tilemap.YOffset);

				tilemap.Tilemap.RenderForegroundTiles(
					displayOutput: translatedDisplayOutput,
					cameraX: cameraX - tilemap.XOffset,
					cameraY: cameraY - tilemap.YOffset,
					windowWidth: windowWidth,
					windowHeight: windowHeight);
			}
		}

		public int GetWidth()
		{
			return this.width;
		}

		public int GetHeight()
		{
			return this.height;
		}

		public IReadOnlyList<IEnemy> GetEnemies(int xOffset, int yOffset)
		{
			List<IEnemy> enemies = new List<IEnemy>();

			foreach (TilemapWithOffset tilemap in this.tilemaps)
			{
				IReadOnlyList<IEnemy> tilemapEnemies = tilemap.Tilemap.GetEnemies(xOffset: tilemap.XOffset + xOffset, yOffset: tilemap.YOffset + yOffset);
				enemies.AddRange(tilemapEnemies);
			}

			return enemies;
		}

		public Tuple<int, int> GetTuxLocation(int xOffset, int yOffset)
		{
			for (int i = 0; i < this.tilemaps.Count; i++)
			{
				TilemapWithOffset tilemap = this.tilemaps[i];
				Tuple<int, int> tuxLocation = tilemap.Tilemap.GetTuxLocation(xOffset: xOffset + tilemap.XOffset, yOffset: yOffset + tilemap.YOffset);

				if (tuxLocation != null)
					return tuxLocation;
			}

			return null;
		}
	}
}
