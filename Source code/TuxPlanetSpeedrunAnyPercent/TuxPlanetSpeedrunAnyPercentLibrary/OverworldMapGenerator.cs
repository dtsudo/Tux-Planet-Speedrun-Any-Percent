
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class OverworldMapGenerator
	{
		public class Result
		{
			public Result(Sprite[][] pathTiles, Sprite[][] backgroundTiles)
			{
				this.PathTiles = pathTiles;
				this.BackgroundTiles = backgroundTiles;
			}

			public Sprite[][] PathTiles { get; private set; }
			public Sprite[][] BackgroundTiles { get; private set; }
		}

		public static Result GenerateSpriteTilemap(
			OverworldGameMap map,
			IDTDeterministicRandom random)
		{
			int numColumns = map.Tilemap.Count;
			int numRows = map.Tilemap[0].Count;

			Sprite[][] pathTilemap = new Sprite[numColumns][];

			for (int i = 0; i < numColumns; i++)
			{
				pathTilemap[i] = new Sprite[numRows];

				for (int j = 0; j < numRows; j++)
				{
					int? spriteX;
					int? spriteY;

					switch (map.Tilemap[i][j].Type)
					{
						case OverworldGameMap.TileType.Path:
						case OverworldGameMap.TileType.Level:
							bool pathOnLeft = i > 0 && map.Tilemap[i - 1][j].Type != OverworldGameMap.TileType.NonPath;
							bool pathOnRight = i < numColumns - 1 && map.Tilemap[i + 1][j].Type != OverworldGameMap.TileType.NonPath;
							bool pathOnBottom = j > 0 && map.Tilemap[i][j - 1].Type != OverworldGameMap.TileType.NonPath;
							bool pathOnTop = j < numRows - 1 && map.Tilemap[i][j + 1].Type != OverworldGameMap.TileType.NonPath;

							if (!pathOnLeft && !pathOnRight && !pathOnBottom && !pathOnTop)
							{
								spriteX = 2;
								spriteY = 3;
							}
							else if (!pathOnLeft && !pathOnRight && !pathOnBottom && pathOnTop)
							{
								spriteX = 0;
								spriteY = 8;
							}
							else if (!pathOnLeft && !pathOnRight && pathOnBottom && !pathOnTop)
							{
								spriteX = 0;
								spriteY = 7;
							}
							else if (!pathOnLeft && !pathOnRight && pathOnBottom && pathOnTop)
							{
								spriteX = 2;
								spriteY = 4;
							}
							else if (!pathOnLeft && pathOnRight && !pathOnBottom && !pathOnTop)
							{
								spriteX = 1;
								spriteY = 7;
							}
							else if (!pathOnLeft && pathOnRight && !pathOnBottom && pathOnTop)
							{
								spriteX = 0;
								spriteY = 2;
							}
							else if (!pathOnLeft && pathOnRight && pathOnBottom && !pathOnTop)
							{
								spriteX = 0;
								spriteY = 0;
							}
							else if (!pathOnLeft && pathOnRight && pathOnBottom && pathOnTop)
							{
								spriteX = 0;
								spriteY = 1;
							}
							else if (pathOnLeft && !pathOnRight && !pathOnBottom && !pathOnTop)
							{
								spriteX = 2;
								spriteY = 7;
							}
							else if (pathOnLeft && !pathOnRight && !pathOnBottom && pathOnTop)
							{
								spriteX = 2;
								spriteY = 2;
							}
							else if (pathOnLeft && !pathOnRight && pathOnBottom && !pathOnTop)
							{
								spriteX = 2;
								spriteY = 0;
							}
							else if (pathOnLeft && !pathOnRight && pathOnBottom && pathOnTop)
							{
								spriteX = 2;
								spriteY = 1;
							}
							else if (pathOnLeft && pathOnRight && !pathOnBottom && !pathOnTop)
							{
								spriteX = 2;
								spriteY = 3;
							}
							else if (pathOnLeft && pathOnRight && !pathOnBottom && pathOnTop)
							{
								spriteX = 1;
								spriteY = 2;
							}
							else if (pathOnLeft && pathOnRight && pathOnBottom && !pathOnTop)
							{
								spriteX = 1;
								spriteY = 0;
							}
							else if (pathOnLeft && pathOnRight && pathOnBottom && pathOnTop)
							{
								spriteX = 1;
								spriteY = 1;
							}
							else
								throw new Exception();

							break;
						case OverworldGameMap.TileType.NonPath:
							spriteX = null;
							spriteY = null;
							break;
						default:
							throw new Exception();
					}

					if (spriteX != null)
						pathTilemap[i][j] = new Sprite(
							image: GameImage.PathDirt,
							x: spriteX.Value << 4,
							y: spriteY.Value << 4,
							width: 16,
							height: 16,
							scalingFactorScaled: 3 * 128);
					else
						pathTilemap[i][j] = null;
				}
			}

			return new Result(
				pathTiles: pathTilemap,
				backgroundTiles: GenerateBackgroundTiles(numColumns: numColumns, numRows: numRows, random: random));
		}

		private static Sprite[][] GenerateBackgroundTiles(int numColumns, int numRows, IDTDeterministicRandom random)
		{
			Sprite[][] tilemap = new Sprite[numColumns][];

			for (int i = 0; i < numColumns; i++)
			{
				tilemap[i] = new Sprite[numRows];

				for (int j = 0; j < numRows; j++)
				{
					tilemap[i][j] = new Sprite(
						image: GameImage.Snow,
						x: random.NextInt(3) << 4,
						y: 5 * 16,
						width: 16,
						height: 16,
						scalingFactorScaled: 3 * 128);
				}
			}

			return tilemap;
		}
	}
}
