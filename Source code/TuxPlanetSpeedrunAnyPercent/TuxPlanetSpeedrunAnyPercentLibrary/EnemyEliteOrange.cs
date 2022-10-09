
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyEliteOrange : IEnemy
	{
		private int xMibi;
		private int yMibi;

		private int xSpeedInMibipixelsPerSecond;
		private int ySpeedInMibipixelsPerSecond;

		private int orbitersAngleScaled;
		private bool isOrbitingClockwise;
		private List<Tuple<int, int>> orbitersXAndYMibi;

		private int elapsedMicros;

		private const int ORBITERS_SPEED_IN_ANGLES_SCALED_PER_SECOND = 180 * 128;
		public const int ORBITERS_RADIUS_IN_PIXELS = 150;

		public string EnemyId { get; private set; }

		private EnemyEliteOrange(
			int xMibi,
			int yMibi,
			int xSpeedInMibipixelsPerSecond,
			int ySpeedInMibipixelsPerSecond,
			int orbitersAngleScaled,
			bool isOrbitingClockwise,
			List<Tuple<int, int>> orbitersXAndYMibi,
			int elapsedMicros,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.xSpeedInMibipixelsPerSecond = xSpeedInMibipixelsPerSecond;
			this.ySpeedInMibipixelsPerSecond = ySpeedInMibipixelsPerSecond;
			this.orbitersAngleScaled = orbitersAngleScaled;
			this.isOrbitingClockwise = isOrbitingClockwise;
			this.orbitersXAndYMibi = orbitersXAndYMibi;
			this.elapsedMicros = elapsedMicros;
			this.EnemyId = enemyId;
		}

		public static EnemyEliteOrange GetEnemyEliteOrange(
			int xMibi,
			int yMibi,
			bool isFacingRight,
			int orbitersAngleScaled,
			bool isOrbitingClockwise,
			string enemyId)
		{
			return new EnemyEliteOrange(
				xMibi: xMibi,
				yMibi: yMibi,
				xSpeedInMibipixelsPerSecond: 150000 * (isFacingRight ? 1 : -1),
				ySpeedInMibipixelsPerSecond: 0,
				orbitersAngleScaled: orbitersAngleScaled,
				isOrbitingClockwise: isOrbitingClockwise,
				orbitersXAndYMibi: ComputeOrbitersXAndYMibi(xMibi: xMibi, yMibi: yMibi, orbitersAngleScaled: orbitersAngleScaled),
				elapsedMicros: 0,
				enemyId: enemyId);
		}

		private static List<Tuple<int, int>> ComputeOrbitersXAndYMibi(int xMibi, int yMibi, int orbitersAngleScaled)
		{
			orbitersAngleScaled = DTMath.NormalizeDegreesScaled(orbitersAngleScaled);

			List<Tuple<int, int>> list = new List<Tuple<int, int>>();

			list.Add(new Tuple<int, int>(
				item1: xMibi + ORBITERS_RADIUS_IN_PIXELS * DTMath.CosineScaled(degreesScaled: orbitersAngleScaled),
				item2: yMibi + ORBITERS_RADIUS_IN_PIXELS * DTMath.SineScaled(degreesScaled: orbitersAngleScaled)));

			orbitersAngleScaled += 120 * 128;

			list.Add(new Tuple<int, int>(
				item1: xMibi + ORBITERS_RADIUS_IN_PIXELS * DTMath.CosineScaled(degreesScaled: orbitersAngleScaled),
				item2: yMibi + ORBITERS_RADIUS_IN_PIXELS * DTMath.SineScaled(degreesScaled: orbitersAngleScaled)));

			orbitersAngleScaled += 120 * 128;

			list.Add(new Tuple<int, int>(
				item1: xMibi + ORBITERS_RADIUS_IN_PIXELS * DTMath.CosineScaled(degreesScaled: orbitersAngleScaled),
				item2: yMibi + ORBITERS_RADIUS_IN_PIXELS * DTMath.SineScaled(degreesScaled: orbitersAngleScaled)));

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

		private static bool IsGroundOrSpike(ITilemap tilemap, int x, int y)
		{
			return tilemap.IsGround(x: x, y: y) || tilemap.IsSpikes(x: x, y: y);
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
			List<IEnemy> list = new List<IEnemy>();

			int x = this.xMibi >> 10;
			int y = this.yMibi >> 10;

			int newXMibi = this.xMibi;
			int newYMibi = this.yMibi;
			int newXSpeedInMibipixelsPerSecond = this.xSpeedInMibipixelsPerSecond;
			int newYSpeedInMibipixelsPerSecond = this.ySpeedInMibipixelsPerSecond;

			bool isOnGround = IsGroundOrSpike(tilemap, x, y - 8 * 3 - 1) && !IsGroundOrSpike(tilemap, x, y - 8 * 3)
				|| IsGroundOrSpike(tilemap, x - 6 * 3, y - 8 * 3 - 1) && !IsGroundOrSpike(tilemap, x - 6 * 3, y - 8 * 3)
				|| IsGroundOrSpike(tilemap, x + 6 * 3, y - 8 * 3 - 1) && !IsGroundOrSpike(tilemap, x + 6 * 3, y - 8 * 3);

			if (isOnGround)
				newYSpeedInMibipixelsPerSecond = 1050000;
			else
				newYSpeedInMibipixelsPerSecond -= elapsedMicrosPerFrame * 3;

			if (newYSpeedInMibipixelsPerSecond < -2000 * 1000)
				newYSpeedInMibipixelsPerSecond = -2000 * 1000;

			int proposedNewYMibi = (int)(((long)newYMibi) + ((long)newYSpeedInMibipixelsPerSecond) * ((long)elapsedMicrosPerFrame) / 1000L / 1000L);
			if (newYSpeedInMibipixelsPerSecond > 0)
			{
				while (true)
				{
					if (IsGroundOrSpike(tilemap, newXMibi / 1024, proposedNewYMibi / 1024 + 8 * 3)
							|| IsGroundOrSpike(tilemap, newXMibi / 1024 - 8 * 3, proposedNewYMibi / 1024 + 8 * 3)
							|| IsGroundOrSpike(tilemap, newXMibi / 1024 + 8 * 3, proposedNewYMibi / 1024 + 8 * 3))
					{
						newYSpeedInMibipixelsPerSecond = 0;
						proposedNewYMibi -= 1024;
						if (proposedNewYMibi <= newYMibi)
						{
							proposedNewYMibi = newYMibi;
							break;
						}
					}
					else
						break;
				}
			}
			if (newYSpeedInMibipixelsPerSecond < 0)
			{
				while (true)
				{
					if (IsGroundOrSpike(tilemap, newXMibi / 1024, proposedNewYMibi / 1024 - 8 * 3)
							|| IsGroundOrSpike(tilemap, newXMibi / 1024 - 8 * 3, proposedNewYMibi / 1024 - 8 * 3)
							|| IsGroundOrSpike(tilemap, newXMibi / 1024 + 8 * 3, proposedNewYMibi / 1024 - 8 * 3))
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
					if (IsGroundOrSpike(tilemap, proposedNewXMibi / 1024 + 8 * 3, newYMibi / 1024 - 8 * 3)
							|| IsGroundOrSpike(tilemap, proposedNewXMibi / 1024 + 8 * 3, newYMibi / 1024)
							|| IsGroundOrSpike(tilemap, proposedNewXMibi / 1024 + 8 * 3, newYMibi / 1024 + 8 * 3))
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
					if (IsGroundOrSpike(tilemap, proposedNewXMibi / 1024 - 8 * 3, newYMibi / 1024 - 8 * 3)
							|| IsGroundOrSpike(tilemap, proposedNewXMibi / 1024 - 8 * 3, newYMibi / 1024)
							|| IsGroundOrSpike(tilemap, proposedNewXMibi / 1024 - 8 * 3, newYMibi / 1024 + 8 * 3))
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
				if (newXMibi <= this.xMibi)
					newXSpeedInMibipixelsPerSecond = -newXSpeedInMibipixelsPerSecond;
			}
			else
			{
				if (newXMibi >= this.xMibi)
					newXSpeedInMibipixelsPerSecond = -newXSpeedInMibipixelsPerSecond;
			}

			int newElapsedMicros = this.elapsedMicros + elapsedMicrosPerFrame;
			if (newElapsedMicros > 2 * 1000 * 1000 * 1000)
				newElapsedMicros = 1;

			bool isOutOfBounds = (this.xMibi >> 10) + ORBITERS_RADIUS_IN_PIXELS + 8 * 3 < cameraX - (windowWidth >> 1) - GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (this.xMibi >> 10) - ORBITERS_RADIUS_IN_PIXELS - 8 * 3 > cameraX + (windowWidth >> 1) + GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (this.yMibi >> 10) + ORBITERS_RADIUS_IN_PIXELS + 8 * 3 < cameraY - (windowHeight >> 1) - GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (this.yMibi >> 10) - ORBITERS_RADIUS_IN_PIXELS - 8 * 3 > cameraY + (windowHeight >> 1) + GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS;

			int newOrbitersAngleScaled;

			if (this.isOrbitingClockwise)
				newOrbitersAngleScaled = this.orbitersAngleScaled - ORBITERS_SPEED_IN_ANGLES_SCALED_PER_SECOND / 1000 * elapsedMicrosPerFrame / 1000;
			else
				newOrbitersAngleScaled = this.orbitersAngleScaled + ORBITERS_SPEED_IN_ANGLES_SCALED_PER_SECOND / 1000 * elapsedMicrosPerFrame / 1000;

			newOrbitersAngleScaled = DTMath.NormalizeDegreesScaled(newOrbitersAngleScaled);

			if (!isOutOfBounds)
				list.Add(new EnemyEliteOrange(
					xMibi: newXMibi,
					yMibi: newYMibi,
					xSpeedInMibipixelsPerSecond: newXSpeedInMibipixelsPerSecond,
					ySpeedInMibipixelsPerSecond: newYSpeedInMibipixelsPerSecond,
					orbitersAngleScaled: newOrbitersAngleScaled,
					isOrbitingClockwise: this.isOrbitingClockwise,
					orbitersXAndYMibi: ComputeOrbitersXAndYMibi(xMibi: newXMibi, yMibi: newYMibi, orbitersAngleScaled: newOrbitersAngleScaled),
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
