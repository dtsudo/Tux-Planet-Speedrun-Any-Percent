
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
				IReadOnlyList<IEnemy> newEnemies, 
				IReadOnlyList<string> newlyKilledEnemies)
			{
				this.NewTuxState = newTuxState;
				this.NewEnemies = new List<IEnemy>(newEnemies);
				this.NewlyKilledEnemies = new List<string>(newlyKilledEnemies);
			}

			public TuxState NewTuxState { get; private set; }
			public IReadOnlyList<IEnemy> NewEnemies { get; private set; }
			public IReadOnlyList<string> NewlyKilledEnemies { get; private set; }
		}

		public static Result ProcessFrame(
			TuxState tuxState,
			IReadOnlyList<IEnemy> enemies,
			ISoundOutput<GameSound> soundOutput)
		{
			if (tuxState.IsDead || tuxState.HasFinishedLevel)
			{
				return new Result(newTuxState: tuxState, newEnemies: enemies, newlyKilledEnemies: new List<string>());
			}

			List<IEnemy> newEnemies = new List<IEnemy>();
			List<string> newlyKilledEnemies = new List<string>();

			List<Hitbox> tuxHitboxes = tuxState.GetHitboxes();

			TuxState newTuxState = tuxState;
			bool isTuxDead = false;

			foreach (IEnemy enemy in enemies)
			{
				IReadOnlyList<Hitbox> enemyDamageBoxes = enemy.GetDamageBoxes();

				bool isSquished = false;
				bool hasCollided = false;

				foreach (Hitbox tuxHitbox in tuxHitboxes)
				{
					foreach (Hitbox enemyDamageBox in enemyDamageBoxes)
					{
						if (HasCollided(tuxHitbox, enemyDamageBox))
						{
							isSquished = tuxHitbox.Y > enemyDamageBox.Y + (enemyDamageBox.Height >> 1) || tuxState.YSpeedInMibipixelsPerSecond < 0;
							break;
						}
					}

					if (isSquished)
						break;
				}

				if (!isSquished)
				{
					foreach (Hitbox tuxHitbox in tuxHitboxes)
					{
						foreach (Hitbox enemyHitbox in enemy.GetHitboxes())
						{
							if (HasCollided(tuxHitbox, enemyHitbox))
							{
								hasCollided = true;
								break;
							}
						}

						if (hasCollided)
							break;
					}
				}

				if (isSquished)
				{
					soundOutput.PlaySound(GameSound.Squish);
					newlyKilledEnemies.Add(enemy.EnemyId);
					newEnemies.Add(enemy.GetDeadEnemy());

					newTuxState = newTuxState.SetYSpeedInMibipixelsPerSecond(ySpeedInMibipixelsPerSecond: TuxState.JUMP_Y_SPEED)
						.SetIsStillHoldingJumpButton(true)
						.SetLastTimeOnGround(null)
						.SetHasAlreadyUsedTeleport(false);
				}
				else if (hasCollided)
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
				newEnemies: newEnemies,
				newlyKilledEnemies: newlyKilledEnemies);
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
