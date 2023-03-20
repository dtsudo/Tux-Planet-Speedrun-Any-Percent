
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class Cutscene_Level8 : ICutscene
	{
		private enum Status
		{
			A_Camera,
			B_Dialogue,
			C_Win
		}

		private Status status;
		private DialogueList dialogueList;

		private const int CUTSCENE_CAMERA_X = 60 * 48;
		private const int CUTSCENE_CAMERA_Y = 600;

		private Cutscene_Level8(
			Status status,
			DialogueList dialogueList)
		{
			this.status = status;
			this.dialogueList = dialogueList;
		}

		public static ICutscene GetCutscene()
		{
			List<Dialogue> dialogues = new List<Dialogue>();

			dialogues.Add(Dialogue.GetDialogue(
				x: 500,
				y: 340,
				width: 490,
				height: 120,
				text: "Meh. A water level. \n\nWater levels are terrible in every \nsingle video game ever."));

			dialogues.Add(Dialogue.GetDialogue(
				x: 10,
				y: 340,
				width: 446,
				height: 63,
				text: "Agreed. And there are sometimes \norcas in the water."));

			dialogues.Add(Dialogue.GetDialogue(
				x: 500,
				y: 360,
				width: 490,
				height: 40,
				text: "Let's just skip this level."));

			DialogueList dialogueList = new DialogueList(dialogues: dialogues);

			return new Cutscene_Level8(
				status: Status.A_Camera,
				dialogueList: dialogueList);
		}

		public string GetCutsceneName()
		{
			return CutsceneProcessing.LEVEL_8_CUTSCENE;
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
			Difficulty difficulty,
			IReadOnlyList<IEnemy> enemies,
			IReadOnlyList<string> levelFlags)
		{
			CameraState newCameraState;
			DialogueList newDialogueList;
			Status newStatus;
			List<string> newlyAddedLevelFlags = new List<string>();

			switch (this.status)
			{
				case Status.A_Camera:
				{
					CameraState destinationCameraState = CameraState.GetCameraState(
						x: CUTSCENE_CAMERA_X,
						y: CUTSCENE_CAMERA_Y);

					newDialogueList = this.dialogueList;

					if (cameraState.X == destinationCameraState.X && cameraState.Y == destinationCameraState.Y)
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
						newStatus = Status.C_Win;
					else
						newStatus = Status.B_Dialogue;

					break;
				case Status.C_Win:
					newlyAddedLevelFlags.Add(LevelConfiguration_Level8.HAS_FINISHED_CUTSCENE);
					newCameraState = cameraState;
					newDialogueList = this.dialogueList;
					newStatus = Status.C_Win;

					break;
				default:
					throw new Exception();
			}

			return new CutsceneProcessing.Result(
				move: new Move(jumped: false, teleported: false, arrowLeft: false, arrowRight: true, arrowUp: false, arrowDown: false, respawn: false),
				cameraState: newCameraState,
				enemies: new List<IEnemy>(enemies),
				newlyAddedLevelFlags: newlyAddedLevelFlags,
				cutscene: new Cutscene_Level8(status: newStatus, dialogueList: newDialogueList),
				shouldGrantSaveStatePower: false,
				shouldGrantTimeSlowdownPower: false,
				shouldGrantTeleportPower: false);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput, int windowWidth, int windowHeight)
		{
			if (this.status == Status.B_Dialogue)
				this.dialogueList.Render(displayOutput: displayOutput, windowWidth: windowWidth, windowHeight: windowHeight);
		}
	}
}
