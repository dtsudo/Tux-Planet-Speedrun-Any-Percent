
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System;
	using System.Collections.Generic;

	public class Cutscene_KonqiBossIntro : ICutscene
	{
		private enum Status
		{
			A_SpawnKonqi,
			B_Camera,
			C_Dialogue,
			D_Delay
		}

		private Status status;
		private DialogueList dialogueList;

		private IReadOnlyDictionary<string, string> customLevelInfo;

		private const string LEVEL_FLAG_KONQI_BOSS_INTRO_CUTSCENE_KONQI_TELEPORT_OUT = "konqiBossIntroCutsceneKonqiTeleportOut";

		private Cutscene_KonqiBossIntro(
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
			List<Dialogue> dialogues = new List<Dialogue>();

			dialogues.Add(Dialogue.GetDialogue(
				x: 500,
				y: 355,
				width: 490,
				height: 40,
				text: "Hello Tux!"));

			dialogues.Add(Dialogue.GetDialogue(
				x: 500,
				y: 245,
				width: 490,
				height: 150,
				text: "The World Boss is currently on \nvacation. \n\nSo I'll serve as the substitute \nboss today."));

			dialogues.Add(Dialogue.GetDialogue(
				x: 500,
				y: 355,
				width: 490,
				height: 40,
				text: "Are you ready?"));

			DialogueList dialogueList = new DialogueList(dialogues: dialogues);

			return new Cutscene_KonqiBossIntro(
				status: Status.A_SpawnKonqi,
				dialogueList: dialogueList,
				customLevelInfo: new Dictionary<string, string>(customLevelInfo));
		}

		public string GetCutsceneName()
		{
			return CutsceneProcessing.KONQI_BOSS_INTRO_CUTSCENE;
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
			List<IEnemy> newEnemies = new List<IEnemy>(enemies);

			List<string> newLevelFlags = new List<string>();

			int konqiXMibi = (this.customLevelInfo[LevelConfiguration_Level10.BOSS_ROOM_X_OFFSET_START].ParseAsIntCultureInvariant() + 48 * 17) << 10;
			int konqiYMibi = (48 * 3 + 24) << 10;

			switch (this.status)
			{
				case Status.A_SpawnKonqi:
					{
						newCameraState = cameraState;
						newStatus = Status.B_Camera;
						newDialogueList = this.dialogueList;
						newEnemies.Add(EnemyKonqiCutscene.GetEnemyKonqiCutscene(
							xMibi: konqiXMibi,
							yMibi: konqiYMibi,
							isFireKonqi: false,
							shouldTeleportOutLevelFlag: LEVEL_FLAG_KONQI_BOSS_INTRO_CUTSCENE_KONQI_TELEPORT_OUT,
							enemyId: "enemyKonqiCutscene_konqiBossIntro"));
						break;
					}
				case Status.B_Camera:
					{
						CameraState destinationCameraState = LevelConfiguration_Level10.GetKonqiBossRoomCameraState(
							customLevelInfo: this.customLevelInfo,
							tilemap: tilemap,
							windowWidth: windowWidth,
							windowHeight: windowHeight);
							
						newDialogueList = this.dialogueList;

						if (cameraState.X == destinationCameraState.X && cameraState.Y == destinationCameraState.Y)
						{
							newCameraState = cameraState;
							newStatus = Status.C_Dialogue;
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
				case Status.C_Dialogue:
					DialogueList.Result dialogueListResult = this.dialogueList.ProcessFrame(
						move: move,
						elapsedMicrosPerFrame: elapsedMicrosPerFrame);

					newCameraState = cameraState;
					newDialogueList = dialogueListResult.DialogueList;

					if (dialogueListResult.IsDone)
					{
						newLevelFlags.Add(LevelConfiguration_Level10.LOCK_CAMERA_ON_KONQI_BOSS_ROOM);
						newLevelFlags.Add(LevelConfiguration_Level10.MARK_LEFT_AND_RIGHT_WALLS_OF_BOSS_ROOM_AS_GROUND);
						newLevelFlags.Add(LEVEL_FLAG_KONQI_BOSS_INTRO_CUTSCENE_KONQI_TELEPORT_OUT);
						newLevelFlags.Add(EnemyBossDoor.LEVEL_FLAG_CLOSE_BOSS_DOORS);
						newLevelFlags.Add(LevelConfiguration_Level10.START_PLAYING_KONQI_BOSS_MUSIC);

						int bossRoomXOffsetStart = this.customLevelInfo[LevelConfiguration_Level10.BOSS_ROOM_X_OFFSET_START].ParseAsIntCultureInvariant();
						int bossRoomXOffsetEnd = this.customLevelInfo[LevelConfiguration_Level10.BOSS_ROOM_X_OFFSET_END].ParseAsIntCultureInvariant();

						newEnemies.Add(EnemyKonqiBoss.GetEnemyKonqiBoss(
							xMibi: konqiXMibi,
							yMibi: konqiYMibi,
							difficulty: difficulty,
							enemyId: "cutscene_konqiBossIntro_konqiBoss",
							rngSeed: this.customLevelInfo[LevelConfiguration_Level10.KONQI_BOSS_RNG_SEED]));
						newStatus = Status.D_Delay;
					}
					else
					{
						newStatus = Status.C_Dialogue;
					}

					break;
				case Status.D_Delay:
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

			if ((tuxXMibi >> 10) < this.customLevelInfo[LevelConfiguration_Level10.BOSS_ROOM_X_OFFSET_START].ParseAsIntCultureInvariant() - 50)
				newMove = new Move(jumped: false, teleported: false, arrowLeft: false, arrowRight: true, arrowUp: false, arrowDown: false, respawn: false);
			else
				newMove = Move.EmptyMove();

			return new CutsceneProcessing.Result(
				move: newMove,
				cameraState: newCameraState,
				enemies: newEnemies,
				newlyAddedLevelFlags: newLevelFlags,
				cutscene: new Cutscene_KonqiBossIntro(status: newStatus, dialogueList: newDialogueList, customLevelInfo: this.customLevelInfo),
				shouldGrantSaveStatePower: false,
				shouldGrantTimeSlowdownPower: false,
				shouldGrantTeleportPower: false);
		}

		public void Render(IDisplayOutput<GameImage, GameFont> displayOutput, int windowWidth, int windowHeight)
		{
			if (this.status == Status.C_Dialogue)
				this.dialogueList.Render(displayOutput: displayOutput, windowWidth: windowWidth, windowHeight: windowHeight);
		}
	}
}
