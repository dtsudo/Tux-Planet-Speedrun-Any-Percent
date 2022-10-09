
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class EnemyKonqiCutscene : IEnemy
	{
		private int x;
		private int y;
		private int elapsedMicros;

		private bool isFireKonqi;

		private string shouldTeleportOutLevelFlag;

		public const string SHOULD_TELEPORT_OUT_DEFAULT_LEVEL_FLAG = "EnemyKonqiCutscene_shouldTeleportOut_default";

		public string EnemyId { get; private set; }

		private EnemyKonqiCutscene(
			int x,
			int y,
			int elapsedMicros,
			bool isFireKonqi,
			string shouldTeleportOutLevelFlag,
			string enemyId)
		{
			this.x = x;
			this.y = y;
			this.elapsedMicros = elapsedMicros;
			this.isFireKonqi = isFireKonqi;
			this.shouldTeleportOutLevelFlag = shouldTeleportOutLevelFlag;
			this.EnemyId = enemyId;
		}

		public static EnemyKonqiCutscene GetEnemyKonqiCutscene(
			int xMibi,
			int yMibi,
			bool isFireKonqi,
			string shouldTeleportOutLevelFlag,
			string enemyId)
		{
			return new EnemyKonqiCutscene(
				x: xMibi >> 10,
				y: yMibi >> 10,
				elapsedMicros: 0,
				isFireKonqi: isFireKonqi,
				shouldTeleportOutLevelFlag: shouldTeleportOutLevelFlag,
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

			if (this.shouldTeleportOutLevelFlag != null && levelFlags.Contains(this.shouldTeleportOutLevelFlag))
			{
				return new EnemyProcessing.Result(
					enemiesImmutableNullable: new List<IEnemy>()
					{
						EnemyKonqiDisappear.GetEnemyKonqiDisappear(
							xMibi: this.x << 10,
							yMibi: this.y << 10,
							enemyId: this.EnemyId + "_konqiDisappear")
					},
					newlyKilledEnemiesImmutableNullable: new List<string>() { this.EnemyId },
					newlyAddedLevelFlagsImmutableNullable: null);
			}

			return new EnemyProcessing.Result(
				enemiesImmutableNullable: new List<IEnemy>()
				{
					new EnemyKonqiCutscene(
						x: this.x,
						y: this.y,
						elapsedMicros: newElapsedMicros,
						isFireKonqi: this.isFireKonqi,
						shouldTeleportOutLevelFlag: this.shouldTeleportOutLevelFlag,
						enemyId: this.EnemyId)
				},
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			int spriteNum = (this.elapsedMicros % 1000000) / 250000;

			displayOutput.DrawImageRotatedClockwise(
				image: this.isFireKonqi ? GameImage.KonqiFireMirrored : GameImage.KonqiMirrored,
				imageX: spriteNum * 32,
				imageY: 0,
				imageWidth: 32,
				imageHeight: 32,
				x: this.x - 16 * 3,
				y: this.y - 8 * 3,
				degreesScaled: 0,
				scalingFactorScaled: 128 * 3);
		}

		public IEnemy GetDeadEnemy()
		{
			return null;
		}
	}
}
