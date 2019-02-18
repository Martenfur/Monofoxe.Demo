using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;
using Monofoxe.Engine;

namespace Monofoxe.Demo.GameLogic.Entities.Core
{
	public class PhysicsSystem : BaseSystem
	{
		public override Type ComponentType => typeof(PhysicsComponent);

		public override int Priority => 2;


		public float GravityMultiplier = 1f;

		/// <summary>
		/// List of solid entities in the current step.
		/// </summary>
		private static List<Entity> _solidEntities;
		
		/// <summary>
		/// Main physics code. Far from the best, but gets job done.
		/// </summary>
		public override void Update(List<Component> components)
		{
			_solidEntities = SceneMgr.CurrentScene.GetEntityListByComponent<SolidComponent>();

			// Gravity.
			foreach(PhysicsComponent cPhysics in components)
			{
				cPhysics.PosAdd = Vector2.Zero;
				cPhysics.CollisionH = 0;
				cPhysics.CollisionV = 0;
				cPhysics.CollidedSolidH = null;
				cPhysics.CollidedSolidV = null;
				cPhysics.Squashed = false;

				var cPosition = cPhysics.Owner.GetComponent<PositionComponent>();
				
				// Setting up the collider.
				var collider = cPhysics.Collider;
				collider.Position = cPosition.Position + Vector2.UnitY;
				collider.PreviousPosition = cPosition.Position;
				// Setting up the collider.

				// Checking space under the entity.
				cPhysics.StandingOn = CheckCollision(cPhysics.Owner, collider); 
				cPhysics.InAir = (cPhysics.StandingOn == null);

				if (cPhysics.InAir)
				{
					// In air.
					if (cPhysics.Speed.Y < cPhysics.MaxFallSpeed)
					{
						cPhysics.Speed.Y += TimeKeeper.GlobalTime(cPhysics.Gravity * GravityMultiplier);

						if (cPhysics.Speed.Y > cPhysics.MaxFallSpeed)
						{
							cPhysics.Speed.Y = cPhysics.MaxFallSpeed;
						}
					}
					// In air.
				}
				else
				{
					// On the ground.

					// Moving entity along with solid object.
					cPhysics.PosAdd = cPhysics.StandingOn.GetComponent<SolidComponent>().Speed * (float)TimeKeeper.GlobalTime();
					
					// On the ground.
				}
			}
			// Gravity.
			
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

				cPhysics.CollidedSolidH = CheckCollision(cPhysics.Owner, collider);
				if (cPhysics.CollidedSolidH != null)
				{
					var solidEntityPosition = cPhysics.CollidedSolidH.GetComponent<PositionComponent>();
					
					// Solid objects are assumed to be stationary, so we're just accomodating 
					// for their speed in physics objects speed. 
					var relativeSpeed = (cPosition.Position - cPosition.PreviousPosition) 
					- (solidEntityPosition.Position - solidEntityPosition.PreviousPosition);

					var sign = 1;
					if (relativeSpeed.X < 0)
					{
						sign = -1;
					}

					var colliderPos = cPhysics.CollidedSolidH.GetComponent<PositionComponent>();
					var colliderSolid = cPhysics.CollidedSolidH.GetComponent<SolidComponent>();
					
					cPhysics.Squashed = true;
					for(var x = 0; x <= Math.Abs(relativeSpeed.X) + 1; x += 1)
					{
						collider.Position -= Vector2.UnitX * sign;
						
						var solidEntity = CheckCollision(cPhysics.Owner, collider);
						if (solidEntity == null)
						{
							cPhysics.Speed.X = 0;
							cPosition.Position = collider.Position;
							cPhysics.CollisionH = sign;
							cPhysics.Squashed = false;
							break;
						}
						else
						{
							cPhysics.CollidedSolidH = solidEntity;
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
				position.Position.Y += TimeKeeper.GlobalTime(solid.Speed.Y);
			}

			foreach(PhysicsComponent cPhysics in components)
			{
				var cPosition = cPhysics.Owner.GetComponent<PositionComponent>();
				var collider = cPhysics.Collider;
	
				cPosition.Position.Y += TimeKeeper.GlobalTime(cPhysics.Speed.Y) + cPhysics.PosAdd.Y;				

				collider.Position = cPosition.Position;
				collider.PreviousPosition = cPosition.PreviousPosition;		
				
				cPhysics.CollidedSolidV = CheckCollision(cPhysics.Owner, collider);
				if (cPhysics.CollidedSolidV != null)
				{
					var solidEntityPosition = cPhysics.CollidedSolidV.GetComponent<PositionComponent>();
					var relativeSpeed = (cPosition.Position - cPosition.PreviousPosition) 
					- (solidEntityPosition.Position - solidEntityPosition.PreviousPosition);

					var sign = 1;
					if (relativeSpeed.Y < 0)
					{
						sign = -1;
					}
					
					cPhysics.Squashed = true;
					for(var y = 0; y <= Math.Abs(relativeSpeed.Y) + 1; y += 1)
					{
						collider.Position -= Vector2.UnitY * sign;
						
						var solidEntity = CheckCollision(cPhysics.Owner, collider);
						if (solidEntity == null)
						{
							cPhysics.Speed.Y = 0;
							cPosition.Position = collider.Position;
							cPhysics.CollisionV = sign;	
							cPhysics.Squashed = false;
							break;
						}
						else
						{
							cPhysics.CollidedSolidV = solidEntity;
						}
					}
					
				}

			}
		}

		
		/// <summary>
		/// Checks collision of a given collider with solid objects.
		/// </summary>
		public static Entity CheckCollision(Entity checker, ICollider collider)
		{
			// Note that this won't work well, if a lot of solid objects will be constantly created\deleted.
			// In case of problems, make an overload searching for a new solid list every call.
			foreach(var solid in _solidEntities)
			{
				if (solid != checker)
				{
					var otherCollider = solid.GetComponent<SolidComponent>().Collider;
					var position = solid.GetComponent<PositionComponent>();
					otherCollider.Position = position.Position;
					otherCollider.PreviousPosition = position.PreviousPosition;

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
