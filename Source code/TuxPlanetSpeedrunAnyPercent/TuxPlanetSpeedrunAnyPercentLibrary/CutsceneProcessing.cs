
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class CutsceneProcessing
	{
		public const string SAVESTATE_CUTSCENE = "savestate_cutscene";
		public const string TIME_SLOWDOWN_CUTSCENE = "time_slowdown_cutscene";
		public const string TELEPORT_CUTSCENE = "teleport_cutscene";
		public const string BOSS_CUTSCENE = "boss_cutscene";
		public const string BOSS_DEFEATED_CUTSCENE = "boss_defeated_cutscene";

		public static ICutscene GetCutscene(string cutsceneName, IReadOnlyDictionary<string, string> customLevelInfo)
		{
			if (cutsceneName == SAVESTATE_CUTSCENE)
				return Cutscene_SaveState.GetCutscene();
			else if (cutsceneName == TIME_SLOWDOWN_CUTSCENE)
				return Cutscene_TimeSlowdown.GetCutscene();
			else if (cutsceneName == TELEPORT_CUTSCENE)
				return Cutscene_Teleport.GetCutscene();
			else if (cutsceneName == BOSS_CUTSCENE)
				return Cutscene_KonqiBossIntro.GetCutscene(customLevelInfo: customLevelInfo);
			else if (cutsceneName == BOSS_DEFEATED_CUTSCENE)
				return Cutscene_KonqiBossDefeated.GetCutscene(customLevelInfo: customLevelInfo);
			else
				throw new Exception();
		}

		public class Result
		{
			public Result(
				Move move,
				CameraState cameraState,
				List<IEnemy> enemies,
				IReadOnlyList<string> newlyAddedLevelFlags,
				ICutscene cutscene,
				bool shouldGrantSaveStatePower,
				bool shouldGrantTimeSlowdownPower,
				bool shouldGrantTeleportPower)
			{
				this.Move = move;
				this.CameraState = cameraState;
				this.Enemies = new List<IEnemy>(enemies);
				if (newlyAddedLevelFlags == null)
					this.NewlyAddedLevelFlags = new List<string>();
				else
					this.NewlyAddedLevelFlags = new List<string>(newlyAddedLevelFlags);
				this.Cutscene = cutscene;
				this.ShouldGrantSaveStatePower = shouldGrantSaveStatePower;
				this.ShouldGrantTimeSlowdownPower = shouldGrantTimeSlowdownPower;
				this.ShouldGrantTeleportPower = shouldGrantTeleportPower;
			}

			public Move Move { get; private set; }

			public CameraState CameraState { get; private set; }

			public IReadOnlyList<IEnemy> Enemies { get; private set; }

			public IReadOnlyList<string> NewlyAddedLevelFlags { get; private set; }

			public ICutscene Cutscene { get; private set; }

			public bool ShouldGrantSaveStatePower { get; private set; }

			public bool ShouldGrantTimeSlowdownPower { get; private set; }

			public bool ShouldGrantTeleportPower { get; private set; }
		}
	}
}
