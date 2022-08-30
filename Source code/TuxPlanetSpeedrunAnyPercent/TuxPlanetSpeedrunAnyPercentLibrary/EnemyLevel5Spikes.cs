
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
		private int endingXMibi;

		private List<Hitbox> emptyHitboxList;
		private List<string> emptyStringList;

		public string EnemyId { get; private set; }

		private EnemyLevel5Spikes(
			int xMibi,
			int yMibiBottom,
			int heightInTiles,
			int endingXMibi,
			List<Hitbox> emptyHitboxList,
			List<string> emptyStringList,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibiBottom = yMibiBottom;
			this.heightInTiles = heightInTiles;
			this.endingXMibi = endingXMibi;
			this.emptyHitboxList = emptyHitboxList;
			this.emptyStringList = emptyStringList;
			this.EnemyId = enemyId;
		}

		public static EnemyLevel5Spikes GetEnemyLevel5Spikes(
			int startingXMibi,
			int yMibiBottom,
			int heightInTiles,
			int endingXMibi,
			string enemyId)
		{
			return new EnemyLevel5Spikes(
				xMibi: startingXMibi,
				yMibiBottom: yMibiBottom,
				heightInTiles: heightInTiles,
				endingXMibi: endingXMibi,
				emptyHitboxList: new List<Hitbox>(),
				emptyStringList: new List<string>(),
				enemyId: enemyId);
		}

		public bool IsKonqiCutscene { get { return false; } }

		public bool IsRemoveKonqi { get { return false; } }

		public bool ShouldAlwaysSpawnRegardlessOfCamera { get { return true; } }

		public IEnemy GetDeadEnemy()
		{
			return null;
		}

		public Tuple<int, int> GetKonqiCutsceneLocation()
		{
			return null;
		}

		public IReadOnlyList<Hitbox> GetHitboxes()
		{
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
			return this.emptyHitboxList;
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
			int newXMibi = this.xMibi + elapsedMicrosPerFrame * 400 / 1000;

			if (newXMibi > this.endingXMibi)
				newXMibi = this.endingXMibi;

			return new EnemyProcessing.Result(
				enemies: new List<IEnemy>()
				{
					new EnemyLevel5Spikes(
						xMibi: newXMibi,
						yMibiBottom: this.yMibiBottom,
						heightInTiles: this.heightInTiles,
						endingXMibi: this.endingXMibi,
						emptyHitboxList: this.emptyHitboxList,
						emptyStringList: this.emptyStringList,
						enemyId: this.EnemyId)
				},
				newlyKilledEnemies: this.emptyStringList,
				newlyAddedLevelFlags: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			int y = this.yMibiBottom >> 10;

			for (int i = 0; i < this.heightInTiles; i++)
			{
				displayOutput.DrawImageRotatedClockwise(
					image: GameImage.Spikes,
					imageX: 7 * 16,
					imageY: 16,
					imageWidth: 16,
					imageHeight: 16,
					x: (this.xMibi >> 10) - 16 * 3 / 2,
					y: y,
					degreesScaled: 0,
					scalingFactorScaled: 3 * 128);

				y += 16 * 3;
			}
		}
	}
}
