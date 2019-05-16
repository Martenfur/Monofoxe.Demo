using ChaiFoxes.FMODAudio;
using Monofoxe.Engine.ECS;

namespace Monofoxe.Demo.GameLogic.Entities
{
	public class PlayerComponent : Component
	{
		public Multibutton Left = GameButtons.Left;
		public Multibutton Right = GameButtons.Right;
		public Multibutton Jump = GameButtons.Jump;
		public Multibutton Crouch = GameButtons.Down;
		
		public Listener3D Listener;
	}
}
