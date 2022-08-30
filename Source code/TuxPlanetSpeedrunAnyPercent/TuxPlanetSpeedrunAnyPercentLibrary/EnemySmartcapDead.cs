
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class EnemySmartcapDead : IEnemy
	{
		private int x;
		private int y;
		private bool isFacingRight;

		private int elapsedMicros;

		private List<string> emptyStringList;
		private List<Hitbox> emptyHitboxList;

		public string EnemyId { get; private set; }

		public bool IsKonqiCutscene { get { return false; } }

		public bool IsRemoveKonqi { get { return false; } }

		public bool ShouldAlwaysSpawnRegardlessOfCamera { get { return false; } }

		private const int DEAD_ANIMATION_DURATION = 1000 * 1000;

		public static EnemySmartcapDead SpawnEnemySmartcapDead(
			int xMibi,
			int yMibi,
			bool isFacingRight,
			string enemyId)
		{
			return new EnemySmartcapDead(
				x: (xMibi >> 10) - 8 * 3,
				y: (yMibi >> 10) - 9 * 3,
				isFacingRight: isFacingRight,
				elapsedMicros: 0,
				emptyStringList: new List<string>(),
				emptyHitboxList: new List<Hitbox>(),
				enemyId: enemyId);
		}

		private EnemySmartcapDead(
			int x,
			int y,
			bool isFacingRight,
			int elapsedMicros,
			List<string> emptyStringList,
			List<Hitbox> emptyHitboxList,
			string enemyId)
		{
			this.x = x;
			this.y = y;
			this.isFacingRight = isFacingRight;
			this.elapsedMicros = elapsedMicros;
			this.emptyStringList = emptyStringList;
			this.emptyHitboxList = emptyHitboxList;
			this.EnemyId = enemyId;
		}

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
			return new List<Hitbox>();
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
			if (this.elapsedMicros > DEAD_ANIMATION_DURATION)
				return new EnemyProcessing.Result(
					enemies: new List<IEnemy>(),
					newlyKilledEnemies: this.emptyStringList,
					newlyAddedLevelFlags: null);

			return new EnemyProcessing.Result(
				enemies: new List<IEnemy>()
				{
					new EnemySmartcapDead(
						x: this.x, 
						y: this.y,
						isFacingRight: this.isFacingRight,
						enemyId: this.EnemyId,  
						elapsedMicros: this.elapsedMicros + elapsedMicrosPerFrame,
						emptyStringList: this.emptyStringList,
						emptyHitboxList: this.emptyHitboxList)
				},
				newlyKilledEnemies: this.emptyStringList,
				newlyAddedLevelFlags: null);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			displayOutput.DrawImageRotatedClockwise(
				image: this.isFacingRight ? GameImage.Smartcap : GameImage.SmartcapMirrored,
				imageX: 64,
				imageY: 0,
				imageWidth: 16,
				imageHeight: 18,
				x: this.x,
				y: this.y,
				degreesScaled: 0,
				scalingFactorScaled: 128 * 3);
		}

		public IEnemy GetDeadEnemy()
		{
			return null;
		}
	}
}
