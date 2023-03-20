
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class EnemyEliteFlyamanitaGreaterOrbiter : IEnemy
	{
		private int xMibi;
		private int yMibi;
		private bool isFacingRight;
		private int elapsedMicros;

		private int eliteFlyamanitaXMibi;
		private int eliteFlyamanitaYMibi;
		private int angleScaled;
		private bool isRotatingClockwise;

		private bool isSpikes;
		private string eliteFlyamanitaIsDeadLevelFlag;
		private string greaterOrbiterIsDeadLevelFlag;

		private string rngSeed;
		private bool hasSpawnedOrbiters;

		private Difficulty difficulty;

		public string EnemyId { get; private set; }

		private EnemyEliteFlyamanitaGreaterOrbiter(
			int xMibi,
			int yMibi,
			bool isFacingRight,
			int elapsedMicros,
			int eliteFlyamanitaXMibi,
			int eliteFlyamanitaYMibi,
			int angleScaled,
			bool isRotatingClockwise,
			bool isSpikes,
			string eliteFlyamanitaIsDeadLevelFlag,
			string greaterOrbiterIsDeadLevelFlag,
			string rngSeed,
			bool hasSpawnedOrbiters,
			Difficulty difficulty,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.isFacingRight = isFacingRight;
			this.elapsedMicros = elapsedMicros;
			this.eliteFlyamanitaXMibi = eliteFlyamanitaXMibi;
			this.eliteFlyamanitaYMibi = eliteFlyamanitaYMibi;
			this.angleScaled = angleScaled;
			this.isRotatingClockwise = isRotatingClockwise;
			this.isSpikes = isSpikes;
			this.eliteFlyamanitaIsDeadLevelFlag = eliteFlyamanitaIsDeadLevelFlag;
			this.greaterOrbiterIsDeadLevelFlag = greaterOrbiterIsDeadLevelFlag;
			this.rngSeed = rngSeed;
			this.hasSpawnedOrbiters = hasSpawnedOrbiters;
			this.difficulty = difficulty;
			this.EnemyId = enemyId;
		}

		public static EnemyEliteFlyamanitaGreaterOrbiter GetEnemyEliteFlyamanitaGreaterOrbiter(
			int eliteFlyamanitaXMibi,
			int eliteFlyamanitaYMibi,
			int angleScaled,
			bool isRotatingClockwise,
			bool isSpikes,
			string eliteFlyamanitaIsDeadLevelFlag,
			string rngSeed,
			Difficulty difficulty,
			string enemyId)
		{
			Tuple<int, int> location = EnemyEliteFlyamanita.GetGreaterOrbiterLocation(
				eliteFlyamanitaXMibi: eliteFlyamanitaXMibi,
				eliteFlyamanitaYMibi: eliteFlyamanitaYMibi,
				angleScaled: angleScaled);

			return new EnemyEliteFlyamanitaGreaterOrbiter(
				xMibi: location.Item1,
				yMibi: location.Item2,
				isFacingRight: false,
				elapsedMicros: 0,
				eliteFlyamanitaXMibi: eliteFlyamanitaXMibi,
				eliteFlyamanitaYMibi: eliteFlyamanitaYMibi,
				angleScaled: DTMath.NormalizeDegreesScaled(angleScaled),
				isRotatingClockwise: isRotatingClockwise,
				isSpikes: isSpikes,
				eliteFlyamanitaIsDeadLevelFlag: eliteFlyamanitaIsDeadLevelFlag,
				greaterOrbiterIsDeadLevelFlag: eliteFlyamanitaIsDeadLevelFlag + "_orbiter[" + enemyId + "]",
				rngSeed: rngSeed,
				hasSpawnedOrbiters: false,
				difficulty: difficulty,
				enemyId: enemyId);
		}

		public IReadOnlyList<Hitbox> GetHitboxes()
		{
			Hitbox hitbox = this.isSpikes
				? new Hitbox(
					x: (this.xMibi >> 10) - 6 * 6,
					y: (this.yMibi >> 10) - 6 * 6,
					width: 12 * 6,
					height: 12 * 6)
				: new Hitbox(
					x: (this.xMibi >> 10) - 8 * 6,
					y: (this.yMibi >> 10) - 8 * 6,
					width: 16 * 6,
					height: 16 * 6);

			List<Hitbox> list = new List<Hitbox>();
			list.Add(hitbox);

			return list;
		}

		public IReadOnlyList<Hitbox> GetDamageBoxes()
		{
			if (this.isSpikes)
				return null;

			List<Hitbox> list = new List<Hitbox>();
			list.Add(new Hitbox(
				x: (this.xMibi >> 10) - 8 * 6,
				y: (this.yMibi >> 10) - 8 * 6,
				width: 16 * 6,
				height: 16 * 6));

			return list;
		}

		public IEnemy GetDeadEnemy()
		{
			DTDeterministicRandom random = new DTDeterministicRandom(seed: 0);
			random.DeserializeFromString(this.rngSeed);

			return EnemyEliteFlyamanitaGreaterOrbiterDead.SpawnEnemyEliteFlyamanitaGreaterOrbiterDead(
				xMibi: this.xMibi,
				yMibi: this.yMibi,
				angularSpeedInAnglesScaledPerSecond: (random.NextInt(400 * 128) + 120 * 128) * (random.NextBool() ? 1 : -1),
				isSpikes: this.isSpikes,
				eliteFlyamanitaGreaterOrbiterIsDeadLevelFlag: this.greaterOrbiterIsDeadLevelFlag,
				enemyId: this.EnemyId + "_SpawnEnemyEliteFlyamanitaGreaterOrbiterDead");
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
			bool isEliteFlyamanitaDead = false;

			for (int i = 0; i < levelFlags.Count; i++)
			{
				string levelFlag = levelFlags[i];
				if (levelFlag == this.eliteFlyamanitaIsDeadLevelFlag)
				{
					isEliteFlyamanitaDead = true;
					break;
				}
			}

			if (isEliteFlyamanitaDead)
			{
				return new EnemyProcessing.Result(
					enemiesImmutableNullable: new List<IEnemy>()
					{
						this.GetDeadEnemy()
					},
					newlyKilledEnemiesImmutableNullable: new List<string>() { this.EnemyId },
					newlyAddedLevelFlagsImmutableNullable: null);
			}

			List<IEnemy> list = new List<IEnemy>();

			bool newIsFacingRight;

			if (tuxState.TeleportStartingLocation != null)
				newIsFacingRight = tuxState.TeleportStartingLocation.Item1 > this.xMibi;
			else
				newIsFacingRight = tuxState.XMibi > this.xMibi;

			int newElapsedMicros = this.elapsedMicros + elapsedMicrosPerFrame;
			if (newElapsedMicros > 2 * 1000 * 1000 * 1000)
				newElapsedMicros = 1;

			int newAngleScaled = this.angleScaled;

			if (this.isRotatingClockwise)
				newAngleScaled = newAngleScaled - EnemyEliteFlyamanita.GREATER_ORBITER_ROTATION_SPEED_IN_ANGLES_SCALED_PER_SECOND / 1000 * elapsedMicrosPerFrame / 1000;
			else
				newAngleScaled = newAngleScaled + EnemyEliteFlyamanita.GREATER_ORBITER_ROTATION_SPEED_IN_ANGLES_SCALED_PER_SECOND / 1000 * elapsedMicrosPerFrame / 1000;

			Tuple<int, int> newLocation = EnemyEliteFlyamanita.GetGreaterOrbiterLocation(
				eliteFlyamanitaXMibi: this.eliteFlyamanitaXMibi,
				eliteFlyamanitaYMibi: this.eliteFlyamanitaYMibi,
				angleScaled: newAngleScaled);

			string newRngSeed = this.rngSeed;

			if (!this.hasSpawnedOrbiters)
			{
				DTDeterministicRandom enemyRng = new DTDeterministicRandom(seed: 0);
				enemyRng.DeserializeFromString(newRngSeed);

				int lesserOrbiterAngleScaledBaseValue = enemyRng.NextInt(360 * 128);

				bool areLesserOrbitersRotatingClockwise = enemyRng.NextBool();

				for (int i = 0; i < 3; i++)
				{
					enemyRng.NextBool();
					string orbiterRngSeed = enemyRng.SerializeToString();
					enemyRng.NextBool();

					list.Add(EnemyEliteFlyamanitaLesserOrbiter.GetEnemyEliteFlyamanitaLesserOrbiter(
						eliteFlyamanitaXMibi: this.eliteFlyamanitaXMibi,
						eliteFlyamanitaYMibi: this.eliteFlyamanitaYMibi,
						greaterOrbiterAngleScaled: this.angleScaled,
						greaterOrbiterIsRotatingClockwise: this.isRotatingClockwise,
						angleScaled: lesserOrbiterAngleScaledBaseValue + i * (120 * 128),
						isRotatingClockwise: areLesserOrbitersRotatingClockwise,
						isSpikes: this.difficulty == Difficulty.Hard ? (i == 0) : false,
						eliteFlyamanitaIsDeadLevelFlag: this.eliteFlyamanitaIsDeadLevelFlag,
						greaterOrbiterIsDeadLevelFlag: this.greaterOrbiterIsDeadLevelFlag, 
						rngSeed: orbiterRngSeed,
						enemyId: this.EnemyId + "_orbiter[" + i.ToStringCultureInvariant() + "]"));
				}

				newRngSeed = enemyRng.SerializeToString();
			}

			list.Add(new EnemyEliteFlyamanitaGreaterOrbiter(
				xMibi: newLocation.Item1,
				yMibi: newLocation.Item2,
				isFacingRight: newIsFacingRight,
				elapsedMicros: newElapsedMicros,
				eliteFlyamanitaXMibi: this.eliteFlyamanitaXMibi,
				eliteFlyamanitaYMibi: this.eliteFlyamanitaYMibi,
				angleScaled: DTMath.NormalizeDegreesScaled(newAngleScaled),
				isRotatingClockwise: this.isRotatingClockwise,
				isSpikes: this.isSpikes,
				eliteFlyamanitaIsDeadLevelFlag: this.eliteFlyamanitaIsDeadLevelFlag,
				greaterOrbiterIsDeadLevelFlag: this.greaterOrbiterIsDeadLevelFlag,
				rngSeed: newRngSeed,
				hasSpawnedOrbiters: true,
				difficulty: this.difficulty,
				enemyId: this.EnemyId));

			return new EnemyProcessing.Result(
				enemiesImmutableNullable: list,
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			if (this.isSpikes)
			{
				displayOutput.DrawImageRotatedClockwise(
					image: GameImage.Spikes,
					imageX: 0,
					imageY: 0,
					imageWidth: 16,
					imageHeight: 16,
					x: (this.xMibi >> 10) - 8 * 6,
					y: (this.yMibi >> 10) - 8 * 6,
					degreesScaled: 0,
					scalingFactorScaled: 128 * 6);
			}
			else
			{
				GameImage image = this.isFacingRight ? GameImage.Flyamanita : GameImage.FlyamanitaMirrored;

				int spriteNum = (this.elapsedMicros / (100 * 1000)) % 4;

				displayOutput.DrawImageRotatedClockwise(
					image: image,
					imageX: spriteNum * 20,
					imageY: 0,
					imageWidth: 20,
					imageHeight: 20,
					x: (this.xMibi >> 10) - 10 * 6,
					y: (this.yMibi >> 10) - 10 * 6,
					degreesScaled: 0,
					scalingFactorScaled: 128 * 6);
			}
		}
	}
}
