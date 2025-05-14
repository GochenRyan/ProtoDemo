using Serialization;
using System;
using System.IO;
using UnityEngine;
using ZeroPass;
using ZeroPass.StateMachine;

public class GSMTestRoot : RMonoBehaviour
{
    private GameObject _player;
    private GameObject _enemy_1;

    public static RuntimeAnimatorController GSMPlayerRAC;
    public static RuntimeAnimatorController GSMEnemy1RAC;

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip bgmMainMenu;
    [SerializeField]
    private AudioClip bgmGameplay;
    [SerializeField]
    private AudioClip _uiClick;
    [SerializeField]
    private AudioClip _walkSound;
    [SerializeField]
    private AudioClip _rainSound;
    [SerializeField]
    private AudioClip _swordSwing;
    [SerializeField]
    private AudioClip _pageTurnSound;

    [Header("Music Settings")]
    [SerializeField] private float _musicFadeDuration = 1e-6f;

    [Header("Spatial Settings")]
    [SerializeField]
    private SpatialSettings _environmentSettings;
    [SerializeField]
    private SpatialSettings _weaponSettings;

    private bool _isRaining;

    protected override void OnPrefabInit()
    {
        base.OnPrefabInit();

        Instance = this;

        var _ = SaveGame.Instance;

        RPlayerPrefs.SetFloat("Music_Volume", 0.6f);
        AudioManager.Instance.SetChannelVolume(AudioChannel.Music, 0.6f);
        var volume = RPlayerPrefs.GetFloat("Music_Volume");

        AudioManager.Instance.SetChannelVolume(AudioChannel.SFX_Environment, 0.4f);
        PlayBackgroundMusic();

        StateMachineManager.CreateInstance();
        LoadAssets();
        EntityTemplates.CreateTemplates();
        LoadEntities();
        SpawnEntities();

        SaveLoader.Instance.saveManager.onRegister += SaveManager_onRegister;
    }

    private void SaveManager_onRegister(SaveLoadRoot saveLoadRoot)
    {
        var gameObject = saveLoadRoot.gameObject;
        RPrefabID prefabID = gameObject.GetComponent<RPrefabID>();

        if (prefabID != null)
        {
            if (prefabID.PrefabTag == GSMPlayerConfig.ID)
            {
                _player = gameObject;
            }
            else if (prefabID.PrefabTag == GSMEnemy1Config.ID)
            {
                _enemy_1 = gameObject;
                _enemy_1.transform.GetComponent<SpriteRenderer>().flipX = true;
            }
        }
    }

    protected override void OnSpawn()
    {
        base.OnSpawn();
    }

    private async void LoadAssets()
    {
        GSMPlayerRAC = Resources.Load<RuntimeAnimatorController>("Anims/Player/Player");
        GSMEnemy1RAC = Resources.Load<RuntimeAnimatorController>("Anims/Enemy_1/Enemy_1");
    }

    public void LoadEntities()
    {
        object obj1 = Activator.CreateInstance(typeof(GSMPlayerConfig));
        object obj2 = Activator.CreateInstance(typeof(GSMEnemy1Config));
        RegisterEntity(obj1 as IEntityConfig);
        RegisterEntity(obj2 as IEntityConfig);
    }

    public void RegisterEntity(IEntityConfig config)
    {
        GameObject gameObject = config.CreatePrefab();
        RPrefabID component = gameObject.GetComponent<RPrefabID>();
        component.prefabInitFn += config.OnPrefabInit;
        component.prefabSpawnFn += config.OnSpawn;
        Assets.AddPrefab(component);
    }

    public void SpawnEntities()
    {
        _player = Util.RInstantiate(Assets.GetPrefab(GSMPlayerConfig.ID));
        _player.transform.position = new Vector3(-5, 0, 0);
        _player.SetActive(true);

        _enemy_1 = Util.RInstantiate(Assets.GetPrefab(GSMEnemy1Config.ID));
        _enemy_1.transform.GetComponent<SpriteRenderer>().flipX = true;
        _enemy_1.transform.position = new Vector3(5, 0, 0);
        _enemy_1.SetActive(true);
    }

    public void StartTest()
    {
        var playerAttackSM = _player.GetComponent<AttackSMComponent>();
        playerAttackSM.smi.sm.attacker.Set(_player, playerAttackSM.smi);
        playerAttackSM.smi.sm.attackTarget.Set(_enemy_1, playerAttackSM.smi);
        playerAttackSM.smi.StartSM();
    }

    #region Audio
    private void PlayBackgroundMusic()
    {
        AudioManager.Instance.PlayMusic(bgmGameplay, _musicFadeDuration);
    }

    public void SwitchToMenuMusic()
    {
        AudioManager.Instance.PlayMusic(bgmMainMenu, _musicFadeDuration);
    }

    public void OnStartButtonClick()
    {
        AudioManager.Instance.PlaySFX(_uiClick, AudioChannel.SFX_UI);
    }

    public void OnPlayerWalk(Vector2 playerPos)
    {
        AudioManager.Instance.PlaySFX(_walkSound, 
            AudioChannel.SFX_Character,
            false,
            new SpatialSettings
            {
                spatialBlend = 0.5f,
                minDistance = 2f,
                maxDistance = 10f
            },
            playerPos);
    }

    public void ToggleRain()
    {
        _isRaining = !_isRaining;
        if (_isRaining)
        {
            AudioManager.Instance.PlaySFX(_rainSound, AudioChannel.SFX_Environment, true,
                _environmentSettings);
        }
        else
        {
            AudioManager.Instance.StopChannel(AudioChannel.SFX_Environment);
        }
    }

    public void OnSwordAttack(Vector2 attackPos)
    {
        AudioManager.Instance.PlaySFX(_swordSwing, AudioChannel.SFX_Weapons, false,
            _weaponSettings, attackPos);
    }

    public void OnCollectDiary(Vector2 collectPos)
    {
        AudioManager.Instance.PlaySFX(_pageTurnSound, AudioChannel.SFX_Environment, false,
            new SpatialSettings
            {
                spatialBlend = 0.3f,
                minDistance = 0.5f,
                maxDistance = 3f
            },
            collectPos);
    }
    #endregion

    #region serialize
    public void Save()
    {
        string savePrefixAndCreateFolder = SaveLoader.GetSavePrefixAndCreateFolder();
        string path = Path.Combine(savePrefixAndCreateFolder, "text_gsm.sav");
        SaveLoader.Instance.Save(path, false);
    }

    public void Load()
    {
        string savePrefixAndCreateFolder = SaveLoader.GetSavePrefixAndCreateFolder();
        string path = Path.Combine(savePrefixAndCreateFolder, "text_gsm.sav");
        SaveLoader.Instance.Load(path);
    }
    #endregion

    public static GSMTestRoot Instance { get; private set; }
}
