
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using System;

	public enum GameMusic
	{
		Airship2,
		Theme,
		PeaceAtLast,
		Chipdisko,
		Jewels,
		BossTheme
	}

	public static class GameMusicUtil
	{
		public class MusicFilenameInfo
		{
			public MusicFilenameInfo(string defaultFilename, string wavFilename)
			{
				this.DefaultFilename = defaultFilename;
				this.WavFilename = wavFilename;
			}

			public string DefaultFilename { get; private set; }
			public string WavFilename { get; private set; }
		}

		public static MusicFilenameInfo GetMusicFilename(this GameMusic music)
		{
			switch (music)
			{
				case GameMusic.Airship2:
					return new MusicFilenameInfo(
						defaultFilename: "JasonLavallee/airship_2.ogg",
						wavFilename: "JasonLavallee/airship_2.wav");
				case GameMusic.Theme:
					return new MusicFilenameInfo(
						defaultFilename: "wansti/theme.ogg",
						wavFilename: "wansti/theme.wav");
				case GameMusic.PeaceAtLast:
					return new MusicFilenameInfo(
						defaultFilename: "Trex0n/peace_at_last.ogg",
						wavFilename: "Trex0n/peace_at_last.wav");
				case GameMusic.Chipdisko:
					return new MusicFilenameInfo(
						defaultFilename: "LukasNystrand/chipdisko.ogg",
						wavFilename: "LukasNystrand/chipdisko.wav");
				case GameMusic.Jewels:
					return new MusicFilenameInfo(
						defaultFilename: "cynicmusic/music_jewels.ogg",
						wavFilename: "cynicmusic/music_jewels.wav");
				case GameMusic.BossTheme:
					return new MusicFilenameInfo(
						defaultFilename: "migfus20/boss.ogg",
						wavFilename: "migfus20/boss.wav");
				default: throw new Exception();
			}
		}

		// From 0 to 100 (both inclusive)
		public static int GetMusicVolume(this GameMusic music)
		{
			switch (music)
			{
				case GameMusic.Airship2: return 40;
				case GameMusic.Theme: return 10;
				case GameMusic.PeaceAtLast: return 30;
				case GameMusic.Chipdisko: return 70;
				case GameMusic.Jewels: return 30;
				case GameMusic.BossTheme: return 30;
				default: throw new Exception();
			}
		}
	}
}
