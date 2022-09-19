
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class EnemyBossDoor : IEnemy
	{
		private int x;
		private int y;
		private int elapsedMicrosClosing;
		private int elapsedMicrosOpening;

		private bool isUpperDoor;

		private const int DOOR_ANIMATION_DURATION = 500 * 1000;

		private List<string> emptyStringList;
		private List<Hitbox> emptyHitboxList;

		public const string LEVEL_FLAG_CLOSE_BOSS_DOORS = "closeBossDoors";
		public const string LEVEL_FLAG_CLOSE_BOSS_DOORS_INSTANTLY = "closeBossDoorsInstantly";
		public const string LEVEL_FLAG_OPEN_BOSS_DOORS = "openBossDoors";

		public string EnemyId { get; private set; }

		private EnemyBossDoor(
			int x,
			int y,
			int elapsedMicrosClosing,
			int elapsedMicrosOpening,
			bool isUpperDoor,
			List<string> emptyStringList,
			List<Hitbox> emptyHitboxList,
			string enemyId)
		{
			this.x = x;
			this.y = y;
			this.elapsedMicrosClosing = elapsedMicrosClosing;
			this.elapsedMicrosOpening = elapsedMicrosOpening;
			this.isUpperDoor = isUpperDoor;
			this.emptyStringList = emptyStringList;
			this.emptyHitboxList = emptyHitboxList;
			this.EnemyId = enemyId;
		}

		public static EnemyBossDoor GetEnemyBossDoor(
			int xMibi,
			int yMibi,
			bool isUpperDoor,
			string enemyId)
		{
			return new EnemyBossDoor(
				x: xMibi >> 10,
				y: yMibi >> 10,
				elapsedMicrosClosing: 0,
				elapsedMicrosOpening: 0,
				isUpperDoor: isUpperDoor,
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
			int newElapsedMicrosClosing = this.elapsedMicrosClosing;
			int newElapsedMicrosOpening = this.elapsedMicrosOpening;

			if (newElapsedMicrosClosing <= DOOR_ANIMATION_DURATION)
			{
				if (levelFlags.Contains(LEVEL_FLAG_CLOSE_BOSS_DOORS_INSTANTLY))
					newElapsedMicrosClosing = DOOR_ANIMATION_DURATION + 1;
				else if (levelFlags.Contains(LEVEL_FLAG_CLOSE_BOSS_DOORS))
					newElapsedMicrosClosing += elapsedMicrosPerFrame;
			}

			if (newElapsedMicrosOpening <= DOOR_ANIMATION_DURATION)
			{
				if (levelFlags.Contains(LEVEL_FLAG_OPEN_BOSS_DOORS))
					newElapsedMicrosOpening += elapsedMicrosPerFrame;
			}

			if (newElapsedMicrosOpening >= DOOR_ANIMATION_DURATION)
			{
				return new EnemyProcessing.Result(
					enemies: new List<IEnemy>(),
					newlyKilledEnemies: new List<string>() { this.EnemyId },
					newlyAddedLevelFlags: null);
			}

			return new EnemyProcessing.Result(
				enemies: new List<IEnemy>()
				{
					new EnemyBossDoor(
						x: this.x,
						y: this.y,
						elapsedMicrosClosing: newElapsedMicrosClosing,
						elapsedMicrosOpening: newElapsedMicrosOpening,
						isUpperDoor: this.isUpperDoor,
						emptyStringList: this.emptyStringList,
						emptyHitboxList: this.emptyHitboxList,
						enemyId: this.EnemyId)
				},
				newlyKilledEnemies: this.emptyStringList,
				newlyAddedLevelFlags: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			int y;

			if (this.elapsedMicrosClosing < DOOR_ANIMATION_DURATION)
			{
				if (this.isUpperDoor)
					y = this.y + 32 * 3 - 32 * 3 * this.elapsedMicrosClosing / DOOR_ANIMATION_DURATION;
				else
					y = this.y - 32 * 3 + 32 * 3 * this.elapsedMicrosClosing / DOOR_ANIMATION_DURATION;
			}
			else if (this.elapsedMicrosOpening < DOOR_ANIMATION_DURATION)
			{
				if (this.isUpperDoor)
					y = this.y + 32 * 3 * this.elapsedMicrosOpening / DOOR_ANIMATION_DURATION;
				else
					y = this.y - 32 * 3 * this.elapsedMicrosOpening / DOOR_ANIMATION_DURATION;
			}
			else
			{
				y = this.y;
			}

			displayOutput.DrawImageRotatedClockwise(
				image: GameImage.BossDoor,
				x: this.x,
				y: y,
				degreesScaled: 0,
				scalingFactorScaled: 128 * 3);
		}

		public IEnemy GetDeadEnemy()
		{
			return null;
		}
	}
}
