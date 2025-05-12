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

    public static GameObject CreateEntity(string id, string name, string anim, bool is_selectable = true)
    {
        GameObject gameObject = null;
        gameObject = ((!is_selectable) ? Object.Instantiate(unselectableEntityTemplate) : Object.Instantiate(selectableEntityTemplate));
        Object.DontDestroyOnLoad(gameObject);
        ConfigEntity(gameObject, id, name, anim, is_selectable);
        return gameObject;
    }

    private static void ConfigEntity(GameObject template, string id, string name, string anim, bool is_selectable = true)
    {
        template.name = id;
        template.AddOrGet<Facing>();
        RPrefabID rPrefabID = template.AddOrGet<RPrefabID>();
        rPrefabID.PrefabTag = TagManager.Create(id, name);

        RAnimControllerBase animController = template.AddOrGet<RAnimControllerBase>();
        animController.initialAnim = anim;

        if (is_selectable)
        {
            RSelectable rSelectable = template.AddOrGet<RSelectable>();
            rSelectable.SetName(name);
        }
    }

    public static GameObject CreateBaseEntity(string id, string name, string anim, string desc)
    {
        GameObject gameObject = Object.Instantiate(baseEntityTemplate);
        Object.DontDestroyOnLoad(gameObject);
        ConfigEntity(gameObject, id, name, anim, true);
        return gameObject;
    }
}
