
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class CollisionProcessing_Enemies
	{
		public class Result
		{
			public Result(IReadOnlyList<IEnemy> newEnemies, IReadOnlyList<string> newlyKilledEnemies)
			{
				this.NewEnemies = new List<IEnemy>(newEnemies);
				this.NewlyKilledEnemies = new List<string>(newlyKilledEnemies);
			}

			public IReadOnlyList<IEnemy> NewEnemies { get; private set; }
			public IReadOnlyList<string> NewlyKilledEnemies { get; private set; }
		}

		public static Result ProcessFrame(
			IReadOnlyList<IEnemy> enemies)
		{
			List<IEnemy> finalEnemies = new List<IEnemy>();

			List<string> killedEnemies = new List<string>();

			int? indexOfRemoveKonqiEnemy = null;
			int? indexOfKonqiEnemy = null;
			string konqiEnemyId = null;
			Tuple<int, int> konqiLocation = null;

			for (int i = 0; i < enemies.Count; i++)
			{
				if (enemies[i].IsRemoveKonqi)
					indexOfRemoveKonqiEnemy = i;

				if (enemies[i].IsKonqiCutscene)
				{
					indexOfKonqiEnemy = i;
					konqiLocation = enemies[i].GetKonqiCutsceneLocation();
					konqiEnemyId = enemies[i].EnemyId;
				}

				finalEnemies.Add(enemies[i]);
			}

			if (indexOfRemoveKonqiEnemy.HasValue && indexOfKonqiEnemy.HasValue)
			{
				List<IEnemy> newFinalEnemies = new List<IEnemy>();

				for (int i = 0; i < finalEnemies.Count; i++)
				{
					if (!finalEnemies[i].IsRemoveKonqi && !finalEnemies[i].IsKonqiCutscene)
						newFinalEnemies.Add(finalEnemies[i]);
				}

				newFinalEnemies.Add(EnemyKonqiDisappear.GetEnemyKonqiDisappear(
					xMibi: konqiLocation.Item1 << 10,
					yMibi: konqiLocation.Item2 << 10,
					enemyId: "konqiDisappear"));

				finalEnemies = newFinalEnemies;

				killedEnemies.Add(konqiEnemyId);
			}

			return new Result(
				newEnemies: finalEnemies,
				newlyKilledEnemies: killedEnemies);
		}
	}
}
