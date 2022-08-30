
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class EnemyKonqiCutscene : IEnemy
	{
		private int x;
		private int y;
		private int elapsedMicros;

		private List<string> emptyStringList;
		private List<Hitbox> emptyHitboxList;

		public string EnemyId { get; private set; }

		public static EnemyKonqiCutscene GetEnemyKonqiCutscene(
			int xMibi,
			int yMibi,
			string enemyId)
		{
			return new EnemyKonqiCutscene(
				x: xMibi >> 10,
				y: yMibi >> 10,
				elapsedMicros: 0,
				emptyStringList: new List<string>(),
				emptyHitboxList: new List<Hitbox>(),
				enemyId: enemyId);
		}

		private EnemyKonqiCutscene(
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

		public bool IsKonqiCutscene { get { return true; } }

		public bool IsRemoveKonqi { get { return false; } }

		public bool ShouldAlwaysSpawnRegardlessOfCamera { get { return true; } }

		public Tuple<int, int> GetKonqiCutsceneLocation()
		{
			return new Tuple<int, int>(this.x, this.y);
		}

		public IReadOnlyList<Hitbox> GetHitboxes()
		{
			return this.emptyHitboxList;
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
			int newElapsedMicros = this.elapsedMicros + elapsedMicrosPerFrame;

			if (newElapsedMicros > 2 * 1000 * 1000 * 1000)
				newElapsedMicros = 1;

			if (levelFlags.Contains(LevelConfiguration_Level6.BEGIN_BOSS_BATTLE))
			{
				return new EnemyProcessing.Result(
					enemies: new List<IEnemy>()
					{
						EnemyKonqiDisappear.GetEnemyKonqiDisappear(
							xMibi: this.x << 10,
							yMibi: this.y << 10,
							enemyId: this.EnemyId + "_konqiDisappear")
					},
					newlyKilledEnemies: this.emptyStringList,
					newlyAddedLevelFlags: null);
			}

			return new EnemyProcessing.Result(
				enemies: new List<IEnemy>()
				{
					new EnemyKonqiCutscene(
						x: this.x,
						y: this.y,
						elapsedMicros: newElapsedMicros,
						emptyStringList: this.emptyStringList,
						emptyHitboxList: this.emptyHitboxList,
						enemyId: this.EnemyId)
				},
				newlyKilledEnemies: this.emptyStringList,
				newlyAddedLevelFlags: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			int spriteNum = (this.elapsedMicros % 1000000) / 250000;

			displayOutput.DrawImageRotatedClockwise(
				image: GameImage.KonqiMirrored,
				imageX: spriteNum * 32,
				imageY: 0,
				imageWidth: 32,
				imageHeight: 32,
				x: this.x - 16 * 3,
				y: this.y - 8 * 3,
				degreesScaled: 0,
				scalingFactorScaled: 128 * 3);
		}

		public IEnemy GetDeadEnemy()
		{
			return null;
		}
	}
}
