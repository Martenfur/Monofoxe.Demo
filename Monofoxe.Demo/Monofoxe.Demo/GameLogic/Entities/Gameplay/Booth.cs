using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Audio;
using Monofoxe.Demo.GameLogic.Collisions;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;
using System;
using System.Collections.Generic;

namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public class Booth : Entity
	{
		public new string Tag => "booth";

		Vector2 _offset = new Vector2(55, -45);

		Entity _gato;

		float _dialogueTriggerR = 200;

		string[] _dialogueLines = {
			"foxe: sup",
			"gato: hey",
			"gato: how's your day been?",
			"foxe: ok, i guess",
			"gato: cool",
			"foxe: do you know that our/speech bubble code has been/shamelessly stolen from/oh no, someone stole my hands?",
			"gato: wow, that's extremely lazy",
			"foxe: yeah",
		};

		Dictionary<string, PositionComponent> _actors = new Dictionary<string, PositionComponent>();

		bool _dialogueStarted = false;

		int _currentLine = 0;

		SpeechBubble _currentBubble;

		Alarm _delayAlarm = new Alarm();
		double _delay = 1.5;

		public Booth(Vector2 position, Entity gato, Layer layer) : base(layer)
		{

			AddComponent(new PositionComponent(position));

			_gato = gato;

			_gato.GetComponent<PositionComponent>().Position = position + _offset;
			_gato.GetComponent<PhysicsComponent>().Enabled = false;
			_gato.RemoveComponent<CatEnemyComponent>();

			var gatoActor = gato.GetComponent<StackableActorComponent>();
			gatoActor.MainSprite = Resources.Sprites.Default.SirGato;
			gatoActor.CurrentSprite = gatoActor.MainSprite;
			gatoActor.Orientation = -1;
			//gatoActor.Talking = true;
			gatoActor.AnimationStateMachine.ChangeState(ActorAnimationStates.Talking);
		}

		public override void Update()
		{
			var playerEntity = Scene.FindEntityByComponent<PlayerComponent>();

			var playerPosition = playerEntity.GetComponent<PositionComponent>();
			var position = GetComponent<PositionComponent>();

			if (!_dialogueStarted)
			{
				if ((playerPosition.Position - position.Position).Length() < _dialogueTriggerR)
				{
					// Disabling controls.
					var player = playerEntity.GetComponent<PlayerComponent>();
					var playerActor = playerEntity.GetComponent<StackableActorComponent>();

					player.ControlsEnabled = false;
					playerActor.LeftAction = false;
					playerActor.RightAction = false;
					playerActor.JumpAction = false;
					playerActor.CrouchAction = false;
					// Disabling controls.

					// Adding actors.
					_actors.Add("foxe", playerPosition);
					_actors.Add("gato", _gato.GetComponent<PositionComponent>());
					// Adding actors.

					_dialogueStarted = true;

					_delayAlarm.Set(_delay);
				}
			}
			else
			{
				_delayAlarm.Update();
				if (
					(_currentBubble == null || _currentBubble.Destroyed)
					&& _dialogueLines.Length > _currentLine
					&& !_delayAlarm.Running
				)
				{
					var line = _dialogueLines[_currentLine];
					_currentLine += 1;

					foreach (var actorPair in _actors)
					{
						if (line.StartsWith(actorPair.Key))
						{
							line = line.Substring(actorPair.Key.Length + 1).Replace("/", Environment.NewLine);
							_currentBubble = new SpeechBubble(actorPair.Value, line, actorPair.Value.Owner.Layer);

							break;
						}
					}
				}

				if (_currentBubble != null)
				{
					_currentBubble.Owner.Owner.GetComponent<StackableActorComponent>().Talking = _currentBubble.Typing;
				}
			}

		}


		public override void Draw()
		{
			var position = GetComponent<PositionComponent>();

			Resources.Sprites.Default.Booth.Draw(
				position.Position,
				Resources.Sprites.Default.Booth.Origin
			);

			//CircleShape.Draw(position.Position, _dialogueTriggerR, true);
		}

	}
}
