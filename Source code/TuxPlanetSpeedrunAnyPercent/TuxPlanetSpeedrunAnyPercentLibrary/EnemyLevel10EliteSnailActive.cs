
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyLevel10EliteSnailActive : IEnemy
	{
		private int xMibi;
		private int yMibi;
		private bool isFacingRight;
		private bool? shouldInitiallyTeleportUpward;

		private int xSpeedInMibipixelsPerSecond;
		private int ySpeedInMibipixelsPerSecond;

		private int jumpCooldown;

		private const int JUMP_Y_SPEED = 1100000;

		private Tuple<int, int> teleportStartingLocation;
		private int? teleportInProgressElapsedMicros;
		private const int TELEPORT_DURATION = 150 * 1000;
		private int? teleportCooldown;
		private const int TELEPORT_COOLDOWN = 500 * 1000;
		private bool hasAlreadyUsedTeleport;

		private int elapsedMicros;

		private int maxXMibi;

		public string EnemyId { get; private set; }

		private EnemyLevel10EliteSnailActive(
			int xMibi,
			int yMibi,
			bool isFacingRight,
			bool? shouldInitiallyTeleportUpward,
			int xSpeedInMibipixelsPerSecond,
			int ySpeedInMibipixelsPerSecond,
			int jumpCooldown,
			Tuple<int, int> teleportStartingLocation,
			int? teleportInProgressElapsedMicros,
			int? teleportCooldown,
			bool hasAlreadyUsedTeleport,
			int elapsedMicros,
			int maxXMibi,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.isFacingRight = isFacingRight;
			this.shouldInitiallyTeleportUpward = shouldInitiallyTeleportUpward;
			this.xSpeedInMibipixelsPerSecond = xSpeedInMibipixelsPerSecond;
			this.ySpeedInMibipixelsPerSecond = ySpeedInMibipixelsPerSecond;
			this.jumpCooldown = jumpCooldown;
			this.teleportStartingLocation = teleportStartingLocation;
			this.teleportInProgressElapsedMicros = teleportInProgressElapsedMicros;
			this.teleportCooldown = teleportCooldown;
			this.hasAlreadyUsedTeleport = hasAlreadyUsedTeleport;
			this.elapsedMicros = elapsedMicros;
			this.maxXMibi = maxXMibi;
			this.EnemyId = enemyId;
		}

		public static EnemyLevel10EliteSnailActive GetEnemyLevel10EliteSnailActive(
			int xMibi,
			int yMibi,
			bool isFacingRight,
			bool shouldInitiallyTeleportUpward,
			int elapsedMicros,
			int maxXMibi,
			string enemyId)
		{
			return new EnemyLevel10EliteSnailActive(
				xMibi: xMibi,
				yMibi: yMibi,
				isFacingRight: isFacingRight,
				shouldInitiallyTeleportUpward: shouldInitiallyTeleportUpward,
				xSpeedInMibipixelsPerSecond: 20 * 1000 * (isFacingRight ? 1 : -1),
				ySpeedInMibipixelsPerSecond: 0,
				jumpCooldown: 0,
				teleportStartingLocation: null,
				teleportInProgressElapsedMicros: null,
				teleportCooldown: null,
				hasAlreadyUsedTeleport: false,
				elapsedMicros: elapsedMicros,
				maxXMibi: maxXMibi,
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
			if (this.teleportInProgressElapsedMicros.HasValue)
				return new List<Hitbox>();

			return new List<Hitbox>()
			{
				new Hitbox(
					x: (this.xMibi >> 10) - 8 * 3,
					y: (this.yMibi >> 10) - 8 * 3,
					width: 16 * 3,
					height: 14 * 3)
			};
		}

		public IReadOnlyList<Hitbox> GetDamageBoxes()
		{
			if (this.teleportInProgressElapsedMicros.HasValue)
				return new List<Hitbox>();

			return new List<Hitbox>()
			{
				new Hitbox(
					x: (this.xMibi >> 10) - 8 * 3,
					y: (this.yMibi >> 10) - 2 * 3,
					width: 16 * 3,
					height: 8 * 3)
			};
		}

		public IEnemy GetDeadEnemy()
		{
			string enemyId = this.EnemyId;

			return EnemyDeadPoof.SpawnEnemyDeadPoof(
				xMibi: this.xMibi,
				yMibi: this.yMibi,
				enemyId: enemyId + "_enemyDeadPoof");
		}

		private static bool IsTeleportable(ITilemap tilemap, int x, int y, int maxXMibi)
		{
			return !tilemap.IsGround(x, y)
				&& !tilemap.IsSpikes(x, y)
				&& !tilemap.IsKillZone(x, y)
				&& (x << 10) <= maxXMibi;
		}

		private EnemyProcessing.Result ProcessFrame_Teleport(int elapsedMicrosPerFrame)
		{
			int? newTeleportInProgressElapsedMicros = this.teleportInProgressElapsedMicros.Value + elapsedMicrosPerFrame;
			bool hasFinishedTeleporting;

			if (newTeleportInProgressElapsedMicros.Value >= TELEPORT_DURATION)
				hasFinishedTeleporting = true;
			else
				hasFinishedTeleporting = false;

			List<IEnemy> list = new List<IEnemy>();

			list.Add(new EnemyLevel10EliteSnailActive(
				xMibi: this.xMibi,
				yMibi: this.yMibi,
				isFacingRight: this.isFacingRight,
				shouldInitiallyTeleportUpward: this.shouldInitiallyTeleportUpward,
				xSpeedInMibipixelsPerSecond: this.xSpeedInMibipixelsPerSecond,
				ySpeedInMibipixelsPerSecond: this.ySpeedInMibipixelsPerSecond,
				jumpCooldown: this.jumpCooldown,
				teleportStartingLocation: hasFinishedTeleporting ? null : this.teleportStartingLocation,
				teleportInProgressElapsedMicros: hasFinishedTeleporting ? null : newTeleportInProgressElapsedMicros,
				teleportCooldown: hasFinishedTeleporting ? TELEPORT_COOLDOWN : (int?)null,
				hasAlreadyUsedTeleport: hasFinishedTeleporting ? true : false,
				elapsedMicros: this.elapsedMicros + elapsedMicrosPerFrame,
				maxXMibi: this.maxXMibi,
				enemyId: this.EnemyId));

			return new EnemyProcessing.Result(
				enemies: list,
				newlyKilledEnemies: new List<string>(),
				newlyAddedLevelFlags: null);
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
			if (this.teleportInProgressElapsedMicros.HasValue)
				return this.ProcessFrame_Teleport(elapsedMicrosPerFrame: elapsedMicrosPerFrame);

			List<IEnemy> list = new List<IEnemy>();

			int newElapsedMicros = this.elapsedMicros + elapsedMicrosPerFrame;

			if (newElapsedMicros > 2 * 1000 * 1000 * 1000)
				newElapsedMicros = 0;

			int tuxXMibi = tuxState.XMibi;
			int tuxYMibi = tuxState.YMibi;

			int newXMibi = this.xMibi;
			int newYMibi = this.yMibi;

			bool isMovingRight;
			bool isMovingLeft;

			int newJumpCooldown = this.jumpCooldown - elapsedMicrosPerFrame;
			if (newJumpCooldown < 0)
				newJumpCooldown = 0;

			if (newXMibi > tuxXMibi + 30 * 1024)
			{
				isMovingLeft = true;
				isMovingRight = false;
			}
			else if (newXMibi < tuxXMibi - 30 * 1024)
			{
				isMovingLeft = false;
				isMovingRight = true;
			}
			else
			{
				isMovingLeft = false;
				isMovingRight = false;
			}

			bool isOnGround = tilemap.IsGround(newXMibi >> 10, (newYMibi >> 10) - 8 * 3 - 1) && !tilemap.IsGround(newXMibi >> 10, (newYMibi >> 10) - 8 * 3 + 1)
				|| tilemap.IsGround((newXMibi >> 10) - 4 * 3, (newYMibi >> 10) - 8 * 3 - 1) && !tilemap.IsGround((newXMibi >> 10) - 4 * 3, (newYMibi >> 10) - 8 * 3 + 1)
				|| tilemap.IsGround((newXMibi >> 10) + 4 * 3, (newYMibi >> 10) - 8 * 3 - 1) && !tilemap.IsGround((newXMibi >> 10) + 4 * 3, (newYMibi >> 10) - 8 * 3 + 1);

			int newXSpeedInMibipixelsPerSecond = this.xSpeedInMibipixelsPerSecond;
			int newYSpeedInMibipixelsPerSecond = this.ySpeedInMibipixelsPerSecond;

			if (isMovingRight)
				newXSpeedInMibipixelsPerSecond += elapsedMicrosPerFrame * 3;

			if (isMovingLeft)
				newXSpeedInMibipixelsPerSecond -= elapsedMicrosPerFrame * 3;

			if (!isMovingRight && !isMovingLeft)
			{
				if (newXSpeedInMibipixelsPerSecond > 0)
				{
					newXSpeedInMibipixelsPerSecond -= elapsedMicrosPerFrame * 3;
					if (newXSpeedInMibipixelsPerSecond < 0)
						newXSpeedInMibipixelsPerSecond = 0;
				}
				if (newXSpeedInMibipixelsPerSecond < 0)
				{
					newXSpeedInMibipixelsPerSecond += elapsedMicrosPerFrame * 3;
					if (newXSpeedInMibipixelsPerSecond > 0)
						newXSpeedInMibipixelsPerSecond = 0;
				}
			}

			if (newXSpeedInMibipixelsPerSecond > 1000 * 1000)
				newXSpeedInMibipixelsPerSecond = 1000 * 1000;

			if (newXSpeedInMibipixelsPerSecond < -1000 * 1000)
				newXSpeedInMibipixelsPerSecond = -1000 * 1000;

			if (newJumpCooldown <= 0 && isOnGround && newYSpeedInMibipixelsPerSecond <= 0)
			{
				newYSpeedInMibipixelsPerSecond = JUMP_Y_SPEED;
				newJumpCooldown = 1000 * 1000;
				isOnGround = false;
			}

			if (!isOnGround && newYSpeedInMibipixelsPerSecond >= -3000 * 1000)
				newYSpeedInMibipixelsPerSecond -= elapsedMicrosPerFrame * 3;

			int proposedNewYMibi = (int)(((long)newYMibi) + ((long)newYSpeedInMibipixelsPerSecond) * ((long)elapsedMicrosPerFrame) / 1024L / 1024L);
			if (newYSpeedInMibipixelsPerSecond > 0)
			{
				while (true)
				{
					if (tilemap.IsGround(newXMibi / 1024, proposedNewYMibi / 1024 + 8 * 3)
							|| tilemap.IsGround(newXMibi / 1024 - 8 * 3, proposedNewYMibi / 1024 + 8 * 3)
							|| tilemap.IsGround(newXMibi / 1024 + 8 * 3, proposedNewYMibi / 1024 + 8 * 3))
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
					if (tilemap.IsGround(newXMibi / 1024, proposedNewYMibi / 1024 - 8 * 3)
							|| tilemap.IsGround(newXMibi / 1024 - 8 * 3, proposedNewYMibi / 1024 - 8 * 3)
							|| tilemap.IsGround(newXMibi / 1024 + 8 * 3, proposedNewYMibi / 1024 - 8 * 3))
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

			int proposedNewXMibi = (int)(((long)newXMibi) + ((long)newXSpeedInMibipixelsPerSecond) * ((long)elapsedMicrosPerFrame) / 1024L / 1024L);
			if (newXSpeedInMibipixelsPerSecond > 0)
			{
				while (true)
				{
					if (tilemap.IsGround(proposedNewXMibi / 1024 + 8 * 3, newYMibi / 1024 - 8 * 3)
							|| tilemap.IsGround(proposedNewXMibi / 1024 + 8 * 3, newYMibi / 1024)
							|| tilemap.IsGround(proposedNewXMibi / 1024 + 8 * 3, newYMibi / 1024 + 8 * 3))
					{
						newXSpeedInMibipixelsPerSecond = 0;
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
					if (tilemap.IsGround(proposedNewXMibi / 1024 - 8 * 3, newYMibi / 1024 - 8 * 3)
							|| tilemap.IsGround(proposedNewXMibi / 1024 - 8 * 3, newYMibi / 1024)
							|| tilemap.IsGround(proposedNewXMibi / 1024 - 8 * 3, newYMibi / 1024 + 8 * 3))
					{
						newXSpeedInMibipixelsPerSecond = 0;
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

			int? newTeleportCooldown = this.teleportCooldown;
			if (newTeleportCooldown != null)
			{
				newTeleportCooldown = newTeleportCooldown.Value - elapsedMicrosPerFrame;
				if (newTeleportCooldown.Value <= 0)
					newTeleportCooldown = null;
			}

			bool? newShouldInitiallyTeleportUpward = this.shouldInitiallyTeleportUpward;

			Tuple<int, int> newTeleportStartingLocation = null;
			int? newTeleportInProgressElapsedMicros = this.teleportInProgressElapsedMicros;
			if (newTeleportCooldown == null && !this.hasAlreadyUsedTeleport)
			{
				newTeleportStartingLocation = new Tuple<int, int>(this.xMibi, this.yMibi);
				newTeleportInProgressElapsedMicros = 0;

				int distanceToTuxX = tuxXMibi - this.xMibi;
				int distanceToTuxY = tuxYMibi - this.yMibi;

				int deltaX;
				int deltaY;

				if (newShouldInitiallyTeleportUpward.HasValue)
				{
					deltaX = random.NextInt(3) == 0
						? 0
						: (random.NextBool() ? -1 : 1);
					deltaY = newShouldInitiallyTeleportUpward.Value ? 1 : -1;
					newShouldInitiallyTeleportUpward = null;
				}
				else if (distanceToTuxX > 0 && distanceToTuxY > 0 && distanceToTuxX > distanceToTuxY)
				{
					deltaX = 1;
					deltaY = random.NextBool() ? 1 : 0;
				}
				else if (distanceToTuxX > 0 && distanceToTuxY > 0)
				{
					deltaX = random.NextBool() ? 1 : 0;
					deltaY = 1;
				}
				else if (distanceToTuxX < 0 && distanceToTuxY > 0 && -distanceToTuxX > distanceToTuxY)
				{
					deltaX = -1;
					deltaY = random.NextBool() ? 1 : 0;
				}
				else if (distanceToTuxX < 0 && distanceToTuxY > 0)
				{
					deltaX = random.NextBool() ? -1 : 0;
					deltaY = 1;
				}
				else if (distanceToTuxX > 0 && distanceToTuxY < 0 && distanceToTuxX > -distanceToTuxY)
				{
					deltaX = 1;
					deltaY = random.NextBool() ? -1 : 0;
				}
				else if (distanceToTuxX > 0 && distanceToTuxY < 0)
				{
					deltaX = random.NextBool() ? 1 : 0;
					deltaY = -1;
				}
				else if (distanceToTuxX < 0 && distanceToTuxY < 0 && -distanceToTuxX > -distanceToTuxY)
				{
					deltaX = -1;
					deltaY = random.NextBool() ? -1 : 0;
				}
				else
				{
					deltaX = random.NextBool() ? -1 : 0;
					deltaY = -1;
				}

				int interval = 100;

				if (deltaX == 0 || deltaY == 0)
					interval = 141;

				while (true)
				{
					if (interval == 0)
					{
						newTeleportStartingLocation = null;
						newTeleportInProgressElapsedMicros = null;
						newTeleportCooldown = TELEPORT_COOLDOWN;
						break;
					}

					proposedNewXMibi = newXMibi + deltaX * 1024 * interval * 2;
					proposedNewYMibi = newYMibi + deltaY * 1024 * interval * 2;
					interval--;

					if (IsTeleportable(tilemap, proposedNewXMibi / 1024 - 8 * 3, proposedNewYMibi / 1024 - 8 * 3, this.maxXMibi)
						&& IsTeleportable(tilemap, proposedNewXMibi / 1024 - 8 * 3, proposedNewYMibi / 1024, this.maxXMibi)
						&& IsTeleportable(tilemap, proposedNewXMibi / 1024 - 8 * 3, proposedNewYMibi / 1024 + 8 * 3, this.maxXMibi)
						&& IsTeleportable(tilemap, proposedNewXMibi / 1024 + 8 * 3, proposedNewYMibi / 1024 - 8 * 3, this.maxXMibi)
						&& IsTeleportable(tilemap, proposedNewXMibi / 1024 + 8 * 3, proposedNewYMibi / 1024, this.maxXMibi)
						&& IsTeleportable(tilemap, proposedNewXMibi / 1024 + 8 * 3, proposedNewYMibi / 1024 + 8 * 3, this.maxXMibi))
					{
						newXMibi = proposedNewXMibi;
						newYMibi = proposedNewYMibi;
						newXSpeedInMibipixelsPerSecond = deltaX * 1024 * (deltaX == 0 || deltaY == 0 ? 1448 : 1024);
						newYSpeedInMibipixelsPerSecond = deltaY * 1024 * (deltaX == 0 || deltaY == 0 ? 1448 : 1024);
						isOnGround = false;
						soundOutput.PlaySound(GameSound.Teleport);
						break;
					}
				}
			}

			bool isOutOfBounds = (newXMibi >> 10) + 8 * 3 < cameraX - windowWidth / 2 - GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (newXMibi >> 10) - 8 * 3 > cameraX + windowWidth / 2 + GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (newYMibi >> 10) + 8 * 3 < cameraY - windowHeight / 2 - GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (newYMibi >> 10) - 8 * 3 > cameraY + windowHeight / 2 + GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS;
			
			if (!isOutOfBounds)
				list.Add(new EnemyLevel10EliteSnailActive(
					xMibi: newXMibi,
					yMibi: newYMibi,
					isFacingRight: isMovingRight
						? true
						: (isMovingLeft ? false : this.isFacingRight),
					shouldInitiallyTeleportUpward: newShouldInitiallyTeleportUpward,
					xSpeedInMibipixelsPerSecond: newXSpeedInMibipixelsPerSecond,
					ySpeedInMibipixelsPerSecond: newYSpeedInMibipixelsPerSecond,
					jumpCooldown: newJumpCooldown,
					teleportStartingLocation: newTeleportStartingLocation,
					teleportInProgressElapsedMicros: newTeleportInProgressElapsedMicros,
					teleportCooldown: newTeleportCooldown,
					hasAlreadyUsedTeleport: this.hasAlreadyUsedTeleport ? !isOnGround : false,
					elapsedMicros: newElapsedMicros,
					maxXMibi: this.maxXMibi,
					enemyId: this.EnemyId));

			return new EnemyProcessing.Result(
				enemies: list,
				newlyKilledEnemies: new List<string>(),
				newlyAddedLevelFlags: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			if (this.teleportInProgressElapsedMicros.HasValue)
			{
				long deltaX = this.xMibi - this.teleportStartingLocation.Item1;
				long deltaY = this.yMibi - this.teleportStartingLocation.Item2;

				for (int i = Math.Max(0, this.teleportInProgressElapsedMicros.Value - 50 * 1000); i < this.teleportInProgressElapsedMicros.Value; i += 5 * 1000)
				{
					int renderXMibi = (int)(this.teleportStartingLocation.Item1 + deltaX * i / TELEPORT_DURATION);
					int renderYMibi = (int)(this.teleportStartingLocation.Item2 + deltaY * i / TELEPORT_DURATION);

					int alpha = (i - (this.teleportInProgressElapsedMicros.Value - 50 * 1000)) * 170 / 50000;

					if (alpha > 0 && alpha <= 255)
						displayOutput.DrawRectangle(
							x: (renderXMibi >> 10) - 20,
							y: (renderYMibi >> 10) - 20,
							width: 40,
							height: 40,
							color: new DTColor(255, 255, 255, alpha),
							fill: true);
				}
			}
			else
			{
				GameImage image = this.isFacingRight ? GameImage.Snail : GameImage.SnailMirrored;

				int spriteNum = (this.elapsedMicros / 250000) % 2;

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
			}
		}
	}
}
