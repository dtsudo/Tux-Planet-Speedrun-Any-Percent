
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemySnail : IEnemy
	{
		public class EnemySnailSpawn : Tilemap.IExtraEnemyToSpawn
		{
			private int xMibi;
			private int yMibi;
			private bool isFacingRight;
			private string enemyId;

			public EnemySnailSpawn(
				int xMibi,
				int yMibi,
				bool isFacingRight,
				string enemyId)
			{
				this.xMibi = xMibi;
				this.yMibi = yMibi;
				this.isFacingRight = isFacingRight;
				this.enemyId = enemyId;
			}

			public IEnemy GetEnemy(int xOffset, int yOffset)
			{
				return new EnemySpawnHelper(
					enemyToSpawn: GetEnemySnail(
						xMibi: this.xMibi + (xOffset << 10),
						yMibi: this.yMibi + (yOffset << 10),
						isFacingRight: this.isFacingRight,
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

		private int elapsedMicros;

		public string EnemyId { get; private set; }

		private EnemySnail(
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

		public static EnemySnail GetEnemySnail(
			int xMibi,
			int yMibi,
			bool isFacingRight,
			string enemyId)
		{
			return new EnemySnail(
				xMibi: xMibi,
				yMibi: yMibi,
				isFacingRight: isFacingRight,
				elapsedMicros: 0,
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

			bool isOutOfBounds = (newXMibi >> 10) + 8 * 3 < cameraX - windowWidth / 2 - GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (newXMibi >> 10) - 8 * 3 > cameraX + windowWidth / 2 + GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (newYMibi >> 10) + 8 * 3 < cameraY - windowHeight / 2 - GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS
				|| (newYMibi >> 10) - 8 * 3 > cameraY + windowHeight / 2 + GameLogicState.MARGIN_FOR_ENEMY_DESPAWN_IN_PIXELS;
			
			if (!isOutOfBounds)
				list.Add(new EnemySnail(
					xMibi: newXMibi,
					yMibi: newYMibi,
					isFacingRight: newIsFacingRight,
					elapsedMicros: newElapsedMicros,
					enemyId: this.EnemyId));

			return new EnemyProcessing.Result(
				enemiesImmutableNullable: list,
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			GameImage image = this.isFacingRight ? GameImage.SnailBlue : GameImage.SnailBlueMirrored;

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
