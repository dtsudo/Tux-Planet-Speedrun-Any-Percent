
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyYetiBoss_Throw : IEnemy
	{
		private int xMibi;
		private int yMibi;
		private int elapsedMicros;

		private bool isFacingRight;

		private int numThrows;

		private int? throwCooldown;
		private const int THROW_COOLDOWN = 200 * 1000;
		private int? postThrowCooldown;
		private const int POST_THROW_COOLDOWN = 100 * 1000;

		private int enemyIdCounter;

		private int numTimesHit;
		private string rngSeed;

		private List<string> emptyStringList;
		private List<Hitbox> emptyHitboxList;

		public string EnemyId { get; private set; }

		private EnemyYetiBoss_Throw(
			int xMibi,
			int yMibi,
			int elapsedMicros,
			bool isFacingRight,
			int numThrows,
			int? throwCooldown,
			int? postThrowCooldown,
			int enemyIdCounter,
			int numTimesHit,
			string rngSeed,
			List<string> emptyStringList,
			List<Hitbox> emptyHitboxList,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.elapsedMicros = elapsedMicros;
			this.isFacingRight = isFacingRight;
			this.numThrows = numThrows;
			this.throwCooldown = throwCooldown;
			this.postThrowCooldown = postThrowCooldown;
			this.enemyIdCounter = enemyIdCounter;
			this.numTimesHit = numTimesHit;
			this.rngSeed = rngSeed;
			this.emptyStringList = emptyStringList;
			this.emptyHitboxList = emptyHitboxList;
			this.EnemyId = enemyId;
		}

		public static EnemyYetiBoss_Throw GetEnemyYetiBoss_Throw(
			int xMibi,
			int yMibi,
			int elapsedMicros,
			bool isFacingRight,
			int enemyIdCounter,
			int numTimesHit,
			string rngSeed,
			string enemyId)
		{
			return new EnemyYetiBoss_Throw(
				xMibi: xMibi,
				yMibi: yMibi,
				elapsedMicros: elapsedMicros,
				isFacingRight: isFacingRight,
				numThrows: 0,
				throwCooldown: THROW_COOLDOWN,
				postThrowCooldown: null,
				enemyIdCounter: enemyIdCounter,
				numTimesHit: numTimesHit,
				rngSeed: rngSeed,
				emptyStringList: new List<string>(),
				emptyHitboxList: new List<Hitbox>(),
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
					x: (this.xMibi >> 10) - 16 * 3,
					y: (this.yMibi >> 10) - 16 * 3,
					width: 32 * 3,
					height: 32 * 3)
			};
		}

		public IReadOnlyList<Hitbox> GetDamageBoxes()
		{
			return this.emptyHitboxList;
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

			int newNumThrows = this.numThrows;
			int? newThrowCooldown = this.throwCooldown;
			int? newPostThrowCooldown = this.postThrowCooldown;

			int newEnemyIdCounter = this.enemyIdCounter;

			string newRngSeed = this.rngSeed;

			if (newThrowCooldown != null)
			{
				newThrowCooldown = newThrowCooldown.Value - elapsedMicrosPerFrame;

				if (newThrowCooldown.Value <= 0)
				{
					DTDeterministicRandom yetiRandom = new DTDeterministicRandom();
					yetiRandom.DeserializeFromString(newRngSeed);
					int xSpeedInMibipixelsPerSecond = 450000 + yetiRandom.NextInt(300000) - 150000;
					int jumpSpeed = 650000 + yetiRandom.NextInt(250000);
					int orbitersAngleScaled = yetiRandom.NextInt(360 * 128);
					bool isOrbitingClockwise = yetiRandom.NextBool();
					newRngSeed = yetiRandom.SerializeToString();

					newThrowCooldown = null;
					newPostThrowCooldown = POST_THROW_COOLDOWN;
					if (newNumThrows == 7 && this.numTimesHit >= 2)
						newEnemies.Add(EnemyEliteOrange_YetiVersion.GetEnemyEliteOrange_YetiVersion(
							xMibi: this.xMibi + 1024 * 30 * (this.isFacingRight ? 1 : -1),
							yMibi: this.yMibi,
							xSpeedInMibipixelsPerSecond: xSpeedInMibipixelsPerSecond * (this.isFacingRight ? 1 : -1),
							jumpSpeed: jumpSpeed,
							orbitersAngleScaled: orbitersAngleScaled,
							isOrbitingClockwise: isOrbitingClockwise,
							enemyId: this.EnemyId + "_" + newEnemyIdCounter.ToStringCultureInvariant()));
					else
						newEnemies.Add(EnemyOrange_YetiVersion.GetEnemyOrange_YetiVersion(
							xMibi: this.xMibi + 1024 * 30 * (this.isFacingRight ? 1 : -1),
							yMibi: this.yMibi,
							xSpeedInMibipixelsPerSecond: xSpeedInMibipixelsPerSecond * (this.isFacingRight ? 1 : -1),
							jumpSpeed: jumpSpeed,
							enemyId: this.EnemyId + "_" + newEnemyIdCounter.ToStringCultureInvariant()));
					newEnemyIdCounter++;
				}
			}

			if (newPostThrowCooldown != null)
			{
				newPostThrowCooldown = newPostThrowCooldown.Value - elapsedMicrosPerFrame;

				if (newPostThrowCooldown.Value <= 0)
				{
					newPostThrowCooldown = null;
					newNumThrows++;
					newThrowCooldown = THROW_COOLDOWN;

					if (this.numTimesHit == 0 && newNumThrows == 3 || newNumThrows == 8)
					{
						return new EnemyProcessing.Result(
							enemies: new List<IEnemy>()
							{
								EnemyYetiBoss_Charge.GetEnemyYetiBoss_Charge(
									xMibi: this.xMibi,
									yMibi: this.yMibi,
									elapsedMicros: 0,
									isFacingRight: tuxState.XMibi > this.xMibi,
									enemyIdCounter: newEnemyIdCounter,
									numTimesHit: this.numTimesHit,
									rngSeed: newRngSeed,
									enemyId: this.EnemyId)
							},
							newlyKilledEnemies: this.emptyStringList,
							newlyAddedLevelFlags: null);
					}
				}
			}

			newEnemies.Insert(0, new EnemyYetiBoss_Throw(
				xMibi: this.xMibi,
				yMibi: this.yMibi,
				elapsedMicros: newElapsedMicros,
				isFacingRight: this.isFacingRight,
				numThrows: newNumThrows,
				throwCooldown: newThrowCooldown,
				postThrowCooldown: newPostThrowCooldown,
				enemyIdCounter: newEnemyIdCounter,
				numTimesHit: this.numTimesHit,
				rngSeed: newRngSeed,
				emptyStringList: this.emptyStringList,
				emptyHitboxList: this.emptyHitboxList,
				enemyId: this.EnemyId));

			return new EnemyProcessing.Result(
				enemies: newEnemies,
				newlyKilledEnemies: this.emptyStringList,
				newlyAddedLevelFlags: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			int imageX;

			if (this.throwCooldown.HasValue)
				imageX = 0;
			else if (this.postThrowCooldown.Value > POST_THROW_COOLDOWN - 33 * 1000)
				imageX = 1;
			else if (this.postThrowCooldown.Value > POST_THROW_COOLDOWN - 67 * 1000)
				imageX = 2;
			else
				imageX = 3;

			displayOutput.DrawImageRotatedClockwise(
				image: this.isFacingRight ? GameImage.Yeti : GameImage.YetiMirrored,
				imageX: imageX * 64,
				imageY: 5 * 64,
				imageWidth: 64,
				imageHeight: 64,
				x: (this.xMibi >> 10) - 32 * 3,
				y: (this.yMibi >> 10) - 32 * 3,
				degreesScaled: 0,
				scalingFactorScaled: 128 * 3);
		}
	}
}
