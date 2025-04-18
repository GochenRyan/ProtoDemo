using Serialization;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.IO;
using UnityEngine;
using ZeroPass;

public class Root : RMonoBehaviour
{
    private static Root _instance;
    private GameObject _enemyI;

    protected override void OnPrefabInit()
    {
        //var _ = SaveLoader.Instance;
        var _ = SaveGame.Instance;
        EntityTemplates.CreateTemplates();
        LoadEntities();
    }

    public void LoadEntities()
    {
        object obj = Activator.CreateInstance(typeof(EnemyIConfig));
        RegisterEntity(obj as IEntityConfig);
    }

    public void RegisterEntity(IEntityConfig config)
    {
        GameObject gameObject = config.CreatePrefab();
        RPrefabID component = gameObject.GetComponent<RPrefabID>();
        component.prefabInitFn += config.OnPrefabInit;
        component.prefabSpawnFn += config.OnSpawn;
        Assets.AddPrefab(component);
    }

    public void Save()
    {
        string savePrefixAndCreateFolder = SaveLoader.GetSavePrefixAndCreateFolder();
        string path = Path.Combine(savePrefixAndCreateFolder, "text1.sav");
        SaveLoader.Instance.Save(path, false);
    }

    public void Load()
    {
        string savePrefixAndCreateFolder = SaveLoader.GetSavePrefixAndCreateFolder();
        string path = Path.Combine(savePrefixAndCreateFolder, "text1.sav");
        SaveLoader.Instance.Load(path);
    }

    public void SpawnObjectCase()
    {
        _enemyI = Util.RInstantiate(Assets.GetPrefab(EnemyIConfig.ID));
        _enemyI.transform.position = new Vector3(5, 0, 0);
        var objectCase = _enemyI.AddOrGet<ObjectCase>();
        objectCase.Obj1 = new Struct1
        {
            floatValue1 = 7.8f,
            intValue1 = 2333,
            longValue1 = 1234567890,
            stringValue1 = "asasasa"
        };
        _enemyI.SetActive(true);
    }
}
