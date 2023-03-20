
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyOrange : IEnemy
	{
		public class EnemyOrangeSpawn : Tilemap.IExtraEnemyToSpawn
		{
			private int xMibi;
			private int yMibi;
			private bool isFacingRight;
			private string enemyId;

			public EnemyOrangeSpawn(
				int xMibi,
				int yMibi,
				bool isFacingRight,
				string enemyId)
			{
				this.xMibi = xMibi;
				this.yMibi = yMibi;
				this.isFacingRight = isFacingRight;
				this.enemyId = enemyId;
			}

			public IEnemy GetEnemy(int xOffset, int yOffset)
			{
				return new EnemySpawnHelper(
					enemyToSpawn: GetEnemyOrange(
						xMibi: this.xMibi + (xOffset << 10),
						yMibi: this.yMibi + (yOffset << 10),
						isFacingRight: this.isFacingRight,
						enemyId: this.enemyId),
					xMibi: this.xMibi + (xOffset << 10),
					yMibi: this.yMibi + (yOffset << 10),
					enemyWidth: 48,
					enemyHeight: 48);
			}
		}

		private int xMibi;
		private int yMibi;

		private int xSpeedInMibipixelsPerSecond;
		private int ySpeedInMibipixelsPerSecond;

		private int elapsedMicros;

		public string EnemyId { get; private set; }

		private EnemyOrange(
			int xMibi,
			int yMibi,
			int xSpeedInMibipixelsPerSecond,
			int ySpeedInMibipixelsPerSecond,
			int elapsedMicros,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.xSpeedInMibipixelsPerSecond = xSpeedInMibipixelsPerSecond;
			this.ySpeedInMibipixelsPerSecond = ySpeedInMibipixelsPerSecond;
			this.elapsedMicros = elapsedMicros;
			this.EnemyId = enemyId;
		}

		public static EnemyOrange GetEnemyOrange(
			int xMibi,
			int yMibi,
			bool isFacingRight,
			string enemyId)
		{
			return new EnemyOrange(
				xMibi: xMibi,
				yMibi: yMibi,
				xSpeedInMibipixelsPerSecond: 150000 * (isFacingRight ? 1 : -1),
				ySpeedInMibipixelsPerSecond: 0,
				elapsedMicros: 0,
				enemyId: enemyId);
		}

		public IReadOnlyList<Hitbox> GetHitboxes()
		{
			return new List<Hitbox>()
			{
				new Hitbox(
					x: (this.xMibi >> 10) - 6 * 3, 
					y: (this.yMibi >> 10) - 6 * 3, 
					width: 12 * 3, 
					height: 12 * 3)
			};
		}

		public IReadOnlyList<Hitbox> GetDamageBoxes()
		{
			return new List<Hitbox>()
			{
				new Hitbox(
					x: (this.xMibi >> 10) - 7 * 3,
					y: (this.yMibi >> 10) - 7 * 3,
					width: 14 * 3,
					height: 14 * 3)
			};
		}

		public IEnemy GetDeadEnemy()
		{
			return EnemyDeadPoof.SpawnEnemyDeadPoof(
				xMibi: this.xMibi,
				yMibi: this.yMibi,
				enemyId: this.EnemyId + "_enemyDeadPoof");
		}

		private static bool IsGroundOrSpike(ITilemap tilemap, int x, int y)
		{
			return tilemap.IsGround(x: x, y: y) || tilemap.IsSpikes(x: x, y: y);
		}

		public EnemyProcessing.Result ProcessFrame(
			int cameraX,
			int cameraY,
			int windowWidth,
			int windowHeight,
			int elapsedMicrosPerFrame,
			TuxState tuxState,
			IDTDeterministicRandom random,
			ITilemap tilemap,
			IReadOnlyList<string> levelFlags,
			ISoundOutput<GameSound> soundOutput)
		{
			List<IEnemy> list = new List<IEnemy>();

			int x = this.xMibi >> 10;
			int y = this.yMibi >> 10;

			int newXMibi = this.xMibi;
			int newYMibi = this.yMibi;
			int newXSpeedInMibipixelsPerSecond = this.xSpeedInMibipixelsPerSecond;
			int newYSpeedInMibipixelsPerSecond = this.ySpeedInMibipixelsPerSecond;

			bool isOnGround = IsGroundOrSpike(tilemap, x, y - 8 * 3 - 1) && !IsGroundOrSpike(tilemap, x, y - 8 * 3)
				|| IsGroundOrSpike(tilemap, x - 6 * 3, y - 8 * 3 - 1) && !IsGroundOrSpike(tilemap, x - 6 * 3, y - 8 * 3)
				|| IsGroundOrSpike(tilemap, x + 6 * 3, y - 8 * 3 - 1) && !IsGroundOrSpike(tilemap, x + 6 * 3, y - 8 * 3);

			if (isOnGround)
				newYSpeedInMibipixelsPerSecond = 1050000;
			else
				newYSpeedInMibipixelsPerSecond -= elapsedMicrosPerFrame * 3;

			if (newYSpeedInMibipixelsPerSecond < -2000 * 1000)
				newYSpeedInMibipixelsPerSecond = -2000 * 1000;

			int proposedNewYMibi = (int)(((long)newYMibi) + ((long)newYSpeedInMibipixelsPerSecond) * ((long)elapsedMicrosPerFrame) / 1000L / 1000L);
			if (newYSpeedInMibipixelsPerSecond > 0)
			{
				while (true)
				{
					if (IsGroundOrSpike(tilemap, newXMibi / 1024, proposedNewYMibi / 1024 + 8 * 3)
							|| IsGroundOrSpike(tilemap, newXMibi / 1024 - 8 * 3, proposedNewYMibi / 1024 + 8 * 3)
							|| IsGroundOrSpike(tilemap, newXMibi / 1024 + 8 * 3, proposedNewYMibi / 1024 + 8 * 3))
					{
						newYSpeedInMibipixelsPerSecond = 0;
						proposedNewYMibi -= 1024;
						if (proposedNewYMibi <= newYMibi)
						{
							proposedNewYMibi = newYMibi;
							break;
						}
					}
					else
						break;
				}
			}
			if (newYSpeedInMibipixelsPerSecond < 0)
			{
				while (true)
				{
					if (IsGroundOrSpike(tilemap, newXMibi / 1024, proposedNewYMibi / 1024 - 8 * 3)
							|| IsGroundOrSpike(tilemap, newXMibi / 1024 - 8 * 3, proposedNewYMibi / 1024 - 8 * 3)
							|| IsGroundOrSpike(tilemap, newXMibi / 1024 + 8 * 3, proposedNewYMibi / 1024 - 8 * 3))
					{
						newYSpeedInMibipixelsPerSecond = 0;
						proposedNewYMibi += 1024;
						if (proposedNewYMibi >= newYMibi)
						{
							proposedNewYMibi = newYMibi;
							break;
						}
					}
					else
						break;
				}
			}

			newYMibi = proposedNewYMibi;

			int proposedNewXMibi = (int)(((long)newXMibi) + ((long)newXSpeedInMibipixelsPerSecond) * ((long)elapsedMicrosPerFrame) / 1000L / 1000L);
			if (newXSpeedInMibipixelsPerSecond > 0)
			{
				while (true)
				{
					if (IsGroundOrSpike(tilemap, proposedNewXMibi / 1024 + 8 * 3, newYMibi / 1024 - 8 * 3)
							|| IsGroundOrSpike(tilemap, proposedNewXMibi / 1024 + 8 * 3, newYMibi / 1024)
							|| IsGroundOrSpike(tilemap, proposedNewXMibi / 1024 + 8 * 3, newYMibi / 1024 + 8 * 3))
					{
						proposedNewXMibi -= 1024;
						if (proposedNewXMibi <= newXMibi)
						{
							proposedNewXMibi = newXMibi;
							break;
						}
					}
					else
						break;
				}
			}
			if (newXSpeedInMibipixelsPerSecond < 0)
			{
				while (true)
				{
					if (IsGroundOrSpike(tilemap, proposedNewXMibi / 1024 - 8 * 3, newYMibi / 1024 - 8 * 3)
							|| IsGroundOrSpike(tilemap, proposedNewXMibi / 1024 - 8 * 3, newYMibi / 1024)
							|| IsGroundOrSpike(tilemap, proposedNewXMibi / 1024 - 8 * 3, newYMibi / 1024 + 8 * 3))
					{
						proposedNewXMibi += 1024;
						if (proposedNewXMibi >= newXMibi)
						{
							proposedNewXMibi = newXMibi;
							break;
						}
					}
					else
						break;
				}
			}

			newXMibi = proposedNewXMibi;

			if (newXSpeedInMibipixelsPerSecond > 0)
			{
				if (newXMibi < this.xMibi + 1024)
					newXSpeedInMibipixelsPerSecond = -newXSpeedInMibipixelsPerSecond;
			}
			else
			{
				if (newXMibi > this.xMibi - 1024)
					newXSpeedInMibipixelsPerSecond = -newXSpeedInMibipixelsPerSecond;
			}

			int newElapsedMicros = this.elapsedMicros + elapsedMicrosPerFrame;
			if (newElapsedMicros > 2 * 1000 * 1000 * 1000)
				newElapsedMicros = 1;

			bool isOutOfBounds = (this.xMibi >> 10) + 8 * 3 < cameraX - (windowWidth >> 1) - GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (this.xMibi >> 10) - 8 * 3 > cameraX + (windowWidth >> 1) + GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (this.yMibi >> 10) + 8 * 3 < cameraY - (windowHeight >> 1) - GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (this.yMibi >> 10) - 8 * 3 > cameraY + (windowHeight >> 1) + GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS;
			
			if (!isOutOfBounds)
				list.Add(new EnemyOrange(
					xMibi: newXMibi,
					yMibi: newYMibi,
					xSpeedInMibipixelsPerSecond: newXSpeedInMibipixelsPerSecond,
					ySpeedInMibipixelsPerSecond: newYSpeedInMibipixelsPerSecond,
					elapsedMicros: newElapsedMicros,
					enemyId: this.EnemyId));

			return new EnemyProcessing.Result(
				enemiesImmutableNullable: list,
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			bool isFacingRight = this.xSpeedInMibipixelsPerSecond > 0;

			GameImage image = isFacingRight ? GameImage.Orange : GameImage.OrangeMirrored;

			int spriteNum = (this.elapsedMicros / (100 * 1000)) % 8;

			displayOutput.DrawImageRotatedClockwise(
				image: image,
				imageX: spriteNum * 16,
				imageY: 0,
				imageWidth: 16,
				imageHeight: 16,
				x: (this.xMibi >> 10) - 8 * 3,
				y: (this.yMibi >> 10) - 8 * 3,
				degreesScaled: 0,
				scalingFactorScaled: 128 * 3);
		}
	}
}
