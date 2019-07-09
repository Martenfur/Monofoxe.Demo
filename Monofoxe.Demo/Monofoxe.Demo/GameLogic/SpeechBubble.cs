using Microsoft.Xna.Framework;
using Monofoxe.Demo.GameLogic.Entities.Core;
using Monofoxe.Engine;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils;
using System;
using System.Collections.Generic;
using ChaiFoxes.FMODAudio;
using System.Text.RegularExpressions;

namespace Monofoxe.Demo.GameLogic.Entities.Gameplay
{
	public class SpeechBubble : Entity
	{
		public string String = "test";
		public int TextPtr = 0;
		public readonly PositionComponent Owner;
		public const int Border = 16;

		float _textRubberBand = 8f / 60f;

		public Vector2 MainOffset = new Vector2(0, -32);
		public Vector2 BubbleOffset = new Vector2(0, -50);


		AutoAlarm _typeAlarm = new AutoAlarm(0.05);
		Alarm _delayAlarm = new Alarm();


		IFont _font = Resources.Fonts.CartonSix;

		Vector2 _textSize, _targetTextSize;

		int _cornerVecticesCount = 4;
		int _sideVecticesCount = 8;

		Vector2[] _wiggleys;
		double[] _wiggleysPhase;
		float _wiggleysSpd = 0.25f * 20;
		float _wiggleysR = 1;
		int _wiggleyId = 0;

		Vector2 _pos, _targetPos;
		float _posRubberBand = 10f / 60f;
		float _maxBubbleDist = 32;

		bool _dead = false;

		TriangleFanPrimitive _primitive;

		public bool Typing {get; private set;} = true;
		
		public Sound SpeechSound = Resources.Sounds.FoxeSpeech;

		public float BasePitch = 1;

		SoundChannel _channel;

		TimeKeeper _keeper = new TimeKeeper();

		public SpeechBubble(PositionComponent owner, string text, Layer layer) : base(layer)
		{
			Owner = owner;
			String = text;

			var r = new RandomExt();

			_wiggleys = new Vector2[(_cornerVecticesCount + _sideVecticesCount) * 4];
			_wiggleysPhase = new double[_wiggleys.Length];

			for (var i = 0; i < _wiggleysPhase.Length; i += 1)
			{
				_wiggleysPhase[i] = r.NextDouble(Math.PI * 2);
			}

			_pos = (Owner.Position + MainOffset).RoundV();

			_primitive = new TriangleFanPrimitive();
			_primitive.Vertices = new List<Vertex>();

			_typeAlarm.TimeKeeper = _keeper;
			_delayAlarm.TimeKeeper = _keeper;
		}

		
		public override void Update()
		{
			var speedUp = (GameButtons.Select.Check() && !GameplayController.PausingEnabled);

			if (speedUp)
			{
				_keeper.TimeMultiplier = 2.5;
			}
			else
			{
				_keeper.TimeMultiplier = 1;
			}


			if (_typeAlarm.Update())
			{
				
				TextPtr += 1;
				
				try
				{
					if (
						Regex.IsMatch(String[TextPtr].ToString(), "[a-zA-Z]") 
						&& !speedUp
					)
					{
						
						_channel = Audio.SoundController.PlaySound(SpeechSound, true);
						_channel.Pitch = SpeechSound.Pitch + (float)GameplayController.Random.NextDouble(-0.2, 0.2);
						var black = GameplayController.GUILayer.FindEntity<LevelRestartEffect>();
						if (black != null)
						{
							_channel.Volume = (float)Math.Min(1 - black.Fade, 1);
						}
						_channel.Resume();
					}
					

					if (String[TextPtr] == Environment.NewLine[0])
					{
						TextPtr += Environment.NewLine.Length;
					}

					if (TextPtr >= String.Length)
					{
						throw new Exception();
					}
				}
				catch (Exception)
				{
					_typeAlarm.Enabled = false;
					TextPtr = String.Length - 1;
					_delayAlarm.Set(1 + String.Length * 0.1);
					Typing = false;
				}
			}

			var str = "";
			if (!_dead)
			{
				str = String.Substring(0, TextPtr + 1);
			}

			_targetTextSize = _font.MeasureString(str);

			_textSize += TimeKeeper.GlobalTime(_targetTextSize - _textSize) / _textRubberBand;
			
			_targetPos = (Owner.Position + MainOffset).RoundV();

			_pos += TimeKeeper.GlobalTime(_targetPos - _pos) / _posRubberBand;
			
			if ((_targetPos - _pos).Length() > _maxBubbleDist)
			{
				var e = _targetPos - _pos;
				e.Normalize();
				_pos = _targetPos - e * _maxBubbleDist;
			}


			if (_delayAlarm.Update())
			{
				_dead = true;
				_textRubberBand = 2f / 60f;
			}

			if (_dead && _textSize.X < 4 && _textSize.Y < 4)
			{
				DestroyEntity();
			}

			var phaseAdd = TimeKeeper.GlobalTime(_wiggleysSpd);

			for (var i = 0; i < _wiggleys.Length; i += 1)
			{
				_wiggleysPhase[i] += phaseAdd;
				if (_wiggleysPhase[i] > Math.PI * 2)
				{
					_wiggleysPhase[i] -= Math.PI * 2;
				}

				_wiggleys[i] = new Vector2(
					(float)Math.Cos(_wiggleysPhase[i]),
					(float)Math.Sin(_wiggleysPhase[i])
				) * _wiggleysR;
			}

		}



		public override void Draw()
		{
			var str = "";
			if (!_dead)
			{
				str = String.Substring(0, TextPtr + 1);
			}

			var floatOffset = (new Vector2(
				(float)Math.Sin(GameMgr.ElapsedTimeTotal * 2.32), 
				(float)-Math.Cos(-GameMgr.ElapsedTimeTotal * 1.82)
			) * 4).RoundV();

			var offset = Text.CurrentFont.MeasureString(str) * Vector2.UnitY / 2 + floatOffset;
		 
			DrawSpeechBubble(_pos + BubbleOffset - offset, offset - floatOffset, _targetPos, _textSize);
			
			Text.CurrentFont = _font;
			Text.HorAlign = TextAlign.Center;
			Text.VerAlign = TextAlign.Center;
			GraphicsMgr.CurrentColor = new Color(37, 43, 45);

			Text.Draw(str, _pos + BubbleOffset - offset);

		}


		void DrawSpeechBubble(Vector2 center, Vector2 offset, Vector2 tarPos, Vector2 size)
		{
			GraphicsMgr.CurrentColor = new Color(230, 230, 255);

			_wiggleyId = 0;

			var flippedSize = new Vector2(size.X, -size.Y);

			TriangleShape.Draw(center - Vector2.UnitX * 8 + offset, center + Vector2.UnitX * 8 + offset, _targetPos, false);

			_primitive.Vertices.Clear();
			
			DrawArc(center - size / 2, 90);
			DrawLine(center - size / 2, Vector2.UnitX, size.X);

			DrawArc(center + flippedSize / 2, 0);
			DrawLine(center + flippedSize / 2, Vector2.UnitY, size.Y);

			DrawArc(center + size / 2, 270);
			DrawLine(center + size / 2, -Vector2.UnitX, size.X);

			DrawArc(center - flippedSize / 2, 180);
			DrawLine(center - flippedSize / 2, -Vector2.UnitY, size.Y);

			_primitive.Draw();

		}

		void DrawArc(Vector2 pos, float ang)
		{
			var stepAng = 90f / _cornerVecticesCount;
			for (var i = _cornerVecticesCount; i >= 0; i -= 1)
			{
				var dir = MathHelper.ToRadians(ang + i * stepAng);
				_primitive.Vertices.Add(new Vertex(pos + _wiggleys[i] + new Vector2((float)Math.Cos(dir), -(float)Math.Sin(dir)) * Border));
				_wiggleyId += 1;
			}
		}

		void DrawLine(Vector2 pos, Vector2 dir, float length)
		{
			var step = length / (_sideVecticesCount + 2);
			var borderOffset = new Vector2(dir.Y, -dir.X) * Border;
			for (var i = 0; i <= _sideVecticesCount; i += 1)
			{
				_primitive.Vertices.Add(new Vertex(pos + _wiggleys[i] + dir * step * (i + 1) + borderOffset));
				_wiggleyId += 1;
			}
		}


	}
}
