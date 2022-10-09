
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyFlyamanitaLevel7 : IEnemy
	{
		private int originalXMibi;
		private int originalYMibi;
		private int xMibi;
		private int yMibi;
		private bool isFacingRight;
		private int elapsedMicros;

		private int initialYMibi;
		private int yAngleScaled;

		private int? deadAngularSpeedInAnglesScaledPerSecond;

		private string enemyIdPrefix;
		private int enemyIdCounter;

		public string EnemyId { get; private set; }

		private EnemyFlyamanitaLevel7(
			int originalXMibi,
			int originalYMibi,
			int xMibi,
			int yMibi,
			bool isFacingRight,
			int elapsedMicros,
			int initialYMibi,
			int yAngleScaled,
			int? deadAngularSpeedInAnglesScaledPerSecond,
			string enemyIdPrefix,
			int enemyIdCounter,
			string enemyId)
		{
			this.originalXMibi = originalXMibi;
			this.originalYMibi = originalYMibi;
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.isFacingRight = isFacingRight;
			this.elapsedMicros = elapsedMicros;
			this.initialYMibi = initialYMibi;
			this.yAngleScaled = yAngleScaled;
			this.deadAngularSpeedInAnglesScaledPerSecond = deadAngularSpeedInAnglesScaledPerSecond;
			this.enemyIdPrefix = enemyIdPrefix;
			this.enemyIdCounter = enemyIdCounter;
			this.EnemyId = enemyId;
		}

		public static EnemyFlyamanitaLevel7 GetEnemyFlyamanitaLevel7(
			int xMibi,
			int yMibi,
			string enemyIdPrefix,
			int enemyIdCounter)
		{
			return new EnemyFlyamanitaLevel7(
				originalXMibi: xMibi,
				originalYMibi: yMibi,
				xMibi: xMibi,
				yMibi: yMibi,
				isFacingRight: false,
				elapsedMicros: 0,
				initialYMibi: yMibi,
				yAngleScaled: 0,
				deadAngularSpeedInAnglesScaledPerSecond: null,
				enemyIdPrefix: enemyIdPrefix,
				enemyIdCounter: enemyIdCounter + 1,
				enemyId: enemyIdPrefix + "_" + enemyIdCounter.ToStringCultureInvariant());
		}

		public IReadOnlyList<Hitbox> GetHitboxes()
		{
			return new List<Hitbox>()
			{
				new Hitbox(
					x: (this.xMibi >> 10) - 9 * 3,
					y: (this.yMibi >> 10) - 9 * 3,
					width: 18 * 3,
					height: 18 * 3)
			};
		}

		public IReadOnlyList<Hitbox> GetDamageBoxes()
		{
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
			int angularSpeedInAnglesScaledPerSecond = 360 * 128;

			if (this.deadAngularSpeedInAnglesScaledPerSecond.HasValue)
				angularSpeedInAnglesScaledPerSecond = this.deadAngularSpeedInAnglesScaledPerSecond.Value;

			return EnemyFlyamanitaDeadLevel7.SpawnEnemyFlyamanitaDeadLevel7(
				originalXMibi: this.originalXMibi,
				originalYMibi: this.originalYMibi,
				xMibi: this.xMibi,
				yMibi: this.yMibi,
				angularSpeedInAnglesScaledPerSecond: angularSpeedInAnglesScaledPerSecond,
				enemyIdPrefix: this.enemyIdPrefix,
				enemyIdCounter: this.enemyIdCounter);
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

			int newYMibi = this.initialYMibi + 100 * DTMath.SineScaled(degreesScaled: this.yAngleScaled);
			bool newIsFacingRight;

			if (tuxState.TeleportStartingLocation != null)
				newIsFacingRight = tuxState.TeleportStartingLocation.Item1 > this.xMibi;
			else
				newIsFacingRight = tuxState.XMibi > this.xMibi;

			int newElapsedMicros = this.elapsedMicros + elapsedMicrosPerFrame;
			if (newElapsedMicros > 2 * 1000 * 1000 * 1000)
				newElapsedMicros = 1;

			int newYAngleScaled = this.yAngleScaled + (elapsedMicrosPerFrame >> 7);
			while (newYAngleScaled >= 360 * 128)
				newYAngleScaled -= 360 * 128;

			list.Add(new EnemyFlyamanitaLevel7(
				originalXMibi: this.originalXMibi,
				originalYMibi: this.originalYMibi,
				xMibi: this.xMibi,
				yMibi: newYMibi,
				isFacingRight: newIsFacingRight,
				elapsedMicros: newElapsedMicros,
				initialYMibi: this.initialYMibi,
				yAngleScaled: newYAngleScaled,
				deadAngularSpeedInAnglesScaledPerSecond: this.deadAngularSpeedInAnglesScaledPerSecond.HasValue
					? this.deadAngularSpeedInAnglesScaledPerSecond.Value 
					: (random.NextInt(400 * 128) + 120 * 128) * (random.NextBool() ? 1 : -1),
				enemyIdPrefix: this.enemyIdPrefix,
				enemyIdCounter: this.enemyIdCounter,
				enemyId: this.EnemyId));

			return new EnemyProcessing.Result(
				enemiesImmutableNullable: list,
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: null);
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
				x: (this.xMibi >> 10) - 10 * 3,
				y: (this.yMibi >> 10) - 10 * 3,
				degreesScaled: 0,
				scalingFactorScaled: 128 * 3);
		}
	}
}
