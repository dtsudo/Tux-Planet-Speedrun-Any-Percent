
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class EnemyYetiBoss_Hit : IEnemy
	{
		private int xMibi;
		private int yMibi;
		private int elapsedMicros;

		private bool hasTriedSpawningSpikes;

		private int xSpeedInMibipixelsPerSecond;
		private int ySpeedInMibipixelsPerSecond;

		private bool isFacingRight;

		private int enemyIdCounter;

		private int numTimesHit;
		private string rngSeed;

		public string EnemyId { get; private set; }

		private EnemyYetiBoss_Hit(
			int xMibi,
			int yMibi,
			int elapsedMicros,
			bool hasTriedSpawningSpikes,
			int xSpeedInMibipixelsPerSecond,
			int ySpeedInMibipixelsPerSecond,
			bool isFacingRight,
			int enemyIdCounter,
			int numTimesHit,
			string rngSeed,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.elapsedMicros = elapsedMicros;
			this.hasTriedSpawningSpikes = hasTriedSpawningSpikes;
			this.xSpeedInMibipixelsPerSecond = xSpeedInMibipixelsPerSecond;
			this.ySpeedInMibipixelsPerSecond = ySpeedInMibipixelsPerSecond;
			this.isFacingRight = isFacingRight;
			this.enemyIdCounter = enemyIdCounter;
			this.numTimesHit = numTimesHit;
			this.rngSeed = rngSeed;
			this.EnemyId = enemyId;
		}

		public static EnemyYetiBoss_Hit GetEnemyYetiBoss_Hit(
			int xMibi,
			int yMibi,
			int elapsedMicros,
			bool isFacingRight,
			int enemyIdCounter,
			int numTimesHit,
			string rngSeed,
			string enemyId)
		{
			return new EnemyYetiBoss_Hit(
				xMibi: xMibi,
				yMibi: yMibi,
				elapsedMicros: elapsedMicros,
				hasTriedSpawningSpikes: false,
				xSpeedInMibipixelsPerSecond: 400 * 1000 * (isFacingRight ? -1 : 1),
				ySpeedInMibipixelsPerSecond: 700 * 1000,
				isFacingRight: isFacingRight,
				enemyIdCounter: enemyIdCounter,
				numTimesHit: numTimesHit,
				rngSeed: rngSeed,
				enemyId: enemyId);
		}

		public IReadOnlyList<Hitbox> GetHitboxes()
		{
			return null;
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

			int newXMibi = this.xMibi;
			int newYMibi = this.yMibi;

			int newXSpeedInMibipixelsPerSecond = this.xSpeedInMibipixelsPerSecond;
			int newYSpeedInMibipixelsPerSecond = this.ySpeedInMibipixelsPerSecond;

			List<string> newlyAddedLevelFlags = new List<string>();

			bool isOnGround = tilemap.IsGround(newXMibi >> 10, (newYMibi >> 10) - 32 * 3 - 1) && this.ySpeedInMibipixelsPerSecond == 0;

			if (this.numTimesHit == 4)
			{
				newlyAddedLevelFlags.Add(EnemyYetiBossSpike.LEVEL_FLAG_DESPAWN_YETI_BOSS_ROOM_SPIKES);
				newlyAddedLevelFlags.Add(EnemyOrange_YetiVersion.LEVEL_FLAG_DESPAWN_ENEMY_ORANGE_YETI_VERSION);
				newlyAddedLevelFlags.Add(EnemyEliteOrange_YetiVersion.LEVEL_FLAG_DESPAWN_ENEMY_ELITE_ORANGE_YETI_VERSION);
			}

			if (!this.hasTriedSpawningSpikes)
			{
				if (this.numTimesHit == 2)
				{
					for (int i = 0; i < 19; i++)
					{
						newEnemies.Add(EnemyYetiBossSpike.GetEnemyYetiBossSpike(
							xMibi: (207 * 48 + 36 * 48 + i * 48 + 24) << 10,
							yMibi: (17 * 48 + 24 + 4) << 10,
							cooldownUntilActive: i * 100 * 1000,
							enemyId: this.EnemyId + "_EnemyYetiBossSpike_" + i.ToStringCultureInvariant()));
					}
				}
			}

			if (isOnGround)
			{
				if (this.numTimesHit == 4)
				{
					newlyAddedLevelFlags.Add(LevelConfiguration_Level10.BEGIN_YETI_DEFEATED_CUTSCENE);

					if (this.isFacingRight)
						newlyAddedLevelFlags.Add(Cutscene_YetiBossDefeated.LEVEL_FLAG_YETI_IS_FACING_RIGHT);

					return new EnemyProcessing.Result(
						enemiesImmutableNullable: new List<IEnemy>()
						{
							EnemyYetiBoss_Defeated.GetEnemyYetiBoss_Defeated(
								xMibi: this.xMibi,
								yMibi: this.yMibi,
								elapsedMicros: 0,
								isFacingRight: this.isFacingRight,
								enemyId: this.EnemyId)
						},
						newlyKilledEnemiesImmutableNullable: null,
						newlyAddedLevelFlagsImmutableNullable: newlyAddedLevelFlags);
				}

				return new EnemyProcessing.Result(
					enemiesImmutableNullable: new List<IEnemy>()
					{
						EnemyYetiBoss_Jump.GetEnemyYetiBoss_Jump(
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
					newlyAddedLevelFlagsImmutableNullable: newlyAddedLevelFlags);
			}
			
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

			newXMibi = (int)(((long)newXMibi) + ((long)newXSpeedInMibipixelsPerSecond) * ((long)elapsedMicrosPerFrame) / 1000L / 1000L);
		
			newEnemies.Add(new EnemyYetiBoss_Hit(
				xMibi: newXMibi,
				yMibi: newYMibi,
				elapsedMicros: newElapsedMicros,
				hasTriedSpawningSpikes: true,
				xSpeedInMibipixelsPerSecond: newXSpeedInMibipixelsPerSecond,
				ySpeedInMibipixelsPerSecond: newYSpeedInMibipixelsPerSecond,
				isFacingRight: this.isFacingRight,
				enemyIdCounter: this.enemyIdCounter,
				numTimesHit: this.numTimesHit,
				rngSeed: this.rngSeed,
				enemyId: this.EnemyId));

			return new EnemyProcessing.Result(
				enemiesImmutableNullable: newEnemies,
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: newlyAddedLevelFlags);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			int spriteNum = (this.elapsedMicros / (100 * 1000)) % 2;

			displayOutput.DrawImageRotatedClockwise(
				image: this.isFacingRight ? GameImage.Yeti : GameImage.YetiMirrored,
				imageX: spriteNum * 64,
				imageY: 4 * 64,
				imageWidth: 64,
				imageHeight: 64,
				x: (this.xMibi >> 10) - 32 * 3,
				y: (this.yMibi >> 10) - 32 * 3,
				degreesScaled: 0,
				scalingFactorScaled: 128 * 3);
		}
	}
}
