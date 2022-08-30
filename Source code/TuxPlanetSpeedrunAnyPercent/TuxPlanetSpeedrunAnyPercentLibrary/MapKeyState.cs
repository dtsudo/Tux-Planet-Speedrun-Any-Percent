
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class MapKeyState
	{
		public IReadOnlyList<MapKey> CollectedKeys { get; private set; }
		private int elapsedMicros;
		private IReadOnlyList<MapKey> listOfMapKeys;

		public const int MAP_KEY_ACTIVATION_RADIUS_IN_PIXELS = 300;

		public static MapKeyState EmptyMapKeyState()
		{
			return new MapKeyState(
				collectedKeys: new List<MapKey>(),
				elapsedMicros: 0,
				listOfMapKeys: new List<MapKey>(MapKeyUtil.GetOrderedListOfMapKeys()));
		}

		private MapKeyState(IReadOnlyList<MapKey> collectedKeys, int elapsedMicros, IReadOnlyList<MapKey> listOfMapKeys)
		{
			this.CollectedKeys = new List<MapKey>(collectedKeys);
			this.elapsedMicros = elapsedMicros;
			this.listOfMapKeys = listOfMapKeys;
		}

		public MapKeyState ProcessFrame(
			int tuxX,
			int tuxY,
			bool isTuxTeleporting,
			ITilemap tilemap,
			int elapsedMicrosPerFrame)
		{
			int newElapsedMicros = this.elapsedMicros + elapsedMicrosPerFrame;

			if (newElapsedMicros > 2 * 1000 * 1000 * 1000)
				newElapsedMicros = 1;

			List<MapKey> newCollectedKeys = new List<MapKey>(this.CollectedKeys);

			foreach (MapKey mapKey in this.listOfMapKeys)
			{
				if (newCollectedKeys.Contains(mapKey))
					continue;

				if (isTuxTeleporting)
					continue;

				Tuple<int, int> mapKeyLocation = tilemap.GetMapKeyLocation(mapKey: mapKey, xOffset: 0, yOffset: 0);

				if (mapKeyLocation == null)
					continue;

				int mapKeyX = mapKeyLocation.Item1;
				int mapKeyY = mapKeyLocation.Item2;

				if (mapKeyX - 8 * 3 <= tuxX + 4 * 3
						&& tuxX - 4 * 3 <= mapKeyX + 8 * 3
						&& mapKeyY - 8 * 3 <= tuxY + 8 * 3
						&& tuxY - 16 * 3 <= mapKeyY + 8 * 3)
					newCollectedKeys.Add(mapKey);
			}

			return new MapKeyState(
				collectedKeys: newCollectedKeys,
				elapsedMicros: newElapsedMicros,
				listOfMapKeys: this.listOfMapKeys);
		}

		public void Render(
			IDisplayOutput<GameImage, GameFont> absoluteDisplayOutput,
			IDisplayOutput<GameImage, GameFont> translatedDisplayOutput,
			ITilemap tilemap,
			int windowWidth, 
			int windowHeight)
		{
			int collectedMapKeysX = 10;

			foreach (MapKey mapKey in this.listOfMapKeys)
			{
				if (this.CollectedKeys.Contains(mapKey))
				{
					absoluteDisplayOutput.DrawImageRotatedClockwise(
						image: mapKey.GetGameImage(),
						imageX: 0,
						imageY: 0,
						imageWidth: 16,
						imageHeight: 16,
						x: collectedMapKeysX,
						y: windowHeight - 16 * 3 - 10,
						degreesScaled: 0,
						scalingFactorScaled: 3 * 128);

					collectedMapKeysX += 16 * 3;
				}
				else
				{
					Tuple<int, int> mapKeyLocation = tilemap.GetMapKeyLocation(mapKey: mapKey, xOffset: 0, yOffset: 0);

					if (mapKeyLocation != null)
					{
						int spriteNum = (this.elapsedMicros / (100 * 1000)) % 4;

						translatedDisplayOutput.DrawImageRotatedClockwise(
							image: mapKey.GetGameImage(),
							imageX: spriteNum * 16,
							imageY: 0,
							imageWidth: 16,
							imageHeight: 16,
							x: mapKeyLocation.Item1 - 8 * 3,
							y: mapKeyLocation.Item2 - 8 * 3,
							degreesScaled: 0,
							scalingFactorScaled: 3 * 128);
					}
				}
			}
		}
	}
}
