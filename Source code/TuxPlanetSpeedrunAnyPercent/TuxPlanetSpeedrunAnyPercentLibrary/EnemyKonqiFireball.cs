
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyKonqiFireball : IEnemy
	{
		private int xMibi;
		private int yMibi;
		private int elapsedMicros;

		private int xSpeedInMibipixelsPerSecond;
		private int ySpeedInMibipixelsPerSecond;

		public string EnemyId { get; private set; }

		public const string LEVEL_FLAG_DESPAWN_KONQI_FIREBALLS = "despawnKonqiFireballs";

		public static EnemyKonqiFireball GetEnemyKonqiFireball(
			int xMibi,
			int yMibi,
			int xSpeedInMibipixelsPerSecond,
			int ySpeedInMibipixelsPerSecond,
			string enemyId)
		{
			return new EnemyKonqiFireball(
				xMibi: xMibi,
				yMibi: yMibi,
				elapsedMicros: 0,
				xSpeedInMibipixelsPerSecond: xSpeedInMibipixelsPerSecond,
				ySpeedInMibipixelsPerSecond: ySpeedInMibipixelsPerSecond,
				enemyId: enemyId);
		}

		private EnemyKonqiFireball(
			int xMibi,
			int yMibi,
			int elapsedMicros,
			int xSpeedInMibipixelsPerSecond,
			int ySpeedInMibipixelsPerSecond,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.elapsedMicros = elapsedMicros;
			this.xSpeedInMibipixelsPerSecond = xSpeedInMibipixelsPerSecond;
			this.ySpeedInMibipixelsPerSecond = ySpeedInMibipixelsPerSecond;
			this.EnemyId = enemyId;
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
			return null;
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
			bool shouldDespawn = false;
			for (int i = 0; i < levelFlags.Count; i++)
			{
				if (levelFlags[i] == LEVEL_FLAG_DESPAWN_KONQI_FIREBALLS)
				{
					shouldDespawn = true;
					break;
				}
			}
			if (shouldDespawn)
			{
				return new EnemyProcessing.Result(
					enemiesImmutableNullable: new List<IEnemy>()
					{
						EnemyDeadPoof.SpawnEnemyDeadPoof(
							xMibi: this.xMibi,
							yMibi: this.yMibi,
							enemyId: this.EnemyId + "_poof")
					},
					newlyKilledEnemiesImmutableNullable: null,
					newlyAddedLevelFlagsImmutableNullable: null);
			}

			int newXMibi = this.xMibi;
			int newYMibi = this.yMibi;
			int newYSpeedInMibipixelsPerSecond = this.ySpeedInMibipixelsPerSecond;

			newXMibi = newXMibi + this.xSpeedInMibipixelsPerSecond / 10000 * elapsedMicrosPerFrame / 100;
			newYMibi = newYMibi + newYSpeedInMibipixelsPerSecond / 10000 * elapsedMicrosPerFrame / 100;

			newYSpeedInMibipixelsPerSecond -= elapsedMicrosPerFrame * 3;

			List<IEnemy> newEnemies = new List<IEnemy>();

			if (tilemap.IsGround(x: newXMibi >> 10, y: newYMibi >> 10))
			{
				soundOutput.PlaySound(GameSound.Explosion02);
				newEnemies.Add(EnemyKonqiFireballExplosion.GetEnemyKonqiFireballExplosion(
					xMibi: newXMibi,
					yMibi: newYMibi,
					enemyId: this.EnemyId + "_explosion"));
			}
			else
				newEnemies.Add(new EnemyKonqiFireball(
					xMibi: newXMibi,
					yMibi: newYMibi,
					elapsedMicros: this.elapsedMicros + elapsedMicrosPerFrame,
					xSpeedInMibipixelsPerSecond: this.xSpeedInMibipixelsPerSecond,
					ySpeedInMibipixelsPerSecond: newYSpeedInMibipixelsPerSecond,
					enemyId: this.EnemyId));

			return new EnemyProcessing.Result(
				enemiesImmutableNullable: newEnemies,
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			int spriteNum = (this.elapsedMicros / (100 * 1000)) % 5;

			displayOutput.DrawImageRotatedClockwise(
				image: GameImage.Flame,
				imageX: spriteNum * 14,
				imageY: 0,
				imageWidth: 14,
				imageHeight: 20,
				x: (this.xMibi >> 10) - 7 * 3,
				y: (this.yMibi >> 10) - 10 * 3,
				degreesScaled: -DTMath.ArcTangentScaled(this.xSpeedInMibipixelsPerSecond, this.ySpeedInMibipixelsPerSecond) - 90 * 128,
				scalingFactorScaled: 128 * 3);
		}

		public IEnemy GetDeadEnemy()
		{
			return null;
		}
	}
}
