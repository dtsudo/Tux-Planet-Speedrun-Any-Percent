
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyDeadMultiplePoof : IEnemy
	{
		private IReadOnlyList<IEnemy> enemyDeadPoofList;

		public string EnemyId { get; private set; }

		private EnemyDeadMultiplePoof(
			IReadOnlyList<IEnemy> enemyDeadPoofList,
			string enemyId)
		{
			this.enemyDeadPoofList = new List<IEnemy>(enemyDeadPoofList);
			this.EnemyId = enemyId;
		}

		public static EnemyDeadMultiplePoof SpawnEnemyDeadMultiplePoof(
			IReadOnlyList<EnemyDeadPoof> enemyDeadPoofList,
			string enemyId)
		{
			return new EnemyDeadMultiplePoof(
				enemyDeadPoofList: enemyDeadPoofList,
				enemyId: enemyId);
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
			List<IEnemy> newEnemyDeadPoofList = new List<IEnemy>();
			List<string> newlyKilledEnemies = new List<string>();
			List<string> newlyAddedLevelFlags = new List<string>();

			for (int i = 0; i < this.enemyDeadPoofList.Count; i++)
			{
				EnemyProcessing.Result result = this.enemyDeadPoofList[i].ProcessFrame(
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
					newEnemyDeadPoofList.AddRange(result.EnemiesNullable);
				if (result.NewlyKilledEnemiesNullable != null)
					newlyKilledEnemies.AddRange(result.NewlyKilledEnemiesNullable);
				if (result.NewlyAddedLevelFlagsNullable != null)
				{
					foreach (string levelFlag in result.NewlyAddedLevelFlagsNullable)
					{
						if (!newlyAddedLevelFlags.Contains(levelFlag))
							newlyAddedLevelFlags.Add(levelFlag);
					}
				}
			}

			List<IEnemy> newEnemies = new List<IEnemy>();

			if (newEnemyDeadPoofList.Count > 0)
				newEnemies.Add(new EnemyDeadMultiplePoof(
						enemyDeadPoofList: newEnemyDeadPoofList,
						enemyId: this.EnemyId));

			return new EnemyProcessing.Result(
				enemiesImmutableNullable: newEnemies,
				newlyKilledEnemiesImmutableNullable: newlyKilledEnemies,
				newlyAddedLevelFlagsImmutableNullable: newlyAddedLevelFlags);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			for (int i = 0; i < this.enemyDeadPoofList.Count; i++)
				this.enemyDeadPoofList[i].Render(displayOutput: displayOutput);
		}
	}
}
