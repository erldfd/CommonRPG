using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
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

                foreach (AudioSource source in bgmAudioQueue)
                {
                    float originalVolume = bgmAudioOriginalVolumeQueue.Dequeue();
                    source.volume = masterVolume * bgmAudioVolume * originalVolume;
                    bgmAudioOriginalVolumeQueue.Enqueue(originalVolume);
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
        private float bgmAudioVolume = 1;
        public float BGMAudioVolume
        {
            get
            {
                return bgmAudioVolume;
            }
            set
            {
                bgmAudioVolume = Mathf.Clamp01(value);

                foreach (AudioSource source in bgmAudioQueue)
                {
                    float originalVolume = bgmAudioOriginalVolumeQueue.Dequeue();
                    source.volume = MasterVolume * bgmAudioVolume * originalVolume;
                    bgmAudioOriginalVolumeQueue.Enqueue(originalVolume);
                }
            }
        }

        [SerializeField]
        private int audio2DCount = 8;

        [SerializeField]
        private int audio3DCount = 8;

        [SerializeField]
        private float bgmFadeOutTime = 2;

        private float elapsedTime_FadeOut = float.MaxValue;

        private GameObject audioCollector = null;

        private Queue<AudioSource> audio2DQueue = new Queue<AudioSource>();
        private Queue<float> audio2DOriginalVolumeQueue = new Queue<float>();

        private Queue<AudioSource> audio3DQueue = new Queue<AudioSource>();
        private Queue<float> audio3DOriginalVolumeQueue = new Queue<float>();

        private Queue<AudioSource> bgmAudioQueue = new Queue<AudioSource>();
        private Queue<float> bgmAudioOriginalVolumeQueue = new Queue<float>();

        private AudioSource nextBGM = null;
        private float currentBGMVolume = 1;

        private void Awake()
        {
            InitAudioManager();
        }

        private void Update()
        {
            if (bgmFadeOutTime > elapsedTime_FadeOut) 
            {
                elapsedTime_FadeOut += Time.unscaledDeltaTime;

                float lerpTime = elapsedTime_FadeOut / bgmFadeOutTime;

                AudioSource currentBGM = bgmAudioQueue.Peek();
                currentBGM.volume = Mathf.Lerp(currentBGMVolume, 0, lerpTime);

                if (bgmFadeOutTime <= elapsedTime_FadeOut) 
                {
                    currentBGM.Stop();

                    currentBGM = nextBGM;
                    currentBGM.Play();

                    bgmAudioQueue.Enqueue(currentBGM);
                    currentBGMVolume = currentBGM.volume;

                    nextBGM = bgmAudioQueue.Dequeue();
                }
            }
        }

        public void PlayAudio2D(AudioClip audioClip, float volume, float pitch = 1)
        {
            Debug.Assert(audio2DQueue.Count > 0);
            Debug.Assert(audio2DOriginalVolumeQueue.Count > 0);
            Debug.Assert(audio2DQueue.Count == audio2DOriginalVolumeQueue.Count);

            AudioSource source = audio2DQueue.Dequeue();
            audio2DOriginalVolumeQueue.Dequeue();

            source.volume = MasterVolume * Audio2DVolume * volume;
            source.clip = audioClip;
            source.pitch = Mathf.Clamp(pitch, -3, 3);
            source.Play();

            audio2DQueue.Enqueue(source);
            audio2DOriginalVolumeQueue.Enqueue(volume);
        }

        public void PlayBGM(AudioClip audioClip, float volume, bool shouldLoop = false, float pitch = 1)
        {
            Debug.Assert(bgmAudioQueue.Count > 0);
            Debug.Assert(bgmAudioOriginalVolumeQueue.Count > 0);
            Debug.Assert(bgmAudioQueue.Count == bgmAudioOriginalVolumeQueue.Count);

            AudioSource currentBGM = bgmAudioQueue.Peek();

            nextBGM.clip = audioClip;

            nextBGM.volume = MasterVolume * BGMAudioVolume * volume;

            nextBGM.pitch = Mathf.Clamp(pitch, -3, 3);
            nextBGM.loop = shouldLoop;

            if (currentBGM.isPlaying) 
            {
                // ready to bgm fade out and play next bgm
                if (bgmFadeOutTime <= elapsedTime_FadeOut) 
                {
                    elapsedTime_FadeOut = 0;
                }
            }
            else
            {
                currentBGM = nextBGM;
                currentBGM.Play();

                bgmAudioQueue.Enqueue(currentBGM);
                currentBGMVolume = currentBGM.volume;

                nextBGM = bgmAudioQueue.Dequeue();
            }
        }

        public void PlayAudio3D(AudioClip audioClip, float volume, Vector2 position, float pitch = 1)
        {
            Debug.Assert(audio3DQueue.Count > 0);
            Debug.Assert(audio3DOriginalVolumeQueue.Count > 0);
            Debug.Assert(audio3DQueue.Count == audio3DOriginalVolumeQueue.Count);

            AudioSource source = audio3DQueue.Dequeue();
            audio3DOriginalVolumeQueue.Dequeue();

            source.volume = MasterVolume * Audio3DVolume * volume;
            source.clip = audioClip;
            source.transform.position = position;
            source.pitch = Mathf.Clamp(pitch, -3, 3);
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

            foreach (AudioSource source in bgmAudioQueue)
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
            bgmAudioQueue.Clear();
            audio3DQueue.Clear();

            audio2DOriginalVolumeQueue.Clear();
            bgmAudioOriginalVolumeQueue.Clear();
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

            {
                AudioSource source = new GameObject("BGMAudio1").AddComponent<AudioSource>();
                source.transform.parent = transform;

                source.playOnAwake = false;
                source.Stop();

                source.transform.SetParent(audioCollector.transform);
                bgmAudioQueue.Enqueue(source);
                bgmAudioOriginalVolumeQueue.Enqueue(new float());

                nextBGM  = new GameObject("BGMAudio2").AddComponent<AudioSource>();
                nextBGM.transform.parent = transform;

                nextBGM.playOnAwake = false;
                nextBGM.Stop();

                nextBGM.transform.SetParent(audioCollector.transform);
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
