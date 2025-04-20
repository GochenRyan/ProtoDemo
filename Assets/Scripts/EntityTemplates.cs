using UnityEngine;
using ZeroPass.StateMachine;
using ZeroPass;

public class EntityTemplates
{
    private static GameObject selectableEntityTemplate;

    private static GameObject unselectableEntityTemplate;

    private static GameObject baseEntityTemplate;

    public static void CreateTemplates()
    {
        unselectableEntityTemplate = new GameObject("unselectableEntityTemplate");
        unselectableEntityTemplate.SetActive(false);
        unselectableEntityTemplate.AddComponent<RPrefabID>();
        Object.DontDestroyOnLoad(unselectableEntityTemplate);
        selectableEntityTemplate = Object.Instantiate(unselectableEntityTemplate);
        selectableEntityTemplate.name = "selectableEntityTemplate";
        //selectableEntityTemplate.AddComponent<RSelectable>();
        Object.DontDestroyOnLoad(selectableEntityTemplate);
        baseEntityTemplate = Object.Instantiate(selectableEntityTemplate);
        baseEntityTemplate.name = "baseEntityTemplate";
        baseEntityTemplate.AddComponent<SpriteRenderer>();
        baseEntityTemplate.AddComponent<Animator>();
        baseEntityTemplate.AddComponent<StateMachineController>();
        Object.DontDestroyOnLoad(baseEntityTemplate);
    }

    public static GameObject CreateEntity(string id, string name, bool is_selectable = true)
    {
        GameObject gameObject = null;
        gameObject = ((!is_selectable) ? Object.Instantiate(unselectableEntityTemplate) : Object.Instantiate(selectableEntityTemplate));
        Object.DontDestroyOnLoad(gameObject);
        ConfigEntity(gameObject, id, name, is_selectable);
        return gameObject;
    }

    private static void ConfigEntity(GameObject template, string id, string name, bool is_selectable = true)
    {
        template.name = id;
        template.AddOrGet<Facing>();
        RPrefabID rPrefabID = template.AddOrGet<RPrefabID>();
        rPrefabID.PrefabTag = TagManager.Create(id, name);
        if (is_selectable)
        {
            RSelectable rSelectable = template.AddOrGet<RSelectable>();
            rSelectable.SetName(name);
        }
    }

    public static GameObject CreateBaseEntity(string id, string name, string desc)
    {
        GameObject gameObject = Object.Instantiate(baseEntityTemplate);
        Object.DontDestroyOnLoad(gameObject);
        ConfigEntity(gameObject, id, name, true);
        return gameObject;
    }
}
