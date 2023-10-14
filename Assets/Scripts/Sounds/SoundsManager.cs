using Common.Extensions;
using Common.Pooling;
using System;
using System.Collections;
using System.Collections.Generic;
using Common.Coroutines;
using UnityEngine;

namespace Game
{
    public class SoundsManager : MonoBehaviour
    {
        private class Sample
        {
            public AudioSource source;
            public Sound data;
            public Action onFinish;
            public float muter;
        }

        [SerializeField]
        private ComponentPool<AudioSource> _sources = new ();
        [Range(0.0f, 1.0f), SerializeField]
        private float _volume = 1.0f;

        private List<Sample> _playing = new ();
        private bool _muted = false;

        public float Volume
        {
            get => _volume;
            set { _volume = value; ApplyVolume(); }
        }

        public AudioSource PlaySound(Sound data, Action onFinish = null)
        {
            var source = _sources.Borrow();
            
            source.clip = data.clip;
            source.volume = data.volume * _volume;
            source.loop = data.loop;
            source.mute = _muted;
            source.Play();

            var sample = new Sample { source = source, data = data, onFinish = onFinish };
            _playing.Add(sample);

            return source;
        }

        public void StopSound(Sound data)
        {
            if (_playing.TryFindIndex(s => Equals(s.data, data), out int i))
            {
                var source = _playing[i].source;
                
                if (source.isPlaying)
                {
                    source.Stop();

                    _playing.RemoveAt(i);

                    _sources.Return(source);
                }
            }
        }

        public void StopSound(AudioSource source)
        {
            if (source.isPlaying && _playing.TryFindIndex(s => Equals(s.source, source), out int i))
            {
                source.Stop();

                _playing.RemoveAt(i);

                _sources.Return(source);
            }
        }

        public void SetVolume(float value)
        {
            foreach (var sample in _playing)
            {
                sample.source.volume = sample.data.volume * value;
            }
        }

        public IEnumerator FadeVolume(float duration, float target)
        {
            var timer = UCoroutine.YieldTimeNormalized(duration);
            
            var current = _volume;
            while (timer.MoveNext())
            {
                var time = timer.Current;
                current = target + (_volume - target) * (1.0f - time);
                SetVolume(current);
                
                yield return null;
            }
            _volume = current;
        }

        public void SetMuted(bool muted)
        {
            foreach (var sample in _playing)
            {
                sample.source.mute = muted;
            }
        }

        public void MuteIncoming()
        {
            _muted = true;
        }

        public void UnmuteIncoming()
        {
            _muted = false;

            SetMuted(_muted);
        }

        public void Clear()
        {
            foreach (var sample in _playing)
            {
                var source = sample.source;

                if (source != null)
                {
                    source.Stop();

                    _sources.Return(source);
                }
            }
            _playing.Clear();
        }

        private void ApplyVolume()
        {
            SetVolume(_volume);
        }

        private void UpdateSamples()
        {
            for (int i = 0; i < _playing.Count; ++i)
            {
                var sample = _playing[i];

                if (!sample.source.isPlaying)
                {
                    _playing.RemoveAt(i);
                    i -= 1;

                    _sources.Return(sample.source);

                    if (sample.onFinish != null)
                    {
                        sample.onFinish();
                    }
                }
            }
        }

        #region Unity methods
        
        private void Awake()
        {
            ApplyVolume();
        }

        private void Update()
        {
            UpdateSamples();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ApplyVolume();
        }
#endif

        private void OnDestroy()
        {
            Clear();
        }
        
        #endregion
    }
}