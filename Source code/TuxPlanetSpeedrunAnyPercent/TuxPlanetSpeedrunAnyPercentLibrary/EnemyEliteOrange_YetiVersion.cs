
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class EnemyEliteOrange_YetiVersion : IEnemy
	{
		private int xMibi;
		private int yMibi;

		private int xSpeedInMibipixelsPerSecond;
		private int ySpeedInMibipixelsPerSecond;

		private int jumpSpeed;

		private int orbitersAngleScaled;
		private int orbitersRadiusInMibipixels;
		private bool isOrbitingClockwise;
		private List<Tuple<int, int>> orbitersXAndYMibi;

		private int elapsedMicros;

		private const int ORBITERS_SPEED_IN_ANGLES_SCALED_PER_SECOND = 180 * 128;
		private const int ORBITERS_MAX_RADIUS_IN_PIXELS = 150;
		private const int ORBITERS_RADIUS_INCREASE_IN_MIBIPIXELS_PER_SECOND = 1024 * 100;

		public string EnemyId { get; private set; }

		public const string LEVEL_FLAG_DESPAWN_ENEMY_ELITE_ORANGE_YETI_VERSION = "despawnEnemyEliteOrangeYetiVersion";

		private EnemyEliteOrange_YetiVersion(
			int xMibi,
			int yMibi,
			int xSpeedInMibipixelsPerSecond,
			int ySpeedInMibipixelsPerSecond,
			int jumpSpeed,
			int orbitersAngleScaled,
			int orbitersRadiusInMibipixels,
			bool isOrbitingClockwise,
			List<Tuple<int, int>> orbitersXAndYMibi,
			int elapsedMicros,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.xSpeedInMibipixelsPerSecond = xSpeedInMibipixelsPerSecond;
			this.ySpeedInMibipixelsPerSecond = ySpeedInMibipixelsPerSecond;
			this.jumpSpeed = jumpSpeed;
			this.orbitersAngleScaled = orbitersAngleScaled;
			this.orbitersRadiusInMibipixels = orbitersRadiusInMibipixels;
			this.isOrbitingClockwise = isOrbitingClockwise;
			this.orbitersXAndYMibi = orbitersXAndYMibi;
			this.elapsedMicros = elapsedMicros;
			this.EnemyId = enemyId;
		}

		public static EnemyEliteOrange_YetiVersion GetEnemyEliteOrange_YetiVersion(
			int xMibi,
			int yMibi,
			int xSpeedInMibipixelsPerSecond,
			int jumpSpeed,
			int orbitersAngleScaled,
			bool isOrbitingClockwise,
			string enemyId)
		{
			return new EnemyEliteOrange_YetiVersion(
				xMibi: xMibi,
				yMibi: yMibi,
				xSpeedInMibipixelsPerSecond: xSpeedInMibipixelsPerSecond,
				ySpeedInMibipixelsPerSecond: 0,
				jumpSpeed: jumpSpeed,
				orbitersAngleScaled: orbitersAngleScaled,
				orbitersRadiusInMibipixels: 0,
				isOrbitingClockwise: isOrbitingClockwise,
				orbitersXAndYMibi: ComputeOrbitersXAndYMibi(xMibi: xMibi, yMibi: yMibi, orbitersAngleScaled: orbitersAngleScaled, orbitersRadiusInMibipixels: 0),
				elapsedMicros: 0,
				enemyId: enemyId);
		}

		private static List<Tuple<int, int>> ComputeOrbitersXAndYMibi(int xMibi, int yMibi, int orbitersAngleScaled, int orbitersRadiusInMibipixels)
		{
			orbitersAngleScaled = DTMath.NormalizeDegreesScaled(orbitersAngleScaled);

			List<Tuple<int, int>> list = new List<Tuple<int, int>>();

			int orbitersRadiusInPixels = orbitersRadiusInMibipixels >> 10;

			list.Add(new Tuple<int, int>(
				item1: xMibi + orbitersRadiusInPixels * DTMath.CosineScaled(degreesScaled: orbitersAngleScaled),
				item2: yMibi + orbitersRadiusInPixels * DTMath.SineScaled(degreesScaled: orbitersAngleScaled)));

			orbitersAngleScaled += 120 * 128;

			list.Add(new Tuple<int, int>(
				item1: xMibi + orbitersRadiusInPixels * DTMath.CosineScaled(degreesScaled: orbitersAngleScaled),
				item2: yMibi + orbitersRadiusInPixels * DTMath.SineScaled(degreesScaled: orbitersAngleScaled)));

			orbitersAngleScaled += 120 * 128;

			list.Add(new Tuple<int, int>(
				item1: xMibi + orbitersRadiusInPixels * DTMath.CosineScaled(degreesScaled: orbitersAngleScaled),
				item2: yMibi + orbitersRadiusInPixels * DTMath.SineScaled(degreesScaled: orbitersAngleScaled)));

			return list;
		}

		public IReadOnlyList<Hitbox> GetHitboxes()
		{
			List<Hitbox> list = new List<Hitbox>()
			{
				new Hitbox(
					x: (this.xMibi >> 10) - 6 * 3, 
					y: (this.yMibi >> 10) - 6 * 3, 
					width: 12 * 3, 
					height: 12 * 3)
			};

			for (int i = 0; i < this.orbitersXAndYMibi.Count; i++)
			{
				Tuple<int, int> orbiterXAndYMibi = this.orbitersXAndYMibi[i];

				list.Add(new Hitbox(
					x: (orbiterXAndYMibi.Item1 >> 10) - 6 * 3,
					y: (orbiterXAndYMibi.Item2 >> 10) - 6 * 3,
					width: 12 * 3,
					height: 12 * 3));
			}

			return list;
		}

		public IReadOnlyList<Hitbox> GetDamageBoxes()
		{
			return new List<Hitbox>()
			{
				new Hitbox(
					x: (this.xMibi >> 10) - 7 * 3,
					y: (this.yMibi >> 10) - 7 * 3,
					width: 14 * 3,
					height: 14 * 3)
			};
		}

		public IEnemy GetDeadEnemy()
		{
			List<EnemyDeadPoof> enemyDeadPoofList = new List<EnemyDeadPoof>();

			enemyDeadPoofList.Add(EnemyDeadPoof.SpawnEnemyDeadPoof(
				xMibi: this.xMibi,
				yMibi: this.yMibi,
				enemyId: this.EnemyId + "_enemyDeadPoof_main"));

			for (int i = 0; i < this.orbitersXAndYMibi.Count; i++)
			{
				Tuple<int, int> orbiterXAndYMibi = this.orbitersXAndYMibi[i];

				enemyDeadPoofList.Add(EnemyDeadPoof.SpawnEnemyDeadPoof(
					xMibi: orbiterXAndYMibi.Item1,
					yMibi: orbiterXAndYMibi.Item2,
					enemyId: this.EnemyId + "_enemyDeadPoof_orbiter" + i.ToStringCultureInvariant()));
			}

			return EnemyDeadMultiplePoof.SpawnEnemyDeadMultiplePoof(
				enemyDeadPoofList: enemyDeadPoofList,
				enemyId: this.EnemyId + "_enemyDeadMultiplePoof");
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
			if (levelFlags.Contains(LEVEL_FLAG_DESPAWN_ENEMY_ELITE_ORANGE_YETI_VERSION))
			{
				return new EnemyProcessing.Result(
					enemiesImmutableNullable: new List<IEnemy>()
					{
						this.GetDeadEnemy()
					},
					newlyKilledEnemiesImmutableNullable: null,
					newlyAddedLevelFlagsImmutableNullable: null);
			}

			List<IEnemy> list = new List<IEnemy>();

			int x = this.xMibi >> 10;
			int y = this.yMibi >> 10;

			int newXMibi = this.xMibi;
			int newYMibi = this.yMibi;
			int newXSpeedInMibipixelsPerSecond = this.xSpeedInMibipixelsPerSecond;
			int newYSpeedInMibipixelsPerSecond = this.ySpeedInMibipixelsPerSecond;

			int newOrbitersRadiusInMibipixels = this.orbitersRadiusInMibipixels;

			bool isOnGround = tilemap.IsGround(x, y - 8 * 3 - 1);

			if (isOnGround)
				newYSpeedInMibipixelsPerSecond = this.jumpSpeed;
			else
				newYSpeedInMibipixelsPerSecond -= elapsedMicrosPerFrame * 3;

			if (newYSpeedInMibipixelsPerSecond < -2000 * 1000)
				newYSpeedInMibipixelsPerSecond = -2000 * 1000;

			int proposedNewYMibi = (int)(((long)newYMibi) + ((long)newYSpeedInMibipixelsPerSecond) * ((long)elapsedMicrosPerFrame) / 1000L / 1000L);
			if (newYSpeedInMibipixelsPerSecond < 0)
			{
				while (true)
				{
					if (tilemap.IsGround(newXMibi >> 10, (proposedNewYMibi >> 10) - 8 * 3))
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

			int proposedNewXMibi = (int)(((long)newXMibi) + ((long)newXSpeedInMibipixelsPerSecond) * ((long)elapsedMicrosPerFrame) / 1000L / 1000L);
			if (newXSpeedInMibipixelsPerSecond > 0)
			{
				while (true)
				{
					if (tilemap.IsGround((proposedNewXMibi >> 10) + 8 * 3, newYMibi >> 10))
					{
						proposedNewXMibi -= 1024;
						if (proposedNewXMibi <= newXMibi)
						{
							proposedNewXMibi = newXMibi;
							break;
						}
					}
					else
						break;
				}
			}
			if (newXSpeedInMibipixelsPerSecond < 0)
			{
				while (true)
				{
					if (tilemap.IsGround((proposedNewXMibi >> 10) - 8 * 3, newYMibi >> 10))
					{
						proposedNewXMibi += 1024;
						if (proposedNewXMibi >= newXMibi)
						{
							proposedNewXMibi = newXMibi;
							break;
						}
					}
					else
						break;
				}
			}

			newXMibi = proposedNewXMibi;

			if (newXSpeedInMibipixelsPerSecond > 0)
			{
				if (newXMibi <= this.xMibi + 1024)
					newXSpeedInMibipixelsPerSecond = -newXSpeedInMibipixelsPerSecond;
			}
			else
			{
				if (newXMibi >= this.xMibi - 1024)
					newXSpeedInMibipixelsPerSecond = -newXSpeedInMibipixelsPerSecond;
			}

			int newElapsedMicros = this.elapsedMicros + elapsedMicrosPerFrame;
			if (newElapsedMicros > 2 * 1000 * 1000 * 1000)
				newElapsedMicros = 1;

			int newOrbitersAngleScaled;

			if (this.isOrbitingClockwise)
				newOrbitersAngleScaled = this.orbitersAngleScaled - ORBITERS_SPEED_IN_ANGLES_SCALED_PER_SECOND / 1000 * elapsedMicrosPerFrame / 1000;
			else
				newOrbitersAngleScaled = this.orbitersAngleScaled + ORBITERS_SPEED_IN_ANGLES_SCALED_PER_SECOND / 1000 * elapsedMicrosPerFrame / 1000;

			newOrbitersAngleScaled = DTMath.NormalizeDegreesScaled(newOrbitersAngleScaled);

			if (newOrbitersRadiusInMibipixels < ORBITERS_MAX_RADIUS_IN_PIXELS * 1024)
				newOrbitersRadiusInMibipixels = newOrbitersRadiusInMibipixels + ORBITERS_RADIUS_INCREASE_IN_MIBIPIXELS_PER_SECOND / 10000 * elapsedMicrosPerFrame / 100;

			list.Add(new EnemyEliteOrange_YetiVersion(
				xMibi: newXMibi,
				yMibi: newYMibi,
				xSpeedInMibipixelsPerSecond: newXSpeedInMibipixelsPerSecond,
				ySpeedInMibipixelsPerSecond: newYSpeedInMibipixelsPerSecond,
				jumpSpeed: this.jumpSpeed,
				orbitersAngleScaled: newOrbitersAngleScaled,
				orbitersRadiusInMibipixels: newOrbitersRadiusInMibipixels,
				isOrbitingClockwise: this.isOrbitingClockwise,
				orbitersXAndYMibi: ComputeOrbitersXAndYMibi(xMibi: newXMibi, yMibi: newYMibi, orbitersAngleScaled: newOrbitersAngleScaled, orbitersRadiusInMibipixels: newOrbitersRadiusInMibipixels),
				elapsedMicros: newElapsedMicros,
				enemyId: this.EnemyId));

			return new EnemyProcessing.Result(
				enemiesImmutableNullable: list,
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			bool isFacingRight = this.xSpeedInMibipixelsPerSecond > 0;

			GameImage image = isFacingRight ? GameImage.Orange : GameImage.OrangeMirrored;

			int spriteNum = (this.elapsedMicros / (100 * 1000)) % 8;

			displayOutput.DrawImageRotatedClockwise(
				image: image,
				imageX: spriteNum * 16,
				imageY: 0,
				imageWidth: 16,
				imageHeight: 16,
				x: (this.xMibi >> 10) - 8 * 3,
				y: (this.yMibi >> 10) - 8 * 3,
				degreesScaled: 0,
				scalingFactorScaled: 128 * 3);

			for (int i = 0; i < this.orbitersXAndYMibi.Count; i++)
			{
				Tuple<int, int> orbiterXAndYMibi = this.orbitersXAndYMibi[i];

				displayOutput.DrawImageRotatedClockwise(
					image: GameImage.Spikes,
					imageX: 0,
					imageY: 0,
					imageWidth: 16,
					imageHeight: 16,
					x: (orbiterXAndYMibi.Item1 >> 10) - 8 * 3,
					y: (orbiterXAndYMibi.Item2 >> 10) - 8 * 3,
					degreesScaled: 0,
					scalingFactorScaled: 128 * 3);
			}
		}
	}
}
