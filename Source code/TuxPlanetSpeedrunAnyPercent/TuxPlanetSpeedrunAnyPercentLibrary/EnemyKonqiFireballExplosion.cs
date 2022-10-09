
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyKonqiFireballExplosion : IEnemy
	{
		private int xMibi;
		private int yMibi;
		private int elapsedMicros;

		private const int EXPLOSION_ANIMATION_DURATION = 500 * 1000;

		public string EnemyId { get; private set; }

		private EnemyKonqiFireballExplosion(
			int xMibi,
			int yMibi,
			int elapsedMicros,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.elapsedMicros = elapsedMicros;
			this.EnemyId = enemyId;
		}

		public static EnemyKonqiFireballExplosion GetEnemyKonqiFireballExplosion(
			int xMibi,
			int yMibi,
			string enemyId)
		{
			return new EnemyKonqiFireballExplosion(
				xMibi: xMibi,
				yMibi: yMibi,
				elapsedMicros: 0,
				enemyId: enemyId);
		}

		public IReadOnlyList<Hitbox> GetHitboxes()
		{
			return new List<Hitbox>()
			{
				new Hitbox(
					x: (this.xMibi >> 10) - 8 * 3,
					y: (this.yMibi >> 10) - 8 * 3,
					width: 16 * 3,
					height: 16 * 3)
			};
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
			int newElapsedMicros = this.elapsedMicros + elapsedMicrosPerFrame;

			if (newElapsedMicros > EXPLOSION_ANIMATION_DURATION)
			{
				return new EnemyProcessing.Result(
					enemiesImmutableNullable: null,
					newlyKilledEnemiesImmutableNullable: null,
					newlyAddedLevelFlagsImmutableNullable: null);
			}

			List<IEnemy> newEnemies = new List<IEnemy>();

			newEnemies.Add(new EnemyKonqiFireballExplosion(
				xMibi: this.xMibi,
				yMibi: this.yMibi,
				elapsedMicros: newElapsedMicros,
				enemyId: this.EnemyId));

			return new EnemyProcessing.Result(
				enemiesImmutableNullable: newEnemies,
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			int spriteNum = this.elapsedMicros / (EXPLOSION_ANIMATION_DURATION / 5);

			if (spriteNum > 4)
				spriteNum = 4;

			displayOutput.DrawImageRotatedClockwise(
				image: GameImage.ExplodeF,
				imageX: spriteNum * 24,
				imageY: 0,
				imageWidth: 24,
				imageHeight: 24,
				x: (this.xMibi >> 10) - 12 * 3,
				y: (this.yMibi >> 10) - 12 * 3,
				degreesScaled: 0,
				scalingFactorScaled: 128 * 3);
		}

		public IEnemy GetDeadEnemy()
		{
			return null;
		}
	}
}
