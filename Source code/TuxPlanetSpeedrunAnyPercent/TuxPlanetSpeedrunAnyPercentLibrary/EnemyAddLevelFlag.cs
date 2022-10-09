
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyAddLevelFlag : IEnemy
	{
		private string levelFlag;

		public string EnemyId { get; private set; }

		public EnemyAddLevelFlag(
			string levelFlag,
			string enemyId)
		{
			this.levelFlag = levelFlag;
			this.EnemyId = enemyId;
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
			return new EnemyProcessing.Result(
				enemiesImmutableNullable: null,
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: new List<string>() { this.levelFlag });
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
		}

		public IEnemy GetDeadEnemy()
		{
			return null;
		}
	}
}
