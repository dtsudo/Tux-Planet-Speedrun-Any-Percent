
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class LevelConfigurationHelper
	{
		public static GameMusic GetRandomGameMusic(IDTDeterministicRandom random)
		{
			int i = random.NextInt(2);

			switch (i)
			{
				case 0: return GameMusic.Airship2;
				case 1: return GameMusic.Chipdisko;
				default: throw new Exception();
			}
		}

		public static ITilemap GetTilemap(
			IReadOnlyList<CompositeTilemap.TilemapWithOffset> normalizedTilemaps,
			int? tuxX, 
			int? tuxY, 
			int? cameraX,
			int? cameraY,
			MapKeyState mapKeyState,
			int windowWidth, 
			int windowHeight)
		{
			int tilemapWidth = 0;
			int tilemapHeight = 0;

			foreach (CompositeTilemap.TilemapWithOffset tilemap in normalizedTilemaps)
			{
				int width = tilemap.XOffset + tilemap.Tilemap.GetWidth();

				if (tilemapWidth < width)
					tilemapWidth = width;

				int height = tilemap.YOffset + tilemap.Tilemap.GetHeight();

				if (tilemapHeight < height)
					tilemapHeight = height;
			}

			if (tuxX == null || tuxY == null || cameraX == null || cameraY == null)
				return new BoundedTilemap(tilemap: new CompositeTilemap(
					normalizedTilemaps: normalizedTilemaps, 
					width: tilemapWidth, 
					height: tilemapHeight,
					tuxX: tuxX,
					tuxY: tuxY,
					mapKeyState: mapKeyState));

			List<CompositeTilemap.TilemapWithOffset> tilemapsNearTuxOrCamera = new List<CompositeTilemap.TilemapWithOffset>();

			int halfWindowWidth = windowWidth >> 1;
			int halfWindowHeight = windowHeight >> 1;

			int tuxLeft = tuxX.Value - halfWindowWidth;
			int tuxRight = tuxX.Value + halfWindowWidth;
			int tuxBottom = tuxY.Value - halfWindowHeight;
			int tuxTop = tuxY.Value + halfWindowHeight;

			int cameraLeft = cameraX.Value - halfWindowWidth;
			int cameraRight = cameraX.Value + halfWindowWidth;
			int cameraBottom = cameraY.Value - halfWindowHeight;
			int cameraTop = cameraY.Value + halfWindowHeight;

			int margin = GameLogicState.MARGIN_FOR_TILEMAP_DESPAWN_IN_PIXELS;

			foreach (CompositeTilemap.TilemapWithOffset tilemap in normalizedTilemaps)
			{
				if (!tilemap.AlwaysIncludeTilemap)
				{
					int tilemapLeft = tilemap.XOffset;
					int tilemapRight = tilemap.XOffset + tilemap.Tilemap.GetWidth();

					if (tilemapRight < tuxLeft - margin && tilemapRight < cameraLeft - margin)
						continue;
					if (tilemapLeft > tuxRight + margin && tilemapLeft > cameraRight + margin)
						continue;

					int tilemapBottom = tilemap.YOffset;
					int tilemapTop = tilemap.YOffset + tilemap.Tilemap.GetHeight();

					if (tilemapTop < tuxBottom - margin && tilemapTop < cameraBottom - margin)
						continue;
					if (tilemapBottom > tuxTop + margin && tilemapBottom > cameraTop + margin)
						continue;
				}

				tilemapsNearTuxOrCamera.Add(tilemap);
			}

			return new BoundedTilemap(tilemap: new CompositeTilemap(
				normalizedTilemaps: tilemapsNearTuxOrCamera, 
				width: tilemapWidth, 
				height: tilemapHeight,
				tuxX: tuxX,
				tuxY: tuxY,
				mapKeyState: mapKeyState));
		}
	}
}
