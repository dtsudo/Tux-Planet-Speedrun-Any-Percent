
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyLevel8Fish : IEnemy
	{
		private int xMibi;
		private int yMibi;

		private int xSpeedInMibipixelsPerSecond;
		private int ySpeedInMibipixelsPerSecond;

		private int elapsedMicros;

		private int changeYSpeedCooldown;

		private GameImage fishImage;
		private GameImage fishImageMirrored;

		private List<string> emptyStringList;
		private List<Hitbox> emptyHitboxList;

		public string EnemyId { get; private set; }

		private EnemyLevel8Fish(
			int xMibi,
			int yMibi,
			int xSpeedInMibipixelsPerSecond,
			int ySpeedInMibipixelsPerSecond,
			int elapsedMicros,
			int changeYSpeedCooldown,
			GameImage fishImage,
			GameImage fishImageMirrored,
			List<string> emptyStringList,
			List<Hitbox> emptyHitboxList,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.xSpeedInMibipixelsPerSecond = xSpeedInMibipixelsPerSecond;
			this.ySpeedInMibipixelsPerSecond = ySpeedInMibipixelsPerSecond;
			this.elapsedMicros = elapsedMicros;
			this.changeYSpeedCooldown = changeYSpeedCooldown;
			this.fishImage = fishImage;
			this.fishImageMirrored = fishImageMirrored;
			this.emptyStringList = emptyStringList;
			this.emptyHitboxList = emptyHitboxList;
			this.EnemyId = enemyId;
		}

		public static EnemyLevel8Fish SpawnLevel8Fish(
			int x,
			int y,
			bool isFacingRight,
			GameImage fishImage,
			GameImage fishImageMirrored,
			string enemyId)
		{
			int xSpeed = 100 * 1024;

			return new EnemyLevel8Fish(
				xMibi: x << 10,
				yMibi: y << 10,
				xSpeedInMibipixelsPerSecond: isFacingRight ? xSpeed : -xSpeed,
				ySpeedInMibipixelsPerSecond: 0,
				elapsedMicros: 0,
				changeYSpeedCooldown: 0,
				fishImage: fishImage,
				fishImageMirrored: fishImageMirrored,
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
			return this.emptyHitboxList;
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

			int newXMibi = this.xMibi;
			int newYMibi = this.yMibi;
			int newXSpeedInMibipixelsPerSecond = this.xSpeedInMibipixelsPerSecond;
			int newYSpeedInMibipixelsPerSecond = this.ySpeedInMibipixelsPerSecond;

			int newChangeYSpeedCooldown = this.changeYSpeedCooldown - elapsedMicrosPerFrame;

			if (newChangeYSpeedCooldown < 0)
			{
				newYSpeedInMibipixelsPerSecond = newYSpeedInMibipixelsPerSecond + random.NextInt(40 * 1024) - 20 * 1024;
				newChangeYSpeedCooldown = 100 * 1000 + random.NextInt(1000 * 1000);

				if (newYSpeedInMibipixelsPerSecond > 50 * 1024)
					newYSpeedInMibipixelsPerSecond = 50 * 1024;
				if (newYSpeedInMibipixelsPerSecond < -50 * 1024)
					newYSpeedInMibipixelsPerSecond = -50 * 1024;
			}

			newXMibi = (int)(((long)newXMibi) + ((long)this.xSpeedInMibipixelsPerSecond) * ((long)elapsedMicrosPerFrame) / 1000L / 1000L);
			newYMibi = (int)(((long)newYMibi) + ((long)this.ySpeedInMibipixelsPerSecond) * ((long)elapsedMicrosPerFrame) / 1000L / 1000L);

			if (newXMibi > 80 * 48 * 1024)
				newXMibi = -3 * 48 * 1024;
			if (newXMibi < -5 * 48 * 1024)
				newXMibi = 78 * 48 * 1024;

			if (newYMibi > 7 * 48 * 1024)
			{
				newYSpeedInMibipixelsPerSecond = newYSpeedInMibipixelsPerSecond - 20 * 1024;
				newYMibi = 7 * 48 * 1024;
			}

			if (newYMibi < -1 * 48 * 1024)
			{
				newYSpeedInMibipixelsPerSecond = newYSpeedInMibipixelsPerSecond + 20 * 1024;
				newYMibi = -1 * 48 * 1024;
			}

			return new EnemyProcessing.Result(
				enemies: new List<IEnemy>()
				{
					new EnemyLevel8Fish(
						xMibi: newXMibi,
						yMibi: newYMibi,
						xSpeedInMibipixelsPerSecond: newXSpeedInMibipixelsPerSecond,
						ySpeedInMibipixelsPerSecond: newYSpeedInMibipixelsPerSecond,
						elapsedMicros: newElapsedMicros,
						changeYSpeedCooldown: newChangeYSpeedCooldown,
						fishImage: this.fishImage,
						fishImageMirrored: this.fishImageMirrored,
						emptyStringList: this.emptyStringList,
						emptyHitboxList: this.emptyHitboxList,
						enemyId: this.EnemyId)
				},
				newlyKilledEnemies: this.emptyStringList,
				newlyAddedLevelFlags: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			int spriteNum = (this.elapsedMicros / (100 * 1000)) % 4;

			bool isFacingRight = this.xSpeedInMibipixelsPerSecond > 0;

			displayOutput.DrawImageRotatedClockwise(
				image: isFacingRight ? this.fishImage : this.fishImageMirrored,
				imageX: spriteNum * 28,
				imageY: 0,
				imageWidth: 28,
				imageHeight: 20,
				x: (this.xMibi >> 10) - 14 * 3,
				y: (this.yMibi >> 10) - 10 * 3,
				degreesScaled: 0,
				scalingFactorScaled: 128 * 3);
		}
	}
}
