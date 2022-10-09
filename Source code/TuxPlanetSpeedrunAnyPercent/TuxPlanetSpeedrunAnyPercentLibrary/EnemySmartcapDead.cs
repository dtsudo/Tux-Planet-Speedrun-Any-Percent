
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemySmartcapDead : IEnemy
	{
		private int x;
		private int y;
		private bool isFacingRight;

		private int elapsedMicros;

		public string EnemyId { get; private set; }

		private const int DEAD_ANIMATION_DURATION = 1000 * 1000;

		public static EnemySmartcapDead SpawnEnemySmartcapDead(
			int xMibi,
			int yMibi,
			bool isFacingRight,
			string enemyId)
		{
			return new EnemySmartcapDead(
				x: (xMibi >> 10) - 8 * 3,
				y: (yMibi >> 10) - 9 * 3,
				isFacingRight: isFacingRight,
				elapsedMicros: 0,
				enemyId: enemyId);
		}

		private EnemySmartcapDead(
			int x,
			int y,
			bool isFacingRight,
			int elapsedMicros,
			string enemyId)
		{
			this.x = x;
			this.y = y;
			this.isFacingRight = isFacingRight;
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
			if (this.elapsedMicros > DEAD_ANIMATION_DURATION)
				return new EnemyProcessing.Result(
					enemiesImmutableNullable: null,
					newlyKilledEnemiesImmutableNullable: null,
					newlyAddedLevelFlagsImmutableNullable: null);

			return new EnemyProcessing.Result(
				enemiesImmutableNullable: new List<IEnemy>()
				{
					new EnemySmartcapDead(
						x: this.x, 
						y: this.y,
						isFacingRight: this.isFacingRight,
						enemyId: this.EnemyId,  
						elapsedMicros: this.elapsedMicros + elapsedMicrosPerFrame)
				},
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			displayOutput.DrawImageRotatedClockwise(
				image: this.isFacingRight ? GameImage.Smartcap : GameImage.SmartcapMirrored,
				imageX: 64,
				imageY: 0,
				imageWidth: 16,
				imageHeight: 18,
				x: this.x,
				y: this.y,
				degreesScaled: 0,
				scalingFactorScaled: 128 * 3);
		}

		public IEnemy GetDeadEnemy()
		{
			return null;
		}
	}
}
