using System.Collections.Generic;
using ChaiFoxes.FMODAudio;
using System;
using Monofoxe.Engine.Utils;

namespace Monofoxe.Demo.GameLogic.Audio
{
	public class LayeredSound
	{
		private Dictionary<string, Sound> _layers;
		
		private Dictionary<string, SoundChannel> _channels;
		
		public float TransitionSpeed = 1;
		
		private Dictionary<string, Transition> _volumeTransitions;
		
		public FMOD.ChannelGroup ChannelGroup;

		private class Transition
		{
			public float Speed;
			public float TargetValue;
			public float CurrentValue;

			public bool Complete;


			public Transition(float speed, float targetValue, float currentValue)
			{
				Speed = speed;
				TargetValue = targetValue;
				CurrentValue = currentValue;
				Complete = false;
			}

			public void Update()
			{
				var valueDelta = CurrentValue - TargetValue;
				var speedDelta = TimeKeeper.GlobalTime(Speed);

				if (Math.Abs(valueDelta) > speedDelta)
				{
					CurrentValue -= speedDelta * Math.Sign(valueDelta);
				}
				else
				{
					CurrentValue = TargetValue;
					Complete = true;
				}
			}
		}

		public LayeredSound(FMOD.ChannelGroup group)
		{
			_layers = new Dictionary<string, Sound>();
			_channels = new Dictionary<string, SoundChannel>();
			
			_volumeTransitions = new Dictionary<string, Transition>();

			ChannelGroup = group;
		}

		public void Update()
		{
			var completedTransitions = new List<string>();
			foreach(var transition in _volumeTransitions)
			{
				transition.Value.Update();
				if (transition.Value.Complete)
				{
					completedTransitions.Add(transition.Key);
				}

				_channels[transition.Key].Volume = transition.Value.CurrentValue;
			}

			foreach(var transition in completedTransitions)
			{
				_volumeTransitions.Remove(transition);
			}
		}

		public void AddVolumeTransition(string name, float targetValue, float speed)
		{
			if (_volumeTransitions.ContainsKey(name))
			{
				_volumeTransitions.Remove(name);
			}
			_volumeTransitions.Add(name, new Transition(speed, targetValue, _channels[name].Volume));
		}


		public void AddLayer(string name, Sound sound) =>
			_layers.Add(name, sound);
		
		
		public void Play(bool paused = false)
		{
			foreach(var layer in _layers)
			{
				_channels.Add(layer.Key, layer.Value.Play(ChannelGroup, paused));
			}
		}

		public void Stop()
		{
			foreach(var channel in _channels)
			{
				channel.Value.Stop();
			}
			_channels.Clear();
		}


		public void Pause()
		{
			foreach(var channel in _channels)
			{
				channel.Value.Pause();
			}
		}

		public void Resume()
		{
			foreach(var channel in _channels)
			{
				channel.Value.Resume();
			}
		}


		public void SetLooping(bool looping)
		{
			foreach(var layer in _layers)
			{
				layer.Value.Looping = looping;
			}
		}


		public SoundChannel GetChannelLayer(string name) =>
			_channels[name];

		public Sound GetSoundLayer(string name) =>
			_layers[name];
		


	}
}
