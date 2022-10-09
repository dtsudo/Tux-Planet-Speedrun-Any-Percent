
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class EnemyYetiBoss_Defeated : IEnemy
	{
		private int xMibi;
		private int yMibi;
		private int elapsedMicros;

		private bool isFacingRight;

		public string EnemyId { get; private set; }

		private EnemyYetiBoss_Defeated(
			int xMibi,
			int yMibi,
			int elapsedMicros,
			bool isFacingRight,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.elapsedMicros = elapsedMicros;
			this.isFacingRight = isFacingRight;
			this.EnemyId = enemyId;
		}

		public static EnemyYetiBoss_Defeated GetEnemyYetiBoss_Defeated(
			int xMibi,
			int yMibi,
			int elapsedMicros,
			bool isFacingRight,
			string enemyId)
		{
			return new EnemyYetiBoss_Defeated(
				xMibi: xMibi,
				yMibi: yMibi,
				elapsedMicros: elapsedMicros,
				isFacingRight: isFacingRight,
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

			if (newElapsedMicros > 2 * 1000 * 1000 * 1000)
				newElapsedMicros = 1;

			List<IEnemy> newEnemies = new List<IEnemy>();

			newEnemies.Add(new EnemyYetiBoss_Defeated(
				xMibi: this.xMibi,
				yMibi: this.yMibi,
				elapsedMicros: newElapsedMicros,
				isFacingRight: this.isFacingRight,
				enemyId: this.EnemyId));

			return new EnemyProcessing.Result(
				enemiesImmutableNullable: newEnemies,
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			int spriteNum = (this.elapsedMicros / (100 * 1000)) % 2;

			displayOutput.DrawImageRotatedClockwise(
				image: this.isFacingRight ? GameImage.Yeti : GameImage.YetiMirrored,
				imageX: 128 + spriteNum * 64,
				imageY: 4 * 64,
				imageWidth: 64,
				imageHeight: 64,
				x: (this.xMibi >> 10) - 32 * 3,
				y: (this.yMibi >> 10) - 32 * 3,
				degreesScaled: 0,
				scalingFactorScaled: 128 * 3);
		}
	}
}
