
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class Level6Tilemap_BossBattle : ITilemap
	{
		private ITilemap mapTilemap;
		private int bossRoomXOffsetStart;
		private int bossRoomXOffsetEnd;

		public Level6Tilemap_BossBattle(
			ITilemap mapTilemap,
			int bossRoomXOffsetStart,
			int bossRoomXOffsetEnd)
		{
			this.mapTilemap = mapTilemap;
			this.bossRoomXOffsetStart = bossRoomXOffsetStart;
			this.bossRoomXOffsetEnd = bossRoomXOffsetEnd;
		}

		public bool IsGround(int x, int y)
		{
			if (x < this.bossRoomXOffsetStart)
				return true;

			if (x >= this.bossRoomXOffsetEnd)
				return true;

			return this.mapTilemap.IsGround(x: x, y: y);
		}

		public bool IsKillZone(int x, int y)
		{
			return this.mapTilemap.IsKillZone(x: x, y: y);
		}

		public bool IsSpikes(int x, int y)
		{
			return this.mapTilemap.IsSpikes(x: x, y: y);
		}

		public bool IsEndOfLevel(int x, int y)
		{
			return this.mapTilemap.IsEndOfLevel(x: x, y: y);
		}

		public string GetCutscene(int x, int y)
		{
			return this.mapTilemap.GetCutscene(x: x, y: y);
		}

		public Tuple<int, int> GetCheckpoint(int x, int y)
		{
			return this.mapTilemap.GetCheckpoint(x: x, y: y);
		}

		public int GetWidth()
		{
			return this.mapTilemap.GetWidth();
		}

		public int GetHeight()
		{
			return this.mapTilemap.GetHeight();
		}

		public Tuple<int, int> GetTuxLocation(int xOffset, int yOffset)
		{
			return this.mapTilemap.GetTuxLocation(xOffset: xOffset, yOffset: yOffset);
		}

		public Tuple<int, int> GetMapKeyLocation(MapKey mapKey, int xOffset, int yOffset)
		{
			return this.mapTilemap.GetMapKeyLocation(mapKey: mapKey, xOffset: xOffset, yOffset: yOffset);
		}

		public IReadOnlyList<IEnemy> GetEnemies(int xOffset, int yOffset)
		{
			return this.mapTilemap.GetEnemies(xOffset: xOffset, yOffset: yOffset);
		}

		public GameMusic? PlayMusic()
		{
			return GameMusic.BossTheme;
		}

		public void RenderBackgroundTiles(IDisplayOutput<GameImage, GameFont> displayOutput, int cameraX, int cameraY, int windowWidth, int windowHeight)
		{
			this.mapTilemap.RenderBackgroundTiles(
				displayOutput: displayOutput,
				cameraX: cameraX,
				cameraY: cameraY,
				windowWidth: windowWidth,
				windowHeight: windowHeight);
		}

		public void RenderForegroundTiles(IDisplayOutput<GameImage, GameFont> displayOutput, int cameraX, int cameraY, int windowWidth, int windowHeight)
		{
			this.mapTilemap.RenderForegroundTiles(
				displayOutput: displayOutput,
				cameraX: cameraX,
				cameraY: cameraY,
				windowWidth: windowWidth,
				windowHeight: windowHeight);
		}
	}
}
