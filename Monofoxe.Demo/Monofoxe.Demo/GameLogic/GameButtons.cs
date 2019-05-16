using Monofoxe.Engine;

namespace Monofoxe.Demo.GameLogic
{
	public static class GameButtons
	{
		public static Multibutton Left = new Multibutton(Buttons.A, Buttons.Left);
		public static Multibutton Right = new Multibutton(Buttons.D, Buttons.Right);
		public static Multibutton Up = new Multibutton(Buttons.W, Buttons.Up);
		public static Multibutton Down = new Multibutton(Buttons.S, Buttons.Down);

		public static Multibutton Jump = new Multibutton(Buttons.W, Buttons.Space, Buttons.Z);

		public static Multibutton Back = new Multibutton(Buttons.Escape, Buttons.Back);
		public static Multibutton Select = new Multibutton(Buttons.Space, Buttons.Enter);
	}
}
