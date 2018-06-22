using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MB_Engine
{
    public class SoundManager : Singleton<SoundManager>
    {
        #region fields
        public AudioClip[] Clips;
        public int PoolSize = 1;
        public Queue<AudioSource> pool;
        public int FreePoolCount;
        private Dictionary<string, AudioClip> clipsDic;
        private bool inited;
        #endregion

        protected SoundManager() { }

        public static void Play(string name)
        {
            main.play(name);
        }

        public static void Play(string name, float pitch, float volume)
        {
            main.play(name, pitch, volume);
        }

        public static void Play(string name, Vector3 pos, float pitch, float volume, float delay)
        {
            main.play(name, pos, pitch, volume, delay);
        }

        public static void Play(string name, Transform emitter, float pitch, float volume, float delay = 0f)
        {
            main.play(name, emitter, pitch, volume, delay);
        }

        #region private

        private void Start()
        {
            StartCoroutine(InitRoutine());
        }

        private void play(string name)
        {
            play(name, transform, 1, 1f);
        }

        private void play(string name, float pitch, float volume)
        {
            play(name, transform, pitch, volume);
        }

        private void play(string name, Transform emitter, float pitch = 1f, float volume = 1f, float delay = 0f)
        {
            AudioClip clip = null;
            if (clipsDic.TryGetValue(name, out clip))
            {
                play(clipsDic[name], emitter, pitch, volume, delay);
            }
        }

        private void play(string name, Vector3 pos, float pitch = 1f, float volume = 1f, float delay = 0f)
        {
            AudioClip clip = null;
            if (clipsDic.TryGetValue(name, out clip))
            {
                StartCoroutine(PlayRoutine(clip, pos, pitch, volume, delay));
            }
        }

        private void play(AudioClip clip, Transform emitter, float pitch, float volume, float delay)
        {
            StartCoroutine(PlayRoutine(clip, emitter.position, pitch, volume, delay));
        }

        #endregion

        private IEnumerator PlayRoutine(AudioClip clip, Vector3 pos, float pitch, float volume, float delay)
        {
            if (inited && pool.Count > 0)
            {
                AudioSource source = pool.Dequeue();
                source.clip = clip;
                source.volume = volume;
                source.pitch = pitch;
                source.PlayDelayed(delay);
                FreePoolCount = pool.Count;
                yield return new WaitForSeconds((clip.length + delay) * 2f);
                pool.Enqueue(source);
                FreePoolCount = pool.Count;
            }
        }

        private IEnumerator InitRoutine()
        {
            inited = false;
            if (clipsDic == null)
            {
                clipsDic = new Dictionary<string, AudioClip>();
                foreach (AudioClip clip in Clips)
                {
                    clipsDic.Add(clip.name, clip);
                    yield return 0;
                }
            }
            if (pool == null)
            {
                pool = new Queue<AudioSource>();
                for (int i = 0; i < PoolSize; i++)
                {
                    AudioSource source = transform.gameObject.AddComponent<AudioSource>();
                    source.playOnAwake = false;
                    pool.Enqueue(source);
                    yield return 0;
                }
            }
            inited = true;
        }
    }
}