using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using ZeroPass;
using Debug = ZeroPass.Debug;


public class Assets : RMonoBehaviour, ISerializationCallbackReceiver
{
    private static Action<RPrefabID> OnAddPrefab;
    public static List<RPrefabID> Prefabs = new List<RPrefabID>();
    private static Dictionary<Tag, RPrefabID> PrefabsByTag = new Dictionary<Tag, RPrefabID>();
    private static Dictionary<Tag, List<RPrefabID>> PrefabsByAdditionalTags = new Dictionary<Tag, List<RPrefabID>>();
    private static HashSet<Tag> CountableTags = new HashSet<Tag>();

    public static Dictionary<HashedString, RuntimeAnimatorController> AnimControllerTable =
        new Dictionary<HashedString, RuntimeAnimatorController>();

    public static Assets instance;

    protected override void OnPrefabInit()
    {
        instance = this;

        CreatePrefabs();
    }

    private void CreatePrefabs()
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
        EntityTemplates.CreateTemplates();
    }

    private static void TryAddCountableTag(RPrefabID prefab)
    {
        foreach (Tag displayAsUnit in GameTags.DisplayAsUnits)
        {
            if (prefab.HasTag(displayAsUnit))
            {
                AddCountableTag(prefab.PrefabTag);
                break;
            }
        }
    }

    public static void AddCountableTag(Tag tag)
    {
        CountableTags.Add(tag);
    }

    public static void AddPrefab(RPrefabID prefab)
    {
        if (!((UnityEngine.Object)prefab == (UnityEngine.Object)null))
        {
            prefab.UpdateSaveLoadTag();
            if (PrefabsByTag.ContainsKey(prefab.PrefabTag))
            {
                Debug.LogWarning("Tried loading prefab with duplicate tag, ignoring: " + prefab.PrefabTag);
            }
            PrefabsByTag[prefab.PrefabTag] = prefab;
            foreach (Tag tag in prefab.Tags)
            {
                if (!PrefabsByAdditionalTags.ContainsKey(tag))
                {
                    PrefabsByAdditionalTags[tag] = new List<RPrefabID>();
                }
                PrefabsByAdditionalTags[tag].Add(prefab);
            }
            Prefabs.Add(prefab);
            TryAddCountableTag(prefab);
            if (OnAddPrefab != null)
            {
                OnAddPrefab(prefab);
            }
        }
    }

    public static void RegisterOnAddPrefab(Action<RPrefabID> on_add)
    {
        OnAddPrefab = (Action<RPrefabID>)Delegate.Combine(OnAddPrefab, on_add);
        foreach (RPrefabID prefab in Prefabs)
        {
            on_add(prefab);
        }
    }

    public static void UnregisterOnAddPrefab(Action<RPrefabID> on_add)
    {
        OnAddPrefab = (Action<RPrefabID>)Delegate.Remove(OnAddPrefab, on_add);
    }

    public static void ClearOnAddPrefab()
    {
        OnAddPrefab = null;
    }

    public static GameObject GetPrefab(Tag tag)
    {
        GameObject gameObject = TryGetPrefab(tag);
        if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
        {
            Debug.LogWarning("Missing prefab: " + tag);
        }
        return gameObject;
    }

    public static GameObject TryGetPrefab(Tag tag)
    {
        RPrefabID value = null;
        PrefabsByTag.TryGetValue(tag, out value);
        return (!((UnityEngine.Object)value != (UnityEngine.Object)null)) ? null : value.gameObject;
    }

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
    }
}