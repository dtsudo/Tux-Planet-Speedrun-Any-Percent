
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyLevel8WaterAnimation : IEnemy
	{
		private int elapsedMicros;

		public string EnemyId { get; private set; }

		public EnemyLevel8WaterAnimation(
			int elapsedMicros,
			string enemyId)
		{
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

			if (newElapsedMicros > 2 * 1000 * 1000 * 1000)
				newElapsedMicros = 1;

			return new EnemyProcessing.Result(
				enemiesImmutableNullable: new List<IEnemy>()
				{
					new EnemyLevel8WaterAnimation(elapsedMicros: newElapsedMicros, enemyId: this.EnemyId)
				},
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			int spriteNum = (this.elapsedMicros / (100 * 1000)) % 4;

			for (int i = 0; i < 75; i++)
			{
				spriteNum = (spriteNum + 1) % 4;

				displayOutput.DrawImageRotatedClockwise(
					image: GameImage.WaterSurface,
					imageX: spriteNum * 16,
					imageY: 0,
					imageWidth: 16,
					imageHeight: 4,
					x: i * 48,
					y: 8 * 48,
					degreesScaled: 0,
					scalingFactorScaled: 3 * 128);
			}

			displayOutput.DrawRectangle(
				x: 0,
				y: 0,
				width: 75 * 48,
				height: 8 * 48,
				color: new DTColor(96, 152, 248, 128),
				fill: true);
		}

		public IEnemy GetDeadEnemy()
		{
			return null;
		}
	}
}
