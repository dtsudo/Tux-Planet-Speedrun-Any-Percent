
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class Tilemap : ITilemap
	{
		public class EnemySpawnLocation
		{
			public EnemySpawnLocation(
				int actorId,
				int tileI,
				int tileJ,
				string enemyId)
			{
				this.ActorId = actorId;
				this.TileI = tileI;
				this.TileJ = tileJ;
				this.EnemyId = enemyId;
			}

			public int ActorId { get; private set; }
			public int TileI { get; private set; }
			public int TileJ { get; private set; }
			public string EnemyId { get; private set; }
		}

		private Sprite[][] backgroundSpritesArray;
		private Sprite[][] foregroundSpritesArray;
		private bool[][] isGroundArray;
		private bool[][] isKillZoneArray;
		private bool[][] isSpikesArray;
		private bool[][] isEndOfLevelArray;
		private bool[][] isCutsceneArray;
		private Tuple<int, int>[][] checkpointArray;

		private int tileWidth;
		private int tileHeight;

		private int tilemapWidth;
		private int tilemapHeight;

		private List<EnemySpawnLocation> enemies;

		private string cutsceneName;

		private Tuple<int, int> tuxLocation;

		private GameMusic gameMusic;

		public Tilemap(
			Sprite[][] backgroundSpritesArray,
			Sprite[][] foregroundSpritesArray,
			bool[][] isGroundArray,
			bool[][] isKillZoneArray,
			bool[][] isSpikesArray,
			bool[][] isEndOfLevelArray,
			bool[][] isCutsceneArray,
			Tuple<int, int>[][] checkpointArray, 
			int tileWidth,
			int tileHeight,
			List<EnemySpawnLocation> enemies,
			string cutsceneName,
			Tuple<int, int> tuxLocation,
			GameMusic gameMusic)
		{
			this.backgroundSpritesArray = SpriteUtil.CopySpriteArray(array: backgroundSpritesArray);
			this.foregroundSpritesArray = SpriteUtil.CopySpriteArray(array: foregroundSpritesArray);
			this.isGroundArray = ArrayUtil.CopyBoolArray(array: isGroundArray);
			this.isKillZoneArray = ArrayUtil.CopyBoolArray(array: isKillZoneArray);
			this.isSpikesArray = ArrayUtil.CopyBoolArray(array: isSpikesArray);
			this.isEndOfLevelArray = ArrayUtil.CopyBoolArray(array: isEndOfLevelArray);
			this.isCutsceneArray = ArrayUtil.CopyBoolArray(array: isCutsceneArray);
			this.checkpointArray = ArrayUtil.ShallowCopyTArray(array: checkpointArray);
			this.tileWidth = tileWidth;
			this.tileHeight = tileHeight;
			this.tilemapWidth = tileWidth * foregroundSpritesArray.Length;
			this.tilemapHeight = tileHeight * foregroundSpritesArray[0].Length;
			this.enemies = new List<EnemySpawnLocation>(enemies);
			this.cutsceneName = cutsceneName;
			this.tuxLocation = tuxLocation;
			this.gameMusic = gameMusic;
		}

		private bool GetArrayValue(bool[][] array, int worldX, int worldY)
		{
			if (worldX < 0 || worldY < 0)
				return false;

			int arrayI = worldX / this.tileWidth;
			int arrayJ = worldY / this.tileHeight;

			if (arrayI < array.Length)
			{
				if (arrayJ < array[arrayI].Length)
					return array[arrayI][arrayJ];
			}

			return false;
		}

		public bool IsGround(int x, int y)
		{
			return this.GetArrayValue(array: this.isGroundArray, worldX: x, worldY: y);
		}

		public bool IsSpikes(int x, int y)
		{
			return this.GetArrayValue(array: this.isSpikesArray, worldX: x, worldY: y);
		}

		public bool IsKillZone(int x, int y)
		{
			return this.GetArrayValue(array: this.isKillZoneArray, worldX: x, worldY: y);
		}

		public bool IsEndOfLevel(int x, int y)
		{
			return this.GetArrayValue(array: this.isEndOfLevelArray, worldX: x, worldY: y);
		}

		public string GetCutscene(int x, int y)
		{
			bool isCutscene = this.GetArrayValue(array: this.isCutsceneArray, worldX: x, worldY: y);

			if (isCutscene)
				return this.cutsceneName;

			return null;
		}

		public Tuple<int, int> GetCheckpoint(int x, int y)
		{
			if (x < 0 || y < 0)
				return null;

			int arrayI = x / this.tileWidth;
			int arrayJ = y / this.tileHeight;

			if (arrayI < this.checkpointArray.Length)
			{
				if (arrayJ < this.checkpointArray[arrayI].Length)
					return this.checkpointArray[arrayI][arrayJ];
			}

			return null;
		}

		public int GetWidth()
		{
			return this.tilemapWidth;
		}

		public int GetHeight()
		{
			return this.tilemapHeight;
		}

		private void RenderSprites(Sprite[][] sprites, int cameraX, int cameraY, int windowWidth, int windowHeight, IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			int worldX = 0;

			int windowLeft = cameraX - windowWidth / 2;
			int windowRight = cameraX + windowWidth / 2;
			int windowBottom = cameraY - windowHeight / 2;
			int windowTop = cameraY + windowHeight / 2;

			for (int i = 0; i < sprites.Length; i++)
			{
				if (windowLeft <= worldX + this.tileWidth && worldX <= windowRight)
				{
					int worldY = 0;

					for (int j = 0; j < sprites[i].Length; j++)
					{
						if (windowBottom <= worldY + this.tileHeight && worldY <= windowTop)
						{
							Sprite sprite = sprites[i][j];

							if (sprite != null)
							{
								displayOutput.DrawImageRotatedClockwise(
									image: sprite.Image,
									imageX: sprite.X,
									imageY: sprite.Y,
									imageWidth: sprite.Width,
									imageHeight: sprite.Height,
									x: worldX,
									y: worldY,
									degreesScaled: 0,
									scalingFactorScaled: sprite.ScalingFactorScaled);
							}
						}

						worldY += this.tileHeight;
					}
				}

				worldX += this.tileWidth;
			}
		}

		public void RenderBackgroundTiles(IDisplayOutput<GameImage, GameFont> displayOutput, int cameraX, int cameraY, int windowWidth, int windowHeight)
		{
			this.RenderSprites(
				sprites: this.backgroundSpritesArray,
				cameraX: cameraX,
				cameraY: cameraY,
				windowWidth: windowWidth,
				windowHeight: windowHeight,
				displayOutput: displayOutput);
		}

		public void RenderForegroundTiles(IDisplayOutput<GameImage, GameFont> displayOutput, int cameraX, int cameraY, int windowWidth, int windowHeight)
		{
			this.RenderSprites(
				sprites: this.foregroundSpritesArray,
				cameraX: cameraX,
				cameraY: cameraY,
				windowWidth: windowWidth,
				windowHeight: windowHeight,
				displayOutput: displayOutput);
		}

		public IReadOnlyList<IEnemy> GetEnemies(int xOffset, int yOffset)
		{
			List<IEnemy> list = new List<IEnemy>();

			int halfTileWidth = this.tileWidth >> 1;
			int halfTileHeight = this.tileHeight >> 1;

			foreach (EnemySpawnLocation enemy in this.enemies)
			{
				if (enemy.ActorId == 13)
				{
					int xMibi = (enemy.TileI * this.tileWidth + halfTileWidth + xOffset) << 10;
					int yMibi = (enemy.TileJ * this.tileHeight + halfTileHeight + 1 * 3 + yOffset) << 10;

					EnemySmartcap enemySmartcap = EnemySmartcap.GetEnemySmartcap(
						xMibi: xMibi,
						yMibi: yMibi,
						isFacingRight: true,
						enemyId: enemy.EnemyId);

					list.Add(new EnemySpawnHelper(
						enemyToSpawn: enemySmartcap,
						xMibi: xMibi,
						yMibi: yMibi,
						enemyWidth: 16 * 3,
						enemyHeight: 18 * 3));
				}
				else if (enemy.ActorId == 23)
				{
					int xMibi = (enemy.TileI * this.tileWidth + halfTileWidth + xOffset) << 10;
					int yMibi = (enemy.TileJ * this.tileHeight + halfTileHeight + yOffset) << 10;

					EnemyKonqiCutscene konqi = EnemyKonqiCutscene.GetEnemyKonqiCutscene(
						xMibi: xMibi,
						yMibi: yMibi,
						enemyId: enemy.EnemyId);

					list.Add(new EnemySpawnHelper(
						enemyToSpawn: konqi,
						xMibi: xMibi,
						yMibi: yMibi,
						enemyWidth: 32 * 3,
						enemyHeight: 32 * 3));
				}
				else if (enemy.ActorId == 67)
				{
					int xMibi = (enemy.TileI * this.tileWidth + halfTileWidth + xOffset) << 10;
					int yMibi = (enemy.TileJ * this.tileHeight + halfTileHeight + yOffset) << 10;

					EnemyBlazeborn enemyBlazeborn = EnemyBlazeborn.GetEnemyBlazeborn(
						xMibi: xMibi,
						yMibi: yMibi,
						isFacingRight: true,
						enemyId: enemy.EnemyId);

					list.Add(new EnemySpawnHelper(
						enemyToSpawn: enemyBlazeborn,
						xMibi: xMibi,
						yMibi: yMibi,
						enemyWidth: 16 * 3,
						enemyHeight: 16 * 3));
				}
				else
					throw new Exception();
			}

			return list;
		}

		public GameMusic? PlayMusic()
		{
			return this.gameMusic;
		}

		public Tuple<int, int> GetTuxLocation(int xOffset, int yOffset)
		{
			if (this.tuxLocation != null)
				return new Tuple<int, int>(this.tuxLocation.Item1 + xOffset, this.tuxLocation.Item2 + yOffset);

			return null;
		}
	}
}
