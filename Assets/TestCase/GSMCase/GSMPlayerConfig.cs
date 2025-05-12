using Serialization;
using UnityEngine;

public class GSMPlayerConfig : IEntityConfig
{
    public const string ID = "GSMPlayer";

    public GameObject CreatePrefab()
    {
        var gameObject = EntityTemplates.CreateBaseEntity(ID, ID, "");
        gameObject.AddComponent<SaveLoadRoot>();
        var animator = gameObject.GetComponent<Animator>();
        animator.runtimeAnimatorController = GSMTestRoot.GSMPlayerRAC;
        gameObject.AddComponent<CombatEntity>();
        gameObject.AddComponent<AttackSMComponent>();

        return gameObject;
    }

    public void OnPrefabInit(GameObject inst)
    {
    }

    public void OnSpawn(GameObject inst)
    {
    }
}
