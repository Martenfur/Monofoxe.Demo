using System;
using System.Collections.Generic;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;
using Microsoft.Xna.Framework;

namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public class Checkpoint : Entity
	{
		public new string Tag => "watermelonSpawner";
		
		// TODO: Replace with better range check.
		private int _activationRadius = 100;

		private bool _active = false;


		public Checkpoint(Vector2 position, Layer layer) : base(layer)
		{
			AddComponent(new PositionComponent(position));
			
		}


		public override void Update()
		{
			var players = SceneMgr.CurrentScene.GetEntityListByComponent<PlayerComponent>();

			var position = GetComponent<PositionComponent>();

			if (!_active)
			{
				foreach(var player in players)
				{
					if (GameMath.Distance(player.GetComponent<PositionComponent>().Position, position.Position) < _activationRadius)
					{
						// Activating this checkpoint and deactivating all others.
						foreach(var checkpoint in SceneMgr.CurrentScene.GetEntityList<Checkpoint>())
						{
							checkpoint._active = false;	
						}
						_active = true;

						var defaultLayer = SceneMgr.GetScene("default")["default"];
						var checkpointMgr = defaultLayer.FindEntity<CheckpointManager>();
						if (checkpointMgr != null)
						{
							checkpointMgr.CheckpointPosition = position.Position;
						}
					}
				}
			}
			
		}



		public override void Draw()
		{
			var position = GetComponent<PositionComponent>();

			if (_active)
			{
				DrawMgr.CurrentColor = Color.Pink * 0.5f;
			}
			else
			{
				DrawMgr.CurrentColor = Color.Gray * 0.5f;
			}

			DrawMgr.DrawCircle(position.Position, _activationRadius, false);
	
			DrawMgr.CurrentColor = Color.White;
		}

	}
}
