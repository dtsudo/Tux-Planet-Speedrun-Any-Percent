
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class Cutscene_YetiBossDefeated : ICutscene
	{
		private enum Status
		{
			A_Dialogue,
			B_Camera
		}

		private Status status;
		private DialogueList dialogueList;

		private IReadOnlyDictionary<string, string> customLevelInfo;

		public const string LEVEL_FLAG_YETI_IS_FACING_RIGHT = "Cutscene_YetiBossDefeated_yetiIsFacingRight";

		private Cutscene_YetiBossDefeated(
			Status status,
			DialogueList dialogueList,
			IReadOnlyDictionary<string, string> customLevelInfo)
		{
			this.status = status;
			this.dialogueList = dialogueList;
			this.customLevelInfo = customLevelInfo;
		}

		public static ICutscene GetCutscene(IReadOnlyDictionary<string, string> customLevelInfo)
		{
			return new Cutscene_YetiBossDefeated(
				status: Status.A_Dialogue,
				dialogueList: null,
				customLevelInfo: new Dictionary<string, string>(customLevelInfo));
		}

		public string GetCutsceneName()
		{
			return CutsceneProcessing.YETI_BOSS_DEFEATED_CUTSCENE;
		}

		public CutsceneProcessing.Result ProcessFrame(
			Move move,
			int tuxXMibi,
			int tuxYMibi,
			CameraState cameraState,
			int elapsedMicrosPerFrame,
			int windowWidth,
			int windowHeight,
			ITilemap tilemap,
			IReadOnlyList<IEnemy> enemies,
			IReadOnlyList<string> levelFlags)
		{
			CameraState newCameraState;
			DialogueList newDialogueList;
			Status newStatus;
			List<IEnemy> newEnemies = new List<IEnemy>(enemies);
			List<string> newLevelFlags = new List<string>();

			DialogueList currentDialogueList = this.dialogueList;
			
			if (currentDialogueList == null)
			{
				List<Dialogue> dialogues = new List<Dialogue>();

				dialogues.Add(Dialogue.GetDialogue(
					x: levelFlags.Contains(LEVEL_FLAG_YETI_IS_FACING_RIGHT) ? 400 : 100,
					y: 185,
					width: 490,
					height: 120,
					text: "Oww... \n\nOk you win. But time isn't called \nuntil you finish the level."));

				currentDialogueList = new DialogueList(dialogues: dialogues);
			}

			switch (this.status)
			{
				case Status.A_Dialogue:
					{
						newLevelFlags.Add(LevelConfiguration_Level10.STOP_PLAYING_YETI_BOSS_MUSIC);
						newLevelFlags.Add(LevelConfiguration_Level10.STOP_LOCKING_CAMERA_ON_YETI_BOSS_ROOM);
						newLevelFlags.Add(LevelConfiguration_Level10.SET_CAMERA_TO_YETI_DEFEATED_LOGIC);

						DialogueList.Result dialogueListResult = currentDialogueList.ProcessFrame(
							move: move,
							elapsedMicrosPerFrame: elapsedMicrosPerFrame);

						newDialogueList = dialogueListResult.DialogueList;

						newCameraState = LevelConfiguration_Level10.GetYetiBossRoomCameraState(
							customLevelInfo: this.customLevelInfo,
							tilemap: tilemap,
							windowWidth: windowWidth,
							windowHeight: windowHeight);

						if (dialogueListResult.IsDone)
						{
							newLevelFlags.Add(EnemyBossDoor.LEVEL_FLAG_OPEN_BOSS_DOORS);
							newLevelFlags.Add(LevelConfiguration_Level10.STOP_MARKING_LEFT_AND_RIGHT_WALLS_OF_BOSS_ROOM_AS_GROUND);
							newStatus = Status.B_Camera;
						}
						else
						{
							newStatus = Status.A_Dialogue;
						}

						break;
					}
				case Status.B_Camera:
					{
						CameraState destinationCameraState = LevelConfiguration_Level10.GetYetiBossDefeatedCameraState(
							customLevelInfo: this.customLevelInfo,
							tilemap: tilemap,
							effectiveTuxXMibi: tuxXMibi,
							effectiveTuxYMibi: tuxYMibi,
							windowWidth: windowWidth,
							windowHeight: windowHeight);

						newDialogueList = currentDialogueList;
						newStatus = Status.B_Camera;

						if (cameraState.X == destinationCameraState.X && cameraState.Y == destinationCameraState.Y)
						{
							return new CutsceneProcessing.Result(
								move: Move.EmptyMove(),
								cameraState: cameraState,
								enemies: newEnemies,
								newlyAddedLevelFlags: newLevelFlags,
								cutscene: null,
								shouldGrantSaveStatePower: false,
								shouldGrantTimeSlowdownPower: false,
								shouldGrantTeleportPower: false);
						}
						else
						{
							newCameraState = CameraState.SmoothCameraState(
								currentCamera: cameraState,
								destinationCamera: destinationCameraState,
								elapsedMicrosPerFrame: elapsedMicrosPerFrame,
								cameraSpeedInPixelsPerSecond: CameraState.CUTSCENE_CAMERA_SPEED);
						}

						break;
					}
				default:
					throw new Exception();
			}

			Move newMove = Move.EmptyMove();

			return new CutsceneProcessing.Result(
				move: newMove,
				cameraState: newCameraState,
				enemies: newEnemies,
				newlyAddedLevelFlags: newLevelFlags,
				cutscene: new Cutscene_YetiBossDefeated(
					status: newStatus, 
					dialogueList: newDialogueList,
					customLevelInfo: this.customLevelInfo),
				shouldGrantSaveStatePower: false,
				shouldGrantTimeSlowdownPower: false,
				shouldGrantTeleportPower: false);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput, int windowWidth, int windowHeight)
		{
			if (this.status == Status.A_Dialogue)
				this.dialogueList.Render(displayOutput: displayOutput, windowWidth: windowWidth, windowHeight: windowHeight);
		}
	}
}
