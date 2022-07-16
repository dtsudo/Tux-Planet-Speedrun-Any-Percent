
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public interface IEnemy
	{
		string EnemyId { get; }

		bool IsKonqi { get; }

		bool IsRemoveKonqi { get; }

		bool ShouldAlwaysSpawnRegardlessOfCamera { get; }

		Tuple<int, int> GetKonqiLocation();

		EnemyProcessing.Result ProcessFrame(
			int cameraX,
			int cameraY,
			int windowWidth,
			int windowHeight,
			int elapsedMicrosPerFrame,
			ITilemap tilemap);

		IReadOnlyList<Hitbox> GetHitboxes();

		IReadOnlyList<Hitbox> GetDamageBoxes();

		IEnemy GetDeadEnemy();

		void Render(IDisplayOutput<GameImage, GameFont> displayOutput);
	}
}
