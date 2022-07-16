﻿
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	using DTLibrary;
	using System.Collections.Generic;

	public class TuxState
	{
		public TuxState(
			int xMibi,
			int yMibi,
			int xSpeedInMibipixelsPerSecond,
			int ySpeedInMibipixelsPerSecond,
			IReadOnlyList<bool> previousJumps,
			bool isOnGround,
			int? lastTimeOnGround,
			int spriteElapsedMicros,
			int? hasFinishedLevelElapsedMicros,
			bool isStillHoldingJumpButton,
			int? isDeadElapsedMicros,
			bool isFacingRight)
		{
			this.XMibi = xMibi;
			this.YMibi = yMibi;
			this.XSpeedInMibipixelsPerSecond = xSpeedInMibipixelsPerSecond;
			this.YSpeedInMibipixelsPerSecond = ySpeedInMibipixelsPerSecond;
			this.PreviousJumps = new List<bool>(previousJumps);
			this.IsOnGround = isOnGround;
			this.LastTimeOnGround = lastTimeOnGround;
			this.SpriteElapsedMicros = spriteElapsedMicros;
			this.HasFinishedLevelElapsedMicros = hasFinishedLevelElapsedMicros;
			this.IsStillHoldingJumpButton = isStillHoldingJumpButton;
			this.IsDeadElapsedMicros = isDeadElapsedMicros;
			this.IsFacingRight = isFacingRight;
		}

		public int XMibi { get; private set; }
		public int YMibi { get; private set; }

		public int XSpeedInMibipixelsPerSecond { get; private set; }
		public int YSpeedInMibipixelsPerSecond { get; private set; }

		public const int JUMP_Y_SPEED = 1100000;

		public IReadOnlyList<bool> PreviousJumps { get; private set; }
		public const int JUMP_BUFFER_DURATION = 1000 * 500;

		public bool IsOnGround { get; private set; }

		public int? LastTimeOnGround { get; private set; }
		public const int LAST_TIME_ON_GROUND_BUFFER_DURATION = 1000 * 500;

		public int SpriteElapsedMicros { get; private set; }

		public int? HasFinishedLevelElapsedMicros { get; private set; }

		public bool HasFinishedLevel { get { return this.HasFinishedLevelElapsedMicros != null; } }

		public const int FINISHED_LEVEL_ANIMATION_DURATION = 1500 * 1000;

		public bool IsStillHoldingJumpButton { get; private set; }

		public int? IsDeadElapsedMicros { get; private set; }

		public bool IsDead { get { return this.IsDeadElapsedMicros != null; } }

		public const int IS_DEAD_ANIMATION_DURATION = 3 * 1000 * 1000;

		public bool IsFacingRight { get; private set; }

		public TuxState SetYSpeedInMibipixelsPerSecond(int ySpeedInMibipixelsPerSecond)
		{
			return new TuxState(
				xMibi: this.XMibi,
				yMibi: this.YMibi,
				xSpeedInMibipixelsPerSecond: this.XSpeedInMibipixelsPerSecond,
				ySpeedInMibipixelsPerSecond: ySpeedInMibipixelsPerSecond,
				previousJumps: this.PreviousJumps,
				isOnGround: this.IsOnGround,
				lastTimeOnGround: this.LastTimeOnGround,
				spriteElapsedMicros: this.SpriteElapsedMicros,
				hasFinishedLevelElapsedMicros: this.HasFinishedLevelElapsedMicros,
				isStillHoldingJumpButton: this.IsStillHoldingJumpButton,
				isDeadElapsedMicros: this.IsDeadElapsedMicros,
				isFacingRight: this.IsFacingRight);
		}

		public TuxState SetIsStillHoldingJumpButton(bool isStillHoldingJumpButton)
		{
			return new TuxState(
				xMibi: this.XMibi,
				yMibi: this.YMibi,
				xSpeedInMibipixelsPerSecond: this.XSpeedInMibipixelsPerSecond,
				ySpeedInMibipixelsPerSecond: this.YSpeedInMibipixelsPerSecond,
				previousJumps: this.PreviousJumps,
				isOnGround: this.IsOnGround,
				lastTimeOnGround: this.LastTimeOnGround,
				spriteElapsedMicros: this.SpriteElapsedMicros,
				hasFinishedLevelElapsedMicros: this.HasFinishedLevelElapsedMicros,
				isStillHoldingJumpButton: isStillHoldingJumpButton,
				isDeadElapsedMicros: this.IsDeadElapsedMicros,
				isFacingRight: this.IsFacingRight);
		}

		public TuxState Kill()
		{
			return new TuxState(
				xMibi: this.XMibi,
				yMibi: this.YMibi,
				xSpeedInMibipixelsPerSecond: 0,
				ySpeedInMibipixelsPerSecond: 0,
				previousJumps: new List<bool>(),
				isOnGround: false,
				lastTimeOnGround: null,
				spriteElapsedMicros: 0,
				hasFinishedLevelElapsedMicros: null,
				isStillHoldingJumpButton: false,
				isDeadElapsedMicros: 0,
				isFacingRight: this.IsFacingRight);
		}

		public List<Hitbox> GetHitboxes()
		{
			return new List<Hitbox>()
			{
				new Hitbox(
					x: (this.XMibi >> 10) - 4 * 3, 
					y: (this.YMibi >> 10) - 16 * 3, 
					width: 8 * 3, 
					height: 24 * 3)
			};
		}

		public static TuxState GetDefaultTuxState(int x, int y)
		{
			return new TuxState(
				xMibi: x << 10,
				yMibi: y << 10,
				xSpeedInMibipixelsPerSecond: 0,
				ySpeedInMibipixelsPerSecond: 0,
				previousJumps: new List<bool>(),
				isOnGround: false,
				lastTimeOnGround: null,
				spriteElapsedMicros: 0,
				hasFinishedLevelElapsedMicros: null,
				isStillHoldingJumpButton: false,
				isDeadElapsedMicros: null,
				isFacingRight: true);
		}
	}
}