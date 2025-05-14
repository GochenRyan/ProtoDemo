using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using ZeroPass;
using ZeroPass.Serialization;

public class AudioManager : RMonoBehaviour
{
    private static AudioManager _instance;

    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var gameObject = new GameObject(nameof(AudioManager));
                DontDestroyOnLoad(gameObject);
                _instance = gameObject.AddComponent<AudioManager>();
            }

            return _instance;
        }
    }

    public static void DestroyInstance()
    {
        _instance.DeleteObject();
        _instance = null;
    }

    private AudioMixer _audioMixer;
    private AudioSource _musicSource;
    private int _initialPoolSize = 8;

    private Dictionary<AudioChannel, AudioMixerGroup> _channelGroups = new Dictionary<AudioChannel, AudioMixerGroup>();
    private List<AudioSource> _sfxPool = new List<AudioSource>();

    private Dictionary<AudioChannel, HashSet<AudioSource>> _activeSourcesByChannel =
    new Dictionary<AudioChannel, HashSet<AudioSource>>();

    private Coroutine _activeFadeCoroutine;

    protected override void OnPrefabInit()
    {
        LoadResources();
        InitializeChannelGroups();
        InitializeObjectPool();
    }

    private void LoadResources()
    {
        _audioMixer = Resources.Load<AudioMixer>("Audio/MainMixer");

        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.playOnAwake = false;
        _musicSource.loop = true;

        _initialPoolSize = 8;
    }

    private void InitializeChannelGroups()
    {
        foreach (AudioChannel channel in System.Enum.GetValues(typeof(AudioChannel)))
        {
            string groupPath = GetGroupPath(channel);
            AudioMixerGroup group = _audioMixer.FindMatchingGroups(groupPath)[0];
            _channelGroups.Add(channel, group);
        }
    }

    private string GetGroupPath(AudioChannel channel)
    {
        return channel switch
        {
            AudioChannel.Master => "Master",
            AudioChannel.Music => "Master/Music",
            AudioChannel.SFX_UI => "Master/SFX/UI",
            AudioChannel.SFX_Environment => "Master/SFX/Environment",
            AudioChannel.SFX_Character => "Master/SFX/Character",
            AudioChannel.SFX_Weapons => "Master/SFX/Weapons",
            AudioChannel.Voice => "Master/Voice",
            _ => "Master"
        };
    }

    private void InitializeObjectPool()
    {
        for (int i = 0; i < _initialPoolSize; i++)
        {
            CreateNewAudioSource();
        }
    }

    private AudioSource CreateNewAudioSource()
    {
        GameObject sourceObj = new GameObject($"SFX_Source_{_sfxPool.Count}");
        sourceObj.transform.SetParent(transform);
        AudioSource source = sourceObj.AddComponent<AudioSource>();
        _sfxPool.Add(source);
        return source;
    }

    public void PlaySFX(AudioClip clip, 
                        AudioChannel channel,
                        bool loop = false,
                        SpatialSettings settings = null,
                        Vector2? position = null)
    {
        AudioSource source = GetAvailableSource();
        source.loop = loop;
        ConfigureSource(source, clip, channel, settings, position);
        source.Play();
    }

    private AudioSource GetAvailableSource()
    {
        foreach (AudioSource source in _sfxPool)
        {
            if (!source.isPlaying) return source;
        }
        return CreateNewAudioSource();
    }

    private void ConfigureSource(AudioSource source, AudioClip clip,
                               AudioChannel channel,
                               SpatialSettings settings,
                               Vector2? position)
    {
#if UNITY_IOS || UNITY_ANDROID
        source.spatialBlend = 0;
#else
        source.clip = clip;
        source.outputAudioMixerGroup = _channelGroups[channel];

        if (position.HasValue)
        {
            source.transform.position = position.Value;
            ApplySpatialSettings(source, settings ?? new SpatialSettings());
        }
        else
        {
            source.spatialBlend = 0;
        }

        if (!_activeSourcesByChannel.ContainsKey(channel))
        {
            _activeSourcesByChannel.Add(channel, new HashSet<AudioSource>());
        }
        _activeSourcesByChannel[channel].Add(source);
#endif
    }

    private void ApplySpatialSettings(AudioSource source, SpatialSettings settings)
    {
        source.spatialBlend = settings.spatialBlend;
        source.minDistance = settings.minDistance;
        source.maxDistance = settings.maxDistance;
        source.spread = settings.spread;
    }

    public void PlayMusic(AudioClip musicClip, float fadeDuration = 1f)
    {
        if (_activeFadeCoroutine != null)
        {
            StopCoroutine(_activeFadeCoroutine);
        }
        _activeFadeCoroutine = StartCoroutine(FadeMusic(musicClip, fadeDuration));
    }

    private IEnumerator FadeMusic(AudioClip newClip, float fadeDuration)
    {
        float currentVolume;
        _audioMixer.GetFloat("Music_Volume", out currentVolume);

        while (currentVolume > -80f)
        {
            currentVolume -= Time.deltaTime * 80 / fadeDuration;
            _audioMixer.SetFloat("Music_Volume", currentVolume);
            yield return null;
        }

        _musicSource.clip = newClip;
        _musicSource.Play();

        while (currentVolume < 0)
        {
            currentVolume += Time.deltaTime * 80 / fadeDuration;
            _audioMixer.SetFloat("Music_Volume", currentVolume);
            yield return null;
        }
    }

    public void SetChannelVolume(AudioChannel channel, float volume)
    {
        string paramName = channel + "_Volume";
        _audioMixer.SetFloat(paramName, Mathf.Log10(volume) * 20);
    }

    private IEnumerator TrackAudioPlayback(AudioSource source, AudioChannel channel)
    {
        yield return new WaitWhile(() => source.isPlaying);
        _activeSourcesByChannel[channel].Remove(source);
    }

    public void StopChannel(AudioChannel channel)
    {
        if (_activeSourcesByChannel.TryGetValue(channel, out var sources))
        {
            foreach (var source in sources)
            {
                if (source.isPlaying)
                {
                    source.Stop();
                }
            }
            sources.Clear();
        }
    }
}
