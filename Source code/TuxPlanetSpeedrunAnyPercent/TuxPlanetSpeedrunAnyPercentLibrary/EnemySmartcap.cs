
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemySmartcap : IEnemy
	{
		private int xMibi;
		private int yMibi;
		private bool isFacingRight;

		private int elapsedMicros;

		public string EnemyId { get; private set; }

		public static EnemySmartcap GetEnemySmartcap(
			int xMibi,
			int yMibi,
			bool isFacingRight,
			string enemyId)
		{
			return new EnemySmartcap(
				xMibi: xMibi,
				yMibi: yMibi,
				isFacingRight: isFacingRight,
				elapsedMicros: 0,
				enemyId: enemyId);
		}

		private EnemySmartcap(
			int xMibi,
			int yMibi,
			bool isFacingRight,
			int elapsedMicros,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.isFacingRight = isFacingRight;
			this.elapsedMicros = elapsedMicros;
			this.EnemyId = enemyId;
		}

		public bool IsKonqi { get { return false; } }

		public bool IsRemoveKonqi { get { return false; } }

		public bool ShouldAlwaysSpawnRegardlessOfCamera { get { return false; } }

		public Tuple<int, int> GetKonqiLocation()
		{
			return null;
		}

		public IReadOnlyList<Hitbox> GetHitboxes()
		{
			return new List<Hitbox>()
			{
				new Hitbox(
					x: (this.xMibi >> 10) - 8 * 3, 
					y: (this.yMibi >> 10) - 9 * 3, 
					width: 16 * 3, 
					height: 18 * 3)
			};
		}

		public IReadOnlyList<Hitbox> GetDamageBoxes()
		{
			return new List<Hitbox>()
			{
				new Hitbox(
					x: (this.xMibi >> 10) - 8 * 3,
					y: (this.yMibi >> 10) - 9 * 3,
					width: 16 * 3,
					height: 18 * 3)
			};
		}

		public IEnemy GetDeadEnemy()
		{
			string enemyId = this.EnemyId;

			return EnemySmartcapDead.SpawnEnemySmartcapDead(
				xMibi: this.xMibi,
				yMibi: this.yMibi,
				isFacingRight: this.isFacingRight,
				enemyId: enemyId + "enemySmartcapDead");
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
			ITilemap tilemap)
		{
			List<IEnemy> list = new List<IEnemy>();

			int newElapsedMicros = this.elapsedMicros + elapsedMicrosPerFrame;

			if (newElapsedMicros > 2 * 1000 * 1000 * 1000)
				newElapsedMicros = 0;

			int newXMibi = this.xMibi;
			int newYMibi = this.yMibi;
			bool newIsFacingRight = this.isFacingRight;

			if (this.isFacingRight)
				newXMibi += elapsedMicrosPerFrame * 90 / 1000;
			else
				newXMibi -= elapsedMicrosPerFrame * 90 / 1000;

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

			bool isOutOfBounds = (newXMibi >> 10) + 8 * 3 < cameraX - windowWidth / 2 - GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (newXMibi >> 10) - 8 * 3 > cameraX + windowWidth / 2 + GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (newYMibi >> 10) + 9 * 3 < cameraY - windowHeight / 2 - GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (newYMibi >> 10) - 9 * 3 > cameraY + windowHeight / 2 + GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS;
			
			if (!isOutOfBounds)
				list.Add(new EnemySmartcap(
					xMibi: newXMibi,
					yMibi: newYMibi,
					isFacingRight: newIsFacingRight,
					elapsedMicros: newElapsedMicros,
					enemyId: this.EnemyId));

			return new EnemyProcessing.Result(
				enemies: list,
				newlyKilledEnemies: new List<string>());
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			GameImage image = this.isFacingRight ? GameImage.Smartcap : GameImage.SmartcapMirrored;

			int spriteNum = (this.elapsedMicros % 1000000) / 250000;

			displayOutput.DrawImageRotatedClockwise(
				image: image,
				imageX: spriteNum * 16,
				imageY: 0,
				imageWidth: 16,
				imageHeight: 18,
				x: (this.xMibi >> 10) - 8 * 3,
				y: (this.yMibi >> 10) - 9 * 3,
				degreesScaled: 0,
				scalingFactorScaled: 128 * 3);
		}
	}
}
