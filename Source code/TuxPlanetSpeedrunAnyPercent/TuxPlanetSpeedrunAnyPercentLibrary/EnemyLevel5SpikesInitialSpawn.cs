
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyLevel5SpikesInitialSpawn : IEnemy
	{
		private IEnemy enemyLevel5Spikes;

		private bool hasSpawnedSpikes;

		public string EnemyId { get; private set; }

		private EnemyLevel5SpikesInitialSpawn(
			IEnemy enemyLevel5Spikes,
			bool hasSpawnedSpikes,
			string enemyId)
		{
			this.enemyLevel5Spikes = enemyLevel5Spikes;
			this.hasSpawnedSpikes = hasSpawnedSpikes;
			this.EnemyId = enemyId;
		}

		public static EnemyLevel5SpikesInitialSpawn GetEnemyLevel5SpikesInitialSpawn(
			int xMibi,
			int startingXMibi,
			int endingXMibi,
			int yMibiBottom,
			int heightInTiles,
			string enemyIdPrefix,
			string enemyId)
		{
			return new EnemyLevel5SpikesInitialSpawn(
				enemyLevel5Spikes: EnemyLevel5Spikes.GetEnemyLevel5Spikes(
					xMibi: xMibi,
					startingXMibi: startingXMibi,
					endingXMibi: endingXMibi,
					yMibiBottom: yMibiBottom,
					heightInTiles: heightInTiles,
					enemyIdPrefix: enemyIdPrefix,
					enemyId: enemyId + "spikes"),
				hasSpawnedSpikes: false,
				enemyId: enemyId + "initialSpawn");
		}

		public IEnemy GetDeadEnemy()
		{
			return null;
		}

		public IReadOnlyList<Hitbox> GetHitboxes()
		{
			return null;
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
			if (this.hasSpawnedSpikes)
				return new EnemyProcessing.Result(
					enemiesImmutableNullable: new List<IEnemy>()
					{
						this
					},
					newlyKilledEnemiesImmutableNullable: null,
					newlyAddedLevelFlagsImmutableNullable: null);

			return new EnemyProcessing.Result(
				enemiesImmutableNullable: new List<IEnemy>()
				{
					new EnemyLevel5SpikesInitialSpawn(
						enemyLevel5Spikes: this.enemyLevel5Spikes,
						hasSpawnedSpikes: true,
						enemyId: this.EnemyId),
					this.enemyLevel5Spikes
				},
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
		}
	}
}
