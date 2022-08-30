
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

		private List<string> emptyStringList;
		private List<Hitbox> emptyHitboxList;

		public string EnemyId { get; private set; }

		private const int DEAD_ANIMATION_DURATION = 500 * 1000;

		private EnemyDeadPoof(
			int x,
			int y,
			int elapsedMicros,
			List<string> emptyStringList,
			List<Hitbox> emptyHitboxList,
			string enemyId)
		{
			this.x = x;
			this.y = y;
			this.elapsedMicros = elapsedMicros;
			this.emptyStringList = emptyStringList;
			this.emptyHitboxList = emptyHitboxList;
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
				emptyStringList: new List<string>(),
				emptyHitboxList: new List<Hitbox>(),
				enemyId: enemyId);
		}

		public bool IsKonqiCutscene { get { return false; } }

		public bool IsRemoveKonqi { get { return false; } }

		public bool ShouldAlwaysSpawnRegardlessOfCamera { get { return false; } }

		public Tuple<int, int> GetKonqiCutsceneLocation()
		{
			return null;
		}

		public IReadOnlyList<Hitbox> GetHitboxes()
		{
			return this.emptyHitboxList;
		}

		public IReadOnlyList<Hitbox> GetDamageBoxes()
		{
			return this.emptyHitboxList;
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
					enemies: new List<IEnemy>(),
					newlyKilledEnemies: this.emptyStringList,
					newlyAddedLevelFlags: null);

			return new EnemyProcessing.Result(
				enemies: new List<IEnemy>()
				{
					new EnemyDeadPoof(
						x: this.x,
						y: this.y,
						enemyId: this.EnemyId,
						elapsedMicros: newElapsedMicros,
						emptyStringList: this.emptyStringList,
						emptyHitboxList: this.emptyHitboxList)
				},
				newlyKilledEnemies: this.emptyStringList,
				newlyAddedLevelFlags: null);
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
