using Microsoft.Xna.Framework;
using Monofoxe.Engine;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Demo.GameLogic.Collisions;
using System;

namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public class Switchblock : Entity
	{
		public new string Tag => "switchblock";
		
		public static readonly float Size = 48;

		public bool Active {get; private set;}
		
		private Button _myButton;

		public Switchblock(Vector2 position, bool active, Layer layer) : base(layer)
		{
			Active = active;

			AddComponent(new PositionComponent(position));
			
			var solid = new SolidComponent();
			
			var collider = new RectangleCollider();
			collider.Size = Vector2.One * Size;
			collider.Position = position + Vector2.One * Size / 2;
			
			solid.Collider = collider;
			solid.Collider.Enabled = active;

			AddComponent(solid);
		}

		public override void Update()
		{
			if (_myButton != null)
			{
				if (_myButton.Pressed)
				{
					Toggle();
				}
			}
			else
			{
				if (TryGetComponent(out LinkComponent link))
				{
					if (link.Pair != null)
					{
						_myButton = (Button)link.Pair.Owner;
						RemoveComponent<LinkComponent>();
					}
				}
			}
		}

		
		public override void Draw()
		{
			var position = GetComponent<PositionComponent>();
			DrawMgr.CurrentColor = Color.White;
			DrawMgr.DrawSprite(Resources.Sprites.Default.Switchblock, 1 - Active.ToInt(), position.Position);
		}

		public void Toggle()
		{
			var solid = GetComponent<SolidComponent>();

			Active = !Active;
			solid.Collider.Enabled = Active;

			if (Active)
			{
				foreach(var actorEntity in Scene.GetEntityListByComponent<StackableActorComponent>())
				{
					var physics = actorEntity.GetComponent<PhysicsComponent>();
					var position = actorEntity.GetComponent<PositionComponent>();

					physics.Collider.Position = position.Position;
					physics.Collider.PreviousPosition = position.PreviousPosition;

					if (CollisionDetector.CheckCollision(solid.Collider, physics.Collider))
					{
						if (actorEntity.TryGetComponent(out PlayerComponent player))
						{
							if (!player.Unkillable)
							{
								PlayerSystem.Kill(player);
							}
						}
						else
						{
							var actor = actorEntity.GetComponent<StackableActorComponent>();
							actor.LogicStateMachine.ChangeState(ActorStates.Dead);
						}
					}
				}

			}
		}


	}
}
