using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public class AudioManager : MonoBehaviour
    {
        [Range(0f, 1f)]
        [SerializeField]
        private float masterVolume = 1;
        public float MasterVolume
        {
            get
            {
                return masterVolume;
            }
            set
            {
                masterVolume = Mathf.Clamp01(value);

                foreach (AudioSource source in audio2DQueue)
                {
                    float originalVolume = audio2DOriginalVolumeQueue.Dequeue();
                    source.volume = masterVolume * audio2DVolume * originalVolume;
                    audio2DOriginalVolumeQueue.Enqueue(originalVolume);
                }

                foreach (AudioSource source in audio3DQueue)
                {
                    float originalVolume = audio3DOriginalVolumeQueue.Dequeue();
                    source.volume = masterVolume * audio3DVolume * originalVolume;
                    audio3DOriginalVolumeQueue.Enqueue(originalVolume);
                }

                foreach (AudioSource source in longAudio2DQueue)
                {
                    float originalVolume = longAudio2DOriginalVolumeQueue.Dequeue();
                    source.volume = masterVolume * longAudio2DVolume * originalVolume;
                    longAudio2DOriginalVolumeQueue.Enqueue(originalVolume);
                }
            }
        }

        [Range(0f, 1f)]
        [SerializeField]
        private float audio2DVolume = 1;
        public float Audio2DVolume
        {
            get
            {
                return audio2DVolume;
            }
            set
            {
                audio2DVolume = Mathf.Clamp01(value);

                foreach (AudioSource source in audio2DQueue)
                {
                    float originalVolume = audio2DOriginalVolumeQueue.Dequeue();
                    source.volume = MasterVolume * audio2DVolume * originalVolume;
                    audio2DOriginalVolumeQueue.Enqueue(originalVolume);
                }
            }
        }

        [Range(0f, 1f)]
        [SerializeField]
        private float audio3DVolume = 1;
        public float Audio3DVolume
        {
            get
            {
                return audio3DVolume;
            }
            set
            {
                audio3DVolume = Mathf.Clamp01(value);

                foreach (AudioSource source in audio3DQueue)
                {
                    float originalVolume = audio3DOriginalVolumeQueue.Dequeue();
                    source.volume = MasterVolume * audio3DVolume * originalVolume;
                    audio3DOriginalVolumeQueue.Enqueue(originalVolume);
                }
            }
        }

        [Range(0f, 1f)]
        [SerializeField]
        private float longAudio2DVolume = 1;
        public float LongAudio2DVolume
        {
            get
            {
                return longAudio2DVolume;
            }
            set
            {
                longAudio2DVolume = Mathf.Clamp01(value);

                foreach (AudioSource source in longAudio2DQueue)
                {
                    float originalVolume = longAudio2DOriginalVolumeQueue.Dequeue();
                    source.volume = MasterVolume * longAudio2DVolume * originalVolume;
                    longAudio2DOriginalVolumeQueue.Enqueue(originalVolume);
                }
            }
        }

        [SerializeField]
        private int audio2DCount = 8;

        [SerializeField]
        private int audio3DCount = 8;

        [SerializeField]
        private int longAudio2DCount = 2;

        private GameObject audioCollector = null;

        private Queue<AudioSource> audio2DQueue = new Queue<AudioSource>();
        private Queue<float> audio2DOriginalVolumeQueue = new Queue<float>();
        //private Queue<AudioSource> deactivatedAudio2DQueue = new Queue<AudioSource>();

        private Queue<AudioSource> audio3DQueue = new Queue<AudioSource>();
        private Queue<float> audio3DOriginalVolumeQueue = new Queue<float>();
        //private Queue<AudioSource> deactivatedAudio3DQueue = new Queue<AudioSource>();

        private Queue<AudioSource> longAudio2DQueue = new Queue<AudioSource>();
        private Queue<float> longAudio2DOriginalVolumeQueue = new Queue<float>();
        //private Queue<AudioSource> deactivatedLongAudio2DQueue = new Queue<AudioSource>();

        private void Awake()
        {
            InitAudioManager();
        }

        public void PlayAudio2D(AudioClip audioClip, float volume)
        {
            Debug.Assert(audio2DQueue.Count > 0);
            Debug.Assert(audio2DOriginalVolumeQueue.Count > 0);
            Debug.Assert(audio2DQueue.Count == audio2DOriginalVolumeQueue.Count);

            AudioSource source = audio2DQueue.Dequeue();
            audio2DOriginalVolumeQueue.Dequeue();

            source.volume = MasterVolume * Audio2DVolume * volume;
            source.clip = audioClip;
            source.Play();

            audio2DQueue.Enqueue(source);
            audio2DOriginalVolumeQueue.Enqueue(volume);
        }

        public void PlayLongAudio2D(AudioClip audioClip, float volume, bool shouldLoop = false)
        {
            Debug.Assert(longAudio2DQueue.Count > 0);
            Debug.Assert(longAudio2DOriginalVolumeQueue.Count > 0);
            Debug.Assert(longAudio2DQueue.Count == longAudio2DOriginalVolumeQueue.Count);

            AudioSource source = longAudio2DQueue.Dequeue();
            longAudio2DOriginalVolumeQueue.Dequeue();

            source.volume = MasterVolume * LongAudio2DVolume * volume;
            source.clip = audioClip;
            source.Play();
            source.loop = shouldLoop;

            longAudio2DQueue.Enqueue(source);
            longAudio2DOriginalVolumeQueue.Enqueue(volume);
        }

        public void PlayAudio3D(AudioClip audioClip, float volume, Vector2 position)
        {
            Debug.Assert(audio3DQueue.Count > 0);
            Debug.Assert(audio3DOriginalVolumeQueue.Count > 0);
            Debug.Assert(audio3DQueue.Count == audio3DOriginalVolumeQueue.Count);

            AudioSource source = audio3DQueue.Dequeue();
            audio3DOriginalVolumeQueue.Dequeue();

            source.volume = MasterVolume * Audio3DVolume * volume;
            source.clip = audioClip;
            source.transform.position = position;
            source.Play();

            audio3DQueue.Enqueue(source);
            audio3DOriginalVolumeQueue.Enqueue(volume);
        }

        public void StopAllAudios()
        {
            foreach (AudioSource source in audio2DQueue)
            {
                if (source.isPlaying)
                {
                    source.Stop();
                }
            }

            StopAllAudio3Ds();

            foreach (AudioSource source in longAudio2DQueue)
            {
                if (source.isPlaying)
                {
                    source.Stop();
                }
            }
        }

        public void StopAllAudio3Ds()
        {
            foreach (AudioSource source in audio3DQueue)
            {
                if (source.isPlaying)
                {
                    source.Stop();
                }
            }
        }

        private void InitAudioManager()
        {
            audio2DQueue.Clear();
            longAudio2DQueue.Clear();
            audio3DQueue.Clear();

            audio2DOriginalVolumeQueue.Clear();
            longAudio2DOriginalVolumeQueue.Clear();
            audio3DOriginalVolumeQueue.Clear();

            if (audioCollector == null)
            {
                audioCollector = new GameObject("Audio Collector");
                audioCollector.transform.SetParent(transform);
            }

            for (int i = 0; i < audio2DCount; ++i)
            {

                AudioSource source = new GameObject("audio2D").AddComponent<AudioSource>();
                source.transform.parent = transform;

                source.playOnAwake = false;

                source.transform.SetParent(audioCollector.transform);
                audio2DQueue.Enqueue(source);
                audio2DOriginalVolumeQueue.Enqueue(new float());
            }

            for (int i = 0; i < longAudio2DCount; ++i)
            {
                AudioSource source = new GameObject("longAudio2D").AddComponent<AudioSource>();
                source.transform.parent = transform;

                source.playOnAwake = false;

                source.transform.SetParent(audioCollector.transform);
                longAudio2DQueue.Enqueue(source);
                longAudio2DOriginalVolumeQueue.Enqueue(new float());
            }

            for (int i = 0; i < audio3DCount; ++i)
            {
                AudioSource source = new GameObject("audio3D").AddComponent<AudioSource>();
                source.transform.parent = transform;

                source.playOnAwake = false;
                source.spread = 360;

                source.transform.SetParent(audioCollector.transform);
                audio3DQueue.Enqueue(source);
                audio3DOriginalVolumeQueue.Enqueue(new float());
            }
        }
    }
}
