using Microsoft.Xna.Framework;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine;
using Monofoxe.Engine.Utils;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Demo.GameLogic.Entities.Gameplay;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Resources.Sprites;
using System;

namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public class SleepParticle : Entity
	{
	
		Vector2 _speed = new Vector2(0, -50);

		float _animation = 0;
		float _animationSpeed = 0.5f;

		float _amplitude = 45; // deg

		float _distance = 16;

		float _alpha = 1;
		float _alphaDecaySpeed = 0.25f;


		public SleepParticle(Vector2 position, Layer layer) : base(layer)
		{
			AddComponent(new PositionComponent(position));
		}

		public override void Update()
		{
			var position = GetComponent<PositionComponent>();
			position.Position += TimeKeeper.GlobalTime(_speed);

			_animation += TimeKeeper.GlobalTime(_animationSpeed);
			if (_animation >= 1)
			{
				_animation -= 1;
			}

			_alpha -= TimeKeeper.GlobalTime(_alphaDecaySpeed);
			if (_alpha <= 0)
			{	
				DestroyEntity();
			}
		}

		public override void Draw()
		{
			var position = GetComponent<PositionComponent>();

			var phase = (float)Math.Sin(_animation * Math.PI * 2) * _amplitude + 90;

			var offset = new Vector2(
				(float)Math.Cos(MathHelper.ToRadians(phase)),
				(float)Math.Sin(MathHelper.ToRadians(phase)) * 2
			) * _distance;

			GraphicsMgr.CurrentColor = Color.White * (1 - (1-_alpha) * (1-_alpha)); // It just works (tm).
			Default.SleepParticle.Draw(position.Position + offset, Default.SleepParticle.Origin);
		}
	}
}
