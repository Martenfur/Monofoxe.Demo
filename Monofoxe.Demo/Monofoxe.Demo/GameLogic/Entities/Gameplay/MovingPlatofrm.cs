using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;


namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public class MovingPlatofrm : Entity
	{
		public new string Tag => "movingPlatform";

		public const float BaseSize = 48;

		public Sprite Sprite = Resources.Sprites.Default.Platform;

		public readonly float Width;

		public readonly float HeightDivider = 4;

		public MovingPlatofrm(Layer layer, Vector2 position, float width, bool looped, float pathSpeed, List<Vector2> pathPoints) : base(layer)
		{
			Width = width;
			
			var cPosition = new PositionComponent(position);
			var cSolid = new SolidComponent();
			
			var collider = new PlatformCollider();
			collider.Size = new Vector2(width * BaseSize, BaseSize / HeightDivider);
			cSolid.Collider = collider;

			var cPath = new PathComponent();

			cPath.Position = cPosition.Position;
			cPath.Points = pathPoints;
			cPath.Speed = pathSpeed;
			cPath.Looped = looped;

			AddComponent(cPosition);
			AddComponent(cSolid);
			AddComponent(cPath);
		}

		public override void Update()
		{
			
		}

		public override void Draw()
		{
			var position = GetComponent<PositionComponent>();
			var offset = new Vector2(Width, 1f / HeightDivider) * BaseSize / 2;
			
			DrawMgr.CurrentColor = Color.White;

			if (Width == 1)
			{
				DrawMgr.DrawFrame(Sprite.Frames[3], (position.Position - offset).Round(), Sprite.Origin);
			}
			else
			{
				DrawMgr.DrawFrame(Sprite.Frames[0], (position.Position - offset).Round(), Sprite.Origin);
				DrawMgr.DrawFrame(Sprite.Frames[2], (position.Position - offset + Vector2.UnitX * BaseSize * (Width - 1)).Round(), Sprite.Origin);

				for(var i = 1; i < Width - 1; i += 1)
				{
					DrawMgr.DrawFrame(Sprite.Frames[1], (position.Position - offset + Vector2.UnitX * BaseSize * i).Round(), Sprite.Origin);
				}

			}
		}


	}
}
