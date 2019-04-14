using Microsoft.Xna.Framework;
using Monofoxe.Engine.Drawing;
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

		private bool _switchOnce;

		private bool _switchingAllowed = true;

		public Switchblock(Vector2 position, bool active, bool switchOnce, Layer layer) : base(layer)
		{
			Active = active;
			_switchOnce = switchOnce;

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
				if (_myButton.Pressed && _switchingAllowed)
				{
					Toggle();
					if (_switchOnce)
					{
						_switchingAllowed = false;
					}
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
			GraphicsMgr.CurrentColor = Color.White;
			
			if (TryGetComponent(out LinkComponent link))
			{
				if (link.Pair == null)
				{
					GraphicsMgr.CurrentColor = Color.Red;
				}
			}
			Resources.Sprites.Default.Switchblock.Draw(1 - Active.ToInt(), position.Position, Resources.Sprites.Default.Switchblock.Origin);
		}

		public void Toggle()
		{
			var solid = GetComponent<SolidComponent>();

			Active = !Active;
			solid.Collider.Enabled = Active;
		}


	}
}
