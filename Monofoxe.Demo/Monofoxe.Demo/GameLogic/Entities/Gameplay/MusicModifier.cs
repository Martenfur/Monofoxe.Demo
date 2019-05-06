using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;


namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public class MusicModifier : Entity
	{
		public new string Tag => "musicModifier";
		
		public Vector2 Size;

		public string SoundLayer;
		public float TransitionSpeed;
		public float TransitionValue;

		private bool _inBoundsPrevious = false;

		public MusicModifier(
			Vector2 position, 
			Vector2 size, 
			Layer layer, 
			string soundLayer,
			float transitionSpeed, 
			float transitionValue
		) : base(layer)
		{
			AddComponent(new PositionComponent(position));

			Size = size;

			SoundLayer = soundLayer;
			TransitionSpeed = transitionSpeed;
			TransitionValue = transitionValue;

			//Visible = false;
		}

		public override void Update()
		{
			var position = GetComponent<PositionComponent>();

			foreach(PlayerComponent player in SceneMgr.CurrentScene.GetComponentList<PlayerComponent>())
			{
				var playerPosition = player.Owner.GetComponent<PositionComponent>();
	
				var inBounds = GameMath.PointInRectangleBySize(playerPosition.Position, position.Position, Size);

				if (inBounds && !_inBoundsPrevious)
				{
					GameplayController.music.AddVolumeTransition(SoundLayer, TransitionValue, TransitionSpeed);
				}

				_inBoundsPrevious = inBounds;

			}

			
		}

		
		public override void Draw()
		{
			var position = GetComponent<PositionComponent>();
			RectangleShape.DrawBySize(position.Position, Size, true);
		}
	}
}
