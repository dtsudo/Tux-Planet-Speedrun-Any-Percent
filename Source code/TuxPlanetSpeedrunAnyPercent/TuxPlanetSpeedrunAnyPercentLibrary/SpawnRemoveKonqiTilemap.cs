
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class SpawnRemoveKonqiTilemap : ITilemap
	{
		private List<IEnemy> removeKonqiEnemy;

		public SpawnRemoveKonqiTilemap()
		{
			this.removeKonqiEnemy = new List<IEnemy>()
			{
				new EnemyRemoveKonqiCutscene(enemyId: "SpawnRemoveKonqiTilemap_removeKonqi")
			};
		}

		public bool IsGround(int x, int y)
		{
			return false;
		}

		public bool IsKillZone(int x, int y)
		{
			return false;
		}

		public bool IsSpikes(int x, int y)
		{
			return false;
		}

		public bool IsEndOfLevel(int x, int y)
		{
			return false;
		}

		public string GetCutscene(int x, int y)
		{
			return null;
		}

		public Tuple<int, int> GetCheckpoint(int x, int y)
		{
			return null;
		}

		public int GetWidth()
		{
			return 100; // arbitrary
		}

		public int GetHeight()
		{
			return 100; // arbitrary
		}

		public Tuple<int, int> GetTuxLocation(int xOffset, int yOffset)
		{
			return null;
		}

		public IReadOnlyList<IEnemy> GetEnemies(int xOffset, int yOffset)
		{
			// EnemyRemoveKonqiCutscene doesn't have an (x,y) coordinate,
			// so xOffset and yOffset don't need to be handled.
			return this.removeKonqiEnemy;
		}

		public GameMusic? PlayMusic()
		{
			return null;
		}

		public void RenderBackgroundTiles(IDisplayOutput<GameImage, GameFont> displayOutput, int cameraX, int cameraY, int windowWidth, int windowHeight)
		{
		}

		public void RenderForegroundTiles(IDisplayOutput<GameImage, GameFont> displayOutput, int cameraX, int cameraY, int windowWidth, int windowHeight)
		{
		}
	}
}
