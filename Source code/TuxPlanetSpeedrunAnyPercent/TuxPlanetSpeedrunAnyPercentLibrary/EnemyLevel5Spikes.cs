
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyLevel5Spikes : IEnemy
	{
		private int xMibi;
		private int yMibiBottom;
		private int heightInTiles;
		private int startingXMibi;
		private int endingXMibi;
		private bool hasSpawnedNextEnemy;
		private bool? isVisible;
		private string enemyIdPrefix;
		private int enemyGeneratorCounter;

		public string EnemyId { get; private set; }

		private const int NUM_PIXELS_BETWEEN_SPIKES = 4000;

		private EnemyLevel5Spikes(
			int xMibi,
			int yMibiBottom,
			int heightInTiles,
			int startingXMibi,
			int endingXMibi,
			bool hasSpawnedNextEnemy,
			bool? isVisible,
			string enemyIdPrefix,
			int enemyGeneratorCounter,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibiBottom = yMibiBottom;
			this.heightInTiles = heightInTiles;
			this.startingXMibi = startingXMibi;
			this.endingXMibi = endingXMibi;
			this.hasSpawnedNextEnemy = hasSpawnedNextEnemy;
			this.isVisible = isVisible;
			this.enemyIdPrefix = enemyIdPrefix;
			this.enemyGeneratorCounter = enemyGeneratorCounter;
			this.EnemyId = enemyId;
		}

		public static EnemyLevel5Spikes GetEnemyLevel5Spikes(
			int xMibi,
			int startingXMibi,
			int endingXMibi,
			int yMibiBottom,
			int heightInTiles,
			string enemyIdPrefix,
			string enemyId)
		{
			return new EnemyLevel5Spikes(
				xMibi: xMibi,
				yMibiBottom: yMibiBottom,
				heightInTiles: heightInTiles,
				startingXMibi: startingXMibi,
				endingXMibi: endingXMibi,
				hasSpawnedNextEnemy: false,
				isVisible: null,
				enemyIdPrefix: enemyIdPrefix,
				enemyGeneratorCounter: 1,
				enemyId: enemyId);
		}

		public IEnemy GetDeadEnemy()
		{
			return null;
		}

		public IReadOnlyList<Hitbox> GetHitboxes()
		{
			if (!this.isVisible.HasValue || this.isVisible.Value == false)
				return null;

			return new List<Hitbox>()
			{
				new Hitbox(
					x: (this.xMibi >> 10) - 16 * 3 / 2,
					y: this.yMibiBottom >> 10,
					width: 16 * 3,
					height: 16 * 3 * this.heightInTiles)
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
			List<IEnemy> list = new List<IEnemy>();

			int newXMibi = this.xMibi - elapsedMicrosPerFrame * 400 / 1000;

			bool hasExitedLevel = newXMibi < this.endingXMibi;

			bool? newIsVisible;

			if (this.isVisible.HasValue)
				newIsVisible = this.isVisible.Value;
			else
				newIsVisible = cameraX + (windowWidth >> 1) < (this.xMibi >> 10) - 16 * 3 / 2;

			bool newHasSpawnedNextEnemy = this.hasSpawnedNextEnemy;

			if (!newHasSpawnedNextEnemy)
			{
				if (this.startingXMibi - this.xMibi > (NUM_PIXELS_BETWEEN_SPIKES << 10))
				{
					newHasSpawnedNextEnemy = true;
					list.Add(new EnemyLevel5Spikes(
						xMibi: this.xMibi + (NUM_PIXELS_BETWEEN_SPIKES << 10),
						yMibiBottom: this.yMibiBottom,
						heightInTiles: this.heightInTiles,
						startingXMibi: this.startingXMibi,
						endingXMibi: this.endingXMibi,
						hasSpawnedNextEnemy: false,
						isVisible: null,
						enemyIdPrefix: this.enemyIdPrefix,
						enemyGeneratorCounter: this.enemyGeneratorCounter + 1,
						enemyId: this.enemyIdPrefix + this.enemyGeneratorCounter.ToStringCultureInvariant()));
				}
			}

			if (!hasExitedLevel)
				list.Add(new EnemyLevel5Spikes(
					xMibi: newXMibi,
					yMibiBottom: this.yMibiBottom,
					heightInTiles: this.heightInTiles,
					startingXMibi: this.startingXMibi,
					endingXMibi: this.endingXMibi,
					hasSpawnedNextEnemy: newHasSpawnedNextEnemy,
					isVisible: newIsVisible,
					enemyIdPrefix: this.enemyIdPrefix,
					enemyGeneratorCounter: this.enemyGeneratorCounter,
					enemyId: this.EnemyId));

			return new EnemyProcessing.Result(
				enemiesImmutableNullable: list,
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			if (!this.isVisible.HasValue || this.isVisible.Value == false)
				return;

			int y = this.yMibiBottom >> 10;
			int x = (this.xMibi >> 10) - 16 * 3 / 2;

			for (int i = 0; i < this.heightInTiles; i++)
			{
				displayOutput.DrawImageRotatedClockwise(
					image: GameImage.Spikes,
					imageX: 0,
					imageY: 0,
					imageWidth: 16,
					imageHeight: 16,
					x: x,
					y: y,
					degreesScaled: 0,
					scalingFactorScaled: 3 * 128);

				y += 16 * 3;
			}
		}
	}
}
