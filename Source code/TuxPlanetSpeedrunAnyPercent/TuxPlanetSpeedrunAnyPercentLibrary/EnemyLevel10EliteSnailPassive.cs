
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyLevel10EliteSnailPassive : IEnemy
	{
		public class EnemyLevel10EliteSnailSpawn : Tilemap.IExtraEnemyToSpawn
		{
			private int xMibi;
			private int yMibi;
			private int activationRadiusInPixels;
			private int activationDelayInMicroseconds;
			private bool shouldInitiallyTeleportUpward;
			private int maxXMibi;
			private string enemyId;

			public EnemyLevel10EliteSnailSpawn(
				int xMibi,
				int yMibi,
				int activationRadiusInPixels,
				int activationDelayInMicroseconds,
				bool shouldInitiallyTeleportUpward,
				int maxXMibi,
				string enemyId)
			{
				this.xMibi = xMibi;
				this.yMibi = yMibi;
				this.activationRadiusInPixels = activationRadiusInPixels;
				this.activationDelayInMicroseconds = activationDelayInMicroseconds;
				this.shouldInitiallyTeleportUpward = shouldInitiallyTeleportUpward;
				this.maxXMibi = maxXMibi;
				this.enemyId = enemyId;
			}

			public IEnemy GetEnemy(int xOffset, int yOffset)
			{
				return new EnemySpawnHelper(
					enemyToSpawn: GetEnemyLevel10EliteSnailPassive(
						xMibi: this.xMibi + (xOffset << 10),
						yMibi: this.yMibi + (yOffset << 10),
						isFacingRight: false,
						shouldInitiallyTeleportUpward: this.shouldInitiallyTeleportUpward,
						activationRadiusInPixels: this.activationRadiusInPixels,
						activationDelayInMicroseconds: this.activationDelayInMicroseconds,
						maxXMibi: this.maxXMibi,
						enemyId: this.enemyId),
					xMibi: this.xMibi + (xOffset << 10),
					yMibi: this.yMibi + (yOffset << 10),
					enemyWidth: 48,
					enemyHeight: 48);
			}
		}

		private int xMibi;
		private int yMibi;
		private bool isFacingRight;
		private bool shouldInitiallyTeleportUpward;

		private int elapsedMicros;

		private bool isActive;
		private int activationRadiusInPixels;
		private int activationDelayInMicroseconds;
		private int? accumulatedActivationDelay;

		private int maxXMibi;

		public string EnemyId { get; private set; }

		public const string CAN_BECOME_ACTIVE_LEVEL_FLAG = "EnemyLevel10EliteSnailPassive_canBecomeActive_levelFlag";

		private EnemyLevel10EliteSnailPassive(
			int xMibi,
			int yMibi,
			bool isFacingRight,
			bool shouldInitiallyTeleportUpward,
			int elapsedMicros,
			bool isActive,
			int activationRadiusInPixels,
			int activationDelayInMicroseconds,
			int? accumulatedActivationDelay,
			int maxXMibi,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.isFacingRight = isFacingRight;
			this.shouldInitiallyTeleportUpward = shouldInitiallyTeleportUpward;
			this.elapsedMicros = elapsedMicros;
			this.isActive = isActive;
			this.activationRadiusInPixels = activationRadiusInPixels;
			this.activationDelayInMicroseconds = activationDelayInMicroseconds;
			this.accumulatedActivationDelay = accumulatedActivationDelay;
			this.maxXMibi = maxXMibi;
			this.EnemyId = enemyId;
		}

		public static EnemyLevel10EliteSnailPassive GetEnemyLevel10EliteSnailPassive(
			int xMibi,
			int yMibi,
			bool isFacingRight,
			bool shouldInitiallyTeleportUpward,
			int activationRadiusInPixels,
			int activationDelayInMicroseconds,
			int maxXMibi,
			string enemyId)
		{
			return new EnemyLevel10EliteSnailPassive(
				xMibi: xMibi,
				yMibi: yMibi,
				isFacingRight: isFacingRight,
				shouldInitiallyTeleportUpward: shouldInitiallyTeleportUpward,
				elapsedMicros: 0,
				isActive: false,
				activationRadiusInPixels: activationRadiusInPixels,
				activationDelayInMicroseconds: activationDelayInMicroseconds,
				accumulatedActivationDelay: null,
				maxXMibi: maxXMibi,
				enemyId: enemyId);
		}

		public IReadOnlyList<Hitbox> GetHitboxes()
		{
			List<Hitbox> list = new List<Hitbox>();
			list.Add(new Hitbox(
				x: (this.xMibi >> 10) - 8 * 3,
				y: (this.yMibi >> 10) - 8 * 3,
				width: 16 * 3,
				height: 14 * 3));
			return list;
		}

		public IReadOnlyList<Hitbox> GetDamageBoxes()
		{
			List<Hitbox> list = new List<Hitbox>();
			list.Add(new Hitbox(
				x: (this.xMibi >> 10) - 8 * 3,
				y: (this.yMibi >> 10) - 8 * 3,
				width: 16 * 3,
				height: 14 * 3));

			return list;
		}

		public IEnemy GetDeadEnemy()
		{
			string enemyId = this.EnemyId;

			return EnemyDeadPoof.SpawnEnemyDeadPoof(
				xMibi: this.xMibi,
				yMibi: this.yMibi,
				enemyId: enemyId + "_enemyDeadPoof");
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
			bool isOutOfBounds = (this.xMibi >> 10) + 8 * 3 < cameraX - (windowWidth >> 1) - GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (this.xMibi >> 10) - 8 * 3 > cameraX + (windowWidth >> 1) + GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (this.yMibi >> 10) + 8 * 3 < cameraY - (windowHeight >> 1) - GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (this.yMibi >> 10) - 8 * 3 > cameraY + (windowHeight >> 1) + GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS;

			if (isOutOfBounds)
				return new EnemyProcessing.Result(
					enemiesImmutableNullable: null,
					newlyKilledEnemiesImmutableNullable: null,
					newlyAddedLevelFlagsImmutableNullable: null);

			bool newIsActive = this.isActive;

			int? newAccumulatedActivationDelay = this.accumulatedActivationDelay;

			if (!newIsActive)
			{
				for (int i = 0; i < levelFlags.Count; i++)
				{
					if (levelFlags[i] == CAN_BECOME_ACTIVE_LEVEL_FLAG)
					{
						newIsActive = true;
						break;
					}
				}
			}

			if (newIsActive && !newAccumulatedActivationDelay.HasValue)
			{
				int deltaX = (this.xMibi >> 10) - (tuxState.XMibi >> 10);
				int deltaY = (this.yMibi >> 10) - (tuxState.YMibi >> 10);

				if (deltaX * deltaX + deltaY * deltaY <= this.activationRadiusInPixels * this.activationRadiusInPixels)
					newAccumulatedActivationDelay = 0;
			}

			if (newAccumulatedActivationDelay.HasValue)
				newAccumulatedActivationDelay = newAccumulatedActivationDelay.Value + elapsedMicrosPerFrame;

			if (newAccumulatedActivationDelay.HasValue && newAccumulatedActivationDelay.Value >= this.activationDelayInMicroseconds)
			{
				return new EnemyProcessing.Result(
					enemiesImmutableNullable: new List<IEnemy>()
					{
						EnemyLevel10EliteSnailActive.GetEnemyLevel10EliteSnailActive(
							xMibi: this.xMibi,
							yMibi: this.yMibi,
							isFacingRight: this.isFacingRight,
							shouldInitiallyTeleportUpward: this.shouldInitiallyTeleportUpward,
							elapsedMicros: this.elapsedMicros,
							maxXMibi: this.maxXMibi,
							enemyId: this.EnemyId + "_active")
					},
					newlyKilledEnemiesImmutableNullable: new List<string>() { this.EnemyId },
					newlyAddedLevelFlagsImmutableNullable: null);
			}

			List<IEnemy> list = new List<IEnemy>();

			int newElapsedMicros = this.elapsedMicros + elapsedMicrosPerFrame;

			if (newElapsedMicros > 2 * 1000 * 1000 * 1000)
				newElapsedMicros = 0;

			int newXMibi = this.xMibi;
			int newYMibi = this.yMibi;
			bool newIsFacingRight = this.isFacingRight;

			if (this.isFacingRight)
				newXMibi += elapsedMicrosPerFrame * 20 / 1000;
			else
				newXMibi -= elapsedMicrosPerFrame * 20 / 1000;

			if (newIsFacingRight)
			{
				if (IsGroundOrSpike(tilemap: tilemap, x: (newXMibi >> 10) + 8 * 3, y: newYMibi >> 10))
					newIsFacingRight = false;
				if (!IsGroundOrSpike(tilemap: tilemap, x: (newXMibi >> 10) + 4 * 3, y: (newYMibi >> 10) - 11 * 3))
					newIsFacingRight = false;
			}
			else
			{
				if (IsGroundOrSpike(tilemap: tilemap, x: (newXMibi >> 10) - 8 * 3, y: newYMibi >> 10))
					newIsFacingRight = true;
				if (!IsGroundOrSpike(tilemap: tilemap, x: (newXMibi >> 10) - 4 * 3, y: (newYMibi >> 10) - 11 * 3))
					newIsFacingRight = true;
			}
			
			list.Add(new EnemyLevel10EliteSnailPassive(
				xMibi: newXMibi,
				yMibi: newYMibi,
				isFacingRight: newIsFacingRight,
				shouldInitiallyTeleportUpward: this.shouldInitiallyTeleportUpward,
				elapsedMicros: newElapsedMicros,
				isActive: newIsActive,
				activationRadiusInPixels: this.activationRadiusInPixels,
				activationDelayInMicroseconds: this.activationDelayInMicroseconds,
				accumulatedActivationDelay: newAccumulatedActivationDelay,
				maxXMibi: this.maxXMibi,
				enemyId: this.EnemyId));

			return new EnemyProcessing.Result(
				enemiesImmutableNullable: list,
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
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
