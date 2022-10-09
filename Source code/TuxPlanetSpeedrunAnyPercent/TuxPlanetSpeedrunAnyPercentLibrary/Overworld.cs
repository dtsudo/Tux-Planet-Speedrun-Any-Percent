
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class Overworld
	{
		private int tuxXIndex;
		private int tuxYIndex;
		private int tuxXMibi;
		private int tuxYMibi;

		private IReadOnlyList<Tuple<int, int>> path;

		private HashSet<Level> completedLevels;

		private HashSet<Tuple<int, int>> reachableTiles;

		private OverworldMap overworldMap;

		private int windowWidth;
		private int windowHeight;

		private int elapsedMicros;

		public Overworld(
			int windowWidth,
			int windowHeight,
			string rngSeed,
			HashSet<Level> completedLevels)
		{
			this.windowWidth = windowWidth;
			this.windowHeight = windowHeight;

			this.overworldMap = OverworldMap.GenerateOverworldMap(windowWidth: windowWidth, windowHeight: windowHeight, rngSeed: rngSeed);

			this.tuxXIndex = this.overworldMap.StartingLocation.Item1;
			this.tuxYIndex = this.overworldMap.StartingLocation.Item2;
			this.tuxXMibi = (this.tuxXIndex * OverworldMap.TILE_WIDTH_IN_PIXELS + OverworldMap.TILE_WIDTH_IN_PIXELS / 2) << 10;
			this.tuxYMibi = (this.tuxYIndex * OverworldMap.TILE_HEIGHT_IN_PIXELS + OverworldMap.TILE_HEIGHT_IN_PIXELS / 2) << 10;
			this.path = new List<Tuple<int, int>>();
			this.completedLevels = new HashSet<Level>(completedLevels);
			this.reachableTiles = OverworldUtil.GetReachableTiles(
				overworldGameMap: this.overworldMap.OverworldGameMap,
				completedLevels: completedLevels.ToList());

			this.elapsedMicros = 0;
		}

		private Overworld(
			int windowWidth,
			int windowHeight,
			OverworldMap overworldMap,
			HashSet<Level> completedLevels,
			HashSet<Tuple<int, int>> reachableTiles,
			int tuxXIndex,
			int tuxYIndex,
			int tuxXMibi,
			int tuxYMibi,
			IReadOnlyList<Tuple<int, int>> path,
			int elapsedMicros)
		{
			this.windowWidth = windowWidth;
			this.windowHeight = windowHeight;
			this.overworldMap = overworldMap;
			this.completedLevels = new HashSet<Level>(completedLevels);
			this.reachableTiles = new HashSet<Tuple<int, int>>(reachableTiles, new IntTupleEqualityComparer());
			this.tuxXIndex = tuxXIndex;
			this.tuxYIndex = tuxYIndex;
			this.tuxXMibi = tuxXMibi;
			this.tuxYMibi = tuxYMibi;
			this.path = new List<Tuple<int, int>>(path);

			this.elapsedMicros = elapsedMicros;
		}

		public int GetNumCompletedLevels()
		{
			return this.completedLevels.Count;
		}

		public Overworld CompleteLevel(Level level)
		{
			HashSet<Level> newCompletedLevels = new HashSet<Level>(this.completedLevels);

			newCompletedLevels.Add(level);

			return new Overworld(
				windowWidth: this.windowWidth,
				windowHeight: this.windowHeight,
				overworldMap: this.overworldMap,
				completedLevels: newCompletedLevels,
				reachableTiles: OverworldUtil.GetReachableTiles(
					overworldGameMap: this.overworldMap.OverworldGameMap,
					completedLevels: newCompletedLevels.ToList()),
				tuxXIndex: this.tuxXIndex,
				tuxYIndex: this.tuxYIndex,
				tuxXMibi: this.tuxXMibi,
				tuxYMibi: this.tuxYMibi,
				path: this.path,
				elapsedMicros: this.elapsedMicros);
		}

		public class Result
		{
			public Result(
				Overworld overworld,
				Level? selectedLevel)
			{
				this.Overworld = overworld;
				this.SelectedLevel = selectedLevel;
			}

			public Overworld Overworld { get; private set; }
			public Level? SelectedLevel { get; private set; }
		}

		public Result ProcessFrame(
			IKeyboard keyboardInput,
			IKeyboard previousKeyboardInput,
			int windowWidth,
			int windowHeight,
			int elapsedMicrosPerFrame)
		{
			Level? selectedLevel = null;

			List<Tuple<int, int>> newPath = new List<Tuple<int, int>>(this.path);

			if (newPath.Count == 0)
			{
				if (keyboardInput.IsPressed(Key.Z) && !previousKeyboardInput.IsPressed(Key.Z)
						|| keyboardInput.IsPressed(Key.Space) && !previousKeyboardInput.IsPressed(Key.Space)
						|| keyboardInput.IsPressed(Key.Enter) && !previousKeyboardInput.IsPressed(Key.Enter))
				{
					selectedLevel = this.overworldMap.GetLevel(i: this.tuxXIndex, j: this.tuxYIndex);
				}

				if (selectedLevel == null)
				{
					if (keyboardInput.IsPressed(Key.RightArrow))
					{
						List<Tuple<int, int>> p = OverworldUtil.GetPath(
							overworldGameMap: this.overworldMap.OverworldGameMap,
							reachableTiles: this.reachableTiles.ToList(),
							currentLocation: new Tuple<int, int>(this.tuxXIndex, this.tuxYIndex),
							directionOfTravel: new Tuple<int, int>(1, 0));

						if (p.Count > 0)
							newPath = p;
					}
					if (keyboardInput.IsPressed(Key.LeftArrow))
					{
						List<Tuple<int, int>> p = OverworldUtil.GetPath(
							overworldGameMap: this.overworldMap.OverworldGameMap,
							reachableTiles: this.reachableTiles.ToList(),
							currentLocation: new Tuple<int, int>(this.tuxXIndex, this.tuxYIndex),
							directionOfTravel: new Tuple<int, int>(-1, 0));

						if (p.Count > 0)
							newPath = p;
					}
					if (keyboardInput.IsPressed(Key.UpArrow))
					{
						List<Tuple<int, int>> p = OverworldUtil.GetPath(
							overworldGameMap: this.overworldMap.OverworldGameMap,
							reachableTiles: this.reachableTiles.ToList(),
							currentLocation: new Tuple<int, int>(this.tuxXIndex, this.tuxYIndex),
							directionOfTravel: new Tuple<int, int>(0, 1));

						if (p.Count > 0)
							newPath = p;
					}
					if (keyboardInput.IsPressed(Key.DownArrow))
					{
						List<Tuple<int, int>> p = OverworldUtil.GetPath(
							overworldGameMap: this.overworldMap.OverworldGameMap,
							reachableTiles: this.reachableTiles.ToList(),
							currentLocation: new Tuple<int, int>(this.tuxXIndex, this.tuxYIndex),
							directionOfTravel: new Tuple<int, int>(0, -1));

						if (p.Count > 0)
							newPath = p;
					}
				}
			}

			int amountOfMibipixelsToWalk = elapsedMicrosPerFrame / 3;

			int newTuxXMibi = this.tuxXMibi;
			int newTuxYMibi = this.tuxYMibi;

			int newTuxXIndex = this.tuxXIndex;
			int newTuxYIndex = this.tuxYIndex;

			while (true)
			{
				if (amountOfMibipixelsToWalk <= 0)
					break;

				if (newPath.Count == 0)
					break;

				Tuple<int, int> nextLocation = newPath[0];

				int nextLocationXMibi = (nextLocation.Item1 * OverworldMap.TILE_WIDTH_IN_PIXELS + OverworldMap.TILE_WIDTH_IN_PIXELS / 2) << 10;
				int nextLocationYMibi = (nextLocation.Item2 * OverworldMap.TILE_HEIGHT_IN_PIXELS + OverworldMap.TILE_HEIGHT_IN_PIXELS / 2) << 10;

				int deltaX = Math.Abs(newTuxXMibi - nextLocationXMibi);

				if (deltaX > amountOfMibipixelsToWalk)
				{
					bool isMovingRight = nextLocationXMibi > newTuxXMibi;
					newTuxXMibi = newTuxXMibi + (isMovingRight ? amountOfMibipixelsToWalk : -amountOfMibipixelsToWalk);
					amountOfMibipixelsToWalk = 0;
					continue;
				}

				newTuxXMibi = nextLocationXMibi;
				amountOfMibipixelsToWalk -= deltaX;

				int deltaY = Math.Abs(newTuxYMibi - nextLocationYMibi);

				if (deltaY > amountOfMibipixelsToWalk)
				{
					bool isMovingUp = nextLocationYMibi > newTuxYMibi;
					newTuxYMibi = newTuxYMibi + (isMovingUp ? amountOfMibipixelsToWalk : -amountOfMibipixelsToWalk);
					amountOfMibipixelsToWalk = 0;
					continue;
				}

				newTuxYMibi = nextLocationYMibi;
				amountOfMibipixelsToWalk -= deltaY;

				newTuxXIndex = newPath[0].Item1;
				newTuxYIndex = newPath[0].Item2;

				newPath = newPath.Skip(1).ToList();
			}

			int newElapsedMicros = this.elapsedMicros + elapsedMicrosPerFrame;
			if (newElapsedMicros > 2 * 1000 * 1000 * 1000)
				newElapsedMicros = 1;

			return new Result(
				overworld: new Overworld(
					windowWidth: this.windowWidth,
					windowHeight: this.windowHeight,
					overworldMap: this.overworldMap,
					completedLevels: this.completedLevels,
					reachableTiles: this.reachableTiles,
					tuxXIndex: newTuxXIndex,
					tuxYIndex: newTuxYIndex,
					tuxXMibi: newTuxXMibi,
					tuxYMibi: newTuxYMibi,
					path: newPath,
					elapsedMicros: newElapsedMicros),
				selectedLevel: selectedLevel);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput)
		{
			int cameraXCenter = this.tuxXMibi >> 10;
			int cameraYCenter = this.tuxYMibi >> 10;

			if (cameraXCenter < this.windowWidth / 2)
				cameraXCenter = this.windowWidth / 2;
			if (cameraXCenter > this.overworldMap.GetMapWidthInPixels() - this.windowWidth / 2)
				cameraXCenter = this.overworldMap.GetMapWidthInPixels() - this.windowWidth / 2;
			if (cameraYCenter < this.windowHeight / 2)
				cameraYCenter = this.windowHeight / 2;
			if (cameraYCenter > this.overworldMap.GetMapHeightInPixels() - this.windowHeight / 2)
				cameraYCenter = this.overworldMap.GetMapHeightInPixels() - this.windowHeight / 2;

			IDisplayOutput<GameImage, GameFont> translatedDisplayOutput = new TuxPlanetSpeedrunTranslatedDisplayOutput(
				display: displayOutput,
				xOffsetInPixels: -(cameraXCenter - this.windowWidth / 2),
				yOffsetInPixels: -(cameraYCenter - this.windowHeight / 2));

			this.overworldMap.Render(displayOutput: translatedDisplayOutput, completedLevels: new HashSet<Level>(this.completedLevels));

			translatedDisplayOutput.DrawImageRotatedClockwise(
				image: GameImage.TuxOverworld,
				imageX: this.path.Count > 0 ? ((this.elapsedMicros / 200000) % 4) * 14 : 0,
				imageY: 0,
				imageWidth: 14,
				imageHeight: 17,
				x: (this.tuxXMibi >> 10) - 14 * 3 / 2,
				y: (this.tuxYMibi >> 10) - 17 * 3 / 2 + 15,
				degreesScaled: 0,
				scalingFactorScaled: 3 * 128);
		}

		public void Serialize(ByteList.Builder list)
		{
			list.AddInt(this.windowWidth);
			list.AddInt(this.windowHeight);

			list.AddInt(this.tuxXIndex);
			list.AddInt(this.tuxYIndex);
			list.AddInt(this.tuxXMibi);
			list.AddInt(this.tuxYMibi);

			list.AddInt(this.elapsedMicros);

			list.AddIntSet(new HashSet<int>(this.completedLevels.Select(x => x.ToSerializableInt())));

			list.AddString(this.overworldMap.RngSeed);

			list.AddInt(this.path.Count);

			foreach (Tuple<int, int> x in this.path)
			{
				list.AddInt(x.Item1);
				list.AddInt(x.Item2);
			}

			list.AddInt(this.reachableTiles.Count);

			foreach (Tuple<int, int> x in this.reachableTiles)
			{
				list.AddInt(x.Item1);
				list.AddInt(x.Item2);
			}
		}

		/// <summary>
		/// Can possibly throw DTDeserializationException
		/// </summary>
		public static Overworld TryDeserialize(ByteList.Iterator iterator)
		{
			int windowWidth = iterator.TryPopInt();
			int windowHeight = iterator.TryPopInt();

			int tuxXIndex = iterator.TryPopInt();
			int tuxYIndex = iterator.TryPopInt();
			int tuxXMibi = iterator.TryPopInt();
			int tuxYMibi = iterator.TryPopInt();

			int elapsedMicros = iterator.TryPopInt();

			HashSet<int> intSet = iterator.TryPopIntSet();

			if (intSet == null)
				throw new DTDeserializationException();

			HashSet<Level> completedLevels = new HashSet<Level>(intSet.Select(x => LevelUtil.FromSerializableInt(x)));

			string rngSeed = iterator.TryPopString();

			int pathCount = iterator.TryPopInt();

			List<Tuple<int, int>> path = new List<Tuple<int, int>>();

			for (int i = 0; i < pathCount; i++)
			{
				int x = iterator.TryPopInt();
				int y = iterator.TryPopInt();
				path.Add(new Tuple<int, int>(x, y));
			}

			int reachableTilesCount = iterator.TryPopInt();

			HashSet<Tuple<int, int>> reachableTiles = new HashSet<Tuple<int, int>>(comparer: new IntTupleEqualityComparer());

			for (int i = 0; i < reachableTilesCount; i++)
			{
				int x = iterator.TryPopInt();
				int y = iterator.TryPopInt();
				reachableTiles.Add(new Tuple<int, int>(x, y));
			}

			return new Overworld(
				windowWidth: windowWidth,
				windowHeight: windowHeight,
				overworldMap: OverworldMap.GenerateOverworldMap(windowWidth: windowWidth, windowHeight: windowHeight, rngSeed: rngSeed),
				completedLevels: completedLevels,
				reachableTiles: reachableTiles,
				tuxXIndex: tuxXIndex,
				tuxYIndex: tuxYIndex,
				tuxXMibi: tuxXMibi,
				tuxYMibi: tuxYMibi,
				path: path,
				elapsedMicros: elapsedMicros);
		}
	}
}
