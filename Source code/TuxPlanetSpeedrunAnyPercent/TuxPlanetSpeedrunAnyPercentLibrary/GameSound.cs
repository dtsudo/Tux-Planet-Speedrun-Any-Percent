
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using System;

	public enum GameSound
	{
		Click,
		JingleWin01,
		Die,
		Squish,
		Jump,
		Teleport
	}

	public static class GameSoundUtil
	{
		public class SoundFilenameInfo
		{
			public SoundFilenameInfo(string defaultFilename, string wavFilename)
			{
				this.DefaultFilename = defaultFilename;
				this.WavFilename = wavFilename;
			}

			public string DefaultFilename { get; private set; }
			public string WavFilename { get; private set; }
		}

		public static SoundFilenameInfo GetSoundFilename(this GameSound sound)
		{
			switch (sound)
			{
				case GameSound.Click: return new SoundFilenameInfo(defaultFilename: "Kenney/click3_Modified.wav", wavFilename: "Kenney/click3_Modified.wav");
				case GameSound.JingleWin01: return new SoundFilenameInfo(defaultFilename: "LittleRobotSoundFactory/Jingle_Win_01.ogg", wavFilename: "LittleRobotSoundFactory/Jingle_Win_01_modified.wav");
				case GameSound.Die: return new SoundFilenameInfo(defaultFilename: "Basto/cut.ogg", wavFilename: "Basto/cut.wav");
				case GameSound.Squish: return new SoundFilenameInfo(defaultFilename: "SuperTux/squish.wav", wavFilename: "SuperTux/squish.wav");
				case GameSound.Jump: return new SoundFilenameInfo(defaultFilename: "LittleRobotSoundFactory/Jump_03.wav", wavFilename: "LittleRobotSoundFactory/Jump_03.wav");
				case GameSound.Teleport: return new SoundFilenameInfo(defaultFilename: "Basto/heavy_splash.ogg", wavFilename: "Basto/heavy_splash.wav");
				default: throw new Exception();
			}
		}

		// From 0 to 100 (both inclusive)
		public static int GetSoundVolume(this GameSound sound)
		{
			switch (sound)
			{
				case GameSound.Click: return 30;
				case GameSound.JingleWin01: return 30;
				case GameSound.Die: return 30;
				case GameSound.Squish: return 30;
				case GameSound.Jump: return 30;
				case GameSound.Teleport: return 30;
				default: throw new Exception();
			}
		}
	}
}
