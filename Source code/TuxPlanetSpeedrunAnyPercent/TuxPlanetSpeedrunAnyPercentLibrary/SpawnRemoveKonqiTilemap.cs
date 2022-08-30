
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class SpawnRemoveKonqiTilemap : ITilemap
	{
		private IEnemy removeKonqiEnemy;
		private ITilemap tilemap;

		public SpawnRemoveKonqiTilemap(ITilemap tilemap)
		{
			this.removeKonqiEnemy = new EnemyRemoveKonqiCutscene(enemyId: "SpawnRemoveKonqiTilemap_removeKonqi");
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
			return this.tilemap.GetCutscene(x: x, y: y);
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

		public Tuple<int, int> GetMapKeyLocation(MapKey mapKey, int xOffset, int yOffset)
		{
			return this.tilemap.GetMapKeyLocation(mapKey: mapKey, xOffset: xOffset, yOffset: yOffset);
		}

		public IReadOnlyList<IEnemy> GetEnemies(int xOffset, int yOffset)
		{
			IReadOnlyList<IEnemy> enemies = this.tilemap.GetEnemies(xOffset: xOffset, yOffset: yOffset);

			List<IEnemy> returnValue = new List<IEnemy>(enemies);
			returnValue.Add(this.removeKonqiEnemy);

			return returnValue;
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
