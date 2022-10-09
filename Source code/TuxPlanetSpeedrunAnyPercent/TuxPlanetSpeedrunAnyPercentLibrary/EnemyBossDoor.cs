
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
			string enemyId)
		{
			this.x = x;
			this.y = y;
			this.elapsedMicrosClosing = elapsedMicrosClosing;
			this.elapsedMicrosOpening = elapsedMicrosOpening;
			this.isUpperDoor = isUpperDoor;
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
				enemyId: enemyId);
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
					enemiesImmutableNullable: null,
					newlyKilledEnemiesImmutableNullable: new List<string>() { this.EnemyId },
					newlyAddedLevelFlagsImmutableNullable: null);
			}

			return new EnemyProcessing.Result(
				enemiesImmutableNullable: new List<IEnemy>()
				{
					new EnemyBossDoor(
						x: this.x,
						y: this.y,
						elapsedMicrosClosing: newElapsedMicrosClosing,
						elapsedMicrosOpening: newElapsedMicrosOpening,
						isUpperDoor: this.isUpperDoor,
						enemyId: this.EnemyId)
				},
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			int numPixels;

			if (this.elapsedMicrosClosing < DOOR_ANIMATION_DURATION)
				numPixels = 32 * this.elapsedMicrosClosing / DOOR_ANIMATION_DURATION;
			else if (this.elapsedMicrosOpening < DOOR_ANIMATION_DURATION)
				numPixels = 32 - 32 * this.elapsedMicrosOpening / DOOR_ANIMATION_DURATION;
			else
				numPixels = 32;

			int imageY;
			int y;

			if (this.isUpperDoor)
			{
				imageY = 32 - numPixels;
				y = this.y + (32 - numPixels) * 3;
			}
			else
			{
				imageY = 0;
				y = this.y;
			}

			if (numPixels > 0)
				displayOutput.DrawImageRotatedClockwise(
					image: GameImage.BossDoor,
					imageX: 0,
					imageY: imageY,
					imageWidth: 16,
					imageHeight: numPixels,
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
