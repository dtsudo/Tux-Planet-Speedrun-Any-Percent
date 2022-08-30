
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
			B_KonqiDisappear,
			C_Camera
		}

		private Status status;
		private DialogueList dialogueList;
		private int konqiDisappearElapsedMicros;
		private bool isFirstFrame;

		private const int KONQI_DISAPPEAR_WAIT_TIME = 500 * 1000;

		private IReadOnlyDictionary<string, string> customLevelInfo;

		private Cutscene_KonqiBossDefeated(
			Status status,
			DialogueList dialogueList,
			int konqiDisappearElapsedMicros,
			bool isFirstFrame,
			IReadOnlyDictionary<string, string> customLevelInfo)
		{
			this.status = status;
			this.dialogueList = dialogueList;
			this.konqiDisappearElapsedMicros = konqiDisappearElapsedMicros;
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
				konqiDisappearElapsedMicros: 0,
				isFirstFrame: true,
				customLevelInfo: new Dictionary<string, string>(customLevelInfo));
		}

		public string GetCutsceneName()
		{
			return CutsceneProcessing.BOSS_DEFEATED_CUTSCENE;
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
			int newKonqiDisappearElapsedMicros = this.konqiDisappearElapsedMicros;

			switch (this.status)
			{
				case Status.A_Dialogue:
					{
						newLevelFlags.Add(LevelConfiguration_Level6.BOSS_DEFEATED_RESTORE_DEFAULT_CAMERA);

						DialogueList.Result dialogueListResult = this.dialogueList.ProcessFrame(
							move: move,
							elapsedMicrosPerFrame: elapsedMicrosPerFrame);

						newCameraState = cameraState;
						newDialogueList = dialogueListResult.DialogueList;

						if (dialogueListResult.IsDone)
						{
							newStatus = Status.B_KonqiDisappear;
							newLevelFlags.Add(LevelConfiguration_Level6.DESPAWN_KONQI_AND_REMOVE_BOSS_DOORS);
						}
						else
						{
							newStatus = Status.A_Dialogue;
						}

						break;
					}
				case Status.B_KonqiDisappear:
					{
						newKonqiDisappearElapsedMicros += elapsedMicrosPerFrame;

						newCameraState = cameraState;
						newDialogueList = this.dialogueList;

						if (newKonqiDisappearElapsedMicros >= KONQI_DISAPPEAR_WAIT_TIME)
							newStatus = Status.C_Camera;
						else
							newStatus = Status.B_KonqiDisappear;

						break;
					}
				case Status.C_Camera:
					{
						CameraState destinationCameraState = CameraStateProcessing.ComputeCameraState(
							tuxXMibi: tuxXMibi,
							tuxYMibi: tuxYMibi,
							tuxTeleportStartingLocation: null,
							tuxTeleportInProgressElapsedMicros: null,
							tilemap: tilemap,
							windowWidth: windowWidth,
							windowHeight: windowHeight);

						newDialogueList = this.dialogueList;
						newStatus = Status.C_Camera;

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
					konqiDisappearElapsedMicros: newKonqiDisappearElapsedMicros,
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
