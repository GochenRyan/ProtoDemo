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
    private GameObject enemyI1;
    private GameObject enemyI2;

    protected override void OnPrefabInit()
    {
        var _ = SaveGame.Instance;
        SaveLoader.Instance.saveManager.onRegister += SaveManager_onRegister;
        EntityTemplates.CreateTemplates();
        LoadEntities();
    }

    private void SaveManager_onRegister(SaveLoadRoot saveLoadRoot)
    {
        var gameObject = saveLoadRoot.gameObject;
        RPrefabID prefabID = gameObject.GetComponent<RPrefabID>();

        if (prefabID != null && prefabID.PrefabTag == EnemyIConfig.ID)
        {
            ObjectCase objectCase = gameObject.GetComponent<ObjectCase>();
            //Struct1 struct1 = objectCase.Obj1 as Struct1;
            InheritCase inheritCase = gameObject.GetComponent<InheritCase>();
        }
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

    public void SpawnInheritCase()
    {
        enemyI1 = Util.RInstantiate(Assets.GetPrefab(EnemyIConfig.ID));
        enemyI1.transform.position = new Vector3(5, 0, 0);
        // Only the components added when assembling the Prefab (via Assets.AddPrefab) will be included in serialization.
        var inheritCase1 = enemyI1.AddOrGet<InheritCase>();
        inheritCase1.baseCls = new ChildCls1
        {
            IntValue1 = 1234
        };
        enemyI1.SetActive(true);

        enemyI2 = Util.RInstantiate(Assets.GetPrefab(EnemyIConfig.ID));
        enemyI2.transform.position = new Vector3(5, 0, 0);
        var inheritCase2 = enemyI2.AddOrGet<InheritCase>();
        inheritCase2.baseCls = new ChildCls2
        {
            FloatValue1 = 1.234f
        };
        enemyI2.SetActive(true);
    }
}
