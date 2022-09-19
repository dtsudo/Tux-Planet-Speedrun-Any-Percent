
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class EnemyYetiBoss_Recoil : IEnemy
	{
		private int xMibi;
		private int yMibi;
		private int elapsedMicros;

		private int xSpeedInMibipixelsPerSecond;
		private int ySpeedInMibipixelsPerSecond;

		private bool isFacingRight;

		private int enemyIdCounter;

		private int numTimesHit;
		private string rngSeed;

		private List<string> emptyStringList;
		private List<Hitbox> emptyHitboxList;

		public string EnemyId { get; private set; }

		private EnemyYetiBoss_Recoil(
			int xMibi,
			int yMibi,
			int elapsedMicros,
			int xSpeedInMibipixelsPerSecond,
			int ySpeedInMibipixelsPerSecond,
			bool isFacingRight,
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
			this.xSpeedInMibipixelsPerSecond = xSpeedInMibipixelsPerSecond;
			this.ySpeedInMibipixelsPerSecond = ySpeedInMibipixelsPerSecond;
			this.isFacingRight = isFacingRight;
			this.enemyIdCounter = enemyIdCounter;
			this.numTimesHit = numTimesHit;
			this.rngSeed = rngSeed;
			this.emptyStringList = emptyStringList;
			this.emptyHitboxList = emptyHitboxList;
			this.EnemyId = enemyId;
		}

		public static EnemyYetiBoss_Recoil GetEnemyYetiBoss_Recoil(
			int xMibi,
			int yMibi,
			int elapsedMicros,
			bool isFacingRight,
			int enemyIdCounter,
			int numTimesHit,
			string rngSeed,
			string enemyId)
		{
			return new EnemyYetiBoss_Recoil(
				xMibi: xMibi,
				yMibi: yMibi,
				elapsedMicros: elapsedMicros,
				xSpeedInMibipixelsPerSecond: 400 * 1000 * (isFacingRight ? -1 : 1),
				ySpeedInMibipixelsPerSecond: 700 * 1000,
				isFacingRight: isFacingRight,
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
			return this.emptyHitboxList;
		}

		public IReadOnlyList<Hitbox> GetDamageBoxes()
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

		public IEnemy GetDeadEnemy()
		{
			return EnemyYetiBoss_Hit.GetEnemyYetiBoss_Hit(
				xMibi: this.xMibi,
				yMibi: this.yMibi,
				elapsedMicros: 0,
				isFacingRight: this.isFacingRight,
				enemyIdCounter: this.enemyIdCounter,
				numTimesHit: this.numTimesHit + 1,
				rngSeed: this.rngSeed,
				enemyId: this.EnemyId + "_hit");
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

			int newXMibi = this.xMibi;
			int newYMibi = this.yMibi;

			int newXSpeedInMibipixelsPerSecond = this.xSpeedInMibipixelsPerSecond;
			int newYSpeedInMibipixelsPerSecond = this.ySpeedInMibipixelsPerSecond;

			bool isOnGround = tilemap.IsGround(newXMibi >> 10, (newYMibi >> 10) - 32 * 3 - 1) && this.ySpeedInMibipixelsPerSecond == 0;

			if (isOnGround)
			{
				return new EnemyProcessing.Result(
					enemies: new List<IEnemy>()
					{
						EnemyYetiBoss_Stunned.GetEnemyYetiBoss_Stunned(
							xMibi: this.xMibi,
							yMibi: this.yMibi,
							elapsedMicros: 0,
							isFacingRight: this.isFacingRight,
							enemyIdCounter: this.enemyIdCounter,
							numTimesHit: this.numTimesHit,
							rngSeed: this.rngSeed,
							enemyId: this.EnemyId)
					},
					newlyKilledEnemies: this.emptyStringList,
					newlyAddedLevelFlags: null);
			}
			
			newYSpeedInMibipixelsPerSecond -= elapsedMicrosPerFrame * 3;
			
			int proposedNewYMibi = (int)(((long)newYMibi) + ((long)newYSpeedInMibipixelsPerSecond) * ((long)elapsedMicrosPerFrame) / 1000L / 1000L);
			if (newYSpeedInMibipixelsPerSecond < 0)
			{
				while (true)
				{
					if (tilemap.IsGround(newXMibi >> 10, (proposedNewYMibi >> 10) - 32 * 3))
					{
						newYSpeedInMibipixelsPerSecond = 0;
						proposedNewYMibi += 1024;
						if (proposedNewYMibi >= newYMibi)
						{
							proposedNewYMibi = newYMibi;
							break;
						}
					}
					else
						break;
				}
			}

			newYMibi = proposedNewYMibi;

			newXMibi = (int)(((long)newXMibi) + ((long)newXSpeedInMibipixelsPerSecond) * ((long)elapsedMicrosPerFrame) / 1000L / 1000L);
		
			newEnemies.Add(new EnemyYetiBoss_Recoil(
				xMibi: newXMibi,
				yMibi: newYMibi,
				elapsedMicros: newElapsedMicros,
				xSpeedInMibipixelsPerSecond: newXSpeedInMibipixelsPerSecond,
				ySpeedInMibipixelsPerSecond: newYSpeedInMibipixelsPerSecond,
				isFacingRight: this.isFacingRight,
				enemyIdCounter: this.enemyIdCounter,
				numTimesHit: this.numTimesHit,
				rngSeed: this.rngSeed,
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
			int spriteNum = (this.elapsedMicros / (100 * 1000)) % 2;

			displayOutput.DrawImageRotatedClockwise(
				image: this.isFacingRight ? GameImage.Yeti : GameImage.YetiMirrored,
				imageX: spriteNum * 64,
				imageY: 4 * 64,
				imageWidth: 64,
				imageHeight: 64,
				x: (this.xMibi >> 10) - 32 * 3,
				y: (this.yMibi >> 10) - 32 * 3,
				degreesScaled: 0,
				scalingFactorScaled: 128 * 3);
		}
	}
}
