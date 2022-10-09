
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyYetiBoss_Charge : IEnemy
	{
		private int xMibi;
		private int yMibi;
		private int elapsedMicros;

		private bool isFacingRight;

		private int cooldown;
		private const int COOLDOWN = 300 * 1000;

		private int enemyIdCounter;

		private int numTimesHit;
		private string rngSeed;

		public string EnemyId { get; private set; }

		private EnemyYetiBoss_Charge(
			int xMibi,
			int yMibi,
			int elapsedMicros,
			bool isFacingRight,
			int cooldown,
			int enemyIdCounter,
			int numTimesHit,
			string rngSeed,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.elapsedMicros = elapsedMicros;
			this.isFacingRight = isFacingRight;
			this.cooldown = cooldown;
			this.enemyIdCounter = enemyIdCounter;
			this.numTimesHit = numTimesHit;
			this.rngSeed = rngSeed;
			this.EnemyId = enemyId;
		}

		public static EnemyYetiBoss_Charge GetEnemyYetiBoss_Charge(
			int xMibi,
			int yMibi,
			int elapsedMicros,
			bool isFacingRight,
			int enemyIdCounter,
			int numTimesHit,
			string rngSeed,
			string enemyId)
		{
			return new EnemyYetiBoss_Charge(
				xMibi: xMibi,
				yMibi: yMibi,
				elapsedMicros: elapsedMicros,
				isFacingRight: isFacingRight,
				cooldown: COOLDOWN,
				enemyIdCounter: enemyIdCounter,
				numTimesHit: numTimesHit,
				rngSeed: rngSeed,
				enemyId: enemyId);
		}

		public IReadOnlyList<Hitbox> GetHitboxes()
		{
			return new List<Hitbox>()
			{
				new Hitbox(
					x: (this.xMibi >> 10) - 16 * 3,
					y: (this.yMibi >> 10) - 16 * 3,
					width: 32 * 3,
					height: 32 * 3)
			};
		}

		public IReadOnlyList<Hitbox> GetDamageBoxes()
		{
			return null;
		}

		public IEnemy GetDeadEnemy()
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
			int newElapsedMicros = this.elapsedMicros + elapsedMicrosPerFrame;

			if (newElapsedMicros > 2 * 1000 * 1000 * 1000)
				newElapsedMicros = 1;

			List<IEnemy> newEnemies = new List<IEnemy>();

			int newCooldown = this.cooldown;

			newCooldown -= elapsedMicrosPerFrame;

			if (newCooldown <= 0)
				newCooldown = 0;

			int newXMibi = this.xMibi;

			bool hasHitWall;

			if (newCooldown <= 0)
			{
				if (this.isFacingRight)
				{
					newXMibi += 2 * elapsedMicrosPerFrame;
					hasHitWall = tilemap.IsGround((newXMibi >> 10) + 32 * 3, this.yMibi >> 10);
				}
				else
				{
					newXMibi -= 2 * elapsedMicrosPerFrame;
					hasHitWall = tilemap.IsGround((newXMibi >> 10) - 32 * 3, this.yMibi >> 10);
				}
			}
			else
				hasHitWall = false;

			if (hasHitWall)
			{
				soundOutput.PlaySound(GameSound.Explosion00Modified);
				return new EnemyProcessing.Result(
					enemiesImmutableNullable: new List<IEnemy>()
					{
						EnemyYetiBoss_Recoil.GetEnemyYetiBoss_Recoil(
							xMibi: this.xMibi,
							yMibi: this.yMibi,
							elapsedMicros: 0,
							isFacingRight: this.isFacingRight,
							enemyIdCounter: this.enemyIdCounter,
							numTimesHit: this.numTimesHit,
							rngSeed: this.rngSeed,
							enemyId: this.EnemyId)
					},
					newlyKilledEnemiesImmutableNullable: null,
					newlyAddedLevelFlagsImmutableNullable: null);
			}

			newEnemies.Add(new EnemyYetiBoss_Charge(
				xMibi: newXMibi,
				yMibi: this.yMibi,
				elapsedMicros: newElapsedMicros,
				isFacingRight: this.isFacingRight,
				cooldown: newCooldown,
				enemyIdCounter: this.enemyIdCounter,
				numTimesHit: this.numTimesHit,
				rngSeed: this.rngSeed,
				enemyId: this.EnemyId));

			return new EnemyProcessing.Result(
				enemiesImmutableNullable: newEnemies,
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			int spriteNum = (this.elapsedMicros / (25 * 1000)) % 8;

			displayOutput.DrawImageRotatedClockwise(
				image: this.isFacingRight ? GameImage.Yeti : GameImage.YetiMirrored,
				imageX: spriteNum * 64,
				imageY: 2 * 64,
				imageWidth: 64,
				imageHeight: 64,
				x: (this.xMibi >> 10) - 32 * 3,
				y: (this.yMibi >> 10) - 32 * 3,
				degreesScaled: 0,
				scalingFactorScaled: 128 * 3);
		}
	}
}
