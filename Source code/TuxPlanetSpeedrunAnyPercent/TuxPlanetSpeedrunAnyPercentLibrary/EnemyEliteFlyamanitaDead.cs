
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyEliteFlyamanitaDead : IEnemy
	{
		private int xMibi;
		private int yMibi;

		private int ySpeedInMibipixelsPerSecond;

		private int angularSpeedInAnglesScaledPerSecond;
		private int angleScaled;

		private string eliteFlyamanitaIsDeadLevelFlag;

		private bool hasAddedLevelFlag;

		public string EnemyId { get; private set; }

		private EnemyEliteFlyamanitaDead(
			int xMibi,
			int yMibi,
			int ySpeedInMibipixelsPerSecond,
			int angularSpeedInAnglesScaledPerSecond,
			int angleScaled,
			string eliteFlyamanitaIsDeadLevelFlag,
			bool hasAddedLevelFlag,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.ySpeedInMibipixelsPerSecond = ySpeedInMibipixelsPerSecond;
			this.angularSpeedInAnglesScaledPerSecond = angularSpeedInAnglesScaledPerSecond;
			this.angleScaled = angleScaled;
			this.eliteFlyamanitaIsDeadLevelFlag = eliteFlyamanitaIsDeadLevelFlag;
			this.hasAddedLevelFlag = hasAddedLevelFlag;
			this.EnemyId = enemyId;
		}

		public static EnemyEliteFlyamanitaDead SpawnEnemyEliteFlyamanitaDead(
			int xMibi,
			int yMibi,
			int angularSpeedInAnglesScaledPerSecond,
			string eliteFlyamanitaIsDeadLevelFlag,
			string enemyId)
		{
			return new EnemyEliteFlyamanitaDead(
				xMibi: xMibi,
				yMibi: yMibi,
				ySpeedInMibipixelsPerSecond: 0,
				angularSpeedInAnglesScaledPerSecond: angularSpeedInAnglesScaledPerSecond,
				angleScaled: 0,
				eliteFlyamanitaIsDeadLevelFlag: eliteFlyamanitaIsDeadLevelFlag,
				hasAddedLevelFlag: false,
				enemyId: enemyId);
		}

		public IEnemy GetDeadEnemy()
		{
			return null;
		}

		public IReadOnlyList<Hitbox> GetHitboxes()
		{
			return null;
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
			List<string> newlyAddedLevelFlags;

			if (this.hasAddedLevelFlag)
				newlyAddedLevelFlags = null;
			else
				newlyAddedLevelFlags = new List<string>() { this.eliteFlyamanitaIsDeadLevelFlag };

			bool isOutOfBounds = (this.xMibi >> 10) + 10 * 9 < cameraX - (windowWidth >> 1) - GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (this.xMibi >> 10) - 10 * 9 > cameraX + (windowWidth >> 1) + GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (this.yMibi >> 10) + 10 * 9 < cameraY - (windowHeight >> 1) - GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (this.yMibi >> 10) - 10 * 9 > cameraY + (windowHeight >> 1) + GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS;

			if (isOutOfBounds)
				return new EnemyProcessing.Result(
					enemiesImmutableNullable: null,
					newlyKilledEnemiesImmutableNullable: null,
					newlyAddedLevelFlagsImmutableNullable: newlyAddedLevelFlags);

			int newYSpeedInMibipixelsPerSecond = this.ySpeedInMibipixelsPerSecond;
			if (newYSpeedInMibipixelsPerSecond >= -5000 * 1000)
				newYSpeedInMibipixelsPerSecond -= elapsedMicrosPerFrame * 3;

			int newAngleScaled = this.angleScaled;
			newAngleScaled += this.angularSpeedInAnglesScaledPerSecond / 1000 * elapsedMicrosPerFrame / 1000;
			while (newAngleScaled >= 360 * 128)
				newAngleScaled -= 360 * 128;
			while (newAngleScaled < 0)
				newAngleScaled += 360 * 128;

			int newYMibi = (int)(((long)this.yMibi) + ((long)newYSpeedInMibipixelsPerSecond) * ((long)elapsedMicrosPerFrame) / 1000L / 1000L);

			return new EnemyProcessing.Result(
				enemiesImmutableNullable: new List<IEnemy>()
				{
					new EnemyEliteFlyamanitaDead(
						xMibi: this.xMibi,
						yMibi: newYMibi,
						ySpeedInMibipixelsPerSecond: newYSpeedInMibipixelsPerSecond,
						angularSpeedInAnglesScaledPerSecond: this.angularSpeedInAnglesScaledPerSecond,
						angleScaled: newAngleScaled,
						eliteFlyamanitaIsDeadLevelFlag: this.eliteFlyamanitaIsDeadLevelFlag,
						hasAddedLevelFlag: true,
						enemyId: this.EnemyId)
				},
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: newlyAddedLevelFlags);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			displayOutput.DrawImageRotatedClockwise(
				image: GameImage.Flyamanita,
				imageX: 0,
				imageY: 0,
				imageWidth: 20,
				imageHeight: 20,
				x: (this.xMibi >> 10) - 10 * 9,
				y: (this.yMibi >> 10) - 10 * 9,
				degreesScaled: this.angleScaled,
				scalingFactorScaled: 128 * 9);
		}
	}
}
