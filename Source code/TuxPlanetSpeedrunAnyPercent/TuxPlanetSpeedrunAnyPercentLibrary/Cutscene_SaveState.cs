
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class Cutscene_SaveState : ICutscene
	{
		private enum Status
		{
			A_Camera,
			B_Dialogue,
			C_KonqiDisappear,
			D_Camera
		}

		private Status status;
		private int konqiDisappearElapsedMicros;
		private DialogueList dialogueList;

		private const int KONQI_DISAPPEAR_WAIT_TIME = 500 * 1000;

		private Cutscene_SaveState(
			Status status,
			int konqiDisappearElapsedMicros,
			DialogueList dialogueList)
		{
			this.status = status;
			this.konqiDisappearElapsedMicros = konqiDisappearElapsedMicros;
			this.dialogueList = dialogueList;
		}

		public static ICutscene GetCutscene()
		{
			List<Dialogue> dialogues = new List<Dialogue>();

			dialogues.Add(Dialogue.GetDialogue(
				x: 500,
				y: 460,
				width: 490,
				height: 40,
				text: "Hello. I'm Konqi! Nice to meet you!"));

			dialogues.Add(Dialogue.GetDialogue(
				x: 10,
				y: 460,
				width: 172,
				height: 40,
				text: "Hello Konqi."));

			dialogues.Add(Dialogue.GetDialogue(
				x: 500,
				y: 400,
				width: 490,
				height: 150,
				text: "These levels are so terribly \ndesigned. \n\nBut I have something that can \nhelp -- savestates!"));

			dialogues.Add(Dialogue.GetDialogue(
				x: 500,
				y: 400,
				width: 490,
				height: 60,
				text: "Press S to create a new savestate \nand press A to load that savestate!"));

			DialogueList dialogueList = new DialogueList(dialogues: dialogues);

			return new Cutscene_SaveState(
				status: Status.A_Camera,
				konqiDisappearElapsedMicros: 0,
				dialogueList: dialogueList);
		}

		public string GetCutsceneName()
		{
			return CutsceneProcessing.SAVESTATE_CUTSCENE;
		}

		public CutsceneProcessing.Result ProcessFrame(
			Move move,
			int tuxXMibi,
			int tuxYMibi,
			CameraState cameraState,
			int elapsedMicrosPerFrame,
			int windowWidth,
			int windowHeight,
			ITilemap tilemap)
		{
			CameraState newCameraState;
			DialogueList newDialogueList;
			Status newStatus;
			int newKonqiDisappearElapsedMicros = this.konqiDisappearElapsedMicros;
			List<IEnemy> newEnemies = new List<IEnemy>();

			switch (this.status)
			{
				case Status.A_Camera:
				{
					CameraState destinationCameraState = CameraState.GetCameraState(
						x: (tuxXMibi >> 10) + 440,
						y: tuxYMibi >> 10);

					newDialogueList = this.dialogueList;

					if (cameraState.X >= destinationCameraState.X)
					{
						newCameraState = cameraState;
						newStatus = Status.B_Dialogue;
					}
					else
					{
						newCameraState = CameraState.SmoothCameraState(
							currentCamera: cameraState,
							destinationCamera: destinationCameraState,
							elapsedMicrosPerFrame: elapsedMicrosPerFrame,
							cameraSpeedInPixelsPerSecond: CameraState.CUTSCENE_CAMERA_SPEED);
						newStatus = Status.A_Camera;
					}

					break;
				}
				case Status.B_Dialogue:
					DialogueList.Result dialogueListResult = this.dialogueList.ProcessFrame(
						move: move,
						elapsedMicrosPerFrame: elapsedMicrosPerFrame);

					newCameraState = cameraState;
					newDialogueList = dialogueListResult.DialogueList;

					if (dialogueListResult.IsDone)
					{
						newStatus = Status.C_KonqiDisappear;
						newEnemies.Add(new EnemyRemoveKonqiCutscene(enemyId: "enemyRemoveKonqiCutscene_cutscene_savestate"));
					}
					else
					{
						newStatus = Status.B_Dialogue;
					}

					break;
				case Status.C_KonqiDisappear:
					newKonqiDisappearElapsedMicros += elapsedMicrosPerFrame;

					newCameraState = cameraState;
					newDialogueList = this.dialogueList;

					if (newKonqiDisappearElapsedMicros >= KONQI_DISAPPEAR_WAIT_TIME)
						newStatus = Status.D_Camera;
					else
						newStatus = Status.C_KonqiDisappear;

					break;
				case Status.D_Camera:
				{
					CameraState destinationCameraState = CameraStateProcessing.ComputeCameraState(
						tuxXMibi: tuxXMibi,
						tuxYMibi: tuxYMibi,
						tilemap: tilemap,
						windowWidth: windowWidth,
						windowHeight: windowHeight);

					newDialogueList = this.dialogueList;
					newStatus = Status.D_Camera;

					if (cameraState.X <= destinationCameraState.X)
					{
						return new CutsceneProcessing.Result(
							move: Move.EmptyMove(),
							cameraState: cameraState,
							newEnemies: newEnemies,
							cutscene: null,
							shouldGrantSaveStatePower: true,
							shouldGrantTimeSlowdownPower: false);
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

			return new CutsceneProcessing.Result(
				move: Move.EmptyMove(),
				cameraState: newCameraState,
				newEnemies: newEnemies,
				cutscene: new Cutscene_SaveState(status: newStatus, konqiDisappearElapsedMicros: newKonqiDisappearElapsedMicros, dialogueList: newDialogueList),
				shouldGrantSaveStatePower: this.status == Status.C_KonqiDisappear || this.status == Status.D_Camera,
				shouldGrantTimeSlowdownPower: false);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput, int windowWidth, int windowHeight)
		{
			if (this.status == Status.B_Dialogue)
				this.dialogueList.Render(displayOutput: displayOutput, windowWidth: windowWidth, windowHeight: windowHeight);
		}
	}
}
