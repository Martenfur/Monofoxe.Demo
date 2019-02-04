using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine.ECS;
using Microsoft.Xna.Framework;
using Monofoxe.Engine;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;
using Monofoxe.Demo.GameLogic.Collisions;

namespace Monofoxe.Demo.GameLogic.Entities
{
	public class PhysicsSystem : BaseSystem
	{
		public override Type ComponentType => typeof(PhysicsComponent);

		private List<Entity> _solidObjects;
		
		public override int Priority => 2;

		public float Gravity = 400;
		public float MaxFallSpeed = 500; // px/sec


		public override void Update(List<Component> components)
		{
			_solidObjects = SceneMgr.CurrentLayer.GetEntityListByComponent<SolidComponent>();

			foreach(PhysicsComponent cPhysics in components)
			{
				var entity = cPhysics.Owner;
				var cPosition = entity.GetComponent<PositionComponent>();
				
				cPhysics.Color = Color.Black;
				
				var collider = cPhysics.Collider;
				

				// Gravity.
				collider.Position = cPosition.Position + Vector2.UnitY;
				collider.PreviousPosition = cPosition.Position;
				if (
					cPhysics.Speed.Y < MaxFallSpeed 
					&& CheckCollision(entity, collider) == null
				)
				{
					cPhysics.Speed.Y += TimeKeeper.GlobalTime(Gravity);
					if (cPhysics.Speed.Y > MaxFallSpeed)
					{
						cPhysics.Speed.Y = MaxFallSpeed;
					}
				}
				// Gravity.
				
				
				cPosition.Position.X += TimeKeeper.GlobalTime(cPhysics.Speed.X);
				
				collider.Position = cPosition.Position;
				collider.PreviousPosition = cPosition.PreviousPosition;
				var solid = CheckCollision(entity, collider);
				if (solid != null)
				{
					var sign = 1;
					if (cPhysics.Speed.X < 0)
					{
						sign = -1;
					}

					var colliderPos = solid.GetComponent<PositionComponent>();
					var colliderSolid = solid.GetComponent<SolidComponent>();
					
					for(var x = 0; x <= TimeKeeper.GlobalTime(Math.Abs(cPhysics.Speed.X)*2) + 1; x += 1)
					{
						collider.Position = cPosition.Position - Vector2.UnitX * x * sign;
						if (CheckCollision(entity, collider) == null)
						{
							cPhysics.Speed.X = 0;
							cPosition.Position.X -= x * sign;
							break;
						}
					}
					
				}



				cPosition.Position.Y += TimeKeeper.GlobalTime(cPhysics.Speed.Y);

				collider.Position = cPosition.Position;
				collider.PreviousPosition = cPosition.PreviousPosition;		

				if (CheckCollision(entity, collider) != null)
				{
					var sign = 1;
					if (cPhysics.Speed.Y < 0)
					{
						sign = -1;
					}

					for(var y = 0; y <= TimeKeeper.GlobalTime(Math.Abs(cPhysics.Speed.Y) * 2) + 1; y += 1)
					{
						collider.Position = cPosition.Position - Vector2.UnitY * y * sign;
						
						if (CheckCollision(entity, collider) == null)
						{
							cPhysics.Speed.Y = 0;
							cPosition.Position.Y -= y * sign;
							break;
						}
					}
				}

			}
		}

		public override void Draw(Component component)
		{

		}


		Entity CheckCollision(Entity checker, ICollider collider)
		{
			
			foreach(var solid in _solidObjects)
			{
				if (solid != checker)
				{
					var otherCollider = solid.GetComponent<SolidComponent>().Collider;
					otherCollider.Position = solid.GetComponent<PositionComponent>().Position;

					if (CollisionDetector.CheckCollision(collider, otherCollider))
					{
						return solid;
					}
				}
			}
			return null;
		}

	}
}
