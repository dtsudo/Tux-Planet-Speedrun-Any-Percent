
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class SessionState
	{
		public SessionState(int windowWidth, int windowHeight)
		{
			IDTDeterministicRandom random = new DTDeterministicRandom(seed: new Random().Next(10000000));

			this.Overworld = new Overworld(
				windowWidth: windowWidth,
				windowHeight: windowHeight,
				rngSeed: random.SerializeToString(),
				completedLevels: new HashSet<Level>());

			this.CanUseSaveStates = false;
			this.CanUseTimeSlowdown = false;
			this.CanUseTeleport = false;
			this.ShouldReplayInputAfterLoadingSaveState = false;
			this.LastSelectedDifficulty = Difficulty.Normal;
			this.HasWon = false;
			this.ElapsedMillis = 0;
			this.random = random;
			this.replays = new Dictionary<Difficulty, Dictionary<Level, Replay>>();
			this.replays[Difficulty.Easy] = new Dictionary<Level, Replay>();
			this.replays[Difficulty.Normal] = new Dictionary<Level, Replay>();
			this.replays[Difficulty.Hard] = new Dictionary<Level, Replay>();
			this.randomValuesUsedForGeneratingLevels = new Dictionary<Difficulty, Dictionary<Level, string>>();
			this.randomValuesUsedForGeneratingLevels[Difficulty.Easy] = new Dictionary<Level, string>();
			this.randomValuesUsedForGeneratingLevels[Difficulty.Normal] = new Dictionary<Level, string>();
			this.randomValuesUsedForGeneratingLevels[Difficulty.Hard] = new Dictionary<Level, string>();
		}

		public Overworld Overworld { get; private set; }

		public bool CanUseSaveStates { get; private set; }
		public bool CanUseTimeSlowdown { get; private set; }
		public bool CanUseTeleport { get; private set; }

		public bool ShouldReplayInputAfterLoadingSaveState { get; private set; }

		public Difficulty LastSelectedDifficulty { get; private set; }

		public bool HasWon { get; private set; }

		public int ElapsedMillis { get; private set; }

		private IDTDeterministicRandom random;

		private Dictionary<Difficulty, Dictionary<Level, Replay>> replays;

		private Dictionary<Difficulty, Dictionary<Level, string>> randomValuesUsedForGeneratingLevels;

		public const int SIMPLE_DATA_VERSION_NUMBER = 1;

		public void ClearData(int windowWidth, int windowHeight)
		{
			this.random.NextBool();

			this.Overworld = new Overworld(
				windowWidth: windowWidth,
				windowHeight: windowHeight,
				rngSeed: this.random.SerializeToString(),
				completedLevels: new HashSet<Level>());

			this.CanUseSaveStates = false;
			this.CanUseTimeSlowdown = false;
			this.CanUseTeleport = false;
			this.ShouldReplayInputAfterLoadingSaveState = false;
			this.LastSelectedDifficulty = Difficulty.Normal;
			this.HasWon = false;
			this.ElapsedMillis = 0;
			this.replays = new Dictionary<Difficulty, Dictionary<Level, Replay>>();
			this.replays[Difficulty.Easy] = new Dictionary<Level, Replay>();
			this.replays[Difficulty.Normal] = new Dictionary<Level, Replay>();
			this.replays[Difficulty.Hard] = new Dictionary<Level, Replay>();
			this.randomValuesUsedForGeneratingLevels = new Dictionary<Difficulty, Dictionary<Level, string>>();
			this.randomValuesUsedForGeneratingLevels[Difficulty.Easy] = new Dictionary<Level, string>();
			this.randomValuesUsedForGeneratingLevels[Difficulty.Normal] = new Dictionary<Level, string>();
			this.randomValuesUsedForGeneratingLevels[Difficulty.Hard] = new Dictionary<Level, string>();
		}

		public void AddRandomSeed(int seed)
		{
			this.random.AddSeed(seed);
			this.random.NextBool();
		}

		public bool HasStarted()
		{
			return this.ElapsedMillis > 0;
		}

		public void AddElapsedMillis(int elapsedMillis)
		{
			this.ElapsedMillis += elapsedMillis;

			if (this.ElapsedMillis > 2 * 1000 * 1000 * 1000)
				this.ElapsedMillis = 2 * 1000 * 1000 * 1000;
		}

		public void SetOverworld(Overworld overworld)
		{
			this.Overworld = overworld;
		}

		public void CompleteLevel(
			Level level,
			Difficulty difficulty,
			Replay replay,
			bool canUseSaveStates,
			bool canUseTimeSlowdown,
			bool canUseTeleport)
		{
			this.Overworld = this.Overworld.CompleteLevel(level: level);
			if (canUseSaveStates)
				this.CanUseSaveStates = true;
			if (canUseTimeSlowdown)
				this.CanUseTimeSlowdown = true;
			if (canUseTeleport)
				this.CanUseTeleport = true;

			this.replays[difficulty][level] = replay;
		}

		public void WinGame()
		{
			this.HasWon = true;
		}

		public Replay TryGetReplay(Level level, Difficulty difficulty)
		{
			if (this.replays[difficulty].ContainsKey(level))
				return this.replays[difficulty][level];

			return null;
		}

		public void SetShouldReplayInputAfterLoadingSaveState(bool shouldReplayInputAfterLoadingSaveState)
		{
			this.ShouldReplayInputAfterLoadingSaveState = shouldReplayInputAfterLoadingSaveState;
		}

		public string GetRngUsedForGeneratingLevel(Level level, Difficulty difficulty)
		{
			if (!this.randomValuesUsedForGeneratingLevels[difficulty].ContainsKey(level))
				throw new Exception();

			string rngSeed = this.randomValuesUsedForGeneratingLevels[difficulty][level];

			if (rngSeed == null)
				throw new Exception();

			return rngSeed;
		}

		public GameLogicState StartLevel(
			Level level, 
			Difficulty difficulty,
			int windowWidth, 
			int windowHeight, 
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo)
		{
			this.random.NextBool();

			if (!this.randomValuesUsedForGeneratingLevels[difficulty].ContainsKey(level))
				this.randomValuesUsedForGeneratingLevels[difficulty][level] = this.random.SerializeToString();

			DTDeterministicRandom rngForGeneratingLevel = new DTDeterministicRandom();
			rngForGeneratingLevel.DeserializeFromString(this.randomValuesUsedForGeneratingLevels[difficulty][level]);

			this.LastSelectedDifficulty = difficulty;

			GameLogicState gameLogicState = new GameLogicState(
				level: level,
				difficulty: difficulty,
				windowWidth: windowWidth, 
				windowHeight: windowHeight,
				canUseSaveStates: this.CanUseSaveStates,
				canUseTimeSlowdown: this.CanUseTimeSlowdown,
				canUseTeleport: this.CanUseTeleport,
				mapInfo: mapInfo, 
				random: rngForGeneratingLevel);

			return gameLogicState;
		}

		public void Serialize(ByteList.Builder list)
		{
			string currentVersion = VersionInfo.GetVersionInfo().Version;
			list.AddString(currentVersion);

			this.Overworld.Serialize(list: list);

			list.AddBool(this.CanUseSaveStates);
			list.AddBool(this.CanUseTimeSlowdown);
			list.AddBool(this.CanUseTeleport);

			list.AddBool(this.ShouldReplayInputAfterLoadingSaveState);
			list.AddInt(this.LastSelectedDifficulty.ToSerializableInt());

			list.AddBool(this.HasWon);
			list.AddInt(this.ElapsedMillis);

			List<Difficulty> difficultyList = new List<Difficulty>() { Difficulty.Easy, Difficulty.Normal, Difficulty.Hard };

			for (int i = 0; i < difficultyList.Count; i++)
			{
				Difficulty difficulty = difficultyList[i];

				list.AddInt(this.randomValuesUsedForGeneratingLevels[difficulty].Count);

				foreach (KeyValuePair<Level, string> kvp in this.randomValuesUsedForGeneratingLevels[difficulty].OrderBy(x => x.Key.ToSerializableInt()).ToList())
				{
					Level level = kvp.Key;
					string rngValue = kvp.Value;

					list.AddInt(level.ToSerializableInt());
					list.AddString(rngValue);
				}

				list.AddInt(this.replays[difficulty].Count);

				foreach (KeyValuePair<Level, Replay> kvp in this.replays[difficulty].OrderBy(x => x.Key.ToSerializableInt()).ToList())
				{
					Level level = kvp.Key;
					Replay replay = kvp.Value;

					list.AddInt(level.ToSerializableInt());
					replay.Serialize(list: list);
				}
			}
		}

		/// <summary>
		/// Can possibly throw DTDeserializationException
		/// </summary>
		public void TryDeserialize(ByteList.Iterator iterator)
		{
			string currentVersion = VersionInfo.GetVersionInfo().Version;
			string savedDataVersion = iterator.TryPopString();
			if (currentVersion != savedDataVersion)
				throw new DTDeserializationException();

			this.Overworld = Overworld.TryDeserialize(iterator: iterator);

			this.CanUseSaveStates = iterator.TryPopBool();
			this.CanUseTimeSlowdown = iterator.TryPopBool();
			this.CanUseTeleport = iterator.TryPopBool();

			this.ShouldReplayInputAfterLoadingSaveState = iterator.TryPopBool();
			this.LastSelectedDifficulty = DifficultyUtil.FromSerializableInt(iterator.TryPopInt());

			bool hasWon = iterator.TryPopBool();
			this.HasWon = hasWon;

			int elapsedMillis = iterator.TryPopInt();
			this.ElapsedMillis = elapsedMillis;

			this.randomValuesUsedForGeneratingLevels = new Dictionary<Difficulty, Dictionary<Level, string>>();
			this.randomValuesUsedForGeneratingLevels[Difficulty.Easy] = new Dictionary<Level, string>();
			this.randomValuesUsedForGeneratingLevels[Difficulty.Normal] = new Dictionary<Level, string>();
			this.randomValuesUsedForGeneratingLevels[Difficulty.Hard] = new Dictionary<Level, string>();

			this.replays = new Dictionary<Difficulty, Dictionary<Level, Replay>>();
			this.replays[Difficulty.Easy] = new Dictionary<Level, Replay>();
			this.replays[Difficulty.Normal] = new Dictionary<Level, Replay>();
			this.replays[Difficulty.Hard] = new Dictionary<Level, Replay>();

			List<Difficulty> difficultyList = new List<Difficulty>() { Difficulty.Easy, Difficulty.Normal, Difficulty.Hard };

			foreach (Difficulty difficulty in difficultyList)
			{
				int numRandomValues = iterator.TryPopInt();

				for (int i = 0; i < numRandomValues; i++)
				{
					int level = iterator.TryPopInt();
					string rngValue = iterator.TryPopString();

					this.randomValuesUsedForGeneratingLevels[difficulty][LevelUtil.FromSerializableInt(level)] = rngValue;
				}
				
				int numReplays = iterator.TryPopInt();

				for (int i = 0; i < numReplays; i++)
				{
					int level = iterator.TryPopInt();
					Replay replay = Replay.TryDeserialize(iterator: iterator);

					this.replays[difficulty][LevelUtil.FromSerializableInt(level)] = replay;
				}
			}
		}

		/// <summary>
		/// Serializes only a subset of data that's likely to remain valid across different versions of the game
		/// </summary>
		public void SerializeSimpleData(ByteList.Builder list)
		{
			list.AddInt(SIMPLE_DATA_VERSION_NUMBER);
			int numCompletedLevels = this.Overworld.GetNumCompletedLevels();
			list.AddInt(numCompletedLevels);
			list.AddBool(this.CanUseSaveStates);
			list.AddBool(this.CanUseTimeSlowdown);
			list.AddBool(this.CanUseTeleport);
			list.AddInt(this.ElapsedMillis);
		}

		/// <summary>
		/// Can possibly throw DTDeserializationException
		/// </summary>
		public void TryDeserializeFromSimpleData(ByteList.Iterator iterator)
		{
			int simpleDataFormattingVersion = iterator.TryPopInt();

			if (simpleDataFormattingVersion != SIMPLE_DATA_VERSION_NUMBER)
				throw new DTDeserializationException();

			int numCompletedLevels = iterator.TryPopInt();

			if (numCompletedLevels >= 1)
				this.Overworld = this.Overworld.CompleteLevel(level: Level.Level1);
			if (numCompletedLevels >= 2)
				this.Overworld = this.Overworld.CompleteLevel(level: Level.Level2);
			if (numCompletedLevels >= 3)
				this.Overworld = this.Overworld.CompleteLevel(level: Level.Level3);
			if (numCompletedLevels >= 4)
				this.Overworld = this.Overworld.CompleteLevel(level: Level.Level4);
			if (numCompletedLevels >= 5)
				this.Overworld = this.Overworld.CompleteLevel(level: Level.Level5);
			if (numCompletedLevels >= 6)
				this.Overworld = this.Overworld.CompleteLevel(level: Level.Level6);
			if (numCompletedLevels >= 7)
				this.Overworld = this.Overworld.CompleteLevel(level: Level.Level7);
			if (numCompletedLevels >= 8)
				this.Overworld = this.Overworld.CompleteLevel(level: Level.Level8);
			if (numCompletedLevels >= 9)
				this.Overworld = this.Overworld.CompleteLevel(level: Level.Level9);
			if (numCompletedLevels >= 10)
				this.Overworld = this.Overworld.CompleteLevel(level: Level.Level10);

			this.CanUseSaveStates = iterator.TryPopBool();
			this.CanUseTimeSlowdown = iterator.TryPopBool();
			this.CanUseTeleport = iterator.TryPopBool();

			this.ShouldReplayInputAfterLoadingSaveState = false;
			this.LastSelectedDifficulty = Difficulty.Normal;

			this.ElapsedMillis = iterator.TryPopInt();

			this.HasWon = numCompletedLevels >= 10;
		}
	}
}
