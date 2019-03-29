using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;

namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public class CannonBall : Entity
	{
		public new string Tag => "cannonBall";
		
		public static readonly float Size = 10;
		
		private float _speed = 600;
		private Vector2 _direction;
		
		private bool _dead = false;
		private Vector2 _deadSpeed = Vector2.Zero;
		private float _deadGravity = 3000;

		private ICollider _collider;

		private Cannon _myCannon;

		private int _deathDispersion = 30;

		public CannonBall(Vector2 position, Vector2 direction, Cannon myCannon, Layer layer) : base(layer)
		{
			AddComponent(new PositionComponent(position));
			
			_collider = new RectangleCollider();
			_collider.Size = Vector2.One * Size;
			_collider.Position = position;
			
			_direction = direction;
			_myCannon = myCannon;
		}


		public override void Update()
		{
			var position = GetComponent<PositionComponent>();

			if (!_dead)
			{
				_collider.Position = position.Position;
				_collider.PreviousPosition = position.PreviousPosition;

				if (!CollisionDetector.CheckCollision(_collider, _myCannon.GetComponent<SolidComponent>().Collider))
				{
					foreach(var solidEntity in Scene.GetEntityListByComponent<SolidComponent>())
					{
						if (CollisionDetector.CheckCollision(_collider, solidEntity.GetComponent<SolidComponent>().Collider))
						{
							Die();
						}
					}
				}

				var actors = SceneMgr.CurrentScene.GetEntityListByComponent<StackableActorComponent>();
				foreach(var actorEntity in actors)
				{
					var playerActor = actorEntity.GetComponent<StackableActorComponent>();
					
					var playerPosition = actorEntity.GetComponent<PositionComponent>();
					var playerPhysics = actorEntity.GetComponent<PhysicsComponent>();	
				
					// Setting up colliders.
					playerPhysics.Collider.Position = playerPosition.Position;
					playerPhysics.Collider.PreviousPosition = playerPosition.PreviousPosition	;
					// Seting up colliders.

					if (CollisionDetector.CheckCollision(_collider, playerPhysics.Collider))
					{
						StackableActorSystem.Damage(actorEntity.GetComponent<StackableActorComponent>());
						Die();
						break;
					}
				}
	
				position.Position += _direction * TimeKeeper.GlobalTime(_speed);
			}
			else
			{
				_deadSpeed.Y += TimeKeeper.GlobalTime(_deadGravity);
				position.Position += TimeKeeper.GlobalTime(_deadSpeed);
			}

		}

		
		public override void Draw()
		{
			var position = GetComponent<PositionComponent>();
			DrawMgr.CurrentColor = Color.Orange;
			
			var rotation = (float)GameMath.Direction(_direction);
			
			DrawMgr.DrawSprite(Resources.Sprites.Default.CannonBall, 0, position.Position, Vector2.One, rotation, Color.White);
		}

		private void Die()
		{
			_dead = true;		
			_deadSpeed.X = -_speed * _direction.X / 4 + GameplayController.Random.Next(-_deathDispersion, _deathDispersion);
			_deadSpeed.Y = -_speed  + GameplayController.Random.Next(-_deathDispersion, _deathDispersion);
		}
	}
}
