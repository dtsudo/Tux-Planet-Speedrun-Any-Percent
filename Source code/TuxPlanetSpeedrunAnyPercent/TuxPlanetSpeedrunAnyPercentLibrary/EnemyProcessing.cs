
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class EnemyProcessing
	{
		public class Result
		{
			public Result(
				IReadOnlyList<IEnemy> enemiesImmutableNullable,
				IReadOnlyList<string> newlyKilledEnemiesImmutableNullable,
				IReadOnlyList<string> newlyAddedLevelFlagsImmutableNullable)
			{
				this.EnemiesNullable = enemiesImmutableNullable;
				this.NewlyKilledEnemiesNullable = newlyKilledEnemiesImmutableNullable;
				this.NewlyAddedLevelFlagsNullable = newlyAddedLevelFlagsImmutableNullable;
			}

			public IReadOnlyList<IEnemy> EnemiesNullable { get; private set; }
			public IReadOnlyList<string> NewlyKilledEnemiesNullable { get; private set; }
			public IReadOnlyList<string> NewlyAddedLevelFlagsNullable { get; private set; }
		}

		public static Result ProcessFrame(
			ITilemap tilemap,
			int cameraX,
			int cameraY,
			int windowWidth,
			int windowHeight,
			TuxState tuxState,
			IDTDeterministicRandom random,
			IReadOnlyList<IEnemy> enemies,
			IReadOnlyList<string> killedEnemies,
			IReadOnlyList<string> levelFlags,
			ISoundOutput<GameSound> soundOutput,
			int elapsedMicrosPerFrame)
		{
			int enemiesCount = enemies.Count;
			int killedEnemiesCount = killedEnemies.Count;

			HashSet<string> newlyAddedLevelFlags = new HashSet<string>();

			HashSet<string> existingAndKilledEnemies = new HashSet<string>();
			for (int i = 0; i < enemiesCount; i++)
				existingAndKilledEnemies.Add(enemies[i].EnemyId);

			HashSet<string> killedEnemiesSet = new HashSet<string>();
			for (int i = 0; i < killedEnemiesCount; i++)
			{
				string killedEnemy = killedEnemies[i];
				killedEnemiesSet.Add(killedEnemy);
				existingAndKilledEnemies.Add(killedEnemy);
			}

			List<IEnemy> newEnemies = new List<IEnemy>();
			List<string> newlyKilledEnemies = new List<string>();

			IReadOnlyList<IEnemy> potentialNewEnemies = tilemap.GetEnemies(xOffset: 0, yOffset: 0);
			int potentialNewEnemiesCount = potentialNewEnemies.Count;
			for (int i = 0; i < potentialNewEnemiesCount; i++)
			{
				IEnemy potentialNewEnemy = potentialNewEnemies[i];
				if (!existingAndKilledEnemies.Contains(potentialNewEnemy.EnemyId))
					newEnemies.Add(potentialNewEnemy);
			}

			List<IEnemy> processedEnemies = new List<IEnemy>();
			HashSet<string> processedEnemiesSet = new HashSet<string>();

			for (int enemyIndex = 0; enemyIndex < enemiesCount; enemyIndex++)
			{
				IEnemy enemy = enemies[enemyIndex];

				Result result = enemy.ProcessFrame(
					cameraX: cameraX,
					cameraY: cameraY,
					windowWidth: windowWidth,
					windowHeight: windowHeight,
					elapsedMicrosPerFrame: elapsedMicrosPerFrame,
					tuxState: tuxState,
					random: random,
					tilemap: tilemap,
					levelFlags: levelFlags,
					soundOutput: soundOutput);

				if (result.EnemiesNullable != null)
				{
					int count = result.EnemiesNullable.Count;
					for (int i = 0; i < count; i++)
					{
						IEnemy e = result.EnemiesNullable[i];
						if (!killedEnemiesSet.Contains(e.EnemyId))
						{
							bool wasAdded = processedEnemiesSet.Add(e.EnemyId);
							if (wasAdded)
								processedEnemies.Add(e);
						}
					}
				}

				if (result.NewlyKilledEnemiesNullable != null)
					newlyKilledEnemies.AddRange(result.NewlyKilledEnemiesNullable);

				if (result.NewlyAddedLevelFlagsNullable != null)
				{
					int count = result.NewlyAddedLevelFlagsNullable.Count;
					for (int i = 0; i < count; i++)
						newlyAddedLevelFlags.Add(result.NewlyAddedLevelFlagsNullable[i]);
				}
			}

			int newEnemiesCount = newEnemies.Count;
			for (int newEnemiesIndex = 0; newEnemiesIndex < newEnemiesCount; newEnemiesIndex++)
			{
				IEnemy enemy = newEnemies[newEnemiesIndex];

				Result result = enemy.ProcessFrame(
					cameraX: cameraX,
					cameraY: cameraY,
					windowWidth: windowWidth,
					windowHeight: windowHeight,
					elapsedMicrosPerFrame: elapsedMicrosPerFrame,
					tuxState: tuxState,
					random: random,
					tilemap: tilemap,
					levelFlags: levelFlags,
					soundOutput: soundOutput);

				if (result.EnemiesNullable != null)
				{
					int count = result.EnemiesNullable.Count;
					for (int i = 0; i < count; i++)
					{
						IEnemy e = result.EnemiesNullable[i];
						if (!killedEnemiesSet.Contains(e.EnemyId))
						{
							bool wasAdded = processedEnemiesSet.Add(e.EnemyId);
							if (wasAdded)
								processedEnemies.Add(e);
						}
					}
				}

				if (result.NewlyKilledEnemiesNullable != null)
					newlyKilledEnemies.AddRange(result.NewlyKilledEnemiesNullable);

				if (result.NewlyAddedLevelFlagsNullable != null)
				{
					int count = result.NewlyAddedLevelFlagsNullable.Count;
					for (int i = 0; i < count; i++)
						newlyAddedLevelFlags.Add(result.NewlyAddedLevelFlagsNullable[i]);
				}
			}

			return new Result(
				enemiesImmutableNullable: processedEnemies,
				newlyKilledEnemiesImmutableNullable: newlyKilledEnemies,
				newlyAddedLevelFlagsImmutableNullable: newlyAddedLevelFlags.ToList());
		}
	}
}
