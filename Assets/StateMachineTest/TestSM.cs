using System.Collections.Generic;
using System;
using UnityEngine;
using ZeroPass;
using System.Reflection;

public class TestSM : RMonoBehaviour
{
    private GameObject _hero;
    private GameObject _enemyI;

    public static RuntimeAnimatorController HeroRAC;
    public static RuntimeAnimatorController EnemyIRAC;


    protected override void OnPrefabInit()
    {
        base.OnPrefabInit();
        LoadAssets();
    }

    private async void LoadAssets()
    {
        HeroRAC = Resources.Load<RuntimeAnimatorController>("Anims/Hero/hero_1_toreplace");
        EnemyIRAC = Resources.Load<RuntimeAnimatorController>("Anims/Hero/hero_1_toreplace");

        LoadConfig();
        SpwanTestEntity();
    }

    protected override void OnSpawn()
    {
        base.OnSpawn();
    }

    private void LoadConfig()
    {
        List<Type> list = new List<Type>();
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies)
        {
            Type[] types = assembly.GetTypes();
            if (types != null)
            {
                list.AddRange(types);
            }
        }
        LoadEntities(list);
    }

    public void LoadEntities(List<Type> types)
    {
        Type typeFromHandle = typeof(IEntityConfig);
        foreach (Type type in types)
        {
            if ((typeFromHandle.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface))
            {
                object obj = Activator.CreateInstance(type);
                RegisterEntity(obj as IEntityConfig);
            }
        }
    }

    public void RegisterEntity(IEntityConfig config)
    {
        GameObject gameObject = config.CreatePrefab();
        RPrefabID component = gameObject.GetComponent<RPrefabID>();
        component.prefabInitFn += config.OnPrefabInit;
        component.prefabSpawnFn += config.OnSpawn;
        Assets.AddPrefab(component);
    }

    private void SpwanTestEntity()
    {
        _hero = Util.RInstantiate(Assets.GetPrefab(HeroConfig.ID));
        _hero.transform.position = new Vector3(-5, 0, 0);
        _hero.SetActive(true);

        _enemyI = Util.RInstantiate(Assets.GetPrefab(EnemyIConfig.ID));
        _enemyI.transform.position = new Vector3(5, 0, 0);
        var facing = _enemyI.GetComponent<Facing>();
        facing.Face(_hero.transform.position.x);
        _enemyI.SetActive(true);
    }
}
