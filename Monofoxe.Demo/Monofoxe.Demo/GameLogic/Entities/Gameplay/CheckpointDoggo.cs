﻿using System;
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
	public class CheckpointDoggo : Entity
	{
		public new string Tag => "checkpointDoggo";
		
		private float _vspeed = -4000;

		private float _maxRainbowLength = 2000;

		private Alarm _deathAlarm = new Alarm();
		private double _deathTime = 15;

		private Vector2 _rainbowOffset = new Vector2(0, -2);

		public CheckpointDoggo(Vector2 position, Layer layer) : base(layer)
		{
			AddComponent(new PositionComponent(position));
			_deathAlarm.Set(_deathTime);

			if (SceneMgr.CurrentScene.TryGetLayer("ObjectsFront", out Layer frontLayer))
			{
				Layer = frontLayer;
			}
		}


		public override void Update()
		{
			
			var position = GetComponent<PositionComponent>();
			
			position.Position.Y += TimeKeeper.GlobalTime(_vspeed);

			if (_deathAlarm.Update())
			{
				DestroyEntity();
			}
		}



		public override void Draw()
		{
			var position = GetComponent<PositionComponent>();

			Resources.Sprites.Default.Rainbow.Draw(
				0, 
				position.Position + _rainbowOffset, 
				Resources.Sprites.Default.Rainbow.Origin, 				
				new Vector2(1, Math.Min(_maxRainbowLength, position.StartingPosition.Y - position.Position.Y)),
				0, 
				Color.White
			);
			
			Resources.Sprites.Default.CheckpointDoggo.Draw(position.Position, Resources.Sprites.Default.CheckpointDoggo.Origin);

		}

	}
}
