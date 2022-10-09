
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyLevel10Coordinator : IEnemy
	{
		private int bossRoomXOffsetStart;
		private int bossRoomXOffsetEnd;

		public string EnemyId { get { return "EnemyLevel10Coordinator"; } }

		public EnemyLevel10Coordinator(
			int bossRoomXOffsetStart,
			int bossRoomXOffsetEnd)
		{
			this.bossRoomXOffsetStart = bossRoomXOffsetStart;
			this.bossRoomXOffsetEnd = bossRoomXOffsetEnd;
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
			List<IEnemy> newEnemies = new List<IEnemy>()
			{
				this
			};

			List<string> newlyAddedLevelFlags = new List<string>();

			if (tuxState.XMibi >= 122 * 48 * 1024)
				newlyAddedLevelFlags.Add(EnemyLevel10EliteSnailPassive.CAN_BECOME_ACTIVE_LEVEL_FLAG);

			if ((tuxState.XMibi >> 10) >= this.bossRoomXOffsetStart)
			{
				if ((tuxState.YMibi >> 10) > 19 * 48 + 3)
				{
					newlyAddedLevelFlags.Add(LevelConfiguration_Level10.BEGIN_YETI_INTRO_CUTSCENE);
					newlyAddedLevelFlags.Add(LevelConfiguration_Level10.MARK_YETI_FLOOR_AS_GROUND);
				}
			}

			return new EnemyProcessing.Result(
				enemiesImmutableNullable: newEnemies,
				newlyKilledEnemiesImmutableNullable: null,
				newlyAddedLevelFlagsImmutableNullable: newlyAddedLevelFlags);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
		}
	}
}
