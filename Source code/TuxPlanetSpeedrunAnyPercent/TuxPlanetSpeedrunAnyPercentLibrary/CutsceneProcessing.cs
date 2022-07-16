
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class CutsceneProcessing
	{
		public const string SAVESTATE_CUTSCENE = "savestate_cutscene";
		public const string TIME_SLOWDOWN_CUTSCENE = "time_slowdown_cutscene";

		public const int LEVEL_THAT_GRANTS_SAVESTATES = 2;
		public const int LEVEL_THAT_GRANTS_TIME_SLOWDOWN = 4;

		public static ICutscene GetCutscene(string cutsceneName)
		{
			if (cutsceneName == SAVESTATE_CUTSCENE)
				return Cutscene_SaveState.GetCutscene();
			else if (cutsceneName == TIME_SLOWDOWN_CUTSCENE)
				return Cutscene_TimeSlowdown.GetCutscene();
			else
				throw new Exception();
		}

		public class Result
		{
			public Result(
				Move move,
				CameraState cameraState,
				List<IEnemy> newEnemies,
				ICutscene cutscene,
				bool shouldGrantSaveStatePower,
				bool shouldGrantTimeSlowdownPower)
			{
				this.Move = move;
				this.CameraState = cameraState;
				this.NewEnemies = new List<IEnemy>(newEnemies);
				this.Cutscene = cutscene;
				this.ShouldGrantSaveStatePower = shouldGrantSaveStatePower;
				this.ShouldGrantTimeSlowdownPower = shouldGrantTimeSlowdownPower;
			}

			public Move Move { get; private set; }

			public CameraState CameraState { get; private set; }

			public IReadOnlyList<IEnemy> NewEnemies { get; private set; }

			public ICutscene Cutscene { get; private set; }

			public bool ShouldGrantSaveStatePower { get; private set; }

			public bool ShouldGrantTimeSlowdownPower { get; private set; }
		}
	}
}
