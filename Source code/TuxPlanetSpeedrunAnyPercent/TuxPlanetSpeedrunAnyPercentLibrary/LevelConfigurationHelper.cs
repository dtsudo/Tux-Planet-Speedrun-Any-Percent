
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class LevelConfigurationHelper
	{
		public static ITilemap GetTilemap(
			IReadOnlyList<CompositeTilemap.TilemapWithOffset> normalizedTilemaps,
			int? tuxX, 
			int? tuxY, 
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

			if (tuxX == null || tuxY == null)
				return new BoundedTilemap(tilemap: new CompositeTilemap(normalizedTilemaps: normalizedTilemaps, width: tilemapWidth, height: tilemapHeight));

			List<CompositeTilemap.TilemapWithOffset> tilemapsNearTux = new List<CompositeTilemap.TilemapWithOffset>();

			int halfWindowWidth = windowWidth >> 1;
			int halfWindowHeight = windowHeight >> 1;

			int cameraLeft = tuxX.Value - halfWindowWidth;
			int cameraRight = tuxX.Value + halfWindowWidth;
			int cameraBottom = tuxY.Value - halfWindowHeight;
			int cameraTop = tuxY.Value + halfWindowHeight;

			int margin = GameLogicState.MARGIN_FOR_TILEMAP_DESPAWN_IN_PIXELS;

			foreach (CompositeTilemap.TilemapWithOffset tilemap in normalizedTilemaps)
			{
				int tilemapLeft = tilemap.XOffset;
				int tilemapRight = tilemap.XOffset + tilemap.Tilemap.GetWidth();

				if (tilemapRight < cameraLeft - margin)
					continue;
				if (tilemapLeft > cameraRight + margin)
					continue;

				int tilemapBottom = tilemap.YOffset;
				int tilemapTop = tilemap.YOffset + tilemap.Tilemap.GetHeight();

				if (tilemapTop < cameraBottom - margin)
					continue;
				if (tilemapBottom > cameraTop + margin)
					continue;

				tilemapsNearTux.Add(tilemap);
			}

			return new BoundedTilemap(tilemap: new CompositeTilemap(normalizedTilemaps: tilemapsNearTux, width: tilemapWidth, height: tilemapHeight));
		}
	}
}
