
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyKonqiFireballBlue : IEnemy
	{
		private int xMibi;
		private int yMibi;
		private int centerXMibi;
		private int centerYMibi;
		private int elapsedMicros;

		private int radiusInMibipixels;
		private int angleScaled;
		private bool isRotatingClockwise;

		private const int ANGULAR_SPEED_IN_ANGLE_SCALED_PER_SECOND = 120 * 128;
		private const int RADIUS_INCREASE_IN_MIBIPIXELS_PER_SECOND = 300 * 1024;

		public string EnemyId { get; private set; }

		public const string LEVEL_FLAG_DESPAWN_KONQI_FIREBALLS_BLUE = "despawnKonqiFireballsBlue";

		private EnemyKonqiFireballBlue(
			int xMibi,
			int yMibi,
			int centerXMibi,
			int centerYMibi,
			int elapsedMicros,
			int radiusInMibipixels,
			int angleScaled,
			bool isRotatingClockwise,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.centerXMibi = centerXMibi;
			this.centerYMibi = centerYMibi;
			this.elapsedMicros = elapsedMicros;
			this.radiusInMibipixels = radiusInMibipixels;
			this.angleScaled = angleScaled;
			this.isRotatingClockwise = isRotatingClockwise;
			this.EnemyId = enemyId;
		}

		public static EnemyKonqiFireballBlue GetEnemyKonqiFireballBlue(
			int xMibi,
			int yMibi,
			int angleScaled,
			bool isRotatingClockwise,
			string enemyId)
		{
			return new EnemyKonqiFireballBlue(
				xMibi: xMibi,
				yMibi: yMibi,
				centerXMibi: xMibi,
				centerYMibi: yMibi,
				elapsedMicros: 0,
				radiusInMibipixels: 0,
				angleScaled: DTMath.NormalizeDegreesScaled(angleScaled),
				isRotatingClockwise: isRotatingClockwise,
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
			return new List<Hitbox>()
			{
				new Hitbox(
					x: (this.xMibi >> 10) - 6 * 3,
					y: (this.yMibi >> 10) - 6 * 3,
					width: 12 * 3,
					height: 12 * 3)
			};
		}

		public IReadOnlyList<Hitbox> GetDamageBoxes()
		{
			return new List<Hitbox>();
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
			bool shouldDespawn = false;
			for (int i = 0; i < levelFlags.Count; i++)
			{
				if (levelFlags[i] == LEVEL_FLAG_DESPAWN_KONQI_FIREBALLS_BLUE)
				{
					shouldDespawn = true;
					break;
				}
			}
			if (shouldDespawn)
			{
				return new EnemyProcessing.Result(
					enemies: new List<IEnemy>()
					{
						EnemyDeadPoof.SpawnEnemyDeadPoof(
							xMibi: this.xMibi,
							yMibi: this.yMibi,
							enemyId: this.EnemyId + "_poof")
					},
					newlyKilledEnemies: new List<string>(),
					newlyAddedLevelFlags: null);
			}

			List<IEnemy> newEnemies = new List<IEnemy>();

			int newAngleScaled;
			if (this.isRotatingClockwise)
				newAngleScaled = this.angleScaled - ANGULAR_SPEED_IN_ANGLE_SCALED_PER_SECOND / 1000 * elapsedMicrosPerFrame / 1000;
			else
				newAngleScaled = this.angleScaled + ANGULAR_SPEED_IN_ANGLE_SCALED_PER_SECOND / 1000 * elapsedMicrosPerFrame / 1000;
			newAngleScaled = DTMath.NormalizeDegreesScaled(newAngleScaled);

			int newRadiusInMibipixels = this.radiusInMibipixels + RADIUS_INCREASE_IN_MIBIPIXELS_PER_SECOND / 10000 * elapsedMicrosPerFrame / 100;

			int newXMibi = this.centerXMibi + (newRadiusInMibipixels >> 10) * DTMath.CosineScaled(newAngleScaled);
			int newYMibi = this.centerYMibi + (newRadiusInMibipixels >> 10) * DTMath.SineScaled(newAngleScaled);

			if (newRadiusInMibipixels > 1250 * 1024)
			{
				return new EnemyProcessing.Result(
					enemies: new List<IEnemy>(),
					newlyKilledEnemies: new List<string>(),
					newlyAddedLevelFlags: null);
			}

			newEnemies.Add(new EnemyKonqiFireballBlue(
				xMibi: newXMibi,
				yMibi: newYMibi,
				centerXMibi: this.centerXMibi,
				centerYMibi: this.centerYMibi,
				elapsedMicros: this.elapsedMicros + elapsedMicrosPerFrame,
				radiusInMibipixels: newRadiusInMibipixels,
				angleScaled: newAngleScaled,
				isRotatingClockwise: this.isRotatingClockwise,
				enemyId: this.EnemyId));

			return new EnemyProcessing.Result(
				enemies: newEnemies,
				newlyKilledEnemies: new List<string>(),
				newlyAddedLevelFlags: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			int spriteNum = (this.elapsedMicros / (100 * 1000)) % 5;

			displayOutput.DrawImageRotatedClockwise(
				image: GameImage.FlameBlue,
				imageX: spriteNum * 14,
				imageY: 0,
				imageWidth: 14,
				imageHeight: 20,
				x: (this.xMibi >> 10) - 7 * 3,
				y: (this.yMibi >> 10) - 10 * 3,
				degreesScaled: this.isRotatingClockwise ? -this.angleScaled : (-this.angleScaled + 180 * 128),
				scalingFactorScaled: 128 * 3);
		}
	}
}
