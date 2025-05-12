using Serialization;
using UnityEngine;

public class GSMPlayerConfig : IEntityConfig
{
    public const string ID = "GSMPlayer";

    public GameObject CreatePrefab()
    {
        var gameObject = EntityTemplates.CreateBaseEntity(ID, ID, "Idle", "");
        gameObject.AddComponent<SaveLoadRoot>();
        var animator = gameObject.GetComponent<Animator>();
        animator.runtimeAnimatorController = GSMTestRoot.GSMPlayerRAC;
        gameObject.AddComponent<CombatEntity>();
        gameObject.AddComponent<AttackSMComponent>();
        gameObject.AddComponent<RAnimControllerBase>();

        return gameObject;
    }

    public void OnPrefabInit(GameObject inst)
    {
    }

    public void OnSpawn(GameObject inst)
    {
    }
}
