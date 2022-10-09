
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyEliteFlyamanitaLesserOrbiterDead : IEnemy
	{
		private int xMibi;
		private int yMibi;

		private int ySpeedInMibipixelsPerSecond;

		private int angularSpeedInAnglesScaledPerSecond;
		private int angleScaled;

		private bool isSpikes;

		public string EnemyId { get; private set; }

		private EnemyEliteFlyamanitaLesserOrbiterDead(
			int xMibi,
			int yMibi,
			int ySpeedInMibipixelsPerSecond,
			int angularSpeedInAnglesScaledPerSecond,
			int angleScaled,
			bool isSpikes,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.ySpeedInMibipixelsPerSecond = ySpeedInMibipixelsPerSecond;
			this.angularSpeedInAnglesScaledPerSecond = angularSpeedInAnglesScaledPerSecond;
			this.angleScaled = angleScaled;
			this.isSpikes = isSpikes;
			this.EnemyId = enemyId;
		}

		public static EnemyEliteFlyamanitaLesserOrbiterDead SpawnEnemyEliteFlyamanitaLesserOrbiterDead(
			int xMibi,
			int yMibi,
			int angularSpeedInAnglesScaledPerSecond,
			bool isSpikes,
			string enemyId)
		{
			return new EnemyEliteFlyamanitaLesserOrbiterDead(
				xMibi: xMibi,
				yMibi: yMibi,
				ySpeedInMibipixelsPerSecond: 0,
				angularSpeedInAnglesScaledPerSecond: angularSpeedInAnglesScaledPerSecond,
				angleScaled: 0,
				isSpikes: isSpikes,
				enemyId: enemyId);
		}

		public IReadOnlyList<Hitbox> GetHitboxes()
		{
			if (this.isSpikes)
			{
				List<Hitbox> list = new List<Hitbox>();
				list.Add(new Hitbox(
					x: (this.xMibi >> 10) - 6 * 3,
					y: (this.yMibi >> 10) - 6 * 3,
					width: 12 * 3,
					height: 12 * 3));

				return list;
			}

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
			bool isOutOfBounds = (this.xMibi >> 10) + 10 * 3 < cameraX - (windowWidth >> 1) - GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (this.xMibi >> 10) - 10 * 3 > cameraX + (windowWidth >> 1) + GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (this.yMibi >> 10) + 10 * 3 < cameraY - (windowHeight >> 1) - GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (this.yMibi >> 10) - 10 * 3 > cameraY + (windowHeight >> 1) + GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS;

			if (isOutOfBounds)
				return new EnemyProcessing.Result(
					enemiesImmutableNullable: null,
					newlyKilledEnemiesImmutableNullable: null,
					newlyAddedLevelFlagsImmutableNullable: null);

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
					new EnemyEliteFlyamanitaLesserOrbiterDead(
						xMibi: this.xMibi,
						yMibi: newYMibi,
						ySpeedInMibipixelsPerSecond: newYSpeedInMibipixelsPerSecond,
						angularSpeedInAnglesScaledPerSecond: this.angularSpeedInAnglesScaledPerSecond,
						angleScaled: newAngleScaled,
						isSpikes: this.isSpikes,
						enemyId: this.EnemyId)
				},
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			if (this.isSpikes)
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
			else
				displayOutput.DrawImageRotatedClockwise(
					image: GameImage.Flyamanita,
					imageX: 0,
					imageY: 0,
					imageWidth: 20,
					imageHeight: 20,
					x: (this.xMibi >> 10) - 10 * 3,
					y: (this.yMibi >> 10) - 10 * 3,
					degreesScaled: this.angleScaled,
					scalingFactorScaled: 128 * 3);
		}

		public IEnemy GetDeadEnemy()
		{
			return null;
		}
	}
}
