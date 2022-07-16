
namespace TuxPlanetSpeedrunAnyPercent
{
	using Bridge;
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using TuxPlanetSpeedrunAnyPercentLibrary;

	public class BridgeMusic : IMusic<GameMusic>
	{
		public BridgeMusic()
		{
			Script.Eval(@"
				window.BridgeMusicJavascript = ((function () {
					'use strict';
						
					var musicDictionary = {};
					
					var numberOfAudioObjectsLoaded = 0;
					
					var loadMusic = function (musicNames) {
						var musicNamesArray = musicNames.split(',');
						
						var numberOfAudioObjects = musicNamesArray.length;
						
						for (var i = 0; i < musicNamesArray.length; i++) {
							var musicName = musicNamesArray[i];
							
							if (musicDictionary[musicName])
								continue;
										
							var musicPath = 'Data/Music/' + musicName + '?doNotCache=' + Date.now().toString();
							
							var audio = new Audio(musicPath);
							audio.addEventListener('canplaythrough', function () {
								numberOfAudioObjectsLoaded++;
							});
							audio.loop = true;
							
							musicDictionary[musicName] = audio;
						}
						
						return numberOfAudioObjects === numberOfAudioObjectsLoaded;
					};
					
					var playMusic = function (musicName, volume) {
						var music = musicDictionary[musicName];
						
						if (volume > 1.0)
							volume = 1.0;
						if (volume < 0.0)
							volume = 0.0;
										
						for (var m in musicDictionary) {
							var audio = musicDictionary[m];
							
							if (audio === music) {
								audio.volume = volume;
								var audioPromise = audio.play();
								if (audioPromise) {
									audioPromise.then(function () {}, function () {});
								}
							} else {
								audio.pause();
								audio.currentTime = 0;
							}
						}
					};
					
					var stopMusic = function () {
						for (var musicName in musicDictionary) {
							var audio = musicDictionary[musicName];
							audio.pause();
							audio.currentTime = 0;
						}
					};
					
					return {
						loadMusic: loadMusic,
						playMusic: playMusic,
						stopMusic: stopMusic
					};
				})());
			");
		}
		
		public bool LoadMusic()
		{
			string musicNames = "";
			bool isFirst = true;
			foreach (GameMusic gameMusic in Enum.GetValues(typeof(GameMusic)))
			{
				if (isFirst)
					isFirst = false;
				else
					musicNames = musicNames + ",";
				musicNames = musicNames + gameMusic.GetMusicFilename();
			}
			
			if (musicNames == "")
				return true;
			
			bool result = Script.Eval<bool>("window.BridgeMusicJavascript.loadMusic('" + musicNames + "')");
			
			return result;
		}
		
		public void PlayMusic(GameMusic music, int volume)
		{
			double finalVolume = (music.GetMusicVolume() / 100.0) * (volume / 100.0);
			if (finalVolume > 1.0)
				finalVolume = 1.0;
			if (finalVolume < 0.0)
				finalVolume = 0.0;
			
			Script.Call("window.BridgeMusicJavascript.playMusic", music.GetMusicFilename(), finalVolume);
		}
		
		public void StopMusic()
		{
			Script.Call("window.BridgeMusicJavascript.stopMusic");
		}
		
		public void DisposeMusic()
		{
		}
	}
}
