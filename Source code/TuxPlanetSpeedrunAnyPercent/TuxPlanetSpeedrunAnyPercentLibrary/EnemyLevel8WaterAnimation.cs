
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

		public bool IsKonqiCutscene { get { return false; } }

		public bool IsRemoveKonqi { get { return false; } }

		public bool ShouldAlwaysSpawnRegardlessOfCamera { get { return true; } }

		public Tuple<int, int> GetKonqiCutsceneLocation()
		{
			return null;
		}

		public IReadOnlyList<Hitbox> GetHitboxes()
		{
			return new List<Hitbox>();
		}

		public IReadOnlyList<Hitbox> GetDamageBoxes()
		{
			return new List<Hitbox>();
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
				enemies: new List<IEnemy>()
				{
					new EnemyLevel8WaterAnimation(elapsedMicros: newElapsedMicros, enemyId: this.EnemyId)
				},
				newlyKilledEnemies: new List<string>(),
				newlyAddedLevelFlags: null);
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
