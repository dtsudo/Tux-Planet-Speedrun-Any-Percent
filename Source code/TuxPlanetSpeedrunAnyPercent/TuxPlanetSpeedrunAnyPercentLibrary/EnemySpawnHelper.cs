
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemySpawnHelper : IEnemy
	{
		private IEnemy enemyToSpawn;

		private int xMibi;
		private int yMibi;

		private int enemyWidth;
		private int enemyHeight;

		public string EnemyId { get; private set; }

		public EnemySpawnHelper(
			IEnemy enemyToSpawn,
			int xMibi,
			int yMibi,
			int enemyWidth,
			int enemyHeight)
		{
			this.enemyToSpawn = enemyToSpawn;
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.enemyWidth = enemyWidth;
			this.enemyHeight = enemyHeight;
			this.EnemyId = enemyToSpawn.EnemyId + "_enemySpawnHelper";
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
			int halfWindowWidth = windowWidth >> 1;
			int halfWindowHeight = windowHeight >> 1;

			int halfEnemyWidth = this.enemyWidth >> 1;
			int halfEnemyHeight = this.enemyHeight >> 1;

			bool isOutOfCameraViewX = (this.xMibi >> 10) - halfEnemyWidth > cameraX + halfWindowWidth 
				|| (this.xMibi >> 10) + halfEnemyWidth < cameraX - halfWindowWidth;
			bool isOutOfCameraViewY = (this.yMibi >> 10) - halfEnemyHeight > cameraY + halfWindowHeight 
				|| (this.yMibi >> 10) + halfEnemyHeight < cameraY - halfWindowHeight;

			if (isOutOfCameraViewX || isOutOfCameraViewY)
			{
				return new EnemyProcessing.Result(
					enemiesImmutableNullable: new List<IEnemy>() { this.enemyToSpawn },
					newlyKilledEnemiesImmutableNullable: null,
					newlyAddedLevelFlagsImmutableNullable: null);
			}

			return new EnemyProcessing.Result(
				enemiesImmutableNullable: null,
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: null);

		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
		}
	}
}
