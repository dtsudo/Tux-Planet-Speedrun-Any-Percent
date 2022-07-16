
namespace TuxPlanetSpeedrunAnyPercentLibrary
{
	public class Move
	{
		public Move(
			bool jumped,
			bool teleported,
			bool arrowLeft,
			bool arrowRight,
			bool arrowUp,
			bool arrowDown,
			bool respawn)
		{
			this.Jumped = jumped;
			this.Teleported = teleported;
			this.ArrowLeft = arrowLeft;
			this.ArrowRight = arrowRight;
			this.ArrowUp = arrowUp;
			this.ArrowDown = arrowDown;
			this.Respawn = respawn;
		}

		public static Move EmptyMove()
		{
			return new Move(
				jumped: false,
				teleported: false,
				arrowLeft: false,
				arrowRight: false,
				arrowUp: false,
				arrowDown: false,
				respawn: false);
		}

		public bool Jumped { get; private set; }
		public bool Teleported { get; private set; }

		public bool ArrowLeft { get; private set; }
		public bool ArrowRight { get; private set; }
		public bool ArrowUp { get; private set; }
		public bool ArrowDown { get; private set; }

		public bool Respawn { get; private set; }
	}
}
