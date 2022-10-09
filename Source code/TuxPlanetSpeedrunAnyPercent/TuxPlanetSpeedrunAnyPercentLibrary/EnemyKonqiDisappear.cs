
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyKonqiDisappear : IEnemy
	{
		private int xMibi;
		private int yMibi;
		private int elapsedMicros;

		public string EnemyId { get; private set; }

		private const int ANIMATION_DURATION = 600 * 1000;

		public static EnemyKonqiDisappear GetEnemyKonqiDisappear(
			int xMibi,
			int yMibi,
			string enemyId)
		{
			return new EnemyKonqiDisappear(
				xMibi: xMibi,
				yMibi: yMibi,
				elapsedMicros: 0,
				enemyId: enemyId);
		}

		private EnemyKonqiDisappear(
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

		public IReadOnlyList<Hitbox> GetHitboxes()
		{
			return null;
		}

		public IReadOnlyList<Hitbox> GetDamageBoxes()
		{
			return null;
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
			if (this.elapsedMicros > ANIMATION_DURATION)
				return new EnemyProcessing.Result(
					enemiesImmutableNullable: null,
					newlyKilledEnemiesImmutableNullable: null,
					newlyAddedLevelFlagsImmutableNullable: null);

			return new EnemyProcessing.Result(
				enemiesImmutableNullable: new List<IEnemy>()
				{
					new EnemyKonqiDisappear(
						xMibi: this.xMibi,
						yMibi: this.yMibi,
						elapsedMicros: this.elapsedMicros + elapsedMicrosPerFrame,
						enemyId: this.EnemyId)
				},
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			int spriteNum = this.elapsedMicros / (ANIMATION_DURATION / 4);

			if (spriteNum > 3)
				spriteNum = 3;

			displayOutput.DrawImageRotatedClockwise(
				image: GameImage.Flash,
				imageX: 32 * spriteNum,
				imageY: 0,
				imageWidth: 32,
				imageHeight: 40,
				x: (this.xMibi >> 10) - 16 * 3,
				y: (this.yMibi >> 10) - 20 * 3,
				degreesScaled: 0,
				scalingFactorScaled: 3 * 128);
		}
	}
}
