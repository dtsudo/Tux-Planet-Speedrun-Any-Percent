
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class EnemyProcessing
	{
		public class Result
		{
			public Result(
				IReadOnlyList<IEnemy> enemies,
				IReadOnlyList<string> newlyKilledEnemies)
			{
				this.Enemies = new List<IEnemy>(enemies);
				this.NewlyKilledEnemies = new List<string>(newlyKilledEnemies);
			}

			public IReadOnlyList<IEnemy> Enemies { get; private set; }
			public IReadOnlyList<string> NewlyKilledEnemies { get; private set; }
		}

		public static Result ProcessFrame(
			ITilemap tilemap,
			int cameraX,
			int cameraY,
			int windowWidth,
			int windowHeight,
			IReadOnlyList<IEnemy> enemies,
			IReadOnlyList<string> killedEnemies,
			int elapsedMicrosPerFrame)
		{
			HashSet<string> existingEnemies = new HashSet<string>();
			foreach (IEnemy enemy in enemies)
				existingEnemies.Add(enemy.EnemyId);

			HashSet<string> killedEnemiesSet = new HashSet<string>();
			foreach (string killedEnemy in killedEnemies)
				killedEnemiesSet.Add(killedEnemy);

			List<IEnemy> newEnemies = new List<IEnemy>();
			List<string> newlyKilledEnemies = new List<string>();

			IReadOnlyList<IEnemy> potentialNewEnemies = tilemap.GetEnemies(xOffset: 0, yOffset: 0);
			foreach (IEnemy potentialNewEnemy in potentialNewEnemies)
			{
				if (!existingEnemies.Contains(potentialNewEnemy.EnemyId) && !killedEnemiesSet.Contains(potentialNewEnemy.EnemyId))
					newEnemies.Add(potentialNewEnemy);
			}

			List<IEnemy> processedEnemies = new List<IEnemy>();
			HashSet<string> processedEnemiesSet = new HashSet<string>();

			foreach (IEnemy enemy in enemies)
			{
				Result result = enemy.ProcessFrame(
					cameraX: cameraX,
					cameraY: cameraY,
					windowWidth: windowWidth,
					windowHeight: windowHeight,
					elapsedMicrosPerFrame: elapsedMicrosPerFrame,
					tilemap: tilemap);

				foreach (IEnemy e in result.Enemies)
				{
					if (!processedEnemiesSet.Contains(e.EnemyId) && !killedEnemiesSet.Contains(e.EnemyId))
					{
						processedEnemies.Add(e);
						processedEnemiesSet.Add(e.EnemyId);
					}
				}
				newlyKilledEnemies.AddRange(result.NewlyKilledEnemies);
			}

			foreach (IEnemy enemy in newEnemies)
			{
				Result result = enemy.ProcessFrame(
					cameraX: cameraX,
					cameraY: cameraY,
					windowWidth: windowWidth,
					windowHeight: windowHeight,
					elapsedMicrosPerFrame: elapsedMicrosPerFrame,
					tilemap: tilemap);

				foreach (IEnemy e in result.Enemies)
				{
					if (!processedEnemiesSet.Contains(e.EnemyId) && !killedEnemiesSet.Contains(e.EnemyId))
					{
						processedEnemies.Add(e);
						processedEnemiesSet.Add(e.EnemyId);
					}
				}
				newlyKilledEnemies.AddRange(result.NewlyKilledEnemies);
			}

			return new Result(
				enemies: processedEnemies,
				newlyKilledEnemies: newlyKilledEnemies);
		}
	}
}
