
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public interface IEnemy
	{
		string EnemyId { get; }

		EnemyProcessing.Result ProcessFrame(
			int cameraX,
			int cameraY,
			int windowWidth,
			int windowHeight,
			int elapsedMicrosPerFrame,
			TuxState tuxState,
			IDTDeterministicRandom random,
			ITilemap tilemap,
			IReadOnlyList<string> levelFlags,
			ISoundOutput<GameSound> soundOutput);

		IReadOnlyList<Hitbox> GetHitboxes();

		IReadOnlyList<Hitbox> GetDamageBoxes();

		IEnemy GetDeadEnemy();

		void Render(IDisplayOutput<GameImage, GameFont> displayOutput);
	}
}
