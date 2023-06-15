﻿using System;
using System.Collections.Generic;

namespace MoonWorks.Audio
{
	internal class AudioTweenManager
	{
		private AudioTweenPool AudioTweenPool = new AudioTweenPool();
		private readonly Dictionary<(SoundInstance, AudioTweenProperty), AudioTween> AudioTweens = new Dictionary<(SoundInstance, AudioTweenProperty), AudioTween>();
		private readonly List<AudioTween> DelayedAudioTweens = new List<AudioTween>();

		public void Update(float elapsedSeconds)
		{
			for (var i = DelayedAudioTweens.Count - 1; i >= 0; i--)
			{
				var audioTween = DelayedAudioTweens[i];
				var soundInstance = audioTween.SoundInstance;

				audioTween.Time += elapsedSeconds;

				if (audioTween.Time >= audioTween.DelayTime)
				{
					// set the tween start value to the current value of the property
					switch (audioTween.Property)
					{
						case AudioTweenProperty.Pan:
							audioTween.StartValue = soundInstance.Pan;
							break;

						case AudioTweenProperty.Pitch:
							audioTween.StartValue = soundInstance.Pitch;
							break;

						case AudioTweenProperty.Volume:
							audioTween.StartValue = soundInstance.Volume;
							break;

						case AudioTweenProperty.FilterFrequency:
							audioTween.StartValue = soundInstance.FilterFrequency;
							break;

						case AudioTweenProperty.Reverb:
							audioTween.StartValue = soundInstance.Reverb;
							break;
					}

					audioTween.Time = 0;
					DelayedAudioTweens.RemoveAt(i);

					AddTween(audioTween);
				}
			}

			foreach (var (key, audioTween) in AudioTweens)
			{
				bool finished = UpdateAudioTween(audioTween, elapsedSeconds);

				if (finished)
				{
					AudioTweenPool.Free(audioTween);
					AudioTweens.Remove(key);
				}
			}
		}

		public void CreateTween(
			SoundInstance soundInstance,
			AudioTweenProperty property,
			System.Func<float, float> easingFunction,
			float start,
			float end,
			float duration,
			float delayTime
		) {
			var tween = AudioTweenPool.Obtain();
			tween.SoundInstance = soundInstance;
			tween.Property = property;
			tween.EasingFunction = easingFunction;
			tween.StartValue = start;
			tween.EndValue = end;
			tween.Duration = duration;
			tween.Time = 0;
			tween.DelayTime = delayTime;

			if (delayTime == 0)
			{
				AddTween(tween);
			}
			else
			{
				DelayedAudioTweens.Add(tween);
			}
		}

		public void ClearTweens(SoundInstance soundInstance, AudioTweenProperty property)
		{
			AudioTweens.Remove((soundInstance, property));
		}

		private void AddTween(
			AudioTween audioTween
		) {
			// if a tween with the same sound and property already exists, get rid of it
			if (AudioTweens.TryGetValue((audioTween.SoundInstance, audioTween.Property), out var currentTween))
			{
				AudioTweenPool.Free(currentTween);
			}

			AudioTweens[(audioTween.SoundInstance, audioTween.Property)] = audioTween;
		}

		private static bool UpdateAudioTween(AudioTween audioTween, float delta)
		{
			float value;
			audioTween.Time += delta;

			var finished = audioTween.Time >= audioTween.Duration;
			if (finished)
			{
				value = audioTween.EndValue;
			}
			else
			{
				value = MoonWorks.Math.Easing.Interp(
					audioTween.StartValue,
					audioTween.EndValue,
					audioTween.Time,
					audioTween.Duration,
					audioTween.EasingFunction
				);
			}

			switch (audioTween.Property)
			{
				case AudioTweenProperty.Pan:
					audioTween.SoundInstance.Pan = value;
					break;

				case AudioTweenProperty.Pitch:
					audioTween.SoundInstance.Pitch = value;
					break;

				case AudioTweenProperty.Volume:
					audioTween.SoundInstance.Volume = value;
					break;

				case AudioTweenProperty.FilterFrequency:
					audioTween.SoundInstance.FilterFrequency = value;
					break;

				case AudioTweenProperty.Reverb:
					audioTween.SoundInstance.Reverb = value;
					break;
			}

			return finished;
		}
	}
}
