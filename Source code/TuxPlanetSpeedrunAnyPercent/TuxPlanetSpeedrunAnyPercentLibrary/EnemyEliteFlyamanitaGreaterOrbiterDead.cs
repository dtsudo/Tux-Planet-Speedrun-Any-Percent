
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyEliteFlyamanitaGreaterOrbiterDead : IEnemy
	{
		private int xMibi;
		private int yMibi;

		private int ySpeedInMibipixelsPerSecond;

		private int angularSpeedInAnglesScaledPerSecond;
		private int angleScaled;

		private bool isSpikes;

		private string eliteFlyamanitaGreaterOrbiterIsDeadLevelFlag;

		private List<string> emptyStringList;
		private List<Hitbox> emptyHitboxList;

		public string EnemyId { get; private set; }

		private EnemyEliteFlyamanitaGreaterOrbiterDead(
			int xMibi,
			int yMibi,
			int ySpeedInMibipixelsPerSecond,
			int angularSpeedInAnglesScaledPerSecond,
			int angleScaled,
			bool isSpikes,
			string eliteFlyamanitaGreaterOrbiterIsDeadLevelFlag,
			List<string> emptyStringList,
			List<Hitbox> emptyHitboxList,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.ySpeedInMibipixelsPerSecond = ySpeedInMibipixelsPerSecond;
			this.angularSpeedInAnglesScaledPerSecond = angularSpeedInAnglesScaledPerSecond;
			this.angleScaled = angleScaled;
			this.isSpikes = isSpikes;
			this.eliteFlyamanitaGreaterOrbiterIsDeadLevelFlag = eliteFlyamanitaGreaterOrbiterIsDeadLevelFlag;
			this.emptyStringList = emptyStringList;
			this.emptyHitboxList = emptyHitboxList;
			this.EnemyId = enemyId;
		}

		public static EnemyEliteFlyamanitaGreaterOrbiterDead SpawnEnemyEliteFlyamanitaGreaterOrbiterDead(
			int xMibi,
			int yMibi,
			int angularSpeedInAnglesScaledPerSecond,
			bool isSpikes,
			string eliteFlyamanitaGreaterOrbiterIsDeadLevelFlag,
			string enemyId)
		{
			return new EnemyEliteFlyamanitaGreaterOrbiterDead(
				xMibi: xMibi,
				yMibi: yMibi,
				ySpeedInMibipixelsPerSecond: 0,
				angularSpeedInAnglesScaledPerSecond: angularSpeedInAnglesScaledPerSecond,
				angleScaled: 0,
				isSpikes: isSpikes,
				eliteFlyamanitaGreaterOrbiterIsDeadLevelFlag: eliteFlyamanitaGreaterOrbiterIsDeadLevelFlag,
				emptyStringList: new List<string>(),
				emptyHitboxList: new List<Hitbox>(),
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
			if (this.isSpikes)
			{
				return new List<Hitbox>()
				{
					new Hitbox(
						x: (this.xMibi >> 10) - 6 * 6,
						y: (this.yMibi >> 10) - 6 * 6,
						width: 12 * 6,
						height: 12 * 6)
				};
			}

			return this.emptyHitboxList;
		}

		public IReadOnlyList<Hitbox> GetDamageBoxes()
		{
			return this.emptyHitboxList;
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
			List<string> newlyAddedLevelFlags = new List<string>() { this.eliteFlyamanitaGreaterOrbiterIsDeadLevelFlag };

			bool isOutOfBounds = (this.xMibi >> 10) + 10 * 6 < cameraX - (windowWidth >> 1) - GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (this.xMibi >> 10) - 10 * 6 > cameraX + (windowWidth >> 1) + GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (this.yMibi >> 10) + 10 * 6 < cameraY - (windowHeight >> 1) - GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (this.yMibi >> 10) - 10 * 6 > cameraY + (windowHeight >> 1) + GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS;

			if (isOutOfBounds)
				return new EnemyProcessing.Result(
					enemies: new List<IEnemy>(),
					newlyKilledEnemies: this.emptyStringList,
					newlyAddedLevelFlags: newlyAddedLevelFlags);

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
				enemies: new List<IEnemy>()
				{
					new EnemyEliteFlyamanitaGreaterOrbiterDead(
						xMibi: this.xMibi,
						yMibi: newYMibi,
						ySpeedInMibipixelsPerSecond: newYSpeedInMibipixelsPerSecond,
						angularSpeedInAnglesScaledPerSecond: this.angularSpeedInAnglesScaledPerSecond,
						angleScaled: newAngleScaled,
						isSpikes: this.isSpikes,
						eliteFlyamanitaGreaterOrbiterIsDeadLevelFlag: this.eliteFlyamanitaGreaterOrbiterIsDeadLevelFlag,
						emptyStringList: this.emptyStringList,
						emptyHitboxList: this.emptyHitboxList,
						enemyId: this.EnemyId)
				},
				newlyKilledEnemies: this.emptyStringList,
				newlyAddedLevelFlags: newlyAddedLevelFlags);
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
					x: (this.xMibi >> 10) - 8 * 6,
					y: (this.yMibi >> 10) - 8 * 6,
					degreesScaled: 0,
					scalingFactorScaled: 128 * 6);
			else
				displayOutput.DrawImageRotatedClockwise(
					image: GameImage.Flyamanita,
					imageX: 0,
					imageY: 0,
					imageWidth: 20,
					imageHeight: 20,
					x: (this.xMibi >> 10) - 10 * 6,
					y: (this.yMibi >> 10) - 10 * 6,
					degreesScaled: this.angleScaled,
					scalingFactorScaled: 128 * 6);
		}

		public IEnemy GetDeadEnemy()
		{
			return null;
		}
	}
}
