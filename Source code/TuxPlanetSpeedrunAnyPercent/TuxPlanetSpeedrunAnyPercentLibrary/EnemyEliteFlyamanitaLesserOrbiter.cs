
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class EnemyEliteFlyamanitaLesserOrbiter : IEnemy
	{
		private int xMibi;
		private int yMibi;
		private bool isFacingRight;
		private int elapsedMicros;

		private int eliteFlyamanitaXMibi;
		private int eliteFlyamanitaYMibi;
		private int greaterOrbiterAngleScaled;
		private bool greaterOrbiterIsRotatingClockwise;

		private int angleScaled;
		private bool isRotatingClockwise;

		private bool isSpikes;
		private string eliteFlyamanitaIsDeadLevelFlag;
		private string greaterOrbiterIsDeadLevelFlag;

		private string rngSeed;

		public string EnemyId { get; private set; }

		private EnemyEliteFlyamanitaLesserOrbiter(
			int xMibi,
			int yMibi,
			bool isFacingRight,
			int elapsedMicros,
			int eliteFlyamanitaXMibi,
			int eliteFlyamanitaYMibi,
			int greaterOrbiterAngleScaled,
			bool greaterOrbiterIsRotatingClockwise,
			int angleScaled,
			bool isRotatingClockwise,
			bool isSpikes,
			string eliteFlyamanitaIsDeadLevelFlag,
			string greaterOrbiterIsDeadLevelFlag,
			string rngSeed,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.isFacingRight = isFacingRight;
			this.elapsedMicros = elapsedMicros;
			this.eliteFlyamanitaXMibi = eliteFlyamanitaXMibi;
			this.eliteFlyamanitaYMibi = eliteFlyamanitaYMibi;
			this.greaterOrbiterAngleScaled = greaterOrbiterAngleScaled;
			this.greaterOrbiterIsRotatingClockwise = greaterOrbiterIsRotatingClockwise;
			this.angleScaled = angleScaled;
			this.isRotatingClockwise = isRotatingClockwise;
			this.isSpikes = isSpikes;
			this.eliteFlyamanitaIsDeadLevelFlag = eliteFlyamanitaIsDeadLevelFlag;
			this.greaterOrbiterIsDeadLevelFlag = greaterOrbiterIsDeadLevelFlag;
			this.rngSeed = rngSeed;
			this.EnemyId = enemyId;
		}

		public static EnemyEliteFlyamanitaLesserOrbiter GetEnemyEliteFlyamanitaLesserOrbiter(
			int eliteFlyamanitaXMibi,
			int eliteFlyamanitaYMibi,
			int greaterOrbiterAngleScaled,
			bool greaterOrbiterIsRotatingClockwise,
			int angleScaled,
			bool isRotatingClockwise,
			bool isSpikes,
			string eliteFlyamanitaIsDeadLevelFlag,
			string greaterOrbiterIsDeadLevelFlag,
			string rngSeed,
			string enemyId)
		{
			Tuple<int, int> location = EnemyEliteFlyamanita.GetLesserOrbiterLocation(
				eliteFlyamanitaXMibi: eliteFlyamanitaXMibi,
				eliteFlyamanitaYMibi: eliteFlyamanitaYMibi,
				greaterOrbiterAngleScaled: greaterOrbiterAngleScaled,
				angleScaled: angleScaled);

			return new EnemyEliteFlyamanitaLesserOrbiter(
				xMibi: location.Item1,
				yMibi: location.Item2,
				isFacingRight: false,
				elapsedMicros: 0,
				eliteFlyamanitaXMibi: eliteFlyamanitaXMibi,
				eliteFlyamanitaYMibi: eliteFlyamanitaYMibi,
				greaterOrbiterAngleScaled: DTMath.NormalizeDegreesScaled(greaterOrbiterAngleScaled),
				greaterOrbiterIsRotatingClockwise: greaterOrbiterIsRotatingClockwise,
				angleScaled: DTMath.NormalizeDegreesScaled(angleScaled),
				isRotatingClockwise: isRotatingClockwise,
				isSpikes: isSpikes,
				eliteFlyamanitaIsDeadLevelFlag: eliteFlyamanitaIsDeadLevelFlag,
				greaterOrbiterIsDeadLevelFlag: greaterOrbiterIsDeadLevelFlag,
				rngSeed: rngSeed,
				enemyId: enemyId);
		}

		public bool IsKonqiCutscene { get { return false; } }

		public bool IsRemoveKonqi { get { return false; } }

		public bool ShouldAlwaysSpawnRegardlessOfCamera { get { return false; } }

		public Tuple<int, int> GetKonqiCutsceneLocation()
		{
			return null;
		}

		public IReadOnlyList<Hitbox> GetHitboxes()
		{
			Hitbox hitbox = this.isSpikes
				? new Hitbox(
					x: (this.xMibi >> 10) - 6 * 3,
					y: (this.yMibi >> 10) - 6 * 3,
					width: 12 * 3,
					height: 12 * 3)
				: new Hitbox(
					x: (this.xMibi >> 10) - 9 * 3,
					y: (this.yMibi >> 10) - 9 * 3,
					width: 18 * 3,
					height: 18 * 3);

			return new List<Hitbox>() { hitbox };
		}

		public IReadOnlyList<Hitbox> GetDamageBoxes()
		{
			if (this.isSpikes)
				return new List<Hitbox>();

			return new List<Hitbox>()
			{
				new Hitbox(
					x: (this.xMibi >> 10) - 10 * 3,
					y: (this.yMibi >> 10) - 10 * 3,
					width: 20 * 3,
					height: 20 * 3)
			};
		}

		public IEnemy GetDeadEnemy()
		{
			DTDeterministicRandom random = new DTDeterministicRandom(seed: 0);
			random.DeserializeFromString(this.rngSeed);

			return EnemyEliteFlyamanitaLesserOrbiterDead.SpawnEnemyEliteFlyamanitaLesserOrbiterDead(
				xMibi: this.xMibi,
				yMibi: this.yMibi,
				angularSpeedInAnglesScaledPerSecond: (random.NextInt(400 * 128) + 120 * 128) * (random.NextBool() ? 1 : -1),
				isSpikes: this.isSpikes,
				enemyId: this.EnemyId + "_SpawnEnemyEliteFlyamanitaLesserOrbiterDead");
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
			bool isEliteFlyamanitaOrGreaterOrbiterDead = false;

			for (int i = 0; i < levelFlags.Count; i++)
			{
				string levelFlag = levelFlags[i];
				if (levelFlag == this.eliteFlyamanitaIsDeadLevelFlag || levelFlag == this.greaterOrbiterIsDeadLevelFlag)
				{
					isEliteFlyamanitaOrGreaterOrbiterDead = true;
					break;
				}
			}

			if (isEliteFlyamanitaOrGreaterOrbiterDead)
			{
				return new EnemyProcessing.Result(
					enemies: new List<IEnemy>()
					{
						this.GetDeadEnemy()
					},
					newlyKilledEnemies: new List<string>() { this.EnemyId },
					newlyAddedLevelFlags: null);
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

			int newGreaterOrbiterAngleScaled = this.greaterOrbiterAngleScaled;

			if (this.greaterOrbiterIsRotatingClockwise)
				newGreaterOrbiterAngleScaled = newGreaterOrbiterAngleScaled - EnemyEliteFlyamanita.GREATER_ORBITER_ROTATION_SPEED_IN_ANGLES_SCALED_PER_SECOND / 1000 * elapsedMicrosPerFrame / 1000;
			else
				newGreaterOrbiterAngleScaled = newGreaterOrbiterAngleScaled + EnemyEliteFlyamanita.GREATER_ORBITER_ROTATION_SPEED_IN_ANGLES_SCALED_PER_SECOND / 1000 * elapsedMicrosPerFrame / 1000;

			int newAngleScaled = this.angleScaled;

			if (this.isRotatingClockwise)
				newAngleScaled = newAngleScaled - EnemyEliteFlyamanita.LESSER_ORBITER_ROTATION_SPEED_IN_ANGLES_SCALED_PER_SECOND / 1000 * elapsedMicrosPerFrame / 1000;
			else
				newAngleScaled = newAngleScaled + EnemyEliteFlyamanita.LESSER_ORBITER_ROTATION_SPEED_IN_ANGLES_SCALED_PER_SECOND / 1000 * elapsedMicrosPerFrame / 1000;

			Tuple<int, int> newLocation = EnemyEliteFlyamanita.GetLesserOrbiterLocation(
				eliteFlyamanitaXMibi: this.eliteFlyamanitaXMibi,
				eliteFlyamanitaYMibi: this.eliteFlyamanitaYMibi,
				greaterOrbiterAngleScaled: this.greaterOrbiterAngleScaled,
				angleScaled: newAngleScaled);

			list.Add(new EnemyEliteFlyamanitaLesserOrbiter(
				xMibi: newLocation.Item1,
				yMibi: newLocation.Item2,
				isFacingRight: newIsFacingRight,
				elapsedMicros: newElapsedMicros,
				eliteFlyamanitaXMibi: this.eliteFlyamanitaXMibi,
				eliteFlyamanitaYMibi: this.eliteFlyamanitaYMibi,
				greaterOrbiterAngleScaled: DTMath.NormalizeDegreesScaled(newGreaterOrbiterAngleScaled),
				greaterOrbiterIsRotatingClockwise: this.greaterOrbiterIsRotatingClockwise,
				angleScaled: DTMath.NormalizeDegreesScaled(newAngleScaled),
				isRotatingClockwise: this.isRotatingClockwise,
				isSpikes: this.isSpikes,
				eliteFlyamanitaIsDeadLevelFlag: this.eliteFlyamanitaIsDeadLevelFlag,
				greaterOrbiterIsDeadLevelFlag: this.greaterOrbiterIsDeadLevelFlag,
				rngSeed: this.rngSeed,
				enemyId: this.EnemyId));

			return new EnemyProcessing.Result(
				enemies: list,
				newlyKilledEnemies: new List<string>(),
				newlyAddedLevelFlags: null);
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
					x: (this.xMibi >> 10) - 8 * 3,
					y: (this.yMibi >> 10) - 8 * 3,
					degreesScaled: 0,
					scalingFactorScaled: 128 * 3);
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
					x: (this.xMibi >> 10) - 10 * 3,
					y: (this.yMibi >> 10) - 10 * 3,
					degreesScaled: 0,
					scalingFactorScaled: 128 * 3);
			}
		}
	}
}
