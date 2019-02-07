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

		private List<Entity> _solidEntities;
		
		public override int Priority => 2;

		public float Gravity = 400;
		public float MaxFallSpeed = 900; // px/sec


		public override void Update(List<Component> components)
		{
			_solidEntities = SceneMgr.CurrentScene.GetEntityListByComponent<SolidComponent>();

			
			
			// Gravity.
			foreach(PhysicsComponent cPhysics in components)
			{
				cPhysics.PosAdd = Vector2.Zero;

				var cPosition = cPhysics.Owner.GetComponent<PositionComponent>();
				
				// Setting up the collider.
				var collider = cPhysics.Collider;
				collider.Position = cPosition.Position + Vector2.UnitY;
				collider.PreviousPosition = cPosition.Position;
				// Setting up the collider.

				// Checking space under the entity.
				var solidBelow = CheckCollision(cPhysics.Owner, collider); 
				cPhysics.InAir = (solidBelow == null);

				if (cPhysics.InAir)
				{
					// In air.
					if (cPhysics.Speed.Y < MaxFallSpeed)
					{
						cPhysics.Speed.Y += TimeKeeper.GlobalTime(Gravity);
						if (cPhysics.Speed.Y > MaxFallSpeed)
						{
							cPhysics.Speed.Y = MaxFallSpeed;
						}
					}
					// In air.
				}
				else
				{
					// On the ground.

					// Moving entity along with solid object.
					cPhysics.PosAdd = solidBelow.GetComponent<SolidComponent>().Speed * (float)TimeKeeper.GlobalTime();
					
					// On the ground.
				}
			}
			// Gravity.
			
			// Y must go first because we've modified y by gravity.
			UpdatePhysicsXAxis(components);

			UpdatePhysicsYAxis(components);
			
		}


		private void UpdatePhysicsXAxis(List<Component> components)
		{
			// Moving solid objects.
			foreach(var solidEntity in _solidEntities)
			{
				var position = solidEntity.GetComponent<PositionComponent>();
				var solid = solidEntity.GetComponent<SolidComponent>();
				position.Position.X += solid.Speed.X * (float)TimeKeeper.GlobalTime();
			}
			// Moving solid objects.


			foreach(PhysicsComponent cPhysics in components)
			{
				var cPosition = cPhysics.Owner.GetComponent<PositionComponent>();
				cPosition.Position.X += TimeKeeper.GlobalTime(cPhysics.Speed.X) + cPhysics.PosAdd.X;
				
				// Setting up the collider.
				var collider = cPhysics.Collider;
				collider.Position = cPosition.Position;
				collider.PreviousPosition = cPosition.PreviousPosition;
				// Setting up the collider.

				var solidEntity = CheckCollision(cPhysics.Owner, collider);
				if (solidEntity != null)
				{
					var solidEntityPosition = solidEntity.GetComponent<PositionComponent>();
					var relativeSpeed = (cPosition.Position - cPosition.PreviousPosition) 
					- (solidEntityPosition.Position - solidEntityPosition.PreviousPosition);

					var sign = 1;
					if (relativeSpeed.X < 0)
					{
						sign = -1;
					}

					var colliderPos = solidEntity.GetComponent<PositionComponent>();
					var colliderSolid = solidEntity.GetComponent<SolidComponent>();
					
					for(var x = 0; x <= Math.Abs(relativeSpeed.X) + 1; x += 1)
					{
						collider.Position -= Vector2.UnitX * sign;
						if (CheckCollision(cPhysics.Owner, collider) == null)
						{
							cPhysics.Speed.X = 0;
							cPosition.Position = collider.Position;
							break;
						}
					}
					
				}
			}
		}



		private void UpdatePhysicsYAxis(List<Component> components)
		{
			foreach(var solidEntity in _solidEntities)
			{
				var position = solidEntity.GetComponent<PositionComponent>();
				var solid = solidEntity.GetComponent<SolidComponent>();
				position.Position.Y += solid.Speed.Y * (float)TimeKeeper.GlobalTime();
			}

			foreach(PhysicsComponent cPhysics in components)
			{
				var entity = cPhysics.Owner;
				var cPosition = entity.GetComponent<PositionComponent>();
				var collider = cPhysics.Collider;
				
				cPosition.Position.Y += TimeKeeper.GlobalTime(cPhysics.Speed.Y) + cPhysics.PosAdd.Y;

				collider.Position = cPosition.Position;
				collider.PreviousPosition = cPosition.PreviousPosition;		
				
				var solidEntity = CheckCollision(entity, collider);
				if (solidEntity != null)
				{
					var solidEntityPosition = solidEntity.GetComponent<PositionComponent>();
					var relativeSpeed = (cPosition.Position - cPosition.PreviousPosition) 
					- (solidEntityPosition.Position - solidEntityPosition.PreviousPosition);

					var sign = 1;
					if (relativeSpeed.Y < 0)
					{
						sign = -1;
					}

					for(var y = 0; y <= Math.Abs(relativeSpeed.Y) + 1; y += 1)
					{
						collider.Position -= Vector2.UnitY * sign;
						
						if (CheckCollision(entity, collider) == null)
						{
							cPhysics.Speed.Y = 0;
							cPosition.Position = collider.Position;
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
			
			foreach(var solid in _solidEntities)
			{
				if (solid != checker)
				{
					var otherCollider = solid.GetComponent<SolidComponent>().Collider;
					otherCollider.Position = solid.GetComponent<PositionComponent>().Position;
					otherCollider.PreviousPosition = solid.GetComponent<PositionComponent>().PreviousPosition;

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
