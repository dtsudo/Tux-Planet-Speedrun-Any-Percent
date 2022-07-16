
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyRemoveKonqiCutscene : IEnemy
	{
		public string EnemyId { get; private set; }

		public EnemyRemoveKonqiCutscene(string enemyId)
		{
			this.EnemyId = enemyId;
		}

		public bool IsKonqi { get { return false; } }

		public bool IsRemoveKonqi { get { return true; } }

		public bool ShouldAlwaysSpawnRegardlessOfCamera { get { return true; } }

		public Tuple<int, int> GetKonqiLocation()
		{
			return null;
		}

		public IReadOnlyList<Hitbox> GetHitboxes()
		{
			return new List<Hitbox>();
		}

		public IReadOnlyList<Hitbox> GetDamageBoxes()
		{
			return new List<Hitbox>();
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
			return new EnemyProcessing.Result(
				enemies: new List<IEnemy>()
				{
					this
				},
				newlyKilledEnemies: new List<string>());
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
		}
	}
}
