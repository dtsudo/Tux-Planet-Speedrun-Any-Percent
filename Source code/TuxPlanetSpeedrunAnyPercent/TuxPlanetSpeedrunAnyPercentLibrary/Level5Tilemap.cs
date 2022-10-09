
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class Level5Tilemap : ITilemap
	{
		private ITilemap mapTilemap;
		private int startingXMibiOfFirstSpike;
		private int startingXMibi;
		private int endingXMibi;

		public Level5Tilemap(
			ITilemap mapTilemap,
			int startingXMibiOfFirstSpike,
			int startingXMibi,
			int endingXMibi)
		{
			this.mapTilemap = mapTilemap;
			this.startingXMibiOfFirstSpike = startingXMibiOfFirstSpike;
			this.startingXMibi = startingXMibi;
			this.endingXMibi = endingXMibi;
		}

		public bool IsGround(int x, int y)
		{
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
			List<IEnemy> enemies = new List<IEnemy>()
			{
				EnemyLevel5SpikesInitialSpawn.GetEnemyLevel5SpikesInitialSpawn(
					xMibi: this.startingXMibiOfFirstSpike + (xOffset << 10),
					startingXMibi: this.startingXMibi + (xOffset << 10),
					endingXMibi:  this.endingXMibi + (xOffset << 10),
					yMibiBottom: yOffset << 10,
					heightInTiles: 50,
					enemyIdPrefix: "level5Spikes_Prefix",
					enemyId: "level5Spikes_GetEnemyLevel5SpikesInitialSpawn")
			};

			IReadOnlyList<IEnemy> mapTilemapEnemies = this.mapTilemap.GetEnemies(xOffset: xOffset, yOffset: yOffset);

			enemies.AddRange(mapTilemapEnemies);

			return enemies;
		}

		public GameMusic? PlayMusic()
		{
			return this.mapTilemap.PlayMusic();
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
