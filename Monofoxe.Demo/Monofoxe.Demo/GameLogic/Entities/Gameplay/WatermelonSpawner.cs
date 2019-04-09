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
		//public Sprite EntitySprite = Resources.Sprites.Default.Gato;


		bool _spawnAnimation = false;
		float _spawnAnimationProgress = 0f;
		
		float _spawnAnimationSpeed = 1f; // 1/second

		/// <summary>
		/// Offset for drawing a dummy sprite.
		/// </summary>
		Vector2 _spawnStemOrigin = new Vector2(-42, -55);

		/// <summary>
		/// Determines, what entity will be spawned.
		/// </summary>
		private string _spawnEntityTag;

		public WatermelonSpawner(Layer layer, Vector2 position, SpawnMode spawnMode, string spawnEntity) : base(layer)
		{
			AddComponent(new PositionComponent(position));

			_spawnMode = spawnMode;
			_spawnEntityTag = spawnEntity;
		}
		

		public override void Update()
		{
			// Spawn, if watermelon been destroyed.
			if (_spawnedEntity == null || _spawnedEntity.Destroyed)
			{
				SpawnEntity();
			}
			// Spawn, if watermelon been destroyed.

			// Spawn, if watermelon been stacked.
			if (
				_spawnedEntity != null
				&& _spawnMode == SpawnMode.OnStacked
				&& _spawnedEntity.GetComponent<StackableActorComponent>().LogicStateMachine.CurrentState == ActorStates.Stacked
			)
			{
				SpawnEntity();
			}
			// Spawn, if watermelon been stacked.

			// Animation.
			if (_spawnAnimation)
			{
				_spawnAnimationProgress += TimeKeeper.GlobalTime(_spawnAnimationSpeed);
				if (_spawnAnimationProgress >= 1f)
				{
					_spawnAnimation = false;
					_spawnedEntity.Enabled = true;
					_spawnedEntity.Visible = true;

				}
			}
			// Animation.
		}
		

		void SpawnEntity()
		{
			if (!_spawnAnimation)
			{
				_spawnAnimationProgress = 0f;
				_spawnAnimation = true;
				
				// Spawner can produce only templated entities.
				_spawnedEntity = CreateFromTemplate(Layer, _spawnEntityTag);
			
				_spawnedEntity.GetComponent<PositionComponent>().Position = GetComponent<PositionComponent>().Position + _spawnPointOffset;
				
				// Disabling spawned entity during the animation.
				_spawnedEntity.Enabled = false;
				_spawnedEntity.Visible = false;
			}
		}

		public override void Draw()
		{
			
			var position = GetComponent<PositionComponent>();
			StemSprite.Draw(position.Position, StemSprite.Origin);
			
			if (_spawnAnimation)
			{
				var animation = (float)Math.Pow(Math.Pow(_spawnAnimationProgress, 3f), 0.5f);
				
				var sprite = _spawnedEntity.GetComponent<StackableActorComponent>().MainSprite;
				
				sprite.Draw( 
					0, 
					position.Position + _spawnStemOrigin, 
					sprite.Origin + new Vector2(0, -sprite.Height),
					animation * Vector2.One,
					0, 
					Color.White
				);

			}
			
		}

		

	}
}
