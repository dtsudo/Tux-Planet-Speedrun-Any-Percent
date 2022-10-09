
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class EnemyYetiCutscene : IEnemy
	{
		private int x;
		private int y;
		private int elapsedMicros;

		private bool isFacingRight;

		private string rngSeed;

		public string EnemyId { get; private set; }

		public const string LEVEL_FLAG_SWITCH_DIRECTIONS = "enemyYetiCutscene_switchDirections";
		public const string LEVEL_FLAG_DESPAWN_YETI_CUTSCENE_AND_SPAWN_YETI_BOSS = "enemyYetiCutscene_despawnYetiCutsceneAndSpawnYetiBoss";

		private EnemyYetiCutscene(
			int x,
			int y,
			int elapsedMicros,
			bool isFacingRight,
			string rngSeed,
			string enemyId)
		{
			this.x = x;
			this.y = y;
			this.elapsedMicros = elapsedMicros;
			this.isFacingRight = isFacingRight;
			this.rngSeed = rngSeed;
			this.EnemyId = enemyId;
		}

		public static EnemyYetiCutscene GetEnemyYetiCutscene(
			int xMibi,
			int yMibi,
			string rngSeed,
			string enemyId)
		{
			return new EnemyYetiCutscene(
				x: xMibi >> 10,
				y: yMibi >> 10,
				elapsedMicros: 0,
				isFacingRight: true,
				rngSeed: rngSeed,
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
			int newElapsedMicros = this.elapsedMicros + elapsedMicrosPerFrame;

			if (newElapsedMicros > 2 * 1000 * 1000 * 1000)
				newElapsedMicros = 1;

			bool newIsFacingRight = this.isFacingRight;

			if (newIsFacingRight && levelFlags.Contains(LEVEL_FLAG_SWITCH_DIRECTIONS))
				newIsFacingRight = false;

			if (levelFlags.Contains(LEVEL_FLAG_DESPAWN_YETI_CUTSCENE_AND_SPAWN_YETI_BOSS))
			{
				return new EnemyProcessing.Result(
					enemiesImmutableNullable: new List<IEnemy>()
					{
						EnemyYetiBoss_Jump.GetEnemyYetiBoss_Jump(
							xMibi: this.x << 10,
							yMibi: this.y << 10,
							elapsedMicros: 0,
							isFacingRight: false,
							enemyIdCounter: 1,
							numTimesHit: 0,
							rngSeed: this.rngSeed,
							enemyId: "EnemyYetiBoss")
					},
					newlyKilledEnemiesImmutableNullable: null,
					newlyAddedLevelFlagsImmutableNullable: null);
			}

			return new EnemyProcessing.Result(
				enemiesImmutableNullable: new List<IEnemy>()
				{
					new EnemyYetiCutscene(
						x: this.x,
						y: this.y,
						elapsedMicros: newElapsedMicros,
						isFacingRight: newIsFacingRight,
						rngSeed: this.rngSeed,
						enemyId: this.EnemyId)
				},
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			int spriteNum = (this.elapsedMicros / 250000) % 8;

			displayOutput.DrawImageRotatedClockwise(
				image: this.isFacingRight ? GameImage.Yeti : GameImage.YetiMirrored,
				imageX: spriteNum * 64,
				imageY: 0,
				imageWidth: 64,
				imageHeight: 64,
				x: this.x - 32 * 3,
				y: this.y - 32 * 3,
				degreesScaled: 0,
				scalingFactorScaled: 128 * 3);
		}
	}
}
