
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class Replay
	{
		public Replay(
			Level level,
			Difficulty difficulty,
			int windowWidth,
			int windowHeight,
			string rngForGeneratingLevel,
			bool canUseSaveStatesAtStartOfLevel,
			bool canUseTimeSlowdownAtStartOfLevel,
			bool canUseTeleportAtStartOfLevel,
			IReadOnlyList<Move> moves)
		{
			this.Level = level;
			this.Difficulty = difficulty;
			this.WindowWidth = windowWidth;
			this.WindowHeight = windowHeight;
			this.RngForGeneratingLevel = rngForGeneratingLevel;
			this.CanUseSaveStatesAtStartOfLevel = canUseSaveStatesAtStartOfLevel;
			this.CanUseTimeSlowdownAtStartOfLevel = canUseTimeSlowdownAtStartOfLevel;
			this.CanUseTeleportAtStartOfLevel = canUseTeleportAtStartOfLevel;
			this.Moves = new List<Move>(moves);
			this.serializedMoves = SerializeMovesToIntList(moves: moves);
		}

		public Level Level { get; private set; }
		public Difficulty Difficulty { get; private set; }
		public int WindowWidth { get; private set; }
		public int WindowHeight { get; private set; }
		public string RngForGeneratingLevel { get; private set; }

		public bool CanUseSaveStatesAtStartOfLevel { get; private set; }
		public bool CanUseTimeSlowdownAtStartOfLevel { get; private set; }
		public bool CanUseTeleportAtStartOfLevel { get; private set; }

		public IReadOnlyList<Move> Moves { get; private set; }

		private List<int> serializedMoves;

		public void Serialize(ByteList.Builder list)
		{
			list.AddInt(this.Level.ToSerializableInt());
			list.AddInt(this.Difficulty.ToSerializableInt());
			list.AddInt(this.WindowWidth);
			list.AddInt(this.WindowHeight);

			list.AddString(this.RngForGeneratingLevel);

			list.AddBool(this.CanUseSaveStatesAtStartOfLevel);
			list.AddBool(this.CanUseTimeSlowdownAtStartOfLevel);
			list.AddBool(this.CanUseTeleportAtStartOfLevel);
			list.AddIntList(this.serializedMoves);
		}

		private const int INT_LIST_MARKER = 1932271361;

		private static List<int> SerializeMovesToIntList(IReadOnlyList<Move> moves)
		{
			if (moves.Count == 0)
				return new List<int>();

			bool[] array = new bool[moves.Count * 7];

			int arrayIndex = 0;

			for (int i = 0; i < moves.Count; i++)
			{
				array[arrayIndex] = moves[i].Jumped;
				arrayIndex++;
			}
			for (int i = 0; i < moves.Count; i++)
			{
				array[arrayIndex] = moves[i].Teleported;
				arrayIndex++;
			}
			for (int i = 0; i < moves.Count; i++)
			{
				array[arrayIndex] = moves[i].ArrowLeft;
				arrayIndex++;
			}
			for (int i = 0; i < moves.Count; i++)
			{
				array[arrayIndex] = moves[i].ArrowRight;
				arrayIndex++;
			}
			for (int i = 0; i < moves.Count; i++)
			{
				array[arrayIndex] = moves[i].ArrowUp;
				arrayIndex++;
			}
			for (int i = 0; i < moves.Count; i++)
			{
				array[arrayIndex] = moves[i].ArrowDown;
				arrayIndex++;
			}
			for (int i = 0; i < moves.Count; i++)
			{
				array[arrayIndex] = moves[i].Respawn;
				arrayIndex++;
			}

			if (arrayIndex != array.Length)
				throw new Exception();

			List<int> returnValue = new List<int>();

			returnValue.Add(INT_LIST_MARKER);

			returnValue.Add(moves.Count);

			returnValue.Add(array[0] ? 1 : 0);

			for (int i = 0; i < array.Length - 1; i++)
			{
				if (array[i] != array[i + 1])
					returnValue.Add(i + 1);
			}

			return returnValue;
		}

		/// <summary>
		/// Can possibly throw DTDeserializationException
		/// </summary>
		public static Replay TryDeserialize(ByteList.Iterator iterator)
		{
			Level level = LevelUtil.FromSerializableInt(iterator.TryPopInt());
			Difficulty difficulty = DifficultyUtil.FromSerializableInt(iterator.TryPopInt());

			int windowWidth = iterator.TryPopInt();
			int windowHeight = iterator.TryPopInt();

			string rngForGeneratingLevel = iterator.TryPopString();

			bool canUseSaveStatesAtStartOfLevel = iterator.TryPopBool();
			bool canUseTimeSlowdownAtStartOfLevel = iterator.TryPopBool();
			bool canUseTeleportAtStartOfLevel = iterator.TryPopBool();

			IReadOnlyList<Move> moves = TryDeserializeMovesFromIntList(intList: iterator.TryPopIntList());

			return new Replay(
				level: level,
				difficulty: difficulty,
				windowWidth: windowWidth,
				windowHeight: windowHeight,
				rngForGeneratingLevel: rngForGeneratingLevel,
				canUseSaveStatesAtStartOfLevel: canUseSaveStatesAtStartOfLevel,
				canUseTimeSlowdownAtStartOfLevel: canUseTimeSlowdownAtStartOfLevel,
				canUseTeleportAtStartOfLevel: canUseTeleportAtStartOfLevel,
				moves: moves);
		}

		/// <summary>
		/// Can possibly throw DTDeserializationException
		/// </summary>
		private static IReadOnlyList<Move> TryDeserializeMovesFromIntList(IReadOnlyList<int> intList)
		{
			if (intList.Count == 0)
				return new List<Move>();

			int markerValue = intList[0];

			if (markerValue != INT_LIST_MARKER)
				throw new DTDeserializationException();

			int movesCount = intList[1];

			bool[] array = new bool[movesCount * 7];

			bool startsWithTrue;

			if (intList[2] == 1)
				startsWithTrue = true;
			else if (intList[2] == 0)
				startsWithTrue = false;
			else
				throw new DTDeserializationException();

			array[0] = startsWithTrue;

			int currentIndex = 1;

			for (int i = 3; i < intList.Count; i++)
			{
				int indexOfSwitch = intList[i];

				while (currentIndex < indexOfSwitch)
				{
					array[currentIndex] = array[currentIndex - 1];
					currentIndex++;
				}

				array[currentIndex] = !array[currentIndex - 1];
				currentIndex++;
			}

			for (int i = currentIndex; i < array.Length; i++)
				array[i] = array[i - 1];

			List<Move> returnValue = new List<Move>();

			for (int i = 0; i < movesCount; i++)
			{
				returnValue.Add(new Move(
					jumped: array[i],
					teleported: array[i + movesCount],
					arrowLeft: array[i + movesCount * 2],
					arrowRight: array[i + movesCount * 3],
					arrowUp: array[i + movesCount * 4],
					arrowDown: array[i + movesCount * 5],
					respawn: array[i + movesCount * 6]));
			}

			return returnValue;
		}
	}
}
