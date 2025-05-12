using Serialization;
using System;
using UnityEngine;
using ZeroPass;
using ZeroPass.StateMachine;

public class GSMTestRoot : RMonoBehaviour
{
    private GameObject _player;
    private GameObject _enemy_1;

    public static RuntimeAnimatorController GSMPlayerRAC;
    public static RuntimeAnimatorController GSMEnemy1RAC;

    protected override void OnPrefabInit()
    {
        base.OnPrefabInit();

        var _ = SaveGame.Instance;
        StateMachineManager.CreateInstance();
        LoadAssets();
        EntityTemplates.CreateTemplates();
        LoadEntities();
        SpawnEntities();
        
    }

    protected override void OnSpawn()
    {
        base.OnSpawn();

        StartTest();
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
}
