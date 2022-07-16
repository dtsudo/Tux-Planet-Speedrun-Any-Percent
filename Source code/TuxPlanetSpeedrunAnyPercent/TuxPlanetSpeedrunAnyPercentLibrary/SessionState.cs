
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class SessionState
	{
		public SessionState()
		{
			this.CurrentLevel = 1;
			this.GameLogic = null;
			this.HasWon = false;
			this.ElapsedMillis = 0;
			this.random = new DTDeterministicRandom(seed: new Random().Next(10000000));
			this.randomValuesUsedForGeneratingLevels = new Dictionary<int, string>();
		}

		public int CurrentLevel { get; private set; }

		public GameLogicState GameLogic { get; private set; }

		public bool HasWon { get; private set; }

		public int ElapsedMillis { get; private set; }

		private IDTDeterministicRandom random;

		private Dictionary<int, string> randomValuesUsedForGeneratingLevels;

		public void ClearData()
		{
			this.CurrentLevel = 1;
			this.GameLogic = null;
			this.HasWon = false;
			this.ElapsedMillis = 0;
			this.randomValuesUsedForGeneratingLevels = new Dictionary<int, string>();
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
		}

		public void WinGame()
		{
			this.HasWon = true;
		}

		public void SetGameLogic(GameLogicState gameLogicState)
		{
			this.GameLogic = gameLogicState;
		}

		public void StartLevel(
			int levelNumber, 
			int windowWidth, 
			int windowHeight, 
			IReadOnlyDictionary<string, MapDataHelper.Map> mapInfo)
		{
			this.CurrentLevel = levelNumber;

			this.random.NextBool();
			this.random.AddSeed(levelNumber);

			if (!this.randomValuesUsedForGeneratingLevels.ContainsKey(this.CurrentLevel))
				this.randomValuesUsedForGeneratingLevels[this.CurrentLevel] = this.random.SerializeToString();

			IDTDeterministicRandom rngForGeneratingLevel = new DTDeterministicRandom();
			rngForGeneratingLevel.DeserializeFromString(this.randomValuesUsedForGeneratingLevels[this.CurrentLevel]);
			
			this.GameLogic = new GameLogicState(
				levelNumber: levelNumber, 
				windowWidth: windowWidth, 
				windowHeight: windowHeight, 
				mapInfo: mapInfo, 
				random: rngForGeneratingLevel);
		}

		public void SerializeEverythingExceptGameLogic(ByteList.Builder list)
		{
			list.AddInt(this.CurrentLevel);
			list.AddBool(this.HasWon);
			list.AddInt(this.ElapsedMillis);

			list.AddInt(this.randomValuesUsedForGeneratingLevels.Count);

			foreach (KeyValuePair<int, string> kvp in this.randomValuesUsedForGeneratingLevels.OrderBy(x => x.Key).ToList())
			{
				int levelNum = kvp.Key;
				string rngValue = kvp.Value;

				list.AddInt(levelNum);
				list.AddString(rngValue);
			}
		}

		/// <summary>
		/// Can possibly throw DTDeserializationException
		/// </summary>
		public void TryDeserializeEverythingExceptGameLogic(ByteList.Iterator iterator)
		{
			int currentLevel = iterator.TryPopInt();
			this.CurrentLevel = currentLevel;

			bool hasWon = iterator.TryPopBool();
			this.HasWon = hasWon;

			int elapsedMillis = iterator.TryPopInt();
			this.ElapsedMillis = elapsedMillis;

			this.randomValuesUsedForGeneratingLevels = new Dictionary<int, string>();
			int numKeyValuePairs = iterator.TryPopInt();

			for (int i = 0; i < numKeyValuePairs; i++)
			{
				int levelNum = iterator.TryPopInt();
				string rngValue = iterator.TryPopString();

				this.randomValuesUsedForGeneratingLevels[levelNum] = rngValue;
			}
		}
	}
}
