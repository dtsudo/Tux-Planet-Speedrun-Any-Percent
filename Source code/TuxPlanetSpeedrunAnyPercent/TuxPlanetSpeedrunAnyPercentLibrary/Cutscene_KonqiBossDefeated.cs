
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class Cutscene_KonqiBossDefeated : ICutscene
	{
		private enum Status
		{
			A_Dialogue,
			B_KonqiTeleportsOut
		}

		private Status status;
		private DialogueList dialogueList;
		private bool isFirstFrame;

		private IReadOnlyDictionary<string, string> customLevelInfo;

		private Cutscene_KonqiBossDefeated(
			Status status,
			DialogueList dialogueList,
			bool isFirstFrame,
			IReadOnlyDictionary<string, string> customLevelInfo)
		{
			this.status = status;
			this.dialogueList = dialogueList;
			this.isFirstFrame = isFirstFrame;
			this.customLevelInfo = customLevelInfo;
		}

		public static ICutscene GetCutscene(IReadOnlyDictionary<string, string> customLevelInfo)
		{
			List<Dialogue> dialogues = new List<Dialogue>();

			dialogues.Add(Dialogue.GetDialogue(
				x: 500,
				y: 250,
				width: 490,
				height: 92,
				text: "Why are you allowed to jump on \nme when I have flames on my \nhead? :("));

			DialogueList dialogueList = new DialogueList(dialogues: dialogues);

			return new Cutscene_KonqiBossDefeated(
				status: Status.A_Dialogue,
				dialogueList: dialogueList,
				isFirstFrame: true,
				customLevelInfo: new Dictionary<string, string>(customLevelInfo));
		}

		public string GetCutsceneName()
		{
			return CutsceneProcessing.KONQI_BOSS_DEFEATED_CUTSCENE;
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

			switch (this.status)
			{
				case Status.A_Dialogue:
					{
						newLevelFlags.Add(LevelConfiguration_Level10.STOP_LOCKING_CAMERA_ON_KONQI_BOSS_ROOM);
						newLevelFlags.Add(LevelConfiguration_Level10.LOCK_CAMERA_ON_KONQI_DEFEATED_BOSS_ROOM);
						newLevelFlags.Add(LevelConfiguration_Level10.STOP_PLAYING_KONQI_BOSS_MUSIC);
						newLevelFlags.Add(EnemyBossDoor.LEVEL_FLAG_CLOSE_BOSS_DOORS_INSTANTLY);

						DialogueList.Result dialogueListResult = this.dialogueList.ProcessFrame(
							move: move,
							elapsedMicrosPerFrame: elapsedMicrosPerFrame);

						newDialogueList = dialogueListResult.DialogueList;

						newCameraState = LevelConfiguration_Level10.GetKonqiBossRoomCameraState(
							customLevelInfo: this.customLevelInfo,
							tilemap: tilemap,
							windowWidth: windowWidth,
							windowHeight: windowHeight);

						if (dialogueListResult.IsDone)
						{
							newStatus = Status.B_KonqiTeleportsOut;
							newLevelFlags.Add(LevelConfiguration_Level10.SPAWN_KONQI_BOSS_DEFEAT);
							newLevelFlags.Add(LevelConfiguration_Level10.SPAWN_MYTHRIL_KEY);
						}
						else
						{
							newStatus = Status.A_Dialogue;
						}

						break;
					}
				case Status.B_KonqiTeleportsOut:
					{
						CameraState destinationCameraState = LevelConfiguration_Level10.GetKonqiDefeatedCameraState(
							customLevelInfo: this.customLevelInfo,
							tilemap: tilemap,
							windowWidth: windowWidth,
							windowHeight: windowHeight);

						newCameraState = CameraState.SmoothCameraState(
							currentCamera: cameraState,
							destinationCamera: destinationCameraState,
							elapsedMicrosPerFrame: elapsedMicrosPerFrame,
							cameraSpeedInPixelsPerSecond: CameraState.CUTSCENE_CAMERA_SPEED);

						if (cameraState.X == destinationCameraState.X && cameraState.Y == destinationCameraState.Y)
						{
							newLevelFlags.Add(LevelConfiguration_Level10.CREATE_CHECKPOINT_AFTER_DEFEATING_KONQI);
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

						newStatus = Status.B_KonqiTeleportsOut;
						newDialogueList = this.dialogueList;

						break;
					}
				default:
					throw new Exception();
			}

			Move newMove;

			if (this.isFirstFrame)
				newMove = new Move(jumped: false, teleported: false, arrowLeft: false, arrowRight: true, arrowUp: false, arrowDown: false, respawn: false);
			else
				newMove = Move.EmptyMove();

			return new CutsceneProcessing.Result(
				move: newMove,
				cameraState: newCameraState,
				enemies: newEnemies,
				newlyAddedLevelFlags: newLevelFlags,
				cutscene: new Cutscene_KonqiBossDefeated(
					status: newStatus, 
					dialogueList: newDialogueList,
					isFirstFrame: false,
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
