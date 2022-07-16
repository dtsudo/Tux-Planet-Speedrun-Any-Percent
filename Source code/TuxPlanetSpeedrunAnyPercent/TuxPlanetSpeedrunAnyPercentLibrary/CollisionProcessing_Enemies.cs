
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
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

			for (int i = 0; i < enemies.Count; i++)
			{
				if (enemies[i].IsRemoveKonqi)
					continue;

				if (enemies[i].IsKonqi)
				{
					bool shouldRemoveKonqi = false;

					for (int j = 0; j < enemies.Count; j++)
					{
						if (enemies[j].IsRemoveKonqi)
						{
							shouldRemoveKonqi = true;
							break;
						}
					}

					if (shouldRemoveKonqi)
					{
						finalEnemies.Add(EnemyKonqiDisappear.GetEnemyKonqiDisappear(
							xMibi: (enemies[i].GetKonqiLocation().Item1) << 10,
							yMibi: (enemies[i].GetKonqiLocation().Item2) << 10,
							enemyId: "konqiDisappear"));

						killedEnemies.Add(enemies[i].EnemyId);
					}
					else
						finalEnemies.Add(enemies[i]);

					continue;
				}

				finalEnemies.Add(enemies[i]);
			}


			return new Result(
				newEnemies: finalEnemies,
				newlyKilledEnemies: killedEnemies);
		}
	}
}
