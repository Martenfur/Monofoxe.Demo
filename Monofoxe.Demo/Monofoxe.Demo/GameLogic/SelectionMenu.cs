using System;
using Microsoft.Xna.Framework;
using Monofoxe.Engine;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;
using System.Collections.Generic;
using Monofoxe.Demo.GameLogic.Entities.Core;


namespace Monofoxe.Demo.GameLogic
{
	public class SelectionMenu : Entity
	{
		public List<Sprite> Items;
		
		public Vector2 ButtonSize = new Vector2(200, 50);
		public float ButtonSpacing = 32;

		public int SelectedItem = 0;

		public bool Triggered;

		public SelectionMenu(Layer layer, List<Sprite> items, Vector2 position) : base(layer)
		{
			Items = items;
			AddComponent(new PositionComponent(position));
		}

		public override void Update()
		{
			if (GameButtons.Up.CheckPress())
			{
				SelectedItem -= 1;
				if (SelectedItem < 0)
				{
					SelectedItem = Items.Count - 1;
				}
			}

			if (GameButtons.Down.CheckPress())
			{
				SelectedItem += 1;
				if (SelectedItem >= Items.Count)
				{
					SelectedItem = 0;
				}
			}

			Triggered = GameButtons.Select.CheckRelease();
			
		}


		public override void Draw()
		{
			var height = GetMenuHeight();
			var offsetPosition = GetComponent<PositionComponent>().Position - Vector2.UnitY * height / 2;

			for(var i = 0; i < Items.Count; i += 1)
			{
				var buttonPosition = offsetPosition + Vector2.UnitY * (ButtonSize.Y + ButtonSpacing) * i;
				Resources.Sprites.Default.MenuButton.Draw(
					(i == SelectedItem).ToInt(),
					buttonPosition,
					Resources.Sprites.Default.MenuButton.Origin
				);

				Items[i].Draw(buttonPosition, Items[i].Origin);
			}
		}


		public float GetMenuHeight() =>
			Items.Count * (ButtonSize.Y + ButtonSpacing) - ButtonSpacing;	
		

	}
}
