
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyBouncecap : IEnemy
	{
		private int xMibi;
		private int yMibi;
		private bool isFacingRight;

		private int ySpeedInMibipixelsPerSecond;

		public string EnemyId { get; private set; }

		private EnemyBouncecap(
			int xMibi,
			int yMibi,
			bool isFacingRight,
			int ySpeedInMibipixelsPerSecond,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.isFacingRight = isFacingRight;
			this.ySpeedInMibipixelsPerSecond = ySpeedInMibipixelsPerSecond;
			this.EnemyId = enemyId;
		}

		public static EnemyBouncecap GetEnemyBouncecap(
			int xMibi,
			int yMibi,
			string enemyId)
		{
			return new EnemyBouncecap(
				xMibi: xMibi,
				yMibi: yMibi,
				isFacingRight: false,
				ySpeedInMibipixelsPerSecond: 0,
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
			return new List<Hitbox>()
			{
				new Hitbox(
					x: (this.xMibi >> 10) - 7 * 3, 
					y: (this.yMibi >> 10) - 7 * 3, 
					width: 14 * 3, 
					height: 14 * 3)
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

			int newYMibi = this.yMibi;
			bool newIsFacingRight;
			int newYSpeedInMibipixelsPerSecond = this.ySpeedInMibipixelsPerSecond;

			if (tuxState.TeleportStartingLocation != null)
				newIsFacingRight = tuxState.TeleportStartingLocation.Item1 > this.xMibi;
			else
				newIsFacingRight = tuxState.XMibi > this.xMibi;

			if (newYSpeedInMibipixelsPerSecond >= -5000 * 1000)
				newYSpeedInMibipixelsPerSecond -= elapsedMicrosPerFrame * 3;

			bool isOnGround = IsGroundOrSpike(tilemap: tilemap, x: this.xMibi >> 10, y: (newYMibi >> 10) - 8 * 3)
				|| IsGroundOrSpike(tilemap: tilemap, x: (this.xMibi >> 10) - 7 * 3, y: (newYMibi >> 10) - 8 * 3)
				|| IsGroundOrSpike(tilemap: tilemap, x: (this.xMibi >> 10) + 7 * 3, y: (newYMibi >> 10) - 8 * 3);

			if (isOnGround)
				newYSpeedInMibipixelsPerSecond = 1100000;

			newYMibi = (int)(((long)newYMibi) + ((long)newYSpeedInMibipixelsPerSecond) * ((long)elapsedMicrosPerFrame) / 1000L / 1000L);
			
			bool isOutOfBounds = (this.xMibi >> 10) + 8 * 3 < cameraX - (windowWidth >> 1) - GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (this.xMibi >> 10) - 8 * 3 > cameraX + (windowWidth >> 1) + GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (newYMibi >> 10) + 8 * 3 < cameraY - (windowHeight >> 1) - GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (newYMibi >> 10) - 8 * 3 > cameraY + (windowHeight >> 1) + GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS;
			
			if (!isOutOfBounds)
				list.Add(new EnemyBouncecap(
					xMibi: this.xMibi,
					yMibi: newYMibi,
					isFacingRight: newIsFacingRight,
					ySpeedInMibipixelsPerSecond: newYSpeedInMibipixelsPerSecond,
					enemyId: this.EnemyId));

			return new EnemyProcessing.Result(
				enemies: list,
				newlyKilledEnemies: new List<string>(),
				newlyAddedLevelFlags: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			GameImage image = this.isFacingRight ? GameImage.Bouncecap : GameImage.BouncecapMirrored;

			displayOutput.DrawImageRotatedClockwise(
				image: image,
				x: (this.xMibi >> 10) - 8 * 3,
				y: (this.yMibi >> 10) - 8 * 3,
				degreesScaled: 0,
				scalingFactorScaled: 128 * 3);
		}
	}
}
