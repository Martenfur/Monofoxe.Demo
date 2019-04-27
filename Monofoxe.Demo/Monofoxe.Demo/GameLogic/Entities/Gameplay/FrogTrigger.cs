using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;


namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	/// <summary>
	/// Frog enemies attack the player only if he is inside trigger's rectangle.
	/// </summary>
	public class FrogTrigger : Entity
	{
		public new string Tag => "frogTrigger";
		
		public Vector2 Size;

		public bool HasPlayer {get; private set;} = false;


		public FrogTrigger(Vector2 position, Vector2 size, Layer layer) : base(layer)
		{
			AddComponent(new PositionComponent(position));

			Size = size;

			Visible = false;
		}

		public override void Update()
		{
			var position = GetComponent<PositionComponent>();
			
			HasPlayer = false;

			foreach(PlayerComponent player in SceneMgr.CurrentScene.GetComponentList<PlayerComponent>())
			{
				var actorPosition = player.Owner.GetComponent<PositionComponent>();
	
				if (GameMath.PointInRectangleBySize(actorPosition.Position, position.Position, Size))
				{
					HasPlayer = true;
					break;
				}
			}

		}

		
		public override void Draw()
		{
			GraphicsMgr.CurrentColor = Color.Red * 0.3f;
			var position = GetComponent<PositionComponent>();
			RectangleShape.DrawBySize(position.Position, Size, false);
		}
	}
}
