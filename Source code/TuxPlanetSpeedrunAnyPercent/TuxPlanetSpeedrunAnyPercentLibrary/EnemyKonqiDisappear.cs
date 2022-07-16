
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyKonqiDisappear : IEnemy
	{
		private int xMibi;
		private int yMibi;
		private int elapsedMicros;

		private List<string> emptyStringList;
		private List<Hitbox> emptyHitboxList;

		public string EnemyId { get; private set; }

		public static EnemyKonqiDisappear GetEnemyKonqiDisappear(
			int xMibi,
			int yMibi,
			string enemyId)
		{
			return new EnemyKonqiDisappear(
				xMibi: xMibi,
				yMibi: yMibi,
				elapsedMicros: 0,
				emptyStringList: new List<string>(),
				emptyHitboxList: new List<Hitbox>(),
				enemyId: enemyId);
		}

		private EnemyKonqiDisappear(
			int xMibi,
			int yMibi,
			int elapsedMicros,
			List<string> emptyStringList,
			List<Hitbox> emptyHitboxList,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.elapsedMicros = elapsedMicros;
			this.emptyStringList = emptyStringList;
			this.emptyHitboxList = emptyHitboxList;
			this.EnemyId = enemyId;
		}

		public bool IsKonqi { get { return false; } }

		public bool IsRemoveKonqi { get { return false; } }

		public bool ShouldAlwaysSpawnRegardlessOfCamera { get { return true; } }

		public Tuple<int, int> GetKonqiLocation()
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
			ITilemap tilemap)
		{
			if (this.elapsedMicros > 200 * 1000)
				return new EnemyProcessing.Result(
					enemies: new List<IEnemy>(),
					newlyKilledEnemies: this.emptyStringList);

			return new EnemyProcessing.Result(
				enemies: new List<IEnemy>()
				{
					new EnemyKonqiDisappear(
						xMibi: this.xMibi,
						yMibi: this.yMibi,
						elapsedMicros: this.elapsedMicros + elapsedMicrosPerFrame,
						emptyStringList: this.emptyStringList,
						emptyHitboxList: this.emptyHitboxList,
						enemyId: this.EnemyId)
				},
				newlyKilledEnemies: this.emptyStringList);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			int distance = this.elapsedMicros / (1000 * 2);

			DTColor color = new DTColor(50, 168, 64);

			for (int i = 0; i < 360 * 128; i = i + 128 * 3)
			{
				int deltaX = DTMath.CosineScaled(degreesScaled: i) * distance;
				int deltaY = DTMath.SineScaled(degreesScaled: i) * distance;

				int x = (this.xMibi + deltaX) >> 10;
				int y = (this.yMibi + deltaY) >> 10;

				displayOutput.DrawRectangle(
					x: x - 2,
					y: y - 2,
					width: 5,
					height: 5,
					color: color,
					fill: true);
			}
		}
	}
}
