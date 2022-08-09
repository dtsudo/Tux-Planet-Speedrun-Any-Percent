
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class OverworldMap
	{
		public const int TILE_WIDTH_IN_PIXELS = 16 * 3;
		public const int TILE_HEIGHT_IN_PIXELS = 16 * 3;

		public OverworldGameMap OverworldGameMap { get; private set; }

		private Sprite[][] pathTilemap;
		private Sprite[][] backgroundTilemap;

		public string RngSeed { get; private set; }

		public Tuple<int, int> StartingLocation
		{
			get
			{
				return this.OverworldGameMap.StartingLocation;
			}
		}

		public static OverworldMap GenerateOverworldMap(
			int windowWidth,
			int windowHeight, 
			string rngSeed)
		{
			return new OverworldMap(
				windowWidth: windowWidth,
				windowHeight: windowHeight,
				rngSeed: rngSeed);
		}

		private OverworldMap(
			int windowWidth,
			int windowHeight,
			string rngSeed)
		{
			DTDeterministicRandom random = new DTDeterministicRandom();
			random.DeserializeFromString(rngSeed);

			this.OverworldGameMap = OverworldGameMap.GenerateOverworldGameMap(windowWidth: windowWidth, windowHeight: windowHeight, random: random);

			OverworldMapGenerator.Result result = OverworldMapGenerator.GenerateSpriteTilemap(
				map: this.OverworldGameMap,
				random: random);

			this.pathTilemap = ArrayUtil.ShallowCopyTArray(result.PathTiles);
			this.backgroundTilemap = ArrayUtil.ShallowCopyTArray(result.BackgroundTiles);

			this.RngSeed = rngSeed;
		}

		public OverworldGameMap.TileType GetTileType(int i, int j)
		{
			if (i < 0 || i >= this.OverworldGameMap.Tilemap.Count)
				return OverworldGameMap.TileType.NonPath;

			if (j < 0 || j >= this.OverworldGameMap.Tilemap[i].Count)
				return OverworldGameMap.TileType.NonPath;

			return this.OverworldGameMap.Tilemap[i][j].Type;
		}

		public Level? GetLevel(int i, int j)
		{
			if (i < 0 || i >= this.OverworldGameMap.Tilemap.Count)
				return null;

			if (j < 0 || j >= this.OverworldGameMap.Tilemap[i].Count)
				return null;

			return this.OverworldGameMap.Tilemap[i][j].Level;
		}

		public int GetMapWidthInPixels()
		{
			return this.pathTilemap.Length * TILE_WIDTH_IN_PIXELS;
		}

		public int GetMapHeightInPixels()
		{
			return this.pathTilemap[0].Length * TILE_HEIGHT_IN_PIXELS;
		}

		public void Render(
			IDisplayOutput<GameImage, GameFont> displayOutput,
			HashSet<Level> completedLevels)
		{
			int renderX = 0;

			for (int i = 0; i < this.pathTilemap.Length; i++)
			{
				int renderY = 0;

				for (int j = 0; j < this.pathTilemap[i].Length; j++)
				{
					Sprite background = this.backgroundTilemap[i][j];

					if (background != null)
						displayOutput.DrawImageRotatedClockwise(
							image: background.Image,
							imageX: background.X,
							imageY: background.Y,
							imageWidth: background.Width,
							imageHeight: background.Height,
							x: renderX,
							y: renderY,
							degreesScaled: 0,
							scalingFactorScaled: background.ScalingFactorScaled);

					Sprite path = this.pathTilemap[i][j];

					if (path != null)
						displayOutput.DrawImageRotatedClockwise(
							image: path.Image,
							imageX: path.X,
							imageY: path.Y,
							imageWidth: path.Width,
							imageHeight: path.Height,
							x: renderX,
							y: renderY,
							degreesScaled: 0,
							scalingFactorScaled: path.ScalingFactorScaled);

					if (this.OverworldGameMap.Tilemap[i][j].Type == OverworldGameMap.TileType.Level)
					{
						bool hasCompletedLevel = completedLevels.Contains(this.OverworldGameMap.Tilemap[i][j].Level.Value);

						displayOutput.DrawImageRotatedClockwise(
							image: GameImage.LevelIcons,
							imageX: hasCompletedLevel ? 16 : 0,
							imageY: 0,
							imageWidth: 16,
							imageHeight: 16,
							x: renderX,
							y: renderY,
							degreesScaled: 0,
							scalingFactorScaled: 3 * 128);
					}

					renderY += TILE_HEIGHT_IN_PIXELS;
				}

				renderX += TILE_WIDTH_IN_PIXELS;
			}
		}
	}
}
