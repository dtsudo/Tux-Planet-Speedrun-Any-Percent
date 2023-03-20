
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class Cutscene_YetiBossIntro : ICutscene
	{
		private enum Status
		{
			A_SpawnEnemies,
			B_Camera,
			C_Dialogue1,
			D_KonqiDisappear,
			E_Dialogue2,
			F_Delay
		}

		private Status status;
		private DialogueList dialogueList1;
		private DialogueList dialogueList2;
		private bool isFirstFrame;

		private IReadOnlyDictionary<string, string> customLevelInfo;

		private const string LEVEL_FLAG_TALKED_WITH_YETI_KONQI_DISAPPEAR = "talkedWithYetiKonqiDisappear";

		private Cutscene_YetiBossIntro(
			Status status,
			DialogueList dialogueList1,
			DialogueList dialogueList2,
			bool isFirstFrame,
			IReadOnlyDictionary<string, string> customLevelInfo)
		{
			this.status = status;
			this.dialogueList1 = dialogueList1;
			this.dialogueList2 = dialogueList2;
			this.isFirstFrame = isFirstFrame;
			this.customLevelInfo = customLevelInfo;
		}

		public static ICutscene GetCutscene(IReadOnlyDictionary<string, string> customLevelInfo)
		{
			List<Dialogue> dialogues1 = new List<Dialogue>();

			dialogues1.Add(Dialogue.GetDialogue(
				x: 730,
				y: 255,
				width: 260,
				height: 90,
				text: "Hi Yeti. I thought \nyou were on \nvacation."));

			dialogues1.Add(Dialogue.GetDialogue(
				x: 590,
				y: 245,
				width: 195,
				height: 40,
				text: "Just got back."));

			dialogues1.Add(Dialogue.GetDialogue(
				x: 730,
				y: 245,
				width: 260,
				height: 180,
				text: "Perfect timing!\n\nYou have a \nchallenger. \n\nThis is Tux!"));

			DialogueList dialogueList1 = new DialogueList(dialogues: dialogues1);

			List<Dialogue> dialogues2 = new List<Dialogue>();

			dialogues2.Add(Dialogue.GetDialogue(
				x: 490,
				y: 245,
				width: 500,
				height: 235,
				text: "Hello Tux. \n\nYou may know me as the Icy Island \nboss from SuperTux and SuperTux \nAdvance. The devs never gave me a \nname but you can just call me Yeti. \n\nNice to meet you!"));

			dialogues2.Add(Dialogue.GetDialogue(
				x: 500,
				y: 245,
				width: 490,
				height: 65,
				text: "I'll be serving as the final boss. \nGood luck; have fun!"));

			DialogueList dialogueList2 = new DialogueList(dialogues: dialogues2);

			return new Cutscene_YetiBossIntro(
				status: Status.A_SpawnEnemies,
				dialogueList1: dialogueList1,
				dialogueList2: dialogueList2,
				isFirstFrame: true,
				customLevelInfo: new Dictionary<string, string>(customLevelInfo));
		}

		public string GetCutsceneName()
		{
			return CutsceneProcessing.YETI_BOSS_INTRO_CUTSCENE;
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
			DialogueList newDialogueList1;
			DialogueList newDialogueList2;
			Status newStatus;
			List<IEnemy> newEnemies = new List<IEnemy>(enemies);

			List<string> newLevelFlags = new List<string>();

			int konqiXMibi = (this.customLevelInfo[LevelConfiguration_Level10.BOSS_ROOM_X_OFFSET_START].ParseAsIntCultureInvariant() + 48 * 17) << 10;

			switch (this.status)
			{
				case Status.A_SpawnEnemies:
					{
						newCameraState = cameraState;
						newStatus = Status.B_Camera;
						newDialogueList1 = this.dialogueList1;
						newDialogueList2 = this.dialogueList2;

						newEnemies.Add(EnemyYetiCutscene.GetEnemyYetiCutscene(
							xMibi: konqiXMibi - 48 * 4 * 1024,
							yMibi: (18 * 48 + 32 * 3) << 10,
							rngSeed: this.customLevelInfo[LevelConfiguration_Level10.YETI_BOSS_RNG_SEED],
							enemyId: "enemyYetiCutscene"));

						newEnemies.Add(EnemyKonqiCutscene.GetEnemyKonqiCutscene(
							xMibi: konqiXMibi,
							yMibi: (18 * 48 + 8 * 3) << 10,
							isFireKonqi: true,
							shouldTeleportOutLevelFlag: LEVEL_FLAG_TALKED_WITH_YETI_KONQI_DISAPPEAR,
							enemyId: "yetiBossIntroCutscene_konqi"));

						break;
					}
				case Status.B_Camera:
					{
						CameraState destinationCameraState = LevelConfiguration_Level10.GetYetiBossRoomCameraState(
							customLevelInfo: this.customLevelInfo,
							tilemap: tilemap,
							windowWidth: windowWidth,
							windowHeight: windowHeight);
							
						newDialogueList1 = this.dialogueList1;
						newDialogueList2 = this.dialogueList2;

						if (cameraState.X == destinationCameraState.X && cameraState.Y == destinationCameraState.Y)
						{
							newCameraState = cameraState;
							newStatus = Status.C_Dialogue1;
						}
						else
						{
							newCameraState = CameraState.SmoothCameraState(
								currentCamera: cameraState,
								destinationCamera: destinationCameraState,
								elapsedMicrosPerFrame: elapsedMicrosPerFrame,
								cameraSpeedInPixelsPerSecond: CameraState.CUTSCENE_CAMERA_SPEED);
							newStatus = Status.B_Camera;
						}

						break;
					}
				case Status.C_Dialogue1:
					{
						DialogueList.Result dialogueListResult = this.dialogueList1.ProcessFrame(
							move: move,
							elapsedMicrosPerFrame: elapsedMicrosPerFrame);

						newCameraState = cameraState;
						newDialogueList1 = dialogueListResult.DialogueList;
						newDialogueList2 = this.dialogueList2;

						if (dialogueListResult.IsDone)
							newStatus = Status.D_KonqiDisappear;
						else
							newStatus = Status.C_Dialogue1;

						break;
					}
				case Status.D_KonqiDisappear:
					newCameraState = cameraState;
					newDialogueList1 = this.dialogueList1;
					newDialogueList2 = this.dialogueList2;
					newStatus = Status.E_Dialogue2;
					newLevelFlags.Add(LEVEL_FLAG_TALKED_WITH_YETI_KONQI_DISAPPEAR);
					newLevelFlags.Add(EnemyYetiCutscene.LEVEL_FLAG_SWITCH_DIRECTIONS);
					break;
				case Status.E_Dialogue2:
					{
						DialogueList.Result dialogueListResult = this.dialogueList2.ProcessFrame(
							move: move,
							elapsedMicrosPerFrame: elapsedMicrosPerFrame);

						newCameraState = cameraState;
						newDialogueList1 = this.dialogueList1;
						newDialogueList2 = dialogueListResult.DialogueList;

						if (dialogueListResult.IsDone)
						{
							newLevelFlags.Add(LevelConfiguration_Level10.STOP_LOCKING_CAMERA_ON_KONQI_DEFEATED_BOSS_ROOM_HARD);
							newLevelFlags.Add(LevelConfiguration_Level10.LOCK_CAMERA_ON_YETI_BOSS_ROOM);
							newLevelFlags.Add(EnemyYetiCutscene.LEVEL_FLAG_DESPAWN_YETI_CUTSCENE_AND_SPAWN_YETI_BOSS);
							newLevelFlags.Add(LevelConfiguration_Level10.START_PLAYING_YETI_BOSS_MUSIC);
							newStatus = Status.F_Delay;
						}
						else
						{
							newStatus = Status.E_Dialogue2;
						}

						break;
					}
				case Status.F_Delay:
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
				cutscene: new Cutscene_YetiBossIntro(
					status: newStatus, 
					dialogueList1: newDialogueList1, 
					dialogueList2: newDialogueList2, 
					isFirstFrame: false, 
					customLevelInfo: this.customLevelInfo),
				shouldGrantSaveStatePower: false,
				shouldGrantTimeSlowdownPower: false,
				shouldGrantTeleportPower: false);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput, int windowWidth, int windowHeight)
		{
			if (this.status == Status.C_Dialogue1)
				this.dialogueList1.Render(displayOutput: displayOutput, windowWidth: windowWidth, windowHeight: windowHeight);
			if (this.status == Status.E_Dialogue2)
				this.dialogueList2.Render(displayOutput: displayOutput, windowWidth: windowWidth, windowHeight: windowHeight);
		}
	}
}
