using System;
using System.Collections.Generic;
using Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZeroPass;
using ZeroPass.StateMachine;

public class Game : RMonoBehaviour
{
    public static Game Instance
    {
        get;
        private set;
    }

    protected override void OnPrefabInit()
    {
        DebugUtil.LogArgs(Time.realtimeSinceStartup, "Level Loaded....", SceneManager.GetActiveScene().name);
        App.OnPreLoadScene = (System.Action)Delegate.Combine(App.OnPreLoadScene, new System.Action(StopBE));
        Instance = this;
        Db.Get();
    }

    public void StopBE()
    {
        // Back-End

        Dictionary<Tag, List<SaveLoadRoot>> lists = SaveLoader.Instance.saveManager.GetLists();
        foreach (List<SaveLoadRoot> value in lists.Values)
        {
            foreach (SaveLoadRoot item in value)
            {
                if (item.gameObject != null)
                {
                    Util.RDestroyGameObject(item.gameObject);
                }
            }
        }
    }

    private void DestroyInstances()
    {
        lastGameObject = null;
        lastObj = null;
        RPrefabIDTracker.DestroyInstance();
        SaveLoader.DestroyInstance();
        // GameScheduler.DestroyInstance();
        Singleton<StateMachineManager>.Instance.Clear();
        Singleton<StateMachineUpdater>.Instance.Clear();
        Instance = null;
        Assets.ClearOnAddPrefab();
        lastGameObject = null;
        lastObj = null;
    }
}