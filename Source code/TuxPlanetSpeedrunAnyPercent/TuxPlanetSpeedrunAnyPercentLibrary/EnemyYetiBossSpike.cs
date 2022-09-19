
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class EnemyYetiBossSpike : IEnemy
	{
		private int xMibi;
		private int yMibi;

		private bool isActive;
		private int cooldownUntilActive;

		public string EnemyId { get; private set; }

		public const string LEVEL_FLAG_DESPAWN_YETI_BOSS_ROOM_SPIKES = "despawnYetiBossRoomSpikes";

		private EnemyYetiBossSpike(
			int xMibi,
			int yMibi,
			bool isActive,
			int cooldownUntilActive,
			string enemyId)
		{
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.isActive = isActive;
			this.cooldownUntilActive = cooldownUntilActive;
			this.EnemyId = enemyId;
		}

		public static EnemyYetiBossSpike GetEnemyYetiBossSpike(
			int xMibi,
			int yMibi,
			int cooldownUntilActive,
			string enemyId)
		{
			return new EnemyYetiBossSpike(
				xMibi: xMibi,
				yMibi: yMibi,
				isActive: false,
				cooldownUntilActive: cooldownUntilActive,
				enemyId: enemyId);
		}

		public bool IsKonqiCutscene { get { return false; } }

		public bool IsRemoveKonqi { get { return false; } }

		public bool ShouldAlwaysSpawnRegardlessOfCamera { get { return false; } }

		public Tuple<int, int> GetKonqiCutsceneLocation()
		{
			return null;
		}

		public IReadOnlyList<Hitbox> GetHitboxes()
		{
			if (!this.isActive)
				return new List<Hitbox>();

			return new List<Hitbox>()
			{
				new Hitbox(
					x: (this.xMibi >> 10) - 8 * 3,
					y: (this.yMibi >> 10) - 8 * 3,
					width: 16 * 3,
					height: 16 * 3)
			};
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
			TuxState tuxState,
			IDTDeterministicRandom random,
			ITilemap tilemap,
			IReadOnlyList<string> levelFlags,
			ISoundOutput<GameSound> soundOutput)
		{
			if (levelFlags.Contains(LEVEL_FLAG_DESPAWN_YETI_BOSS_ROOM_SPIKES))
			{
				return new EnemyProcessing.Result(
					enemies: new List<IEnemy>()
					{
						EnemyDeadPoof.SpawnEnemyDeadPoof(
							xMibi: this.xMibi,
							yMibi: this.yMibi,
							enemyId: this.EnemyId + "_disappearPoof")
					},
					newlyKilledEnemies: new List<string>(),
					newlyAddedLevelFlags: null);
			}

			List<IEnemy> list = new List<IEnemy>();

			int newCooldownUntilActive;
			bool newIsActive;
			bool shouldSpawnEnemyDeadPoof;

			if (this.isActive)
			{
				newCooldownUntilActive = 0;
				newIsActive = true;
				shouldSpawnEnemyDeadPoof = false;
			}
			else
			{
				newCooldownUntilActive = this.cooldownUntilActive - elapsedMicrosPerFrame;
				if (newCooldownUntilActive <= 0)
				{
					newCooldownUntilActive = 0;
					newIsActive = true;
					shouldSpawnEnemyDeadPoof = true;
				}
				else
				{
					newIsActive = false;
					shouldSpawnEnemyDeadPoof = false;
				}
			}

			list.Add(new EnemyYetiBossSpike(
				xMibi: this.xMibi,
				yMibi: this.yMibi,
				isActive: newIsActive,
				cooldownUntilActive: newCooldownUntilActive,
				enemyId: this.EnemyId));

			if (shouldSpawnEnemyDeadPoof)
				list.Add(EnemyDeadPoof.SpawnEnemyDeadPoof(
					xMibi: this.xMibi,
					yMibi: this.yMibi,
					enemyId: this.EnemyId + "_spawnPoof"));

			return new EnemyProcessing.Result(
				enemies: list,
				newlyKilledEnemies: new List<string>(),
				newlyAddedLevelFlags: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			if (this.isActive)
				displayOutput.DrawImageRotatedClockwise(
					image: GameImage.Spikes,
					imageX: 16,
					imageY: 16,
					imageWidth: 16,
					imageHeight: 16,
					x: (this.xMibi >> 10) - 8 * 3,
					y: (this.yMibi >> 10) - 8 * 3,
					degreesScaled: 0,
					scalingFactorScaled: 128 * 3);
		}
	}
}
