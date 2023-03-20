
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class Level10CoordinatorTilemap : ITilemap
	{
		private ITilemap mapTilemap;
		private int bossRoomXOffsetStart;
		private int bossRoomXOffsetEnd;

		private bool markYetiFloorAsGround;
		private bool markLeftAndRightWallsOfBossRoomAsGround;
		private bool stopMarkingLeftAndRightWallsOfBossRoomAsGround;
		private bool beginKonqiDefeatedCutscene;
		private bool beginYetiIntroCutscene;
		private bool beginYetiDefeatedCutscene;
		private bool createCheckpointAfterDefeatingKonqi;
		private bool spawnMythrilKey;
		private bool startPlayingKonqiBossMusic;
		private bool stopPlayingKonqiBossMusic;
		private bool startPlayingYetiBossMusic;
		private bool stopPlayingYetiBossMusic;
		private bool continuouslyRenderKonqiBlocks;

		public Level10CoordinatorTilemap(
			ITilemap mapTilemap, 
			IReadOnlyList<string> levelFlags,
			int bossRoomXOffsetStart,
			int bossRoomXOffsetEnd)
		{
			this.mapTilemap = mapTilemap;
			this.bossRoomXOffsetStart = bossRoomXOffsetStart;
			this.bossRoomXOffsetEnd = bossRoomXOffsetEnd;

			this.markYetiFloorAsGround = false;
			this.markLeftAndRightWallsOfBossRoomAsGround = false;
			this.stopMarkingLeftAndRightWallsOfBossRoomAsGround = false;
			this.beginKonqiDefeatedCutscene = false;
			this.beginYetiIntroCutscene = false;
			this.beginYetiDefeatedCutscene = false;
			this.createCheckpointAfterDefeatingKonqi = false;
			this.spawnMythrilKey = false;
			this.startPlayingKonqiBossMusic = false;
			this.stopPlayingKonqiBossMusic = false;
			this.startPlayingYetiBossMusic = false;
			this.stopPlayingYetiBossMusic = false;
			this.continuouslyRenderKonqiBlocks = false;

			for (int i = 0; i < levelFlags.Count; i++)
			{
				string levelFlag = levelFlags[i];

				if (levelFlag == LevelConfiguration_Level10.MARK_YETI_FLOOR_AS_GROUND)
					this.markYetiFloorAsGround = true;
				else if (levelFlag == LevelConfiguration_Level10.MARK_LEFT_AND_RIGHT_WALLS_OF_BOSS_ROOM_AS_GROUND)
					this.markLeftAndRightWallsOfBossRoomAsGround = true;
				else if (levelFlag == LevelConfiguration_Level10.STOP_MARKING_LEFT_AND_RIGHT_WALLS_OF_BOSS_ROOM_AS_GROUND)
					this.stopMarkingLeftAndRightWallsOfBossRoomAsGround = true;
				else if (levelFlag == LevelConfiguration_Level10.BEGIN_KONQI_DEFEATED_CUTSCENE)
					this.beginKonqiDefeatedCutscene = true;
				else if (levelFlag == LevelConfiguration_Level10.BEGIN_YETI_INTRO_CUTSCENE)
					this.beginYetiIntroCutscene = true;
				else if (levelFlag == LevelConfiguration_Level10.BEGIN_YETI_DEFEATED_CUTSCENE)
					this.beginYetiDefeatedCutscene = true;
				else if (levelFlag == LevelConfiguration_Level10.CREATE_CHECKPOINT_AFTER_DEFEATING_KONQI)
					this.createCheckpointAfterDefeatingKonqi = true;
				else if (levelFlag == LevelConfiguration_Level10.SPAWN_MYTHRIL_KEY)
					this.spawnMythrilKey = true;
				else if (levelFlag == LevelConfiguration_Level10.START_PLAYING_KONQI_BOSS_MUSIC)
					this.startPlayingKonqiBossMusic = true;
				else if (levelFlag == LevelConfiguration_Level10.STOP_PLAYING_KONQI_BOSS_MUSIC)
					this.stopPlayingKonqiBossMusic = true;
				else if (levelFlag == LevelConfiguration_Level10.START_PLAYING_YETI_BOSS_MUSIC)
					this.startPlayingYetiBossMusic = true;
				else if (levelFlag == LevelConfiguration_Level10.STOP_PLAYING_YETI_BOSS_MUSIC)
					this.stopPlayingYetiBossMusic = true;
				else if (levelFlag == LevelConfiguration_Level10.CONTINUOUSLY_RENDER_KONQI_BLOCKS)
					this.continuouslyRenderKonqiBlocks = true;
			}
		}

		public bool IsGround(int x, int y)
		{
			if (this.markYetiFloorAsGround)
			{
				if (y < 18 * 48)
					return true;
			}
			else
			{
				if (y > 16 * 48 && x >= this.bossRoomXOffsetStart + 5 * 48 && x < this.bossRoomXOffsetStart + 6 * 48)
					return true;
			}

			if (this.markLeftAndRightWallsOfBossRoomAsGround && !this.stopMarkingLeftAndRightWallsOfBossRoomAsGround)
			{
				if (x < this.bossRoomXOffsetStart)
					return true;

				if (x >= this.bossRoomXOffsetEnd)
					return true;
			}

			return this.mapTilemap.IsGround(x: x, y: y);
		}

		public bool IsKillZone(int x, int y)
		{
			return this.mapTilemap.IsKillZone(x: x, y: y);
		}

		public bool IsSpikes(int x, int y)
		{
			return this.mapTilemap.IsSpikes(x: x, y: y);
		}

		public bool IsEndOfLevel(int x, int y)
		{
			return this.mapTilemap.IsEndOfLevel(x: x, y: y);
		}

		public string GetCutscene(int x, int y)
		{
			if (this.beginYetiDefeatedCutscene)
				return CutsceneProcessing.YETI_BOSS_DEFEATED_CUTSCENE;

			if (this.beginYetiIntroCutscene)
				return CutsceneProcessing.YETI_BOSS_INTRO_CUTSCENE;

			if (this.beginKonqiDefeatedCutscene)
				return CutsceneProcessing.KONQI_BOSS_DEFEATED_CUTSCENE;

			return this.mapTilemap.GetCutscene(x: x, y: y);
		}

		public Tuple<int, int> GetCheckpoint(int x, int y)
		{
			if (this.createCheckpointAfterDefeatingKonqi)
			{
				if (y < 14 * 48 && x > this.bossRoomXOffsetStart + 48 * 9)
					return new Tuple<int, int>(
						item1: this.bossRoomXOffsetStart + 48 * 3,
						item2: 5 * 48);
			}

			return this.mapTilemap.GetCheckpoint(x: x, y: y);
		}

		public int GetWidth()
		{
			return this.mapTilemap.GetWidth();
		}

		public int GetHeight()
		{
			return this.mapTilemap.GetHeight();
		}

		public Tuple<int, int> GetTuxLocation(int xOffset, int yOffset)
		{
			return this.mapTilemap.GetTuxLocation(xOffset: xOffset, yOffset: yOffset);
		}

		public Tuple<int, int> GetMapKeyLocation(MapKey mapKey, int xOffset, int yOffset)
		{
			if (mapKey == MapKey.Mythril)
			{
				if (this.spawnMythrilKey)
					return new Tuple<int, int>(
						item1: this.bossRoomXOffsetStart + 16 * 48 + xOffset,
						item2: 4 * 48 + 24 + yOffset);
			}

			return this.mapTilemap.GetMapKeyLocation(mapKey: mapKey, xOffset: xOffset, yOffset: yOffset);
		}

		public IReadOnlyList<IEnemy> GetEnemies(int xOffset, int yOffset)
		{
			List<IEnemy> enemies = new List<IEnemy>();
			enemies.AddRange(this.mapTilemap.GetEnemies(xOffset: xOffset, yOffset: yOffset));
			enemies.Add(EnemyBossDoor.GetEnemyBossDoor(
				xMibi: (this.bossRoomXOffsetStart - 48) << 10,
				yMibi: (48 * 3) << 10,
				isUpperDoor: false,
				enemyId: "konqiBoss_bossDoor1"));
			enemies.Add(EnemyBossDoor.GetEnemyBossDoor(
				xMibi: (this.bossRoomXOffsetStart - 48) << 10,
				yMibi: (48 * 5) << 10,
				isUpperDoor: true,
				enemyId: "konqiBoss_bossDoor2"));
			enemies.Add(EnemyBossDoor.GetEnemyBossDoor(
				xMibi: this.bossRoomXOffsetEnd << 10,
				yMibi: (48 * 3) << 10,
				isUpperDoor: false,
				enemyId: "konqiBoss_bossDoor3"));
			enemies.Add(EnemyBossDoor.GetEnemyBossDoor(
				xMibi: this.bossRoomXOffsetEnd << 10,
				yMibi: (48 * 5) << 10,
				isUpperDoor: true,
				enemyId: "konqiBoss_bossDoor4"));
			enemies.Add(EnemyBossDoor.GetEnemyBossDoor(
				xMibi: this.bossRoomXOffsetEnd << 10,
				yMibi: (48 * 18) << 10,
				isUpperDoor: false,
				enemyId: "yetiBoss_bossDoor1"));
			enemies.Add(EnemyBossDoor.GetEnemyBossDoor(
				xMibi: this.bossRoomXOffsetEnd << 10,
				yMibi: (48 * 20) << 10,
				isUpperDoor: true,
				enemyId: "yetiBoss_bossDoor2"));

			return enemies;
		}

		public GameMusic? PlayMusic()
		{
			if (this.startPlayingKonqiBossMusic && !this.stopPlayingKonqiBossMusic)
				return GameMusic.KonqiBossTheme;

			if (this.startPlayingYetiBossMusic && !this.stopPlayingYetiBossMusic)
				return GameMusic.YetiBossTheme;

			return this.mapTilemap.PlayMusic();
		}

		public void RenderBackgroundTiles(IDisplayOutput<GameImage, GameFont> displayOutput, int cameraX, int cameraY, int windowWidth, int windowHeight)
		{
			this.mapTilemap.RenderBackgroundTiles(
				displayOutput: displayOutput,
				cameraX: cameraX,
				cameraY: cameraY,
				windowWidth: windowWidth,
				windowHeight: windowHeight);
		}

		public void RenderForegroundTiles(IDisplayOutput<GameImage, GameFont> displayOutput, int cameraX, int cameraY, int windowWidth, int windowHeight)
		{
			if (this.continuouslyRenderKonqiBlocks)
			{
				for (int i = 0; i < 14; i++)
				{
					EnemyKonqiBossDefeat.RenderKonqiBlock(
						blockNumber: i,
						konqiXMibi: null,
						konqiYMibi: null,
						displayOutput: displayOutput);
				}
			}

			this.mapTilemap.RenderForegroundTiles(
				displayOutput: displayOutput,
				cameraX: cameraX,
				cameraY: cameraY,
				windowWidth: windowWidth,
				windowHeight: windowHeight);
		}
	}
}
