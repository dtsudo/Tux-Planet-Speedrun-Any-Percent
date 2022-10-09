
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System.Collections.Generic;

	public class CollisionProcessing_Tux
	{
		public class Result
		{
			public Result(
				TuxState newTuxState, 
				IReadOnlyList<IEnemy> newEnemiesImmutable, 
				IReadOnlyList<string> newlyKilledEnemiesImmutableNullable)
			{
				this.NewTuxState = newTuxState;
				this.NewEnemies = newEnemiesImmutable;
				this.NewlyKilledEnemiesNullable = newlyKilledEnemiesImmutableNullable;
			}

			public TuxState NewTuxState { get; private set; }
			public IReadOnlyList<IEnemy> NewEnemies { get; private set; }
			public IReadOnlyList<string> NewlyKilledEnemiesNullable { get; private set; }
		}

		public static Result ProcessFrame(
			TuxState tuxState,
			IReadOnlyList<IEnemy> enemiesImmutable,
			bool debug_tuxInvulnerable,
			ISoundOutput<GameSound> soundOutput)
		{
			if (tuxState.IsDead || tuxState.TeleportInProgressElapsedMicros != null || tuxState.HasFinishedLevel)
				return new Result(newTuxState: tuxState, newEnemiesImmutable: enemiesImmutable, newlyKilledEnemiesImmutableNullable: null);

			List<IEnemy> newEnemies = new List<IEnemy>(capacity: enemiesImmutable.Count);
			List<string> newlyKilledEnemies = null;

			Hitbox tuxHitbox = tuxState.GetHitbox();

			TuxState newTuxState = tuxState;
			bool isTuxDead = false;

			int numEnemies = enemiesImmutable.Count;
			for (int enemiesIndex = 0; enemiesIndex < numEnemies; enemiesIndex++)
			{
				IEnemy enemy = enemiesImmutable[enemiesIndex];

				IReadOnlyList<Hitbox> enemyDamageBoxes = enemy.GetDamageBoxes();

				bool isSquished = false;
				bool hasCollided = false;

				if (enemyDamageBoxes != null)
				{
					int numDamageBoxes = enemyDamageBoxes.Count;
					for (int i = 0; i < numDamageBoxes; i++)
					{
						Hitbox enemyDamageBox = enemyDamageBoxes[i];

						if (HasCollided(tuxHitbox, enemyDamageBox))
						{
							isSquished = tuxHitbox.Y > enemyDamageBox.Y + (enemyDamageBox.Height >> 1) || tuxState.YSpeedInMibipixelsPerSecond < 0;
							break;
						}
					}
				}

				if (!isSquished)
				{
					IReadOnlyList<Hitbox> enemyHitboxes = enemy.GetHitboxes();
					if (enemyHitboxes != null)
					{
						int numHitboxes = enemyHitboxes.Count;
						for (int i = 0; i < numHitboxes; i++)
						{
							Hitbox enemyHitbox = enemyHitboxes[i];
							if (HasCollided(tuxHitbox, enemyHitbox))
							{
								hasCollided = true;
								break;
							}
						}
					}
				}

				if (isSquished)
				{
					soundOutput.PlaySound(GameSound.Squish);
					if (newlyKilledEnemies == null)
						newlyKilledEnemies = new List<string>();
					newlyKilledEnemies.Add(enemy.EnemyId);
					newEnemies.Add(enemy.GetDeadEnemy());

					newTuxState = newTuxState.SetYSpeedInMibipixelsPerSecond(ySpeedInMibipixelsPerSecond: TuxState.JUMP_Y_SPEED)
						.SetIsStillHoldingJumpButton(true)
						.SetLastTimeOnGround(null)
						.SetHasAlreadyUsedTeleport(false);
				}
				else if (hasCollided && !debug_tuxInvulnerable)
				{
					isTuxDead = true;
					newEnemies.Add(enemy);
				}
				else
				{
					newEnemies.Add(enemy);
				}
			}

			if (isTuxDead)
				newTuxState = newTuxState.Kill();

			return new Result(
				newTuxState: newTuxState,
				newEnemiesImmutable: newEnemies,
				newlyKilledEnemiesImmutableNullable: newlyKilledEnemies);
		}

		private static bool HasCollided(Hitbox a, Hitbox b)
		{
			if (a.X > b.X + b.Width)
				return false;

			if (b.X > a.X + a.Width)
				return false;

			if (a.Y > b.Y + b.Height)
				return false;

			if (b.Y > a.Y + a.Height)
				return false;

			return true;
		}
	}
}
