
namespace TuxPlanetSpeedrunAnyPercentLibrary 
{
	using DTLibrary;
	using System.Collections.Generic;
	using System.Linq;

	public class MapDataHelper
    {
		public class Tileset
		{
			public Tileset(
				int firstGid,
				int imageWidth,
				int imageHeight,
				string name,
				int tileWidth,
				int tileHeight)
			{
				this.FirstGid = firstGid;
				this.ImageWidth = imageWidth;
				this.ImageHeight = imageHeight;
				this.Name = name;
				this.TileWidth = tileWidth;
				this.TileHeight = tileHeight;
			}

			public int FirstGid { get; private set; }
			public int ImageWidth { get; private set; }
			public int ImageHeight { get; private set; }
			public string Name { get; private set; }
			public int TileWidth { get; private set; }
			public int TileHeight { get; private set; }
		}

		public class Layer
		{
			public Layer(
				List<int> data,
				int width,
				int height,
				string name)
			{
				this.Data = new List<int>(data);
				this.Width = width;
				this.Height = height;
				this.Name = name;
			}

			public IReadOnlyList<int> Data { get; private set; }
			public int Width { get; private set; }
			public int Height { get; private set; }
			public string Name { get; private set; }
		}

		public class Map
		{
			public Map(
				List<Layer> layers,
				List<Tileset> tilesets)
			{
				this.Layers = new List<Layer>(layers);
				this.Tilesets = new List<Tileset>(tilesets);
			}

			public IReadOnlyList<Layer> Layers { get; private set; }
			public IReadOnlyList<Tileset> Tilesets { get; private set; }
		}

		public static IReadOnlyDictionary<string, Map> GetStronglyTypedMapData(Dictionary<string, Dictionary<string, List<Dictionary<string, string>>>> mapData)
		{
			Dictionary<string, Map> returnVal = new Dictionary<string, Map>();

			foreach (KeyValuePair<string, Dictionary<string, List<Dictionary<string, string>>>> kvp in mapData)
			{
				string mapName = kvp.Key;
				Dictionary<string, List<Dictionary<string, string>>> data = kvp.Value;

				returnVal[mapName] = GetStronglyTypedMapDataHelper(data);
			}

			return returnVal;
		}

		private static Map GetStronglyTypedMapDataHelper(Dictionary<string, List<Dictionary<string, string>>> mapData)
		{
			List<Layer> layers = mapData["layers"].Select(x => GetLayer(x)).ToList();
			List<Tileset> tilesets = mapData["tilesets"].Select(x => GetTileset(x)).ToList();

			return new Map(
				layers: layers,
				tilesets: tilesets);
		}

		private static Layer GetLayer(Dictionary<string, string> layerDictionary)
		{
			return new Layer(
				data: layerDictionary["data"].Split(',').Select(x => StringUtil.ParseInt(x)).ToList(),
				width: StringUtil.ParseInt(layerDictionary["width"]),
				height: StringUtil.ParseInt(layerDictionary["height"]),
				name: layerDictionary["name"]);
		}

		private static Tileset GetTileset(Dictionary<string, string> tilesetDictionary)
		{
			return new Tileset(
				firstGid: StringUtil.ParseInt(tilesetDictionary["firstgid"]),
				imageWidth: StringUtil.ParseInt(tilesetDictionary["imagewidth"]),
				imageHeight: StringUtil.ParseInt(tilesetDictionary["imageheight"]),
				name: tilesetDictionary["name"],
				tileWidth: StringUtil.ParseInt(tilesetDictionary["tilewidth"]),
				tileHeight: StringUtil.ParseInt(tilesetDictionary["tileheight"]));
		}
    } 
} 

