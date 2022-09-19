
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyEliteFlyamanita : IEnemy
	{
		private int xMibi;
		private int yMibi;
		private bool isFacingRight;
		private int elapsedMicros;

		private string rngSeed;
		private bool hasSpawnedOrbiters;

		private string eliteFlyamanitaIsDeadLevelFlag;

		public const int GREATER_ORBITER_RADIUS_IN_PIXELS = 400;
		public const int GREATER_ORBITER_ROTATION_SPEED_IN_ANGLES_SCALED_PER_SECOND = 50 * 128;

		public const int LESSER_ORBITER_RADIUS_IN_PIXELS = 150;
		public const int LESSER_ORBITER_ROTATION_SPEED_IN_ANGLES_SCALED_PER_SECOND = 60 * 128;

		public string EnemyId { get; private set; }

		private EnemyEliteFlyamanita(
			int xMibi,
			int yMibi,
			bool isFacingRight,
			int elapsedMicros,
			string rngSeed,
			bool hasSpawnedOrbiters,
			string isDeadLevelFlag,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.isFacingRight = isFacingRight;
			this.elapsedMicros = elapsedMicros;
			this.rngSeed = rngSeed;
			this.hasSpawnedOrbiters = hasSpawnedOrbiters;
			this.eliteFlyamanitaIsDeadLevelFlag = isDeadLevelFlag;
			this.EnemyId = enemyId;
		}

		public static EnemyEliteFlyamanita GetEnemyEliteFlyamanita(
			int xMibi,
			int yMibi,
			string rngSeed,
			string enemyId)
		{
			return new EnemyEliteFlyamanita(
				xMibi: xMibi,
				yMibi: yMibi,
				isFacingRight: false,
				elapsedMicros: 0,
				rngSeed: rngSeed,
				hasSpawnedOrbiters: false,
				isDeadLevelFlag: "enemyEliteFlyamanitaIsDeadLevelFlag[" + enemyId + "]",
				enemyId: enemyId);
		}

		public static Tuple<int, int> GetGreaterOrbiterLocation(int eliteFlyamanitaXMibi, int eliteFlyamanitaYMibi, int angleScaled)
		{
			return new Tuple<int, int>(
				item1: eliteFlyamanitaXMibi + GREATER_ORBITER_RADIUS_IN_PIXELS * DTMath.CosineScaled(degreesScaled: angleScaled),
				item2: eliteFlyamanitaYMibi + GREATER_ORBITER_RADIUS_IN_PIXELS * DTMath.SineScaled(degreesScaled: angleScaled));
		}

		public static Tuple<int, int> GetLesserOrbiterLocation(
			int eliteFlyamanitaXMibi, 
			int eliteFlyamanitaYMibi, 
			int greaterOrbiterAngleScaled,
			int angleScaled)
		{
			Tuple<int, int> greaterOrbiterLocation = GetGreaterOrbiterLocation(
				eliteFlyamanitaXMibi: eliteFlyamanitaXMibi,
				eliteFlyamanitaYMibi: eliteFlyamanitaYMibi,
				angleScaled: greaterOrbiterAngleScaled);

			return new Tuple<int, int>(
				item1: greaterOrbiterLocation.Item1 + LESSER_ORBITER_RADIUS_IN_PIXELS * DTMath.CosineScaled(degreesScaled: angleScaled),
				item2: greaterOrbiterLocation.Item2 + LESSER_ORBITER_RADIUS_IN_PIXELS * DTMath.SineScaled(degreesScaled: angleScaled));
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
			return new List<Hitbox>()
			{
				new Hitbox(
					x: (this.xMibi >> 10) - 7 * 9,
					y: (this.yMibi >> 10) - 7 * 9,
					width: 14 * 9,
					height: 14 * 9)
			};
		}

		public IReadOnlyList<Hitbox> GetDamageBoxes()
		{
			return new List<Hitbox>()
			{
				new Hitbox(
					x: (this.xMibi >> 10) - 7 * 9,
					y: (this.yMibi >> 10) - 7 * 9,
					width: 14 * 9,
					height: 14 * 9)
			};
		}

		public IEnemy GetDeadEnemy()
		{
			DTDeterministicRandom random = new DTDeterministicRandom(seed: 0);
			random.DeserializeFromString(this.rngSeed);

			int angularSpeedInAnglesScaledPerSecond = (random.NextInt(400 * 128) + 120 * 128) * (random.NextBool() ? 1 : -1);

			return EnemyEliteFlyamanitaDead.SpawnEnemyEliteFlyamanitaDead(
				xMibi: this.xMibi,
				yMibi: this.yMibi,
				angularSpeedInAnglesScaledPerSecond: angularSpeedInAnglesScaledPerSecond,
				eliteFlyamanitaIsDeadLevelFlag: this.eliteFlyamanitaIsDeadLevelFlag,
				enemyId: this.EnemyId + "_EnemyEliteFlyamanitaDead");
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

			bool newIsFacingRight;

			if (tuxState.TeleportStartingLocation != null)
				newIsFacingRight = tuxState.TeleportStartingLocation.Item1 > this.xMibi;
			else
				newIsFacingRight = tuxState.XMibi > this.xMibi;

			int newElapsedMicros = this.elapsedMicros + elapsedMicrosPerFrame;
			if (newElapsedMicros > 2 * 1000 * 1000 * 1000)
				newElapsedMicros = 1;

			string newRngSeed = this.rngSeed;

			if (!this.hasSpawnedOrbiters)
			{
				DTDeterministicRandom enemyRng = new DTDeterministicRandom(seed: 0);
				enemyRng.DeserializeFromString(newRngSeed);

				int angleScaledBaseValue = enemyRng.NextInt(360 * 128);

				bool isRotatingClockwise = enemyRng.NextBool();

				for (int i = 0; i < 3; i++)
				{
					enemyRng.NextBool();
					string orbiterRngSeed = enemyRng.SerializeToString();
					enemyRng.NextBool();

					list.Add(EnemyEliteFlyamanitaGreaterOrbiter.GetEnemyEliteFlyamanitaGreaterOrbiter(
						eliteFlyamanitaXMibi: this.xMibi,
						eliteFlyamanitaYMibi: this.yMibi,
						angleScaled: angleScaledBaseValue + i * (120 * 128),
						isRotatingClockwise: isRotatingClockwise,
						isSpikes: i != 0,
						eliteFlyamanitaIsDeadLevelFlag: this.eliteFlyamanitaIsDeadLevelFlag,
						rngSeed: orbiterRngSeed,
						enemyId: this.EnemyId + "_orbiter[" + i.ToStringCultureInvariant() + "]"));
				}

				newRngSeed = enemyRng.SerializeToString();
			}

			list.Add(new EnemyEliteFlyamanita(
				xMibi: this.xMibi,
				yMibi: this.yMibi,
				isFacingRight: newIsFacingRight,
				elapsedMicros: newElapsedMicros,
				rngSeed: newRngSeed,
				hasSpawnedOrbiters: true,
				isDeadLevelFlag: this.eliteFlyamanitaIsDeadLevelFlag,
				enemyId: this.EnemyId));

			return new EnemyProcessing.Result(
				enemies: list,
				newlyKilledEnemies: new List<string>(),
				newlyAddedLevelFlags: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			GameImage image = this.isFacingRight ? GameImage.Flyamanita : GameImage.FlyamanitaMirrored;

			int spriteNum = (this.elapsedMicros / (100 * 1000)) % 4;

			displayOutput.DrawImageRotatedClockwise(
				image: image,
				imageX: spriteNum * 20,
				imageY: 0,
				imageWidth: 20,
				imageHeight: 20,
				x: (this.xMibi >> 10) - 10 * 9,
				y: (this.yMibi >> 10) - 10 * 9,
				degreesScaled: 0,
				scalingFactorScaled: 128 * 9);
		}
	}
}
