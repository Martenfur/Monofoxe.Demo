using System;
using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;
using Monofoxe.Engine.Utils.Cameras;
using Monofoxe.Engine;


namespace Monofoxe.Demo.GameLogic
{
	public class Pause : Entity
	{
		private Scene _targetScene;

		public Pause(Layer layer, Scene targetScene) : base(layer)
		{
			_targetScene = targetScene;	
			_targetScene.Enabled = false;
		}

		public override void Update()
		{
			if (Input.CheckButtonPress(Buttons.Escape))
			{
				Unpause();
			}
		}

		public override void Draw()
		{

		}


		private void Unpause()
		{
			_targetScene.Enabled = !_targetScene.Enabled;
			DestroyEntity();
		}

	}
}
