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
		public new string Tag => "checkpoint";
		
		private Vector2 _activationRegion = new Vector2(64, 200);
		private Vector2 _activationRegionOffset = new Vector2(0, -100);

		private bool _active = false;

		private Vector2 _doggoOffset = new Vector2(0, -39);

		/// <summary>
		/// Alarm, which determines if checkpoint activation effect should be created,
		/// if checkpoint was activated. This is needed for cases when map reloads and 
		/// player gets teleported to the checkpoint.
		/// </summary>
		private Alarm _noEffectAlarm = new Alarm();
		private double _noEffectTime = 1;

		public Checkpoint(Vector2 position, Layer layer) : base(layer)
		{
			AddComponent(new PositionComponent(position));

			_noEffectAlarm.Set(_noEffectTime);
		}


		public override void Update()
		{
			var players = SceneMgr.CurrentScene.GetEntityListByComponent<PlayerComponent>();

			var position = GetComponent<PositionComponent>();

			_noEffectAlarm.Update();

			if (!_active)
			{
				foreach(var player in players)
				{
					if (
						GameMath.PointInRectangle(
							player.GetComponent<PositionComponent>().Position, 	
							position.Position + _activationRegionOffset - _activationRegion / 2, 
							position.Position + _activationRegionOffset + _activationRegion / 2
						)
					)
					{
						// Activating this checkpoint and deactivating all others.
						foreach(var checkpoint in SceneMgr.CurrentScene.GetEntityList<Checkpoint>())
						{
							checkpoint._active = false;	
						}
						_active = true;

						// Don't create any effects, if checkpoint was activated right after it was created.
						if (!_noEffectAlarm.Running)
						{
							new CheckpointDoggo(position.Position + _doggoOffset, Layer);
						}

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
			
			DrawMgr.CurrentColor = Color.White;
			DrawMgr.DrawSprite(Resources.Sprites.Default.CheckpointPedestal, position.Position);

			if (!_active)
			{
				DrawMgr.DrawSprite(Resources.Sprites.Default.CheckpointDoggo, position.Position + _doggoOffset);
			}

		}

	}
}
