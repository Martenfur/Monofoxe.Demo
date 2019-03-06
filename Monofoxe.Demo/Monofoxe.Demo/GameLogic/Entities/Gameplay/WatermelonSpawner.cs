using System;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;

namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public enum SpawnMode
	{
		// Spawn, if watermelon was destroyed or stacked.
		OnStacked,
		// Spawn, if watermelon was destroyed.
		OnDestroyed,
	}


	public class WatermelonSpawner : Entity
	{
		public new string Tag => "watermelonSpawner";
		
		Entity _spawnedEntity;

		/// <summary>
		/// Offset for spawning actual entity.
		/// </summary>
		Vector2 _spawnPointOffset = new Vector2(-42, -17);

		SpawnMode _spawnMode = SpawnMode.OnStacked;

		public Sprite StemSprite = Resources.Sprites.Default.WatermelonStem;
		public Sprite WatermelonSprite = Resources.Sprites.Default.Watermelon;


		bool _spawningWatermelon = false;
		float _watermelonSpawnAnimation = 0f;
		
		float _watermelonSpawnAnimationSpeed = 1f; // 1/seconds

		/// <summary>
		/// Offset for drawing a dummy sprite.
		/// </summary>
		Vector2 _watermelonSpawnStemOrigin = new Vector2(-42, -55);


		public WatermelonSpawner(Layer layer, Vector2 position, SpawnMode spawnMode) : base(layer)
		{
			AddComponent(new PositionComponent(position));

			_spawnMode = spawnMode;
		}
		
		public override void Update()
		{
			// Spawn, if watermelon been destroyed.
			if (_spawnedEntity == null || _spawnedEntity.Destroyed)
			{
				StartSpawnAnimation();
			}
			// Spawn, if watermelon been destroyed.

			// Spawn, if watermelon been stacked.
			if (
				_spawnedEntity != null
				&& _spawnMode == SpawnMode.OnStacked
				&& _spawnedEntity.GetComponent<StackableActorComponent>().LogicStateMachine.CurrentState == ActorStates.Stacked
			)
			{
				StartSpawnAnimation();
			}
			// Spawn, if watermelon been stacked.

			// Animation.
			if (_spawningWatermelon)
			{
				_watermelonSpawnAnimation += TimeKeeper.GlobalTime(_watermelonSpawnAnimationSpeed);
				if (_watermelonSpawnAnimation >= 1f)
				{
					_spawningWatermelon = false;
					SpawnWatermelon();
				}
			}
			// Animation.
		}
		

		void SpawnWatermelon()
		{
			_spawnedEntity = EntityMgr.CreateEntityFromTemplate(Layer, "watermelon");
			_spawnedEntity.GetComponent<PositionComponent>().Position = GetComponent<PositionComponent>().Position + _spawnPointOffset;
			ComponentMgr.InitComponent(_spawnedEntity.GetComponent<StackableActorComponent>());
		}

		void StartSpawnAnimation()
		{
			if (!_spawningWatermelon)
			{
				_watermelonSpawnAnimation = 0f;
				_spawningWatermelon = true;
			}
		}

		public override void Draw()
		{
			
			var position = GetComponent<PositionComponent>();
			DrawMgr.DrawSprite(StemSprite, position.Position);
			
			if (_spawningWatermelon)
			{
				var animation = (float)Math.Pow(Math.Pow(_watermelonSpawnAnimation, 3f), 0.5f);

				DrawMgr.DrawSprite(
					WatermelonSprite, 
					0, 
					position.Position + _watermelonSpawnStemOrigin, 
					new Vector2(0, -WatermelonSprite.Height),
					animation * Vector2.One,
					0, 
					Color.White
				);

			}
			
		}

		

	}
}
