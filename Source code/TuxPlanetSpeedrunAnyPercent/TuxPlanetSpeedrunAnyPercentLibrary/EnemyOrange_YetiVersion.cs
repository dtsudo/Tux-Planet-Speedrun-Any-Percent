
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class EnemyOrange_YetiVersion : IEnemy
	{
		private int xMibi;
		private int yMibi;

		private int xSpeedInMibipixelsPerSecond;
		private int ySpeedInMibipixelsPerSecond;

		private int jumpSpeed;

		private int elapsedMicros;

		public string EnemyId { get; private set; }

		public const string LEVEL_FLAG_DESPAWN_ENEMY_ORANGE_YETI_VERSION = "despawnEnemyOrangeYetiVersion";

		private EnemyOrange_YetiVersion(
			int xMibi,
			int yMibi,
			int xSpeedInMibipixelsPerSecond,
			int ySpeedInMibipixelsPerSecond,
			int jumpSpeed,
			int elapsedMicros,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.xSpeedInMibipixelsPerSecond = xSpeedInMibipixelsPerSecond;
			this.ySpeedInMibipixelsPerSecond = ySpeedInMibipixelsPerSecond;
			this.jumpSpeed = jumpSpeed;
			this.elapsedMicros = elapsedMicros;
			this.EnemyId = enemyId;
		}

		public static EnemyOrange_YetiVersion GetEnemyOrange_YetiVersion(
			int xMibi,
			int yMibi,
			int xSpeedInMibipixelsPerSecond,
			int jumpSpeed,
			string enemyId)
		{
			return new EnemyOrange_YetiVersion(
				xMibi: xMibi,
				yMibi: yMibi,
				xSpeedInMibipixelsPerSecond: xSpeedInMibipixelsPerSecond,
				ySpeedInMibipixelsPerSecond: 0,
				jumpSpeed: jumpSpeed,
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
			if (levelFlags.Contains(LEVEL_FLAG_DESPAWN_ENEMY_ORANGE_YETI_VERSION))
			{
				return new EnemyProcessing.Result(
					enemiesImmutableNullable: new List<IEnemy>()
					{
						EnemyDeadPoof.SpawnEnemyDeadPoof(
							xMibi: this.xMibi,
							yMibi: this.yMibi,
							enemyId: this.EnemyId + "_despawned")
					},
					newlyKilledEnemiesImmutableNullable: null,
					newlyAddedLevelFlagsImmutableNullable: null);
			}

			List<IEnemy> list = new List<IEnemy>();

			int x = this.xMibi >> 10;
			int y = this.yMibi >> 10;

			int newXMibi = this.xMibi;
			int newYMibi = this.yMibi;
			int newXSpeedInMibipixelsPerSecond = this.xSpeedInMibipixelsPerSecond;
			int newYSpeedInMibipixelsPerSecond = this.ySpeedInMibipixelsPerSecond;

			bool isOnGround = tilemap.IsGround(x, y - 8 * 3 - 1);

			if (isOnGround)
				newYSpeedInMibipixelsPerSecond = this.jumpSpeed;
			else
				newYSpeedInMibipixelsPerSecond -= elapsedMicrosPerFrame * 3;

			if (newYSpeedInMibipixelsPerSecond < -2000 * 1000)
				newYSpeedInMibipixelsPerSecond = -2000 * 1000;

			int proposedNewYMibi = (int)(((long)newYMibi) + ((long)newYSpeedInMibipixelsPerSecond) * ((long)elapsedMicrosPerFrame) / 1000L / 1000L);
			if (newYSpeedInMibipixelsPerSecond < 0)
			{
				while (true)
				{
					if (tilemap.IsGround(newXMibi >> 10, (proposedNewYMibi >> 10) - 8 * 3))
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
					if (tilemap.IsGround((proposedNewXMibi >> 10) + 8 * 3, newYMibi >> 10))
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
					if (tilemap.IsGround((proposedNewXMibi >> 10) - 8 * 3, newYMibi >> 10))
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
				if (newXMibi <= this.xMibi + 1024)
					newXSpeedInMibipixelsPerSecond = -newXSpeedInMibipixelsPerSecond;
			}
			else
			{
				if (newXMibi >= this.xMibi - 1024)
					newXSpeedInMibipixelsPerSecond = -newXSpeedInMibipixelsPerSecond;
			}

			int newElapsedMicros = this.elapsedMicros + elapsedMicrosPerFrame;
			if (newElapsedMicros > 2 * 1000 * 1000 * 1000)
				newElapsedMicros = 1;

			list.Add(new EnemyOrange_YetiVersion(
				xMibi: newXMibi,
				yMibi: newYMibi,
				xSpeedInMibipixelsPerSecond: newXSpeedInMibipixelsPerSecond,
				ySpeedInMibipixelsPerSecond: newYSpeedInMibipixelsPerSecond,
				jumpSpeed: this.jumpSpeed,
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
