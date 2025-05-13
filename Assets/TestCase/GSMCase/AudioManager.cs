using Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using ZeroPass;
using ZeroPass.Serialization;

[SerializationConfig(MemberSerialization.OptIn)]
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
                gameObject.SetActive(false);
                _instance = gameObject.AddComponent<AudioManager>();
                RPrefabID rPrefabID = gameObject.AddOrGet<RPrefabID>();
                rPrefabID.PrefabTag = TagManager.Create(nameof(AudioManager), nameof(AudioManager));
                rPrefabID.UpdateSaveLoadTag();
                gameObject.AddComponent<SaveLoadRoot>();
                gameObject.SetActive(true);
            }

            return _instance;
        }
    }

    public static void DestroyInstance()
    {
        _instance.DeleteObject();
        _instance = null;
    }

    [Header("Mixer Configuration")]
    [SerializeField]
    private AudioMixer _audioMixer;
    [SerializeField]
    private AudioMixerGroup[] _mixerGroups;

    [Header("Audio Source")]
    [SerializeField]
    private AudioSource _musicSource;
    [SerializeField]
    private int _initialPoolSize = 8;

    private Dictionary<AudioChannel, AudioMixerGroup> _channelGroups = new Dictionary<AudioChannel, AudioMixerGroup>();
    private List<AudioSource> _sfxPool = new List<AudioSource>();

    protected override void OnPrefabInit()
    {
        
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

    public void PlaySFX(AudioClip clip, AudioChannel channel,
                      SpatialSettings settings = null,
                      Vector2? position = null)
    {
        AudioSource source = GetAvailableSource();
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
        StartCoroutine(FadeMusic(musicClip, fadeDuration));
    }

    private IEnumerator FadeMusic(AudioClip newClip, float fadeDuration)
    {
        float currentVolume;
        _audioMixer.GetFloat("MusicVolume", out currentVolume);

        while (currentVolume > -80f)
        {
            currentVolume -= Time.deltaTime * 80 / fadeDuration;
            _audioMixer.SetFloat("MusicVolume", currentVolume);
            yield return null;
        }

        _musicSource.clip = newClip;
        _musicSource.Play();

        while (currentVolume < 0)
        {
            currentVolume += Time.deltaTime * 80 / fadeDuration;
            _audioMixer.SetFloat("MusicVolume", currentVolume);
            yield return null;
        }
    }

    public void SetChannelVolume(AudioChannel channel, float volume)
    {
        string paramName = channel + "_Volume";
        _audioMixer.SetFloat(paramName, Mathf.Log10(volume) * 20);
    }

    public void StopChannel(AudioChannel channel)
    {
        if (!_channelGroups.TryGetValue(channel, out AudioMixerGroup targetGroup))
        {
            return;
        }

        foreach (AudioSource source in _sfxPool)
        {
            if (source.outputAudioMixerGroup == targetGroup && source.isPlaying)
            {
                source.Stop();
            }
        }
    }
}
