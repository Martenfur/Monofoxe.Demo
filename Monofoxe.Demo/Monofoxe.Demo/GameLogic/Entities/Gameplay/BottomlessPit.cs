using System;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine;
using Monofoxe.Engine.Utils;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;


namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public class BottomlessPit : Entity
	{
		public new string Tag => "bottomlessPit";
		
		public Vector2 Size;

		public BottomlessPit(Vector2 position, Vector2 size, Layer layer) : base(layer)
		{
			AddComponent(new PositionComponent(position));

			Size = size;

			Visible = false;
		}

		public override void Update()
		{
			var position = GetComponent<PositionComponent>();

			foreach(StackableActorComponent actor in SceneMgr.CurrentScene.GetComponentList<StackableActorComponent>())
			{
				var actorPosition = actor.Owner.GetComponent<PositionComponent>();
	
				if (GameMath.PointInRectangleBySize(actorPosition.Position, position.Position, Size))
				{
					StackableActorSystem.Kill(actor, true);
				}
			}

			
		}

		
		public override void Draw()
		{
			var position = GetComponent<PositionComponent>();
			DrawMgr.DrawRectangle(position.Position - Size / 2, position.Position + Size / 2, true);
		}
	}
}
