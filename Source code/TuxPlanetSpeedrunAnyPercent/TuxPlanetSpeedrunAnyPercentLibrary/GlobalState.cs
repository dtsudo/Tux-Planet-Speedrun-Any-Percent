
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System.Collections.Generic;

	public class GlobalState
	{
		public const int DEFAULT_VOLUME = 50;

		public GlobalState(
			int windowWidth,
			int windowHeight,
			int fps,
			IDTRandom rng,
			GuidGenerator guidGenerator,
			IDTLogger logger,
			ITimer timer,
			IFileIO fileIO,
			bool isWebBrowserVersion,
			bool isWebPortalVersion,
			bool debugMode,
			int? initialMusicVolume)
		{
			this.WindowWidth = windowWidth;
			this.WindowHeight = windowHeight;
			this.Fps = fps;
			this.Rng = rng;
			this.GuidGenerator = guidGenerator;
			this.Logger = logger;
			this.Timer = timer;
			this.IsWebBrowserVersion = isWebBrowserVersion;
			this.IsWebPortalVersion = isWebPortalVersion;
			this.DebugMode = debugMode;
			this.desiredMusicVolume = initialMusicVolume ?? GlobalState.DEFAULT_VOLUME;
			this.currentMusicVolume = this.desiredMusicVolume;

			int elapsedMicrosPerFrame = 1000 * 1000 / fps;

			this.MusicPlayer = new MusicPlayer(elapsedMicrosPerFrame: elapsedMicrosPerFrame);
			this.ElapsedMicrosPerFrame = elapsedMicrosPerFrame;

			this.saveAndLoadData = new SaveAndLoadData(fileIO: fileIO);

			this.Debug_ShowHitBoxes = false;

			this.MapInfo = MapDataHelper.GetStronglyTypedMapData(mapData: MapData.GetMapData());
		}

		public int WindowWidth { get; private set; }
		public int WindowHeight { get; private set; }
		public int Fps { get; private set; }
		public IDTRandom Rng { get; private set; }
		public GuidGenerator GuidGenerator { get; private set; }
		public IDTLogger Logger { get; private set; }
		public ITimer Timer { get; private set; }
		public bool IsWebBrowserVersion { get; private set; }
		public bool IsWebPortalVersion { get; private set; }
		public bool DebugMode { get; private set; }

		public bool Debug_ShowHitBoxes { get; set; }

		public IReadOnlyDictionary<string, MapDataHelper.Map> MapInfo { get; private set; }

		private SaveAndLoadData saveAndLoadData;

		private int desiredMusicVolume;
		private int currentMusicVolume;
		public int MusicVolume
		{
			get { return this.desiredMusicVolume; }
			set { this.desiredMusicVolume = value; }
		}

		public MusicPlayer MusicPlayer { get; private set; }

		public int ElapsedMicrosPerFrame { get; private set; }

		public void ProcessMusic()
		{
			this.MusicPlayer.ProcessFrame();
			this.currentMusicVolume = VolumeUtil.GetVolumeSmoothed(
				elapsedMicrosPerFrame: this.ElapsedMicrosPerFrame,
				currentVolume: this.currentMusicVolume,
				desiredVolume: this.desiredMusicVolume);
		}

		public void RenderMusic(IMusicOutput<GameMusic> musicOutput)
		{
			this.MusicPlayer.RenderMusic(musicOutput: musicOutput, userVolume: this.currentMusicVolume);
		}

		public void SaveData(SessionState sessionState, int soundVolume)
		{
			this.saveAndLoadData.SaveData(sessionState: sessionState, soundVolume: soundVolume, musicVolume: this.desiredMusicVolume);
		}

		public void LoadSessionState(SessionState sessionState)
		{
			this.saveAndLoadData.LoadSessionState(sessionState: sessionState, windowWidth: this.WindowWidth, windowHeight: this.WindowHeight, mapInfo: this.MapInfo);
		}

		public int? LoadSoundVolume()
		{
			return this.saveAndLoadData.LoadSoundVolume();
		}

		public void LoadMusicVolume()
		{
			int? musicVolume = this.saveAndLoadData.LoadMusicVolume();

			if (musicVolume.HasValue)
			{
				this.desiredMusicVolume = musicVolume.Value;
				this.currentMusicVolume = musicVolume.Value;
			}
		}
	}
}
