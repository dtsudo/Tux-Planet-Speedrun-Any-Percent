
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyDeadPoof : IEnemy
	{
		private int x;
		private int y;

		private int elapsedMicros;

		public string EnemyId { get; private set; }

		private const int DEAD_ANIMATION_DURATION = 500 * 1000;

		private EnemyDeadPoof(
			int x,
			int y,
			int elapsedMicros,
			string enemyId)
		{
			this.x = x;
			this.y = y;
			this.elapsedMicros = elapsedMicros;
			this.EnemyId = enemyId;
		}

		public static EnemyDeadPoof SpawnEnemyDeadPoof(
			int xMibi,
			int yMibi,
			string enemyId)
		{
			return new EnemyDeadPoof(
				x: xMibi >> 10,
				y: yMibi >> 10,
				elapsedMicros: 0,
				enemyId: enemyId);
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
			int newElapsedMicros = this.elapsedMicros + elapsedMicrosPerFrame;

			if (newElapsedMicros >= DEAD_ANIMATION_DURATION)
				return new EnemyProcessing.Result(
					enemiesImmutableNullable: null,
					newlyKilledEnemiesImmutableNullable: null,
					newlyAddedLevelFlagsImmutableNullable: null);

			return new EnemyProcessing.Result(
				enemiesImmutableNullable: new List<IEnemy>()
				{
					new EnemyDeadPoof(
						x: this.x,
						y: this.y,
						enemyId: this.EnemyId,
						elapsedMicros: newElapsedMicros)
				},
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			int spriteNum = (this.elapsedMicros / (DEAD_ANIMATION_DURATION / 4)) % 4;

			displayOutput.DrawImageRotatedClockwise(
				image: GameImage.Poof,
				imageX: spriteNum * 16,
				imageY: 0,
				imageWidth: 16,
				imageHeight: 16,
				x: this.x - 8 * 3,
				y: this.y - 8 * 3,
				degreesScaled: 0,
				scalingFactorScaled: 128 * 3);
		}
	}
}
