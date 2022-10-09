
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyYetiBoss_Jump : IEnemy
	{
		private int xMibi;
		private int yMibi;
		private int elapsedMicros;

		private int xSpeedInMibipixelsPerSecond;
		private int ySpeedInMibipixelsPerSecond;
		private bool isFacingRight;

		private int numJumps;

		private int? jumpCooldown;
		private const int FIRST_JUMP_COOLDOWN = 1000 * 1000;
		private const int SUBSEQUENT_JUMP_COOLDOWN = 500 * 1000;

		private int enemyIdCounter;

		private int numTimesHit;
		private string rngSeed;

		public string EnemyId { get; private set; }

		private EnemyYetiBoss_Jump(
			int xMibi,
			int yMibi,
			int elapsedMicros,
			int xSpeedInMibipixelsPerSecond,
			int ySpeedInMibipixelsPerSecond,
			bool isFacingRight,
			int numJumps,
			int? jumpCooldown,
			int enemyIdCounter,
			int numTimesHit,
			string rngSeed,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.elapsedMicros = elapsedMicros;
			this.xSpeedInMibipixelsPerSecond = xSpeedInMibipixelsPerSecond;
			this.ySpeedInMibipixelsPerSecond = ySpeedInMibipixelsPerSecond;
			this.isFacingRight = isFacingRight;
			this.numJumps = numJumps;
			this.jumpCooldown = jumpCooldown;
			this.enemyIdCounter = enemyIdCounter;
			this.numTimesHit = numTimesHit;
			this.rngSeed = rngSeed;
			this.EnemyId = enemyId;
		}

		public static EnemyYetiBoss_Jump GetEnemyYetiBoss_Jump(
			int xMibi,
			int yMibi,
			int elapsedMicros,
			bool isFacingRight,
			int enemyIdCounter,
			int numTimesHit,
			string rngSeed,
			string enemyId)
		{
			return new EnemyYetiBoss_Jump(
				xMibi: xMibi,
				yMibi: yMibi,
				elapsedMicros: elapsedMicros,
				xSpeedInMibipixelsPerSecond: 0,
				ySpeedInMibipixelsPerSecond: 0,
				isFacingRight: isFacingRight,
				numJumps: 0,
				jumpCooldown: FIRST_JUMP_COOLDOWN,
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

			int newXMibi = this.xMibi;
			int newYMibi = this.yMibi;

			int newXSpeedInMibipixelsPerSecond = this.xSpeedInMibipixelsPerSecond;
			int newYSpeedInMibipixelsPerSecond = this.ySpeedInMibipixelsPerSecond;

			int newNumJumps = this.numJumps;

			bool isOnGround = tilemap.IsGround(newXMibi >> 10, (newYMibi >> 10) - 32 * 3 - 1);

			List<IEnemy> newEnemies = new List<IEnemy>();

			bool newIsFacingRight = this.isFacingRight;

			int? newJumpCooldown = this.jumpCooldown;

			if (isOnGround && newYSpeedInMibipixelsPerSecond <= 0)
			{
				newXSpeedInMibipixelsPerSecond = 0;
				newIsFacingRight = newXMibi < tuxState.XMibi;

				if (newJumpCooldown == null)
				{
					newJumpCooldown = SUBSEQUENT_JUMP_COOLDOWN;
					soundOutput.PlaySound(GameSound.Explosion00Modified);
					newElapsedMicros = 0;
				}
				newJumpCooldown = newJumpCooldown.Value - elapsedMicrosPerFrame;

				if (newJumpCooldown.Value <= 0)
				{
					newJumpCooldown = null;
					if (newNumJumps == 3)
					{
						return new EnemyProcessing.Result(
							enemiesImmutableNullable: new List<IEnemy>()
							{
								EnemyYetiBoss_Throw.GetEnemyYetiBoss_Throw(
									xMibi: newXMibi,
									yMibi: newYMibi,
									elapsedMicros: 0,
									isFacingRight: newIsFacingRight,
									enemyIdCounter: this.enemyIdCounter,
									numTimesHit: this.numTimesHit,
									rngSeed: this.rngSeed,
									enemyId: this.EnemyId)
							},
							newlyKilledEnemiesImmutableNullable: null,
							newlyAddedLevelFlagsImmutableNullable: null);
					}

					newYSpeedInMibipixelsPerSecond = 1700000;
					newElapsedMicros = 0;

					int deltaX = tuxState.XMibi - newXMibi;

					newXSpeedInMibipixelsPerSecond = deltaX * 9 / 10;

					newNumJumps++;
				}
			}

			if (!isOnGround)
				newYSpeedInMibipixelsPerSecond -= elapsedMicrosPerFrame * 3;

			int proposedNewYMibi = (int)(((long)newYMibi) + ((long)newYSpeedInMibipixelsPerSecond) * ((long)elapsedMicrosPerFrame) / 1000L / 1000L);
			if (newYSpeedInMibipixelsPerSecond < 0)
			{
				while (true)
				{
					if (tilemap.IsGround(newXMibi >> 10, (proposedNewYMibi >> 10) - 32 * 3))
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
					if (tilemap.IsGround((proposedNewXMibi >> 10) + 32 * 3, newYMibi >> 10))
					{
						newXSpeedInMibipixelsPerSecond = 0;
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
					if (tilemap.IsGround((proposedNewXMibi >> 10) - 32 * 3, newYMibi >> 10))
					{
						newXSpeedInMibipixelsPerSecond = 0;
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

			newEnemies.Add(new EnemyYetiBoss_Jump(
				xMibi: newXMibi,
				yMibi: newYMibi,
				elapsedMicros: newElapsedMicros,
				xSpeedInMibipixelsPerSecond: newXSpeedInMibipixelsPerSecond,
				ySpeedInMibipixelsPerSecond: newYSpeedInMibipixelsPerSecond,
				isFacingRight: newIsFacingRight,
				numJumps: newNumJumps,
				jumpCooldown: newJumpCooldown,
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
			int imageX;

			if (this.jumpCooldown.HasValue)
				imageX = this.jumpCooldown <= (SUBSEQUENT_JUMP_COOLDOWN / 2) ? 1 : 0;
			else if (this.ySpeedInMibipixelsPerSecond > 0)
				imageX = this.elapsedMicros < 300 * 1000 ? 2 : 3;
			else
				imageX = 4;

			displayOutput.DrawImageRotatedClockwise(
				image: this.isFacingRight ? GameImage.Yeti : GameImage.YetiMirrored,
				imageX: imageX * 64,
				imageY: 3 * 64,
				imageWidth: 64,
				imageHeight: 64,
				x: (this.xMibi >> 10) - 32 * 3,
				y: (this.yMibi >> 10) - 32 * 3,
				degreesScaled: 0,
				scalingFactorScaled: 128 * 3);
		}
	}
}
