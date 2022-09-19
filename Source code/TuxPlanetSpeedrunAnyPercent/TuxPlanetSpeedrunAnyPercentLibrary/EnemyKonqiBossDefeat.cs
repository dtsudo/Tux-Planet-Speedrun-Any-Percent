
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemyKonqiBossDefeat : IEnemy
	{
		private enum Status
		{
			A_Start,
			B_Jumping,
			C_Teleporting,
			D_Finish,
			E_Disappear
		}

		private Status status;

		private int xMibi;
		private int yMibi;
		private int elapsedMicros;

		private int xSpeedInMibipixelsPerSecond;
		private int ySpeedInMibipixelsPerSecond;
		private const int JUMP_Y_SPEED = 1100000;

		private Tuple<int, int> teleportStartingLocation;
		private int? teleportInProgressElapsedMicros;
		private const int TELEPORT_DURATION = 150 * 1000;

		private List<string> emptyStringList;
		private List<Hitbox> emptyHitboxList;

		public string EnemyId { get; private set; }

		private EnemyKonqiBossDefeat(
			Status status,
			int xMibi,
			int yMibi,
			int elapsedMicros,
			int xSpeedInMibipixelsPerSecond,
			int ySpeedInMibipixelsPerSecond,
			Tuple<int, int> teleportStartingLocation,
			int? teleportInProgressElapsedMicros,
			List<string> emptyStringList,
			List<Hitbox> emptyHitboxList,
			string enemyId)
		{
			this.status = status;
			this.xMibi = xMibi;
			this.yMibi = yMibi;
			this.elapsedMicros = elapsedMicros;
			this.xSpeedInMibipixelsPerSecond = xSpeedInMibipixelsPerSecond;
			this.ySpeedInMibipixelsPerSecond = ySpeedInMibipixelsPerSecond;
			this.teleportStartingLocation = teleportStartingLocation;
			this.teleportInProgressElapsedMicros = teleportInProgressElapsedMicros;
			this.emptyStringList = emptyStringList;
			this.emptyHitboxList = emptyHitboxList;
			this.EnemyId = enemyId;
		}

		public static EnemyKonqiBossDefeat GetEnemyKonqiBossDefeat(
			int xMibi,
			int yMibi,
			int elapsedMicros,
			string enemyId)
		{
			return new EnemyKonqiBossDefeat(
				status: Status.A_Start,
				xMibi: xMibi,
				yMibi: yMibi,
				elapsedMicros: elapsedMicros,
				xSpeedInMibipixelsPerSecond: 0,
				ySpeedInMibipixelsPerSecond: 0,
				teleportStartingLocation: null,
				teleportInProgressElapsedMicros: null,
				emptyStringList: new List<string>(),
				emptyHitboxList: new List<Hitbox>(),
				enemyId: enemyId);
		}

		public bool IsKonqiCutscene { get { return false; } }

		public bool IsRemoveKonqi { get { return false; } }

		public bool ShouldAlwaysSpawnRegardlessOfCamera { get { return true; } }

		public Tuple<int, int> GetKonqiCutsceneLocation()
		{
			return null;
		}

		public IReadOnlyList<Hitbox> GetHitboxes()
		{
			return this.emptyHitboxList;
		}

		public IReadOnlyList<Hitbox> GetDamageBoxes()
		{
			return this.emptyHitboxList;
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

			Status newStatus;
			int newYMibi;
			int newYSpeedInMibipixelsPerSecond;
			Tuple<int, int> newTeleportStartingLocation;
			int? newTeleportInProgressElapsedMicros;

			List<string> newlyAddedLevelFlags = new List<string>();

			switch (this.status)
			{
				case Status.A_Start:
					newStatus = Status.B_Jumping;
					newYMibi = this.yMibi;
					newYSpeedInMibipixelsPerSecond = JUMP_Y_SPEED;
					newTeleportStartingLocation = null;
					newTeleportInProgressElapsedMicros = null;
					break;
				case Status.B_Jumping:
					newYMibi = (int)(((long)this.yMibi) + ((long)this.ySpeedInMibipixelsPerSecond) * ((long)elapsedMicrosPerFrame) / 1000L / 1000L);
					newYSpeedInMibipixelsPerSecond = this.ySpeedInMibipixelsPerSecond - elapsedMicrosPerFrame * 3;

					if (this.ySpeedInMibipixelsPerSecond <= 0)
					{
						newTeleportStartingLocation = new Tuple<int, int>(this.xMibi, this.yMibi);
						newYMibi = newYMibi + 1024 * 141 * 2;
						newYSpeedInMibipixelsPerSecond = 1024 * 1448;
						newTeleportInProgressElapsedMicros = 0;
						soundOutput.PlaySound(GameSound.Teleport);
						newStatus = Status.C_Teleporting;
					}
					else
					{
						newStatus = Status.B_Jumping;
						newTeleportStartingLocation = null;
						newTeleportInProgressElapsedMicros = null;
					}
					break;
				case Status.C_Teleporting:
					newYMibi = this.yMibi;
					newYSpeedInMibipixelsPerSecond = this.ySpeedInMibipixelsPerSecond;

					if (this.teleportInProgressElapsedMicros.Value >= TELEPORT_DURATION)
					{
						newStatus = Status.D_Finish;
						newTeleportStartingLocation = null;
						newTeleportInProgressElapsedMicros = null;
					}
					else
					{
						newStatus = Status.C_Teleporting;
						newTeleportStartingLocation = this.teleportStartingLocation;
						newTeleportInProgressElapsedMicros = this.teleportInProgressElapsedMicros.Value + elapsedMicrosPerFrame;
					}
					break;
				case Status.D_Finish:
					newYMibi = (int)(((long)this.yMibi) + ((long)this.ySpeedInMibipixelsPerSecond) * ((long)elapsedMicrosPerFrame) / 1000L / 1000L);
					newYSpeedInMibipixelsPerSecond = this.ySpeedInMibipixelsPerSecond - elapsedMicrosPerFrame * 3;
					newTeleportStartingLocation = null;
					newTeleportInProgressElapsedMicros = null;

					if (newYSpeedInMibipixelsPerSecond <= 0)
					{
						newStatus = Status.E_Disappear;
						newlyAddedLevelFlags.Add(LevelConfiguration_Level10.CONTINUOUSLY_RENDER_KONQI_BLOCKS);
					}
					else
						newStatus = Status.D_Finish;

					break;
				case Status.E_Disappear:
					newStatus = Status.E_Disappear;
					newYMibi = this.yMibi;
					newYSpeedInMibipixelsPerSecond = this.ySpeedInMibipixelsPerSecond;
					newTeleportStartingLocation = null;
					newTeleportInProgressElapsedMicros = null;

					break;
				default:
					throw new Exception();
			}

			return new EnemyProcessing.Result(
				enemies: new List<IEnemy>()
				{
					new EnemyKonqiBossDefeat(
						status: newStatus,
						xMibi: this.xMibi,
						yMibi: newYMibi,
						elapsedMicros: newElapsedMicros,
						xSpeedInMibipixelsPerSecond: this.xSpeedInMibipixelsPerSecond,
						ySpeedInMibipixelsPerSecond: newYSpeedInMibipixelsPerSecond,
						teleportStartingLocation: newTeleportStartingLocation,
						teleportInProgressElapsedMicros: newTeleportInProgressElapsedMicros,
						emptyStringList: this.emptyStringList,
						emptyHitboxList: this.emptyHitboxList,
						enemyId: this.EnemyId)
				},
				newlyKilledEnemies: this.emptyStringList,
				newlyAddedLevelFlags: newlyAddedLevelFlags);
		}

		public static void RenderKonqiBlock(
			int blockNumber,
			int? konqiXMibi,
			int? konqiYMibi,
			IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			int blockX = 207 * 48 + 36 * 48 + 5 * 48 + blockNumber * 48;
			int blockY = 15 * 48;

			bool isKonqiInRange;

			if (konqiXMibi == null || konqiYMibi == null)
				isKonqiInRange = false;
			else
			{
				int deltaX = (konqiXMibi.Value >> 10) - (blockX + 24);
				int deltaY = (konqiYMibi.Value >> 10) - (blockY + 24);

				isKonqiInRange = deltaX * deltaX + deltaY * deltaY <= 130 * 130;
			}

			if (!isKonqiInRange)
				displayOutput.DrawImageRotatedClockwise(
					image: GameImage.Lock,
					imageX: 0,
					imageY: 0,
					imageWidth: 16,
					imageHeight: 16,
					x: blockX,
					y: blockY,
					degreesScaled: 0,
					scalingFactorScaled: 128 * 3);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			for (int i = 0; i < 14; i++)
			{
				int konqiXMibi = this.xMibi;
				int konqiYMibi = this.yMibi;

				if (this.teleportInProgressElapsedMicros != null)
				{
					long konqiDeltaY = this.yMibi - this.teleportStartingLocation.Item2;

					konqiYMibi = (int)(this.teleportStartingLocation.Item2 + konqiDeltaY * this.teleportInProgressElapsedMicros.Value / TELEPORT_DURATION);
				}

				RenderKonqiBlock(
					blockNumber: i,
					konqiXMibi: konqiXMibi,
					konqiYMibi: konqiYMibi,
					displayOutput: displayOutput);
			}

			switch (this.status)
			{
				case Status.A_Start:
					int spriteNum = (this.elapsedMicros % 1000000) / 250000;

					displayOutput.DrawImageRotatedClockwise(
						image: GameImage.KonqiFireMirrored,
						imageX: spriteNum * 32,
						imageY: 0,
						imageWidth: 32,
						imageHeight: 32,
						x: (this.xMibi >> 10) - 16 * 3,
						y: (this.yMibi >> 10) - 8 * 3,
						degreesScaled: 0,
						scalingFactorScaled: 128 * 3);

					break;
				case Status.B_Jumping:
					displayOutput.DrawImageRotatedClockwise(
						image: GameImage.KonqiFireMirrored,
						imageX: 1 * 32,
						imageY: 4 * 32,
						imageWidth: 32,
						imageHeight: 32,
						x: (this.xMibi >> 10) - 16 * 3,
						y: (this.yMibi >> 10) - 8 * 3,
						degreesScaled: 0,
						scalingFactorScaled: 128 * 3);
					break;
				case Status.C_Teleporting:
					long deltaX = this.xMibi - this.teleportStartingLocation.Item1;
					long deltaY = this.yMibi - this.teleportStartingLocation.Item2;

					for (int i = Math.Max(0, this.teleportInProgressElapsedMicros.Value - 50 * 1000); i < this.teleportInProgressElapsedMicros.Value; i += 5 * 1000)
					{
						int renderXMibi = (int)(this.teleportStartingLocation.Item1 + deltaX * i / TELEPORT_DURATION);
						int renderYMibi = (int)(this.teleportStartingLocation.Item2 + deltaY * i / TELEPORT_DURATION);

						int alpha = (i - (this.teleportInProgressElapsedMicros.Value - 50 * 1000)) * 170 / 50000;

						if (alpha > 0 && alpha <= 255)
							displayOutput.DrawRectangle(
								x: (renderXMibi >> 10) - 20,
								y: (renderYMibi >> 10) - 20,
								width: 40,
								height: 40,
								color: new DTColor(255, 255, 255, alpha),
								fill: true);
					}
					break;
				case Status.D_Finish:
					displayOutput.DrawImageRotatedClockwise(
						image: GameImage.KonqiFireMirrored,
						imageX: 1 * 32,
						imageY: 4 * 32,
						imageWidth: 32,
						imageHeight: 32,
						x: (this.xMibi >> 10) - 16 * 3,
						y: (this.yMibi >> 10) - 8 * 3,
						degreesScaled: 0,
						scalingFactorScaled: 128 * 3);
					break;
				case Status.E_Disappear:
					break;
				default:
					throw new Exception();
			}
		}
	}
}
