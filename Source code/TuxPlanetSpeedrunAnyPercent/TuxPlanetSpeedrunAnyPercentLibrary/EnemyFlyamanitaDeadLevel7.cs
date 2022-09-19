
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyFlyamanitaDeadLevel7 : IEnemy
	{
		private int originalXMibi;
		private int originalYMibi;
		private int xMibi;
		private int yMibi;

		private int ySpeedInMibipixelsPerSecond;

		private int angularSpeedInAnglesScaledPerSecond;
		private int angleScaled;

		private List<string> emptyStringList;
		private List<Hitbox> emptyHitboxList;

		private bool hasLeftScreen;

		private string enemyIdPrefix;
		private int enemyIdCounter;

		public string EnemyId { get; private set; }

		private const int NUM_PIXELS_OFFSCREEN_BEFORE_ENEMY_RESPAWNS = 400;

		private EnemyFlyamanitaDeadLevel7(
			int originalXMibi,
			int originalYMibi,
			int xMibi,
			int yMibi,
			int ySpeedInMibipixelsPerSecond,
			int angularSpeedInAnglesScaledPerSecond,
			int angleScaled,
			List<string> emptyStringList,
			List<Hitbox> emptyHitboxList,
			bool hasLeftScreen,
			string enemyIdPrefix,
			int enemyIdCounter,
			string enemyId)
		{
			this.originalXMibi = originalXMibi;
			this.originalYMibi = originalYMibi;
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.ySpeedInMibipixelsPerSecond = ySpeedInMibipixelsPerSecond;
			this.angularSpeedInAnglesScaledPerSecond = angularSpeedInAnglesScaledPerSecond;
			this.angleScaled = angleScaled;
			this.emptyStringList = emptyStringList;
			this.emptyHitboxList = emptyHitboxList;
			this.hasLeftScreen = hasLeftScreen;
			this.enemyIdPrefix = enemyIdPrefix;
			this.enemyIdCounter = enemyIdCounter;
			this.EnemyId = enemyId;
		}

		public static EnemyFlyamanitaDeadLevel7 SpawnEnemyFlyamanitaDeadLevel7(
			int originalXMibi,
			int originalYMibi,
			int xMibi,
			int yMibi,
			int angularSpeedInAnglesScaledPerSecond,
			string enemyIdPrefix,
			int enemyIdCounter)
		{
			return new EnemyFlyamanitaDeadLevel7(
				originalXMibi: originalXMibi,
				originalYMibi: originalYMibi,
				xMibi: xMibi,
				yMibi: yMibi,
				ySpeedInMibipixelsPerSecond: 0,
				angularSpeedInAnglesScaledPerSecond: angularSpeedInAnglesScaledPerSecond,
				angleScaled: 0,
				emptyStringList: new List<string>(),
				emptyHitboxList: new List<Hitbox>(),
				hasLeftScreen: false,
				enemyIdPrefix: enemyIdPrefix,
				enemyIdCounter: enemyIdCounter + 1,
				enemyId: enemyIdPrefix + "_" + enemyIdCounter.ToStringCultureInvariant());
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
			if (this.hasLeftScreen)
			{
				bool shouldRespawn = (this.originalXMibi >> 10) + 10 * 3 < cameraX - (windowWidth >> 1) - NUM_PIXELS_OFFSCREEN_BEFORE_ENEMY_RESPAWNS
					|| (this.originalXMibi >> 10) - 10 * 3 > cameraX + (windowWidth >> 1) + NUM_PIXELS_OFFSCREEN_BEFORE_ENEMY_RESPAWNS
					|| (this.originalYMibi >> 10) + 10 * 3 < cameraY - (windowHeight >> 1) - NUM_PIXELS_OFFSCREEN_BEFORE_ENEMY_RESPAWNS
					|| (this.originalYMibi >> 10) - 10 * 3 > cameraY + (windowHeight >> 1) + NUM_PIXELS_OFFSCREEN_BEFORE_ENEMY_RESPAWNS;

				List<IEnemy> list = new List<IEnemy>();

				if (shouldRespawn)
					list.Add(EnemyFlyamanitaLevel7.GetEnemyFlyamanitaLevel7(
						xMibi: this.originalXMibi,
						yMibi: this.originalYMibi,
						enemyIdPrefix: this.enemyIdPrefix,
						enemyIdCounter: this.enemyIdCounter));
				else
					list.Add(this);

				return new EnemyProcessing.Result(
					enemies: list,
					newlyKilledEnemies: this.emptyStringList,
					newlyAddedLevelFlags: null);
			}
			else
			{
				bool isOutOfBounds = (this.xMibi >> 10) + 10 * 3 < cameraX - (windowWidth >> 1) - GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
					|| (this.xMibi >> 10) - 10 * 3 > cameraX + (windowWidth >> 1) + GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
					|| (this.yMibi >> 10) + 10 * 3 < cameraY - (windowHeight >> 1) - GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
					|| (this.yMibi >> 10) - 10 * 3 > cameraY + (windowHeight >> 1) + GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS;

				bool newHasLeftScreen = isOutOfBounds;

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
						new EnemyFlyamanitaDeadLevel7(
							originalXMibi: this.originalXMibi,
							originalYMibi: this.originalYMibi,
							xMibi: this.xMibi,
							yMibi: newYMibi,
							ySpeedInMibipixelsPerSecond: newYSpeedInMibipixelsPerSecond,
							angularSpeedInAnglesScaledPerSecond: this.angularSpeedInAnglesScaledPerSecond,
							angleScaled: newAngleScaled,
							emptyStringList: this.emptyStringList,
							emptyHitboxList: this.emptyHitboxList,
							hasLeftScreen: newHasLeftScreen,
							enemyIdPrefix: this.enemyIdPrefix,
							enemyIdCounter: this.enemyIdCounter,
							enemyId: this.EnemyId)
					},
					newlyKilledEnemies: this.emptyStringList,
					newlyAddedLevelFlags: null);
			}
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			if (!this.hasLeftScreen)
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
