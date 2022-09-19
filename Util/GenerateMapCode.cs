
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

public class GenerateMapCode
{
	private const string SOURCE_CODE_FOLDER = "Source code/TuxPlanetSpeedrunAnyPercent";
	
	public static void Main(string[] args)
	{
		List<MapFileInfo> allFiles = GetAllMapFiles();

		string output = "\n";
		output += "namespace TuxPlanetSpeedrunAnyPercentLibrary \n";
		output += "{ \n";
		output += "    using System.Collections.Generic; \n\n";
		output += "    public class MapData \n";
		output += "    { \n";
		output += "        public static Dictionary<string, Dictionary<string, List<Dictionary<string, string>>>> GetMapData() \n";
		output += "        { \n";
		output += "            Dictionary<string, Dictionary<string, List<Dictionary<string, string>>>> dictionary = new Dictionary<string, Dictionary<string, List<Dictionary<string, string>>>>(); \n\n\n";

		for (int i = 0; i < allFiles.Count; i++)
		{
			string fileContents = GetFileContents(fileName: allFiles[i].FileNameWithPath);

			output += ProcessMapFile(fileContents: fileContents, partiallyQualifiedFileName: allFiles[i].PartiallyQualifiedFileName, index: i) + "\n\n\n";
		}

		output += "            return dictionary; \n";
		output += "        } \n";
		output += "    } \n";
		output += "} \n\n";

		string sourceCodeFolder = GetSourceCodeFolder();

		WriteFileContents(fileName: sourceCodeFolder + "TuxPlanetSpeedrunAnyPercentLibrary/MapData.cs", text: output);
	}

	private static string ProcessMapFile(string fileContents, string partiallyQualifiedFileName, int index)
	{
		string indexStr = index.ToString(CultureInfo.InvariantCulture);
		string mapDictionaryName = "mapDictionary" + indexStr;

		string mapName = partiallyQualifiedFileName;
		mapName = mapName.Substring(0, mapName.Length - 4);

		string returnVal = "            Dictionary<string, List<Dictionary<string, string>>> " + mapDictionaryName + " = new Dictionary<string, List<Dictionary<string, string>>>(); \n";
		returnVal += "            dictionary[\"" + mapName + "\"] = " + mapDictionaryName + "; \n\n";

		List<Dictionary<string, string>> tilesets = ProcessTilesets(fileContents: fileContents);
		returnVal += "            " + mapDictionaryName + "[\"tilesets\"] = new List<Dictionary<string, string>>(); \n";

		for (int i = 0; i < tilesets.Count; i++)
		{
			string dictionaryName = "tilesetDictionary" + indexStr + "_" + i.ToString(CultureInfo.InvariantCulture);
			returnVal += ProcessDictionary(dictionary: tilesets[i], dictionaryName: dictionaryName);
			returnVal += "            " + mapDictionaryName + "[\"tilesets\"].Add(" + dictionaryName + "); \n";
		}

		List<Dictionary<string, string>> layers = ProcessLayers(fileContents);
		returnVal += "\n            " + mapDictionaryName + "[\"layers\"] = new List<Dictionary<string, string>>(); \n";

		for (int i = 0; i < layers.Count; i++)
		{
			string dictionaryName = "layersDictionary" + indexStr + "_" + i.ToString(CultureInfo.InvariantCulture);
			returnVal += ProcessDictionary(dictionary: layers[i], dictionaryName: dictionaryName);
			returnVal += "            " + mapDictionaryName + "[\"layers\"].Add(" + dictionaryName + "); \n";
		}

		return returnVal;
	}

	private static string ProcessDictionary(Dictionary<string, string> dictionary, string dictionaryName)
	{
		string returnVal = "            Dictionary<string, string> " + dictionaryName + " = new Dictionary<string, string>(); \n";

		foreach (KeyValuePair<string, string> kvp in dictionary)
		{
			string k = kvp.Key;
			string v = kvp.Value;
			returnVal += "            " + dictionaryName + "[\"" + k + "\"] = \"" + v + "\"; \n";
		}

		return returnVal;
	}

	private static List<Dictionary<string, string>> ProcessTilesets(string fileContents)
	{
		int startIndex = fileContents.IndexOf("\"tilesets\":", StringComparison.InvariantCulture);

		int endIndex = fileContents.IndexOf(']', startIndex: startIndex);

		fileContents = fileContents.Substring(startIndex, length: endIndex - startIndex);

		List<Dictionary<string, string>> tilesets = new List<Dictionary<string, string>>();

		while (true)
		{
			startIndex = fileContents.IndexOf('{');

			if (startIndex == -1)
				break;

			Dictionary<string, string> tileset = new Dictionary<string, string>();

			startIndex = fileContents.IndexOf("firstgid", StringComparison.InvariantCulture) + 10;
			endIndex = fileContents.IndexOf(',', startIndex: startIndex);
			string firstGid = fileContents.Substring(startIndex, length: endIndex - startIndex);
			tileset["firstgid"] = firstGid;

			startIndex = fileContents.IndexOf("imagewidth", StringComparison.InvariantCulture) + 12;
			endIndex = fileContents.IndexOf(',', startIndex: startIndex);
			string imageWidth = fileContents.Substring(startIndex, length: endIndex - startIndex);
			tileset["imagewidth"] = imageWidth;

			startIndex = fileContents.IndexOf("imageheight", StringComparison.InvariantCulture) + 13;
			endIndex = fileContents.IndexOf(',', startIndex: startIndex);
			string imageHeight = fileContents.Substring(startIndex, length: endIndex - startIndex);
			tileset["imageheight"] = imageHeight;

			startIndex = fileContents.IndexOf("name\":", StringComparison.InvariantCulture) + 7;
			endIndex = fileContents.IndexOf(',', startIndex: startIndex);
			string name = fileContents.Substring(startIndex, length: endIndex - startIndex - 1);
			tileset["name"] = name;

			startIndex = fileContents.IndexOf("tilewidth\":", StringComparison.InvariantCulture) + 11;
			endIndex = startIndex;
			while (true)
			{
				if (fileContents[endIndex] > '9' || fileContents[endIndex] < '0')
					break;
				endIndex++;
			}
			string tileWidth = fileContents.Substring(startIndex, length: endIndex - startIndex);
			tileset["tilewidth"] = tileWidth;

			startIndex = fileContents.IndexOf("tileheight\":", StringComparison.InvariantCulture) + 12;
			endIndex = fileContents.IndexOf(',', startIndex: startIndex);
			string tileHeight = fileContents.Substring(startIndex, length: endIndex - startIndex);
			tileset["tileheight"] = tileHeight;

			tilesets.Add(tileset);

			fileContents = fileContents.Substring(fileContents.IndexOf('}') + 1);
		}

		return tilesets;
	}

	private static List<Dictionary<string, string>> ProcessLayers(string fileContents)
	{
		int startIndex = fileContents.IndexOf("\"layers\":", StringComparison.InvariantCulture);

		int squareBracketCount = 0;
		int endIndex = startIndex;
		while (true)
		{
			if (fileContents[endIndex] == '[')
				squareBracketCount++;

			if (fileContents[endIndex] == ']')
			{
				squareBracketCount--;
				if (squareBracketCount == 0)
					break;
			}

			endIndex++;
		}

		fileContents = fileContents.Substring(startIndex, length: endIndex - startIndex);

		List<Dictionary<string, string>> layers = new List<Dictionary<string, string>>();

		while (true)
		{
			startIndex = fileContents.IndexOf('{');

			if (startIndex == -1)
				break;

			Dictionary<string, string> layer = new Dictionary<string, string>();

			startIndex = fileContents.IndexOf("data", StringComparison.InvariantCulture) + 7;
			endIndex = fileContents.IndexOf(']', startIndex: startIndex);
			string data = fileContents.Substring(startIndex, length: endIndex - startIndex).Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", "");
			layer["data"] = data;

			startIndex = fileContents.IndexOf("width", StringComparison.InvariantCulture) + 7;
			endIndex = fileContents.IndexOf(',', startIndex: startIndex);
			string width = fileContents.Substring(startIndex, length: endIndex - startIndex);
			layer["width"] = width;

			startIndex = fileContents.IndexOf("height", StringComparison.InvariantCulture) + 8;
			endIndex = fileContents.IndexOf(',', startIndex: startIndex);
			string height = fileContents.Substring(startIndex, length: endIndex - startIndex);
			layer["height"] = height;

			startIndex = fileContents.IndexOf("name\":", StringComparison.InvariantCulture) + 7;
			endIndex = fileContents.IndexOf(',', startIndex: startIndex);
			string name = fileContents.Substring(startIndex, length: endIndex - startIndex - 1);
			layer["name"] = name;

			layers.Add(layer);

			fileContents = fileContents.Substring(fileContents.IndexOf('}') + 1);
		}

		return layers;
	}

	private class MapFileInfo
	{
		public MapFileInfo(
			string fileNameWithPath,
			string partiallyQualifiedFileName)
		{
			this.FileNameWithPath = fileNameWithPath;
			this.PartiallyQualifiedFileName = partiallyQualifiedFileName;
		}
		
		public string FileNameWithPath { get; private set; }
		public string PartiallyQualifiedFileName { get; private set; }
	}
	
	private static List<MapFileInfo> GetAllMapFiles()
	{
		List<MapFileInfo> returnValue = new List<MapFileInfo>();
		
		string mapFolder = GetDataFolder() + "Maps/";
		
		GetAllMapFilesHelper(list: returnValue, folder: mapFolder, partiallyQualifiedPath: "");
		
		return returnValue;
	}
	
	private static void GetAllMapFilesHelper(List<MapFileInfo> list, string folder, string partiallyQualifiedPath)
	{
		string[] files = Directory.GetFiles(folder);

		foreach (string file in files)
		{
			if (file.ToUpperInvariant().EndsWith(".TMJ", StringComparison.Ordinal))
				list.Add(new MapFileInfo(fileNameWithPath: file, partiallyQualifiedFileName: partiallyQualifiedPath + GetNonQualifiedFilename(file)));
		}
		
		string[] subfolders = Directory.GetDirectories(folder);
		
		for (int i = 0; i < subfolders.Length; i++)
		{
			string subfolder = subfolders[i];
			
			if (!subfolder.EndsWith("/", StringComparison.Ordinal) && !subfolder.EndsWith("\\", StringComparison.Ordinal))
					subfolder = subfolder + "/";
			
			string subfolderNonQualifiedName = GetNonQualifiedFilename(subfolder.Substring(0, subfolder.Length - 1));
			
			GetAllMapFilesHelper(list: list, folder: subfolder, partiallyQualifiedPath: partiallyQualifiedPath + subfolderNonQualifiedName + "/");
		}
	}

	private static string GetNonQualifiedFilename(string fileName)
	{
		int i1 = fileName.LastIndexOf('/');
		int i2 = fileName.LastIndexOf('\\');

		if (i1 == -1 && i2 == -1)
			return fileName;

		int index;
		if (i1 == -1)
			index = i2;
		else if (i2 == -1)
			index = i1;
		else
			index = Math.Max(i1, i2);

		return fileName.Substring(index + 1);
	}

	private static string GetFileContents(string fileName)
	{
		string fileContents;

		using (FileStream fileStream = new FileStream(path: fileName, mode: FileMode.Open, access: FileAccess.Read))
		{
			using (StreamReader streamReader = new StreamReader(fileStream))
			{
				fileContents = streamReader.ReadToEnd();
			}
		}

		return fileContents;
	}

	private static void WriteFileContents(string fileName, string text)
	{
		using (FileStream fileStream = new FileStream(path: fileName, mode: FileMode.Create))
		{
			using (StreamWriter streamWriter = new StreamWriter(fileStream))
			{
				streamWriter.Write(text);
			}
		}
	}

	private static string GetExecutablePath()
	{
		string executablePath = Process.GetCurrentProcess().MainModule.FileName;
		DirectoryInfo executableDirectory = Directory.GetParent(executablePath);

		return executableDirectory.FullName;
	}

	private static string GetDataFolder()
	{
		string path = GetExecutablePath();

		if (Directory.Exists(path + "/Data"))
			return path + "/Data" + "/";

		while (true)
		{
			int i = Math.Max(path.LastIndexOf("/", StringComparison.Ordinal), path.LastIndexOf("\\", StringComparison.Ordinal));

			if (i == -1)
				throw new Exception("Cannot find directory");

			path = path.Substring(0, i);

			if (Directory.Exists(path + "/Data"))
				return path + "/Data" + "/";
		}
	}

	private static string GetSourceCodeFolder()
	{
		string path = GetExecutablePath();

		if (Directory.Exists(path + "/" + SOURCE_CODE_FOLDER))
			return path + "/" + SOURCE_CODE_FOLDER + "/";

		while (true)
		{
			int i = Math.Max(path.LastIndexOf("/", StringComparison.Ordinal), path.LastIndexOf("\\", StringComparison.Ordinal));

			if (i == -1)
				throw new Exception("Cannot find directory");

			path = path.Substring(0, i);

			if (Directory.Exists(path + "/" + SOURCE_CODE_FOLDER))
				return path + "/" + SOURCE_CODE_FOLDER + "/";
		}
	}
}
