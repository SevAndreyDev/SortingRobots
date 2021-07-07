using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace EnglishKids.SortingRobots
{
    public enum Audio
    {
        None,
        Pick,
        CorrectSlot,
        WrongSlot,
        FactoryMoving,
        Green,
        Blue
    }
        
    public class AudioManager : MonoSingleton<AudioManager>
    {
        [System.Serializable]
        private class AudioTrack
        {
            public Audio kind;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume = 1f;
            public int priority;
        }

        //==================================================
        // Fields
        //==================================================

        [Space]
        [Range(0f, 1f)] [SerializeField] private float _musicVolume = 1f;
        [Range(0f, 1f)] [SerializeField] private float _soundVolume = 1f;
        [Range(0f, 1f)] [SerializeField] private float _speachVolume = 1f;

        [Header("Audio Sources Settings")]        
        [SerializeField] private GameObject _sourcePanel;
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _speachSource;
        [SerializeField] private int _maxSoundSourceCount;

        [Header("Music List")]
        [SerializeField] private AudioTrack[] _musics;

        [Header("Audio List")]
        [SerializeField] private AudioTrack[] _sounds;

        [Header("Speach List")]
        [SerializeField] private AudioTrack[] _speaches;

        private List<AudioSource> _soundSources;
        private AudioTrack _speachOrder;

        //==================================================
        // Methods
        //==================================================

        protected override void Init()
        {            
            base.Init();
                        
            _soundSources = new List<AudioSource>();

            for (int i = 0; i < _maxSoundSourceCount; i++)
            {
                AudioSource source = _sourcePanel.AddComponent<AudioSource>();
                _soundSources.Add(source);
            }

            StartCoroutine(PlayingMusicProcess());
            StartCoroutine(PlayingSpeachProcess());
        }

        private IEnumerator PlayingMusicProcess()
        {
            const int MIN_COUNT_FOR_CHECKING_LAST_TRACK = 2;
            bool checkLastTrack = _musics.Length >= MIN_COUNT_FOR_CHECKING_LAST_TRACK;      // Don't repeat the same track in a row.

            List<AudioTrack> actualMusics = new List<AudioTrack>(_musics);
            AudioTrack track = null;
            AudioTrack lastTrack = null;

            while (true)
            {
                int index = Random.Range(0, actualMusics.Count);
                track = actualMusics[index];

                if (checkLastTrack)
                {
                    actualMusics.RemoveAt(index);

                    if (lastTrack != null)
                        actualMusics.Add(lastTrack);

                    lastTrack = track;
                }
                                
                _musicSource.clip = track.clip;
                _musicSource.volume = _musicVolume * track.volume;
                _musicSource.Play();

                yield return new WaitForSeconds(track.clip.length);
            }
        }

        public void PlaySound(Audio kind)
        {
            AudioTrack target = FindTrack(_sounds, kind);

            if (target == null)
            {
                Debug.LogError("Counldn't find sound");
                return;
            }

            AudioSource source = null;
            AudioTrack sourceTrack = null;

            // Find AudioSource with min priority track. If not playing - it's min.
            foreach (AudioSource item in _soundSources)
            {
                if (source == null)
                {
                    source = item;

                    if (source.isPlaying)
                    {
                        sourceTrack = FindTrack(_sounds, source.clip.name);
                        continue;
                    }
                    else
                        break;
                }

                if (!item.isPlaying)
                {
                    source = item;
                    break;
                }

                AudioTrack itemTrack = FindTrack(_sounds, item.clip.name);
                if (sourceTrack.priority > itemTrack.priority)
                {
                    source = item;
                    sourceTrack = itemTrack;
                }
            }

            // Play sound;
            if (source != null && (!source.isPlaying || sourceTrack.priority < target.priority))
            {
                source.Stop();
                source.clip = target.clip;
                source.volume = _soundVolume * target.volume;
                source.Play();
            }
        }

        public void PlaySpeach(Audio speachSound)
        {
            _speachOrder = FindTrack(_speaches, speachSound);
        }

        private IEnumerator PlayingSpeachProcess()
        {
            while (true)
            {
                if (_speachSource.isPlaying)
                {
                    yield return new WaitForEndOfFrame();                    
                }
                else
                {
                    if (_speachOrder != null)
                    {
                        _speachSource.clip = _speachOrder.clip;
                        _speachSource.volume = _speachVolume * _speachOrder.volume;
                        _speachSource.Play();

                        float duration = _speachOrder.clip.length;
                        _speachOrder = null;

                        yield return new WaitForSeconds(duration);
                    }
                    else
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }
            }
        }
                
        private AudioTrack FindTrack(AudioTrack[] list, Audio kind)
        {
            foreach (AudioTrack item in list)
            {
                if (item.kind == kind)
                    return item;
            }

            return null;
        }

        private AudioTrack FindTrack(AudioTrack[] list, string name)
        {
            foreach (AudioTrack item in list)
            {
                if (item.clip.name == name)
                    return item;
            }

            return null;
        }
    }
}