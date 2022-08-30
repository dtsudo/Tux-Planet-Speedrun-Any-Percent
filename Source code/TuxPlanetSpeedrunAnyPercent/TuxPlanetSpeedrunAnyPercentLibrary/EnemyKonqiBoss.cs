﻿
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class EnemyKonqiBoss : IEnemy
	{
		private int xMibi;
		private int yMibi;
		private int elapsedMicros;

		private int numTimesHit;
		// Note that Konqi isn't actually ever invulnerable; he just appears to be so visually
		private int? invulnerabilityElapsedMicros;

		private int currentAttackCooldown;

		private int enemyIdCounter;

		private string rngSeed;

		private const int INVULNERABILITY_DURATION = 1000 * 1000;

		private List<string> emptyStringList;
		private List<Hitbox> emptyHitboxList;
		private int startingYMibi;

		public string EnemyId { get; private set; }

		private EnemyKonqiBoss(
			int xMibi,
			int yMibi,
			int elapsedMicros,
			int numTimesHit,
			int? invulnerabilityElapsedMicros,
			int currentAttackCooldown,
			int enemyIdCounter,
			string rngSeed,
			List<string> emptyStringList,
			List<Hitbox> emptyHitboxList,
			int startingYMibi,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.elapsedMicros = elapsedMicros;
			this.numTimesHit = numTimesHit;
			this.invulnerabilityElapsedMicros = invulnerabilityElapsedMicros;
			this.currentAttackCooldown = currentAttackCooldown;
			this.enemyIdCounter = enemyIdCounter;
			this.rngSeed = rngSeed;
			this.emptyStringList = emptyStringList;
			this.emptyHitboxList = emptyHitboxList;
			this.startingYMibi = startingYMibi;
			this.EnemyId = enemyId;
		}

		public static EnemyKonqiBoss GetEnemyKonqiBoss(
			int xMibi,
			int yMibi,
			string enemyId,
			string rngSeed)
		{
			return new EnemyKonqiBoss(
				xMibi: xMibi,
				yMibi: yMibi,
				elapsedMicros: 0,
				numTimesHit: 0,
				invulnerabilityElapsedMicros: null,
				currentAttackCooldown: 1000 * 1000,
				enemyIdCounter: 0,
				rngSeed: rngSeed,
				emptyStringList: new List<string>(),
				emptyHitboxList: new List<Hitbox>(),
				startingYMibi: yMibi,
				enemyId: enemyId);
		}

		public bool IsKonqiCutscene { get { return false; } }

		public bool IsRemoveKonqi { get { return false; } }

		public bool ShouldAlwaysSpawnRegardlessOfCamera { get { return true; } }

		public Tuple<int, int> GetKonqiCutsceneLocation()
		{
			return null;
		}

		public IReadOnlyList<Hitbox> GetHitboxes()
		{
			return new List<Hitbox>()
			{
				new Hitbox(
					x: (this.xMibi >> 10) - 8 * 3,
					y: (this.yMibi >> 10) - 8 * 3,
					width: 16 * 3,
					height: 26 * 3)
			};
		}

		public IReadOnlyList<Hitbox> GetDamageBoxes()
		{
			return new List<Hitbox>()
			{
				new Hitbox(
					x: (this.xMibi >> 10) - 8 * 3,
					y: (this.yMibi >> 10) - 8 * 3,
					width: 16 * 3,
					height: 26 * 3)
			};
		}

		private bool IsFacingRight()
		{
			return this.numTimesHit % 2 == 1;
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
			if (levelFlags.Contains(LevelConfiguration_Level6.DESPAWN_KONQI_AND_REMOVE_BOSS_DOORS))
			{
				return new EnemyProcessing.Result(
					enemies: new List<IEnemy>()
					{
						EnemyKonqiDisappear.GetEnemyKonqiDisappear(
							xMibi: this.xMibi,
							yMibi: this.yMibi,
							enemyId: this.EnemyId + "_konqiDisappear" + this.enemyIdCounter.ToStringCultureInvariant())
					},
					newlyKilledEnemies: this.emptyStringList,
					newlyAddedLevelFlags: null);
			}

			int newXMibi = this.xMibi;
			int newYMibi = this.yMibi;
			int? newInvulnerabilityElapsedMicros = this.invulnerabilityElapsedMicros;
			string newRngSeed = this.rngSeed;
			int newCurrentAttackCooldown = this.currentAttackCooldown;
			int newEnemyIdCounter = this.enemyIdCounter;

			List<IEnemy> newEnemies = new List<IEnemy>();

			int newElapsedMicros = this.elapsedMicros + elapsedMicrosPerFrame;

			if (newElapsedMicros > 2 * 1000 * 1000 * 1000)
				newElapsedMicros = 1;

			if (newInvulnerabilityElapsedMicros.HasValue && newInvulnerabilityElapsedMicros.Value == 0)
			{
				newEnemies.Add(EnemyKonqiDisappear.GetEnemyKonqiDisappear(
					xMibi: newXMibi,
					yMibi: newYMibi,
					enemyId: this.EnemyId + "_konqiDisappear" + newEnemyIdCounter.ToStringCultureInvariant()));
				newEnemyIdCounter++;

				if (this.numTimesHit % 2 == 0)
				{
					newXMibi += 48 * 15 * 1024;
				}
				else
				{
					newXMibi -= 48 * 15 * 1024;
				}

				newYMibi = this.startingYMibi;

				newEnemies.Add(EnemyKonqiDisappear.GetEnemyKonqiDisappear(
					xMibi: newXMibi,
					yMibi: newYMibi,
					enemyId: this.EnemyId + "_konqiDisappear" + newEnemyIdCounter.ToStringCultureInvariant()));
				newEnemyIdCounter++;
			}

			newCurrentAttackCooldown -= elapsedMicrosPerFrame;
			if (newCurrentAttackCooldown <= 0)
			{
				DTDeterministicRandom rng = new DTDeterministicRandom();
				rng.DeserializeFromString(newRngSeed);
				newCurrentAttackCooldown = 300 * 1000 + rng.NextInt(500 * 1000);
				int fireballYSpeed = 300 * 1000 + rng.NextInt(1500 * 1000);
				newRngSeed = rng.SerializeToString();

				if (this.numTimesHit < 4)
				{
					newEnemies.Add(EnemyKonqiFireball.GetEnemyKonqiFireball(
						xMibi: newXMibi + (this.IsFacingRight() ? 5 * 1024 : -5 * 1024),
						yMibi: newYMibi + 24 * 1024,
						xSpeedInMibipixelsPerSecond: this.IsFacingRight() ? 700 * 1000 : -700 * 1000,
						ySpeedInMibipixelsPerSecond: fireballYSpeed,
						enemyId: this.EnemyId + "_fireball" + newEnemyIdCounter.ToStringCultureInvariant()));
					newEnemyIdCounter++;
				}
			}

			if (newInvulnerabilityElapsedMicros != null)
			{
				newInvulnerabilityElapsedMicros = newInvulnerabilityElapsedMicros.Value + elapsedMicrosPerFrame;
				if (newInvulnerabilityElapsedMicros.Value >= INVULNERABILITY_DURATION)
					newInvulnerabilityElapsedMicros = null;
			}

			newEnemies.Add(new EnemyKonqiBoss(
				xMibi: newXMibi,
				yMibi: newYMibi,
				elapsedMicros: newElapsedMicros,
				numTimesHit: this.numTimesHit,
				invulnerabilityElapsedMicros: newInvulnerabilityElapsedMicros,
				currentAttackCooldown: newCurrentAttackCooldown,
				enemyIdCounter: newEnemyIdCounter,
				rngSeed: newRngSeed,
				emptyStringList: this.emptyStringList,
				emptyHitboxList: this.emptyHitboxList,
				startingYMibi: this.startingYMibi,
				enemyId: this.EnemyId));

			List<string> newlyAddedLevelFlags;

			if (this.numTimesHit == 4)
				newlyAddedLevelFlags = new List<string>() { LevelConfiguration_Level6.BOSS_DEFEATED };
			else
				newlyAddedLevelFlags = null;

			return new EnemyProcessing.Result(
				enemies: newEnemies,
				newlyKilledEnemies: this.emptyStringList,
				newlyAddedLevelFlags: newlyAddedLevelFlags);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			if (this.invulnerabilityElapsedMicros != null)
			{
				if ((this.invulnerabilityElapsedMicros.Value / (100 * 1000)) % 2 == 0)
					return;
			}

			int spriteNum = (this.elapsedMicros % 1000000) / 250000;

			GameImage image = this.IsFacingRight() ? GameImage.KonqiFire : GameImage.KonqiFireMirrored;

			displayOutput.DrawImageRotatedClockwise(
				image: image,
				imageX: spriteNum * 32,
				imageY: 0,
				imageWidth: 32,
				imageHeight: 32,
				x: (this.xMibi >> 10) - 16 * 3,
				y: (this.yMibi >> 10) - 8 * 3,
				degreesScaled: 0,
				scalingFactorScaled: 128 * 3);
		}

		public IEnemy GetDeadEnemy()
		{
			return new EnemyKonqiBoss(
				xMibi: this.xMibi,
				yMibi: this.yMibi,
				elapsedMicros: this.elapsedMicros,
				numTimesHit: this.numTimesHit + 1,
				invulnerabilityElapsedMicros: 0,
				currentAttackCooldown: this.currentAttackCooldown,
				enemyIdCounter: this.enemyIdCounter,
				rngSeed: this.rngSeed,
				emptyStringList: this.emptyStringList,
				emptyHitboxList: this.emptyHitboxList,
				startingYMibi: this.startingYMibi,
				enemyId: this.EnemyId + "_hit");
		}
	}
}