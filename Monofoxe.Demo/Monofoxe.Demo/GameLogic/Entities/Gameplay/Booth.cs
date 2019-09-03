using ChaiFoxes.FMODAudio;
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
			"gato: hello there, good sir!",
			"foxe: sup",
			"gato: you've reached the rightmost/point of this world!",
			"foxe: but what about all that stuff/further to the right?",
			"gato: ha ha ha!",
			"gato: questions, questions...",
			"gato: anyways, you can't go any further",
			"foxe: why, though?",
			"gato: try it",
			"foxe: ...",
			"foxe: ......",
			"foxe: !!!!!!!!!!",
			"foxe: damn, i can't",
			"gato: see? you'll have to wait until/the full game comes out",
			"foxe: that's a long wait",
			"foxe: i've heard it will be/completely different",
			"foxe: and most horrifying of all",
			"foxe: it won't feature stacking mechanics!",
			"gato: unbelievable!",
			"gato: i bet they'll replace it with/something stupid",
			"gato: like flying foxes",
			"foxe: or playing as a block of dirt",
			"gato: ha ha, you got a sense/of humor, good sir!",
			"foxe: so, what now?",
			"gato: just enjoy the moment while it lasts",
			"foxe: you know, i've been catching up on redwall books recently,/and at some point it just gets kinda frustrating." +
			"/the thing has so much potential, so much atmosphere - especially/the first couple of books - but the worldbuilding" +
			"/is just disappointing. The world doesn’t feel coherent./there are no villages, towns or countries - just an" +
			"/abbey, the mountain and a lot of bandits. the irony of it/is, the bandits seem to be the most organized out of" +
			"/everybody. and don't even get me started on the bad guys - it's/just racism through and through. if the character is a" +
			"/mouse, they are always the good guy, but if it is a rat - they/are guaranteed to be a bandit with no good traits. like, i" +
			"/get that these are supposed to be children's books and this makes it/easier for the young reader to understand, but it gets" +
			"/old after a couple of books. and there is a lot of them, you know."
		};

		Dictionary<string, PositionComponent> _actors = new Dictionary<string, PositionComponent>();

		Dictionary<string, Sound> _actorSounds = new Dictionary<string, Sound>();

		bool _dialogueStarted = false;

		int _currentLine = 0;

		SpeechBubble _currentBubble;

		Alarm _startDelayAlarm = new Alarm();
		double _startDelay = 1.5;

		Alarm _fadeDelayAlarm = new Alarm();
		double _fadeDelay = 3;


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

			_actorSounds.Add("gato", Resources.Sounds.CatSpeech);
			_actorSounds.Add("foxe", Resources.Sounds.FoxeSpeech);

		}

		public override void Update()
		{
			var playerEntity = Scene.FindEntityByComponent<PlayerComponent>();
			if (playerEntity == null)
			{
				return;
			}

			var playerPosition = playerEntity.GetComponent<PositionComponent>();
			var position = GetComponent<PositionComponent>();

			if (_fadeDelayAlarm.Update())
			{
				new LevelRestartEffect(GameplayController.GUILayer, 5, true);
			}



			if (!_dialogueStarted)
			{
				if ((playerPosition.Position - position.Position).Length() < _dialogueTriggerR)
				{
					GameplayController.PausingEnabled = false;

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

					_startDelayAlarm.Set(_startDelay);
				}
			}
			else
			{
				_startDelayAlarm.Update();
				if (
					(_currentBubble == null || _currentBubble.Destroyed)
					&& _dialogueLines.Length > _currentLine
					&& !_startDelayAlarm.Running
				)
				{
					var line = _dialogueLines[_currentLine];
					_currentLine += 1;

					if (_currentLine == _dialogueLines.Length)
					{
						_fadeDelayAlarm.Set(_fadeDelay);
					}

					foreach (var actorPair in _actors)
					{
						if (line.StartsWith(actorPair.Key))
						{
							line = line.Substring(actorPair.Key.Length + 1).Replace("/", Environment.NewLine);
							_currentBubble = new SpeechBubble(actorPair.Value, line, actorPair.Value.Owner.Layer);
							_currentBubble.SpeechSound = _actorSounds[actorPair.Key];
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
