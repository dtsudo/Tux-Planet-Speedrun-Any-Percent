
namespace TuxPlanetSpeedrunAnyPercentLibrary
{	
	using DTLibrary;

	public class TuxPlanetSpeedrunAnyPercent
	{
		public const int FILE_ID_FOR_GLOBAL_CONFIGURATION = 1;
		public const int FILE_ID_FOR_SESSION_STATE = 2;
		public const int FILE_ID_FOR_SOUND_AND_MUSIC_VOLUME = 3;
		public const int FILE_ID_FOR_SIMPLE_DATA_SESSION_STATE = 4;

		public static IFrame<GameImage, GameFont, GameSound, GameMusic> GetFirstFrame(GlobalState globalState)
		{
			var frame = new InitialLoadingScreenFrame(globalState: globalState, isPerformanceTest: false);
			return frame;
		}

		public static IFrame<GameImage, GameFont, GameSound, GameMusic> GetPerformanceTest(GlobalState globalState)
		{
			var frame = new InitialLoadingScreenFrame(globalState: globalState, isPerformanceTest: true);
			return frame;
		}
	}
}
