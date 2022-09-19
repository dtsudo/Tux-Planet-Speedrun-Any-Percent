
namespace TuxPlanetSpeedrunAnyPercent
{
	using Bridge;
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using TuxPlanetSpeedrunAnyPercentLibrary;

	public class BridgeFileIO : IFileIO
	{
		public BridgeFileIO()
		{
			Script.Eval(@"
				window.BridgeFileIOJavascript = ((function () {
					'use strict';
					
					var persistData = function (fileName, base64String) {
						try {
							localStorage.setItem(fileName, base64String);
						} catch (error) {
							// do nothing
						}
					};
										
					var fetchData = function (fileName) {
						try {
							var value = localStorage.getItem(fileName);
							return value;
						} catch (error) {
							return null;
						}
					};
					
					var hasData = function (fileName) {
						try {
							var value = localStorage.getItem(fileName);
							return value !== null;
						} catch (error) {
							return false;
						}
					};
										
					return {
						persistData: persistData,
						fetchData: fetchData,
						hasData: hasData
					};
				})());
			");
		}
		
		private string GetFileName(int fileId)
		{
			VersionInfo versionInfo = VersionInfo.GetVersionInfo();
			string alphanumericVersionGuid = versionInfo.AlphanumericVersionGuid;
			return "guid" + alphanumericVersionGuid + "_file" + fileId.ToStringCultureInvariant();
		}
		
		private string GetVersionedFileName(int fileId, int version)
		{
			string gameGuid = VersionInfo.GetGuidForGame();
			return "guid" + gameGuid + "_file" + fileId.ToStringCultureInvariant() + "_version" + version.ToStringCultureInvariant();
		}
		
		public void PersistData(int fileId, ByteList data)
		{
			List<byte> list = new List<byte>();
			
			ByteList.Iterator iterator = data.GetIterator();
			
			while (true)
			{
				if (!iterator.HasNextByte())
					break;
				
				list.Add(iterator.TryPop());
			}
			
			byte[] array = new byte[list.Count];
			for (int i = 0; i < array.Length; i++)
				array[i] = list[i];
			
			string base64String = Convert.ToBase64String(array);
			
			Script.Call("window.BridgeFileIOJavascript.persistData", this.GetFileName(fileId: fileId), base64String);
		}
		
		public void PersistVersionedData(int fileId, int version, ByteList data)
		{
			List<byte> list = new List<byte>();
			
			ByteList.Iterator iterator = data.GetIterator();
			
			while (true)
			{
				if (!iterator.HasNextByte())
					break;
				
				list.Add(iterator.TryPop());
			}
			
			byte[] array = new byte[list.Count];
			for (int i = 0; i < array.Length; i++)
				array[i] = list[i];
			
			string base64String = Convert.ToBase64String(array);
			
			Script.Call("window.BridgeFileIOJavascript.persistData", this.GetVersionedFileName(fileId: fileId, version: version), base64String);
		}
		
		public ByteList FetchData(int fileId)
		{
			string fileName = this.GetFileName(fileId: fileId);
			
			bool hasData = Script.Eval<bool>("window.BridgeFileIOJavascript.hasData('" + fileName + "')");
			
			if (!hasData)
				return null;
			
			string result = Script.Eval<string>("window.BridgeFileIOJavascript.fetchData('" + fileName + "')");
			
			if (result == null)
				return null;
			
			try
			{
				byte[] array = Convert.FromBase64String(result);
				ByteList.Builder byteList = new ByteList.Builder();
				
				for (int i = 0; i < array.Length; i++)
					byteList.Add(array[i]);
				
				return byteList.ToByteList();
			}
			catch (Exception)
			{
			}
			
			return null;
		}
		
		public ByteList FetchVersionedData(int fileId, int version)
		{
			string fileName = this.GetVersionedFileName(fileId: fileId, version: version);
			
			bool hasData = Script.Eval<bool>("window.BridgeFileIOJavascript.hasData('" + fileName + "')");
			
			if (!hasData)
				return null;
			
			string result = Script.Eval<string>("window.BridgeFileIOJavascript.fetchData('" + fileName + "')");
			
			if (result == null)
				return null;
			
			try
			{
				byte[] array = Convert.FromBase64String(result);
				ByteList.Builder byteList = new ByteList.Builder();
				
				for (int i = 0; i < array.Length; i++)
					byteList.Add(array[i]);
				
				return byteList.ToByteList();
			}
			catch (Exception)
			{
			}
			
			return null;
		}
	}
}
