
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System.Collections.Generic;

	public class TuxStateProcessing
	{
		private const int SPRITE_MILLIS = 300;

		public class Result
		{
			public Result(
				TuxState tuxState,
				bool endLevel,
				bool hasDied,
				bool shouldStopMusic)
			{
				this.TuxState = tuxState;
				this.EndLevel = endLevel;
				this.HasDied = hasDied;
				this.ShouldStopMusic = shouldStopMusic;
			}

			public TuxState TuxState { get; private set; }
			public bool EndLevel { get; private set; }
			public bool HasDied { get; private set; }
			public bool ShouldStopMusic { get; private set; }
		}

		private static Result ProcessFrame_TuxDead(
			TuxState tuxState,
			Move move,
			int elapsedMicrosPerFrame,
			ISoundOutput<GameSound> soundOutput)
		{
			if (tuxState.IsDeadElapsedMicros.Value == 0)
				soundOutput.PlaySound(GameSound.Die);

			int newIsDeadElapsedMicros = tuxState.IsDeadElapsedMicros.Value + elapsedMicrosPerFrame;

			return new Result(
				tuxState: new TuxState(
					xMibi: tuxState.XMibi,
					yMibi: tuxState.YMibi,
					xSpeedInMibipixelsPerSecond: 0,
					ySpeedInMibipixelsPerSecond: 0,
					previousJumps: new List<bool>(),
					isOnGround: false,
					lastTimeOnGround: null,
					spriteElapsedMicros: tuxState.SpriteElapsedMicros + elapsedMicrosPerFrame,
					hasFinishedLevelElapsedMicros: null,
					isStillHoldingJumpButton: false,
					isDeadElapsedMicros: newIsDeadElapsedMicros,
					isFacingRight: tuxState.IsFacingRight),
				endLevel: false,
				hasDied: newIsDeadElapsedMicros >= TuxState.IS_DEAD_ANIMATION_DURATION || move.Respawn,
				shouldStopMusic: false);
		}

		private class MoveInfo
		{
			public MoveInfo(
				bool isMovingRight,
				bool isMovingLeft,
				bool hasJumped)
			{
				this.IsMovingRight = isMovingRight;
				this.IsMovingLeft = isMovingLeft;
				this.HasJumped = hasJumped;
			}

			public bool IsMovingRight { get; private set; }
			public bool IsMovingLeft { get; private set; }
			public bool HasJumped { get; private set; }
		}

		private static MoveInfo GetMoveInfo(
			TuxState tuxState,
			Move move)
		{
			if (tuxState.HasFinishedLevel)
				return new MoveInfo(
					isMovingRight: true,
					isMovingLeft: false,
					hasJumped: false);
			
			bool isMovingRight = move.ArrowRight && !move.ArrowLeft;
			bool isMovingLeft = move.ArrowLeft && !move.ArrowRight;
			bool hasJumped = move.Jumped && (tuxState.PreviousJumps.Count == 0 || !tuxState.PreviousJumps[0]);

			if (!hasJumped && move.Jumped)
			{
				int previousMoveIndex = 0;

				while (true)
				{
					if (previousMoveIndex >= tuxState.PreviousJumps.Count - 1)
						break;

					if (tuxState.PreviousJumps[previousMoveIndex] && !tuxState.PreviousJumps[previousMoveIndex + 1])
					{
						hasJumped = true;
						break;
					}

					previousMoveIndex++;
				}
			}

			return new MoveInfo(
				isMovingRight: isMovingRight,
				isMovingLeft: isMovingLeft,
				hasJumped: hasJumped);
		}

		public static Result ProcessFrame(
			TuxState tuxState,
			Move move,
			bool debugMode,
			IKeyboard debugKeyboardInput,
			IKeyboard debugPreviousKeyboardInput,
			IDisplayProcessing<GameImage> displayProcessing,
			ISoundOutput<GameSound> soundOutput,
			int elapsedMicrosPerFrame,
			ITilemap tilemap)
		{
			if (tuxState.IsDead)
				return ProcessFrame_TuxDead(tuxState: tuxState, move: move, elapsedMicrosPerFrame: elapsedMicrosPerFrame, soundOutput: soundOutput);

			MoveInfo moveInfo = GetMoveInfo(tuxState: tuxState, move: move);

			bool isMovingRight = moveInfo.IsMovingRight;
			bool isMovingLeft = moveInfo.IsMovingLeft;
			bool hasJumped = moveInfo.HasJumped;

			int tuxX = tuxState.XMibi >> 10;
			int tuxY = tuxState.YMibi >> 10;

			bool newIsOnGround = tilemap.IsGround(tuxX, tuxY - 16 * 3 - 1) && !tilemap.IsGround(tuxX, tuxY - 16 * 3)
				|| tilemap.IsGround(tuxX - 4 * 3, tuxY - 16 * 3 - 1) && !tilemap.IsGround(tuxX - 4 * 3, tuxY - 16 * 3)
				|| tilemap.IsGround(tuxX + 4 * 3, tuxY - 16 * 3 - 1) && !tilemap.IsGround(tuxX + 4 * 3, tuxY - 16 * 3);

			int? newLastTimeOnGround;
			if (newIsOnGround && tuxState.YSpeedInMibipixelsPerSecond <= 0)
				newLastTimeOnGround = 0;
			else if (tuxState.LastTimeOnGround == null)
				newLastTimeOnGround = null;
			else
				newLastTimeOnGround = tuxState.LastTimeOnGround.Value + elapsedMicrosPerFrame;

			int newXMibi = tuxState.XMibi;
			int newYMibi = tuxState.YMibi;

			int newXSpeedInMibipixelsPerSecond = tuxState.XSpeedInMibipixelsPerSecond;
			int newYSpeedInMibipixelsPerSecond = tuxState.YSpeedInMibipixelsPerSecond;

			List<bool> newPreviousJumps = new List<bool>();
			int numFramesToTrack = TuxState.JUMP_BUFFER_DURATION / elapsedMicrosPerFrame + 1;
			if (tuxState.PreviousJumps.Count < numFramesToTrack)
			{
				newPreviousJumps.Add(move.Jumped);
				newPreviousJumps.AddRange(tuxState.PreviousJumps);
			}
			else
			{
				newPreviousJumps.Add(move.Jumped);
				for (int i = 0; i < numFramesToTrack - 1; i++)
					newPreviousJumps.Add(tuxState.PreviousJumps[i]);
			}

			int? newHasFinishedLevelElapsedMicros = tuxState.HasFinishedLevelElapsedMicros;

			if (newHasFinishedLevelElapsedMicros.HasValue)
				newHasFinishedLevelElapsedMicros = newHasFinishedLevelElapsedMicros.Value + elapsedMicrosPerFrame;
			else
			{
				if (tilemap.IsEndOfLevel(x: tuxX, y: tuxY))
				{
					soundOutput.PlaySound(GameSound.JingleWin01);
					newHasFinishedLevelElapsedMicros = 0;
				}
				else
				{
					if (debugMode && debugKeyboardInput.IsPressed(Key.One) && !debugPreviousKeyboardInput.IsPressed(Key.One))
					{
						soundOutput.PlaySound(GameSound.JingleWin01);
						newHasFinishedLevelElapsedMicros = TuxState.FINISHED_LEVEL_ANIMATION_DURATION;
					}
				}
			}

			int newSpriteElapsedMicros = tuxState.SpriteElapsedMicros + elapsedMicrosPerFrame;

			if (newSpriteElapsedMicros >= 2 * 1000 * 1000 * 1000)
				newSpriteElapsedMicros = 0;

			if (isMovingRight)
				newXSpeedInMibipixelsPerSecond += elapsedMicrosPerFrame * 3;
			
			if (isMovingLeft)
				newXSpeedInMibipixelsPerSecond -= elapsedMicrosPerFrame * 3;
			
			if (!isMovingRight && !isMovingLeft)
			{
				if (newXSpeedInMibipixelsPerSecond > 0)
				{
					newXSpeedInMibipixelsPerSecond -= elapsedMicrosPerFrame * 3;
					if (newXSpeedInMibipixelsPerSecond < 0)
						newXSpeedInMibipixelsPerSecond = 0;
				}
				if (newXSpeedInMibipixelsPerSecond < 0)
				{
					newXSpeedInMibipixelsPerSecond += elapsedMicrosPerFrame * 3;
					if (newXSpeedInMibipixelsPerSecond > 0)
						newXSpeedInMibipixelsPerSecond = 0;
				}
			}

			if (newXSpeedInMibipixelsPerSecond > 1000 * 1000)
				newXSpeedInMibipixelsPerSecond = 1000 * 1000;

			if (newXSpeedInMibipixelsPerSecond < -1000 * 1000)
				newXSpeedInMibipixelsPerSecond = -1000 * 1000;

			bool newIsStillHoldingJumpButton = tuxState.IsStillHoldingJumpButton && move.Jumped;

			if (hasJumped && tuxState.LastTimeOnGround.HasValue && tuxState.LastTimeOnGround.Value <= TuxState.LAST_TIME_ON_GROUND_BUFFER_DURATION && tuxState.YSpeedInMibipixelsPerSecond <= 0)
			{
				soundOutput.PlaySound(GameSound.Jump);
				newYSpeedInMibipixelsPerSecond = TuxState.JUMP_Y_SPEED;
				newIsStillHoldingJumpButton = true;
				newLastTimeOnGround = null;
				newPreviousJumps = new List<bool>();
			}

			if (!newIsStillHoldingJumpButton && tuxState.IsStillHoldingJumpButton)
				newYSpeedInMibipixelsPerSecond = newYSpeedInMibipixelsPerSecond * 2 / 5;

			if (!tuxState.IsOnGround && newYSpeedInMibipixelsPerSecond >= -5000 * 1000)
				newYSpeedInMibipixelsPerSecond -= elapsedMicrosPerFrame * 3;

			int proposedNewYMibi = (int)(((long)newYMibi) + ((long)newYSpeedInMibipixelsPerSecond) * ((long)elapsedMicrosPerFrame) / 1024L / 1024L);
			if (newYSpeedInMibipixelsPerSecond > 0)
			{
				while (true)
				{
					if (tilemap.IsGround(newXMibi / 1024, proposedNewYMibi / 1024 + 8 * 3)
							|| tilemap.IsGround(newXMibi / 1024 - 4 * 3, proposedNewYMibi / 1024 + 8 * 3)
							|| tilemap.IsGround(newXMibi / 1024 + 4 * 3, proposedNewYMibi / 1024 + 8 * 3))
					{
						newYSpeedInMibipixelsPerSecond = 0;
						proposedNewYMibi -= 1024;
						if (proposedNewYMibi <= newYMibi)
						{
							proposedNewYMibi = newYMibi;
							break;
						}
					}
					else
						break;
				}
			}
			if (newYSpeedInMibipixelsPerSecond < 0)
			{
				while (true)
				{
					if (tilemap.IsGround(newXMibi / 1024, proposedNewYMibi / 1024 - 16 * 3)
							|| tilemap.IsGround(newXMibi / 1024 - 4 * 3, proposedNewYMibi / 1024 - 16 * 3)
							|| tilemap.IsGround(newXMibi / 1024 + 4 * 3, proposedNewYMibi / 1024 - 16 * 3))
					{
						newYSpeedInMibipixelsPerSecond = 0;
						proposedNewYMibi += 1024;
						if (proposedNewYMibi >= newYMibi)
						{
							proposedNewYMibi = newYMibi;
							break;
						}
					}
					else
						break;
				}
			}

			newYMibi = proposedNewYMibi;

			int proposedNewXMibi = (int)(((long)newXMibi) + ((long)newXSpeedInMibipixelsPerSecond) * ((long)elapsedMicrosPerFrame) / 1024L / 1024L);
			if (newXSpeedInMibipixelsPerSecond > 0)
			{
				while (true)
				{
					if (tilemap.IsGround(proposedNewXMibi / 1024 + 4 * 3, newYMibi / 1024 - 16 * 3)
							|| tilemap.IsGround(proposedNewXMibi / 1024 + 4 * 3, newYMibi / 1024)
							|| tilemap.IsGround(proposedNewXMibi / 1024 + 4 * 3, newYMibi / 1024 + 8 * 3))
					{
						newXSpeedInMibipixelsPerSecond = 0;
						proposedNewXMibi -= 1024;
						if (proposedNewXMibi <= newXMibi)
						{
							proposedNewXMibi = newXMibi;
							break;
						}
					}
					else
						break;
				}
			}
			if (newXSpeedInMibipixelsPerSecond < 0)
			{
				while (true)
				{
					if (tilemap.IsGround(proposedNewXMibi / 1024 - 4 * 3, newYMibi / 1024 - 16 * 3)
							|| tilemap.IsGround(proposedNewXMibi / 1024 - 4 * 3, newYMibi / 1024)
							|| tilemap.IsGround(proposedNewXMibi / 1024 - 4 * 3, newYMibi / 1024 + 8 * 3))
					{
						newXSpeedInMibipixelsPerSecond = 0;
						proposedNewXMibi += 1024;
						if (proposedNewXMibi >= newXMibi)
						{
							proposedNewXMibi = newXMibi;
							break;
						}
					}
					else
						break;
				}
			}

			newXMibi = proposedNewXMibi;

			int? newIsDeadElapsedMicros = null;

			if (tilemap.IsKillZone((tuxState.XMibi >> 10) - 4 * 3, (tuxState.YMibi >> 10) - 16 * 3)
					|| tilemap.IsKillZone((tuxState.XMibi >> 10) + 4 * 3, (tuxState.YMibi >> 10) - 16 * 3)
					|| tilemap.IsKillZone((tuxState.XMibi >> 10) - 4 * 3, (tuxState.YMibi >> 10) + 8 * 3)
					|| tilemap.IsKillZone((tuxState.XMibi >> 10) + 4 * 3, (tuxState.YMibi >> 10) + 8 * 3))
				newIsDeadElapsedMicros = 0;
			if (tilemap.IsSpikes((tuxState.XMibi >> 10) - 4 * 3, (tuxState.YMibi >> 10) - 16 * 3)
					|| tilemap.IsSpikes((tuxState.XMibi >> 10) + 4 * 3, (tuxState.YMibi >> 10) - 16 * 3)
					|| tilemap.IsSpikes((tuxState.XMibi >> 10) - 4 * 3, (tuxState.YMibi >> 10) + 8 * 3)
					|| tilemap.IsSpikes((tuxState.XMibi >> 10) + 4 * 3, (tuxState.YMibi >> 10) + 8 * 3))
				newIsDeadElapsedMicros = 0;

			return new Result(
				tuxState: new TuxState(
					xMibi: newXMibi,
					yMibi: newYMibi,
					xSpeedInMibipixelsPerSecond: newXSpeedInMibipixelsPerSecond,
					ySpeedInMibipixelsPerSecond: newYSpeedInMibipixelsPerSecond,
					previousJumps: newPreviousJumps,
					isOnGround: newIsOnGround,
					lastTimeOnGround: newLastTimeOnGround,
					spriteElapsedMicros: newSpriteElapsedMicros,
					hasFinishedLevelElapsedMicros: newHasFinishedLevelElapsedMicros,
					isStillHoldingJumpButton: newIsStillHoldingJumpButton,
					isDeadElapsedMicros: newIsDeadElapsedMicros,
					isFacingRight: isMovingRight
						? true
						: (isMovingLeft ? false : tuxState.IsFacingRight)),
				endLevel: newHasFinishedLevelElapsedMicros.HasValue && newHasFinishedLevelElapsedMicros.Value >= TuxState.FINISHED_LEVEL_ANIMATION_DURATION,
				hasDied: false,
				shouldStopMusic: newHasFinishedLevelElapsedMicros.HasValue);
		}

		public static void Render(
			TuxState tuxState, 
			IDisplayOutput<GameImage, GameFont> displayOutput, 
			CameraState camera, 
			int windowWidth, 
			int windowHeight)
		{
			GameImage image = tuxState.IsFacingRight ? GameImage.Tux : GameImage.TuxMirrored;

			displayOutput = new TranslatedDisplayOutput<GameImage, GameFont>(
				display: displayOutput,
				xOffsetInPixels: -(camera.X - windowWidth / 2),
				yOffsetInPixels: -(camera.Y - windowHeight / 2));

			if (tuxState.IsDead)
			{
				int spriteNum = (tuxState.SpriteElapsedMicros / (1000 * SPRITE_MILLIS)) % 2;

				// velocity = at + v_0
				// position = 1/2 at^2 + v_0 t + y_0

				int initialDeadYVelocity = 450;
				int acceleration = -800;
				int time = tuxState.IsDeadElapsedMicros.Value / 1000;
				int deadYOffset = acceleration * time / 1000 * time / 2 / 1000 + initialDeadYVelocity * time / 1000;

				displayOutput.DrawImageRotatedClockwise(
					image: image,
					imageX: 2 * 32 + spriteNum * 32,
					imageY: 6 * 32,
					imageWidth: 32,
					imageHeight: 32,
					x: tuxState.XMibi / 1024 - 16 * 3,
					y: tuxState.YMibi / 1024 - 16 * 3 + deadYOffset,
					degreesScaled: 0,
					scalingFactorScaled: 3 * 128);
			}
			else
			{
				int spriteNum;

				int tuxXSpeedNormalized = tuxState.IsFacingRight ? tuxState.XSpeedInMibipixelsPerSecond : -tuxState.XSpeedInMibipixelsPerSecond;

				if (tuxXSpeedNormalized == 0 && tuxState.YSpeedInMibipixelsPerSecond == 0)
					spriteNum = (tuxState.SpriteElapsedMicros / (1000 * SPRITE_MILLIS)) % 4;
				else if (tuxState.YSpeedInMibipixelsPerSecond == 0 && tuxXSpeedNormalized < 0)
					spriteNum = 4;
				else if (tuxState.YSpeedInMibipixelsPerSecond == 0 && tuxXSpeedNormalized < 1000 * 10)
					spriteNum = ((tuxState.SpriteElapsedMicros / (1000 * 80)) % 8) + 8;
				else if (tuxState.YSpeedInMibipixelsPerSecond == 0)
					spriteNum = ((tuxState.SpriteElapsedMicros / (1000 * 80)) % 8) + 16;
				else if (tuxState.YSpeedInMibipixelsPerSecond > 0)
					spriteNum = 33;
				else if (tuxState.YSpeedInMibipixelsPerSecond < 0)
					spriteNum = 36;
				else
					spriteNum = 0;

				int imageX = (spriteNum % 8) * 32;
				int imageY = spriteNum / 8 * 32;

				displayOutput.DrawImageRotatedClockwise(
					image: image,
					imageX: imageX,
					imageY: imageY,
					imageWidth: 32,
					imageHeight: 32,
					x: tuxState.XMibi / 1024 - 16 * 3,
					y: tuxState.YMibi / 1024 - 16 * 3,
					degreesScaled: 0,
					scalingFactorScaled: 3 * 128);
			}
		}
	}
}
